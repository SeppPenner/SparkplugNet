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
    /// The default option to add session numbers to command messages.
    /// </summary>
    public const bool DefaultAddSessionNumberToCommandMessages = true;

    /// <inheritdoc cref="SparkplugBaseOptions"/>
    /// <summary>
    /// For serializers only.
    /// Initializes a new instance of the <see cref="SparkplugApplicationOptions"/> class.
    /// </summary>
    /// <seealso cref="SparkplugBaseOptions"/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public SparkplugApplicationOptions() : this(brokerAddress: DefaultBroker)
    {
    }

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
    /// <param name="isPrimaryApplication">A value indicating whether the application is a primary application or not.</param>
    /// <param name="getTlsParameters">The delegate to provide TLS parameters.</param>
    /// <param name="webSocketParameters">The web socket parameters.</param>
    /// <param name="proxyOptions">The proxy options.</param>
    /// <seealso cref="SparkplugBaseOptions"/>
    public SparkplugApplicationOptions(
        string brokerAddress = DefaultBroker,
        int port = DefaultPort,
        string clientId = DefaultClientId,
        string userName = DefaultUserName,
        string password = DefaultPassword,
        bool useTls = DefaultUseTls,
        string scadaHostIdentifier = DefaultScadaHostIdentifier,
        bool isPrimaryApplication = false,
        GetTlsParametersDelegate? getTlsParameters = null,
        MqttClientOptionsBuilderWebSocketParameters? webSocketParameters = null,
        MqttClientWebSocketProxyOptions? proxyOptions = null)
        : this(brokerAddress, port, clientId, userName, password, useTls, scadaHostIdentifier,
              reconnectInterval: TimeSpan.FromSeconds(30),
              isPrimaryApplication: isPrimaryApplication,
              getTlsParameters: getTlsParameters,
              webSocketParameters: webSocketParameters,
              proxyOptions: proxyOptions)
    {
    }

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
    /// <param name="isPrimaryApplication">A value indicating whether the application is a primary application or not.</param>
    /// <param name="webSocketParameters">The web socket parameters.</param>
    /// <param name="proxyOptions">The proxy options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <seealso cref="SparkplugBaseOptions"/>
    public SparkplugApplicationOptions(
        string brokerAddress,
        int port,
        string clientId,
        string userName,
        string password,
        bool useTls,
        string scadaHostIdentifier,
        TimeSpan reconnectInterval,
        bool isPrimaryApplication = false,
        MqttClientOptionsBuilderWebSocketParameters? webSocketParameters = null,
        MqttClientWebSocketProxyOptions? proxyOptions = null,
        SystemCancellationToken? cancellationToken = null)
        : this(brokerAddress, port, clientId, userName, password, useTls, scadaHostIdentifier,
          reconnectInterval: reconnectInterval,
          isPrimaryApplication: isPrimaryApplication,
          getTlsParameters: null,
          webSocketParameters: webSocketParameters,
          proxyOptions: proxyOptions,
          cancellationToken: cancellationToken)
    {
    }

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
    /// <param name="isPrimaryApplication">A value indicating whether the application is a primary application or not.</param>
    /// <param name="getTlsParameters">The delegate to provide TLS parameters.</param>
    /// <param name="webSocketParameters">The web socket parameters.</param>
    /// <param name="proxyOptions">The proxy options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <seealso cref="SparkplugBaseOptions"/>
    public SparkplugApplicationOptions(
        string brokerAddress,
        int port,
        string clientId,
        string userName,
        string password,
        bool useTls,
        string scadaHostIdentifier,
        TimeSpan reconnectInterval,
        bool isPrimaryApplication = false,
        GetTlsParametersDelegate? getTlsParameters = null,
        MqttClientOptionsBuilderWebSocketParameters? webSocketParameters = null,
        MqttClientWebSocketProxyOptions? proxyOptions = null,
        SystemCancellationToken? cancellationToken = null)
        : base(brokerAddress: brokerAddress,
            port: port,
            clientId: clientId,
            userName: userName,
            password: password,
            useTls: useTls,
            scadaHostIdentifier: scadaHostIdentifier,
            reconnectInterval: reconnectInterval,
            getTlsParameters: getTlsParameters,
            webSocketParameters: webSocketParameters,
            proxyOptions: proxyOptions)
    {
        this.IsPrimaryApplication = isPrimaryApplication;
        this.CancellationToken = cancellationToken ?? SystemCancellationToken.None;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the application the primary application or not.
    /// If this value is set to <c>true</c>, the application sends STATE messages, else not.
    /// </summary>
    public bool IsPrimaryApplication { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether session messages are added to command messages or not.
    /// </summary>
    [DefaultValue(DefaultAddSessionNumberToCommandMessages)]
    public bool AddSessionNumberToCommandMessages { get; set; } = DefaultAddSessionNumberToCommandMessages;

    /// <summary>
    /// Gets or sets the cancellation token.
    /// </summary>
    [JsonIgnore]
    [XmlIgnore]
    [Browsable(false)]
    public SystemCancellationToken? CancellationToken { get; set; }
}
