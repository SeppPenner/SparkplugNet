// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugApplicationBase.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class that handles a Sparkplug application.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Core.Application;

using SparkplugNet.Core.Data;

public interface ISparkplugApplication
{

}

/// <inheritdoc cref="SparkplugBase{T}"/>
/// <summary>
/// A class that handles a Sparkplug application.
/// </summary>
/// <seealso cref="SparkplugBase{T}"/>
public abstract class SparkplugApplicationBase<T> : SparkplugBase<T> where T : IMetric, new()
{
    /// <summary>
    /// The options.
    /// </summary>
    protected SparkplugApplicationOptions? options { private set; get; }

    /// <inheritdoc cref="SparkplugBase{T}"/>
    /// <summary>
    /// Initializes a new instance of the <see cref="SparkplugApplicationBase{T}"/> class.
    /// </summary>
    /// <param name="knownMetrics">The metric names.</param>
    /// <param name="logger">The logger.</param>
    /// <seealso cref="SparkplugBase{T}"/>
    public SparkplugApplicationBase(List<T> knownMetrics, ILogger? logger = null) : base(knownMetrics, logger)
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
    /// Gets or sets the callback for the device data received event.
    /// </summary>
    public Action<string, string, string, T>? OnDeviceDataReceived { get; set; } = null;

    /// <summary>
    /// Gets or sets the callback for the node data received event.
    /// </summary>
    public Action<string, string, T>? OnNodeDataReceived { get; set; } = null;

    /// <summary>
    /// Starts the Sparkplug application.
    /// </summary>
    /// <param name="applicationOptions">The application option.</param>
    /// <exception cref="ArgumentNullException">The options are null.</exception>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    public async Task Start(SparkplugApplicationOptions applicationOptions)
    {
        // Storing the options.
        this.options = applicationOptions;

        if (this.options is null)
        {
            throw new ArgumentNullException(nameof(this.options), "The options aren't set properly.");
        }

        // Clear states.
        this.NodeStates.Clear();
        this.DeviceStates.Clear();

        // Add handlers.
        this.AddDisconnectedHandler();
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
    public async Task PublishNodeCommand(List<T> metrics, string groupIdentifier, string edgeNodeIdentifier)
    {
        if (this.options is null)
        {
            throw new ArgumentNullException(nameof(this.options), "The options aren't set properly.");
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
    protected abstract Task PublishNodeCommandMessage(List<T> metrics, string groupIdentifier, string edgeNodeIdentifier);

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
    public async Task PublishDeviceCommand(List<T> metrics, string groupIdentifier, string edgeNodeIdentifier, string deviceIdentifier)
    {
        if (this.options is null)
        {
            throw new ArgumentNullException(nameof(this.options), "The options aren't set properly.");
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
    protected abstract Task PublishDeviceCommandMessage(List<T> metrics, string groupIdentifier, string edgeNodeIdentifier, string deviceIdentifier);

    /// <summary>
    /// Adds the disconnected handler and the reconnect functionality to the client.
    /// </summary>
    /// <exception cref="ArgumentNullException">The options are null.</exception>
    private void AddDisconnectedHandler()
    {
        this.Client.DisconnectedAsync += this.OnClientDisconnected;
    }

    /// <summary>
    /// Handles the client disconnection.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">The options are null.</exception>
    private async Task OnClientDisconnected(MqttClientDisconnectedEventArgs args)
    {
        if (this.options is null)
        {
            throw new ArgumentNullException(nameof(this.options), "The options aren't set properly.");
        }

        // Set all metrics to stale.
        this.UpdateMetricState(SparkplugMetricStatus.Offline);

        // Invoke disconnected callback.
        this.OnDisconnected?.Invoke();

        // Wait until the disconnect interval is reached.
        await Task.Delay(this.options.ReconnectInterval);

        // Connect, subscribe to incoming messages and send a state message.
        await this.ConnectInternal();
        this.UpdateMetricState(SparkplugMetricStatus.Online);
        await this.SubscribeInternal();
        await this.PublishInternal();
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
        var topic = args.ApplicationMessage.Topic;

        // Skip the STATE messages as they're UTF-8 encoded.
        if (topic.Contains(SparkplugMessageType.StateMessage.GetDescription()))
        {
            return Task.CompletedTask;
        }

        return this.OnMessageReceived(topic, args.ApplicationMessage.Payload);
    }

    /// <summary>
    /// Called when [message received].
    /// </summary>
    /// <param name="topic">The topic.</param>
    /// <param name="payload">The payload.</param>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    protected abstract Task OnMessageReceived(string topic, byte[] payload);

    /// <summary>
    /// Connects the Sparkplug application to the MQTT broker.
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

        // Get the will message.
        var willMessage = SparkplugMessageGenerator.GetSparkplugStateMessage(
            this.NameSpace,
            this.options.ScadaHostIdentifier,
            false);

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

        if (this.options.IsPrimaryApplication)
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

        // Only send state messages for the primary application.
        if (this.options.IsPrimaryApplication)
        {
            // Get the online message.
            var onlineMessage = SparkplugMessageGenerator.GetSparkplugStateMessage(
                this.NameSpace,
                this.options.ScadaHostIdentifier,
                true);

            // Increment the sequence number.
            this.IncrementLastSequenceNumber();

            // Publish message.
            this.options.CancellationToken ??= CancellationToken.None;
            await this.Client.PublishAsync(onlineMessage, this.options.CancellationToken.Value);
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
