// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugNodeRebirthTests.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   Tests the metrics used in a rebirth.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Tests.Node;

/// <summary>
/// Tests the metrics used in a rebirth.
/// </summary>
[TestClass]
public sealed class SparkplugNodeRebirthTests
{
    /// <summary>
    /// Tests decoding the reserved rebirth request without a known metric or a wire datatype.
    /// </summary>
    [TestMethod]
    public void TestRebirthControlCanBeDecodedWithoutAConfiguredMetric()
    {
        var storage = new SparkplugBase<VersionBData.Metric>.KnownMetricStorage([]);
        var payload = new VersionBProtoBufPayload
        {
            Metrics = [new() { Name = Constants.RebirthMetricName, BooleanValue = true }]
        };

        var converted = VersionBMain.PayloadConverter.ConvertVersionBPayload(payload, SparkplugMessageType.NodeCommand, storage);
        var control = converted.Metrics.Single();
        Assert.IsTrue(Constants.IsRebirthControl(control));
        Assert.AreEqual(VersionBData.DataType.Boolean, control.DataType);
        Assert.AreEqual(true, control.Value);
        Assert.AreSame(control, storage.FilterMetrics(converted.Metrics, SparkplugMessageType.NodeCommand).Single());
    }

    /// <summary>
    /// Tests preserving birth definitions and unchanged values when recording data deltas.
    /// </summary>
    [TestMethod]
    public void TestCurrentValuesKeepBirthDefinitions()
    {
        var definition = new VersionBData.Metric("Temperature", VersionBData.DataType.Int32, 10)
        {
            Alias = 3,
            MetaData = new() { Description = "Room temperature" }
        };
        var unchanged = new VersionBData.Metric("Pressure", VersionBData.DataType.Int32, 20);
        var storage = new SparkplugBase<VersionBData.Metric>.KnownMetricStorage([definition, unchanged]);

        storage.UpdateCurrentValues(
        [
            new(VersionBData.DataType.Int32, null) { Alias = 3, Timestamp = 42 },
            new(VersionBData.DataType.Int32, 12) { Alias = 3, IsHistorical = true },
            new("Unknown", VersionBData.DataType.Int32, 30)
        ]);

        Assert.HasCount(2, storage.Metrics);
        Assert.AreEqual("Temperature", definition.Name);
        Assert.AreEqual(3UL, definition.Alias);
        Assert.AreEqual("Room temperature", definition.MetaData!.Description);
        Assert.AreEqual(VersionBData.DataType.Int32, definition.DataType);
        Assert.AreEqual(42UL, definition.Timestamp);
        Assert.IsTrue(definition.IsNull);
        Assert.AreEqual(20, unchanged.Value);
    }
}
