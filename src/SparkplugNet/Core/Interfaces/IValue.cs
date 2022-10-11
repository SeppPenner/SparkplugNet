namespace SparkplugNet.Core.Interfaces;

/// <summary>
/// The interface for all values.
/// </summary>
public interface IValue<DataTypeEnum> where DataTypeEnum : Enum
{
    /// <summary>
    /// Gets the value.
    /// </summary>
    object? Value { get; }

    /// <summary>
    /// Gets or sets the Value Case.
    /// </summary>
    uint ValueCase { get; set; }

    /// <summary>
    /// Gets or sets the DataType.
    /// </summary>
    DataTypeEnum DataType { get; set; }

    /// <summary>
    /// Sets the value.
    /// </summary>
    /// <param name="dataType">Type of the data.</param>
    /// <param name="value">The value.</param>
    /// <returns>The metric value.</returns>
    IValue<DataTypeEnum> SetValue(DataTypeEnum dataType, object? value);
}

