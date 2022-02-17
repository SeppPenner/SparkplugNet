// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertySet.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   The externally used Sparkplug B property set class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.VersionB.Data;

/// <summary>
/// The externally used Sparkplug B property set class.
/// </summary>
public class PropertySet
{
    /// <summary>
    /// Gets or sets the keys.
    /// </summary>
    public List<string> Keys { get; set; } = new();

    /// <summary>
    /// Gets or sets the values.
    /// </summary>
    public List<PropertyValue> Values { get; set; } = new();

    /// <summary>
    /// Gets or sets the details.
    /// </summary>
    public List<byte> Details { get; set; } = new();
}
