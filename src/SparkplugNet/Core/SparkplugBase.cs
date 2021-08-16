// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugBase.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A base class for all Sparkplug applications, nodes and devices.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Core
{
    using System.Collections.Generic;
    using SparkplugNet.Core.Enumerations;
    using SparkplugNet.Core.Messages;

    /// <summary>A base class for all Sparkplug applications, nodes and devices.</summary>
    /// <typeparam name="T">The type parameter.</typeparam>
    public class SparkplugBase<T>
        where T : class, new()
    {
        /// <summary>The message generator.</summary>
        internal readonly SparkplugMessageGenerator MessageGenerator = new();

        /// <summary>The topic generator.</summary>
        internal readonly SparkplugTopicGenerator TopicGenerator = new();

        /// <summary>Initializes a new instance of the <see cref="SparkplugBase{T}" /> class.</summary>
        /// <param name="knownMetrics">The metric names.</param>
        public SparkplugBase(List<T> knownMetrics)
        {
            this.KnownMetrics = knownMetrics;

            this.NameSpace = this.KnownMetrics switch
            {
                List<VersionA.Payload.KuraMetric> => SparkplugNamespace.VersionA,
                List<VersionB.Payload.Metric> => SparkplugNamespace.VersionB,
                _ => SparkplugNamespace.VersionB,
            };
        }

        /// <summary>Gets the known metric names.</summary>
        public List<T> KnownMetrics { get; }

        /// <summary>Gets the last session number. Starts at -1 as it is incremented before the connect already.</summary>
        protected long LastSessionNumber { get; private set; } = -1;

        /// <summary>Gets the Sparkplug namespace.</summary>
        protected SparkplugNamespace NameSpace { get; }

        /// <summary>Gets the last sequence number. Starts at 0 as it is incremented after the publishing (For the device and node relevant only).</summary>
        internal int LastSequenceNumber { get; set; }

        /// <summary>Resets the last sequence number.</summary>
        internal void ResetLastSequenceNumber()
        {
            this.LastSequenceNumber = 0;
        }

        /// <summary>Increments the last sequence number.</summary>
        internal virtual void IncrementLastSequenceNumber()
        {
            if (this.LastSequenceNumber == 255)
            {
                this.LastSequenceNumber = 0;
            }
            else
            {
                this.LastSequenceNumber++;
            }
        }

        /// <summary>Increments the last session number.</summary>
        internal void IncrementLastSessionNumber()
        {
            if (this.LastSessionNumber == long.MaxValue)
            {
                this.LastSessionNumber = 0;
            }
            else
            {
                this.LastSessionNumber++;
            }
        }
    }
}