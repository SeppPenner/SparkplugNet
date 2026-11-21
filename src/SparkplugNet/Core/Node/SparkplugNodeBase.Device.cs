// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugNodeBase.Device.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class that handles a Sparkplug node.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Core.Node;

/// <inheritdoc cref="SparkplugBase{T}"/>
/// <summary>
/// A class that handles a Sparkplug node.
/// </summary>
/// <seealso cref="SparkplugBase{T}"/>
public partial class SparkplugNodeBase<T>
{
    /// <summary>
    /// Gets the known devices.
    /// </summary>
    public ConcurrentDictionary<string, KnownMetricStorage> KnownDevices { get; } = new();

    /// <summary>
    /// Gets the metric definitions for the recipient of an incoming command.
    /// </summary>
    /// <param name="topic">The command topic.</param>
    /// <returns>The recipient's metrics, if the topic addresses a known recipient.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the device identifier is invalid.</exception>
    private protected KnownMetricStorage? GetCommandMetricStorage(SparkplugMessageTopic topic)
    {
        if (topic.MessageType == SparkplugMessageType.NodeCommand)
        {
            return this.KnownMetricsStorage;
        }

        if (topic.MessageType != SparkplugMessageType.DeviceCommand)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(topic.DeviceIdentifier))
        {
            throw new InvalidOperationException($"Topic {topic} is invalid!");
        }

