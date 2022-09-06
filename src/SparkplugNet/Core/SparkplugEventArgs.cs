namespace SparkplugNet.Core
{
    using System;

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
        public ISparkplugConnection Sender { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SparkplugEventArgs"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public SparkplugEventArgs(ISparkplugConnection sender)
        {
            this.Sender = sender;   
        }
    }
}
