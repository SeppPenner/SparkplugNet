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
    /// <summary>
    /// The current state and birth definitions for each Sparkplug source.
    /// </summary>
    private readonly ConcurrentDictionary<(string GroupIdentifier, string EdgeNodeIdentifier, string? DeviceIdentifier), MetricState<T>> metricStates = new();

    /// <summary>
    /// Serializes online and offline STATE publications.
    /// </summary>
    private readonly SemaphoreSlim statePublicationLock = new(1);

    /// <summary>
    /// The primary host STATE messages for the current MQTT connection.
    /// </summary>
    private volatile PrimaryStateSession? primaryStateSession;

    /// <summary>
    /// The prepared online STATE for one connection.
    /// </summary>
    /// <param name="HostIdentifier">The host identifier used in the MQTT Will topic.</param>
    /// <param name="Birth">The online STATE paired with the MQTT Will.</param>
    private sealed record PrimaryStateSession(string HostIdentifier, MqttApplicationMessage Birth)
    {
        /// <summary>
        /// Whether the host has successfully published its online STATE.
        /// </summary>
        internal volatile bool BirthPublished;
    }

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
    /// Gets the most recently updated node state for each edge node identifier.
    /// Use <see cref="MetricStatesBySource"/> when edge node identifiers repeat across groups.
    /// </summary>
    public ConcurrentDictionary<string, MetricState<T>> NodeStates { get; } = new();

    /// <summary>
    /// Gets the most recently updated device state for each edge node and device identifier pair.
    /// Use <see cref="MetricStatesBySource"/> when identifiers repeat across groups.
    /// </summary>
    public ConcurrentDictionary<string, MetricState<T>> DeviceStates { get; } = new();

    /// <summary>
    /// Gets the node and device states keyed by group, edge node, and optional device identifier.
    /// A null device identifier selects the node itself.
    /// </summary>
    public IReadOnlyDictionary<(string GroupIdentifier, string EdgeNodeIdentifier, string? DeviceIdentifier), MetricState<T>> MetricStatesBySource => this.metricStates;

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

        ArgumentNullException.ThrowIfNull(applicationOptions);
        this.IsRunning = true;
        this.Options = applicationOptions;

        // Clear states.
        this.NodeStates.Clear();
        this.DeviceStates.Clear();
        this.metricStates.Clear();

        // Add handlers.
        this.AddEventHandlers();

        try
        {
            await this.ConnectInternal();
            await this.SubscribeInternal();
            await this.PublishInternal();
        }
        catch
        {
            this.IsRunning = false;
            this.primaryStateSession = null;
            this.RemoveEventHandlers();
            await this.client.DisconnectAsync();
            throw;
        }
    }

    /// <summary>
    /// Stops the Sparkplug application.
    /// </summary>
    public async Task Stop()
    {
        this.IsRunning = false;
        try
        {
            await this.SendOfflineStateMessage();
        }
        finally
        {
            this.RemoveEventHandlers();
            try
            {
                await this.FireDisconnected();
            }
            finally
            {
                await this.client.DisconnectAsync();
            }
        }
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
    /// Gets the definitions learned from the source or the configured registry before a birth arrives.
    /// </summary>
    /// <param name="groupIdentifier">The group identifier.</param>
    /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
    /// <param name="deviceIdentifier">The device identifier, if applicable.</param>
    /// <returns>The metric definitions for decoding or validating a message.</returns>
    private protected KnownMetricStorage GetSourceMetricStorage(string groupIdentifier, string edgeNodeIdentifier, string? deviceIdentifier = null) =>
        this.metricStates.GetValueOrDefault((groupIdentifier, edgeNodeIdentifier, deviceIdentifier))?.BirthMetrics ?? this.KnownMetricsStorage;

    /// <summary>
    /// Validates metrics against the source definitions and any configured custom filter.
    /// </summary>
    /// <param name="metrics">The metrics to validate.</param>
    /// <param name="messageType">The type of the message.</param>
    /// <param name="groupIdentifier">The group identifier.</param>
    /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
    /// <param name="deviceIdentifier">The device identifier, if applicable.</param>
    /// <returns>The metrics accepted by the source and the configured filter.</returns>
    private protected IEnumerable<T> FilterSourceMetrics(
        IEnumerable<T> metrics, SparkplugMessageType messageType, string groupIdentifier, string edgeNodeIdentifier, string? deviceIdentifier = null)
    {
        var definitions = this.GetSourceMetricStorage(groupIdentifier, edgeNodeIdentifier, deviceIdentifier);
        var configured = this.KnownMetricsStorage;
        var accepted = definitions.FilterMetrics(metrics, messageType);
        if (!ReferenceEquals(definitions, configured) && configured.GetType() != typeof(KnownMetricStorage))
        {
            return configured.FilterMetrics(accepted, messageType);
        }

        return accepted;
    }

    /// <summary>
    /// Filters a received payload and applies it to the source's cached state.
    /// </summary>
    /// <param name="topic">The received topic.</param>
    /// <param name="metrics">The payload metrics.</param>
    /// <param name="sessionNumberMetric">The session number metric, if present.</param>
    /// <returns>The metrics to deliver to event handlers.</returns>
    private protected List<T> ProcessReceivedMetrics(SparkplugMessageTopic topic, IReadOnlyList<T> metrics, out T? sessionNumberMetric)
    {
        sessionNumberMetric = metrics.FirstOrDefault(metric => metric.Name == Constants.SessionNumberMetricName);
        var payloadMetrics = metrics.Where(metric => metric.Name != Constants.SessionNumberMetricName);
        var isBirth = topic.MessageType is SparkplugMessageType.NodeBirth or SparkplugMessageType.DeviceBirth;
        var source = GetMetricSource(topic);
        List<T> filteredMetrics;
        var configured = this.KnownMetricsStorage;
        if (isBirth && (this.Options?.DiscoverMetricsFromBirth == true || (configured.IsEmpty && configured.GetType() == typeof(KnownMetricStorage))))
        {
            filteredMetrics = payloadMetrics.Where(metric =>
                !string.IsNullOrWhiteSpace(metric.Name) && (metric is Metric { IsNull: true } || metric.Value is not null)).ToList();
        }
        else
        {
            var accepted = isBirth ? configured.FilterMetrics(payloadMetrics, topic.MessageType)
                : this.FilterSourceMetrics(payloadMetrics, topic.MessageType, source.GroupIdentifier, source.EdgeNodeIdentifier, source.DeviceIdentifier);
            filteredMetrics = accepted.ToList();
        }

        if (sessionNumberMetric is not null)
        {
            filteredMetrics.Add(sessionNumberMetric);
        }

        this.UpdateReceivedMetricState(topic, source, filteredMetrics);
        return filteredMetrics;
    }

    /// <summary>
    /// Gets the source of a node or device message.
    /// </summary>
    /// <param name="topic">The received topic.</param>
    /// <returns>The source identifiers shared across message types.</returns>
    /// <exception cref="InvalidOperationException">Thrown if a device message has no valid device identifier.</exception>
    private static (string GroupIdentifier, string EdgeNodeIdentifier, string? DeviceIdentifier) GetMetricSource(SparkplugMessageTopic topic)
    {
        var isDevice = topic.MessageType is SparkplugMessageType.DeviceBirth or SparkplugMessageType.DeviceData or SparkplugMessageType.DeviceDeath;
        if (isDevice && string.IsNullOrWhiteSpace(topic.DeviceIdentifier))
        {
            throw new InvalidOperationException($"Topic {topic} is invalid!");
        }

        return (topic.GroupIdentifier, topic.EdgeNodeIdentifier, isDevice ? topic.DeviceIdentifier : null);
    }

    /// <summary>
    /// Applies a received birth, data, or death message to the cached metric state.
    /// </summary>
    /// <param name="topic">The topic.</param>
    /// <param name="source">The source of the message.</param>
    /// <param name="metrics">The received metrics.</param>
    private void UpdateReceivedMetricState(
        SparkplugMessageTopic topic,
        (string GroupIdentifier, string EdgeNodeIdentifier, string? DeviceIdentifier) source,
        IReadOnlyList<T> metrics)
    {
        if (topic.MessageType == SparkplugMessageType.NodeDeath)
        {
            this.MarkNodeOffline(topic, metrics);
            return;
        }

        var isBirth = topic.MessageType is SparkplugMessageType.NodeBirth or SparkplugMessageType.DeviceBirth;
        var isDeviceDeath = topic.MessageType == SparkplugMessageType.DeviceDeath;
        if (!isBirth && !isDeviceDeath && topic.MessageType is not (SparkplugMessageType.NodeData or SparkplugMessageType.DeviceData))
        {
            return;
        }

        var state = isBirth
            ? new MetricState<T>
            {
                BirthMetrics = new KnownMetricStorage(metrics.Where(metric => metric.Name != Constants.SessionNumberMetricName), this.KnownMetricsStorage.Logger),
                BirthSessionNumber = source.DeviceIdentifier is null ? ReadSessionNumber(metrics) : null
            }
            : this.metricStates.GetOrAdd(source, static _ => new MetricState<T>());

        if (isDeviceDeath)
        {
            state.MetricStatus = SparkplugMetricStatus.Offline;
            this.UpdateLegacyMetricState(source, state);
            return;
        }

        foreach (var metric in metrics)
        {
            var name = metric.Name;
            if (string.IsNullOrWhiteSpace(name) && metric is Metric { Alias: ulong alias })
            {
                name = state.BirthMetrics?.GetMetricName(alias) ?? name;
            }

            if (name is not null)
            {
                state.Metrics[name] = metric;
            }
        }

        state.MetricStatus = SparkplugMetricStatus.Online;
        if (isBirth)
        {
            this.metricStates[source] = state;
        }

        this.UpdateLegacyMetricState(source, state);
    }

    /// <summary>
    /// Updates the state exposed through the edge and device identifier indexes.
    /// </summary>
    /// <param name="source">The source of the state.</param>
    /// <param name="state">The metric state.</param>
    private void UpdateLegacyMetricState(
        (string GroupIdentifier, string EdgeNodeIdentifier, string? DeviceIdentifier) source, MetricState<T> state)
    {
        if (source.DeviceIdentifier is null)
        {
            this.NodeStates[source.EdgeNodeIdentifier] = state;
        }
        else
        {
            this.DeviceStates[$"{source.EdgeNodeIdentifier}/{source.DeviceIdentifier}"] = state;
        }
    }

    /// <summary>
    /// Marks the node and its devices offline when the death matches their group and node session.
    /// </summary>
    /// <param name="topic">The received node death topic.</param>
    /// <param name="metrics">The node death metrics.</param>
    private void MarkNodeOffline(SparkplugMessageTopic topic, IReadOnlyList<T> metrics)
    {
        var source = (topic.GroupIdentifier, topic.EdgeNodeIdentifier, (string?)null);
        var node = this.metricStates.GetOrAdd(source, static _ => new MetricState<T>());
        if (node.BirthSessionNumber is ulong birth && ReadSessionNumber(metrics) is ulong death && birth != death)
        {
            return;
        }

        node.MetricStatus = SparkplugMetricStatus.Offline;
        this.UpdateLegacyMetricState(source, node);
        foreach (var (key, device) in this.metricStates)
        {
            if (key.DeviceIdentifier is not null && key.GroupIdentifier == topic.GroupIdentifier && key.EdgeNodeIdentifier == topic.EdgeNodeIdentifier)
            {
                device.MetricStatus = SparkplugMetricStatus.Offline;
                this.UpdateLegacyMetricState(key, device);
            }
        }
    }

    /// <summary>
    /// Reads the session number from the birth or death certificate.
    /// </summary>
    /// <param name="metrics">The received metrics.</param>
    /// <returns>The session number if present.</returns>
    private static ulong? ReadSessionNumber(IReadOnlyList<T> metrics)
    {
        foreach (var metric in metrics)
        {
            if (metric.Name == Constants.SessionNumberMetricName)
            {
                return metric.Value switch
                {
                    ulong value => value,
                    long value when value >= 0 => (ulong)value,
                    _ => null
                };
            }
        }

        return null;
    }

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
        this.primaryStateSession = null;
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

            if (this.messageGenerator.SpecificationVersion == SparkplugSpecificationVersion.Version30
                && this.NameSpace == SparkplugNamespace.VersionB
                && this.primaryStateSession is { BirthPublished: true } session
                && topic == session.Birth.Topic)
            {
                // A retained snapshot from the subscription can arrive after the new birth.
                if (args.ApplicationMessage.Retain)
                {
                    return Task.CompletedTask;
                }

                var payload = Encoding.UTF8.GetString(args.ApplicationMessage.Payload.ToArray());
                if (StateMessage.TryReadJson(payload, out var online, out _) && !online)
                {
                    return this.PublishOnlineState(session);
                }

                return Task.CompletedTask;
            }

            if (SparkplugMessageTopic.TryParse(topic, out var topicParsed))
            {
                var data = args.ApplicationMessage.Payload.ToArray();
                return this.OnMessageReceived(topicParsed!, data);
            }

            return Task.CompletedTask;
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

            this.primaryStateSession = null;
            if (this.Options.IsPrimaryApplication)
            {
                var hostIdentifier = this.Options.ScadaHostIdentifier;
                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                var willMessage = this.messageGenerator.GetSparkplugStateMessage(this.NameSpace, hostIdentifier, false, timestamp);
                var birthMessage = this.messageGenerator.GetSparkplugStateMessage(this.NameSpace, hostIdentifier, true, timestamp);
                this.primaryStateSession = new PrimaryStateSession(hostIdentifier, birthMessage);

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
    /// Publishes the initial online STATE for a primary host.
    /// </summary>
    private Task PublishInternal()
    {
        if (this.primaryStateSession is not { } session)
        {
            return Task.CompletedTask;
        }

        this.IncrementLastSequenceNumber();
        return this.PublishOnlineState(session);
    }

    /// <summary>
    /// Publishes the online STATE prepared for the current MQTT connection.
    /// </summary>
    /// <param name="session">The primary host state for the connection.</param>
    private async Task PublishOnlineState(PrimaryStateSession session)
    {
        var cancellationToken = this.Options?.CancellationToken ?? SystemCancellationToken.None;
        await this.statePublicationLock.WaitAsync(cancellationToken);
        try
        {
            if (this.IsRunning && this.IsConnected && ReferenceEquals(this.primaryStateSession, session))
            {
                var result = await this.client.PublishAsync(session.Birth, cancellationToken);
                if (!result.IsSuccess)
                {
                    throw new InvalidOperationException($"Could not publish the online STATE ({result.ReasonCode}).");
                }

                session.BirthPublished = true;
            }
        }
        finally
        {
            this.statePublicationLock.Release();
        }
    }

    /// <summary>
    /// Subscribes the client to the application subscribe topic.
    /// </summary>
    private async Task SubscribeInternal()
    {
        var topic = SparkplugTopicGenerator.GetWildcardNamespaceSubscribeTopic(this.NameSpace);
        await this.client.SubscribeAsync(topic, MqttQualityOfServiceLevel.AtLeastOnce);

        if (this.messageGenerator.SpecificationVersion == SparkplugSpecificationVersion.Version30
            && this.NameSpace == SparkplugNamespace.VersionB
            && this.primaryStateSession is { } session)
        {
            var subscription = await this.client.SubscribeAsync(session.Birth.Topic, MqttQualityOfServiceLevel.AtLeastOnce);
            if (subscription.Items.Single().ResultCode is not (MqttClientSubscribeResultCode.GrantedQoS0 or MqttClientSubscribeResultCode.GrantedQoS1))
            {
                throw new InvalidOperationException($"Could not subscribe to the primary host STATE topic {session.Birth.Topic}.");
            }
        }
    }

    /// <summary>
    /// Updates the metric state.
    /// </summary>
    /// <param name="metricState">The metric state.</param>
    private void UpdateMetricState(SparkplugMetricStatus metricState)
    {
        foreach (var (key, state) in this.metricStates)
        {
            if (key.DeviceIdentifier is null)
            {
                state.MetricStatus = metricState;
            }
        }
    }

    /// <summary>
    /// Sends the offline state message when intentionally stopping the application.
    /// </summary>
    private async Task SendOfflineStateMessage()
    {
        await this.statePublicationLock.WaitAsync();
        try
        {
            if (this.IsConnected && this.primaryStateSession is { } session)
            {
                var offlineStateMessage = this.messageGenerator.GetSparkplugStateMessage(this.NameSpace, session.HostIdentifier, false);
                var result = await this.client.PublishAsync(offlineStateMessage);
                if (!result.IsSuccess)
                {
                    throw new InvalidOperationException($"Could not publish the offline STATE ({result.ReasonCode}).");
                }
            }
        }
        finally
        {
            this.primaryStateSession = null;
            this.statePublicationLock.Release();
        }
    }
}
