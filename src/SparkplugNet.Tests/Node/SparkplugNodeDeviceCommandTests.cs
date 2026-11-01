// -------------------------------------------------------------------------------------------------
// <copyright file="SparkplugNodeDeviceCommandTests.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class to test the device commands of the version B node.
// </summary>
// -------------------------------------------------------------------------------------------------

namespace SparkplugNet.Tests.Node;

/// <summary>
/// A class to test the device commands of the version B node.
/// </summary>
[TestClass]
public sealed class SparkplugNodeDeviceCommandTests
{
    /// <summary>
    /// Tests that a device command keeps a metric that only the device knows.
    /// </summary>
    [TestMethod]
    public async Task TestDeviceCommandKeepsTheMetricsOfTheDevice()
    {
        var nodeMetric = new VersionBData.Metric("NodeMetric", VersionBData.DataType.Int32, 1);
        var deviceMetric = new VersionBData.Metric("DeviceMetric", VersionBData.DataType.Int32, 1);
        var node = new VersionBMain.SparkplugNode([nodeMetric], SparkplugSpecificationVersion.Version30);
        node.KnownDevices.TryAdd("device", new SparkplugBase<VersionBData.Metric>.KnownMetricStorage([deviceMetric]));

        // The options are set by Start, which needs a broker.
        var options = new SparkplugNet.Core.Node.SparkplugNodeOptions { GroupIdentifier = "group", EdgeNodeIdentifier = "node" };
        typeof(SparkplugNet.Core.Node.SparkplugNodeBase<VersionBData.Metric>).GetProperty("Options")!.SetValue(node, options);

        var received = new List<string>();
        node.DeviceCommandReceived += args =>
        {
            received.AddRange(args.Metrics.Select(metric => metric.Name));
            return Task.CompletedTask;
        };

        Assert.IsTrue(SparkplugMessageTopic.TryParse("spBv1.0/group/DCMD/node/device", out var topic));
        var payload = PayloadHelper.Serialize(new VersionBProtoBufPayload
        {
            Seq = 0,
            Metrics =
            [
                new VersionBProtoBufPayload.Metric { Name = "DeviceMetric", DataType = (uint)VersionBProtoBuf.DataType.Int32, IntValue = 2 }
            ]
        });

        var method = typeof(VersionBMain.SparkplugNode).GetMethod("OnMessageReceived", BindingFlags.Instance | BindingFlags.NonPublic);
        await (Task)method!.Invoke(node, [topic, payload])!;

        CollectionAssert.AreEqual(new[] { "DeviceMetric" }, received);
    }
}
