// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumExtensions.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   The Sparkplug client class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet
{
    using System.Threading;
    using System.Threading.Tasks;

    using SparkplugNet.Enumerations;

    using MQTTnet;
    using MQTTnet.Client;
    using MQTTnet.Client.Options;
    using MQTTnet.Formatter;
    using MQTTnet.Protocol;

    using SparkplugNet.Messages;

    /// <summary>
    /// The Sparkplug client class.
    /// </summary>
    public class SparkplugClient
    {
        /// <summary>
        /// The MQTT client.
        /// </summary>
        private readonly IMqttClient client;

        /// <summary>
        /// The Sparkplug version.
        /// </summary>
        private readonly SparkplugVersion version;

        /// <summary>
        /// The Sparkplug namespace.
        /// </summary>
        private readonly SparkplugNamespace nameSpace;

        /// <summary>
        /// The MQTT client options.
        /// </summary>
        private IMqttClientOptions? clientOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="SparkplugClient"/> class.
        /// </summary>
        /// <param name="version">The Sparkplug version.</param>
        /// <param name="nameSpace">The Sparkplug namespace.</param>
        public SparkplugClient(SparkplugVersion version, SparkplugNamespace nameSpace)
        {
            this.client = new MqttFactory().CreateMqttClient();
            this.version = version;
            this.nameSpace = nameSpace;
        }

        // Todo: exceptions dokumentieren
        public async Task Connect(
            string brokerAddress,
            string clientId,
            string userName,
            byte[] password,
            bool useTls,
            MqttClientOptionsBuilderWebSocketParameters? webSocketParameters = null,
            MqttClientWebSocketProxyOptions? proxyOptions = null,
            CancellationToken? cancellationToken = null)
        {
            cancellationToken ??= CancellationToken.None;

            var builder = new MqttClientOptionsBuilder()
                .WithClientId(clientId)
                .WithCredentials(userName, password)
                .WithCleanSession(false)
                .WithProtocolVersion(MqttProtocolVersion.V311)
                // Todo: Add death will message here!
                .WithWillMessage(new MqttApplicationMessage());

            if (useTls)
            {
                builder.WithTls();
            }

            if (webSocketParameters is null)
            {
                builder.WithTcpServer(brokerAddress);
            }
            else
            {
                builder.WithWebSocketServer(brokerAddress, webSocketParameters);
            }

            if (proxyOptions != null)
            {
                builder.WithProxy(proxyOptions.Address, proxyOptions.Username, proxyOptions.Password, proxyOptions.Domain, proxyOptions.BypassOnLocal);
            }

            this.clientOptions = builder.Build();

            await this.client.ConnectAsync(this.clientOptions, cancellationToken.Value);
        }

        public async Task Disconnect()
        {
            await this.client.DisconnectAsync();
        }

        public async Task Subscribe(string topic, SparkplugQualityOfServiceLevel qualityOfServiceLevel)
        {
            await this.client.SubscribeAsync(topic, (MqttQualityOfServiceLevel)qualityOfServiceLevel);
        }

        public async Task Publish(
            string groupIdentifier,
            SparkplugMessageType messageType,
            string edgeNodeIdentifier,
            string? deviceIdentifier,
            SparkplugQualityOfServiceLevel qualityOfServiceLevel,
            CancellationToken? cancellationToken = null)
        {
            cancellationToken ??= CancellationToken.None;

            var applicationMessage = SparkplugMessageGenerator.CreateSparkplugMessage(
                this.version,
                this.nameSpace,
                groupIdentifier,
                messageType,
                edgeNodeIdentifier,
                deviceIdentifier,
                qualityOfServiceLevel);

            await this.client.PublishAsync(applicationMessage, cancellationToken.Value);
        }
    }
}