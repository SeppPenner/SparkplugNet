// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugBase.KnownMetricStorage.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A base class for all Sparkplug applications, nodes and devices.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Core;

/// <inheritdoc cref="ISparkplugConnection" />
/// <summary>
/// A base class for all Sparkplug applications, nodes and devices.
/// </summary>
/// <seealso cref="ISparkplugConnection" />
public partial class SparkplugBase<T> : ISparkplugConnection where T : IMetric, new()
{
    /// <inheritdoc cref="ConcurrentDictionary{TKey, TValue}"/>
    /// <summary>
    /// A class to handle the known metric storage.
    /// </summary>
    /// <seealso cref="ConcurrentDictionary{TKey, TValue}"/>
    public class KnownMetricStorage : ConcurrentDictionary<string, T>
    {
        /// <summary>
        /// Gets the metrics as <see cref="List{T}"/>.
        /// </summary>
        public List<T> Metrics => this.Values.ToList();

        /// <summary>
        /// Initializes a new instance of the <see cref="KnownMetricStorage"/> class.
        /// </summary>
        /// <param name="knownMetrics">The known metrics.</param>
        public KnownMetricStorage(IEnumerable<T> knownMetrics) : base(StringComparer.InvariantCultureIgnoreCase)
        {
            if (knownMetrics is not null)
            {
                foreach (var metric in knownMetrics)
                {
                    this[metric.Name] = metric;
                }
            }
        }

        /// <summary>
        /// Filters the outgoing metrics.
        /// </summary>
        /// <param name="metrics">The metric.</param>
        /// <returns>The filtered metrics.</returns>
        public virtual IEnumerable<T> FilterOutgoingMetrics(IEnumerable<T> metrics)
        {
            return metrics.Where(m =>
                // Remove the session number metric if a user might have added it.
                !string.Equals(m.Name, Constants.SessionNumberMetricName,
                    StringComparison.InvariantCultureIgnoreCase) &&
                // Remove all not known metrics.
                this.ContainsKey(m.Name)
            );
        }

        /// <summary>
        /// Validates the incoming metrics.
        /// </summary>
        /// <param name="metrics">The metrics.</param>
        /// <exception cref="Exception">Thrown if the metric name is an unknown metric.</exception>
        public virtual void ValidateIncomingMetrics(IEnumerable<T> metrics)
        {
            foreach (var metric in metrics.Where(metric => !this.ContainsKey(metric.Name)))
            {
                throw new Exception($"Metric {metric.Name} is an unknown metric.");
            }
        }

        /// <summary>
        /// Filters the incoming metrics.
        /// </summary>
        /// <param name="metrics">The metrics.</param>
        /// <returns>The filtered metrics.</returns>
        public virtual IEnumerable<T> FilterIncomingMetrics(IEnumerable<T> metrics)
            => metrics.Where(metric => this.ContainsKey(metric.Name) || IsSessionNumberMetric(metric));

        /// <summary>
        /// Checks whether the given metric equals the session number metric.
        /// </summary>
        /// <param name="metric">The metric.</param>
        /// <returns>Whether the given metric equals the session number metric</returns>
        private static bool IsSessionNumberMetric(T metric) =>
            metric.Name.ToUpper().Equals(Constants.SessionNumberMetricName);

        /// <summary>
        /// Screens the incoming metrics based on the specified method.
        /// </summary>
        /// <param name="metrics">The metrics.</param>
        /// <param name="method">The screening method.</param>
        /// <returns>The (filtered) metrics.</returns>
        public virtual IEnumerable<T> ScreenIncomingMetrics(IEnumerable<T> metrics, MetricScreenMethod method)
        {
            switch (method)
            {
                // If we have any invalid metrics, filter them
                case MetricScreenMethod.Filter:
                    metrics = this.FilterIncomingMetrics(metrics);
                    break;
                // If we have any invalid metrics, throw an exception
                case MetricScreenMethod.Validate:
                    IEnumerable<T> metricsWithoutSequenceMetric =
                        metrics.Where(m => !IsSessionNumberMetric(m));
                    this.ValidateIncomingMetrics(metricsWithoutSequenceMetric);
                    break;
                // All metrics are allowed, do nothing
                case MetricScreenMethod.None:
                default:
                    break;
            }

            return metrics;
        }
    }
}