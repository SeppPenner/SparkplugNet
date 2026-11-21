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
public sealed class SparkplugApplication : SparkplugApplicationBase<Metric>
{
    /// <inheritdoc cref="SparkplugApplicationBase{T}"/>
    /// <summary>
    /// Initializes a new instance of the <see cref="SparkplugApplication"/> class.
    /// </summary>
    /// <param name="knownMetrics">The known metrics.</param>
    /// <param name="specificationVersion">The Sparkplug specification version.</param>
    /// <param name="logger">The logger.</param>
    /// <seealso cref="SparkplugApplicationBase{T}"/>
    public SparkplugApplication(
        IEnumerable<Metric> knownMetrics,
        SparkplugSpecificationVersion specificationVersion,
        ILogger<KnownMetricStorage>? logger = null)
        : base(knownMetrics, specificationVersion, logger)
    {
    }

    /// <inheritdoc cref="SparkplugApplicationBase{T}"/>
    /// <summary>
    /// Initializes a new instance of the <see cref="SparkplugApplication"/> class.
    /// </summary>
    /// <param name="knownMetricsStorage">The metric names.</param>
    /// <param name="specificationVersion">The Sparkplug specification version.</param>
    /// <seealso cref="SparkplugApplicationBase{T}"/>
    public SparkplugApplication(
        KnownMetricStorage knownMetricsStorage,
        SparkplugSpecificationVersion specificationVersion)
        : base(knownMetricsStorage, specificationVersion)
    {
    }

    /// <summary>
    /// Publishes a version B node command message.
    /// </summary>
    /// <param name="metrics">The metrics.</param>
    /// <param name="groupIdentifier">The group identifier.</param>
    /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
    /// <exception cref="ArgumentNullException">Thrown if the options are null.</exception>
    protected override async Task PublishNodeCommandMessage(IEnumerable<Metric> metrics, string groupIdentifier, string edgeNodeIdentifier)
    {
        if (this.Options is null)
        {
            throw new ArgumentNullException(nameof(this.Options), "The options arent't set properly.");
        }

        // Get the data message.
        var dataMessage = this.messageGenerator.GetSparkplugNodeCommandMessage(
            this.NameSpace,
            groupIdentifier,
            edgeNodeIdentifier,
            this.KnownMetricsStorage.FilterMetrics(metrics, SparkplugMessageType.NodeCommand),
            this.LastSequenceNumber,
            this.Options.AddSessionNumberToCommandMessages ? this.LastSessionNumber : null,
            DateTimeOffset.UtcNow);

        // Increment the sequence number.
        this.IncrementLastSequenceNumber();

        // Publish the message.
        await this.client.PublishAsync(dataMessage);
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
    protected override async Task PublishDeviceCommandMessage(IEnumerable<Metric> metrics, string groupIdentifier, string edgeNodeIdentifier, string deviceIdentifier)
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
        var dataMessage = this.messageGenerator.GetSparkplugDeviceCommandMessage(
            this.NameSpace,
            groupIdentifier,
            edgeNodeIdentifier,
            deviceIdentifier,
            this.KnownMetricsStorage.FilterMetrics(metrics, SparkplugMessageType.DeviceCommand),
            this.LastSequenceNumber,
            this.Options.AddSessionNumberToCommandMessages ? this.LastSessionNumber : null,
            DateTimeOffset.UtcNow);

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
    protected override async Task OnMessageReceived(SparkplugMessageTopic topic, byte[] payload)
    {
        var payloadVersionB = PayloadHelper.Deserialize<VersionBProtoBuf.ProtoBufPayload>(payload);

        if (payloadVersionB is not null)
        {
            var convertedPayload = PayloadConverter.ConvertVersionBPayload(payloadVersionB, topic.MessageType, this.KnownMetricsStorage);

            if (convertedPayload is not Payload _)
            {
                throw new InvalidCastException("The metric cast didn't work properly.");
            }

            await this.HandleMessagesForVersionB(topic, convertedPayload);
        }
    }

    /// <summary>
    /// Handles the received Sparkplug B payloads.
    /// </summary>
    /// <param name="topic">The topic.</param>
    /// <param name="payload">The payload.</param>
    /// <exception cref="InvalidOperationException">Thrown if the topic is unknown or the device identifier is invalid.</exception>
    internal async Task HandleMessagesForVersionB(SparkplugMessageTopic topic, Payload payload)
    {
        // Filter out session number metric.
        var sessionNumberMetric = payload.Metrics.FirstOrDefault(m => m.Name == Constants.SessionNumberMetricName);
        var metricsWithoutSequenceMetric = payload.Metrics.Where(m => m.Name != Constants.SessionNumberMetricName);
        var filteredMetrics = this.KnownMetricsStorage.FilterMetrics(metricsWithoutSequenceMetric, topic.MessageType).ToList();

        if (sessionNumberMetric is not null)
        {
            filteredMetrics.Add(sessionNumberMetric);
        }

        this.UpdateReceivedMetricState(topic, filteredMetrics);

        // Handle messages.
        switch (topic.MessageType)
        {
            case SparkplugMessageType.NodeBirth:
                await this.FireNodeBirthReceived(new NodeBirthEventArgs(this, topic.GroupIdentifier, topic.EdgeNodeIdentifier,
                    filteredMetrics)
                {
                    SequenceNumber = payload.Seq
                });
                break;
            case SparkplugMessageType.DeviceBirth:
                await this.FireDeviceBirthReceived(new DeviceBirthEventArgs(this, topic.GroupIdentifier, topic.EdgeNodeIdentifier, topic.DeviceIdentifier!,
                    filteredMetrics)
                {
                    SequenceNumber = payload.Seq
                });
                break;
            case SparkplugMessageType.NodeData:
                await this.FireNodeDataReceived(new NodeDataEventArgs(this, topic.GroupIdentifier, topic.EdgeNodeIdentifier, filteredMetrics)
                {
                    SequenceNumber = payload.Seq
                });
                break;
            case SparkplugMessageType.DeviceData:
                await this.FireDeviceDataReceived(new DeviceDataEventArgs(this, topic.GroupIdentifier, topic.EdgeNodeIdentifier, topic.DeviceIdentifier!, filteredMetrics)
                {
                    SequenceNumber = payload.Seq
                });
                break;
            case SparkplugMessageType.NodeDeath:
                await this.FireNodeDeathReceived(topic.GroupIdentifier, topic.EdgeNodeIdentifier, sessionNumberMetric);
                break;
            case SparkplugMessageType.DeviceDeath:
                await this.FireDeviceDeathReceived(new DeviceEventArgs(this, topic.GroupIdentifier, topic.EdgeNodeIdentifier, topic.DeviceIdentifier!)
                {
                    SequenceNumber = payload.Seq
                });
                break;
        }
    }
}
