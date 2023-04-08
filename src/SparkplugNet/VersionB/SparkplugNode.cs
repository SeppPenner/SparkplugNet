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
public class SparkplugNode : SparkplugNodeBase<VersionBData.Metric>
{
    /// <inheritdoc cref="SparkplugNodeBase{T}"/>
    /// <summary>
    /// Initializes a new instance of the <see cref="SparkplugNode"/> class.
    /// </summary>
    /// <param name="knownMetrics">The known metrics.</param>
    /// <param name="logger">The logger.</param>
    /// <seealso cref="SparkplugNodeBase{T}"/>
    public SparkplugNode(IEnumerable<VersionBData.Metric> knownMetrics, ILogger? logger = null) : base(knownMetrics, logger)
    {
    }

    /// <inheritdoc cref="SparkplugNodeBase{T}"/>
    /// <summary>
    /// Initializes a new instance of the <see cref="SparkplugNode"/> class.
    /// </summary>
    /// <param name="knownMetricsStorage">The metric names.</param>
    /// <param name="logger">The logger.</param>
    /// <seealso cref="SparkplugNodeBase{T}"/>
    public SparkplugNode(KnownMetricStorage knownMetricsStorage, ILogger? logger = null) : base(knownMetricsStorage, logger)
    {
    }

    /// <summary>
    /// Publishes version B metrics for a node.
    /// </summary>
    /// <param name="metrics">The metrics.</param>
    /// <exception cref="ArgumentNullException">Thrown if the options are null.</exception>
    /// <exception cref="Exception">Thrown if an invalid metric type was specified.</exception>
    /// <returns>A <see cref="MqttClientPublishResult"/>.</returns>
    protected override async Task<MqttClientPublishResult> PublishMessage(IEnumerable<VersionBData.Metric> metrics)
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
        var dataMessage = this.MessageGenerator.GetSparkPlugNodeDataMessage(
            this.NameSpace,
            this.Options.GroupIdentifier,
            this.Options.EdgeNodeIdentifier,
            this.KnownMetricsStorage.FilterOutgoingMetrics(metrics),
            this.LastSequenceNumber,
            this.LastSessionNumber,
            DateTimeOffset.Now);

        // Debug output.
        this.Logger?.Debug("NDATA Message: {@DataMessage}", dataMessage);

        // Increment the sequence number.
        this.IncrementLastSequenceNumber();

        // Publish the message.
        return await this.Client.PublishAsync(dataMessage);
    }

    /// <summary>
    /// Called when a node message was received.
    /// </summary>
    /// <param name="topic">The topic.</param>
    /// <param name="payload">The payload.</param>
    /// <exception cref="InvalidCastException">Thrown if the metric cast didn't work properly.</exception>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    protected override async Task OnMessageReceived(SparkplugMessageTopic topic, byte[] payload)
    {
        var payloadVersionB = PayloadHelper.Deserialize<VersionBProtoBuf.ProtoBufPayload>(payload);

        if (payloadVersionB is not null)
        {
            var convertedPayload = PayloadConverter.ConvertVersionBPayload(payloadVersionB);

            if (convertedPayload is not VersionBData.Payload convertedPayloadVersionB)
            {
                throw new InvalidCastException("The metric cast didn't work properly.");
            }

            switch (topic.MessageType)
            {
                case SparkplugMessageType.DeviceCommand:
                    if (!string.IsNullOrWhiteSpace(topic.DeviceIdentifier))
                    {
                        foreach (var metric in convertedPayloadVersionB.Metrics)
                        {
                            if (metric is VersionBData.Metric convertedMetric)
                            {
                                await this.FireDeviceCommandReceivedAsync(topic.DeviceIdentifier, convertedMetric);
                            }
                        }
                    }

                    break;

                case SparkplugMessageType.NodeCommand:
                    foreach (var metric in convertedPayloadVersionB.Metrics)
                    {
                        if (metric is VersionBData.Metric convertedMetric)
                        {
                            await this.FireNodeCommandReceivedAsync(convertedMetric);
                        }
                    }

                    break;
            }
        }
    }
}
