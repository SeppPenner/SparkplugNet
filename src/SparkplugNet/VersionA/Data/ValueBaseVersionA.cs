namespace SparkplugNet.VersionA.Data;

/// <summary>
/// Base class containing values using Sparkplug Version A.
/// </summary>
[Obsolete("Sparkplug version A is obsolete since version 3 of the specification, use version B where possible.")]
public abstract class ValueBaseVersionA : ValueBase<VersionADataTypeEnum>
{
    /// <summary>
    /// Gets the value.
    /// </summary>
    public override object? Value => this.DataType switch
    {
        VersionADataTypeEnum.Double => this.Value.ConvertOrDefaultTo<double>(),
        VersionADataTypeEnum.Float => this.Value.ConvertOrDefaultTo<float>(),
        VersionADataTypeEnum.Int64 => this.Value.ConvertOrDefaultTo<long>(),
        VersionADataTypeEnum.Int32 => this.Value.ConvertOrDefaultTo<int>(),
        VersionADataTypeEnum.Boolean => this.Value.ConvertOrDefaultTo<bool>(),
        VersionADataTypeEnum.String => this.Value.ConvertOrDefaultTo<string>(),
        VersionADataTypeEnum.Bytes => this.Value.ConvertOrDefaultTo<byte[]>(),
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
                this.Value = value.ConvertOrDefaultTo<double>();
                break;
            case VersionADataTypeEnum.Float:
                this.Value = value.ConvertOrDefaultTo<float>();
                break;
            case VersionADataTypeEnum.Int64:
                this.Value = value.ConvertOrDefaultTo<long>();
                break;
            case VersionADataTypeEnum.Int32:
                this.Value = value.ConvertOrDefaultTo<int>();
                break;
            case VersionADataTypeEnum.Boolean:
                this.Value = value.ConvertOrDefaultTo<bool>();
                break;
            case VersionADataTypeEnum.String:
                this.Value = value.ConvertOrDefaultTo<string>();
                break;
            case VersionADataTypeEnum.Bytes:
                this.Value = value.ConvertOrDefaultTo<byte[]>();
                break;
            default:
                throw new NotImplementedException($"Type {dataType} is not supported yet");
        }

        return this;
    }
}