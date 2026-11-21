#nullable disable

using System;
using System.Text;

namespace SparkplugNet.NanoFramework
{
    /// <summary>Reads and writes the scalar metric subset of the Sparkplug B protobuf schema.</summary>
    public static class SparkplugBCodec
    {
        internal const string RebirthMetricName = "Node Control/Rebirth";
        internal const string BirthDeathMetricName = "bdSeq";

        /// <summary>Encodes scalar metrics and a Unix millisecond timestamp as a Sparkplug B payload.</summary>
        public static byte[] Encode(ulong timestamp, bool hasSequence, byte sequence, Metric[] metrics)
        {
            return Encode(timestamp, hasSequence, sequence, metrics, includeDataTypes: true, fillMissingTimestamps: false);
        }

        internal static byte[] Encode(ulong timestamp, bool hasSequence, byte sequence, Metric[] metrics,
            bool includeDataTypes, bool fillMissingTimestamps)
        {
            if (metrics == null) throw new ArgumentNullException(nameof(metrics));

            var buffer = new WireBuffer();
            WriteVarint(buffer, 1, timestamp);
            foreach (Metric metric in metrics)
            {
                WriteBytes(buffer, 2, EncodeMetric(metric, includeDataTypes, fillMissingTimestamps ? timestamp : 0));
            }

            if (hasSequence) WriteVarint(buffer, 3, sequence);
            return buffer.ToArray();
        }

        /// <summary>Decodes the supported fields of a Sparkplug B payload.</summary>
        public static SparkplugPayload Decode(byte[] payload)
        {
            return Decode(payload, null);
        }

        /// <summary>Decodes Sparkplug B metrics, using birth metrics to resolve omitted data types.</summary>
        public static SparkplugPayload Decode(byte[] payload, Metric[] birthMetrics)
        {
            return Decode(payload, birthMetrics, command: false, nodeCommand: false);
        }

        internal static SparkplugPayload DecodeCommand(byte[] payload, Metric[] birthMetrics, bool nodeCommand)
        {
            return Decode(payload, birthMetrics, command: true, nodeCommand);
        }

        private static SparkplugPayload Decode(byte[] payload, Metric[] birthMetrics, bool command, bool nodeCommand)
        {
            if (payload == null) throw new ArgumentNullException(nameof(payload));
            if (payload.Length > WireBuffer.MaxLength) throw new FormatException("The Sparkplug payload is too large.");

            var reader = new WireReader(payload);
            var result = new SparkplugPayload();
            var metrics = new Metric[4];
            int count = 0;
            while (reader.Remaining > 0)
            {
                int field = ReadTag(reader);
                switch (field)
                {
                    case 8:
                        result.Timestamp = reader.ReadVarint();
                        break;
                    case 18:
                        Metric metric = DecodeMetric(reader.ReadMessage(), birthMetrics, command, nodeCommand);
                        if (metric != null)
                        {
                            if (count == metrics.Length)
                            {
                                var larger = new Metric[count * 2];
                                Array.Copy(metrics, larger, count);
                                metrics = larger;
                            }

                            metrics[count++] = metric;
                        }

                        break;
                    case 24:
                        ulong sequence = reader.ReadVarint();
                        if (sequence > byte.MaxValue) throw new FormatException("Invalid Sparkplug sequence number.");
                        result.Sequence = (byte)sequence;
                        result.HasSequence = true;
                        break;
                    default:
                        SkipField(reader, field);
                        break;
                }
            }

            result.Metrics = new Metric[count];
            Array.Copy(metrics, result.Metrics, count);
            return result;
        }

        private static byte[] EncodeMetric(Metric metric, bool includeDataType, ulong fallbackTimestamp)
        {
            if (metric == null) throw new ArgumentNullException(nameof(metric));
            if (!IsSupported(metric.DataType)) throw new ArgumentException("Unsupported Sparkplug metric data type.");

            var buffer = new WireBuffer();
            if (!StringHelper.IsNullOrWhiteSpace(metric.Name)) WriteBytes(buffer, 1, Encoding.UTF8.GetBytes(metric.Name));
            if (metric.HasAlias) WriteVarint(buffer, 2, metric.Alias);
            ulong timestamp = metric.Timestamp != 0 ? metric.Timestamp : fallbackTimestamp;
            if (timestamp != 0) WriteVarint(buffer, 3, timestamp);
            if (includeDataType) WriteVarint(buffer, 4, (uint)metric.DataType);
            if (metric.IsHistorical) WriteVarint(buffer, 5, 1);
            if (metric.IsTransient) WriteVarint(buffer, 6, 1);

            if (metric.Value == null)
            {
                WriteVarint(buffer, 7, 1);
            }
            else
            {
                WriteMetricValue(buffer, metric.DataType, metric.Value);
            }

            return buffer.ToArray();
        }

