// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugDevice.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class that handles a Sparkplug device.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Device
{
    using System.Threading;
    using System.Threading.Tasks;

    using MQTTnet;
    using MQTTnet.Client;
    using MQTTnet.Client.Options;
    using MQTTnet.Formatter;
    using MQTTnet.Protocol;

    using SparkplugNet.Enumerations;
    using SparkplugNet.Extensions;

    using VersionAPayload = Payloads.VersionA.Payload;
    using VersionBPayload = Payloads.VersionB.Payload;

    /// <inheritdoc cref="SparkplugBase"/>
    /// <summary>
    /// A class that handles a Sparkplug application.
    /// </summary>
    /// <seealso cref="SparkplugBase"/>
    public class SparkplugDevice : SparkplugBase
    {
        /// <summary>
        /// The will message.
        /// </summary>
        private MqttApplicationMessage? willMessage;

        /// <summary>
        /// The device online message.
        /// </summary>
        private MqttApplicationMessage? deviceOnlineMessage;

        /// <inheritdoc cref="SparkplugBase"/>
        /// <summary>
        /// Initializes a new instance of the <see cref="SparkplugDevice"/> class.
        /// </summary>
        /// <param name="nameSpace">The namespace.</param>
        /// <seealso cref="SparkplugBase"/>
        public SparkplugDevice(SparkplugNamespace nameSpace) : base(nameSpace)
        {
        }

        /// <summary>
        /// Starts the Sparkplug device.
        /// </summary>
        /// <param name="options">The configuration option.</param>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        public async Task Start(SparkplugDeviceOptions options)
        {
            // Load messages
            this.LoadMessages(options);

            // Add handlers
            this.AddDisconnectedHandler(options);
            this.AddMessageReceivedHandler();

            // Connect, subscribe to incoming messages and send a state message
            await this.ConnectInternal(options);
            await this.SubscribeInternal(options);
            await this.PublishInternal(options);
        }

        /// <summary>
        /// Stops the Sparkplug device.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        public async Task Stop()
        {
            await this.Client.DisconnectAsync();
        }

        /// <summary>
        /// Loads the messages used by the the Sparkplug device.
        /// </summary>
        /// <param name="options">The configuration option.</param>
        private void LoadMessages(SparkplugDeviceOptions options)
        {
            this.willMessage = this.MessageGenerator.CreateSparkplugMessage(
                this.NameSpace,
                options.GroupIdentifier,
                SparkplugMessageType.DeviceDeath,
                options.EdgeNodeIdentifier,
                options.DeviceIdentifier);

            this.deviceOnlineMessage = this.MessageGenerator.CreateSparkplugMessage(
                this.NameSpace,
                options.GroupIdentifier,
                SparkplugMessageType.DeviceBirth,
                options.EdgeNodeIdentifier,
                null);
        }

        /// <summary>
        /// Adds the disconnected handler and the reconnect functionality to the client.
        /// </summary>
        /// <param name="options">The configuration option.</param>
        private void AddDisconnectedHandler(SparkplugDeviceOptions options)
        {
            this.Client.UseDisconnectedHandler(
                async e =>
                    {
                        // Wait until the disconnect interval is reached
                        await Task.Delay(options.ReconnectInterval);

                        // Connect, subscribe to incoming messages and send a state message
                        await this.ConnectInternal(options);
                        await this.SubscribeInternal(options);
                        await this.PublishInternal(options);
                    });
        }

        /// <summary>
        /// Adds the message received handler to handle incoming messages.
        /// </summary>
        private void AddMessageReceivedHandler()
        {
            this.Client.UseApplicationMessageReceivedHandler(
                e =>
                    {
                        var topic = e.ApplicationMessage.Topic;

                        if (topic.Contains(SparkplugMessageType.DeviceCommand.GetDescription()))
                        {
                            switch (this.NameSpace)
                            {
                                case SparkplugNamespace.VersionA:
                                    var payloadVersionA = PayloadHelper.Deserialize<VersionAPayload>(e.ApplicationMessage.Payload);

                                    if (payloadVersionA != null)
                                    {
                                        this.VersionAPayloadReceived?.Invoke(payloadVersionA);
                                    }

                                    break;

                                case SparkplugNamespace.VersionB:
                                    var payloadVersionB = PayloadHelper.Deserialize<VersionBPayload>(e.ApplicationMessage.Payload);

                                    if (payloadVersionB != null)
                                    {
                                        this.VersionBPayloadReceived?.Invoke(payloadVersionB);
                                    }

                                    break;
                            }
                        }
                    });
        }

        /// <summary>
        /// Connects the Sparkplug device to the MQTT broker.
        /// </summary>
        /// <param name="options">The configuration option.</param>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        private async Task ConnectInternal(SparkplugDeviceOptions options)
        {
            options.CancellationToken ??= CancellationToken.None;

            var builder = new MqttClientOptionsBuilder()
                .WithClientId(options.ClientId)
                .WithCredentials(options.UserName, options.Password)
                .WithCleanSession(false)
                .WithProtocolVersion(MqttProtocolVersion.V311);

            if (options.UseTls)
            {
                builder.WithTls();
            }

            if (options.WebSocketParameters is null)
            {
                builder.WithTcpServer(options.BrokerAddress, options.Port);
            }
            else
            {
                builder.WithWebSocketServer(options.BrokerAddress, options.WebSocketParameters);
            }

            if (options.ProxyOptions != null)
            {
                builder.WithProxy(
                    options.ProxyOptions.Address,
                    options.ProxyOptions.Username,
                    options.ProxyOptions.Password,
                    options.ProxyOptions.Domain,
                    options.ProxyOptions.BypassOnLocal);
            }

            if (this.willMessage != null)
            {
                builder.WithWillMessage(this.willMessage);
            }

            this.ClientOptions = builder.Build();

            await this.Client.ConnectAsync(this.ClientOptions, options.CancellationToken.Value);
        }

        /// <summary>
        /// Publishes data to the MQTT broker.
        /// </summary>
        /// <param name="options">The configuration option.</param>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        private async Task PublishInternal(SparkplugDeviceOptions options)
        {
            options.CancellationToken ??= CancellationToken.None;
            await this.Client.PublishAsync(this.deviceOnlineMessage, options.CancellationToken.Value);
        }

        /// <summary>
        /// Subscribes the client to the device subscribe topic.
        /// </summary>
        /// <param name="options">The configuration option.</param>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        private async Task SubscribeInternal(SparkplugDeviceOptions options)
        {
            var deviceCommandSubscribeTopic = this.TopicGenerator.GetDeviceCommandSubscribeTopic(this.NameSpace, options.GroupIdentifier, options.EdgeNodeIdentifier, options.DeviceIdentifier);
            await this.Client.SubscribeAsync(deviceCommandSubscribeTopic, MqttQualityOfServiceLevel.AtLeastOnce);
        }
    }
}