// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugBrokerEndpoint.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Core;

/// <summary>
/// An MQTT broker endpoint in a multi-broker topology.
/// </summary>
/// <param name="Address">The TCP host name, IP address, or WebSocket URI.</param>
/// <param name="Port">The port used for TCP connections.</param>
public sealed record SparkplugBrokerEndpoint(string Address, int Port = SparkplugBaseOptions.DefaultPort);
