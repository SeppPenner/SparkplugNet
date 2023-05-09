// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugMessageGeneratorTest.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class to test the <see cref="SparkplugMessageGenerator"/> class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Tests;

using System.Text.Json;

/// <summary>
/// A class to test the <see cref="SparkplugMessageGenerator"/> class.
/// </summary>
[TestClass]
public class SparkplugMessageGeneratorTest
{
    /// <summary>
    /// The metrics for namespace B.
    /// </summary>
    private readonly List<VersionB.Data.Metric> metricsB = new()
    {
        new VersionB.Data.Metric
        {
            Name = "Test",
            ValueCase = (uint)VersionB.Data.DataType.Int32,
            IntValue = 20
        }
    };

    /// <summary>
    /// The SEQ metric for namespace B.
    /// </summary>
    private readonly VersionB.Data.Metric seqMetricB = new()
    {
        Name = Constants.SessionNumberMetricName,
        LongValue = 1,
        ValueCase = (uint)VersionB.Data.DataType.Int64
    };

    /// <summary>
    /// The message generator.
    /// </summary>
    private readonly SparkplugMessageGenerator messageGenerator = new(new LoggerConfiguration().WriteTo.Console().CreateLogger());

    /// <summary>
    /// Tests the Sparkplug message generator with a message with a version B namespace and a online state.
    /// </summary>
    [TestMethod]
    public void TestStateMessageNamespaceBOnline()
    {
        var message = SparkplugMessageGenerator.GetSparkplugStateMessage(SparkplugNamespace.VersionB, "scada1", true);

        Assert.AreEqual("STATE/scada1", message.Topic);
        string payloadString = message.ConvertPayloadToString();
        JsonDocument jsonDoc = JsonDocument.Parse(payloadString);
        Assert.IsTrue(jsonDoc.RootElement.TryGetProperty("online", out JsonElement value) && value.GetBoolean());
    }

    /// <summary>
    /// Tests the Sparkplug message generator with a message with a version B namespace and a offline state.
    /// </summary>
    [TestMethod]
    public void TestStateMessageNamespaceBOffline()
    {
        var message = SparkplugMessageGenerator.GetSparkplugStateMessage(SparkplugNamespace.VersionB, "scada1", false);

        Assert.AreEqual("STATE/scada1", message.Topic);
        string payloadString = message.ConvertPayloadToString();
        JsonDocument jsonDoc = JsonDocument.Parse(payloadString);
        Assert.IsTrue(jsonDoc.RootElement.TryGetProperty("online", out JsonElement value) && !value.GetBoolean());
    }

    /// <summary>
    /// Tests the Sparkplug message generator with a device birth message with a version B namespace.
    /// </summary>
    [TestMethod]
    public void TestDeviceBirthMessageNamespaceB()
    {
        var dateTime = DateTimeOffset.UtcNow;
        var message = this.messageGenerator.GetSparkPlugDeviceBirthMessage(SparkplugNamespace.VersionB, "group1", "edge1", "device1", this.metricsB, 0, 1, dateTime);
        var payloadVersionB = PayloadHelper.Deserialize<VersionBProtoBufPayload>(message.Payload);

        Assert.AreEqual("spBv1.0/group1/DBIRTH/edge1/device1", message.Topic);
        Assert.IsNotNull(payloadVersionB);
        Assert.AreEqual((ulong)dateTime.ToUnixTimeMilliseconds(), payloadVersionB.Timestamp);
        Assert.AreEqual(2, payloadVersionB.Metrics.Count);

        Assert.AreEqual(this.metricsB.First().Name, payloadVersionB.Metrics.ElementAt(0).Name);
        Assert.AreEqual(this.metricsB.First().IntValue, payloadVersionB.Metrics.ElementAt(0).IntValue);
        Assert.AreEqual(this.metricsB.First().ValueCase, payloadVersionB.Metrics.ElementAt(0).Datatype);

        Assert.AreEqual(this.seqMetricB.Name, payloadVersionB.Metrics.ElementAt(1).Name);
        Assert.AreEqual(this.seqMetricB.LongValue, payloadVersionB.Metrics.ElementAt(1).LongValue);
        Assert.AreEqual(this.seqMetricB.ValueCase, payloadVersionB.Metrics.ElementAt(1).Datatype);
    }

