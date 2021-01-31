namespace SparkplugNet.Tests
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SparkplugNet.Enumerations;
    using SparkplugNet.Node;

    [TestClass]
    public class SparkplugNodeTest
    {
        private SparkplugNodeOptions options = new SparkplugNodeOptions(
            "localhost",
            "testApplication",
            "test",
            "password",
            false,
            "scala1",
            "group1",
            "edge1",
            TimeSpan.FromSeconds(5));

        /// <summary>
        /// Tests the Sparkplug application with the version A namespace.
        /// </summary>
        [TestMethod]
        public async Task TestNamespaceA()
        {
            var node = new SparkplugNode(SparkplugVersion.V22, SparkplugNamespace.VersionA);
            await node.Start(this.options);
        }

        /// <summary>
        /// Tests the Sparkplug application with the version B namespace.
        /// </summary>
        [TestMethod]
        public async Task TestNamespaceB()
        {
            var node = new SparkplugNode(SparkplugVersion.V22, SparkplugNamespace.VersionB);
            await node.Start(this.options);
        }
    }
}