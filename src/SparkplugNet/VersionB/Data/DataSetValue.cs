// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataSetValue.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   The externally used Sparkplug B data set value class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.VersionB.Data;

/// <summary>
/// The externally used Sparkplug B data set value class.
/// </summary>
public class DataSetValue : ValueBaseVersionB
{
    /// <summary>
    /// The extension value.
    /// </summary>
    private DataSetValueExtension? extensionValue;

    /// <summary>
    /// Gets or sets the extension value.
    /// </summary>
    public DataSetValueExtension ExtensionValue
    {
        get => this.extensionValue ?? new();
        set
        {
            this.extensionValue = value;
            this.DataType = VersionBDataTypeEnum.Unknown;
        }
    }

    /// <summary>
    /// Initializes the DataSetValue
    /// </summary>
    public DataSetValue()
    {
    }

    /// <summary>
    /// Initializes the DataSetValue with the given value with the given type
    /// </summary>
    public DataSetValue(VersionBDataTypeEnum type, object? value)
    {
        this.SetValue(type, value);
    }
}
