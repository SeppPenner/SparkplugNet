// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugApplication.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class that handles a Sparkplug application.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Core.Application
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using MQTTnet.Client;
    using MQTTnet.Client.Options;
    using MQTTnet.Formatter;
    using MQTTnet.Protocol;

    using SparkplugNet.Core.Enumerations;
    using SparkplugNet.Core.Extensions;

    using VersionAPayload = VersionA.Payload;
    using VersionBPayload = VersionB.Payload;

    /// <inheritdoc cref="SparkplugBase{T}"/>
    /// <summary>
    /// A class that handles a Sparkplug application.
    /// </summary>
    /// <seealso cref="SparkplugBase{T}"/>
    public class SparkplugApplicationBase<T> : SparkplugBase<T> where T : class, new()
    {
        /// <inheritdoc cref="SparkplugBase{T}"/>
        /// <summary>
        /// Initializes a new instance of the <see cref="SparkplugApplicationBase{T}"/> class.
        /// </summary>
        /// <param name="knownMetrics">The metric names.</param>
        /// <seealso cref="SparkplugBase{T}"/>
        public SparkplugApplicationBase(List<T> knownMetrics) : base(knownMetrics)
        {
        }
        
        /// <summary>
        /// Gets the node states.
        /// </summary>
        public ConcurrentDictionary<string, MetricState<T>> NodeStates{ get; } = new ();

        /// <summary>
        /// Gets the node states.
        /// </summary>
        public ConcurrentDictionary<string, MetricState<T>> DeviceStates{ get; } = new ();

        /// <summary>
        /// Starts the Sparkplug application.
        /// </summary>
        /// <param name="options">The configuration option.</param>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        public async Task Start(SparkplugApplicationOptions options)
        {
            // Clear states
            this.NodeStates.Clear();
            this.DeviceStates.Clear();

            // Load messages
            this.LoadMessages(options);

            // Add handlers
            this.AddDisconnectedHandler(options);
            this.AddMessageReceivedHandler();

            // Connect, subscribe to incoming messages and send a state message
            await this.ConnectInternal(options);
            await this.SubscribeInternal();
            await this.PublishInternal(options);
        }

        /// <summary>
        /// Stops the Sparkplug application.
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
        private void LoadMessages(SparkplugApplicationOptions options)
        {
            this.WillMessage = this.MessageGenerator.GetSparkplugStateMessage(
                this.NameSpace,
                options.ScadaHostIdentifier,
                false);

            this.OnlineMessage = this.MessageGenerator.GetSparkplugStateMessage(
                this.NameSpace,
                options.ScadaHostIdentifier,
                true);
        }

        /// <summary>
        /// Adds the disconnected handler and the reconnect functionality to the client.
        /// </summary>
        /// <param name="options">The configuration option.</param>
        private void AddDisconnectedHandler(SparkplugApplicationOptions options)
        {
            this.Client.UseDisconnectedHandler(
                async _ =>
                    {
                        // Set all metrics to stale
                        this.UpdateMetricState(SparkplugMetricStatus.Offline);

                        // Invoke disconnected callback
                        this.OnDisconnected?.Invoke();

                        // Wait until the disconnect interval is reached
                        await Task.Delay(options.ReconnectInterval);

                        // Connect, subscribe to incoming messages and send a state message
                        await this.ConnectInternal(options);
                        this.UpdateMetricState(SparkplugMetricStatus.Online);
                        await this.SubscribeInternal();
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

                        var needsPayloadHanding = topic.Contains(SparkplugMessageType.NodeBirth.GetDescription())
                                                  || topic.Contains(SparkplugMessageType.NodeDeath.GetDescription())
                                                  || topic.Contains(SparkplugMessageType.DeviceBirth.GetDescription())
                                                  || topic.Contains(SparkplugMessageType.DeviceDeath.GetDescription())
                                                  || topic.Contains(SparkplugMessageType.NodeData.GetDescription())
                                                  || topic.Contains(SparkplugMessageType.DeviceData.GetDescription())
                                                  || topic.Contains(SparkplugMessageType.NodeCommand.GetDescription())
                                                  || topic.Contains(SparkplugMessageType.DeviceCommand.GetDescription());

                        if (needsPayloadHanding)
                        {
                            switch (this.NameSpace)
                            {
                                case SparkplugNamespace.VersionA:
                                    var payloadVersionA = PayloadHelper.Deserialize<VersionAPayload>(e.ApplicationMessage.Payload);

                                    if (payloadVersionA != null)
                                    {
                                        // Todo: Store metrics for node if metrics are known
                                        // Todo: Store metrics for device if metrics are known
                                        this.VersionAPayloadReceived?.Invoke(payloadVersionA);
                                    }

                                    break;

                                case SparkplugNamespace.VersionB:
                                    var payloadVersionB = PayloadHelper.Deserialize<VersionBPayload>(e.ApplicationMessage.Payload);

                                    if (payloadVersionB != null)
                                    {
                                        // Todo: Store metrics for node if metrics are known
                                        // Todo: Store metrics for device if metrics are known
                                        this.VersionBPayloadReceived?.Invoke(payloadVersionB);
                                    }

                                    break;
                            }
                        }
                    });
        }

        /// <summary>
        /// Connects the Sparkplug application to the MQTT broker.
        /// </summary>
        /// <param name="options">The configuration option.</param>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        private async Task ConnectInternal(SparkplugApplicationOptions options)
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

            if (this.WillMessage != null && options.IsPrimaryApplication)
            {
                builder.WithWillMessage(this.WillMessage);
            }

            this.ClientOptions = builder.Build();

            await this.Client.ConnectAsync(this.ClientOptions, options.CancellationToken.Value);
        }

        /// <summary>
        /// Publishes data to the MQTT broker.
        /// </summary>
        /// <param name="options">The configuration option.</param>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        private async Task PublishInternal(SparkplugApplicationOptions options)
        {
            // Only send state messages for the primary application
            if (options.IsPrimaryApplication)
            {
                options.CancellationToken ??= CancellationToken.None;
                await this.Client.PublishAsync(this.OnlineMessage, options.CancellationToken.Value);
            }
        }

        /// <summary>
        /// Subscribes the client to the application subscribe topic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        private async Task SubscribeInternal()
        {
            var topic = this.TopicGenerator.GetWildcardNamespaceSubscribeTopic(this.NameSpace);
            await this.Client.SubscribeAsync(topic, MqttQualityOfServiceLevel.AtLeastOnce);
        }

        /// <summary>
        /// Updates the metric state.
        /// </summary>
        /// <param name="metricState">The metric state.</param>
        private void UpdateMetricState(SparkplugMetricStatus metricState)
        {
            var keys = new List<string>(this.NodeStates.Keys.ToList());

            foreach (string key in keys)
            {
                this.NodeStates[key].MetricStatus = metricState;
            }
        }
    }
}