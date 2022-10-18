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
    public event Func<DeviceDataEventArgs, Task> DeviceDataReceivedAsync
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
    /// <param name="metric">The metric.</param>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    protected virtual Task FireDeviceDataReceivedAsync(string groupIdentifier, string edgeNodeIdentifier, string deviceIdentifier, T metric)
    {
        return this.DeviceDataReceivedEvent.InvokeAsync(new DeviceDataEventArgs(this, groupIdentifier, edgeNodeIdentifier, deviceIdentifier, metric));
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
    public event Func<NodeDataEventArgs, Task> NodeDataReceivedAsync
    {
        add => this.NodeDataReceivedEvent.AddHandler(value);
        remove => this.NodeDataReceivedEvent.RemoveHandler(value);
    }

    /// <summary>
    /// Fires the node data received asynchronously.
    /// </summary>
    /// <param name="groupIdentifier">The group identifier.</param>
    /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
    /// <param name="metric">The metric.</param>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    protected virtual Task FireNodeDataReceivedAsync(string groupIdentifier, string edgeNodeIdentifier, T metric)
    {
        return this.NodeDataReceivedEvent.InvokeAsync(new NodeDataEventArgs(this, groupIdentifier, edgeNodeIdentifier, metric));
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
    public event Func<DeviceBirthEventArgs, Task> DeviceBirthReceivedAsync
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
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    protected virtual Task FireDeviceBirthReceivedAsync(string groupIdentifier, string edgeNodeIdentifier, string deviceIdentifier, IEnumerable<T> metrics)
    {
        return this.DeviceBirthReceivedEvent.InvokeAsync(new DeviceBirthEventArgs(this, groupIdentifier, edgeNodeIdentifier, deviceIdentifier, metrics));
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
    public event Func<DeviceEventArgs, Task> DeviceDeathReceivedAsync
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
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    protected virtual Task FireDeviceDeathReceivedAsync(string groupIdentifier, string edgeNodeIdentifier, string deviceIdentifier)
    {
        return this.DeviceDeathReceivedEvent.InvokeAsync(new DeviceEventArgs(this, groupIdentifier, edgeNodeIdentifier, deviceIdentifier));
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
    public event Func<NodeBirthEventArgs, Task> NodeBirthReceivedAsync
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
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    protected virtual Task FireNodeBirthReceivedAsync(string groupIdentifier, string edgeNodeIdentifier, IEnumerable<T> metrics)
    {
        return this.NodeBirthReceivedEvent.InvokeAsync(new NodeBirthEventArgs(this, groupIdentifier, edgeNodeIdentifier, metrics));
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
    public event Func<NodeEventArgs, Task> NodeDeathReceivedAsync
    {
        add => this.NodeDeathReceivedEvent.AddHandler(value);
        remove => this.NodeDeathReceivedEvent.RemoveHandler(value);
    }

    /// <summary>
    /// Fires the node death received asynchronously.
    /// </summary>
    /// <param name="groupIdentifier">The group identifier.</param>
    /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    protected virtual Task FireNodeDeathReceivedAsync(string groupIdentifier, string edgeNodeIdentifier)
    {
        return this.NodeDeathReceivedEvent.InvokeAsync(new NodeEventArgs(this, groupIdentifier, edgeNodeIdentifier));
    }
    #endregion
}
