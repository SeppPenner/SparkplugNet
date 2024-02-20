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
    /// Gets or sets the name.
    /// </summary>
    [DefaultValue("")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the extension value.
    /// </summary>
    public ParameterValueExtension? ExtensionValue { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Parameter"/> class.
    /// </summary>
    public Parameter()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Parameter"/> class.
    /// </summary>
    /// <param name="dataType">The data type.</param>
    /// <param name="value">The value.</param>
    public Parameter(VersionBDataTypeEnum dataType, object value)
    {
        this.SetValue(dataType, value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Parameter"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="dataType">The data type.</param>
    /// <param name="value">The value.</param>
    public Parameter(string name, VersionBDataTypeEnum dataType, object value)
    {
        this.Name = name;
        this.SetValue(dataType, value);
    }
}
