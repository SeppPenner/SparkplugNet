// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugDeviceSe.quentialTest.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license
// </copyright>
// <summary>
//   A class to test the <see cref="SparkplugNode"/> class with live MQTT Server and Sparkplug Host.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.IntegrationTests.SparkplugB
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SparkplugNet.Core;
    using SparkplugNet.Core.Device;
    using SparkplugNet.Core.Enumerations;
    using SparkplugNet.Core.Node;
    using SparkplugNet.VersionB;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// A class to test the <see cref="SparkplugNode" /> class with live MQTT Server and Sparkplug Host.
    /// These tests are designed to execute synchronously in alphabetic order. (e.g. T1_xxx, T2_yyy, T3_zzz)
    /// </summary>
    [TestClass]
    public class SparkplugDeviceSequentialTest
    {
        private static SparkplugNode nodeUnderTest;
        private static SparkplugDevice deviceUnderTest;
        private static List<Payload.Metric> nodeKnownMetrics;
        private static List<Payload.Metric> nodeMetrics;
        private static List<Payload.Metric> deviceKnownMetrics;
        private static List<Payload.Metric> deviceMetrics;

        /// <summary>
        /// Tests Sparkplug CONNECT requirements (NDEATH, NBIRTH).
        /// </summary>
        [TestMethod]
        public async Task T001_Node_ConnectBirth()
        {
            var userName = "admin";
            var password = "admin";
            var clientIdentifier = "client1";
            var groupIdentifier = "group1";
            var scadaHostIdentifier = "scada1";
            var edgeNodeIdentifier = "node1";
            var useTls = false;
            var nodeOptions = new SparkplugNodeOptions(MqttServerUnderTest.ServerAddress, MqttServerUnderTest.ServerPort, clientIdentifier,
                userName, password, useTls, scadaHostIdentifier, groupIdentifier, edgeNodeIdentifier, TimeSpan.FromSeconds(30), null, null, false);

            nodeKnownMetrics = GetNodeTestMetrics();
            nodeMetrics = GetNodeTestMetrics();
            nodeUnderTest = new SparkplugNode(nodeKnownMetrics);

            await nodeUnderTest.Start(nodeOptions);

            Assert.IsTrue(nodeUnderTest.IsConnected);
        }

        /// <summary>
        /// Tests Sparkplug PublishMetricsAsync (NDATA)
        /// </summary>
        [TestMethod]
        public async Task T002_Node_PublishMetrics()
        {
            // publish nodeMetrics with changes
            for (var i = 0; i < 3; i++)
            {
                await Task.Delay(1000);
                RandomUpdateTestMetrics(nodeMetrics);
                nodeUnderTest.PublishMetrics(nodeMetrics);
                ////var result = await nodeUnderTest.PublishMetricsAsync(nodeMetrics);
                ////Assert.IsTrue(result.ReasonCode is (MqttClientPublishReasonCode)0 or (MqttClientPublishReasonCode)16);
            }
        }

        /// <summary>
        /// Tests Sparkplug DBIRTH requirements (DBIRTH).
        /// </summary>
        [TestMethod]
        public async Task T010_Device_DBIRTH()
        {
            var userName = "admin";
            var password = "admin";
            var clientIdentifier = "client2";
            var groupIdentifier = "group1";
            var scadaHostIdentifier = "scada1";
            var edgeNodeIdentifier = "node1";
            var deviceIdentifier = "device1";
            var deviceGuid = Guid.NewGuid();
            var useTls = false;
            var deviceOptions = new SparkplugDeviceOptions(MqttServerUnderTest.ServerAddress, MqttServerUnderTest.ServerPort,
                clientIdentifier, userName, password, useTls, scadaHostIdentifier, groupIdentifier, edgeNodeIdentifier, deviceIdentifier, deviceGuid,
                TimeSpan.FromSeconds(30), null, null, false);

            // generate list of known metrics
            // create and start new instance of SparkplugNode
            deviceKnownMetrics = GetDeviceTestMetrics();
            deviceMetrics = GetDeviceTestMetrics();
            deviceUnderTest = new SparkplugDevice(deviceKnownMetrics) { ChildOf = nodeUnderTest };
            await deviceUnderTest.Start(deviceOptions);
            //Assert.IsTrue(deviceUnderTest.IsConnected);
        }

        /// <summary>
        /// Tests Welding Events
        /// </summary>
        [TestMethod]
        public async Task T011_Device_SimulateTransientMetrics()
        {
            // publish nodeMetrics with changes
            for (var i = 0; i < 2; i++)
            {
                await Task.Delay(1000);
                await SimulateTransientMetrics(2000, 100);
            }
        }

        /// <summary>
        /// Tests MQTT Device Disconnect
        /// </summary>
        [TestMethod]
        public async Task T012_Device_DDEATH()
        {
            // stop instance of SparkplugNode
            await Task.Delay(100);
            await deviceUnderTest.Stop();

            // delay 2 seconds
            await Task.Delay(2000);
        }

        /// <summary>
        /// Tests Sparkplug DBIRTH requirements (DBIRTH).
        /// </summary>
        [TestMethod]
        public async Task T020_Device_DBIRTH()
        {
            var userName = "admin";
            var password = "admin";
            var clientIdentifier = "client2";
            var groupIdentifier = "group1";
            var scadaHostIdentifier = "scada1";
            var edgeNodeIdentifier = "node1";
            var deviceIdentifier = "device1";
            var deviceGuid = Guid.NewGuid();
            var useTls = false;
            var deviceOptions = new SparkplugDeviceOptions(MqttServerUnderTest.ServerAddress, MqttServerUnderTest.ServerPort,
                clientIdentifier, userName, password, useTls, scadaHostIdentifier, groupIdentifier, edgeNodeIdentifier, deviceIdentifier, deviceGuid,
                TimeSpan.FromSeconds(30), null, null, false);

            await deviceUnderTest.Start(deviceOptions);
            //Assert.IsTrue(deviceUnderTest.IsConnected);
        }

        /// <summary>
        /// Tests Welding Events
        /// </summary>
        [TestMethod]
        public async Task T021_Device_SimulateTransientMetrics()
        {
            // publish nodeMetrics with changes
            for (var i = 0; i < 3; i++)
            {
                await Task.Delay(1000);
                await SimulateTransientMetrics(3000, 100);
            }
        }

        /// <summary>
        /// Tests MQTT Device Disconnect
        /// </summary>
        [TestMethod]
        public async Task T022_Device_DDEATH()
        {
            // stop instance of SparkplugNode
            await deviceUnderTest.Stop();
        }

        /// <summary>
        /// Tests MQTT Client Disconnect
        /// </summary>
        [TestMethod]
        public async Task T099_Node_StopDisconnect()
        {
            // assert IsConnected = true
            Assert.IsTrue(nodeUnderTest.IsConnected);

            // stop instance of SparkplugNode
            await Task.Delay(1000);
            await nodeUnderTest.Stop();

            // assert IsConnected = false
            Assert.IsFalse(nodeUnderTest.IsConnected);
        }

        private static List<Payload.Metric> GetNodeTestMetrics()
        {
            var random = new Random();
            var unixNow = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var metrics = new List<Payload.Metric>
            {
                new() { Name = "Name", Timestamp = unixNow, Datatype = (uint)SparkplugBDataType.String, StringValue = "EoN Node Name 1" },
                new()
                {
                    Name = "Some Int Value", Timestamp = unixNow, Datatype = (uint)SparkplugBDataType.Int32,
                    IntValue = (uint)random.Next(0, short.MaxValue)
                },
                new()
                {
                    Name = "Aggregates/Some Long Value", Timestamp = unixNow, Datatype = (uint)SparkplugBDataType.Int64,
                    LongValue = (ulong)random.Next(short.MaxValue, int.MaxValue)
                },
            };
            return metrics;
        }

        private static List<Payload.Metric> GetDeviceTestMetrics()
        {
            var random = new Random();
            var timestamp = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var metrics = new List<Payload.Metric>
            {
                //new() { Name = "Unknown Metric", Timestamp = timestamp, Datatype = (uint)SparkplugBDataType.Unknown },
                new() { Name = "Int8 Metric", Timestamp = timestamp, Datatype = (uint)SparkplugBDataType.Int8, IntValue = byte.MaxValue },
                new() { Name = "Int16 Metric", Timestamp = timestamp, Datatype = (uint)SparkplugBDataType.Int16, IntValue = (uint)short.MaxValue },
                new() { Name = "Int32 Metric", Timestamp = timestamp, Datatype = (uint)SparkplugBDataType.Int32, IntValue = int.MaxValue },
                new() { Name = "Int64 Metric", Timestamp = timestamp, Datatype = (uint)SparkplugBDataType.Int64, LongValue = long.MaxValue },
                new() { Name = "UInt8 Metric", Timestamp = timestamp, Datatype = (uint)SparkplugBDataType.UInt8, IntValue = byte.MaxValue },
                new() { Name = "UInt16 Metric", Timestamp = timestamp, Datatype = (uint)SparkplugBDataType.UInt16, IntValue = (uint)short.MaxValue },
                new() { Name = "UInt32 Metric", Timestamp = timestamp, Datatype = (uint)SparkplugBDataType.UInt32, IntValue = uint.MaxValue },
                new() { Name = "UInt64 Metric", Timestamp = timestamp, Datatype = (uint)SparkplugBDataType.UInt64, LongValue = ulong.MaxValue },
                new() { Name = "Float Metric", Timestamp = timestamp, Datatype = (uint)SparkplugBDataType.Float, FloatValue = 2.2f },
                new() { Name = "Double Metric", Timestamp = timestamp, Datatype = (uint)SparkplugBDataType.Double, DoubleValue = 3.3d },
                new() { Name = "Boolean Metric", Timestamp = timestamp, Datatype = (uint)SparkplugBDataType.Boolean, BooleanValue = false },
                new() { Name = "String Metric", Timestamp = timestamp, Datatype = (uint)SparkplugBDataType.String, StringValue = "Test String" },
                new() { Name = "DateTime Metric", Timestamp = timestamp, Datatype = (uint)SparkplugBDataType.DateTime, LongValue = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()},
                new() { Name = "Text Metric", Timestamp = timestamp, Datatype = (uint)SparkplugBDataType.Text, StringValue = "Test Text" },
                new() { Name = "UUID Metric", Timestamp = timestamp, Datatype = (uint)SparkplugBDataType.Uuid, StringValue = Guid.NewGuid().ToString() },
                //new() { Name = "DataSet Metric", Timestamp = timestamp, Datatype = (uint)SparkplugBDataType.Dataset, DatasetValue = new Payload.DataSet() },
                //new() { Name = "Bytes Metric", Timestamp = timestamp, Datatype = (uint)SparkplugBDataType.Bytes, BytesValue = new byte[0] },
                //new() { Name = "File Metric", Timestamp = timestamp, Datatype = (uint)SparkplugBDataType.File, BytesValue = new byte[0] },
                //new() { Name = "Template Metric", Timestamp = timestamp, Datatype = (uint)SparkplugBDataType.Template, TemplateValue = new Payload.Template() },
            };
            return metrics;
        }

        private static async Task SimulateTransientMetrics(int durationMs, int frequencyMs)
        {
            var metrics = deviceUnderTest.KnownMetrics;
            var messageCount = durationMs / frequencyMs;
            var random = new Random();

            for (var i = 1; i < messageCount + 1; i++)
            {
                await Task.Delay(frequencyMs);
                RandomUpdateTestMetrics(metrics);
                var now = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                foreach (var metric in metrics)
                {
                    metrics.SetMetricTransient(metric.Name, now, metric.GetValue());
                }

                // publish
                var changedMetrics = metrics.GetChangedMetrics(now);
                //var result = deviceUnderTest.PublishMetricsAsync(changedMetrics, 1);
                deviceUnderTest.PublishMetricsAsync(changedMetrics, 1);
                //Assert.IsTrue(result.ReasonCode == 0);
            }
        }

        private static void RandomUpdateTestMetrics(List<Payload.Metric> metrics)
        {
            var random = new Random();
            var unixUtcNow = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            ////// add extra metric after NBIRTH
            ////metrics.Add(new Payload.Metric()
            ////{
            ////    Name = "Extra Metric",
            ////    Timestamp = unixUtcNow,
            ////    Datatype = (uint)SparkplugBDataType.Int64,
            ////    LongValue = (ulong)random.Next(0, int.MaxValue)
            ////});

            foreach (var metric in metrics)
            {
                if (metric.Name == Constants.SessionNumberMetricName)
                {
                    return;
                }

                metric.IsNull = false;
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
                        metric.IntValue = (uint)random.Next(0, byte.MaxValue);
                        break;
                    case (int)SparkplugBDataType.Int16:
                    case (int)SparkplugBDataType.UInt16:
                        metric.IntValue = (uint)random.Next(0, short.MaxValue);
                        break;
                    case (int)SparkplugBDataType.Int32:
                    case (int)SparkplugBDataType.UInt32:
                        metric.IntValue = (uint)random.Next(0, int.MaxValue);
                        break;
                    case (int)SparkplugBDataType.Int64:
                    case (int)SparkplugBDataType.UInt64:
                        metric.LongValue = (ulong)random.Next(short.MaxValue, int.MaxValue);
                        break;
                    case (int)SparkplugBDataType.DateTime:
                        metric.LongValue = unixUtcNow;
                        break;
                    case (int)SparkplugBDataType.Float:
                        metric.FloatValue = (float)random.NextDouble() * random.Next(0, 1000);
                        break;
                    case (int)SparkplugBDataType.Double:
                        metric.DoubleValue = random.NextDouble() * random.Next(0, 1000);
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