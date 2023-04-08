// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugMqttProtocolVersion.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   The Sparkplug MQTT protocol version.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Core.Enumerations;

/// <summary>
/// The Sparkplug MQTT protocol version.
/// </summary>
public enum SparkplugMqttProtocolVersion
{
    /// <summary>
    /// MQTT protocol version 3.1.1.
    /// </summary>
    V311 = 4,

    /// <summary>
    /// MQTT protocol version 5.0.0.
    /// </summary>
    V500 = 5
}
