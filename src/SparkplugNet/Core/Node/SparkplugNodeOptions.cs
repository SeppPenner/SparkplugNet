// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugNodeOptions.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class that contains the node options.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Core.Node;

/// <inheritdoc cref="SparkplugBaseOptions"/>
/// <summary>
/// A class that contains the node options.
/// </summary>
/// <seealso cref="SparkplugBaseOptions"/>
[Serializable]
public class SparkplugNodeOptions : SparkplugBaseOptions
{
    /// <summary>
    /// The default groug identifier.
    /// </summary>
    public const string DefaultGroupIdentifier = "";

    /// <summary>
    /// The default edge node identifier.
    /// </summary>
    public const string DefaultEdgeNodeIdentifier = "";

    /// <summary>
    /// The default option to add session numbers to data messages.
    /// </summary>
    public const bool DefaultAddSessionNumberToDataMessages = true;

    /// <summary>
    /// The default option to publish known device metrics on reconnect.
    /// </summary>
    public const bool DefaultPublishKnownDeviceMetricsOnReconnect = true;

    /// <inheritdoc cref="SparkplugBaseOptions"/>
    /// <summary>
    /// Initializes a new instance of the <see cref="SparkplugNodeOptions"/> class.
    /// </summary>
    /// <seealso cref="SparkplugBaseOptions"/>
    public SparkplugNodeOptions() : this(brokerAddress: DefaultBroker)
    {
    }

    /// <inheritdoc cref="SparkplugBaseOptions"/>
    /// <summary>
    /// Initializes a new instance of the <see cref="SparkplugNodeOptions"/> class.
    /// </summary>
    /// <param name="brokerAddress">The broker address.</param>
    /// <param name="port">The port.</param>
    /// <param name="clientId">The client identifier.</param>
    /// <param name="userName">The name of the user.</param>
    /// <param name="password">The password.</param>
    /// <param name="useTls">A value indicating whether TLS is used or not.</param>
    /// <param name="scadaHostIdentifier">The SCADA host identifier.</param>
    /// <param name="groupIdentifier">The group identifier.</param>
    /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
    /// <param name="getTlsParameters">The delegate to provide TLS parameters.</param>
    /// <param name="webSocketParameters">The web socket parameters.</param>
    /// <param name="proxyOptions">The proxy options.</param>
    /// <seealso cref="SparkplugBaseOptions"/>
    public SparkplugNodeOptions(
        string brokerAddress = DefaultBroker,
        int port = DefaultPort,
        string clientId = DefaultClientId,
        string userName = DefaultUserName,
        string password = DefaultPassword,
        bool useTls = DefaultUseTls,
        string scadaHostIdentifier = DefaultScadaHostIdentifier,
        string groupIdentifier = DefaultGroupIdentifier,
        string edgeNodeIdentifier = DefaultEdgeNodeIdentifier,
        GetTlsParametersDelegate? getTlsParameters = null,
        MqttClientOptionsBuilderWebSocketParameters? webSocketParameters = null,
        MqttClientWebSocketProxyOptions? proxyOptions = null)
        : this(brokerAddress, port, clientId, userName, password, useTls, scadaHostIdentifier,
              reconnectInterval: TimeSpan.FromSeconds(30),
              groupIdentifier: groupIdentifier,
              edgeNodeIdentifier: edgeNodeIdentifier,
              getTlsParameters: getTlsParameters,
              webSocketParameters: webSocketParameters,
              proxyOptions: proxyOptions)
    {
    }

