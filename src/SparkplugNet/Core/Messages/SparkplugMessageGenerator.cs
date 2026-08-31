// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugMessageGenerator.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   The Sparkplug message generator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Core.Messages;

/// <summary>
/// The Sparkplug message generator.
/// </summary>
internal sealed class SparkplugMessageGenerator
{
    /// <summary>
    /// The Sparkplug specification version.
    /// </summary>
    private readonly SparkplugSpecificationVersion specificationVersion;

    /// <summary>
    /// Initializes a new instance of the <see cref="SparkplugMessageGenerator"/> class.
    /// </summary>
    /// <param name="specificationVersion">The Sparkplug specification version.</param>
    public SparkplugMessageGenerator(SparkplugSpecificationVersion specificationVersion)
    {
        this.specificationVersion = specificationVersion;
    }

    /// <summary>
    /// Gets a STATE message.
    /// </summary>
    /// <param name="nameSpace">The namespace.</param>
    /// <param name="scadaHostIdentifier">The SCADA host identifier.</param>
    /// <param name="online">A value indicating whether the message sender is online or not.</param>
    /// <exception cref="ArgumentException">Thrown if the SCADA host identifier is invalid.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the namespace is out of range.</exception>
    /// <returns>A new STATE <see cref="MqttApplicationMessage"/>.</returns>
    internal MqttApplicationMessage GetSparkplugStateMessage(
        SparkplugNamespace nameSpace,
        string scadaHostIdentifier,
        bool online)
    {
        if (!scadaHostIdentifier.IsIdentifierValid())
        {
            throw new ArgumentException("The SCADA host identifier wasn't set properly.", nameof(scadaHostIdentifier));
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
    /// <exception cref="ArgumentException">Thrown if the group identifier or the edge node identifier is invalid.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the namespace is out of range.</exception>
    /// <returns>A new NBIRTH <see cref="MqttApplicationMessage"/>.</returns>
    internal MqttApplicationMessage GetSparkplugNodeBirthMessage<T>(
        SparkplugNamespace nameSpace,
        string groupIdentifier,
        string edgeNodeIdentifier,
        IEnumerable<T> metrics,
        int sequenceNumber,
        long sessionNumber,
        DateTimeOffset dateTime)
        where T : IMetric, new()
    {
        if (!groupIdentifier.IsIdentifierValid())
        {
            throw new ArgumentException("The group identifier wasn't set properly.", nameof(groupIdentifier));
        }

        if (!edgeNodeIdentifier.IsIdentifierValid())
        {
            throw new ArgumentException("The edge node identifier wasn't set properly.", nameof(edgeNodeIdentifier));
        }

        switch (nameSpace)
        {
            case SparkplugNamespace.VersionA:
                {
                    var newMetrics = metrics as IEnumerable<VersionAData.KuraMetric> ?? new List<VersionAData.KuraMetric>();
                    return this.GetSparkplugNodeBirthA(nameSpace, groupIdentifier, edgeNodeIdentifier,
                        AddSessionNumberToMetrics(newMetrics, sessionNumber), dateTime);
                }

            case SparkplugNamespace.VersionB:
                {
                    var newMetrics = metrics as IEnumerable<Metric> ?? new List<Metric>();
                    return this.GetSparkplugNodeBirthB(nameSpace, groupIdentifier, edgeNodeIdentifier,
                        AddSessionNumberToMetrics(newMetrics, sessionNumber), sequenceNumber, dateTime);
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
    /// <exception cref="ArgumentException">Thrown if the group identifier or the edge node identifier or the device identifier is invalid.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the namespace is out of range.</exception>
    /// <returns>A new DBIRTH <see cref="MqttApplicationMessage"/>.</returns>
    internal MqttApplicationMessage GetSparkplugDeviceBirthMessage<T>(
        SparkplugNamespace nameSpace,
        string groupIdentifier,
        string edgeNodeIdentifier,
        string deviceIdentifier,
        IEnumerable<T> metrics,
        int sequenceNumber,
        long sessionNumber,
        DateTimeOffset dateTime)
        where T : IMetric, new()
    {
        if (!groupIdentifier.IsIdentifierValid())
        {
            throw new ArgumentException("The group identifier wasn't set properly.", nameof(groupIdentifier));
        }

        if (!edgeNodeIdentifier.IsIdentifierValid())
        {
            throw new ArgumentException("The edge node identifier wasn't set properly.", nameof(edgeNodeIdentifier));
        }

        if (!deviceIdentifier.IsIdentifierValid())
        {
            throw new ArgumentException("The device identifier wasn't set properly.", nameof(deviceIdentifier));
        }

        switch (nameSpace)
        {
            case SparkplugNamespace.VersionA:
                {
                    var newMetrics = metrics as IEnumerable<VersionAData.KuraMetric>
                                     ?? new List<VersionAData.KuraMetric>();
                    return this.GetSparkplugDeviceBirthA(nameSpace, groupIdentifier, edgeNodeIdentifier, deviceIdentifier,
                         AddSessionNumberToMetrics(newMetrics, sessionNumber), dateTime);
                }

            case SparkplugNamespace.VersionB:
                {
                    var newMetrics = metrics as IEnumerable<Metric> ?? new List<Metric>();

                    return this.GetSparkplugDeviceBirthB(nameSpace, groupIdentifier, edgeNodeIdentifier, deviceIdentifier,
                        AddSessionNumberToMetrics(newMetrics, sessionNumber), sequenceNumber, dateTime);
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
    /// <exception cref="ArgumentException">Thrown if the group identifier or the edge node identifier is invalid.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the namespace is out of range.</exception>
    /// <returns>A new NDEATH <see cref="MqttApplicationMessage"/>.</returns>
    internal MqttApplicationMessage GetSparkplugNodeDeathMessage(
        SparkplugNamespace nameSpace,
        string groupIdentifier,
        string edgeNodeIdentifier,
        long sessionNumber)
    {
        if (!groupIdentifier.IsIdentifierValid())
        {
            throw new ArgumentException("The group identifier wasn't set properly.", nameof(groupIdentifier));
        }

        if (!edgeNodeIdentifier.IsIdentifierValid())
        {
            throw new ArgumentException("The edge node identifier wasn't set properly.", nameof(edgeNodeIdentifier));
        }

        switch (nameSpace)
        {
            case SparkplugNamespace.VersionA:
                {
                    var metrics = new List<VersionAData.KuraMetric>();
                    return this.GetSparkplugNodeDeathA(nameSpace, groupIdentifier, edgeNodeIdentifier,
                        AddSessionNumberToMetrics(metrics, sessionNumber));
                }

            case SparkplugNamespace.VersionB:
                {
                    var metrics = new List<Metric>();
                    return this.GetSparkplugNodeDeathB(nameSpace, groupIdentifier, edgeNodeIdentifier,
                        AddSessionNumberToMetrics(metrics, sessionNumber));
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
    /// <exception cref="ArgumentException">Thrown if the group identifier or the edge node identifier or the device identifier is invalid.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the namespace is out of range.</exception>
    /// <returns>A new DDEATH <see cref="MqttApplicationMessage"/>.</returns>
    internal MqttApplicationMessage GetSparkplugDeviceDeathMessage(
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
            throw new ArgumentException("The group identifier wasn't set properly.", nameof(groupIdentifier));
        }

        if (!edgeNodeIdentifier.IsIdentifierValid())
        {
            throw new ArgumentException("The edge node identifier wasn't set properly.", nameof(edgeNodeIdentifier));
        }

        if (!deviceIdentifier.IsIdentifierValid())
        {
            throw new ArgumentException("The device identifier wasn't set properly.", nameof(deviceIdentifier));
        }

        switch (nameSpace)
        {
            case SparkplugNamespace.VersionA:
                {
                    var metrics = new List<VersionAData.KuraMetric>();
                    return this.GetSparkplugDeviceDeathA(nameSpace, groupIdentifier, edgeNodeIdentifier, deviceIdentifier,
                        AddSessionNumberToMetrics(metrics, sessionNumber), dateTime);
                }

            case SparkplugNamespace.VersionB:
                {
                    var metrics = new List<Metric>();
                    return this.GetSparkplugDeviceDeathB(nameSpace, groupIdentifier, edgeNodeIdentifier, deviceIdentifier,
                        AddSessionNumberToMetrics(metrics, sessionNumber), sequenceNumber, dateTime);
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
    /// <exception cref="ArgumentException">Thrown if the group identifier or the edge node identifier is invalid.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the namespace is out of range.</exception>
    /// <returns>A new NDATA <see cref="MqttApplicationMessage"/>.</returns>
    internal MqttApplicationMessage GetSparkplugNodeDataMessage<T>(
        SparkplugNamespace nameSpace,
        string groupIdentifier,
        string edgeNodeIdentifier,
        IEnumerable<T> metrics,
        int sequenceNumber,
        long sessionNumber,
        DateTimeOffset dateTime)
        where T : IMetric, new()
    {
        if (!groupIdentifier.IsIdentifierValid())
        {
            throw new ArgumentException("The group identifier wasn't set properly.", nameof(groupIdentifier));
        }

        if (!edgeNodeIdentifier.IsIdentifierValid())
        {
            throw new ArgumentException("The edge node identifier wasn't set properly.", nameof(edgeNodeIdentifier));
        }

        switch (nameSpace)
        {
            case SparkplugNamespace.VersionA:
                {
                    var newMetrics = metrics as IEnumerable<VersionAData.KuraMetric>
                                     ?? new List<VersionAData.KuraMetric>();
                    return this.GetSparkplugNodeDataA(nameSpace, groupIdentifier, edgeNodeIdentifier,
                        AddSessionNumberToMetrics(newMetrics, sessionNumber), dateTime);
                }

            case SparkplugNamespace.VersionB:
                {
                    var newMetrics = metrics as IEnumerable<Metric>
                                     ?? new List<Metric>();
                    return this.GetSparkplugNodeDataB(nameSpace, groupIdentifier, edgeNodeIdentifier,
                        AddSessionNumberToMetrics(newMetrics, sessionNumber), sequenceNumber, dateTime);
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
    /// <exception cref="ArgumentException">Thrown if the group identifier or the edge node identifier or the device identifier is invalid.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the namespace is out of range.</exception>
    /// <returns>A new DDATA <see cref="MqttApplicationMessage"/>.</returns>
    internal MqttApplicationMessage GetSparkplugDeviceDataMessage<T>(
        SparkplugNamespace nameSpace,
        string groupIdentifier,
        string edgeNodeIdentifier,
        string deviceIdentifier,
        IEnumerable<T> metrics,
        int sequenceNumber,
        long sessionNumber,
        DateTimeOffset dateTime)
        where T : IMetric, new()
    {
        if (!groupIdentifier.IsIdentifierValid())
        {
            throw new ArgumentException("The group identifier wasn't set properly.", nameof(groupIdentifier));
        }

        if (!edgeNodeIdentifier.IsIdentifierValid())
        {
            throw new ArgumentException("The edge node identifier wasn't set properly.", nameof(edgeNodeIdentifier));
        }

        if (!deviceIdentifier.IsIdentifierValid())
        {
            throw new ArgumentException("The device identifier wasn't set properly.", nameof(deviceIdentifier));
        }

        switch (nameSpace)
        {
            case SparkplugNamespace.VersionA:
                {
                    var newMetrics = metrics as IEnumerable<VersionAData.KuraMetric>
                                     ?? new List<VersionAData.KuraMetric>();
                    return this.GetSparkplugDeviceDataA(nameSpace, groupIdentifier, edgeNodeIdentifier, deviceIdentifier,
                        AddSessionNumberToMetrics(newMetrics, sessionNumber), dateTime);
                }

            case SparkplugNamespace.VersionB:
                {
                    var newMetrics = metrics as IEnumerable<Metric>
                                     ?? new List<Metric>();
                    return this.GetSparkplugDeviceDataB(nameSpace, groupIdentifier, edgeNodeIdentifier, deviceIdentifier,
                         AddSessionNumberToMetrics(newMetrics, sessionNumber), sequenceNumber, dateTime);
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
    /// <exception cref="ArgumentException">Thrown if the group identifier or the edge node identifier is invalid.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the namespace is out of range.</exception>
    /// <returns>A new NCMD <see cref="MqttApplicationMessage"/>.</returns>
    internal MqttApplicationMessage GetSparkplugNodeCommandMessage<T>(
        SparkplugNamespace nameSpace,
        string groupIdentifier,
        string edgeNodeIdentifier,
        IEnumerable<T> metrics,
        int sequenceNumber,
        long sessionNumber,
        DateTimeOffset dateTime)
        where T : IMetric, new()
    {
        if (!groupIdentifier.IsIdentifierValid())
        {
            throw new ArgumentException("The group identifier wasn't set properly.", nameof(groupIdentifier));
        }

        if (!edgeNodeIdentifier.IsIdentifierValid())
        {
            throw new ArgumentException("The edge node identifier wasn't set properly.", nameof(edgeNodeIdentifier));
        }

        switch (nameSpace)
        {
            case SparkplugNamespace.VersionA:
                {
                    var newMetrics = metrics as IEnumerable<VersionAData.KuraMetric>
                                     ?? new List<VersionAData.KuraMetric>();
                    return GetSparkplugNodeCommandA(nameSpace, groupIdentifier, edgeNodeIdentifier,
                        AddSessionNumberToMetrics(newMetrics, sessionNumber), dateTime);
                }

            case SparkplugNamespace.VersionB:
                {
                    var newMetrics = metrics as IEnumerable<Metric>
                                     ?? new List<Metric>();

                    return GetSparkplugNodeCommandB(nameSpace, groupIdentifier, edgeNodeIdentifier,
                         AddSessionNumberToMetrics(newMetrics, sessionNumber), sequenceNumber, dateTime);
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
    /// <exception cref="ArgumentException">Thrown if the group identifier or the edge node identifier or the device identifier is invalid.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the namespace is out of range.</exception>
    /// <returns>A new DCMD <see cref="MqttApplicationMessage"/>.</returns>
    internal MqttApplicationMessage GetSparkplugDeviceCommandMessage<T>(
        SparkplugNamespace nameSpace,
        string groupIdentifier,
        string edgeNodeIdentifier,
        string deviceIdentifier,
        IEnumerable<T> metrics,
        int sequenceNumber,
        long sessionNumber,
        DateTimeOffset dateTime)
        where T : IMetric, new()
    {
        if (!groupIdentifier.IsIdentifierValid())
        {
            throw new ArgumentException("The group identifier wasn't set properly.", nameof(groupIdentifier));
        }

        if (!edgeNodeIdentifier.IsIdentifierValid())
        {
            throw new ArgumentException("The edge node identifier wasn't set properly.", nameof(edgeNodeIdentifier));
        }

        if (!deviceIdentifier.IsIdentifierValid())
        {
            throw new ArgumentException("The device identifier wasn't set properly.", nameof(deviceIdentifier));
        }

        switch (nameSpace)
        {
            case SparkplugNamespace.VersionA:
                {
                    var newMetrics = metrics as IEnumerable<VersionAData.KuraMetric>
                                     ?? new List<VersionAData.KuraMetric>();
                    newMetrics = AddSessionNumberToMetrics(newMetrics, sessionNumber);

                    return GetSparkplugDeviceCommandA(nameSpace, groupIdentifier, edgeNodeIdentifier, deviceIdentifier, newMetrics, dateTime);
                }

            case SparkplugNamespace.VersionB:
                {
                    var newMetrics = metrics as IEnumerable<Metric>
                                     ?? new List<Metric>();

                    newMetrics = AddSessionNumberToMetrics(newMetrics, sessionNumber);

                    return GetSparkplugDeviceCommandB(nameSpace, groupIdentifier, edgeNodeIdentifier, deviceIdentifier, newMetrics, sequenceNumber, dateTime);
                }

            default:
                throw new ArgumentOutOfRangeException(nameof(nameSpace));
        }
    }

    /// <summary>Adds the session number to the version A metrics.</summary>
    /// <param name="metrics">The metrics.</param>
    /// <param name="sessionSequenceNumber">The session sequence number.</param>
    /// <returns>The metrics.</returns>
    private static IEnumerable<VersionAData.KuraMetric> AddSessionNumberToMetrics(
        IEnumerable<VersionAData.KuraMetric> metrics,
        long sessionSequenceNumber)
    {
        // Add a BDSEQ metric.
        return metrics.Concat(
        [
            new VersionAData.KuraMetric(Constants.SessionNumberMetricName, VersionADataTypeEnum.Int64, sessionSequenceNumber)
        ]);
    }

    /// <summary>Adds the session number to the version B metrics.</summary>
    /// <param name="metrics">The metrics.</param>
    /// <param name="sessionSequenceNumber">The session sequence number.</param>
    /// <returns>The metrics.</returns>
    private static IEnumerable<Metric> AddSessionNumberToMetrics(
        IEnumerable<Metric> metrics,
        long sessionSequenceNumber)
    {
        // Add a BDSEQ metric.
        return metrics.Concat(new Metric[]
        {
            new Metric(Constants.SessionNumberMetricName, VersionBDataTypeEnum.Int64, sessionSequenceNumber)
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
            .WithTopic(SparkplugTopicGenerator.GetSparkplugStateMessageTopic(scadaHostIdentifier, this.specificationVersion))
            .WithPayload(online ? "ONLINE" : "OFFLINE").WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce).WithRetainFlag().Build();
    }

    /// <summary>
    /// Gets a STATE message with namespace version B.
    /// </summary>
    /// <param name="scadaHostIdentifier">The SCADA host identifier.</param>
    /// <param name="online">A value indicating whether the message sender is online or not.</param>
    /// <returns>A new STATE <see cref="MqttApplicationMessage"/>.</returns>
    private MqttApplicationMessage GetSparkplugStateMessageB(string scadaHostIdentifier, bool online)
    {
        var stateString = string.Empty;

        switch (this.specificationVersion)
        {
            case SparkplugSpecificationVersion.Version22:
                stateString = online ? "ONLINE" : "OFFLINE";
                break;
            case SparkplugSpecificationVersion.Version30:
                stateString = GetSparkplugStateMessage(online);
                break;
        }

        return new MqttApplicationMessageBuilder()
            .WithTopic(SparkplugTopicGenerator.GetSparkplugStateMessageTopic(scadaHostIdentifier, this.specificationVersion))
            .WithPayload(stateString)
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
            .WithRetainFlag()
            .Build();
    }

    /// <summary>
    /// Gets the STATE message payload.
    /// </summary>
    /// <param name="online">A value indicating whether the state is online or offline.</param>
    /// <returns>The STATE message as JSON string.</returns>
    private static string GetSparkplugStateMessage(bool online)
    {
        return JsonSerializer.Serialize(new StateMessage
        {
            Online = online,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
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
    private MqttApplicationMessage GetSparkplugNodeBirthA(
        SparkplugNamespace nameSpace,
        string groupIdentifier,
        string edgeNodeIdentifier,
        IEnumerable<VersionAData.KuraMetric> metrics,
        DateTimeOffset dateTime)
    {
        var payload = new VersionAData.Payload
        {
            Metrics = metrics.ToList(),
            Timestamp = dateTime.ToUnixTimeMilliseconds()
        };

        var convertedPayload = VersionA.PayloadConverter.ConvertVersionAPayload(payload);
        var serialized = PayloadHelper.Serialize(convertedPayload);

        return new MqttApplicationMessageBuilder()
            .WithTopic(
                SparkplugTopicGenerator.GetTopic(
                    nameSpace,
                    groupIdentifier,
                    SparkplugMessageType.NodeBirth,
                    edgeNodeIdentifier,
                    string.Empty)).WithPayload(serialized)
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
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
    private MqttApplicationMessage GetSparkplugNodeBirthB(
        SparkplugNamespace nameSpace,
        string groupIdentifier,
        string edgeNodeIdentifier,
        IEnumerable<Metric> metrics,
        int sequenceNumber,
        DateTimeOffset dateTime)
    {
        var payload = new Payload
        {
            Metrics = metrics.ToList(),
            Seq = (ulong)sequenceNumber,
            Timestamp = (ulong)dateTime.ToUnixTimeMilliseconds()
        };
        EnsureSparkplugBMetricTimestamps(ref payload);

        var convertedPayload = VersionB.PayloadConverter.ConvertVersionBPayload(payload);
        var serialized = PayloadHelper.Serialize(convertedPayload);

        return new MqttApplicationMessageBuilder()
            .WithTopic(
                SparkplugTopicGenerator.GetTopic(
                    nameSpace,
                    groupIdentifier,
                    SparkplugMessageType.NodeBirth,
                    edgeNodeIdentifier,
                    string.Empty))
            .WithPayload(serialized)
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtMostOnce)
            .WithRetainFlag(false)
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
    private MqttApplicationMessage GetSparkplugDeviceBirthA(
        SparkplugNamespace nameSpace,
        string groupIdentifier,
        string edgeNodeIdentifier,
        string deviceIdentifier,
        IEnumerable<VersionAData.KuraMetric> metrics,
        DateTimeOffset dateTime)
    {
        var payload = new VersionAData.Payload
        {
            Metrics = metrics.ToList(),
            Timestamp = dateTime.ToUnixTimeMilliseconds()
        };

        var convertedPayload = VersionA.PayloadConverter.ConvertVersionAPayload(payload);
        var serialized = PayloadHelper.Serialize(convertedPayload);

        return new MqttApplicationMessageBuilder()
            .WithTopic(
                SparkplugTopicGenerator.GetTopic(
                    nameSpace,
                    groupIdentifier,
                    SparkplugMessageType.DeviceBirth,
                    edgeNodeIdentifier,
                    deviceIdentifier))
            .WithPayload(serialized)
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
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
    private MqttApplicationMessage GetSparkplugDeviceBirthB(
        SparkplugNamespace nameSpace,
        string groupIdentifier,
        string edgeNodeIdentifier,
        string deviceIdentifier,
        IEnumerable<Metric> metrics,
        int sequenceNumber,
        DateTimeOffset dateTime)
    {
        var payload = new Payload
        {
            Metrics = metrics.ToList(),
            Seq = (ulong)sequenceNumber,
            Timestamp = (ulong)dateTime.ToUnixTimeMilliseconds()
        };
        EnsureSparkplugBMetricTimestamps(ref payload);

        var convertedPayload = VersionB.PayloadConverter.ConvertVersionBPayload(payload);
        var serialized = PayloadHelper.Serialize(convertedPayload);

        return new MqttApplicationMessageBuilder()
            .WithTopic(
                SparkplugTopicGenerator.GetTopic(
                    nameSpace,
                    groupIdentifier,
                    SparkplugMessageType.DeviceBirth,
                    edgeNodeIdentifier,
                    deviceIdentifier))
            .WithPayload(serialized)
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtMostOnce)
            .WithRetainFlag(false)
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
    private MqttApplicationMessage GetSparkplugNodeDeathA(
        SparkplugNamespace nameSpace,
        string groupIdentifier,
        string edgeNodeIdentifier,
        IEnumerable<VersionAData.KuraMetric> metrics)
    {
        var payload = new VersionAData.Payload
        {
            Metrics = metrics.ToList()
        };

        var convertedPayload = VersionA.PayloadConverter.ConvertVersionAPayload(payload);
        var serialized = PayloadHelper.Serialize(convertedPayload);

        return new MqttApplicationMessageBuilder()
            .WithTopic(
                SparkplugTopicGenerator.GetTopic(
                    nameSpace,
                    groupIdentifier,
                    SparkplugMessageType.NodeDeath,
                    edgeNodeIdentifier,
                    string.Empty))
            .WithPayload(serialized)
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
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
    private MqttApplicationMessage GetSparkplugNodeDeathB(
        SparkplugNamespace nameSpace,
        string groupIdentifier,
        string edgeNodeIdentifier,
        IEnumerable<Metric> metrics)
    {
        var payload = new Payload
        {
            Metrics = metrics.ToList()
        };

        var convertedPayload = VersionB.PayloadConverter.ConvertVersionBPayload(payload);
        var serialized = PayloadHelper.Serialize(convertedPayload);

        return new MqttApplicationMessageBuilder()
            .WithTopic(
                SparkplugTopicGenerator.GetTopic(
                    nameSpace,
                    groupIdentifier,
                    SparkplugMessageType.NodeDeath,
                    edgeNodeIdentifier,
                    string.Empty))
            .WithPayload(serialized)
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
            .WithRetainFlag(false)
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
    private MqttApplicationMessage GetSparkplugDeviceDeathA(
        SparkplugNamespace nameSpace,
        string groupIdentifier,
        string edgeNodeIdentifier,
        string deviceIdentifier,
        IEnumerable<VersionAData.KuraMetric> metrics,
        DateTimeOffset dateTime)
    {
        var payload = new VersionAData.Payload
        {
            Metrics = metrics.ToList(),
            Timestamp = dateTime.ToUnixTimeMilliseconds()
        };

        var convertedPayload = VersionA.PayloadConverter.ConvertVersionAPayload(payload);
        var serialized = PayloadHelper.Serialize(convertedPayload);

        return new MqttApplicationMessageBuilder()
            .WithTopic(
                SparkplugTopicGenerator.GetTopic(
                    nameSpace,
                    groupIdentifier,
                    SparkplugMessageType.DeviceDeath,
                    edgeNodeIdentifier,
                    deviceIdentifier))
            .WithPayload(serialized)
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
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
    private MqttApplicationMessage GetSparkplugDeviceDeathB(
        SparkplugNamespace nameSpace,
        string groupIdentifier,
        string edgeNodeIdentifier,
        string deviceIdentifier,
        IEnumerable<Metric> metrics,
        int sequenceNumber,
        DateTimeOffset dateTime)
    {
        var payload = new Payload
        {
            Metrics = metrics.ToList(),
            Seq = (ulong)sequenceNumber,
            Timestamp = (ulong)dateTime.ToUnixTimeMilliseconds()
        };

        var convertedPayload = VersionB.PayloadConverter.ConvertVersionBPayload(payload);
        var serialized = PayloadHelper.Serialize(convertedPayload);

        return new MqttApplicationMessageBuilder()
            .WithTopic(
                SparkplugTopicGenerator.GetTopic(
                    nameSpace,
                    groupIdentifier,
                    SparkplugMessageType.DeviceDeath,
                    edgeNodeIdentifier,
                    deviceIdentifier))
            .WithPayload(serialized)
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtMostOnce)
            .WithRetainFlag(false)
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
    private MqttApplicationMessage GetSparkplugNodeDataA(
        SparkplugNamespace nameSpace,
        string groupIdentifier,
        string edgeNodeIdentifier,
        IEnumerable<VersionAData.KuraMetric> metrics,
        DateTimeOffset dateTime)
    {
        var payload = new VersionAData.Payload
        {
            Metrics = metrics.ToList(),
            Timestamp = dateTime.ToUnixTimeMilliseconds()
        };

        var convertedPayload = VersionA.PayloadConverter.ConvertVersionAPayload(payload);
        var serialized = PayloadHelper.Serialize(convertedPayload);

        return new MqttApplicationMessageBuilder()
            .WithTopic(
                SparkplugTopicGenerator.GetTopic(
                    nameSpace,
                    groupIdentifier,
                    SparkplugMessageType.NodeData,
                    edgeNodeIdentifier,
                    string.Empty))
            .WithPayload(serialized)
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
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
    private MqttApplicationMessage GetSparkplugNodeDataB(
        SparkplugNamespace nameSpace,
        string groupIdentifier,
        string edgeNodeIdentifier,
        IEnumerable<Metric> metrics,
        int sequenceNumber,
        DateTimeOffset dateTime)
    {
        var payload = new Payload
        {
            Metrics = metrics.ToList(),
            Seq = (ulong)sequenceNumber,
            Timestamp = (ulong)dateTime.ToUnixTimeMilliseconds()
        };
        EnsureSparkplugBMetricTimestamps(ref payload);

        var convertedPayload = VersionB.PayloadConverter.ConvertVersionBPayload(payload);
        var serialized = PayloadHelper.Serialize(convertedPayload);

        return new MqttApplicationMessageBuilder()
            .WithTopic(
                SparkplugTopicGenerator.GetTopic(
                    nameSpace,
                    groupIdentifier,
                    SparkplugMessageType.NodeData,
                    edgeNodeIdentifier,
                    string.Empty))
            .WithPayload(serialized)
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtMostOnce)
            .WithRetainFlag(false)
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
    private MqttApplicationMessage GetSparkplugDeviceDataA(
        SparkplugNamespace nameSpace,
        string groupIdentifier,
        string edgeNodeIdentifier,
        string deviceIdentifier,
        IEnumerable<VersionAData.KuraMetric> metrics,
        DateTimeOffset dateTime)
    {
        var payload = new VersionAData.Payload
        {
            Metrics = metrics.ToList(),
            Timestamp = dateTime.ToUnixTimeMilliseconds()
        };

        var convertedPayload = VersionA.PayloadConverter.ConvertVersionAPayload(payload);
        var serialized = PayloadHelper.Serialize(convertedPayload);

        return new MqttApplicationMessageBuilder()
            .WithTopic(
                SparkplugTopicGenerator.GetTopic(
                    nameSpace,
                    groupIdentifier,
                    SparkplugMessageType.DeviceData,
                    edgeNodeIdentifier,
                    deviceIdentifier))
            .WithPayload(serialized)
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
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
    private MqttApplicationMessage GetSparkplugDeviceDataB(
        SparkplugNamespace nameSpace,
        string groupIdentifier,
        string edgeNodeIdentifier,
        string deviceIdentifier,
        IEnumerable<Metric> metrics,
        int sequenceNumber,
        DateTimeOffset dateTime)
    {
        var payload = new Payload
        {
            Metrics = metrics.ToList(),
            Seq = (ulong)sequenceNumber,
            Timestamp = (ulong)dateTime.ToUnixTimeMilliseconds()
        };
        EnsureSparkplugBMetricTimestamps(ref payload);

        var convertedPayload = VersionB.PayloadConverter.ConvertVersionBPayload(payload);
        var serialized = PayloadHelper.Serialize(convertedPayload);

        return new MqttApplicationMessageBuilder()
            .WithTopic(
                SparkplugTopicGenerator.GetTopic(
                    nameSpace,
                    groupIdentifier,
                    SparkplugMessageType.DeviceData,
                    edgeNodeIdentifier,
                    deviceIdentifier))
            .WithPayload(serialized)
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtMostOnce)
            .WithRetainFlag(false)
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
    private static MqttApplicationMessage GetSparkplugNodeCommandA(
        SparkplugNamespace nameSpace,
        string groupIdentifier,
        string edgeNodeIdentifier,
        IEnumerable<VersionAData.KuraMetric> metrics,
        DateTimeOffset dateTime)
    {
        var payload = new VersionAData.Payload
        {
            Metrics = metrics.ToList(),
            Timestamp = dateTime.ToUnixTimeMilliseconds()
        };

        var convertedPayload = VersionA.PayloadConverter.ConvertVersionAPayload(payload);
        var serialized = PayloadHelper.Serialize(convertedPayload);

        return new MqttApplicationMessageBuilder()
            .WithTopic(
                SparkplugTopicGenerator.GetTopic(
                    nameSpace,
                    groupIdentifier,
                    SparkplugMessageType.NodeCommand,
                    edgeNodeIdentifier,
                    string.Empty)).WithPayload(serialized)
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
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
    private MqttApplicationMessage GetSparkplugNodeCommandB(
        SparkplugNamespace nameSpace,
        string groupIdentifier,
        string edgeNodeIdentifier,
        IEnumerable<Metric> metrics,
        int sequenceNumber,
        DateTimeOffset dateTime)
    {
        var payload = new Payload
        {
            Metrics = metrics.ToList(),
            Seq = (ulong)sequenceNumber,
            Timestamp = (ulong)dateTime.ToUnixTimeMilliseconds()
        };
        EnsureSparkplugBMetricTimestamps(ref payload);
        
        var convertedPayload = VersionB.PayloadConverter.ConvertVersionBPayload(payload);
        var serialized = PayloadHelper.Serialize(convertedPayload);

        return new MqttApplicationMessageBuilder()
            .WithTopic(
                SparkplugTopicGenerator.GetTopic(
                    nameSpace,
                    groupIdentifier,
                    SparkplugMessageType.NodeCommand,
                    edgeNodeIdentifier,
                    string.Empty)).WithPayload(serialized)
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtMostOnce)
            .WithRetainFlag(false)
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
    private static MqttApplicationMessage GetSparkplugDeviceCommandA(
        SparkplugNamespace nameSpace,
        string groupIdentifier,
        string edgeNodeIdentifier,
        string deviceIdentifier,
        IEnumerable<VersionAData.KuraMetric> metrics,
        DateTimeOffset dateTime)
    {
        var payload = new VersionAData.Payload
        {
            Metrics = metrics.ToList(),
            Timestamp = dateTime.ToUnixTimeMilliseconds()
        };

        var convertedPayload = VersionA.PayloadConverter.ConvertVersionAPayload(payload);
        var serialized = PayloadHelper.Serialize(convertedPayload);

        return new MqttApplicationMessageBuilder()
            .WithTopic(
                SparkplugTopicGenerator.GetTopic(
                    nameSpace,
                    groupIdentifier,
                    SparkplugMessageType.DeviceCommand,
                    edgeNodeIdentifier,
                    deviceIdentifier)).WithPayload(serialized)
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
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
    private MqttApplicationMessage GetSparkplugDeviceCommandB(
        SparkplugNamespace nameSpace,
        string groupIdentifier,
        string edgeNodeIdentifier,
        string deviceIdentifier,
        IEnumerable<Metric> metrics,
        int sequenceNumber,
        DateTimeOffset dateTime)
    {
        var payload = new Payload
        {
            Metrics = metrics.ToList(),
            Seq = (ulong)sequenceNumber,
            Timestamp = (ulong)dateTime.ToUnixTimeMilliseconds()
        };
        EnsureSparkplugBMetricTimestamps(ref payload);

        var convertedPayload = VersionB.PayloadConverter.ConvertVersionBPayload(payload);
        var serialized = PayloadHelper.Serialize(convertedPayload);

        return new MqttApplicationMessageBuilder()
            .WithTopic(
                SparkplugTopicGenerator.GetTopic(
                    nameSpace,
                    groupIdentifier,
                    SparkplugMessageType.DeviceCommand,
                    edgeNodeIdentifier,
                    deviceIdentifier)).WithPayload(serialized)
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtMostOnce)
            .WithRetainFlag(false)
            .Build();
    }
    
    /// <summary>
    /// Ensures that all metrics will contain a Timestamp if Sparkplug protol version is Version 3.0
    /// Message timestamp will be added to all metrics that does not already contain timestamps 
    /// [tck-id-payloads-name-birth-data-requirement]
    /// </summary>
    /// <param name="payload">The payload to update</param>
    private void EnsureSparkplugBMetricTimestamps(ref Payload payload)
    {
        if (this.specificationVersion == SparkplugSpecificationVersion.Version30)
        {
            foreach (var metric in payload.Metrics)
            {
                metric.Timestamp ??= payload.Timestamp;
            }
        }
    }
}