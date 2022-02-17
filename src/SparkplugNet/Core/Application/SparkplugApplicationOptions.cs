// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugApplicationOptions.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class that contains the application options.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Core.Application;

/// <summary>
/// A class that contains the application options.
/// </summary>
public class SparkplugApplicationOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SparkplugApplicationOptions"/> class.
    /// </summary>
    /// <param name="brokerAddress">The broker address.</param>
    /// <param name="port">The broker port.</param>
    /// <param name="clientId">The client identifier.</param>
    /// <param name="userName">The user name.</param>
    /// <param name="password">The password.</param>
    /// <param name="useTls">A value indicating whether TLS should be used or not</param>
    /// <param name="scadaHostIdentifier">The SCADA host identifier.</param>
    /// <param name="reconnectInterval">The reconnect interval.</param>
    /// <param name="isPrimaryApplication">A value indicating whether the application the primary application or not. If this value is set to <c>true</c>, the application sends STATE messages, else not.</param>
    /// <param name="webSocketParameters">The WebSocket parameters.</param>
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
        bool isPrimaryApplication,
        MqttClientOptionsBuilderWebSocketParameters? webSocketParameters = null,
        MqttClientWebSocketProxyOptions? proxyOptions = null,
        CancellationToken? cancellationToken = null)
    {
        this.BrokerAddress = brokerAddress;
        this.Port = port;
        this.ClientId = clientId;
        this.UserName = userName;
        this.Password = password;
        this.UseTls = useTls;
        this.ScadaHostIdentifier = scadaHostIdentifier;
        this.ReconnectInterval = reconnectInterval;
        this.IsPrimaryApplication = isPrimaryApplication;
        this.WebSocketParameters = webSocketParameters;
        this.ProxyOptions = proxyOptions;
        this.CancellationToken = cancellationToken ?? SystemCancellationToken.None;
    }

    /// <summary>
    /// Gets or sets the broker address.
    /// </summary>
    public string BrokerAddress { get; set; }

    /// <summary>
    /// Gets or sets the port.
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// Gets or sets the client identifier.
    /// </summary>
    public string ClientId { get; set; }

    /// <summary>
    /// Gets or sets the user name.
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether TLS should be used or not.
    /// </summary>
    public bool UseTls { get; set; }

    /// <summary>
    /// Gets or sets the SCADA host identifier.
    /// </summary>
    public string ScadaHostIdentifier { get; set; }

    /// <summary>
    /// Gets or sets the reconnect interval.
    /// </summary>
    public TimeSpan ReconnectInterval { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the application the primary application or not.
    /// If this value is set to <c>true</c>, the application sends STATE messages, else not.
    /// </summary>
    public bool IsPrimaryApplication { get; set; }

    /// <summary>
    /// Gets or sets the WebSocket parameters.
    /// </summary>
    public MqttClientOptionsBuilderWebSocketParameters? WebSocketParameters { get; set; }

    /// <summary>
    /// Gets or sets the proxy options.
    /// </summary>
    public MqttClientWebSocketProxyOptions? ProxyOptions { get; set; }

    /// <summary>
    /// Gets or sets the cancellation token.
    /// </summary>
    public CancellationToken? CancellationToken { get; set; }
}