    /// <summary>
    /// Tests the Sparkplug message generator with a node birth message with a version B namespace.
    /// </summary>
    [TestMethod]
    public void TestNodeBirthMessageNamespaceB()
    {
        var dateTime = DateTimeOffset.UtcNow;
        var message = this.messageGenerator.GetSparkPlugNodeBirthMessage(SparkplugNamespace.VersionB, "group1", "edge1", this.metricsB, 0, 1, dateTime);
        var payloadVersionB = PayloadHelper.Deserialize<VersionBProtoBufPayload>(message.Payload);

        Assert.AreEqual("spBv1.0/group1/NBIRTH/edge1", message.Topic);
        Assert.IsNotNull(payloadVersionB);
        Assert.AreEqual((ulong)dateTime.ToUnixTimeMilliseconds(), payloadVersionB.Timestamp);
        Assert.AreEqual(2, payloadVersionB.Metrics.Count);

        Assert.AreEqual(this.metricsB.First().Name, payloadVersionB.Metrics.ElementAt(0).Name);
        Assert.AreEqual(this.metricsB.First().IntValue, payloadVersionB.Metrics.ElementAt(0).IntValue);
        Assert.AreEqual(this.metricsB.First().ValueCase, payloadVersionB.Metrics.ElementAt(0).Datatype);

        Assert.AreEqual(this.seqMetricB.Name, payloadVersionB.Metrics.ElementAt(1).Name);
        Assert.AreEqual(this.seqMetricB.LongValue, payloadVersionB.Metrics.ElementAt(1).LongValue);
        Assert.AreEqual(this.seqMetricB.ValueCase, payloadVersionB.Metrics.ElementAt(1).Datatype);
    }

    /// <summary>
    /// Tests the Sparkplug message generator with a device death message with a version B namespace.
    /// </summary>
    [TestMethod]
    public void TestDeviceDeathMessageNamespaceB()
    {
        var dateTime = DateTimeOffset.UtcNow;
        var message = this.messageGenerator.GetSparkPlugDeviceDeathMessage(SparkplugNamespace.VersionB, "group1", "edge1", "device1", 0, 1, dateTime);
        var payloadVersionB = PayloadHelper.Deserialize<VersionBProtoBufPayload>(message.Payload);

        Assert.AreEqual("spBv1.0/group1/DDEATH/edge1/device1", message.Topic);
        Assert.IsNotNull(payloadVersionB);
        Assert.AreEqual((ulong)dateTime.ToUnixTimeMilliseconds(), payloadVersionB.Timestamp);
        Assert.AreEqual(1, payloadVersionB.Metrics.Count);

        Assert.AreEqual(this.seqMetricB.Name, payloadVersionB.Metrics.ElementAt(0).Name);
        Assert.AreEqual(this.seqMetricB.LongValue, payloadVersionB.Metrics.ElementAt(0).LongValue);
        Assert.AreEqual(this.seqMetricB.ValueCase, payloadVersionB.Metrics.ElementAt(0).Datatype);
    }

    /// <summary>
    /// Tests the Sparkplug message generator with a node death message with a version B namespace.
    /// </summary>
    [TestMethod]
    public void TestNodeDeathMessageNamespaceB()
    {
        var message = this.messageGenerator.GetSparkPlugNodeDeathMessage(SparkplugNamespace.VersionB, "group1", "edge1", 1);
        var payloadVersionB = PayloadHelper.Deserialize<VersionBProtoBufPayload>(message.Payload);

        Assert.AreEqual("spBv1.0/group1/NDEATH/edge1", message.Topic);
        Assert.IsNotNull(payloadVersionB);
        Assert.AreEqual(1, payloadVersionB.Metrics.Count);

        Assert.AreEqual(this.seqMetricB.Name, payloadVersionB.Metrics.ElementAt(0).Name);
        Assert.AreEqual(this.seqMetricB.LongValue, payloadVersionB.Metrics.ElementAt(0).LongValue);
        Assert.AreEqual(this.seqMetricB.ValueCase, payloadVersionB.Metrics.ElementAt(0).Datatype);
    }

    /// <summary>
    /// Tests the Sparkplug message generator with a device data message with a version B namespace.
    /// </summary>
    [TestMethod]
    public void TestDeviceDataMessageNamespaceB()
    {
        var dateTime = DateTimeOffset.UtcNow;
        var message = this.messageGenerator.GetSparkPlugDeviceDataMessage(SparkplugNamespace.VersionB, "group1", "edge1", "device1", this.metricsB, 0, 1, dateTime);
        var payloadVersionB = PayloadHelper.Deserialize<VersionBProtoBufPayload>(message.Payload);

        Assert.AreEqual("spBv1.0/group1/DDATA/edge1/device1", message.Topic);
        Assert.IsNotNull(payloadVersionB);
        Assert.AreEqual((ulong)dateTime.ToUnixTimeMilliseconds(), payloadVersionB.Timestamp);
        Assert.AreEqual(2, payloadVersionB.Metrics.Count);

        Assert.AreEqual(this.metricsB.First().Name, payloadVersionB.Metrics.ElementAt(0).Name);
        Assert.AreEqual(this.metricsB.First().IntValue, payloadVersionB.Metrics.ElementAt(0).IntValue);
        Assert.AreEqual(this.metricsB.First().ValueCase, payloadVersionB.Metrics.ElementAt(0).Datatype);

        Assert.AreEqual(this.seqMetricB.Name, payloadVersionB.Metrics.ElementAt(1).Name);
        Assert.AreEqual(this.seqMetricB.LongValue, payloadVersionB.Metrics.ElementAt(1).LongValue);
        Assert.AreEqual(this.seqMetricB.ValueCase, payloadVersionB.Metrics.ElementAt(1).Datatype);
    }

