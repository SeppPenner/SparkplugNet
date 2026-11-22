// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugMessageGeneratorTestVersion30.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class to test the <see cref="SparkplugMessageGenerator"/> class with specification version 3.0.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Tests.Messages;

/// <summary>
/// A class to test the <see cref="SparkplugMessageGenerator"/> class with specification version 3.0.
/// </summary>
[TestClass]
public sealed class SparkplugMessageGeneratorTestVersion30
{
    /// <summary>
    /// The metrics for namespace B.
    /// </summary>
    private readonly List<VersionBData.Metric> metricsB =
    [
        new VersionBData.Metric("Test", VersionBData.DataType.Int32, 20)
    ];

    /// <summary>
    /// The SEQ metric for namespace B.
    /// </summary>
    private readonly VersionBData.Metric seqMetricB = new(Constants.SessionNumberMetricName, VersionBData.DataType.Int64, 1);

    /// <summary>
    /// The message generator.
    /// </summary>
    private readonly SparkplugMessageGenerator messageGenerator = new(SparkplugSpecificationVersion.Version30);

    /// <summary>
    /// Tests chronological ordering of live samples without moving historical metrics.
    /// </summary>
    [TestMethod]
    public void TestLiveMetricsArePublishedChronologically()
    {
        var publishedAt = DateTimeOffset.UtcNow;
        var olderTimestamp = (ulong)publishedAt.AddMilliseconds(-100).ToUnixTimeMilliseconds();
        var metrics = new VersionBData.Metric[]
        {
            new("Newest", VersionBData.DataType.Int32, 20) { IsHistorical = false },
            new("Historical", VersionBData.DataType.Int32, 10) { IsHistorical = true, Timestamp = olderTimestamp - 1 },
            new("Older", VersionBData.DataType.Int32, 15) { IsHistorical = false, Timestamp = olderTimestamp }
        };

        var message = this.messageGenerator.GetSparkplugNodeDataMessage(SparkplugNamespace.VersionB, "group", "edge", metrics, 1, 0, publishedAt);
        var payload = PayloadHelper.Deserialize<VersionBProtoBufPayload>(message.Payload.ToArray());

        Assert.IsNotNull(payload);
        CollectionAssert.AreEqual(new[] { "Older", "Historical", "Newest", Constants.SessionNumberMetricName },
            payload.Metrics.Select(metric => metric.Name).ToArray());
    }

    /// <summary>
    /// Tests the Sparkplug message generator with a message with a version B namespace and a online state.
    /// </summary>
    [TestMethod]
    public void TestStateMessageNamespaceBOnline()
    {
        var timestampBefore = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var message = this.messageGenerator.GetSparkplugStateMessage(SparkplugNamespace.VersionB, "scada1", true);
        var payloadVersionB = JsonSerializer.Deserialize<StateMessage>(message.Payload.ToArray());

        Assert.AreEqual("spBv1.0/STATE/scada1", message.Topic);
        Assert.AreEqual(message.Topic, this.messageGenerator.GetSparkplugStateTopic("scada1"));
        Assert.IsNotNull(payloadVersionB);
        Assert.AreEqual(true, payloadVersionB.Online);
        Assert.IsTrue(payloadVersionB.Timestamp >= timestampBefore);
    }

    /// <summary>
    /// Tests the Sparkplug message generator with a message with a version B namespace and a offline state.
    /// </summary>
    [TestMethod]
    public void TestStateMessageNamespaceBOffline()
    {
        var timestampBefore = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var message = this.messageGenerator.GetSparkplugStateMessage(SparkplugNamespace.VersionB, "scada1", false);
        var payloadVersionB = JsonSerializer.Deserialize<StateMessage>(message.Payload.ToArray());

        Assert.AreEqual("spBv1.0/STATE/scada1", message.Topic);
        Assert.IsNotNull(payloadVersionB);
        Assert.AreEqual(false, payloadVersionB.Online);
        Assert.IsTrue(payloadVersionB.Timestamp >= timestampBefore);
    }

