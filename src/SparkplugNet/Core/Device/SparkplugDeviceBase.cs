// --------------------------------------------------------------------------------------------------------------------
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
    using MQTTnet.Formatter;
    using MQTTnet.Protocol;

    using SparkplugNet.Core.Enumerations;
    using SparkplugNet.Core.Extensions;

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
        /// The callback for the version A device command received event.
        /// </summary>
        public readonly Action<VersionAPayload>? VersionADeviceCommandReceived = null;

        /// <summary>
        /// The callback for the version B device command received event.
        /// </summary>
        public readonly Action<VersionBPayload>? VersionBDeviceCommandReceived = null;

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
        /// Starts the Sparkplug device.
        /// </summary>
        /// <param name="deviceOptions">The device options.</param>
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
            await this.Client.DisconnectAsync();
        }

        /// <summary>
        /// Publishes some metrics.
        /// </summary>
        /// <param name="metrics">The metrics.</param>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        public async Task PublishMetrics(List<T> metrics)
        {
            if(!this.Client.IsConnected)
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
            var dataMessage = this.MessageGenerator.GetSparkPlugDeviceDataMessage(
                this.NameSpace,
                this.options.GroupIdentifier,
                this.options.EdgeNodeIdentifier,
                this.options.DeviceIdentifier,
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
            var dataMessage = this.MessageGenerator.GetSparkPlugDeviceDataMessage(
                this.NameSpace,
                this.options.GroupIdentifier,
                this.options.EdgeNodeIdentifier,
                this.options.DeviceIdentifier,
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
                                        this.VersionADeviceCommandReceived?.Invoke(payloadVersionA);
                                    }
                                }

                                break;

                            case SparkplugNamespace.VersionB:

                                if (topic.Contains(SparkplugMessageType.DeviceCommand.GetDescription()))
                                {
                                    var payloadVersionB = PayloadHelper.Deserialize<VersionBPayload>(e.ApplicationMessage.Payload);

                                    if (payloadVersionB != null)
                                    {
                                        this.VersionBDeviceCommandReceived?.Invoke(payloadVersionB);
                                    }
                                }

                                break;
                        }
                    });
        }

        /// <summary>
        /// Connects the Sparkplug device to the MQTT broker.
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
            var willMessage = this.MessageGenerator.GetSparkPlugDeviceDeathMessage(
                this.NameSpace,
                this.options.GroupIdentifier,
                this.options.EdgeNodeIdentifier,
                this.options.DeviceIdentifier,
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
            var onlineMessage = this.MessageGenerator.GetSparkPlugDeviceBirthMessage(
                this.NameSpace,
                this.options.GroupIdentifier,
                this.options.EdgeNodeIdentifier,
                this.options.DeviceIdentifier,
                this.KnownMetrics,
                this.LastSequenceNumber,
                LastSessionNumber,
                DateTimeOffset.Now);
            this.IncrementLastSequenceNumber();

            // Publish data
            this.options.CancellationToken ??= CancellationToken.None;
            await this.Client.PublishAsync(onlineMessage, this.options.CancellationToken.Value);
        }

        /// <summary>
        /// Subscribes the client to the device subscribe topic.
        /// </summary>
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
    }
}