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
    /// The property set value.
    /// </summary>
    private PropertySet? propertySetValue;

    /// <summary>
    /// The property set list value.
    /// </summary>
    private PropertySetList? propertySetListValue;

    /// <summary>
    /// The extension value.
    /// </summary>
    private PropertyValueExtension? extensionValue;

    /// <summary>
    /// Gets or sets a value indicating whether the property value is null or not.
    /// </summary>
    public bool? IsNull { get; set; }

    /// <summary>
    /// Gets or sets the property set value.
    /// </summary>
    public PropertySet? PropertySetValue
    {
        get => this.propertySetValue ?? new();
        set
        {
            this.propertySetValue = value;
            this.DataType = VersionBDataTypeEnum.PropertySet;
        }
    }

    /// <summary>
    /// Gets or sets the property set list value.
    /// </summary>
    public PropertySetList? PropertySetListValue
    {
        get => this.propertySetListValue ?? new();
        set
        {
            this.propertySetListValue = value;
            this.DataType = VersionBDataTypeEnum.PropertySet;
        }
    }

    /// <summary>
    /// Gets or sets the extension value.
    /// </summary>
    public PropertyValueExtension ExtensionValue
    {
        get => this.extensionValue ?? new();
        set
        {
            this.extensionValue = value;
            this.DataType = VersionBDataTypeEnum.Unknown;
        }
    }
}
