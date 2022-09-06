namespace SparkplugNet.Core
{
    using System.Collections.Generic;

    public partial class SparkplugBase<T> : ISparkplugConnection
         where T : IMetric, new()
    {

        /// <summary>
        /// Node event args
        /// </summary>
        /// <seealso cref="SparkplugNet.Core.SparkplugBase&lt;T&gt;" />
        public class NodeEventArgs : SparkplugEventArgs
        {
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
            /// <param name="nodeIdentifier">The node identifier.</param>
            public NodeEventArgs(SparkplugBase<T> sender, string nodeIdentifier)
                : base(sender)
            {
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
            /// <param name="nodeIdentifier">The node identifier.</param>
            /// <param name="deviceIdentifier">The device identifier.</param>
            public DeviceEventArgs(SparkplugBase<T> sender, string nodeIdentifier, string deviceIdentifier)
                : base(sender, nodeIdentifier)
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
            /// <param name="nodeIdentifier">The node identifier.</param>
            /// <param name="deviceIdentifier">The device identifier.</param>
            /// <param name="metrics">The metrics.</param>
            public DeviceBirthEventArgs(SparkplugBase<T> sender, string nodeIdentifier, string deviceIdentifier, IEnumerable<T> metrics)
                : base(sender, nodeIdentifier, deviceIdentifier)
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
            /// <param name="nodeIdentifier">The node identifier.</param>
            /// <param name="metrics">The metrics.</param>
            public NodeBirthEventArgs(SparkplugBase<T> sender, string nodeIdentifier, IEnumerable<T> metrics)
                : base(sender, nodeIdentifier)
            {
                this.Metrics = metrics;

            }
        }
    }
}
