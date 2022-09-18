// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugApplication.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   Defines the SparkplugApplication type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.VersionA;

using SparkplugNet.Core;

/// <inheritdoc cref="SparkplugApplicationBase{T}"/>
public class SparkplugApplication : SparkplugApplicationBase<VersionAData.KuraMetric>
{
    /// <inheritdoc cref="SparkplugApplicationBase{T}"/>
    /// <summary>
    /// Initializes a new instance of the <see cref="SparkplugApplication"/> class.
    /// </summary>
    /// <param name="knownMetrics">The known metrics.</param>
    /// <param name="logger">The logger.</param>
    public SparkplugApplication(IEnumerable<VersionAData.KuraMetric> knownMetrics, ILogger? logger = null) : base(knownMetrics, logger)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SparkplugApplication"/> class.
    /// </summary>
    /// <param name="knownMetricsStorage">The known metrics storage.</param>
    /// <param name="logger">The logger.</param>
    public SparkplugApplication(KnownMetricStorage knownMetricsStorage, ILogger? logger = null) : base(knownMetricsStorage, logger)
    {
    }

    /// <summary>
    /// Publishes a version A node command message.
    /// </summary>
    /// <param name="metrics">The metrics.</param>
    /// <param name="groupIdentifier">The group identifier.</param>
    /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
    /// <exception cref="ArgumentNullException">The options are null.</exception>
    /// <exception cref="Exception">An invalid metric type was specified.</exception>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    protected override async Task PublishNodeCommandMessage(IEnumerable<VersionAData.KuraMetric> metrics, string groupIdentifier, string edgeNodeIdentifier)
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
        var dataMessage = SparkplugMessageGenerator.GetSparkPlugNodeCommandMessage(
            this.NameSpace,
            groupIdentifier,
            edgeNodeIdentifier,
            this.KnownMetricsStorage.FilterOutgoingMetrics(metrics),
            this.LastSequenceNumber,
            this.LastSessionNumber,
            DateTimeOffset.Now,
            this.Options.AddSessionNumberToCommandMessages);

        // Increment the sequence number.
        this.IncrementLastSequenceNumber();

        // Publish the message.
        await this.Client.PublishAsync(dataMessage);
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
    protected override async Task PublishDeviceCommandMessage(IEnumerable<VersionAData.KuraMetric> metrics, string groupIdentifier, string edgeNodeIdentifier, string deviceIdentifier)
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
        var dataMessage = SparkplugMessageGenerator.GetSparkPlugDeviceCommandMessage(
            this.NameSpace,
            groupIdentifier,
            edgeNodeIdentifier,
            deviceIdentifier,
            this.KnownMetricsStorage.FilterOutgoingMetrics(metrics),
            this.LastSequenceNumber,
            this.LastSessionNumber,
            DateTimeOffset.Now,
            this.Options.AddSessionNumberToCommandMessages);

        // Debug output.
        this.Logger?.Debug("NDATA Message: {@DataMessage}", dataMessage);

        // Increment the sequence number.
        this.IncrementLastSequenceNumber();

