// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugMessageOrder.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   Reorders Sparkplug B messages within an edge node session.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.VersionB;

/// <summary>
/// Tracks the sequence shared by an edge node and all of its devices.
/// </summary>
internal sealed class SparkplugMessageOrder
{
    /// <summary>
    /// The order state for the current broker connection.
    /// </summary>
    private Connection connection = new();

    /// <summary>
    /// Discards buffered messages and cancels their timers on disconnect.
    /// </summary>
    internal void Reset()
    {
        var previous = Interlocked.Exchange(ref this.connection, new Connection());
        previous.Stopping.Cancel();
        previous.Stopping.Dispose();
    }

    /// <summary>
    /// Discards the buffered messages for one source when its broker connection ends.
    /// </summary>
    /// <param name="group">The group identifier.</param>
    /// <param name="node">The edge node identifier.</param>
    internal void Reset(string group, string node)
    {
        var current = Volatile.Read(ref this.connection);
        if (current.Nodes.TryRemove((group, node), out var previous))
        {
            previous.Stopping.Cancel();
            previous.Stopping.Dispose();
        }
    }

    /// <summary>
    /// Delivers a message in sequence or holds it until the gap closes.
    /// </summary>
    /// <param name="topic">The source and message type.</param>
    /// <param name="payload">The wire payload.</param>
    /// <param name="timeout">The time to wait for a missing message.</param>
    /// <param name="deliver">Processes a message using the definitions announced before it.</param>
    /// <param name="requestRebirth">Requests a fresh birth sequence for the source.</param>
    /// <returns>The receive task.</returns>
    internal async Task Receive(
        SparkplugMessageTopic topic, VersionBProtoBuf.ProtoBufPayload payload, TimeSpan timeout,
        Func<SparkplugMessageTopic, VersionBProtoBuf.ProtoBufPayload, Task> deliver,
        Func<SparkplugMessageTopic, Task> requestRebirth)
    {
        if (topic.MessageType is not (SparkplugMessageType.NodeBirth or SparkplugMessageType.NodeDeath
            or SparkplugMessageType.DeviceBirth or SparkplugMessageType.DeviceDeath
            or SparkplugMessageType.NodeData or SparkplugMessageType.DeviceData))
        {
            return;
        }

        var current = Volatile.Read(ref this.connection);
        var node = current.Nodes.GetOrAdd((topic.GroupIdentifier, topic.EdgeNodeIdentifier), static (_, token) => new Node(token), current.Token);
        await node.Gate.WaitAsync();
        try
        {
            if (node.Token.IsCancellationRequested)
            {
                return;
            }

            if (topic.MessageType is SparkplugMessageType.NodeBirth or SparkplugMessageType.NodeDeath)
            {
                var bdSeq = payload.Metrics.Find(metric => metric.Name == Constants.SessionNumberMetricName)?.LongValue;
                if (topic.MessageType == SparkplugMessageType.NodeBirth || node.BirthSession is null || bdSeq is null || node.BirthSession == bdSeq)
                {
                    ClearWindow(node);
                    node.Pending.Clear();
                    node.NextSequence = topic.MessageType == SparkplugMessageType.NodeBirth ? Next(payload.Seq ?? 0) : null;
                    node.BirthSession = topic.MessageType == SparkplugMessageType.NodeBirth ? bdSeq : null;
                    node.RebirthRequested = false;
                }

                await deliver(topic, payload);
                return;
            }

            if (payload.Seq is not (<= byte.MaxValue) || node.NextSequence is not byte expected)
            {
                await RequestOnce(node, topic, requestRebirth);
                return;
            }

            var sequence = (byte)payload.Seq.Value;
            if (sequence == expected)
            {
                node.NextSequence = Next(sequence);
                await deliver(topic, payload);
                while (!node.Token.IsCancellationRequested && node.Pending.Remove(node.NextSequence.Value, out var pending))
                {
                    node.NextSequence = Next(node.NextSequence.Value);
                    await deliver(pending.Topic, pending.Payload);
                }

                if (node.Pending.Count == 0)
                {
                    ClearWindow(node);
                }
            }
            else if ((sequence - expected + 256) % 256 < 128)
            {
                node.Pending.TryAdd(sequence, (topic, payload));
                if (node.Window is null)
                {
                    var window = CancellationTokenSource.CreateLinkedTokenSource(node.Token);
                    node.Window = window;
                    _ = AwaitMissingMessage(node, topic, timeout, window, requestRebirth);
                }
            }
        }
        finally
        {
            node.Gate.Release();
        }
    }

