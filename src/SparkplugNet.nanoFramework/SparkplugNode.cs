using System;
using System.Text;

namespace SparkplugNet.NanoFramework
{
    public enum SparkplugVersion
    {
        Version22,
        Version30
    }

    public delegate void SparkplugCommandHandler(string deviceId, SparkplugPayload command);

    /// <summary>A Sparkplug B Edge Node. Use the node from a single thread and poll regularly.</summary>
    public sealed class SparkplugNode : IDisposable
    {
        private static readonly long UnixEpochTicks = new DateTime(1970, 1, 1).Ticks;
        private readonly Mqtt311Client mqtt = new Mqtt311Client();
        private readonly MqttConnectionOptions options;
        private readonly string topicPrefix;
        private readonly string edgeNodeId;
        private readonly string nodeCommandTopic;
        private readonly string deviceCommandPrefix;
        private readonly string stateTopic;
        private readonly SparkplugVersion version;
        private Device devices;
        private Metric[] nodeMetrics;
        private byte bdSeq;
        private byte nextBdSeq;
        private byte sequence;
        private bool online;
        private bool hostOnline;
        private ulong lastStateTimestamp;

        public SparkplugNode(MqttConnectionOptions options, string groupId, string edgeNodeId,
            SparkplugVersion version = SparkplugVersion.Version30, string primaryHostId = null, byte initialBdSeq = 0)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            ValidateIdentifier(groupId);
            ValidateIdentifier(edgeNodeId);
            if (version != SparkplugVersion.Version22 && version != SparkplugVersion.Version30)
            {
                throw new ArgumentOutOfRangeException(nameof(version));
            }

            this.options = options;
            this.edgeNodeId = edgeNodeId;
            this.version = version;
            nextBdSeq = initialBdSeq;
            topicPrefix = "spBv1.0/" + groupId + "/";
            nodeCommandTopic = Topic("NCMD");
            deviceCommandPrefix = Topic("DCMD") + "/";
            if (primaryHostId != null)
            {
                ValidateIdentifier(primaryHostId);
                stateTopic = (version == SparkplugVersion.Version30 ? "spBv1.0/STATE/" : "STATE/") + primaryHostId;
            }

            mqtt.PayloadReceived = Receive;
        }

        /// <summary>Delivers node commands with a null device ID and device commands with their device ID.</summary>
        public event SparkplugCommandHandler CommandReceived;

        public bool IsConnected { get { return mqtt.IsConnected; } }
        public bool IsOnline { get { return mqtt.IsConnected && online; } }

        /// <summary>The bdSeq to use next. Persist it if the sequence needs to survive a restart.</summary>
        public byte NextBirthDeathSequence { get { return nextBdSeq; } }

        public void Connect(Metric[] metrics)
        {
            if (IsConnected) throw new InvalidOperationException("The Edge Node is already connected.");
            nodeMetrics = CopyBirthMetrics(metrics, true);
            bdSeq = nextBdSeq;
            hostOnline = stateTopic == null;
            online = false;

            try
            {
                mqtt.Connect(options, Topic("NDEATH"), DeathPayload(), false);
                nextBdSeq = unchecked((byte)(bdSeq + 1));
                mqtt.Subscribe(nodeCommandTopic, 1);
                mqtt.Subscribe(deviceCommandPrefix + "+", 1);
                if (stateTopic != null) mqtt.Subscribe(stateTopic, 1);
                if (hostOnline) PublishBirth();
            }
            catch
            {
                mqtt.Close();
                throw;
            }
        }

        /// <summary>Processes incoming packets and sends MQTT keepalive traffic.</summary>
        public void Poll(int timeoutMilliseconds = 0)
        {
            mqtt.Poll(timeoutMilliseconds);
            if (!IsConnected) return;
            if (!hostOnline && IsOnline)
            {
                Disconnect();
                return;
            }

            if (hostOnline && !online)
            {
                PublishBirth();
            }
        }

        public void PublishNodeData(Metric[] metrics)
        {
            RequireOnline();
            PublishData(Topic("NDATA"), nodeMetrics, metrics);
        }

        /// <summary>Announces an attached device. Announced devices are also sent on each node rebirth.</summary>
        public void AddDevice(string deviceId, Metric[] metrics)
        {
            ValidateIdentifier(deviceId);
            if (FindDevice(deviceId) != null) throw new ArgumentException("The device is already registered.");

            var device = new Device(deviceId, CopyBirthMetrics(metrics, false));
            if (IsOnline) PublishSequenced(Topic("DBIRTH", deviceId), device.Metrics);
            if (devices == null)
            {
                devices = device;
                return;
            }

            Device last = devices;
            while (last.Next != null) last = last.Next;
            last.Next = device;
        }

        public void PublishDeviceData(string deviceId, Metric[] metrics)
        {
            RequireOnline();
            Device device = FindDevice(deviceId);
            if (device == null) throw new ArgumentException("The device is not registered.");
            PublishData(Topic("DDATA", deviceId), device.Metrics, metrics);
        }

