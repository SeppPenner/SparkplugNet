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
    public ConcurrentDictionary<string, List<T>> KnownDevices { get; } = new ConcurrentDictionary<string,List<T>>();

    /// <summary>
    /// Gets or sets the callback for the device birth received event.
    /// </summary>
    public Action<KeyValuePair<string, List<T>>>? DeviceBirthReceived { get; set; } = null;

    /// <summary>
    /// Gets or sets the callback for the device death received event.
    /// </summary>
    public Action<string>? DeviceDeathReceived { get; set; } = null;

    /// <summary>
    /// Publishes a device birth message to the MQTT broker.
    /// </summary>
    /// <param name="knownMetrics">The known metrics.</param>
    /// <param name="deviceIdentifier">The device identifier.</param>
    /// <exception cref="ArgumentNullException">The options are null.</exception>
    /// <exception cref="Exception">The MQTT client is not connected.</exception>
    /// <returns>A <see cref="MqttClientPublishResult"/>.</returns>
    public async Task<MqttClientPublishResult> PublishDeviceBirthMessage(List<T> knownMetrics, string deviceIdentifier)
    {
        if (this.options is null)
        {
            throw new ArgumentNullException(nameof(this.options), "The options aren't set properly.");
        }

        if (!this.Client.IsConnected)
        {
            throw new Exception("The MQTT client is not connected, please try again.");
        }

        // Get the device birth message.
        var deviceBirthMessage = this.MessageGenerator.GetSparkPlugDeviceBirthMessage(
            this.NameSpace,
            this.options.GroupIdentifier,
            this.options.EdgeNodeIdentifier,
            deviceIdentifier,
            knownMetrics,
            this.LastSequenceNumber,
            this.LastSessionNumber,
            DateTimeOffset.Now);

        // Increment the sequence number.
        this.IncrementLastSequenceNumber();

        // Add the known metrics to the known devices.
        this.KnownDevices.TryAdd(deviceIdentifier, knownMetrics);

        // Invoke the device birth event.
        this.DeviceBirthReceived?.Invoke(new KeyValuePair<string, List<T>>(deviceIdentifier, knownMetrics));

        // Publish the message.
        this.options.CancellationToken ??= CancellationToken.None;
        return await this.Client.PublishAsync(deviceBirthMessage, this.options.CancellationToken.Value);
    }

    /// <summary>
    /// Publishes some metrics for the device.
    /// </summary>
    /// <param name="metrics">The metrics.</param>
    /// <param name="deviceIdentifier">The device identifier.</param>
    /// <exception cref="ArgumentNullException">The options are null.</exception>
    /// <exception cref="Exception">The MQTT client is not connected or the device is unknown or an invalid metric type was specified.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The namespace is out of range.</exception>
    /// <returns>A <see cref="MqttClientPublishResult"/>.</returns>
    public async Task<MqttClientPublishResult> PublishDeviceData(List<T> metrics, string deviceIdentifier)
    {
        if (this.options is null)
        {
            throw new ArgumentNullException(nameof(this.options), "The options aren't set properly.");
        }

        if (!this.Client.IsConnected)
        {
            throw new Exception("The MQTT client is not connected, please try again.");
        }

        if (!this.KnownDevices.ContainsKey(deviceIdentifier))
        {
            throw new Exception("The device is unknown, please publish a device birth message first.");
        }

        switch (this.NameSpace)
        {
            case SparkplugNamespace.VersionA:
                {
                    if (metrics is not List<VersionAData.KuraMetric> convertedMetrics)
                    {
                        throw new Exception("Invalid metric type specified for version A metric.");
                    }

                    return await this.PublishVersionAMessageForDevice(convertedMetrics, deviceIdentifier);
                }
            case SparkplugNamespace.VersionB:
                {
                    if (metrics is not List<VersionBData.Metric> convertedMetrics)
                    {
                        throw new Exception("Invalid metric type specified for version B metric.");
                    }

                    return await this.PublishVersionBMessageForDevice(convertedMetrics, deviceIdentifier);
                }
            default:
                throw new ArgumentOutOfRangeException(nameof(this.NameSpace), "The namespace is invalid.");
        }
    }

    /// <summary>
    /// Publishes a device death message to the MQTT broker.
    /// </summary>
    /// <param name="deviceIdentifier">The device identifier.</param>
    /// <exception cref="ArgumentNullException">The options are null.</exception>
    /// <exception cref="Exception">The MQTT client is not connected or the device is unknown.</exception>
    /// <returns>A <see cref="MqttClientPublishResult"/>.</returns>
    public async Task<MqttClientPublishResult> PublishDeviceDeathMessage(string deviceIdentifier)
    {
        if (this.options is null)
        {
            throw new ArgumentNullException(nameof(this.options), "The options aren't set properly.");
        }

        if (!this.Client.IsConnected)
        {
            throw new Exception("The MQTT client is not connected, please try again.");
        }

        if (!this.KnownDevices.ContainsKey(deviceIdentifier))
        {
            throw new Exception("The device is unknown, please publish a device birth message first.");
        }

        // Get the device death message.
        var deviceDeathMessage = this.MessageGenerator.GetSparkPlugDeviceDeathMessage(
            this.NameSpace,
            this.options.GroupIdentifier,
            this.options.EdgeNodeIdentifier,
            deviceIdentifier,
            this.LastSequenceNumber,
            this.LastSessionNumber,
            DateTimeOffset.Now);

        // Increment the sequence number.
        this.IncrementLastSequenceNumber();

        // Invoke the device death event.
        this.DeviceDeathReceived?.Invoke(deviceIdentifier);

        // Publish the message.
        this.options.CancellationToken ??= CancellationToken.None;
        return await this.Client.PublishAsync(deviceDeathMessage, this.options.CancellationToken.Value);
    }

    /// <summary>
    /// Publishes version A metrics for a device.
    /// </summary>
    /// <param name="metrics">The metrics.</param>
    /// <param name="deviceIdentifier">The device identifier.</param>
    /// <exception cref="ArgumentNullException">The options are null.</exception>
    /// <exception cref="Exception">The device is unknown or an invalid metric type was specified.</exception>
    /// <returns>A <see cref="MqttClientPublishResult"/>.</returns>
    private async Task<MqttClientPublishResult> PublishVersionAMessageForDevice(List<VersionAData.KuraMetric> metrics, string deviceIdentifier)
    {
        if (this.options is null)
        {
            throw new ArgumentNullException(nameof(this.options), "The options aren't set properly.");
        }

        if (!this.KnownDevices.ContainsKey(deviceIdentifier))
        {
            throw new Exception("The device is unknown, please publish a device birth message first.");
        }

        var deviceMetrics = this.KnownDevices[deviceIdentifier];

        if (deviceMetrics is not List<VersionAData.KuraMetric> knownMetrics)
        {
            throw new Exception("Invalid metric type specified for version A metric.");
        }

        // Remove all not known metrics.
        metrics.RemoveAll(m => knownMetrics.FirstOrDefault(m2 => m2.Name == m.Name) == default);

        // Remove the session number metric if a user might have added it.
        metrics.RemoveAll(m => m.Name == Constants.SessionNumberMetricName);

        // Get the data message.
        var dataMessage = this.MessageGenerator.GetSparkPlugDeviceDataMessage(
            this.NameSpace,
            this.options.GroupIdentifier,
            this.options.EdgeNodeIdentifier,
            deviceIdentifier,
            metrics,
            this.LastSequenceNumber,
            this.LastSessionNumber,
            DateTimeOffset.Now);

        // Increment the sequence number.
        this.IncrementLastSequenceNumber();

        // Publish the message.
        return await this.Client.PublishAsync(dataMessage);
    }

    /// <summary>
    /// Publishes version B metrics for a device.
    /// </summary>
    /// <param name="metrics">The metrics.</param>
    /// <param name="deviceIdentifier">The device identifier.</param>
    /// <exception cref="ArgumentNullException">The options are null.</exception>
    /// <exception cref="Exception">The device is unknown or an invalid metric type was specified.</exception>
    /// <returns>A <see cref="MqttClientPublishResult"/>.</returns>
    private async Task<MqttClientPublishResult> PublishVersionBMessageForDevice(List<VersionBData.Metric> metrics, string deviceIdentifier)
    {
        if (this.options is null)
        {
            throw new ArgumentNullException(nameof(this.options), "The options aren't set properly.");
        }

        if (!this.KnownDevices.ContainsKey(deviceIdentifier))
        {
            throw new Exception("The device is unknown, please publish a device birth message first.");
        }

        var deviceMetrics = this.KnownDevices[deviceIdentifier];

        if (deviceMetrics is not List<VersionBData.Metric> knownMetrics)
        {
            throw new Exception("Invalid metric type specified for version B metric.");
        }

        // Remove all not known metrics.
        metrics.RemoveAll(m => knownMetrics.FirstOrDefault(m2 => m2.Name == m.Name) == default);

        // Remove the session number metric if a user might have added it.
        metrics.RemoveAll(m => m.Name == Constants.SessionNumberMetricName);

        // Get the data message.
        var dataMessage = this.MessageGenerator.GetSparkPlugDeviceDataMessage(
            this.NameSpace,
            this.options.GroupIdentifier,
            this.options.EdgeNodeIdentifier,
            deviceIdentifier,
            metrics,
            this.LastSequenceNumber,
            this.LastSessionNumber,
            DateTimeOffset.Now);

        // Increment the sequence number.
        this.IncrementLastSequenceNumber();

        // Publish the message.
        return await this.Client.PublishAsync(dataMessage);
    }
}
