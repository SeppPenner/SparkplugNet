// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugDevice.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class that handles a Sparkplug device.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Core.Device
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MQTTnet.Client;
    using MQTTnet.Client.Publishing;
    using SparkplugNet.Core.Extensions;
    using SparkplugNet.Core.Node;
    using SparkplugNet.VersionB;

    /// <inheritdoc cref="SparkplugBase{T}" />
    /// <summary>A class that handles a Sparkplug device.</summary>
    /// <seealso cref="SparkplugBase{T}" />
    public class SparkplugDeviceBase<T> : SparkplugBase<T>
        where T : class, new()
    {
        private SparkplugDeviceOptions? options;
        private bool birthCertificateSent;

        /// <inheritdoc cref="SparkplugBase{T}" />
        /// <summary>Initializes a new instance of the <see cref="SparkplugDeviceBase{T}" /> class.</summary>
        /// <param name="knownMetrics">The metric names.</param>
        /// <seealso cref="SparkplugBase{T}" />
        protected SparkplugDeviceBase(List<T> knownMetrics)
            : base(knownMetrics)

        {
        }

        /// <summary>Gets or sets the device identifier.</summary>
        public string? DeviceId { get; set; }

        /// <summary>Gets or sets the device unique identifier.</summary>
        public Guid DeviceGuid { get; set; }

        /// <summary>Gets or sets the child of node.</summary>
        public SparkplugNodeBase<T>? ChildOf { get; set; }

        /// <summary>Starts the Sparkplug device.</summary>
        /// <param name="deviceOptions">The device options.</param>
        /// <exception cref="ArgumentNullException">The options are null.</exception>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        public async Task Start(SparkplugDeviceOptions deviceOptions)
        {
            // Storing the options.
            this.options = deviceOptions;
            this.DeviceGuid = this.options.DeviceGuid;

            if (this.options is null)
            {
                throw new ArgumentNullException(nameof(this.options));
            }

            // Connect, subscribe to incoming messages and send a state message.
            if (this.ChildOf?.Client.IsConnected ?? false)
            {
                await this.PublishInternal();
            }
        }

        /// <summary>Stops the Sparkplug device.</summary>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        public async Task Stop()
        {
            await this.DisconnectInternal();
        }

        /// <summary>
        /// Publishes some metrics.
        /// </summary>
        /// <param name="metrics">The metrics.</param>
        /// <param name="qosLevel">The qos level.</param>
        /// <param name="convertPayloadToJson">if set to <c>true</c> [convert payload to json].</param>
        /// <returns>
        /// A <see cref="Task" /> representing any asynchronous operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">The options are null.</exception>
        /// <exception cref="Exception">The MQTT client is not connected or an invalid metric type was specified.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The namespace is out of range.</exception>
        public async Task<MqttClientPublishResult> PublishMetricsAsync(List<T> metrics, int qosLevel)
        {
            if (!this.birthCertificateSent)
            {
                return new MqttClientPublishResult { ReasonCode = MqttClientPublishReasonCode.UnspecifiedError };
            }

            if (this.options is null)
            {
                throw new ArgumentNullException(nameof(this.options));
            }

            try
            {
                switch (metrics)
                {
                    case List<VersionA.Payload.KuraMetric> list:
                        return await this.PublishVersionAMessage(list, qosLevel);
                        break;
                    case List<VersionB.Payload.Metric> list:
                        return await this.PublishVersionBMessage(list, qosLevel);
                        break;
                    default:
                        return new MqttClientPublishResult { ReasonCode = MqttClientPublishReasonCode.UnspecifiedError };
                }
            }
            catch (Exception e)
            {
                this.ChildOf?.OnException?.Invoke(e);
                return new MqttClientPublishResult { ReasonCode = MqttClientPublishReasonCode.UnspecifiedError };
            }
        }

        /// <summary>Increments the last sequence number.</summary>
        internal override void IncrementLastSequenceNumber()
        {
            if (this.ChildOf == null)
            {
                return;
            }

            if (this.ChildOf.LastSequenceNumber == 255)
            {
                this.ChildOf.LastSequenceNumber = 0;
            }
            else
            {
                this.ChildOf.LastSequenceNumber++;
            }
        }

        /// <summary>
        /// Publishes a version A message.
        /// </summary>
        /// <param name="metrics">The metrics.</param>
        /// <param name="qosLevel">The qos level.</param>
        /// <param name="convertPayloadToJson">if set to <c>true</c> [convert payload to json].</param>
        /// <returns>
        /// A <see cref="Task" /> representing any asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">options</exception>
        /// <exception cref="System.Exception">Invalid metric type specified for version A metric.</exception>
        /// <exception cref="System.InvalidOperationException"></exception>
        /// <exception cref="ArgumentNullException">The options are null.</exception>
        /// <exception cref="Exception">An invalid metric type was specified.</exception>
        private async Task<MqttClientPublishResult> PublishVersionAMessage(List<VersionA.Payload.KuraMetric> metrics, int qosLevel)
        {
            if (this.options is null)
            {
                throw new ArgumentNullException(nameof(this.options));
            }

            if (!(this.KnownMetrics is List<VersionA.Payload.KuraMetric> knownMetrics))
            {
                throw new Exception("Invalid metric type specified for version A metric.");
            }

            if (this.ChildOf is null)
            {
                throw new InvalidOperationException($"{nameof(this.ChildOf)} cannot be null");
            }

            this.IncrementLastSequenceNumber();

            // Remove all not known metrics.
            metrics.RemoveAll(m => knownMetrics.FirstOrDefault(m2 => m2.Name == m.Name) == null);

            // Remove the session number metric if a user might have added it.
            metrics.RemoveAll(m => m.Name == Constants.SessionNumberMetricName);

            // Get the data message and increase the sequence counter.
            var dataMessage = this.MessageGenerator.GetSparkPlugDeviceDataMessage(this.NameSpace, this.options.GroupIdentifier, this.options.EdgeNodeIdentifier,
                this.options.DeviceIdentifier, metrics, this.ChildOf.LastSequenceNumber, this.LastSessionNumber, DateTimeOffset.Now, qosLevel, this.options.ConvertPayloadToJson);

            // Publish data.
            try
            {
                return await this.ChildOf.Client.PublishAsync(dataMessage);
            }
            catch (Exception e)
            {
                this.ChildOf.OnException?.Invoke(e);
                return new MqttClientPublishResult { ReasonCode = MqttClientPublishReasonCode.UnspecifiedError };
            }
        }

        /// <summary>
        /// Publishes a version B message.
        /// </summary>
        /// <param name="metrics">The metrics.</param>
        /// <param name="qosLevel">The qos level.</param>
        /// <returns>
        /// A <see cref="Task" /> representing any asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">options</exception>
        /// <exception cref="System.Exception">Invalid metric type specified for version B metric.</exception>
        /// <exception cref="System.InvalidOperationException"></exception>
        /// <exception cref="ArgumentNullException">The options are null.</exception>
        /// <exception cref="Exception">An invalid metric type was specified.</exception>
        private async Task<MqttClientPublishResult> PublishVersionBMessage(List<VersionB.Payload.Metric> metrics, int qosLevel)
        {
            if (this.options is null)
            {
                throw new ArgumentNullException(nameof(this.options));
            }

            if (!(this.KnownMetrics is List<VersionB.Payload.Metric> knownMetrics))
            {
                throw new Exception("Invalid metric type specified for version B metric.");
            }

            if (this.ChildOf is null)
            {
                throw new InvalidOperationException($"{nameof(this.ChildOf)} cannot be null");
            }

            // Increment here?
            this.IncrementLastSequenceNumber();

            // Remove all not known metrics.
            metrics.RemoveAll(m => knownMetrics.FirstOrDefault(m2 => m2.Name == m.Name) == null);

            // Remove the session number metric if a user might have added it.
            metrics.RemoveAll(m => m.Name == Constants.SessionNumberMetricName);

            // Get the data message and increase the sequence counter.
            var dataMessage = this.MessageGenerator.GetSparkPlugDeviceDataMessage(this.NameSpace, this.options.GroupIdentifier, this.options.EdgeNodeIdentifier,
                this.options.DeviceIdentifier, metrics, this.ChildOf.LastSequenceNumber, this.LastSessionNumber, DateTimeOffset.Now, qosLevel, this.options.ConvertPayloadToJson);

            // Publish data.
            try
            {
                return await this.ChildOf.Client.PublishAsync(dataMessage);
            }
            catch (Exception e)
            {
                this.ChildOf.OnException?.Invoke(e);
                return new MqttClientPublishResult { ReasonCode = MqttClientPublishReasonCode.UnspecifiedError };
            }
        }

        /// <summary>Publishes data to the MQTT broker.</summary>
        /// <exception cref="ArgumentNullException">The options are null.</exception>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        private async Task PublishInternal()
        {
            if (this.options is null)
            {
                throw new ArgumentNullException(nameof(this.options));
            }

            if (this.ChildOf is null)
            {
                throw new InvalidOperationException($"{nameof(this.ChildOf)} cannot be null");
            }

            this.IncrementLastSequenceNumber();

            // Get the online message and increase the sequence counter.
            var onlineMessage = this.MessageGenerator.GetSparkPlugDeviceBirthMessage(this.NameSpace, this.options.GroupIdentifier,
                this.options.EdgeNodeIdentifier, this.options.DeviceIdentifier, this.KnownMetrics, this.ChildOf.LastSequenceNumber, this.LastSessionNumber,
                DateTimeOffset.Now, 1, this.options.ConvertPayloadToJson);

            // Debug output.
            onlineMessage.ToJson();

            // Publish data.
            this.options.CancellationToken ??= CancellationToken.None;

            try
            {
                var result = await this.ChildOf.Client.PublishAsync(onlineMessage, this.options.CancellationToken.Value);
                switch (result.ReasonCode)
                {
                    case MqttClientPublishReasonCode.Success:
                    case MqttClientPublishReasonCode.NoMatchingSubscribers:
                        ////this.IncrementLastSequenceNumber();
                        this.birthCertificateSent = true;
                        break;
                }
            }
            catch (Exception e)
            {
                this.ChildOf.OnException?.Invoke(e);
            }
        }

        /// <summary>Disconnects the Sparkplug device from the MQTT broker.</summary>
        /// <exception cref="ArgumentNullException">The options are null.</exception>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        private async Task DisconnectInternal()
        {
            if (this.options is null)
            {
                throw new ArgumentNullException(nameof(this.options));
            }

            if (this.ChildOf == null)
            {
                return;
            }

            this.IncrementLastSequenceNumber();

            // Get the will message.
            var willMessage = this.MessageGenerator.GetSparkPlugDeviceDeathMessage(this.NameSpace, this.options.GroupIdentifier,
                this.options.EdgeNodeIdentifier, this.options.DeviceIdentifier, this.ChildOf.LastSequenceNumber, this.LastSessionNumber, DateTimeOffset.UtcNow,
                1, this.options.ConvertPayloadToJson);

            this.options.CancellationToken ??= CancellationToken.None;

            try
            {
                await this.ChildOf.Client.PublishAsync(willMessage, this.options.CancellationToken.Value);
            }
            catch (Exception e)
            {
                this.ChildOf.OnException?.Invoke(e);
            }
        }
    }
}