        public void RemoveDevice(string deviceId)
        {
            Device previous = null;
            for (Device device = devices; device != null; device = device.Next)
            {
                if (device.Id == deviceId)
                {
                    if (IsOnline) PublishSequenced(Topic("DDEATH", deviceId), new Metric[0]);
                    if (previous == null) devices = device.Next;
                    else previous.Next = device.Next;
                    return;
                }

                previous = device;
            }

            throw new ArgumentException("The device is not registered.");
        }

        public void Disconnect()
        {
            if (!IsConnected) return;
            bool wasOnline = online;
            online = false;
            try
            {
                if (wasOnline) mqtt.Publish(Topic("NDEATH"), DeathPayload(), 1, false);
                mqtt.Disconnect();
            }
            finally
            {
                mqtt.Close();
            }
        }

        public void Dispose()
        {
            Disconnect();
        }

        private void Receive(string topic, byte[] payload)
        {
            if (topic == stateTopic)
            {
                ReceiveState(payload);
                return;
            }

            if (!IsOnline) return;
            Metric[] birthMetrics = nodeMetrics;
            string deviceId = null;
            if (topic != nodeCommandTopic)
            {
                if (!topic.StartsWith(deviceCommandPrefix)) return;
                deviceId = topic.Substring(deviceCommandPrefix.Length);
                Device device = FindDevice(deviceId);
                if (device == null) return;
                birthMetrics = device.Metrics;
            }

            SparkplugPayload command;
            try { command = SparkplugBCodec.DecodeCommand(payload, birthMetrics, nodeCommand: deviceId == null); }
            catch (FormatException) { return; }

            if (deviceId == null) HandleRebirth(command);
            if (command.Metrics.Length != 0 && CommandReceived != null) CommandReceived(deviceId, command);
        }

        private void HandleRebirth(SparkplugPayload command)
        {
            int remaining = 0;
            foreach (Metric metric in command.Metrics)
            {
                if (metric.Name == SparkplugBCodec.RebirthMetricName && metric.Value is bool requested && requested)
                {
                    continue;
                }

                command.Metrics[remaining++] = metric;
            }

            if (remaining == command.Metrics.Length) return;
            PublishBirth();
            var unhandled = new Metric[remaining];
            Array.Copy(command.Metrics, unhandled, remaining);
            command.Metrics = unhandled;
        }

        private void ReceiveState(byte[] payload)
        {
            string state = Encoding.UTF8.GetString(payload, 0, payload.Length);
            if (version == SparkplugVersion.Version22)
            {
                if (state == "ONLINE") hostOnline = true;
                else if (state == "OFFLINE") hostOnline = false;
                return;
            }

            state = state.Trim();
            if (state.Length < 2 || state[0] != '{' || state[state.Length - 1] != '}'
                || !TryReadStateValue(state, "\"online\"", out string status)
                || !TryReadStateValue(state, "\"timestamp\"", out string stamp)
                || (status != "true" && status != "false"))
            {
                return;
            }

            ulong timestamp = 0;
            foreach (char digit in stamp)
            {
                if (digit < '0' || digit > '9' || timestamp > ((ulong)long.MaxValue - (uint)(digit - '0')) / 10)
                {
                    return;
                }

                timestamp = timestamp * 10 + (uint)(digit - '0');
            }

            if (timestamp < lastStateTimestamp) return;
            hostOnline = status == "true";
            lastStateTimestamp = timestamp;
        }

        private static bool TryReadStateValue(string state, string key, out string value)
        {
            value = null;
            int index = state.IndexOf(key);
            if (index < 0) return false;
            int colon = index + key.Length;
            while (colon < state.Length && " \t\r\n".IndexOf(state[colon]) >= 0) colon++;
            if (colon == state.Length || state[colon] != ':') return false;
            int end = state.IndexOfAny(new[] { ',', '}' }, colon + 1);
            if (end < 0) return false;
            value = state.Substring(colon + 1, end - colon - 1).Trim();
            return value.Length != 0;
        }

        private void PublishBirth()
        {
            var metrics = new Metric[nodeMetrics.Length + 2];
            metrics[0] = BirthDeathMetric();
            metrics[1] = new Metric(SparkplugBCodec.RebirthMetricName, MetricDataType.Boolean, false);
            Array.Copy(nodeMetrics, 0, metrics, 2, nodeMetrics.Length);
            online = false;
            sequence = 0;
            try
            {
                PublishSequenced(Topic("NBIRTH"), metrics);
                for (Device device = devices; device != null; device = device.Next)
                {
                    PublishSequenced(Topic("DBIRTH", device.Id), device.Metrics);
                }
            }
            catch
            {
                mqtt.Close();
                throw;
            }

            online = true;
        }