    /// <summary>
    /// Tests sending the same explicit timestamp in the Will and the online STATE.
    /// </summary>
    /// <param name="online">Whether the STATE is online.</param>
    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void TestStateMessageNamespaceBWithConnectionTimestamp(bool online)
    {
        const long timestamp = 1_700_000_000_000;
        var message = this.messageGenerator.GetSparkplugStateMessage(SparkplugNamespace.VersionB, "scada1", online, timestamp);
        var state = JsonSerializer.Deserialize<StateMessage>(message.Payload.ToArray());

        Assert.IsNotNull(state);
        Assert.AreEqual("spBv1.0/STATE/scada1", message.Topic);
        Assert.AreEqual(online, state.Online);
        Assert.AreEqual(timestamp, state.Timestamp);
        Assert.AreEqual(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce, message.QualityOfServiceLevel);
        Assert.IsTrue(message.Retain);
    }

    /// <summary>
    /// Tests the Sparkplug message generator with a device birth message with a version B namespace.
    /// </summary>
    [TestMethod]
    public void TestDeviceBirthMessageNamespaceB()
    {
        var dateTime = DateTimeOffset.UtcNow;
        var message = this.messageGenerator.GetSparkplugDeviceBirthMessage(SparkplugNamespace.VersionB, "group1", "edge1", "device1", this.metricsB, 0, 1, dateTime);
        var payloadVersionB = PayloadHelper.Deserialize<VersionBProtoBufPayload>(message.Payload.ToArray());

        Assert.AreEqual("spBv1.0/group1/DBIRTH/edge1/device1", message.Topic);
        Assert.IsNotNull(payloadVersionB);
        Assert.AreEqual((ulong)dateTime.ToUnixTimeMilliseconds(), payloadVersionB.Timestamp);
        Assert.AreEqual(2, payloadVersionB.Metrics.Count);
        Assert.IsTrue(payloadVersionB.Metrics.All(metric => metric.Timestamp == payloadVersionB.Timestamp));

        Assert.AreEqual(this.metricsB.First().Name, payloadVersionB.Metrics.ElementAt(0).Name);
        Assert.AreEqual(Convert.ToUInt32(this.metricsB.First().Value), payloadVersionB.Metrics.ElementAt(0).IntValue);
        Assert.AreEqual((uint?)this.metricsB.First().DataType, payloadVersionB.Metrics.ElementAt(0).DataType);

        Assert.AreEqual(this.seqMetricB.Name, payloadVersionB.Metrics.ElementAt(1).Name);
        Assert.AreEqual(Convert.ToUInt64(this.seqMetricB.Value), payloadVersionB.Metrics.ElementAt(1).LongValue);
        Assert.AreEqual((uint?)this.seqMetricB.DataType, payloadVersionB.Metrics.ElementAt(1).DataType);
    }

    /// <summary>
    /// Tests the Sparkplug message generator with a node birth message with a version B namespace.
    /// </summary>
    [TestMethod]
    public void TestNodeBirthMessageNamespaceB()
    {
        var dateTime = DateTimeOffset.UtcNow;
        var message = this.messageGenerator.GetSparkplugNodeBirthMessage(SparkplugNamespace.VersionB, "group1", "edge1", this.metricsB, 0, 1, dateTime);
        var payloadVersionB = PayloadHelper.Deserialize<VersionBProtoBufPayload>(message.Payload.ToArray());

        Assert.AreEqual("spBv1.0/group1/NBIRTH/edge1", message.Topic);
        Assert.IsNotNull(payloadVersionB);
        Assert.AreEqual((ulong)dateTime.ToUnixTimeMilliseconds(), payloadVersionB.Timestamp);
        Assert.AreEqual(2, payloadVersionB.Metrics.Count);
        Assert.IsTrue(payloadVersionB.Metrics.All(metric => metric.Timestamp == payloadVersionB.Timestamp));

        Assert.AreEqual(this.metricsB.First().Name, payloadVersionB.Metrics.ElementAt(0).Name);
        Assert.AreEqual(Convert.ToUInt32(this.metricsB.First().Value), payloadVersionB.Metrics.ElementAt(0).IntValue);
        Assert.AreEqual((uint?)this.metricsB.First().DataType, payloadVersionB.Metrics.ElementAt(0).DataType);

        Assert.AreEqual(this.seqMetricB.Name, payloadVersionB.Metrics.ElementAt(1).Name);
        Assert.AreEqual(Convert.ToUInt64(this.seqMetricB.Value), payloadVersionB.Metrics.ElementAt(1).LongValue);
        Assert.AreEqual((uint?)this.seqMetricB.DataType, payloadVersionB.Metrics.ElementAt(1).DataType);
    }

