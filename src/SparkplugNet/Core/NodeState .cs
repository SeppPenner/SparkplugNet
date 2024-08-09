// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MetricState.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A state class for the metrics.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Core;

/// <summary>
/// The node state class.
/// </summary>
/// <typeparam name="T">The type parameter.</typeparam>
public sealed class NodeState<T> : MetricState<T> where T : IMetric, new()
{
    /// <summary>
    /// Get the device states.
    /// </summary>
    public ConcurrentDictionary<string, MetricState<T>> DeviceStates { get; set; } = new();
}
