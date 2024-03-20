// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugApplicationBase.Events.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class that handles a Sparkplug application.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Core.Application;

/// <inheritdoc cref="SparkplugBase{T}"/>
/// <summary>
/// A class that handles a Sparkplug application.
/// </summary>
/// <seealso cref="SparkplugBase{T}"/>
public partial class SparkplugApplicationBase<T> : SparkplugBase<T> where T : IMetric, new()
{
    #region DeviceDataReceived
    /// <summary>
    /// The device data received event.
    /// </summary>
    protected AsyncEvent<DeviceDataEventArgs> DeviceDataReceivedEvent = new();

    /// <summary>
    /// Occurs when the device data was received.
    /// </summary>
    public event Func<DeviceDataEventArgs, Task> DeviceDataReceived
    {
        add => this.DeviceDataReceivedEvent.AddHandler(value);
        remove => this.DeviceDataReceivedEvent.RemoveHandler(value);
    }

    /// <summary>
    /// Fires the device data received event asynchronously.
    /// </summary>
    /// <param name="groupIdentifier">The group identifier.</param>
    /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
    /// <param name="deviceIdentifier">The device identifier.</param>
    /// <param name="metrics">The metrics.</param>
    protected virtual Task FireDeviceDataReceived(string groupIdentifier, string edgeNodeIdentifier, string deviceIdentifier, IEnumerable<T> metrics)
    {
        var deviceDataEventArgs = new DeviceDataEventArgs(this, groupIdentifier, edgeNodeIdentifier, deviceIdentifier, metrics);
        return this.DeviceDataReceivedEvent.InvokeAsync(deviceDataEventArgs);
    }
    #endregion

    #region NodeDataReceived
    /// <summary>
    /// The node data received event.
    /// </summary>
    protected AsyncEvent<NodeDataEventArgs> NodeDataReceivedEvent = new();

    /// <summary>
    /// Occurs when the node data was received.
    /// </summary>
    public event Func<NodeDataEventArgs, Task> NodeDataReceived
    {
        add => this.NodeDataReceivedEvent.AddHandler(value);
        remove => this.NodeDataReceivedEvent.RemoveHandler(value);
    }

    /// <summary>
    /// Fires the node data received asynchronously.
    /// </summary>
    /// <param name="groupIdentifier">The group identifier.</param>
    /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
    /// <param name="metrics">The metrics.</param>
    protected virtual Task FireNodeDataReceived(string groupIdentifier, string edgeNodeIdentifier, IEnumerable<T> metrics)
    {
        var nodeDataEventArgs = new NodeDataEventArgs(this, groupIdentifier, edgeNodeIdentifier, metrics);
        return this.NodeDataReceivedEvent.InvokeAsync(nodeDataEventArgs);
    }
    #endregion

    #region DeviceBirthReceived
    /// <summary>
    /// The device birth received event.
    /// </summary>
    protected AsyncEvent<DeviceBirthEventArgs> DeviceBirthReceivedEvent = new();

    /// <summary>
    /// Occurs when the device birth was received.
    /// </summary>
    public event Func<DeviceBirthEventArgs, Task> DeviceBirthReceived
    {
        add => this.DeviceBirthReceivedEvent.AddHandler(value);
        remove => this.DeviceBirthReceivedEvent.RemoveHandler(value);
    }

    /// <summary>
    /// Fires the device birth received asynchronously.
    /// </summary>
    /// <param name="groupIdentifier">The group identifier.</param>
    /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
    /// <param name="deviceIdentifier">The device identifier.</param>
    /// <param name="metrics">The metrics.</param>
    protected virtual Task FireDeviceBirthReceived(string groupIdentifier, string edgeNodeIdentifier, string deviceIdentifier, IEnumerable<T> metrics)
    {
        var deviceBirthEventArgs = new DeviceBirthEventArgs(this, groupIdentifier, edgeNodeIdentifier, deviceIdentifier, metrics);
        return this.DeviceBirthReceivedEvent.InvokeAsync(deviceBirthEventArgs);
    }
    #endregion

    #region DeviceDeathReceived
    /// <summary>
    /// The device death received event.
    /// </summary>
    protected AsyncEvent<DeviceEventArgs> DeviceDeathReceivedEvent = new();

    /// <summary>
    /// Occurs when the device death was received.
    /// </summary>
    public event Func<DeviceEventArgs, Task> DeviceDeathReceived
    {
        add => this.DeviceDeathReceivedEvent.AddHandler(value);
        remove => this.DeviceDeathReceivedEvent.RemoveHandler(value);
    }

    /// <summary>
    /// Fires the device death received asynchronously.
    /// </summary>
    /// <param name="groupIdentifier">The group identifier.</param>
    /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
    /// <param name="deviceIdentifier">The device identifier.</param>
    protected virtual Task FireDeviceDeathReceived(string groupIdentifier, string edgeNodeIdentifier, string deviceIdentifier)
    {
        var deviceEventArgs = new DeviceEventArgs(this, groupIdentifier, edgeNodeIdentifier, deviceIdentifier);
        return this.DeviceDeathReceivedEvent.InvokeAsync(deviceEventArgs);
    }
    #endregion

    #region NodeBirthReceived
    /// <summary>
    /// The node birth received event.
    /// </summary>
    protected AsyncEvent<NodeBirthEventArgs> NodeBirthReceivedEvent = new();

    /// <summary>
    /// Occurs when the node birth was received.
    /// </summary>
    public event Func<NodeBirthEventArgs, Task> NodeBirthReceived
    {
        add => this.NodeBirthReceivedEvent.AddHandler(value);
        remove => this.NodeBirthReceivedEvent.RemoveHandler(value);
    }

    /// <summary>
    /// Fires the node birth received asynchronously.
    /// </summary>
    /// <param name="groupIdentifier">The group identifier.</param>
    /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
    /// <param name="metrics">The metrics.</param>
    protected virtual Task FireNodeBirthReceived(string groupIdentifier, string edgeNodeIdentifier, IEnumerable<T> metrics)
    {
        var nodeBirthEventArgs = new NodeBirthEventArgs(this, groupIdentifier, edgeNodeIdentifier, metrics);
        return this.NodeBirthReceivedEvent.InvokeAsync(nodeBirthEventArgs);
    }
    #endregion

    #region NodeDeathReceived
    /// <summary>
    /// The node death received event.
    /// </summary>
    protected AsyncEvent<NodeEventArgs> NodeDeathReceivedEvent = new();

    /// <summary>
    /// Occurs when the node death was received.
    /// </summary>
    public event Func<NodeEventArgs, Task> NodeDeathReceived
    {
        add => this.NodeDeathReceivedEvent.AddHandler(value);
        remove => this.NodeDeathReceivedEvent.RemoveHandler(value);
    }

    /// <summary>
    /// Fires the node death received asynchronously.
    /// </summary>
    /// <param name="groupIdentifier">The group identifier.</param>
    /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
    protected virtual Task FireNodeDeathReceived(string groupIdentifier, string edgeNodeIdentifier)
    {
        var nodeEventArgs = new NodeEventArgs(this, groupIdentifier, edgeNodeIdentifier);
        return this.NodeDeathReceivedEvent.InvokeAsync(nodeEventArgs);
    }
    #endregion
}
