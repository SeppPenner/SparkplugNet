// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugApplicationBase.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class that handles a Sparkplug application.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Core.Application;

using MQTTnet.Internal;

/// <inheritdoc cref="SparkplugBase{T}"/>
/// <summary>
/// A class that handles a Sparkplug application.
/// </summary>
/// <seealso cref="SparkplugBase{T}"/>
public abstract partial class SparkplugApplicationBase<T> : SparkplugBase<T> where T : IMetric, new()
{
    /// <summary>
    /// The options.
    /// </summary>
    public SparkplugApplicationOptions? Options { private set; get; }

    /// <inheritdoc cref="SparkplugBase{T}"/>
    /// <summary>
    /// Initializes a new instance of the <see cref="SparkplugApplicationBase{T}"/> class.
    /// </summary>
    /// <param name="knownMetrics">The metric names.</param>
    /// <param name="logger">The logger.</param>
    /// <seealso cref="SparkplugBase{T}"/>
    public SparkplugApplicationBase(IEnumerable<T> knownMetrics, ILogger? logger = null) : base(knownMetrics, logger)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SparkplugApplicationBase{T}"/> class.
    /// </summary>
    /// <param name="knownMetricsStorage">The metric names.</param>
    /// <param name="logger">The logger.</param>
    /// /// <seealso cref="SparkplugBase{T}"/>
    public SparkplugApplicationBase(KnownMetricStorage knownMetricsStorage, ILogger? logger = null) : base(knownMetricsStorage, logger)
    {
    }

    /// <summary>
    /// Gets the node states.
    /// </summary>
    public ConcurrentDictionary<string, MetricState<T>> NodeStates { get; } = new();

    /// <summary>
    /// Gets the node states.
    /// </summary>
    public ConcurrentDictionary<string, MetricState<T>> DeviceStates { get; } = new();

    /// <summary>
    /// Starts the Sparkplug application.
    /// </summary>
    /// <param name="applicationOptions">The application option.</param>
    /// <exception cref="ArgumentNullException">The options are null.</exception>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
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
        this.AddEventHandler();
        this.AddMessageReceivedHandler();

