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
    /// Gets or sets the server address.
    /// </summary>
    public static string ServerAddress { get; set; } = "localhost";

    /// <summary>
    /// Gets or sets the server port.
    /// </summary>
    public static int ServerPort { get; set; } = 1883;

    /// <summary>
    /// Gets or sets the client identifier.
    /// </summary>
    public static string ClientId { get; set; } = "client1";

    /// <summary>
    /// Gets or sets the SCADA host identifier.
    /// </summary>
    public static string ScadaHostIdentifier { get; set; } = "scadaHost1";
}
