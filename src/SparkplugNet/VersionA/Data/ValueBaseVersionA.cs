// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueBaseVersionA.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A base class containing values using Sparkplug Version A.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.VersionA.Data;

/// <summary>
/// A base class containing values using Sparkplug Version A.
/// </summary>
[Obsolete("Sparkplug version A is obsolete since version 3 of the specification, use version B where possible.")]
public abstract class ValueBaseVersionA : ValueBase<VersionADataTypeEnum>
{
    /// <summary>
    /// Gets the value.
    /// </summary>
    public override object? Value => this.DataType switch
    {
        VersionADataTypeEnum.Double => this.ObjectValue.ConvertOrDefaultTo<double>(),
        VersionADataTypeEnum.Float => this.ObjectValue.ConvertOrDefaultTo<float>(),
        VersionADataTypeEnum.Int64 => this.ObjectValue.ConvertOrDefaultTo<long>(),
        VersionADataTypeEnum.Int32 => this.ObjectValue.ConvertOrDefaultTo<int>(),
        VersionADataTypeEnum.Boolean => this.ObjectValue.ConvertOrDefaultTo<bool>(),
        VersionADataTypeEnum.String => this.ObjectValue.ConvertOrDefaultTo<string>(),
        VersionADataTypeEnum.Bytes => this.ObjectValue.ConvertOrDefaultTo<byte[]>(),
        _ => null
    };

    /// <summary>
    /// Sets the value.
    /// </summary>
    /// <param name="dataType">Type of the data.</param>
    /// <param name="value">The value.</param>
    public override IValue<VersionADataTypeEnum> SetValue(VersionADataTypeEnum dataType, object? value)
    {
        this.DataType = dataType;

        switch (dataType)
        {
            case VersionADataTypeEnum.Double:
                this.ObjectValue = value.ConvertOrDefaultTo<double>();
                break;
            case VersionADataTypeEnum.Float:
                this.ObjectValue = value.ConvertOrDefaultTo<float>();
                break;
            case VersionADataTypeEnum.Int64:
                this.ObjectValue = value.ConvertOrDefaultTo<long>();
                break;
            case VersionADataTypeEnum.Int32:
                this.ObjectValue = value.ConvertOrDefaultTo<int>();
                break;
            case VersionADataTypeEnum.Boolean:
                this.ObjectValue = value.ConvertOrDefaultTo<bool>();
                break;
            case VersionADataTypeEnum.String:
                this.ObjectValue = value.ConvertOrDefaultTo<string>();
                break;
            case VersionADataTypeEnum.Bytes:
                this.ObjectValue = value.ConvertOrDefaultTo<byte[]>();
                break;
            default:
                throw new NotImplementedException($"Type {dataType} is not supported yet");
        }

        return this;
    }
}