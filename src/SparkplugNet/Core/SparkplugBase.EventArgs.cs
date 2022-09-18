namespace SparkplugNet.Core;

public partial class SparkplugBase<T> : ISparkplugConnection where T : IMetric, new()
{
    /// <summary>
    /// Sparkplug Base EventArgs
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class SparkplugEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the sender.
        /// </summary>
        public SparkplugBase<T> Sender { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SparkplugEventArgs"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public SparkplugEventArgs(SparkplugBase<T> sender)
        {
            this.Sender = sender;
        }
    }

    /// <summary>
    /// Node event args
    /// </summary>
    /// <seealso cref="SparkplugNet.Core.SparkplugBase&lt;T&gt;" />
    public class NodeEventArgs : SparkplugEventArgs
    {
        /// <summary>
        /// Gets the group identifier.
        /// </summary>
        public string GroupIdentifier { get; }

        /// <summary>
        /// Gets the edge node identifier.
        /// </summary>
        public string EdgeNodeIdentifier { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeEventArgs"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        public NodeEventArgs(SparkplugBase<T> sender, string groupIdentifier, string edgeNodeIdentifier)
            : base(sender)
        {
            this.GroupIdentifier = groupIdentifier;
            this.EdgeNodeIdentifier = edgeNodeIdentifier;
        }
    }

    /// <summary>
    /// Device event args
    /// </summary>
    /// <seealso cref="SparkplugNet.Core.SparkplugBase&lt;T&gt;" />
    public class DeviceEventArgs : NodeEventArgs
    {
        /// <summary>
        /// Gets the device identifier.
        /// </summary>
        public string DeviceIdentifier { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceEventArgs"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="deviceIdentifier">The device identifier.</param>
        public DeviceEventArgs(SparkplugBase<T> sender, string groupIdentifier, string edgeNodeIdentifier, string deviceIdentifier)
            : base(sender, groupIdentifier, edgeNodeIdentifier)
        {
            this.DeviceIdentifier = deviceIdentifier;
        }
    }

    /// <summary>
    /// Device birth event args
    /// </summary>
    /// <seealso cref="SparkplugNet.Core.SparkplugBase&lt;T&gt;" />
    public class DeviceBirthEventArgs : DeviceEventArgs
    {
        /// <summary>
        /// Gets the metrics.
        /// </summary>
        public IEnumerable<T> Metrics { get; }
        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceBirthEventArgs"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="deviceIdentifier">The device identifier.</param>
        /// <param name="metrics">The metrics.</param>
        public DeviceBirthEventArgs(SparkplugBase<T> sender, string groupIdentifier, string edgeNodeIdentifier, string deviceIdentifier, IEnumerable<T> metrics)
            : base(sender, groupIdentifier, edgeNodeIdentifier, deviceIdentifier)
        {
            this.Metrics = metrics;
        }
    }

    /// <summary>
    /// Device birth event args
    /// </summary>
    /// <seealso cref="SparkplugNet.Core.SparkplugBase&lt;T&gt;" />
    public class NodeBirthEventArgs : NodeEventArgs
    {
        /// <summary>
        /// Gets the metrics.
        /// </summary>
        public IEnumerable<T> Metrics { get; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceBirthEventArgs"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="metrics">The metrics.</param>
        public NodeBirthEventArgs(SparkplugBase<T> sender, string groupIdentifier, string edgeNodeIdentifier, IEnumerable<T> metrics)
            : base(sender, groupIdentifier, edgeNodeIdentifier)
        {
            this.Metrics = metrics;
        }
    }
}
