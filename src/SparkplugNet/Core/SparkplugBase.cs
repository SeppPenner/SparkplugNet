// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugBase.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A base class for all Sparkplug applications, nodes and devices.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Core;
/// <summary>
/// A base class for all Sparkplug applications, nodes and devices.
/// </summary>
/// <typeparam name="T">The type parameter.</typeparam>
public partial class SparkplugBase<T> : ISparkplugConnection
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
    protected KnownMetricStorage _knonwMetrics;

    /// <summary>
    /// Initializes a new instance of the <see cref="SparkplugBase{T}"/> class.
    /// </summary>
    /// <param name="knownMetrics">The metric names.</param>
    /// <param name="logger">The logger.</param>
    public SparkplugBase(IEnumerable<T> knownMetrics, ILogger? logger = null)
        : this(new KnownMetricStorage(knownMetrics), logger)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SparkplugBase{T}"/> class.
    /// </summary>
    /// <param name="knownMetricsStorage">The known metrics storage.</param>
    /// <param name="logger">The logger.</param>
    public SparkplugBase(KnownMetricStorage knownMetricsStorage, ILogger? logger = null)
    {
        this._knonwMetrics = knownMetricsStorage;

        if (typeof(T).IsAssignableFrom(typeof(VersionAData.KuraMetric)))
        {
            this.NameSpace = SparkplugNamespace.VersionA;
        }
        else
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
    /// Gets the known metric names.
    /// </summary>
    public IEnumerable<T> KnownMetrics => this._knonwMetrics.Values;

    /// <summary>
    /// Gets the known metrics storage.
    /// </summary>
    /// <value>
    /// The known metrics storage.
    /// </value>
    public KnownMetricStorage KnownMetricsStorage => this._knonwMetrics;

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
