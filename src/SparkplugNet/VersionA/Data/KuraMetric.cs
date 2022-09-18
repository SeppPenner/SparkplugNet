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
public class KuraMetric : MetricBase<DataType>
{
    /// <summary>
    /// The double value.
    /// </summary>
    private double? doubleValue;

    /// <summary>
    /// The float value.
    /// </summary>
    private float? floatValue;

    /// <summary>
    /// The long value.
    /// </summary>
    private long? longValue;

    /// <summary>
    /// The integer value.
    /// </summary>
    private int? intValue;

    /// <summary>
    /// The boolean value.
    /// </summary>
    private bool? boolValue;

    /// <summary>
    /// The string value.
    /// </summary>
    private string? stringValue;

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

    /// <summary>
    /// Gets or sets the double value.
    /// </summary>
    public double? DoubleValue
    {
        get => this.doubleValue.GetValueOrDefault();
        set => this.doubleValue = value;
    }

    /// <summary>
    /// Gets or sets the float value.
    /// </summary>
    public float? FloatValue
    {
        get => this.floatValue.GetValueOrDefault();
        set => this.floatValue = value;
    }

    /// <summary>
    /// Gets or sets the long value.
    /// </summary>
    public long? LongValue
    {
        get => this.longValue.GetValueOrDefault();
        set => this.longValue = value;
    }

    /// <summary>
    /// Gets or sets the integer value.
    /// </summary>
    public int? IntValue
    {
        get => this.intValue.GetValueOrDefault();
        set => this.intValue = value;
    }

    /// <summary>
    /// Gets or sets the boolean value.
    /// </summary>
    public bool? BoolValue
    {
        get => this.boolValue.GetValueOrDefault();
        set => this.boolValue = value;
    }

    /// <summary>
    /// Gets or sets the string value.
    /// </summary>
    [DefaultValue("")]
    public string? StringValue
    {
        get => this.stringValue ?? string.Empty;
        set => this.stringValue = value;
    }

    /// <summary>
    /// Gets or sets the bytes value.
    /// </summary>
    public byte[]? BytesValue { get; set; }

    /// <summary>
    /// Sets the value.
    /// </summary>
    /// <param name="dataType">Type of the data.</param>
    /// <param name="value">The value.</param>
    public override MetricBase<DataType> SetValue(DataType dataType, object value)
    {
        switch (dataType)
        {
            case DataType.Double:
                this.DoubleValue = this.ConvertValue<double>(value);
                break;
            case DataType.Float:
                this.FloatValue = this.ConvertValue<float>(value);
                break;
            case DataType.Int64:
                this.LongValue = this.ConvertValue<long>(value);
                break;
            case DataType.Int32:
                this.IntValue = this.ConvertValue<int>(value);
                break;
            case DataType.Bool:
                this.BoolValue = this.ConvertValue<bool>(value);
                break;
            case DataType.String:
                this.StringValue = this.ConvertValue<string>(value);
                break;
            case DataType.Bytes:
                this.BytesValue = this.ConvertValue<byte[]>(value);
                break;
            default:
                throw new NotImplementedException($"Type {dataType} is not supported yet");
        }

        this.Type = dataType;

        return this;
    }

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <exception cref="NotImplementedException">Type {this.Type} is not supported yet</exception>
    public override object? Value
    {
        get
        {
            return this.Type switch
            {
                DataType.Double => this.DoubleValue,
                DataType.Float => this.FloatValue,
                DataType.Int64 => this.LongValue,
                DataType.Int32 => this.IntValue,
                DataType.Bool => this.BoolValue,
                DataType.String => this.StringValue,
                DataType.Bytes => this.BytesValue,
                _ => throw new NotImplementedException($"Type {this.Type} is not supported yet"),
            };
        }
    }
}