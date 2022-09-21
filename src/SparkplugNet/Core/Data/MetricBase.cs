// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MetricBase.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A base class for metrics.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Core.Data;

/// <inheritdoc cref="IMetric"/>
/// <summary>
/// A base class for metrics.
/// </summary>
/// <typeparam name="DataTypeEnum">The type of the ata type enum.</typeparam>
/// <seealso cref="IMetric" />
public abstract class MetricBase<DataTypeEnum> : IMetric where DataTypeEnum : Enum
{
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    [DefaultValue("")]
    public virtual string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets the value.
    /// </summary>
    public abstract object? Value { get; }

    /// <summary>
    /// Gets or sets the type.
    /// </summary>
    public DataTypeEnum Type { get; set; } = default!;

    /// <summary>
    /// Sets the value.
    /// </summary>
    /// <param name="dataType">Type of the data.</param>
    /// <param name="value">The value.</param>
    /// <returns>The metric value.</returns>
    public abstract MetricBase<DataTypeEnum> SetValue(DataTypeEnum dataType, object value);

    /// <summary>
    /// Converts the value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="objValue">The object value.</param>
    /// <returns>The converted metric value.</returns>
    protected T? ConvertValue<T>(object objValue)
    {
        if (objValue is null)
        {
            return default;
        }
        else
        {
            return objValue is T valueAsT ? valueAsT : (T)Convert.ChangeType(objValue, typeof(T));
        }
    }
}
