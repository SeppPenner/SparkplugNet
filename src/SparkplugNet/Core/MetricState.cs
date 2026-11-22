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
    /// Gets the metric definitions announced by this source in its last birth.
    /// </summary>
    internal SparkplugBase<T>.KnownMetricStorage? BirthMetrics { get; init; }

    /// <summary>
    /// Gets the MQTT session number announced by the node at birth.
    /// </summary>
    internal ulong? BirthSessionNumber { get; init; }

    /// <summary>
    /// Gets or sets the metric status.
    /// </summary>
    public SparkplugMetricStatus MetricStatus { get; set; } = SparkplugMetricStatus.Unknown;

    /// <summary>
    /// Gets the UTC Unix timestamp in milliseconds at which the source became online or offline.
    /// </summary>
    public ulong? StatusTimestamp { get; internal set; }

    /// <summary>
    /// Gets whether the cached values belong to an offline source and must be treated as stale.
    /// </summary>
    public bool MetricsAreStale => this.MetricStatus == SparkplugMetricStatus.Offline;

    /// <summary>
    /// Gets or sets the metrics.
    /// </summary>
    public ConcurrentDictionary<string, T> Metrics { get; set; } = new();
}