    /// <summary>
    /// Tests that NBIRTH includes template references for instances and omits them for definitions.
    /// </summary>
    [TestMethod]
    [DataRow(true, "", null)]
    [DataRow(true, "TemplateType", null)]
    [DataRow(false, "TemplateType", "TemplateType")]
    public void TestNodeBirthTemplateReference(bool isDefinition, string templateRef, string? expectedTemplateRef)
    {
        var template = new VersionBData.Template { IsDefinition = isDefinition, TemplateRef = templateRef };
        var metricName = isDefinition ? "_types_/TemplateType" : "TemplateInstance";
        var metric = new VersionBData.Metric(metricName, VersionBData.DataType.Template, template);

        var message = this.messageGenerator.GetSparkplugNodeBirthMessage(
            SparkplugNamespace.VersionB, "group1", "edge1", new[] { metric }, 0, 1, DateTimeOffset.UtcNow);
        var payload = PayloadHelper.Deserialize<VersionBProtoBufPayload>(message.Payload.ToArray());

        Assert.IsNotNull(payload);
        var protoTemplate = payload.Metrics.Single(item => item.Name == metricName).TemplateValue;
        Assert.IsNotNull(protoTemplate);
        Assert.AreEqual(isDefinition, protoTemplate.IsDefinition);
        Assert.AreEqual(expectedTemplateRef, protoTemplate.TemplateRef);
    }

    /// <summary>
    /// Tests the Sparkplug message generator with a device death message with a version B namespace.
    /// </summary>
    [TestMethod]
    public void TestDeviceDeathMessageNamespaceB()
    {
        var dateTime = DateTimeOffset.UtcNow;
        var message = this.messageGenerator.GetSparkplugDeviceDeathMessage(SparkplugNamespace.VersionB, "group1", "edge1", "device1", 0, 1, dateTime);
        var payloadVersionB = PayloadHelper.Deserialize<VersionBProtoBufPayload>(message.Payload.ToArray());

        Assert.AreEqual("spBv1.0/group1/DDEATH/edge1/device1", message.Topic);
        Assert.IsNotNull(payloadVersionB);
        Assert.AreEqual((ulong)dateTime.ToUnixTimeMilliseconds(), payloadVersionB.Timestamp);
        Assert.AreEqual(1, payloadVersionB.Metrics.Count);

        Assert.AreEqual(this.seqMetricB.Name, payloadVersionB.Metrics.ElementAt(0).Name);
        Assert.AreEqual(Convert.ToUInt64(this.seqMetricB.Value), payloadVersionB.Metrics.ElementAt(0).LongValue);
        Assert.AreEqual((uint?)this.seqMetricB.DataType, payloadVersionB.Metrics.ElementAt(0).DataType);
    }

    /// <summary>
    /// Tests the Sparkplug message generator with a node death message with a version B namespace.
    /// </summary>
    [TestMethod]
    public void TestNodeDeathMessageNamespaceB()
    {
        var message = this.messageGenerator.GetSparkplugNodeDeathMessage(SparkplugNamespace.VersionB, "group1", "edge1", 1);
        var payloadVersionB = PayloadHelper.Deserialize<VersionBProtoBufPayload>(message.Payload.ToArray());

        Assert.AreEqual("spBv1.0/group1/NDEATH/edge1", message.Topic);
        Assert.IsNotNull(payloadVersionB);
        Assert.AreEqual(1, payloadVersionB.Metrics.Count);

        Assert.AreEqual(this.seqMetricB.Name, payloadVersionB.Metrics.ElementAt(0).Name);
        Assert.AreEqual(Convert.ToUInt64(this.seqMetricB.Value), payloadVersionB.Metrics.ElementAt(0).LongValue);
        Assert.AreEqual((uint?)this.seqMetricB.DataType, payloadVersionB.Metrics.ElementAt(0).DataType);
    }

