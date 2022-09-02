// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugApplicationOptions.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class that contains the application options.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Core.Application;

using System.Xml.Serialization;
using Newtonsoft.Json;

/// <summary>
/// A class that contains the application options.
/// </summary>
[Serializable]
public class SparkplugApplicationOptions : SparkplugBaseOptions
{
    /// <summary>
    /// For serializers only
    /// Initializes a new instance of the <see cref="SparkplugApplicationOptions"/> class.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public SparkplugApplicationOptions()
        : this(brokerAddress: DefaultBroker)
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SparkplugApplicationOptions"/> class.
    /// </summary>
    /// <param name="brokerAddress">The broker address.</param>
    /// <param name="port">The port.</param>
    /// <param name="clientId">The client identifier.</param>
    /// <param name="userName">Name of the user.</param>
    /// <param name="password">The password.</param>
    /// <param name="useTls">if set to <c>true</c> [use TLS].</param>
    /// <param name="scadaHostIdentifier">The scada host identifier.</param>
    /// <param name="isPrimaryApplication">if set to <c>true</c> [is primary application].</param>
    /// <param name="tlsParameters">The TLS parameters.</param>
    /// <param name="webSocketParameters">The web socket parameters.</param>
    /// <param name="proxyOptions">The proxy options.</param>
    public SparkplugApplicationOptions(
        string brokerAddress = DefaultBroker,
        int port = DefaultPort,
        string clientId = DefaultClientId,
        string userName = DefaultUserName,
        string password = DefaultPassword,
        bool useTls = DefaultUseTls,
        string scadaHostIdentifier = DefaultScadaHostIdentifier,
        bool isPrimaryApplication = false,
        MqttClientOptionsBuilderTlsParameters? tlsParameters = null,
        MqttClientOptionsBuilderWebSocketParameters? webSocketParameters = null,
        MqttClientWebSocketProxyOptions? proxyOptions = null
)
        : this(brokerAddress, port, clientId, userName, password, useTls, scadaHostIdentifier,
              reconnectInterval: TimeSpan.FromSeconds(30),
              isPrimaryApplication: isPrimaryApplication,
              tlsParameters: tlsParameters,
              webSocketParameters: webSocketParameters,
              proxyOptions: proxyOptions)
    {

    }


    /// <summary>
    /// Initializes a new instance of the <see cref="SparkplugApplicationOptions"/> class.
    /// </summary>
    /// <param name="brokerAddress">The broker address.</param>
    /// <param name="port">The port.</param>
    /// <param name="clientId">The client identifier.</param>
    /// <param name="userName">Name of the user.</param>
    /// <param name="password">The password.</param>
    /// <param name="useTls">if set to <c>true</c> [use TLS].</param>
    /// <param name="scadaHostIdentifier">The scada host identifier.</param>
    /// <param name="reconnectInterval">The reconnect interval.</param>
    /// <param name="isPrimaryApplication">if set to <c>true</c> [is primary application].</param>
    /// <param name="webSocketParameters">The web socket parameters.</param>
    /// <param name="proxyOptions">The proxy options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public SparkplugApplicationOptions(
    string brokerAddress,
    int port ,
    string clientId ,
    string userName ,
    string password,
    bool useTls,
    string scadaHostIdentifier,
    TimeSpan reconnectInterval,
    bool isPrimaryApplication = false,
    MqttClientOptionsBuilderWebSocketParameters? webSocketParameters = null,
    MqttClientWebSocketProxyOptions? proxyOptions = null,
        CancellationToken? cancellationToken = null)
    : this(brokerAddress, port, clientId, userName, password, useTls, scadaHostIdentifier,
          reconnectInterval: reconnectInterval,
          isPrimaryApplication: isPrimaryApplication,
          tlsParameters: null,
          webSocketParameters: webSocketParameters,
          proxyOptions: proxyOptions,
          cancellationToken: cancellationToken)
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SparkplugApplicationOptions"/> class.
    /// </summary>
    /// <param name="brokerAddress">The broker address.</param>
    /// <param name="port">The port.</param>
    /// <param name="clientId">The client identifier.</param>
    /// <param name="userName">Name of the user.</param>
    /// <param name="password">The password.</param>
    /// <param name="useTls">if set to <c>true</c> [use TLS].</param>
    /// <param name="scadaHostIdentifier">The scada host identifier.</param>
    /// <param name="reconnectInterval">The reconnect interval.</param>
    /// <param name="isPrimaryApplication">if set to <c>true</c> [is primary application].</param>
    /// <param name="tlsParameters">The TLS parameters.</param>
    /// <param name="webSocketParameters">The web socket parameters.</param>
    /// <param name="proxyOptions">The proxy options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
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

        this.IsPrimaryApplication = isPrimaryApplication;
        this.CancellationToken = cancellationToken ?? SystemCancellationToken.None;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the application the primary application or not.
    /// If this value is set to <c>true</c>, the application sends STATE messages, else not.
    /// </summary>
    public bool IsPrimaryApplication { get; set; } = false;

    /// <summary>
    /// Gets or sets the cancellation token.
    /// </summary>
    [JsonIgnore]
    [XmlIgnore]
    public CancellationToken? CancellationToken { get; set; }
}
