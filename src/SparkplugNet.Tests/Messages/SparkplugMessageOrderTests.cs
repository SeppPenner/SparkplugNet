// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugMessageOrderTests.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   Tests host-side message reordering.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Tests.Messages;

/// <summary>
/// Tests host-side message reordering.
/// </summary>
[TestClass]
public sealed class SparkplugMessageOrderTests
{
    /// <summary>
    /// Delivers gaps in order across wraparound and ignores duplicate sequence numbers.
    /// </summary>
    [TestMethod]
    public async Task TestOrderedDeliveryAcrossWraparound()
    {
        var order = new VersionBMain.SparkplugMessageOrder();
        var delivered = new List<ulong?>();

        await Receive("NBIRTH", 254);
        await Receive("NDATA", 0);
        CollectionAssert.AreEqual(new ulong?[] { 254 }, delivered);
        await Receive("DBIRTH", 255, "/device");
        await Receive("NDATA", 0);
        CollectionAssert.AreEqual(new ulong?[] { 254, 255, 0 }, delivered);
        order.Reset();

        Task Receive(string verb, ulong sequence, string suffix = "") =>
            order.Receive(SparkplugMessageTopic.Parse($"spBv1.0/group/{verb}/edge{suffix}"), new() { Seq = sequence },
                TimeSpan.FromSeconds(1), (_, payload) =>
                {
                    delivered.Add(payload.Seq);
                    return Task.CompletedTask;
                }, _ => throw new AssertFailedException("A filled gap must not request rebirth."));
    }

    /// <summary>
    /// Rebirth is requested once on a timeout, and a new birth restores delivery.
    /// </summary>
    [TestMethod]
    public async Task TestTimeoutRequestsRebirthAndWaitsForBirth()
    {
        var order = new VersionBMain.SparkplugMessageOrder();
        var rebirth = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var delivered = new List<ulong?>();
        var requestCount = 0;

        await Receive("NBIRTH", 0);
        await Receive("NDATA", 2);
        await rebirth.Task.WaitAsync(TimeSpan.FromSeconds(5));
        await Receive("NDATA", 3);
        await Receive("NBIRTH", 0);
        await Receive("NDATA", 1);
        Assert.AreEqual(1, requestCount);
        CollectionAssert.AreEqual(new ulong?[] { 0, 0, 1 }, delivered);
        order.Reset();

        Task Receive(string verb, ulong sequence) =>
            order.Receive(SparkplugMessageTopic.Parse($"spBv1.0/group/{verb}/edge"), new() { Seq = sequence },
                TimeSpan.FromMilliseconds(50), (_, payload) =>
                {
                    delivered.Add(payload.Seq);
                    return Task.CompletedTask;
                }, _ =>
                {
                    requestCount++;
                    rebirth.TrySetResult();
                    return Task.CompletedTask;
                });
    }
}
