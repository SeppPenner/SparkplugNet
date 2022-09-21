// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugBase.EventArgs.cs" company="Hämmer Electronics">
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
    /// <inheritdoc cref="EventArgs" />
    /// <summary>
    /// A class for the Sparkplug event args.
    /// </summary>
    /// <seealso cref="EventArgs" />
    public class SparkplugEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SparkplugEventArgs"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public SparkplugEventArgs(SparkplugBase<T> sender)
        {
            this.Sender = sender;
        }

        /// <summary>
        /// Gets the sender.
        /// </summary>
        public SparkplugBase<T> Sender { get; }
    }

    /// <inheritdoc cref="SparkplugEventArgs" />
    /// <summary>
    /// A class for the node event args.
    /// </summary>
    /// <seealso cref="SparkplugEventArgs" />
    public class NodeEventArgs : SparkplugEventArgs
    {
        /// <inheritdoc cref="SparkplugEventArgs" />
        /// <summary>
        /// Initializes a new instance of the <see cref="NodeEventArgs"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <seealso cref="SparkplugEventArgs" />
        public NodeEventArgs(SparkplugBase<T> sender, string groupIdentifier, string edgeNodeIdentifier) : base(sender)
        {
            this.GroupIdentifier = groupIdentifier;
            this.EdgeNodeIdentifier = edgeNodeIdentifier;
        }

        /// <summary>
        /// Gets the group identifier.
        /// </summary>
        public string GroupIdentifier { get; }

        /// <summary>
        /// Gets the edge node identifier.
        /// </summary>
        public string EdgeNodeIdentifier { get; }
    }

    /// <inheritdoc cref="NodeEventArgs" />
    /// <summary>
    /// A class for the device event args.
    /// </summary>
    /// <seealso cref="NodeEventArgs" />
    public class DeviceEventArgs : NodeEventArgs
    {
        /// <inheritdoc cref="NodeEventArgs" />
        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceEventArgs"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="deviceIdentifier">The device identifier.</param>
        /// <seealso cref="NodeEventArgs" />
        public DeviceEventArgs(SparkplugBase<T> sender, string groupIdentifier, string edgeNodeIdentifier, string deviceIdentifier)
            : base(sender, groupIdentifier, edgeNodeIdentifier)
        {
            this.DeviceIdentifier = deviceIdentifier;
        }

        /// <summary>
        /// Gets the device identifier.
        /// </summary>
        public string DeviceIdentifier { get; }
    }

    /// <inheritdoc cref="DeviceEventArgs" />
    /// <summary>
    /// A class for the device birth event args.
    /// </summary>
    /// <seealso cref="DeviceEventArgs" />
    public class DeviceBirthEventArgs : DeviceEventArgs
    {
        /// <inheritdoc cref="DeviceEventArgs" />
        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceBirthEventArgs"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="deviceIdentifier">The device identifier.</param>
        /// <param name="metrics">The metrics.</param>
        /// <seealso cref="DeviceEventArgs" />
        public DeviceBirthEventArgs(SparkplugBase<T> sender, string groupIdentifier, string edgeNodeIdentifier, string deviceIdentifier, IEnumerable<T> metrics)
            : base(sender, groupIdentifier, edgeNodeIdentifier, deviceIdentifier)
        {
            this.Metrics = metrics;
        }

        /// <summary>
        /// Gets the metrics.
        /// </summary>
        public IEnumerable<T> Metrics { get; }
    }

    /// <inheritdoc cref="NodeEventArgs" />
    /// <summary>
    /// A class for the node birth event args.
    /// </summary>
    /// <seealso cref="NodeEventArgs" />
    public class NodeBirthEventArgs : NodeEventArgs
    {
        /// <inheritdoc cref="NodeEventArgs" />
        /// <summary>
        /// Initializes a new instance of the <see cref="NodeBirthEventArgs"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="metrics">The metrics.</param>
        /// <seealso cref="NodeEventArgs" />
        public NodeBirthEventArgs(SparkplugBase<T> sender, string groupIdentifier, string edgeNodeIdentifier, IEnumerable<T> metrics)
            : base(sender, groupIdentifier, edgeNodeIdentifier)
        {
            this.Metrics = metrics;
        }

        /// <summary>
        /// Gets the metrics.
        /// </summary>
        public IEnumerable<T> Metrics { get; }
    }
}