    /// <summary>
    /// Tests the Sparkplug message generator with a device data message with a version B namespace.
    /// </summary>
    [TestMethod]
    public void TestDeviceDataMessageNamespaceB()
    {
        var dateTime = DateTimeOffset.FromUnixTimeMilliseconds(2_000);
        var message = this.messageGenerator.GetSparkplugDeviceDataMessage(SparkplugNamespace.VersionB, "group1", "edge1", "device1", this.metricsB, 2, 1, dateTime);
        var payloadVersionB = PayloadHelper.Deserialize<VersionBProtoBufPayload>(message.Payload.ToArray());

        Assert.AreEqual("spBv1.0/group1/DDATA/edge1/device1", message.Topic);
        Assert.IsNotNull(payloadVersionB);
        Assert.AreEqual((ulong)dateTime.ToUnixTimeMilliseconds(), payloadVersionB.Timestamp);
        Assert.AreEqual(2UL, payloadVersionB.Seq);
        Assert.AreEqual(1, payloadVersionB.Metrics.Count);
        Assert.IsNull(payloadVersionB.Metrics[0].DataType);

        Assert.AreEqual(this.metricsB.First().Name, payloadVersionB.Metrics.ElementAt(0).Name);
        Assert.AreEqual(Convert.ToUInt32(this.metricsB.First().Value), payloadVersionB.Metrics.ElementAt(0).IntValue);
        Assert.IsNull(payloadVersionB.Metrics.ElementAt(0).Alias);
        Assert.AreEqual(payloadVersionB.Timestamp, payloadVersionB.Metrics.ElementAt(0).Timestamp);
        Assert.IsNull(this.metricsB.First().Timestamp);
    }

    /// <summary>
    /// Tests that device data preserves explicitly supplied metric timestamps.
    /// </summary>
    [TestMethod]
    public void TestDeviceDataPreservesMetricTimestamps()
    {
        var dateTime = DateTimeOffset.FromUnixTimeMilliseconds(2_000);
        VersionBData.Metric[] metrics =
        [
            new("Recorded", VersionBData.DataType.Int32, 20) { Timestamp = 1_000 },
            new("Epoch", VersionBData.DataType.Int32, 21) { Timestamp = 0 }
        ];

        var message = this.messageGenerator.GetSparkplugDeviceDataMessage(
            SparkplugNamespace.VersionB, "group1", "edge1", "device1", metrics, 2, 1, dateTime);
        var payload = PayloadHelper.Deserialize<VersionBProtoBufPayload>(message.Payload.ToArray());

        Assert.IsNotNull(payload);
        Assert.AreEqual(1_000UL, payload.Metrics[0].Timestamp);
        Assert.AreEqual(0UL, payload.Metrics[1].Timestamp);
    }

    /// <summary>
    /// Tests the Sparkplug message generator with a node data message with a version B namespace.
    /// </summary>
    [TestMethod]
    public void TestNodeDataMessageNamespaceB()
    {
        var dateTime = DateTimeOffset.UtcNow;
        var message = this.messageGenerator.GetSparkplugNodeDataMessage(SparkplugNamespace.VersionB, "group1", "edge1", this.metricsB, 0, 1, dateTime);
        var payloadVersionB = PayloadHelper.Deserialize<VersionBProtoBufPayload>(message.Payload.ToArray());

        Assert.AreEqual("spBv1.0/group1/NDATA/edge1", message.Topic);
        Assert.IsNotNull(payloadVersionB);
        Assert.AreEqual((ulong)dateTime.ToUnixTimeMilliseconds(), payloadVersionB.Timestamp);
        Assert.AreEqual(2, payloadVersionB.Metrics.Count);
        Assert.IsTrue(payloadVersionB.Metrics.All(metric => metric.Timestamp == payloadVersionB.Timestamp));
        Assert.IsTrue(payloadVersionB.Metrics.All(metric => metric.DataType is null));

        Assert.AreEqual(this.metricsB.First().Name, payloadVersionB.Metrics.ElementAt(0).Name);
        Assert.AreEqual(Convert.ToUInt32(this.metricsB.First().Value), payloadVersionB.Metrics.ElementAt(0).IntValue);

        Assert.AreEqual(this.seqMetricB.Name, payloadVersionB.Metrics.ElementAt(1).Name);
        Assert.AreEqual(Convert.ToUInt64(this.seqMetricB.Value), payloadVersionB.Metrics.ElementAt(1).LongValue);
    }

