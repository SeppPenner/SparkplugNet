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
            this.FilterSourceMetrics(metrics, SparkplugMessageType.NodeCommand, groupIdentifier, edgeNodeIdentifier),
            this.LastSequenceNumber,
            this.LastSessionNumber,
            DateTimeOffset.Now);

        // Increment the sequence number.
        this.IncrementLastSequenceNumber();

        // Publish the message.
        await this.GetCommandClient(groupIdentifier, edgeNodeIdentifier).PublishAsync(dataMessage);
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
            this.FilterSourceMetrics(metrics, SparkplugMessageType.DeviceCommand, groupIdentifier, edgeNodeIdentifier, deviceIdentifier),
            this.LastSequenceNumber,
            this.LastSessionNumber,
            DateTimeOffset.Now);

        // Increment the sequence number.
        this.IncrementLastSequenceNumber();

        // Publish the message.
        await this.GetCommandClient(groupIdentifier, edgeNodeIdentifier).PublishAsync(dataMessage);
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
        var filteredMetrics = this.ProcessReceivedMetrics(topic, payload.Metrics, out var sessionNumberMetric);

        // Handle messages.
        switch (topic.MessageType)
        {
            case SparkplugMessageType.NodeBirth:
                await this.FireNodeBirthReceived(topic.GroupIdentifier, topic.EdgeNodeIdentifier,
                    filteredMetrics);
                break;
            case SparkplugMessageType.DeviceBirth:
                await this.FireDeviceBirthReceived(topic.GroupIdentifier, topic.EdgeNodeIdentifier, topic.DeviceIdentifier!,
                    filteredMetrics);
                break;
            case SparkplugMessageType.NodeData:
                await this.FireNodeDataReceived(topic.GroupIdentifier, topic.EdgeNodeIdentifier, filteredMetrics);
                break;
            case SparkplugMessageType.DeviceData:
                await this.FireDeviceDataReceived(topic.GroupIdentifier, topic.EdgeNodeIdentifier, topic.DeviceIdentifier!, filteredMetrics);
                break;
            case SparkplugMessageType.NodeDeath:
                await this.FireNodeDeathReceived(topic.GroupIdentifier, topic.EdgeNodeIdentifier, sessionNumberMetric);
                break;
            case SparkplugMessageType.DeviceDeath:
                await this.FireDeviceDeathReceived(topic.GroupIdentifier, topic.EdgeNodeIdentifier, topic.DeviceIdentifier!);
                break;
        }
    }
}
