namespace SparkplugNet.Core.Application
{
    public partial class SparkplugApplicationBase<T> : SparkplugBase<T> where T : IMetric, new()
    {
        /// <summary>
        /// Event Args for DataReceived Events
        /// </summary>
        /// <seealso cref="SparkplugNet.Core.SparkplugBase&lt;T&gt;" />
        public class DataEventArgs : SparkplugEventArgs
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
            /// Gets the metric.
            /// </summary>
            /// <value>
            /// The metric.
            /// </value>
            public T Metric { get; }

            /// <summary>
            /// Initializes a new instance of the <see cref="DataEventArgs"/> class.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="groupIdentifier">The group identifier.</param>
            /// <param name="nodeIdentifier">The node identifier.</param>
            /// <param name="metric">The metric.</param>
            public DataEventArgs(SparkplugApplicationBase<T> sender, string groupIdentifier, string nodeIdentifier, T metric)
                : base(sender)
            {
                this.GroupIdentifier = groupIdentifier;
                this.NodeIdentifier = nodeIdentifier;
                this.Metric = metric;
            }
        }
        /// <summary>
        /// Event Args for DeviceDataReceived Events
        /// </summary>
        /// <seealso cref="SparkplugNet.Core.SparkplugBase&lt;T&gt;" />
        public class DeviceDataEventArgs : DataEventArgs
        {
            /// <summary>
            /// Gets the device identifier.
            /// </summary>
            /// <value>
            /// The device identifier.
            /// </value>
            public string DeviceIdentifier { get; }
            /// <summary>
            /// Initializes a new instance of the <see cref="DeviceDataEventArgs"/> class.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="groupIdentifier">The group identifier.</param>
            /// <param name="nodeIdentifier">The node identifier.</param>
            /// <param name="deviceIdentifier">The device identifier.</param>
            /// <param name="metric">The metric.</param>
            public DeviceDataEventArgs(SparkplugApplicationBase<T> sender, string groupIdentifier, string nodeIdentifier, string deviceIdentifier, T metric)
                : base(sender, groupIdentifier, nodeIdentifier, metric)
            {
                this.DeviceIdentifier = deviceIdentifier;
            }
        }
    }
}
