﻿// --------------------------------------------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
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
    /// A class that handles a Sparkplug node.
    /// </summary>
    /// <seealso cref="SparkplugBase{T}"/>
    public class SparkplugNodeBase<T> : SparkplugBase<T> where T : class, new()
    {
        /// <summary>
        /// The callback for the device command received event.
        /// </summary>
        public readonly Action<T>? DeviceCommandReceived = null;

        /// <summary>
        /// The callback for the node command received event.
        /// </summary>
        public readonly Action<T>? NodeCommandReceived = null;

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
        /// Starts the Sparkplug node.
        /// </summary>
        /// <param name="nodeOptions">The node options.</param>
        /// <exception cref="ArgumentNullException">The options are null.</exception>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        public async Task Start(SparkplugNodeOptions nodeOptions)
        {
            // Storing the options.
            this.options = nodeOptions;

            if (this.options is null)
            {
                throw new ArgumentNullException(nameof(this.options));
            }

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
            await this.DisconnectInternal();
        }

        /// <summary>
        /// Publishes some metrics.
        /// </summary>
        /// <param name="metrics">The metrics.</param>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation with result of MqttClientPublishResult</returns>
        /// <exception cref="ArgumentNullException">The options are null.</exception>
        /// <exception cref="Exception">The MQTT client is not connected or an invalid metric type was specified.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The namespace is out of range.</exception>
        public async Task<MqttClientPublishResult> PublishMetrics(List<T> metrics)
        {
            if (this.options is null)
            {
                throw new ArgumentNullException(nameof(this.options));
            }

            if (!this.Client.IsConnected)
            {
                throw new Exception("The MQTT client is not connected, please try again.");
            }

            switch (this.NameSpace)
            {
                case SparkplugNamespace.VersionA:
                {
                    if (!(metrics is List<VersionAPayload.KuraMetric> convertedMetrics))
                    {
                        throw new Exception("Invalid metric type specified for version A metric.");
                    }

                    return await this.PublishVersionAMessage(convertedMetrics);
                }
                case SparkplugNamespace.VersionB:
                {
                    if (!(metrics is List<VersionBPayload.Metric> convertedMetrics))
                    {
                        throw new Exception("Invalid metric type specified for version B metric.");
                    }

                    return await this.PublishVersionBMessage(convertedMetrics);
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(this.NameSpace));
            }
        }

        /// <summary>
        /// Publishes a version A metric.
        /// </summary>
        /// <param name="metrics">The metrics.</param>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation with result of MqttClientPublishResult</returns>
        /// <exception cref="ArgumentNullException">The options are null.</exception>
        /// <exception cref="Exception">An invalid metric type was specified.</exception>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        private async Task<MqttClientPublishResult> PublishVersionAMessage(List<VersionAPayload.KuraMetric> metrics)
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

            // Get the data message and increase the sequence counter.
            var dataMessage = this.MessageGenerator.GetSparkPlugNodeDataMessage(
                this.NameSpace,
                this.options.GroupIdentifier,
                this.options.EdgeNodeIdentifier,
                metrics,
                this.LastSequenceNumber,
                this.LastSessionNumber,
                DateTimeOffset.Now,
                1);

            // Debug output.
            dataMessage.ToOutputWindowJson("NDATA Message");

            this.IncrementLastSequenceNumber();

            return await this.Client.PublishAsync(dataMessage);
        }

        /// <summary>
        /// Publishes a version B metric.
        /// </summary>
        /// <param name="metrics">The metrics.</param>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation with result of MqttClientPublishResult</returns>
        /// <exception cref="ArgumentNullException">The options are null.</exception>
        /// <exception cref="Exception">An invalid metric type was specified.</exception>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        private async Task<MqttClientPublishResult> PublishVersionBMessage(List<VersionBPayload.Metric> metrics)
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

            // Get the data message and increase the sequence counter.
            var dataMessage = this.MessageGenerator.GetSparkPlugNodeDataMessage(
                this.NameSpace,
                this.options.GroupIdentifier,
                this.options.EdgeNodeIdentifier,
                metrics,
                this.LastSequenceNumber,
                this.LastSessionNumber,
                DateTimeOffset.Now,
                1);

            // Debug output.
            dataMessage.ToOutputWindowJson("NDATA Message");

            this.IncrementLastSequenceNumber();

            return await this.Client.PublishAsync(dataMessage);
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

                        // Invoke disconnected callback.
                        this.OnDisconnected?.Invoke();

                        // Wait until the disconnect interval is reached.
                        await Task.Delay(this.options.ReconnectInterval);

                        // Connect, subscribe to incoming messages and send a state message.
                        await this.ConnectInternal();
                        await this.SubscribeInternal();
                        await this.PublishInternal();
                    });
        }

        /// <summary>
        /// Adds the message received handler to handle incoming messages.
        /// </summary>
        /// <exception cref="InvalidCastException">The metric cast is invalid.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The namespace is out of range.</exception>
        private void AddMessageReceivedHandler()
        {
            this.Client.UseApplicationMessageReceivedHandler(
                e =>
                    {
                        var topic = e.ApplicationMessage.Topic;

                        // Handle the STATE message before anything else as they're UTF-8 encoded.
                        if (topic.Contains(SparkplugMessageType.StateMessage.GetDescription()))
                        {
                            this.StatusMessageReceived?.Invoke(Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
                            return;
                        }

                        switch (this.NameSpace)
                        {
                            case SparkplugNamespace.VersionA:
                                var payloadVersionA = PayloadHelper.Deserialize<VersionAPayload>(e.ApplicationMessage.Payload);

                                if (payloadVersionA != null)
                                {
                                    if (topic.Contains(SparkplugMessageType.DeviceCommand.GetDescription()))
                                    {
                                        if (!(payloadVersionA is T convertedPayloadVersionA))
                                        {
                                            throw new InvalidCastException("The metric cast didn't work properly.");
                                        }

                                        this.DeviceCommandReceived?.Invoke(convertedPayloadVersionA);
                                    }

                                    if (topic.Contains(SparkplugMessageType.NodeCommand.GetDescription()))
                                    {
                                        if (!(payloadVersionA is T convertedPayloadVersionA))
                                        {
                                            throw new InvalidCastException("The metric cast didn't work properly.");
                                        }

                                        this.NodeCommandReceived?.Invoke(convertedPayloadVersionA);
                                    }
                                }

                                break;

                            case SparkplugNamespace.VersionB:
                                var payloadVersionB = PayloadHelper.Deserialize<VersionBPayload>(e.ApplicationMessage.Payload);

                                if (payloadVersionB != null)
                                {
                                    if (topic.Contains(SparkplugMessageType.DeviceCommand.GetDescription()))
                                    {
                                        if (!(payloadVersionB is T convertedPayloadVersionB))
                                        {
                                            throw new InvalidCastException("The metric cast didn't work properly.");
                                        }

                                        this.DeviceCommandReceived?.Invoke(convertedPayloadVersionB);
                                    }

                                    if (topic.Contains(SparkplugMessageType.NodeCommand.GetDescription()))
                                    {
                                        if (!(payloadVersionB is T convertedPayloadVersionB))
                                        {
                                            //throw new InvalidCastException("The metric cast didn't work properly.");
                                        }

                                        //this.NodeCommandReceived?.Invoke(convertedPayloadVersionB);
                                    }
                                }

                                break;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(this.NameSpace));
                        }
                    });
        }

        /// <summary>
        /// Connects the Sparkplug node to the MQTT broker.
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

            // Reset the sequence number.
            this.ResetLastSequenceNumber();

            // Get the will message.
            var willMessage = this.MessageGenerator.GetSparkPlugNodeDeathMessage(
                this.NameSpace,
                this.options.GroupIdentifier,
                this.options.EdgeNodeIdentifier,
                this.LastSequenceNumber,
                this.LastSessionNumber,
                DateTimeOffset.Now,
                1);

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

            // Debug output.
            this.ClientOptions.ToOutputWindowJson("CONNECT Message");

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

            // Get the online message and increase the sequence counter.
            var onlineMessage = this.MessageGenerator.GetSparkPlugNodeBirthMessage(
                this.NameSpace,
                this.options.GroupIdentifier,
                this.options.EdgeNodeIdentifier,
                this.KnownMetrics,
                this.LastSequenceNumber,
                this.LastSessionNumber,
                DateTimeOffset.Now,
                1);

            // Debug output.
            onlineMessage.ToOutputWindowJson("NBIRTH Message");

            // Increment
            this.IncrementLastSequenceNumber();

            // Publish data.
            this.options.CancellationToken ??= CancellationToken.None;
            await this.Client.PublishAsync(onlineMessage, this.options.CancellationToken.Value);
        }

        /// <summary>
        /// Subscribes the client to the node subscribe topics.
        /// </summary>
        /// <exception cref="ArgumentNullException">The options are null.</exception>
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
        /// Disconnects the Sparkplug device from the MQTT broker.
        /// </summary>
        /// <exception cref="ArgumentNullException">The options are null.</exception>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        private async Task DisconnectInternal()
        {
            if (this.options is null)
            {
                throw new ArgumentNullException(nameof(this.options));
            }


            // Get the will message.
            var willMessage = this.MessageGenerator.GetSparkPlugNodeDeathMessage(
                this.NameSpace,
                this.options.GroupIdentifier,
                this.options.EdgeNodeIdentifier,
                this.LastSequenceNumber,
                this.LastSessionNumber,
                DateTimeOffset.Now,
                1);

            this.options.CancellationToken ??= CancellationToken.None;
            await this.Client.PublishAsync(willMessage, this.options.CancellationToken.Value);
            await this.Client.DisconnectAsync();
        }
    }
}