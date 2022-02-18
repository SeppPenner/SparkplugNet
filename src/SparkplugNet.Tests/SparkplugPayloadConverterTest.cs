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
    /// Tests the Sparkplug payload converter for converting a version A payload from Proto.
    /// </summary>
    [TestMethod]
    public void TestConvertVersionAPayloadFromProto()
    {
        var payload = PayloadConverter.ConvertVersionAPayload(new VersionA.ProtoBuf.ProtoBufPayload());
        Assert.IsNotNull(payload);
    }

    /// <summary>
    /// Tests the Sparkplug payload converter for converting a version A payload to Proto.
    /// </summary>
    [TestMethod]
    public void TestConvertVersionAPayloadToProto()
    {
        var payload = PayloadConverter.ConvertVersionAPayload(new VersionA.Data.Payload());
        Assert.IsNotNull(payload);
    }

    /// <summary>
    /// Tests the Sparkplug payload converter for converting a version B payload from Proto.
    /// </summary>
    [TestMethod]
    public void TestConvertVersionBPayloadFromProto()
    {
        var payload = PayloadConverter.ConvertVersionBPayload(new VersionB.ProtoBuf.ProtoBufPayload());
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
    /// Tests the Sparkplug payload converter for converting a version A data type from Proto.
    /// </summary>
    [TestMethod]
    public void TestConvertVersionADataTypeFromProto()
    {
        var payload = PayloadConverter.ConvertVersionADataType(new VersionA.ProtoBuf.ProtoBufPayload.KuraMetric.ValueType());
        Assert.IsNotNull(payload);
    }

    /// <summary>
    /// Tests the Sparkplug payload converter for converting a version A data type to Proto.
    /// </summary>
    [TestMethod]
    public void TestConvertVersionADataTypeToProto()
    {
        var payload = PayloadConverter.ConvertVersionADataType(new VersionA.Data.DataType());
        Assert.IsNotNull(payload);
    }

    /// <summary>
    /// Tests the Sparkplug payload converter for converting a version B data type data set value from Proto.
    /// </summary>
    [TestMethod]
    public void TestConvertVersionBDataTypeDataSetValueFromProto()
    {
        var payload = PayloadConverter.ConvertVersionBDataTypeDataSetValue(new VersionB.ProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase());
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
        var payload = PayloadConverter.ConvertVersionBDataTypeMetric(new VersionB.ProtoBuf.ProtoBufPayload.Metric.ValueOneofCase());
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
        var payload = PayloadConverter.ConvertVersionBDataTypeParameter(new VersionB.ProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase());
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
        var payload = PayloadConverter.ConvertVersionBDataTypePropertyValue(new VersionB.ProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase());
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
}
