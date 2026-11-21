// -------------------------------------------------------------------------------------------------
// <copyright file="SparkplugPayloadConverterUInt32Test.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class to test how the version B payload converter decodes UInt32.
// </summary>
// -------------------------------------------------------------------------------------------------

namespace SparkplugNet.Tests.Payloads;

/// <summary>
/// A class to test how the version B payload converter decodes UInt32. Sparkplug gives it the
/// protobuf type uint32, Eclipse Tahu writes it to long_value, and both are in use.
/// </summary>
[TestClass]
public sealed class SparkplugPayloadConverterUInt32Test
{
    /// <summary>
    /// Tests a metric in both fields.
    /// </summary>
    [TestMethod]
    public void TestConvertVersionBMetricReadsBothFields()
    {
        var dataType = (uint)VersionBProtoBuf.DataType.UInt32;
        var fromInt = new VersionBProtoBufPayload.Metric { Name = "M", DataType = dataType, IntValue = 7 };
        var fromLong = new VersionBProtoBufPayload.Metric { Name = "M", DataType = dataType, LongValue = 7 };

        Assert.AreEqual(7u, VersionBMain.PayloadConverter.ConvertVersionBMetric(fromInt).Value);
        Assert.AreEqual(7u, VersionBMain.PayloadConverter.ConvertVersionBMetric(fromLong).Value);
    }

    /// <summary>
    /// Tests a data set value in both fields.
    /// </summary>
    [TestMethod]
    public void TestConvertVersionBDataSetValueReadsBothFields()
    {
        var dataType = (uint)VersionBProtoBuf.DataType.UInt32;
        var fromInt = new VersionBProtoBufPayload.DataSet.DataSetValue { IntValue = 7 };
        var fromLong = new VersionBProtoBufPayload.DataSet.DataSetValue { LongValue = 7 };

        Assert.AreEqual(7u, VersionBMain.PayloadConverter.ConvertVersionBDataSetValue(fromInt, dataType).Value);
        Assert.AreEqual(7u, VersionBMain.PayloadConverter.ConvertVersionBDataSetValue(fromLong, dataType).Value);
    }

    /// <summary>
    /// Tests a template parameter in both fields.
    /// </summary>
    [TestMethod]
    public void TestConvertVersionBParameterReadsBothFields()
    {
        var dataType = (uint)VersionBProtoBuf.DataType.UInt32;
        var fromInt = new VersionBProtoBufPayload.Template.Parameter { Name = "P", DataType = dataType, IntValue = 7 };
        var fromLong = new VersionBProtoBufPayload.Template.Parameter { Name = "P", DataType = dataType, LongValue = 7 };

        Assert.AreEqual(7u, VersionBMain.PayloadConverter.ConvertVersionBParameter(fromInt).Value);
        Assert.AreEqual(7u, VersionBMain.PayloadConverter.ConvertVersionBParameter(fromLong).Value);
    }

    /// <summary>
    /// Tests a property value in both fields.
    /// </summary>
    [TestMethod]
    public void TestConvertVersionBPropertyValueReadsBothFields()
    {
        var dataType = (uint)VersionBProtoBuf.DataType.UInt32;
        var fromInt = new VersionBProtoBufPayload.PropertyValue { DataType = dataType, IntValue = 7 };
        var fromLong = new VersionBProtoBufPayload.PropertyValue { DataType = dataType, LongValue = 7 };

        Assert.AreEqual(7u, VersionBMain.PayloadConverter.ConvertVersionBPropertyValue(fromInt).Value);
        Assert.AreEqual(7u, VersionBMain.PayloadConverter.ConvertVersionBPropertyValue(fromLong).Value);
    }
}
