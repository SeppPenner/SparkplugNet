// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BasicMetrics.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class containing the basic metrics.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Metrics
{
    using System;

    using SparkplugNet.Enumerations;

    /// Todo: Remove later
    /// <summary>
    /// A class containing the basic metrics.
    /// </summary>
    public class BasicMetrics
    {
        /// <summary>
        /// Gets or sets the connection status.
        /// </summary>
        public SparkplugConnectionStatus ConnectionStatus { get; set; }

        /// <summary>
        /// Gets or sets the last online time.
        /// </summary>
        public DateTimeOffset? LastOnlineTime { get; set; }

        /// <summary>
        /// Gets or sets the offline message received time.
        /// </summary>
        public DateTimeOffset? OfflineMessageReceivedTime { get; set; }
    }
}