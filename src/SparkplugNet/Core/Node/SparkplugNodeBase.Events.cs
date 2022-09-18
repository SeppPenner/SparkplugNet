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
    /// Gets or sets the callback for the device command received event.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Browsable(false)]
    [Obsolete("Please use DeviceCommandReceivedAsync", false)]
    public Action<T>? DeviceCommandReceived { get; set; } = null;

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
#pragma warning disable CS0618 // Typ oder Element ist veraltet
        this.DeviceCommandReceived?.Invoke(metric);
#pragma warning restore CS0618 // Typ oder Element ist veraltet

        return this.DeviceCommandReceivedEvent.InvokeAsync(new DeviceCommandEventArgs(this, this.Options!.GroupIdentifier, this.Options!.EdgeNodeIdentifier, deviceIdentifier, metric));
    }
    #endregion

    #region NodeCommandReceived
    /// <summary>
    /// Gets or sets the callback for the node command received event.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Browsable(false)]
    [Obsolete("Please use DeviceCommandReceivedAsync", false)]
    public Action<T>? NodeCommandReceived { get; set; } = null;

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
#pragma warning disable CS0618 // Typ oder Element ist veraltet
        this.NodeCommandReceived?.Invoke(metric);
#pragma warning restore CS0618 // Typ oder Element ist veraltet

        return this.NodeCommandReceivedEvent.InvokeAsync(new NodeCommandEventArgs(this, this.Options!.GroupIdentifier, this.Options!.EdgeNodeIdentifier, metric));
    }
    #endregion

    #region StatusMessageReceived
    /// <summary>
    /// Gets or sets the callback for the status message received event.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Browsable(false)]
    [Obsolete("Please use DeviceCommandReceivedAsync", false)]
    public Action<string>? StatusMessageReceived { get; set; } = null;

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
#pragma warning disable CS0618 // Typ oder Element ist veraltet
        this.StatusMessageReceived?.Invoke(status);
#pragma warning restore CS0618 // Typ oder Element ist veraltet

        return this.StatusMessageReceivedEvent.InvokeAsync(new StatusMessageEventArgs(this, status));
    }
    #endregion

    #region DeviceBirthPublishing
    /// <summary>
    /// Gets or sets the callback for the device birth publishing event.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Browsable(false)]
    [Obsolete("Please use DeviceBirthSendingAsync", false)]
    public Action<KeyValuePair<string, List<T>>>? DeviceBirthReceived { get; set; } = null;

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
#pragma warning disable CS0618 // Typ oder Element ist veraltet
        this.DeviceBirthReceived?.Invoke(new KeyValuePair<string, List<T>>(deviceIdentifier, metrics.ToList()));
#pragma warning restore CS0618 // Typ oder Element ist veraltet

        return this.DeviceBirthPublishingEvent.InvokeAsync(new DeviceBirthEventArgs(this, this.Options!.GroupIdentifier, this.Options!.EdgeNodeIdentifier, deviceIdentifier, metrics));
    }
    #endregion

    #region DeviceDeathReceived
    /// <summary>
    /// Gets or sets the callback for the device death publishing event.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Browsable(false)]
    [Obsolete("Please use DeviceDeathPublishingAsync", false)]
    public Action<string>? DeviceDeathReceived { get; set; } = null;

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
#pragma warning disable CS0618 // Typ oder Element ist veraltet
        this.DeviceDeathReceived?.Invoke(deviceIdentifier);
#pragma warning restore CS0618 // Typ oder Element ist veraltet

        return this.DeviceDeathPublishingEvent.InvokeAsync(new DeviceEventArgs(this, this.Options!.GroupIdentifier, this.Options!.EdgeNodeIdentifier, deviceIdentifier));
    }
    #endregion
}
