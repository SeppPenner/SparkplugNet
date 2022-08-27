// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugNode.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   Defines the SparkplugNode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.VersionA;

using SparkplugNet.Core;

/// <inheritdoc cref="SparkplugNodeBase{T}"/>
public class SparkplugNode : SparkplugNodeBase<VersionAData.KuraMetric>
{
    /// <inheritdoc cref="SparkplugNodeBase{T}"/>
    /// <summary>
    /// Initializes a new instance of the <see cref="SparkplugNode"/> class.
    /// </summary>
    /// <param name="knownMetrics">The known metrics.</param>
    /// <param name="logger">The logger.</param>
    public SparkplugNode(List<VersionAData.KuraMetric> knownMetrics, ILogger? logger = null) : base(knownMetrics, logger)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SparkplugNode"/> class.
    /// </summary>
    /// <param name="knownMetricsStorage">The metric names.</param>
    /// <param name="logger">The logger.</param>
    /// /// <seealso cref="SparkplugNodeBase{T}"/>
    public SparkplugNode(KnownMetricStorage knownMetricsStorage, ILogger? logger = null) : base(knownMetricsStorage, logger)
    {
    }

    /// <summary>
    /// Publishes version A metrics for a node.
    /// </summary>
    /// <param name="metrics">The metrics.</param>
    /// <exception cref="ArgumentNullException">The options are null.</exception>
    /// <exception cref="Exception">An invalid metric type was specified.</exception>
    /// <returns>A <see cref="MqttClientPublishResult"/>.</returns>
    protected override async Task<MqttClientPublishResult> PublishMessage(List<VersionAData.KuraMetric> metrics)
    {
        if (this.options is null)
        {
            throw new ArgumentNullException(nameof(this.options), "The options aren't set properly.");
        }

        if (this.KnownMetrics is not IEnumerable<VersionAData.KuraMetric> knownMetrics)
        {
            throw new Exception("Invalid metric type specified for version A metric.");
        }

        // Remove all not known metrics.
        metrics.RemoveAll(m => knownMetrics.FirstOrDefault(m2 => m2.Name == m.Name) == default);

        // Remove the session number metric if a user might have added it.
        metrics.RemoveAll(m => m.Name == Constants.SessionNumberMetricName);

        // Get the data message.
        var dataMessage = this.MessageGenerator.GetSparkPlugNodeDataMessage(
            this.NameSpace,
            this.options.GroupIdentifier,
            this.options.EdgeNodeIdentifier,
            metrics,
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
    /// Called when [message received].
    /// </summary>
    /// <param name="topic">The topic.</param>
    /// <param name="payload">The payload.</param>
    /// <returns>
    /// A <see cref="T:System.Threading.Tasks.Task" /> representing any asynchronous operation.
    /// </returns>
    /// <exception cref="System.InvalidCastException">The metric cast didn't work properly.</exception>
    protected override Task OnMessageReceived(string topic, byte[] payload)
    {
        var payloadVersionA = PayloadHelper.Deserialize<VersionAProtoBuf.ProtoBufPayload>(payload);

        if (payloadVersionA is not null)
        {
            var convertedPayload = PayloadConverter.ConvertVersionAPayload(payloadVersionA);

            if (topic.Contains(SparkplugMessageType.DeviceCommand.GetDescription()))
            {
                if (convertedPayload is not VersionAData.Payload convertedPayloadVersionA)
                {
                    throw new InvalidCastException("The metric cast didn't work properly.");
                }

                foreach (var metric in convertedPayloadVersionA.Metrics)
                {
                    if (metric is VersionAData.KuraMetric convertedMetric)
                    {
                        this.DeviceCommandReceived?.Invoke(convertedMetric);
                    }
                }
            }

            if (topic.Contains(SparkplugMessageType.NodeCommand.GetDescription()))
            {
                if (convertedPayload is not VersionAData.Payload convertedPayloadVersionA)
                {
                    throw new InvalidCastException("The metric cast didn't work properly.");
                }

                foreach (var metric in convertedPayloadVersionA.Metrics)
                {
                    if (metric is VersionAData.KuraMetric convertedMetric)
                    {
                        this.NodeCommandReceived?.Invoke(convertedMetric);
                    }
                }
            }
        }

        return Task.CompletedTask;
    }
}
