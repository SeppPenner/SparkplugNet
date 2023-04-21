// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MQTTServerUnderTest.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class to define a MQTT Server for reuse throughout integration tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.IntegrationTests;

/// <summary>
/// A class to define a MQTT Server for reuse throughout integration tests.
/// </summary>
public static class MqttServerUnderTest
{
    /// <summary>
    /// The server address.
    /// </summary>
    public const string ServerAddress = "localhost";

    /// <summary>
    /// The server port.
    /// </summary>
    public const int ServerPort = 1883;

    /// <summary>
    /// The client identifier.
    /// </summary>
    public const string ClientId = "client1";

    /// <summary>
    /// The SCADA host identifier.
    /// </summary>
    public const string ScadaHostIdentifier = "scadaHost1";
}
