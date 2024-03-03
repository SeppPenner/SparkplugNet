// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueBase.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A base value class for shared properties amongst the data classes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Core.Data;

/// <summary>
/// A base value class for shared properties amongst the data classes.
/// </summary>
/// <typeparam name="DataTypeEnum">The data type.</typeparam>
public abstract class ValueBase<DataTypeEnum> : IValue<DataTypeEnum> where DataTypeEnum : Enum
{
    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    protected object? ObjectValue;

    /// <summary>
    /// Gets or sets the DataType.
    /// </summary>
    public DataTypeEnum DataType { get; protected set; } = default!;

    /// <summary>
    /// Gets the value.
    /// </summary>
    public virtual object? Value { get; protected set; }

    /// <summary>
    /// Sets the value.
    /// </summary>
    /// <param name="dataType">Type of the data.</param>
    /// <param name="value">The value.</param>
    /// <returns>The metric value.</returns>
    public abstract IValue<DataTypeEnum> SetValue(DataTypeEnum dataType, object? value);
}
