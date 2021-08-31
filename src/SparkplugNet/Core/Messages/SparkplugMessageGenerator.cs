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

    using MQTTnet;

    using Serilog;

    using SparkplugNet.Core.Enumerations;
    using SparkplugNet.Core.Extensions;

    using VersionA = VersionA.Data;
    using VersionB = VersionB.Data;

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
        /// The logger.
        /// </summary>
        private readonly ILogger? logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SparkplugMessageGenerator"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public SparkplugMessageGenerator(ILogger? logger)
        {
            this.logger = logger;
        }

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
        /// <typeparam name="T">The type parameter.</typeparam>
        /// <param name="nameSpace">The namespace.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="metrics">The metrics.</param>
        /// <param name="sequenceNumber">The sequence number.</param>
        /// <param name="sessionNumber">The session number.</param>
        /// <param name="dateTime">The date time.</param>
        /// <exception cref="ArgumentException">The group identifier or the edge node identifier is invalid.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The namespace is out of range.</exception>
        /// <returns>A new NBIRTH <see cref="MqttApplicationMessage"/>.</returns>
        public MqttApplicationMessage GetSparkPlugNodeBirthMessage<T>(
            SparkplugNamespace nameSpace,
            string groupIdentifier,
            string edgeNodeIdentifier,
            List<T> metrics,
            int sequenceNumber,
            long sessionNumber,
            DateTimeOffset dateTime)
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
                    var newMetrics = metrics as List<VersionA.KuraMetric> ?? new List<VersionA.KuraMetric>();
                    AddSessionNumberToMetrics(newMetrics, sessionNumber);
                    return this.GetSparkPlugNodeBirthA(nameSpace, groupIdentifier, edgeNodeIdentifier, newMetrics, dateTime);
                }

                case SparkplugNamespace.VersionB:
                {
                    var newMetrics = metrics as List<VersionB.Metric> ?? new List<VersionB.Metric>();
                    AddSessionNumberToMetrics(newMetrics, sessionNumber);
                    return this.GetSparkPlugNodeBirthB(nameSpace, groupIdentifier, edgeNodeIdentifier, newMetrics, sequenceNumber, dateTime);
                }

                default:
                    throw new ArgumentOutOfRangeException(nameof(nameSpace));
            }
        }

        /// <summary>
        /// Gets a DBIRTH message.
        /// </summary>
        /// <typeparam name="T">The type parameter.</typeparam>
        /// <param name="nameSpace">The namespace.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="deviceIdentifier">The device identifier.</param>
        /// <param name="metrics">The metrics.</param>
        /// <param name="sequenceNumber">The sequence number.</param>
        /// <param name="sessionNumber">The session number.</param>
        /// <param name="dateTime">The date time.</param>
        /// <exception cref="ArgumentException">The group identifier or the device identifier or the edge node identifier is invalid.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The namespace is out of range.</exception>
        /// <returns>A new DBIRTH <see cref="MqttApplicationMessage"/>.</returns>
        public MqttApplicationMessage GetSparkPlugDeviceBirthMessage<T>(
            SparkplugNamespace nameSpace,
            string groupIdentifier,
            string edgeNodeIdentifier,
            string deviceIdentifier,
            List<T> metrics,
            int sequenceNumber,
            long sessionNumber,
            DateTimeOffset dateTime)
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
                        var newMetrics = metrics as List<VersionA.KuraMetric>
                                         ?? new List<VersionA.KuraMetric>();
                        AddSessionNumberToMetrics(newMetrics, sessionNumber);
                        return this.GetSparkPlugDeviceBirthA(nameSpace, groupIdentifier, edgeNodeIdentifier, deviceIdentifier, newMetrics, dateTime);
                    }

                case SparkplugNamespace.VersionB:
                    {
                        var newMetrics = metrics as List<VersionB.Metric> ?? new List<VersionB.Metric>();
                        AddSessionNumberToMetrics(newMetrics, sessionNumber);
                        return this.GetSparkPlugDeviceBirthB(nameSpace, groupIdentifier, edgeNodeIdentifier, deviceIdentifier, newMetrics, sequenceNumber, dateTime);
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
        /// <param name="sessionNumber">The session number.</param>
        /// <exception cref="ArgumentException">The group identifier or the edge node identifier is invalid.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The namespace is out of range.</exception>
        /// <returns>A new NDEATH <see cref="MqttApplicationMessage"/>.</returns>
        public MqttApplicationMessage GetSparkPlugNodeDeathMessage(
            SparkplugNamespace nameSpace,
            string groupIdentifier,
            string edgeNodeIdentifier,
            long sessionNumber)
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
                        var metrics = new List<VersionA.KuraMetric>();
                        AddSessionNumberToMetrics(metrics, sessionNumber);
                        return this.GetSparkPlugNodeDeathA(nameSpace, groupIdentifier, edgeNodeIdentifier, metrics);
                    }

                case SparkplugNamespace.VersionB:
                    {
                        var metrics = new List<VersionB.Metric>();
                        AddSessionNumberToMetrics(metrics, sessionNumber);
                        return this.GetSparkPlugNodeDeathB(nameSpace, groupIdentifier, edgeNodeIdentifier, metrics);
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
        /// <exception cref="ArgumentException">The group identifier or the device identifier or the edge node identifier is invalid.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The namespace is out of range.</exception>
        /// <returns>A new DDEATH <see cref="MqttApplicationMessage"/>.</returns>
        public MqttApplicationMessage GetSparkPlugDeviceDeathMessage(
            SparkplugNamespace nameSpace,
            string groupIdentifier,
            string edgeNodeIdentifier,
            string deviceIdentifier,
            int sequenceNumber,
            long sessionNumber,
            DateTimeOffset dateTime)
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
                        var metrics = new List<VersionA.KuraMetric>();
                        AddSessionNumberToMetrics(metrics, sessionNumber);
                        return this.GetSparkPlugDeviceDeathA(nameSpace, groupIdentifier, edgeNodeIdentifier, deviceIdentifier, metrics, dateTime);
                    }

                case SparkplugNamespace.VersionB:
                    {
                        var metrics = new List<VersionB.Metric>();
                        AddSessionNumberToMetrics(metrics, sessionNumber);
                        return this.GetSparkPlugDeviceDeathB(nameSpace, groupIdentifier, edgeNodeIdentifier, deviceIdentifier, metrics, sequenceNumber, dateTime);
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(nameSpace));
            }
        }

        /// <summary>
        /// Gets a NDATA message.
        /// </summary>
        /// <typeparam name="T">The type parameter.</typeparam>
        /// <param name="nameSpace">The namespace.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="metrics">The metrics.</param>
        /// <param name="sequenceNumber">The sequence number.</param>
        /// <param name="sessionNumber">The session number.</param>
        /// <param name="dateTime">The date time.</param>
        /// <exception cref="ArgumentException">The group identifier or the edge node identifier is invalid.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The namespace is out of range.</exception>
        /// <returns>A new NDATA <see cref="MqttApplicationMessage"/>.</returns>
        public MqttApplicationMessage GetSparkPlugNodeDataMessage<T>(
            SparkplugNamespace nameSpace,
            string groupIdentifier,
            string edgeNodeIdentifier,
            List<T> metrics,
            int sequenceNumber,
            long sessionNumber,
            DateTimeOffset dateTime)
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
                        var newMetrics = metrics as List<VersionA.KuraMetric>
                                         ?? new List<VersionA.KuraMetric>();
                        AddSessionNumberToMetrics(newMetrics, sessionNumber);
                        return this.GetSparkPlugNodeDataA(nameSpace, groupIdentifier, edgeNodeIdentifier, newMetrics, dateTime);
                    }

                case SparkplugNamespace.VersionB:
                    {
                        var newMetrics = metrics as List<VersionB.Metric>
                                         ?? new List<VersionB.Metric>();
                        AddSessionNumberToMetrics(newMetrics, sessionNumber);
                        return this.GetSparkPlugNodeDataB(nameSpace, groupIdentifier, edgeNodeIdentifier, newMetrics, sequenceNumber, dateTime);
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(nameSpace));
            }
        }

        /// <summary>
        /// Gets a DDATA message.
        /// </summary>
        /// <typeparam name="T">The type parameter.</typeparam>
        /// <param name="nameSpace">The namespace.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="deviceIdentifier">The device identifier.</param>
        /// <param name="metrics">The metrics.</param>
        /// <param name="sequenceNumber">The sequence number.</param>
        /// <param name="sessionNumber">The session number.</param>
        /// <param name="dateTime">The date time.</param>
        /// <exception cref="ArgumentException">The group identifier or the device identifier or the edge node identifier is invalid.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The namespace is out of range.</exception>
        /// <returns>A new DDATA <see cref="MqttApplicationMessage"/>.</returns>
        public MqttApplicationMessage GetSparkPlugDeviceDataMessage<T>(
            SparkplugNamespace nameSpace,
            string groupIdentifier,
            string edgeNodeIdentifier,
            string deviceIdentifier,
            List<T> metrics,
            int sequenceNumber,
            long sessionNumber,
            DateTimeOffset dateTime)
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
                        var newMetrics = metrics as List<VersionA.KuraMetric>
                                         ?? new List<VersionA.KuraMetric>();
                        AddSessionNumberToMetrics(newMetrics, sessionNumber);
                        return this.GetSparkPlugDeviceDataA(nameSpace, groupIdentifier, edgeNodeIdentifier, deviceIdentifier, newMetrics, dateTime);
                    }

                case SparkplugNamespace.VersionB:
                    {
                        var newMetrics = metrics as List<VersionB.Metric>
                                         ?? new List<VersionB.Metric>();
                        AddSessionNumberToMetrics(newMetrics, sessionNumber);
                        return this.GetSparkPlugDeviceDataB(nameSpace, groupIdentifier, edgeNodeIdentifier, deviceIdentifier, newMetrics, sequenceNumber, dateTime);
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(nameSpace));
            }
        }

        /// <summary>
        /// Gets a NCMD message.
        /// </summary>
        /// <typeparam name="T">The type parameter.</typeparam>
        /// <param name="nameSpace">The namespace.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="metrics">The metrics.</param>
        /// <param name="sequenceNumber">The sequence number.</param>
        /// <param name="sessionNumber">The session number.</param>
        /// <param name="dateTime">The date time.</param>
        /// <exception cref="ArgumentException">The group identifier or the edge node identifier is invalid.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The namespace is out of range.</exception>
        /// <returns>A new NCMD <see cref="MqttApplicationMessage"/>.</returns>
        public MqttApplicationMessage GetSparkPlugNodeCommandMessage<T>(
            SparkplugNamespace nameSpace,
            string groupIdentifier,
            string edgeNodeIdentifier,
            List<T> metrics,
            int sequenceNumber,
            long sessionNumber,
            DateTimeOffset dateTime)
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
                        var newMetrics = metrics as List<VersionA.KuraMetric>
                                         ?? new List<VersionA.KuraMetric>();
                        AddSessionNumberToMetrics(newMetrics, sessionNumber);
                        return this.GetSparkPlugNodeCommandA(nameSpace, groupIdentifier, edgeNodeIdentifier, newMetrics, dateTime);
                    }

                case SparkplugNamespace.VersionB:
                    {
                        var newMetrics = metrics as List<VersionB.Metric>
                                         ?? new List<VersionB.Metric>();
                        AddSessionNumberToMetrics(newMetrics, sessionNumber);
                        return this.GetSparkPlugNodeCommandB(nameSpace, groupIdentifier, edgeNodeIdentifier, newMetrics, sequenceNumber, dateTime);
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(nameSpace));
            }
        }

        /// <summary>
        /// Gets a DCMD message.
        /// </summary>
        /// <typeparam name="T">The type parameter.</typeparam>
        /// <param name="nameSpace">The namespace.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="deviceIdentifier">The device identifier.</param>
        /// <param name="metrics">The metrics.</param>
        /// <param name="sequenceNumber">The sequence number.</param>
        /// <param name="sessionNumber">The session number.</param>
        /// <param name="dateTime">The date time.</param>
        /// <exception cref="ArgumentException">The group identifier or the device identifier or the edge node identifier is invalid.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The namespace is out of range.</exception>
        /// <returns>A new DCMD <see cref="MqttApplicationMessage"/>.</returns>
        public MqttApplicationMessage GetSparkPlugDeviceCommandMessage<T>(
            SparkplugNamespace nameSpace,
            string groupIdentifier,
            string edgeNodeIdentifier,
            string deviceIdentifier,
            List<T> metrics,
            int sequenceNumber,
            long sessionNumber,
            DateTimeOffset dateTime)
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
                        var newMetrics = metrics as List<VersionA.KuraMetric>
                                         ?? new List<VersionA.KuraMetric>();
                        AddSessionNumberToMetrics(newMetrics, sessionNumber);
                        return this.GetSparkPlugDeviceCommandA(nameSpace, groupIdentifier, edgeNodeIdentifier, deviceIdentifier, newMetrics, dateTime);
                    }

                case SparkplugNamespace.VersionB:
                    {
                        var newMetrics = metrics as List<VersionB.Metric>
                                         ?? new List<VersionB.Metric>();
                        AddSessionNumberToMetrics(newMetrics, sessionNumber);
                        return this.GetSparkPlugDeviceCommandB(nameSpace, groupIdentifier, edgeNodeIdentifier, deviceIdentifier, newMetrics, sequenceNumber, dateTime);
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(nameSpace));
            }
        }

        /// <summary>
        /// Adds the session number to the metrics.
        /// </summary>
        /// <param name="metrics">The metrics.</param>
        /// <param name="sessionSequenceNumber">The session number.</param>
        private static void AddSessionNumberToMetrics(ICollection<VersionA.KuraMetric> metrics, long sessionSequenceNumber)
        {
            // Add a BDSEQ metric.
            metrics.Add(new VersionA.KuraMetric
            {
                Name = Constants.SessionNumberMetricName,
                LongValue = sessionSequenceNumber,
                Type = VersionA.DataType.Int64
            });
        }

        /// <summary>
        /// Adds the session number to the metrics.
        /// </summary>
        /// <param name="metrics">The metrics.</param>
        /// <param name="sessionSequenceNumber">The session number.</param>
        private static void AddSessionNumberToMetrics(ICollection<VersionB.Metric> metrics, long sessionSequenceNumber)
        {
            // Add a BDSEQ metric.
            metrics.Add(new VersionB.Metric
            {
                Name = Constants.SessionNumberMetricName,
                LongValue = (ulong)sessionSequenceNumber,
                DataType = (uint)VersionB.DataType.Int64
            });
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
        /// <returns>A new NBIRTH <see cref="MqttApplicationMessage"/>.</returns>
        private MqttApplicationMessage GetSparkPlugNodeBirthA(
            SparkplugNamespace nameSpace,
            string groupIdentifier,
            string edgeNodeIdentifier,
            List<VersionA.KuraMetric> metrics,
            DateTimeOffset dateTime)
        {
            var payload = new VersionA.Payload
            {
                Metrics = metrics,
                Timestamp = dateTime.ToUnixTimeMilliseconds()
            };

            // Debug output.
            this.logger?.Debug("NBIRTH: VersionAPayload: {@Payload}", payload);

            var convertedPayload = PayloadConverter.ConvertVersionAPayload(payload);
            var serialized = PayloadHelper.Serialize(convertedPayload);

            return new MqttApplicationMessageBuilder()
                .WithTopic(
                    this.topicGenerator.GetTopic(
                        nameSpace,
                        groupIdentifier,
                        SparkplugMessageType.NodeBirth,
                        edgeNodeIdentifier,
                        string.Empty)).WithPayload(serialized)
                .WithAtLeastOnceQoS()
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
        /// <returns>A new NBIRTH <see cref="MqttApplicationMessage"/>.</returns>
        private MqttApplicationMessage GetSparkPlugNodeBirthB(
            SparkplugNamespace nameSpace,
            string groupIdentifier,
            string edgeNodeIdentifier,
            List<VersionB.Metric> metrics,
            int sequenceNumber,
            DateTimeOffset dateTime)
        {
            var payload = new VersionB.Payload
            {
                Metrics = metrics,
                Seq = (ulong)sequenceNumber,
                Timestamp = (ulong)dateTime.ToUnixTimeMilliseconds()
            };

            // Debug output.
            this.logger?.Debug("NBIRTH: VersionBPayload: {@Payload}", payload);

            var convertedPayload = PayloadConverter.ConvertVersionBPayload(payload);
            var serialized = PayloadHelper.Serialize(convertedPayload);

            return new MqttApplicationMessageBuilder()
                .WithTopic(
                    this.topicGenerator.GetTopic(
                        nameSpace,
                        groupIdentifier,
                        SparkplugMessageType.NodeBirth,
                        edgeNodeIdentifier,
                        string.Empty))
                .WithPayload(serialized)
                .WithAtLeastOnceQoS()
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
        /// <returns>A new DBIRTH <see cref="MqttApplicationMessage"/>.</returns>
        private MqttApplicationMessage GetSparkPlugDeviceBirthA(
            SparkplugNamespace nameSpace,
            string groupIdentifier,
            string edgeNodeIdentifier,
            string deviceIdentifier,
            List<VersionA.KuraMetric> metrics,
            DateTimeOffset dateTime)
        {
            var payload = new VersionA.Payload
            {
                Metrics = metrics,
                Timestamp = dateTime.ToUnixTimeMilliseconds()
            };

            // Debug output.
            this.logger?.Debug("DBIRTH: VersionAPayload: {@Payload}", payload);

            var convertedPayload = PayloadConverter.ConvertVersionAPayload(payload);
            var serialized = PayloadHelper.Serialize(convertedPayload);

            return new MqttApplicationMessageBuilder()
                .WithTopic(
                    this.topicGenerator.GetTopic(
                        nameSpace,
                        groupIdentifier,
                        SparkplugMessageType.DeviceBirth,
                        edgeNodeIdentifier,
                        deviceIdentifier))
                .WithPayload(serialized)
                .WithAtLeastOnceQoS()
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
        /// <returns>A new DBIRTH <see cref="MqttApplicationMessage"/>.</returns>
        private MqttApplicationMessage GetSparkPlugDeviceBirthB(
            SparkplugNamespace nameSpace,
            string groupIdentifier,
            string edgeNodeIdentifier,
            string deviceIdentifier,
            List<VersionB.Metric> metrics,
            int sequenceNumber,
            DateTimeOffset dateTime)
        {
            var payload = new VersionB.Payload
            {
                Metrics = metrics,
                Seq = (ulong)sequenceNumber,
                Timestamp = (ulong)dateTime.ToUnixTimeMilliseconds()
            };

            // Debug output.
            this.logger?.Debug("DBIRTH: VersionBPayload: {@Payload}", payload);

            var convertedPayload = PayloadConverter.ConvertVersionBPayload(payload);
            var serialized = PayloadHelper.Serialize(convertedPayload);

            return new MqttApplicationMessageBuilder()
                .WithTopic(
                    this.topicGenerator.GetTopic(
                        nameSpace,
                        groupIdentifier,
                        SparkplugMessageType.DeviceBirth,
                        edgeNodeIdentifier,
                        deviceIdentifier))
                .WithPayload(serialized)
                .WithAtLeastOnceQoS()
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
        /// <returns>A new NDEATH <see cref="MqttApplicationMessage"/>.</returns>
        private MqttApplicationMessage GetSparkPlugNodeDeathA(
            SparkplugNamespace nameSpace,
            string groupIdentifier,
            string edgeNodeIdentifier,
            List<VersionA.KuraMetric> metrics)
        {
            var payload = new VersionA.Payload
            {
                Metrics = metrics
            };

            // Debug output.
            this.logger?.Debug("NDEATH: VersionAPayload: {@Payload}", payload);

            var convertedPayload = PayloadConverter.ConvertVersionAPayload(payload);
            var serialized = PayloadHelper.Serialize(convertedPayload);

            return new MqttApplicationMessageBuilder()
                .WithTopic(
                    this.topicGenerator.GetTopic(
                        nameSpace,
                        groupIdentifier,
                        SparkplugMessageType.NodeDeath,
                        edgeNodeIdentifier,
                        string.Empty))
                .WithPayload(serialized)
                .WithAtLeastOnceQoS()
                .WithRetainFlag()
                .Build();
        }

        /// <summary>
        /// Gets a NDEATH message with namespace version B.
        /// </summary>
        /// <param name="nameSpace">The namespace.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="metrics">The metrics.</param>
        /// <returns>A new NDEATH <see cref="MqttApplicationMessage"/>.</returns>
        private MqttApplicationMessage GetSparkPlugNodeDeathB(
            SparkplugNamespace nameSpace,
            string groupIdentifier,
            string edgeNodeIdentifier,
            List<VersionB.Metric> metrics)
        {
            var payload = new VersionB.Payload
            {
                Metrics = metrics
            };

            // Debug output.
            this.logger?.Debug("NDEATH: VersionBPayload: {@Payload}", payload);

            var convertedPayload = PayloadConverter.ConvertVersionBPayload(payload);
            var serialized = PayloadHelper.Serialize(convertedPayload);

            return new MqttApplicationMessageBuilder()
                .WithTopic(
                    this.topicGenerator.GetTopic(
                        nameSpace,
                        groupIdentifier,
                        SparkplugMessageType.NodeDeath,
                        edgeNodeIdentifier,
                        string.Empty))
                .WithPayload(serialized)
                .WithAtLeastOnceQoS()
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
        /// <param name="metrics">The metrics.</param>
        /// <param name="dateTime">The date time.</param>
        /// <returns>A new DDEATH <see cref="MqttApplicationMessage"/>.</returns>
        private MqttApplicationMessage GetSparkPlugDeviceDeathA(
            SparkplugNamespace nameSpace,
            string groupIdentifier,
            string edgeNodeIdentifier,
            string deviceIdentifier,
            List<VersionA.KuraMetric> metrics,
            DateTimeOffset dateTime)
        {
            var payload = new VersionA.Payload
            {
                Metrics = metrics,
                Timestamp = dateTime.ToUnixTimeMilliseconds()
            };

            // Debug output.
            this.logger?.Debug("DDEATH: VersionAPayload: {@Payload}", payload);

            var convertedPayload = PayloadConverter.ConvertVersionAPayload(payload);
            var serialized = PayloadHelper.Serialize(convertedPayload);

            return new MqttApplicationMessageBuilder()
                .WithTopic(
                    this.topicGenerator.GetTopic(
                        nameSpace,
                        groupIdentifier,
                        SparkplugMessageType.DeviceDeath,
                        edgeNodeIdentifier,
                        deviceIdentifier))
                .WithPayload(serialized)
                .WithAtLeastOnceQoS()
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
        /// <param name="metrics">The metrics.</param>
        /// <param name="sequenceNumber">The sequence number.</param>
        /// <param name="dateTime">The date time.</param>
        /// <returns>A new DDEATH <see cref="MqttApplicationMessage"/>.</returns>
        private MqttApplicationMessage GetSparkPlugDeviceDeathB(
            SparkplugNamespace nameSpace,
            string groupIdentifier,
            string edgeNodeIdentifier,
            string deviceIdentifier,
            List<VersionB.Metric> metrics,
            int sequenceNumber,
            DateTimeOffset dateTime)
        {
            var payload = new VersionB.Payload
            {
                Metrics = metrics,
                Seq = (ulong)sequenceNumber,
                Timestamp = (ulong)dateTime.ToUnixTimeMilliseconds()
            };

            // Debug output.
            this.logger?.Debug("DDEATH: VersionBPayload: {@Payload}", payload);

            var convertedPayload = PayloadConverter.ConvertVersionBPayload(payload);
            var serialized = PayloadHelper.Serialize(convertedPayload);

            return new MqttApplicationMessageBuilder()
                .WithTopic(
                    this.topicGenerator.GetTopic(
                        nameSpace,
                        groupIdentifier,
                        SparkplugMessageType.DeviceDeath,
                        edgeNodeIdentifier,
                        deviceIdentifier))
                .WithPayload(serialized)
                .WithAtLeastOnceQoS()
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
        /// <returns>A new NDATA <see cref="MqttApplicationMessage"/>.</returns>
        private MqttApplicationMessage GetSparkPlugNodeDataA(
            SparkplugNamespace nameSpace,
            string groupIdentifier,
            string edgeNodeIdentifier,
            List<VersionA.KuraMetric> metrics,
            DateTimeOffset dateTime)
        {
            var payload = new VersionA.Payload
            {
                Metrics = metrics,
                Timestamp = dateTime.ToUnixTimeMilliseconds()
            };

            // Debug output.
            this.logger?.Debug("NDATA: VersionAPayload: {@Payload}", payload);

            var convertedPayload = PayloadConverter.ConvertVersionAPayload(payload);
            var serialized = PayloadHelper.Serialize(convertedPayload);

            return new MqttApplicationMessageBuilder()
                .WithTopic(
                    this.topicGenerator.GetTopic(
                        nameSpace,
                        groupIdentifier,
                        SparkplugMessageType.NodeData,
                        edgeNodeIdentifier,
                        string.Empty))
                .WithPayload(serialized)
                .WithAtLeastOnceQoS()
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
        /// <returns>A new NDATA <see cref="MqttApplicationMessage"/>.</returns>
        private MqttApplicationMessage GetSparkPlugNodeDataB(
            SparkplugNamespace nameSpace,
            string groupIdentifier,
            string edgeNodeIdentifier,
            List<VersionB.Metric> metrics,
            int sequenceNumber,
            DateTimeOffset dateTime)
        {
            var payload = new VersionB.Payload
            {
                Metrics = metrics,
                Seq = (ulong)sequenceNumber,
                Timestamp = (ulong)dateTime.ToUnixTimeMilliseconds()
            };

            // Debug output.
            this.logger?.Debug("NDATA: VersionBPayload: {@Payload}", payload);

            var convertedPayload = PayloadConverter.ConvertVersionBPayload(payload);
            var serialized = PayloadHelper.Serialize(convertedPayload);

            return new MqttApplicationMessageBuilder()
                .WithTopic(
                    this.topicGenerator.GetTopic(
                        nameSpace,
                        groupIdentifier,
                        SparkplugMessageType.NodeData,
                        edgeNodeIdentifier,
                        string.Empty))
                .WithPayload(serialized)
                .WithAtLeastOnceQoS()
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
        /// <returns>A new DDATA <see cref="MqttApplicationMessage"/>.</returns>
        private MqttApplicationMessage GetSparkPlugDeviceDataA(
            SparkplugNamespace nameSpace,
            string groupIdentifier,
            string edgeNodeIdentifier,
            string deviceIdentifier,
            List<VersionA.KuraMetric> metrics,
            DateTimeOffset dateTime)
        {
            var payload = new VersionA.Payload
            {
                Metrics = metrics,
                Timestamp = dateTime.ToUnixTimeMilliseconds()
            };

            // Debug output.
            this.logger?.Debug("DDATA: VersionAPayload: {@Payload}", payload);

            var convertedPayload = PayloadConverter.ConvertVersionAPayload(payload);
            var serialized = PayloadHelper.Serialize(convertedPayload);

            return new MqttApplicationMessageBuilder()
                .WithTopic(
                    this.topicGenerator.GetTopic(
                        nameSpace,
                        groupIdentifier,
                        SparkplugMessageType.DeviceData,
                        edgeNodeIdentifier,
                        deviceIdentifier))
                .WithPayload(serialized)
                .WithAtLeastOnceQoS()
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
        /// <returns>A new DDATA <see cref="MqttApplicationMessage"/>.</returns>
        private MqttApplicationMessage GetSparkPlugDeviceDataB(
            SparkplugNamespace nameSpace,
            string groupIdentifier,
            string edgeNodeIdentifier,
            string deviceIdentifier,
            List<VersionB.Metric> metrics,
            int sequenceNumber,
            DateTimeOffset dateTime)
        {
            var payload = new VersionB.Payload
            {
                Metrics = metrics,
                Seq = (ulong)sequenceNumber,
                Timestamp = (ulong)dateTime.ToUnixTimeMilliseconds()
            };

            // Debug output.
            this.logger?.Debug("DDATA: VersionBPayload: {@Payload}", payload);

            var convertedPayload = PayloadConverter.ConvertVersionBPayload(payload);
            var serialized = PayloadHelper.Serialize(convertedPayload);

            return new MqttApplicationMessageBuilder()
                .WithTopic(
                    this.topicGenerator.GetTopic(
                        nameSpace,
                        groupIdentifier,
                        SparkplugMessageType.DeviceData,
                        edgeNodeIdentifier,
                        deviceIdentifier))
                .WithPayload(serialized)
                .WithAtLeastOnceQoS()
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
        /// <returns>A new NCMD <see cref="MqttApplicationMessage"/>.</returns>
        private MqttApplicationMessage GetSparkPlugNodeCommandA(
            SparkplugNamespace nameSpace,
            string groupIdentifier,
            string edgeNodeIdentifier,
            List<VersionA.KuraMetric> metrics,
            DateTimeOffset dateTime)
        {
            var payload = new VersionA.Payload
            {
                Metrics = metrics,
                Timestamp = dateTime.ToUnixTimeMilliseconds()
            };

            var convertedPayload = PayloadConverter.ConvertVersionAPayload(payload);
            var serialized = PayloadHelper.Serialize(convertedPayload);

            return new MqttApplicationMessageBuilder()
                .WithTopic(
                    this.topicGenerator.GetTopic(
                        nameSpace,
                        groupIdentifier,
                        SparkplugMessageType.NodeCommand,
                        edgeNodeIdentifier,
                        string.Empty)).WithPayload(serialized)
                .WithAtLeastOnceQoS()
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
        /// <returns>A new NCMD <see cref="MqttApplicationMessage"/>.</returns>
        private MqttApplicationMessage GetSparkPlugNodeCommandB(
            SparkplugNamespace nameSpace,
            string groupIdentifier,
            string edgeNodeIdentifier,
            List<VersionB.Metric> metrics,
            int sequenceNumber,
            DateTimeOffset dateTime)
        {
            var payload = new VersionB.Payload
            {
                Metrics = metrics,
                Seq = (ulong)sequenceNumber,
                Timestamp = (ulong)dateTime.ToUnixTimeMilliseconds()
            };

            var convertedPayload = PayloadConverter.ConvertVersionBPayload(payload);
            var serialized = PayloadHelper.Serialize(convertedPayload);

            return new MqttApplicationMessageBuilder()
                .WithTopic(
                    this.topicGenerator.GetTopic(
                        nameSpace,
                        groupIdentifier,
                        SparkplugMessageType.NodeCommand,
                        edgeNodeIdentifier,
                        string.Empty)).WithPayload(serialized)
                .WithAtLeastOnceQoS()
                .WithRetainFlag()
                .Build();
        }

        /// <summary>
        /// Gets a DCMD message with namespace version A.
        /// </summary>
        /// <param name="nameSpace">The namespace.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="deviceIdentifier">The device identifier.</param>
        /// <param name="metrics">The metrics.</param>
        /// <param name="dateTime">The date time.</param>
        /// <returns>A new DCMD <see cref="MqttApplicationMessage"/>.</returns>
        private MqttApplicationMessage GetSparkPlugDeviceCommandA(
            SparkplugNamespace nameSpace,
            string groupIdentifier,
            string edgeNodeIdentifier,
            string deviceIdentifier,
            List<VersionA.KuraMetric> metrics,
            DateTimeOffset dateTime)
        {
            var payload = new VersionA.Payload
            {
                Metrics = metrics,
                Timestamp = dateTime.ToUnixTimeMilliseconds()
            };

            var convertedPayload = PayloadConverter.ConvertVersionAPayload(payload);
            var serialized = PayloadHelper.Serialize(convertedPayload);

            return new MqttApplicationMessageBuilder()
                .WithTopic(
                    this.topicGenerator.GetTopic(
                        nameSpace,
                        groupIdentifier,
                        SparkplugMessageType.DeviceCommand,
                        edgeNodeIdentifier,
                        deviceIdentifier)).WithPayload(serialized)
                .WithAtLeastOnceQoS()
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
        /// <returns>A new DCMD <see cref="MqttApplicationMessage"/>.</returns>
        private MqttApplicationMessage GetSparkPlugDeviceCommandB(
            SparkplugNamespace nameSpace,
            string groupIdentifier,
            string edgeNodeIdentifier,
            string deviceIdentifier,
            List<VersionB.Metric> metrics,
            int sequenceNumber,
            DateTimeOffset dateTime)
        {
            var payload = new VersionB.Payload
            {
                Metrics = metrics,
                Seq = (ulong)sequenceNumber,
                Timestamp = (ulong)dateTime.ToUnixTimeMilliseconds()
            };

            var convertedPayload = PayloadConverter.ConvertVersionBPayload(payload);
            var serialized = PayloadHelper.Serialize(convertedPayload);

            return new MqttApplicationMessageBuilder()
                .WithTopic(
                    this.topicGenerator.GetTopic(
                        nameSpace,
                        groupIdentifier,
                        SparkplugMessageType.DeviceCommand,
                        edgeNodeIdentifier,
                        deviceIdentifier)).WithPayload(serialized)
                .WithAtLeastOnceQoS()
                .WithRetainFlag()
                .Build();
        }
    }
}