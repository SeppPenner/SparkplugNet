// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugApplicationMetricStateTests.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   Tests the host's received metric state.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Tests.Messages;

/// <summary>
/// Tests the host's received metric state.
/// </summary>
[TestClass]
public sealed class SparkplugApplicationMetricStateTests
{
    /// <summary>
    /// Tests retaining unchanged values regardless of whether the application has event handlers.
    /// </summary>
    /// <param name="isDevice">Whether to send device or node messages.</param>
    /// <param name="handleEvents">Whether to subscribe to birth and data events.</param>
    [TestMethod]
    [DataRow(false, false)]
    [DataRow(false, true)]
    [DataRow(true, false)]
    [DataRow(true, true)]
    public async Task TestPartialUpdatesRetainOtherMetrics(bool isDevice, bool handleEvents)
    {
        var speed = new VersionBData.Metric("Speed", VersionBData.DataType.Int32, 10);
        var pressure = new VersionBData.Metric("Pressure", VersionBData.DataType.Int32, 20);
        var updatedSpeed = new VersionBData.Metric("Speed", VersionBData.DataType.Int32, 11);
        var application = new VersionBMain.SparkplugApplication([speed, pressure], SparkplugSpecificationVersion.Version30);
        var states = isDevice ? application.DeviceStates : application.NodeStates;
        var key = isDevice ? "edge/device" : "edge";
        var received = new List<string[]>();
        if (handleEvents)
        {
            application.NodeBirthReceived += args => Record(args.Metrics);
            application.DeviceBirthReceived += args => Record(args.Metrics);
            application.NodeDataReceived += args => Record(args.Metrics);
            application.DeviceDataReceived += args => Record(args.Metrics);
        }

        var birthTopic = SparkplugMessageTopic.Parse(isDevice ? "spBv1.0/group/DBIRTH/edge/device" : "spBv1.0/group/NBIRTH/edge");
        var dataTopic = SparkplugMessageTopic.Parse(isDevice ? "spBv1.0/group/DDATA/edge/device" : "spBv1.0/group/NDATA/edge");
        await application.HandleMessagesForVersionB(birthTopic, new VersionBData.Payload { Metrics = [speed, pressure] });
        await application.HandleMessagesForVersionB(dataTopic, new VersionBData.Payload { Metrics = [updatedSpeed] });

        var state = states[key];
        Assert.AreEqual(SparkplugMetricStatus.Online, state.MetricStatus);
        Assert.AreEqual(2, state.Metrics.Count);
        Assert.AreSame(updatedSpeed, state.Metrics["Speed"]);
        Assert.AreSame(pressure, state.Metrics["Pressure"]);
        if (handleEvents)
        {
            Assert.AreEqual(2, received.Count);
            CollectionAssert.AreEqual(new[] { "Speed", "Pressure" }, received[0]);
            CollectionAssert.AreEqual(new[] { "Speed" }, received[1]);
        }

        Task Record(IEnumerable<VersionBData.Metric> metrics)
        {
            var state = states[key];
            Assert.AreSame(received.Count == 0 ? speed : updatedSpeed, state.Metrics["Speed"]);
            Assert.AreSame(pressure, state.Metrics["Pressure"]);
            received.Add(metrics.Select(metric => metric.Name).ToArray());
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Tests applying an alias-only update to the metric name announced in the birth.
    /// </summary>
    [TestMethod]
    public async Task TestAliasOnlyDataUpdatesNamedMetric()
    {
        var speed = new VersionBData.Metric("Speed", VersionBData.DataType.Int32, 10) { Alias = 1 };
        var pressure = new VersionBData.Metric("Pressure", VersionBData.DataType.Int32, 20) { Alias = 2 };
        var updatedSpeed = new VersionBData.Metric(VersionBData.DataType.Int32, 11) { Alias = 1 };
        var latestSpeed = new VersionBData.Metric(VersionBData.DataType.Int32, 12) { Alias = 1 };
        var application = new VersionBMain.SparkplugApplication([speed, pressure, updatedSpeed], SparkplugSpecificationVersion.Version30);
        var birth = SparkplugMessageTopic.Parse("spBv1.0/group/NBIRTH/edge");
        var data = SparkplugMessageTopic.Parse("spBv1.0/group/NDATA/edge");

        await application.HandleMessagesForVersionB(birth, new VersionBData.Payload { Metrics = [speed, pressure] });
        await application.HandleMessagesForVersionB(data, new VersionBData.Payload { Metrics = [updatedSpeed] });
        await application.HandleMessagesForVersionB(data, new VersionBData.Payload { Metrics = [latestSpeed] });

        var metrics = application.NodeStates["edge"].Metrics;
        Assert.AreEqual(2, metrics.Count);
        Assert.AreSame(latestSpeed, metrics["Speed"]);
        Assert.AreSame(pressure, metrics["Pressure"]);
    }

    /// <summary>
    /// Tests marking a cached state offline and replacing its metrics on a new birth.
    /// </summary>
    /// <param name="isDevice">Whether to send device or node messages.</param>
    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task TestDeathAndRebirthUpdateStateWithoutEventHandlers(bool isDevice)
    {
        var speed = new VersionBData.Metric("Speed", VersionBData.DataType.Int32, 10);
        var pressure = new VersionBData.Metric("Pressure", VersionBData.DataType.Int32, 20);
        var application = new VersionBMain.SparkplugApplication([speed, pressure], SparkplugSpecificationVersion.Version30);
        var birth = SparkplugMessageTopic.Parse(isDevice ? "spBv1.0/group/DBIRTH/edge/device" : "spBv1.0/group/NBIRTH/edge");
        var death = SparkplugMessageTopic.Parse(isDevice ? "spBv1.0/group/DDEATH/edge/device" : "spBv1.0/group/NDEATH/edge");
        var states = isDevice ? application.DeviceStates : application.NodeStates;
        var key = isDevice ? "edge/device" : "edge";

        await application.HandleMessagesForVersionB(birth, new VersionBData.Payload { Metrics = [speed, pressure] });
        await application.HandleMessagesForVersionB(death, new VersionBData.Payload());

        var offline = states[key];
        Assert.AreEqual(SparkplugMetricStatus.Offline, offline.MetricStatus);
        Assert.AreEqual(2, offline.Metrics.Count);

        await application.HandleMessagesForVersionB(birth, new VersionBData.Payload { Metrics = [speed] });

        var reborn = states[key];
        Assert.AreEqual(SparkplugMetricStatus.Online, reborn.MetricStatus);
        Assert.AreSame(speed, reborn.Metrics.Values.Single());
    }
}
