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
public class Metric
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
    /// Gets or sets the name.
    /// </summary>
    [DefaultValue("")]
    public string Name { get; set; } = string.Empty;

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
    public uint DataType { get; set; }

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
    public MetaData Metadata { get; set; } = new ();

    /// <summary>
    /// Gets or sets the properties.
    /// </summary>
    public PropertySet Properties { get; set; } = new ();

    /// <summary>
    /// Gets or sets the integer value.
    /// </summary>
    public uint IntValue
    {
        get => this.integerValue ?? default;
        set
        {
            this.integerValue = value;
            this.ValueCase = VersionBDataTypeEnum.Int32;
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
            this.ValueCase = VersionBDataTypeEnum.Int64;
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
            this.ValueCase = VersionBDataTypeEnum.Float;
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
            this.ValueCase = VersionBDataTypeEnum.Double;
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
            this.ValueCase = VersionBDataTypeEnum.Boolean;
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
            this.ValueCase = VersionBDataTypeEnum.String;
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
            this.ValueCase = VersionBDataTypeEnum.Bytes;
        }
    }

    /// <summary>
    /// Gets or sets the data set value.
    /// </summary>
    public DataSet DataSetValue
    {
        get => this.dataSetValue ?? new ();
        set
        {
            this.dataSetValue = value;
            this.ValueCase = VersionBDataTypeEnum.DataSet;
        }
    }

    /// <summary>
    /// Gets or sets the template value.
    /// </summary>
    public Template TemplateValue
    {
        get => this.templateValue ?? new ();
        set
        {
            this.templateValue = value;
            this.ValueCase = VersionBDataTypeEnum.Template;
        }
    }

    /// <summary>
    /// Gets or sets the extension value.
    /// </summary>
    public MetricValueExtension ExtensionValue
    {
        get => this.extensionValue ?? new ();
        set
        {
            this.extensionValue = value;
            this.ValueCase = VersionBDataTypeEnum.Unknown;
        }
    }

    /// <summary>
    /// Gets or sets the value case.
    /// </summary>
    public DataType ValueCase { get; set; }
}
