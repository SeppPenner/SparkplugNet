// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KuraPayload.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class that handles a Sparkplug version A payload.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Payloads.VersionA
{
    using System;

    using ProtoBuf;

    /// <summary>
    /// A class that handles a Sparkplug version A payload.
    /// </summary>
    [ProtoContract]
    public class KuraPayload
    {
        /// <summary>
        /// The metrics array.
        /// </summary>
        private KuraMetric[]? metric = new KuraMetric[4997];

        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        [ProtoMember(1)]
        public long? Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        [ProtoMember(2)]
        public KuraPosition Position { get; set; } = new KuraPosition();

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        public KuraMetric[]? Metric
        {
            get => this.metric;
            set
            {
                if (value != null)
                {
                    if (value.Length > 4997)
                    {
                        throw new ArgumentException(nameof(this.Metric));
                    }
                }

                this.metric = value;
            }
        }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        [ProtoMember(5001)]
        public byte[]? Body { get; set; }
    }
}