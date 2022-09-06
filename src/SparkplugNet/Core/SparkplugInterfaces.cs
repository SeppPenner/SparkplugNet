namespace SparkplugNet.Core
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

    /// <summary>
    /// A base interface for all Sparkplug applications, nodes and devices.
    /// </summary>
    public interface ISparkplugConnection
    {
        /// <summary>
        /// Gets the known metric names.
        /// </summary>
        IEnumerable<IMetric> KnownMetrics { get; }
        /// <summary>
        /// Gets a value indicating whether this instance is connected.
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Occurs when [connected asynchronous].
        /// </summary>
        event Func<SparkplugEventArgs,Task> ConnectedAsync;
        /// <summary>
        /// Occurs when [disconnected asynchronous].
        /// </summary>
        event Func<SparkplugEventArgs, Task> DisconnectedAsync;
    }
}
