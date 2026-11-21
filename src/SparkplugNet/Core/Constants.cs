// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Constants.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class that contains constant values.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Core;

/// <summary>
/// A class that contains constant values.
/// </summary>
internal static class Constants
{
    /// <summary>
    /// The session number metric name.
    /// </summary>
    internal const string SessionNumberMetricName = "bdSeq";

    /// <summary>
    /// The required node control metric for requesting a rebirth.
    /// </summary>
    internal const string RebirthMetricName = "Node Control/Rebirth";

    /// <summary>
    /// Determines whether a metric is the named rebirth control with a boolean value.
    /// </summary>
    /// <param name="metric">The command metric.</param>
    /// <returns>Whether the metric is the rebirth control.</returns>
    internal static bool IsRebirthControl(IMetric metric) =>
        metric.Name == RebirthMetricName && metric.Value is bool && metric is not Metric { Alias: not null };

    /// <summary>
    /// The epoch.
    /// </summary>
    internal static readonly DateTime Epoch = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
}
