// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugNode.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   Defines the SparkplugNode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.VersionB
{
    using System.Collections.Generic;

    using Serilog;

    using SparkplugNet.Core.Node;
    using SparkplugNet.VersionB.Data;

    /// <inheritdoc cref="SparkplugNodeBase{T}"/>
    public class SparkplugNode : SparkplugNodeBase<Metric>
    {
        /// <inheritdoc cref="SparkplugNodeBase{T}"/>
        /// <summary>
        /// Initializes a new instance of the <see cref="SparkplugNode"/> class.
        /// </summary>
        /// <param name="knownMetrics">The known metrics.</param>
        /// <param name="logger">The logger.</param>
        public SparkplugNode(List<Metric> knownMetrics, ILogger? logger = null) : base(knownMetrics, logger)
        {
        }
    }
}