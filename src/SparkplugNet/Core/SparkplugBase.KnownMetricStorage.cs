namespace SparkplugNet.Core;

public partial class SparkplugBase<T> : ISparkplugConnection where T : IMetric, new()
{
    /// <summary>
    /// Storage for the Known Metrics
    /// </summary>
    /// <seealso cref="ISparkplugConnection" />
    public class KnownMetricStorage : ConcurrentDictionary<string, T>
    {
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
        /// <returns></returns>
        public virtual IEnumerable<T> FilterOutgoingMetrics(IEnumerable<T> metrics)
        {
            return metrics.Where(m =>
                // Remove the session number metric if a user might have added it.
                !string.Equals(m.Name, Constants.SessionNumberMetricName, StringComparison.InvariantCultureIgnoreCase) &&
                // Remove all not known metrics.
                this.ContainsKey(m.Name)
            );
        }

        /// <summary>
        /// Validates the incomming metrics.
        /// </summary>
        /// <param name="metrics">The metrics.</param>
        /// <exception cref="Exception">Metric {metric.Name} is an unknown metric.</exception>
        public virtual void ValidateIncommingMetrics(IEnumerable<T> metrics)
        {
            foreach (var metric in metrics.Where(metric => !this.ContainsKey(metric.Name)))
            {
                throw new Exception($"Metric {metric.Name} is an unknown metric.");
            }
        }
    }
}
