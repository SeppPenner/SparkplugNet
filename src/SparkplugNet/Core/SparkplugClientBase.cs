// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugNode.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class that handles a Sparkplug node.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Authentication;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using MQTTnet;
    using MQTTnet.Client;
    using MQTTnet.Client.Options;
    using MQTTnet.Client.Publishing;
    using MQTTnet.Formatter;
    using MQTTnet.Protocol;
    using SparkplugNet.Core.Enumerations;
    using SparkplugNet.Core.Extensions;
    using SparkplugNet.Core.Node;
    using VersionAPayload = SparkplugNet.VersionA.Payload;
    using VersionBPayload = SparkplugNet.VersionB.Payload;

    /// <inheritdoc cref="SparkplugBase{T}" />
    /// <summary>A class that handles a Sparkplug MQTT Client connection.</summary>
    /// <seealso cref="SparkplugBase{T}" />
    public class SparkplugClientBase<T> : SparkplugBase<T>, IDisposable
        where T : class, new()
    {
        /// <summary>The log message action</summary>
        public Action<string>? LogAction = null;

        /// <summary>The exception thrown action</summary>
        public Action<Exception>? OnException = null;

        /// <summary>The callback for the device command received event.</summary>
        public Action<string, T>? DeviceCommandReceived = null;

        /// <summary>The callback for the node command received event.</summary>
        public Action<string, T>? NodeCommandReceived = null;

        /// <summary>The callback for the status message received event.</summary>
        public Action<string>? StatusMessageReceived = null;

        /// <summary>The callback for the disconnected event. Indicates that metrics might be stale.</summary>
        public Action? OnDisconnected = null;

        /// <summary>The callback for the connected event. Indicates that a new MQTT connection was established and should trigger startup messages.</summary>
        public Action? OnConnected = null;

        /// <summary>The MQTT client.</summary>
        internal readonly IMqttClient Client;

        /// <inheritdoc cref="SparkplugBase{T}" />
        /// <summary>Initializes a new instance of the <see cref="SparkplugNodeBase{T}" /> class.</summary>
        /// <param name="knownMetrics">The metric names.</param>
        /// <seealso cref="SparkplugBase{T}" />
        public SparkplugClientBase(List<T> knownMetrics)
            : base(knownMetrics)
        {
            this.Client = new MqttFactory().CreateMqttClient();
        }

        /// <summary>Gets a value indicating whether this instance is connected.</summary>
        public bool IsConnected => this.Client.IsConnected;

        /// <summary>Gets or sets the MQTT client options.</summary>
        internal IMqttClientOptions? ClientOptions { get; set; }

        /// <summary>Stops the Sparkplug node.</summary>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
#pragma warning disable 1998
        public virtual async Task Stop()
#pragma warning restore 1998
        {
        }

        /// <summary>Closes the Sparkplug node.</summary>
#pragma warning disable 1998
        public virtual async Task Close()
#pragma warning restore 1998
        {
            this.Client.Dispose();
        }

        /// <summary>Disposes this instance.</summary>
        public virtual void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Releases unmanaged and - optionally - managed resources.</summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Client.Dispose();
            }
        }
    }
}