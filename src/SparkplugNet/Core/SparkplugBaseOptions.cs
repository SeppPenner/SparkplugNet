// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugApplicationOptions.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   A base class that contains the Sparkplug options.
// </summary>

namespace SparkplugNet.Core;

/// <summary>
/// A base class that contains the Sparkplug options.
/// </summary>
public abstract class SparkplugBaseOptions
{
    /// <summary>
    /// The default broker.
    /// </summary>
    public const string DefaultBroker = "localhost";

    /// <summary>
    /// The default port.
    /// </summary>
    public const int DefaultPort = 1883;

    /// <summary>
    /// The default client identifier.
    /// </summary>
    public const string DefaultClientId = "SparkplugNet";

    /// <summary>
    /// The default user name.
    /// </summary>
    public const string DefaultUserName = "";

    /// <summary>
    /// The default password.
    /// </summary>
    public const string DefaultPassword = "";

    /// <summary>
    /// The default value whether TLS is used or not.
    /// </summary>
    public const bool DefaultUseTls = false;

    /// <summary>
    /// The default SCADA host identifier.
    /// </summary>
    public const string DefaultScadaHostIdentifier = "SparkplugNet";

    /// <summary>
    /// The default reconnect interval.
    /// </summary>
    public static readonly TimeSpan DefaultReconnectInterval = TimeSpan.FromSeconds(30);

    /// <summary>
    /// The default MQTT protocol version.
    /// </summary>
    public const SparkplugMqttProtocolVersion DefaultMqttProtocolVersion = SparkplugMqttProtocolVersion.V311;

    /// <summary>
    /// Returns a <see cref="MqttClientOptionsBuilderTlsParameters"/> instance or null.
    /// </summary>
    public delegate MqttClientOptionsBuilderTlsParameters? GetTlsParametersDelegate();

    /// <summary>
    /// Initializes a new instance of the <see cref="SparkplugBaseOptions"/> class.
    /// </summary>
    /// <param name="reconnectInterval">The reconnect interval.</param>
    /// <param name="brokerAddress">The broker address.</param>
    /// <param name="port">The port.</param>
    /// <param name="clientId">The client identifier.</param>
    /// <param name="userName">The name of the user.</param>
    /// <param name="password">The password.</param>
    /// <param name="useTls">A value indicating whether TLS should be used or not.</param>
    /// <param name="scadaHostIdentifier">The SCADA host identifier.</param>
    /// <param name="mqttProtocolVersion">The MQTT protocol version.</param>
    /// <param name="getTlsParameters">The delegate to provide TLS parameters.</param>
    /// <param name="webSocketParameters">The web socket parameters.</param>
    /// <param name="proxyOptions">The proxy options.</param>
    public SparkplugBaseOptions(
        string? brokerAddress = null,
        int? port = null,
        string? clientId = null,
        string? userName = null,
        string? password = null,
        bool? useTls = null,
        string? scadaHostIdentifier = null,
        TimeSpan? reconnectInterval = null,
        SparkplugMqttProtocolVersion? mqttProtocolVersion = null,
        GetTlsParametersDelegate? getTlsParameters = null,
        MqttClientOptionsBuilderWebSocketParameters? webSocketParameters = null,
        MqttClientWebSocketProxyOptions? proxyOptions = null)
    {
        this.BrokerAddress = string.IsNullOrWhiteSpace(brokerAddress) ? DefaultBroker : brokerAddress;
        this.Port = port ?? DefaultPort;
        this.ClientId = string.IsNullOrWhiteSpace(clientId) ? DefaultClientId : clientId;
        this.UserName = string.IsNullOrWhiteSpace(userName) ? DefaultUserName : userName;
        this.Password = string.IsNullOrWhiteSpace(password) ? DefaultPassword : password;
        this.UseTls = useTls ?? DefaultUseTls;
        this.ScadaHostIdentifier = string.IsNullOrWhiteSpace(scadaHostIdentifier) ? DefaultScadaHostIdentifier : scadaHostIdentifier;
        this.ReconnectInterval = reconnectInterval ?? DefaultReconnectInterval;
        this.MqttProtocolVersion = mqttProtocolVersion ?? DefaultMqttProtocolVersion;
        this.GetTlsParameters = getTlsParameters;
        this.WebSocketParameters = webSocketParameters;
        this.ProxyOptions = proxyOptions;
    }

    /// <summary>
    /// Gets or sets the broker address.
    /// </summary>
    [DefaultValue(DefaultBroker)]
    public string BrokerAddress { get; set; } = DefaultBroker;

    /// <summary>
    /// Gets or sets the port.
    /// </summary>
    [DefaultValue(DefaultPort)]
    public int Port { get; set; } = DefaultPort;

    /// <summary>
    /// Gets or sets the client identifier.
    /// </summary>
    [DefaultValue(DefaultClientId)]
    public string ClientId { get; set; } = DefaultClientId;

    /// <summary>
    /// Gets or sets the user name.
    /// </summary>
    [DefaultValue(DefaultUserName)]
    public string UserName { get; set; } = DefaultUserName;

    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    [DefaultValue(DefaultPassword)]
    public string Password { get; set; } = DefaultPassword;

    /// <summary>
    /// Gets or sets a value indicating whether TLS should be used or not.
    /// </summary>
    [DefaultValue(DefaultUseTls)]
    public bool UseTls { get; set; } = DefaultUseTls;

    /// <summary>
    /// Gets or sets the SCADA host identifier.
    /// </summary>
    [DefaultValue(DefaultScadaHostIdentifier)]
    public string ScadaHostIdentifier { get; set; } = DefaultScadaHostIdentifier;

    /// <summary>
    /// Gets or sets the reconnect interval.
    /// </summary>
    public TimeSpan ReconnectInterval { get; set; } = DefaultReconnectInterval;

    /// <summary>
    /// Gets or sets the MQTT protocol version.
    /// </summary>
    [DefaultValue(DefaultMqttProtocolVersion)]
    public SparkplugMqttProtocolVersion MqttProtocolVersion { get; set; } = DefaultMqttProtocolVersion;

    /// <summary>
    /// Gets or sets the delegate to provide TLS parameters.
    /// </summary>
    [XmlIgnore]
    [JsonIgnore]
    [DefaultValue(null)]
    public GetTlsParametersDelegate? GetTlsParameters { get; set; }

    /// <summary>
    /// Gets or sets the WebSocket parameters.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DefaultValue(null)]
    public MqttClientOptionsBuilderWebSocketParameters? WebSocketParameters { get; set; }

    /// <summary>
    /// Gets or sets the proxy options.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DefaultValue(null)]
    public MqttClientWebSocketProxyOptions? ProxyOptions { get; set; }
}
