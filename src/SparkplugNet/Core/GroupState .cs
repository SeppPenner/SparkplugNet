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
/// The group state class.
/// </summary>
/// <typeparam name="T">The type parameter.</typeparam>
public sealed class GroupState<T> where T : IMetric, new()
{
    /// <summary>
    /// Get the device states.
    /// </summary>
    public ConcurrentDictionary<string, NodeState<T>> NodeStates { get; } = new();
}
