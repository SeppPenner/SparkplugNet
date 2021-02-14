// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KuraPosition.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class that handles a Sparkplug version A payload position.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Payloads.VersionA
{
    using ProtoBuf;

    /// <summary>
    /// A class that handles a Sparkplug version A payload position.
    /// </summary>
    [ProtoContract]
    public class KuraPosition
    {
        /// <summary>
        /// Gets or sets the latitude.
        /// </summary>
        [ProtoMember(1)]
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude.
        /// </summary>
        [ProtoMember(2)]
        public double Longitude { get; set; }

        /// <summary>
        /// Gets or sets the altitude.
        /// </summary>
        [ProtoMember(3)]
        public double? Altitude { get; set; }

        /// <summary>
        /// Gets or sets the precision dilution of the current satellite fix. 
        /// </summary>
        [ProtoMember(4)]
        public double? Precision { get; set; }

        /// <summary>
        /// Gets or sets the heading in degrees.
        /// </summary>
        [ProtoMember(5)]
        public double? Heading { get; set; }

        /// <summary>
        /// Gets or sets the speed in  meters per second.
        /// </summary>
        [ProtoMember(6)]
        public double? Speed { get; set; }

        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        [ProtoMember(7)]
        public long? Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the satellites. The number of satellites locked by the GPS device.
        /// </summary>
        [ProtoMember(8)]
        public int? Satellites { get; set; }

        /// <summary>
        /// Gets or sets the status indicator for the GPS data: 1 = no GPS response; 2 = error in response; 4 = valid.
        /// </summary>
        [ProtoMember(9)]
        public int? Status { get; set; }

        /// <summary>
        /// Gets the position status.
        /// </summary>
        public KuraPositionStatus? PositionStatus => this.Status is null ? null : (KuraPositionStatus?)this.Status;
    }
}