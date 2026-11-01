// -------------------------------------------------------------------------------------------------
// <copyright file="SparkplugApplicationStateTests.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class to test the node and device states of the version B application.
// </summary>
// -------------------------------------------------------------------------------------------------

namespace SparkplugNet.Tests.Application;

/// <summary>
/// A class to test the node and device states of the version B application.
/// </summary>
[TestClass]
public sealed class SparkplugApplicationStateTests
{
    /// <summary>
    /// Tests that a node goes offline on a node death message.
    /// </summary>
    [TestMethod]
    public async Task TestNodeDeathSetsTheNodeOffline()
    {
        var application = new VersionBMain.SparkplugApplication([], SparkplugSpecificationVersion.Version30);
        await Receive(application, "spBv1.0/group/NBIRTH/node");
        await Receive(application, "spBv1.0/group/NDEATH/node");

        Assert.AreEqual(SparkplugMetricStatus.Offline, application.NodeStates["node"].MetricStatus);
    }

    /// <summary>
    /// Tests that a device goes offline on a device death message.
    /// </summary>
    [TestMethod]
    public async Task TestDeviceDeathSetsTheDeviceOffline()
    {
        var application = new VersionBMain.SparkplugApplication([], SparkplugSpecificationVersion.Version30);
        await Receive(application, "spBv1.0/group/DBIRTH/node/device");
        await Receive(application, "spBv1.0/group/DDEATH/node/device");

        Assert.AreEqual(SparkplugMetricStatus.Offline, application.DeviceStates["node/device"].MetricStatus);
    }

    /// <summary>
    /// Hands a message to the application as if it came from the broker.
    /// </summary>
    /// <param name="application">The application.</param>
    /// <param name="topic">The topic.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    private static async Task Receive(VersionBMain.SparkplugApplication application, string topic)
    {
        Assert.IsTrue(SparkplugMessageTopic.TryParse(topic, out var parsedTopic));
        var payload = PayloadHelper.Serialize(new VersionBProtoBufPayload { Seq = 0 });
        var method = typeof(VersionBMain.SparkplugApplication).GetMethod("OnMessageReceived", BindingFlags.Instance | BindingFlags.NonPublic);
        await (Task)method!.Invoke(application, [parsedTopic, payload])!;
    }
}
