// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugApplicationSequenceTests.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   Tests the payload sequence numbers exposed by the Sparkplug host events.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Tests.Messages;

/// <summary>
/// Tests the payload sequence numbers exposed by the Sparkplug host events.
/// </summary>
[TestClass]
public sealed class SparkplugApplicationSequenceTests
{
    /// <summary>
    /// Tests that node and device events report the sequence from the payload.
    /// </summary>
    /// <param name="topicSuffix">The message type and its edge and device identifiers.</param>
    /// <param name="sequenceNumber">The sequence number in the payload.</param>
    [TestMethod]
    [DataRow("NBIRTH/edge", 0UL)]
    [DataRow("DBIRTH/edge/device", 1UL)]
    [DataRow("NDATA/edge", 2UL)]
    [DataRow("DDATA/edge/device", 255UL)]
    [DataRow("DDEATH/edge/device", 0UL)]
    [DataRow("NDATA/edge", null)]
    public async Task TestReceivedEventIncludesPayloadSequenceNumber(string topicSuffix, ulong? sequenceNumber)
    {
        var application = new VersionBMain.SparkplugApplication([], SparkplugSpecificationVersion.Version30);
        var messages = new List<(string Topic, ulong? SequenceNumber)>();
        application.NodeBirthReceived += args => Record("NBIRTH/edge", args);
        application.DeviceBirthReceived += args => Record("DBIRTH/edge/device", args);
        application.NodeDataReceived += args => Record("NDATA/edge", args);
        application.DeviceDataReceived += args => Record("DDATA/edge/device", args);
        application.DeviceDeathReceived += args => Record("DDEATH/edge/device", args);

        var wire = new VersionBProtoBufPayload
        {
            Seq = sequenceNumber,
            Metrics =
            [
                new() { Name = Constants.SessionNumberMetricName, DataType = (uint)VersionBData.DataType.Int64, LongValue = 42 }
            ]
        };
        var received = PayloadHelper.Deserialize<VersionBProtoBufPayload>(PayloadHelper.Serialize(wire));
        Assert.IsNotNull(received);
        var topic = SparkplugMessageTopic.Parse($"spBv1.0/group/{topicSuffix}");
        await application.HandleMessagesForVersionB(topic, VersionBMain.PayloadConverter.ConvertVersionBPayload(received));

        Assert.AreEqual((topicSuffix, sequenceNumber), messages.Single());

        Task Record(string message, SparkplugBase<VersionBData.Metric>.NodeEventArgs args)
        {
            messages.Add((message, args.SequenceNumber));
            return Task.CompletedTask;
        }
    }
}
