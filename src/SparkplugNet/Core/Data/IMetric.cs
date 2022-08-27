namespace SparkplugNet.Core.Data
{
    /// <summary>
    /// Inteface defining a metric
    /// </summary>
    public interface IMetric
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; }
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public object? Value { get; }
    }
}
