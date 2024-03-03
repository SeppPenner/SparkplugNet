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
    private readonly List<VersionBData.Metric> metricsB = new()
    {
        new VersionBData.Metric("Test", VersionBData.DataType.Int32, 20)
    };

    /// <summary>
    /// The SEQ metric for namespace B.
    /// </summary>
    private readonly VersionBData.Metric seqMetricB = new(Constants.SessionNumberMetricName, VersionBData.DataType.Int64, 1);

    /// <summary>
    /// The message generator.
    /// </summary>
    private readonly SparkplugMessageGenerator messageGenerator = new(new LoggerConfiguration().WriteTo.Console().CreateLogger(),
        SparkplugSpecificationVersion.Version30);

    /// <summary>
    /// Tests the Sparkplug message generator with a message with a version B namespace and a online state.
    /// </summary>
    [TestMethod]
    public void TestStateMessageNamespaceBOnline()
    {
        var message = this.messageGenerator.GetSparkplugStateMessage(SparkplugNamespace.VersionB, "scada1", true);
        var payloadVersionB = JsonSerializer.Deserialize<StateMessage>(message.Payload);

        Assert.AreEqual("spBv1.0/STATE/scada1", message.Topic);
        Assert.IsNotNull(payloadVersionB);
        Assert.AreEqual(true, payloadVersionB.Online);
        // Todo: Check timestamp here?
        Assert.IsNotNull(payloadVersionB.Timestamp);
    }

    /// <summary>
    /// Tests the Sparkplug message generator with a message with a version B namespace and a offline state.
    /// </summary>
    [TestMethod]
    public void TestStateMessageNamespaceBOffline()
    {
        var message = this.messageGenerator.GetSparkplugStateMessage(SparkplugNamespace.VersionB, "scada1", false);
        var payloadVersionB = JsonSerializer.Deserialize<StateMessage>(message.Payload);

        Assert.AreEqual("spBv1.0/STATE/scada1", message.Topic);
        Assert.IsNotNull(payloadVersionB);
        Assert.AreEqual(false, payloadVersionB.Online);
        // Todo: Check timestamp here?
        Assert.IsNotNull(payloadVersionB.Timestamp);
    }

    /// <summary>
    /// Tests the Sparkplug message generator with a device birth message with a version B namespace.
    /// </summary>
    [TestMethod]
    public void TestDeviceBirthMessageNamespaceB()
    {
        var dateTime = DateTimeOffset.UtcNow;
        var message = this.messageGenerator.GetSparkplugDeviceBirthMessage(SparkplugNamespace.VersionB, "group1", "edge1", "device1", this.metricsB, 0, 1, dateTime);
        var payloadVersionB = PayloadHelper.Deserialize<VersionBProtoBufPayload>(message.Payload);

        Assert.AreEqual("spBv1.0/group1/DBIRTH/edge1/device1", message.Topic);
        Assert.IsNotNull(payloadVersionB);
        Assert.AreEqual((ulong)dateTime.ToUnixTimeMilliseconds(), payloadVersionB.Timestamp);
        Assert.AreEqual(2, payloadVersionB.Metrics.Count);

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
        var payloadVersionB = PayloadHelper.Deserialize<VersionBProtoBufPayload>(message.Payload);

        Assert.AreEqual("spBv1.0/group1/NBIRTH/edge1", message.Topic);
        Assert.IsNotNull(payloadVersionB);
        Assert.AreEqual((ulong)dateTime.ToUnixTimeMilliseconds(), payloadVersionB.Timestamp);
        Assert.AreEqual(2, payloadVersionB.Metrics.Count);

        Assert.AreEqual(this.metricsB.First().Name, payloadVersionB.Metrics.ElementAt(0).Name);
        Assert.AreEqual(Convert.ToUInt32(this.metricsB.First().Value), payloadVersionB.Metrics.ElementAt(0).IntValue);
        Assert.AreEqual((uint?)this.metricsB.First().DataType, payloadVersionB.Metrics.ElementAt(0).DataType);

        Assert.AreEqual(this.seqMetricB.Name, payloadVersionB.Metrics.ElementAt(1).Name);
        Assert.AreEqual(Convert.ToUInt64(this.seqMetricB.Value), payloadVersionB.Metrics.ElementAt(1).LongValue);
        Assert.AreEqual((uint?)this.seqMetricB.DataType, payloadVersionB.Metrics.ElementAt(1).DataType);
    }

    /// <summary>
    /// Tests the Sparkplug message generator with a device death message with a version B namespace.
    /// </summary>
    [TestMethod]
    public void TestDeviceDeathMessageNamespaceB()
    {
        var dateTime = DateTimeOffset.UtcNow;
        var message = this.messageGenerator.GetSparkplugDeviceDeathMessage(SparkplugNamespace.VersionB, "group1", "edge1", "device1", 0, 1, dateTime);
        var payloadVersionB = PayloadHelper.Deserialize<VersionBProtoBufPayload>(message.Payload);

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
        var payloadVersionB = PayloadHelper.Deserialize<VersionBProtoBufPayload>(message.Payload);

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
        var dateTime = DateTimeOffset.UtcNow;
        var message = this.messageGenerator.GetSparkplugDeviceDataMessage(SparkplugNamespace.VersionB, "group1", "edge1", "device1", this.metricsB, 0, 1, dateTime);
        var payloadVersionB = PayloadHelper.Deserialize<VersionBProtoBufPayload>(message.Payload);

        Assert.AreEqual("spBv1.0/group1/DDATA/edge1/device1", message.Topic);
        Assert.IsNotNull(payloadVersionB);
        Assert.AreEqual((ulong)dateTime.ToUnixTimeMilliseconds(), payloadVersionB.Timestamp);
        Assert.AreEqual(2, payloadVersionB.Metrics.Count);

        Assert.AreEqual(this.metricsB.First().Name, payloadVersionB.Metrics.ElementAt(0).Name);
        Assert.AreEqual(Convert.ToUInt32(this.metricsB.First().Value), payloadVersionB.Metrics.ElementAt(0).IntValue);
        Assert.AreEqual((uint?)this.metricsB.First().DataType, payloadVersionB.Metrics.ElementAt(0).DataType);

        Assert.AreEqual(this.seqMetricB.Name, payloadVersionB.Metrics.ElementAt(1).Name);
        Assert.AreEqual(Convert.ToUInt64(this.seqMetricB.Value), payloadVersionB.Metrics.ElementAt(1).LongValue);
        Assert.AreEqual((uint?)this.seqMetricB.DataType, payloadVersionB.Metrics.ElementAt(1).DataType);
    }

    /// <summary>
    /// Tests the Sparkplug message generator with a node data message with a version B namespace.
    /// </summary>
    [TestMethod]
    public void TestNodeDataMessageNamespaceB()
    {
        var dateTime = DateTimeOffset.UtcNow;
        var message = this.messageGenerator.GetSparkplugNodeDataMessage(SparkplugNamespace.VersionB, "group1", "edge1", this.metricsB, 0, 1, dateTime);
        var payloadVersionB = PayloadHelper.Deserialize<VersionBProtoBufPayload>(message.Payload);

        Assert.AreEqual("spBv1.0/group1/NDATA/edge1", message.Topic);
        Assert.IsNotNull(payloadVersionB);
        Assert.AreEqual((ulong)dateTime.ToUnixTimeMilliseconds(), payloadVersionB.Timestamp);
        Assert.AreEqual(2, payloadVersionB.Metrics.Count);

        Assert.AreEqual(this.metricsB.First().Name, payloadVersionB.Metrics.ElementAt(0).Name);
        Assert.AreEqual(Convert.ToUInt32(this.metricsB.First().Value), payloadVersionB.Metrics.ElementAt(0).IntValue);
        Assert.AreEqual((uint?)this.metricsB.First().DataType, payloadVersionB.Metrics.ElementAt(0).DataType);

        Assert.AreEqual(this.seqMetricB.Name, payloadVersionB.Metrics.ElementAt(1).Name);
        Assert.AreEqual(Convert.ToUInt64(this.seqMetricB.Value), payloadVersionB.Metrics.ElementAt(1).LongValue);
        Assert.AreEqual((uint?)this.seqMetricB.DataType, payloadVersionB.Metrics.ElementAt(1).DataType);
    }

    /// <summary>
    /// Tests the Sparkplug message generator with a device command message with a version B namespace.
    /// </summary>
    [TestMethod]
    public void TestDeviceCommandMessageNamespaceB()
    {
        var dateTime = DateTimeOffset.UtcNow;
        var message = this.messageGenerator.GetSparkplugDeviceCommandMessage(SparkplugNamespace.VersionB, "group1", "edge1", "device1", this.metricsB, 0, 1, dateTime);
        var payloadVersionB = PayloadHelper.Deserialize<VersionBProtoBufPayload>(message.Payload);

        Assert.AreEqual("spBv1.0/group1/DCMD/edge1/device1", message.Topic);
        Assert.IsNotNull(payloadVersionB);
        Assert.AreEqual((ulong)dateTime.ToUnixTimeMilliseconds(), payloadVersionB.Timestamp);
        Assert.AreEqual(2, payloadVersionB.Metrics.Count);

        Assert.AreEqual(this.metricsB.First().Name, payloadVersionB.Metrics.ElementAt(0).Name);
        Assert.AreEqual(Convert.ToUInt32(this.metricsB.First().Value), payloadVersionB.Metrics.ElementAt(0).IntValue);
        Assert.AreEqual((uint?)this.metricsB.First().DataType, payloadVersionB.Metrics.ElementAt(0).DataType);

        Assert.AreEqual(this.seqMetricB.Name, payloadVersionB.Metrics.ElementAt(1).Name);
        Assert.AreEqual(Convert.ToUInt64(this.seqMetricB.Value), payloadVersionB.Metrics.ElementAt(1).LongValue);
        Assert.AreEqual((uint?)this.seqMetricB.DataType, payloadVersionB.Metrics.ElementAt(1).DataType);
    }

    /// <summary>
    /// Tests the Sparkplug message generator with a node command message with a version B namespace.
    /// </summary>
    [TestMethod]
    public void TestNodeCommandMessageNamespaceB()
    {
        var dateTime = DateTimeOffset.UtcNow;
        var message = this.messageGenerator.GetSparkplugNodeCommandMessage(SparkplugNamespace.VersionB, "group1", "edge1", this.metricsB, 0, 1, dateTime);
        var payloadVersionB = PayloadHelper.Deserialize<VersionBProtoBufPayload>(message.Payload);

        Assert.AreEqual("spBv1.0/group1/NCMD/edge1", message.Topic);
        Assert.IsNotNull(payloadVersionB);
        Assert.AreEqual((ulong)dateTime.ToUnixTimeMilliseconds(), payloadVersionB.Timestamp);
        Assert.AreEqual(2, payloadVersionB.Metrics.Count);

        Assert.AreEqual(this.metricsB.First().Name, payloadVersionB.Metrics.ElementAt(0).Name);
        Assert.AreEqual(Convert.ToUInt32(this.metricsB.First().Value), payloadVersionB.Metrics.ElementAt(0).IntValue);
        Assert.AreEqual((uint?)this.metricsB.First().DataType, payloadVersionB.Metrics.ElementAt(0).DataType);

        Assert.AreEqual(this.seqMetricB.Name, payloadVersionB.Metrics.ElementAt(1).Name);
        Assert.AreEqual(Convert.ToUInt64(this.seqMetricB.Value), payloadVersionB.Metrics.ElementAt(1).LongValue);
        Assert.AreEqual((uint?)this.seqMetricB.DataType, payloadVersionB.Metrics.ElementAt(1).DataType);
    }
}
