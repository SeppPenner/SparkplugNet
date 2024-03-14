// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueBaseVersionB.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A base value class for Sparkplug Version B.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.VersionB.Data;

/// <summary>
/// A base value class for Sparkplug Version B.
/// </summary>
public abstract class ValueBaseVersionB : ValueBase<VersionBDataTypeEnum>
{
    /// <summary>
    /// Gets the value.
    /// </summary>
    public override object? Value => this.DataType switch
    {
        VersionBDataTypeEnum.Int8 => this.ObjectValue.ConvertOrDefaultTo<sbyte>(),
        VersionBDataTypeEnum.Int16 => this.ObjectValue.ConvertOrDefaultTo<short>(),
        VersionBDataTypeEnum.Int32 => this.ObjectValue.ConvertOrDefaultTo<int>(),
        VersionBDataTypeEnum.Int64 => this.ObjectValue.ConvertOrDefaultTo<long>(),
        VersionBDataTypeEnum.UInt8 => this.ObjectValue.ConvertOrDefaultTo<byte>(),
        VersionBDataTypeEnum.UInt16 => this.ObjectValue.ConvertOrDefaultTo<ushort>(),
        VersionBDataTypeEnum.UInt32 => this.ObjectValue.ConvertOrDefaultTo<uint>(),
        VersionBDataTypeEnum.UInt64 => this.ObjectValue.ConvertOrDefaultTo<ulong>(),
        VersionBDataTypeEnum.Float => this.ObjectValue.ConvertOrDefaultTo<float>(),
        VersionBDataTypeEnum.Double => this.ObjectValue.ConvertOrDefaultTo<double>(),
        VersionBDataTypeEnum.Boolean => this.ObjectValue.ConvertOrDefaultTo<bool>(),
        VersionBDataTypeEnum.String => this.ObjectValue.ConvertOrDefaultTo<string>(),
        VersionBDataTypeEnum.DateTime => MetricTimeValue.GetDateTimeOffset(this.ObjectValue.ConvertOrDefaultTo<ulong>()),
        VersionBDataTypeEnum.Text => this.ObjectValue.ConvertOrDefaultTo<string>(),
        VersionBDataTypeEnum.Uuid => Guid.Parse(this.ObjectValue.ConvertOrDefaultTo<string>()),
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
        this.DataType = dataType;

        switch (dataType)
        {
            case VersionBDataTypeEnum.Int8:
                this.ObjectValue = value.ConvertOrDefaultTo<sbyte>();
                break;
            case VersionBDataTypeEnum.Int16:
                this.ObjectValue = value.ConvertOrDefaultTo<short>();
                break;
            case VersionBDataTypeEnum.Int32:
                this.ObjectValue = value.ConvertOrDefaultTo<int>();
                break;
            case VersionBDataTypeEnum.Int64:
                this.ObjectValue = value.ConvertOrDefaultTo<long>();
                break;
            case VersionBDataTypeEnum.UInt8:
                this.ObjectValue = value.ConvertOrDefaultTo<byte>();
                break;
            case VersionBDataTypeEnum.UInt16:
                this.ObjectValue = value.ConvertOrDefaultTo<ushort>();
                break;
            case VersionBDataTypeEnum.UInt32:
                this.ObjectValue = value.ConvertOrDefaultTo<uint>();
                break;
            case VersionBDataTypeEnum.UInt64:
                this.ObjectValue = value.ConvertOrDefaultTo<ulong>();
                break;
            case VersionBDataTypeEnum.DateTime:
                if (value is null)
                {
                    return this;
                }

                this.ObjectValue = MetricTimeValue.GetMilliSeconds(value).ConvertTo<ulong>();
                break;
            case VersionBDataTypeEnum.Float:
                this.ObjectValue = value.ConvertTo<float>();
                break;
            case VersionBDataTypeEnum.Double:
                this.ObjectValue = value.ConvertTo<double>();
                break;
            case VersionBDataTypeEnum.Boolean:
                this.ObjectValue = value.ConvertTo<bool>();
                break;
            case VersionBDataTypeEnum.String:
                this.ObjectValue = value.ConvertOrDefaultTo<string>();
                break;
            case VersionBDataTypeEnum.Text:
                this.ObjectValue = value.ConvertOrDefaultTo<string>();
                break;
            case VersionBDataTypeEnum.Uuid:
                if (value is null)
                {
                    return this;
                }

                if (value is string stringValue)
                {
                    this.ObjectValue = stringValue;
                    break;
                }
                else if (value is Guid guidValue)
                {
                    this.ObjectValue = guidValue.ToString();
                    break;
                }
                else
                {
                    throw new InvalidOperationException($"Value {value} is not a valid Guid value");
                }
            case VersionBDataTypeEnum.Unknown:
            default:
                throw new NotImplementedException($"Type {dataType} is not supported yet");
        }
        return this;
    }
}