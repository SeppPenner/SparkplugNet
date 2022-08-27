// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugApplication.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   Defines the SparkplugApplication type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.VersionB;

using SparkplugNet.Core;

/// <inheritdoc cref="SparkplugApplicationBase{T}"/>
public class SparkplugApplication : SparkplugApplicationBase<VersionBData.Metric>
{
    /// <inheritdoc cref="SparkplugApplicationBase{T}"/>
    /// <summary>
    /// Initializes a new instance of the <see cref="SparkplugApplication"/> class.
    /// </summary>
    /// <param name="knownMetrics">The known metrics.</param>
    /// <param name="logger">The logger.</param>
    public SparkplugApplication(IEnumerable<VersionBData.Metric> knownMetrics, ILogger? logger = null) : base(knownMetrics, logger)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SparkplugApplication"/> class.
    /// </summary>
    /// <param name="knownMetricsStorage">The metric names.</param>
    /// <param name="logger">The logger.</param>
    /// /// <seealso cref="SparkplugApplicationBase{T}"/>
    public SparkplugApplication(KnownMetricStorage knownMetricsStorage, ILogger? logger = null) : base(knownMetricsStorage, logger)
    {
    }

    /// <summary>
    /// Publishes a version B node command message.
    /// </summary>
    /// <param name="metrics">The metrics.</param>
    /// <param name="groupIdentifier">The group identifier.</param>
    /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
    /// <exception cref="ArgumentNullException">The options are null.</exception>
    /// <exception cref="Exception">An invalid metric type was specified.</exception>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    protected override async Task PublishNodeCommandMessage(IEnumerable<VersionBData.Metric> metrics, string groupIdentifier, string edgeNodeIdentifier)
    {
        if (this.options is null)
        {
            throw new ArgumentNullException(nameof(this.options), "The options arent't set properly.");
        }

        if (this.KnownMetrics is not IEnumerable<VersionBData.Metric> knownMetrics)
        {
            throw new Exception("Invalid metric type specified for version B metric.");
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
    /// <exception cref="ArgumentNullException">The options are null.</exception>
    /// <exception cref="Exception">An invalid metric type was specified.</exception>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    protected override async Task PublishDeviceCommandMessage(IEnumerable<VersionBData.Metric> metrics, string groupIdentifier, string edgeNodeIdentifier, string deviceIdentifier)
    {
        if (this.options is null)
        {
            throw new ArgumentNullException(nameof(this.options), "The options aren't set properly.");
        }

        if (this.KnownMetrics is not IEnumerable<VersionBData.Metric> knownMetrics)
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
    /// Called when [application message received].
    /// </summary>
    /// <param name="topic">The topic.</param>
    /// <param name="payload">The payload.</param>
    /// <returns>
    /// A <see cref="T:System.Threading.Tasks.Task" /> representing any asynchronous operation.
    /// </returns>
    protected override Task OnMessageReceived(string topic, byte[] payload)
    {
        var payloadVersionB = PayloadHelper.Deserialize<VersionBProtoBuf.ProtoBufPayload>(payload);

        if (payloadVersionB != null)
        {
            var convertedPayload = PayloadConverter.ConvertVersionBPayload(payloadVersionB);
            this.HandleMessagesForVersionB(topic, convertedPayload);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the received messages for payload version B.
    /// </summary>
    /// <param name="topic">The topic.</param>
    /// <param name="payload">The payload.</param>
    /// <exception cref="ArgumentNullException">The known metrics are null.</exception>
    /// <exception cref="Exception">The metric is unknown.</exception>
    private void HandleMessagesForVersionB(string topic, VersionBData.Payload payload)
    {
        if (this.KnownMetrics is not IEnumerable<VersionBData.Metric> knownMetrics)
        {
            throw new ArgumentNullException(nameof(knownMetrics), "The known metrics are invalid.");
        }

        // If we have any not valid metric, throw an exception.
        var metricsWithoutSequenceMetric = payload.Metrics.Where(m => m.Name != Constants.SessionNumberMetricName);

        this.KnownMetricsStorage.ValidateIncommingMetrics(metricsWithoutSequenceMetric);

        if (topic.Contains(SparkplugMessageType.NodeBirth.GetDescription()))
        {
            this.HandleNodeMessage(topic, payload, SparkplugMetricStatus.Online);
        }

        if (topic.Contains(SparkplugMessageType.NodeDeath.GetDescription()))
        {
            this.HandleNodeMessage(topic, payload, SparkplugMetricStatus.Offline);
        }

        if (topic.Contains(SparkplugMessageType.DeviceBirth.GetDescription()))
        {
            this.HandleDeviceMessage(topic, payload, SparkplugMetricStatus.Online);
        }

        if (topic.Contains(SparkplugMessageType.DeviceDeath.GetDescription()))
        {
            this.HandleDeviceMessage(topic, payload, SparkplugMetricStatus.Offline);
        }

        if (topic.Contains(SparkplugMessageType.NodeData.GetDescription()))
        {
            this.HandleNodeMessage(topic, payload, SparkplugMetricStatus.Online, true);
        }

        if (topic.Contains(SparkplugMessageType.DeviceData.GetDescription()))
        {
            this.HandleDeviceMessage(topic, payload, SparkplugMetricStatus.Online, true);
        }
    }

    /// <summary>
    /// Handles the device message.
    /// </summary>
    /// <param name="topic">The topic.</param>
    /// <param name="payload">The payload.</param>
    /// <param name="metricStatus">The metric status.</param>
    /// <param name="invokeDeviceDataCallback">A value indicating whether the device data callback is invoked or not.</param>
    /// <exception cref="InvalidCastException">The metric cast is invalid.</exception>
    private void HandleDeviceMessage(string topic, VersionBData.Payload payload, SparkplugMetricStatus metricStatus, bool invokeDeviceDataCallback = false)
    {
        var splitTopic = topic.Split('/');
        if (splitTopic.Length != 5)
        {
            return;
        }

        var groupId = splitTopic[1];
        var nodeId = splitTopic[3];
        var deviceId = splitTopic[4];

        var metricState = new MetricState<VersionBData.Metric>
        {
            MetricStatus = metricStatus
        };

        foreach (var payloadMetric in payload.Metrics)
        {
            if (payloadMetric is not VersionBData.Metric convertedMetric)
            {
                throw new InvalidCastException("The metric cast didn't work properly.");
            }

            metricState.Metrics[payloadMetric.Name] = convertedMetric;

            if (invokeDeviceDataCallback)
            {
                this.OnDeviceDataReceived?.Invoke(groupId, nodeId, deviceId, convertedMetric);
            }
        }

        this.DeviceStates[deviceId] = metricState;
    }

    /// <summary>
    /// Handles the node message.
    /// </summary>
    /// <param name="topic">The topic.</param>
    /// <param name="payload">The payload.</param>
    /// <param name="metricStatus">The metric status.</param>
    /// <param name="invokeNodeDataCallback">A value indicating whether the node data callback is invoked or not.</param>
    /// <exception cref="InvalidCastException">The metric cast is invalid.</exception>
    private void HandleNodeMessage(string topic, VersionBData.Payload payload, SparkplugMetricStatus metricStatus, bool invokeNodeDataCallback = false)
    {
        var splitTopic = topic.Split('/');
        if (splitTopic.Length != 4)
        {
            return;
        }

        var groupId = splitTopic[1];
        var nodeId = splitTopic[3];

        var metricState = new MetricState<VersionBData.Metric>
        {
            MetricStatus = metricStatus
        };

        foreach (var payloadMetric in payload.Metrics)
        {
            if (payloadMetric is not VersionBData.Metric convertedMetric)
            {
                throw new InvalidCastException("The metric cast didn't work properly.");
            }

            metricState.Metrics[payloadMetric.Name] = convertedMetric;

            if (invokeNodeDataCallback)
            {
                this.OnNodeDataReceived?.Invoke(groupId, nodeId, convertedMetric);
            }
        }

        this.NodeStates[nodeId] = metricState;
    }
}