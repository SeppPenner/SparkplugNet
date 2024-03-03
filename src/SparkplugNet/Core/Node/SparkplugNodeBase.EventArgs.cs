// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugNodeBase.EventArgs.cs" company="Hämmer Electronics">
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
    /// <summary>
    /// A class for the node command event args.
    /// </summary>
    public class NodeCommandEventArgs : NodeEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NodeCommandEventArgs"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="metrics">The metrics.</param>
        public NodeCommandEventArgs(SparkplugNodeBase<T> sender, string groupIdentifier, string edgeNodeIdentifier, IEnumerable<T> metrics)
            : base(sender, groupIdentifier, edgeNodeIdentifier)
        {
            this.Metrics = metrics;
        }

        /// <summary>
        /// Gets the metrics.
        /// </summary>
        public IEnumerable<T> Metrics { get; }
    }

    /// <inheritdoc cref="NodeCommandEventArgs"/>
    /// <summary>
    /// A class for the device command event args.
    /// </summary>
    /// <seealso cref="NodeCommandEventArgs"/>
    public sealed class DeviceCommandEventArgs : NodeCommandEventArgs
    {
        /// <inheritdoc cref="NodeCommandEventArgs"/>
        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceCommandEventArgs"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="deviceIdentifier">The device identifier.</param>
        /// <param name="metrics">The metrics.</param>
        /// <seealso cref="NodeCommandEventArgs"/>
        public DeviceCommandEventArgs(SparkplugNodeBase<T> sender, string groupIdentifier, string edgeNodeIdentifier, string deviceIdentifier, IEnumerable<T> metrics)
           : base(sender, groupIdentifier, edgeNodeIdentifier, metrics)
        {
            this.DeviceIdentifier = deviceIdentifier;
        }

        /// <summary>
        /// Gets the device identifier.
        /// </summary>
        public string DeviceIdentifier { get; }
    }

    /// <summary>
    /// A class for the status message event args.
    /// </summary>
    public sealed class StatusMessageEventArgs : SparkplugEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatusMessageEventArgs"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="status">The status.</param>
        public StatusMessageEventArgs(SparkplugNodeBase<T> sender, string status) : base(sender)
        {
            this.Status = status;
        }

        /// <summary>
        /// Gets the status.
        /// </summary>
        public string Status { get; }
    }
}
