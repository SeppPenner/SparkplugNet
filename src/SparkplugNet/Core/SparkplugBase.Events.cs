namespace SparkplugNet.Core;

public partial class SparkplugBase<T> : ISparkplugConnection where T : IMetric, new()
{
    #region Disconnected 
    /// <summary>
    /// The disconnected event
    /// </summary>
    protected AsyncEvent<SparkplugEventArgs> disconnectedEvent = new();

    /// <summary>
    /// Gets or sets the callback for the disconnected event. Indicates that metrics might be stale.
    /// Obsolete, please use <see cref="DisconnectedAsync"/>
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Browsable(false)]
    [Obsolete("Please use DisconnectedAsync", false)]
    public Action? OnDisconnected { get; set; } = null;

    /// <summary>
    /// Occurs when [connected asynchronous].
    /// </summary>
    public event Func<SparkplugEventArgs, Task> DisconnectedAsync
    {
        add => this.disconnectedEvent.AddHandler(value);
        remove => this.disconnectedEvent.RemoveHandler(value);
    }

    /// <summary>
    /// Fires the disconnected asynchronous.
    /// </summary>
    /// <returns></returns>
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
    /// The connected event
    /// </summary>
    protected AsyncEvent<SparkplugEventArgs> _connectedEvent = new();

    /// <summary>
    /// Occurs when [connected asynchronous].
    /// </summary>
    public event Func<SparkplugEventArgs, Task> ConnectedAsync
    {
        add => this._connectedEvent.AddHandler(value);
        remove => this._connectedEvent.RemoveHandler(value);
    }
    /// <summary>
    /// Fires the disconnected asynchronous.
    /// </summary>
    /// <returns></returns>
    protected Task FireConnectedAsync()
    {
        return this._connectedEvent.InvokeAsync(new SparkplugEventArgs(this));
    }
    #endregion

}
