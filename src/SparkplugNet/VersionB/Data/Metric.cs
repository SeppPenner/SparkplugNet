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
[DebuggerDisplay("{Name}|{Value}")]
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
    public Metric(VersionBDataTypeEnum dataType, object? value, DateTimeOffset? timestamp = null)
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
    public Metric(string name, VersionBDataTypeEnum dataType, object? value, DateTimeOffset? timestamp = null)
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
    public bool IsNull { get; protected set; }

    /// <summary>
    /// Gets or sets the meta data.
    /// </summary>
    public MetaData? MetaData { get; set; }

    /// <summary>
    /// Gets the value.
    /// </summary>
    public override object? Value => base.Value ?? this.GetValue();

    /// <summary>
    /// Sets the value.
    /// </summary>
    /// <param name="dataType">Type of the data.</param>
    /// <param name="value">The value.</param>
    /// <returns>The metric.</returns>
    public override IValue<VersionBDataTypeEnum> SetValue(VersionBDataTypeEnum dataType, object? value)
    {
        this.IsNull = value is null;
        this.DataType = dataType;

        switch (dataType)
        {
            case VersionBDataTypeEnum.Int8:
                base.SetValue(VersionBDataTypeEnum.Int8, value);
                break;
            case VersionBDataTypeEnum.Int16:
                base.SetValue(VersionBDataTypeEnum.Int16, value);
                break;
            case VersionBDataTypeEnum.Int32:
                base.SetValue(VersionBDataTypeEnum.Int32, value);
                break;
            case VersionBDataTypeEnum.Int64:
                base.SetValue(VersionBDataTypeEnum.Int64, value);
                break;
            case VersionBDataTypeEnum.UInt8:
                base.SetValue(VersionBDataTypeEnum.UInt8, value);
                break;
            case VersionBDataTypeEnum.UInt16:
                base.SetValue(VersionBDataTypeEnum.UInt16, value);
                break;
            case VersionBDataTypeEnum.UInt32:
                base.SetValue(VersionBDataTypeEnum.UInt32, value);
                break;
            case VersionBDataTypeEnum.UInt64:
                base.SetValue(VersionBDataTypeEnum.UInt64, value);
                break;
            case VersionBDataTypeEnum.DateTime:
                base.SetValue(VersionBDataTypeEnum.DateTime, value);
                break;
            case VersionBDataTypeEnum.Float:
                base.SetValue(VersionBDataTypeEnum.Float, value);
                break;
            case VersionBDataTypeEnum.Double:
                base.SetValue(VersionBDataTypeEnum.Double, value);
                break;
            case VersionBDataTypeEnum.Boolean:
                base.SetValue(VersionBDataTypeEnum.Boolean, value);
                break;
            case VersionBDataTypeEnum.String:
                base.SetValue(VersionBDataTypeEnum.String, value);
                break;
            case VersionBDataTypeEnum.Text:
                base.SetValue(VersionBDataTypeEnum.Text, value);
                break;
            case VersionBDataTypeEnum.Uuid:
                base.SetValue(VersionBDataTypeEnum.Uuid, value);
                break;
            case VersionBDataTypeEnum.Bytes:
            case VersionBDataTypeEnum.File:
                this.ObjectValue = value.ConvertOrDefaultTo<byte[]>();
                break;
            case VersionBDataTypeEnum.DataSet:
                this.ObjectValue = value.ConvertOrDefaultTo<DataSet>();
                break;
            case VersionBDataTypeEnum.Template:
                this.ObjectValue = value.ConvertOrDefaultTo<Template>();
                break;
            case VersionBDataTypeEnum.PropertySet:
                this.ObjectValue = value.ConvertOrDefaultTo<PropertySet>();
                break;
            case VersionBDataTypeEnum.Int8Array:
                this.ObjectValue = value.ConvertOrDefaultTo<sbyte[]>();
                break;
            case VersionBDataTypeEnum.Int16Array:
                this.ObjectValue = value.ConvertOrDefaultTo<short[]>();
                break;
            case VersionBDataTypeEnum.Int32Array:
                this.ObjectValue = value.ConvertOrDefaultTo<int[]>();
                break;
            case VersionBDataTypeEnum.Int64Array:
                this.ObjectValue = value.ConvertOrDefaultTo<long[]>();
                break;
            case VersionBDataTypeEnum.UInt8Array:
                this.ObjectValue = value.ConvertOrDefaultTo<byte[]>();
                break;
            case VersionBDataTypeEnum.UInt16Array:
                this.ObjectValue = value.ConvertOrDefaultTo<ushort[]>();
                break;
            case VersionBDataTypeEnum.UInt32Array:
                this.ObjectValue = value.ConvertOrDefaultTo<uint[]>();
                break;
            case VersionBDataTypeEnum.UInt64Array:
                this.ObjectValue = value.ConvertOrDefaultTo<ulong[]>();
                break;
            case VersionBDataTypeEnum.DateTimeArray:
                if (value is null)
                {
                    return this;
                }

                if (value is DateTime[] dateTimes)
                {
                    this.ObjectValue = dateTimes.Select(d => (ulong)new DateTimeOffset(d).ToUnixTimeMilliseconds()).ToArray();
                    break;
                }
                else if (value is ulong[] ulongValues)
                {
                    this.ObjectValue = ulongValues;
                    break;
                }
                else
                {
                    throw new InvalidOperationException($"Value {value} is not a valid date time array value");
                }
            case VersionBDataTypeEnum.FloatArray:
                this.ObjectValue = value.ConvertOrDefaultTo<float[]>();
                break;
            case VersionBDataTypeEnum.DoubleArray:
                this.ObjectValue = value.ConvertOrDefaultTo<double[]>();
                break;
            case VersionBDataTypeEnum.BooleanArray:
                this.ObjectValue = value.ConvertOrDefaultTo<bool[]>();
                break;
            case VersionBDataTypeEnum.StringArray:
                this.ObjectValue = value.ConvertOrDefaultTo<string[]>();
                break;
            // Todo: What to do here?
            case VersionBDataTypeEnum.PropertySetList:
                break;
            case VersionBDataTypeEnum.Unknown:
            default:
                break;
        }

        return this;
    }

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <returns>The value as <see cref="object"/>.</returns>
    private object? GetValue()
    {
        if (this.IsNull)
        {
            return null;
        }

        switch (this.DataType)
        {
            case VersionBDataTypeEnum.Bytes:
            case VersionBDataTypeEnum.File:
                return this.ObjectValue.ConvertOrDefaultTo<byte[]>();
            case VersionBDataTypeEnum.DataSet:
                return this.ObjectValue.ConvertOrDefaultTo<DataSet>();
            case VersionBDataTypeEnum.Template:
                return this.ObjectValue.ConvertOrDefaultTo<Template>();
            case VersionBDataTypeEnum.PropertySet:
                return this.ObjectValue.ConvertOrDefaultTo<PropertySet>();
            case VersionBDataTypeEnum.Int8Array:
                return this.ObjectValue.ConvertOrDefaultTo<sbyte[]>();
            case VersionBDataTypeEnum.Int16Array:
                return this.ObjectValue.ConvertOrDefaultTo<short[]>();
            case VersionBDataTypeEnum.Int32Array:
                return this.ObjectValue.ConvertOrDefaultTo<int[]>();
            case VersionBDataTypeEnum.Int64Array:
                return this.ObjectValue.ConvertOrDefaultTo<long[]>();
            case VersionBDataTypeEnum.UInt8Array:
                return this.ObjectValue.ConvertOrDefaultTo<byte[]>();
            case VersionBDataTypeEnum.UInt16Array:
                return this.ObjectValue.ConvertOrDefaultTo<ushort[]>();
            case VersionBDataTypeEnum.UInt32Array:
                return this.ObjectValue.ConvertOrDefaultTo<uint[]>();
            case VersionBDataTypeEnum.UInt64Array:
                return this.ObjectValue.ConvertOrDefaultTo<ulong[]>();
            case VersionBDataTypeEnum.DateTimeArray:
                return this.ObjectValue.ConvertOrDefaultTo<ulong[]>().Select(v => DateTimeOffset.FromUnixTimeMilliseconds((long)v).DateTime).ToArray();
            case VersionBDataTypeEnum.FloatArray:
                return this.ObjectValue.ConvertOrDefaultTo<float[]>();
            case VersionBDataTypeEnum.DoubleArray:
                return this.ObjectValue.ConvertOrDefaultTo<double[]>();
            case VersionBDataTypeEnum.BooleanArray:
                return this.ObjectValue.ConvertOrDefaultTo<bool[]>();
            case VersionBDataTypeEnum.StringArray:
                return this.ObjectValue.ConvertOrDefaultTo<string[]>();
            // Todo: What to do here?
            case VersionBDataTypeEnum.PropertySetList:
            default:
                return base.Value;
        }
    }
}