    /// <inheritdoc cref="SparkplugBaseOptions"/>
    /// <summary>
    /// Initializes a new instance of the <see cref="SparkplugNodeOptions"/> class.
    /// </summary>
    /// <param name="brokerAddress">The broker address.</param>
    /// <param name="port">The port.</param>
    /// <param name="clientId">The client identifier.</param>
    /// <param name="userName">The name of the user.</param>
    /// <param name="password">The password.</param>
    /// <param name="useTls">A value indicating whether TLS is used or not.</param>
    /// <param name="scadaHostIdentifier">The SCADA host identifier.</param>
    /// <param name="groupIdentifier">The group identifier.</param>
    /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
    /// <param name="reconnectInterval">The reconnect interval.</param>
    /// <param name="webSocketParameters">The web socket parameters.</param>
    /// <param name="proxyOptions">The proxy options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <seealso cref="SparkplugBaseOptions"/>
    public SparkplugNodeOptions(
       string brokerAddress,
       int port,
       string clientId,
       string userName,
       string password,
       bool useTls,
       string scadaHostIdentifier,
       string groupIdentifier,
       string edgeNodeIdentifier,
       TimeSpan reconnectInterval,
       MqttClientOptionsBuilderWebSocketParameters? webSocketParameters = null,
       MqttClientWebSocketProxyOptions? proxyOptions = null,
       SystemCancellationToken? cancellationToken = null)
       : this(brokerAddress, port, clientId, userName, password, useTls, scadaHostIdentifier,
             reconnectInterval: reconnectInterval,
             groupIdentifier: groupIdentifier,
             edgeNodeIdentifier: edgeNodeIdentifier,
             getTlsParameters: null,
             webSocketParameters: webSocketParameters,
             proxyOptions: proxyOptions,
             cancellationToken: cancellationToken)
    {
    }

    /// <inheritdoc cref="SparkplugBaseOptions"/>
    /// <summary>
    /// Initializes a new instance of the <see cref="SparkplugNodeOptions"/> class.
    /// </summary>
    /// <param name="brokerAddress">The broker address.</param>
    /// <param name="port">The port.</param>
    /// <param name="clientId">The client identifier.</param>
    /// <param name="userName">The name of the user.</param>
    /// <param name="password">The password.</param>
    /// <param name="useTls">A value indicating whether TLS is used or not.</param>
    /// <param name="scadaHostIdentifier">The SCADA host identifier.</param>
    /// <param name="groupIdentifier">The group identifier.</param>
    /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
    /// <param name="reconnectInterval">The reconnect interval.</param>
    /// <param name="getTlsParameters">The delegate to provide TLS parameters.</param>
    /// <param name="webSocketParameters">The web socket parameters.</param>
    /// <param name="proxyOptions">The proxy options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <seealso cref="SparkplugBaseOptions"/>
    public SparkplugNodeOptions(
        string brokerAddress,
        int port,
        string clientId,
        string userName,
        string password,
        bool useTls,
        string scadaHostIdentifier,
        string groupIdentifier,
        string edgeNodeIdentifier,
        TimeSpan reconnectInterval,
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
        this.GroupIdentifier = groupIdentifier;
        this.EdgeNodeIdentifier = edgeNodeIdentifier;
        this.CancellationToken = cancellationToken ?? SystemCancellationToken.None;
    }

    /// <summary>
    /// Gets or sets the group identifier.
    /// </summary>
    [DefaultValue(DefaultGroupIdentifier)]
    public string GroupIdentifier { get; set; } = DefaultGroupIdentifier;

    /// <summary>
    /// Gets or sets the edge node identifier.
    /// </summary>
    [DefaultValue(DefaultEdgeNodeIdentifier)]
    public string EdgeNodeIdentifier { get; set; } = DefaultEdgeNodeIdentifier;

    /// <summary>
    /// Gets or sets a value indicating whether to add session numbers to data messages or not.
    /// </summary>
    [DefaultValue(DefaultAddSessionNumberToDataMessages)]
    public bool AddSessionNumberToDataMessages { get; set; } = DefaultAddSessionNumberToDataMessages;

    /// <summary>
    /// Gets or sets a value indicating whether to publish known device metrics on reconnect or not.
    /// </summary>
    [DefaultValue(DefaultPublishKnownDeviceMetricsOnReconnect)]
    public bool PublishKnownDeviceMetricsOnReconnect { get; set; } = DefaultPublishKnownDeviceMetricsOnReconnect;

    /// <summary>
    /// Gets or sets the cancellation token.
    /// </summary>
    [JsonIgnore]
    [XmlIgnore]
    [Browsable(false)]
    public SystemCancellationToken? CancellationToken { get; set; }
}
