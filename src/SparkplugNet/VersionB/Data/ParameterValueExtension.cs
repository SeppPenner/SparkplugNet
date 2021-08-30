// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterValueExtension.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   The externally used Sparkplug B parameter value extension class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.VersionB.Data
{
    using System.Collections.Generic;

    /// <summary>
    /// The externally used Sparkplug B parameter value extension class.
    /// </summary>
    public class ParameterValueExtension
    {
        /// <summary>
        /// Gets or sets the extensions.
        /// </summary>
        public List<byte> Extensions { get; set; } = new List<byte>();
    }
}
