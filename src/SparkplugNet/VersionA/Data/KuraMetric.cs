// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KuraMetric.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   The externally used Sparkplug A Kura metric class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.VersionA.Data;

/// <summary>
/// The externally used Sparkplug A Kura metric class.
/// </summary>
[Obsolete("Sparkplug version A is obsolete since version 3 of the specification, use version B where possible.")]
public class KuraMetric : ValueBaseVersionA, IMetric
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KuraMetric"/> class.
    /// Don't use this constructor, it's only for internal purposes.
    /// </summary>
    public KuraMetric()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KuraMetric"/> class.
    /// </summary>
    /// <param name="dataType">The data type.</param>
    /// <param name="value">The value.</param>
    public KuraMetric(VersionADataTypeEnum dataType, object value)
    {
        this.SetValue(dataType, value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KuraMetric"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="dataType">The data type.</param>
    /// <param name="value">The value.</param>
    public KuraMetric(string name, VersionADataTypeEnum dataType, object value)
    {
        this.Name = name;
        this.SetValue(dataType, value);
    }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    [DefaultValue("")]
    public string Name { get; set; } = string.Empty;
}