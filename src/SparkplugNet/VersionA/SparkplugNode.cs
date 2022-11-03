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
public class SparkplugNode : SparkplugNodeBase<VersionAData.KuraMetric>
{
    /// <inheritdoc cref="SparkplugNodeBase{T}"/>
    /// <summary>
    /// Initializes a new instance of the <see cref="SparkplugNode"/> class.
    /// </summary>
    /// <param name="knownMetrics">The known metrics.</param>
    /// <param name="logger">The logger.</param>
    /// <seealso cref="SparkplugNodeBase{T}"/>
    public SparkplugNode(IEnumerable<VersionAData.KuraMetric> knownMetrics, ILogger? logger = null) : base(knownMetrics, logger)
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
    /// Publishes version A metrics for a node.
    /// </summary>
    /// <param name="metrics">The metrics.</param>
    /// <exception cref="ArgumentNullException">Thrown if the options are null.</exception>
    /// <exception cref="Exception">Thrown if an invalid metric type was specified.</exception>
    /// <returns>A <see cref="MqttClientPublishResult"/>.</returns>
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
        var dataMessage = this.MessageGenerator.GetSparkPlugNodeDataMessage(
            this.NameSpace,
            this.Options.GroupIdentifier,
            this.Options.EdgeNodeIdentifier,
            this.KnownMetricsStorage.FilterOutgoingMetrics(metrics),
            this.LastSequenceNumber,
            this.LastSessionNumber,
            DateTimeOffset.Now,
            this.Options.AddSessionNumberToDataMessages);

        // Debug output.
        this.Logger?.Debug("NDATA Message: {@DataMessage}", dataMessage);

        // Increment the sequence number.
        this.IncrementLastSequenceNumber();

        // Publish the message.
        return await this.Client.PublishAsync(dataMessage);
    }

    /// <summary>
    /// Called when the node message is received.
    /// </summary>
    /// <param name="topic">The topic.</param>
    /// <param name="payload">The payload.</param>
    /// <exception cref="InvalidCastException">Thrown if the metric cast didn't work properly.</exception>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    protected override async Task OnMessageReceived(SparkplugMessageTopic topic, byte[] payload)
    {
        var payloadVersionA = PayloadHelper.Deserialize<VersionAProtoBuf.ProtoBufPayload>(payload);

        if (payloadVersionA is not null)
        {
            var convertedPayload = PayloadConverter.ConvertVersionAPayload(payloadVersionA);

            if (convertedPayload is not VersionAData.Payload convertedPayloadVersionA)
            {
                throw new InvalidCastException("The metric cast didn't work properly.");
            }

            switch (topic.MessageType)
            {
                case SparkplugMessageType.DeviceCommand:
                    if (!string.IsNullOrWhiteSpace(topic.DeviceIdentifier))
                    {
                        foreach (var metric in convertedPayloadVersionA.Metrics)
                        {
                            if (metric is VersionAData.KuraMetric convertedMetric)
                            {
                                await this.FireDeviceCommandReceivedAsync(topic.DeviceIdentifier, convertedMetric);
                            }
                        }
                    }

                    break;

                case SparkplugMessageType.NodeCommand:
                    foreach (var metric in convertedPayloadVersionA.Metrics)
                    {
                        if (metric is VersionAData.KuraMetric convertedMetric)
                        {
                            await this.FireNodeCommandReceivedAsync(convertedMetric);
                        }
                    }

                    break;
            }
        }
    }
}
