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
    public SparkplugApplication(List<VersionAData.KuraMetric> knownMetrics, ILogger? logger = null) : base(knownMetrics, logger)
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
    protected override async Task PublishNodeCommandMessage(List<VersionAData.KuraMetric> metrics, string groupIdentifier, string edgeNodeIdentifier)
    {
        if (this.options is null)
        {
            throw new ArgumentNullException(nameof(this.options), "The options aren't set properly.");
        }

        if (this.KnownMetrics is not List<VersionAData.KuraMetric> knownMetrics)
        {
            throw new Exception("Invalid metric type specified for version A metric.");
        }

        // Remove all not known metrics.
        metrics.RemoveAll(m => knownMetrics.FirstOrDefault(m2 => m2.Name == m.Name) == default);

        // Remove the session number metric if a user might have added it.
        metrics.RemoveAll(m => m.Name == Constants.SessionNumberMetricName);

        // Get the data message.
        var dataMessage = SparkplugMessageGenerator.GetSparkPlugNodeCommandMessage(
            this.NameSpace,
            groupIdentifier,
            edgeNodeIdentifier,
            metrics,
            this.LastSequenceNumber,
            this.LastSessionNumber,
            DateTimeOffset.Now);

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
    protected override async Task PublishDeviceCommandMessage(List<VersionAData.KuraMetric> metrics, string groupIdentifier, string edgeNodeIdentifier, string deviceIdentifier)
    {
        if (this.options is null)
        {
            throw new ArgumentNullException(nameof(this.options), "The options aren't set properly.");
        }

        if (this.KnownMetrics is not List<VersionAData.KuraMetric> knownMetrics)
        {
            throw new Exception("Invalid metric type specified for version A metric.");
        }

        // Remove all not known metrics.
        metrics.RemoveAll(m => knownMetrics.FirstOrDefault(m2 => m2.Name == m.Name) == default);

        // Remove the session number metric if a user might have added it.
        metrics.RemoveAll(m => m.Name == Constants.SessionNumberMetricName);

        // Get the data message.
        var dataMessage = SparkplugMessageGenerator.GetSparkPlugDeviceCommandMessage(
            this.NameSpace,
            groupIdentifier,
            edgeNodeIdentifier,
            deviceIdentifier,
            metrics,
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
    /// Called when [application message received].
    /// </summary>
    /// <param name="topic">The topic.</param>
    /// <param name="payload">The payload.</param>
    /// <returns>
    /// A <see cref="T:System.Threading.Tasks.Task" /> representing any asynchronous operation.
    /// </returns>
    protected override Task OnMessageReceived(string topic, byte[] payload)
    {
        var payloadVersionA = PayloadHelper.Deserialize<VersionAProtoBuf.ProtoBufPayload>(payload);

        if (payloadVersionA != null)
        {
            var convertedPayload = PayloadConverter.ConvertVersionAPayload(payloadVersionA);
            this.HandleMessagesForVersionA(topic, convertedPayload);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the received messages for payload version A.
    /// </summary>
    /// <param name="topic">The topic.</param>
    /// <param name="payload">The payload.</param>
    /// <exception cref="ArgumentNullException">The known metrics are null.</exception>
    /// <exception cref="Exception">The metric is unknown.</exception>
    private void HandleMessagesForVersionA(string topic, VersionAData.Payload payload)
    {
        if (this.KnownMetrics is not List<VersionAData.KuraMetric> knownMetrics)
        {
            throw new ArgumentNullException(nameof(knownMetrics), "The known metrics are invalid.");
        }

        // If we have any not valid metric, throw an exception.
        var metricsWithoutSequenceMetric = payload.Metrics.Where(m => m.Name != Constants.SessionNumberMetricName);

        foreach (var metric in metricsWithoutSequenceMetric.Where(metric => knownMetrics.FirstOrDefault(m => m.Name == metric.Name) == default))
        {
            throw new Exception($"Metric {metric.Name} is an unknown metric.");
        }

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
    private void HandleDeviceMessage(string topic, VersionAData.Payload payload, SparkplugMetricStatus metricStatus, bool invokeDeviceDataCallback = false)
    {
        var splitTopic = topic.Split('/');
        if (splitTopic.Length != 5)
        {
            return;
        }

        var groupId = splitTopic[1];
        var nodeId = splitTopic[3];
        var deviceId = splitTopic[4];

        var metricState = new MetricState<VersionAData.KuraMetric>
        {
            MetricStatus = metricStatus
        };

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
    private void HandleNodeMessage(string topic, VersionAData.Payload payload, SparkplugMetricStatus metricStatus, bool invokeNodeDataCallback = false)
    {
        var splitTopic = topic.Split('/');
        if (splitTopic.Length != 4)
        {
            return;
        }

        var groupId = splitTopic[1];
        var nodeId = splitTopic[3];

        var metricState = new MetricState<VersionAData.KuraMetric>
        {
            MetricStatus = metricStatus
        };

        foreach (var payloadMetric in payload.Metrics)
        {
            if (payloadMetric is not VersionAData.KuraMetric convertedMetric)
            {
                throw new InvalidCastException("The metric cast didn't work properly.");
            }

            if (payloadMetric.Name is not null)
            {
                metricState.Metrics[payloadMetric.Name] = convertedMetric;
            }

            if (invokeNodeDataCallback)
            {
                this.OnNodeDataReceived?.Invoke(groupId, nodeId, convertedMetric);
            }
        }

        this.NodeStates[nodeId] = metricState;
    }
}
