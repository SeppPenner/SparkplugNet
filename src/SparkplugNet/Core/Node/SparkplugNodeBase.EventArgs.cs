namespace SparkplugNet.Core.Node
{
    public abstract partial class SparkplugNodeBase<T> : SparkplugBase<T> where T : IMetric, new()
    {
        /// <summary>
        /// NodeCommandEventArgs
        /// </summary>
        /// <seealso cref="SparkplugNet.Core.SparkplugBase&lt;T&gt;" />
        public class NodeCommandEventArgs : NodeEventArgs
        {
            /// <summary>
            /// Gets the metric value.
            /// </summary>
            /// <value>
            /// The metric value.
            /// </value>
            public T Metric { get; }

            /// <summary>
            /// Initializes a new instance of the <see cref="NodeCommandEventArgs"/> class.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="groupIdentifier">The group identifier.</param>
            /// <param name="nodeIdentifier">The node identifier.</param>
            /// <param name="metric">The metric value.</param>
            public NodeCommandEventArgs(SparkplugNodeBase<T> sender, string groupIdentifier, string nodeIdentifier, T metric)
                : base(sender,groupIdentifier,nodeIdentifier)
            {
                this.Metric = metric;
            }
        }

        /// <summary>
        /// DeviceCommandEventArgs
        /// </summary>
        /// <seealso cref="SparkplugNet.Core.SparkplugBase&lt;T&gt;" />
        public class DeviceCommandEventArgs : NodeCommandEventArgs
        {
            /// <summary>
            /// Gets the device identifier.
            /// </summary>
            public string DeviceIdentifier { get; }

            /// <summary>
            /// Initializes a new instance of the <see cref="DeviceCommandEventArgs"/> class.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="groupIdentifier">The group identifier.</param>
            /// <param name="nodeIdentifier">The node identifier.</param>
            /// <param name="deviceIdentifier">The device identifier.</param>
            /// <param name="metric">The metric.</param>
            public DeviceCommandEventArgs(SparkplugNodeBase<T> sender, string groupIdentifier, string nodeIdentifier, string deviceIdentifier, T metric)
               : base(sender, groupIdentifier, nodeIdentifier, metric)
            {
                this.DeviceIdentifier = deviceIdentifier;
            }
        }

        /// <summary>
        /// StatusMessageEvnetArgs
        /// </summary>
        /// <seealso cref="SparkplugNet.Core.SparkplugBase&lt;T&gt;" />
        public class StatusMessageEvnetArgs : SparkplugEventArgs
        {
            /// <summary>
            /// Gets the status.
            /// </summary>
            /// <value>
            /// The status.
            /// </value>
            public string Status { get; }
            /// <summary>
            /// Initializes a new instance of the <see cref="StatusMessageEvnetArgs"/> class.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="status">The status.</param>
            public StatusMessageEvnetArgs(SparkplugNodeBase<T> sender, string status)
                : base(sender)
            {
                this.Status = status;
            }
        }
    }
}