        private static void WriteMetricValue(WireBuffer buffer, MetricDataType type, object value)
        {
            switch (type)
            {
                case MetricDataType.Int8: WriteVarint(buffer, 10, unchecked((uint)(sbyte)value)); break;
                case MetricDataType.Int16: WriteVarint(buffer, 10, unchecked((uint)(short)value)); break;
                case MetricDataType.Int32: WriteVarint(buffer, 10, unchecked((uint)(int)value)); break;
                case MetricDataType.Int64: WriteVarint(buffer, 11, unchecked((ulong)(long)value)); break;
                case MetricDataType.UInt8: WriteVarint(buffer, 10, (byte)value); break;
                case MetricDataType.UInt16: WriteVarint(buffer, 10, (ushort)value); break;
                case MetricDataType.UInt32: WriteVarint(buffer, 10, (uint)value); break;
                case MetricDataType.UInt64:
                case MetricDataType.DateTime: WriteVarint(buffer, 11, (ulong)value); break;
                case MetricDataType.Float: WriteFixed(buffer, 12, 5, BitConverter.GetBytes((float)value)); break;
                case MetricDataType.Double: WriteFixed(buffer, 13, 1, BitConverter.GetBytes((double)value)); break;
                case MetricDataType.Boolean: WriteVarint(buffer, 14, (bool)value ? 1u : 0u); break;
                case MetricDataType.String:
                case MetricDataType.Text:
                case MetricDataType.Uuid: WriteBytes(buffer, 15, Encoding.UTF8.GetBytes((string)value)); break;
                case MetricDataType.Bytes:
                case MetricDataType.File: WriteBytes(buffer, 16, (byte[])value); break;
            }
        }

        private static Metric DecodeMetric(WireReader reader, Metric[] birthMetrics, bool command, bool nodeCommand)
        {
            var metric = new Metric(string.Empty, MetricDataType.Unknown, null);
            ulong number = 0;
            object value = null;
            bool isNull = false;
            bool hasDataType = false;
            int valueField = 0;
            while (reader.Remaining > 0)
            {
                int field = ReadTag(reader);
                switch (field)
                {
                    case 10:
                        byte[] name = reader.ReadBytes(reader.ReadLength());
                        metric.Name = Encoding.UTF8.GetString(name, 0, name.Length);
                        break;
                    case 16: metric.Alias = reader.ReadVarint(); break;
                    case 24: metric.Timestamp = reader.ReadVarint(); break;
                    case 32:
                        hasDataType = true;
                        metric.DataType = (MetricDataType)ReadUInt32(reader.ReadVarint());
                        break;
                    case 40: metric.IsHistorical = reader.ReadVarint() != 0; break;
                    case 48: metric.IsTransient = reader.ReadVarint() != 0; break;
                    case 56: isNull = reader.ReadVarint() != 0; break;
                    case 80:
                    case 88:
                    case 112: valueField = field; number = reader.ReadVarint(); break;
                    case 101: valueField = field; value = BitConverter.ToSingle(ReadFixed(reader, 4), 0); break;
                    case 105: valueField = field; value = BitConverter.ToDouble(ReadFixed(reader, 8), 0); break;
                    case 122:
                        valueField = field;
                        byte[] text = reader.ReadBytes(reader.ReadLength());
                        value = Encoding.UTF8.GetString(text, 0, text.Length);
                        break;
                    case 130: valueField = field; value = reader.ReadBytes(reader.ReadLength()); break;
                    default: SkipField(reader, field); break;
                }
            }

            if (!hasDataType || command)
            {
                MetricDataType expected = ResolveDataType(metric, birthMetrics, command, nodeCommand);
                if (!hasDataType) metric.DataType = expected;
                if (command && metric.DataType != expected) return null;
            }

            if (!IsSupported(metric.DataType)) return null;
            if (!isNull) metric.Value = DecodeValue(metric.DataType, valueField, number, value);
            return metric;
        }

