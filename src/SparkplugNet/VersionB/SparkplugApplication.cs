// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugApplication.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class that handles a Sparkplug application.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.VersionB;

using System.Diagnostics.Metrics;

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
            this.LastSessionNumber,
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
            this.LastSessionNumber,
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

        if (payloadVersionB == null) { return; }

        ConcurrentDictionary<string, Metric>? metrics = null;

        if (!(topic.MessageType == SparkplugMessageType.NodeBirth || topic.MessageType == SparkplugMessageType.DeviceBirth))
        {
            // Get known metrics
            if (!this.GroupStates.TryGetValue(topic.GroupIdentifier, out var groupState)) { return; }

            if (!groupState.NodeStates.TryGetValue(topic.EdgeNodeIdentifier, out var nodeState)) { return; }

            if (topic.DeviceIdentifier is null)
            {
                metrics = nodeState.Metrics;
            }
            else if (nodeState.DeviceStates.TryGetValue(topic.DeviceIdentifier, out var metricState))
            {
                metrics = metricState.Metrics;
            }
            else
            {
                return;
            }
        }

        var convertedPayload = PayloadConverter.ConvertVersionBPayload(payloadVersionB, metrics);

        if (convertedPayload is not Payload _)
        {
            throw new InvalidCastException("The metric cast didn't work properly.");
        }

        await this.HandleMessagesForVersionB(topic, convertedPayload);
    }

    /// <summary>
    /// Handles the received messages for payload version A.
    /// </summary>
    /// <param name="topic">The topic.</param>
    /// <param name="payload">The payload.</param>
    /// <exception cref="InvalidOperationException">Thrown if the topic is unknown or the device identifier is invalid.</exception>
    private async Task HandleMessagesForVersionB(SparkplugMessageTopic topic, Payload payload)
    {
        // Filter out session number metric.
        var sessionNumberMetric = payload.Metrics.FirstOrDefault(m => m.Name == Constants.SessionNumberMetricName);
        var metrics = payload.Metrics.Where(m => m.Name != Constants.SessionNumberMetricName).ToList();
        // var filteredMetrics = this.KnownMetricsStorage.FilterMetrics(metricsWithoutSequenceMetric, topic.MessageType).ToList();

        if (sessionNumberMetric is not null)
        {
            metrics.Add(sessionNumberMetric);
        }

        // Handle messages.
        switch (topic.MessageType)
        {
            case SparkplugMessageType.NodeBirth:
                await this.FireNodeBirthReceived(topic.GroupIdentifier, topic.EdgeNodeIdentifier,
                    this.ProcessPayload(topic, metrics, SparkplugMetricStatus.Online));
                break;
            case SparkplugMessageType.DeviceBirth:
                if (string.IsNullOrWhiteSpace(topic.DeviceIdentifier))
                {
                    throw new InvalidOperationException($"The device identifier is invalid!");
                }

                await this.FireDeviceBirthReceived(topic.GroupIdentifier, topic.EdgeNodeIdentifier, topic.DeviceIdentifier,
                    this.ProcessPayload(topic, metrics, SparkplugMetricStatus.Online));
                break;
            case SparkplugMessageType.NodeData:
                var nodeDataMetrics = this.ProcessPayload(topic, metrics, SparkplugMetricStatus.Online);
                await this.FireNodeDataReceived(topic.GroupIdentifier, topic.EdgeNodeIdentifier, nodeDataMetrics);
                break;
            case SparkplugMessageType.DeviceData:
                if (string.IsNullOrWhiteSpace(topic.DeviceIdentifier))
                {
                    throw new InvalidOperationException($"Topic {topic} is invalid!");
                }

                var deviceDataMetrics = this.ProcessPayload(topic, metrics, SparkplugMetricStatus.Online);
                await this.FireDeviceDataReceived(topic.GroupIdentifier, topic.EdgeNodeIdentifier, topic.DeviceIdentifier, deviceDataMetrics);
                break;
            case SparkplugMessageType.NodeDeath:
                this.ProcessPayload(topic, metrics, SparkplugMetricStatus.Offline);
                await this.FireNodeDeathReceived(topic.GroupIdentifier, topic.EdgeNodeIdentifier, sessionNumberMetric);
                break;
            case SparkplugMessageType.DeviceDeath:
                if (string.IsNullOrWhiteSpace(topic.DeviceIdentifier))
                {
                    throw new InvalidOperationException($"Topic {topic} is invalid!");
                }

                this.ProcessPayload(topic, metrics, SparkplugMetricStatus.Offline);
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
    /// <exception cref="InvalidOperationException">Thrown if any identifier is invalid.</exception>
    /// <exception cref="InvalidCastException">Thrown if the metric cast is invalid.</exception>
    private IEnumerable<Metric> ProcessPayload(SparkplugMessageTopic topic, List<Metric> metrics, SparkplugMetricStatus metricStatus)
    {
        // Check group id.
        if (string.IsNullOrWhiteSpace(topic.GroupIdentifier))
        {
            throw new InvalidOperationException($"The group identifier is invalid {topic.GroupIdentifier}.");
        }

        if (!this.GroupStates.ContainsKey(topic.GroupIdentifier))
        {
            this.GroupStates[topic.GroupIdentifier] = new GroupState<Metric>();
        }

        NodeState<Metric> metricState = new()
        {
            MetricStatus = metricStatus
        };

        // Check node id.
        if (string.IsNullOrWhiteSpace(topic.EdgeNodeIdentifier))
        {
            throw new InvalidOperationException($"The edge node identifier is invalid {topic.EdgeNodeIdentifier}.");
        }

        if (!string.IsNullOrWhiteSpace(topic.DeviceIdentifier))
        {
            // If the group doesn't contain the node, create a new node.
            if (!this.GroupStates[topic.GroupIdentifier].NodeStates.ContainsKey(topic.EdgeNodeIdentifier))
            {
                this.GroupStates[topic.GroupIdentifier]
                    .NodeStates[topic.EdgeNodeIdentifier] = metricState;
            }

            if (this.GroupStates[topic.GroupIdentifier]
                .NodeStates[topic.EdgeNodeIdentifier]
                .DeviceStates.TryGetValue(topic.DeviceIdentifier, out var metric))
            {
                metricState.Metrics = metric.Metrics;
            }

            this.GroupStates[topic.GroupIdentifier]
                .NodeStates[topic.EdgeNodeIdentifier]
                .DeviceStates[topic.DeviceIdentifier] = metricState;
        }
        else
        {
            if (this.GroupStates[topic.GroupIdentifier]
                .NodeStates.TryGetValue(topic.EdgeNodeIdentifier, out var metric))
            {
                metricState.Metrics = metric.Metrics;
                metricState.DeviceStates = metric.DeviceStates;
            }

            this.GroupStates[topic.GroupIdentifier]
                .NodeStates[topic.EdgeNodeIdentifier] = metricState;
        }

        foreach (var payloadMetric in metrics)
        {
            if (payloadMetric is not Metric convertedMetric)
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