    /// <summary>
    /// Requests rebirth if the gap remains open at the end of the reorder window.
    /// </summary>
    private static async Task AwaitMissingMessage(Node node, SparkplugMessageTopic topic, TimeSpan timeout, CancellationTokenSource window,
        Func<SparkplugMessageTopic, Task> requestRebirth)
    {
        try
        {
            await Task.Delay(timeout, window.Token);
        }
        catch (OperationCanceledException)
        {
            // The missing message arrived or the broker connection ended.
        }

        await node.Gate.WaitAsync();
        try
        {
            if (ReferenceEquals(node.Window, window))
            {
                node.Window = null;
                if (!window.IsCancellationRequested)
                {
                    node.Pending.Clear();
                    node.NextSequence = null;
                    await RequestOnce(node, topic, requestRebirth);
                }
            }
        }
        finally
        {
            window.Dispose();
            node.Gate.Release();
        }
    }

    /// <summary>
    /// Sends at most one rebirth request before a new session is announced.
    /// </summary>
    private static Task RequestOnce(Node node, SparkplugMessageTopic topic, Func<SparkplugMessageTopic, Task> requestRebirth)
    {
        if (node.RebirthRequested)
        {
            return Task.CompletedTask;
        }

        node.RebirthRequested = true;
        return requestRebirth(topic);
    }

    /// <summary>
    /// Stops the timer after the gap closes or the node starts a new session.
    /// </summary>
    private static void ClearWindow(Node node)
    {
        var window = node.Window;
        node.Window = null;
        window?.Cancel();
    }

    /// <summary>
    /// Advances a sequence number with Sparkplug's eight-bit wraparound.
    /// </summary>
    private static byte Next(ulong sequence) => unchecked((byte)(sequence + 1));

    /// <summary>
    /// Owns the state and cancellation of a single broker connection.
    /// </summary>
    private sealed class Connection
    {
        /// <summary>
        /// Signals that the connection has ended.
        /// </summary>
        internal CancellationTokenSource Stopping { get; } = new();

        /// <summary>
        /// The token remains usable by in-flight operations after cancellation.
        /// </summary>
        internal CancellationToken Token { get; }

        /// <summary>
        /// The node sessions on this connection.
        /// </summary>
        internal ConcurrentDictionary<(string Group, string Edge), Node> Nodes { get; } = new();

        /// <summary>
        /// Captures the connection's stop token for any receive operations.
        /// </summary>
        internal Connection() => this.Token = this.Stopping.Token;
    }

    /// <summary>
    /// The state of one edge node and its devices.
    /// </summary>
    private sealed class Node
    {
        /// <summary>Signals that the source lost its broker connection.</summary>
        internal CancellationTokenSource Stopping { get; }

        /// <summary>The token for this node's current reorder generation.</summary>
        internal CancellationToken Token { get; }

        /// <summary>Captures the stop token for in-flight receives.</summary>
        internal Node(CancellationToken connectionToken)
        {
            this.Stopping = CancellationTokenSource.CreateLinkedTokenSource(connectionToken);
            this.Token = this.Stopping.Token;
        }

        /// <summary>Serializes the timer and delivery of messages for this node.</summary>
        internal SemaphoreSlim Gate { get; } = new(1);

        /// <summary>The messages after a gap, indexed by sequence number.</summary>
        internal Dictionary<byte, (SparkplugMessageTopic Topic, VersionBProtoBuf.ProtoBufPayload Payload)> Pending { get; } = new();

        /// <summary>The next expected sequence, if a birth has been processed.</summary>
        internal byte? NextSequence { get; set; }

        /// <summary>The session number of the latest birth.</summary>
        internal ulong? BirthSession { get; set; }

        /// <summary>The cancellation source of the active reorder window.</summary>
        internal CancellationTokenSource? Window { get; set; }

        /// <summary>Whether recovery is awaiting a new birth.</summary>
        internal bool RebirthRequested { get; set; }
    }
}
