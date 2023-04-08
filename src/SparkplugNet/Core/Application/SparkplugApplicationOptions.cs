// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugApplicationOptions.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class that contains the Sparkplug application options.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Core.Application;

/// <inheritdoc cref="SparkplugBaseOptions"/>
/// <summary>
/// A class that contains the Sparkplug application options.
/// </summary>
/// <seealso cref="SparkplugBaseOptions"/>
[Serializable]
public class SparkplugApplicationOptions : SparkplugBaseOptions
{
    /// <summary>
    /// The default value whether the application is a primary application or not.
    /// </summary>
    public const bool DefaultIsPrimaryApplication = false;

    /// <inheritdoc cref="SparkplugBaseOptions"/>
    /// <summary>
    /// Initializes a new instance of the <see cref="SparkplugApplicationOptions"/> class.
    /// </summary>
    /// <param name="brokerAddress">The broker address.</param>
    /// <param name="port">The port.</param>
    /// <param name="clientId">The client identifier.</param>
    /// <param name="userName">The name of the user.</param>
    /// <param name="password">The password.</param>
    /// <param name="useTls">A value indicating whether TLS is used or not.</param>
    /// <param name="scadaHostIdentifier">The SCADA host identifier.</param>
    /// <param name="reconnectInterval">The reconnect interval.</param>
    /// <param name="mqttProtocolVersion">The MQTT protocol version.</param>
    /// <param name="isPrimaryApplication">A value indicating whether the application is a primary application or not.</param>
    /// <param name="getTlsParameters">The delegate to provide TLS parameters.</param>
    /// <param name="webSocketParameters">The web socket parameters.</param>
    /// <param name="proxyOptions">The proxy options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <seealso cref="SparkplugBaseOptions"/>
    public SparkplugApplicationOptions(
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
        MqttClientWebSocketProxyOptions? proxyOptions = null,
        bool? isPrimaryApplication = null,
        SystemCancellationToken? cancellationToken = null)
        : base(
            brokerAddress,
            port,
            clientId,
            userName,
            password,
            useTls,
            scadaHostIdentifier,
            reconnectInterval,
            mqttProtocolVersion,
            getTlsParameters,
            webSocketParameters,
            proxyOptions)
    {
        this.IsPrimaryApplication = isPrimaryApplication ?? DefaultIsPrimaryApplication;
        this.CancellationToken = cancellationToken ?? SystemCancellationToken.None;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the application the primary application or not.
    /// If this value is set to <c>true</c>, the application sends STATE messages, else not.
    /// </summary>
    [DefaultValue(DefaultIsPrimaryApplication)]
    public bool IsPrimaryApplication { get; set; } = DefaultIsPrimaryApplication;

    /// <summary>
    /// Gets or sets the cancellation token.
    /// </summary>
    [JsonIgnore]
    [XmlIgnore]
    [Browsable(false)]
    public SystemCancellationToken? CancellationToken { get; set; } = SystemCancellationToken.None;
}
