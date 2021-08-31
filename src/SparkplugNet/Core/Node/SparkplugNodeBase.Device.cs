// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugNodeBase.Device.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class that handles a Sparkplug node.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Core.Node
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MQTTnet.Client;
    using MQTTnet.Client.Publishing;

    using SparkplugNet.Core.Enumerations;

    using VersionAData = VersionA.Data;
    using VersionBData = VersionB.Data;

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
        /// Publishes a device birth message to the MQTT broker.
        /// </summary>
        /// <param name="knownMetrics">The known metrics.</param>
        /// <param name="deviceIdentifier">The device identifier.</param>
        /// <returns>A <see cref="MqttClientPublishResult"/>.</returns>
        public async Task<MqttClientPublishResult> PublishDeviceBirthMessage(List<T> knownMetrics, string deviceIdentifier)
        {
            if (this.options is null)
            {
                throw new ArgumentNullException(nameof(this.options));
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
                LastSessionNumber,
                DateTimeOffset.Now);

            // Increment the sequence number.
            this.IncrementLastSequenceNumber();

            // Add the known metrics to the known devices.
            this.KnownDevices.TryAdd(deviceIdentifier, knownMetrics);

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
        /// <exception cref="Exception">The MQTT client is not connected or an invalid metric type was specified.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The namespace is out of range.</exception>
        /// <returns>A <see cref="MqttClientPublishResult"/>.</returns>
        public async Task<MqttClientPublishResult> PublishDeviceData(List<T> metrics, string deviceIdentifier)
        {
            if (this.options is null)
            {
                throw new ArgumentNullException(nameof(this.options));
            }

            if (!this.Client.IsConnected)
            {
                throw new Exception("The MQTT client is not connected, please try again.");
            }

            switch (this.NameSpace)
            {
                case SparkplugNamespace.VersionA:
                    {
                        if (!(metrics is List<VersionAData.KuraMetric> convertedMetrics))
                        {
                            throw new Exception("Invalid metric type specified for version A metric.");
                        }

                        return await this.PublishVersionAMessageForDevice(convertedMetrics, deviceIdentifier);
                    }
                case SparkplugNamespace.VersionB:
                    {
                        if (!(metrics is List<VersionBData.Metric> convertedMetrics))
                        {
                            throw new Exception("Invalid metric type specified for version B metric.");
                        }

                        return await this.PublishVersionBMessageForDevice(convertedMetrics, deviceIdentifier);
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(this.NameSpace));
            }
        }

        /// <summary>
        /// Publishes a device death message to the MQTT broker.
        /// </summary>
        /// <param name="deviceIdentifier">The device identifier.</param>
        /// <returns>A <see cref="MqttClientPublishResult"/>.</returns>
        public async Task<MqttClientPublishResult> PublishDeviceDeathMessage(string deviceIdentifier)
        {
            if (this.options is null)
            {
                throw new ArgumentNullException(nameof(this.options));
            }

            if (!this.Client.IsConnected)
            {
                throw new Exception("The MQTT client is not connected, please try again.");
            }

            // Get the device death message.
            var deviceDeathMessage = this.MessageGenerator.GetSparkPlugDeviceDeathMessage(
                this.NameSpace,
                this.options.GroupIdentifier,
                this.options.EdgeNodeIdentifier,
                deviceIdentifier,
                this.LastSessionNumber);

            // Increment the sequence number.
            this.IncrementLastSequenceNumber();

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
        /// <exception cref="Exception">An invalid metric type was specified.</exception>
        /// <returns>A <see cref="MqttClientPublishResult"/>.</returns>
        private async Task<MqttClientPublishResult> PublishVersionAMessageForDevice(List<VersionAData.KuraMetric> metrics, string deviceIdentifier)
        {
            // Todo: Check metrics per device
            if (this.options is null)
            {
                throw new ArgumentNullException(nameof(this.options));
            }

            if (!(this.KnownMetrics is List<VersionAData.KuraMetric> knownMetrics))
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
                LastSessionNumber,
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
        /// <exception cref="Exception">An invalid metric type was specified.</exception>
        /// <returns>A <see cref="MqttClientPublishResult"/>.</returns>
        private async Task<MqttClientPublishResult> PublishVersionBMessageForDevice(List<VersionBData.Metric> metrics, string deviceIdentifier)
        {
            // Todo: Check metrics per device
            if (this.options is null)
            {
                throw new ArgumentNullException(nameof(this.options));
            }

            if (!(this.KnownMetrics is List<VersionBData.Metric> knownMetrics))
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
                LastSessionNumber,
                DateTimeOffset.Now);

            // Increment the sequence number.
            this.IncrementLastSequenceNumber();

            // Publish the message.
            return await this.Client.PublishAsync(dataMessage);
        }
    }
}
