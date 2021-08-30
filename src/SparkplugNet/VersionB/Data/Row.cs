// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Row.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   The externally used Sparkplug B row class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.VersionB.Data
{
    using System.Collections.Generic;

    /// <summary>
    /// The externally used Sparkplug B row class.
    /// </summary>
    public class Row
    {
        /// <summary>
        /// Gets or sets the elements.
        /// </summary>
        public List<DataSetValue> Elements { get; set; } = new List<DataSetValue>();

        /// <summary>
        /// Gets or sets the details.
        /// </summary>
        public List<byte> Details { get; set; } = new List<byte>();
    }
}
