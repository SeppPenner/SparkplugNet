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
    using System;
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
        /// <summary>
        /// The options.
        /// </summary>
        private SparkplugApplicationOptions? options;

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
        /// <param name="applicationOptions">The application option.</param>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        public async Task Start(SparkplugApplicationOptions applicationOptions)
        {
            // Storing the options.
            this.options = applicationOptions;

            if (this.options is null)
            {
                throw new ArgumentNullException(nameof(this.options));
            }

            // Clear states.
            this.NodeStates.Clear();
            this.DeviceStates.Clear();

            // Add handlers.
            this.AddDisconnectedHandler();
            this.AddMessageReceivedHandler();

            // Connect, subscribe to incoming messages and send a state message.
            await this.ConnectInternal();
            await this.SubscribeInternal();
            await this.PublishInternal();
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
        /// Adds the disconnected handler and the reconnect functionality to the client.
        /// </summary>
        private void AddDisconnectedHandler()
        {
            this.Client.UseDisconnectedHandler(
                async _ =>
                    {
                        if (this.options is null)
                        {
                            throw new ArgumentNullException(nameof(this.options));
                        }

                        // Set all metrics to stale.
                        this.UpdateMetricState(SparkplugMetricStatus.Offline);

                        // Invoke disconnected callback.
                        this.OnDisconnected?.Invoke();

                        // Wait until the disconnect interval is reached.
                        await Task.Delay(this.options.ReconnectInterval);

                        // Connect, subscribe to incoming messages and send a state message.
                        await this.ConnectInternal();
                        this.UpdateMetricState(SparkplugMetricStatus.Online);
                        await this.SubscribeInternal();
                        await this.PublishInternal();
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

                                default:
                                    throw new ArgumentOutOfRangeException(nameof(this.NameSpace));
                            }
                        }
                    });
        }

        /// <summary>
        /// Connects the Sparkplug application to the MQTT broker.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        private async Task ConnectInternal()
        {
            if (this.options is null)
            {
                throw new ArgumentNullException(nameof(this.options));
            }

            // Increment the session number.
            this.IncrementLastSessionNumber();

            // Get the will message.
            var willMessage = this.MessageGenerator.GetSparkplugStateMessage(
                this.NameSpace,
                this.options.ScadaHostIdentifier,
                false);

            // Build up the MQTT client and connect.
            this.options.CancellationToken ??= CancellationToken.None;

            var builder = new MqttClientOptionsBuilder()
                .WithClientId(this.options.ClientId)
                .WithCredentials(this.options.UserName, this.options.Password)
                .WithCleanSession(false)
                .WithProtocolVersion(MqttProtocolVersion.V311);

            if (this.options.UseTls)
            {
                builder.WithTls();
            }

            if (this.options.WebSocketParameters is null)
            {
                builder.WithTcpServer(this.options.BrokerAddress, this.options.Port);
            }
            else
            {
                builder.WithWebSocketServer(this.options.BrokerAddress, this.options.WebSocketParameters);
            }

            if (this.options.ProxyOptions != null)
            {
                builder.WithProxy(
                    this.options.ProxyOptions.Address,
                    this.options.ProxyOptions.Username,
                    this.options.ProxyOptions.Password,
                    this.options.ProxyOptions.Domain,
                    this.options.ProxyOptions.BypassOnLocal);
            }

            if (this.options.IsPrimaryApplication)
            {
                builder.WithWillMessage(willMessage);
            }

            this.ClientOptions = builder.Build();
            await this.Client.ConnectAsync(this.ClientOptions, this.options.CancellationToken.Value);
        }

        /// <summary>
        /// Publishes data to the MQTT broker.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        private async Task PublishInternal()
        {
            if (this.options is null)
            {
                throw new ArgumentNullException(nameof(this.options));
            }

            // Only send state messages for the primary application.
            if (this.options.IsPrimaryApplication)
            {
                // Get the online message and increase the sequence counter.
                var onlineMessage = this.MessageGenerator.GetSparkplugStateMessage(
                    this.NameSpace,
                    this.options.ScadaHostIdentifier,
                    true);
                this.IncrementLastSequenceNumber();

                // Publish data
                this.options.CancellationToken ??= CancellationToken.None;
                await this.Client.PublishAsync(onlineMessage, this.options.CancellationToken.Value);
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