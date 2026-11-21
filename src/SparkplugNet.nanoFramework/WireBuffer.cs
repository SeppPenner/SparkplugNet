using System;

namespace SparkplugNet.NanoFramework
{
    internal sealed class WireBuffer
    {
        internal const int MaxLength = 65535;
        private byte[] bytes = new byte[64];
        private int length;

        internal void WriteByte(byte value)
        {
            EnsureCapacity(1);
            bytes[length++] = value;
        }

        internal void Write(byte[] value)
        {
            EnsureCapacity(value.Length);
            Array.Copy(value, 0, bytes, length, value.Length);
            length += value.Length;
        }

        internal void WriteVarint(ulong value)
        {
            while (value >= 128)
            {
                WriteByte((byte)((value & 127) | 128));
                value >>= 7;
            }

            WriteByte((byte)value);
        }

        internal byte[] ToArray()
        {
            var result = new byte[length];
            Array.Copy(bytes, result, length);
            return result;
        }

        private void EnsureCapacity(int count)
        {
            if (count > MaxLength - length)
            {
                throw new ArgumentException("The packet exceeds the nanoFramework buffer limit.");
            }

            var required = length + count;
            if (required > bytes.Length)
            {
                int capacity = bytes.Length * 2;
                if (capacity < required) capacity = required;
                if (capacity > MaxLength) capacity = MaxLength;
                var replacement = new byte[capacity];
                Array.Copy(bytes, replacement, length);
                bytes = replacement;
            }
        }
    }

    internal sealed class WireReader
    {
        private readonly byte[] bytes;
        private readonly int end;
        private int position;

        internal WireReader(byte[] bytes) : this(bytes, 0, bytes.Length) { }

        private WireReader(byte[] bytes, int start, int length)
        {
            this.bytes = bytes;
            position = start;
            end = start + length;
        }

        internal int Remaining { get { return end - position; } }

        internal byte ReadByte()
        {
            Require(1);
            return bytes[position++];
        }

        internal ulong ReadVarint()
        {
            ulong value = 0;
            for (int shift = 0; shift < 64; shift += 7)
            {
                byte current = ReadByte();
                if (shift == 63 && current > 1)
                {
                    break;
                }

                value |= (ulong)(current & 127) << shift;
                if (current < 128)
                {
                    return value;
                }
            }

            throw new FormatException("Invalid protobuf varint.");
        }

        internal int ReadLength()
        {
            ulong size = ReadVarint();
            if (size > (ulong)Remaining)
            {
                throw new FormatException("Truncated protobuf field.");
            }

            return (int)size;
        }

        internal WireReader ReadMessage()
        {
            int size = ReadLength();
            var message = new WireReader(bytes, position, size);
            position += size;
            return message;
        }

        internal byte[] ReadBytes(int count)
        {
            Require(count);
            var value = new byte[count];
            Array.Copy(bytes, position, value, 0, count);
            position += count;
            return value;
        }

        internal void Skip(int count)
        {
            Require(count);
            position += count;
        }

        private void Require(int count)
        {
            if (count < 0 || count > Remaining)
            {
                throw new FormatException("Truncated packet.");
            }
        }
    }
}
