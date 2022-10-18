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
    public event Func<DeviceCommandEventArgs, Task> DeviceCommandReceivedAsync
    {
        add => this.DeviceCommandReceivedEvent.AddHandler(value);
        remove => this.DeviceCommandReceivedEvent.RemoveHandler(value);
    }

    /// <summary>
    /// Fires the device command received event asynchronously.
    /// </summary>
    /// <param name="deviceIdentifier">The device identifier.</param>
    /// <param name="metric">The metric.</param>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    protected virtual Task FireDeviceCommandReceivedAsync(string deviceIdentifier, T metric)
    {
        return this.DeviceCommandReceivedEvent.InvokeAsync(new DeviceCommandEventArgs(this, this.Options!.GroupIdentifier, this.Options!.EdgeNodeIdentifier, deviceIdentifier, metric));
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
    public event Func<NodeCommandEventArgs, Task> NodeCommandReceivedAsync
    {
        add => this.NodeCommandReceivedEvent.AddHandler(value);
        remove => this.NodeCommandReceivedEvent.RemoveHandler(value);
    }

    /// <summary>
    /// Fires the node command received event asynchronously.
    /// </summary>
    /// <param name="metric">The metric.</param>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    protected virtual Task FireNodeCommandReceivedAsync(T metric)
    {
        return this.NodeCommandReceivedEvent.InvokeAsync(new NodeCommandEventArgs(this, this.Options!.GroupIdentifier, this.Options!.EdgeNodeIdentifier, metric));
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
    public event Func<StatusMessageEventArgs, Task> StatusMessageReceivedAsync
    {
        add => this.StatusMessageReceivedEvent.AddHandler(value);
        remove => this.StatusMessageReceivedEvent.RemoveHandler(value);
    }

    /// <summary>
    /// Fires the status message received event asynchronously.
    /// </summary>
    /// <param name="status">The status.</param>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    protected virtual Task FireStatusMessageReceivedAsync(string status)
    {
        return this.StatusMessageReceivedEvent.InvokeAsync(new StatusMessageEventArgs(this, status));
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
    public event Func<DeviceBirthEventArgs, Task> DeviceBirthPublishingAsync
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
    protected virtual Task FireDeviceBirthPublishingAsync(string deviceIdentifier, IEnumerable<T> metrics)
    {
        return this.DeviceBirthPublishingEvent.InvokeAsync(new DeviceBirthEventArgs(this, this.Options!.GroupIdentifier, this.Options!.EdgeNodeIdentifier, deviceIdentifier, metrics));
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
    public event Func<DeviceEventArgs, Task> DeviceDeathPublishingAsync
    {
        add => this.DeviceDeathPublishingEvent.AddHandler(value);
        remove => this.DeviceDeathPublishingEvent.RemoveHandler(value);
    }

    /// <summary>
    /// Fires the device death publishing event asynchronously.
    /// </summary>
    /// <param name="deviceIdentifier">The device identifier.</param>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    protected virtual Task FireDeviceDeathPublishingAsync(string deviceIdentifier)
    {
        return this.DeviceDeathPublishingEvent.InvokeAsync(new DeviceEventArgs(this, this.Options!.GroupIdentifier, this.Options!.EdgeNodeIdentifier, deviceIdentifier));
    }
    #endregion
}
