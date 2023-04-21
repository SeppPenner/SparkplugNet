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
internal class SparkplugMessageGenerator
{
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
    /// <exception cref="ArgumentException">Thrown if the SCADA host identifier is invalid.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the namespace is out of range.</exception>
    /// <returns>A new STATE <see cref="MqttApplicationMessage"/>.</returns>
    public static MqttApplicationMessage GetSparkplugStateMessage(
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
            SparkplugNamespace.VersionA => GetSparkplugStateMessageA(scadaHostIdentifier, online),
            SparkplugNamespace.VersionB => GetSparkplugStateMessageB(scadaHostIdentifier, online),
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
    public MqttApplicationMessage GetSparkPlugNodeBirthMessage<T>(
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
                    return this.GetSparkPlugNodeBirthA(nameSpace, groupIdentifier, edgeNodeIdentifier,
                        AddSessionNumberToMetrics(newMetrics, sessionNumber), dateTime);
                }

            case SparkplugNamespace.VersionB:
                {
                    var newMetrics = metrics as IEnumerable<VersionBData.Metric> ?? new List<VersionBData.Metric>();
                    return this.GetSparkPlugNodeBirthB(nameSpace, groupIdentifier, edgeNodeIdentifier,
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
    public MqttApplicationMessage GetSparkPlugDeviceBirthMessage<T>(
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
                    return this.GetSparkPlugDeviceBirthA(nameSpace, groupIdentifier, edgeNodeIdentifier, deviceIdentifier,
                         AddSessionNumberToMetrics(newMetrics, sessionNumber), dateTime);
                }

            case SparkplugNamespace.VersionB:
                {
                    var newMetrics = metrics as IEnumerable<VersionBData.Metric> ?? new List<VersionBData.Metric>();

                    return this.GetSparkPlugDeviceBirthB(nameSpace, groupIdentifier, edgeNodeIdentifier, deviceIdentifier,
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
    public MqttApplicationMessage GetSparkPlugNodeDeathMessage(
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
                    return this.GetSparkPlugNodeDeathA(nameSpace, groupIdentifier, edgeNodeIdentifier,
                        AddSessionNumberToMetrics(metrics, sessionNumber));
                }

            case SparkplugNamespace.VersionB:
                {
                    var metrics = new List<VersionBData.Metric>();
                    return this.GetSparkPlugNodeDeathB(nameSpace, groupIdentifier, edgeNodeIdentifier,
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
                    return this.GetSparkPlugDeviceDeathA(nameSpace, groupIdentifier, edgeNodeIdentifier, deviceIdentifier,
                        AddSessionNumberToMetrics(metrics, sessionNumber), dateTime);
                }

            case SparkplugNamespace.VersionB:
                {
                    var metrics = new List<VersionBData.Metric>();
                    return this.GetSparkPlugDeviceDeathB(nameSpace, groupIdentifier, edgeNodeIdentifier, deviceIdentifier,
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
    /// <param name="addSessionNumbers">A value indicating whether to add the 'SessionNumber' metric or not.</param>
    /// <exception cref="ArgumentException">Thrown if the group identifier or the edge node identifier is invalid.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the namespace is out of range.</exception>
    /// <returns>A new NDATA <see cref="MqttApplicationMessage"/>.</returns>
    public MqttApplicationMessage GetSparkPlugNodeDataMessage<T>(
        SparkplugNamespace nameSpace,
        string groupIdentifier,
        string edgeNodeIdentifier,
        IEnumerable<T> metrics,
        int sequenceNumber,
        long sessionNumber,
        DateTimeOffset dateTime,
        bool addSessionNumbers)
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
                    return this.GetSparkPlugNodeDataA(nameSpace, groupIdentifier, edgeNodeIdentifier,
                        AddSessionNumberToMetrics(newMetrics, sessionNumber, !addSessionNumbers), dateTime);
                }

            case SparkplugNamespace.VersionB:
                {
                    var newMetrics = metrics as IEnumerable<VersionBData.Metric>
                                     ?? new List<VersionBData.Metric>();
                    return this.GetSparkPlugNodeDataB(nameSpace, groupIdentifier, edgeNodeIdentifier,
                        AddSessionNumberToMetrics(newMetrics, sessionNumber, !addSessionNumbers), sequenceNumber, dateTime);
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
    /// <param name="addSessionNumbers">A value indicating whether to add the 'SessionNumber' metric or not.</param>
    /// <exception cref="ArgumentException">Thrown if the group identifier or the edge node identifier or the device identifier is invalid.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the namespace is out of range.</exception>
    /// <returns>A new DDATA <see cref="MqttApplicationMessage"/>.</returns>
    public MqttApplicationMessage GetSparkPlugDeviceDataMessage<T>(
        SparkplugNamespace nameSpace,
        string groupIdentifier,
        string edgeNodeIdentifier,
        string deviceIdentifier,
        IEnumerable<T> metrics,
        int sequenceNumber,
        long sessionNumber,
        DateTimeOffset dateTime,
        bool addSessionNumbers)
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
                    return this.GetSparkPlugDeviceDataA(nameSpace, groupIdentifier, edgeNodeIdentifier, deviceIdentifier,
                        AddSessionNumberToMetrics(newMetrics, sessionNumber, !addSessionNumbers), dateTime);
                }

            case SparkplugNamespace.VersionB:
                {
                    var newMetrics = metrics as IEnumerable<VersionBData.Metric>
                                     ?? new List<VersionBData.Metric>();
                    return this.GetSparkPlugDeviceDataB(nameSpace, groupIdentifier, edgeNodeIdentifier, deviceIdentifier,
                         AddSessionNumberToMetrics(newMetrics, sessionNumber, !addSessionNumbers), sequenceNumber, dateTime);
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
    /// <param name="addSessionNumbers">A value indicating whether to add the 'SessionNumber' metric or not.</param>
    /// <exception cref="ArgumentException">Thrown if the group identifier or the edge node identifier is invalid.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the namespace is out of range.</exception>
    /// <returns>A new NCMD <see cref="MqttApplicationMessage"/>.</returns>
    public static MqttApplicationMessage GetSparkPlugNodeCommandMessage<T>(
        SparkplugNamespace nameSpace,
        string groupIdentifier,
        string edgeNodeIdentifier,
        IEnumerable<T> metrics,
        int sequenceNumber,
        long sessionNumber,
        DateTimeOffset dateTime,
        bool addSessionNumbers)
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
                    return GetSparkPlugNodeCommandA(nameSpace, groupIdentifier, edgeNodeIdentifier,
                        AddSessionNumberToMetrics(newMetrics, sessionNumber, !addSessionNumbers), dateTime);
                }

            case SparkplugNamespace.VersionB:
                {
                    var newMetrics = metrics as IEnumerable<VersionBData.Metric>
                                     ?? new List<VersionBData.Metric>();

                    return GetSparkPlugNodeCommandB(nameSpace, groupIdentifier, edgeNodeIdentifier,
                         AddSessionNumberToMetrics(newMetrics, sessionNumber, !addSessionNumbers), sequenceNumber, dateTime);
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
    /// <param name="addSessionNumbers">A value indicating whether to add the 'SessionNumber' metric or not.</param>
    /// <exception cref="ArgumentException">Thrown if the group identifier or the edge node identifier or the device identifier is invalid.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the namespace is out of range.</exception>
    /// <returns>A new DCMD <see cref="MqttApplicationMessage"/>.</returns>
    public static MqttApplicationMessage GetSparkPlugDeviceCommandMessage<T>(
        SparkplugNamespace nameSpace,
        string groupIdentifier,
        string edgeNodeIdentifier,
        string deviceIdentifier,
        IEnumerable<T> metrics,
        int sequenceNumber,
        long sessionNumber,
        DateTimeOffset dateTime, bool addSessionNumbers)
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
                    newMetrics = AddSessionNumberToMetrics(newMetrics, sessionNumber, !addSessionNumbers);

                    return GetSparkPlugDeviceCommandA(nameSpace, groupIdentifier, edgeNodeIdentifier, deviceIdentifier, newMetrics, dateTime);
                }

            case SparkplugNamespace.VersionB:
                {
                    var newMetrics = metrics as IEnumerable<VersionBData.Metric>
                                     ?? new List<VersionBData.Metric>();

                    newMetrics = AddSessionNumberToMetrics(newMetrics, sessionNumber, !addSessionNumbers);

                    return GetSparkPlugDeviceCommandB(nameSpace, groupIdentifier, edgeNodeIdentifier, deviceIdentifier, newMetrics, sequenceNumber, dateTime);
                }

            default:
                throw new ArgumentOutOfRangeException(nameof(nameSpace));
        }
    }

    /// <summary>Adds the session number to the version A metrics.</summary>
    /// <param name="metrics">The metrics.</param>
    /// <param name="sessionSequenceNumber">The session sequence number.</param>
    /// <param name="skipSessionSequenceNumber">A value indicating whether the session sequence number will be skipped or not.</param>
    /// <returns>The metrics.</returns>
    private static IEnumerable<VersionAData.KuraMetric> AddSessionNumberToMetrics(
        IEnumerable<VersionAData.KuraMetric> metrics,
        long sessionSequenceNumber,
        bool skipSessionSequenceNumber = false)
    {
        // The session sequence number in the message is disabled.
        if (skipSessionSequenceNumber) 
        {
            return metrics;
        }
        else
        {
            // Add a BDSEQ metric.
            return metrics.Concat(new VersionAData.KuraMetric[]
            {
                new VersionAData.KuraMetric
                {
                    Name = Constants.SessionNumberMetricName,
                    LongValue = sessionSequenceNumber,
                    DataType = VersionAData.DataType.Int64
                }
            });
        }
    }

    /// <summary>Adds the session number to the version B metrics.</summary>
    /// <param name="metrics">The metrics.</param>
    /// <param name="sessionSequenceNumber">The session sequence number.</param>
    /// <param name="skipSessionSequenceNumber">A value indicating whether the session sequence number will be skipped or not.</param>
    /// <returns>The metrics.</returns>
    private static IEnumerable<VersionBData.Metric> AddSessionNumberToMetrics(
        IEnumerable<VersionBData.Metric> metrics,
        long sessionSequenceNumber,
        bool skipSessionSequenceNumber = false)
    {
        // The session sequence number in the message is disabled.
        if (skipSessionSequenceNumber)
        {
            return metrics;
        }
        else
        {
            // Add a BDSEQ metric.
            return metrics.Concat(new VersionBData.Metric[]
            {
                new VersionBData.Metric
                {
                    Name = Constants.SessionNumberMetricName,
                    ULongValue = (ulong)sessionSequenceNumber,
                    ValueCase = (uint)VersionBData.DataType.UInt64
                }
            });
        }
    }

    /// <summary>
    /// Gets a STATE message with namespace version A.
    /// </summary>
    /// <param name="scadaHostIdentifier">The SCADA host identifier.</param>
    /// <param name="online">A value indicating whether the message sender is online or not.</param>
    /// <returns>A new STATE <see cref="MqttApplicationMessage"/>.</returns>
    private static MqttApplicationMessage GetSparkplugStateMessageA(string scadaHostIdentifier, bool online)
    {
        return new MqttApplicationMessageBuilder()
            .WithTopic(SparkplugTopicGenerator.GetSparkplugStateMessageTopic(scadaHostIdentifier))
            .WithPayload(online ? "ONLINE" : "OFFLINE").WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce).WithRetainFlag().Build();
    }

    /// <summary>
    /// Gets a STATE message with namespace version B.
    /// </summary>
    /// <param name="scadaHostIdentifier">The SCADA host identifier.</param>
    /// <param name="online">A value indicating whether the message sender is online or not.</param>
    /// <returns>A new STATE <see cref="MqttApplicationMessage"/>.</returns>
    private static MqttApplicationMessage GetSparkplugStateMessageB(string scadaHostIdentifier, bool online)
    {
        return new MqttApplicationMessageBuilder()
            .WithTopic(SparkplugTopicGenerator.GetSparkplugStateMessageTopic(scadaHostIdentifier))
            .WithPayload(online ? "ONLINE" : "OFFLINE").WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce).WithRetainFlag().Build();
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
        IEnumerable<VersionAData.KuraMetric> metrics,
        DateTimeOffset dateTime)
    {
        var payload = new VersionAData.Payload
        {
            Metrics = metrics.ToList(),
            Timestamp = dateTime.ToUnixTimeMilliseconds()
        };

        // Debug output.
        this.logger?.Debug("NBIRTH: VersionADataPayload: {@Payload}", payload);

        var convertedPayload = PayloadConverter.ConvertVersionAPayload(payload);
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
    private MqttApplicationMessage GetSparkPlugNodeBirthB(
        SparkplugNamespace nameSpace,
        string groupIdentifier,
        string edgeNodeIdentifier,
        IEnumerable<VersionBData.Metric> metrics,
        int sequenceNumber,
        DateTimeOffset dateTime)
    {
        var payload = new VersionBData.Payload
        {
            Metrics = metrics.ToList(),
            Seq = (ulong)sequenceNumber,
            Timestamp = (ulong)dateTime.ToUnixTimeMilliseconds()
        };

        // Debug output.
        this.logger?.Debug("NBIRTH: VersionBDataPayload: {@Payload}", payload);

        var convertedPayload = PayloadConverter.ConvertVersionBPayload(payload);
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
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
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
        IEnumerable<VersionAData.KuraMetric> metrics,
        DateTimeOffset dateTime)
    {
        var payload = new VersionAData.Payload
        {
            Metrics = metrics.ToList(),
            Timestamp = dateTime.ToUnixTimeMilliseconds()
        };

        // Debug output.
        this.logger?.Debug("DBIRTH: VersionADataPayload: {@Payload}", payload);

        var convertedPayload = PayloadConverter.ConvertVersionAPayload(payload);
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
    private MqttApplicationMessage GetSparkPlugDeviceBirthB(
        SparkplugNamespace nameSpace,
        string groupIdentifier,
        string edgeNodeIdentifier,
        string deviceIdentifier,
        IEnumerable<VersionBData.Metric> metrics,
        int sequenceNumber,
        DateTimeOffset dateTime)
    {
        var payload = new VersionBData.Payload
        {
            Metrics = metrics.ToList(),
            Seq = (ulong)sequenceNumber,
            Timestamp = (ulong)dateTime.ToUnixTimeMilliseconds()
        };

        // Debug output.
        this.logger?.Debug("DBIRTH: VersionBDataPayload: {@Payload}", payload);

        var convertedPayload = PayloadConverter.ConvertVersionBPayload(payload);
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
        IEnumerable<VersionAData.KuraMetric> metrics)
    {
        var payload = new VersionAData.Payload
        {
            Metrics = metrics.ToList()
        };

        // Debug output.
        this.logger?.Debug("NDEATH: VersionADataPayload: {@Payload}", payload);

        var convertedPayload = PayloadConverter.ConvertVersionAPayload(payload);
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
    private MqttApplicationMessage GetSparkPlugNodeDeathB(
        SparkplugNamespace nameSpace,
        string groupIdentifier,
        string edgeNodeIdentifier,
        IEnumerable<VersionBData.Metric> metrics)
    {
        var payload = new VersionBData.Payload
        {
            Metrics = metrics.ToList()
        };

        // Debug output.
        this.logger?.Debug("NDEATH: VersionBDataPayload: {@Payload}", payload);

        var convertedPayload = PayloadConverter.ConvertVersionBPayload(payload);
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
        IEnumerable<VersionAData.KuraMetric> metrics,
        DateTimeOffset dateTime)
    {
        var payload = new VersionAData.Payload
        {
            Metrics = metrics.ToList(),
            Timestamp = dateTime.ToUnixTimeMilliseconds()
        };

        // Debug output.
        this.logger?.Debug("DDEATH: VersionADataPayload: {@Payload}", payload);

        var convertedPayload = PayloadConverter.ConvertVersionAPayload(payload);
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
    private MqttApplicationMessage GetSparkPlugDeviceDeathB(
        SparkplugNamespace nameSpace,
        string groupIdentifier,
        string edgeNodeIdentifier,
        string deviceIdentifier,
        IEnumerable<VersionBData.Metric> metrics,
        int sequenceNumber,
        DateTimeOffset dateTime)
    {
        var payload = new VersionBData.Payload
        {
            Metrics = metrics.ToList(),
            Seq = (ulong)sequenceNumber,
            Timestamp = (ulong)dateTime.ToUnixTimeMilliseconds()
        };

        // Debug output.
        this.logger?.Debug("DDEATH: VersionBDataPayload: {@Payload}", payload);

        var convertedPayload = PayloadConverter.ConvertVersionBPayload(payload);
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
        IEnumerable<VersionAData.KuraMetric> metrics,
        DateTimeOffset dateTime)
    {
        var payload = new VersionAData.Payload
        {
            Metrics = metrics.ToList(),
            Timestamp = dateTime.ToUnixTimeMilliseconds()
        };

        // Debug output.
        this.logger?.Debug("NDATA: VersionADataPayload: {@Payload}", payload);

        var convertedPayload = PayloadConverter.ConvertVersionAPayload(payload);
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
    private MqttApplicationMessage GetSparkPlugNodeDataB(
        SparkplugNamespace nameSpace,
        string groupIdentifier,
        string edgeNodeIdentifier,
        IEnumerable<VersionBData.Metric> metrics,
        int sequenceNumber,
        DateTimeOffset dateTime)
    {
        var payload = new VersionBData.Payload
        {
            Metrics = metrics.ToList(),
            Seq = (ulong)sequenceNumber,
            Timestamp = (ulong)dateTime.ToUnixTimeMilliseconds()
        };

        // Debug output.
        this.logger?.Debug("NDATA: VersionBDataPayload: {@Payload}", payload);

        var convertedPayload = PayloadConverter.ConvertVersionBPayload(payload);
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
        IEnumerable<VersionAData.KuraMetric> metrics,
        DateTimeOffset dateTime)
    {
        var payload = new VersionAData.Payload
        {
            Metrics = metrics.ToList(),
            Timestamp = dateTime.ToUnixTimeMilliseconds()
        };

        // Debug output.
        this.logger?.Debug("DDATA: VersionADataPayload: {@Payload}", payload);

        var convertedPayload = PayloadConverter.ConvertVersionAPayload(payload);
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
    private MqttApplicationMessage GetSparkPlugDeviceDataB(
        SparkplugNamespace nameSpace,
        string groupIdentifier,
        string edgeNodeIdentifier,
        string deviceIdentifier,
        IEnumerable<VersionBData.Metric> metrics,
        int sequenceNumber,
        DateTimeOffset dateTime)
    {
        var payload = new VersionBData.Payload
        {
            Metrics = metrics.ToList(),
            Seq = (ulong)sequenceNumber,
            Timestamp = (ulong)dateTime.ToUnixTimeMilliseconds()
        };

        // Debug output.
        this.logger?.Debug("DDATA: VersionBDataPayload: {@Payload}", payload);

        var convertedPayload = PayloadConverter.ConvertVersionBPayload(payload);
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
    /// Gets a NCMD message with namespace version A.
    /// </summary>
    /// <param name="nameSpace">The namespace.</param>
    /// <param name="groupIdentifier">The group identifier.</param>
    /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
    /// <param name="metrics">The metrics.</param>
    /// <param name="dateTime">The date time.</param>
    /// <returns>A new NCMD <see cref="MqttApplicationMessage"/>.</returns>
    private static MqttApplicationMessage GetSparkPlugNodeCommandA(
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

        var convertedPayload = PayloadConverter.ConvertVersionAPayload(payload);
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
    private static MqttApplicationMessage GetSparkPlugNodeCommandB(
        SparkplugNamespace nameSpace,
        string groupIdentifier,
        string edgeNodeIdentifier,
        IEnumerable<VersionBData.Metric> metrics,
        int sequenceNumber,
        DateTimeOffset dateTime)
    {
        var payload = new VersionBData.Payload
        {
            Metrics = metrics.ToList(),
            Seq = (ulong)sequenceNumber,
            Timestamp = (ulong)dateTime.ToUnixTimeMilliseconds()
        };

        var convertedPayload = PayloadConverter.ConvertVersionBPayload(payload);
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
    /// Gets a DCMD message with namespace version A.
    /// </summary>
    /// <param name="nameSpace">The namespace.</param>
    /// <param name="groupIdentifier">The group identifier.</param>
    /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
    /// <param name="deviceIdentifier">The device identifier.</param>
    /// <param name="metrics">The metrics.</param>
    /// <param name="dateTime">The date time.</param>
    /// <returns>A new DCMD <see cref="MqttApplicationMessage"/>.</returns>
    private static MqttApplicationMessage GetSparkPlugDeviceCommandA(
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

        var convertedPayload = PayloadConverter.ConvertVersionAPayload(payload);
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
    private static MqttApplicationMessage GetSparkPlugDeviceCommandB(
        SparkplugNamespace nameSpace,
        string groupIdentifier,
        string edgeNodeIdentifier,
        string deviceIdentifier,
        IEnumerable<VersionBData.Metric> metrics,
        int sequenceNumber,
        DateTimeOffset dateTime)
    {
        var payload = new VersionBData.Payload
        {
            Metrics = metrics.ToList(),
            Seq = (ulong)sequenceNumber,
            Timestamp = (ulong)dateTime.ToUnixTimeMilliseconds()
        };

        var convertedPayload = PayloadConverter.ConvertVersionBPayload(payload);
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
}
