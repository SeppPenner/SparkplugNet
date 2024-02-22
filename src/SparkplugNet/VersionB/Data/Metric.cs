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
public class Metric : ValueBaseVersionB, IMetric
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Metric"/> class.
    /// </summary>
    public Metric()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Metric"/> class.
    /// </summary>
    /// <param name="dataType">The data type.</param>
    /// <param name="value">The value.</param>
    /// <param name="timestamp">The timestamp.</param>
    public Metric(VersionBDataTypeEnum dataType, object? value, DateTimeOffset? timestamp = null)
    {
        this.SetValue(dataType, value);

        if (timestamp is not null)
        {
            this.Timestamp = (ulong)timestamp.Value.ToUnixTimeMilliseconds();
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Metric"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="dataType">The data type.</param>
    /// <param name="value">The value.</param>
    /// <param name="timestamp">The timestamp.</param>
    public Metric(string name, VersionBDataTypeEnum dataType, object? value, DateTimeOffset? timestamp = null)
    {
        this.Name = name;
        this.SetValue(dataType, value);

        if (timestamp is not null)
        {
            this.Timestamp = (ulong)timestamp.Value.ToUnixTimeMilliseconds();
        }
    }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    [DefaultValue("")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the alias.
    /// </summary>
    public ulong? Alias { get; set; }

    /// <summary>
    /// Gets or sets the timestamp.
    /// </summary>
    public ulong? Timestamp { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the metric is historical or not.
    /// </summary>
    public bool? IsHistorical { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the metric is transient or not.
    /// </summary>
    public bool? IsTransient { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the metric is null or not.
    /// </summary>
    public bool IsNull { get; protected set; }

    /// <summary>
    /// Gets or sets the meta data.
    /// </summary>
    public MetaData? MetaData { get; set; }

    /// <summary>
    /// Gets the value.
    /// </summary>
    public override object? Value => base.Value ?? this.GetValue();

    /// <summary>
    /// Sets the value.
    /// </summary>
    /// <param name="dataType">Type of the data.</param>
    /// <param name="value">The value.</param>
    /// <returns>The metric.</returns>
    public override IValue<VersionBDataTypeEnum> SetValue(VersionBDataTypeEnum dataType, object? value)
    {
        this.IsNull = value is null;

        switch (dataType)
        {
            case VersionBDataTypeEnum.Int8:
                base.SetValue(VersionBDataTypeEnum.Int8, this.Value);
                break;
            case VersionBDataTypeEnum.Int16:
                base.SetValue(VersionBDataTypeEnum.Int16, this.Value);
                break;
            case VersionBDataTypeEnum.Int32:
                base.SetValue(VersionBDataTypeEnum.Int32, this.Value);
                break;
            case VersionBDataTypeEnum.Int64:
                base.SetValue(VersionBDataTypeEnum.Int64, this.Value);
                break;
            case VersionBDataTypeEnum.UInt8:
                base.SetValue(VersionBDataTypeEnum.UInt8, this.Value);
                break;
            case VersionBDataTypeEnum.UInt16:
                base.SetValue(VersionBDataTypeEnum.UInt16, this.Value);
                break;
            case VersionBDataTypeEnum.UInt32:
                base.SetValue(VersionBDataTypeEnum.UInt32, this.Value);
                break;
            case VersionBDataTypeEnum.UInt64:
                base.SetValue(VersionBDataTypeEnum.UInt64, this.Value);
                break;
            case VersionBDataTypeEnum.DateTime:
                base.SetValue(VersionBDataTypeEnum.DateTime, this.Value);
                break;
            case VersionBDataTypeEnum.Float:
                base.SetValue(VersionBDataTypeEnum.Float, this.Value);
                break;
            case VersionBDataTypeEnum.Double:
                base.SetValue(VersionBDataTypeEnum.Double, this.Value);
                break;
            case VersionBDataTypeEnum.Boolean:
                base.SetValue(VersionBDataTypeEnum.Boolean, this.Value);
                break;
            case VersionBDataTypeEnum.String:
                base.SetValue(VersionBDataTypeEnum.String, this.Value);
                break;
            case VersionBDataTypeEnum.Text:
                base.SetValue(VersionBDataTypeEnum.Text, this.Value);
                break;
            case VersionBDataTypeEnum.Uuid:
                base.SetValue(VersionBDataTypeEnum.Uuid, this.Value);
                break;
            case VersionBDataTypeEnum.Bytes:
            case VersionBDataTypeEnum.File:
                this.Value = value.ConvertOrDefaultTo<byte[]>();
                break;
            case VersionBDataTypeEnum.DataSet:
                this.Value = value.ConvertOrDefaultTo<DataSet>();
                break;
            case VersionBDataTypeEnum.Template:
                this.Value = value.ConvertOrDefaultTo<Template>();
                break;
            case VersionBDataTypeEnum.PropertySet:
                this.Value = value.ConvertOrDefaultTo<PropertySet>();
                break;
            case VersionBDataTypeEnum.Int8Array:
                {
                    var originalData = value?.ConvertTo<sbyte[]>()?.ToList() ?? new();
                    var data = new List<byte>();

                    for (var index = 0; index < originalData.Count; index++)
                    {
                        var number = originalData[index];
                        data.Add((byte)number);
                    }

                    this.Value = data.ToArray();
                }

                break;
            case VersionBDataTypeEnum.Int16Array:
                {
                    var originalData = value?.ConvertTo<short[]>()?.ToList() ?? new();
                    var data = new List<byte>();

                    foreach (var number in originalData)
                    {
                        var bytes = BitConverter.GetBytes(number);
                        data.AddRange(bytes);
                    }

                    this.Value = data.ToArray();
                }

                break;
            case VersionBDataTypeEnum.Int32Array:
                {
                    var originalData = value?.ConvertTo<int[]>()?.ToList() ?? new();
                    var data = new List<byte>();

                    foreach (var number in originalData)
                    {
                        var bytes = BitConverter.GetBytes(number);
                        data.AddRange(bytes);
                    }

                    this.Value = data.ToArray();
                }

                break;
            case VersionBDataTypeEnum.Int64Array:
                {
                    var originalData = value?.ConvertTo<long[]>()?.ToList() ?? new();
                    var data = new List<byte>();

                    foreach (var number in originalData)
                    {
                        var bytes = BitConverter.GetBytes(number);
                        data.AddRange(bytes);
                    }

                    this.Value = data.ToArray();
                }

                break;
            case VersionBDataTypeEnum.UInt8Array:
                {
                    this.Value = value?.ConvertTo<byte[]>() ?? Array.Empty<byte>();
                }

                break;
            case VersionBDataTypeEnum.UInt16Array:
                {
                    var originalData = value?.ConvertTo<ushort[]>()?.ToList() ?? new();
                    var data = new List<byte>();

                    foreach (var number in originalData)
                    {
                        var bytes = BitConverter.GetBytes(number);
                        data.AddRange(bytes);
                    }

                    this.Value = data.ToArray();
                }

                break;
            case VersionBDataTypeEnum.UInt32Array:
                {
                    var originalData = value?.ConvertTo<uint[]>()?.ToList() ?? new();
                    var data = new List<byte>();

                    foreach (var number in originalData)
                    {
                        var bytes = BitConverter.GetBytes(number);
                        data.AddRange(bytes);
                    }

                    this.Value = data.ToArray();
                }

                break;
            case VersionBDataTypeEnum.UInt64Array:
                {
                    var originalData = value?.ConvertTo<ulong[]>()?.ToList() ?? new();
                    var data = new List<byte>();

                    foreach (var number in originalData)
                    {
                        var bytes = BitConverter.GetBytes(number);
                        data.AddRange(bytes);
                    }

                    this.Value = data.ToArray();
                }

                break;
            case VersionBDataTypeEnum.DateTimeArray:
                {
                    var originalData = value?.ConvertTo<DateTimeOffset[]>()?.ToList() ?? new();
                    var data = new List<byte>();

                    foreach (var number in originalData)
                    {
                        var bytes = BitConverter.GetBytes(number.ToUnixTimeMilliseconds());
                        data.AddRange(bytes);
                    }

                    this.Value = data.ToArray();
                }

                break;
            case VersionBDataTypeEnum.FloatArray:
                {
                    var originalData = value?.ConvertTo<float[]>()?.ToList() ?? new();
                    var data = new List<byte>();

                    foreach (var number in originalData)
                    {
                        var bytes = BitConverter.GetBytes(number);
                        data.AddRange(bytes);
                    }

                    this.Value = data.ToArray();
                }

                break;
            case VersionBDataTypeEnum.DoubleArray:
                {
                    var originalData = value?.ConvertTo<double[]>()?.ToList() ?? new();
                    var data = new List<byte>();

                    foreach (var number in originalData)
                    {
                        var bytes = BitConverter.GetBytes(number);
                        data.AddRange(bytes);
                    }

                    this.Value = data.ToArray();
                }

                break;
            case VersionBDataTypeEnum.BooleanArray:
                {
                    var originalData = value?.ConvertTo<bool[]>()?.ToList() ?? new();
                    var data = new List<byte>();

                    foreach (var number in originalData)
                    {
                        var bytes = BitConverter.GetBytes(number);
                        data.AddRange(bytes);
                    }

                    this.Value = data.ToArray();
                }

                break;
            // Todo: What to do here?
            case VersionBDataTypeEnum.StringArray:
                //{
                //    var originalData = value?.ConvertTo<string[]>()?.ToList() ?? new();
                //    var data = new List<byte>();

                //    foreach (var number in originalData)
                //    {
                //        var bytes = Encoding.UTF8.GetBytes(number);
                //        data.AddRange(bytes);
                //    }

                //    this.Value = data.ToArray();
                //}

                break;
            // Todo: What to do here?
            case VersionBDataTypeEnum.PropertySetList:
                break;
            case VersionBDataTypeEnum.Unknown:
            default:
                break;
        }

        return this;
    }

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <returns>The value as <see cref="object"/>.</returns>
    private object? GetValue()
    {
        if (this.IsNull)
        {
            return null;
        }

        switch (this.DataType)
        {
            case VersionBDataTypeEnum.Bytes:
            case VersionBDataTypeEnum.File:
                return this.Value.ConvertOrDefaultTo<byte[]>();
            case VersionBDataTypeEnum.DataSet:
                return this.Value.ConvertOrDefaultTo<DataSet>();
            case VersionBDataTypeEnum.Template:
                return this.Value.ConvertOrDefaultTo<Template>();
            case VersionBDataTypeEnum.PropertySet:
                return this.Value.ConvertOrDefaultTo<PropertySet>();
            case VersionBDataTypeEnum.Int8Array:
                return this.Value.ConvertOrDefaultTo<byte[]>().Select(b => (sbyte)b).ToArray();
            case VersionBDataTypeEnum.Int16Array:
                {
                    var size = this.Value.ConvertOrDefaultTo<byte[]>().Count() / sizeof(short);
                    var data = new short[size];

                    for (var index = 0; index < size; index++)
                    {
                        data[index] = BitConverter.ToInt16(this.Value.ConvertOrDefaultTo<byte[]>(), index * sizeof(short));
                    }

                    return data;
                }
            case VersionBDataTypeEnum.Int32Array:
                {
                    var size = this.Value.ConvertOrDefaultTo<byte[]>().Count() / sizeof(int);
                    var data = new int[size];

                    for (var index = 0; index < size; index++)
                    {
                        data[index] = BitConverter.ToInt32(this.Value.ConvertOrDefaultTo<byte[]>(), index * sizeof(int));
                    }

                    return data;
                }
            case VersionBDataTypeEnum.Int64Array:
                {
                    var size = this.Value.ConvertOrDefaultTo<byte[]>().Count() / sizeof(long);
                    var data = new long[size];

                    for (var index = 0; index < size; index++)
                    {
                        data[index] = BitConverter.ToInt64(this.Value.ConvertOrDefaultTo<byte[]>(), index * sizeof(long));
                    }

                    return data;
                }
            case VersionBDataTypeEnum.UInt8Array:
                return this.Value.ConvertOrDefaultTo<byte[]>();
            case VersionBDataTypeEnum.UInt16Array:
                {
                    var size = this.Value.ConvertOrDefaultTo<byte[]>().Count() / sizeof(ushort);
                    var data = new ushort[size];

                    for (var index = 0; index < size; index++)
                    {
                        data[index] = BitConverter.ToUInt16(this.Value.ConvertOrDefaultTo<byte[]>(), index * sizeof(ushort));
                    }

                    return data;
                }
            case VersionBDataTypeEnum.UInt32Array:
                {
                    var size = this.Value.ConvertOrDefaultTo<byte[]>().Count() / sizeof(uint);
                    var data = new uint[size];

                    for (var index = 0; index < size; index++)
                    {
                        data[index] = BitConverter.ToUInt32(this.Value.ConvertOrDefaultTo<byte[]>(), index * sizeof(uint));
                    }

                    return data;
                }
            case VersionBDataTypeEnum.UInt64Array:
                {
                    var size = this.Value.ConvertOrDefaultTo<byte[]>().Count() / sizeof(ulong);
                    var data = new ulong[size];

                    for (var index = 0; index < size; index++)
                    {
                        data[index] = BitConverter.ToUInt64(this.Value.ConvertOrDefaultTo<byte[]>(), index * sizeof(ulong));
                    }

                    return data;
                }
            case VersionBDataTypeEnum.DateTimeArray:
                {
                    var size = this.Value.ConvertOrDefaultTo<byte[]>().Count() / sizeof(long);
                    var data = new DateTimeOffset[size];

                    for (var index = 0; index < size; index++)
                    {
                        var number = BitConverter.ToInt64(this.Value.ConvertOrDefaultTo<byte[]>(), index * sizeof(long));
                        data[index] = DateTimeOffset.FromUnixTimeMilliseconds(number);
                    }

                    return data;
                }
            case VersionBDataTypeEnum.FloatArray:
                {
                    var size = this.Value.ConvertOrDefaultTo<byte[]>().Count() / sizeof(float);
                    var data = new float[size];

                    for (var index = 0; index < size; index++)
                    {
                        data[index] = BitConverter.ToSingle(this.Value.ConvertOrDefaultTo<byte[]>(), index * sizeof(float));
                    }

                    return data;
                }
            case VersionBDataTypeEnum.DoubleArray:
                {
                    var size = this.Value.ConvertOrDefaultTo<byte[]>().Count() / sizeof(double);
                    var data = new double[size];

                    for (var index = 0; index < size; index++)
                    {
                        data[index] = BitConverter.ToDouble(this.Value.ConvertOrDefaultTo<byte[]>(), index * sizeof(double));
                    }

                    return data;
                }
            case VersionBDataTypeEnum.BooleanArray:
                {
                    var size = this.Value.ConvertOrDefaultTo<byte[]>().Count() / sizeof(bool);
                    var data = new bool[size];

                    for (var index = 0; index < size; index++)
                    {
                        data[index] = BitConverter.ToBoolean(this.Value.ConvertOrDefaultTo<byte[]>(), index * sizeof(bool));
                    }

                    return data;
                }
            // Todo: What to do here?
            case VersionBDataTypeEnum.StringArray:
                {
                    return null;

                    //var size = this.Value.ConvertOrDefaultTo<byte[]>().Count() / sizeof(string);
                    //var data = new string[size];

                    //for (var index = 0; index < size; index++)
                    //{
                    //    data[index] = Encoding.UTF8.ToString(this.Value.ConvertOrDefaultTo<byte[]>(), index * sizeof(string));
                    //}

                    //return data;
                }
            // Todo: What to do here?
            case VersionBDataTypeEnum.PropertySetList:
            default:
                return base.Value;
        }
    }
}
