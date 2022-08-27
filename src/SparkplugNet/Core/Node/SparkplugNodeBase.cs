// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugNodeBase.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class that handles a Sparkplug node.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Core.Node;

using SparkplugNet.Core.Data;

/// <inheritdoc cref="SparkplugBase{T}"/>
/// <summary>
/// A class that handles a Sparkplug node.
/// </summary>
/// <seealso cref="SparkplugBase{T}"/>
public abstract partial class SparkplugNodeBase<T> : SparkplugBase<T> where T : IMetric, new()
{
    /// <summary>
    /// The options.
    /// </summary>
    protected SparkplugNodeOptions? options { get; private set; }

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

        return await this.PublishMessage(metrics);
    }

    /// <summary>
    /// Publishes metrics for a node.
    /// </summary>
    /// <param name="metrics">The metrics.</param>
    /// <exception cref="ArgumentNullException">The options are null.</exception>
    /// <exception cref="Exception">An invalid metric type was specified.</exception>
    /// <returns>A <see cref="MqttClientPublishResult"/>.</returns>
    protected abstract Task<MqttClientPublishResult> PublishMessage(List<T> metrics);

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
    }

    /// <summary>
    /// Adds the message received handler to handle incoming messages.
    /// </summary>
    /// <exception cref="InvalidCastException">The metric cast is invalid.</exception>
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

        // Handle the STATE message before anything else as they're UTF-8 encoded.
        if (topic.Contains(SparkplugMessageType.StateMessage.GetDescription()))
        {
            this.StatusMessageReceived?.Invoke(Encoding.UTF8.GetString(args.ApplicationMessage.Payload));
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

        // Add the will message data.
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
        await this.Client.SubscribeAsync(nodeCommandSubscribeTopic, (MqttQualityOfServiceLevel)SparkplugQualityOfServiceLevel.AtLeastOnce);

        var deviceCommandSubscribeTopic = SparkplugTopicGenerator.GetWildcardDeviceCommandSubscribeTopic(this.NameSpace, this.options.GroupIdentifier, this.options.EdgeNodeIdentifier);
        await this.Client.SubscribeAsync(deviceCommandSubscribeTopic, (MqttQualityOfServiceLevel)SparkplugQualityOfServiceLevel.AtLeastOnce);

        var stateSubscribeTopic = SparkplugTopicGenerator.GetStateSubscribeTopic(this.options.ScadaHostIdentifier);
        await this.Client.SubscribeAsync(stateSubscribeTopic, (MqttQualityOfServiceLevel)SparkplugQualityOfServiceLevel.AtLeastOnce);
    }
}
