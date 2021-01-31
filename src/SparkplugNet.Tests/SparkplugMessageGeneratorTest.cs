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
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using MQTTnet;

    using SparkplugNet.Enumerations;
    using SparkplugNet.Messages;

    /// <summary>
    /// A class to test the <see cref="SparkplugMessageGenerator"/> class.
    /// </summary>
    [TestClass]
    public class SparkplugMessageGeneratorTest
    {
        /// <summary>
        /// The SCADA host identifier.
        /// </summary>
        private const string ScadaHostIdentifier = "scada1";

        /// <summary>
        /// The message generator.
        /// </summary>
        private readonly SparkplugMessageGenerator messageGenerator = new SparkplugMessageGenerator();

        /// <summary>
        /// Tests the Sparkplug message generator with a message with Sparkplug version 2.2, a version A namespace and a online state.
        /// </summary>
        [TestMethod]
        public void TestNamespaceAOnline()
        {
            var message = this.messageGenerator.GetSparkplugStateMessage(
                SparkplugVersion.V22,
                SparkplugNamespace.VersionA,
                "scada1",
                true);

            Assert.AreEqual($"STATE/{ScadaHostIdentifier}", message.Topic);
            Assert.AreEqual("ONLINE", message.ConvertPayloadToString());
        }

        /// <summary>
        /// Tests the Sparkplug message generator with a message with Sparkplug version 2.2, a version A namespace and a offline state.
        /// </summary>
        [TestMethod]
        public void TestNamespaceAOffline()
        {
            var message = this.messageGenerator.GetSparkplugStateMessage(
                SparkplugVersion.V22,
                SparkplugNamespace.VersionA,
                "scada1",
                false);

            Assert.AreEqual($"STATE/{ScadaHostIdentifier}", message.Topic);
            Assert.AreEqual("OFFLINE", message.ConvertPayloadToString());
        }

        /// <summary>
        /// Tests the Sparkplug message generator with a message with Sparkplug version 2.2, a version B namespace and a online state.
        /// </summary>
        [TestMethod]
        public void TestNamespaceBOnline()
        {
            var message = this.messageGenerator.GetSparkplugStateMessage(
                SparkplugVersion.V22,
                SparkplugNamespace.VersionB,
                "scada1",
                true);

            Assert.AreEqual($"STATE/{ScadaHostIdentifier}", message.Topic);

            // Todo: Add test once payload is final
            Assert.AreEqual(string.Empty, message.ConvertPayloadToString());
        }

        /// <summary>
        /// Tests the Sparkplug message generator with a message with Sparkplug version 2.2, a version B namespace and a offline state.
        /// </summary>
        [TestMethod]
        public void TestNamespaceBOffline()
        {
            var message = this.messageGenerator.GetSparkplugStateMessage(
                SparkplugVersion.V22,
                SparkplugNamespace.VersionB,
                "scada1",
                false);

            Assert.AreEqual($"STATE/{ScadaHostIdentifier}", message.Topic);

            // Todo: Add test once payload is final
            Assert.AreEqual("OFFLINE", message.ConvertPayloadToString());
        }
    }
}