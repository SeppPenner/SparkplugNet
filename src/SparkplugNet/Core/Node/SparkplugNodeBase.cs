// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugNodeBase.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class that handles a Sparkplug node.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Core.Node;

/// <inheritdoc cref="SparkplugBase{T}"/>
/// <summary>
/// A class that handles a Sparkplug node.
/// </summary>
/// <seealso cref="SparkplugBase{T}"/>
public partial class SparkplugNodeBase<T> : SparkplugBase<T> where T : class, new()
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
    /// <param name="logger">The logger.</param>
    /// <seealso cref="SparkplugBase{T}"/>
    public SparkplugNodeBase(List<T> knownMetrics, ILogger? logger = null) : base(knownMetrics, logger)
    {
    }

    /// <summary>
    /// Gets or sets the callback for the device command received event.
    /// </summary>
    public Action<T>? DeviceCommandReceived { get; set; } = null;

    /// <summary>
    /// Gets or sets the callback for the node command received event.
    /// </summary>
    public Action<T>? NodeCommandReceived { get; set; } = null;

    /// <summary>
    /// Gets or sets the callback for the status message received event.
    /// </summary>
    public Action<string>? StatusMessageReceived { get; set; } = null;

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
            throw new ArgumentNullException(nameof(this.options), "The options aren't set properly.");
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
        await this.Client.DisconnectAsync();
    }

    /// <summary>
    /// Publishes some metrics.
    /// </summary>
    /// <param name="metrics">The metrics.</param>
    /// <exception cref="ArgumentNullException">The options are null.</exception>
    /// <exception cref="Exception">The MQTT client is not connected or an invalid metric type was specified.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The namespace is out of range.</exception>
    /// <returns>A <see cref="MqttClientPublishResult"/>.</returns>
    public async Task<MqttClientPublishResult> PublishMetrics(List<T> metrics)
    {
        if (this.options is null)
        {
            throw new ArgumentNullException(nameof(this.options), "The options aren't set properly.");
        }

        if (!this.Client.IsConnected)
        {
            throw new Exception("The MQTT client is not connected, please try again.");
        }

        switch (this.NameSpace)
        {
            case SparkplugNamespace.VersionA:
            {
                if (metrics is not List<VersionAData.KuraMetric> convertedMetrics)
                {
                    throw new Exception("Invalid metric type specified for version A metric.");
                }

                return await this.PublishVersionAMessage(convertedMetrics);
            }
            case SparkplugNamespace.VersionB:
            {
                if (metrics is not List<VersionBData.Metric> convertedMetrics)
                {
                    throw new Exception("Invalid metric type specified for version B metric.");
                }

                return await this.PublishVersionBMessage(convertedMetrics);
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(this.NameSpace), "The namespace is invalid.");
        }
    }

    /// <summary>
    /// Publishes version A metrics for a node.
    /// </summary>
    /// <param name="metrics">The metrics.</param>
    /// <exception cref="ArgumentNullException">The options are null.</exception>
    /// <exception cref="Exception">An invalid metric type was specified.</exception>
    /// <returns>A <see cref="MqttClientPublishResult"/>.</returns>
    private async Task<MqttClientPublishResult> PublishVersionAMessage(List<VersionAData.KuraMetric> metrics)
    {
        if (this.options is null)
        {
            throw new ArgumentNullException(nameof(this.options), "The options aren't set properly.");
        }

        if (this.KnownMetrics is not List<VersionAData.KuraMetric> knownMetrics)
        {
            throw new Exception("Invalid metric type specified for version A metric.");
        }

        // Remove all not known metrics.
        metrics.RemoveAll(m => knownMetrics.FirstOrDefault(m2 => m2.Name == m.Name) == default);

        // Remove the session number metric if a user might have added it.
        metrics.RemoveAll(m => m.Name == Constants.SessionNumberMetricName);

        // Get the data message.
        var dataMessage = this.MessageGenerator.GetSparkPlugNodeDataMessage(
            this.NameSpace,
            this.options.GroupIdentifier,
            this.options.EdgeNodeIdentifier,
            metrics,
            this.LastSequenceNumber,
            this.LastSessionNumber,
            DateTimeOffset.Now);

        // Debug output.
        this.Logger?.Debug("NDATA Message: {@DataMessage}", dataMessage);

        // Increment the sequence number.
        this.IncrementLastSequenceNumber();

        // Publish the message.
        return await this.Client.PublishAsync(dataMessage);
    }

    /// <summary>
    /// Publishes version B metrics for a node.
    /// </summary>
    /// <param name="metrics">The metrics.</param>
    /// <exception cref="ArgumentNullException">The options are null.</exception>
    /// <exception cref="Exception">An invalid metric type was specified.</exception>
    /// <returns>A <see cref="MqttClientPublishResult"/>.</returns>
    private async Task<MqttClientPublishResult> PublishVersionBMessage(List<VersionBData.Metric> metrics)
    {
        if (this.options is null)
        {
            throw new ArgumentNullException(nameof(this.options), "The options aren't set properly.");
        }

        if (this.KnownMetrics is not List<VersionBData.Metric> knownMetrics)
        {
            throw new Exception("Invalid metric type specified for version B metric.");
        }

        // Remove all not known metrics.
        metrics.RemoveAll(m => knownMetrics.FirstOrDefault(m2 => m2.Name == m.Name) == default);

        // Remove the session number metric if a user might have added it.
        metrics.RemoveAll(m => m.Name == Constants.SessionNumberMetricName);

        // Get the data message.
        var dataMessage = this.MessageGenerator.GetSparkPlugNodeDataMessage(
            this.NameSpace,
            this.options.GroupIdentifier,
            this.options.EdgeNodeIdentifier,
            metrics,
            this.LastSequenceNumber,
            this.LastSessionNumber,
            DateTimeOffset.Now);

        // Debug output.
        this.Logger?.Debug("NDATA Message: {@DataMessage}", dataMessage);

        // Increment the sequence number.
        this.IncrementLastSequenceNumber();

        // Publish the message.
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
                            var payloadVersionA = PayloadHelper.Deserialize<VersionAProtoBuf.ProtoBufPayload>(e.ApplicationMessage.Payload);

                            if (payloadVersionA is not null)
                            {
                                var convertedPayload = PayloadConverter.ConvertVersionAPayload(payloadVersionA);

                                if (topic.Contains(SparkplugMessageType.DeviceCommand.GetDescription()))
                                {
                                    if (convertedPayload is not T convertedPayloadVersionA)
                                    {
                                        throw new InvalidCastException("The metric cast didn't work properly.");
                                    }

                                    this.DeviceCommandReceived?.Invoke(convertedPayloadVersionA);
                                }

                                if (topic.Contains(SparkplugMessageType.NodeCommand.GetDescription()))
                                {
                                    if (convertedPayload is not T convertedPayloadVersionA)
                                    {
                                        throw new InvalidCastException("The metric cast didn't work properly.");
                                    }

                                    this.NodeCommandReceived?.Invoke(convertedPayloadVersionA);
                                }
                            }

                            break;

                        case SparkplugNamespace.VersionB:
                            var payloadVersionB = PayloadHelper.Deserialize<VersionBProtoBuf.ProtoBufPayload>(e.ApplicationMessage.Payload);

                            if (payloadVersionB is not null)
                            {
                                var convertedPayload = PayloadConverter.ConvertVersionBPayload(payloadVersionB);

                                if (topic.Contains(SparkplugMessageType.DeviceCommand.GetDescription()))
                                {
                                    if (convertedPayload is not T convertedPayloadVersionB)
                                    {
                                        throw new InvalidCastException("The metric cast didn't work properly.");
                                    }

                                    this.DeviceCommandReceived?.Invoke(convertedPayloadVersionB);
                                }

                                if (topic.Contains(SparkplugMessageType.NodeCommand.GetDescription()))
                                {
                                    if (convertedPayload is not T convertedPayloadVersionB)
                                    {
                                        throw new InvalidCastException("The metric cast didn't work properly.");
                                    }

                                    this.NodeCommandReceived?.Invoke(convertedPayloadVersionB);
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

        // Debug output.
        this.Logger?.Debug("CONNECT Message: {@ClientOptions}", this.ClientOptions);

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

        // Get the online message.
        var onlineMessage = this.MessageGenerator.GetSparkPlugNodeBirthMessage(
            this.NameSpace,
            this.options.GroupIdentifier,
            this.options.EdgeNodeIdentifier,
            this.KnownMetrics,
            this.LastSequenceNumber,
            this.LastSessionNumber,
            DateTimeOffset.Now);

        // Publish data.
        this.options.CancellationToken ??= CancellationToken.None;

        // Debug output.
        this.Logger?.Debug("NBIRTH Message: {@OnlineMessage}", onlineMessage);

        // Increment the sequence number.
        this.IncrementLastSequenceNumber();

        // Publish the message.
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

        var nodeCommandSubscribeTopic = SparkplugTopicGenerator.GetNodeCommandSubscribeTopic(this.NameSpace, this.options.GroupIdentifier, this.options.EdgeNodeIdentifier);
        await this.Client.SubscribeAsync(nodeCommandSubscribeTopic, (MqttQualityOfServiceLevel) SparkplugQualityOfServiceLevel.AtLeastOnce);

        var deviceCommandSubscribeTopic = SparkplugTopicGenerator.GetWildcardDeviceCommandSubscribeTopic(this.NameSpace, this.options.GroupIdentifier, this.options.EdgeNodeIdentifier);
        await this.Client.SubscribeAsync(deviceCommandSubscribeTopic, (MqttQualityOfServiceLevel) SparkplugQualityOfServiceLevel.AtLeastOnce);

        var stateSubscribeTopic = SparkplugTopicGenerator.GetStateSubscribeTopic(this.options.ScadaHostIdentifier);
        await this.Client.SubscribeAsync(stateSubscribeTopic, (MqttQualityOfServiceLevel) SparkplugQualityOfServiceLevel.AtLeastOnce);
    }
}
