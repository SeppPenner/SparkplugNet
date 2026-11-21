// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PrimaryHostState.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   Tracks the primary host state across MQTT connections.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Core.Node;

/// <summary>
/// Tracks the primary host state across MQTT connections.
/// </summary>
/// <param name="stateTopic">The expected host STATE topic.</param>
/// <param name="specificationVersion">The Sparkplug specification version.</param>
internal sealed class PrimaryHostState(string stateTopic, SparkplugSpecificationVersion specificationVersion)
{
    /// <summary>
    /// Synchronizes connection and STATE transitions.
    /// </summary>
    private readonly Lock sync = new();

    /// <summary>
    /// Cancels connection establishment when this node run stops.
    /// </summary>
    private readonly CancellationTokenSource stopping = new();

    /// <summary>
    /// Signals the first online STATE on the current connection.
    /// </summary>
    private TaskCompletionSource? online;

    /// <summary>
    /// The timestamp of the most recent valid STATE from the configured host.
    /// </summary>
    private long? lastTimestamp;

    /// <summary>
    /// Whether NBIRTH has been published for the current connection.
    /// </summary>
    private bool birthPublished;

    /// <summary>
    /// Gets the cancellation token for this node run.
    /// </summary>
    internal SystemCancellationToken StoppingToken => this.stopping.Token;

    /// <summary>
    /// Cancels all waits for this node run.
    /// </summary>
    internal void Stop()
    {
        this.stopping.Cancel();
        this.EndConnection();
    }

    /// <summary>
    /// Gets whether the host is online for this connection.
    /// </summary>
    internal bool IsOnline
    {
        get
        {
            lock (this.sync)
            {
                return this.online?.Task.IsCompletedSuccessfully == true;
            }
        }
    }

    /// <summary>
    /// Gets whether the node has published NBIRTH and the host is online for this connection.
    /// </summary>
    internal bool CanPublish
    {
        get
        {
            lock (this.sync)
            {
                return this.birthPublished && this.online?.Task.IsCompletedSuccessfully == true;
            }
        }
    }

    /// <summary>
    /// Starts waiting for STATE on a new connection.
    /// </summary>
    /// <returns>A task that completes when the configured host is online.</returns>
    internal Task BeginConnection()
    {
        lock (this.sync)
        {
            this.online?.TrySetCanceled();
            var signal = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            this.online = signal;
            this.birthPublished = false;
            return signal.Task;
        }
    }

    /// <summary>
    /// Clears the online state and cancels any pending wait for this connection.
    /// </summary>
    internal void EndConnection()
    {
        lock (this.sync)
        {
            this.online?.TrySetCanceled();
            this.online = null;
            this.birthPublished = false;
        }
    }

    /// <summary>
    /// Records successful publication of NBIRTH for this connection.
    /// </summary>
    internal void MarkBirthPublished()
    {
        lock (this.sync)
        {
            this.birthPublished = true;
        }
    }

    /// <summary>
    /// Applies a valid STATE message from the configured host.
    /// </summary>
    /// <param name="topic">The received topic.</param>
    /// <param name="payload">The received STATE payload.</param>
    /// <returns>Whether an online host has gone offline and the node should reconnect.</returns>
    internal bool Observe(string topic, string payload)
    {
        if (!string.Equals(topic, stateTopic, StringComparison.Ordinal) || !this.TryReadState(payload, out var isOnline, out var timestamp))
        {
            return false;
        }

        lock (this.sync)
        {
            if (this.online is null || timestamp < this.lastTimestamp)
            {
                return false;
            }

            this.lastTimestamp = timestamp;
            if (isOnline)
            {
                this.online.TrySetResult();
                return false;
            }

            if (!this.online.Task.IsCompletedSuccessfully)
            {
                return false;
            }

            this.online = null;
            this.birthPublished = false;
            return true;
        }
    }

    /// <summary>
    /// Reads and validates the STATE payload for the configured Sparkplug version.
    /// </summary>
    /// <param name="payload">The STATE payload.</param>
    /// <param name="isOnline">Whether the host is online.</param>
    /// <param name="timestamp">The timestamp supplied by the host.</param>
    /// <returns>Whether the payload contains a valid STATE.</returns>
    private bool TryReadState(string payload, out bool isOnline, out long? timestamp)
    {
        isOnline = false;
        timestamp = null;
        if (specificationVersion == SparkplugSpecificationVersion.Version22)
        {
            isOnline = payload == "ONLINE";
            return isOnline || payload == "OFFLINE";
        }

        try
        {
            using var document = JsonDocument.Parse(payload);
            var root = document.RootElement;
            if (root.ValueKind != JsonValueKind.Object ||
                !root.TryGetProperty("online", out var onlineValue) || onlineValue.ValueKind is not (JsonValueKind.True or JsonValueKind.False) ||
                !root.TryGetProperty("timestamp", out var timestampValue) || timestampValue.ValueKind != JsonValueKind.Number ||
                !timestampValue.TryGetInt64(out var stateTimestamp))
            {
                return false;
            }

            isOnline = onlineValue.GetBoolean();
            timestamp = stateTimestamp;
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }
}
