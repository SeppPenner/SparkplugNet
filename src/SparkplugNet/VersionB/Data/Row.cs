// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Row.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   The externally used Sparkplug B row class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.VersionB.Data;

/// <summary>
/// The externally used Sparkplug B row class.
/// </summary>
public sealed class Row
{
    /// <summary>
    /// Gets or sets the elements.
    /// </summary>
    public List<DataSetValue> Elements { get; set; } = [];
}
