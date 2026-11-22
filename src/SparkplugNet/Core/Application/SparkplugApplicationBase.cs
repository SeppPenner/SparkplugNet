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
    /// Tracks the current broker for each node and its devices.
    /// </summary>
    private readonly ConcurrentDictionary<(string Group, string Node), BrokerConnection> sourceBrokers = new();

    /// <summary>
    /// The MQTT connections for the current host run, with the configured broker first.
    /// </summary>
    private BrokerConnection[] brokers = [];

    /// <summary>
    /// Cancels reconnection when the current host run stops.
    /// </summary>
    private CancellationTokenSource? stopping;

    /// <summary>
    /// Holds the connection and STATE publication for a single broker.
    /// </summary>
    /// <param name="client">The broker's MQTT client.</param>
    /// <param name="endpoint">The broker endpoint.</param>
    private sealed class BrokerConnection(IMqttClient client, SparkplugBrokerEndpoint endpoint)
    {
        /// <summary>The broker's MQTT client.</summary>
        internal IMqttClient Client { get; } = client;

        /// <summary>The broker endpoint.</summary>
        internal SparkplugBrokerEndpoint Endpoint { get; } = endpoint;

        /// <summary>Serializes online and offline STATE publications.</summary>
        internal SemaphoreSlim StateLock { get; } = new(1);

        /// <summary>Serializes attempts to connect and subscribe to this broker.</summary>
        internal SemaphoreSlim ConnectionLock { get; } = new(1);

        /// <summary>The primary host STATE for the current connection.</summary>
        internal volatile PrimaryStateSession? Session;

        /// <summary>The registered disconnect handler.</summary>
        internal Func<MqttClientDisconnectedEventArgs, Task>? Disconnected;

        /// <summary>The registered receive handler.</summary>
        internal Func<MqttApplicationMessageReceivedEventArgs, Task>? Received;
    }

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

    /// <inheritdoc />
    public override bool IsConnected => this.brokers.Any(broker => broker.Client.IsConnected);

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
        if (applicationOptions.MessageReorderTimeout is { } timeout
            && (timeout < TimeSpan.FromMilliseconds(1) || timeout > TimeSpan.FromMilliseconds(int.MaxValue)))
        {
            throw new ArgumentOutOfRangeException(nameof(applicationOptions), "The message reorder timeout is out of range.");
        }

        this.IsRunning = true;
        this.Options = applicationOptions;

        // Clear states.
        this.NodeStates.Clear();
        this.DeviceStates.Clear();
        this.metricStates.Clear();
        this.sourceBrokers.Clear();
        this.ResetReceivedMessageOrder();

        var first = new BrokerConnection(this.client, new(applicationOptions.BrokerAddress, applicationOptions.Port));
        var factory = new MqttClientFactory();
        this.brokers = [first, .. applicationOptions.AdditionalBrokers.Select(endpoint => new BrokerConnection(factory.CreateMqttClient(), endpoint))];
        this.stopping = CancellationTokenSource.CreateLinkedTokenSource(applicationOptions.CancellationToken ?? SystemCancellationToken.None);
        foreach (var broker in this.brokers)
        {
            this.AddEventHandlers(broker);
        }
        try
        {
            foreach (var broker in this.brokers)
            {
                await this.EstablishConnection(broker, this.stopping.Token);
            }
        }
        catch
        {
            this.IsRunning = false;
            this.ResetReceivedMessageOrder();
            await this.StopBrokers();
            throw;
        }
    }

    /// <summary>
    /// Stops the Sparkplug application.
    /// </summary>
    public async Task Stop()
    {
        this.IsRunning = false;
        this.ResetReceivedMessageOrder();
        try
        {
            await this.StopBrokers();
        }
        finally
        {
            this.MarkSourcesOffline();
            await this.FireDisconnected();
        }
    }

    /// <summary>
    /// Publishes offline STATE for every connected broker before a caller-managed transport shutdown.
    /// Call <see cref="Stop"/> for the usual shutdown with MQTT DISCONNECT.
    /// </summary>
    public Task PublishOfflineState() => Task.WhenAll(this.brokers.Select(this.SendOfflineStateMessage));

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

        if (!this.IsConnected)
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

        if (!this.IsConnected)
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
    /// <param name="timestamp">The UTC Unix timestamp in milliseconds from the payload, if available.</param>
    /// <returns>The metrics to deliver to event handlers.</returns>
    private protected List<T> ProcessReceivedMetrics(
        SparkplugMessageTopic topic, IReadOnlyList<T> metrics, out T? sessionNumberMetric, ulong? timestamp = null)
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

        this.UpdateReceivedMetricState(topic, source, filteredMetrics, timestamp);
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
    /// <param name="timestamp">The payload timestamp in UTC Unix milliseconds.</param>
    private void UpdateReceivedMetricState(
        SparkplugMessageTopic topic,
        (string GroupIdentifier, string EdgeNodeIdentifier, string? DeviceIdentifier) source,
        IReadOnlyList<T> metrics,
        ulong? timestamp)
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
            SetMetricStatus(state, SparkplugMetricStatus.Offline, timestamp ?? GetUtcTimestamp());
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

        if (state.MetricStatus != SparkplugMetricStatus.Online)
        {
            SetMetricStatus(state, SparkplugMetricStatus.Online, timestamp ?? GetUtcTimestamp());
        }
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

        var timestamp = GetUtcTimestamp();
        SetMetricStatus(node, SparkplugMetricStatus.Offline, timestamp);
        this.UpdateLegacyMetricState(source, node);
        foreach (var (key, device) in this.metricStates)
        {
            if (key.DeviceIdentifier is not null && key.GroupIdentifier == topic.GroupIdentifier && key.EdgeNodeIdentifier == topic.EdgeNodeIdentifier)
            {
                SetMetricStatus(device, SparkplugMetricStatus.Offline, timestamp);
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
    protected virtual Task OnClientDisconnected(MqttClientDisconnectedEventArgs args) =>
        args.ClientWasConnected && this.brokers.FirstOrDefault() is { } broker
            ? this.HandleBrokerDisconnection(broker)
            : Task.CompletedTask;

    /// <summary>
    /// Marks the sources on a disconnected broker stale and reestablishes that broker's connection.
    /// </summary>
    /// <param name="broker">The disconnected broker.</param>
    private async Task HandleBrokerDisconnection(BrokerConnection broker)
    {
        if (!this.IsRunning || broker.Client.IsConnected || this.stopping is not { } run)
        {
            return;
        }

        broker.Session = null;
        this.MarkSourcesOffline(broker);
        var options = this.Options!;
        var cancellationToken = run.Token;
        try
        {
            await this.FireDisconnected();
            while (this.IsRunning && this.stopping == run && !broker.Client.IsConnected)
            {
                await Task.Delay(options.ReconnectInterval, cancellationToken);
                try
                {
                    await this.EstablishConnection(broker, cancellationToken);
                }
                catch (MQTTnet.Exceptions.MqttCommunicationException exception) when (!run.IsCancellationRequested)
                {
                    this.KnownMetricsStorage.Logger?.LogDebug(exception, "Could not reconnect to the Sparkplug broker.");
                }
            }
        }
        catch (OperationCanceledException) when (run.IsCancellationRequested)
        {
        }
    }

    /// <summary>
    /// Discards any messages buffered for the previous broker connection.
    /// </summary>
    private protected virtual void ResetReceivedMessageOrder()
    {
    }

    /// <summary>
    /// Discards messages buffered for the given edge node.
    /// </summary>
    /// <param name="group">The group identifier.</param>
    /// <param name="node">The edge node identifier.</param>
    private protected virtual void ResetReceivedMessageOrder(string group, string node)
    {
    }

    /// <summary>
    /// Gets the MQTT client to which an edge node is connected.
    /// </summary>
    /// <param name="group">The group identifier.</param>
    /// <param name="node">The edge node identifier.</param>
    /// <returns>The client for publishing commands.</returns>
    private protected IMqttClient GetCommandClient(string group, string node) =>
        this.sourceBrokers.TryGetValue((group, node), out var broker)
            ? broker.Client
            : this.brokers.FirstOrDefault(candidate => candidate.Client.IsConnected)?.Client ?? this.client;

    /// <summary>
    /// Adds the event handlers to the client.
    /// </summary>
    /// <param name="broker">The broker connection.</param>
    private void AddEventHandlers(BrokerConnection broker)
    {
        broker.Disconnected = broker.Client == this.client
            ? this.OnClientDisconnected
            : args => args.ClientWasConnected ? this.HandleBrokerDisconnection(broker) : Task.CompletedTask;
        broker.Received = args => this.OnApplicationMessageReceived(broker, args);
        broker.Client.DisconnectedAsync += broker.Disconnected;
        broker.Client.ConnectedAsync += this.OnClientConnected;
        broker.Client.ApplicationMessageReceivedAsync += broker.Received;
    }

    /// <summary>
    /// Removes the event handlers from the client.
    /// </summary>
    /// <param name="broker">The broker connection.</param>
    private void RemoveEventHandlers(BrokerConnection broker)
    {
        broker.Client.DisconnectedAsync -= broker.Disconnected;
        broker.Client.ConnectedAsync -= this.OnClientConnected;
        broker.Client.ApplicationMessageReceivedAsync -= broker.Received;
    }

    /// <summary>
    /// Handles the message received handler.
    /// </summary>
    /// <param name="broker">The broker that received the message.</param>
    /// <param name="args">The arguments.</param>
    private Task OnApplicationMessageReceived(BrokerConnection broker, MqttApplicationMessageReceivedEventArgs args)
    {
        try
        {
            var topic = args.ApplicationMessage.Topic;

            if (this.messageGenerator.SpecificationVersion == SparkplugSpecificationVersion.Version30
                && this.NameSpace == SparkplugNamespace.VersionB
                && broker.Session is { BirthPublished: true } session
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
                    return this.PublishOnlineState(broker, session);
                }

                return Task.CompletedTask;
            }

            if (SparkplugMessageTopic.TryParse(topic, out var topicParsed))
            {
                var source = (topicParsed!.GroupIdentifier, topicParsed.EdgeNodeIdentifier);
                if (topicParsed.MessageType == SparkplugMessageType.NodeBirth)
                {
                    this.sourceBrokers[source] = broker;
                }
                else if (this.sourceBrokers.TryGetValue(source, out var previous) && previous != broker)
                {
                    return Task.CompletedTask;
                }

                var data = args.ApplicationMessage.Payload.ToArray();
                return this.OnMessageReceived(topicParsed, data);
            }

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            return Task.FromException(ex);
        }
    }

    /// <summary>
    /// Connects, subscribes and publishes the initial STATE for a broker.
    /// </summary>
    /// <param name="broker">The broker connection.</param>
    /// <param name="cancellationToken">Cancels connection establishment.</param>
    private async Task EstablishConnection(BrokerConnection broker, SystemCancellationToken cancellationToken)
    {
        await broker.ConnectionLock.WaitAsync(cancellationToken);
        try
        {
            if (!broker.Client.IsConnected)
            {
                await this.ConnectInternal(broker, cancellationToken);
                await this.SubscribeInternal(broker);
                if (broker.Session is { } session)
                {
                    this.IncrementLastSequenceNumber();
                    await this.PublishOnlineState(broker, session);
                }
            }
        }
        finally
        {
            broker.ConnectionLock.Release();
        }
    }

    /// <summary>
    /// Connects the Sparkplug application to the MQTT broker.
    /// </summary>
    /// <param name="broker">The broker connection.</param>
    /// <param name="cancellationToken">Cancels connection establishment.</param>
    /// <exception cref="ArgumentNullException">Thrown if the options are null.</exception>
    private async Task ConnectInternal(BrokerConnection broker, SystemCancellationToken cancellationToken)
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
                builder.WithTcpServer(broker.Endpoint.Address, broker.Endpoint.Port);
            }
            else
            {
                builder.WithWebSocketServer(options => 
                    options.WithCookieContainer(this.Options.MqttWebSocketOptions.CookieContainer)
                    .WithCookieContainer(this.Options.MqttWebSocketOptions.Credentials)
                    .WithProxyOptions(this.Options.MqttWebSocketOptions.ProxyOptions)
                    .WithRequestHeaders(this.Options.MqttWebSocketOptions.RequestHeaders)
                    .WithSubProtocols(this.Options.MqttWebSocketOptions.SubProtocols)
                    .WithUri(broker.Endpoint.Address)
                    .WithKeepAliveInterval(this.Options.MqttWebSocketOptions.KeepAliveInterval)
                    .WithUseDefaultCredentials(this.Options.MqttWebSocketOptions.UseDefaultCredentials)
                );
            }

            broker.Session = null;
            if (this.Options.IsPrimaryApplication)
            {
                var hostIdentifier = this.Options.ScadaHostIdentifier;
                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                var willMessage = this.messageGenerator.GetSparkplugStateMessage(this.NameSpace, hostIdentifier, false, timestamp);
                var birthMessage = this.messageGenerator.GetSparkplugStateMessage(this.NameSpace, hostIdentifier, true, timestamp);
                broker.Session = new PrimaryStateSession(hostIdentifier, birthMessage);

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

            var clientOptions = builder.Build();
            if (broker.Client == this.client)
            {
                this.ClientOptions = clientOptions;
            }

            await broker.Client.ConnectAsync(clientOptions, cancellationToken);
        }
        catch (Exception ex)
        {
            await Task.FromException(ex);
        }
    }

    /// <summary>
    /// Publishes the online STATE prepared for the current MQTT connection.
    /// </summary>
    /// <param name="broker">The broker connection.</param>
    /// <param name="session">The primary host state for the connection.</param>
    private async Task PublishOnlineState(BrokerConnection broker, PrimaryStateSession session)
    {
        var cancellationToken = this.Options?.CancellationToken ?? SystemCancellationToken.None;
        await broker.StateLock.WaitAsync(cancellationToken);
        try
        {
            if (this.IsRunning && broker.Client.IsConnected && ReferenceEquals(broker.Session, session))
            {
                var wasPublished = session.BirthPublished;
                // Another publisher can respond to STATE before MQTT acknowledges our publication.
                session.BirthPublished = true;
                try
                {
                    var result = await broker.Client.PublishAsync(session.Birth, cancellationToken);
                    if (!result.IsSuccess)
                    {
                        throw new InvalidOperationException($"Could not publish the online STATE ({result.ReasonCode}).");
                    }
                }
                catch
                {
                    session.BirthPublished = wasPublished;
                    throw;
                }
            }
        }
        finally
        {
            broker.StateLock.Release();
        }
    }

    /// <summary>
    /// Subscribes the client to the application subscribe topic.
    /// </summary>
    /// <param name="broker">The broker connection.</param>
    private async Task SubscribeInternal(BrokerConnection broker)
    {
        var topic = SparkplugTopicGenerator.GetWildcardNamespaceSubscribeTopic(this.NameSpace);
        await broker.Client.SubscribeAsync(topic, MqttQualityOfServiceLevel.AtLeastOnce);

        if (this.messageGenerator.SpecificationVersion == SparkplugSpecificationVersion.Version30
            && this.NameSpace == SparkplugNamespace.VersionB
            && broker.Session is { } session)
        {
            var subscription = await broker.Client.SubscribeAsync(session.Birth.Topic, MqttQualityOfServiceLevel.AtLeastOnce);
            if (subscription.Items.Single().ResultCode is not (MqttClientSubscribeResultCode.GrantedQoS0 or MqttClientSubscribeResultCode.GrantedQoS1))
            {
                throw new InvalidOperationException($"Could not subscribe to the primary host STATE topic {session.Birth.Topic}.");
            }
        }
    }

    /// <summary>
    /// Marks cached nodes and devices offline when their broker connection is lost.
    /// </summary>
    /// <param name="broker">The disconnected broker, or null when stopping the host.</param>
    private void MarkSourcesOffline(BrokerConnection? broker = null)
    {
        if (broker is not null)
        {
            foreach (var (source, connection) in this.sourceBrokers)
            {
                if (connection == broker)
                {
                    this.ResetReceivedMessageOrder(source.Group, source.Node);
                }
            }
        }

        var timestamp = GetUtcTimestamp();
        foreach (var (source, state) in this.metricStates)
        {
            if (broker is null || this.brokers.Length == 1
                || (this.sourceBrokers.TryGetValue((source.GroupIdentifier, source.EdgeNodeIdentifier), out var connection) && connection == broker))
            {
                SetMetricStatus(state, SparkplugMetricStatus.Offline, timestamp);
            }
        }
    }

    /// <summary>
    /// Updates a source's status and the timestamp applicable to its cached metrics.
    /// </summary>
    /// <param name="state">The source state.</param>
    /// <param name="status">The new status.</param>
    /// <param name="timestamp">The UTC Unix timestamp in milliseconds.</param>
    private static void SetMetricStatus(MetricState<T> state, SparkplugMetricStatus status, ulong timestamp)
    {
        state.StatusTimestamp = timestamp;
        state.MetricStatus = status;
    }

    /// <summary>
    /// Gets the current host UTC time in Unix milliseconds.
    /// </summary>
    /// <returns>The host timestamp.</returns>
    private static ulong GetUtcTimestamp() => (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    /// <summary>
    /// Sends the offline state message when intentionally stopping the application.
    /// </summary>
    /// <param name="broker">The broker connection.</param>
    private async Task SendOfflineStateMessage(BrokerConnection broker)
    {
        await broker.StateLock.WaitAsync();
        try
        {
            if (broker.Client.IsConnected && broker.Session is { } session)
            {
                var offlineStateMessage = this.messageGenerator.GetSparkplugStateMessage(this.NameSpace, session.HostIdentifier, false);
                var result = await broker.Client.PublishAsync(offlineStateMessage);
                if (!result.IsSuccess)
                {
                    throw new InvalidOperationException($"Could not publish the offline STATE ({result.ReasonCode}).");
                }
            }
        }
        finally
        {
            broker.Session = null;
            broker.StateLock.Release();
        }
    }

    /// <summary>
    /// Ends all connections and releases the additional MQTT clients.
    /// </summary>
    private async Task StopBrokers()
    {
        var run = this.stopping;
        run?.Cancel();
        var connections = Interlocked.Exchange(ref this.brokers, []);
        await Task.WhenAll(connections.Select(async broker =>
        {
            this.RemoveEventHandlers(broker);
            try
            {
                await this.SendOfflineStateMessage(broker);
            }
            finally
            {
                try
                {
                    await broker.Client.DisconnectAsync();
                }
                finally
                {
                    if (broker.Client != this.client)
                    {
                        broker.Client.Dispose();
                    }
                }
            }
        }));
    }
}
