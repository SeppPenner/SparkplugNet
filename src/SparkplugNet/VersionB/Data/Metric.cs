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
    public PropertySet? PropertySetValue { get; protected set; }

    /// <summary>
    /// Gets or sets the bytes value.
    /// </summary>
    public byte[] BytesValue { get; protected set; } = Array.Empty<byte>();

    /// <summary>
    /// Gets or sets the data set value.
    /// </summary>
    public DataSet? DataSetValue { get; protected set; }

    /// <summary>
    /// Gets or sets the template value.
    /// </summary>
    public Template? TemplateValue { get; protected set; }

    /// <summary>
    /// Gets or sets the extension value.
    /// </summary>
    public MetricValueExtension? ExtensionValue { get; protected set; }

    /// <summary>
    /// Sets the value.
    /// </summary>
    /// <param name="dataType">Type of the data.</param>
    /// <param name="value">The value.</param>
    /// <returns>The metric.</returns>
    public override IValue<VersionBDataTypeEnum> SetValue(VersionBDataTypeEnum dataType, object? value)
    {
        this.IsNull = value is null;

        // Todo: This is the correct code, test if anything is missing
        //switch (dataType)
        //{
        //    case VersionBDataTypeEnum.Bytes:
        //    case VersionBDataTypeEnum.File:
        //        this.BytesValue = value.ConvertOrDefaultTo<byte[]>();
        //        break;
        //    case VersionBDataTypeEnum.DataSet:
        //        this.DataSetValue = value.ConvertOrDefaultTo<DataSet>();
        //        break;
        //    case VersionBDataTypeEnum.Template:
        //        this.TemplateValue = value.ConvertOrDefaultTo<Template>();
        //        break;
        //    case VersionBDataTypeEnum.PropertySet:
        //        this.PropertySetValue = value.ConvertOrDefaultTo<PropertySet>();
        //        break;
        //    default:
        //        base.SetValue(dataType, value);
        //        break;
        //}

        switch (metric.DataType)
        {
            case VersionBDataTypeEnum.Int8:
                metric.SetValue(VersionBDataTypeEnum.Int8, protoMetric.IntValue);
                break;
            case VersionBDataTypeEnum.Int16:
            case VersionBDataTypeEnum.Int32:
            case VersionBDataTypeEnum.UInt8:
            case VersionBDataTypeEnum.UInt16:
                metric.IntValue = protoMetric.IntValue;
                break;
            case VersionBDataTypeEnum.Int64:
            case VersionBDataTypeEnum.UInt32:
            case VersionBDataTypeEnum.UInt64:
            case VersionBDataTypeEnum.DateTime:
                metric.LongValue = protoMetric.LongValue;
                break;
            case VersionBDataTypeEnum.Float:
                metric.FloatValue = protoMetric.FloatValue;
                break;
            case VersionBDataTypeEnum.Double:
                metric.DoubleValue = protoMetric.DoubleValue;
                break;
            case VersionBDataTypeEnum.Boolean:
                metric.BooleanValue = protoMetric.BooleanValue;
                break;
            case VersionBDataTypeEnum.String:
            case VersionBDataTypeEnum.Text:
            case VersionBDataTypeEnum.Uuid:
                metric.StringValue = protoMetric.StringValue;
                break;
            case VersionBDataTypeEnum.Bytes:
            case VersionBDataTypeEnum.File:
                metric.BytesValue = protoMetric.BytesValue;
                break;
            case VersionBDataTypeEnum.DataSet:
                metric.DataSetValue = ConvertVersionBDataSet(protoMetric.DataSetValue);
                break;
            case VersionBDataTypeEnum.Template:
                metric.TemplateValue = ConvertVersionBTemplate(protoMetric.TemplateValue);
                break;
            case VersionBDataTypeEnum.PropertySet:
                metric.PropertySetValue = ConvertVersionBPropertySet(protoMetric.PropertySetValue);
                break;
            case VersionBDataTypeEnum.Int8Array:
            case VersionBDataTypeEnum.Int16Array:
            case VersionBDataTypeEnum.Int32Array:
            case VersionBDataTypeEnum.Int64Array:
            case VersionBDataTypeEnum.UInt8Array:
            case VersionBDataTypeEnum.UInt16Array:
            case VersionBDataTypeEnum.UInt32Array:
            case VersionBDataTypeEnum.UInt64Array:
            case VersionBDataTypeEnum.FloatArray:
            case VersionBDataTypeEnum.DoubleArray:
            case VersionBDataTypeEnum.BooleanArray:
            case VersionBDataTypeEnum.StringArray:
            case VersionBDataTypeEnum.DateTimeArray:
                metric.BytesValue = protoMetric.BytesValue;
                break;
            case VersionBDataTypeEnum.PropertySetList:
            case VersionBDataTypeEnum.Unknown:
            default:
                throw new ArgumentOutOfRangeException(nameof(protoMetric.DataType), protoMetric.DataType, "Unknown metric data type");
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
