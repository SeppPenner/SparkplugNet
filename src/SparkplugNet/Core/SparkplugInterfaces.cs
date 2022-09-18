namespace SparkplugNet.Core;

/// <summary>
/// The interface defining a metric.
/// </summary>
public interface IMetric
{
    /// <summary>
    /// Gets the name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the value.
    /// </summary>
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
    /// Gets a value indicating whether this instance is connected or not.
    /// </summary>
    bool IsConnected { get; }
}
