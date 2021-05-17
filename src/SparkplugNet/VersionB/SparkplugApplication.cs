namespace SparkplugNet.VersionB
{
    using System.Collections.Generic;

    using SparkplugNet.Core.Application;

    /// <inheritdoc cref="SparkplugApplicationBase{T}"/>
    public class SparkplugApplication : SparkplugApplicationBase<Payload.Metric>
    {
        /// <inheritdoc cref="SparkplugApplicationBase{T}"/>
        /// <summary>
        /// Initializes a new instance of the <see cref="SparkplugApplication"/> class.
        /// </summary>
        /// <param name="knownMetrics">The known metrics.</param>
        public SparkplugApplication(List<Payload.Metric> knownMetrics) : base(knownMetrics)
        {
        }
    }
}