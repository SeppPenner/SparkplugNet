// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugApplication.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class that handles a Sparkplug application.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.VersionA;

/// <inheritdoc cref="SparkplugApplicationBase{T}"/>
/// <summary>
///   A class that handles a Sparkplug application.
/// </summary>
/// <seealso cref="SparkplugApplicationBase{T}"/>
[Obsolete("Sparkplug version A is obsolete since version 3 of the specification, use version B where possible.")]
public sealed class SparkplugApplication : SparkplugApplicationBase<VersionAData.KuraMetric>
{
    /// <inheritdoc cref="SparkplugApplicationBase{T}"/>
    /// <summary>
    /// Initializes a new instance of the <see cref="SparkplugApplication"/> class.
    /// </summary>
    /// <param name="knownMetrics">The known metrics.</param>
    /// <param name="specificationVersion">The Sparkplug specification version.</param>
    /// <param name="logger">The logger.</param>
    /// <seealso cref="SparkplugApplicationBase{T}"/>
    [Obsolete("Sparkplug version A is obsolete since version 3 of the specification, use version B where possible.")]
    public SparkplugApplication(
        IEnumerable<VersionAData.KuraMetric> knownMetrics,
        SparkplugSpecificationVersion specificationVersion,
        ILogger<KnownMetricStorage>? logger = null)
        : base(knownMetrics, specificationVersion, logger)
    {
    }

    /// <inheritdoc cref="SparkplugApplicationBase{T}"/>
    /// <summary>
    /// Initializes a new instance of the <see cref="SparkplugApplication"/> class.
    /// </summary>
    /// <param name="knownMetricsStorage">The known metrics storage.</param>
    /// <param name="specificationVersion">The Sparkplug specification version.</param>
    /// <seealso cref="SparkplugApplicationBase{T}"/>
    [Obsolete("Sparkplug version A is obsolete since version 3 of the specification, use version B where possible.")]
    public SparkplugApplication(
        KnownMetricStorage knownMetricsStorage,
        SparkplugSpecificationVersion specificationVersion)
        : base(knownMetricsStorage, specificationVersion)
    {
    }

    /// <summary>
    /// Publishes a version A node command message.
    /// </summary>
    /// <param name="metrics">The metrics.</param>
    /// <param name="groupIdentifier">The group identifier.</param>
    /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
    /// <exception cref="ArgumentNullException">Thrown if the options are null.</exception>
    /// <exception cref="Exception">Thrown if an invalid metric type was specified.</exception>
    [Obsolete("Sparkplug version A is obsolete since version 3 of the specification, use version B where possible.")]
    protected override async Task PublishNodeCommandMessage(
        IEnumerable<VersionAData.KuraMetric> metrics,
        string groupIdentifier,
        string edgeNodeIdentifier)
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
        var dataMessage = this.messageGenerator.GetSparkplugNodeCommandMessage(
            this.NameSpace,
            groupIdentifier,
            edgeNodeIdentifier,
            this.KnownMetricsStorage.FilterMetrics(metrics, SparkplugMessageType.NodeCommand),
            this.LastSequenceNumber,
            this.LastSessionNumber,
            DateTimeOffset.Now);

        // Increment the sequence number.
        this.IncrementLastSequenceNumber();

