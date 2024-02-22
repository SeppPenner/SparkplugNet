namespace SparkplugNet.VersionB.Data;

/// <summary>
/// Base Value class for Sparkplug Version B.
/// </summary>
public abstract class ValueBaseVersionB : ValueBase<VersionBDataTypeEnum>
{
    /// <summary>
    /// Gets the value.
    /// </summary>
    public override object? Value => this.DataType switch
    {
        VersionBDataTypeEnum.Int8 => this.Value.ConvertOrDefaultTo<sbyte>(),
        VersionBDataTypeEnum.Int16 => this.Value.ConvertOrDefaultTo<short>(),
        VersionBDataTypeEnum.Int32 => this.Value.ConvertOrDefaultTo<int>(),
        VersionBDataTypeEnum.Int64 => this.Value.ConvertOrDefaultTo<long>(),
        VersionBDataTypeEnum.UInt8 => this.Value.ConvertOrDefaultTo<byte>(),
        VersionBDataTypeEnum.UInt16 => this.Value.ConvertOrDefaultTo<ushort>(),
        VersionBDataTypeEnum.UInt32 => this.Value.ConvertOrDefaultTo<uint>(),
        VersionBDataTypeEnum.UInt64 => this.Value.ConvertOrDefaultTo<ulong>(),
        VersionBDataTypeEnum.Float => this.Value.ConvertOrDefaultTo<float>(),
        VersionBDataTypeEnum.Double => this.Value.ConvertOrDefaultTo<double>(),
        VersionBDataTypeEnum.Boolean => this.Value.ConvertOrDefaultTo<bool>(),
        VersionBDataTypeEnum.String => this.Value.ConvertOrDefaultTo<string>(),
        VersionBDataTypeEnum.DateTime => DateTimeOffset.FromUnixTimeMilliseconds(this.Value.ConvertOrDefaultTo<long>()).DateTime,
        VersionBDataTypeEnum.Text => this.Value.ConvertOrDefaultTo<string>(),
        VersionBDataTypeEnum.Uuid => Guid.Parse(this.Value.ConvertOrDefaultTo<string>()),
        _ => null
    };

    /// <summary>
    /// Sets the value.
    /// </summary>
    /// <param name="dataType">Type of the data.</param>
    /// <param name="value">The value.</param>
    /// <returns>The metric.</returns>
    public override IValue<VersionBDataTypeEnum> SetValue(VersionBDataTypeEnum dataType, object? value)
    {
        switch (dataType)
        {
            case VersionBDataTypeEnum.Int8:
                this.Value = value.ConvertOrDefaultTo<sbyte>();
                break;
            case VersionBDataTypeEnum.Int16:
                this.Value = value.ConvertOrDefaultTo<short>();
                break;
            case VersionBDataTypeEnum.Int32:
                this.Value = value.ConvertOrDefaultTo<int>();
                break;
            case VersionBDataTypeEnum.Int64:
                this.Value = value.ConvertOrDefaultTo<long>();
                break;
            case VersionBDataTypeEnum.UInt8:
                this.Value = value.ConvertOrDefaultTo<byte>();
                break;
            case VersionBDataTypeEnum.UInt16:
                this.Value = value.ConvertOrDefaultTo<ushort>();
                break;
            case VersionBDataTypeEnum.UInt32:
                this.Value = value.ConvertOrDefaultTo<uint>();
                break;
            case VersionBDataTypeEnum.UInt64:
                this.Value = value.ConvertOrDefaultTo<ulong>();
                break;
            case VersionBDataTypeEnum.DateTime:
                this.Value = (ulong)new DateTimeOffset(value.ConvertOrDefaultTo<DateTime>()).ToUnixTimeMilliseconds();
                break;
            case VersionBDataTypeEnum.Float:
                this.Value = value.ConvertTo<float>();
                break;
            case VersionBDataTypeEnum.Double:
                this.Value = value.ConvertTo<double>();
                break;
            case VersionBDataTypeEnum.Boolean:
                this.Value = value.ConvertTo<bool>();
                break;
            case VersionBDataTypeEnum.String:
                this.Value = value.ConvertOrDefaultTo<string>();
                break;
            case VersionBDataTypeEnum.Text:
                this.Value = value.ConvertOrDefaultTo<string>();
                break;
            case VersionBDataTypeEnum.Uuid:
                this.Value = value.ConvertOrDefaultTo<string>();
                break;
            case VersionBDataTypeEnum.Unknown:
            default:
                throw new NotImplementedException($"Type {dataType} is not supported yet");
        }

        return this;
    }
}