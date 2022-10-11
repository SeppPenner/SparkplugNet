namespace SparkplugNet.VersionB.Data;

/// <summary>
/// Base Value class for Sparkplug Version B
/// </summary>
public abstract class ValueBaseVersionB : ValueBase<VersionBDataTypeEnum>
{
    /// <summary>
    /// The integer value.
    /// </summary>
    private uint? intValue;

    /// <summary>
    /// The long value.
    /// </summary>
    private ulong? longValue;

    /// <summary>
    /// Gets or sets the integer value.
    /// </summary>
    public virtual uint IntValue
    {
        get => this.intValue ?? default;
        set
        {
            this.intValue = value;
            this.DataType = VersionBDataTypeEnum.Int32;
        }
    }

    /// <summary>
    /// Gets or sets the long value.
    /// </summary>
    public virtual ulong LongValue
    {
        get => this.longValue ?? default;
        set
        {
            this.longValue = value;
            this.DataType = VersionBDataTypeEnum.Int64;
        }
    }

    /// <summary>
    /// Gets or sets the float value.
    /// </summary>
    public override float FloatValue
    {
        set
        {
            base.FloatValue = value;
            this.DataType = VersionBDataTypeEnum.Float;
        }
    }

    /// <summary>
    /// Gets or sets the double value.
    /// </summary>
    public override double DoubleValue
    {
        set
        {
            base.DoubleValue = value;
            this.DataType = VersionBDataTypeEnum.Double;
        }
    }

    /// <summary>
    /// Gets or sets the boolean value.
    /// </summary>
    public override bool BooleanValue
    {
        set
        {
            base.BooleanValue = value;
            this.DataType = VersionBDataTypeEnum.Boolean;
        }
    }

    /// <summary>
    /// Gets or sets the string value.
    /// </summary>
    [DefaultValue("")]
    public override string StringValue
    {
        set
        {
            base.StringValue = value;
            this.DataType = VersionBDataTypeEnum.String;
        }
    }

    /// <summary>
    /// Gets or sets the ValueCase.
    /// </summary>
    public override uint ValueCase
    {
        get => (uint)this.DataType;
        set => this.DataType = (VersionBDataTypeEnum)value;
    }

    /// <summary>
    /// Gets or sets the DataType.
    /// </summary>
    public override VersionBDataTypeEnum DataType { get; set; } = default!;

    /// <summary>
    /// Gets the value.
    /// </summary>
    public override object? Value => this.DataType switch
    {
        VersionBDataTypeEnum.Int8 => (sbyte)this.IntValue,
        VersionBDataTypeEnum.Int16 => (short)this.IntValue,
        VersionBDataTypeEnum.Int32 => (int)this.IntValue,
        VersionBDataTypeEnum.Int64 => (long)this.LongValue,
        VersionBDataTypeEnum.UInt8 => (byte)this.IntValue,
        VersionBDataTypeEnum.UInt16 => (ushort)this.IntValue,
        VersionBDataTypeEnum.UInt32 => this.IntValue,
        VersionBDataTypeEnum.UInt64 => this.LongValue,
        VersionBDataTypeEnum.Float => this.FloatValue,
        VersionBDataTypeEnum.Double => this.DoubleValue,
        VersionBDataTypeEnum.Boolean => this.BooleanValue,
        VersionBDataTypeEnum.String => this.StringValue,
        VersionBDataTypeEnum.DateTime => DateTimeOffset.FromUnixTimeMilliseconds((long)this.LongValue).DateTime,
        VersionBDataTypeEnum.Text => this.StringValue,
        VersionBDataTypeEnum.Uuid => Guid.Parse(this.StringValue),
        _ => null,
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
            case VersionBDataTypeEnum.PropertySetList:
            case VersionBDataTypeEnum.Unknown:
            case VersionBDataTypeEnum.Int8:
            case VersionBDataTypeEnum.Int16:
            case VersionBDataTypeEnum.Int32:
            case VersionBDataTypeEnum.UInt8:
            case VersionBDataTypeEnum.UInt16:
            case VersionBDataTypeEnum.UInt32:
                this.IntValue = value.ConvertTo<uint>();
                break;
            case VersionBDataTypeEnum.Int64:
            case VersionBDataTypeEnum.UInt64:
                this.LongValue = value.ConvertTo<ulong>();
                break;
            case VersionBDataTypeEnum.Float:
                this.FloatValue = value.ConvertTo<float>();
                break;
            case VersionBDataTypeEnum.Double:
                this.DoubleValue = value.ConvertTo<double>();
                break;
            case VersionBDataTypeEnum.Boolean:
                this.BooleanValue = value.ConvertTo<bool>();
                break;
            case VersionBDataTypeEnum.String:
            case VersionBDataTypeEnum.Text:
            case VersionBDataTypeEnum.Uuid:
                this.StringValue = value.ConvertOrDefaultTo<string>();
                break;
            case VersionBDataTypeEnum.DateTime:
                this.LongValue = (ulong)new DateTimeOffset(value.ConvertTo<DateTime>()).ToUnixTimeMilliseconds();
                break;
            default:
                throw new NotImplementedException($"Type {dataType} is not supported yet");
        }

        return this;
    }
}