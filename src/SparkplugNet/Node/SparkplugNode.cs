// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugNode.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class that handles a Sparkplug node.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Node
{
    using System;
    using System.Collections.Concurrent;
    using System.Text;
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
    /// A class that handles a Sparkplug node.
    /// </summary>
    /// <seealso cref="SparkplugBase"/>
    public class SparkplugNode : SparkplugBase
    {
        /// <summary>
        /// The will message.
        /// </summary>
        private MqttApplicationMessage? willMessage;

        /// <summary>
        /// The node online message.
        /// </summary>
        private MqttApplicationMessage? nodeOnlineMessage;

        /// <inheritdoc cref="SparkplugBase"/>
        /// <summary>
        /// Initializes a new instance of the <see cref="SparkplugNode"/> class.
        /// </summary>
        /// <param name="nameSpace">The namespace.</param>
        /// <seealso cref="SparkplugBase"/>
        public SparkplugNode(SparkplugNamespace nameSpace) : base(nameSpace)
        {
        }

        /// <summary>
        /// The callback for the status message received event.
        /// </summary>
        public readonly Action<string>? StatusMessageReceived = null;

        /// <summary>
        /// Gets the device states for the payload version A.
        /// </summary>
        public ConcurrentDictionary<string, VersionAPayload.KuraMetric> DeviceStatesPayloadA { get; } = new ();

        /// <summary>
        /// Gets the device states for the payload version B.
        /// </summary>
        public ConcurrentDictionary<string, VersionBPayload.Metric> DeviceStatesPayloadB { get; } = new ();

        /// <summary>
        /// Starts the Sparkplug node.
        /// </summary>
        /// <param name="options">The configuration option.</param>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        public async Task Start(SparkplugNodeOptions options)
        {
            // Clear states
            this.DeviceStatesPayloadA.Clear();
            this.DeviceStatesPayloadB.Clear();

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
        /// Stops the Sparkplug node.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        public async Task Stop()
        {
            await this.Client.DisconnectAsync();
        }

        /// <summary>
        /// Loads the messages used by the the Sparkplug application.
        /// </summary>
        /// <param name="options">The configuration option.</param>
        private void LoadMessages(SparkplugNodeOptions options)
        {
            this.willMessage = this.MessageGenerator.CreateSparkplugMessage(
                this.NameSpace,
                options.GroupIdentifier,
                SparkplugMessageType.NodeDeath,
                options.EdgeNodeIdentifier,
                null);

            this.nodeOnlineMessage = this.MessageGenerator.CreateSparkplugMessage(
                this.NameSpace,
                options.GroupIdentifier,
                SparkplugMessageType.NodeBirth,
                options.EdgeNodeIdentifier,
                null);
        }

        /// <summary>
        /// Adds the disconnected handler and the reconnect functionality to the client.
        /// </summary>
        /// <param name="options">The configuration option.</param>
        private void AddDisconnectedHandler(SparkplugNodeOptions options)
        {
            this.Client.UseDisconnectedHandler(
                async e =>
                    {
                        // Todo: Use the metrics correctly.
                        //// Set all states to unknown as we are disconnected
                        //foreach (var nodeState in this.DeviceStatesPayloadA)
                        //{
                        //    var value = this.DeviceStatesPayloadA[nodeState.Key];
                        //    value.ConnectionStatus = SparkplugConnectionStatus.Unknown;
                        //    this.DeviceStatesPayloadA[nodeState.Key] = value;
                        //}

                        //// Set all states to unknown as we are disconnected
                        //foreach (var nodeState in this.DeviceStatesPayloadB)
                        //{
                        //    var value = this.DeviceStatesPayloadB[nodeState.Key];
                        //    value.ConnectionStatus = SparkplugConnectionStatus.Unknown;
                        //    this.DeviceStatesPayloadB[nodeState.Key] = value;
                        //}

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

                        if (topic.Contains(SparkplugMessageType.NodeCommand.GetDescription()) || topic.Contains(SparkplugMessageType.DeviceCommand.GetDescription()))
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

                        if (topic.Contains(SparkplugMessageType.StateMessage.GetDescription()))
                        {
                            this.StatusMessageReceived?.Invoke(Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
                        }
                    });
        }

        /// <summary>
        /// Connects the Sparkplug node to the MQTT broker.
        /// </summary>
        /// <param name="options">The configuration option.</param>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        private async Task ConnectInternal(SparkplugNodeOptions options)
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
        private async Task PublishInternal(SparkplugNodeOptions options)
        {
            options.CancellationToken ??= CancellationToken.None;
            await this.Client.PublishAsync(this.nodeOnlineMessage, options.CancellationToken.Value);
        }

        /// <summary>
        /// Subscribes the client to the node subscribe topics.
        /// </summary>
        /// <param name="options">The configuration option.</param>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        private async Task SubscribeInternal(SparkplugNodeOptions options)
        {
            var nodeCommandSubscribeTopic = this.TopicGenerator.GetNodeCommandSubscribeTopic(this.NameSpace, options.GroupIdentifier, options.EdgeNodeIdentifier);
            await this.Client.SubscribeAsync(nodeCommandSubscribeTopic, MqttQualityOfServiceLevel.AtLeastOnce);

            var deviceCommandSubscribeTopic = this.TopicGenerator.GetWildcardDeviceCommandSubscribeTopic(this.NameSpace, options.GroupIdentifier, options.EdgeNodeIdentifier);
            await this.Client.SubscribeAsync(deviceCommandSubscribeTopic, MqttQualityOfServiceLevel.AtLeastOnce);

            var stateSubscribeTopic = this.TopicGenerator.GetStateSubscribeTopic(options.ScadaHostIdentifier);
            await this.Client.SubscribeAsync(stateSubscribeTopic, MqttQualityOfServiceLevel.AtLeastOnce);
        }
    }
}