// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugNodeOptions.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class that contains the application options.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Core.Node;

using System.Xml.Serialization;
using Newtonsoft.Json;

/// <summary>
/// A class that contains the application options.
/// </summary>
[Serializable]
public class SparkplugNodeOptions : SparkplugBaseOptions
{
    /// <summary>
    /// The default groug identifier
    /// </summary>
    public const string DefaultGroupIdentifier = "";
    /// <summary>
    /// The default node identifier
    /// </summary>
    public const string DefaultNodeIdentifier = "";

    /// <summary>
    /// Initializes a new instance of the <see cref="SparkplugNodeOptions"/> class.
    /// </summary>
    public SparkplugNodeOptions()
        : this(brokerAddress: DefaultBroker)
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SparkplugNodeOptions"/> class.
    /// </summary>
    /// <param name="brokerAddress">The broker address.</param>
    /// <param name="port">The port.</param>
    /// <param name="clientId">The client identifier.</param>
    /// <param name="userName">Name of the user.</param>
    /// <param name="password">The password.</param>
    /// <param name="useTls">if set to <c>true</c> [use TLS].</param>
    /// <param name="scadaHostIdentifier">The scada host identifier.</param>
    /// <param name="groupIdentifier">The group identifier.</param>
    /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
    /// <param name="tlsParameters">The TLS parameters.</param>
    /// <param name="webSocketParameters">The web socket parameters.</param>
    /// <param name="proxyOptions">The proxy options.</param>
    public SparkplugNodeOptions(
        string brokerAddress = DefaultBroker,
        int port = DefaultPort,
        string clientId = DefaultClientId,
        string userName = DefaultUserName,
        string password = DefaultPassword,
        bool useTls = DefaultUseTls,
        string scadaHostIdentifier = DefaultScadaHostIdentifier,
        string groupIdentifier = DefaultGroupIdentifier,
        string edgeNodeIdentifier = DefaultNodeIdentifier,
        MqttClientOptionsBuilderTlsParameters? tlsParameters = null,
        MqttClientOptionsBuilderWebSocketParameters? webSocketParameters = null,
        MqttClientWebSocketProxyOptions? proxyOptions = null)
        : this(brokerAddress, port, clientId, userName, password, useTls, scadaHostIdentifier,
              reconnectInterval: TimeSpan.FromSeconds(30),
              groupIdentifier: groupIdentifier,
              edgeNodeIdentifier: edgeNodeIdentifier,
              tlsParameters: tlsParameters,
              webSocketParameters: webSocketParameters,
              proxyOptions: proxyOptions)
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SparkplugNodeOptions"/> class.
    /// </summary>
    /// <param name="brokerAddress">The broker address.</param>
    /// <param name="port">The port.</param>
    /// <param name="clientId">The client identifier.</param>
    /// <param name="userName">Name of the user.</param>
    /// <param name="password">The password.</param>
    /// <param name="useTls">if set to <c>true</c> [use TLS].</param>
    /// <param name="scadaHostIdentifier">The scada host identifier.</param>
    /// <param name="groupIdentifier">The group identifier.</param>
    /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
    /// <param name="reconnectInterval">The reconnect interval.</param>
    /// <param name="webSocketParameters">The web socket parameters.</param>
    /// <param name="proxyOptions">The proxy options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public SparkplugNodeOptions(
       string brokerAddress,
       int port ,
       string clientId ,
       string userName,
       string password,
       bool useTls ,
       string scadaHostIdentifier,
       string groupIdentifier,
       string edgeNodeIdentifier,
       TimeSpan reconnectInterval,
       MqttClientOptionsBuilderWebSocketParameters? webSocketParameters = null,
       MqttClientWebSocketProxyOptions? proxyOptions = null,
        CancellationToken? cancellationToken = null)
       : this(brokerAddress, port, clientId, userName, password, useTls, scadaHostIdentifier,
             reconnectInterval: reconnectInterval,
             groupIdentifier: groupIdentifier,
             edgeNodeIdentifier: edgeNodeIdentifier,
             tlsParameters: null,
             webSocketParameters: webSocketParameters,
             proxyOptions : proxyOptions,
             cancellationToken: cancellationToken)
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SparkplugNodeOptions"/> class.
    /// </summary>
    /// <param name="brokerAddress">The broker address.</param>
    /// <param name="port">The port.</param>
    /// <param name="clientId">The client identifier.</param>
    /// <param name="userName">Name of the user.</param>
    /// <param name="password">The password.</param>
    /// <param name="useTls">if set to <c>true</c> [use TLS].</param>
    /// <param name="scadaHostIdentifier">The scada host identifier.</param>
    /// <param name="groupIdentifier">The group identifier.</param>
    /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
    /// <param name="reconnectInterval">The reconnect interval.</param>
    /// <param name="tlsParameters">The TLS parameters.</param>
    /// <param name="webSocketParameters">The web socket parameters.</param>
    /// <param name="proxyOptions">The proxy options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
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
        MqttClientOptionsBuilderTlsParameters? tlsParameters = null,
        MqttClientOptionsBuilderWebSocketParameters? webSocketParameters = null,
        MqttClientWebSocketProxyOptions? proxyOptions = null,
        CancellationToken? cancellationToken = null)
        : base(brokerAddress: brokerAddress,
            port: port,
            clientId: clientId,
            userName: userName,
            password: password,
            useTls: useTls,
            scadaHostIdentifier: scadaHostIdentifier,
            reconnectInterval: reconnectInterval,
            tlsParameters: tlsParameters,
            webSocketParameters: webSocketParameters,
            proxyOptions: proxyOptions
            )
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
    [DefaultValue(DefaultNodeIdentifier)]
    public string EdgeNodeIdentifier { get; set; } = DefaultNodeIdentifier;

    /// <summary>
    /// Gets or sets the cancellation token.
    /// </summary>
    [JsonIgnore]
    [XmlIgnore]
    [Browsable(false)]
    public CancellationToken? CancellationToken { get; set; }
}
