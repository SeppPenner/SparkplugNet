#nullable disable

namespace SparkplugNet.NanoFramework
{
    /// <summary>The scalar Sparkplug B data types supported by the nanoFramework port.</summary>
    public enum MetricDataType : uint
    {
        /// <summary>Unspecified data type.</summary>
        Unknown = 0,
        /// <summary>Signed 8-bit integer.</summary>
        Int8 = 1,
        /// <summary>Signed 16-bit integer.</summary>
        Int16 = 2,
        /// <summary>Signed 32-bit integer.</summary>
        Int32 = 3,
        /// <summary>Signed 64-bit integer.</summary>
        Int64 = 4,
        /// <summary>Unsigned 8-bit integer.</summary>
        UInt8 = 5,
        /// <summary>Unsigned 16-bit integer.</summary>
        UInt16 = 6,
        /// <summary>Unsigned 32-bit integer.</summary>
        UInt32 = 7,
        /// <summary>Unsigned 64-bit integer.</summary>
        UInt64 = 8,
        /// <summary>Single-precision floating-point number.</summary>
        Float = 9,
        /// <summary>Double-precision floating-point number.</summary>
        Double = 10,
        /// <summary>Boolean value.</summary>
        Boolean = 11,
        /// <summary>UTF-8 string.</summary>
        String = 12,
        /// <summary>Unsigned Unix timestamp in milliseconds.</summary>
        DateTime = 13,
        /// <summary>UTF-8 text.</summary>
        Text = 14,
        /// <summary>UUID represented as a string.</summary>
        Uuid = 15,
        /// <summary>Raw byte array.</summary>
        Bytes = 17,
        /// <summary>File content represented as a byte array.</summary>
        File = 18
    }

    /// <summary>A Sparkplug B metric. A null value is sent with the is_null flag.</summary>
    public sealed class Metric
    {
        private ulong alias;

        /// <summary>Creates a metric with the given name, type, and value.</summary>
        public Metric(string name, MetricDataType dataType, object value)
        {
            Name = name;
            DataType = dataType;
            Value = value;
        }

        /// <summary>The metric name. Aliased updates can omit it.</summary>
        public string Name { get; set; }
        /// <summary>The Sparkplug data type.</summary>
        public MetricDataType DataType { get; set; }
        /// <summary>The boxed metric value, or null for an explicitly missing value.</summary>
        public object Value { get; set; }
        /// <summary>Unix time in milliseconds, or zero to omit the metric timestamp.</summary>
        public ulong Timestamp { get; set; }
        /// <summary>Whether the value was collected in the past.</summary>
        public bool IsHistorical { get; set; }
        /// <summary>Whether the value is transient.</summary>
        public bool IsTransient { get; set; }
        /// <summary>Whether an alias has been assigned.</summary>
        public bool HasAlias { get; private set; }

        /// <summary>The assigned alias. Zero is a valid alias.</summary>
        public ulong Alias
        {
            get { return alias; }
            set
            {
                alias = value;
                HasAlias = true;
            }
        }
    }

    /// <summary>A decoded Sparkplug B payload.</summary>
    public sealed class SparkplugPayload
    {
        /// <summary>The payload timestamp in Unix milliseconds.</summary>
        public ulong Timestamp { get; set; }
        /// <summary>The sequence number when present.</summary>
        public byte Sequence { get; set; }
        /// <summary>Whether the payload contains a sequence number.</summary>
        public bool HasSequence { get; set; }
        /// <summary>The supported metrics decoded from the payload.</summary>
        public Metric[] Metrics { get; set; }
    }
}
