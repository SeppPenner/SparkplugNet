// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugApplicationSourceMetricsTests.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   Tests the metric definitions learned from each Sparkplug source.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Tests.Messages;

/// <summary>
/// Tests the metric definitions learned from each Sparkplug source.
/// </summary>
[TestClass]
public sealed class SparkplugApplicationSourceMetricsTests
{
    /// <summary>
    /// Tests that sources in different groups can reuse identifiers and aliases.
    /// </summary>
    /// <param name="isDevice">Whether the sources are devices.</param>
    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task TestBirthDefinitionsAreScopedToTheSource(bool isDevice)
    {
        var application = new VersionBMain.SparkplugApplication([], SparkplugSpecificationVersion.Version30);
        var device = isDevice ? "device" : null;
        var identifier = isDevice ? "edge/device" : "edge";
        var birth = isDevice ? "DBIRTH" : "NBIRTH";
        var data = isDevice ? "DDATA" : "NDATA";

        await Receive(application, $"first/{birth}/{identifier}",
            new VersionBProtoBufPayload.Metric { Name = "Temperature", Alias = 7, DataType = (uint)VersionBData.DataType.Int32, IntValue = 10 });
        await Receive(application, $"second/{birth}/{identifier}",
            new VersionBProtoBufPayload.Metric { Name = "Run", Alias = 7, DataType = (uint)VersionBData.DataType.Boolean, BooleanValue = false });
        await Receive(application, $"first/{data}/{identifier}", new VersionBProtoBufPayload.Metric { Alias = 7, IntValue = 11 });
        await Receive(application, $"second/{data}/{identifier}", new VersionBProtoBufPayload.Metric { Alias = 7, BooleanValue = true });

        var temperature = application.MetricStatesBySource[("first", "edge", device)].Metrics["Temperature"];
        var run = application.MetricStatesBySource[("second", "edge", device)].Metrics["Run"];
        Assert.AreEqual(VersionBData.DataType.Int32, temperature.DataType);
        Assert.AreEqual(11, temperature.Value);
        Assert.AreEqual(VersionBData.DataType.Boolean, run.DataType);
        Assert.AreEqual(true, run.Value);
        Assert.HasCount(2, application.MetricStatesBySource);

        await Receive(application, "first/NDEATH/edge", new VersionBProtoBufPayload.Metric { Name = "bdSeq", DataType = (uint)VersionBData.DataType.Int64, LongValue = 1 });
        Assert.AreEqual(SparkplugMetricStatus.Offline, application.MetricStatesBySource[("first", "edge", device)].MetricStatus);
        Assert.AreEqual(SparkplugMetricStatus.Online, application.MetricStatesBySource[("second", "edge", device)].MetricStatus);
    }

    /// <summary>
    /// Tests that a later birth replaces previous alias definitions and rejects unannounced metrics.
    /// </summary>
    [TestMethod]
    public async Task TestNewBirthReplacesMetricDefinitions()
    {
        var application = new VersionBMain.SparkplugApplication([], SparkplugSpecificationVersion.Version30);
        await Receive(application, "group/NBIRTH/edge",
            new VersionBProtoBufPayload.Metric { Name = "Temperature", Alias = 7, DataType = (uint)VersionBData.DataType.Int32, IntValue = 10 },
            new VersionBProtoBufPayload.Metric { Name = "Pressure", Alias = 8, DataType = (uint)VersionBData.DataType.Int32, IntValue = 20 });
        await Receive(application, "group/NBIRTH/edge",
            new VersionBProtoBufPayload.Metric { Name = "Run", Alias = 7, DataType = (uint)VersionBData.DataType.Boolean, BooleanValue = false });
        await Receive(application, "group/NDATA/edge", new VersionBProtoBufPayload.Metric { Alias = 8, IntValue = 21 });
        await Receive(application, "group/NDATA/edge",
            new VersionBProtoBufPayload.Metric { Name = "Unexpected", DataType = (uint)VersionBData.DataType.Int32, IntValue = 1 });
        await Receive(application, "group/NDATA/edge", new VersionBProtoBufPayload.Metric { Alias = 7, BooleanValue = true });

        var metrics = application.MetricStatesBySource[("group", "edge", null)].Metrics;
        Assert.HasCount(1, metrics);
        Assert.AreEqual(true, metrics["Run"].Value);
        Assert.AreSame(application.NodeStates["edge"], application.MetricStatesBySource[("group", "edge", null)]);
    }

    /// <summary>
    /// Tests that a configured host still restricts the metrics it accepts at birth.
    /// </summary>
    /// <param name="configureByAlias">Whether the configured filter identifies the metric by alias.</param>
    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task TestConfiguredBirthFilterStillApplies(bool configureByAlias)
    {
        var metric = configureByAlias
            ? new VersionBData.Metric(VersionBData.DataType.Int32, 10) { Alias = 7 }
            : new VersionBData.Metric("Temperature", VersionBData.DataType.Int32, 10);
        var application = new VersionBMain.SparkplugApplication([metric], SparkplugSpecificationVersion.Version30);
        await Receive(application, "group/NBIRTH/edge",
            new VersionBProtoBufPayload.Metric { Name = "Temperature", Alias = 7, DataType = (uint)VersionBData.DataType.Int32, IntValue = 10 },
            new VersionBProtoBufPayload.Metric { Name = "Pressure", Alias = 8, DataType = (uint)VersionBData.DataType.Int32, IntValue = 20 });
        await Receive(application, "group/NDATA/edge", new VersionBProtoBufPayload.Metric { Alias = 7, IntValue = 11 }, new VersionBProtoBufPayload.Metric { Alias = 8, IntValue = 21 });

        var metrics = application.MetricStatesBySource[("group", "edge", null)].Metrics;
        Assert.HasCount(1, metrics);
        Assert.AreEqual(11, metrics["Temperature"].Value);
    }

    /// <summary>
    /// Delivers a serialized message through the application's normal payload decoder.
    /// </summary>
    /// <param name="application">The receiving application.</param>
    /// <param name="topicSuffix">The topic after the namespace.</param>
    /// <param name="metrics">The wire metrics.</param>
    /// <returns>The receive task.</returns>
    private static Task Receive(VersionBMain.SparkplugApplication application, string topicSuffix, params VersionBProtoBufPayload.Metric[] metrics)
    {
        var receive = typeof(VersionBMain.SparkplugApplication).GetMethod("OnMessageReceived", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var payload = PayloadHelper.Serialize(new VersionBProtoBufPayload { Metrics = [.. metrics] });
        return (Task)receive.Invoke(application, [SparkplugMessageTopic.Parse($"spBv1.0/{topicSuffix}"), payload])!;
    }
}
