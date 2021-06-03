// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugNodeSequentialTest.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class to test the <see cref="SparkplugNode"/> class with live MQTT Server and Sparkplug Host.
//   These tests are designed to execute synchronously in alphabetic order. (e.g. T1_xxx, T2_yyy, T3_zzz).
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Serilog;

    using SparkplugNet.Core;
    using SparkplugNet.Core.Enumerations;
    using SparkplugNet.Core.Node;
    using SparkplugNet.VersionB;

    /// <summary>
    /// A class to test the <see cref="SparkplugNode" /> class with live MQTT Server and Sparkplug Host.
    /// These tests are designed to execute synchronously in alphabetic order. (e.g. T1_xxx, T2_yyy, T3_zzz)
    /// </summary>
    [TestClass]
    // ReSharper disable once StyleCop.SA1650
    public class SparkplugNodeSequentialTest
    {
        /// <summary>
        /// The node.
        /// </summary>
        private static SparkplugNode node;

        /// <summary>
        /// The metrics.
        /// </summary>
        private static List<Payload.Metric> metrics;

        /// <summary>
        /// The cancellation token source.
        /// </summary>
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        /// <summary>
        /// Tests the Sparkplug CONNECT requirements (NDEATH, NBIRTH).
        /// </summary>
        [TestMethod]
        [Ignore]
        public async Task T1TestNodeVersionBConnectBirth()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            var userName = "username";
            var password = "password";
            var groupIdentifier = "group1";
            var edgeNodeIdentifier = "node1";
            var nodeOptions = new SparkplugNodeOptions(
                MqttServerUnderTest.ServerAddress,
                MqttServerUnderTest.ServerPort,
                MqttServerUnderTest.ClientId,
                userName,
                password,
                false,
                MqttServerUnderTest.ScadaHostIdentifier,
                groupIdentifier,
                edgeNodeIdentifier,
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
        [TestMethod] 
        [Ignore]
        public async Task T2TestNodeVersionBPublishMetrics()
        {
            // Publish metrics with changes.
            for (var i = 0; i < 5; i++)
            {
                await Task.Delay(1000);
                UpdateTestMetrics(metrics);
                var result = await node.PublishMetrics(metrics);
                Assert.IsTrue(result.ReasonCode == 0);
            }
        }

        /// <summary>
        /// Tests MQTT client disconnect.
        /// </summary>
        [TestMethod]
        [Ignore]
        public async Task T3TestNodeVersionBStopDisconnect()
        {
            // Assert IsConnected == true.
            Assert.IsTrue(node.IsConnected);

            // Stop instance of SparkplugNode
            await node.Stop();

            // assert IsConnected = false
            Assert.IsFalse(node.IsConnected);
        }

        /// <summary>
        /// Gets the test metrics.
        /// </summary>
        /// <returns>A <see cref="List{T}"/> of <see cref="Payload.Metric"/>s.</returns>
        private static List<Payload.Metric> GetTestMetrics()
        {
            var random = new Random();
            var unixNow = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            var testMetrics = new List<Payload.Metric>
            {
                new Payload.Metric { Name = "General/Name", Timestamp = unixNow, Datatype = (uint)SparkplugBDataType.String, StringValue = "Some Name" },
                new Payload.Metric { Name = "General/Some Int Value", Timestamp = unixNow, Datatype = (uint)SparkplugBDataType.Int64, LongValue = (ulong)random.Next(0, int.MaxValue) },
                new Payload.Metric { Name = "General/Aggregates/Some Int Value", Timestamp = unixNow, Datatype = (uint)SparkplugBDataType.Int64, LongValue = (ulong)random.Next(0, int.MaxValue) },
            };

            return testMetrics;
        }

        /// <summary>
        /// Updates the test metrics.
        /// </summary>
        /// <param name="newMetrics">The new metrics.</param>
        private static void UpdateTestMetrics(List<Payload.Metric> newMetrics)
        {
            var random = new Random();
            var unixUtcNow = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            // add extra metric after NBIRTH
            newMetrics.Add(new Payload.Metric
            {
                Name = "General/Extra Metric",
                Timestamp = unixUtcNow,
                Datatype = (uint)Payload.Metric.ValueOneofCase.LongValue,
                LongValue = (ulong)random.Next(0, int.MaxValue)
            });

            foreach (var metric in newMetrics)
            {
                if (metric.Name == Constants.SessionNumberMetricName)
                {
                    return;
                }

                metric.Timestamp = unixUtcNow;
                switch (metric.Datatype)
                {
                    case (int)SparkplugBDataType.String:
                    case (int)SparkplugBDataType.Text:
                    case (int)SparkplugBDataType.Uuid:
                        metric.StringValue = metric.StringValue;
                        break;
                    case (int)SparkplugBDataType.Int8:
                    case (int)SparkplugBDataType.UInt8:
                    case (int)SparkplugBDataType.Int16:
                    case (int)SparkplugBDataType.UInt16:
                    case (int)SparkplugBDataType.Int32:
                    case (int)SparkplugBDataType.UInt32:
                        metric.IntValue = (uint)random.Next(0, int.MaxValue);
                        break;
                    case (int)SparkplugBDataType.Int64:
                    case (int)SparkplugBDataType.UInt64:
                    case (int)SparkplugBDataType.DateTime:
                        metric.LongValue = (ulong)random.Next(0, int.MaxValue);
                        break;
                    case (int)SparkplugBDataType.Float:
                        metric.FloatValue = random.Next(0, int.MaxValue);
                        break;
                    case (int)SparkplugBDataType.Double:
                        metric.DoubleValue = random.Next(0, int.MaxValue);
                        break;
                    case (int)SparkplugBDataType.Boolean:
                        metric.BooleanValue = !metric.BooleanValue;
                        break;
                    case (int)SparkplugBDataType.Bytes:
                    case (int)SparkplugBDataType.File:
                        metric.BytesValue = metric.BytesValue;
                        break;
                    case (int)SparkplugBDataType.Dataset:
                        metric.DatasetValue = metric.DatasetValue;
                        break;
                    case (int)SparkplugBDataType.Template:
                        metric.TemplateValue = metric.TemplateValue;
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
        }
    }
}