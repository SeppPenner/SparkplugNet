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
        /// <param name="metric">The metric.</param>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        public async Task PublishMetrics(T metric)
        {
            if(!this.Client.IsConnected)
            {
                throw new Exception("The MQTT client is not connected, please try again.");
            }

            if (this.NameSpace == SparkplugNamespace.VersionA)
            {
                if (!(metric is VersionAPayload.KuraMetric convertedMetric))
                {
                    throw new Exception("Invalid metric type specified for version A metric.");
                }
                
                await this.PublishVersionAMessage(convertedMetric);
            }

            if (this.NameSpace == SparkplugNamespace.VersionB)
            {
                if (!(metric is VersionBPayload.Metric convertedMetric))
                {
                    throw new Exception("Invalid metric type specified for version B metric.");
                }

                await this.PublishVersionBMessage(convertedMetric);
            }
        }

        /// <summary>
        /// Publishes a version A metric.
        /// </summary>
        /// <param name="metric">The metric.</param>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        private async Task PublishVersionAMessage(VersionAPayload.KuraMetric metric)
        {
            if (!(this.KnownMetrics is List<VersionAPayload.KuraMetric> knownMetrics))
            {
                throw new Exception("Invalid metric type specified for version A metric.");
            }

            if (knownMetrics.FirstOrDefault(m => m.Name == metric.Name) != default)
            {
                // Todo : Publish metrics if they're valid!
            }
        }

        /// <summary>
        /// Publishes a version B metric.
        /// </summary>
        /// <param name="metric">The metric.</param>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        private async Task PublishVersionBMessage(VersionBPayload.Metric metric)
        {
            if (!(this.KnownMetrics is List<VersionBPayload.Metric> knownMetrics))
            {
                throw new Exception("Invalid metric type specified for version B metric.");
            }

            if (knownMetrics.FirstOrDefault(m => m.Name == metric.Name) != default)
            {
                // Todo : Publish metrics if they're valid!
            }
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