        private static MetricDataType ResolveDataType(Metric metric, Metric[] birthMetrics, bool command, bool nodeCommand)
        {
            if (nodeCommand && metric.Name == RebirthMetricName) return MetricDataType.Boolean;
            if (command && metric.Name == BirthDeathMetricName) return MetricDataType.Int64;

            int index = FindMetric(birthMetrics, metric);
            return index < 0 ? MetricDataType.Unknown : birthMetrics[index].DataType;
        }

        internal static int FindMetric(Metric[] metrics, Metric update)
        {
            if (metrics == null) return -1;

            bool hasName = !StringHelper.IsNullOrWhiteSpace(update.Name);
            if (!update.HasAlias && !hasName) return -1;

            for (int index = 0; index < metrics.Length; index++)
            {
                Metric metric = metrics[index];
                if (metric == null) continue;
                if (hasName && update.Name != metric.Name) continue;
                if (update.HasAlias && (!metric.HasAlias || update.Alias != metric.Alias)) continue;
                return index;
            }

            return -1;
        }

        private static object DecodeValue(MetricDataType type, int field, ulong number, object value)
        {
            switch (type)
            {
                case MetricDataType.Int8 when field == 80: return unchecked((sbyte)ReadUInt32(number));
                case MetricDataType.Int16 when field == 80: return unchecked((short)ReadUInt32(number));
                case MetricDataType.Int32 when field == 80: return unchecked((int)ReadUInt32(number));
                case MetricDataType.Int64 when field == 88: return unchecked((long)number);
                case MetricDataType.UInt8 when field == 80 && number <= byte.MaxValue: return (byte)number;
                case MetricDataType.UInt16 when field == 80 && number <= ushort.MaxValue: return (ushort)number;
                case MetricDataType.UInt32 when field == 80 || field == 88: return ReadUInt32(number);
                case MetricDataType.UInt64 when field == 88:
                case MetricDataType.DateTime when field == 88: return number;
                case MetricDataType.Boolean when field == 112: return number != 0;
                case MetricDataType.Float when field == 101:
                case MetricDataType.Double when field == 105:
                case MetricDataType.String when field == 122:
                case MetricDataType.Text when field == 122:
                case MetricDataType.Uuid when field == 122:
                case MetricDataType.Bytes when field == 130:
                case MetricDataType.File when field == 130: return value;
                default: throw new FormatException("The protobuf value does not match the Sparkplug metric data type.");
            }
        }

        private static uint ReadUInt32(ulong number)
        {
            if (number > uint.MaxValue) throw new FormatException("Invalid protobuf uint32 value.");
            return (uint)number;
        }

        private static bool IsSupported(MetricDataType type)
        {
            return (type >= MetricDataType.Int8 && type <= MetricDataType.Uuid)
                || type == MetricDataType.Bytes || type == MetricDataType.File;
        }

        private static int ReadTag(WireReader reader)
        {
            ulong tag = reader.ReadVarint();
            if (tag < 8 || tag > int.MaxValue) throw new FormatException("Invalid protobuf field tag.");
            return (int)tag;
        }

        private static void SkipField(WireReader reader, int tag)
        {
            switch (tag & 7)
            {
                case 0: reader.ReadVarint(); break;
                case 1: reader.Skip(8); break;
                case 2: reader.Skip(reader.ReadLength()); break;
                case 5: reader.Skip(4); break;
                default: throw new FormatException("Unsupported protobuf wire type.");
            }
        }

        private static void WriteVarint(WireBuffer buffer, uint field, ulong value)
        {
            buffer.WriteVarint(field << 3);
            buffer.WriteVarint(value);
        }

        private static void WriteBytes(WireBuffer buffer, uint field, byte[] value)
        {
            buffer.WriteVarint((field << 3) | 2);
            buffer.WriteVarint((uint)value.Length);
            buffer.Write(value);
        }

        private static void WriteFixed(WireBuffer buffer, uint field, uint wireType, byte[] value)
        {
            buffer.WriteVarint((field << 3) | wireType);
            if (!BitConverter.IsLittleEndian) Reverse(value);
            buffer.Write(value);
        }

        private static byte[] ReadFixed(WireReader reader, int count)
        {
            byte[] bytes = reader.ReadBytes(count);
            if (!BitConverter.IsLittleEndian) Reverse(bytes);
            return bytes;
        }

        private static void Reverse(byte[] bytes)
        {
            for (int left = 0, right = bytes.Length - 1; left < right; left++, right--)
            {
                byte first = bytes[left];
                bytes[left] = bytes[right];
                bytes[right] = first;
            }
        }
    }
}
