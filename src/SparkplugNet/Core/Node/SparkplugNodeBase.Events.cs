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

        #region DeviceBirthReceived
        /// <summary>
        /// The status message received event
        /// </summary>
        protected AsyncEvent<DeviceBirthEventArgs> _DeviceBirthReceivedEvent = new AsyncEvent<DeviceBirthEventArgs>();
        /// <summary>
        /// Gets or sets the callback for the device birth received event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        [Obsolete("Please use DeviceCommandReceivedAsync", false)]
        public Action<KeyValuePair<string, List<T>>>? DeviceBirthReceived { get; set; } = null;
        /// <summary>
        /// Occurs when [Node command received asynchronous].
        /// </summary>
        public event Func<DeviceBirthEventArgs, Task> DeviceBirthReceivedAsync
        {
            add => this._DeviceBirthReceivedEvent.AddHandler(value);
            remove => this._DeviceBirthReceivedEvent.RemoveHandler(value);
        }

        /// <summary>
        /// Fires the status message received asynchronous.
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <param name="metrics">The metrics</param>
        /// <returns></returns>
        protected virtual Task FireDeviceBirthReceivedAsync(string deviceId, IEnumerable<T> metrics)
        {
#pragma warning disable CS0618 // Typ oder Element ist veraltet
            this.DeviceBirthReceived?.Invoke(new KeyValuePair<string, List<T>>(deviceId, metrics.ToList()));
#pragma warning restore CS0618 // Typ oder Element ist veraltet

            return this._DeviceBirthReceivedEvent.InvokeAsync(new DeviceBirthEventArgs(this, deviceId, metrics));
        }
        #endregion

        #region DeviceDeathReceived
        /// <summary>
        /// The status message received event
        /// </summary>
        protected AsyncEvent<DeviceEventArgs> _DeviceDeathReceivedEvent = new AsyncEvent<DeviceEventArgs>();
        /// <summary>
        /// Gets or sets the callback for the device death received event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        [Obsolete("Please use DeviceCommandReceivedAsync", false)]
        public Action<string>? DeviceDeathReceived { get; set; } = null;
        /// <summary>
        /// Occurs when [Node command received asynchronous].
        /// </summary>
        public event Func<DeviceEventArgs, Task> DeviceDeathReceivedAsync
        {
            add => this._DeviceDeathReceivedEvent.AddHandler(value);
            remove => this._DeviceDeathReceivedEvent.RemoveHandler(value);
        }

        /// <summary>
        /// Fires the status message received asynchronous.
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <returns></returns>
        protected virtual Task FireDeviceDeathReceivedAsync(string deviceId)
        {
#pragma warning disable CS0618 // Typ oder Element ist veraltet
            this.DeviceDeathReceived?.Invoke(deviceId);
#pragma warning restore CS0618 // Typ oder Element ist veraltet

            return this._DeviceDeathReceivedEvent.InvokeAsync(new DeviceEventArgs(this, deviceId));
        }
        #endregion
    }
}
