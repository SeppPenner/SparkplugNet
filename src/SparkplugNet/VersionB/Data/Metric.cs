// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Metric.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   The externally used Sparkplug B metric class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.VersionB.Data;

/// <summary>
/// The externally used Sparkplug B metric class.
/// </summary>
public class Metric : ValueBaseVersionB, IMetric
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Metric"/> class.
    /// </summary>
    public Metric()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Metric"/> class.
    /// </summary>
    /// <param name="dataType">The data type.</param>
    /// <param name="value">The value.</param>
    /// <param name="timestamp">The timestamp.</param>
    public Metric(VersionBDataTypeEnum dataType, object value, DateTimeOffset? timestamp = null)
    {
        this.SetValue(dataType, value);

        if (timestamp is not null)
        {
            this.Timestamp = (ulong)timestamp.Value.ToUnixTimeMilliseconds();
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Metric"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="dataType">The data type.</param>
    /// <param name="value">The value.</param>
    /// <param name="timestamp">The timestamp.</param>
    public Metric(string name, VersionBDataTypeEnum dataType, object value, DateTimeOffset? timestamp = null)
    {
        this.Name = name;
        this.SetValue(dataType, value);

        if (timestamp is not null)
        {
            this.Timestamp = (ulong)timestamp.Value.ToUnixTimeMilliseconds();
        }
    }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    [DefaultValue("")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the alias.
    /// </summary>
    public ulong? Alias { get; set; }

    /// <summary>
    /// Gets or sets the timestamp.
    /// </summary>
    public ulong? Timestamp { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the metric is historical or not.
    /// </summary>
    public bool? IsHistorical { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the metric is transient or not.
    /// </summary>
    public bool? IsTransient { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the metric is null or not.
    /// </summary>
    public bool? IsNull { get; set; }

    /// <summary>
    /// Gets or sets the meta data.
    /// </summary>
    public MetaData? MetaData { get; set; }

    /// <summary>
    /// Gets or sets the properties.
    /// </summary>
    public PropertySet? PropertySetValue { get; set; }

    /// <summary>
    /// Gets or sets the bytes value.
    /// </summary>
    public virtual byte[] BytesValue { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// Gets or sets the data set value.
    /// </summary>
    public DataSet? DataSetValue { get; set; }

    /// <summary>
    /// Gets or sets the template value.
    /// </summary>
    public Template? TemplateValue { get; set; }

    /// <summary>
    /// Gets or sets the extension value.
    /// </summary>
    public MetricValueExtension? ExtensionValue { get; set; }

    /// <summary>
    /// Sets the value.
    /// </summary>
    /// <param name="dataType">Type of the data.</param>
    /// <param name="value">The value.</param>
    /// <returns>The metric.</returns>
    public override IValue<VersionBDataTypeEnum> SetValue(VersionBDataTypeEnum dataType, object? value)
    {
        this.IsNull = value is null;

        switch (dataType)
        {
            case VersionBDataTypeEnum.Bytes:
            case VersionBDataTypeEnum.File:
                this.BytesValue = value.ConvertOrDefaultTo<byte[]>();
                break;
            case VersionBDataTypeEnum.DataSet:
                this.DataSetValue = value.ConvertOrDefaultTo<DataSet>();
                break;
            case VersionBDataTypeEnum.Template:
                this.TemplateValue = value.ConvertOrDefaultTo<Template>();
                break;
            case VersionBDataTypeEnum.PropertySet:
                this.PropertySetValue = value.ConvertOrDefaultTo<PropertySet>();
                break;
            default:
                base.SetValue(dataType, value);
                break;
        }

        return this;
    }

    /// <summary>
    /// Gets the value.
    /// </summary>
    public override object? Value => base.Value ?? (this.DataType switch
    {
        VersionBDataTypeEnum.Bytes
        or VersionBDataTypeEnum.File => this.BytesValue,
        VersionBDataTypeEnum.DataSet => this.DataSetValue,
        VersionBDataTypeEnum.Template => this.TemplateValue,
        VersionBDataTypeEnum.PropertySet => this.PropertySetValue,
        _ => null,
    });
}
