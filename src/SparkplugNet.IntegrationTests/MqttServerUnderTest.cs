// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MQTTServerUnderTest.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class to define MQTT Server co for reuse throughout integration tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.IntegrationTests
{
    /// <summary>
    /// A class to define MQTT Server for reuse throughout integration tests.
    /// </summary>
    public static class MqttServerUnderTest
    {
        /// <summary>
        /// The server address
        /// </summary>
        public static string ServerAddress { get; set; } = "server1";

        /// <summary>
        /// The server port
        /// </summary>
        public static int ServerPort { get; set; } = 1883;

        /// <summary>
        /// The client identifier
        /// </summary>
        public static string ClientId { get; set; } = "client1";

        /// <summary>
        /// Gets or sets the scada host identifier.
        /// </summary>
        public static string ScadaHostIdentifier { get; set; } = "scadaHost1";
    }
}