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
public abstract partial class SparkplugNodeBase<T> : SparkplugBase<T> where T : IMetric, new()
{
    /// <inheritdoc cref="SparkplugBase{T}"/>
    /// <summary>
    /// Initializes a new instance of the <see cref="SparkplugNodeBase{T}"/> class.
    /// </summary>
    /// <param name="knownMetrics">The metric names.</param>
    /// <param name="specificationVersion">The Sparkplug specification version.</param>
    /// <param name="logger">The logger.</param>
    /// <seealso cref="SparkplugBase{T}"/>
    public SparkplugNodeBase(
        IEnumerable<T> knownMetrics,
        SparkplugSpecificationVersion specificationVersion,
        ILogger<KnownMetricStorage>? logger = null)
        : base(knownMetrics, specificationVersion, logger)
    {
    }

    /// <inheritdoc cref="SparkplugBase{T}"/>
    /// <summary>
    /// Initializes a new instance of the <see cref="SparkplugNodeBase{T}"/> class.
    /// </summary>
    /// <param name="knownMetricsStorage">The known metrics storage.</param>
    /// <param name="specificationVersion">The Sparkplug specification version.</param>
    /// <seealso cref="SparkplugBase{T}"/>
    public SparkplugNodeBase(
        KnownMetricStorage knownMetricsStorage,
        SparkplugSpecificationVersion specificationVersion)
        : base(knownMetricsStorage, specificationVersion)
    {
    }

    /// <summary>
    /// Gets the options.
    /// </summary>
    public SparkplugNodeOptions? Options { get; private set; }

    /// <summary>
    /// Starts the Sparkplug node.
    /// </summary>
    /// <param name="nodeOptions">The node options.</param>
    /// <param name="knownMetricsStorage">The known metrics storage.</param>
    /// <exception cref="ArgumentNullException">Thrown if the options are null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the start method is called more than once.</exception>
    public async Task Start(SparkplugNodeOptions nodeOptions, KnownMetricStorage? knownMetricsStorage = null)
    {
        if (nodeOptions is null)
        {
            throw new ArgumentNullException(nameof(nodeOptions), "The options aren't set properly.");
        }

        if (this.IsRunning)
        {
            throw new InvalidOperationException("Start should only be called once!");
        }

        this.IsRunning = true;

        if (knownMetricsStorage is not null)
        {
            this.knownMetrics = knownMetricsStorage;
        }

        // Storing the options.
        this.Options = nodeOptions;

        // Add handlers.
        this.AddEventHandlers();

        // Connect, subscribe to incoming messages and send a state message.
        await this.ConnectInternal();
        await this.SubscribeInternal();

        if (string.IsNullOrEmpty(this.Options.ScadaHostIdentifier))
        {
            await this.PublishNodeAndDeviceBirthsInternal(); 
        }
    }

    /// <summary>
    /// Stops the Sparkplug node.
    /// </summary>
    public async Task Stop()
    {
        this.IsRunning = false;
        await this.SendNodeDeathMessage();
        this.RemoveEventHandlers();
        await this.FireDisconnected();
        await this.client.DisconnectAsync();
    }

    /// <summary>
    /// Publishes some metrics.
    /// </summary>
    /// <param name="metrics">The metrics.</param>
    /// <exception cref="ArgumentNullException">Thrown if the options are null.</exception>
    /// <exception cref="Exception">Thrown if the MQTT client is not connected or an invalid metric type was specified.</exception>
    /// <returns>A <see cref="MqttClientPublishResult"/>.</returns>
    public async Task<MqttClientPublishResult> PublishMetrics(IEnumerable<T> metrics)
    {
        if (this.Options is null)
        {
            throw new ArgumentNullException(nameof(this.Options), "The options aren't set properly.");
        }

        if (!this.client.IsConnected)
        {
            throw new Exception("The MQTT client is not connected, please try again.");
        }

        return await this.PublishMessage(metrics);
    }

    /// <summary>
    /// Does a node birth.
    /// </summary>
    /// <param name="metrics">The new metrics.</param>
    public async Task Birth(IEnumerable<T> metrics)
    {
        // Reset the known metrics.
        this.knownMetrics = new KnownMetricStorage(metrics);

        // Send node birth and device births.
        await this.PublishNodeAndDeviceBirthsInternal();
    }

    /// <summary>
    /// Does a node rebirth.
    /// </summary>
    /// <param name="metrics">The new metrics.</param>
    public async Task Rebirth(IEnumerable<T> metrics)
    {
        // Send node death first.
        await this.SendNodeDeathMessage();

        await this.Birth(metrics);
    }

    /// <summary>
    /// Publishes metrics for a node.
    /// </summary>
    /// <param name="metrics">The metrics.</param>
    /// <returns>A <see cref="MqttClientPublishResult"/>.</returns>
    protected abstract Task<MqttClientPublishResult> PublishMessage(IEnumerable<T> metrics);

    /// <summary>
    /// Called when the message is received.
    /// </summary>
    /// <param name="topic">The topic.</param>
    /// <param name="payload">The payload.</param>
    protected abstract Task OnMessageReceived(SparkplugMessageTopic topic, byte[] payload);

