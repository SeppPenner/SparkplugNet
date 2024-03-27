// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugApplicationBase.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class that handles a Sparkplug application.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Core.Application;

/// <inheritdoc cref="SparkplugBase{T}"/>
/// <summary>
/// A class that handles a Sparkplug application.
/// </summary>
/// <seealso cref="SparkplugBase{T}"/>
public abstract partial class SparkplugApplicationBase<T> : SparkplugBase<T> where T : IMetric, new()
{
    /// <inheritdoc cref="SparkplugBase{T}"/>
    /// <summary>
    /// Initializes a new instance of the <see cref="SparkplugApplicationBase{T}"/> class.
    /// </summary>
    /// <param name="knownMetrics">The known metric names.</param>
    /// <param name="specificationVersion">The Sparkplug specification version.</param>
    /// <param name="logger">The logger.</param>
    /// <seealso cref="SparkplugBase{T}"/>
    public SparkplugApplicationBase(
        IEnumerable<T> knownMetrics,
        SparkplugSpecificationVersion specificationVersion,
        ILogger<KnownMetricStorage>? logger = null)
        : base(knownMetrics, specificationVersion, logger)
    {
    }

    /// <inheritdoc cref="SparkplugBase{T}"/>
    /// <summary>
    /// Initializes a new instance of the <see cref="SparkplugApplicationBase{T}"/> class.
    /// </summary>
    /// <param name="knownMetricsStorage">The known metric storage.</param>
    /// <param name="specificationVersion">The Sparkplug specification version.</param>
    /// <seealso cref="SparkplugBase{T}"/>
    public SparkplugApplicationBase(
        KnownMetricStorage knownMetricsStorage,
        SparkplugSpecificationVersion specificationVersion)
        : base(knownMetricsStorage, specificationVersion)
    {
    }

    /// <summary>
    /// Gets the node states.
    /// </summary>
    public ConcurrentDictionary<string, MetricState<T>> NodeStates { get; } = new();

    /// <summary>
    /// Gets the device states.
    /// </summary>
    public ConcurrentDictionary<string, MetricState<T>> DeviceStates { get; } = new();

    /// <summary>
    /// Gets the options.
    /// </summary>
    public SparkplugApplicationOptions? Options { private set; get; }

    /// <summary>
    /// Starts the Sparkplug application.
    /// </summary>
    /// <param name="applicationOptions">The application options.</param>
    /// <exception cref="InvalidOperationException">Thrown if the start should be only called once.</exception>
    /// <exception cref="ArgumentNullException">Thrown if the options are null.</exception>
    public async Task Start(SparkplugApplicationOptions applicationOptions)
    {
        if (this.IsRunning)
        {
            throw new InvalidOperationException("Start should only be called once!");
        }

        this.IsRunning = true;

        // Storing the options.
        this.Options = applicationOptions;

        if (this.Options is null)
        {
            throw new ArgumentNullException(nameof(this.Options), "The options aren't set properly.");
        }

        // Clear states.
        this.NodeStates.Clear();
        this.DeviceStates.Clear();

        // Add handlers.
        this.AddEventHandlers();

        // Connect, subscribe to incoming messages and send a state message.
        await this.ConnectInternal();
        await this.SubscribeInternal();
        await this.PublishInternal();
    }

    /// <summary>
    /// Stops the Sparkplug application.
    /// </summary>
    public async Task Stop()
    {
        this.IsRunning = false;
        await this.SendOfflineStateMessage();
        this.RemoveEventHandlers();
        await this.FireDisconnected();
        await this.client.DisconnectAsync();
    }

    /// <summary>
    /// Publishes a node command.
    /// </summary>
    /// <param name="metrics">The metrics.</param>
    /// <param name="groupIdentifier">The group identifier.</param>
    /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
    /// <exception cref="ArgumentNullException">Thrown if the options are null.</exception>
    /// <exception cref="Exception">Thrown if the MQTT client is not connected or an invalid metric type was specified.</exception>
    /// <exception cref="ArgumentException">Thrown if the group or edge node identifier is invalid.</exception>
    public async Task PublishNodeCommand(IEnumerable<T> metrics, string groupIdentifier, string edgeNodeIdentifier)
    {
        if (this.Options is null)
        {
            throw new ArgumentNullException(nameof(this.Options), "The options aren't set properly.");
        }

        if (!this.client.IsConnected)
        {
            throw new Exception("The MQTT client is not connected, please try again.");
        }

        if (!groupIdentifier.IsIdentifierValid())
        {
            throw new ArgumentException("The group identifier wasn't set properly.", nameof(groupIdentifier));
        }

        if (!edgeNodeIdentifier.IsIdentifierValid())
        {
            throw new ArgumentException("The edge node identifier wasn't set properly.", nameof(edgeNodeIdentifier));
        }

        await this.PublishNodeCommandMessage(metrics, groupIdentifier, edgeNodeIdentifier);
    }

