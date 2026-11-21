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
    /// <summary>
    /// Tracks the configured primary host for the lifetime of the node run.
    /// </summary>
    private PrimaryHostState? primaryHost;

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
    /// Starts the Sparkplug node and waits for the primary host before publishing NBIRTH when configured to do so.
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
        var primaryHost = nodeOptions.WaitForPrimaryHost
            ? new PrimaryHostState(this.messageGenerator.GetSparkplugStateTopic(nodeOptions.ScadaHostIdentifier), this.messageGenerator.SpecificationVersion)
            : null;
        this.primaryHost = primaryHost;

        // Add handlers.
        this.AddEventHandlers();

        try
        {
            await this.ConnectAndPublishBirthsInternal(primaryHost);
        }
        catch (Exception exception)
        {
            if (ReferenceEquals(this.primaryHost, primaryHost) && ReferenceEquals(this.Options, nodeOptions))
            {
                var wasStopped = !this.IsRunning;
                this.IsRunning = false;
                primaryHost?.Stop();
                this.RemoveEventHandlers();
                await this.DisconnectAfterStoppingInternal();
                if (wasStopped && primaryHost is not null && exception is not OperationCanceledException)
                {
                    throw new OperationCanceledException("The node was stopped while starting.", exception);
                }
            }

            throw;
        }
    }

    /// <summary>
    /// Stops the Sparkplug node.
    /// </summary>
    public async Task Stop()
    {
        this.IsRunning = false;
        this.primaryHost?.Stop();
        this.RemoveEventHandlers();
        try
        {
            if (this.IsConnected)
            {
                await this.SendNodeDeathMessage();
            }

            await this.FireDisconnected();
        }
        finally
        {
            await this.DisconnectAfterStoppingInternal();
        }
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

        this.EnsurePrimaryHostReady();
        return await this.PublishMessage(metrics);
    }

    /// <summary>
    /// Does a node rebirth.
    /// </summary>
    /// <param name="metrics">The new metrics.</param>
    public async Task Rebirth(IEnumerable<T> metrics)
    {
        this.EnsurePrimaryHostReady();

        // Send node death first.
        await this.SendNodeDeathMessage();

        // Reset the known metrics.
        this.knownMetrics = new KnownMetricStorage(metrics);

        // Send node birth and device births.
        await this.PublishNodeAndDeviceBirthsInternal();
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
        if (!this.IsRunning || this.IsConnected)
        {
            return;
        }

        var options = this.Options ?? throw new ArgumentNullException(nameof(this.Options));
        var primaryHost = this.primaryHost;
        primaryHost?.EndConnection();
        var cancellationToken = options.CancellationToken ?? SystemCancellationToken.None;
        try
        {
            // Invoke disconnected callback.
            await this.FireDisconnected();

            // Wait until the reconnect interval is reached.
            await Task.Delay(options.ReconnectInterval, cancellationToken);

            if (this.IsRunning && !this.IsConnected && ReferenceEquals(this.primaryHost, primaryHost) && ReferenceEquals(this.Options, options))
            {
                await this.ConnectAndPublishBirthsInternal(primaryHost);
            }
        }
        catch (OperationCanceledException) when (!this.IsRunning || !this.IsConnected || cancellationToken.IsCancellationRequested)
        {
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
            var data = args.ApplicationMessage.Payload.ToArray();
            await this.OnMessageReceived(messageTopic!, data);
        }
        else if (topic.Contains(SparkplugMessageType.StateMessage.GetDescription()))
        {
            var status = Encoding.UTF8.GetString(args.ApplicationMessage.Payload.ToArray());
            if (this.primaryHost is { } primaryHost && primaryHost.Observe(topic, status))
            {
                var sessionNumber = this.LastSessionNumber;
                // Disconnecting from the MQTT receive callback would wait on the receive loop itself.
                _ = Task.Run(() => this.DisconnectForOfflinePrimaryHost(primaryHost, sessionNumber));
            }

            await this.FireStatusMessageReceived(status);
        }
        else
        {
            throw new InvalidOperationException($"Received message on unkown topic {topic}: {Convert.ToHexString(args.ApplicationMessage.Payload.ToArray())}.");
        }
    }

    /// <summary>
    /// Establishes an MQTT session, then publishes NBIRTH and known DBIRTH messages when the host is ready.
    /// </summary>
    /// <param name="primaryHost">The optional primary host state for this node run.</param>
    private async Task ConnectAndPublishBirthsInternal(PrimaryHostState? primaryHost)
    {
        var configuredToken = this.Options?.CancellationToken ?? SystemCancellationToken.None;
        using var startupCancellation = primaryHost is null
            ? null
            : CancellationTokenSource.CreateLinkedTokenSource(configuredToken, primaryHost.StoppingToken);
        var cancellationToken = startupCancellation?.Token ?? configuredToken;
        var hostOnline = primaryHost?.BeginConnection();
        await this.ConnectInternal(cancellationToken);
        await this.SubscribeInternal(cancellationToken);
        if (hostOnline is not null)
        {
            await hostOnline.WaitAsync(cancellationToken);
        }

        cancellationToken.ThrowIfCancellationRequested();
        await this.PublishNodeAndDeviceBirthsInternal();
    }

    /// <summary>
    /// Disconnects a stopped node and waits for MQTTnet to deliver the disconnect notification before it can be restarted.
    /// </summary>
    private async Task DisconnectAfterStoppingInternal()
    {
        var disconnected = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        Task CompleteDisconnect(MqttClientDisconnectedEventArgs args)
        {
            disconnected.TrySetResult();
            return Task.CompletedTask;
        }

        this.client.DisconnectedAsync += CompleteDisconnect;
        try
        {
            if (this.IsConnected)
            {
                await this.client.DisconnectAsync();
                await disconnected.Task;
            }
        }
        finally
        {
            this.client.DisconnectedAsync -= CompleteDisconnect;
        }
    }

    /// <summary>
    /// Ends the current MQTT session when a configured primary host goes offline.
    /// </summary>
    /// <param name="primaryHost">The primary host state that requested disconnection.</param>
    /// <param name="sessionNumber">The MQTT session the offline STATE was received on.</param>
    private async Task DisconnectForOfflinePrimaryHost(PrimaryHostState primaryHost, long sessionNumber)
    {
        if (!this.IsRunning || !this.IsConnected || !ReferenceEquals(this.primaryHost, primaryHost) || this.LastSessionNumber != sessionNumber)
        {
            return;
        }

        try
        {
            try
            {
                await this.SendNodeDeathMessage();
            }
            finally
            {
                if (this.IsConnected && this.LastSessionNumber == sessionNumber)
                {
                    await this.client.DisconnectAsync();
                }
            }
        }
        catch (Exception exception)
        {
            this.KnownMetricsStorage.Logger?.LogError(exception, "Could not end the Sparkplug session after the primary host went offline.");
        }
    }

    /// <summary>
    /// Verifies the configured host is online and NBIRTH has been published before sending device births or data.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown while waiting for the host or NBIRTH.</exception>
    private void EnsurePrimaryHostReady()
    {
        if (this.primaryHost is { CanPublish: false })
        {
            throw new InvalidOperationException("The node has not published NBIRTH with the primary host online.");
        }
    }

    /// <summary>
    /// Connects the Sparkplug node to the MQTT broker.
    /// </summary>
    /// <param name="cancellationToken">Cancels connection establishment.</param>
    /// <exception cref="ArgumentNullException">Thrown if the options are null.</exception>
    private async Task ConnectInternal(SystemCancellationToken cancellationToken)
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
        builder.WithWillPayload(willMessage.Payload.ToArray());
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
        await this.client.ConnectAsync(this.ClientOptions, cancellationToken);
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

        var primaryHost = this.primaryHost;
        if (primaryHost is { IsOnline: false })
        {
            throw new InvalidOperationException("The primary host is offline.");
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
        primaryHost?.MarkBirthPublished();

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
    /// <param name="cancellationToken">Cancels the subscriptions.</param>
    /// <exception cref="ArgumentNullException">Thrown if the options are null.</exception>
    private async Task SubscribeInternal(SystemCancellationToken cancellationToken)
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
        await this.client.SubscribeAsync(nodeCommandSubscribeTopic, (MqttQualityOfServiceLevel)SparkplugQualityOfServiceLevel.AtLeastOnce, cancellationToken);

        // Subscribe to the device command topic.
        var deviceCommandSubscribeTopic = SparkplugTopicGenerator.GetWildcardDeviceCommandSubscribeTopic(
            this.NameSpace,
            this.Options.GroupIdentifier,
            this.Options.EdgeNodeIdentifier);
        await this.client.SubscribeAsync(deviceCommandSubscribeTopic, (MqttQualityOfServiceLevel)SparkplugQualityOfServiceLevel.AtLeastOnce, cancellationToken);

        // Subscribe to the state topic.
        var stateSubscribeTopic = this.messageGenerator.GetSparkplugStateTopic(this.Options.ScadaHostIdentifier);
        await this.client.SubscribeAsync(stateSubscribeTopic, (MqttQualityOfServiceLevel)SparkplugQualityOfServiceLevel.AtLeastOnce, cancellationToken);
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
