namespace SparkplugNet.VersionA.Data;

/// <summary>
/// Base class containing values using Sparkplug Version A.
/// </summary>
[Obsolete("Sparkplug version A is obsolete since version 3 of the specification, use version B where possible.")]
public abstract class ValueBaseVersionA : ValueBase<DataType>
{
    /// <summary>
    /// Gets or sets the integer value.
    /// </summary>
    public virtual int IntValue { get; set; }

    /// <summary>
    /// Gets or sets the long value.
    /// </summary>
    public virtual long LongValue { get; set; }

    /// <summary>
    /// Gets or sets the float value.
    /// </summary>
    public override float FloatValue { get; set; }

    /// <summary>
    /// Gets or sets the double value.
    /// </summary>
    public override double DoubleValue { get; set; }

    /// <summary>
    /// Gets or sets the boolean value.
    /// </summary>
    public override bool BooleanValue { get; set; }

    /// <summary>
    /// Gets or sets the string value.
    /// </summary>
    [DefaultValue("")]
    public override string StringValue { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the bytes value.
    /// </summary>
    public virtual byte[] BytesValue { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// Gets or sets the Value Case.
    /// </summary>
    public override uint ValueCase
    {
        get => (uint)this.DataType;
        set => this.DataType = (DataType)value;
    }

    /// <summary>
    /// Gets or sets the DataType.
    /// </summary>
    public override DataType DataType { get; set; } = default;

    /// <summary>
    /// Gets the value.
    /// </summary>
    public override object? Value => this.DataType switch
    {
        DataType.Int32 => this.IntValue,
        DataType.Int64 => this.LongValue,
        DataType.Float => this.FloatValue,
        DataType.Double => this.DoubleValue,
        DataType.Boolean => this.BooleanValue,
        DataType.String => this.StringValue,
        DataType.Bytes => this.BytesValue,
        _ => null
    };

    /// <summary>
    /// Sets the value.
    /// </summary>
    /// <param name="dataType">Type of the data.</param>
    /// <param name="value">The value.</param>
    public override IValue<DataType> SetValue(DataType dataType, object? value)
    {
        switch (dataType)
        {
            case DataType.Double:
                this.DoubleValue = value.ConvertTo<double>();
                break;
            case DataType.Float:
                this.FloatValue = value.ConvertTo<float>();
                break;
            case DataType.Int64:
                this.LongValue = value.ConvertTo<long>();
                break;
            case DataType.Int32:
                this.IntValue = value.ConvertTo<int>();
                break;
            case DataType.Boolean:
                this.BooleanValue = value.ConvertTo<bool>();
                break;
            case DataType.String:
                this.StringValue = value.ConvertOrDefaultTo<string>();
                break;
            case DataType.Bytes:
                this.BytesValue = value.ConvertOrDefaultTo<byte[]>();
                break;
            default:
                throw new NotImplementedException($"Type {dataType} is not supported yet");
        }

        return this;
    }
}