    /// <summary>
    /// Tests the Sparkplug message generator with a device command message with a version B namespace.
    /// </summary>
    /// <param name="includeSessionNumber">Whether to include the session number metric.</param>
    /// <param name="includeDataTypes">Whether to include command data types.</param>
    [TestMethod]
    [DataRow(true, false)]
    [DataRow(false, false)]
    [DataRow(false, true)]
    public void TestDeviceCommandMessageNamespaceB(bool includeSessionNumber, bool includeDataTypes)
    {
        var dateTime = DateTimeOffset.UtcNow;
        var message = this.messageGenerator.GetSparkplugDeviceCommandMessage(SparkplugNamespace.VersionB, "group1", "edge1", "device1", this.metricsB, 7, includeSessionNumber ? 1L : null, dateTime, includeDataTypes);
        var payloadVersionB = PayloadHelper.Deserialize<VersionBProtoBufPayload>(message.Payload.ToArray());

        Assert.AreEqual("spBv1.0/group1/DCMD/edge1/device1", message.Topic);
        Assert.IsNotNull(payloadVersionB);
        Assert.IsNull(payloadVersionB.Seq);
        Assert.AreEqual((ulong)dateTime.ToUnixTimeMilliseconds(), payloadVersionB.Timestamp);
        Assert.AreEqual(includeSessionNumber ? 2 : 1, payloadVersionB.Metrics.Count);
        Assert.IsTrue(payloadVersionB.Metrics.All(metric => metric.Timestamp is null));
        Assert.IsTrue(payloadVersionB.Metrics.All(metric => metric.DataType.HasValue == includeDataTypes));

        Assert.AreEqual(this.metricsB.First().Name, payloadVersionB.Metrics.ElementAt(0).Name);
        Assert.AreEqual(Convert.ToUInt32(this.metricsB.First().Value), payloadVersionB.Metrics.ElementAt(0).IntValue);

        if (includeSessionNumber)
        {
            Assert.AreEqual(this.seqMetricB.Name, payloadVersionB.Metrics.ElementAt(1).Name);
            Assert.AreEqual(Convert.ToUInt64(this.seqMetricB.Value), payloadVersionB.Metrics.ElementAt(1).LongValue);
        }
    }

    /// <summary>
    /// Tests the Sparkplug message generator with a node command message with a version B namespace.
    /// </summary>
    /// <param name="includeSessionNumber">Whether to include the session number metric.</param>
    /// <param name="includeDataTypes">Whether to include command data types.</param>
    [TestMethod]
    [DataRow(true, false)]
    [DataRow(false, false)]
    [DataRow(false, true)]
    public void TestNodeCommandMessageNamespaceB(bool includeSessionNumber, bool includeDataTypes)
    {
        var dateTime = DateTimeOffset.UtcNow;
        var message = this.messageGenerator.GetSparkplugNodeCommandMessage(SparkplugNamespace.VersionB, "group1", "edge1", this.metricsB, 7, includeSessionNumber ? 1L : null, dateTime, includeDataTypes);
        var payloadVersionB = PayloadHelper.Deserialize<VersionBProtoBufPayload>(message.Payload.ToArray());

        Assert.AreEqual("spBv1.0/group1/NCMD/edge1", message.Topic);
        Assert.IsNotNull(payloadVersionB);
        Assert.IsNull(payloadVersionB.Seq);
        Assert.AreEqual((ulong)dateTime.ToUnixTimeMilliseconds(), payloadVersionB.Timestamp);
        Assert.AreEqual(includeSessionNumber ? 2 : 1, payloadVersionB.Metrics.Count);
        Assert.IsTrue(payloadVersionB.Metrics.All(metric => metric.Timestamp is null));
        Assert.IsTrue(payloadVersionB.Metrics.All(metric => metric.DataType.HasValue == includeDataTypes));

        Assert.AreEqual(this.metricsB.First().Name, payloadVersionB.Metrics.ElementAt(0).Name);
        Assert.AreEqual(Convert.ToUInt32(this.metricsB.First().Value), payloadVersionB.Metrics.ElementAt(0).IntValue);

        if (includeSessionNumber)
        {
            Assert.AreEqual(this.seqMetricB.Name, payloadVersionB.Metrics.ElementAt(1).Name);
            Assert.AreEqual(Convert.ToUInt64(this.seqMetricB.Value), payloadVersionB.Metrics.ElementAt(1).LongValue);
        }
    }
}
