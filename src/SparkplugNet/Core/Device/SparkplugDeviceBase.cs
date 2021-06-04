﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugDevice.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class that handles a Sparkplug device.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Core.Device
{
    using System;
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
    using SparkplugNet.Core.Node;
    using VersionAPayload = VersionA.Payload;
    using VersionBPayload = VersionB.Payload;

    /// <inheritdoc cref="SparkplugBase{T}"/>
    /// <summary>
    /// A class that handles a Sparkplug device.
    /// </summary>
    /// <seealso cref="SparkplugBase{T}"/>
    public class SparkplugDeviceBase<T> : SparkplugBase<T> where T : class, new()
    {
        /// <summary>
        /// The options.
        /// </summary>
        private SparkplugDeviceOptions? options;

        /// <summary>
        /// The callback for the device command received event.
        /// </summary>
        public readonly Action<T>? DeviceCommandReceived = null;

        /// <inheritdoc cref="SparkplugBase{T}"/>
        /// <summary>
        /// Initializes a new instance of the <see cref="SparkplugDeviceBase{T}"/> class.
        /// </summary>
        /// <param name="knownMetrics">The metric names.</param>
        /// <seealso cref="SparkplugBase{T}"/>
        public SparkplugDeviceBase(List<T> knownMetrics) : base(knownMetrics)
        {
        }

        /// <summary>
        /// Gets or sets the child of node.
        /// </summary>
        public SparkplugNodeBase<T> ChildOf { get; set; }

        /// <summary>
        /// Starts the Sparkplug device.
        /// </summary>
        /// <param name="deviceOptions">The device options.</param>
        /// <exception cref="ArgumentNullException">The options are null.</exception>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        public async Task Start(SparkplugDeviceOptions deviceOptions)
        {
            // Storing the options.
            this.options = deviceOptions;

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
        /// Stops the Sparkplug device.
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
        /// <param name="qosLevel">The qos level.</param>
        /// <exception cref="ArgumentNullException">The options are null.</exception>
        /// <exception cref="Exception">The MQTT client is not connected or an invalid metric type was specified.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The namespace is out of range.</exception>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        public async Task<MqttClientPublishResult> PublishMetrics(List<T> metrics, int qosLevel)
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

                    return await this.PublishVersionAMessage(convertedMetrics, qosLevel);
                }
                case SparkplugNamespace.VersionB:
                {
                    if (!(metrics is List<VersionBPayload.Metric> convertedMetrics))
                    {
                        throw new Exception("Invalid metric type specified for version B metric.");
                    }

                    return await this.PublishVersionBMessage(convertedMetrics, qosLevel);
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(this.NameSpace));
            }
        }

        /// <summary>
        /// Publishes a version A metric.
        /// </summary>
        /// <param name="metrics">The metrics.</param>
        /// <param name="qosLevel">The qos level.</param>
        /// <exception cref="ArgumentNullException">The options are null.</exception>
        /// <exception cref="Exception">An invalid metric type was specified.</exception>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        private async Task<MqttClientPublishResult> PublishVersionAMessage(List<VersionAPayload.KuraMetric> metrics, int qosLevel)
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
            var dataMessage = this.MessageGenerator.GetSparkPlugDeviceDataMessage(
                this.NameSpace,
                this.options.GroupIdentifier,
                this.options.EdgeNodeIdentifier,
                this.options.DeviceIdentifier,
                metrics,
                this.ChildOf.LastSequenceNumber,
                LastSessionNumber,
                DateTimeOffset.Now,
                qosLevel);
            this.IncrementLastSequenceNumber();

            return await this.Client.PublishAsync(dataMessage);
        }

        /// <summary>
        /// Publishes a version B metric.
        /// </summary>
        /// <param name="metrics">The metrics.</param>
        /// <param name="qosLevel">The qos level.</param>
        /// <exception cref="ArgumentNullException">The options are null.</exception>
        /// <exception cref="Exception">An invalid metric type was specified.</exception>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        private async Task<MqttClientPublishResult> PublishVersionBMessage(List<VersionBPayload.Metric> metrics, int qosLevel)
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
            var dataMessage = this.MessageGenerator.GetSparkPlugDeviceDataMessage(
                this.NameSpace,
                this.options.GroupIdentifier,
                this.options.EdgeNodeIdentifier,
                this.options.DeviceIdentifier,
                metrics,
                this.ChildOf.LastSequenceNumber,
                LastSessionNumber,
                DateTimeOffset.Now,
                qosLevel);

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

                        switch (this.NameSpace)
                        {
                            case SparkplugNamespace.VersionA:
                                if (topic.Contains(SparkplugMessageType.DeviceCommand.GetDescription()))
                                {
                                    var payloadVersionA = PayloadHelper.Deserialize<VersionAPayload>(e.ApplicationMessage.Payload);

                                    if (payloadVersionA != null)
                                    {
                                        if (!(payloadVersionA is T convertedPayloadVersionA))
                                        {
                                            throw new InvalidCastException("The metric cast didn't work properly.");
                                        }

                                        this.DeviceCommandReceived?.Invoke(convertedPayloadVersionA);
                                    }
                                }

                                break;

                            case SparkplugNamespace.VersionB:

                                if (topic.Contains(SparkplugMessageType.DeviceCommand.GetDescription()))
                                {
                                    var payloadVersionB = PayloadHelper.Deserialize<VersionBPayload>(e.ApplicationMessage.Payload);

                                    if (payloadVersionB != null)
                                    {
                                        if (!(payloadVersionB is T convertedPayloadVersionB))
                                        {
                                            throw new InvalidCastException("The metric cast didn't work properly.");
                                        }

                                        this.DeviceCommandReceived?.Invoke(convertedPayloadVersionB);
                                    }
                                }

                                break;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(this.NameSpace));
                        }
                    });
        }

        /// <summary>
        /// Connects the Sparkplug device to the MQTT broker.
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
            var willMessage = this.MessageGenerator.GetSparkPlugDeviceDeathMessage(
                this.NameSpace,
                this.options.GroupIdentifier,
                this.options.EdgeNodeIdentifier,
                this.options.DeviceIdentifier,
                this.ChildOf.LastSequenceNumber,
                this.LastSessionNumber,
                DateTimeOffset.Now, 1);

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
            var onlineMessage = this.MessageGenerator.GetSparkPlugDeviceBirthMessage(
                this.NameSpace,
                this.options.GroupIdentifier,
                this.options.EdgeNodeIdentifier,
                this.options.DeviceIdentifier,
                this.KnownMetrics,
                this.ChildOf.LastSequenceNumber,
                LastSessionNumber,
                DateTimeOffset.Now,
                1);

            // Debug output.
            onlineMessage.ToOutputWindowJson("DBIRTH Message");

            // Increment
            this.IncrementLastSequenceNumber();

            // Publish data.
            this.options.CancellationToken ??= CancellationToken.None;
            await this.Client.PublishAsync(onlineMessage, this.options.CancellationToken.Value);
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
            var willMessage = this.MessageGenerator.GetSparkPlugDeviceDeathMessage(
                this.NameSpace,
                this.options.GroupIdentifier,
                this.options.EdgeNodeIdentifier,
                this.options.DeviceIdentifier,
                this.ChildOf.LastSequenceNumber,
                this.LastSessionNumber,
                DateTimeOffset.Now, 1);

            this.options.CancellationToken ??= CancellationToken.None;
            await this.Client.PublishAsync(willMessage, this.options.CancellationToken.Value);
            await this.Client.DisconnectAsync();
        }

        /// <summary>
        /// Subscribes the client to the device subscribe topic.
        /// </summary>
        /// <exception cref="ArgumentNullException">The options are null.</exception>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        private async Task SubscribeInternal()
        {
            if (this.options is null)
            {
                throw new ArgumentNullException(nameof(this.options));
            }

            var deviceCommandSubscribeTopic = this.TopicGenerator.GetDeviceCommandSubscribeTopic(this.NameSpace, this.options.GroupIdentifier, this.options.EdgeNodeIdentifier, this.options.DeviceIdentifier);
            await this.Client.SubscribeAsync(deviceCommandSubscribeTopic, MqttQualityOfServiceLevel.AtLeastOnce);
        }

        /// <summary>
        /// Increments the last sequence number.
        /// </summary>
        internal override void IncrementLastSequenceNumber()
        {
            if (this.ChildOf.LastSequenceNumber == 255)
            {
                this.ChildOf.LastSequenceNumber = 0;
            }
            else
            {
                this.ChildOf.LastSequenceNumber++;
            }
        }
    }
}