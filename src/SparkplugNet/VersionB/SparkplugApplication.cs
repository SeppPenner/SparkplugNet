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
    /// <summary>
    /// Buffers out-of-order messages when a reorder timeout has been configured.
    /// </summary>
    private readonly SparkplugMessageOrder messageOrder = new();

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
            this.FilterSourceMetrics(metrics, SparkplugMessageType.NodeCommand, groupIdentifier, edgeNodeIdentifier),
            this.LastSequenceNumber,
            this.Options.AddSessionNumberToCommandMessages ? this.LastSessionNumber : null,
            DateTimeOffset.UtcNow,
            this.Options.IncludeDataTypesInCommandMessages);

        // Increment the sequence number.
        this.IncrementLastSequenceNumber();

        // Publish the message.
        await this.GetCommandClient(groupIdentifier, edgeNodeIdentifier).PublishAsync(dataMessage);
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
            this.FilterSourceMetrics(metrics, SparkplugMessageType.DeviceCommand, groupIdentifier, edgeNodeIdentifier, deviceIdentifier),
            this.LastSequenceNumber,
            this.Options.AddSessionNumberToCommandMessages ? this.LastSessionNumber : null,
            DateTimeOffset.UtcNow,
            this.Options.IncludeDataTypesInCommandMessages);

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
    protected override async Task OnMessageReceived(SparkplugMessageTopic topic, byte[] payload)
    {
        if (topic.MessageType is SparkplugMessageType.NodeCommand or SparkplugMessageType.DeviceCommand)
        {
            return;
        }

        var payloadVersionB = PayloadHelper.Deserialize<VersionBProtoBuf.ProtoBufPayload>(payload);
        if (payloadVersionB is not null)
        {
            if (this.messageGenerator.SpecificationVersion == SparkplugSpecificationVersion.Version30
                && this.Options?.MessageReorderTimeout is { } timeout)
            {
                try
                {
                    await this.messageOrder.Receive(topic, payloadVersionB, timeout, this.ProcessPayload, this.RequestRebirth);
                }
                catch (Exception exception)
                {
                    this.KnownMetricsStorage.Logger?.LogError(exception, "Could not process the ordered Sparkplug message.");
                    throw;
                }
            }
            else
            {
                await this.ProcessPayload(topic, payloadVersionB);
            }
        }
    }

    /// <inheritdoc />
    private protected override void ResetReceivedMessageOrder() => this.messageOrder.Reset();

    /// <inheritdoc />
    private protected override void ResetReceivedMessageOrder(string group, string node) => this.messageOrder.Reset(group, node);

    /// <summary>
    /// Converts and processes a payload against the definitions from the ordered birth messages.
    /// </summary>
    /// <param name="topic">The Sparkplug topic.</param>
    /// <param name="payload">The serialized Sparkplug message contents.</param>
    /// <returns>The processing task.</returns>
    private Task ProcessPayload(SparkplugMessageTopic topic, VersionBProtoBuf.ProtoBufPayload payload)
    {
        var definitions = this.GetSourceMetricStorage(topic.GroupIdentifier, topic.EdgeNodeIdentifier, topic.DeviceIdentifier);
        var convertedPayload = PayloadConverter.ConvertVersionBPayload(payload, topic.MessageType, definitions)
            ?? throw new InvalidCastException("The metric cast didn't work properly.");
        return this.HandleMessagesForVersionB(topic, convertedPayload);
    }

    /// <summary>
    /// Asks the edge node to announce its definitions again when a message is missing.
    /// </summary>
    /// <param name="topic">A topic from the edge node requesting recovery.</param>
    /// <returns>The publication task.</returns>
    private async Task RequestRebirth(SparkplugMessageTopic topic)
    {
        if (this.IsRunning && this.IsConnected)
        {
            try
            {
                await this.PublishNodeCommand([new(Constants.RebirthMetricName, VersionBDataTypeEnum.Boolean, true)],
                    topic.GroupIdentifier, topic.EdgeNodeIdentifier);
            }
            catch (Exception exception)
            {
                this.KnownMetricsStorage.Logger?.LogWarning(exception, "Could not request a Sparkplug rebirth.");
            }
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
        var filteredMetrics = this.ProcessReceivedMetrics(topic, payload.Metrics, out var sessionNumberMetric, payload.Timestamp);

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
