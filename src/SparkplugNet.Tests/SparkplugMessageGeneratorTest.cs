// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugMessageGeneratorTest.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class to test the <see cref="SparkplugMessageGenerator"/> class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Tests;

/// <summary>
/// A class to test the <see cref="SparkplugMessageGenerator"/> class.
/// </summary>
[TestClass]
public class SparkplugMessageGeneratorTest
{
    /// <summary>
    /// The metrics for namespace A.
    /// </summary>
    private readonly List<VersionA.Data.KuraMetric> metricsA = new()
    {
        new VersionA.Data.KuraMetric
        {
            Name = "Test",
            BooleanValue = true,
            DataType = VersionA.Data.DataType.Boolean
        }
    };

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
    /// The SEQ metric for namespace A.
    /// </summary>
    private readonly VersionA.Data.KuraMetric seqMetricA = new()
    {
        Name = Constants.SessionNumberMetricName,
        LongValue = 1,
        DataType = VersionA.Data.DataType.Int64
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
    /// Tests the Sparkplug message generator with a message with a version A namespace and a online state.
    /// </summary>
    [TestMethod]
    public void TestStateMessageNamespaceAOnline()
    {
        var message = SparkplugMessageGenerator.GetSparkplugStateMessage(SparkplugNamespace.VersionA, "scada1", true);

        Assert.AreEqual("STATE/scada1", message.Topic);
        Assert.AreEqual("ONLINE", message.ConvertPayloadToString());
    }

    /// <summary>
    /// Tests the Sparkplug message generator with a message with a version A namespace and a offline state.
    /// </summary>
    [TestMethod]
    public void TestStateMessageNamespaceAOffline()
    {
        var message = SparkplugMessageGenerator.GetSparkplugStateMessage(SparkplugNamespace.VersionA, "scada1", false);

        Assert.AreEqual("STATE/scada1", message.Topic);
        Assert.AreEqual("OFFLINE", message.ConvertPayloadToString());
    }

    /// <summary>
    /// Tests the Sparkplug message generator with a message with a version B namespace and a online state.
    /// </summary>
    [TestMethod]
    public void TestStateMessageNamespaceBOnline()
    {
        var message = SparkplugMessageGenerator.GetSparkplugStateMessage(SparkplugNamespace.VersionB, "scada1", true);

        Assert.AreEqual("STATE/scada1", message.Topic);
        Assert.AreEqual("ONLINE", message.ConvertPayloadToString());
    }

    /// <summary>
    /// Tests the Sparkplug message generator with a message with a version B namespace and a offline state.
    /// </summary>
    [TestMethod]
    public void TestStateMessageNamespaceBOffline()
    {
        var message = SparkplugMessageGenerator.GetSparkplugStateMessage(SparkplugNamespace.VersionB, "scada1", false);

        Assert.AreEqual("STATE/scada1", message.Topic);
        Assert.AreEqual("OFFLINE", message.ConvertPayloadToString());
    }

    /// <summary>
    /// Tests the Sparkplug message generator with a device birth message with a version A namespace.
    /// </summary>
    [TestMethod]
    public void TestDeviceBirthMessageNamespaceA()
    {
        var dateTime = DateTimeOffset.Now;
        var message = this.messageGenerator.GetSparkPlugDeviceBirthMessage(SparkplugNamespace.VersionA, "group1", "edge1", "device1", this.metricsA, 0, 1, dateTime);
        var payloadVersionA = PayloadHelper.Deserialize<VersionAProtoBufPayload>(message.Payload);

        Assert.AreEqual("spAv1.0/group1/DBIRTH/edge1/device1", message.Topic);
        Assert.IsNotNull(payloadVersionA);
        Assert.AreEqual(dateTime.ToUnixTimeMilliseconds(), payloadVersionA.Timestamp);
        Assert.AreEqual(2, payloadVersionA.Metrics.Count);

        Assert.AreEqual(this.metricsA.First().Name, payloadVersionA.Metrics.ElementAt(0).Name);
        Assert.AreEqual(this.metricsA.First().BooleanValue, payloadVersionA.Metrics.ElementAt(0).BoolValue);
        Assert.AreEqual(this.metricsA.First().DataType, PayloadConverter.ConvertVersionADataType(payloadVersionA.Metrics.ElementAt(0).Type));

        Assert.AreEqual(this.seqMetricA.Name, payloadVersionA.Metrics.ElementAt(1).Name);
        Assert.AreEqual(this.seqMetricA.LongValue, payloadVersionA.Metrics.ElementAt(1).LongValue);
        Assert.AreEqual(this.seqMetricA.DataType, PayloadConverter.ConvertVersionADataType(payloadVersionA.Metrics.ElementAt(1).Type));
    }

    /// <summary>
    /// Tests the Sparkplug message generator with a device birth message with a version B namespace.
    /// </summary>
    [TestMethod]
    public void TestDeviceBirthMessageNamespaceB()
    {
        var dateTime = DateTimeOffset.Now;
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
        Assert.AreEqual(this.seqMetricB.ULongValue, payloadVersionB.Metrics.ElementAt(1).UlongValue);
        Assert.AreEqual(this.seqMetricB.ValueCase, payloadVersionB.Metrics.ElementAt(1).Datatype);
    }

    /// <summary>
    /// Tests the Sparkplug message generator with a node birth message with a version A namespace.
    /// </summary>
    [TestMethod]
    public void TestNodeBirthMessageNamespaceA()
    {
        var dateTime = DateTimeOffset.Now;
        var message = this.messageGenerator.GetSparkPlugNodeBirthMessage(SparkplugNamespace.VersionA, "group1", "edge1", this.metricsA, 0, 1, dateTime);
        var payloadVersionA = PayloadHelper.Deserialize<VersionAProtoBufPayload>(message.Payload);

        Assert.AreEqual("spAv1.0/group1/NBIRTH/edge1", message.Topic);
        Assert.IsNotNull(payloadVersionA);
        Assert.AreEqual(dateTime.ToUnixTimeMilliseconds(), payloadVersionA.Timestamp);
        Assert.AreEqual(2, payloadVersionA.Metrics.Count);

        Assert.AreEqual(this.metricsA.First().Name, payloadVersionA.Metrics.ElementAt(0).Name);
        Assert.AreEqual(this.metricsA.First().BooleanValue, payloadVersionA.Metrics.ElementAt(0).BoolValue);
        Assert.AreEqual(this.metricsA.First().DataType, PayloadConverter.ConvertVersionADataType(payloadVersionA.Metrics.ElementAt(0).Type));

        Assert.AreEqual(this.seqMetricA.Name, payloadVersionA.Metrics.ElementAt(1).Name);
        Assert.AreEqual(this.seqMetricA.LongValue, payloadVersionA.Metrics.ElementAt(1).LongValue);
        Assert.AreEqual(this.seqMetricA.DataType, PayloadConverter.ConvertVersionADataType(payloadVersionA.Metrics.ElementAt(1).Type));
    }

    /// <summary>
    /// Tests the Sparkplug message generator with a node birth message with a version B namespace.
    /// </summary>
    [TestMethod]
    public void TestNodeBirthMessageNamespaceB()
    {
        var dateTime = DateTimeOffset.Now;
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
        Assert.AreEqual(this.seqMetricB.ULongValue, payloadVersionB.Metrics.ElementAt(1).UlongValue);
        Assert.AreEqual(this.seqMetricB.ValueCase, payloadVersionB.Metrics.ElementAt(1).Datatype);
    }

    /// <summary>
    /// Tests the Sparkplug message generator with a device death message with a version A namespace.
    /// </summary>
    [TestMethod]
    public void TestDeviceDeathMessageNamespaceA()
    {
        var dateTime = DateTimeOffset.Now;
        var message = this.messageGenerator.GetSparkPlugDeviceDeathMessage(SparkplugNamespace.VersionA, "group1", "edge1", "device1", 0, 1, dateTime);
        var payloadVersionA = PayloadHelper.Deserialize<VersionAProtoBufPayload>(message.Payload);

        Assert.AreEqual("spAv1.0/group1/DDEATH/edge1/device1", message.Topic);
        Assert.IsNotNull(payloadVersionA);
        Assert.AreEqual(dateTime.ToUnixTimeMilliseconds(), payloadVersionA.Timestamp);
        Assert.AreEqual(1, payloadVersionA.Metrics.Count);

        Assert.AreEqual(this.seqMetricA.Name, payloadVersionA.Metrics.ElementAt(0).Name);
        Assert.AreEqual(this.seqMetricA.LongValue, payloadVersionA.Metrics.ElementAt(0).LongValue);
        Assert.AreEqual(this.seqMetricA.DataType, PayloadConverter.ConvertVersionADataType(payloadVersionA.Metrics.ElementAt(0).Type));
    }

    /// <summary>
    /// Tests the Sparkplug message generator with a device death message with a version B namespace.
    /// </summary>
    [TestMethod]
    public void TestDeviceDeathMessageNamespaceB()
    {
        var dateTime = DateTimeOffset.Now;
        var message = this.messageGenerator.GetSparkPlugDeviceDeathMessage(SparkplugNamespace.VersionB, "group1", "edge1", "device1", 0, 1, dateTime);
        var payloadVersionB = PayloadHelper.Deserialize<VersionBProtoBufPayload>(message.Payload);

        Assert.AreEqual("spBv1.0/group1/DDEATH/edge1/device1", message.Topic);
        Assert.IsNotNull(payloadVersionB);
        Assert.AreEqual((ulong)dateTime.ToUnixTimeMilliseconds(), payloadVersionB.Timestamp);
        Assert.AreEqual(1, payloadVersionB.Metrics.Count);

        Assert.AreEqual(this.seqMetricB.Name, payloadVersionB.Metrics.ElementAt(0).Name);
        Assert.AreEqual(this.seqMetricB.ULongValue, payloadVersionB.Metrics.ElementAt(0).UlongValue);
        Assert.AreEqual(this.seqMetricB.ValueCase, payloadVersionB.Metrics.ElementAt(0).Datatype);
    }

    /// <summary>
    /// Tests the Sparkplug message generator with a node death message with a version A namespace.
    /// </summary>
    [TestMethod]
    public void TestNodeDeathMessageNamespaceA()
    {
        var message = this.messageGenerator.GetSparkPlugNodeDeathMessage(SparkplugNamespace.VersionA, "group1", "edge1", 1);
        var payloadVersionA = PayloadHelper.Deserialize<VersionAProtoBufPayload>(message.Payload);

        Assert.AreEqual("spAv1.0/group1/NDEATH/edge1", message.Topic);
        Assert.IsNotNull(payloadVersionA);
        Assert.AreEqual(1, payloadVersionA.Metrics.Count);

        Assert.AreEqual(this.seqMetricA.Name, payloadVersionA.Metrics.ElementAt(0).Name);
        Assert.AreEqual(this.seqMetricA.LongValue, payloadVersionA.Metrics.ElementAt(0).LongValue);
        Assert.AreEqual(this.seqMetricA.DataType, PayloadConverter.ConvertVersionADataType(payloadVersionA.Metrics.ElementAt(0).Type));
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
        Assert.AreEqual(this.seqMetricB.ULongValue, payloadVersionB.Metrics.ElementAt(0).UlongValue);
        Assert.AreEqual(this.seqMetricB.ValueCase, payloadVersionB.Metrics.ElementAt(0).Datatype);
    }

    /// <summary>
    /// Tests the Sparkplug message generator with a device data message with a version A namespace.
    /// </summary>
    [TestMethod]
    public void TestDeviceDataMessageNamespaceA()
    {
        var dateTime = DateTimeOffset.Now;
        var message = this.messageGenerator.GetSparkPlugDeviceDataMessage(SparkplugNamespace.VersionA, "group1", "edge1", "device1", this.metricsA, 0, 1, dateTime, true);
        var payloadVersionA = PayloadHelper.Deserialize<VersionAProtoBufPayload>(message.Payload);

        Assert.AreEqual("spAv1.0/group1/DDATA/edge1/device1", message.Topic);
        Assert.IsNotNull(payloadVersionA);
        Assert.AreEqual(dateTime.ToUnixTimeMilliseconds(), payloadVersionA.Timestamp);
        Assert.AreEqual(2, payloadVersionA.Metrics.Count);

        Assert.AreEqual(this.metricsA.First().Name, payloadVersionA.Metrics.ElementAt(0).Name);
        Assert.AreEqual(this.metricsA.First().BooleanValue, payloadVersionA.Metrics.ElementAt(0).BoolValue);
        Assert.AreEqual(this.metricsA.First().DataType, PayloadConverter.ConvertVersionADataType(payloadVersionA.Metrics.ElementAt(0).Type));

        Assert.AreEqual(this.seqMetricA.Name, payloadVersionA.Metrics.ElementAt(1).Name);
        Assert.AreEqual(this.seqMetricA.LongValue, payloadVersionA.Metrics.ElementAt(1).LongValue);
        Assert.AreEqual(this.seqMetricA.DataType, PayloadConverter.ConvertVersionADataType(payloadVersionA.Metrics.ElementAt(1).Type));
    }

    /// <summary>
    /// Tests the Sparkplug message generator with a device data message with a version B namespace.
    /// </summary>
    [TestMethod]
    public void TestDeviceDataMessageNamespaceB()
    {
        var dateTime = DateTimeOffset.Now;
        var message = this.messageGenerator.GetSparkPlugDeviceDataMessage(SparkplugNamespace.VersionB, "group1", "edge1", "device1", this.metricsB, 0, 1, dateTime, true);
        var payloadVersionB = PayloadHelper.Deserialize<VersionBProtoBufPayload>(message.Payload);

        Assert.AreEqual("spBv1.0/group1/DDATA/edge1/device1", message.Topic);
        Assert.IsNotNull(payloadVersionB);
        Assert.AreEqual((ulong)dateTime.ToUnixTimeMilliseconds(), payloadVersionB.Timestamp);
        Assert.AreEqual(2, payloadVersionB.Metrics.Count);

        Assert.AreEqual(this.metricsB.First().Name, payloadVersionB.Metrics.ElementAt(0).Name);
        Assert.AreEqual(this.metricsB.First().IntValue, payloadVersionB.Metrics.ElementAt(0).IntValue);
        Assert.AreEqual(this.metricsB.First().ValueCase, payloadVersionB.Metrics.ElementAt(0).Datatype);

        Assert.AreEqual(this.seqMetricB.Name, payloadVersionB.Metrics.ElementAt(1).Name);
        Assert.AreEqual(this.seqMetricB.ULongValue, payloadVersionB.Metrics.ElementAt(1).UlongValue);
        Assert.AreEqual(this.seqMetricB.ValueCase, payloadVersionB.Metrics.ElementAt(1).Datatype);
    }

    /// <summary>
    /// Tests the Sparkplug message generator with a node data message with a version A namespace.
    /// </summary>
    [TestMethod]
    public void TestNodeDataMessageNamespaceA()
    {
        var dateTime = DateTimeOffset.Now;
        var message = this.messageGenerator.GetSparkPlugNodeDataMessage(SparkplugNamespace.VersionA, "group1", "edge1", this.metricsA, 0, 1, dateTime, true);
        var payloadVersionA = PayloadHelper.Deserialize<VersionAProtoBufPayload>(message.Payload);

        Assert.AreEqual("spAv1.0/group1/NDATA/edge1", message.Topic);
        Assert.IsNotNull(payloadVersionA);
        Assert.AreEqual(dateTime.ToUnixTimeMilliseconds(), payloadVersionA.Timestamp);
        Assert.AreEqual(2, payloadVersionA.Metrics.Count);

        Assert.AreEqual(this.metricsA.First().Name, payloadVersionA.Metrics.ElementAt(0).Name);
        Assert.AreEqual(this.metricsA.First().BooleanValue, payloadVersionA.Metrics.ElementAt(0).BoolValue);
        Assert.AreEqual(this.metricsA.First().DataType, PayloadConverter.ConvertVersionADataType(payloadVersionA.Metrics.ElementAt(0).Type));

        Assert.AreEqual(this.seqMetricA.Name, payloadVersionA.Metrics.ElementAt(1).Name);
        Assert.AreEqual(this.seqMetricA.LongValue, payloadVersionA.Metrics.ElementAt(1).LongValue);
        Assert.AreEqual(this.seqMetricA.DataType, PayloadConverter.ConvertVersionADataType(payloadVersionA.Metrics.ElementAt(1).Type));
    }

    /// <summary>
    /// Tests the Sparkplug message generator with a node data message with a version B namespace.
    /// </summary>
    [TestMethod]
    public void TestNodeDataMessageNamespaceB()
    {
        var dateTime = DateTimeOffset.Now;
        var message = this.messageGenerator.GetSparkPlugNodeDataMessage(SparkplugNamespace.VersionB, "group1", "edge1", this.metricsB, 0, 1, dateTime, true);
        var payloadVersionB = PayloadHelper.Deserialize<VersionBProtoBufPayload>(message.Payload);

        Assert.AreEqual("spBv1.0/group1/NDATA/edge1", message.Topic);
        Assert.IsNotNull(payloadVersionB);
        Assert.AreEqual((ulong)dateTime.ToUnixTimeMilliseconds(), payloadVersionB.Timestamp);
        Assert.AreEqual(2, payloadVersionB.Metrics.Count);

        Assert.AreEqual(this.metricsB.First().Name, payloadVersionB.Metrics.ElementAt(0).Name);
        Assert.AreEqual(this.metricsB.First().IntValue, payloadVersionB.Metrics.ElementAt(0).IntValue);
        Assert.AreEqual(this.metricsB.First().ValueCase, payloadVersionB.Metrics.ElementAt(0).Datatype);

        Assert.AreEqual(this.seqMetricB.Name, payloadVersionB.Metrics.ElementAt(1).Name);
        Assert.AreEqual(this.seqMetricB.ULongValue, payloadVersionB.Metrics.ElementAt(1).UlongValue);
        Assert.AreEqual(this.seqMetricB.ValueCase, payloadVersionB.Metrics.ElementAt(1).Datatype);
    }

    /// <summary>
    /// Tests the Sparkplug message generator with a device command message with a version A namespace.
    /// </summary>
    [TestMethod]
    public void TestDeviceCommandMessageNamespaceA()
    {
        var dateTime = DateTimeOffset.Now;
        var message = SparkplugMessageGenerator.GetSparkPlugDeviceCommandMessage(SparkplugNamespace.VersionA, "group1", "edge1", "device1", this.metricsA, 0, 1, dateTime, true);
        var payloadVersionA = PayloadHelper.Deserialize<VersionAProtoBufPayload>(message.Payload);

        Assert.AreEqual("spAv1.0/group1/DCMD/edge1/device1", message.Topic);
        Assert.IsNotNull(payloadVersionA);
        Assert.AreEqual(dateTime.ToUnixTimeMilliseconds(), payloadVersionA.Timestamp);
        Assert.AreEqual(2, payloadVersionA.Metrics.Count);

        Assert.AreEqual(this.metricsA.First().Name, payloadVersionA.Metrics.ElementAt(0).Name);
        Assert.AreEqual(this.metricsA.First().BooleanValue, payloadVersionA.Metrics.ElementAt(0).BoolValue);
        Assert.AreEqual(this.metricsA.First().DataType, PayloadConverter.ConvertVersionADataType(payloadVersionA.Metrics.ElementAt(0).Type));

        Assert.AreEqual(this.seqMetricA.Name, payloadVersionA.Metrics.ElementAt(1).Name);
        Assert.AreEqual(this.seqMetricA.LongValue, payloadVersionA.Metrics.ElementAt(1).LongValue);
        Assert.AreEqual(this.seqMetricA.DataType, PayloadConverter.ConvertVersionADataType(payloadVersionA.Metrics.ElementAt(1).Type));
    }

    /// <summary>
    /// Tests the Sparkplug message generator with a device command message with a version B namespace.
    /// </summary>
    [TestMethod]
    public void TestDeviceCommandMessageNamespaceB()
    {
        var dateTime = DateTimeOffset.Now;
        var message = SparkplugMessageGenerator.GetSparkPlugDeviceCommandMessage(SparkplugNamespace.VersionB, "group1", "edge1", "device1", this.metricsB, 0, 1, dateTime, true);
        var payloadVersionB = PayloadHelper.Deserialize<VersionBProtoBufPayload>(message.Payload);

        Assert.AreEqual("spBv1.0/group1/DCMD/edge1/device1", message.Topic);
        Assert.IsNotNull(payloadVersionB);
        Assert.AreEqual((ulong)dateTime.ToUnixTimeMilliseconds(), payloadVersionB.Timestamp);
        Assert.AreEqual(2, payloadVersionB.Metrics.Count);

        Assert.AreEqual(this.metricsB.First().Name, payloadVersionB.Metrics.ElementAt(0).Name);
        Assert.AreEqual(this.metricsB.First().IntValue, payloadVersionB.Metrics.ElementAt(0).IntValue);
        Assert.AreEqual(this.metricsB.First().ValueCase, payloadVersionB.Metrics.ElementAt(0).Datatype);

        Assert.AreEqual(this.seqMetricB.Name, payloadVersionB.Metrics.ElementAt(1).Name);
        Assert.AreEqual(this.seqMetricB.ULongValue, payloadVersionB.Metrics.ElementAt(1).UlongValue);
        Assert.AreEqual(this.seqMetricB.ValueCase, payloadVersionB.Metrics.ElementAt(1).Datatype);
    }

    /// <summary>
    /// Tests the Sparkplug message generator with a node command message with a version A namespace.
    /// </summary>
    [TestMethod]
    public void TestNodeCommandMessageNamespaceA()
    {
        var dateTime = DateTimeOffset.Now;
        var message = SparkplugMessageGenerator.GetSparkPlugNodeCommandMessage(SparkplugNamespace.VersionA, "group1", "edge1", this.metricsA, 0, 1, dateTime, true);
        var payloadVersionA = PayloadHelper.Deserialize<VersionAProtoBufPayload>(message.Payload);

        Assert.AreEqual("spAv1.0/group1/NCMD/edge1", message.Topic);
        Assert.IsNotNull(payloadVersionA);
        Assert.AreEqual(dateTime.ToUnixTimeMilliseconds(), payloadVersionA.Timestamp);
        Assert.AreEqual(2, payloadVersionA.Metrics.Count);

        Assert.AreEqual(this.metricsA.First().Name, payloadVersionA.Metrics.ElementAt(0).Name);
        Assert.AreEqual(this.metricsA.First().BooleanValue, payloadVersionA.Metrics.ElementAt(0).BoolValue);
        Assert.AreEqual(this.metricsA.First().DataType, PayloadConverter.ConvertVersionADataType(payloadVersionA.Metrics.ElementAt(0).Type));

        Assert.AreEqual(this.seqMetricA.Name, payloadVersionA.Metrics.ElementAt(1).Name);
        Assert.AreEqual(this.seqMetricA.LongValue, payloadVersionA.Metrics.ElementAt(1).LongValue);
        Assert.AreEqual(this.seqMetricA.DataType, PayloadConverter.ConvertVersionADataType(payloadVersionA.Metrics.ElementAt(1).Type));
    }

    /// <summary>
    /// Tests the Sparkplug message generator with a node command message with a version B namespace.
    /// </summary>
    [TestMethod]
    public void TestNodeCommandMessageNamespaceB()
    {
        var dateTime = DateTimeOffset.Now;
        var message = SparkplugMessageGenerator.GetSparkPlugNodeCommandMessage(SparkplugNamespace.VersionB, "group1", "edge1", this.metricsB, 0, 1, dateTime, true);
        var payloadVersionB = PayloadHelper.Deserialize<VersionBProtoBufPayload>(message.Payload);

        Assert.AreEqual("spBv1.0/group1/NCMD/edge1", message.Topic);
        Assert.IsNotNull(payloadVersionB);
        Assert.AreEqual((ulong)dateTime.ToUnixTimeMilliseconds(), payloadVersionB.Timestamp);
        Assert.AreEqual(2, payloadVersionB.Metrics.Count);

        Assert.AreEqual(this.metricsB.First().Name, payloadVersionB.Metrics.ElementAt(0).Name);
        Assert.AreEqual(this.metricsB.First().IntValue, payloadVersionB.Metrics.ElementAt(0).IntValue);
        Assert.AreEqual(this.metricsB.First().ValueCase, payloadVersionB.Metrics.ElementAt(0).Datatype);

        Assert.AreEqual(this.seqMetricB.Name, payloadVersionB.Metrics.ElementAt(1).Name);
        Assert.AreEqual(this.seqMetricB.ULongValue, payloadVersionB.Metrics.ElementAt(1).UlongValue);
        Assert.AreEqual(this.seqMetricB.ValueCase, payloadVersionB.Metrics.ElementAt(1).Datatype);
    }


    /// <summary>
    /// Tests the Sparkplug message generator with a node command message with a version B namespace.
    /// </summary>
    [TestMethod]
    public void TestSignedDataTypesConsistency()
    {
        var dateTime = DateTimeOffset.Now;
        List<VersionB.Data.Metric> metrics = new()
        {
            new VersionB.Data.Metric("Int8", VersionB.Data.DataType.Int8, (sbyte)-1),
            new VersionB.Data.Metric("Int16", VersionB.Data.DataType.Int16, (short)-1),
            new VersionB.Data.Metric("Int32", VersionB.Data.DataType.Int32, -1),
            new VersionB.Data.Metric("Int64", VersionB.Data.DataType.Int64, -1L),
        };

        var message = SparkplugMessageGenerator.GetSparkPlugNodeCommandMessage(SparkplugNamespace.VersionB, "group1", "edge1", metrics, 0, 1, dateTime, true);
        var payloadVersionB = PayloadHelper.Deserialize<VersionBProtoBufPayload>(message.Payload);

        Assert.AreEqual("spBv1.0/group1/NCMD/edge1", message.Topic);
        Assert.IsNotNull(payloadVersionB);
        Assert.AreEqual((ulong)dateTime.ToUnixTimeMilliseconds(), payloadVersionB.Timestamp);
        Assert.AreEqual(metrics.Count + 1, payloadVersionB.Metrics.Count);

        VersionB.Data.Metric metric = metrics[0];
        VersionBProtoBufPayload.Metric protobufMetric = payloadVersionB.Metrics[0];
        Assert.AreEqual(metric.IntValue, protobufMetric.IntValue);
        metric = metrics[1];
        protobufMetric = payloadVersionB.Metrics[1];
        Assert.AreEqual(metric.IntValue, protobufMetric.IntValue);
        metric = metrics[2];
        protobufMetric = payloadVersionB.Metrics[2];
        Assert.AreEqual(metric.IntValue, protobufMetric.IntValue);
        metric = metrics[3];
        protobufMetric = payloadVersionB.Metrics[3];
        Assert.AreEqual(metric.LongValue, protobufMetric.LongValue);
    }
}
