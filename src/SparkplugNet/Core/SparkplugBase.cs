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
    using System;
    using System.Collections.Generic;

    using MQTTnet;
    using MQTTnet.Client;
    using MQTTnet.Client.Options;

    using SparkplugNet.Core.Enumerations;
    using SparkplugNet.Core.Messages;

    using VersionAPayload = VersionA.Payload;
    using VersionBPayload = VersionB.Payload;

    /// <summary>
    /// A base class for all Sparkplug applications, nodes and devices.
    /// </summary>
    /// <typeparam name="T">The type parameter.</typeparam>
    public class SparkplugBase<T> where T : class, new()
    {
        /// <summary>
        /// The callback for the disconnected event. Indicates that metrics might be stale.
        /// </summary>
        public readonly Action? OnDisconnected = null;

        /// <summary>
        /// The message generator.
        /// </summary>
        protected readonly SparkplugMessageGenerator MessageGenerator = new ();

        /// <summary>
        /// The topic generator.
        /// </summary>
        protected readonly SparkplugTopicGenerator TopicGenerator = new ();

        /// <summary>
        /// The MQTT client.
        /// </summary>
        protected readonly IMqttClient Client;

        /// <summary>
        /// Initializes a new instance of the <see cref="SparkplugBase{T}"/> class.
        /// </summary>
        /// <param name="knownMetrics">The metric names.</param>
        public SparkplugBase(List<T> knownMetrics)
        {
            this.KnownMetrics = knownMetrics;

            this.NameSpace = this.KnownMetrics switch
            {
                List<VersionAPayload.KuraMetric> => SparkplugNamespace.VersionA,
                List<VersionBPayload.Metric> => SparkplugNamespace.VersionB,
                _ => SparkplugNamespace.VersionB
            };

            this.Client = new MqttFactory().CreateMqttClient();
        }

        /// <summary>
        /// Gets or sets the MQTT client options.
        /// </summary>
        protected IMqttClientOptions? ClientOptions { get; set; }
        
        /// <summary>
        /// Gets the last sequence number. Starts at 0 as it is incremented after the publishing (For the device and node relevant only).
        /// </summary>
        protected int LastSequenceNumber { get; private set; }

        /// <summary>
        /// Gets the last session number. Starts at -1 as it is incremented before the connect already.
        /// </summary>
        protected long LastSessionNumber { get; private set; } = -1;

        /// <summary>
        /// Gets the Sparkplug namespace.
        /// </summary>
        protected SparkplugNamespace NameSpace { get; }

        /// <summary>
        /// Gets the known metric names.
        /// </summary>
        public List<T> KnownMetrics { get; }

        /// <summary>
        /// Increments the last sequence number.
        /// </summary>
        public void IncrementLastSequenceNumber()
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

        /// <summary>
        /// Increments the last session number.
        /// </summary>
        public void IncrementLastSessionNumber()
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