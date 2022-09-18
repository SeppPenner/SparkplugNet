// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Metric.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   The externally used Sparkplug B metric class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.VersionB.Data;

using SparkplugNet.Core.Data;

/// <summary>
/// The externally used Sparkplug B metric class.
/// </summary>
public class Metric : MetricBase<DataType>
{
    /// <summary>
    /// The integer value.
    /// </summary>
    private uint? integerValue;

    /// <summary>
    /// The long value.
    /// </summary>
    private ulong? longValue;

    /// <summary>
    /// The float value.
    /// </summary>
    private float? floatValue;

    /// <summary>
    /// The double value.
    /// </summary>
    private double? doubleValue;

    /// <summary>
    /// The boolean value.
    /// </summary>
    private bool? booleanValue;

    /// <summary>
    /// The string value.
    /// </summary>
    private string? stringValue;

    /// <summary>
    /// The bytes value.
    /// </summary>
    private byte[]? bytesValue;

    /// <summary>
    /// The template value.
    /// </summary>
    private Template? templateValue;

    /// <summary>
    /// The data set value.
    /// </summary>
    private DataSet? dataSetValue;

    /// <summary>
    /// The extension value.
    /// </summary>
    private MetricValueExtension? extensionValue;

    /// <summary>
    /// Gets or sets the alias.
    /// </summary>
    public ulong Alias { get; set; }

