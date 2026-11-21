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
/// The metric state class.
/// </summary>
/// <typeparam name="T">The type parameter.</typeparam>
public sealed class MetricState<T> where T : IMetric, new()
{
    /// <summary>
    /// Gets the group from which this state was received.
    /// </summary>
    internal string? GroupIdentifier { get; init; }

    /// <summary>
    /// Gets the MQTT session number announced by the node at birth.
    /// </summary>
    internal ulong? BirthSessionNumber { get; init; }

    /// <summary>
    /// Gets or sets the metric status.
    /// </summary>
    public SparkplugMetricStatus MetricStatus { get; set; } = SparkplugMetricStatus.Unknown;

    /// <summary>
    /// Gets or sets the metrics.
    /// </summary>
    public ConcurrentDictionary<string, T> Metrics { get; set; } = new();
}
