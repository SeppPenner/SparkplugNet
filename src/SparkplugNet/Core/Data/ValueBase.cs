namespace SparkplugNet.Core.Data;

/// <summary>
/// Base Value class for shared properties amongst Data classes
/// </summary>
/// <typeparam name="DataTypeEnum"></typeparam>
public abstract class ValueBase<DataTypeEnum> : IValue<DataTypeEnum> where DataTypeEnum : Enum
{
    /// <summary>
    /// Gets or sets the float value.
    /// </summary>
    public float FloatValue { get; protected set; }

    /// <summary>
    /// Gets or sets the double value.
    /// </summary>
    public double DoubleValue { get; protected set; }

    /// <summary>
    /// Gets or sets the boolean value.
    /// </summary>
    public bool BooleanValue { get; protected set; }

    /// <summary>
    /// Gets or sets the string value.
    /// </summary>
    [DefaultValue("")]
    public string StringValue { get; protected set; } = string.Empty;

    /// <summary>
    /// Gets or sets the DataType.
    /// </summary>
    public DataTypeEnum DataType { get; protected set; } = default!;

    /// <summary>
    /// Gets the value.
    /// </summary>
    public abstract object? Value { get; }

    /// <summary>
    /// Sets the value.
    /// </summary>
    /// <param name="dataType">Type of the data.</param>
    /// <param name="value">The value.</param>
    /// <returns>The metric value.</returns>
    public abstract IValue<DataTypeEnum> SetValue(DataTypeEnum dataType, object? value);
}
