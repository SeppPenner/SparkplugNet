using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SparkplugNet.NanoFramework
{
    /// <summary>TCP and MQTT session settings for a nanoFramework Sparkplug client.</summary>
    public sealed class MqttConnectionOptions
    {
        public MqttConnectionOptions(string broker, string clientId)
        {
            Broker = broker;
            ClientId = clientId;
        }

        public string Broker { get; private set; }
        public string ClientId { get; private set; }
        public int Port { get; set; } = 1883;
        public string Username { get; set; }
        public string Password { get; set; }
    }

    internal delegate void MqttPayloadHandler(string topic, byte[] payload);

    internal sealed class Mqtt311Client
    {
        private const ushort KeepAliveSeconds = 30;
        private const int SocketTimeout = 5000;
        private readonly byte[] singleByte = new byte[1];
        private Socket socket;
        private ushort packetId;
        private long lastSend;
        private bool pingPending;

        internal MqttPayloadHandler PayloadReceived { get; set; }
        internal bool IsConnected { get { return socket != null; } }

        internal void Connect(MqttConnectionOptions options, string willTopic, byte[] will, bool retainWill)
        {
            if (IsConnected) throw new InvalidOperationException("The MQTT client is already connected.");
            if (options == null) throw new ArgumentNullException(nameof(options));

            var body = new WireBuffer();
            WriteString(body, "MQTT");
            body.WriteByte(4);
            byte flags = 2;
            if (will != null) flags |= (byte)(retainWill ? 44 : 12);
            if (options.Username != null) flags |= 128;
            if (options.Password != null)
            {
                if (options.Username == null) throw new ArgumentException("An MQTT password requires a username.");
                flags |= 64;
            }

            body.WriteByte(flags);
            WriteUInt16(body, KeepAliveSeconds);
            WriteString(body, options.ClientId);
            if (will != null)
            {
                WriteString(body, willTopic);
                WriteBinary(body, will);
            }

            if (options.Username != null) WriteString(body, options.Username);
            if (options.Password != null) WriteBinary(body, Encoding.UTF8.GetBytes(options.Password));

            var addresses = Dns.GetHostEntry(options.Broker).AddressList;
            foreach (IPAddress address in addresses)
            {
                if (address.AddressFamily != AddressFamily.InterNetwork) continue;

                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    socket.SendTimeout = SocketTimeout;
                    socket.ReceiveTimeout = SocketTimeout;
                    socket.Connect(new IPEndPoint(address, options.Port));
                    Send(0x10, body.ToArray());
                    byte[] ack = ReadPacket(out byte header);
                    if (header != 0x20 || ack.Length != 2 || ack[0] != 0 || ack[1] != 0)
                    {
                        throw new InvalidOperationException("The broker refused the MQTT connection.");
                    }

                    return;
                }
                catch
                {
                    Close();
                    throw;
                }
            }

            throw new InvalidOperationException("The MQTT broker did not resolve to an IPv4 address.");
        }

        internal void Subscribe(string topic, byte qos)
        {
            ushort id = NextPacketId();
            var body = new WireBuffer();
            WriteUInt16(body, id);
            WriteString(body, topic);
            body.WriteByte(qos);
            Send(0x82, body.ToArray());
            byte[] ack = WaitForAck(0x90, id);
            if (ack.Length != 3 || ack[2] > qos)
            {
                throw new InvalidOperationException("The broker rejected the MQTT subscription.");
            }
        }

        internal void Publish(string topic, byte[] payload, byte qos, bool retain)
        {
            var body = new WireBuffer();
            WriteString(body, topic);
            ushort id = qos == 0 ? (ushort)0 : NextPacketId();
            if (id != 0) WriteUInt16(body, id);
            body.Write(payload);
            Send((byte)(0x30 | (qos << 1) | (retain ? 1 : 0)), body.ToArray());
            if (id != 0) WaitForAck(0x40, id);
        }

        internal void Poll(int timeoutMilliseconds)
        {
            if (timeoutMilliseconds < 0 || timeoutMilliseconds > SocketTimeout)
            {
                throw new ArgumentOutOfRangeException(nameof(timeoutMilliseconds));
            }

            RequireConnection();
            if (DateTime.UtcNow.Ticks - lastSend >= KeepAliveSeconds * TimeSpan.TicksPerSecond / 2)
            {
                if (pingPending)
                {
                    Close();
                    throw new InvalidOperationException("The broker did not answer the MQTT keepalive.");
                }

                Send(0xc0, new byte[0]);
                pingPending = true;
            }

            if (socket.Poll(timeoutMilliseconds * 1000, SelectMode.SelectRead))
            {
                byte[] body = ReadPacket(out byte header);
                Dispatch(header, body);
            }
        }

        internal void Disconnect()
        {
            if (!IsConnected) return;
            try { Send(0xe0, new byte[0]); }
            finally { Close(); }
        }

        internal void Close()
        {
            Socket connection = socket;
            socket = null;
            pingPending = false;
            if (connection != null) connection.Close();
        }

        private byte[] WaitForAck(byte expectedHeader, ushort expectedId)
        {
            long limit = DateTime.UtcNow.Ticks + SocketTimeout * TimeSpan.TicksPerMillisecond;
            while (DateTime.UtcNow.Ticks < limit)
            {
                byte[] body = ReadPacket(out byte header);
                if (header == expectedHeader && body.Length >= 2 && ((body[0] << 8) | body[1]) == expectedId)
                {
                    return body;
                }

                Dispatch(header, body);
            }

            Close();
            throw new InvalidOperationException("The broker did not acknowledge the MQTT packet.");
        }

        private void Dispatch(byte header, byte[] body)
        {
            if (header == 0xd0)
            {
                pingPending = false;
                return;
            }

            if ((header & 0xf0) != 0x30) return;

            int qos = (header >> 1) & 3;
            if (qos > 1) throw new FormatException("Unsupported MQTT publish quality of service.");
            var reader = new WireReader(body);
            byte[] topic = reader.ReadBytes((reader.ReadByte() << 8) | reader.ReadByte());
            byte[] id = qos == 0 ? null : reader.ReadBytes(2);
            byte[] data = reader.ReadBytes(reader.Remaining);
            if (id != null) Send(0x40, id);
            if (PayloadReceived != null) PayloadReceived(Encoding.UTF8.GetString(topic, 0, topic.Length), data);
        }

        private byte[] ReadPacket(out byte header)
        {
            RequireConnection();
            try
            {
                Receive(singleByte);
                header = singleByte[0];
                int length = 0;
                for (int shift = 0; shift < 28; shift += 7)
                {
                    Receive(singleByte);
                    length |= (singleByte[0] & 127) << shift;
                    if (singleByte[0] < 128)
                    {
                        if (length > WireBuffer.MaxLength) throw new FormatException("The MQTT packet is too large.");
                        var body = new byte[length];
                        Receive(body);
                        return body;
                    }
                }

                throw new FormatException("Invalid MQTT remaining length.");
            }
            catch
            {
                Close();
                throw;
            }
        }

        private void Receive(byte[] bytes)
        {
            for (int offset = 0; offset < bytes.Length;)
            {
                int count = socket.Receive(bytes, offset, bytes.Length - offset, SocketFlags.None);
                if (count == 0) throw new InvalidOperationException("The broker closed the MQTT connection.");
                offset += count;
            }
        }

        private void Send(byte header, byte[] body)
        {
            RequireConnection();
            var prefix = new WireBuffer();
            prefix.WriteByte(header);
            prefix.WriteVarint((uint)body.Length);
            try
            {
                SendBytes(prefix.ToArray());
                SendBytes(body);
                lastSend = DateTime.UtcNow.Ticks;
            }
            catch
            {
                Close();
                throw;
            }
        }

        private void SendBytes(byte[] bytes)
        {
            for (int offset = 0; offset < bytes.Length;)
            {
                int count = socket.Send(bytes, offset, bytes.Length - offset, SocketFlags.None);
                if (count == 0) throw new InvalidOperationException("The broker closed the MQTT connection.");
                offset += count;
            }
        }

        private void RequireConnection()
        {
            if (!IsConnected) throw new InvalidOperationException("The MQTT client is disconnected.");
        }

        private ushort NextPacketId()
        {
            packetId = packetId == ushort.MaxValue ? (ushort)1 : (ushort)(packetId + 1);
            return packetId;
        }

        private static void WriteString(WireBuffer buffer, string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            WriteBinary(buffer, Encoding.UTF8.GetBytes(value));
        }

        private static void WriteBinary(WireBuffer buffer, byte[] value)
        {
            WriteUInt16(buffer, checked((ushort)value.Length));
            buffer.Write(value);
        }

        private static void WriteUInt16(WireBuffer buffer, ushort value)
        {
            buffer.WriteByte((byte)(value >> 8));
            buffer.WriteByte((byte)value);
        }
    }
}
