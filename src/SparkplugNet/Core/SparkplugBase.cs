// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugBase.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A base class for all Sparkplug applications, nodes and devices.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Core;

/// <inheritdoc cref="ISparkplugConnection"/>
/// <summary>
/// A base class for all Sparkplug applications, nodes and devices.
/// </summary>
/// <typeparam name="T">The type parameter.</typeparam>
/// <seealso cref="ISparkplugConnection"/>
public partial class SparkplugBase<T> : ISparkplugConnection where T : IMetric, new()
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
    /// The known metrics.
    /// </summary>
    protected KnownMetricStorage knownMetrics;

    /// <inheritdoc cref="ISparkplugConnection"/>
    /// <summary>
    /// Initializes a new instance of the <see cref="SparkplugBase{T}"/> class.
    /// </summary>
    /// <param name="knownMetrics">The metric names.</param>
    /// <param name="logger">The logger.</param>
    /// <seealso cref="ISparkplugConnection"/>
    public SparkplugBase(IEnumerable<T> knownMetrics, ILogger? logger = null)
        : this(new KnownMetricStorage(knownMetrics), logger)
    {
    }

    /// <inheritdoc cref="ISparkplugConnection"/>
    /// <summary>
    /// Initializes a new instance of the <see cref="SparkplugBase{T}"/> class.
    /// </summary>
    /// <param name="knownMetricsStorage">The known metrics storage.</param>
    /// <param name="logger">The logger.</param>
    /// <seealso cref="ISparkplugConnection"/>
    public SparkplugBase(KnownMetricStorage knownMetricsStorage, ILogger? logger = null)
    {
        this.knownMetrics = knownMetricsStorage;

        if (typeof(T).IsAssignableFrom(typeof(VersionBData.Metric)))
        {
            this.NameSpace = SparkplugNamespace.VersionB;
        }

        this.Client = new MqttFactory().CreateMqttClient();
        this.Logger = logger;

        this.MessageGenerator = new SparkplugMessageGenerator(logger);
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
    /// Gets or sets a value indicating whether this instance is running.
    /// </summary>
    public bool IsRunning { get; protected set; }

    /// <summary>
    /// Gets the known metric names.
    /// </summary>
    public IEnumerable<T> KnownMetrics => this.knownMetrics.Values;

    /// <summary>
    /// Gets the known metrics storage.
    /// </summary>
    public KnownMetricStorage KnownMetricsStorage => this.knownMetrics;

    /// <summary>
    /// Gets the known metric names.
    /// </summary>
    IEnumerable<IMetric> ISparkplugConnection.KnownMetrics => this.KnownMetrics.Cast<IMetric>();

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
