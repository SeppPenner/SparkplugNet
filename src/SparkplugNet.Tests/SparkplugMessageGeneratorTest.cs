// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugMessageGeneratorTest.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class to test the <see cref="SparkplugMessageGenerator"/> class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using MQTTnet;

    using SparkplugNet.Core;
    using SparkplugNet.Core.Enumerations;
    using SparkplugNet.Core.Messages;

    using VersionAPayload = VersionA.Payload;
    using VersionBPayload = VersionB.Payload;


    /// <summary>
    /// A class to test the <see cref="SparkplugMessageGenerator"/> class.
    /// </summary>
    [TestClass]
    public class SparkplugMessageGeneratorTest
    {
        /// <summary>
        /// The metrics for namespace A.
        /// </summary>
        private readonly List<VersionAPayload.KuraMetric> metricsA = new List<VersionAPayload.KuraMetric>
        {
            new VersionAPayload.KuraMetric
            {
                Name = "Test",
                BoolValue = true,
                Type = VersionAPayload.KuraMetric.ValueType.Bool
            }
        };

        /// <summary>
        /// The metrics for namespace B.
        /// </summary>
        private readonly List<VersionBPayload.Metric> metricsB = new List<VersionBPayload.Metric>
        {
            new VersionBPayload.Metric
            {
                Name = "Test",
                Datatype = 10,
                IntValue = 20
            }
        };

        /// <summary>
        /// The SEQ metric for namespace A.
        /// </summary>
        private readonly VersionAPayload.KuraMetric seqMetricA = new VersionAPayload.KuraMetric
        {
            Name = Constants.SessionNumberMetricName,
            LongValue = 1,
            Type = VersionAPayload.KuraMetric.ValueType.Int64
        };

        /// <summary>
        /// The SEQ metric for namespace B.
        /// </summary>
        private readonly VersionBPayload.Metric seqMetricB = new VersionBPayload.Metric
        {
            Name = Constants.SessionNumberMetricName,
            LongValue = 1,
            Datatype = 11
        };

        /// <summary>
        /// The message generator.
        /// </summary>
        private readonly SparkplugMessageGenerator messageGenerator = new ();

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
            var message = this.messageGenerator.GetSparkPlugDeviceBirthMessage(SparkplugNamespace.VersionA, "group1", "edge1", "device1", this.metricsA, 0, 1, dateTime);
            var payloadVersionA = PayloadHelper.Deserialize<VersionAPayload>(message.Payload);

            Assert.AreEqual("spAv1.0/group1/DBIRTH/edge1/device1", message.Topic);
            Assert.IsNotNull(payloadVersionA);
            Assert.AreEqual(dateTime.ToUnixTimeMilliseconds(), payloadVersionA.Timestamp);

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
            var message = this.messageGenerator.GetSparkPlugDeviceBirthMessage(SparkplugNamespace.VersionB, "group1", "edge1", "device1", this.metricsB, 0, 1, dateTime);
            var payloadVersionB = PayloadHelper.Deserialize<VersionBPayload>(message.Payload);

            Assert.AreEqual("spBv1.0/group1/DBIRTH/edge1/device1", message.Topic);
            Assert.IsNotNull(payloadVersionB);
            Assert.AreEqual((ulong)dateTime.ToUnixTimeMilliseconds(), payloadVersionB.Timestamp);

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
            var message = this.messageGenerator.GetSparkPlugNodeBirthMessage(SparkplugNamespace.VersionA, "group1", "edge1", this.metricsA, 0, 1, dateTime);
            var payloadVersionA = PayloadHelper.Deserialize<VersionAPayload>(message.Payload);

            Assert.AreEqual("spAv1.0/group1/NBIRTH/edge1", message.Topic);
            Assert.IsNotNull(payloadVersionA);
            Assert.AreEqual(dateTime.ToUnixTimeMilliseconds(), payloadVersionA.Timestamp);

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
            var message = this.messageGenerator.GetSparkPlugNodeBirthMessage(SparkplugNamespace.VersionB, "group1", "edge1", this.metricsB, 0, 1, dateTime);
            var payloadVersionB = PayloadHelper.Deserialize<VersionBPayload>(message.Payload);

            Assert.AreEqual("spBv1.0/group1/NBIRTH/edge1", message.Topic);
            Assert.IsNotNull(payloadVersionB);
            Assert.AreEqual((ulong)dateTime.ToUnixTimeMilliseconds(), payloadVersionB.Timestamp);

            Assert.AreEqual(this.metricsB.First().Name, payloadVersionB.Metrics.ElementAt(0).Name);
            Assert.AreEqual(this.metricsB.First().IntValue, payloadVersionB.Metrics.ElementAt(0).IntValue);
            Assert.AreEqual(this.metricsB.First().Datatype, payloadVersionB.Metrics.ElementAt(0).Datatype);

            Assert.AreEqual(this.seqMetricB.Name, payloadVersionB.Metrics.ElementAt(1).Name);
            Assert.AreEqual(this.seqMetricB.LongValue, payloadVersionB.Metrics.ElementAt(1).LongValue);
            Assert.AreEqual(this.seqMetricB.Datatype, payloadVersionB.Metrics.ElementAt(1).Datatype);
        }
    }
}