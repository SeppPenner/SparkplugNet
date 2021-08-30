// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugDevice.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   Defines the SparkplugDevice type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.VersionB
{
    using System.Collections.Generic;

    using Serilog;

    using SparkplugNet.Core.Device;
    using SparkplugNet.VersionB.Data;

    /// <inheritdoc cref="SparkplugDeviceBase{T}"/>
    public class SparkplugDevice : SparkplugDeviceBase<Metric>
    {
        /// <inheritdoc cref="SparkplugDeviceBase{T}"/>
        /// <summary>
        /// Initializes a new instance of the <see cref="SparkplugDevice"/> class.
        /// </summary>
        /// <param name="knownMetrics">The known metrics.</param>
        /// <param name="logger">The logger.</param>
        public SparkplugDevice(List<Metric> knownMetrics, ILogger? logger = null) : base(knownMetrics, logger)
        {
        }
    }
}