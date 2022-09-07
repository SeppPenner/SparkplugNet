namespace SparkplugNet.Core
{
    using System.Collections.Generic;

    public partial class SparkplugBase<T> : ISparkplugConnection
         where T : IMetric, new()
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
            /// <value>
            /// The sender.
            /// </value>
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
            /// <value>
            /// The group identifier.
            /// </value>
            public string GroupIdentifier { get; }

            /// <summary>
            /// Gets the node identifier.
            /// </summary>
            /// <value>
            /// The node identifier.
            /// </value>
            public string NodeIdentifier { get; }

            /// <summary>
            /// Initializes a new instance of the <see cref="NodeEventArgs"/> class.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="groupIdentifier">The group identifier.</param>
            /// <param name="nodeIdentifier">The node identifier.</param>
            public NodeEventArgs(SparkplugBase<T> sender, string groupIdentifier, string nodeIdentifier)
                : base(sender)
            {
                this.GroupIdentifier = groupIdentifier;
                this.NodeIdentifier = nodeIdentifier;
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
            /// <value>
            /// The device identifier.
            /// </value>
            public string DeviceIdentifier { get; }

            /// <summary>
            /// Initializes a new instance of the <see cref="DeviceEventArgs"/> class.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="groupIdentifier">The group identifier.</param>
            /// <param name="nodeIdentifier">The node identifier.</param>
            /// <param name="deviceIdentifier">The device identifier.</param>
            public DeviceEventArgs(SparkplugBase<T> sender, string groupIdentifier, string nodeIdentifier, string deviceIdentifier)
                : base(sender,groupIdentifier, nodeIdentifier)
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
            /// <value>
            /// The metrics.
            /// </value>
            public IEnumerable<T> Metrics { get; }
            /// <summary>
            /// Initializes a new instance of the <see cref="DeviceBirthEventArgs"/> class.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="groupIdentifier">The group identifier.</param>
            /// <param name="nodeIdentifier">The node identifier.</param>
            /// <param name="deviceIdentifier">The device identifier.</param>
            /// <param name="metrics">The metrics.</param>
            public DeviceBirthEventArgs(SparkplugBase<T> sender, string groupIdentifier, string nodeIdentifier, string deviceIdentifier, IEnumerable<T> metrics)
                : base(sender,groupIdentifier, nodeIdentifier, deviceIdentifier)
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
            /// <value>
            /// The metrics.
            /// </value>
            public IEnumerable<T> Metrics { get; }
            /// <summary>
            /// Initializes a new instance of the <see cref="DeviceBirthEventArgs"/> class.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="groupIdentifier">The group identifier.</param>
            /// <param name="nodeIdentifier">The node identifier.</param>
            /// <param name="metrics">The metrics.</param>
            public NodeBirthEventArgs(SparkplugBase<T> sender, string groupIdentifier, string nodeIdentifier, IEnumerable<T> metrics)
                : base(sender,groupIdentifier, nodeIdentifier)
            {
                this.Metrics = metrics;

            }
        }
    }
}
