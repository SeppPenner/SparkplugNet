namespace SparkplugNet.VersionB
{
    using System.Collections.Generic;
    
    using SparkplugNet.Core.Device;

    /// <inheritdoc cref="SparkplugDeviceBase{T}"/>
    public class SparkplugDevice : SparkplugDeviceBase<Payload.Metric>
    {
        /// <inheritdoc cref="SparkplugDeviceBase{T}"/>
        /// <summary>
        /// Initializes a new instance of the <see cref="SparkplugDevice"/> class.
        /// </summary>
        /// <param name="knownMetrics">The known metrics.</param>
        public SparkplugDevice(List<Payload.Metric> knownMetrics) : base(knownMetrics)
        {
        }
    }
}