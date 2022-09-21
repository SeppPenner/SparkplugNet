// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugBase.Events.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A base class for all Sparkplug applications, nodes and devices.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Core;

/// <inheritdoc cref="ISparkplugConnection"/>
/// <summary>
/// A base class for all Sparkplug applications, nodes and devices.
/// </summary>
/// <seealso cref="ISparkplugConnection"/>
public partial class SparkplugBase<T> : ISparkplugConnection where T : IMetric, new()
{
    #region Disconnected
    /// <summary>
    /// Gets or sets the callback for the disconnected event. Indicates that metrics might be stale.
    /// Obsolete, please use <see cref="DisconnectedAsync"/>.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Browsable(false)]
    [Obsolete("Please use DisconnectedAsync", false)]
    public Action? OnDisconnected { get; set; } = null;

    /// <summary>
    /// The disconnected event.
    /// </summary>
    protected AsyncEvent<SparkplugEventArgs> disconnectedEvent = new();

    /// <summary>
    /// Occurs when the disconnected event was received.
    /// </summary>
    public event Func<SparkplugEventArgs, Task> DisconnectedAsync
    {
        add => this.disconnectedEvent.AddHandler(value);
        remove => this.disconnectedEvent.RemoveHandler(value);
    }

    /// <summary>
    /// Fires the disconnected event asynchronously.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    protected Task FireDisconnectedAsync()
    {
#pragma warning disable CS0618 // Typ oder Element ist veraltet
        this.OnDisconnected?.Invoke();
#pragma warning restore CS0618 // Typ oder Element ist veraltet
        return this.disconnectedEvent.InvokeAsync(new SparkplugEventArgs(this));
    }
    #endregion

    #region Connected
    /// <summary>
    /// The connected event.
    /// </summary>
    protected AsyncEvent<SparkplugEventArgs> connectedEvent = new();

    /// <summary>
    /// Occurs when the connected event was received.
    /// </summary>
    public event Func<SparkplugEventArgs, Task> ConnectedAsync
    {
        add => this.connectedEvent.AddHandler(value);
        remove => this.connectedEvent.RemoveHandler(value);
    }

    /// <summary>
    /// Fires the disconnected event asynchronously.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    protected Task FireConnectedAsync()
    {
        return this.connectedEvent.InvokeAsync(new SparkplugEventArgs(this));
    }
    #endregion
}
