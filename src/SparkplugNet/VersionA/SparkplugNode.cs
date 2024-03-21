// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugNode.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class that handles a Sparkplug node.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.VersionA;

/// <inheritdoc cref="SparkplugNodeBase{T}"/>
/// <summary>
///   A class that handles a Sparkplug node.
/// </summary>
/// <seealso cref="SparkplugNodeBase{T}"/>
[Obsolete("Sparkplug version A is obsolete since version 3 of the specification, use version B where possible.")]
public sealed class SparkplugNode : SparkplugNodeBase<VersionAData.KuraMetric>
{
    /// <inheritdoc cref="SparkplugNodeBase{T}"/>
    /// <summary>
    /// Initializes a new instance of the <see cref="SparkplugNode"/> class.
    /// </summary>
    /// <param name="knownMetrics">The known metrics.</param>
    /// <param name="specificationVersion">The Sparkplug specification version.</param>
    /// <param name="logger">The logger.</param>
    /// <seealso cref="SparkplugNodeBase{T}"/>
    [Obsolete("Sparkplug version A is obsolete since version 3 of the specification, use version B where possible.")]
    public SparkplugNode(
        IEnumerable<VersionAData.KuraMetric> knownMetrics,
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
    [Obsolete("Sparkplug version A is obsolete since version 3 of the specification, use version B where possible.")]
    public SparkplugNode(
        KnownMetricStorage knownMetricsStorage,
        SparkplugSpecificationVersion specificationVersion)
        : base(knownMetricsStorage, specificationVersion)
    {
    }

    /// <summary>
    /// Publishes version A metrics for a node.
    /// </summary>
    /// <param name="metrics">The metrics.</param>
    /// <exception cref="ArgumentNullException">Thrown if the options are null.</exception>
    /// <exception cref="Exception">Thrown if an invalid metric type was specified.</exception>
    /// <returns>A <see cref="MqttClientPublishResult"/>.</returns>
    [Obsolete("Sparkplug version A is obsolete since version 3 of the specification, use version B where possible.")]
    protected override async Task<MqttClientPublishResult> PublishMessage(IEnumerable<VersionAData.KuraMetric> metrics)
    {
        if (this.Options is null)
        {
            throw new ArgumentNullException(nameof(this.Options), "The options aren't set properly.");
        }

        if (this.KnownMetrics is null)
        {
            throw new Exception("Invalid metric type specified for version A metric.");
        }

        // Get the data message.
        var dataMessage = this.messageGenerator.GetSparkplugNodeDataMessage(
            this.NameSpace,
            this.Options.GroupIdentifier,
            this.Options.EdgeNodeIdentifier,
            this.KnownMetricsStorage.FilterMetrics(metrics, SparkplugMessageType.NodeData),
            this.LastSequenceNumber,
            this.LastSessionNumber,
            DateTimeOffset.Now);

        // Increment the sequence number.
        this.IncrementLastSequenceNumber();

        // Publish the message.
        return await this.client.PublishAsync(dataMessage);
    }

    /// <summary>
    /// Called when the node message is received.
    /// </summary>
    /// <param name="topic">The topic.</param>
    /// <param name="payload">The payload.</param>
    /// <exception cref="InvalidCastException">Thrown if the metric cast didn't work properly.</exception>
    [Obsolete("Sparkplug version A is obsolete since version 3 of the specification, use version B where possible.")]
    protected override async Task OnMessageReceived(SparkplugMessageTopic topic, byte[] payload)
    {
        var payloadVersionA = PayloadHelper.Deserialize<VersionAProtoBuf.ProtoBufPayload>(payload);

        if (payloadVersionA is not null)
        {
            var convertedPayload = PayloadConverter.ConvertVersionAPayload(payloadVersionA);

            if (convertedPayload is not VersionAData.Payload _)
            {
                throw new InvalidCastException("The metric cast didn't work properly.");
            }

            await this.HandleMessagesForVersionB(topic, convertedPayload);
        }
    }

    /// <summary>
    /// Handles the received messages for payload version B.
    /// </summary>
    /// <param name="topic">The topic.</param>
    /// <param name="payload">The payload.</param>
    /// <exception cref="ArgumentNullException">Thrown if the known metrics are null.</exception>
    private async Task HandleMessagesForVersionB(SparkplugMessageTopic topic, VersionAData.Payload payload)
    {
        // Filter out session number metric.
        var sessionNumberMetric = payload.Metrics.FirstOrDefault(m => m.Name != Constants.SessionNumberMetricName);
        var metricsWithoutSequenceMetric = payload.Metrics.Where(m => m.Name != Constants.SessionNumberMetricName);
        var filteredMetrics = this.KnownMetricsStorage.FilterMetrics(metricsWithoutSequenceMetric, topic.MessageType).ToList();

        if (sessionNumberMetric is not null)
        {
            filteredMetrics.Add(sessionNumberMetric);
        }

        // Handle messages.
        switch (topic.MessageType)
        {
            case SparkplugMessageType.DeviceCommand:
                if (string.IsNullOrWhiteSpace(topic.DeviceIdentifier))
                {
                    throw new InvalidOperationException($"Topic {topic} is invalid!");
                }

                await this.FireDeviceCommandReceived(topic.DeviceIdentifier, filteredMetrics);
                break;

            case SparkplugMessageType.NodeCommand:
                await this.FireNodeCommandReceived(filteredMetrics);
                break;
        }
    }
}
