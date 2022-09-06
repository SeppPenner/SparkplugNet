namespace SparkplugNet.Core.Application
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using MQTTnet.Internal;

    public partial class SparkplugApplicationBase<T> : SparkplugBase<T> where T : IMetric, new()
    {
        #region DeviceDataReceived
        /// <summary>
        /// Gets or sets the callback for the device data received event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        [Obsolete("Please use DeviceDataReceivedAsync", false)]
        public Action<string, string, string, T>? OnDeviceDataReceived { get; set; } = null;
        /// <summary>
        /// The device data received event
        /// </summary>
        protected AsyncEvent<DeviceDataEventArgs> _DeviceDataReceivedEvent = new AsyncEvent<DeviceDataEventArgs>();
        /// <summary>
        /// Occurs when [device data received asynchronous].
        /// </summary>
        public event Func<DeviceDataEventArgs, Task> DeviceDataReceivedAsync
        {
            add => this._DeviceDataReceivedEvent.AddHandler(value);
            remove => this._DeviceDataReceivedEvent.RemoveHandler(value);
        }
        /// <summary>
        /// Fires the device data received asynchronous.
        /// </summary>
        /// <param name="groupId">The group identifier.</param>
        /// <param name="nodeId">The node identifier.</param>
        /// <param name="deviceId">The device identifier.</param>
        /// <param name="metric">The metric.</param>
        /// <returns></returns>
        protected virtual Task FireDeviceDataReceivedAsync(string groupId, string nodeId, string deviceId, T metric)
        {
#pragma warning disable CS0618 // Typ oder Element ist veraltet
            this.OnDeviceDataReceived?.Invoke(groupId, nodeId, deviceId, metric);
#pragma warning restore CS0618 // Typ oder Element ist veraltet

            return this._DeviceDataReceivedEvent.InvokeAsync(new DeviceDataEventArgs(this, groupId, nodeId, deviceId, metric));
        }
        #endregion

        #region NodeDataReceived
        /// <summary>
        /// Gets or sets the callback for the node data received event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        [Obsolete("Please use NodeDataReceivedAsync", false)]
        public Action<string, string, T>? OnNodeDataReceived { get; set; } = null;

        /// <summary>
        /// The node data received event
        /// </summary>
        protected AsyncEvent<DataEventArgs> _NodeDataReceivedEvent = new AsyncEvent<DataEventArgs>();
        /// <summary>
        /// Occurs when [node data received asynchronous].
        /// </summary>
        public event Func<DataEventArgs, Task> NodeDataReceivedAsync
        {
            add => this._NodeDataReceivedEvent.AddHandler(value);
            remove => this._NodeDataReceivedEvent.RemoveHandler(value);
        }
        /// <summary>
        /// Fires the node data received asynchronous.
        /// </summary>
        /// <param name="groupId">The group identifier.</param>
        /// <param name="nodeId">The node identifier.</param>
        /// <param name="metric">The metric.</param>
        /// <returns></returns>
        protected virtual Task FireNodeDataReceivedAsync(string groupId, string nodeId, T metric)
        {
#pragma warning disable CS0618 // Typ oder Element ist veraltet
            this.OnNodeDataReceived?.Invoke(groupId, nodeId, metric);
#pragma warning restore CS0618 // Typ oder Element ist veraltet

            return this._NodeDataReceivedEvent.InvokeAsync(new DataEventArgs(this, groupId, nodeId, metric));
        }
        #endregion

        #region DeviceBirthReceived
        /// <summary>
        /// The status message received event
        /// </summary>
        protected AsyncEvent<DeviceBirthEventArgs> _DeviceBirthReceivedEvent = new AsyncEvent<DeviceBirthEventArgs>();
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
        /// <param name="nodeId">The node identifier.</param>
        /// <param name="deviceId">The device identifier.</param>
        /// <param name="metrics">The metrics</param>
        /// <returns></returns>
        protected virtual Task FireDeviceBirthReceivedAsync(string nodeId, string deviceId, IEnumerable<T> metrics)
        {

            return this._DeviceBirthReceivedEvent.InvokeAsync(new DeviceBirthEventArgs(this, nodeId, deviceId, metrics));
        }
        #endregion

        #region DeviceDeathReceived
        /// <summary>
        /// The status message received event
        /// </summary>
        protected AsyncEvent<DeviceEventArgs> _DeviceDeathReceivedEvent = new AsyncEvent<DeviceEventArgs>();
        /// <summary>
        /// Occurs when [Node command received asynchronous].
        /// </summary>
        public event Func<DeviceEventArgs, Task> DeviceDeathReceivedAsync
        {
            add => this._DeviceDeathReceivedEvent.AddHandler(value);
            remove => this._DeviceDeathReceivedEvent.RemoveHandler(value);
        }

        /// <summary>
        /// Fires the device death received asynchronous.
        /// </summary>
        /// <param name="edgeNodeId">The edge node identifier.</param>
        /// <param name="deviceId">The device identifier.</param>
        /// <returns></returns>
        protected virtual Task FireDeviceDeathReceivedAsync(string edgeNodeId, string deviceId)
        {
            return this._DeviceDeathReceivedEvent.InvokeAsync(new DeviceEventArgs(this, edgeNodeId, deviceId));
        }
        #endregion

        #region NodeBirthReceived
        /// <summary>
        /// The status message received event
        /// </summary>
        protected AsyncEvent<NodeBirthEventArgs> _NodeBirthReceivedEvent = new AsyncEvent<NodeBirthEventArgs>();
        /// <summary>
        /// Occurs when [Node command received asynchronous].
        /// </summary>
        public event Func<NodeBirthEventArgs, Task> NodeBirthReceivedAsync
        {
            add => this._NodeBirthReceivedEvent.AddHandler(value);
            remove => this._NodeBirthReceivedEvent.RemoveHandler(value);
        }

        /// <summary>
        /// Fires the node birth Received asynchronous.
        /// </summary>
        /// <param name="nodeIdentifier">The node identifier.</param>
        /// <param name="metrics">The metrics.</param>
        /// <returns></returns>
        protected virtual Task FireNodeBirthReceivedAsync(string nodeIdentifier, IEnumerable<T> metrics)
        {
            return this._NodeBirthReceivedEvent.InvokeAsync(new NodeBirthEventArgs(this, nodeIdentifier, metrics));
        }
        #endregion

        #region NodeDeathReceived
        /// <summary>
        /// The status message received event
        /// </summary>
        protected AsyncEvent<NodeEventArgs> _NodeDeathReceivedEvent = new AsyncEvent<NodeEventArgs>();
        /// <summary>
        /// Occurs when [Node command received asynchronous].
        /// </summary>
        public event Func<NodeEventArgs, Task> NodeDeathReceivedAsync
        {
            add => this._NodeDeathReceivedEvent.AddHandler(value);
            remove => this._NodeDeathReceivedEvent.RemoveHandler(value);
        }

        /// <summary>
        /// Fires the node death Received asynchronous.
        /// </summary>
        /// <param name="nodeIdentifier">The node identifier.</param>
        /// <returns></returns>
        protected virtual Task FireNodeDeathReceivedAsync(string nodeIdentifier)
        {
            return this._NodeDeathReceivedEvent.InvokeAsync(new NodeEventArgs(this, nodeIdentifier));
        }
        #endregion
    }
}
