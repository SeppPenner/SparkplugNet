// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugNode.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class that handles a Sparkplug node.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.VersionB;

/// <inheritdoc cref="SparkplugNodeBase{T}"/>
/// <summary>
///   A class that handles a Sparkplug node.
/// </summary>
/// <seealso cref="SparkplugNodeBase{T}"/>
public sealed class SparkplugNode : SparkplugNodeBase<Metric>
{
    /// <inheritdoc cref="SparkplugNodeBase{T}"/>
    /// <summary>
    /// Initializes a new instance of the <see cref="SparkplugNode"/> class.
    /// </summary>
    /// <param name="knownMetrics">The known metrics.</param>
    /// <param name="specificationVersion">The Sparkplug specification version.</param>
    /// <param name="logger">The logger.</param>
    /// <seealso cref="SparkplugNodeBase{T}"/>
    public SparkplugNode(
        IEnumerable<Metric> knownMetrics,
        SparkplugSpecificationVersion specificationVersion,
        ILogger<KnownMetricStorage>? logger = null)
        : base(knownMetrics, specificationVersion, logger)
    {
    }

    /// <inheritdoc cref="SparkplugNodeBase{T}"/>
    /// <summary>
    /// Initializes a new instance of the <see cref="SparkplugNode"/> class.
    /// </summary>
    /// <param name="knownMetricsStorage">The metric names.</param>
    /// <param name="specificationVersion">The Sparkplug specification version.</param>
    /// <seealso cref="SparkplugNodeBase{T}"/>
    public SparkplugNode(
        KnownMetricStorage knownMetricsStorage,
        SparkplugSpecificationVersion specificationVersion)
        : base(knownMetricsStorage, specificationVersion)
    {
    }

    /// <summary>
    /// Publishes version B metrics for a node.
    /// </summary>
    /// <param name="metrics">The metrics.</param>
    /// <exception cref="ArgumentNullException">Thrown if the options are null.</exception>
    /// <returns>A <see cref="MqttClientPublishResult"/>.</returns>
    protected override async Task<MqttClientPublishResult> PublishMessage(IEnumerable<Metric> metrics)
    {
        if (this.Options is null)
        {
            throw new ArgumentNullException(nameof(this.Options), "The options aren't set properly.");
        }

        if (this.KnownMetrics is null)
        {
            throw new ArgumentNullException(nameof(this.KnownMetrics), "The KnownMetrics aren't set properly.");
        }

        // Get the data message.
        var dataMessage = this.messageGenerator.GetSparkplugNodeDataMessage(
            this.NameSpace,
            this.Options.GroupIdentifier,
            this.Options.EdgeNodeIdentifier,
            this.KnownMetricsStorage.FilterMetrics(metrics, SparkplugMessageType.NodeData),
            this.LastSequenceNumber,
            this.LastSessionNumber,
            DateTimeOffset.UtcNow);

        // Increment the sequence number.
        this.IncrementLastSequenceNumber();

        // Publish the message.
        return await this.client.PublishAsync(dataMessage);
    }

    /// <summary>
    /// Called when a node message was received.
    /// </summary>
    /// <param name="topic">The topic.</param>
    /// <param name="payload">The payload.</param>
    /// <exception cref="InvalidCastException">Thrown if the metric cast didn't work properly.</exception>
    protected override async Task OnMessageReceived(SparkplugMessageTopic topic, byte[] payload)
    {
        var payloadVersionB = PayloadHelper.Deserialize<VersionBProtoBuf.ProtoBufPayload>(payload);

        if (payloadVersionB == null) { return; }

        ConcurrentDictionary<string, Metric>? metrics = null;

        if (!(topic.MessageType == SparkplugMessageType.NodeBirth || topic.MessageType == SparkplugMessageType.DeviceBirth))
        {
            // Get known metrics
            if (topic.DeviceIdentifier is null)
            {
                metrics = this.knownMetrics.GetKnownMetricsByName();
            }
            else if (this.KnownDevices.TryGetValue(topic.DeviceIdentifier, out var knownMetricStorage))
            {
                metrics = knownMetricStorage.GetKnownMetricsByName();
            }
            else
            {
                return;
            }
        }

        var convertedPayload = PayloadConverter.ConvertVersionBPayload(payloadVersionB, metrics);

        if (convertedPayload is not Payload _)
        {
            throw new InvalidCastException("The metric cast didn't work properly.");
        }

        await this.HandleMessagesForVersionB(topic, convertedPayload);

    }

    /// <summary>
    /// Handles the received messages for payload version B.
    /// </summary>
    /// <param name="topic">The topic.</param>
    /// <param name="payload">The payload.</param>
    /// <exception cref="InvalidOperationException">Thrown if the topic is invalid.</exception>
    private async Task HandleMessagesForVersionB(SparkplugMessageTopic topic, Payload payload)
    {
        // Filter out session number metric.
        var sessionNumberMetric = payload.Metrics.FirstOrDefault(m => m.Name == Constants.SessionNumberMetricName);
        var metrics = payload.Metrics.Where(m => m.Name != Constants.SessionNumberMetricName).ToList();
        // var filteredMetrics = this.KnownMetricsStorage.FilterMetrics(metricsWithoutSequenceMetric, topic.MessageType).ToList();

        if (sessionNumberMetric is not null)
        {
            metrics.Add(sessionNumberMetric);
        }

        // Handle messages.
        switch (topic.MessageType)
        {
            case SparkplugMessageType.DeviceCommand:
                if (string.IsNullOrWhiteSpace(topic.DeviceIdentifier))
                {
                    throw new InvalidOperationException($"Topic {topic} is invalid!");
                }

                await this.FireDeviceCommandReceived(topic.DeviceIdentifier, metrics);
                break;

            case SparkplugMessageType.NodeCommand:
                await this.FireNodeCommandReceived(metrics);
                break;
        }
    }
}
