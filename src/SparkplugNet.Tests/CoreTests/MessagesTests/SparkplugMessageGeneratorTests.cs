// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugMessageGeneratorTest.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class to test the <see cref="SparkplugMessageGenerator"/> class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Tests.CoreTests.MessagesTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using MQTTnet;
    using SparkplugNet.Core;
    using SparkplugNet.Core.Enumerations;

    /// <summary>
    /// A class to test the <see cref="SparkplugNet.Core.Messages.SparkplugMessageGenerator"/> class.
    /// </summary>
    [TestClass]
    public class SparkplugMessageGeneratorTests
    {
        /// <summary>
        /// The metrics for namespace A.
        /// </summary>
        private readonly List<VersionA.Payload.KuraMetric> metricsA = new List<VersionA.Payload.KuraMetric>
        {
            new VersionA.Payload.KuraMetric
            {
                Name = "Test",
                BoolValue = true,
                Type = VersionA.Payload.KuraMetric.ValueType.Bool,
            }
        };

        /// <summary>
        /// The metrics for namespace B.
        /// </summary>
        private readonly List<VersionB.Payload.Metric> metricsB = new List<VersionB.Payload.Metric>
        {
            new VersionB.Payload.Metric
            {
                Name = "Test",
                Datatype = (uint)SparkplugBDataType.Int32,
                IntValue = 20,
                Timestamp = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            }
        };

        /// <summary>
        /// The SEQ metric for namespace A.
        /// </summary>
        private readonly VersionA.Payload.KuraMetric seqMetricA = new VersionA.Payload.KuraMetric
        {
            Name = Constants.SessionNumberMetricName,
            LongValue = 1,
            Type = VersionA.Payload.KuraMetric.ValueType.Int64,
        };

        /// <summary>
        /// The SEQ metric for namespace B.
        /// </summary>
        private readonly VersionB.Payload.Metric seqMetricB = new VersionB.Payload.Metric
        {
            Name = Constants.SessionNumberMetricName,
            LongValue = 1,
            Datatype = (uint)SparkplugBDataType.Int64,
        };

        /// <summary>
        /// The message generator.
        /// </summary>
        private readonly SparkplugNet.Core.Messages.SparkplugMessageGenerator messageGenerator = new ();

        /// <summary>
        /// Tests the Sparkplug message generator with a message with a version A namespace and a online state.
        /// </summary>
        [TestMethod]
        public void TestStateMessageNamespaceAOnline()
        {
            var message = this.messageGenerator.GetSparkplugStateMessage(SparkplugNamespace.VersionA, "scada1", true);

            Assert.AreEqual("STATE/scada1", message.Topic);
            Assert.AreEqual("ONLINE", message.ConvertPayloadToString());
        }

        /// <summary>
        /// Tests the Sparkplug message generator with a message with a version A namespace and a offline state.
        /// </summary>
        [TestMethod]
        public void TestStateMessageNamespaceAOffline()
        {
            var message = this.messageGenerator.GetSparkplugStateMessage(SparkplugNamespace.VersionA, "scada1", false);

            Assert.AreEqual("STATE/scada1", message.Topic);
            Assert.AreEqual("OFFLINE", message.ConvertPayloadToString());
        }

        /// <summary>
        /// Tests the Sparkplug message generator with a message with a version B namespace and a online state.
        /// </summary>
        [TestMethod]
        public void TestStateMessageNamespaceBOnline()
        {
            var message = this.messageGenerator.GetSparkplugStateMessage(SparkplugNamespace.VersionB, "scada1", true);

            Assert.AreEqual("STATE/scada1", message.Topic);
            Assert.AreEqual("ONLINE", message.ConvertPayloadToString());
        }

        /// <summary>
        /// Tests the Sparkplug message generator with a message with a version B namespace and a offline state.
        /// </summary>
        [TestMethod]
        public void TestStateMessageNamespaceBOffline()
        {
            var message = this.messageGenerator.GetSparkplugStateMessage(SparkplugNamespace.VersionB, "scada1", false);

            Assert.AreEqual("STATE/scada1", message.Topic);
            Assert.AreEqual("OFFLINE", message.ConvertPayloadToString());
        }

        /// <summary>
        /// Tests the Sparkplug message generator with a device birth message with a version A namespace.
        /// </summary>
        [TestMethod]
        public void TestDeviceBirthMessageNamespaceA()
        {
            var dateTime = DateTimeOffset.Now;
            var message = this.messageGenerator.GetSparkPlugDeviceBirthMessage(SparkplugNamespace.VersionA, "group1", "edge1", "device1", this.metricsA, 0, 1, dateTime, 1);
            var payloadVersionA = Core.PayloadHelper.Deserialize<VersionA.Payload>(message.Payload);

            Assert.AreEqual("spAv1.0/group1/DBIRTH/edge1/device1", message.Topic);
            Assert.IsNotNull(payloadVersionA);
            Assert.AreEqual(dateTime.ToUnixTimeMilliseconds(), payloadVersionA.Timestamp);
            Assert.AreEqual(2, payloadVersionA.Metrics.Count);

            Assert.AreEqual(this.metricsA.First().Name, payloadVersionA.Metrics.ElementAt(0).Name);
            Assert.AreEqual(this.metricsA.First().BoolValue, payloadVersionA.Metrics.ElementAt(0).BoolValue);
            Assert.AreEqual(this.metricsA.First().Type, payloadVersionA.Metrics.ElementAt(0).Type);

            Assert.AreEqual(this.seqMetricA.Name, payloadVersionA.Metrics.ElementAt(1).Name);
            Assert.AreEqual(this.seqMetricA.LongValue, payloadVersionA.Metrics.ElementAt(1).LongValue);
            Assert.AreEqual(this.seqMetricA.Type, payloadVersionA.Metrics.ElementAt(1).Type);
        }

        /// <summary>
        /// Tests the Sparkplug message generator with a device birth message with a version B namespace.
        /// </summary>
        [TestMethod]
        public void TestDeviceBirthMessageNamespaceB()
        {
            var dateTime = DateTimeOffset.Now;
            var message = this.messageGenerator.GetSparkPlugDeviceBirthMessage(SparkplugNamespace.VersionB, "group1", "edge1", "device1", this.metricsB, 0, 1, dateTime, 1);
            var payloadVersionB = PayloadHelper.Deserialize<VersionB.Payload>(message.Payload);

            Assert.AreEqual("spBv1.0/group1/DBIRTH/edge1/device1", message.Topic);
            Assert.IsNotNull(payloadVersionB);
            Assert.AreEqual((ulong)dateTime.ToUnixTimeMilliseconds(), payloadVersionB.Timestamp);
            Assert.AreEqual(2, payloadVersionB.Metrics.Count);

            Assert.AreEqual(this.metricsB.First().Name, payloadVersionB.Metrics.ElementAt(0).Name);
            Assert.AreEqual(this.metricsB.First().IntValue, payloadVersionB.Metrics.ElementAt(0).IntValue);
            Assert.AreEqual(this.metricsB.First().Datatype, payloadVersionB.Metrics.ElementAt(0).Datatype);

            Assert.AreEqual(this.seqMetricB.Name, payloadVersionB.Metrics.ElementAt(1).Name);
            Assert.AreEqual(this.seqMetricB.LongValue, payloadVersionB.Metrics.ElementAt(1).LongValue);
            Assert.AreEqual(this.seqMetricB.Datatype, payloadVersionB.Metrics.ElementAt(1).Datatype);
        }

        /// <summary>
        /// Tests the Sparkplug message generator with a node birth message with a version A namespace.
        /// </summary>
        [TestMethod]
        public void TestNodeBirthMessageNamespaceA()
        {
            var dateTime = DateTimeOffset.Now;
            var message = this.messageGenerator.GetSparkPlugNodeBirthMessage(SparkplugNamespace.VersionA, "group1", "edge1", this.metricsA, 0, 1, dateTime, 1);
            var payloadVersionA = PayloadHelper.Deserialize<VersionA.Payload>(message.Payload);

            Assert.AreEqual("spAv1.0/group1/NBIRTH/edge1", message.Topic);
            Assert.IsNotNull(payloadVersionA);
            Assert.AreEqual(dateTime.ToUnixTimeMilliseconds(), payloadVersionA.Timestamp);
            Assert.AreEqual(2, payloadVersionA.Metrics.Count);

            Assert.AreEqual(this.metricsA.First().Name, payloadVersionA.Metrics.ElementAt(0).Name);
            Assert.AreEqual(this.metricsA.First().BoolValue, payloadVersionA.Metrics.ElementAt(0).BoolValue);
            Assert.AreEqual(this.metricsA.First().Type, payloadVersionA.Metrics.ElementAt(0).Type);

            Assert.AreEqual(this.seqMetricA.Name, payloadVersionA.Metrics.ElementAt(1).Name);
            Assert.AreEqual(this.seqMetricA.LongValue, payloadVersionA.Metrics.ElementAt(1).LongValue);
            Assert.AreEqual(this.seqMetricA.Type, payloadVersionA.Metrics.ElementAt(1).Type);
        }

        /// <summary>
        /// Tests the Sparkplug message generator with a node birth message with a version B namespace.
        /// </summary>
        [TestMethod]
        public void TestNodeBirthMessageNamespaceB()
        {
            var dateTime = DateTimeOffset.Now;
            var message = this.messageGenerator.GetSparkPlugNodeBirthMessage(SparkplugNamespace.VersionB, "group1", "edge1", this.metricsB, 0, 1, dateTime, 1);
            var payloadVersionB = PayloadHelper.Deserialize<VersionB.Payload>(message.Payload);

            Assert.AreEqual("spBv1.0/group1/NBIRTH/edge1", message.Topic);
            Assert.IsNotNull(payloadVersionB);
            Assert.AreEqual((ulong)dateTime.ToUnixTimeMilliseconds(), payloadVersionB.Timestamp);
            Assert.AreEqual(2, payloadVersionB.Metrics.Count);

            Assert.AreEqual(this.metricsB.First().Name, payloadVersionB.Metrics.ElementAt(0).Name);
            Assert.AreEqual(this.metricsB.First().IntValue, payloadVersionB.Metrics.ElementAt(0).IntValue);
            Assert.AreEqual(this.metricsB.First().Datatype, payloadVersionB.Metrics.ElementAt(0).Datatype);

            Assert.AreEqual(this.seqMetricB.Name, payloadVersionB.Metrics.ElementAt(1).Name);
            Assert.AreEqual(this.seqMetricB.LongValue, payloadVersionB.Metrics.ElementAt(1).LongValue);
            Assert.AreEqual(this.seqMetricB.Datatype, payloadVersionB.Metrics.ElementAt(1).Datatype);
        }

        /// <summary>
        /// Tests the Sparkplug message generator with a device death message with a version A namespace.
        /// </summary>
        [TestMethod]
        public void TestDeviceDeathMessageNamespaceA()
        {
            var message = this.messageGenerator.GetSparkPlugDeviceDeathMessage(SparkplugNamespace.VersionA, "group1", "edge1", "device1", 0, 1, DateTimeOffset.Now, 1);
            var payloadVersionA = PayloadHelper.Deserialize<VersionA.Payload>(message.Payload);

            Assert.AreEqual("spAv1.0/group1/DDEATH/edge1/device1", message.Topic);
            Assert.IsNotNull(payloadVersionA);
            Assert.AreEqual(1, payloadVersionA.Metrics.Count);

            Assert.AreEqual(this.seqMetricA.Name, payloadVersionA.Metrics.ElementAt(0).Name);
            Assert.AreEqual(this.seqMetricA.LongValue, payloadVersionA.Metrics.ElementAt(0).LongValue);
            Assert.AreEqual(this.seqMetricA.Type, payloadVersionA.Metrics.ElementAt(0).Type);
        }

        /// <summary>
        /// Tests the Sparkplug message generator with a device death message with a version B namespace.
        /// </summary>
        [TestMethod]
        public void TestDeviceDeathMessageNamespaceB()
        {
            var message = this.messageGenerator.GetSparkPlugDeviceDeathMessage(SparkplugNamespace.VersionB, "group1", "edge1", "device1", 0, 1, DateTimeOffset.Now, 1);
            var payloadVersionB = PayloadHelper.Deserialize<VersionB.Payload>(message.Payload);

            Assert.AreEqual("spBv1.0/group1/DDEATH/edge1/device1", message.Topic);
            Assert.IsNotNull(payloadVersionB);
            Assert.AreEqual(1, payloadVersionB.Metrics.Count);

            Assert.AreEqual(this.seqMetricB.Name, payloadVersionB.Metrics.ElementAt(0).Name);
            Assert.AreEqual(this.seqMetricB.LongValue, payloadVersionB.Metrics.ElementAt(0).LongValue);
            Assert.AreEqual(this.seqMetricB.Datatype, payloadVersionB.Metrics.ElementAt(0).Datatype);
        }

        /// <summary>
        /// Tests the Sparkplug message generator with a node death message with a version A namespace.
        /// </summary>
        [TestMethod]
        public void TestNodeDeathMessageNamespaceA()
        {
            var message = this.messageGenerator.GetSparkPlugNodeDeathMessage(SparkplugNamespace.VersionA, "group1", "edge1", 0, 1, DateTimeOffset.Now, 1);
            var payloadVersionA = PayloadHelper.Deserialize<VersionA.Payload>(message.Payload);

            Assert.AreEqual("spAv1.0/group1/NDEATH/edge1", message.Topic);
            Assert.IsNotNull(payloadVersionA);
            Assert.AreEqual(1, payloadVersionA.Metrics.Count);

            Assert.AreEqual(this.seqMetricA.Name, payloadVersionA.Metrics.ElementAt(0).Name);
            Assert.AreEqual(this.seqMetricA.LongValue, payloadVersionA.Metrics.ElementAt(0).LongValue);
            Assert.AreEqual(this.seqMetricA.Type, payloadVersionA.Metrics.ElementAt(0).Type);
        }

        /// <summary>
        /// Tests the Sparkplug message generator with a node death message with a version B namespace.
        /// </summary>
        [TestMethod]
        public void TestNodeDeathMessageNamespaceB()
        {
            var message = this.messageGenerator.GetSparkPlugNodeDeathMessage(SparkplugNamespace.VersionB, "group1", "edge1", 0, 1, DateTimeOffset.Now, 1);
            var payloadVersionB = PayloadHelper.Deserialize<VersionB.Payload>(message.Payload);

            Assert.AreEqual("spBv1.0/group1/NDEATH/edge1", message.Topic);
            Assert.IsNotNull(payloadVersionB);
            Assert.AreEqual(1, payloadVersionB.Metrics.Count);

            Assert.AreEqual(this.seqMetricB.Name, payloadVersionB.Metrics.ElementAt(0).Name);
            Assert.AreEqual(this.seqMetricB.LongValue, payloadVersionB.Metrics.ElementAt(0).LongValue);
            Assert.AreEqual(this.seqMetricB.Datatype, payloadVersionB.Metrics.ElementAt(0).Datatype);
        }

        /// <summary>
        /// Tests the Sparkplug message generator with a device data message with a version A namespace.
        /// </summary>
        [TestMethod]
        public void TestDeviceDataMessageNamespaceA()
        {
            var dateTime = DateTimeOffset.Now;
            var message = this.messageGenerator.GetSparkPlugDeviceDataMessage(SparkplugNamespace.VersionA, "group1", "edge1", "device1", this.metricsA, 0, 1, dateTime, 1);
            var payloadVersionA = PayloadHelper.Deserialize<VersionA.Payload>(message.Payload);

            Assert.AreEqual("spAv1.0/group1/DDATA/edge1/device1", message.Topic);
            Assert.IsNotNull(payloadVersionA);
            Assert.AreEqual(dateTime.ToUnixTimeMilliseconds(), payloadVersionA.Timestamp);
            Assert.AreEqual(1, payloadVersionA.Metrics.Count);

            Assert.AreEqual(this.metricsA.First().Name, payloadVersionA.Metrics.ElementAt(0).Name);
            Assert.AreEqual(this.metricsA.First().BoolValue, payloadVersionA.Metrics.ElementAt(0).BoolValue);
            Assert.AreEqual(this.metricsA.First().Type, payloadVersionA.Metrics.ElementAt(0).Type);

            // removed as bdSeq metric is only required by spec in NBIRTH and NDEATH
            ////Assert.AreEqual(this.seqMetricA.Name, payloadVersionA.Metrics.ElementAt(1).Name);
            ////Assert.AreEqual(this.seqMetricA.LongValue, payloadVersionA.Metrics.ElementAt(1).LongValue);
            ////Assert.AreEqual(this.seqMetricA.Type, payloadVersionA.Metrics.ElementAt(1).Type);
        }

        /// <summary>
        /// Tests the Sparkplug message generator with a device data message with a version B namespace.
        /// </summary>
        [TestMethod]
        public void TestDeviceDataMessageNamespaceB()
        {
            var dateTime = DateTimeOffset.Now;
            var message = this.messageGenerator.GetSparkPlugDeviceDataMessage(SparkplugNamespace.VersionB, "group1", "edge1", "device1", this.metricsB, 0, 1, dateTime, 1);
            var payloadVersionB = PayloadHelper.Deserialize<VersionB.Payload>(message.Payload);

            Assert.AreEqual("spBv1.0/group1/DDATA/edge1/device1", message.Topic);
            Assert.IsNotNull(payloadVersionB);
            Assert.AreEqual((ulong)dateTime.ToUnixTimeMilliseconds(), payloadVersionB.Timestamp);
            Assert.AreEqual(1, payloadVersionB.Metrics.Count);

            Assert.AreEqual(this.metricsB.First().Name, payloadVersionB.Metrics.ElementAt(0).Name);
            Assert.AreEqual(this.metricsB.First().IntValue, payloadVersionB.Metrics.ElementAt(0).IntValue);
            Assert.AreEqual(this.metricsB.First().Datatype, payloadVersionB.Metrics.ElementAt(0).Datatype);

            // removed as bdSeq metric is only required by spec in NBIRTH and NDEATH
            ////Assert.AreEqual(this.seqMetricB.Name, payloadVersionB.Metrics.ElementAt(1).Name);
            ////Assert.AreEqual(this.seqMetricB.LongValue, payloadVersionB.Metrics.ElementAt(1).LongValue);
            ////Assert.AreEqual(this.seqMetricB.Datatype, payloadVersionB.Metrics.ElementAt(1).Datatype);
        }

        /// <summary>
        /// Tests the Sparkplug message generator with a node data message with a version A namespace.
        /// </summary>
        [TestMethod]
        public void TestNodeDataMessageNamespaceA()
        {
            var dateTime = DateTimeOffset.Now;
            var message = this.messageGenerator.GetSparkPlugNodeDataMessage(SparkplugNamespace.VersionA, "group1", "edge1", this.metricsA, 0, 1, dateTime, 1);
            var payloadVersionA = PayloadHelper.Deserialize<VersionA.Payload>(message.Payload);

            Assert.AreEqual("spAv1.0/group1/NDATA/edge1", message.Topic);
            Assert.IsNotNull(payloadVersionA);
            Assert.AreEqual(dateTime.ToUnixTimeMilliseconds(), payloadVersionA.Timestamp);
            Assert.AreEqual(1, payloadVersionA.Metrics.Count);

            Assert.AreEqual(this.metricsA.First().Name, payloadVersionA.Metrics.ElementAt(0).Name);
            Assert.AreEqual(this.metricsA.First().BoolValue, payloadVersionA.Metrics.ElementAt(0).BoolValue);
            Assert.AreEqual(this.metricsA.First().Type, payloadVersionA.Metrics.ElementAt(0).Type);

            // removed as bdSeq metric is only required by spec in NBIRTH and NDEATH
            ////Assert.AreEqual(this.seqMetricA.Name, payloadVersionA.Metrics.ElementAt(1).Name);
            ////Assert.AreEqual(this.seqMetricA.LongValue, payloadVersionA.Metrics.ElementAt(1).LongValue);
            ////Assert.AreEqual(this.seqMetricA.Type, payloadVersionA.Metrics.ElementAt(1).Type);
        }

        /// <summary>
        /// Tests the Sparkplug message generator with a node data message with a version B namespace.
        /// </summary>
        [TestMethod]
        public void TestNodeDataMessageNamespaceB()
        {
            var dateTime = DateTimeOffset.Now;
            var message = this.messageGenerator.GetSparkPlugNodeDataMessage(SparkplugNamespace.VersionB, "group1", "edge1", this.metricsB, 0, 1, dateTime, 1);
            var payloadVersionB = PayloadHelper.Deserialize<VersionB.Payload>(message.Payload);

            Assert.AreEqual("spBv1.0/group1/NDATA/edge1", message.Topic);
            Assert.IsNotNull(payloadVersionB);
            Assert.AreEqual((ulong)dateTime.ToUnixTimeMilliseconds(), payloadVersionB.Timestamp);
            Assert.AreEqual(1, payloadVersionB.Metrics.Count);

            Assert.AreEqual(this.metricsB.First().Name, payloadVersionB.Metrics.ElementAt(0).Name);
            Assert.AreEqual(this.metricsB.First().IntValue, payloadVersionB.Metrics.ElementAt(0).IntValue);
            Assert.AreEqual(this.metricsB.First().Datatype, payloadVersionB.Metrics.ElementAt(0).Datatype);

            ////////// removed as bdSeq metric is only required by spec in NBIRTH and NDEATH
            ////Assert.AreEqual(this.seqMetricB.Name, payloadVersionB.Metrics.ElementAt(1).Name);
            ////Assert.AreEqual(this.seqMetricB.LongValue, payloadVersionB.Metrics.ElementAt(1).LongValue);
            ////Assert.AreEqual(this.seqMetricB.Datatype, payloadVersionB.Metrics.ElementAt(1).Datatype);
        }

        /// <summary>
        /// Tests the Sparkplug message generator with a device command message with a version A namespace.
        /// </summary>
        [TestMethod]
        public void TestDeviceCommandMessageNamespaceA()
        {
            var dateTime = DateTimeOffset.Now;
            var message = this.messageGenerator.GetSparkPlugDeviceCommandMessage(SparkplugNamespace.VersionA, "group1", "edge1", "device1", this.metricsA, 0, 1, dateTime, 1);
            var payloadVersionA = PayloadHelper.Deserialize<VersionA.Payload>(message.Payload);

            Assert.AreEqual("spAv1.0/group1/DCMD/edge1/device1", message.Topic);
            Assert.IsNotNull(payloadVersionA);
            Assert.AreEqual(dateTime.ToUnixTimeMilliseconds(), payloadVersionA.Timestamp);
            Assert.AreEqual(1, payloadVersionA.Metrics.Count);

            Assert.AreEqual(this.metricsA.First().Name, payloadVersionA.Metrics.ElementAt(0).Name);
            Assert.AreEqual(this.metricsA.First().BoolValue, payloadVersionA.Metrics.ElementAt(0).BoolValue);
            Assert.AreEqual(this.metricsA.First().Type, payloadVersionA.Metrics.ElementAt(0).Type);

            ////// removed as bdSeq metric is only required by spec in NBIRTH and NDEATH
            ////Assert.AreEqual(this.seqMetricA.Name, payloadVersionA.Metrics.ElementAt(1).Name);
            ////Assert.AreEqual(this.seqMetricA.LongValue, payloadVersionA.Metrics.ElementAt(1).LongValue);
            ////Assert.AreEqual(this.seqMetricA.Type, payloadVersionA.Metrics.ElementAt(1).Type);
        }

        /// <summary>
        /// Tests the Sparkplug message generator with a device command message with a version B namespace.
        /// </summary>
        [TestMethod]
        public void TestDeviceCommandMessageNamespaceB()
        {
            var dateTime = DateTimeOffset.Now;
            var message = this.messageGenerator.GetSparkPlugDeviceCommandMessage(SparkplugNamespace.VersionB, "group1", "edge1", "device1", this.metricsB, 0, 1, dateTime, 1);
            var payloadVersionB = PayloadHelper.Deserialize<VersionB.Payload>(message.Payload);

            Assert.AreEqual("spBv1.0/group1/DCMD/edge1/device1", message.Topic);
            Assert.IsNotNull(payloadVersionB);
            Assert.AreEqual((ulong)dateTime.ToUnixTimeMilliseconds(), payloadVersionB.Timestamp);
            Assert.AreEqual(1, payloadVersionB.Metrics.Count);

            Assert.AreEqual(this.metricsB.First().Name, payloadVersionB.Metrics.ElementAt(0).Name);
            Assert.AreEqual(this.metricsB.First().IntValue, payloadVersionB.Metrics.ElementAt(0).IntValue);
            Assert.AreEqual(this.metricsB.First().Datatype, payloadVersionB.Metrics.ElementAt(0).Datatype);

            // removed as bdSeq metric is only required by spec in NBIRTH and NDEATH
            ////Assert.AreEqual(this.seqMetricB.Name, payloadVersionB.Metrics.ElementAt(1).Name);
            ////Assert.AreEqual(this.seqMetricB.LongValue, payloadVersionB.Metrics.ElementAt(1).LongValue);
            ////Assert.AreEqual(this.seqMetricB.Datatype, payloadVersionB.Metrics.ElementAt(1).Datatype);
        }

        /// <summary>
        /// Tests the Sparkplug message generator with a node command message with a version A namespace.
        /// </summary>
        [TestMethod]
        public void TestNodeCommandMessageNamespaceA()
        {
            var dateTime = DateTimeOffset.Now;
            var message = this.messageGenerator.GetSparkPlugNodeCommandMessage(SparkplugNamespace.VersionA, "group1", "edge1", this.metricsA, 0, 1, dateTime, 1);
            var payloadVersionA = PayloadHelper.Deserialize<VersionA.Payload>(message.Payload);

            Assert.AreEqual("spAv1.0/group1/NCMD/edge1", message.Topic);
            Assert.IsNotNull(payloadVersionA);
            Assert.AreEqual(dateTime.ToUnixTimeMilliseconds(), payloadVersionA.Timestamp);
            Assert.AreEqual(1, payloadVersionA.Metrics.Count);

            Assert.AreEqual(this.metricsA.First().Name, payloadVersionA.Metrics.ElementAt(0).Name);
            Assert.AreEqual(this.metricsA.First().BoolValue, payloadVersionA.Metrics.ElementAt(0).BoolValue);
            Assert.AreEqual(this.metricsA.First().Type, payloadVersionA.Metrics.ElementAt(0).Type);

            // removed as bdSeq metric is only required by spec in NBIRTH and NDEATH
            ////Assert.AreEqual(this.seqMetricA.Name, payloadVersionA.Metrics.ElementAt(1).Name);
            ////Assert.AreEqual(this.seqMetricA.LongValue, payloadVersionA.Metrics.ElementAt(1).LongValue);
            ////Assert.AreEqual(this.seqMetricA.Type, payloadVersionA.Metrics.ElementAt(1).Type);
        }

        /// <summary>
        /// Tests the Sparkplug message generator with a node command message with a version B namespace.
        /// </summary>
        [TestMethod]
        public void TestNodeCommandMessageNamespaceB()
        {
            var dateTime = DateTimeOffset.Now;
            var message = this.messageGenerator.GetSparkPlugNodeCommandMessage(SparkplugNamespace.VersionB, "group1", "edge1", this.metricsB, 0, 1, dateTime, 1);
            var payloadVersionB = PayloadHelper.Deserialize<VersionB.Payload>(message.Payload);

            Assert.AreEqual("spBv1.0/group1/NCMD/edge1", message.Topic);
            Assert.IsNotNull(payloadVersionB);
            Assert.AreEqual((ulong)dateTime.ToUnixTimeMilliseconds(), payloadVersionB.Timestamp);
            Assert.AreEqual(1, payloadVersionB.Metrics.Count);

            Assert.AreEqual(this.metricsB.First().Name, payloadVersionB.Metrics.ElementAt(0).Name);
            Assert.AreEqual(this.metricsB.First().IntValue, payloadVersionB.Metrics.ElementAt(0).IntValue);
            Assert.AreEqual(this.metricsB.First().Datatype, payloadVersionB.Metrics.ElementAt(0).Datatype);

            // removed as bdSeq metric is only required by spec in NBIRTH and NDEATH
            ////Assert.AreEqual(this.seqMetricB.Name, payloadVersionB.Metrics.ElementAt(1).Name);
            ////Assert.AreEqual(this.seqMetricB.LongValue, payloadVersionB.Metrics.ElementAt(1).LongValue);
            ////Assert.AreEqual(this.seqMetricB.Datatype, payloadVersionB.Metrics.ElementAt(1).Datatype);
        }
    }
}