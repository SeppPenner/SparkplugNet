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

    /// <summary>
    /// Tests that a metric property keeps its ordered list of property sets on the wire.
    /// </summary>
    [TestMethod]
    public void TestRoundTripPropertySetList()
    {
        var limits = new VersionBData.PropertySet
        {
            Keys = ["maxRPM"],
            Values = [new VersionBData.PropertyValue(VersionBData.DataType.UInt32, 1_800u)]
        };
        var profiles = new VersionBData.PropertySetList
        {
            PropertySets =
            [
                new()
                {
                    Keys = ["engUnit"],
                    Values = [new VersionBData.PropertyValue(VersionBData.DataType.String, "rpm")]
                },
                new()
                {
                    Keys = ["Limits"],
                    Values = [new VersionBData.PropertyValue(VersionBData.DataType.PropertySet, limits)]
                }
            ]
        };
        var metric = new VersionBData.Metric("Speed", VersionBData.DataType.Int32, 42)
        {
            Properties = new VersionBData.PropertySet
            {
                Keys = ["Profiles"],
                Values = [new VersionBData.PropertyValue(VersionBData.DataType.PropertySetList, profiles)]
            }
        };

        var serialized = PayloadHelper.Serialize(VersionBMain.PayloadConverter.ConvertVersionBMetric(metric));
        var wireMetric = PayloadHelper.Deserialize<VersionBProtoBufPayload.Metric>(serialized);
        Assert.IsNotNull(wireMetric);
        Assert.IsTrue(wireMetric.PropertySetValue!.Values.Single().ShouldSerializePropertysetsValue());

        var decoded = VersionBMain.PayloadConverter.ConvertVersionBMetric(wireMetric);
        CollectionAssert.AreEqual(new[] { "Profiles" }, decoded.Properties!.Keys);
        var property = decoded.Properties.Values.Single();
        Assert.AreEqual(VersionBData.DataType.PropertySetList, property.DataType);
        Assert.IsFalse(property.IsNull);
        var decodedProfiles = property.Value as VersionBData.PropertySetList;
        Assert.IsNotNull(decodedProfiles);
        Assert.AreEqual(2, decodedProfiles.PropertySets.Count);
        Assert.AreEqual("engUnit", decodedProfiles.PropertySets[0].Keys.Single());
        Assert.AreEqual("rpm", decodedProfiles.PropertySets[0].Values.Single().Value);
        Assert.AreEqual("Limits", decodedProfiles.PropertySets[1].Keys.Single());
        var decodedLimits = decodedProfiles.PropertySets[1].Values.Single().Value as VersionBData.PropertySet;
        Assert.IsNotNull(decodedLimits);
        Assert.AreEqual("maxRPM", decodedLimits.Keys.Single());
        Assert.AreEqual(1_800u, decodedLimits.Values.Single().Value);
    }

    /// <summary>
    /// Tests that an empty list and a null list remain distinct after updating and encoding a property.
    /// </summary>
    /// <param name="isNull">Whether the property has a null value.</param>
    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void TestRoundTripEmptyOrNullPropertySetList(bool isNull)
    {
        var property = new VersionBData.PropertyValue();
        property.SetValue(VersionBData.DataType.PropertySetList, new VersionBData.PropertySetList());
        property.SetValue(VersionBData.DataType.PropertySetList, isNull ? null : new VersionBData.PropertySetList());

        var serialized = PayloadHelper.Serialize(VersionBMain.PayloadConverter.ConvertVersionBPropertyValue(property));
        var wireProperty = PayloadHelper.Deserialize<VersionBProtoBufPayload.PropertyValue>(serialized);
        Assert.IsNotNull(wireProperty);
        Assert.AreEqual(isNull, wireProperty.IsNull);
        Assert.AreEqual(!isNull, wireProperty.ShouldSerializePropertysetsValue());

        var decoded = VersionBMain.PayloadConverter.ConvertVersionBPropertyValue(wireProperty);
        Assert.AreEqual(VersionBData.DataType.PropertySetList, decoded.DataType);
        Assert.AreEqual(isNull, decoded.IsNull);
        if (isNull)
        {
            Assert.IsNull(decoded.Value);
        }
        else
        {
            var list = decoded.Value as VersionBData.PropertySetList;
            Assert.IsNotNull(list);
            Assert.IsEmpty(list.PropertySets);
        }
    }
}
