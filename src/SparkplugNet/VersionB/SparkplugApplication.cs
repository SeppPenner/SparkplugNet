// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugApplication.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class that handles a Sparkplug application.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.VersionB;

/// <inheritdoc cref="SparkplugApplicationBase{T}"/>
/// <summary>
///   A class that handles a Sparkplug application.
/// </summary>
/// <seealso cref="SparkplugApplicationBase{T}"/>
public class SparkplugApplication : SparkplugApplicationBase<VersionBData.Metric>
{
    /// <inheritdoc cref="SparkplugApplicationBase{T}"/>
    /// <summary>
    /// Initializes a new instance of the <see cref="SparkplugApplication"/> class.
    /// </summary>
    /// <param name="knownMetrics">The known metrics.</param>
    /// <param name="logger">The logger.</param>
    /// <seealso cref="SparkplugApplicationBase{T}"/>
    public SparkplugApplication(IEnumerable<VersionBData.Metric> knownMetrics, ILogger? logger = null) : base(knownMetrics, logger)
    {
    }

    /// <inheritdoc cref="SparkplugApplicationBase{T}"/>
    /// <summary>
    /// Initializes a new instance of the <see cref="SparkplugApplication"/> class.
    /// </summary>
    /// <param name="knownMetricsStorage">The metric names.</param>
    /// <param name="logger">The logger.</param>
    /// <seealso cref="SparkplugApplicationBase{T}"/>
    public SparkplugApplication(KnownMetricStorage knownMetricsStorage, ILogger? logger = null) : base(knownMetricsStorage, logger)
    {
    }

    /// <summary>
    /// Publishes a version B node command message.
    /// </summary>
    /// <param name="metrics">The metrics.</param>
    /// <param name="groupIdentifier">The group identifier.</param>
    /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
    /// <exception cref="ArgumentNullException">Thrown if the options are null.</exception>
    /// <exception cref="Exception">Thrown if an invalid metric type was specified.</exception>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    protected override async Task PublishNodeCommandMessage(IEnumerable<VersionBData.Metric> metrics, string groupIdentifier, string edgeNodeIdentifier)
    {
        if (this.Options is null)
        {
            throw new ArgumentNullException(nameof(this.Options), "The options arent't set properly.");
        }

        // Get the data message.
        var dataMessage = SparkplugMessageGenerator.GetSparkPlugNodeCommandMessage(
            this.NameSpace,
            groupIdentifier,
            edgeNodeIdentifier,
            this.KnownMetricsStorage.FilterOutgoingMetrics(metrics),
            this.LastSequenceNumber,
            this.LastSessionNumber,
            DateTimeOffset.Now);

        // Debug output.
        this.Logger?.Debug("NDATA Message: {@DataMessage}", dataMessage);

        // Increment the sequence number.
        this.IncrementLastSequenceNumber();

        // Publish the message.
        await this.Client.PublishAsync(dataMessage);
    }

    /// <summary>
    /// Publishes a version B device command message.
    /// </summary>
    /// <param name="metrics">The metrics.</param>
    /// <param name="groupIdentifier">The group identifier.</param>
    /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
    /// <param name="deviceIdentifier">The device identifier.</param>
    /// <exception cref="ArgumentNullException">Thrown if the options are null.</exception>
    /// <exception cref="Exception">Thrown if an invalid metric type was specified.</exception>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    protected override async Task PublishDeviceCommandMessage(IEnumerable<VersionBData.Metric> metrics, string groupIdentifier, string edgeNodeIdentifier, string deviceIdentifier)
    {
        if (this.Options is null)
        {
            throw new ArgumentNullException(nameof(this.Options), "The options aren't set properly.");
        }

        if (this.KnownMetrics is null)
        {
            throw new Exception("Invalid metric type specified for version B metric.");
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
            DateTimeOffset.Now);

        // Increment the sequence number.
        this.IncrementLastSequenceNumber();

        // Publish the message.
        await this.Client.PublishAsync(dataMessage);
    }

    /// <summary>
    /// Called when an application message was received.
    /// </summary>
    /// <param name="topic">The topic.</param>
    /// <param name="payload">The payload.</param>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    protected override async Task OnMessageReceived(SparkplugMessageTopic topic, byte[] payload)
    {
        var payloadVersionB = PayloadHelper.Deserialize<VersionBProtoBuf.ProtoBufPayload>(payload);

        if (payloadVersionB is not null)
        {
            var convertedPayload = PayloadConverter.ConvertVersionBPayload(payloadVersionB);
            await this.HandleMessagesForVersionBAsync(topic, convertedPayload);
        }
    }

    /// <summary>
    /// Handles the received messages for payload version B.
    /// </summary>
    /// <param name="topic">The topic.</param>
    /// <param name="payload">The payload.</param>
    /// <exception cref="ArgumentNullException">Thrown if the known metrics are null.</exception>
    /// <exception cref="Exception">Thrown if the metric is unknown.</exception>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    private async Task HandleMessagesForVersionBAsync(SparkplugMessageTopic topic, VersionBData.Payload payload)
    {
        // If we have any not valid metric, throw an exception.
        var metricsWithoutSequenceMetric = payload.Metrics.Where(m => m.Name != Constants.SessionNumberMetricName);

        this.KnownMetricsStorage.ValidateIncomingMetrics(metricsWithoutSequenceMetric);

        switch (topic.MessageType)
        {
            case SparkplugMessageType.NodeBirth:
                await this.FireNodeBirthReceivedAsync(topic.GroupIdentifier, topic.EdgeNodeIdentifier,
                    this.ProcessPayload(topic, payload, SparkplugMetricStatus.Online));
                break;
            case SparkplugMessageType.DeviceBirth:
                await this.FireDeviceBirthReceivedAsync(topic.GroupIdentifier, topic.EdgeNodeIdentifier, topic.DeviceIdentifier,
                    this.ProcessPayload(topic, payload, SparkplugMetricStatus.Online));
                break;
            case SparkplugMessageType.NodeData:
                foreach (var metric in this.ProcessPayload(topic, payload, SparkplugMetricStatus.Online))
                {
                    await this.FireNodeDataReceivedAsync(topic.GroupIdentifier, topic.EdgeNodeIdentifier, metric);
                }

                break;
            case SparkplugMessageType.DeviceData:
                if (string.IsNullOrWhiteSpace(topic.DeviceIdentifier))
                {
                    throw new InvalidOperationException($"Topic {topic} is invalid!");
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
                if (string.IsNullOrWhiteSpace(topic.DeviceIdentifier))
                {
                    throw new InvalidOperationException($"Topic {topic} is invalid!");
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
    /// <exception cref="InvalidCastException">Thrown if the metric cast is invalid.</exception>
    private IEnumerable<VersionBData.Metric> ProcessPayload(SparkplugMessageTopic topic, VersionBData.Payload payload, SparkplugMetricStatus metricStatus)
    {
        var metricState = new MetricState<VersionBData.Metric>
        {
            MetricStatus = metricStatus
        };

        if (!string.IsNullOrWhiteSpace(topic.DeviceIdentifier))
        {
            this.DeviceStates[topic.DeviceIdentifier] = metricState;
        }
        else
        {
            this.NodeStates[topic.EdgeNodeIdentifier] = metricState;
        }

        foreach (var payloadMetric in payload.Metrics)
        {
            if (payloadMetric is not VersionB.Data.Metric convertedMetric)
            {
                throw new InvalidCastException("The metric cast didn't work properly.");
            }

            if (payloadMetric.Name is not null)
            {
                metricState.Metrics[payloadMetric.Name] = convertedMetric;
            }

            yield return convertedMetric;
        }
    }
}