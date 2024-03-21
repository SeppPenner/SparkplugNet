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
    /// <summary>
    /// A class to handle the known metric storage.
    /// </summary>
    public class KnownMetricStorage
    {
        /// <summary>
        /// The known metrics by name.
        /// </summary>
        private readonly ConcurrentDictionary<string, T> knownMetricsByName = new();

        /// <summary>
        /// The known metrics by alias.
        /// </summary>
        private readonly ConcurrentDictionary<ulong, T> knownMetricsByAlias = new();

        /// <summary>
        /// Gets the metrics as <see cref="List{T}"/>.
        /// </summary>
        public List<T> Metrics => [.. this.knownMetricsByName.Values, .. this.knownMetricsByAlias.Values];

        /// <summary>
        /// The logger.
        /// </summary>
        public readonly ILogger<KnownMetricStorage>? Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="KnownMetricStorage"/> class.
        /// </summary>
        /// <param name="knownMetrics">The known metrics.</param>
        /// <param name="logger">The logger.</param>
        public KnownMetricStorage(IEnumerable<T> knownMetrics, ILogger<KnownMetricStorage>? logger = null)
        {
            this.Logger = logger;

            foreach (var metric in knownMetrics)
            {
                // Version A: KuraMetric has name only.
                if (metric is VersionAData.KuraMetric _)
                {
                    this.AddVersionAMetric(metric);
                }

                // Version B: Metric might have name and alias.
                if (metric is Metric versionBMetric)
                {
                    this.AddVersionBMetric(metric, versionBMetric);
                }
            }
        }

        /// <summary>
        /// Filters the outgoing metrics.
        /// </summary>
        /// <param name="metrics">The metric.</param>
        /// <param name="sparkplugMessageType">The Sparkplug message type.</param>
        /// <returns>The filtered metrics.</returns>
        public virtual IEnumerable<T> FilterMetrics(IEnumerable<T> metrics, SparkplugMessageType sparkplugMessageType)
        {
            // The filtered metrics.
            var filteredMetrics = new List<T>();

            foreach (var metric in metrics)
            {
                // Version A: KuraMetric has name only.
                if (metric is VersionAData.KuraMetric versionAMetric)
                {
                    if (this.ShouldVersionAMetricBeAdded(versionAMetric))
                    {
                        filteredMetrics.Add(metric);
                        continue;
                    }
                }

                // Version B: Metric might have name and alias.
                if (metric is Metric versionBMetric)
                {
                    if (this.ShouldVersionBMetricBeAdded(sparkplugMessageType, versionBMetric))
                    {
                        filteredMetrics.Add(metric);
                        continue;
                    }
                }
            }

            return filteredMetrics;
        }

        /// <summary>
        /// Gets a value indicating whether a version B metric should be added to the result or not.
        /// </summary>
        /// <param name="sparkplugMessageType">The Sparkplug message type.</param>
        /// <param name="metric">The converted (typed) metric.</param>
        /// <returns>A value indicating whether a version B metric should be added to the result or not.</returns>
        private bool ShouldVersionBMetricBeAdded(SparkplugMessageType sparkplugMessageType, Metric metric)
        {
            // NBIRTH and DBIRTH messages MUST include both a metric name and an alias (if aliases should be used).
            var isBirthMessage = sparkplugMessageType == SparkplugMessageType.NodeBirth || sparkplugMessageType == SparkplugMessageType.DeviceBirth;

            var shouldbeAdded = true;

            if (string.IsNullOrWhiteSpace(metric.Name) && metric.Alias is null)
            {
                // Name and alias are not set.
                shouldbeAdded = false;
                this.Logger?.LogError("A metric without a name and an alias is not allowed: {Metric}.", metric);
                return shouldbeAdded;
            }
            else if (!string.IsNullOrWhiteSpace(metric.Name) && metric.Alias is null)
            {
                // Name only is set.

                // Check if the metric is known.
                if (!this.knownMetricsByName.TryGetValue(metric.Name, out var foundMetric))
                {
                    shouldbeAdded = false;
                    this.Logger?.LogError("The metric {Metric} is removed because it is unknown.", metric);
                }

                // Check if the found metric is a version B metric.
                if (foundMetric is not Metric foundVersionBMetric)
                {
                    shouldbeAdded = false;
                    this.Logger?.LogError("The metric cast didn't work properly.");
                }
                else if (foundVersionBMetric.DataType != metric.DataType)
                {
                    shouldbeAdded = false;
                    this.Logger?.LogError("The metric's data type is invalid.");
                }
            }
            else if (string.IsNullOrWhiteSpace(metric.Name) && metric.Alias is not null)
            {
                // Alias only is set.

                // NBIRTH and DBIRTH messages MUST include both a metric name and an alias (if aliases should be used).
                if (isBirthMessage)
                {
                    shouldbeAdded = false;
                    this.Logger?.LogError("The metric {Metric} is removed because it comes from a NBIRTH or DBIRTH message, but has only an alias set.", metric);
                }
                else
                {
                    // Check if the metric is known.
                    if (!this.knownMetricsByAlias.TryGetValue(metric.Alias.Value, out var foundMetric))
                    {
                        shouldbeAdded = false;
                        this.Logger?.LogError("The metric {Metric} is removed because it is unknown.", metric);
                    }

                    // Check if the found metric is a version B metric.
                    if (foundMetric is not Metric foundVersionBMetric)
                    {
                        shouldbeAdded = false;
                        this.Logger?.LogError("The metric cast didn't work properly.");
                    }
                    else if (foundVersionBMetric.DataType != metric.DataType)
                    {
                        shouldbeAdded = false;
                        this.Logger?.LogError("The metric's data type is invalid.");
                    }
                }
            }
            else
            {
                // Alias and name are set.

                // NDATA, DDATA, NCMD, and DCMD messages MUST only include an alias and the metric name MUST be excluded.
                if (!isBirthMessage)
                {
                    shouldbeAdded = false;
                    this.Logger?.LogError("The metric {Metric} is removed because it comes from a NDATA, DDATA, NCMD or DCMD message, but has a name and an alias set.", metric);
                }
                else
                {
                    // Check if the metric is known.
                    if (!this.knownMetricsByName.TryGetValue(metric.Name, out var foundMetric))
                    {
                        shouldbeAdded = false;
                        this.Logger?.LogError("The metric {Metric} is removed because it is unknown.", metric);
                    }

                    // Check if the found metric is a version B metric.
                    if (foundMetric is not Metric foundVersionBMetric)
                    {
                        shouldbeAdded = false;
                        this.Logger?.LogError("The metric cast didn't work properly.");
                    }
                    else if (foundVersionBMetric.DataType != metric.DataType)
                    {
                        shouldbeAdded = false;
                        this.Logger?.LogError("The metric's data type is invalid.");
                    }
                }
            }

            return shouldbeAdded;
        }

        /// <summary>
        /// Gets a value indicating whether a version A metric should be added to the result or not.
        /// </summary>
        /// <param name="metric">The converted (typed) metric.</param>
        /// <returns>A value indicating whether a version A metric should be added to the result or not.</returns>
        private bool ShouldVersionAMetricBeAdded(VersionAData.KuraMetric metric)
        {
            var shouldbeAdded = true;

            // Check metric name.
            if (string.IsNullOrWhiteSpace(metric.Name))
            {
                shouldbeAdded = false;
                this.Logger?.LogError("A metric without a name is not allowed: {Metric}.", metric);
            }

            // Check if the metric is known.
            if (!this.knownMetricsByName.TryGetValue(metric.Name, out var foundMetric))
            {
                shouldbeAdded = false;
                this.Logger?.LogError("The metric {Metric} is removed because it is unknown.", metric);
            }

            // Check if the found metric is a version A metric.
            if (foundMetric is not VersionAData.KuraMetric foundVersionAMetric)
            {
                shouldbeAdded = false;
                this.Logger?.LogError("The metric cast didn't work properly.");
            }
            else if (foundVersionAMetric.DataType != metric.DataType)
            {
                shouldbeAdded = false;
                this.Logger?.LogError("The metric's data type is invalid.");
            }

            return shouldbeAdded;
        }

        /// <summary>
        /// Adds a version A metric to the known metrics.
        /// </summary>
        /// <param name="metric">The metric.</param>
        /// <exception cref="InvalidMetricException">Thrown if the metric is invalid.</exception>
        private void AddVersionAMetric(T metric)
        {
            // Check the name of the metric.
            if (string.IsNullOrWhiteSpace(metric.Name))
            {
                throw new InvalidMetricException($"A metric without a name is not allowed.");
            }

            // Check the value of the metric.
            if (metric.Value is null)
            {
                throw new InvalidMetricException($"A metric without a current value is not allowed.");
            }

            // Hint: Data type doesn't need to be checked, is not nullable.
            this.knownMetricsByName[metric.Name] = metric;
        }

        /// <summary>
        /// Adds a version B metric to the known metrics.
        /// </summary>
        /// <param name="metric">The metric.</param>
        /// <param name="versionBMetric">The converted (typed) metric.</param>
        /// <exception cref="InvalidMetricException">Thrown if the metric is invalid.</exception>
        private void AddVersionBMetric(T metric, Metric versionBMetric)
        {
            // Check the name of the metric.
            if (string.IsNullOrWhiteSpace(versionBMetric.Name))
            {
                // Check the alias of the metric.
                if (versionBMetric.Alias is null)
                {
                    throw new InvalidMetricException($"A metric without a name and an alias is not allowed.");
                }

                // Check the value of the metric.
                if (versionBMetric.Value is null)
                {
                    throw new InvalidMetricException($"A metric without a current value is not allowed.");
                }

                // Hint: Data type doesn't need to be checked, is not nullable.
                this.knownMetricsByAlias[versionBMetric.Alias.Value] = metric;
            }
            else
            {
                // Check the value of the metric.
                if (metric.Value is null)
                {
                    throw new InvalidMetricException($"A metric without a current value is not allowed.");
                }

                // Hint: Data type doesn't need to be checked, is not nullable.
                this.knownMetricsByName[metric.Name] = metric;
            }
        }
    }
}
