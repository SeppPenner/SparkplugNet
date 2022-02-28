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
    using MQTTnet.Client.Publishing;
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
    public class SparkplugApplicationBase<T> : SparkplugClientBase<T> where T : class, new()
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
        /// <exception cref="ArgumentNullException">The options are null.</exception>
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
        public override async Task Stop()
        {
            await this.DisconnectInternal();
            await base.Stop();
        }

        /// <summary>
        /// Publishes a node command.
        /// </summary>
        /// <param name="metrics">The metrics.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="qosLevel">The qos level.</param>
        /// <exception cref="ArgumentNullException">The options are null.</exception>
        /// <exception cref="Exception">The MQTT client is not connected or an invalid metric type was specified.</exception>
        /// <exception cref="ArgumentException">The group or edge node identifier is invalid.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The namespace is out of range.</exception>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        public async Task PublishNodeCommand(List<T> metrics, string groupIdentifier, string edgeNodeIdentifier, int qosLevel)
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

            switch (this.NameSpace)
            {
                case SparkplugNamespace.VersionA:
                {
                    if (!(metrics is List<VersionAPayload.KuraMetric> convertedMetrics))
                    {
                        throw new Exception("Invalid metric type specified for version A metric.");
                    }

                    await this.PublishVersionANodeCommandMessage(convertedMetrics, groupIdentifier, edgeNodeIdentifier, qosLevel);
                    break;
                }
                case SparkplugNamespace.VersionB:
                {
                    if (!(metrics is List<VersionBPayload.Metric> convertedMetrics))
                    {
                        throw new Exception("Invalid metric type specified for version B metric.");
                    }

                    await this.PublishVersionBNodeCommandMessage(convertedMetrics, groupIdentifier, edgeNodeIdentifier, qosLevel);
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(this.NameSpace));
            }
        }

        /// <summary>
        /// Publishes a device command.
        /// </summary>
        /// <param name="metrics">The metrics.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="deviceIdentifier">The device identifier.</param>
        /// <param name="qosLevel">The qos level.</param>
        /// <exception cref="ArgumentNullException">The options are null.</exception>
        /// <exception cref="Exception">The MQTT client is not connected or an invalid metric type was specified.</exception>
        /// <exception cref="ArgumentException">The group or edge node or device identifier is invalid.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The namespace is out of range.</exception>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        public async Task PublishDeviceCommand(List<T> metrics, string groupIdentifier, string edgeNodeIdentifier, string deviceIdentifier, int qosLevel)
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

            switch (this.NameSpace)
            {
                case SparkplugNamespace.VersionA:
                {
                    if (!(metrics is List<VersionAPayload.KuraMetric> convertedMetrics))
                    {
                        throw new Exception("Invalid metric type specified for version A metric.");
                    }

                    await this.PublishVersionADeviceCommandMessage(convertedMetrics, groupIdentifier, edgeNodeIdentifier, deviceIdentifier, qosLevel);
                    break;
                }
                case SparkplugNamespace.VersionB:
                {
                    if (!(metrics is List<VersionBPayload.Metric> convertedMetrics))
                    {
                        throw new Exception("Invalid metric type specified for version B metric.");
                    }

                    await this.PublishVersionBDeviceCommandMessage(convertedMetrics, groupIdentifier, edgeNodeIdentifier, deviceIdentifier, qosLevel);
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(this.NameSpace));
            }
        }

        /// <summary>
        /// Adds the disconnected handler and the reconnect functionality to the client.
        /// </summary>
        /// <exception cref="ArgumentNullException">The options are null.</exception>
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
        /// Publishes a version A node command message.
        /// </summary>
        /// <param name="metrics">The metrics.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="qosLevel">The qos level.</param>
        /// <exception cref="ArgumentNullException">The options are null.</exception>
        /// <exception cref="Exception">An invalid metric type was specified.</exception>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        private async Task<MqttClientPublishResult> PublishVersionANodeCommandMessage(List<VersionAPayload.KuraMetric> metrics, string groupIdentifier, string edgeNodeIdentifier, int qosLevel)
        {
            if (this.options is null)
            {
                throw new ArgumentNullException(nameof(this.options));
            }

            if (!(this.KnownMetrics is List<VersionAPayload.KuraMetric> knownMetrics))
            {
                throw new Exception("Invalid metric type specified for version A metric.");
            }

            // Remove all not known metrics.
            metrics.RemoveAll(m => knownMetrics.FirstOrDefault(m2 => m2.Name == m.Name) == null);

            // Remove the session number metric if a user might have added it.
            metrics.RemoveAll(m => m.Name == Constants.SessionNumberMetricName);

            this.IncrementLastSequenceNumber();

            // Get the data message and increase the sequence counter.
            var dataMessage = this.MessageGenerator.GetSparkPlugNodeCommandMessage(
                this.NameSpace,
                groupIdentifier,
                edgeNodeIdentifier,
                metrics,
                this.LastSequenceNumber,
                LastSessionNumber,
                DateTimeOffset.Now,
                qosLevel);

            var result =  await this.Client.PublishAsync(dataMessage);
            switch (result.ReasonCode)
            {
                case MqttClientPublishReasonCode.Success:
                case MqttClientPublishReasonCode.NoMatchingSubscribers:
                    ////this.IncrementLastSequenceNumber();
                    break;
            }

            return result;
        }

        /// <summary>
        /// Publishes a version B node command message.
        /// </summary>
        /// <param name="metrics">The metrics.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="qosLevel">The qos level.</param>
        /// <exception cref="ArgumentNullException">The options are null.</exception>
        /// <exception cref="Exception">An invalid metric type was specified.</exception>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        private async Task<MqttClientPublishResult> PublishVersionBNodeCommandMessage(List<VersionBPayload.Metric> metrics, string groupIdentifier, string edgeNodeIdentifier, int qosLevel)
        {
            if (this.options is null)
            {
                throw new ArgumentNullException(nameof(this.options));
            }

            if (!(this.KnownMetrics is List<VersionBPayload.Metric> knownMetrics))
            {
                throw new Exception("Invalid metric type specified for version B metric.");
            }

            // Remove all not known metrics.
            metrics.RemoveAll(m => knownMetrics.FirstOrDefault(m2 => m2.Name == m.Name) == null);

            // Remove the session number metric if a user might have added it.
            metrics.RemoveAll(m => m.Name == Constants.SessionNumberMetricName);

            this.IncrementLastSequenceNumber();

            // Get the data message and increase the sequence counter.
            var dataMessage = this.MessageGenerator.GetSparkPlugNodeCommandMessage(
                this.NameSpace,
                groupIdentifier,
                edgeNodeIdentifier,
                metrics,
                this.LastSequenceNumber,
                LastSessionNumber,
                DateTimeOffset.Now,
                qosLevel);

            // Debug output.
            dataMessage.ToJson();

            var result =  await this.Client.PublishAsync(dataMessage);
            switch (result.ReasonCode)
            {
                case MqttClientPublishReasonCode.Success:
                case MqttClientPublishReasonCode.NoMatchingSubscribers:
                    ////this.IncrementLastSequenceNumber();
                    break;
            }

            return result;
        }

        /// <summary>
        /// Publishes a version A device command message.
        /// </summary>
        /// <param name="metrics">The metrics.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="deviceIdentifier">The device identifier.</param>
        /// <param name="qosLevel">The qos level.</param>
        /// <exception cref="ArgumentNullException">The options are null.</exception>
        /// <exception cref="Exception">An invalid metric type was specified.</exception>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        private async Task<MqttClientPublishResult> PublishVersionADeviceCommandMessage(List<VersionAPayload.KuraMetric> metrics, string groupIdentifier, string edgeNodeIdentifier, string deviceIdentifier, int qosLevel)
        {
            if (this.options is null)
            {
                throw new ArgumentNullException(nameof(this.options));
            }

            if (!(this.KnownMetrics is List<VersionAPayload.KuraMetric> knownMetrics))
            {
                throw new Exception("Invalid metric type specified for version A metric.");
            }

            // Remove all not known metrics.
            metrics.RemoveAll(m => knownMetrics.FirstOrDefault(m2 => m2.Name == m.Name) == null);

            // Remove the session number metric if a user might have added it.
            metrics.RemoveAll(m => m.Name == Constants.SessionNumberMetricName);

            this.IncrementLastSequenceNumber();

            // Get the data message and increase the sequence counter.
            var dataMessage = this.MessageGenerator.GetSparkPlugDeviceCommandMessage(
                this.NameSpace,
                groupIdentifier,
                edgeNodeIdentifier,
                deviceIdentifier,
                metrics,
                this.LastSequenceNumber,
                LastSessionNumber,
                DateTimeOffset.Now,
                qosLevel);

            // Debug output.
            dataMessage.ToJson();

            var result =  await this.Client.PublishAsync(dataMessage);
            switch (result.ReasonCode)
            {
                case MqttClientPublishReasonCode.Success:
                case MqttClientPublishReasonCode.NoMatchingSubscribers:
                    ////this.IncrementLastSequenceNumber();
                    break;
            }

            return result;
        }

        /// <summary>
        /// Publishes a version B device command message.
        /// </summary>
        /// <param name="metrics">The metrics.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="deviceIdentifier">The device identifier.</param>
        /// <param name="qosLevel">The qos level.</param>
        /// <exception cref="ArgumentNullException">The options are null.</exception>
        /// <exception cref="Exception">An invalid metric type was specified.</exception>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        private async Task<MqttClientPublishResult> PublishVersionBDeviceCommandMessage(List<VersionBPayload.Metric> metrics, string groupIdentifier, string edgeNodeIdentifier, string deviceIdentifier, int qosLevel)
        {
            if (this.options is null)
            {
                throw new ArgumentNullException(nameof(this.options));
            }

            if (!(this.KnownMetrics is List<VersionBPayload.Metric> knownMetrics))
            {
                throw new Exception("Invalid metric type specified for version B metric.");
            }

            // Remove all not known metrics.
            metrics.RemoveAll(m => knownMetrics.FirstOrDefault(m2 => m2.Name == m.Name) == null);

            // Remove the session number metric if a user might have added it.
            metrics.RemoveAll(m => m.Name == Constants.SessionNumberMetricName);

            this.IncrementLastSequenceNumber();

            // Get the data message and increase the sequence counter.
            var dataMessage = this.MessageGenerator.GetSparkPlugDeviceCommandMessage(
                this.NameSpace,
                groupIdentifier,
                edgeNodeIdentifier,
                deviceIdentifier,
                metrics,
                this.LastSequenceNumber,
                LastSessionNumber,
                DateTimeOffset.Now,
                qosLevel);

            var result =  await this.Client.PublishAsync(dataMessage);
            switch (result.ReasonCode)
            {
                case MqttClientPublishReasonCode.Success:
                case MqttClientPublishReasonCode.NoMatchingSubscribers:
                    ////this.IncrementLastSequenceNumber();
                    break;
            }

            return result;
        }

        /// <summary>
        /// Adds the message received handler to handle incoming messages.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">The namespace is out of range.</exception>
        private void AddMessageReceivedHandler()
        {
            this.Client.UseApplicationMessageReceivedHandler(
                e =>
                    {
                        var topic = e.ApplicationMessage.Topic;

                        // Skip the STATE messages as they're UTF-8 encoded.
                        if (topic.Contains(SparkplugMessageType.StateMessage.GetDescription()))
                        {
                            return;
                        }

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
        /// <exception cref="ArgumentNullException">The known metrics are null.</exception>
        /// <exception cref="Exception">The metric is unknown.</exception>
        private void HandleMessagesForVersionA(string topic, VersionAPayload payload)
        {
            if (!(this.KnownMetrics is List<VersionAPayload.KuraMetric> knownMetrics))
            {
                throw new ArgumentNullException(nameof(knownMetrics));
            }

            // If we have any not valid metric, throw an exception.
            var metricsWithoutSequenceMetric = payload.Metrics.Where(m => m.Name != Constants.SessionNumberMetricName);

            foreach (var metric in metricsWithoutSequenceMetric.Where(metric => knownMetrics.FirstOrDefault(m => m.Name == metric.Name) == default))
            {
                throw new Exception($"Metric {metric.Name} is an unknown metric.");
            }

            if (topic.Contains(SparkplugMessageType.NodeBirth.GetDescription()))
            {
                this.HandleNodeMessage(topic, payload, SparkplugMetricStatus.Online);
            }

            if (topic.Contains(SparkplugMessageType.NodeDeath.GetDescription()))
            {
                this.HandleNodeMessage(topic, payload, SparkplugMetricStatus.Offline);
            }

            if (topic.Contains(SparkplugMessageType.DeviceBirth.GetDescription()))
            {
                this.HandleDeviceMessage(topic, payload, SparkplugMetricStatus.Online);
            }

            if (topic.Contains(SparkplugMessageType.DeviceDeath.GetDescription()))
            {
                this.HandleDeviceMessage(topic, payload, SparkplugMetricStatus.Offline);
            }

            if (topic.Contains(SparkplugMessageType.NodeData.GetDescription()))
            {
                this.HandleNodeMessage(topic, payload, SparkplugMetricStatus.Online);
            }

            if (topic.Contains(SparkplugMessageType.DeviceData.GetDescription()))
            {
                this.HandleDeviceMessage(topic, payload, SparkplugMetricStatus.Online);
            }
        }

        /// <summary>
        /// Handles the received messages for payload version B.
        /// </summary>
        /// <param name="topic">The topic.</param>
        /// <param name="payload">The payload.</param>
        /// <exception cref="ArgumentNullException">The known metrics are null.</exception>
        /// <exception cref="Exception">The metric is unknown.</exception>
        private void HandleMessagesForVersionB(string topic, VersionBPayload payload)
        {
            if (!(this.KnownMetrics is List<VersionBPayload.Metric> knownMetrics))
            {
                throw new ArgumentNullException(nameof(knownMetrics));
            }

            // If we have any not valid metric, throw an exception.
            var metricsWithoutSequenceMetric = payload.Metrics.Where(m => m.Name != Constants.SessionNumberMetricName);

            foreach (var metric in metricsWithoutSequenceMetric.Where(metric => knownMetrics.FirstOrDefault(m => m.Name == metric.Name) == default))
            {
                throw new Exception($"Metric {metric.Name} is an unknown metric.");
            }

            if (topic.Contains(SparkplugMessageType.NodeBirth.GetDescription()))
            {
                this.HandleNodeMessage(topic, payload, SparkplugMetricStatus.Online);
            }

            if (topic.Contains(SparkplugMessageType.NodeDeath.GetDescription()))
            {
                this.HandleNodeMessage(topic, payload, SparkplugMetricStatus.Offline);
            }

            if (topic.Contains(SparkplugMessageType.DeviceBirth.GetDescription()))
            {
                this.HandleDeviceMessage(topic, payload, SparkplugMetricStatus.Online);
            }

            if (topic.Contains(SparkplugMessageType.DeviceDeath.GetDescription()))
            {
                this.HandleDeviceMessage(topic, payload, SparkplugMetricStatus.Offline);
            }

            if (topic.Contains(SparkplugMessageType.NodeData.GetDescription()))
            {
                this.HandleNodeMessage(topic, payload, SparkplugMetricStatus.Online);
            }

            if (topic.Contains(SparkplugMessageType.DeviceData.GetDescription()))
            {
                this.HandleDeviceMessage(topic, payload, SparkplugMetricStatus.Online);
            }
        }

        /// <summary>
        /// Handles the device message.
        /// </summary>
        /// <param name="topic">The topic.</param>
        /// <param name="payload">The payload.</param>
        /// <param name="metricStatus">The metric status.</param>
        /// <exception cref="InvalidCastException">The metric cast is invalid.</exception>
        private void HandleDeviceMessage(string topic, VersionBPayload payload, SparkplugMetricStatus metricStatus)
        {
            var deviceId = topic.Split('/')[4];
            var metricState = new MetricState<T>
            {
                MetricStatus = metricStatus
            };

            foreach (var payloadMetric in payload.Metrics)
            {
                if (!(payloadMetric is T convertedMetric))
                {
                    throw new InvalidCastException("The metric cast didn't work properly.");
                }

                metricState.Metrics[payloadMetric.Name] = convertedMetric;
            }

            this.DeviceStates[deviceId] = metricState;
        }

        /// <summary>
        /// Handles the device message.
        /// </summary>
        /// <param name="topic">The topic.</param>
        /// <param name="payload">The payload.</param>
        /// <param name="metricStatus">The metric status.</param>
        /// <exception cref="InvalidCastException">The metric cast is invalid.</exception>
        private void HandleDeviceMessage(string topic, VersionAPayload payload, SparkplugMetricStatus metricStatus)
        {
            var deviceId = topic.Split('/')[4];
            var metricState = new MetricState<T>
            {
                MetricStatus = metricStatus
            };

            foreach (var payloadMetric in payload.Metrics)
            {
                if (!(payloadMetric is T convertedMetric))
                {
                    throw new InvalidCastException("The metric cast didn't work properly.");
                }

                metricState.Metrics[payloadMetric.Name] = convertedMetric;
            }

            this.DeviceStates[deviceId] = metricState;
        }

        /// <summary>
        /// Handles the node message.
        /// </summary>
        /// <param name="topic">The topic.</param>
        /// <param name="payload">The payload.</param>
        /// <param name="metricStatus">The metric status.</param>
        /// <exception cref="InvalidCastException">The metric cast is invalid.</exception>
        private void HandleNodeMessage(string topic, VersionBPayload payload, SparkplugMetricStatus metricStatus)
        {
            var nodeId = topic.Split('/')[3];
            var metricState = new MetricState<T>
            {
                MetricStatus = metricStatus
            };

            foreach (var payloadMetric in payload.Metrics)
            {
                if (!(payloadMetric is T convertedMetric))
                {
                    throw new InvalidCastException("The metric cast didn't work properly.");
                }

                metricState.Metrics[payloadMetric.Name] = convertedMetric;
            }

            this.NodeStates[nodeId] = metricState;
        }

        /// <summary>
        /// Handles the node message.
        /// </summary>
        /// <param name="topic">The topic.</param>
        /// <param name="payload">The payload.</param>
        /// <param name="metricStatus">The metric status.</param>
        /// <exception cref="InvalidCastException">The metric cast is invalid.</exception>
        private void HandleNodeMessage(string topic, VersionAPayload payload, SparkplugMetricStatus metricStatus)
        {
            var nodeId = topic.Split('/')[3];
            var metricState = new MetricState<T>
            {
                MetricStatus = metricStatus
            };

            foreach (var payloadMetric in payload.Metrics)
            {
                if (!(payloadMetric is T convertedMetric))
                {
                    throw new InvalidCastException("The metric cast didn't work properly.");
                }

                metricState.Metrics[payloadMetric.Name] = convertedMetric;
            }

            this.NodeStates[nodeId] = metricState;
        }

        /// <summary>
        /// Connects the Sparkplug application to the MQTT broker.
        /// </summary>
        /// <exception cref="ArgumentNullException">The options are null.</exception>
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
        /// <exception cref="ArgumentNullException">The options are null.</exception>
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
                this.IncrementLastSequenceNumber();

                var onlineMessage = this.MessageGenerator.GetSparkplugStateMessage(
                    this.NameSpace,
                    this.options.ScadaHostIdentifier,
                    true);

                // Publish data.
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
        /// Disconnects from the MQTT broker.
        /// </summary>
        /// <exception cref="ArgumentNullException">The options are null.</exception>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        private async Task DisconnectInternal()
        {
            if (this.options is null)
            {
                throw new ArgumentNullException(nameof(this.options));
            }

            // Only send state messages for the primary application.
            if (this.options.IsPrimaryApplication)
            {
                this.IncrementLastSequenceNumber();

                // Get the online message and increase the sequence counter.
                var offlineMessage = this.MessageGenerator.GetSparkplugStateMessage(
                    this.NameSpace,
                    this.options.ScadaHostIdentifier,
                    false);

                // Publish STATE offline.
                this.options.CancellationToken ??= CancellationToken.None;
                var result = await this.Client.PublishAsync(offlineMessage, this.options.CancellationToken.Value);

                switch (result.ReasonCode)
                {
                    case MqttClientPublishReasonCode.Success:
                    case MqttClientPublishReasonCode.NoMatchingSubscribers:
                        ////this.IncrementLastSequenceNumber();
                        break;
                }

                // Disconnect client.
                await this.Client.DisconnectAsync();
            }
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