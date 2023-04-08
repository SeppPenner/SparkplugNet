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

    // Todo: Check if we still need this.
    /// <summary>
    /// The default option to publish known device metrics on reconnect.
    /// </summary>
    public const bool DefaultPublishKnownDeviceMetricsOnReconnect = true;

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
    /// <param name="mqttProtocolVersion">The MQTT protocol version.</param>
    /// <param name="getTlsParameters">The delegate to provide TLS parameters.</param>
    /// <param name="webSocketParameters">The web socket parameters.</param>
    /// <param name="proxyOptions">The proxy options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <seealso cref="SparkplugBaseOptions"/>
    public SparkplugNodeOptions(
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
        string? groupIdentifier = null,
        string? edgeNodeIdentifier = null,
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
        this.GroupIdentifier = string.IsNullOrWhiteSpace(groupIdentifier) ? DefaultGroupIdentifier : groupIdentifier;
        this.EdgeNodeIdentifier = string.IsNullOrWhiteSpace(edgeNodeIdentifier) ? DefaultEdgeNodeIdentifier : edgeNodeIdentifier;
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

    // Todo: Check if we still need this.
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
    public SystemCancellationToken? CancellationToken { get; set; } = SystemCancellationToken.None;
}
