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
public class KuraMetric : ValueBaseVersionA, IMetric
{
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    [DefaultValue("")]
    public virtual string Name { get; set; } = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="KuraMetric"/> class.
    /// </summary>
    public KuraMetric()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KuraMetric"/> class.
    /// </summary>
    /// <param name="strName">Name of the string.</param>
    /// <param name="dataType">Type of the data.</param>
    /// <param name="value">The value.</param>
    public KuraMetric(string strName, DataType dataType, object value)
    {
        this.Name = strName;
        this.SetValue(dataType, value);
    }
}