        // Publish the message.
        await this.client.PublishAsync(dataMessage);
    }

    /// <summary>
    /// Publishes a version A device command message.
    /// </summary>
    /// <param name="metrics">The metrics.</param>
    /// <param name="groupIdentifier">The group identifier.</param>
    /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
    /// <param name="deviceIdentifier">The device identifier.</param>
    /// <exception cref="ArgumentNullException">Thrown if the options are null.</exception>
    /// <exception cref="Exception">Thrown if an invalid metric type was specified.</exception>
    [Obsolete("Sparkplug version A is obsolete since version 3 of the specification, use version B where possible.")]
    protected override async Task PublishDeviceCommandMessage(
        IEnumerable<VersionAData.KuraMetric> metrics,
        string groupIdentifier,
        string edgeNodeIdentifier,
        string deviceIdentifier)
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
        var dataMessage = this.messageGenerator.GetSparkplugDeviceCommandMessage(
            this.NameSpace,
            groupIdentifier,
            edgeNodeIdentifier,
            deviceIdentifier,
            this.KnownMetricsStorage.FilterMetrics(metrics, SparkplugMessageType.DeviceCommand),
            this.LastSequenceNumber,
            this.LastSessionNumber,
            DateTimeOffset.Now);

        // Increment the sequence number.
        this.IncrementLastSequenceNumber();

        // Publish the message.
        await this.client.PublishAsync(dataMessage);
    }

    /// <summary>
    /// Called when an application message was received.
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

            await this.HandleMessagesForVersionA(topic, convertedPayload);
        }
    }

    /// <summary>
    /// Handles the received messages for payload version A.
    /// </summary>
    /// <param name="topic">The topic.</param>
    /// <param name="payload">The payload.</param>
    /// <exception cref="InvalidOperationException">Thrown if the topic is invalid.</exception>
    private async Task HandleMessagesForVersionA(SparkplugMessageTopic topic, VersionAData.Payload payload)
    {
        // Filter out session number metric.
        var sessionNumberMetric = payload.Metrics.FirstOrDefault(m => m.Name == Constants.SessionNumberMetricName);
        var metricsWithoutSequenceMetric = payload.Metrics.Where(m => m.Name != Constants.SessionNumberMetricName);
        var filteredMetrics = this.KnownMetricsStorage.FilterMetrics(metricsWithoutSequenceMetric, topic.MessageType).ToList();

        if (sessionNumberMetric is not null)
        {
            filteredMetrics.Add(sessionNumberMetric);
        }

        // Handle messages.
        switch (topic.MessageType)
        {
            case SparkplugMessageType.NodeBirth:
                await this.FireNodeBirthReceived(topic.GroupIdentifier, topic.EdgeNodeIdentifier,
                    this.ProcessPayload(topic, filteredMetrics, SparkplugMetricStatus.Online));
                break;
            case SparkplugMessageType.DeviceBirth:
                await this.FireDeviceBirthReceived(topic.GroupIdentifier, topic.EdgeNodeIdentifier, topic.EdgeNodeIdentifier,
                    this.ProcessPayload(topic, filteredMetrics, SparkplugMetricStatus.Online));
                break;
            case SparkplugMessageType.NodeData:
                var nodeDataMetrics = this.ProcessPayload(topic, filteredMetrics, SparkplugMetricStatus.Online);
                await this.FireNodeDataReceived(topic.GroupIdentifier, topic.EdgeNodeIdentifier, nodeDataMetrics);
                break;
            case SparkplugMessageType.DeviceData:
                if (string.IsNullOrWhiteSpace(topic.DeviceIdentifier))
                {
                    throw new InvalidOperationException($"Topic {topic} is invalid!");
                }

                var deviceDataMetrics = this.ProcessPayload(topic, filteredMetrics, SparkplugMetricStatus.Online);
                await this.FireDeviceDataReceived(topic.GroupIdentifier, topic.EdgeNodeIdentifier, topic.DeviceIdentifier, deviceDataMetrics);
                break;
            case SparkplugMessageType.NodeDeath:
                this.ProcessPayload(topic, filteredMetrics, SparkplugMetricStatus.Offline);
                await this.FireNodeDeathReceived(topic.GroupIdentifier, topic.EdgeNodeIdentifier, sessionNumberMetric);
                break;
            case SparkplugMessageType.DeviceDeath:
                if (string.IsNullOrWhiteSpace(topic.DeviceIdentifier))
                {
                    throw new InvalidOperationException($"Topic {topic} is invalid!");
                }

                this.ProcessPayload(topic, filteredMetrics, SparkplugMetricStatus.Offline);
                await this.FireDeviceDeathReceived(topic.GroupIdentifier, topic.EdgeNodeIdentifier, topic.DeviceIdentifier);
                break;
        }
    }

    /// <summary>
    /// Handles the device message.
    /// </summary>
    /// <param name="topic">The topic.</param>
    /// <param name="metrics">The metrics.</param>
    /// <param name="metricStatus">The metric status.</param>
    /// <exception cref="InvalidOperationException">Thrown if the edge node identifier is invalid.</exception>
    /// <exception cref="InvalidCastException">Thrown if the metric cast is invalid.</exception>
    private IEnumerable<VersionAData.KuraMetric> ProcessPayload(
        SparkplugMessageTopic topic,
        List<VersionAData.KuraMetric> metrics,
        SparkplugMetricStatus metricStatus)
    {
        var metricState = new MetricState<VersionAData.KuraMetric>
        {
            MetricStatus = metricStatus
        };

        if (!string.IsNullOrWhiteSpace(topic.DeviceIdentifier))
        {
            if (string.IsNullOrWhiteSpace(topic.EdgeNodeIdentifier))
            {
                throw new InvalidOperationException($"The edge node identifier is invalid for device {topic.DeviceIdentifier}.");
            }

            this.DeviceStates[$"{topic.EdgeNodeIdentifier}/{topic.DeviceIdentifier}"] = metricState;
        }
        else
        {
            this.NodeStates[topic.EdgeNodeIdentifier] = metricState;
        }

        foreach (var payloadMetric in metrics)
        {
            if (payloadMetric is not VersionAData.KuraMetric convertedMetric)
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