    /// <summary>
    /// Handles the client disconnection event.
    /// </summary>
    /// <param name="args">The event args.</param>
    protected virtual async Task OnClientConnected(MqttClientConnectedEventArgs args)
    {
        try
        {
            await this.FireConnected();
        }
        catch (Exception ex)
        {
            await Task.FromException(ex);
        }
    }

    /// <summary>
    /// Adds the event handlers to the client.
    /// </summary>
    private void AddEventHandlers()
    {
        this.client.DisconnectedAsync += this.OnClientDisconnected;
        this.client.ConnectedAsync += this.OnClientConnected;
        this.client.ApplicationMessageReceivedAsync += this.OnApplicationMessageReceived;
    }

    /// <summary>
    /// Removes the event handlers from the client.
    /// </summary>
    private void RemoveEventHandlers()
    {
        this.client.DisconnectedAsync -= this.OnClientDisconnected;
        this.client.ConnectedAsync -= this.OnClientConnected;
        this.client.ApplicationMessageReceivedAsync -= this.OnApplicationMessageReceived;
    }

    /// <summary>
    /// Handles the client disconnection.
    /// </summary>
    /// <param name="args">The event args.</param>
    /// <exception cref="ArgumentNullException">Thrown if the options are null.</exception>
    private async Task OnClientDisconnected(MqttClientDisconnectedEventArgs args)
    {
        try
        {
            if (this.Options is null)
            {
                throw new ArgumentNullException(nameof(this.Options));
            }

            // Invoke disconnected callback.
            await this.FireDisconnected();

            // Wait until the reconnect interval is reached.
            await Task.Delay(this.Options.ReconnectInterval);

            if (this.IsRunning)
            {
                // Connect, subscribe to incoming messages and send a state message.
                await this.ConnectInternal();
                await this.SubscribeInternal();
                await this.PublishNodeAndDeviceBirthsInternal();
            }
        }
        catch (Exception ex)
        {
            await Task.FromException(ex);
        }
    }

    /// <summary>
    /// Handles the message received handler.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <exception cref="InvalidOperationException">Thrown if a message on an unknown topic is received.</exception>
    private async Task OnApplicationMessageReceived(MqttApplicationMessageReceivedEventArgs args)
    {
        var topic = args.ApplicationMessage.Topic;

        if (SparkplugMessageTopic.TryParse(topic, out var messageTopic))
        {
            var data = args.ApplicationMessage.PayloadSegment.Array ?? [];
            await this.OnMessageReceived(messageTopic!, data);
        }
        else if (topic.Contains(SparkplugMessageType.StateMessage.GetDescription()))
        {
            // Handle the STATE message before anything else as they're UTF-8 encoded.
            await this.FireStatusMessageReceived(Encoding.UTF8.GetString(args.ApplicationMessage.PayloadSegment));
        }
        else
        {
            throw new InvalidOperationException($"Received message on unkown topic {topic}: {args.ApplicationMessage.PayloadSegment:X2}.");
        }
    }

    /// <summary>
    /// Connects the Sparkplug node to the MQTT broker.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown if the options are null.</exception>
    private async Task ConnectInternal()
    {
        if (this.Options is null)
        {
            throw new ArgumentNullException(nameof(this.Options));
        }

        // Increment the session number.
        this.IncrementLastSessionNumber();

        // Reset the sequence number.
        this.ResetLastSequenceNumber();

        // Get the will message.
        var willMessage = this.messageGenerator.GetSparkplugNodeDeathMessage(
            this.NameSpace,
            this.Options.GroupIdentifier,
            this.Options.EdgeNodeIdentifier,
            this.LastSessionNumber);

        // Build up the MQTT client and connect.
        this.Options.CancellationToken ??= SystemCancellationToken.None;

        var builder = new MqttClientOptionsBuilder()
            .WithClientId(this.Options.ClientId)
            .WithCredentials(this.Options.UserName, this.Options.Password)
            .WithProtocolVersion((MqttProtocolVersion)this.Options.MqttProtocolVersion);

        switch (this.Options.MqttProtocolVersion)
        {
            case SparkplugMqttProtocolVersion.V311:
                builder.WithCleanSession(true);
                break;
            case SparkplugMqttProtocolVersion.V500:
                builder.WithCleanStart(true);
                builder.WithSessionExpiryInterval(0);
                break;
        }

        if (this.Options.MqttTlsOptions is not null)
        {
            builder.WithTlsOptions(this.Options.MqttTlsOptions);
        }

        if (this.Options.MqttWebSocketOptions is null)
        {
            builder.WithTcpServer(this.Options.BrokerAddress, this.Options.Port);
        }
        else
        {
            builder.WithWebSocketServer(o =>
                o.WithCookieContainer(this.Options.MqttWebSocketOptions.CookieContainer)
                .WithCookieContainer(this.Options.MqttWebSocketOptions.Credentials)
                .WithProxyOptions(this.Options.MqttWebSocketOptions.ProxyOptions)
                .WithRequestHeaders(this.Options.MqttWebSocketOptions.RequestHeaders)
                .WithSubProtocols(this.Options.MqttWebSocketOptions.SubProtocols)
                .WithUri(this.Options.BrokerAddress)
                .WithKeepAliveInterval(this.Options.MqttWebSocketOptions.KeepAliveInterval)
                .WithUseDefaultCredentials(this.Options.MqttWebSocketOptions.UseDefaultCredentials)
            );
        }

        // Add the will message data.
        builder.WithWillContentType(willMessage.ContentType);
        builder.WithWillCorrelationData(willMessage.CorrelationData);
        builder.WithWillDelayInterval(willMessage.MessageExpiryInterval);
        builder.WithWillPayload(willMessage.PayloadSegment);
        builder.WithWillPayloadFormatIndicator(willMessage.PayloadFormatIndicator);
        builder.WithWillQualityOfServiceLevel(willMessage.QualityOfServiceLevel);
        builder.WithWillResponseTopic(willMessage.ResponseTopic);
        builder.WithWillRetain(willMessage.Retain);
        builder.WithWillTopic(willMessage.Topic);

        if (willMessage.UserProperties is not null)
        {
            foreach (var userProperty in willMessage.UserProperties)
            {
                builder.WithWillUserProperty(userProperty.Name, userProperty.Value);
            }
        }

        this.ClientOptions = builder.Build();
        await this.client.ConnectAsync(this.ClientOptions, this.Options.CancellationToken.Value);
    }

