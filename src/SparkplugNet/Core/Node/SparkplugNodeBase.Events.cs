// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugNodeBase.Events.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class that handles a Sparkplug node.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Core.Node;

/// <inheritdoc cref="SparkplugBase{T}"/>
/// <summary>
/// A class that handles a Sparkplug node.
/// </summary>
/// <seealso cref="SparkplugBase{T}"/>
public abstract partial class SparkplugNodeBase<T> : SparkplugBase<T> where T : IMetric, new()
{
    #region DeviceCommandReceived
    /// <summary>
    /// The device command received event.
    /// </summary>
    protected AsyncEvent<DeviceCommandEventArgs> DeviceCommandReceivedEvent = new();

    /// <summary>
    /// Occurs when the device command was received.
    /// </summary>
    public event Func<DeviceCommandEventArgs, Task> DeviceCommandReceived
    {
        add => this.DeviceCommandReceivedEvent.AddHandler(value);
        remove => this.DeviceCommandReceivedEvent.RemoveHandler(value);
    }

    /// <summary>
    /// Fires the device command received event asynchronously.
    /// </summary>
    /// <param name="deviceIdentifier">The device identifier.</param>
    /// <param name="metrics">The metrics.</param>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    protected virtual Task FireDeviceCommandReceived(string deviceIdentifier, IEnumerable<T> metrics)
    {
        var deviceCommandEventArgs = new DeviceCommandEventArgs(this, this.Options!.GroupIdentifier, this.Options!.EdgeNodeIdentifier, deviceIdentifier, metrics);
        return this.DeviceCommandReceivedEvent.InvokeAsync(deviceCommandEventArgs);
    }
    #endregion

    #region NodeCommandReceived
    /// <summary>
    /// The node command received event.
    /// </summary>
    protected AsyncEvent<NodeCommandEventArgs> NodeCommandReceivedEvent = new();

    /// <summary>
    /// Occurs when the node command was received.
    /// </summary>
    public event Func<NodeCommandEventArgs, Task> NodeCommandReceived
    {
        add => this.NodeCommandReceivedEvent.AddHandler(value);
        remove => this.NodeCommandReceivedEvent.RemoveHandler(value);
    }

    /// <summary>
    /// Fires the node command received event asynchronously.
    /// </summary>
    /// <param name="metrics">The metrics.</param>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    protected virtual Task FireNodeCommandReceived(IEnumerable<T> metrics)
    {
        var nodeCommandEventArgs = new NodeCommandEventArgs(this, this.Options!.GroupIdentifier, this.Options!.EdgeNodeIdentifier, metrics);
        return this.NodeCommandReceivedEvent.InvokeAsync(nodeCommandEventArgs);
    }
    #endregion

    #region StatusMessageReceived
    /// <summary>
    /// The status message received event.
    /// </summary>
    protected AsyncEvent<StatusMessageEventArgs> StatusMessageReceivedEvent = new();

    /// <summary>
    /// Occurs when the status message command was received.
    /// </summary>
    public event Func<StatusMessageEventArgs, Task> StatusMessageReceived
    {
        add => this.StatusMessageReceivedEvent.AddHandler(value);
        remove => this.StatusMessageReceivedEvent.RemoveHandler(value);
    }

    /// <summary>
    /// Fires the status message received event asynchronously.
    /// </summary>
    /// <param name="status">The status.</param>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    protected virtual Task FireStatusMessageReceived(string status)
    {
        var statusMessageEventArgs = new StatusMessageEventArgs(this, status);
        return this.StatusMessageReceivedEvent.InvokeAsync(statusMessageEventArgs);
    }
    #endregion

    #region DeviceBirthPublishing
    /// <summary>
    /// The device birth publishing event.
    /// </summary>
    protected AsyncEvent<DeviceBirthEventArgs> DeviceBirthPublishingEvent = new();

    /// <summary>
    /// Occurs when the device birth command was published.
    /// </summary>
    public event Func<DeviceBirthEventArgs, Task> DeviceBirthPublishing
    {
        add => this.DeviceBirthPublishingEvent.AddHandler(value);
        remove => this.DeviceBirthPublishingEvent.RemoveHandler(value);
    }

    /// <summary>
    /// Fires the device birth publishing event asynchronously.
    /// </summary>
    /// <param name="deviceIdentifier">The device identifier.</param>
    /// <param name="metrics">The metrics</param>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    protected virtual Task FireDeviceBirthPublishing(string deviceIdentifier, IEnumerable<T> metrics)
    {
        var deviceBirthEventArgs = new DeviceBirthEventArgs(this, this.Options!.GroupIdentifier, this.Options!.EdgeNodeIdentifier, deviceIdentifier, metrics);
        return this.DeviceBirthPublishingEvent.InvokeAsync(deviceBirthEventArgs);
    }
    #endregion

    #region DeviceDeathPublishing
    /// <summary>
    /// The device death publishing event.
    /// </summary>
    protected AsyncEvent<DeviceEventArgs> DeviceDeathPublishingEvent = new();

    /// <summary>
    /// Occurs when the device death command was published.
    /// </summary>
    public event Func<DeviceEventArgs, Task> DeviceDeathPublishing
    {
        add => this.DeviceDeathPublishingEvent.AddHandler(value);
        remove => this.DeviceDeathPublishingEvent.RemoveHandler(value);
    }

    /// <summary>
    /// Fires the device death publishing event asynchronously.
    /// </summary>
    /// <param name="deviceIdentifier">The device identifier.</param>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    protected virtual Task FireDeviceDeathPublishing(string deviceIdentifier)
    {
        var deviceEventArgs = new DeviceEventArgs(this, this.Options!.GroupIdentifier, this.Options!.EdgeNodeIdentifier, deviceIdentifier);
        return this.DeviceDeathPublishingEvent.InvokeAsync(deviceEventArgs);
    }
    #endregion
}
