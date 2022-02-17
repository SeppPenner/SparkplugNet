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
public class MetricState<T> where T : class, new()
{
    /// <summary>
    /// Gets or sets the metric status.
    /// </summary>
    public SparkplugMetricStatus MetricStatus { get; set; } = SparkplugMetricStatus.Unknown;

    /// <summary>
    /// Gets or sets the metrics.
    /// </summary>
    public ConcurrentDictionary<string, T> Metrics { get; set; } = new ();
}