    /// <summary>
    /// Publishes data to the MQTT broker.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown if the options are null.</exception>
    private async Task PublishNodeAndDeviceBirthsInternal()
    {
        if (this.Options is null)
        {
            throw new ArgumentNullException(nameof(this.Options));
        }

        // Get the node birth message.
        var nodeBirthMessage = this.messageGenerator.GetSparkplugNodeBirthMessage(
            this.NameSpace,
            this.Options.GroupIdentifier,
            this.Options.EdgeNodeIdentifier,
            this.KnownMetrics,
            this.LastSequenceNumber,
            this.LastSessionNumber,
            DateTimeOffset.UtcNow);

        // Set the cancellation token.
        this.Options.CancellationToken ??= SystemCancellationToken.None;

        // Increment the sequence number.
        this.IncrementLastSequenceNumber();

        // Publish the message.
        await this.client.PublishAsync(nodeBirthMessage, this.Options.CancellationToken.Value);

        // Publish device births for all known devices.
        if (this.Options.PublishKnownDeviceMetricsOnReconnect)
        {
            foreach (var device in this.KnownDevices)
            {
                await this.PublishDeviceBirthMessage(device.Value.Metrics, device.Key, this.KnownMetricsStorage.Logger);
            }
        }
    }

    /// <summary>
    /// Subscribes the client to the node subscribe topics.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown if the options are null.</exception>
    private async Task SubscribeInternal()
    {
        if (this.Options is null)
        {
            throw new ArgumentNullException(nameof(this.Options));
        }

        // Subscribe to the node command topic.
        var nodeCommandSubscribeTopic = SparkplugTopicGenerator.GetNodeCommandSubscribeTopic(
            this.NameSpace,
            this.Options.GroupIdentifier,
            this.Options.EdgeNodeIdentifier);
        await this.client.SubscribeAsync(nodeCommandSubscribeTopic, (MqttQualityOfServiceLevel)SparkplugQualityOfServiceLevel.AtLeastOnce);

        // Subscribe to the device command topic.
        var deviceCommandSubscribeTopic = SparkplugTopicGenerator.GetWildcardDeviceCommandSubscribeTopic(
            this.NameSpace,
            this.Options.GroupIdentifier,
            this.Options.EdgeNodeIdentifier);
        await this.client.SubscribeAsync(deviceCommandSubscribeTopic, (MqttQualityOfServiceLevel)SparkplugQualityOfServiceLevel.AtLeastOnce);

        // Subscribe to the state topic.
        //var stateSubscribeTopic = SparkplugTopicGenerator.GetStateSubscribeTopic(this.Options.ScadaHostIdentifier);
        var stateSubscribeTopic = SparkplugTopicGenerator.GetSparkplugStateMessageTopic(this.Options.ScadaHostIdentifier, this.specificationVersion);
        await this.client.SubscribeAsync(stateSubscribeTopic, (MqttQualityOfServiceLevel)SparkplugQualityOfServiceLevel.AtLeastOnce);
    }

    /// <summary>
    /// Sends the node death message when intentionally stopping the node.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown if the options are null.</exception>
    private async Task SendNodeDeathMessage()
    {
        if (this.Options is null)
        {
            throw new ArgumentNullException(nameof(this.Options));
        }

        // Get the node death message.
        var nodeDeathMessage = this.messageGenerator.GetSparkplugNodeDeathMessage(
            this.NameSpace,
            this.Options.GroupIdentifier,
            this.Options.EdgeNodeIdentifier,
            this.LastSessionNumber);

        // Set the cancellation token.
        this.Options.CancellationToken ??= SystemCancellationToken.None;

        // Publish message.
        await this.client.PublishAsync(nodeDeathMessage, this.Options.CancellationToken.Value);
    }
}
