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
            GroupIdentifier,
            EdgeNodeIdentifier,
            TimeSpan.FromSeconds(30),
            null,
            null,
            this.cancellationTokenSource.Token);
        metrics = GetTestMetrics();

        // Create and start new instance of a Sparkplug node.
        node = new SparkplugNode(metrics, Log.Logger);
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
        var unixNow = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        var testMetrics = new List<Metric>
        {
            new () { Name = "General/Name", Timestamp = unixNow, ValueCase = (uint)DataType.String, StringValue = "Some Name" },
            new () { Name = "General/Some Int Value", Timestamp = unixNow, ValueCase = (uint)DataType.Int64, LongValue = random.Next(0, int.MaxValue) },
            new () { Name = "General/Aggregates/Some Int Value", Timestamp = unixNow, ValueCase = (uint)DataType.Int64, LongValue = random.Next(0, int.MaxValue) }
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
        var unixUtcNow = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        // Add extra metric after NBIRTH.
        newMetrics.Add(new Metric
        {
            Name = "General/Extra Metric",
            Timestamp = unixUtcNow,
            ValueCase = (uint)DataType.Int64,
            LongValue = random.Next(0, int.MaxValue)
        });

        foreach (var metric in newMetrics)
        {
            if (metric.Name == Constants.SessionNumberMetricName)
            {
                return;
            }

            metric.Timestamp = unixUtcNow;
            switch (metric.ValueCase)
            {
                case (int)DataType.String:
                case (int)DataType.Text:
                case (int)DataType.Uuid:
                    metric.StringValue = metric.StringValue;
                    break;
                case (int)DataType.Int8:
                case (int)DataType.UInt8:
                case (int)DataType.Int16:
                case (int)DataType.UInt16:
                case (int)DataType.Int32:
                case (int)DataType.UInt32:
                    metric.IntValue = random.Next(0, int.MaxValue);
                    break;
                case (int)DataType.Int64:
                case (int)DataType.UInt64:
                case (int)DataType.DateTime:
                    metric.LongValue = random.Next(0, int.MaxValue);
                    break;
                case (int)DataType.Float:
                    metric.FloatValue = random.Next(0, int.MaxValue);
                    break;
                case (int)DataType.Double:
                    metric.DoubleValue = random.Next(0, int.MaxValue);
                    break;
                case (int)DataType.Boolean:
                    metric.BooleanValue = !metric.BooleanValue;
                    break;
                case (int)DataType.Bytes:
                case (int)DataType.File:
                    metric.BytesValue = metric.BytesValue;
                    break;
                case (int)DataType.DataSet:
                    metric.DataSetValue = metric.DataSetValue;
                    break;
                case (int)DataType.Template:
                    metric.TemplateValue = metric.TemplateValue;
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
