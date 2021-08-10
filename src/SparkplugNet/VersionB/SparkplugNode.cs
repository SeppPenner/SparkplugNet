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
    using System;
    using System.Collections.Generic;
    
    using SparkplugNet.Core.Node;

    /// <inheritdoc cref="SparkplugNodeBase{T}"/>
    public class SparkplugNode : SparkplugNodeBase<Payload.Metric>
    {
        /// <inheritdoc cref="SparkplugNodeBase{T}"/>
        /// <summary>
        /// Initializes a new instance of the <see cref="SparkplugNode"/> class.
        /// </summary>
        /// <param name="knownMetrics">The known metrics.</param>
        public SparkplugNode(List<Payload.Metric> knownMetrics) : base(knownMetrics)
        {
        }
    }
}