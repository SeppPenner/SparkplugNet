// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISparkplugConnection.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A base interface for all Sparkplug applications, nodes and devices.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Core.Interfaces;

/// <summary>
/// A base interface for all Sparkplug applications, nodes and devices.
/// </summary>
public interface ISparkplugConnection
{
    /// <summary>
    /// Gets the known metric names.
    /// </summary>
    IEnumerable<IMetric> KnownMetrics { get; }

    /// <summary>
    /// Gets a value indicating whether this instance is connected or not.
    /// </summary>
    bool IsConnected { get; }
}
