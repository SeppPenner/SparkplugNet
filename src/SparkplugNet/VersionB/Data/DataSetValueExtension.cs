// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataSetValueExtension.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   The externally used Sparkplug B data set extension class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.VersionB.Data;

/// <summary>
/// The externally used Sparkplug B data set extension class.
/// </summary>
public class DataSetValueExtension
{
    /// <summary>
    /// Gets or sets the details.
    /// </summary>
    public List<byte> Details { get; set; } = new();
}
