// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugNode.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   Defines the SparkplugNode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.VersionA
{
    using System.Collections.Generic;
    
    using SparkplugNet.Core.Node;

    /// <inheritdoc cref="SparkplugNodeBase{T}"/>
    public class SparkplugNode : SparkplugNodeBase<Payload.KuraMetric>
    {
        /// <inheritdoc cref="SparkplugNodeBase{T}"/>
        /// <summary>
        /// Initializes a new instance of the <see cref="SparkplugNode"/> class.
        /// </summary>
        /// <param name="knownMetrics">The known metrics.</param>
        public SparkplugNode(List<Payload.KuraMetric> knownMetrics) : base(knownMetrics)
        {
        }
    }
}