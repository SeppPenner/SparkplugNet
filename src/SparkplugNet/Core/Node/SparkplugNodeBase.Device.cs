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
    /// Publishes a device birth message to the MQTT broker.
    /// </summary>
    /// <param name="knownMetrics">The known metrics.</param>
    /// <param name="deviceIdentifier">The device identifier.</param>
    /// <exception cref="ArgumentNullException">Thrown if the options are null.</exception>
    /// <exception cref="Exception">Thrown if the MQTT client is not connected.</exception>
    /// <returns>A <see cref="MqttClientPublishResult"/>.</returns>
    public async Task<MqttClientPublishResult> PublishDeviceBirthMessage(List<T> knownMetrics, string deviceIdentifier)
    {
        if (this.Options is null)
        {
            throw new ArgumentNullException(nameof(this.Options), "The options aren't set properly.");
        }

        if (!this.Client.IsConnected)
        {
            throw new Exception("The MQTT client is not connected, please try again.");
        }

        // Get the device birth message.
        var deviceBirthMessage = this.MessageGenerator.GetSparkPlugDeviceBirthMessage(
            this.NameSpace,
            this.Options.GroupIdentifier,
            this.Options.EdgeNodeIdentifier,
            deviceIdentifier,
            knownMetrics,
            this.LastSequenceNumber,
            this.LastSessionNumber,
            DateTimeOffset.Now);

        // Increment the sequence number.
        this.IncrementLastSequenceNumber();

        // Add the known metrics to the known devices.
        this.KnownDevices.TryAdd(deviceIdentifier, new KnownMetricStorage(knownMetrics));

        // Invoke the device birth event.
        await this.FireDeviceBirthPublishingAsync(deviceIdentifier, knownMetrics);

        // Publish the message.
        this.Options.CancellationToken ??= SystemCancellationToken.None;
        return await this.Client.PublishAsync(deviceBirthMessage, this.Options.CancellationToken.Value);
    }

    /// <summary>
    /// Publishes some metrics for the device.
    /// </summary>
    /// <param name="metrics">The metrics.</param>
    /// <param name="deviceIdentifier">The device identifier.</param>
    /// <exception cref="ArgumentNullException">Thrown if the options are null.</exception>
    /// <exception cref="Exception">Thrown if the MQTT client is not connected or the device is unknown or an invalid metric type was specified.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the namespace is out of range.</exception>
    /// <returns>A <see cref="MqttClientPublishResult"/>.</returns>
    public Task<MqttClientPublishResult> PublishDeviceData(IEnumerable<T> metrics, string deviceIdentifier)
    {
        if (this.Options is null)
        {
            throw new ArgumentNullException(nameof(this.Options), "The options aren't set properly.");
        }

        if (!this.Client.IsConnected)
        {
            throw new Exception("The MQTT client is not connected, please try again.");
        }

        if (!this.KnownDevices.ContainsKey(deviceIdentifier))
        {
            throw new Exception("The device is unknown, please publish a device birth message first.");
        }

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
            this.Options.GroupIdentifier,
            this.Options.EdgeNodeIdentifier,
            deviceIdentifier,
            this.LastSequenceNumber,
            this.LastSessionNumber,
            DateTimeOffset.Now);

        // Increment the sequence number.
        this.IncrementLastSequenceNumber();

        // Invoke the device death event.
        await this.FireDeviceDeathPublishingAsync(deviceIdentifier);

        this.KnownDevices.TryRemove(deviceIdentifier, out _);
        // Publish the message.
        this.Options.CancellationToken ??= SystemCancellationToken.None;
        return await this.Client.PublishAsync(deviceDeathMessage, this.Options.CancellationToken.Value);
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

        // Get the data message.
        var dataMessage = this.MessageGenerator.GetSparkPlugDeviceDataMessage(
            this.NameSpace,
            this.Options.GroupIdentifier,
            this.Options.EdgeNodeIdentifier,
            deviceIdentifier,
            deviceMetricStorage.FilterOutgoingMetrics(metrics),
            this.LastSequenceNumber,
            this.LastSessionNumber,
            DateTimeOffset.Now,
            this.Options.AddSessionNumberToDataMessages);

        // Increment the sequence number.
        this.IncrementLastSequenceNumber();

        // Publish the message.
        return await this.Client.PublishAsync(dataMessage);
    }
}
