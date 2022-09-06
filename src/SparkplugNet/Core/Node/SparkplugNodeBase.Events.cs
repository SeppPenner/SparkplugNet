namespace SparkplugNet.Core.Node
{
    using MQTTnet.Internal;

    public abstract partial class SparkplugNodeBase<T> : SparkplugBase<T> where T : IMetric, new()
    {
        #region DeviceCommandReceived
        /// <summary>
        /// The device command received event
        /// </summary>
        protected AsyncEvent<CommandEventArgs> _deviceCommandReceivedEvent = new AsyncEvent<CommandEventArgs>();

        /// <summary>
        /// Gets or sets the callback for the device command received event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        [Obsolete("Please use DeviceCommandReceivedAsync", false)]
        public Action<T>? DeviceCommandReceived { get; set; } = null;

        /// <summary>
        /// Occurs when [device command received asynchronous].
        /// </summary>
        public event Func<CommandEventArgs, Task> DeviceCommandReceivedAsync
        {
            add => this._deviceCommandReceivedEvent.AddHandler(value);
            remove => this._deviceCommandReceivedEvent.RemoveHandler(value);
        }

        /// <summary>
        /// Fires the device command received asynchronous.
        /// </summary>
        /// <param name="metric">The metric.</param>
        /// <returns></returns>
        protected virtual Task FireDeviceCommandReceivedAsync(T metric)
        {
#pragma warning disable CS0618 // Typ oder Element ist veraltet
            this.DeviceCommandReceived?.Invoke(metric);
#pragma warning restore CS0618 // Typ oder Element ist veraltet

            return this._deviceCommandReceivedEvent.InvokeAsync(new CommandEventArgs(this, metric));
        }
        #endregion

        #region NodeCommandReceived         
        /// <summary>
        /// The node command received event
        /// </summary>
        protected AsyncEvent<CommandEventArgs> _nodeCommandReceivedEvent = new AsyncEvent<CommandEventArgs>();
        /// <summary>
        /// Gets or sets the callback for the node command received event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        [Obsolete("Please use DeviceCommandReceivedAsync", false)]
        public Action<T>? NodeCommandReceived { get; set; } = null;

        /// <summary>
        /// Occurs when [Node command received asynchronous].
        /// </summary>
        public event Func<CommandEventArgs, Task> NodeCommandReceivedAsync
        {
            add => this._nodeCommandReceivedEvent.AddHandler(value);
            remove => this._nodeCommandReceivedEvent.RemoveHandler(value);
        }

        /// <summary>
        /// Fires the Node command received asynchronous.
        /// </summary>
        /// <param name="metric">The metric.</param>
        /// <returns></returns>
        protected virtual Task FireNodeCommandReceivedAsync(T metric)
        {
#pragma warning disable CS0618 // Typ oder Element ist veraltet
            this.NodeCommandReceived?.Invoke(metric);
#pragma warning restore CS0618 // Typ oder Element ist veraltet

            return this._nodeCommandReceivedEvent.InvokeAsync(new CommandEventArgs(this, metric));
        }
        #endregion

        #region StatusMessageReceived
        /// <summary>
        /// The status message received event
        /// </summary>
        protected AsyncEvent<StatusMessageEvnetArgs> _statusMessageReceivedEvent = new AsyncEvent<StatusMessageEvnetArgs>();
        /// <summary>
        /// Gets or sets the callback for the status message received event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        [Obsolete("Please use DeviceCommandReceivedAsync", false)]
        public Action<string>? StatusMessageReceived { get; set; } = null;
        /// <summary>
        /// Occurs when [Node command received asynchronous].
        /// </summary>
        public event Func<StatusMessageEvnetArgs, Task> StatusMessageReceivedAsync
        {
            add => this._statusMessageReceivedEvent.AddHandler(value);
            remove => this._statusMessageReceivedEvent.RemoveHandler(value);
        }

        /// <summary>
        /// Fires the status message received asynchronous.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <returns></returns>
        protected virtual Task FireStatusMessageReceivedAsync(string status)
        {
#pragma warning disable CS0618 // Typ oder Element ist veraltet
            this.StatusMessageReceived?.Invoke(status);
#pragma warning restore CS0618 // Typ oder Element ist veraltet

            return this._statusMessageReceivedEvent.InvokeAsync(new StatusMessageEvnetArgs(this, status));
        }
        #endregion

        #region DeviceBirthPublishing
        /// <summary>
        /// The status message received event
        /// </summary>
        protected AsyncEvent<DeviceBirthEventArgs> _DeviceBirthPublishingEvent = new AsyncEvent<DeviceBirthEventArgs>();
        /// <summary>
        /// Gets or sets the callback for the device birth received event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        [Obsolete("Please use DeviceBirthSendingAsync", false)]
        public Action<KeyValuePair<string, List<T>>>? DeviceBirthReceived { get; set; } = null;
        /// <summary>
        /// Occurs when [Node command received asynchronous].
        /// </summary>
        public event Func<DeviceBirthEventArgs, Task> DeviceBirthPublishingAsync
        {
            add => this._DeviceBirthPublishingEvent.AddHandler(value);
            remove => this._DeviceBirthPublishingEvent.RemoveHandler(value);
        }

        /// <summary>
        /// Fires the status message received asynchronous.
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <param name="metrics">The metrics</param>
        /// <returns></returns>
        protected virtual Task FireDeviceBirthPublishingAsync(string deviceId, IEnumerable<T> metrics)
        {
#pragma warning disable CS0618 // Typ oder Element ist veraltet
            this.DeviceBirthReceived?.Invoke(new KeyValuePair<string, List<T>>(deviceId, metrics.ToList()));
#pragma warning restore CS0618 // Typ oder Element ist veraltet

            return this._DeviceBirthPublishingEvent.InvokeAsync(new DeviceBirthEventArgs(this, this.Options!.EdgeNodeIdentifier, deviceId, metrics));
        }
        #endregion

        #region DeviceDeathReceived
        /// <summary>
        /// The status message received event
        /// </summary>
        protected AsyncEvent<DeviceEventArgs> _DeviceDeathPublishingEvent = new AsyncEvent<DeviceEventArgs>();
        /// <summary>
        /// Gets or sets the callback for the device death received event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        [Obsolete("Please use DeviceDeathPublishingAsync", false)]
        public Action<string>? DeviceDeathReceived { get; set; } = null;
        /// <summary>
        /// Occurs when [Node command received asynchronous].
        /// </summary>
        public event Func<DeviceEventArgs, Task> DeviceDeathPublishingAsync
        {
            add => this._DeviceDeathPublishingEvent.AddHandler(value);
            remove => this._DeviceDeathPublishingEvent.RemoveHandler(value);
        }

        /// <summary>
        /// Fires the status message received asynchronous.
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <returns></returns>
        protected virtual Task FireDeviceDeathPublishingAsync(string deviceId)
        {
#pragma warning disable CS0618 // Typ oder Element ist veraltet
            this.DeviceDeathReceived?.Invoke(deviceId);
#pragma warning restore CS0618 // Typ oder Element ist veraltet

            return this._DeviceDeathPublishingEvent.InvokeAsync(new DeviceEventArgs(this, this.Options!.EdgeNodeIdentifier, deviceId));
        }
        #endregion
    }
}
