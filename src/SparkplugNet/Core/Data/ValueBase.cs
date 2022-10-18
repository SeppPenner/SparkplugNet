namespace SparkplugNet.Core.Data;
using System;

/// <summary>
/// Base Value class for shared properties amongst Data classes
/// </summary>
/// <typeparam name="DataTypeEnum"></typeparam>
public abstract class ValueBase<DataTypeEnum> : IValue<DataTypeEnum> where DataTypeEnum : Enum
{
    /// <summary>
    /// The float value.
    /// </summary>
    private float? floatValue;

    /// <summary>
    /// The double value.
    /// </summary>
    private double? doubleValue;

    /// <summary>
    /// The boolean value.
    /// </summary>
    private bool? booleanValue;

    /// <summary>
    /// The string value.
    /// </summary>
    private string? stringValue;

    /// <summary>
    /// Gets or sets the float value.
    /// </summary>
    public virtual float FloatValue
    {
        get => this.floatValue ?? default;
        set => this.floatValue = value;
    }

    /// <summary>
    /// Gets or sets the double value.
    /// </summary>
    public virtual double DoubleValue
    {
        get => this.doubleValue ?? default;
        set => this.doubleValue = value;
    }

    /// <summary>
    /// Gets or sets the boolean value.
    /// </summary>
    public virtual bool BooleanValue
    {
        get => this.booleanValue ?? default;
        set => this.booleanValue = value;
    }

    /// <summary>
    /// Gets or sets the string value.
    /// </summary>
    [DefaultValue("")]
    public virtual string StringValue
    {
        get => this.stringValue ?? string.Empty;
        set => this.stringValue = value;
    }

    /// <summary>
    /// Gets the value.
    /// </summary>
    public abstract object? Value { get; }

    /// <summary>
    /// Gets or sets the Value Case.
    /// </summary>
    public abstract uint ValueCase { get; set; }

    /// <summary>
    /// Gets or sets the DataType.
    /// </summary>
    public abstract DataTypeEnum DataType { get; set; }

    /// <summary>
    /// Sets the value.
    /// </summary>
    /// <param name="dataType">Type of the data.</param>
    /// <param name="value">The value.</param>
    /// <returns>The metric value.</returns>
    public abstract IValue<DataTypeEnum> SetValue(DataTypeEnum dataType, object? value);
}