    /// <summary>
    /// Publishes a device command message.
    /// </summary>
    /// <param name="metrics">The metrics.</param>
    /// <param name="groupIdentifier">The group identifier.</param>
    /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
    /// <param name="deviceIdentifier">The device identifier.</param>
    /// <exception cref="ArgumentNullException">Thrown if the options are null.</exception>
    /// <exception cref="Exception">Thrown if the MQTT client is not connected or an invalid metric type was specified.</exception>
    /// <exception cref="ArgumentException">Thrown if the group or edge node or device identifier is invalid.</exception>
    public async Task PublishDeviceCommand(IEnumerable<T> metrics, string groupIdentifier, string edgeNodeIdentifier, string deviceIdentifier)
    {
        if (this.Options is null)
        {
            throw new ArgumentNullException(nameof(this.Options), "The options aren't set properly.");
        }

        if (!this.client.IsConnected)
        {
            throw new Exception("The MQTT client is not connected, please try again.");
        }

        if (!groupIdentifier.IsIdentifierValid())
        {
            throw new ArgumentException("The group identifier wasn't set properly.", nameof(groupIdentifier));
        }

        if (!edgeNodeIdentifier.IsIdentifierValid())
        {
            throw new ArgumentException("The edge node identifier wasn't set properly.", nameof(edgeNodeIdentifier));
        }

        if (!deviceIdentifier.IsIdentifierValid())
        {
            throw new ArgumentException("The device identifier wasn't set properly.", nameof(deviceIdentifier));
        }

        await this.PublishDeviceCommandMessage(metrics, groupIdentifier, edgeNodeIdentifier, deviceIdentifier);
    }

    /// <summary>
    /// Publishes a node command message.
    /// </summary>
    /// <param name="metrics">The metrics.</param>
    /// <param name="groupIdentifier">The group identifier.</param>
    /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
    protected abstract Task PublishNodeCommandMessage(IEnumerable<T> metrics, string groupIdentifier, string edgeNodeIdentifier);

    /// <summary>
    /// Publishes a device command message.
    /// </summary>
    /// <param name="metrics">The metrics.</param>
    /// <param name="groupIdentifier">The group identifier.</param>
    /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
    /// <param name="deviceIdentifier">The device identifier.</param>
    protected abstract Task PublishDeviceCommandMessage(IEnumerable<T> metrics, string groupIdentifier, string edgeNodeIdentifier, string deviceIdentifier);

    /// <summary>
    /// Called when a message is received.
    /// </summary>
    /// <param name="topic">The topic.</param>
    /// <param name="payload">The payload.</param>
    protected abstract Task OnMessageReceived(SparkplugMessageTopic topic, byte[] payload);

