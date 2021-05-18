// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugNode.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class that handles a Sparkplug node.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Core.Node
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
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
    /// A class that handles a Sparkplug node.
    /// </summary>
    /// <seealso cref="SparkplugBase{T}"/>
    public class SparkplugNodeBase<T> : SparkplugBase<T> where T : class, new()
    {
        /// <summary>
        /// The options.
        /// </summary>
        private SparkplugNodeOptions? options;

        /// <inheritdoc cref="SparkplugBase{T}"/>
        /// <summary>
        /// Initializes a new instance of the <see cref="SparkplugNodeBase{T}"/> class.
        /// </summary>
        /// <param name="knownMetrics">The metric names.</param>
        /// <seealso cref="SparkplugBase{T}"/>
        public SparkplugNodeBase(List<T> knownMetrics) : base(knownMetrics)
        {
        }

        /// <summary>
        /// The callback for the status message received event.
        /// </summary>
        public readonly Action<string>? StatusMessageReceived = null;

        /// <summary>
        /// Gets the device states.
        /// </summary>
        public ConcurrentDictionary<string, MetricState<T>> DeviceStates { get; } = new ();

        /// <summary>
        /// Starts the Sparkplug node.
        /// </summary>
        /// <param name="nodeOptions">The node options.</param>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        public async Task Start(SparkplugNodeOptions nodeOptions)
        {
            // Storing the options.
            this.options = nodeOptions;

            if (this.options is null)
            {
                throw new ArgumentNullException(nameof(this.options));
            }

            // Clear states.
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
        /// Stops the Sparkplug node.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        public async Task Stop()
        {
            await this.Client.DisconnectAsync();
        }

        /// <summary>
        /// Publishes some metrics.
        /// </summary>
        /// <param name="metrics">The metrics.</param>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        public async Task PublishMetrics(List<T> metrics)
        {
            if (this.options is null)
            {
                throw new ArgumentNullException(nameof(this.options));
            }

            if (!this.Client.IsConnected)
            {
                throw new Exception("The MQTT client is not connected, please try again.");
            }

            if (this.NameSpace == SparkplugNamespace.VersionA)
            {
                if (!(metrics is List<VersionAPayload.KuraMetric> convertedMetrics))
                {
                    throw new Exception("Invalid metric type specified for version A metric.");
                }

                await this.PublishVersionAMessage(convertedMetrics);
            }

            if (this.NameSpace == SparkplugNamespace.VersionB)
            {
                if (!(metrics is List<VersionBPayload.Metric> convertedMetrics))
                {
                    throw new Exception("Invalid metric type specified for version B metric.");
                }

                await this.PublishVersionBMessage(convertedMetrics);
            }
        }

        /// <summary>
        /// Publishes a version A metric.
        /// </summary>
        /// <param name="metrics">The metrics.</param>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        private async Task PublishVersionAMessage(List<VersionAPayload.KuraMetric> metrics)
        {
            if (this.options is null)
            {
                throw new ArgumentNullException(nameof(this.options));
            }

            if (!(this.KnownMetrics is List<VersionAPayload.KuraMetric> knownMetrics))
            {
                throw new Exception("Invalid metric type specified for version A metric.");
            }

            metrics.RemoveAll(m => knownMetrics.FirstOrDefault(m2 => m2.Name == m.Name) != default);

            // Get the data message and increase the sequence counter.
            var dataMessage = this.MessageGenerator.GetSparkPlugNodeDataMessage(
                this.NameSpace,
                this.options.GroupIdentifier,
                this.options.EdgeNodeIdentifier,
                metrics,
                this.LastSequenceNumber,
                LastSessionNumber,
                DateTimeOffset.Now);
            this.IncrementLastSequenceNumber();

            await this.Client.PublishAsync(dataMessage);
        }

        /// <summary>
        /// Publishes a version B metric.
        /// </summary>
        /// <param name="metrics">The metrics.</param>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        private async Task PublishVersionBMessage(List<VersionBPayload.Metric> metrics)
        {
            if (this.options is null)
            {
                throw new ArgumentNullException(nameof(this.options));
            }

            if (!(this.KnownMetrics is List<VersionBPayload.Metric> knownMetrics))
            {
                throw new Exception("Invalid metric type specified for version B metric.");
            }

            metrics.RemoveAll(m => knownMetrics.FirstOrDefault(m2 => m2.Name == m.Name) != default);

            // Get the data message and increase the sequence counter.
            var dataMessage = this.MessageGenerator.GetSparkPlugNodeDataMessage(
                this.NameSpace,
                this.options.GroupIdentifier,
                this.options.EdgeNodeIdentifier,
                metrics,
                this.LastSequenceNumber,
                LastSessionNumber,
                DateTimeOffset.Now);
            this.IncrementLastSequenceNumber();

            await this.Client.PublishAsync(dataMessage);
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

                        // Set all metrics to stale
                        this.UpdateMetricState(SparkplugMetricStatus.Offline);

                        // Invoke disconnected callback
                        this.OnDisconnected?.Invoke();

                        // Wait until the disconnect interval is reached
                        await Task.Delay(this.options.ReconnectInterval);

                        // Connect, subscribe to incoming messages and send a state message
                        await this.ConnectInternal();
                        await this.SubscribeInternal();
                        this.UpdateMetricState(SparkplugMetricStatus.Online);
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

                        if (topic.Contains(SparkplugMessageType.NodeCommand.GetDescription()) || topic.Contains(SparkplugMessageType.DeviceCommand.GetDescription()))
                        {
                            switch (this.NameSpace)
                            {
                                case SparkplugNamespace.VersionA:
                                    var payloadVersionA = PayloadHelper.Deserialize<VersionAPayload>(e.ApplicationMessage.Payload);

                                    if (payloadVersionA != null)
                                    {
                                        // Todo: Store metrics for device if metrics are known
                                        this.VersionAPayloadReceived?.Invoke(payloadVersionA);
                                    }

                                    break;

                                case SparkplugNamespace.VersionB:
                                    var payloadVersionB = PayloadHelper.Deserialize<VersionBPayload>(e.ApplicationMessage.Payload);

                                    if (payloadVersionB != null)
                                    {
                                        // Todo: Store metrics for device if metrics are known
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
            var willMessage = this.MessageGenerator.GetSparkPlugNodeDeathMessage(
                this.NameSpace,
                this.options.GroupIdentifier,
                this.options.EdgeNodeIdentifier,
                this.LastSessionNumber);

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

            builder.WithWillMessage(willMessage);
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

            // Get the online message and increase the sequence counter.
            var onlineMessage = this.MessageGenerator.GetSparkPlugNodeBirthMessage(
                this.NameSpace,
                this.options.GroupIdentifier,
                this.options.EdgeNodeIdentifier,
                this.KnownMetrics,
                0,
                this.LastSessionNumber,
                DateTimeOffset.Now);

            // Publish data.
            this.options.CancellationToken ??= CancellationToken.None;
            await this.Client.PublishAsync(onlineMessage, this.options.CancellationToken.Value);
        }

        /// <summary>
        /// Subscribes the client to the node subscribe topics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        private async Task SubscribeInternal()
        {
            if (this.options is null)
            {
                throw new ArgumentNullException(nameof(this.options));
            }

            var nodeCommandSubscribeTopic = this.TopicGenerator.GetNodeCommandSubscribeTopic(this.NameSpace, this.options.GroupIdentifier, this.options.EdgeNodeIdentifier);
            await this.Client.SubscribeAsync(nodeCommandSubscribeTopic, MqttQualityOfServiceLevel.AtLeastOnce);

            var deviceCommandSubscribeTopic = this.TopicGenerator.GetWildcardDeviceCommandSubscribeTopic(this.NameSpace, this.options.GroupIdentifier, this.options.EdgeNodeIdentifier);
            await this.Client.SubscribeAsync(deviceCommandSubscribeTopic, MqttQualityOfServiceLevel.AtLeastOnce);

            var stateSubscribeTopic = this.TopicGenerator.GetStateSubscribeTopic(this.options.ScadaHostIdentifier);
            await this.Client.SubscribeAsync(stateSubscribeTopic, MqttQualityOfServiceLevel.AtLeastOnce);
        }

        /// <summary>
        /// Updates the metric state.
        /// </summary>
        /// <param name="metricState">The metric state.</param>
        private void UpdateMetricState(SparkplugMetricStatus metricState)
        {
            var keys = new List<string>(this.DeviceStates.Keys.ToList());

            foreach (string key in keys)
            {
                this.DeviceStates[key].MetricStatus = metricState;
            }
        }
    }
}