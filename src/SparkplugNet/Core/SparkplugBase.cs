// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugBase.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A base class for all Sparkplug applications, nodes and devices.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Core;

using SparkplugNet.Core.Data;

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
    /// Gets or sets the callback for the disconnected event. Indicates that metrics might be stale.
    /// </summary>
    Action? OnDisconnected { get; set; }
}

/// <summary>
/// A base class for all Sparkplug applications, nodes and devices.
/// </summary>
/// <typeparam name="T">The type parameter.</typeparam>
public class SparkplugBase<T> : ISparkplugConnection
    where T : IMetric, new()
{
    /// <summary>
    /// The message generator.
    /// </summary>
    internal readonly SparkplugMessageGenerator MessageGenerator;

    /// <summary>
    /// The MQTT client.
    /// </summary>
    internal readonly IMqttClient Client;

    /// <summary>
    /// The knonw metrics by Name
    /// </summary>
    protected Dictionary<string, T> _knonwMetrics = new Dictionary<string, T>(StringComparer.InvariantCultureIgnoreCase);

    /// <summary>
    /// Initializes a new instance of the <see cref="SparkplugBase{T}"/> class.
    /// </summary>
    /// <param name="knownMetrics">The metric names.</param>
    /// <param name="logger">The logger.</param>
    public SparkplugBase(IEnumerable<T> knownMetrics, ILogger? logger = null)
    {
        this._knonwMetrics = knownMetrics.ToDictionary(metric => metric.Name);

        this.NameSpace = this.KnownMetrics switch
        {
            IEnumerable<VersionAData.KuraMetric> => SparkplugNamespace.VersionA,
            IEnumerable<VersionBData.Metric> => SparkplugNamespace.VersionB,
            _ => SparkplugNamespace.VersionB
        };

        this.Client = new MqttFactory().CreateMqttClient();
        this.Logger = logger;

        this.MessageGenerator = new SparkplugMessageGenerator(logger);
    }

    /// <summary>
    /// Filters the outgoing metrics.
    /// </summary>
    /// <param name="metric">The metric.</param>
    /// <returns></returns>
    protected virtual IEnumerable<T> FilterOutgoingMetrics(IEnumerable<T> metric)
    {
        return metric.Where(m =>
            // Remove the session number metric if a user might have added it.
            !string.Equals(m.Name, Constants.SessionNumberMetricName, StringComparison.InvariantCultureIgnoreCase) &&
            // Remove all not known metrics.
            this._knonwMetrics.ContainsKey(m.Name)
        );
    }

    /// <summary>
    /// Gets or sets the MQTT client options.
    /// </summary>
    internal MqttClientOptions? ClientOptions { get; set; }

    /// <summary>
    /// Gets the last sequence number. Starts at 0 as it is incremented after the publishing (For the device and node relevant only).
    /// </summary>
    protected int LastSequenceNumber { get; private set; }

    /// <summary>
    /// Gets the last session number. Starts at -1 as it is incremented before the connect already.
    /// </summary>
    protected long LastSessionNumber { get; private set; } = -1;

    /// <summary>
    /// Gets the Sparkplug namespace.
    /// </summary>
    protected SparkplugNamespace NameSpace { get; }

    /// <summary>
    /// Gets the logger.
    /// </summary>
    protected ILogger? Logger { get; }

    /// <summary>
    /// Gets a value indicating whether this instance is connected.
    /// </summary>
    public bool IsConnected => this.Client.IsConnected;

    /// <summary>
    /// Gets the known metric names.
    /// </summary>
    public IEnumerable<T> KnownMetrics => this._knonwMetrics.Values;

    /// <summary>
    /// Gets the known metric names.
    /// </summary>
    IEnumerable<IMetric> ISparkplugConnection.KnownMetrics => this.KnownMetrics.Cast<IMetric>();

    /// <summary>
    /// Gets or sets the callback for the disconnected event. Indicates that metrics might be stale.
    /// </summary>
    public Action? OnDisconnected { get; set; } = null;


    /// <summary>
    /// Resets the last sequence number.
    /// </summary>
    internal void ResetLastSequenceNumber()
    {
        this.LastSequenceNumber = 0;
    }

    /// <summary>
    /// Increments the last sequence number.
    /// </summary>
    internal void IncrementLastSequenceNumber()
    {
        if (this.LastSequenceNumber == 255)
        {
            this.LastSequenceNumber = 0;
        }
        else
        {
            this.LastSequenceNumber++;
        }
    }

    /// <summary>
    /// Increments the last session number.
    /// </summary>
    internal void IncrementLastSessionNumber()
    {
        if (this.LastSessionNumber == long.MaxValue)
        {
            this.LastSessionNumber = 0;
        }
        else
        {
            this.LastSessionNumber++;
        }
    }
}
