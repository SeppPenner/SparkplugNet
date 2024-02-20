namespace SparkplugNet.VersionA.Data;

/// <summary>
/// Base class containing values using Sparkplug Version A.
/// </summary>
[Obsolete("Sparkplug version A is obsolete since version 3 of the specification, use version B where possible.")]
public abstract class ValueBaseVersionA : ValueBase<VersionADataTypeEnum>
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
        VersionADataTypeEnum.Double => this.DoubleValue,
        VersionADataTypeEnum.Float => this.FloatValue,
        VersionADataTypeEnum.Int64 => this.LongValue,
        VersionADataTypeEnum.Int32 => this.IntValue,
        VersionADataTypeEnum.Boolean => this.BooleanValue,
        VersionADataTypeEnum.String => this.StringValue,
        VersionADataTypeEnum.Bytes => this.BytesValue,
        _ => null
    };

    /// <summary>
    /// Sets the value.
    /// </summary>
    /// <param name="dataType">Type of the data.</param>
    /// <param name="value">The value.</param>
    public override IValue<VersionADataTypeEnum> SetValue(VersionADataTypeEnum dataType, object? value)
    {
        switch (dataType)
        {
            case VersionADataTypeEnum.Double:
                this.DoubleValue = value.ConvertTo<double>();
                break;
            case VersionADataTypeEnum.Float:
                this.FloatValue = value.ConvertTo<float>();
                break;
            case VersionADataTypeEnum.Int64:
                this.LongValue = value.ConvertTo<long>();
                break;
            case VersionADataTypeEnum.Int32:
                this.IntValue = value.ConvertTo<int>();
                break;
            case VersionADataTypeEnum.Boolean:
                this.BooleanValue = value.ConvertTo<bool>();
                break;
            case VersionADataTypeEnum.String:
                this.StringValue = value.ConvertOrDefaultTo<string>();
                break;
            case VersionADataTypeEnum.Bytes:
                this.BytesValue = value.ConvertOrDefaultTo<byte[]>();
                break;
            default:
                throw new NotImplementedException($"Type {dataType} is not supported yet");
        }

        return this;
    }
}