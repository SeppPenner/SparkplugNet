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
    public SparkplugNode(IEnumerable<VersionAData.KuraMetric> knownMetrics, ILogger? logger = null) : base(knownMetrics, logger)
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
    /// Called when [message received].
    /// </summary>
    /// <param name="topic">The topic.</param>
    /// <param name="payload">The payload.</param>
    /// <returns>
    /// A <see cref="T:System.Threading.Tasks.Task" /> representing any asynchronous operation.
    /// </returns>
    /// <exception cref="System.InvalidCastException">The metric cast didn't work properly.</exception>
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
                    if (!string.IsNullOrEmpty(topic.DeviceIdentifier))
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

    /// <summary>
    /// Publishes version A metrics for a device.
    /// </summary>
    /// <param name="metrics">The metrics.</param>
    /// <param name="deviceIdentifier">The device identifier.</param>
    /// <exception cref="ArgumentNullException">The options are null.</exception>
    /// <exception cref="Exception">The device is unknown or an invalid metric type was specified.</exception>
    /// <returns>A <see cref="MqttClientPublishResult"/>.</returns>
    protected override async Task<MqttClientPublishResult> PublishMessageForDevice(IEnumerable<VersionAData.KuraMetric> metrics, string deviceIdentifier)
    {
        if (this.Options is null)
        {
            throw new ArgumentNullException(nameof(this.Options), "The options aren't set properly.");
        }

        if (!this.KnownDevices.ContainsKey(deviceIdentifier))
        {
            throw new Exception("The device is unknown, please publish a device birth message first.");
        }

        var deviceMetrics = this.KnownDevices[deviceIdentifier];

        if (deviceMetrics is null)
        {
            throw new Exception("Invalid metric type specified for version A metric.");
        }

        // Get the data message.
        var dataMessage = this.MessageGenerator.GetSparkPlugDeviceDataMessage(
            this.NameSpace,
            this.Options.GroupIdentifier,
            this.Options.EdgeNodeIdentifier,
            deviceIdentifier,
            this.KnownMetricsStorage.FilterOutgoingMetrics(metrics),
            this.LastSequenceNumber,
            this.LastSessionNumber,
            DateTimeOffset.Now,
            this.Options.AddSessionNumberToDataMessages);

        // Increment the sequence number.
        this.IncrementLastSequenceNumber();

        // Publish the message.
        return await this.Client.PublishAsync(dataMessage);
    }
}
