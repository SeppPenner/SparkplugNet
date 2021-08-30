// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyValueExtension.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   The externally used Sparkplug B property value extension class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.VersionB.Data
{
    using System.Collections.Generic;

    /// <summary>
    /// The externally used Sparkplug B property value extension class.
    /// </summary>
    public class PropertyValueExtension
    {
        /// <summary>
        /// Gets or sets the details.
        /// </summary>
        public List<byte> Details { get; set; } = new List<byte>();
    }
}
