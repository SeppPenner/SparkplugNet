// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugApplicationBase.EventArgs.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class that handles a Sparkplug application.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Core.Application;

/// <inheritdoc cref="SparkplugBase{T}"/>
/// <summary>
/// A class that handles a Sparkplug application.
/// </summary>
/// <seealso cref="SparkplugBase{T}"/>
public partial class SparkplugApplicationBase<T> : SparkplugBase<T> where T : IMetric, new()
{
    /// <summary>
    /// A class for the node data event args.
    /// </summary>
    public class NodeDataEventArgs : NodeEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NodeDataEventArgs"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="metric">The metric.</param>
        public NodeDataEventArgs(SparkplugApplicationBase<T> sender, string groupIdentifier, string edgeNodeIdentifier, T metric)
            : base(sender, groupIdentifier, edgeNodeIdentifier)
        {
            this.Metric = metric;
        }

        /// <summary>
        /// Gets the metric.
        /// </summary>
        public T Metric { get; }
    }

    /// <inheritdoc cref="NodeDataEventArgs"/>
    /// <summary>
    /// A class for the device data event args.
    /// </summary>
    /// <seealso cref="NodeDataEventArgs"/>
    public class DeviceDataEventArgs : NodeDataEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceDataEventArgs"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="deviceIdentifier">The device identifier.</param>
        /// <param name="metric">The metric.</param>
        public DeviceDataEventArgs(SparkplugApplicationBase<T> sender, string groupIdentifier, string edgeNodeIdentifier, string deviceIdentifier, T metric)
            : base(sender, groupIdentifier, edgeNodeIdentifier, metric)
        {
            this.DeviceIdentifier = deviceIdentifier;
        }

        /// <summary>
        /// Gets the device identifier.
        /// </summary>
        public string DeviceIdentifier { get; }
    }
}