    /// <summary>
    /// Gets or sets the timestamp.
    /// </summary>
    public ulong Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the data type.
    /// </summary>
    public uint DataType
    {
        get { return (uint)this.Type; }
        set { this.Type = (DataType)value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the metric is historical or not.
    /// </summary>
    public bool IsHistorical { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the metric is transient or not.
    /// </summary>
    public bool IsTransient { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the metric is null or not.
    /// </summary>
    public bool IsNull { get; set; }

    /// <summary>
    /// Gets or sets the meta data.
    /// </summary>
    public MetaData? Metadata { get; set; }

    /// <summary>
    /// Gets or sets the properties.
    /// </summary>
    public PropertySet? Properties { get; set; }

    /// <summary>
    /// Gets or sets the integer value.
    /// </summary>
    public uint IntValue
    {
        get => this.integerValue ?? default;
        set
        {
            this.integerValue = value;
            this.Type = VersionBDataTypeEnum.Int32;
        }
    }

    /// <summary>
    /// Gets or sets the long value.
    /// </summary>
    public ulong LongValue
    {
        get => this.longValue ?? default;
        set
        {
            this.longValue = value;
            this.Type = VersionBDataTypeEnum.Int64;
        }
    }

    /// <summary>
    /// Gets or sets the float value.
    /// </summary>
    public float FloatValue
    {
        get => this.floatValue ?? default;
        set
        {
            this.floatValue = value;
            this.Type = VersionBDataTypeEnum.Float;
        }
    }

    /// <summary>
    /// Gets or sets the double value.
    /// </summary>
    public double DoubleValue
    {
        get => this.doubleValue ?? default;
        set
        {
            this.doubleValue = value;
            this.Type = VersionBDataTypeEnum.Double;
        }
    }

    /// <summary>
    /// Gets or sets the boolean value.
    /// </summary>
    public bool BooleanValue
    {
        get => this.booleanValue ?? default;
        set
        {
            this.booleanValue = value;
            this.Type = VersionBDataTypeEnum.Boolean;
        }
    }

    /// <summary>
    /// Gets or sets the string value.
    /// </summary>
    [DefaultValue("")]
    public string StringValue
    {
        get => this.stringValue ?? string.Empty;
        set
        {
            this.stringValue = value;
            this.Type = VersionBDataTypeEnum.String;
        }
    }

    /// <summary>
    /// Gets or sets the bytes value.
    /// </summary>
    public byte[] BytesValue
    {
        get => this.bytesValue ?? Array.Empty<byte>();
        set
        {
            this.bytesValue = value;
            this.Type = VersionBDataTypeEnum.Bytes;
        }
    }

    /// <summary>
    /// Gets or sets the data set value.
    /// </summary>
    public DataSet DataSetValue
    {
        get => this.dataSetValue ?? new();
        set
        {
            this.dataSetValue = value;
            this.Type = VersionBDataTypeEnum.DataSet;
        }
    }

    /// <summary>
    /// Gets or sets the template value.
    /// </summary>
    public Template? TemplateValue
    {
        get => this.templateValue;
        set
        {
            this.templateValue = value;
            this.Type = VersionBDataTypeEnum.Template;
        }
    }

    /// <summary>
    /// Gets or sets the extension value.
    /// </summary>
    public MetricValueExtension? ExtensionValue
    {
        get => this.extensionValue;
        set
        {
            this.extensionValue = value;
            this.Type = VersionBDataTypeEnum.Unknown;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Metric"/> class.
    /// </summary>
    public Metric()
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Metric"/> class.
    /// </summary>
    /// <param name="strName">Name of the string.</param>
    /// <param name="dataType">Type of the data.</param>
    /// <param name="value">The value.</param>
    /// <param name="timestamp">The timestamp.</param>
    public Metric(string strName, VersionBDataTypeEnum dataType, object value, DateTimeOffset? timestamp = null)
    {
        this.Name = strName;
        this.SetValue(dataType, value);
        if (timestamp is not null)
        {
            this.Timestamp = (ulong)(timestamp.Value.ToUnixTimeMilliseconds());
        }
    }

    /// <summary>
    /// Sets the value.
    /// </summary>
    /// <param name="dataType">Type of the data.</param>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public override MetricBase<VersionBDataTypeEnum> SetValue(VersionBDataTypeEnum dataType, object value)
    {
        if (value is not null)
        {
            switch (dataType)
            {
                case VersionBDataTypeEnum.PropertySetList:
                case VersionBDataTypeEnum.Unknown:
                default:
                case VersionBDataTypeEnum.Int8:
                    this.IntValue = this.ConvertValue<uint>(value);
                    break;
                case VersionBDataTypeEnum.Int16:
                    this.IntValue = this.ConvertValue<uint>(value);
                    break;
                case VersionBDataTypeEnum.Int32:
                    this.IntValue = this.ConvertValue<uint>(value);
                    break;
                case VersionBDataTypeEnum.Int64:
                    this.LongValue = this.ConvertValue<ulong>(value);
                    break;
                case VersionBDataTypeEnum.UInt8:
                    this.IntValue = this.ConvertValue<uint>(value);
                    break;
                case VersionBDataTypeEnum.UInt16:
                    this.IntValue = this.ConvertValue<uint>(value);
                    break;
                case VersionBDataTypeEnum.UInt32:
                    this.IntValue = this.ConvertValue<uint>(value);
                    break;
                case VersionBDataTypeEnum.UInt64:
                    this.LongValue = this.ConvertValue<ulong>(value);
                    break;
                case VersionBDataTypeEnum.Float:
                    this.FloatValue = this.ConvertValue<float>(value);
                    break;
                case VersionBDataTypeEnum.Double:
                    this.DoubleValue = this.ConvertValue<double>(value);
                    break;
                case VersionBDataTypeEnum.Boolean:
                    this.BooleanValue = this.ConvertValue<bool>(value);
                    break;
                case VersionBDataTypeEnum.String:
                case VersionBDataTypeEnum.Text:
                case VersionBDataTypeEnum.Uuid:
                    this.StringValue = this.ConvertValue<string>(value)!;
                    break;
                case VersionBDataTypeEnum.DateTime:
                    this.LongValue = (ulong)new DateTimeOffset(this.ConvertValue<DateTime>(value)).ToUnixTimeMilliseconds();
                    break;
                case VersionBDataTypeEnum.DataSet:
                    this.DataSetValue = this.ConvertValue<DataSet>(value) ?? new();
                    break;
                case VersionBDataTypeEnum.Bytes:
                case VersionBDataTypeEnum.File:
                    this.BytesValue = this.ConvertValue<byte[]>(value) ?? Array.Empty<byte>();
                    break;
                case VersionBDataTypeEnum.Template:
                    this.TemplateValue = this.ConvertValue<Template>(value) ?? new();
                    break;
                case VersionBDataTypeEnum.PropertySet:
                    this.Properties = this.ConvertValue<PropertySet>(value) ?? new();
                    break;
            }
        }
        else
        {
            switch (dataType)
            {
                case VersionBDataTypeEnum.PropertySetList:
                case VersionBDataTypeEnum.Unknown:
                default:
                case VersionBDataTypeEnum.Int8:
                case VersionBDataTypeEnum.Int16:

                case VersionBDataTypeEnum.Int32:

                case VersionBDataTypeEnum.UInt8:
                case VersionBDataTypeEnum.UInt16:
                case VersionBDataTypeEnum.UInt32:
                    this.IntValue = 0;
                    break;
                case VersionBDataTypeEnum.Int64:
                case VersionBDataTypeEnum.UInt64:
                case VersionBDataTypeEnum.DateTime:
                    this.LongValue = 0;
                    break;
                case VersionBDataTypeEnum.Float:
                    this.FloatValue = 0;
                    break;
                case VersionBDataTypeEnum.Double:
                    this.DoubleValue = 0;
                    break;
                case VersionBDataTypeEnum.Boolean:
                    this.BooleanValue = false;
                    break;
                case VersionBDataTypeEnum.String:
                case VersionBDataTypeEnum.Text:
                case VersionBDataTypeEnum.Uuid:
                    this.StringValue = "";
                    break;

                case VersionBDataTypeEnum.DataSet:
                    this.DataSetValue = new();
                    break;
                case VersionBDataTypeEnum.Bytes:
                case VersionBDataTypeEnum.File:
                    this.BytesValue = Array.Empty<byte>();
                    break;
                case VersionBDataTypeEnum.Template:
                    this.TemplateValue = new();
                    break;
                case VersionBDataTypeEnum.PropertySet:
                    this.Properties = new();
                    break;
            }

            this.IsNull = true;
        }

        this.Type = dataType;
        return this;
    }

    /// <summary>
    /// Gets the value.
    /// </summary>
    public override object? Value
    {
        get
        {
            return this.Type switch
            {
                VersionBDataTypeEnum.Int8 => (sbyte)this.IntValue,
                VersionBDataTypeEnum.Int16 => (Int16)this.IntValue,
                VersionBDataTypeEnum.Int32 => (Int32)this.IntValue,
                VersionBDataTypeEnum.Int64 => (Int64)this.LongValue,
                VersionBDataTypeEnum.UInt8 => (byte)this.IntValue,
                VersionBDataTypeEnum.UInt16 => (UInt16)this.IntValue,
                VersionBDataTypeEnum.UInt32 => (UInt32)this.IntValue,
                VersionBDataTypeEnum.UInt64 => (UInt64)this.LongValue,
                VersionBDataTypeEnum.Float => (float)this.FloatValue,
                VersionBDataTypeEnum.Double => (double)this.DoubleValue,
                VersionBDataTypeEnum.Boolean => this.BooleanValue,
                VersionBDataTypeEnum.String => this.StringValue,
                VersionBDataTypeEnum.DateTime => DateTimeOffset.FromUnixTimeMilliseconds((long)this.LongValue).DateTime,
                VersionBDataTypeEnum.Text => this.StringValue,
                VersionBDataTypeEnum.Uuid => Guid.Parse(this.StringValue),
                VersionBDataTypeEnum.DataSet => this.DataSetValue,
                VersionBDataTypeEnum.Bytes => this.BytesValue,
                VersionBDataTypeEnum.File => this.BytesValue,
                VersionBDataTypeEnum.Template => this.TemplateValue,
                VersionBDataTypeEnum.PropertySet => this.Properties,
                _ => null,
            };
        }
    }
}