        private void PublishData(string topic, Metric[] birth, Metric[] updates)
        {
            if (updates == null) throw new ArgumentNullException(nameof(updates));
            var data = new Metric[updates.Length];
            var positions = new int[updates.Length];
            for (int index = 0; index < updates.Length; index++)
            {
                Metric update = updates[index];
                if (update == null) throw new ArgumentException("A metric cannot be null.");
                int position = SparkplugBCodec.FindMetric(birth, update);
                if (position < 0 || birth[position].DataType != update.DataType)
                {
                    throw new ArgumentException("The metric and its data type must be announced in the birth message.");
                }

                var metric = CopyMetric(update, birth[position].Name);
                if (birth[position].HasAlias)
                {
                    metric.Alias = birth[position].Alias;
                    metric.Name = null;
                }

                data[index] = metric;
                positions[index] = position;
            }

            PublishSequenced(topic, data, dataMessage: true);
            for (int index = 0; index < data.Length; index++)
            {
                if (data[index].IsHistorical || data[index].IsTransient) continue;
                Metric metric = birth[positions[index]];
                metric.Value = data[index].Value;
                metric.Timestamp = data[index].Timestamp;
            }
        }

        private void PublishSequenced(string topic, Metric[] metrics, bool dataMessage = false)
        {
            bool version30 = version == SparkplugVersion.Version30;
            byte[] payload = SparkplugBCodec.Encode(Timestamp(), true, sequence, metrics,
                includeDataTypes: !version30 || !dataMessage, fillMissingTimestamps: version30);
            mqtt.Publish(topic, payload, 0, false);
            sequence = unchecked((byte)(sequence + 1));
        }

        private byte[] DeathPayload()
        {
            return SparkplugBCodec.Encode(Timestamp(), false, 0, new[] { BirthDeathMetric() });
        }

        private Metric BirthDeathMetric()
        {
            return new Metric(SparkplugBCodec.BirthDeathMetricName, MetricDataType.Int64, (long)bdSeq);
        }

        private string Topic(string messageType, string deviceId = null)
        {
            string topic = topicPrefix + messageType + "/" + edgeNodeId;
            return deviceId == null ? topic : topic + "/" + deviceId;
        }

        private Device FindDevice(string deviceId)
        {
            for (Device device = devices; device != null; device = device.Next)
            {
                if (device.Id == deviceId) return device;
            }

            return null;
        }

        private void RequireOnline()
        {
            if (!IsOnline)
            {
                throw new InvalidOperationException("The Edge Node must publish a birth before sending data.");
            }
        }

        private static Metric[] CopyBirthMetrics(Metric[] metrics, bool node)
        {
            if (metrics == null) throw new ArgumentNullException(nameof(metrics));
            var copy = new Metric[metrics.Length];
            for (int index = 0; index < metrics.Length; index++)
            {
                Metric metric = metrics[index];
                if (metric == null || StringHelper.IsNullOrWhiteSpace(metric.Name))
                {
                    throw new ArgumentException("A birth metric requires a name.");
                }

                if (node && (metric.Name == SparkplugBCodec.BirthDeathMetricName || metric.Name == SparkplugBCodec.RebirthMetricName))
                {
                    throw new ArgumentException("The Edge Node provides the reserved metric " + metric.Name + ".");
                }

                for (int previous = 0; previous < index; previous++)
                {
                    Metric existing = copy[previous];
                    if (existing.Name == metric.Name || (existing.HasAlias && metric.HasAlias && existing.Alias == metric.Alias))
                    {
                        throw new ArgumentException("Birth metric names and aliases must be unique.");
                    }
                }

                copy[index] = CopyMetric(metric, metric.Name);
            }

            return copy;
        }

        private static Metric CopyMetric(Metric metric, string name)
        {
            object value = metric.Value;
            if (value is byte[] bytes)
            {
                var copy = new byte[bytes.Length];
                Array.Copy(bytes, copy, copy.Length);
                value = copy;
            }

            var result = new Metric(name, metric.DataType, value)
            {
                Timestamp = metric.Timestamp,
                IsHistorical = metric.IsHistorical,
                IsTransient = metric.IsTransient
            };
            if (metric.HasAlias) result.Alias = metric.Alias;
            return result;
        }

        private static ulong Timestamp()
        {
            return checked((ulong)((DateTime.UtcNow.Ticks - UnixEpochTicks) / TimeSpan.TicksPerMillisecond));
        }

        private static void ValidateIdentifier(string id)
        {
            if (StringHelper.IsNullOrWhiteSpace(id) || id.IndexOfAny(new[] { '/', '+', '#', '\0' }) >= 0)
            {
                throw new ArgumentException("Invalid Sparkplug topic identifier.");
            }
        }

        private sealed class Device
        {
            internal Device(string id, Metric[] metrics)
            {
                Id = id;
                Metrics = metrics;
            }

            internal string Id { get; }
            internal Metric[] Metrics { get; }
            internal Device Next { get; set; }
        }
    }
}
