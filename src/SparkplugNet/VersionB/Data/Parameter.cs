// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Parameter.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   The externally used Sparkplug B parameter class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.VersionB.Data;

/// <summary>
/// The externally used Sparkplug B parameter class.
/// </summary>
public class Parameter : ValueBaseVersionB, IMetric
{
    /// <summary>
    /// The extension value.
    /// </summary>
    private ParameterValueExtension? extensionValue;

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    [DefaultValue("")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the extension value.
    /// </summary>
    public ParameterValueExtension ExtensionValue
    {
        get => this.extensionValue ?? new();
        set
        {
            this.extensionValue = value;
            this.DataType = VersionBDataTypeEnum.Unknown;
        }
    }
}
