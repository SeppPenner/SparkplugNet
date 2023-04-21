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
/// A class to test the <see cref="PayloadConverter"/> class.
/// </summary>
[TestClass]
public class SparkplugPayloadConverterTest
{
    /// <summary>
    /// Tests the Sparkplug payload converter for converting a version B payload from Proto.
    /// </summary>
    [TestMethod]
    public void TestConvertVersionBPayloadFromProto()
    {
        var payload = PayloadConverter.ConvertVersionBPayload(new VersionBProtoBufPayload());
        Assert.IsNotNull(payload);
    }

    /// <summary>
    /// Tests the Sparkplug payload converter for converting a version B payload to Proto.
    /// </summary>
    [TestMethod]
    public void TestConvertVersionBPayloadToProto()
    {
        var payload = PayloadConverter.ConvertVersionBPayload(new VersionB.Data.Payload());
        Assert.IsNotNull(payload);
    }

    /// <summary>
    /// Tests the Sparkplug payload converter for converting a version B data type data set value from Proto.
    /// </summary>
    [TestMethod]
    public void TestConvertVersionBDataTypeDataSetValueFromProto()
    {
        var payload = PayloadConverter.ConvertVersionBDataTypeDataSetValue(new VersionBProtoBufPayload.DataSet.DataSetValue.ValueOneofCase());
        Assert.IsNotNull(payload);
    }

    /// <summary>
    /// Tests the Sparkplug payload converter for converting a version B data type data set value to Proto.
    /// </summary>
    [TestMethod]
    public void TestConvertVersionBDataTypeDataSetValueToProto()
    {
        var payload = PayloadConverter.ConvertVersionBDataTypeDataSetValue(new VersionB.Data.DataType());
        Assert.IsNotNull(payload);
    }

    /// <summary>
    /// Tests the Sparkplug payload converter for converting a version B data type metric from Proto.
    /// </summary>
    [TestMethod]
    public void TestConvertVersionBDataTypeMetricFromProto()
    {
        var payload = PayloadConverter.ConvertVersionBDataTypeMetric(new VersionBProtoBufPayload.Metric.ValueOneofCase());
        Assert.IsNotNull(payload);
    }

    /// <summary>
    /// Tests the Sparkplug payload converter for converting a version B data type metric to Proto.
    /// </summary>
    [TestMethod]
    public void TestConvertVersionBDataTypeMetricToProto()
    {
        var payload = PayloadConverter.ConvertVersionBDataTypeMetric(new VersionB.Data.DataType());
        Assert.IsNotNull(payload);
    }

    /// <summary>
    /// Tests the Sparkplug payload converter for converting a version B data type parameter from Proto.
    /// </summary>
    [TestMethod]
    public void TestConvertVersionBDataTypeParameterFromProto()
    {
        var payload = PayloadConverter.ConvertVersionBDataTypeParameter(new VersionBProtoBufPayload.Template.Parameter.ValueOneofCase());
        Assert.IsNotNull(payload);
    }

    /// <summary>
    /// Tests the Sparkplug payload converter for converting a version B data type parameter to Proto.
    /// </summary>
    [TestMethod]
    public void TestConvertVersionBDataTypeParameterToProto()
    {
        var payload = PayloadConverter.ConvertVersionBDataTypeParameter(new VersionB.Data.DataType());
        Assert.IsNotNull(payload);
    }

    /// <summary>
    /// Tests the Sparkplug payload converter for converting a version B data type property value from Proto.
    /// </summary>
    [TestMethod]
    public void TestConvertVersionBDataTypePropertyValueFromProto()
    {
        var payload = PayloadConverter.ConvertVersionBDataTypePropertyValue(new VersionBProtoBufPayload.PropertyValue.ValueOneofCase());
        Assert.IsNotNull(payload);
    }

    /// <summary>
    /// Tests the Sparkplug payload converter for converting a version B data type property value to Proto.
    /// </summary>
    [TestMethod]
    public void TestConvertVersionBDataTypePropertyValueToProto()
    {
        var payload = PayloadConverter.ConvertVersionBDataTypePropertyValue(new VersionB.Data.DataType());
        Assert.IsNotNull(payload);
    }

    /// <summary>
    /// Tests the Sparkplug payload converter for converting a version B data type property value to Proto with byte data in the metrics.
    /// Test code for https://github.com/SeppPenner/SparkplugNet/issues/30.
    /// </summary>
    [TestMethod]
    public void TestConvertVersionBMetricsWithByteValues()
    {
        var payload = new VersionB.Data.Payload
        {
            Metrics = new VersionB.Data.Metric[]
            {
                new VersionB.Data.Metric()
                {
                    BytesValue = new byte[] { 1, 2, 3, 4 }
                }
            }.ToList(),
            Seq = 1,
            Timestamp = (ulong)DateTimeOffset.Now.ToUnixTimeMilliseconds()
        };

        var payloadConverted = PayloadConverter.ConvertVersionBPayload(payload);

        Assert.IsNotNull(payloadConverted);
        Assert.IsNotNull(payloadConverted.Metrics);
        Assert.IsTrue(payloadConverted.Metrics.Count > 0);
        Assert.IsNotNull(payloadConverted.Metrics[0]);
        CollectionAssert.AreEqual(new byte[] { 1, 2, 3, 4 }, payloadConverted.Metrics[0].BytesValue);
        Assert.AreEqual((uint)17, payloadConverted.Metrics[0].Datatype);
    }
}