        // Publish the message.
        await this.Client.PublishAsync(dataMessage);
    }

    /// <summary>
    /// Called when [application message received].
    /// </summary>
    /// <param name="topic">The topic.</param>
    /// <param name="payload">The payload.</param>
    /// <returns>
    /// A <see cref="T:System.Threading.Tasks.Task" /> representing any asynchronous operation.
    /// </returns>
    protected override async Task OnMessageReceived(SparkplugMessageTopic topic, byte[] payload)
    {
        var payloadVersionA = PayloadHelper.Deserialize<VersionAProtoBuf.ProtoBufPayload>(payload);

        if (payloadVersionA != null)
        {
            var convertedPayload = PayloadConverter.ConvertVersionAPayload(payloadVersionA);
            await this.HandleMessagesForVersionA(topic, convertedPayload);
        }
    }

    /// <summary>
    /// Handles the received messages for payload version A.
    /// </summary>
    /// <param name="topic">The topic.</param>
    /// <param name="payload">The payload.</param>
    /// <exception cref="ArgumentNullException">The known metrics are null.</exception>
    /// <exception cref="Exception">The metric is unknown.</exception>
    private async Task HandleMessagesForVersionA(SparkplugMessageTopic topic, VersionAData.Payload payload)
    {
        // If we have any not valid metric, throw an exception.
        var metricsWithoutSequenceMetric = payload.Metrics.Where(m => m.Name != Constants.SessionNumberMetricName);

        this.KnownMetricsStorage.ValidateIncommingMetrics(metricsWithoutSequenceMetric);

        switch (topic.MessageType)
        {
            case SparkplugMessageType.NodeBirth:
                await this.FireNodeBirthReceivedAsync(topic.GroupIdentifier, topic.EdgeNodeIdentifier,
                    this.ProcessPayload(topic, payload, SparkplugMetricStatus.Online));
                break;
            case SparkplugMessageType.DeviceBirth:
                await this.FireDeviceBirthReceivedAsync(topic.GroupIdentifier, topic.EdgeNodeIdentifier, topic.EdgeNodeIdentifier,
                    this.ProcessPayload(topic, payload, SparkplugMetricStatus.Online));
                break;
            case SparkplugMessageType.NodeData:
                foreach (var metric in this.ProcessPayload(topic, payload, SparkplugMetricStatus.Online))
                {
                    await this.FireNodeDataReceivedAsync(topic.GroupIdentifier, topic.EdgeNodeIdentifier, metric);
                }

                break;
            case SparkplugMessageType.DeviceData:
                if (string.IsNullOrEmpty(topic.DeviceIdentifier))
                {
                    throw new InvalidOperationException($"topic {topic} is invalid!");
                }

                foreach (var metric in this.ProcessPayload(topic, payload, SparkplugMetricStatus.Online))
                {
                    await this.FireDeviceDataReceivedAsync(topic.GroupIdentifier, topic.EdgeNodeIdentifier, topic.DeviceIdentifier, metric);
                }

                break;
            case SparkplugMessageType.NodeDeath:
                this.ProcessPayload(topic, payload, SparkplugMetricStatus.Offline);
                await this.FireNodeDeathReceivedAsync(topic.GroupIdentifier, topic.EdgeNodeIdentifier);
                break;
            case SparkplugMessageType.DeviceDeath:
                if (string.IsNullOrEmpty(topic.DeviceIdentifier))
                {
                    throw new InvalidOperationException($"topic {topic} is invalid!");
                }

                this.ProcessPayload(topic, payload, SparkplugMetricStatus.Offline);
                await this.FireDeviceDeathReceivedAsync(topic.GroupIdentifier, topic.EdgeNodeIdentifier, topic.DeviceIdentifier);
                break;
        }
    }

    /// <summary>
    /// Handles the device message.
    /// </summary>
    /// <param name="topic">The topic.</param>
    /// <param name="payload">The payload.</param>
    /// <param name="metricStatus">The metric status.</param>
    /// <exception cref="InvalidCastException">The metric cast is invalid.</exception>
    private IEnumerable<VersionAData.KuraMetric> ProcessPayload(SparkplugMessageTopic topic, VersionAData.Payload payload, SparkplugMetricStatus metricStatus)
    {
        var metricState = new MetricState<VersionAData.KuraMetric>
        {
            MetricStatus = metricStatus
        };

        if (!string.IsNullOrEmpty(topic.DeviceIdentifier))
        {
            this.DeviceStates[topic.DeviceIdentifier] = metricState;
        }
        else
        {
            this.NodeStates[topic.EdgeNodeIdentifier] = metricState;
        }

        foreach (var payloadMetric in payload.Metrics)
        {
            if (payloadMetric is not VersionAData.KuraMetric convertedMetric)
            {
                throw new InvalidCastException("The metric cast didn't work properly.");
            }

            if (payloadMetric.Name != null)
            {
                metricState.Metrics[payloadMetric.Name] = convertedMetric;
            }

            yield return convertedMetric;
        }
    }
}