        return this.KnownDevices.GetValueOrDefault(topic.DeviceIdentifier);
    }

    /// <summary>
    /// Filters a decoded command and dispatches it to the corresponding event.
    /// </summary>
    /// <param name="topic">The command topic.</param>
    /// <param name="metrics">The decoded command metrics.</param>
    /// <param name="metricStorage">The recipient's metric definitions.</param>
    /// <returns>The event dispatch task.</returns>
    private protected Task HandleCommand(SparkplugMessageTopic topic, IEnumerable<T> metrics, KnownMetricStorage metricStorage)
    {
        var sessionNumberMetric = metrics.FirstOrDefault(m => m.Name == Constants.SessionNumberMetricName);
        var metricsWithoutSessionNumber = metrics.Where(m => m.Name != Constants.SessionNumberMetricName);
        var filteredMetrics = metricStorage.FilterMetrics(metricsWithoutSessionNumber, topic.MessageType).ToList();

        if (sessionNumberMetric is not null)
        {
            filteredMetrics.Add(sessionNumberMetric);
        }

        return topic.MessageType == SparkplugMessageType.DeviceCommand
            ? this.FireDeviceCommandReceived(topic.DeviceIdentifier!, filteredMetrics)
            : this.FireNodeCommandReceived(filteredMetrics);
    }

    /// <summary>
    /// Publishes a device birth message to the MQTT broker.
    /// </summary>
    /// <param name="knownMetrics">The known metrics.</param>
    /// <param name="deviceIdentifier">The device identifier.</param>
    /// <param name="logger">The logger.</param>
    /// <exception cref="ArgumentNullException">Thrown if the options are null.</exception>
    /// <exception cref="Exception">Thrown if the MQTT client is not connected.</exception>
    /// <returns>A <see cref="MqttClientPublishResult"/>.</returns>
    public Task<MqttClientPublishResult> PublishDeviceBirthMessage(
        List<T> knownMetrics,
        string deviceIdentifier,
        ILogger<KnownMetricStorage>? logger = null) =>
        this.PublishDeviceBirthMessage(knownMetrics, deviceIdentifier, logger, updateKnownDevice: true);

    /// <summary>
    /// Publishes a device birth and optionally replaces the device's metric definitions.
    /// </summary>
    /// <param name="knownMetrics">The known metrics.</param>
    /// <param name="deviceIdentifier">The device identifier.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="updateKnownDevice">Whether a new metric definition was supplied.</param>
    /// <returns>The publish result.</returns>
    private async Task<MqttClientPublishResult> PublishDeviceBirthMessage(
        List<T> knownMetrics,
        string deviceIdentifier,
        ILogger<KnownMetricStorage>? logger,
        bool updateKnownDevice)
    {
        if (this.Options is null)
        {
            throw new ArgumentNullException(nameof(this.Options), "The options aren't set properly.");
        }

        if (!this.client.IsConnected)
        {
            throw new Exception("The MQTT client is not connected, please try again.");
        }

        this.EnsurePrimaryHostReady();
        // Get the device birth message.
        var deviceBirthMessage = this.messageGenerator.GetSparkplugDeviceBirthMessage(
            this.NameSpace,
            this.Options.GroupIdentifier,
            this.Options.EdgeNodeIdentifier,
            deviceIdentifier,
            knownMetrics,
            this.LastSequenceNumber,
            this.LastSessionNumber,
            DateTimeOffset.UtcNow);

        // Increment the sequence number.
        this.IncrementLastSequenceNumber();

        KnownMetricStorage? previousStorage = null;
        KnownMetricStorage? newStorage = null;
        if (updateKnownDevice)
        {
            newStorage = new KnownMetricStorage(knownMetrics, logger);
            this.KnownDevices.TryGetValue(deviceIdentifier, out previousStorage);
            this.KnownDevices[deviceIdentifier] = newStorage;
        }

        var published = false;
        try
        {
            await this.FireDeviceBirthPublishing(deviceIdentifier, knownMetrics);
            this.EnsurePrimaryHostReady();

            this.Options.CancellationToken ??= SystemCancellationToken.None;
            var result = await this.client.PublishAsync(deviceBirthMessage, this.Options.CancellationToken.Value);
            published = result.IsSuccess;
            return result;
        }
        finally
        {
            if (!published && newStorage is not null)
            {
                // Restore the previous definitions only if this birth still owns the entry.
                if (previousStorage is null)
                {
                    this.KnownDevices.TryRemove(new KeyValuePair<string, KnownMetricStorage>(deviceIdentifier, newStorage));
                }
                else
                {
                    this.KnownDevices.TryUpdate(deviceIdentifier, previousStorage, newStorage);
                }
            }
        }
    }

    /// <summary>
    /// Publishes some metrics for the device.
    /// </summary>
    /// <param name="metrics">The metrics.</param>
    /// <param name="deviceIdentifier">The device identifier.</param>
    /// <exception cref="ArgumentNullException">Thrown if the options are null.</exception>
    /// <exception cref="Exception">Thrown if the MQTT client is not connected or the device is unknown or an invalid metric type was specified.</exception>
    /// <returns>A <see cref="MqttClientPublishResult"/>.</returns>
    public Task<MqttClientPublishResult> PublishDeviceData(IEnumerable<T> metrics, string deviceIdentifier)
    {
        if (this.Options is null)
        {
            throw new ArgumentNullException(nameof(this.Options), "The options aren't set properly.");
        }

        if (!this.client.IsConnected)
        {
            throw new Exception("The MQTT client is not connected, please try again.");
        }

        if (!this.KnownDevices.ContainsKey(deviceIdentifier))
        {
            throw new Exception("The device is unknown, please publish a device birth message first.");
        }

        this.EnsurePrimaryHostReady();
        return this.PublishMessageForDevice(metrics, deviceIdentifier);
    }

    /// <summary>
    /// Publishes a device death message to the MQTT broker.
    /// </summary>
    /// <param name="deviceIdentifier">The device identifier.</param>
    /// <exception cref="ArgumentNullException">Thrown if the options are null.</exception>
    /// <exception cref="Exception">Thrown if the MQTT client is not connected or the device is unknown.</exception>
    /// <returns>A <see cref="MqttClientPublishResult"/>.</returns>
    public async Task<MqttClientPublishResult> PublishDeviceDeathMessage(string deviceIdentifier)
    {
        if (this.Options is null)
        {
            throw new ArgumentNullException(nameof(this.Options), "The options aren't set properly.");
        }

        if (!this.client.IsConnected)
        {
            throw new Exception("The MQTT client is not connected, please try again.");
        }

        if (!this.KnownDevices.ContainsKey(deviceIdentifier))
        {
            throw new Exception("The device is unknown, please publish a device birth message first.");
        }

        // Get the device death message.
        var deviceDeathMessage = this.messageGenerator.GetSparkplugDeviceDeathMessage(
            this.NameSpace,
            this.Options.GroupIdentifier,
            this.Options.EdgeNodeIdentifier,
            deviceIdentifier,
            this.LastSequenceNumber,
            this.LastSessionNumber,
            DateTimeOffset.UtcNow);

        // Increment the sequence number.
        this.IncrementLastSequenceNumber();

        // Invoke the device death event.
        await this.FireDeviceDeathPublishing(deviceIdentifier);

        this.KnownDevices.TryRemove(deviceIdentifier, out _);
        // Publish the message.
        this.Options.CancellationToken ??= SystemCancellationToken.None;
        return await this.client.PublishAsync(deviceDeathMessage, this.Options.CancellationToken.Value);
    }

    /// <summary>
    /// Publishes the message for a device.
    /// </summary>
    /// <param name="metrics">The metrics.</param>
    /// <param name="deviceIdentifier">The device identifier.</param>
    /// <exception cref="ArgumentNullException">Thrown if the options are null.</exception>
    /// <exception cref="Exception">Thrown if the device is unknown or an invalid metric type was specified.</exception>
    /// <returns>A <see cref="MqttClientPublishResult"/>.</returns>
    protected virtual async Task<MqttClientPublishResult> PublishMessageForDevice(IEnumerable<T> metrics, string deviceIdentifier)
    {
        if (this.Options is null)
        {
            throw new ArgumentNullException(nameof(this.Options), "The options aren't set properly.");
        }

        if (!this.KnownDevices.TryGetValue(deviceIdentifier, out KnownMetricStorage? deviceMetricStorage))
        {
            throw new Exception("The device is unknown, please publish a device birth message first.");
        }

        if (deviceMetricStorage is null)
        {
            throw new ArgumentNullException(deviceIdentifier, $"The known metrics for the device {deviceIdentifier} aren't set properly.");
        }

        // Get the device data message.
        var deviceDataMessage = this.messageGenerator.GetSparkplugDeviceDataMessage(
            this.NameSpace,
            this.Options.GroupIdentifier,
            this.Options.EdgeNodeIdentifier,
            deviceIdentifier,
            deviceMetricStorage.FilterMetrics(metrics, SparkplugMessageType.DeviceData),
            this.LastSequenceNumber,
            this.LastSessionNumber,
            DateTimeOffset.UtcNow);

        // Increment the sequence number.
        this.IncrementLastSequenceNumber();

        // Publish the message.
        return await this.client.PublishAsync(deviceDataMessage);
    }
}
