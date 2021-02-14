// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugBase.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A base class for all Sparkplug applications, nodes and devices.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet
{
    using MQTTnet;
    using MQTTnet.Client;
    using MQTTnet.Client.Options;

    using SparkplugNet.Enumerations;
    using SparkplugNet.Messages;

    /// <summary>
    /// A base class for all Sparkplug applications, nodes and devices.
    /// </summary>
    public class SparkplugBase
    {
        /// <summary>
        /// The message generator.
        /// </summary>
        protected readonly SparkplugMessageGenerator MessageGenerator = new SparkplugMessageGenerator();

        /// <summary>
        /// The topic generator.
        /// </summary>
        protected readonly SparkplugTopicGenerator TopicGenerator = new SparkplugTopicGenerator();

        /// <summary>
        /// The MQTT client.
        /// </summary>
        protected readonly IMqttClient Client;

        /// <summary>
        /// The Sparkplug namespace.
        /// </summary>
        protected readonly SparkplugNamespace NameSpace;

        /// <summary>
        /// Initializes a new instance of the <see cref="SparkplugBase"/> class.
        /// </summary>
        /// <param name="nameSpace">The namespace.</param>
        public SparkplugBase(SparkplugNamespace nameSpace)
        {
            this.NameSpace = nameSpace;
            this.Client = new MqttFactory().CreateMqttClient();
        }

        /// <summary>
        /// Gets or sets the MQTT client options.
        /// </summary>
        protected IMqttClientOptions? ClientOptions { get; set; }
    }
}