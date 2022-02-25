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
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Authentication;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using MQTTnet;
    using MQTTnet.Client;
    using MQTTnet.Client.Options;
    using MQTTnet.Client.Publishing;
    using MQTTnet.Formatter;
    using MQTTnet.Protocol;
    using SparkplugNet.Core.Enumerations;
    using SparkplugNet.Core.Extensions;
    using VersionAPayload = SparkplugNet.VersionA.Payload;
    using VersionBPayload = SparkplugNet.VersionB.Payload;

    /// <inheritdoc cref="SparkplugBase{T}" />
    /// <summary>A class that handles a Sparkplug node.</summary>
    /// <seealso cref="SparkplugBase{T}" />
    public class SparkplugNodeBase<T> : SparkplugClientBase<T>
        where T : class, new()
    {
        /// <summary>The options.</summary>
        private SparkplugNodeOptions? options;

        /// <inheritdoc cref="SparkplugBase{T}" />
        /// <summary>Initializes a new instance of the <see cref="SparkplugNodeBase{T}" /> class.</summary>
        /// <param name="knownMetrics">The metric names.</param>
        /// <seealso cref="SparkplugBase{T}" />
        public SparkplugNodeBase(List<T> knownMetrics)
            : base(knownMetrics)
        {
        }

        /// <summary>
        /// Gets or sets the node identifier.
        /// </summary>
        public string NodeId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the node unique identifier.
        /// </summary>
        public Guid NodeGuid { get; set; }

        /// <summary>Starts the Sparkplug node.</summary>
        /// <param name="nodeOptions">The node options.</param>
        /// <exception cref="ArgumentNullException">The options are null.</exception>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        public async Task<bool> Start(SparkplugNodeOptions nodeOptions)
        {
            // Storing the options.
            this.options = nodeOptions;

            if (this.options is null)
            {
                throw new ArgumentNullException(nameof(this.options));
            }

            // Add handlers.
            this.ResetLastSequenceNumber();
            this.AddConnectedHandler();
            this.AddDisconnectedHandler();
            this.AddMessageReceivedHandler();

            return await this.InitConnection();
        }

        /// <summary>Stops the Sparkplug node.</summary>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        public override async Task Stop()
        {
            if (this.Client.IsConnected)
            {
                await this.DisconnectInternal();
            }

            await base.Stop();
        }

        /// <summary>Closes the Sparkplug node.</summary>
        public override async Task Close()
        {
            await this.DisconnectInternal();
            this.Client.Dispose();
            await base.Close();
        }

        /// <summary>Publishes some metrics.</summary>
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

            switch (metrics)
            {
                case List<VersionAPayload.KuraMetric>:
                {
                    if (!(metrics is List<VersionAPayload.KuraMetric> convertedMetrics))
                    {
                        throw new Exception("Invalid metric type specified for version A metric.");
                    }

                    return await this.PublishVersionAMessage(convertedMetrics);
                }
                case List<VersionBPayload.Metric>:
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

        /// <summary>Disposes this instance.</summary>
        public override void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>Releases unmanaged and - optionally - managed resources.</summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Client.Dispose();
            }
        }

        private async Task<bool> InitConnection()
        {
            // Connect, subscribe to incoming messages and send a state message.
            try
            {
                await this.ConnectInternal();
                await this.SubscribeInternal();
                await this.PublishInternal();
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>Publishes a version A metric.</summary>
        /// <param name="metrics">The metrics.</param>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation with result of MqttClientPublishResult</returns>
        /// <exception cref="ArgumentNullException">The options are null.</exception>
        /// <exception cref="Exception">An invalid metric type was specified.</exception>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
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

            this.IncrementLastSequenceNumber();

            // Remove all not known metrics.
            metrics.RemoveAll(m => knownMetrics.FirstOrDefault(m2 => m2.Name == m.Name) == null);

            // Remove the session number metric if a user might have added it.
            metrics.RemoveAll(m => m.Name == Constants.SessionNumberMetricName);

            // Get the data message and increase the sequence counter.
            var dataMessage = this.MessageGenerator.GetSparkPlugNodeDataMessage(this.NameSpace, this.options.GroupIdentifier, this.options.EdgeNodeIdentifier,
                metrics, this.LastSequenceNumber, this.LastSessionNumber, DateTimeOffset.Now, 1, this.options.ConvertPayloadToJson);

            // Return early if client not connected or payload is empty
            //if (!this.Client.IsConnected || PayloadHelper.Deserialize<VersionAPayload>(dataMessage.Payload)?.Metrics?.Count == 0)
            //{
            //    return new MqttClientPublishResult { ReasonCode = MqttClientPublishReasonCode.UnspecifiedError, ReasonString = "No modified metrics to publish"};
            //}

            // Debug output.
            dataMessage.ToJson();

            // Publish data.
            this.options.CancellationToken ??= CancellationToken.None;
            return await this.Client.PublishAsync(dataMessage, this.options.CancellationToken.Value);
            ////this.IncrementLastSequenceNumber();
        }

        /// <summary>Publishes a version B metric.</summary>
        /// <param name="metrics">The metrics.</param>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation with result of MqttClientPublishResult</returns>
        /// <exception cref="ArgumentNullException">The options are null.</exception>
        /// <exception cref="Exception">An invalid metric type was specified.</exception>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
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

            this.IncrementLastSequenceNumber();

            // Remove all not known metrics.
            metrics.RemoveAll(m => knownMetrics.FirstOrDefault(m2 => m2.Name == m.Name) == null);

            // Remove the session number metric if a user might have added it.
            metrics.RemoveAll(m => m.Name == Constants.SessionNumberMetricName);

            // Get the data message and increase the sequence counter.
            var dataMessage = this.MessageGenerator.GetSparkPlugNodeDataMessage(this.NameSpace, this.options.GroupIdentifier, this.options.EdgeNodeIdentifier,
                metrics, this.LastSequenceNumber, this.LastSessionNumber, DateTimeOffset.Now, 1, this.options.ConvertPayloadToJson);

            // Return early if client not connected or payload is empty
            //if (!this.Client.IsConnected || PayloadHelper.Deserialize<VersionBPayload>(dataMessage.Payload)?.Metrics?.Count == 0)
            //{
            //    return new MqttClientPublishResult { ReasonCode = MqttClientPublishReasonCode.UnspecifiedError, ReasonString = "No modified metrics to publish"};
            //}

            // Debug output.
            dataMessage.ToJson();

            // Publish data.
            this.options.CancellationToken ??= CancellationToken.None;
            return await this.Client.PublishAsync(dataMessage);
            ////this.IncrementLastSequenceNumber();
        }

        private void AddConnectedHandler()
        {
            this.Client.UseConnectedHandler(_ =>
            {
                // Invoke connected callback.
                this.OnConnected?.Invoke();
            });
        }

        /// <summary>Adds the disconnected handler and the reconnect functionality to the client.</summary>
        /// <exception cref="ArgumentNullException">The options are null.</exception>
        private void AddDisconnectedHandler()
        {
            this.Client.UseDisconnectedHandler(_ =>
            {
                // Invoke disconnected callback.
                this.OnDisconnected?.Invoke();
            });
        }

        /// <summary>Adds the message received handler to handle incoming messages.</summary>
        /// <exception cref="InvalidCastException">The metric cast is invalid.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The namespace is out of range.</exception>
        private void AddMessageReceivedHandler()
        {
            this.Client.UseApplicationMessageReceivedHandler(e =>
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
                                if (!(payloadVersionA.Metrics is T convertedPayloadVersionA))
                                {
                                    throw new InvalidCastException("The metric cast didn't work properly.");
                                }

                                this.DeviceCommandReceived?.Invoke(topic, convertedPayloadVersionA);
                            }

                            if (topic.Contains(SparkplugMessageType.NodeCommand.GetDescription()))
                            {
                                if (!(payloadVersionA.Metrics is T convertedPayloadVersionA))
                                {
                                    throw new InvalidCastException("The metric cast didn't work properly.");
                                }

                                this.NodeCommandReceived?.Invoke(topic, convertedPayloadVersionA);
                            }
                        }

                        break;

                    case SparkplugNamespace.VersionB:
                        var payloadVersionB = PayloadHelper.Deserialize<VersionBPayload>(e.ApplicationMessage.Payload);

                        if (payloadVersionB != null)
                        {
                            if (topic.Contains(SparkplugMessageType.DeviceCommand.GetDescription()))
                            {
                                foreach (var metric in payloadVersionB.Metrics)
                                {
                                    if (!(metric is T convertedPayloadVersionB))
                                    {
                                        throw new InvalidCastException("The metric cast didn't work properly.");
                                    }

                                    this.DeviceCommandReceived?.Invoke(topic, convertedPayloadVersionB);
                                }
                            }

                            if (topic.Contains(SparkplugMessageType.NodeCommand.GetDescription()))
                            {
                                foreach (var metric in payloadVersionB.Metrics)
                                {
                                    if (!(metric is T convertedPayloadVersionB))
                                    {
                                        throw new InvalidCastException("The metric cast didn't work properly.");
                                    }

                                    this.NodeCommandReceived?.Invoke(topic, convertedPayloadVersionB);
                                }
                            }
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(this.NameSpace));
                }
            });
        }

        /// <summary>Connects the Sparkplug node to the MQTT broker.</summary>
        /// <exception cref="ArgumentNullException">The options are null.</exception>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        private async Task ConnectInternal()
        {
            if (this.options is null)
            {
                throw new ArgumentNullException(nameof(this.options));
            }

            // return early if MQTTNet Client already connected
            if (this.Client.IsConnected)
            {
                await this.Stop();
            }

            // Increment the session number.
            this.IncrementLastSessionNumber();
            this.LogAction?.Invoke($"Incremented Last Session Number to {this.LastSessionNumber}");

            // Reset the sequence number.
            this.ResetLastSequenceNumber();
            this.LogAction?.Invoke($"Incremented Last Sequence Number to {this.LastSequenceNumber}");

            // Get the will message.
            var willMessage = this.MessageGenerator.GetSparkPlugNodeDeathMessage(this.NameSpace, this.options.GroupIdentifier, this.options.EdgeNodeIdentifier,
                this.LastSequenceNumber, this.LastSessionNumber, DateTimeOffset.Now, 1, this.options.ConvertPayloadToJson);

            // Build up the MQTT client and connect.
            var builder = new MqttClientOptionsBuilder().WithClientId(this.options.ClientId).WithCredentials(this.options.UserName, this.options.Password)
                .WithCleanSession().WithProtocolVersion(MqttProtocolVersion.V311);

            if (this.options.UseTls)
            {
                var tlsOptions = new MqttClientOptionsBuilderTlsParameters
                {
                    UseTls = true,
                    SslProtocol = SslProtocols.Tls12,
                    AllowUntrustedCertificates = true,
                    IgnoreCertificateChainErrors = true,
                    IgnoreCertificateRevocationErrors = true,
                };
                builder.WithTls(tlsOptions);
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
                builder.WithProxy(this.options.ProxyOptions.Address, this.options.ProxyOptions.Username, this.options.ProxyOptions.Password,
                    this.options.ProxyOptions.Domain, this.options.ProxyOptions.BypassOnLocal);
            }

            builder.WithWillMessage(willMessage);
            this.ClientOptions = builder.Build();

            // Debug output.
            var json = this.ClientOptions.ToJson();
            this.LogAction?.Invoke($"Node ConnectAsync attempt\n{json}");
            try
            {
                this.options.CancellationToken ??= CancellationToken.None;
                await this.Client.ConnectAsync(this.ClientOptions, this.options.CancellationToken.Value);
            }
            catch (Exception e)
            {
                this.OnException?.Invoke(e);
            }
        }

        /// <summary>Publishes data to the MQTT broker.</summary>
        /// <exception cref="ArgumentNullException">The options are null.</exception>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        private async Task PublishInternal()
        {
            if (this.options is null)
            {
                throw new ArgumentNullException(nameof(this.options));
            }

            // Get the online message and increase the sequence counter.
            var onlineMessage = this.MessageGenerator.GetSparkPlugNodeBirthMessage(this.NameSpace, this.options.GroupIdentifier,
                this.options.EdgeNodeIdentifier, this.KnownMetrics, this.LastSequenceNumber, this.LastSessionNumber, DateTimeOffset.Now, 1, this.options.ConvertPayloadToJson);

            // Debug output.
            var json = onlineMessage.ToJson();
            this.LogAction?.Invoke($"Attempting Node ConnectAsync (NBIRTH)\n{json}");

            // Publish data.
            try
            {
                this.options.CancellationToken ??= CancellationToken.None;
                await this.Client.PublishAsync(onlineMessage, this.options.CancellationToken.Value);
            }
            catch (Exception e)
            {
                this.OnException?.Invoke(e);
            }
        }

        /// <summary>Subscribes the client to the node subscribe topics.</summary>
        /// <exception cref="ArgumentNullException">The options are null.</exception>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        private async Task SubscribeInternal()
        {
            if (this.options is null)
            {
                throw new ArgumentNullException(nameof(this.options));
            }

            try
            {
                var nodeCommandSubscribeTopic =
                    this.TopicGenerator.GetNodeCommandSubscribeTopic(this.NameSpace, this.options.GroupIdentifier, this.options.EdgeNodeIdentifier);
                this.LogAction?.Invoke($"Subscribing to Node Commands\n{nodeCommandSubscribeTopic}");
                await this.Client.SubscribeAsync(nodeCommandSubscribeTopic, MqttQualityOfServiceLevel.AtLeastOnce);

                var deviceCommandSubscribeTopic =
                    this.TopicGenerator.GetWildcardDeviceCommandSubscribeTopic(this.NameSpace, this.options.GroupIdentifier, this.options.EdgeNodeIdentifier);
                this.LogAction?.Invoke($"Subscribing to Device Commands\n{deviceCommandSubscribeTopic}");
                await this.Client.SubscribeAsync(deviceCommandSubscribeTopic, MqttQualityOfServiceLevel.AtLeastOnce);

                var stateSubscribeTopic = this.TopicGenerator.GetStateSubscribeTopic(this.options.ScadaHostIdentifier);
                this.LogAction?.Invoke($"Subscribing to State Status\n{stateSubscribeTopic}");
                await this.Client.SubscribeAsync(stateSubscribeTopic, MqttQualityOfServiceLevel.AtLeastOnce);
            }
            catch (Exception e)
            {
                this.OnException?.Invoke(e);
            }
        }

        /// <summary>Disconnects the Sparkplug device from the MQTT broker.</summary>
        /// <exception cref="ArgumentNullException">The options are null.</exception>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        private async Task DisconnectInternal()
        {
            if (this.options is null)
            {
                throw new ArgumentNullException(nameof(this.options));
            }

            if (!this.Client.IsConnected)
            {
                return;
            }

            this.IncrementLastSequenceNumber();

            // Get the will message.
            var willMessage = this.MessageGenerator.GetSparkPlugNodeDeathMessage(this.NameSpace, this.options.GroupIdentifier, this.options.EdgeNodeIdentifier,
                this.LastSequenceNumber, this.LastSessionNumber, DateTimeOffset.Now, 1, this.options.ConvertPayloadToJson);

            // Debug output.
            var json = willMessage.ToJson();
            this.LogAction?.Invoke($"Attempting Node DisconnectAsync (NDEATH)\n{json}");

            try
            {
                this.options.CancellationToken ??= CancellationToken.None;
                await this.Client.PublishAsync(willMessage, this.options.CancellationToken.Value);
                await this.Client.DisconnectAsync();
            }
            catch (Exception e)
            {
                this.OnException?.Invoke(e);
            }
        }
    }
}