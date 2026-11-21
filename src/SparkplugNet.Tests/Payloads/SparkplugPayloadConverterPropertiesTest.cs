// -------------------------------------------------------------------------------------------------
// <copyright file="SparkplugPayloadConverterPropertiesTest.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class to test the properties of metrics in the version B payload converter.
// </summary>
// -------------------------------------------------------------------------------------------------

namespace SparkplugNet.Tests.Payloads;

/// <summary>
/// A class to test the properties of metrics in the version B payload converter.
/// </summary>
[TestClass]
public sealed class SparkplugPayloadConverterPropertiesTest
{
    /// <summary>
    /// Tests that the properties of a metric survive a round trip.
    /// </summary>
    [TestMethod]
    public void TestConvertVersionBMetricKeepsProperties()
    {
        var metric = new VersionBData.Metric("Metric", VersionBData.DataType.Int32, 1)
        {
            Properties = new VersionBData.PropertySet
            {
                Keys = ["engUnit"],
                Values = [new VersionBData.PropertyValue(VersionBData.DataType.String, "rpm")]
            }
        };

        var converted = VersionBMain.PayloadConverter.ConvertVersionBMetric(VersionBMain.PayloadConverter.ConvertVersionBMetric(metric));

        Assert.IsNotNull(converted.Properties);
        CollectionAssert.AreEqual(new[] { "engUnit" }, converted.Properties.Keys);
        Assert.AreEqual("rpm", converted.Properties.Values[0].Value);
    }
}
