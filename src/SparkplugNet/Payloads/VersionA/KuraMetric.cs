// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KuraMetric.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class that handles a Sparkplug version A payload metric.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Payloads.VersionA
{
    using ProtoBuf;

    /// <summary>
    /// A class that handles a Sparkplug version A payload metric.
    /// </summary>
    // ReSharper disable InconsistentNaming
    [ProtoContract]
    public class KuraMetric
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [ProtoMember(1)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        [ProtoMember(2)]
        public KuraValueType Type { get; set; }

        /// <summary>
        /// Gets or sets the double value.
        /// </summary>
        [ProtoMember(3)]
        public double? Double_Value { get; set; }

        /// <summary>
        /// Gets or sets the float value.
        /// </summary>
        [ProtoMember(4)]
        public float? Float_Value { get; set; }

        /// <summary>
        /// Gets or sets the long value.
        /// </summary>
        [ProtoMember(5)]
        public long? Long_Value { get; set; }

        /// <summary>
        /// Gets or sets the int value.
        /// </summary>
        [ProtoMember(6)]
        public int? Int_Value { get; set; }

        /// <summary>
        /// Gets or sets the bool value.
        /// </summary>
        [ProtoMember(7)]
        public bool? Bool_Value { get; set; }

        /// <summary>
        /// Gets or sets the string value.
        /// </summary>
        [ProtoMember(8)]
        public string? String_Value { get; set; }

        /// <summary>
        /// Gets or sets the bytes value.
        /// </summary>
        [ProtoMember(9)]
        public byte[]? Bytes_Value { get; set; }
    }
}