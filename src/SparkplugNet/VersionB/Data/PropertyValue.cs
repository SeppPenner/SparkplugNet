// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyValue.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   The externally used Sparkplug B property value class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.VersionB.Data;

/// <summary>
/// The externally used Sparkplug B property value class.
/// </summary>
public class PropertyValue : ValueBaseVersionB
{
    /// <summary>
    /// Gets or sets a value indicating whether the property value is null or not.
    /// </summary>
    public bool? IsNull { get; protected set; }

    /// <summary>
    /// Gets or sets the property set value.
    /// </summary>
    public PropertySet? PropertySetValue { get; protected set; }

    /// <summary>
    /// Gets or sets the property set list value.
    /// </summary>
    public PropertySetList? PropertySetListValue { get; protected set; }

    /// <summary>
    /// Gets or sets the extension value.
    /// </summary>
    public PropertyValueExtension? ExtensionValue { get; protected set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyValue"/> class.
    /// </summary>
    public PropertyValue()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyValue"/> class.
    /// </summary>
    /// <param name="dataType">The data type.</param>
    /// <param name="value">The value.</param>
    public PropertyValue(VersionBDataTypeEnum dataType, object? value)
    {
        this.SetValue(dataType, value);
    }
}