    /// <summary>
    /// Tests the Sparkplug message generator with a node data message with a version B namespace.
    /// </summary>
    [TestMethod]
    public void TestNodeDataMessageNamespaceB()
    {
        var dateTime = DateTimeOffset.UtcNow;
        var message = this.messageGenerator.GetSparkPlugNodeDataMessage(SparkplugNamespace.VersionB, "group1", "edge1", this.metricsB, 0, 1, dateTime);
        var payloadVersionB = PayloadHelper.Deserialize<VersionBProtoBufPayload>(message.Payload);

        Assert.AreEqual("spBv1.0/group1/NDATA/edge1", message.Topic);
        Assert.IsNotNull(payloadVersionB);
        Assert.AreEqual((ulong)dateTime.ToUnixTimeMilliseconds(), payloadVersionB.Timestamp);
        Assert.AreEqual(2, payloadVersionB.Metrics.Count);

        Assert.AreEqual(this.metricsB.First().Name, payloadVersionB.Metrics.ElementAt(0).Name);
        Assert.AreEqual(this.metricsB.First().IntValue, payloadVersionB.Metrics.ElementAt(0).IntValue);
        Assert.AreEqual(this.metricsB.First().ValueCase, payloadVersionB.Metrics.ElementAt(0).Datatype);

        Assert.AreEqual(this.seqMetricB.Name, payloadVersionB.Metrics.ElementAt(1).Name);
        Assert.AreEqual(this.seqMetricB.LongValue, payloadVersionB.Metrics.ElementAt(1).LongValue);
        Assert.AreEqual(this.seqMetricB.ValueCase, payloadVersionB.Metrics.ElementAt(1).Datatype);
    }

    /// <summary>
    /// Tests the Sparkplug message generator with a device command message with a version B namespace.
    /// </summary>
    [TestMethod]
    public void TestDeviceCommandMessageNamespaceB()
    {
        var dateTime = DateTimeOffset.UtcNow;
        var message = SparkplugMessageGenerator.GetSparkPlugDeviceCommandMessage(SparkplugNamespace.VersionB, "group1", "edge1", "device1", this.metricsB, 0, 1, dateTime);
        var payloadVersionB = PayloadHelper.Deserialize<VersionBProtoBufPayload>(message.Payload);

        Assert.AreEqual("spBv1.0/group1/DCMD/edge1/device1", message.Topic);
        Assert.IsNotNull(payloadVersionB);
        Assert.AreEqual((ulong)dateTime.ToUnixTimeMilliseconds(), payloadVersionB.Timestamp);
        Assert.AreEqual(2, payloadVersionB.Metrics.Count);

        Assert.AreEqual(this.metricsB.First().Name, payloadVersionB.Metrics.ElementAt(0).Name);
        Assert.AreEqual(this.metricsB.First().IntValue, payloadVersionB.Metrics.ElementAt(0).IntValue);
        Assert.AreEqual(this.metricsB.First().ValueCase, payloadVersionB.Metrics.ElementAt(0).Datatype);

        Assert.AreEqual(this.seqMetricB.Name, payloadVersionB.Metrics.ElementAt(1).Name);
        Assert.AreEqual(this.seqMetricB.LongValue, payloadVersionB.Metrics.ElementAt(1).LongValue);
        Assert.AreEqual(this.seqMetricB.ValueCase, payloadVersionB.Metrics.ElementAt(1).Datatype);
    }

    /// <summary>
    /// Tests the Sparkplug message generator with a node command message with a version B namespace.
    /// </summary>
    [TestMethod]
    public void TestNodeCommandMessageNamespaceB()
    {
        var dateTime = DateTimeOffset.UtcNow;
        var message = SparkplugMessageGenerator.GetSparkPlugNodeCommandMessage(SparkplugNamespace.VersionB, "group1", "edge1", this.metricsB, 0, 1, dateTime);
        var payloadVersionB = PayloadHelper.Deserialize<VersionBProtoBufPayload>(message.Payload);

        Assert.AreEqual("spBv1.0/group1/NCMD/edge1", message.Topic);
        Assert.IsNotNull(payloadVersionB);
        Assert.AreEqual((ulong)dateTime.ToUnixTimeMilliseconds(), payloadVersionB.Timestamp);
        Assert.AreEqual(2, payloadVersionB.Metrics.Count);

        Assert.AreEqual(this.metricsB.First().Name, payloadVersionB.Metrics.ElementAt(0).Name);
        Assert.AreEqual(this.metricsB.First().IntValue, payloadVersionB.Metrics.ElementAt(0).IntValue);
        Assert.AreEqual(this.metricsB.First().ValueCase, payloadVersionB.Metrics.ElementAt(0).Datatype);

        Assert.AreEqual(this.seqMetricB.Name, payloadVersionB.Metrics.ElementAt(1).Name);
        Assert.AreEqual(this.seqMetricB.LongValue, payloadVersionB.Metrics.ElementAt(1).LongValue);
        Assert.AreEqual(this.seqMetricB.ValueCase, payloadVersionB.Metrics.ElementAt(1).Datatype);
    }
}
