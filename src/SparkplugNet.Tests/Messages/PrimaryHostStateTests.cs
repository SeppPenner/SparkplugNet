// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PrimaryHostStateTests.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   Tests the node's primary host STATE handling.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Tests.Messages;

/// <summary>
/// Tests the node's primary host STATE handling.
/// </summary>
[TestClass]
public sealed class PrimaryHostStateTests
{
    /// <summary>
    /// Tests startup against offline, malformed, stale and wrong-host STATE messages.
    /// </summary>
    [TestMethod]
    public void TestWaitForValidOnlineState()
    {
        const string topic = "spBv1.0/STATE/host";
        var state = new PrimaryHostState(topic, SparkplugSpecificationVersion.Version30);
        var ready = state.BeginConnection();

        Assert.IsFalse(state.Observe(topic, """{"online":false,"timestamp":10}"""));
        Assert.IsFalse(state.Observe(topic, """{"online":true,"timestamp":9}"""));
        Assert.IsFalse(state.Observe(topic, """{"online":true}"""));
        Assert.IsFalse(state.Observe(topic, """{"online":"true","timestamp":12}"""));
        Assert.IsFalse(state.Observe(topic, "not JSON"));
        Assert.IsFalse(state.Observe("spBv1.0/STATE/other", """{"online":true,"timestamp":12}"""));
        Assert.IsFalse(ready.IsCompleted);
        Assert.IsFalse(state.CanPublish);

        Assert.IsFalse(state.Observe(topic, """{"online":true,"timestamp":10}"""));
        Assert.IsTrue(ready.IsCompletedSuccessfully);
        Assert.IsFalse(state.CanPublish);
        state.MarkBirthPublished();
        Assert.IsTrue(state.CanPublish);
    }

    /// <summary>
    /// Tests that only a current offline message ends the session and a new connection waits for STATE again.
    /// </summary>
    [TestMethod]
    public void TestReconnectAfterValidOfflineState()
    {
        const string topic = "spBv1.0/STATE/host";
        var state = new PrimaryHostState(topic, SparkplugSpecificationVersion.Version30);
        var ready = state.BeginConnection();
        state.Observe(topic, """{"online":true,"timestamp":10}""");
        Assert.IsTrue(ready.IsCompletedSuccessfully);
        state.MarkBirthPublished();

        Assert.IsFalse(state.Observe(topic, """{"online":false,"timestamp":9}"""));
        Assert.IsTrue(state.CanPublish);
        Assert.IsTrue(state.Observe(topic, """{"online":false,"timestamp":10}"""));
        Assert.IsFalse(state.CanPublish);
        Assert.IsFalse(state.Observe(topic, """{"online":false,"timestamp":10}"""));

        var reconnected = state.BeginConnection();
        state.Observe(topic, """{"online":true,"timestamp":9}""");
        Assert.IsFalse(reconnected.IsCompleted);
        state.Observe(topic, """{"online":true,"timestamp":11}""");
        Assert.IsTrue(reconnected.IsCompletedSuccessfully);
        Assert.IsFalse(state.CanPublish);
    }

    /// <summary>
    /// Tests the plain text STATE format and cancellation of a pending wait.
    /// </summary>
    [TestMethod]
    public void TestVersion22StateAndCanceledWait()
    {
        const string topic = "STATE/host";
        var state = new PrimaryHostState(topic, SparkplugSpecificationVersion.Version22);
        var ready = state.BeginConnection();
        state.Observe(topic, "OFFLINE");
        state.Observe(topic, """{"online":true,"timestamp":10}""");
        Assert.IsFalse(ready.IsCompleted);
        state.EndConnection();
        Assert.IsTrue(ready.IsCanceled);

        var reconnected = state.BeginConnection();
        state.Observe(topic, "ONLINE");
        Assert.IsTrue(reconnected.IsCompletedSuccessfully);
        Assert.IsTrue(state.Observe(topic, "OFFLINE"));
    }
}
