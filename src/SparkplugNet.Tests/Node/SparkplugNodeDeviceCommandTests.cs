// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugNodeDeviceCommandTests.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   Tests the metric definitions used by incoming device commands.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Tests.Node;

/// <summary>
/// Tests the metric definitions used by incoming device commands.
/// </summary>
[TestClass]
public sealed class SparkplugNodeDeviceCommandTests
{
    /// <summary>
    /// Tests that a device command keeps the target device's metrics and filters the node's metrics.
    /// </summary>
    [TestMethod]
    public async Task TestDeviceCommandUsesDeviceMetrics()
    {
        var nodeMetric = new VersionBData.Metric("NodeOnly", VersionBData.DataType.Int32, 1);
        var deviceMetric = new VersionBData.Metric("DeviceOnly", VersionBData.DataType.Int32, 1);
        var node = new VersionBMain.SparkplugNode([nodeMetric], SparkplugSpecificationVersion.Version30);
        node.KnownDevices["device"] = new([deviceMetric]);
        var received = new List<string[]>();
        node.DeviceCommandReceived += args =>
        {
            received.Add(args.Metrics.Select(metric => metric.Name).ToArray());
            return Task.CompletedTask;
        };

        var payload = new VersionBProtoBufPayload
        {
            Metrics =
            [
                new() { Name = "DeviceOnly", DataType = (uint)VersionBProtoBuf.DataType.Int32, IntValue = 2 },
                new() { Name = "NodeOnly", DataType = (uint)VersionBProtoBuf.DataType.Int32, IntValue = 3 }
            ]
        };
        await ReceiveCommand(node, "spBv1.0/group/DCMD/node/device", payload);
        Assert.HasCount(1, received);
        CollectionAssert.AreEqual(new[] { "DeviceOnly" }, received[0]);

        await ReceiveCommand(node, "spBv1.0/group/DCMD/node/unregistered", payload);
        Assert.HasCount(1, received);
    }

    /// <summary>
    /// Tests decoding an alias using the target device's type when another device uses the same alias.
    /// </summary>
    /// <param name="includeDataType">Whether the command carries the metric's data type.</param>
    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task TestDeviceCommandResolvesAliasFromTargetDevice(bool includeDataType)
    {
        const ulong alias = 7;
        var nodeMetric = new VersionBData.Metric("Node", VersionBData.DataType.Boolean, false) { Alias = alias };
        var firstMetric = new VersionBData.Metric("Temperature", VersionBData.DataType.Float, 1f) { Alias = alias };
        var secondMetric = new VersionBData.Metric("Setting", VersionBData.DataType.Int32, 1) { Alias = alias };
        var node = new VersionBMain.SparkplugNode([nodeMetric], SparkplugSpecificationVersion.Version30);
        node.KnownDevices["first"] = new([firstMetric]);
        node.KnownDevices["second"] = new([secondMetric]);
        var received = new List<VersionBData.Metric>();
        node.DeviceCommandReceived += args =>
        {
            received.AddRange(args.Metrics);
            return Task.CompletedTask;
        };

        var command = new VersionBProtoBufPayload.Metric { Alias = alias, IntValue = 25 };
        if (includeDataType)
        {
            command.DataType = (uint)VersionBProtoBuf.DataType.Int32;
        }

        await ReceiveCommand(node, "spBv1.0/group/DCMD/node/second", new VersionBProtoBufPayload { Metrics = [command] });

        Assert.HasCount(1, received);
        Assert.AreEqual(alias, received[0].Alias);
        Assert.AreEqual(string.Empty, received[0].Name);
        Assert.AreEqual(VersionBData.DataType.Int32, received[0].DataType);
        Assert.AreEqual(25, received[0].Value);
        Assert.HasCount(1, node.KnownDevices["second"].Metrics);
    }

    /// <summary>
    /// Delivers a command through the same receive method used by the MQTT client.
    /// </summary>
    /// <param name="node">The receiving node.</param>
    /// <param name="topic">The MQTT command topic.</param>
    /// <param name="payload">The command payload.</param>
    /// <returns>The receive task.</returns>
    private static Task ReceiveCommand(VersionBMain.SparkplugNode node, string topic, VersionBProtoBufPayload payload)
    {
        // Start sets these options when connecting to a broker.
        typeof(SparkplugNodeBase<VersionBData.Metric>).GetProperty(nameof(node.Options))!
            .SetValue(node, new SparkplugNodeOptions(groupIdentifier: "group", edgeNodeIdentifier: "node"));
        var receive = typeof(VersionBMain.SparkplugNode).GetMethod("OnMessageReceived", BindingFlags.Instance | BindingFlags.NonPublic)!;
        return (Task)receive.Invoke(node, [SparkplugMessageTopic.Parse(topic), PayloadHelper.Serialize(payload)])!;
    }
}
