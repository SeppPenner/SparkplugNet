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
    public int IntValue { get; protected set; }

    /// <summary>
    /// Gets or sets the long value.
    /// </summary>
    public long LongValue { get; protected set; }

    /// <summary>
    /// Gets or sets the bytes value.
    /// </summary>
    public byte[] BytesValue { get; protected set; } = Array.Empty<byte>();

    /// <summary>
    /// Gets the value.
    /// </summary>
    public override object? Value => this.DataType switch
    {
        DataType.Double => this.DoubleValue,
        DataType.Float => this.FloatValue,
        DataType.Int64 => this.LongValue,
        DataType.Int32 => this.IntValue,
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