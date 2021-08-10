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
        public const string ServerAddress = "mqttserverhost";

        /// <summary>
        /// The server port
        /// </summary>
        public const int ServerPort = 1883;
    }
}