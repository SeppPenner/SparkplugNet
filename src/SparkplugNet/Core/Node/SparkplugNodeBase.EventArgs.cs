namespace SparkplugNet.Core.Node
{
    public abstract partial class SparkplugNodeBase<T> : SparkplugBase<T> where T : IMetric, new()
    {
        /// <summary>
        /// CommandEventArgs
        /// </summary>
        /// <seealso cref="SparkplugNet.Core.SparkplugBase&lt;T&gt;" />
        public class CommandEventArgs : SparkplugEventArgs
        {
            /// <summary>
            /// Gets the metric value.
            /// </summary>
            /// <value>
            /// The metric value.
            /// </value>
            public T Metric { get; }

            /// <summary>
            /// Initializes a new instance of the <see cref="CommandEventArgs"/> class.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="metric">The metric value.</param>
            public CommandEventArgs(SparkplugNodeBase<T> sender, T metric)
                : base(sender)
            {
                this.Metric = metric;
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
            public string Status { get;  }
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

        /// <summary>
        /// Device event args
        /// </summary>
        /// <seealso cref="SparkplugNet.Core.SparkplugBase&lt;T&gt;" />
        public class DeviceEventArgs : SparkplugEventArgs
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
            /// <param name="deviceIdentifier">The device identifier.</param>
            public DeviceEventArgs(SparkplugNodeBase<T> sender, string deviceIdentifier)
                : base(sender)
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
            /// <param name="deviceIdentifier">The device identifier.</param>
            /// <param name="metrics">The metrics.</param>
            public DeviceBirthEventArgs(SparkplugNodeBase<T> sender, string deviceIdentifier, IEnumerable<T> metrics)
                : base(sender, deviceIdentifier)
            {
                this.Metrics = metrics;

            }
        }
    }
}
