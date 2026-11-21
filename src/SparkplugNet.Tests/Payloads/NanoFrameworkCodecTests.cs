using SparkplugNet.NanoFramework;
using NanoMetric = SparkplugNet.NanoFramework.Metric;

namespace SparkplugNet.Tests.Payloads;

/// <summary>Checks compatibility between the desktop and nanoFramework Sparkplug B codecs.</summary>
[TestClass]
public sealed class NanoFrameworkCodecTests
{
    /// <summary>Checks that encoded signed integers and explicit nulls retain their wire values.</summary>
    [TestMethod]
    public void DesktopCanReadNanoFrameworkPayload()
    {
        var signed = new NanoMetric("signed", MetricDataType.Int16, (short)-321);
        var missing = new NanoMetric("missing", MetricDataType.String, null) { Alias = 0 };

        var bytes = SparkplugBCodec.Encode(42, true, 0, [signed, missing]);
        var payload = PayloadHelper.Deserialize<VersionBProtoBufPayload>(bytes);
        Assert.IsNotNull(payload);
        Assert.AreEqual(42UL, payload.Timestamp);
        Assert.AreEqual(0UL, payload.Seq);
        Assert.HasCount(2, payload.Metrics);
        Assert.AreEqual((short)-321, VersionBMain.PayloadConverter.ConvertVersionBMetric(payload.Metrics[0]).Value);
        Assert.AreEqual(0UL, payload.Metrics[1].Alias);
        Assert.IsTrue(payload.Metrics[1].IsNull);
        Assert.IsFalse(payload.Metrics[1].ShouldSerializeStringValue());
    }

    /// <summary>Checks both wire representations of signed integers and the explicit null flag.</summary>
    [TestMethod]
    [DataRow(0xFEBFu)]
    [DataRow(0xFFFFFEBFu)]
    public void NanoFrameworkCanReadDesktopPayload(uint signedBits)
    {
        var payload = new VersionBProtoBufPayload
        {
            Timestamp = 42,
            Seq = 0,
            Metrics =
            [
                new() { Name = "signed", DataType = (uint)MetricDataType.Int16, IntValue = signedBits },
                new() { Alias = 0, DataType = (uint)MetricDataType.String, IsNull = true, StringValue = string.Empty },
                new() { Name = "empty", DataType = (uint)MetricDataType.String, StringValue = string.Empty }
            ]
        };

        var decoded = SparkplugBCodec.Decode(PayloadHelper.Serialize(payload));
        Assert.AreEqual(42UL, decoded.Timestamp);
        Assert.IsTrue(decoded.HasSequence);
        Assert.AreEqual((byte)0, decoded.Sequence);
        Assert.HasCount(3, decoded.Metrics);
        Assert.AreEqual((short)-321, decoded.Metrics[0].Value);
        Assert.IsTrue(decoded.Metrics[1].HasAlias);
        Assert.AreEqual(0UL, decoded.Metrics[1].Alias);
        Assert.IsNull(decoded.Metrics[1].Value);
        Assert.AreEqual(string.Empty, decoded.Metrics[2].Value);
    }

    /// <summary>Checks both protobuf representations of UInt32, including its largest value.</summary>
    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void NanoFrameworkCanReadUInt32FromEitherWireField(bool useLongValue)
    {
        var metric = new VersionBProtoBufPayload.Metric { Name = "counter", DataType = (uint)MetricDataType.UInt32 };
        if (useLongValue)
        {
            metric.LongValue = uint.MaxValue;
        }
        else
        {
            metric.IntValue = uint.MaxValue;
        }

        var wire = PayloadHelper.Serialize(new VersionBProtoBufPayload { Metrics = [metric] });
        Assert.AreEqual(uint.MaxValue, SparkplugBCodec.Decode(wire).Metrics.Single().Value);
    }

    /// <summary>Checks compact data encoding, timestamp fallback and resolution by both name and alias.</summary>
    [TestMethod]
    public void Sparkplug30DataUsesBirthTypesAndMetricTimestamps()
    {
        var temperature = new NanoMetric("temperature", MetricDataType.Float, 18.5f) { Alias = 0 };
        var counter = new NanoMetric("counter", MetricDataType.UInt32, 0u);
        var wire = SparkplugBCodec.Encode(42, true, 2,
        [
            new(null, MetricDataType.Float, 19.5f) { Alias = 0 },
            new("counter", MetricDataType.UInt32, null) { Timestamp = 35 }
        ], includeDataTypes: false, fillMissingTimestamps: true);

        var proto = PayloadHelper.Deserialize<VersionBProtoBufPayload>(wire);
        Assert.IsNotNull(proto);
        Assert.IsNull(proto.Metrics[0].DataType);
        Assert.AreEqual(42UL, proto.Metrics[0].Timestamp);
        Assert.IsNull(proto.Metrics[1].DataType);
        Assert.AreEqual(35UL, proto.Metrics[1].Timestamp);

        var metrics = SparkplugBCodec.Decode(wire, [temperature, counter]).Metrics;
        Assert.AreEqual(19.5f, metrics[0].Value);
        Assert.AreEqual(MetricDataType.UInt32, metrics[1].DataType);
        Assert.IsNull(metrics[1].Value);
    }

    /// <summary>Checks that commands without a datatype use the recipient's metric definitions.</summary>
    [TestMethod]
    public void Sparkplug30CommandsUseTargetDefinitionsAndReservedRebirthType()
    {
        var command = new VersionBProtoBufPayload
        {
            Metrics =
            [
                new() { Alias = 7, IntValue = 25 },
                new() { Name = "deviceOnly", DataType = (uint)MetricDataType.Int16, FloatValue = 2.5f },
                new() { Name = "otherDevice", DataType = (uint)MetricDataType.Boolean, BooleanValue = true }
            ]
        };
        var selectedDevice = new NanoMetric("deviceOnly", MetricDataType.Int32, 0) { Alias = 7 };
        var deviceWire = PayloadHelper.Serialize(command);
        var deviceMetrics = SparkplugBCodec.DecodeCommand(deviceWire, [selectedDevice], nodeCommand: false).Metrics;
        Assert.AreEqual(25, deviceMetrics.Single().Value);
        Assert.AreEqual(MetricDataType.Int32, deviceMetrics.Single().DataType);

        var rebirth = new VersionBProtoBufPayload
        {
            Metrics = [new() { Name = "Node Control/Rebirth", BooleanValue = true }]
        };
        var nodeWire = PayloadHelper.Serialize(rebirth);
        var nodeMetric = SparkplugBCodec.DecodeCommand(nodeWire, [], nodeCommand: true).Metrics.Single();
        Assert.AreEqual(MetricDataType.Boolean, nodeMetric.DataType);
        Assert.AreEqual(true, nodeMetric.Value);
        Assert.IsEmpty(SparkplugBCodec.DecodeCommand(nodeWire, [], nodeCommand: false).Metrics);
    }
}
