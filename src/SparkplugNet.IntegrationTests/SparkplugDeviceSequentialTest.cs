// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugDeviceSequentialTest.cs" company="Hämmer Electronics">
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
    using SparkplugNet.Core.Device;
    using SparkplugNet.Core.Enumerations;
    using SparkplugNet.Core.Node;
    using SparkplugNet.VersionB;

    /// <summary>
    /// A class to test the <see cref="SparkplugNode" /> class with live MQTT Server and Sparkplug Host.
    /// These tests are designed to execute synchronously in alphabetic order. (e.g. T1_xxx, T2_yyy, T3_zzz)
    /// </summary>
    [TestClass]
    public class SparkplugDeviceSequentialTest
    {
        private readonly CancellationTokenSource cts = new CancellationTokenSource();
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
        public async Task T01_Node_VersionB_ConnectBirth()
        {
            var userName = "admin";
            var password = "admin";
            var clientIdentifier = "client1";
            var groupIdentifier = "group1";
            var scadaHostIdentifier = "scada1";
            var edgeNodeIdentifier = "node1";
            var nodeOptions = new SparkplugNodeOptions(MqttServerUnderTest.ServerAddress, MqttServerUnderTest.ServerPort, clientIdentifier, userName,
                password, false, scadaHostIdentifier, groupIdentifier, edgeNodeIdentifier, TimeSpan.FromSeconds(30), null, null,
                this.cts.Token);

            nodeKnownMetrics = GetNodeTestMetrics();
            nodeMetrics = GetNodeTestMetrics();
            nodeUnderTest = new SparkplugNode(nodeKnownMetrics);

            await nodeUnderTest.Start(nodeOptions);

            Assert.IsTrue(nodeUnderTest.IsConnected);
        }

        /// <summary>
        /// Tests Sparkplug PublishMetrics (NDATA)
        /// </summary>
        [TestMethod]
        public async Task T02_Node_VersionB_PublishMetrics()
        {
            // publish nodeMetrics with changes
            for (var i = 0; i < 3; i++)
            {
                await Task.Delay(1000);
                UpdateTestMetrics(nodeMetrics);
                var result = await nodeUnderTest.PublishMetrics(nodeMetrics);
                Assert.IsTrue(result.ReasonCode == 0);
            }
        }

        /// <summary>
        /// Tests Sparkplug DBIRTH requirements (DBIRTH).
        /// </summary>
        [TestMethod]
        public async Task T20_Device_VersionB_DBIRTH()
        {
            var userName = "admin";
            var password = "admin";
            var clientIdentifier = "client2";
            var groupIdentifier = "group1";
            var scadaHostIdentifier = "scada1";
            var edgeNodeIdentifier = "node1";
            var deviceIdentifier = "device1";
            var deviceOptions = new SparkplugDeviceOptions(MqttServerUnderTest.ServerAddress, MqttServerUnderTest.ServerPort, clientIdentifier, userName,
                password, false, scadaHostIdentifier, groupIdentifier, edgeNodeIdentifier, deviceIdentifier, TimeSpan.FromSeconds(30), null, null,
                this.cts.Token);

            // generate list of known metrics
            // create and start new instance of SparkplugNode
            deviceKnownMetrics = GetDeviceTestMetrics();
            deviceMetrics = GetDeviceTestMetrics();
            deviceUnderTest = new SparkplugDevice(deviceKnownMetrics) { ChildOf = nodeUnderTest };
            await deviceUnderTest.Start(deviceOptions);
            Assert.IsTrue(deviceUnderTest.IsConnected);
        }

        /// <summary>
        /// Tests Sparkplug Device PublishMetrics (DDATA)
        /// </summary>
        [TestMethod]
        public async Task T28_Device_VersionB_PublishMetrics()
        {
            // publish nodeMetrics with changes
            for (var i = 0; i < 3; i++)
            {
                await Task.Delay(1000);
                UpdateTestMetrics(deviceMetrics);
                var result = await deviceUnderTest.PublishMetrics(deviceMetrics, 1);
                Assert.IsTrue(result.ReasonCode == 0);
            }
        }

        /// <summary>
        /// Tests MQTT Device Disconnect
        /// </summary>
        [TestMethod]
        public async Task T29_Device_VersionB_DDEATH()
        {
            // assert IsConnected = true
            Assert.IsTrue(deviceUnderTest.IsConnected);

            // stop instance of SparkplugNode
            await deviceUnderTest.Stop();

            // assert IsConnected = false
            Assert.IsFalse(deviceUnderTest.IsConnected);
        }

        /// <summary>
        /// Tests MQTT Client Disconnect
        /// </summary>
        [TestMethod]
        public async Task T99_Node_VersionB_StopDisconnect()
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
                new() { Name = "Some Int Value", Timestamp = unixNow, Datatype = (uint)SparkplugBDataType.Int32, IntValue = (uint)random.Next(0, short.MaxValue) },
                new() { Name = "Aggregates/Some Long Value", Timestamp = unixNow, Datatype = (uint)SparkplugBDataType.Int64, LongValue = (ulong)random.Next(short.MaxValue, int.MaxValue) },
            };
            return metrics;
        }

        private static List<Payload.Metric> GetDeviceTestMetrics()
        {
            var random = new Random();
            var unixNow = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var metrics = new List<Payload.Metric>
            {
                new() { Name = "Name", Timestamp = unixNow, Datatype = (uint)SparkplugBDataType.String, StringValue = "Device Node Name 1" },
                new() { Name = "Some Long Value", Timestamp = unixNow, Datatype = (uint)SparkplugBDataType.Int64, LongValue = (ulong)random.Next(short.MaxValue, int.MaxValue), IsHistorical = true, IsTransient = false },
                new() { Name = "Sub1/Some Int Value", Timestamp = unixNow, Datatype = (uint)SparkplugBDataType.Int32, IntValue = (uint)random.Next(0, short.MaxValue), IsHistorical = true, IsTransient = false },
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
                Name = "Extra Metric",
                Timestamp = unixUtcNow,
                Datatype = (uint)SparkplugBDataType.Int64,
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
                        metric.IntValue = (uint)random.Next(0, short.MaxValue);
                        break;
                    case (int)SparkplugBDataType.Int64:
                    case (int)SparkplugBDataType.UInt64:
                    case (int)SparkplugBDataType.DateTime:
                        metric.LongValue = (ulong)random.Next(short.MaxValue, int.MaxValue);
                        break;
                    case (int)SparkplugBDataType.Float:
                        metric.FloatValue = (float)random.Next(0, int.MaxValue);
                        break;
                    case (int)SparkplugBDataType.Double:
                        metric.DoubleValue = (float)random.Next(0, int.MaxValue);
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