        // Connect, subscribe to incoming messages and send a state message.
        await this.ConnectInternal();
        await this.SubscribeInternal();
        await this.PublishInternal();
    }

    /// <summary>
    /// Stops the Sparkplug application.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    public async Task Stop()
    {
        this.IsRunning = false;
        await this.Client.DisconnectAsync();
    }

    /// <summary>
    /// Publishes a node command.
    /// </summary>
    /// <param name="metrics">The metrics.</param>
    /// <param name="groupIdentifier">The group identifier.</param>
    /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
    /// <exception cref="ArgumentNullException">The options are null.</exception>
    /// <exception cref="Exception">The MQTT client is not connected or an invalid metric type was specified.</exception>
    /// <exception cref="ArgumentException">The group or edge node identifier is invalid.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The namespace is out of range.</exception>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    public async Task PublishNodeCommand(IEnumerable<T> metrics, string groupIdentifier, string edgeNodeIdentifier)
    {
        if (this.Options is null)
        {
            throw new ArgumentNullException(nameof(this.Options), "The options aren't set properly.");
        }

        if (!this.Client.IsConnected)
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
    /// Publishes a node command message.
    /// </summary>
    /// <param name="metrics">The metrics.</param>
    /// <param name="groupIdentifier">The group identifier.</param>
    /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
    /// <exception cref="ArgumentNullException">The options are null.</exception>
    /// <exception cref="Exception">An invalid metric type was specified.</exception>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    protected abstract Task PublishNodeCommandMessage(IEnumerable<T> metrics, string groupIdentifier, string edgeNodeIdentifier);

    /// <summary>
    /// Publishes a device command.
    /// </summary>
    /// <param name="metrics">The metrics.</param>
    /// <param name="groupIdentifier">The group identifier.</param>
    /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
    /// <param name="deviceIdentifier">The device identifier.</param>
    /// <exception cref="ArgumentNullException">The options are null.</exception>
    /// <exception cref="Exception">The MQTT client is not connected or an invalid metric type was specified.</exception>
    /// <exception cref="ArgumentException">The group or edge node or device identifier is invalid.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The namespace is out of range.</exception>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    public async Task PublishDeviceCommand(IEnumerable<T> metrics, string groupIdentifier, string edgeNodeIdentifier, string deviceIdentifier)
    {
        if (this.Options is null)
        {
            throw new ArgumentNullException(nameof(this.Options), "The options aren't set properly.");
        }

        if (!this.Client.IsConnected)
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
    /// Publishes a version A device command message.
    /// </summary>
    /// <param name="metrics">The metrics.</param>
    /// <param name="groupIdentifier">The group identifier.</param>
    /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
    /// <param name="deviceIdentifier">The device identifier.</param>
    /// <exception cref="ArgumentNullException">The options are null.</exception>
    /// <exception cref="Exception">An invalid metric type was specified.</exception>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    protected abstract Task PublishDeviceCommandMessage(IEnumerable<T> metrics, string groupIdentifier, string edgeNodeIdentifier, string deviceIdentifier);

    /// <summary>
    /// Adds the event handler and the reconnect functionality to the client.
    /// </summary>
    /// <exception cref="ArgumentNullException">The options are null.</exception>
    private void AddEventHandler()
    {
        this.Client.DisconnectedAsync += this.OnClientDisconnectedAsync;
        this.Client.ConnectedAsync += this.OnClientConnectedAsync;
    }

    /// <summary>
    /// Handles the client disconnection event.
    /// </summary>
    /// <param name="arg">The <see cref="MqttClientConnectedEventArgs"/> instance containing the event data.</param>
    protected virtual async Task OnClientConnectedAsync(MqttClientConnectedEventArgs arg)
    {
        try
        {
            await this.FireConnectedAsync();
        }
        catch (Exception ex)
        {
            this.Logger?.Error(ex, "OnClientConnectedAsync");
            await Task.FromException(ex);
        }
    }

    /// <summary>
    /// Handles the client disconnection.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">The options are null.</exception>
    protected virtual async Task OnClientDisconnectedAsync(MqttClientDisconnectedEventArgs args)
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
            await this.FireDisconnectedAsync();

            this.Logger?.Warning("Connection lost, retrying to connect in {@reconnectInterval}", this.Options.ReconnectInterval);

            // Wait until the disconnect interval is reached.
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
            this.Logger?.Error(ex, "OnClientDisconnectedAsync");
            await Task.FromException(ex);
        }
    }

    /// <summary>
    /// Adds the message received handler to handle incoming messages.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">The namespace is out of range.</exception>
    private void AddMessageReceivedHandler()
    {
        this.Client.ApplicationMessageReceivedAsync += this.OnApplicationMessageReceived;
    }

    /// <summary>
    /// Handles the message received handler.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The namespace is out of range.</exception>
    private Task OnApplicationMessageReceived(MqttApplicationMessageReceivedEventArgs args)
    {
        try
        {
            var topic = args.ApplicationMessage.Topic;

            // Skip the STATE messages as they're UTF-8 encoded.
            if (topic.Contains(SparkplugMessageType.StateMessage.GetDescription()))
            {
                return Task.CompletedTask;
            }

            return this.OnMessageReceived(SparkplugMessageTopic.Parse(topic), args.ApplicationMessage.Payload);
        }
        catch (Exception ex)
        {
            this.Logger?.Error(ex, "OnApplicationMessageReceived");
            return Task.FromException(ex);
        }
    }

    /// <summary>
    /// Called when [message received].
    /// </summary>
    /// <param name="topic">The topic.</param>
    /// <param name="payload">The payload.</param>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    protected abstract Task OnMessageReceived(SparkplugMessageTopic topic, byte[] payload);

    /// <summary>
    /// Connects the Sparkplug application to the MQTT broker.
    /// </summary>
    /// <exception cref="ArgumentNullException">The options are null.</exception>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
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
            var willMessage = SparkplugMessageGenerator.GetSparkplugStateMessage(
                this.NameSpace,
                this.Options.ScadaHostIdentifier,
                false);

            // Build up the MQTT client and connect.
            this.Options.CancellationToken ??= CancellationToken.None;

            var builder = new MqttClientOptionsBuilder()
                .WithClientId(this.Options.ClientId)
                .WithCredentials(this.Options.UserName, this.Options.Password)
                .WithCleanSession(false)
                .WithProtocolVersion(MqttProtocolVersion.V311);

            if (this.Options.UseTls)
            {
                builder.WithTls();
            }

            if (this.Options.WebSocketParameters is null)
            {
                builder.WithTcpServer(this.Options.BrokerAddress, this.Options.Port);
            }
            else
            {
                builder.WithWebSocketServer(this.Options.BrokerAddress, this.Options.WebSocketParameters);
            }

            if (this.Options.ProxyOptions != null)
            {
                builder.WithProxy(
                    this.Options.ProxyOptions.Address,
                    this.Options.ProxyOptions.Username,
                    this.Options.ProxyOptions.Password,
                    this.Options.ProxyOptions.Domain,
                    this.Options.ProxyOptions.BypassOnLocal);
            }

            if (this.Options.IsPrimaryApplication)
            {
                builder.WithWillContentType(willMessage.ContentType);
                builder.WithWillCorrelationData(willMessage.CorrelationData);
                builder.WithWillDelayInterval(willMessage.MessageExpiryInterval);
                builder.WithWillPayload(willMessage.Payload);
                builder.WithWillPayloadFormatIndicator(willMessage.PayloadFormatIndicator);
                builder.WithWillQualityOfServiceLevel(willMessage.QualityOfServiceLevel);
                builder.WithWillResponseTopic(willMessage.ResponseTopic);
                builder.WithWillRetain(willMessage.Retain);
                builder.WithWillTopic(willMessage.Topic);

                if (willMessage.UserProperties != null)
                {
                    foreach (var userProperty in willMessage.UserProperties)
                    {
                        builder.WithWillUserProperty(userProperty.Name, userProperty.Value);
                    }
                }
            }

            this.ClientOptions = builder.Build();
            await this.Client.ConnectAsync(this.ClientOptions, this.Options.CancellationToken.Value);

        }
        catch (Exception ex)
        {
            this.Logger?.Error(ex, "ConnectInternal");
            await Task.FromException(ex);
        }
    }

    /// <summary>
    /// Publishes data to the MQTT broker.
    /// </summary>
    /// <exception cref="ArgumentNullException">The options are null.</exception>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
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
            var onlineMessage = SparkplugMessageGenerator.GetSparkplugStateMessage(
                this.NameSpace,
                this.Options.ScadaHostIdentifier,
                true);

            // Increment the sequence number.
            this.IncrementLastSequenceNumber();

            // Publish message.
            this.Options.CancellationToken ??= CancellationToken.None;
            await this.Client.PublishAsync(onlineMessage, this.Options.CancellationToken.Value);
        }
    }

    /// <summary>
    /// Subscribes the client to the application subscribe topic.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    private async Task SubscribeInternal()
    {
        var topic = SparkplugTopicGenerator.GetWildcardNamespaceSubscribeTopic(this.NameSpace);
        await this.Client.SubscribeAsync(topic, (MqttQualityOfServiceLevel)SparkplugQualityOfServiceLevel.AtLeastOnce);
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
}
