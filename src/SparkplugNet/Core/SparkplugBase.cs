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
        /// The callback for the version A payload received event.
        /// </summary>
        public readonly Action<VersionAPayload>? VersionAPayloadReceived = null;

        /// <summary>
        /// The callback for the version B payload received event.
        /// </summary>
        public readonly Action<VersionBPayload>? VersionBPayloadReceived = null;

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
        /// Gets or sets the will message.
        /// </summary>
        protected MqttApplicationMessage? WillMessage { get; set; }

        /// <summary>
        /// Gets or sets the online message.
        /// </summary>
        protected MqttApplicationMessage? OnlineMessage { get; set; }

        /// <summary>
        /// Gets the Sparkplug namespace.
        /// </summary>
        protected SparkplugNamespace NameSpace { get; }

        /// <summary>
        /// Gets the known metric names.
        /// </summary>
        public List<T> KnownMetrics { get; }
    }
}