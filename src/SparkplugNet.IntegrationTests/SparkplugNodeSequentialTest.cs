// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugNodeSequentialTest.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class to test the <see cref="SparkplugNode"/> class with live MQTT Server and Sparkplug Host.
//   These tests are designed to execute synchronously in alphabetic order. (e.g. T1_xxx, T2_yyy, T3_zzz).
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.IntegrationTests;

/// <summary>
/// A class to test the <see cref="SparkplugNode" /> class with live MQTT Server and Sparkplug Host.
/// These tests are designed to execute synchronously in alphabetic order. (e.g. T1_xxx, T2_yyy, T3_zzz)
/// </summary>
[TestClass]
public class SparkplugNodeSequentialTest
{
    /// <summary>
    /// The node.
    /// </summary>
    private static SparkplugNode? node;

    /// <summary>
    /// The metrics.
    /// </summary>
    private static List<Metric> metrics = new();

    /// <summary>
    /// The cancellation token source.
    /// </summary>
    private readonly CancellationTokenSource cancellationTokenSource = new();

    /// <summary>
    /// Tests the Sparkplug CONNECT requirements (NDEATH, NBIRTH).
    /// </summary>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    [TestMethod]
    [Ignore]
    public async Task T1TestNodeVersionBConnectBirth()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        const string? UserName = "username";
        const string Password = "password";
        const string? GroupIdentifier = "group1";
        const string? EdgeNodeIdentifier = "node1";
        var nodeOptions = new SparkplugNodeOptions(
            MqttServerUnderTest.ServerAddress,
            MqttServerUnderTest.ServerPort,
            MqttServerUnderTest.ClientId,
            UserName,
            Password,
            false,
            MqttServerUnderTest.ScadaHostIdentifier,
            TimeSpan.FromSeconds(30),
            SparkplugMqttProtocolVersion.V311,
            null,
            null,
            null,
            GroupIdentifier,
            EdgeNodeIdentifier,
            this.cancellationTokenSource.Token);
        metrics = GetTestMetrics();

        // Create and start new instance of a Sparkplug node.
        node = new SparkplugNode(metrics, SparkplugSpecificationVersion.Version22, Log.Logger);
        await node.Start(nodeOptions);
        Assert.IsTrue(node.IsConnected);
    }

    /// <summary>
    /// Tests the publishing of Sparkplug metrics (NDATA).
    /// </summary>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    [TestMethod]
    [Ignore]
    public async Task T2TestNodeVersionBPublishMetrics()
    {
        // Publish metrics with changes.
        for (var i = 0; i < 5; i++)
        {
            await Task.Delay(1000);
            UpdateTestMetrics(metrics);

            // Assert that the node is not null.
            Assert.IsNotNull(node);

            var result = await node.PublishMetrics(metrics);
            Assert.IsTrue(result.ReasonCode == 0);
        }
    }

    /// <summary>
    /// Tests MQTT client disconnect.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    [TestMethod]
    [Ignore]
    public async Task T3TestNodeVersionBStopDisconnect()
    {
        // Assert that the node is not null.
        Assert.IsNotNull(node);

        // Assert IsConnected == true.
        Assert.IsTrue(node.IsConnected);

        // Stop the instance of the Sparkplug node.
        await node.Stop();

        // Assert IsConnected = false.
        Assert.IsFalse(node.IsConnected);
    }

    /// <summary>
    /// Gets the test metrics.
    /// </summary>
    /// <returns>A <see cref="List{T}"/> of <see cref="Metric"/>s.</returns>
    private static List<Metric> GetTestMetrics()
    {
        var random = new Random();
        var utcNow = DateTimeOffset.UtcNow;

        var testMetrics = new List<Metric>
        {
            new ("General/Name", DataType.String, "Some Name", utcNow),
            new ("General/Some Int Value", DataType.UInt32, (uint)random.Next(0, int.MaxValue), utcNow),
            new ("General/Aggregates/Some Int Value", DataType.Int64, (long)random.Next(0, int.MaxValue), utcNow)
        };

        return testMetrics;
    }

    /// <summary>
    /// Updates the test metrics.
    /// </summary>
    /// <param name="newMetrics">The new metrics.</param>
    private static void UpdateTestMetrics(ICollection<Metric> newMetrics)
    {
        var random = new Random();
        var utcNow = DateTimeOffset.UtcNow;

        // Add extra metric after NBIRTH.
        newMetrics.Add(new Metric("General/Extra Metric", DataType.Int64, (long)random.Next(0, int.MaxValue), utcNow));

        foreach (var metric in newMetrics)
        {
            if (metric.Name == Constants.SessionNumberMetricName)
            {
                return;
            }

            metric.Timestamp = (ulong)utcNow.ToUnixTimeMilliseconds();

            switch (metric.DataType)
            {
                case DataType.String:
                    metric.SetValue(DataType.String, metric.StringValue);
                    break;
                case DataType.Text:
                    metric.SetValue(DataType.Text, metric.StringValue);
                    break;
                case DataType.Uuid:
                    metric.SetValue(DataType.Uuid, metric.StringValue);
                    break;
                case DataType.Int8:
                    metric.SetValue(DataType.Int8, random.Next(0, int.MaxValue));
                    break;
                case DataType.UInt8:
                    metric.SetValue(DataType.UInt8, random.Next(0, int.MaxValue));
                    break;
                case DataType.Int16:
                    metric.SetValue(DataType.Int16, random.Next(0, int.MaxValue));
                    break;
                case DataType.UInt16:
                    metric.SetValue(DataType.UInt16, random.Next(0, int.MaxValue));
                    break;
                case DataType.Int32:
                    metric.SetValue(DataType.Int32, random.Next(0, int.MaxValue));
                    break;
                case DataType.UInt32:
                    metric.SetValue(DataType.UInt32, random.Next(0, int.MaxValue));
                    break;
                case DataType.Int64:
                    metric.SetValue(DataType.Int64, random.Next(0, int.MaxValue));
                    break;
                case DataType.UInt64:
                    metric.SetValue(DataType.UInt64, (ulong)random.Next(0, int.MaxValue));
                    break;
                case DataType.DateTime:
                    metric.SetValue(DataType.DateTime, (ulong)random.Next(0, int.MaxValue));
                    break;
                case DataType.Float:
                    metric.SetValue(DataType.Float, (float)random.Next(0, int.MaxValue));
                    break;
                case DataType.Double:
                    metric.SetValue(DataType.Double, (double)random.Next(0, int.MaxValue));
                    break;
                case DataType.Boolean:
                    metric.SetValue(DataType.Boolean, !metric.BooleanValue);
                    break;
                case DataType.Bytes:
                    metric.SetValue(DataType.Bytes, metric.BytesValue);
                    break;
                case DataType.File:
                    metric.SetValue(DataType.File, metric.BytesValue);
                    break;
                case DataType.DataSet:
                    metric.SetValue(DataType.DataSet, metric.DataSetValue);
                    break;
                case DataType.Template:
                    metric.SetValue(DataType.Template, metric.TemplateValue);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
