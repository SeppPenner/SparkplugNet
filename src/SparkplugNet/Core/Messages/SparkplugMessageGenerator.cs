// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugMessageGenerator.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   The Sparkplug message generator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Core.Messages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MQTTnet;
    using MQTTnet.Protocol;
    using SparkplugNet.Core.Enumerations;
    using SparkplugNet.Core.Extensions;
    using VersionAPayload = VersionA.Payload;
    using VersionBPayload = VersionB.Payload;

    /// <summary>
    /// The Sparkplug message generator.
    /// </summary>
    public class SparkplugMessageGenerator
    {
        /// <summary>
        /// The topic generator.
        /// </summary>
        private readonly SparkplugTopicGenerator topicGenerator = new ();

        /// <summary>
        /// Gets a STATE message.
        /// </summary>
        /// <param name="nameSpace">The namespace.</param>
        /// <param name="scadaHostIdentifier">The SCADA host identifier.</param>
        /// <param name="online">A value indicating whether the message sender is online or not.</param>
        /// <exception cref="ArgumentException">The SCADA host identifier is invalid.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The namespace is out of range.</exception>
        /// <returns>A new STATE <see cref="MqttApplicationMessage"/>.</returns>
        public MqttApplicationMessage GetSparkplugStateMessage(
            SparkplugNamespace nameSpace,
            string scadaHostIdentifier,
            bool online)
        {
            if (!scadaHostIdentifier.IsIdentifierValid())
            {
                throw new ArgumentException(nameof(scadaHostIdentifier));
            }

            return nameSpace switch
            {
                SparkplugNamespace.VersionA => this.GetSparkplugStateMessageA(scadaHostIdentifier, online),
                SparkplugNamespace.VersionB => this.GetSparkplugStateMessageB(scadaHostIdentifier, online),
                _ => throw new ArgumentOutOfRangeException(nameof(nameSpace))
            };
        }

        /// <summary>
        /// Gets a NBIRTH message.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nameSpace">The namespace.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="metrics">The metrics.</param>
        /// <param name="sequenceNumber">The sequence number.</param>
        /// <param name="sessionNumber">The session number.</param>
        /// <param name="dateTime">The date time.</param>
        /// <param name="qosLevel">The qos level.</param>
        /// <param name="convertPayloadToJson">if set to <c>true</c> [convert payload to json].</param>
        /// <returns>
        /// A new NBIRTH <see cref="MqttApplicationMessage" />.
        /// </returns>
        /// <exception cref="System.ArgumentException">
        /// groupIdentifier
        /// or
        /// edgeNodeIdentifier
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">nameSpace</exception>
        /// <exception cref="ArgumentException">The group identifier or the edge node identifier is invalid.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The namespace is out of range.</exception>
        public MqttApplicationMessage GetSparkPlugNodeBirthMessage<T>(
            SparkplugNamespace nameSpace,
            string groupIdentifier,
            string edgeNodeIdentifier,
            List<T> metrics,
            int sequenceNumber,
            long sessionNumber,
            DateTimeOffset dateTime,
            int qosLevel,
            bool convertPayloadToJson = false)
            where T : class, new()
        {
            if (!groupIdentifier.IsIdentifierValid())
            {
                throw new ArgumentException(nameof(groupIdentifier));
            }

            if (!edgeNodeIdentifier.IsIdentifierValid())
            {
                throw new ArgumentException(nameof(edgeNodeIdentifier));
            }

            switch (nameSpace)
            {
                case SparkplugNamespace.VersionA:
                {
                    var newMetrics = metrics as List<VersionAPayload.KuraMetric> ?? new List<VersionAPayload.KuraMetric>();
                    AddSessionNumberToMetrics(newMetrics, sessionNumber);
                    return this.GetSparkPlugNodeBirthA(nameSpace, groupIdentifier, edgeNodeIdentifier, newMetrics, dateTime, qosLevel, convertPayloadToJson);
                }

                case SparkplugNamespace.VersionB:
                {
                    var newMetrics = metrics as List<VersionBPayload.Metric> ?? new List<VersionBPayload.Metric>();
                    AddSessionNumberToMetrics(newMetrics, sessionNumber);
                    return this.GetSparkPlugNodeBirthB(nameSpace, groupIdentifier, edgeNodeIdentifier, newMetrics, sequenceNumber, dateTime, qosLevel, convertPayloadToJson);
                }

                default:
                    throw new ArgumentOutOfRangeException(nameof(nameSpace));
            }
        }

        /// <summary>
        /// Gets a DBIRTH message.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nameSpace">The namespace.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="deviceIdentifier">The device identifier.</param>
        /// <param name="metrics">The metrics.</param>
        /// <param name="sequenceNumber">The sequence number.</param>
        /// <param name="sessionNumber">The session number.</param>
        /// <param name="dateTime">The date time.</param>
        /// <param name="qosLevel">The qos level.</param>
        /// <param name="convertPayloadToJson">if set to <c>true</c> [convert payload to json].</param>
        /// <returns>
        /// A new DBIRTH <see cref="MqttApplicationMessage" />.
        /// </returns>
        /// <exception cref="System.ArgumentException">
        /// groupIdentifier
        /// or
        /// edgeNodeIdentifier
        /// or
        /// deviceIdentifier
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">nameSpace</exception>
        /// <exception cref="ArgumentException">The group identifier or the device identifier or the edge node identifier is invalid.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The namespace is out of range.</exception>
        public MqttApplicationMessage GetSparkPlugDeviceBirthMessage<T>(
            SparkplugNamespace nameSpace,
            string groupIdentifier,
            string edgeNodeIdentifier,
            string deviceIdentifier,
            List<T> metrics,
            int sequenceNumber,
            long sessionNumber,
            DateTimeOffset dateTime,
            int qosLevel,
            bool convertPayloadToJson = false)
            where T : class, new()
        {
            if (!groupIdentifier.IsIdentifierValid())
            {
                throw new ArgumentException(nameof(groupIdentifier));
            }

            if (!edgeNodeIdentifier.IsIdentifierValid())
            {
                throw new ArgumentException(nameof(edgeNodeIdentifier));
            }

            if (!deviceIdentifier.IsIdentifierValid())
            {
                throw new ArgumentException(nameof(deviceIdentifier));
            }

            switch (nameSpace)
            {
                case SparkplugNamespace.VersionA:
                    {
                        var newMetrics = metrics as List<VersionAPayload.KuraMetric>
                                         ?? new List<VersionAPayload.KuraMetric>();
                        return this.GetSparkPlugDeviceBirthA(nameSpace, groupIdentifier, edgeNodeIdentifier, deviceIdentifier, newMetrics, dateTime, qosLevel, convertPayloadToJson);
                    }

                case SparkplugNamespace.VersionB:
                    {
                        var newMetrics = metrics as List<VersionBPayload.Metric> ?? new List<VersionBPayload.Metric>();
                        return this.GetSparkPlugDeviceBirthB(nameSpace, groupIdentifier, edgeNodeIdentifier, deviceIdentifier, newMetrics, sequenceNumber, dateTime, qosLevel, convertPayloadToJson);
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(nameSpace));
            }
        }

        /// <summary>
        /// Gets a NDEATH message.
        /// </summary>
        /// <param name="nameSpace">The namespace.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="sequenceNumber">The sequence number.</param>
        /// <param name="sessionNumber">The session number.</param>
        /// <param name="dateTime">The date time.</param>
        /// <param name="qosLevel">The qos level.</param>
        /// <param name="convertPayloadToJson">if set to <c>true</c> [convert payload to json].</param>
        /// <returns>
        /// A new NDEATH <see cref="MqttApplicationMessage" />.
        /// </returns>
        /// <exception cref="System.ArgumentException">
        /// groupIdentifier
        /// or
        /// edgeNodeIdentifier
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">nameSpace</exception>
        /// <exception cref="ArgumentException">The group identifier or the edge node identifier is invalid.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The namespace is out of range.</exception>
        public MqttApplicationMessage GetSparkPlugNodeDeathMessage(
            SparkplugNamespace nameSpace,
            string groupIdentifier,
            string edgeNodeIdentifier,
            int sequenceNumber,
            long sessionNumber,
            DateTimeOffset dateTime,
            int qosLevel,
            bool convertPayloadToJson = false)
        {
            if (!groupIdentifier.IsIdentifierValid())
            {
                throw new ArgumentException(nameof(groupIdentifier));
            }

            if (!edgeNodeIdentifier.IsIdentifierValid())
            {
                throw new ArgumentException(nameof(edgeNodeIdentifier));
            }

            switch (nameSpace)
            {
                case SparkplugNamespace.VersionA:
                    {
                        var metrics = new List<VersionAPayload.KuraMetric>();
                        AddSessionNumberToMetrics(metrics, sessionNumber);
                        return this.GetSparkPlugNodeDeathA(nameSpace, groupIdentifier, edgeNodeIdentifier, metrics, dateTime, qosLevel, convertPayloadToJson);
                    }

                case SparkplugNamespace.VersionB:
                    {
                        var metrics = new List<VersionBPayload.Metric>();
                        AddSessionNumberToMetrics(metrics, sessionNumber);
                        return this.GetSparkPlugNodeDeathB(nameSpace, groupIdentifier, edgeNodeIdentifier, sequenceNumber, dateTime, metrics, qosLevel, convertPayloadToJson);
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(nameSpace));
            }
        }

        /// <summary>
        /// Gets a DDEATH message.
        /// </summary>
        /// <param name="nameSpace">The namespace.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="deviceIdentifier">The device identifier.</param>
        /// <param name="sequenceNumber">The sequence number.</param>
        /// <param name="sessionNumber">The session number.</param>
        /// <param name="dateTime">The date time.</param>
        /// <param name="qosLevel">The qos level.</param>
        /// <param name="convertPayloadToJson">if set to <c>true</c> [convert payload to json].</param>
        /// <returns>
        /// A new DDEATH <see cref="MqttApplicationMessage" />.
        /// </returns>
        /// <exception cref="System.ArgumentException">
        /// groupIdentifier
        /// or
        /// edgeNodeIdentifier
        /// or
        /// deviceIdentifier
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">nameSpace</exception>
        /// <exception cref="ArgumentException">The group identifier or the device identifier or the edge node identifier is invalid.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The namespace is out of range.</exception>
        public MqttApplicationMessage GetSparkPlugDeviceDeathMessage(
            SparkplugNamespace nameSpace,
            string groupIdentifier,
            string edgeNodeIdentifier,
            string deviceIdentifier,
            int sequenceNumber,
            long sessionNumber,
            DateTimeOffset dateTime,
            int qosLevel,
            bool convertPayloadToJson = false)
        {
            if (!groupIdentifier.IsIdentifierValid())
            {
                throw new ArgumentException(nameof(groupIdentifier));
            }

            if (!edgeNodeIdentifier.IsIdentifierValid())
            {
                throw new ArgumentException(nameof(edgeNodeIdentifier));
            }

            if (!deviceIdentifier.IsIdentifierValid())
            {
                throw new ArgumentException(nameof(deviceIdentifier));
            }

            switch (nameSpace)
            {
                case SparkplugNamespace.VersionA:
                    {
                        var metrics = new List<VersionAPayload.KuraMetric>();
                        return this.GetSparkPlugDeviceDeathA(nameSpace, groupIdentifier, edgeNodeIdentifier, deviceIdentifier, dateTime, metrics, qosLevel, convertPayloadToJson);
                    }

                case SparkplugNamespace.VersionB:
                    {
                        var metrics = new List<VersionBPayload.Metric>();
                        return this.GetSparkPlugDeviceDeathB(nameSpace, groupIdentifier, edgeNodeIdentifier, deviceIdentifier, sequenceNumber, dateTime, metrics, qosLevel, convertPayloadToJson);
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(nameSpace));
            }
        }

        /// <summary>
        /// Gets a NDATA message.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nameSpace">The namespace.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="metrics">The metrics.</param>
        /// <param name="sequenceNumber">The sequence number.</param>
        /// <param name="sessionNumber">The session number.</param>
        /// <param name="dateTime">The date time.</param>
        /// <param name="qosLevel">The qos level.</param>
        /// <param name="convertPayloadToJson">if set to <c>true</c> [convert payload to json].</param>
        /// <returns>
        /// A new NDATA <see cref="MqttApplicationMessage" />.
        /// </returns>
        /// <exception cref="System.ArgumentException">
        /// groupIdentifier
        /// or
        /// edgeNodeIdentifier
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">nameSpace</exception>
        /// <exception cref="ArgumentException">The group identifier or the edge node identifier is invalid.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The namespace is out of range.</exception>
        public MqttApplicationMessage GetSparkPlugNodeDataMessage<T>(
            SparkplugNamespace nameSpace,
            string groupIdentifier,
            string edgeNodeIdentifier,
            List<T> metrics,
            int sequenceNumber,
            long sessionNumber,
            DateTimeOffset dateTime,
            int qosLevel,
            bool convertPayloadToJson = false)
            where T : class, new()
        {
            if (!groupIdentifier.IsIdentifierValid())
            {
                throw new ArgumentException(nameof(groupIdentifier));
            }

            if (!edgeNodeIdentifier.IsIdentifierValid())
            {
                throw new ArgumentException(nameof(edgeNodeIdentifier));
            }

            switch (nameSpace)
            {
                case SparkplugNamespace.VersionA:
                    {
                        var newMetrics = metrics as List<VersionAPayload.KuraMetric>
                                         ?? new List<VersionAPayload.KuraMetric>();
                        ////AddSessionNumberToMetrics(newMetrics, sessionNumber);
                        return this.GetSparkPlugNodeDataA(nameSpace, groupIdentifier, edgeNodeIdentifier, newMetrics, dateTime, qosLevel, convertPayloadToJson);
                    }

                case SparkplugNamespace.VersionB:
                    {
                        var newMetrics = metrics as List<VersionBPayload.Metric>
                                         ?? new List<VersionBPayload.Metric>();
                        ////AddSessionNumberToMetrics(newMetrics, sessionNumber);
                        return this.GetSparkPlugNodeDataB(nameSpace, groupIdentifier, edgeNodeIdentifier, newMetrics, sequenceNumber, dateTime, qosLevel, convertPayloadToJson);
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(nameSpace));
            }
        }

        /// <summary>
        /// Gets a DDATA message.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nameSpace">The namespace.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="deviceIdentifier">The device identifier.</param>
        /// <param name="metrics">The metrics.</param>
        /// <param name="sequenceNumber">The sequence number.</param>
        /// <param name="sessionNumber">The session number.</param>
        /// <param name="dateTime">The date time.</param>
        /// <param name="qosLevel">The qos level.</param>
        /// <param name="convertPayloadToJson">if set to <c>true</c> [convert payload to json].</param>
        /// <returns>
        /// A new DDATA <see cref="MqttApplicationMessage" />.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">nameSpace</exception>
        /// <exception cref="ArgumentException">The group identifier or the device identifier or the edge node identifier is invalid.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The namespace is out of range.</exception>
        public MqttApplicationMessage GetSparkPlugDeviceDataMessage<T>(
            SparkplugNamespace nameSpace,
            string groupIdentifier,
            string edgeNodeIdentifier,
            string deviceIdentifier,
            List<T> metrics,
            int sequenceNumber,
            long sessionNumber,
            DateTimeOffset dateTime,
            int qosLevel,
            bool convertPayloadToJson = false)
            where T : class, new()
        {
            if (!groupIdentifier.IsIdentifierValid())
            {
                throw new ArgumentException(nameof(groupIdentifier));
            }

            if (!edgeNodeIdentifier.IsIdentifierValid())
            {
                throw new ArgumentException(nameof(edgeNodeIdentifier));
            }

            if (!deviceIdentifier.IsIdentifierValid())
            {
                throw new ArgumentException(nameof(deviceIdentifier));
            }

            switch (nameSpace)
            {
                case SparkplugNamespace.VersionA:
                    {
                        var newMetrics = metrics as List<VersionAPayload.KuraMetric>
                                         ?? new List<VersionAPayload.KuraMetric>();
                        ////AddSessionNumberToMetrics(newMetrics, sessionNumber);
                        return this.GetSparkPlugDeviceDataA(nameSpace, groupIdentifier, edgeNodeIdentifier, deviceIdentifier, newMetrics, dateTime, qosLevel, convertPayloadToJson);
                    }

                case SparkplugNamespace.VersionB:
                    {
                        var newMetrics = metrics as List<VersionBPayload.Metric>
                                         ?? new List<VersionBPayload.Metric>();
                        ////AddSessionNumberToMetrics(newMetrics, sessionNumber);
                        return this.GetSparkPlugDeviceDataB(nameSpace, groupIdentifier, edgeNodeIdentifier, deviceIdentifier, newMetrics, sequenceNumber, dateTime, qosLevel, convertPayloadToJson);
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(nameSpace));
            }
        }

        /// <summary>
        /// Gets a NCMD message.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nameSpace">The namespace.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="metrics">The metrics.</param>
        /// <param name="sequenceNumber">The sequence number.</param>
        /// <param name="sessionNumber">The session number.</param>
        /// <param name="dateTime">The date time.</param>
        /// <param name="qosLevel">The qos level.</param>
        /// <param name="convertPayloadToJson">if set to <c>true</c> [convert payload to json].</param>
        /// <returns>
        /// A new NCMD <see cref="MqttApplicationMessage" />.
        /// </returns>
        /// <exception cref="System.ArgumentException">
        /// groupIdentifier
        /// or
        /// edgeNodeIdentifier
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">nameSpace</exception>
        /// <exception cref="ArgumentException">The group identifier or the edge node identifier is invalid.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The namespace is out of range.</exception>
        public MqttApplicationMessage GetSparkPlugNodeCommandMessage<T>(
            SparkplugNamespace nameSpace,
            string groupIdentifier,
            string edgeNodeIdentifier,
            List<T> metrics,
            int sequenceNumber,
            long sessionNumber,
            DateTimeOffset dateTime,
            int qosLevel,
            bool convertPayloadToJson = false)
            where T : class, new()
        {
            if (!groupIdentifier.IsIdentifierValid())
            {
                throw new ArgumentException(nameof(groupIdentifier));
            }

            if (!edgeNodeIdentifier.IsIdentifierValid())
            {
                throw new ArgumentException(nameof(edgeNodeIdentifier));
            }

            switch (nameSpace)
            {
                case SparkplugNamespace.VersionA:
                    {
                        var newMetrics = metrics as List<VersionAPayload.KuraMetric>
                                         ?? new List<VersionAPayload.KuraMetric>();
                        ////AddSessionNumberToMetrics(newMetrics, sessionNumber);
                        return this.GetSparkPlugNodeCommandA(nameSpace, groupIdentifier, edgeNodeIdentifier, newMetrics, dateTime, qosLevel, convertPayloadToJson);
                    }

                case SparkplugNamespace.VersionB:
                    {
                        var newMetrics = metrics as List<VersionBPayload.Metric>
                                         ?? new List<VersionBPayload.Metric>();
                        ////AddSessionNumberToMetrics(newMetrics, sessionNumber);
                        return this.GetSparkPlugNodeCommandB(nameSpace, groupIdentifier, edgeNodeIdentifier, newMetrics, sequenceNumber, dateTime, qosLevel, convertPayloadToJson);
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(nameSpace));
            }
        }

        /// <summary>
        /// Gets a DCMD message.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nameSpace">The namespace.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="deviceIdentifier">The device identifier.</param>
        /// <param name="metrics">The metrics.</param>
        /// <param name="sequenceNumber">The sequence number.</param>
        /// <param name="sessionNumber">The session number.</param>
        /// <param name="dateTime">The date time.</param>
        /// <param name="qosLevel">The qos level.</param>
        /// <param name="convertPayloadToJson">if set to <c>true</c> [convert payload to json].</param>
        /// <returns>
        /// A new DCMD <see cref="MqttApplicationMessage" />.
        /// </returns>
        /// <exception cref="System.ArgumentException">
        /// groupIdentifier
        /// or
        /// edgeNodeIdentifier
        /// or
        /// deviceIdentifier
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">nameSpace</exception>
        /// <exception cref="ArgumentException">The group identifier or the device identifier or the edge node identifier is invalid.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The namespace is out of range.</exception>
        public MqttApplicationMessage GetSparkPlugDeviceCommandMessage<T>(
            SparkplugNamespace nameSpace,
            string groupIdentifier,
            string edgeNodeIdentifier,
            string deviceIdentifier,
            List<T> metrics,
            int sequenceNumber,
            long sessionNumber,
            DateTimeOffset dateTime,
            int qosLevel,
            bool convertPayloadToJson = false)
            where T : class, new()
        {
            if (!groupIdentifier.IsIdentifierValid())
            {
                throw new ArgumentException(nameof(groupIdentifier));
            }

            if (!edgeNodeIdentifier.IsIdentifierValid())
            {
                throw new ArgumentException(nameof(edgeNodeIdentifier));
            }

            if (!deviceIdentifier.IsIdentifierValid())
            {
                throw new ArgumentException(nameof(deviceIdentifier));
            }

            switch (nameSpace)
            {
                case SparkplugNamespace.VersionA:
                    {
                        var newMetrics = metrics as List<VersionAPayload.KuraMetric>
                                         ?? new List<VersionAPayload.KuraMetric>();
                        ////AddSessionNumberToMetrics(newMetrics, sessionNumber); // removed only required by spec for 16.1 NBIRTH and 16.8 NDEATH
                        return this.GetSparkPlugDeviceCommandA(nameSpace, groupIdentifier, edgeNodeIdentifier, deviceIdentifier, newMetrics, dateTime, qosLevel, convertPayloadToJson);
                    }

                case SparkplugNamespace.VersionB:
                    {
                        var newMetrics = metrics as List<VersionBPayload.Metric>
                                         ?? new List<VersionBPayload.Metric>();
                        ////AddSessionNumberToMetrics(newMetrics, sessionNumber); // removed only required by spec for 16.1 NBIRTH and 16.8 NDEATH
                        return this.GetSparkPlugDeviceCommandB(nameSpace, groupIdentifier, edgeNodeIdentifier, deviceIdentifier, newMetrics, sequenceNumber, dateTime, qosLevel, convertPayloadToJson);
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(nameSpace));
            }
        }

        /// <summary>
        /// Gets a STATE message with namespace version A.
        /// </summary>
        /// <param name="scadaHostIdentifier">The SCADA host identifier.</param>
        /// <param name="online">A value indicating whether the message sender is online or not.</param>
        /// <returns>A new STATE <see cref="MqttApplicationMessage"/>.</returns>
        private MqttApplicationMessage GetSparkplugStateMessageA(string scadaHostIdentifier, bool online)
        {
            return new MqttApplicationMessageBuilder()
                .WithTopic(this.topicGenerator.GetSparkplugStateMessageTopic(scadaHostIdentifier))
                .WithPayload(online ? "ONLINE" : "OFFLINE").WithAtLeastOnceQoS().WithRetainFlag().Build();
        }

        /// <summary>
        /// Gets a STATE message with namespace version B.
        /// </summary>
        /// <param name="scadaHostIdentifier">The SCADA host identifier.</param>
        /// <param name="online">A value indicating whether the message sender is online or not.</param>
        /// <returns>A new STATE <see cref="MqttApplicationMessage"/>.</returns>
        private MqttApplicationMessage GetSparkplugStateMessageB(string scadaHostIdentifier, bool online)
        {
            return new MqttApplicationMessageBuilder()
                .WithTopic(this.topicGenerator.GetSparkplugStateMessageTopic(scadaHostIdentifier))
                .WithPayload(online ? "ONLINE" : "OFFLINE").WithAtLeastOnceQoS().WithRetainFlag().Build();
        }

        /// <summary>
        /// Gets a NBIRTH message with namespace version A.
        /// </summary>
        /// <param name="nameSpace">The namespace.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="metrics">The metrics.</param>
        /// <param name="dateTime">The date time.</param>
        /// <param name="qosLevel">The qos level.</param>
        /// <param name="convertPayloadToJson">if set to <c>true</c> [convert payload to json].</param>
        /// <returns>
        /// A new NBIRTH <see cref="MqttApplicationMessage" />.
        /// </returns>
        private MqttApplicationMessage GetSparkPlugNodeBirthA(
            SparkplugNamespace nameSpace,
            string groupIdentifier,
            string edgeNodeIdentifier,
            List<VersionAPayload.KuraMetric> metrics,
            DateTimeOffset dateTime,
            int qosLevel,
            bool convertPayloadToJson = false)
        {
            var payload = new VersionAPayload
            {
                Metrics = metrics,
                Timestamp = dateTime.ToUnixTimeMilliseconds()
            };

            // Debug output.
            payload.ToJson();

            var serialized = PayloadHelper.Serialize(payload, convertPayloadToJson);

            return new MqttApplicationMessageBuilder()
                .WithTopic(
                    this.topicGenerator.GetTopic(
                        nameSpace,
                        groupIdentifier,
                        SparkplugMessageType.NodeBirth,
                        edgeNodeIdentifier,
                        string.Empty)).WithPayload(serialized)
                .WithQualityOfServiceLevel((MqttQualityOfServiceLevel)qosLevel)
                .WithRetainFlag()
                .Build();
        }

        /// <summary>
        /// Gets a NBIRTH message with namespace version B.
        /// </summary>
        /// <param name="nameSpace">The namespace.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="metrics">The metrics.</param>
        /// <param name="sequenceNumber">The sequence number.</param>
        /// <param name="dateTime">The date time.</param>
        /// <param name="qosLevel">The qos level.</param>
        /// <param name="convertPayloadToJson">if set to <c>true</c> [convert payload to json].</param>
        /// <returns>
        /// A new NBIRTH <see cref="MqttApplicationMessage" />.
        /// </returns>
        private MqttApplicationMessage GetSparkPlugNodeBirthB(
            SparkplugNamespace nameSpace,
            string groupIdentifier,
            string edgeNodeIdentifier,
            List<VersionBPayload.Metric> metrics,
            int sequenceNumber,
            DateTimeOffset dateTime,
            int qosLevel,
            bool convertPayloadToJson = false)
        {
            var payload = new VersionBPayload
            {
                Metrics = metrics,
                Seq = (ulong)sequenceNumber,
                Timestamp = (ulong)dateTime.ToUnixTimeMilliseconds()
            };

            // Debug output.
            payload.ToJson();

            var serialized = PayloadHelper.Serialize(payload, convertPayloadToJson);

            return new MqttApplicationMessageBuilder()
                .WithTopic(
                    this.topicGenerator.GetTopic(
                        nameSpace,
                        groupIdentifier,
                        SparkplugMessageType.NodeBirth,
                        edgeNodeIdentifier,
                        string.Empty))
                .WithPayload(serialized)
                .WithQualityOfServiceLevel((MqttQualityOfServiceLevel)qosLevel)
                .WithRetainFlag()
                .Build();
        }

        /// <summary>
        /// Gets a DBIRTH message with namespace version A.
        /// </summary>
        /// <param name="nameSpace">The namespace.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="deviceIdentifier">The device identifier.</param>
        /// <param name="metrics">The metrics.</param>
        /// <param name="dateTime">The date time.</param>
        /// <param name="qosLevel">The qos level.</param>
        /// <param name="convertPayloadToJson">if set to <c>true</c> [convert payload to json].</param>
        /// <returns>
        /// A new DBIRTH <see cref="MqttApplicationMessage" />.
        /// </returns>
        private MqttApplicationMessage GetSparkPlugDeviceBirthA(
            SparkplugNamespace nameSpace,
            string groupIdentifier,
            string edgeNodeIdentifier,
            string deviceIdentifier,
            List<VersionAPayload.KuraMetric> metrics,
            DateTimeOffset dateTime,
            int qosLevel,
            bool convertPayloadToJson = false)
        {
            var payload = new VersionAPayload
            {
                Metrics = metrics,
                Timestamp = dateTime.ToUnixTimeMilliseconds()
            };

            // Debug output.
            payload.ToJson();

            var serialized = PayloadHelper.Serialize(payload, convertPayloadToJson);

            return new MqttApplicationMessageBuilder()
                .WithTopic(
                    this.topicGenerator.GetTopic(
                        nameSpace,
                        groupIdentifier,
                        SparkplugMessageType.DeviceBirth,
                        edgeNodeIdentifier,
                        deviceIdentifier))
                .WithPayload(serialized)
                .WithQualityOfServiceLevel((MqttQualityOfServiceLevel)qosLevel)
                .WithRetainFlag()
                .Build();
        }

        /// <summary>
        /// Gets a DBIRTH message with namespace version B.
        /// </summary>
        /// <param name="nameSpace">The namespace.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="deviceIdentifier">The device identifier.</param>
        /// <param name="metrics">The metrics.</param>
        /// <param name="sequenceNumber">The sequence number.</param>
        /// <param name="dateTime">The date time.</param>
        /// <param name="qosLevel">The qos level.</param>
        /// <param name="convertPayloadToJson">if set to <c>true</c> [convert payload to json].</param>
        /// <returns>
        /// A new DBIRTH <see cref="MqttApplicationMessage" />.
        /// </returns>
        private MqttApplicationMessage GetSparkPlugDeviceBirthB(
            SparkplugNamespace nameSpace,
            string groupIdentifier,
            string edgeNodeIdentifier,
            string deviceIdentifier,
            List<VersionBPayload.Metric> metrics,
            int sequenceNumber,
            DateTimeOffset dateTime,
            int qosLevel,
            bool convertPayloadToJson = false)
        {
            var payload = new VersionBPayload
            {
                Metrics = metrics,
                Seq = (ulong)sequenceNumber,
                Timestamp = (ulong)dateTime.ToUnixTimeMilliseconds()
            };

            // Debug output.
            payload.ToJson();

            var serialized = PayloadHelper.Serialize(payload, convertPayloadToJson);

            return new MqttApplicationMessageBuilder()
                .WithTopic(
                    this.topicGenerator.GetTopic(
                        nameSpace,
                        groupIdentifier,
                        SparkplugMessageType.DeviceBirth,
                        edgeNodeIdentifier,
                        deviceIdentifier))
                .WithPayload(serialized)
                .WithQualityOfServiceLevel((MqttQualityOfServiceLevel)qosLevel)
                .WithRetainFlag()
                .Build();
        }

        /// <summary>
        /// Gets a NDEATH message with namespace version A.
        /// </summary>
        /// <param name="nameSpace">The namespace.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="metrics">The metrics.</param>
        /// <param name="dateTime">The date time.</param>
        /// <param name="qosLevel">The qos level.</param>
        /// <param name="convertPayloadToJson">if set to <c>true</c> [convert payload to json].</param>
        /// <returns>
        /// A new NDEATH <see cref="MqttApplicationMessage" />.
        /// </returns>
        private MqttApplicationMessage GetSparkPlugNodeDeathA(
            SparkplugNamespace nameSpace,
            string groupIdentifier,
            string edgeNodeIdentifier,
            List<VersionAPayload.KuraMetric> metrics,
            DateTimeOffset dateTime,
            int qosLevel,
            bool convertPayloadToJson = false)
        {
            var payload = new VersionAPayload
            {
                Metrics = metrics,
                Timestamp = dateTime.ToUnixTimeMilliseconds(),
            };

            // Debug output.
            payload.ToJson();

            var serialized = PayloadHelper.Serialize(payload, convertPayloadToJson);

            return new MqttApplicationMessageBuilder()
                .WithTopic(
                    this.topicGenerator.GetTopic(
                        nameSpace,
                        groupIdentifier,
                        SparkplugMessageType.NodeDeath,
                        edgeNodeIdentifier,
                        string.Empty))
                .WithPayload(serialized)
                .WithQualityOfServiceLevel((MqttQualityOfServiceLevel)qosLevel)
                .WithRetainFlag()
                .Build();
        }

        /// <summary>
        /// Gets a NDEATH message with namespace version B.
        /// </summary>
        /// <param name="nameSpace">The namespace.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="sequenceNumber">The sequence number.</param>
        /// <param name="dateTime">The date time.</param>
        /// <param name="metrics">The metrics.</param>
        /// <param name="qosLevel">The qos level.</param>
        /// <param name="convertPayloadToJson">if set to <c>true</c> [convert payload to json].</param>
        /// <returns>
        /// A new NDEATH <see cref="MqttApplicationMessage" />.
        /// </returns>
        private MqttApplicationMessage GetSparkPlugNodeDeathB(
            SparkplugNamespace nameSpace,
            string groupIdentifier,
            string edgeNodeIdentifier,
            int sequenceNumber,
            DateTimeOffset dateTime,
            List<VersionBPayload.Metric> metrics,
            int qosLevel,
            bool convertPayloadToJson = false)
        {
            var payload = new VersionBPayload
            {
                Metrics = metrics,
                Seq = (ulong)sequenceNumber,
                Timestamp = (ulong)dateTime.ToUnixTimeMilliseconds()
            };

            // Debug output.
            payload.ToJson();

            var serialized = PayloadHelper.Serialize(payload, convertPayloadToJson);

            return new MqttApplicationMessageBuilder()
                .WithTopic(
                    this.topicGenerator.GetTopic(
                        nameSpace,
                        groupIdentifier,
                        SparkplugMessageType.NodeDeath,
                        edgeNodeIdentifier,
                        string.Empty))
                .WithPayload(serialized)
                .WithQualityOfServiceLevel((MqttQualityOfServiceLevel)qosLevel)
                .WithRetainFlag()
                .Build();
        }

        /// <summary>
        /// Gets a DDEATH message with namespace version A.
        /// </summary>
        /// <param name="nameSpace">The namespace.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="deviceIdentifier">The device identifier.</param>
        /// <param name="dateTime">The date time.</param>
        /// <param name="metrics">The metrics.</param>
        /// <param name="qosLevel">The qos level.</param>
        /// <param name="convertPayloadToJson">if set to <c>true</c> [convert payload to json].</param>
        /// <returns>
        /// A new DDEATH <see cref="MqttApplicationMessage" />.
        /// </returns>
        private MqttApplicationMessage GetSparkPlugDeviceDeathA(
            SparkplugNamespace nameSpace,
            string groupIdentifier,
            string edgeNodeIdentifier,
            string deviceIdentifier,
            DateTimeOffset dateTime,
            List<VersionAPayload.KuraMetric> metrics,
            int qosLevel,
            bool convertPayloadToJson = false)
        {
            var payload = new VersionAPayload
            {
                Metrics = metrics,
                Timestamp = dateTime.ToUnixTimeMilliseconds(),
            };

            // Debug output.
            payload.ToJson();

            var serialized = PayloadHelper.Serialize(payload, convertPayloadToJson);

            return new MqttApplicationMessageBuilder()
                .WithTopic(
                    this.topicGenerator.GetTopic(
                        nameSpace,
                        groupIdentifier,
                        SparkplugMessageType.DeviceDeath,
                        edgeNodeIdentifier,
                        deviceIdentifier))
                .WithPayload(serialized)
                .WithQualityOfServiceLevel((MqttQualityOfServiceLevel)qosLevel)
                .WithRetainFlag()
                .Build();
        }

        /// <summary>
        /// Gets a DDEATH message with namespace version B.
        /// </summary>
        /// <param name="nameSpace">The namespace.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="deviceIdentifier">The device identifier.</param>
        /// <param name="sequenceNumber">The sequence number.</param>
        /// <param name="dateTime">The date time.</param>
        /// <param name="metrics">The metrics.</param>
        /// <param name="qosLevel">The qos level.</param>
        /// <param name="convertPayloadToJson">if set to <c>true</c> [convert payload to json].</param>
        /// <returns>
        /// A new DDEATH <see cref="MqttApplicationMessage" />.
        /// </returns>
        private MqttApplicationMessage GetSparkPlugDeviceDeathB(
            SparkplugNamespace nameSpace,
            string groupIdentifier,
            string edgeNodeIdentifier,
            string deviceIdentifier,
            int sequenceNumber,
            DateTimeOffset dateTime,
            List<VersionBPayload.Metric> metrics,
            int qosLevel,
            bool convertPayloadToJson = false)
        {
            var payload = new VersionBPayload
            {
                Metrics = metrics,
                Seq = (ulong)sequenceNumber,
                Timestamp = (ulong)dateTime.ToUnixTimeMilliseconds()
            };

            // Debug output.
            payload.ToJson();

            var serialized = PayloadHelper.Serialize(payload, convertPayloadToJson);

            return new MqttApplicationMessageBuilder()
                .WithTopic(
                    this.topicGenerator.GetTopic(
                        nameSpace,
                        groupIdentifier,
                        SparkplugMessageType.DeviceDeath,
                        edgeNodeIdentifier,
                        deviceIdentifier))
                .WithPayload(serialized)
                .WithQualityOfServiceLevel((MqttQualityOfServiceLevel)qosLevel)
                .WithRetainFlag()
                .Build();
        }

        /// <summary>
        /// Gets a NDATA message with namespace version A.
        /// </summary>
        /// <param name="nameSpace">The namespace.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="metrics">The metrics.</param>
        /// <param name="dateTime">The date time.</param>
        /// <param name="qosLevel">The qos level.</param>
        /// <param name="convertPayloadToJson">if set to <c>true</c> [convert payload to json].</param>
        /// <returns>
        /// A new NDATA <see cref="MqttApplicationMessage" />.
        /// </returns>
        private MqttApplicationMessage GetSparkPlugNodeDataA(
            SparkplugNamespace nameSpace,
            string groupIdentifier,
            string edgeNodeIdentifier,
            List<VersionAPayload.KuraMetric> metrics,
            DateTimeOffset dateTime,
            int qosLevel,
            bool convertPayloadToJson = false)
        {
            var payload = new VersionAPayload
            {
                Metrics = metrics,
                Timestamp = dateTime.ToUnixTimeMilliseconds()
            };

            // Debug output.
            payload.ToJson();

            var serialized = PayloadHelper.Serialize(payload, convertPayloadToJson);

            return new MqttApplicationMessageBuilder()
                .WithTopic(
                    this.topicGenerator.GetTopic(
                        nameSpace,
                        groupIdentifier,
                        SparkplugMessageType.NodeData,
                        edgeNodeIdentifier,
                        string.Empty))
                .WithPayload(serialized)
                .WithQualityOfServiceLevel((MqttQualityOfServiceLevel)qosLevel)
                .WithRetainFlag()
                .Build();
        }

        /// <summary>
        /// Gets a NDATA message with namespace version B.
        /// </summary>
        /// <param name="nameSpace">The namespace.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="metrics">The metrics.</param>
        /// <param name="sequenceNumber">The sequence number.</param>
        /// <param name="dateTime">The date time.</param>
        /// <param name="qosLevel">The qos level.</param>
        /// <param name="convertPayloadToJson">if set to <c>true</c> [convert payload to json].</param>
        /// <returns>
        /// A new NDATA <see cref="MqttApplicationMessage" />.
        /// </returns>
        private MqttApplicationMessage GetSparkPlugNodeDataB(
            SparkplugNamespace nameSpace,
            string groupIdentifier,
            string edgeNodeIdentifier,
            List<VersionBPayload.Metric> metrics,
            int sequenceNumber,
            DateTimeOffset dateTime,
            int qosLevel,
            bool convertPayloadToJson = false)
        {
            var payload = new VersionBPayload
            {
                Metrics = metrics,
                Seq = (ulong)sequenceNumber,
                Timestamp = (ulong)dateTime.ToUnixTimeMilliseconds(),
                // REMOVE ME IF STILL HERE //Timestamp = metrics.FirstOrDefault()?.Timestamp ?? (ulong)dateTime.ToUnixTimeMilliseconds(),
            };

            // Debug output.
            payload.ToJson();

            var serialized = PayloadHelper.Serialize(payload, convertPayloadToJson);

            return new MqttApplicationMessageBuilder()
                .WithTopic(
                    this.topicGenerator.GetTopic(
                        nameSpace,
                        groupIdentifier,
                        SparkplugMessageType.NodeData,
                        edgeNodeIdentifier,
                        string.Empty))
                .WithPayload(serialized)
                .WithQualityOfServiceLevel((MqttQualityOfServiceLevel)qosLevel)
                .WithRetainFlag()
                .Build();
        }

        /// <summary>
        /// Gets a DDATA message with namespace version A.
        /// </summary>
        /// <param name="nameSpace">The namespace.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="deviceIdentifier">The device identifier.</param>
        /// <param name="metrics">The metrics.</param>
        /// <param name="dateTime">The date time.</param>
        /// <param name="qosLevel">The qos level.</param>
        /// <param name="convertPayloadToJson">if set to <c>true</c> [convert payload to json].</param>
        /// <returns>
        /// A new DDATA <see cref="MqttApplicationMessage" />.
        /// </returns>
        private MqttApplicationMessage GetSparkPlugDeviceDataA(
            SparkplugNamespace nameSpace,
            string groupIdentifier,
            string edgeNodeIdentifier,
            string deviceIdentifier,
            List<VersionAPayload.KuraMetric> metrics,
            DateTimeOffset dateTime,
            int qosLevel,
            bool convertPayloadToJson = false)
        {
            var payload = new VersionAPayload
            {
                Metrics = metrics,
                Timestamp = dateTime.ToUnixTimeMilliseconds()
            };

            // Debug output.
            payload.ToJson();

            var serialized = PayloadHelper.Serialize(payload, convertPayloadToJson);

            return new MqttApplicationMessageBuilder()
                .WithTopic(
                    this.topicGenerator.GetTopic(
                        nameSpace,
                        groupIdentifier,
                        SparkplugMessageType.DeviceData,
                        edgeNodeIdentifier,
                        deviceIdentifier))
                .WithPayload(serialized)
                .WithQualityOfServiceLevel((MqttQualityOfServiceLevel)qosLevel)
                .WithRetainFlag()
                .Build();
        }

        /// <summary>
        /// Gets a DDATA message with namespace version B.
        /// </summary>
        /// <param name="nameSpace">The namespace.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="deviceIdentifier">The device identifier.</param>
        /// <param name="metrics">The metrics.</param>
        /// <param name="sequenceNumber">The sequence number.</param>
        /// <param name="dateTime">The date time.</param>
        /// <param name="qosLevel">The qos level.</param>
        /// <param name="convertPayloadToJson">if set to <c>true</c> [convert payload to json].</param>
        /// <returns>A new DDATA <see cref="MqttApplicationMessage" />.</returns>
        private MqttApplicationMessage GetSparkPlugDeviceDataB(
            SparkplugNamespace nameSpace,
            string groupIdentifier,
            string edgeNodeIdentifier,
            string deviceIdentifier,
            List<VersionBPayload.Metric> metrics,
            int sequenceNumber,
            DateTimeOffset dateTime,
            int qosLevel,
            bool convertPayloadToJson = false)
        {
            var payload = new VersionBPayload
            {
                Metrics = metrics,
                Seq = (ulong)sequenceNumber,
                Timestamp = (ulong)dateTime.ToUnixTimeMilliseconds(),
                // REMOVE ME IF STILL HERE //Timestamp = metrics.FirstOrDefault()?.Timestamp ?? (ulong)dateTime.ToUnixTimeMilliseconds(),
            };

            // Debug output.
            payload.ToJson();

            var serialized = PayloadHelper.Serialize(payload, convertPayloadToJson);

            return new MqttApplicationMessageBuilder()
                .WithTopic(
                    this.topicGenerator.GetTopic(
                        nameSpace,
                        groupIdentifier,
                        SparkplugMessageType.DeviceData,
                        edgeNodeIdentifier,
                        deviceIdentifier))
                .WithPayload(serialized)
                .WithQualityOfServiceLevel((MqttQualityOfServiceLevel)qosLevel)
                .WithRetainFlag()
                .Build();
        }

        /// <summary>
        /// Gets a NCMD message with namespace version A.
        /// </summary>
        /// <param name="nameSpace">The namespace.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="metrics">The metrics.</param>
        /// <param name="dateTime">The date time.</param>
        /// <param name="qosLevel">The qos level.</param>
        /// <param name="convertPayloadToJson">if set to <c>true</c> [convert payload to json].</param>
        /// <returns>
        /// A new NCMD <see cref="MqttApplicationMessage" />.
        /// </returns>
        private MqttApplicationMessage GetSparkPlugNodeCommandA(
            SparkplugNamespace nameSpace,
            string groupIdentifier,
            string edgeNodeIdentifier,
            List<VersionAPayload.KuraMetric> metrics,
            DateTimeOffset dateTime,
            int qosLevel,
            bool convertPayloadToJson = false)
        {
            var payload = new VersionAPayload
            {
                Metrics = metrics,
                Timestamp = dateTime.ToUnixTimeMilliseconds()
            };

            var serialized = PayloadHelper.Serialize(payload, convertPayloadToJson);

            return new MqttApplicationMessageBuilder()
                .WithTopic(
                    this.topicGenerator.GetTopic(
                        nameSpace,
                        groupIdentifier,
                        SparkplugMessageType.NodeCommand,
                        edgeNodeIdentifier,
                        string.Empty)).WithPayload(serialized)
                .WithQualityOfServiceLevel((MqttQualityOfServiceLevel)qosLevel)
                .WithRetainFlag()
                .Build();
        }

        /// <summary>
        /// Gets a NCMD message with namespace version B.
        /// </summary>
        /// <param name="nameSpace">The namespace.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="metrics">The metrics.</param>
        /// <param name="sequenceNumber">The sequence number.</param>
        /// <param name="dateTime">The date time.</param>
        /// <param name="qosLevel">The qos level.</param>
        /// <param name="convertPayloadToJson">if set to <c>true</c> [convert payload to json].</param>
        /// <returns>
        /// A new NCMD <see cref="MqttApplicationMessage" />.
        /// </returns>
        private MqttApplicationMessage GetSparkPlugNodeCommandB(
            SparkplugNamespace nameSpace,
            string groupIdentifier,
            string edgeNodeIdentifier,
            List<VersionBPayload.Metric> metrics,
            int sequenceNumber,
            DateTimeOffset dateTime,
            int qosLevel,
            bool convertPayloadToJson = false)
        {
            var payload = new VersionBPayload
            {
                Metrics = metrics,
                Seq = (ulong)sequenceNumber,
                Timestamp = (ulong)dateTime.ToUnixTimeMilliseconds(),
                // REMOVE IF STILL HERE //Timestamp = metrics.FirstOrDefault()?.Timestamp ?? (ulong)dateTime.ToUnixTimeMilliseconds(),
            };

            var serialized = PayloadHelper.Serialize(payload, convertPayloadToJson);

            return new MqttApplicationMessageBuilder()
                .WithTopic(
                    this.topicGenerator.GetTopic(
                        nameSpace,
                        groupIdentifier,
                        SparkplugMessageType.NodeCommand,
                        edgeNodeIdentifier,
                        string.Empty)).WithPayload(serialized)
                .WithQualityOfServiceLevel((MqttQualityOfServiceLevel)qosLevel)
                .WithRetainFlag()
                .Build();
        }

        /// <summary>
        /// Gets a DCMD message with namespace version A.WithQualityOfServiceLevel((MqttQualityOfServiceLevel)qosLevel)
        /// </summary>
        /// <param name="nameSpace">The namespace.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="deviceIdentifier">The device identifier.</param>
        /// <param name="metrics">The metrics.</param>
        /// <param name="dateTime">The date time.</param>
        /// <param name="qosLevel">The qos level.</param>
        /// <param name="convertPayloadToJson">if set to <c>true</c> [convert payload to json].</param>
        /// <returns>
        /// A new DCMD <see cref="MqttApplicationMessage" />.
        /// </returns>
        private MqttApplicationMessage GetSparkPlugDeviceCommandA(
            SparkplugNamespace nameSpace,
            string groupIdentifier,
            string edgeNodeIdentifier,
            string deviceIdentifier,
            List<VersionAPayload.KuraMetric> metrics,
            DateTimeOffset dateTime,
            int qosLevel,
            bool convertPayloadToJson = false)
        {
            var payload = new VersionAPayload
            {
                Metrics = metrics,
                Timestamp = dateTime.ToUnixTimeMilliseconds()
            };

            var serialized = PayloadHelper.Serialize(payload, convertPayloadToJson);

            return new MqttApplicationMessageBuilder()
                .WithTopic(
                    this.topicGenerator.GetTopic(
                        nameSpace,
                        groupIdentifier,
                        SparkplugMessageType.DeviceCommand,
                        edgeNodeIdentifier,
                        deviceIdentifier)).WithPayload(serialized)
                .WithQualityOfServiceLevel((MqttQualityOfServiceLevel)qosLevel)
                .WithRetainFlag()
                .Build();
        }

        /// <summary>
        /// Gets a DCMD message with namespace version B.
        /// </summary>
        /// <param name="nameSpace">The namespace.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="deviceIdentifier">The device identifier.</param>
        /// <param name="metrics">The metrics.</param>
        /// <param name="sequenceNumber">The sequence number.</param>
        /// <param name="dateTime">The date time.</param>
        /// <param name="qosLevel">The qos level.</param>
        /// <param name="convertPayloadToJson">if set to <c>true</c> [convert payload to json].</param>
        /// <returns>
        /// A new DCMD <see cref="MqttApplicationMessage" />.
        /// </returns>
        private MqttApplicationMessage GetSparkPlugDeviceCommandB(
            SparkplugNamespace nameSpace,
            string groupIdentifier,
            string edgeNodeIdentifier,
            string deviceIdentifier,
            List<VersionBPayload.Metric> metrics,
            int sequenceNumber,
            DateTimeOffset dateTime,
            int qosLevel,
            bool convertPayloadToJson = false)
        {
            var payload = new VersionBPayload
            {
                Metrics = metrics,
                Seq = (ulong)sequenceNumber,
                Timestamp = (ulong)dateTime.ToUnixTimeMilliseconds(),
                //// DELETE IF STILL HERE //// Timestamp = metrics.FirstOrDefault()?.Timestamp ?? (ulong)dateTime.ToUnixTimeMilliseconds(),
            };

            var serialized = PayloadHelper.Serialize(payload, convertPayloadToJson);

            return new MqttApplicationMessageBuilder()
                .WithTopic(
                    this.topicGenerator.GetTopic(
                        nameSpace,
                        groupIdentifier,
                        SparkplugMessageType.DeviceCommand,
                        edgeNodeIdentifier,
                        deviceIdentifier)).WithPayload(serialized)
                .WithQualityOfServiceLevel((MqttQualityOfServiceLevel)qosLevel)
                .WithRetainFlag()
                .Build();
        }

        /// <summary>
        /// Adds the session number to the metrics.
        /// </summary>
        /// <param name="metrics">The metrics.</param>
        /// <param name="sessionSequenceNumber">The session number.</param>
        private static void AddSessionNumberToMetrics(ICollection<VersionAPayload.KuraMetric> metrics, long sessionSequenceNumber)
        {
            // updated existing BDSEQ metric if exists
            var existingMetric = metrics.FirstOrDefault(x => x.Name == Constants.SessionNumberMetricName);
            if (existingMetric != null)
            {
                existingMetric.LongValue = sessionSequenceNumber;
                return;
            }

            // Add a BDSEQ metric.
            metrics.Add(new VersionAPayload.KuraMetric
            {
                Name = Constants.SessionNumberMetricName,
                LongValue = sessionSequenceNumber,
                Type = VersionAPayload.KuraMetric.ValueType.Int64
            });
        }

        /// <summary>
        /// Adds the session number to the metrics.
        /// </summary>
        /// <param name="metrics">The metrics.</param>
        /// <param name="sessionSequenceNumber">The session number.</param>
        private static void AddSessionNumberToMetrics(ICollection<VersionBPayload.Metric> metrics, long sessionSequenceNumber)
        {
            // updated existing BDSEQ metric if exists
            var existingMetric = metrics.FirstOrDefault(x => x.Name == Constants.SessionNumberMetricName);
            if (existingMetric != null)
            {
                existingMetric.LongValue = (ulong)sessionSequenceNumber;
                return;
            }

            // Add a BDSEQ metric.
            metrics.Add(new VersionBPayload.Metric
            {
                Name = Constants.SessionNumberMetricName,
                LongValue = (ulong)sessionSequenceNumber,
                Datatype = (uint)SparkplugBDataType.Int64,
            });
        }
    }
}