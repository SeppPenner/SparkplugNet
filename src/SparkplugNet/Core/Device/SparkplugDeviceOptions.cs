// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugDeviceOptions.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class that contains the device options.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Core.Device
{
    using System;

    using MQTTnet.Client.Options;

    using CancelToken = System.Threading.CancellationToken;

    /// <summary>
    /// A class that contains the device options.
    /// </summary>
    public class SparkplugDeviceOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SparkplugDeviceOptions" /> class.
        /// </summary>
        /// <param name="brokerAddress">The broker address.</param>
        /// <param name="port">The broker port.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="userName">The user name.</param>
        /// <param name="password">The password.</param>
        /// <param name="useTls">A value indicating whether TLS should be used or not</param>
        /// <param name="scadaHostIdentifier">The SCADA host identifier.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="deviceIdentifier">The device identifier.</param>
        /// <param name="deviceGuid">The device unique identifier.</param>
        /// <param name="reconnectInterval">The reconnect interval.</param>
        /// <param name="webSocketParameters">The WebSocket parameters.</param>
        /// <param name="proxyOptions">The proxy options.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public SparkplugDeviceOptions(
            string brokerAddress,
            int port,
            string clientId,
            string userName,
            string password,
            bool useTls,
            string scadaHostIdentifier,
            string groupIdentifier,
            string edgeNodeIdentifier,
            string deviceIdentifier,
            Guid deviceGuid,
            TimeSpan reconnectInterval,
            MqttClientOptionsBuilderWebSocketParameters? webSocketParameters = null,
            MqttClientWebSocketProxyOptions? proxyOptions = null,
            CancelToken? cancellationToken = null)
        {
            this.BrokerAddress = brokerAddress;
            this.Port = port;
            this.ClientId = clientId;
            this.UserName = userName;
            this.Password = password;
            this.UseTls = useTls;
            this.ScadaHostIdentifier = scadaHostIdentifier;
            this.GroupIdentifier = groupIdentifier;
            this.EdgeNodeIdentifier = edgeNodeIdentifier;
            this.DeviceIdentifier = deviceIdentifier;
            this.DeviceGuid = deviceGuid;
            this.ReconnectInterval = reconnectInterval;
            this.WebSocketParameters = webSocketParameters;
            this.ProxyOptions = proxyOptions;
            this.CancellationToken = cancellationToken ?? CancelToken.None;
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
        /// Gets or sets the group identifier.
        /// </summary>
        public string GroupIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the edge node identifier.
        /// </summary>
        public string EdgeNodeIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the device identifier.
        /// </summary>
        public string DeviceIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the device unique identifier.
        /// </summary>
        public Guid DeviceGuid { get; set; }

        /// <summary>
        /// Gets or sets the reconnect interval.
        /// </summary>
        public TimeSpan ReconnectInterval { get; set; }

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
        public CancelToken? CancellationToken { get; set; }
    }
}