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
        /// Publishes a node command.
        /// </summary>
        /// <param name="metrics">The metrics.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        public async Task PublishNodeCommand(List<T> metrics, string groupIdentifier, string edgeNodeIdentifier)
        {
            if (this.options is null)
            {
                throw new ArgumentNullException(nameof(this.options));
            }

            if (!this.Client.IsConnected)
            {
                throw new Exception("The MQTT client is not connected, please try again.");
            }

            if (!groupIdentifier.IsIdentifierValid())
            {
                throw new ArgumentException(nameof(groupIdentifier));
            }

            if (!edgeNodeIdentifier.IsIdentifierValid())
            {
                throw new ArgumentException(nameof(edgeNodeIdentifier));
            }

            if (this.NameSpace == SparkplugNamespace.VersionA)
            {
                if (!(metrics is List<VersionAPayload.KuraMetric> convertedMetrics))
                {
                    throw new Exception("Invalid metric type specified for version A metric.");
                }

                await this.PublishVersionANodeCommandMessage(convertedMetrics, groupIdentifier, edgeNodeIdentifier);
            }

            if (this.NameSpace == SparkplugNamespace.VersionB)
            {
                if (!(metrics is List<VersionBPayload.Metric> convertedMetrics))
                {
                    throw new Exception("Invalid metric type specified for version B metric.");
                }

                await this.PublishVersionBNodeCommandMessage(convertedMetrics, groupIdentifier, edgeNodeIdentifier);
            }
        }

        /// <summary>
        /// Publishes a device command.
        /// </summary>
        /// <param name="metrics">The metrics.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="deviceIdentifier">The device identifier.</param>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        public async Task PublishDeviceCommand(List<T> metrics, string groupIdentifier, string edgeNodeIdentifier, string deviceIdentifier)
        {
            if (this.options is null)
            {
                throw new ArgumentNullException(nameof(this.options));
            }

            if (!this.Client.IsConnected)
            {
                throw new Exception("The MQTT client is not connected, please try again.");
            }

            if (!groupIdentifier.IsIdentifierValid())
            {
                throw new ArgumentException(nameof(groupIdentifier));
            }

            if (!edgeNodeIdentifier.IsIdentifierValid())
            {
                throw new ArgumentException(nameof(edgeNodeIdentifier));
            }

            if (!deviceIdentifier.IsIdentifierValid())
            {
                throw new ArgumentException(nameof(deviceIdentifier));
            }

            if (this.NameSpace == SparkplugNamespace.VersionA)
            {
                if (!(metrics is List<VersionAPayload.KuraMetric> convertedMetrics))
                {
                    throw new Exception("Invalid metric type specified for version A metric.");
                }

                await this.PublishVersionADeviceCommandMessage(convertedMetrics, groupIdentifier, edgeNodeIdentifier, deviceIdentifier);
            }

            if (this.NameSpace == SparkplugNamespace.VersionB)
            {
                if (!(metrics is List<VersionBPayload.Metric> convertedMetrics))
                {
                    throw new Exception("Invalid metric type specified for version B metric.");
                }

                await this.PublishVersionBDeviceCommandMessage(convertedMetrics, groupIdentifier, edgeNodeIdentifier, deviceIdentifier);
            }
        }

        /// <summary>
        /// Publishes a version A node command message.
        /// </summary>
        /// <param name="metrics">The metrics.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        private async Task PublishVersionANodeCommandMessage(List<VersionAPayload.KuraMetric> metrics, string groupIdentifier, string edgeNodeIdentifier)
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
            var dataMessage = this.MessageGenerator.GetSparkPlugNodeCommandMessage(
                this.NameSpace,
                groupIdentifier,
                edgeNodeIdentifier,
                metrics,
                this.LastSequenceNumber,
                LastSessionNumber,
                DateTimeOffset.Now);
            this.IncrementLastSequenceNumber();

            await this.Client.PublishAsync(dataMessage);
        }

        /// <summary>
        /// Publishes a version B node command message.
        /// </summary>
        /// <param name="metrics">The metrics.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        private async Task PublishVersionBNodeCommandMessage(List<VersionBPayload.Metric> metrics, string groupIdentifier, string edgeNodeIdentifier)
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
            var dataMessage = this.MessageGenerator.GetSparkPlugNodeCommandMessage(
                this.NameSpace,
                groupIdentifier,
                edgeNodeIdentifier,
                metrics,
                this.LastSequenceNumber,
                LastSessionNumber,
                DateTimeOffset.Now);
            this.IncrementLastSequenceNumber();

            await this.Client.PublishAsync(dataMessage);
        }

        /// <summary>
        /// Publishes a version A device command message.
        /// </summary>
        /// <param name="metrics">The metrics.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="deviceIdentifier">The device identifier.</param>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        private async Task PublishVersionADeviceCommandMessage(List<VersionAPayload.KuraMetric> metrics, string groupIdentifier, string edgeNodeIdentifier, string deviceIdentifier)
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
            var dataMessage = this.MessageGenerator.GetSparkPlugDeviceCommandMessage(
                this.NameSpace,
                groupIdentifier,
                edgeNodeIdentifier,
                deviceIdentifier,
                metrics,
                this.LastSequenceNumber,
                LastSessionNumber,
                DateTimeOffset.Now);
            this.IncrementLastSequenceNumber();

            await this.Client.PublishAsync(dataMessage);
        }

        /// <summary>
        /// Publishes a version B device command message.
        /// </summary>
        /// <param name="metrics">The metrics.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="deviceIdentifier">The device identifier.</param>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        private async Task PublishVersionBDeviceCommandMessage(List<VersionBPayload.Metric> metrics, string groupIdentifier, string edgeNodeIdentifier, string deviceIdentifier)
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
            var dataMessage = this.MessageGenerator.GetSparkPlugDeviceCommandMessage(
                this.NameSpace,
                groupIdentifier,
                edgeNodeIdentifier,
                deviceIdentifier,
                metrics,
                this.LastSequenceNumber,
                LastSessionNumber,
                DateTimeOffset.Now);
            this.IncrementLastSequenceNumber();

            await this.Client.PublishAsync(dataMessage);
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

                        switch (this.NameSpace)
                        {
                            case SparkplugNamespace.VersionA:
                                var payloadVersionA = PayloadHelper.Deserialize<VersionAPayload>(e.ApplicationMessage.Payload);

                                if (payloadVersionA != null)
                                {
                                    this.HandleMessagesForVersionA(topic, payloadVersionA);
                                }

                                break;

                            case SparkplugNamespace.VersionB:
                                var payloadVersionB = PayloadHelper.Deserialize<VersionBPayload>(e.ApplicationMessage.Payload);

                                if (payloadVersionB != null)
                                {
                                    this.HandleMessagesForVersionB(topic, payloadVersionB);
                                }

                                break;

                            default:
                                throw new ArgumentOutOfRangeException(nameof(this.NameSpace));
                        }
                    });
        }

        /// <summary>
        /// Handles the received messages for payload version A.
        /// </summary>
        /// <param name="topic">The topic.</param>
        /// <param name="payload">The payload.</param>
        private void HandleMessagesForVersionA(string topic, VersionAPayload payload)
        {
            if (!(this.KnownMetrics is List<VersionAPayload.KuraMetric> knownMetrics))
            {
                throw new ArgumentNullException(nameof(knownMetrics));
            }

            // If we have any not valid metric, throw an exception.
            foreach (var metric in payload.Metrics.Where(metric => knownMetrics.FirstOrDefault(m => m.Name == metric.Name) == default))
            {
                throw new Exception($"Metric {metric.Name} is an unknown metric.");
            }

            if (topic.Contains(SparkplugMessageType.NodeBirth.GetDescription()))
            {
                var nodeId = topic.Split('/')[3];
                var metricState = new MetricState<T>
                {
                    MetricStatus = SparkplugMetricStatus.Online
                };

                foreach (var payloadMetric in payload.Metrics)
                {
                    metricState.Metrics.AddOrUpdate(
                        payloadMetric.Name,
                        payloadMetric,
                        (_, _) => payloadMetric);
                }

                this.NodeStates.AddOrUpdate(nodeId, metricState, (_, _) => metricState);
            }

            if (topic.Contains(SparkplugMessageType.NodeDeath.GetDescription()))
            {

            }

            if (topic.Contains(SparkplugMessageType.DeviceBirth.GetDescription()))
            {

            }

            if (topic.Contains(SparkplugMessageType.DeviceDeath.GetDescription()))
            {

            }

            if (topic.Contains(SparkplugMessageType.NodeData.GetDescription()))
            {

            }

            if (topic.Contains(SparkplugMessageType.DeviceData.GetDescription()))
            {

            }

            if (topic.Contains(SparkplugMessageType.NodeCommand.GetDescription()))
            {

            }

            if (topic.Contains(SparkplugMessageType.DeviceCommand.GetDescription()))
            {

            }
        }

        /// <summary>
        /// Handles the received messages for payload version B.
        /// </summary>
        /// <param name="topic">The topic.</param>
        /// <param name="payload">The payload.</param>
        private void HandleMessagesForVersionB(string topic, VersionBPayload payload)
        {
            if (!(this.KnownMetrics is List<VersionBPayload.Metric> knownMetrics))
            {
                throw new ArgumentNullException(nameof(knownMetrics));
            }

            // If we have any not valid metric, throw an exception.
            foreach (var metric in payload.Metrics.Where(metric => knownMetrics.FirstOrDefault(m => m.Name == metric.Name) == default))
            {
                throw new Exception($"Metric {metric.Name} is an unknown metric.");
            }

            if (topic.Contains(SparkplugMessageType.NodeBirth.GetDescription()))
            {

            }

            if (topic.Contains(SparkplugMessageType.NodeDeath.GetDescription()))
            {

            }

            if (topic.Contains(SparkplugMessageType.DeviceBirth.GetDescription()))
            {

            }

            if (topic.Contains(SparkplugMessageType.DeviceDeath.GetDescription()))
            {

            }

            if (topic.Contains(SparkplugMessageType.NodeData.GetDescription()))
            {

            }

            if (topic.Contains(SparkplugMessageType.DeviceData.GetDescription()))
            {

            }

            if (topic.Contains(SparkplugMessageType.NodeCommand.GetDescription()))
            {

            }

            if (topic.Contains(SparkplugMessageType.DeviceCommand.GetDescription()))
            {

            }
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