    /// <summary>
    /// Handles the client connected event.
    /// </summary>
    /// <param name="args">The arguments.</param>
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
    /// Handles the client disconnection event.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <exception cref="ArgumentNullException">Thrown if the options are null.</exception>
    protected virtual async Task OnClientDisconnected(MqttClientDisconnectedEventArgs args)
    {
        try
        {
            if (this.Options is null)
            {
                throw new ArgumentNullException(nameof(this.Options), "The options aren't set properly.");
            }

            // Set all metrics to stale.
            this.UpdateMetricState(SparkplugMetricStatus.Offline);

            // Invoke disconnected callback.
            await this.FireDisconnected();

            // Wait until the reconnect interval is reached.
            await Task.Delay(this.Options.ReconnectInterval);

            if (this.IsRunning)
            {
                // Connect, subscribe to incoming messages and send a state message.
                await this.ConnectInternal();
                this.UpdateMetricState(SparkplugMetricStatus.Online);
                await this.SubscribeInternal();
                await this.PublishInternal();
            }
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
    /// Handles the message received handler.
    /// </summary>
    /// <param name="args">The arguments.</param>
    private Task OnApplicationMessageReceived(MqttApplicationMessageReceivedEventArgs args)
    {
        try
        {
            var topic = args.ApplicationMessage.Topic;

            if (SparkplugMessageTopic.TryParse(topic, out var topicParsed))
            {
                var data = args.ApplicationMessage.PayloadSegment.Array ?? [];
                return this.OnMessageReceived(topicParsed!, data);
            }
            else if (topic.Contains(SparkplugMessageType.StateMessage.GetDescription()))
            {
                // Skip the STATE messages as they're UTF-8 encoded.
                return Task.CompletedTask;
            }
            else
            {
                return Task.CompletedTask;
            }
        }
        catch (Exception ex)
        {
            return Task.FromException(ex);
        }
    }

    /// <summary>
    /// Connects the Sparkplug application to the MQTT broker.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown if the options are null.</exception>
    private async Task ConnectInternal()
    {
        try
        {
            if (this.Options is null)
            {
                throw new ArgumentNullException(nameof(this.Options));
            }

            // Increment the session number.
            this.IncrementLastSessionNumber();

            // Get the will message.
            var willMessage = this.messageGenerator.GetSparkplugStateMessage(
                this.NameSpace,
                this.Options.ScadaHostIdentifier,
                false);

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
            else
            {
                builder.WithTlsOptions(o => o.UseTls());
            }

            if (this.Options.MqttWebSocketOptions is null)
            {
                builder.WithTcpServer(this.Options.BrokerAddress, this.Options.Port);
            }
            else
            {
                builder.WithWebSocketServer(options => 
                    options.WithCookieContainer(this.Options.MqttWebSocketOptions.CookieContainer)
                    .WithCookieContainer(this.Options.MqttWebSocketOptions.Credentials)
                    .WithProxyOptions(this.Options.MqttWebSocketOptions.ProxyOptions)
                    .WithRequestHeaders(this.Options.MqttWebSocketOptions.RequestHeaders)
                    .WithSubProtocols(this.Options.MqttWebSocketOptions.SubProtocols)
                    .WithUri(this.Options.BrokerAddress)
                    .WithKeepAliveInterval(this.Options.MqttWebSocketOptions.KeepAliveInterval)
                    .WithUseDefaultCredentials(this.Options.MqttWebSocketOptions.UseDefaultCredentials)
                );
            }

            if (this.Options.IsPrimaryApplication)
            {
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
            }

            this.ClientOptions = builder.Build();
            await this.client.ConnectAsync(this.ClientOptions, this.Options.CancellationToken.Value);
        }
        catch (Exception ex)
        {
            await Task.FromException(ex);
        }
    }

    /// <summary>
    /// Publishes data to the MQTT broker.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown if the options are null.</exception>
    private async Task PublishInternal()
    {
        if (this.Options is null)
        {
            throw new ArgumentNullException(nameof(this.Options));
        }

        // Only send state messages for the primary application.
        if (this.Options.IsPrimaryApplication)
        {
            // Get the online message.
            var onlineMessage = this.messageGenerator.GetSparkplugStateMessage(
                this.NameSpace,
                this.Options.ScadaHostIdentifier,
                true);

            // Increment the sequence number.
            this.IncrementLastSequenceNumber();

            // Publish message.
            this.Options.CancellationToken ??= SystemCancellationToken.None;
            await this.client.PublishAsync(onlineMessage, this.Options.CancellationToken.Value);
        }
    }

    /// <summary>
    /// Subscribes the client to the application subscribe topic.
    /// </summary>
    private async Task SubscribeInternal()
    {
        var topic = SparkplugTopicGenerator.GetWildcardNamespaceSubscribeTopic(this.NameSpace);
        await this.client.SubscribeAsync(topic, (MqttQualityOfServiceLevel)SparkplugQualityOfServiceLevel.AtLeastOnce);
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

    /// <summary>
    /// Sends the offline state message when intentionally stopping the application.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown if the options are null.</exception>
    private async Task SendOfflineStateMessage()
    {
        if (this.Options is null)
        {
            throw new ArgumentNullException(nameof(this.Options));
        }

        // Get the node death message.
        var offlineStateMessage = this.messageGenerator.GetSparkplugStateMessage(
            this.NameSpace,
            this.Options.ScadaHostIdentifier,
            false);

        // Publish message.
        this.Options.CancellationToken ??= SystemCancellationToken.None;
        await this.client.PublishAsync(offlineStateMessage, this.Options.CancellationToken.Value);
    }
}
