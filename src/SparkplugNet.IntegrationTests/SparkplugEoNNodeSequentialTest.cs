// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugEoNNodeTest.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class to test the <see cref="SparkplugNode"/> class with live MQTT Server and Sparkplug Host.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SparkplugNet.Core;
    using SparkplugNet.Core.Enumerations;
    using SparkplugNet.Core.Node;
    using SparkplugNet.VersionB;

    /// <summary>
    /// A class to test the <see cref="SparkplugNode" /> class with live MQTT Server and Sparkplug Host.
    /// These tests are designed to execute synchronously in alphabetic order. (e.g. T1_xxx, T2_yyy, T3_zzz)
    /// </summary>
    [TestClass]
    public class SparkplugEoNNodeSequentialTest
    {
        private readonly CancellationTokenSource cts = new CancellationTokenSource();
        private static SparkplugNode sut;
        private static List<Payload.Metric> metrics;

        /// <summary>
        /// Tests Sparkplug CONNECT requirements (NDEATH, NBIRTH).
        /// </summary>
        [TestMethod]
        public async Task T1_TestEoNNode_VersionB_ConnectBirth()
        {
            var userName = "username";
            var password = "password";
            var groupIdentifier = "group1";
            var edgeNodeIdentifier = "node1";
            var nodeOptions = new SparkplugNodeOptions(MqttServerUnderTest.ServerAddress, MqttServerUnderTest.ServerPort, MqttServerUnderTest.ClientId, userName,
                password, false, MqttServerUnderTest.ScadaHostIdentifier, groupIdentifier, edgeNodeIdentifier, TimeSpan.FromSeconds(30), null, null,
                this.cts.Token);
            metrics = GetTestMetrics();

            // create and start new instance of SparkplugNode
            sut = new SparkplugNode(metrics);
            await sut.Start(nodeOptions);
            Assert.IsTrue(sut.IsConnected);
        }

        /// <summary>
        /// Tests Sparkplug PublishMetrics (NDATA)
        /// </summary>
        [TestMethod]
        public async Task T2_TestEoNNode_VersionB_PublishMetrics()
        {
            // publish metrics with changes
            for (var i = 0; i < 5; i++)
            {
                await Task.Delay(1000);
                UpdateTestMetrics(metrics);
                var result = await sut.PublishMetrics(metrics);
                Assert.IsTrue(result.ReasonCode == 0);
            }
        }

        /// <summary>
        /// Tests MQTT Client Disconnect
        /// </summary>
        [TestMethod]
        public async Task T3_TestEoNNode_VersionB_StopDisconnect()
        {
            // assert IsConnected = true
            Assert.IsTrue(sut.IsConnected);

            // stop instance of SparkplugNode
            await sut.Stop();

            // assert IsConnected = false
            Assert.IsFalse(sut.IsConnected);
        }

        private static List<Payload.Metric> GetTestMetrics()
        {
            var random = new Random();
            var unixNow = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var metrics = new List<Payload.Metric>
            {
                new() { Name = "General/Name", Timestamp = unixNow, Datatype = (uint)Payload.Metric.ValueOneofCase.StringValue, StringValue = "Some Name" },
                new() { Name = "General/Some Int Value", Timestamp = unixNow, Datatype = (uint)Payload.Metric.ValueOneofCase.LongValue, LongValue = (ulong)random.Next(0, int.MaxValue) },
                new() { Name = "General/Aggregates/Some Int Value", Timestamp = unixNow, Datatype = (uint)Payload.Metric.ValueOneofCase.LongValue, LongValue = (ulong)random.Next(0, int.MaxValue) },
            };
            return metrics;
        }

        private static void UpdateTestMetrics(List<Payload.Metric> metrics)
        {
            var random = new Random();
            var unixUtcNow = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            // add extra metric after NBIRTH
            metrics.Add(new Payload.Metric()
            {
                Name = "General/Extra Metric", Timestamp = unixUtcNow, Datatype = (uint)Payload.Metric.ValueOneofCase.LongValue,
                LongValue = (ulong)random.Next(0, int.MaxValue)
            });

            foreach (var metric in metrics)
            {
                if (metric.Name == Constants.SessionNumberMetricName)
                {
                    return;
                }

                metric.Timestamp = unixUtcNow;
                switch (metric.Datatype)
                {
                    case (uint)Payload.Metric.ValueOneofCase.StringValue:
                        metric.StringValue = metric.StringValue;
                        break;
                    case (uint)Payload.Metric.ValueOneofCase.IntValue:
                        metric.IntValue = (uint)random.Next(0, int.MaxValue);
                        break;
                    case (uint)Payload.Metric.ValueOneofCase.LongValue:
                        metric.LongValue = (ulong)random.Next(0, int.MaxValue);
                        break;
                    case (uint)Payload.Metric.ValueOneofCase.FloatValue:
                        metric.FloatValue = (float)random.Next(0, int.MaxValue);
                        break;
                    case (uint)Payload.Metric.ValueOneofCase.DoubleValue:
                        metric.DoubleValue = (float)random.Next(0, int.MaxValue);
                        break;
                    case (uint)Payload.Metric.ValueOneofCase.BooleanValue:
                        metric.BooleanValue = !metric.BooleanValue;
                        break;
                    case (uint)Payload.Metric.ValueOneofCase.BytesValue:
                        metric.BytesValue = metric.BytesValue;
                        break;
                    case (uint)Payload.Metric.ValueOneofCase.DatasetValue:
                        metric.DatasetValue = metric.DatasetValue;
                        break;
                    case (uint)Payload.Metric.ValueOneofCase.TemplateValue:
                        metric.TemplateValue = metric.TemplateValue;
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
        }
    }
}