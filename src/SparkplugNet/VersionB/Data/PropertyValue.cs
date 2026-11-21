// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyValue.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   The externally used Sparkplug B property value class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.VersionB.Data;

/// <summary>
/// The externally used Sparkplug B property value class.
/// </summary>
public class PropertyValue : ValueBaseVersionB
{
    /// <summary>
    /// Gets or sets a value indicating whether the property value is null or not.
    /// </summary>
    public bool? IsNull { get; protected set; }

    /// <summary>
    /// Gets the property value.
    /// </summary>
    public override object? Value => this.DataType is VersionBDataTypeEnum.PropertySet or VersionBDataTypeEnum.PropertySetList
        ? this.ObjectValue
        : base.Value;

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyValue"/> class.
    /// </summary>
    public PropertyValue()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyValue"/> class.
    /// </summary>
    /// <param name="dataType">The data type.</param>
    /// <param name="value">The value.</param>
    public PropertyValue(VersionBDataTypeEnum dataType, object? value)
    {
        this.IsNull = value is null;
        this.SetValue(dataType, value);
    }

    /// <summary>
    /// Sets the property type and value.
    /// </summary>
    /// <param name="dataType">The data type.</param>
    /// <param name="value">The value.</param>
    /// <returns>The property value.</returns>
    public override IValue<VersionBDataTypeEnum> SetValue(VersionBDataTypeEnum dataType, object? value)
    {
        if (dataType is VersionBDataTypeEnum.PropertySet or VersionBDataTypeEnum.PropertySetList)
        {
            this.ObjectValue = dataType == VersionBDataTypeEnum.PropertySet ? (PropertySet?)value : (PropertySetList?)value;
            this.DataType = dataType;
        }
        else
        {
            base.SetValue(dataType, value);
        }

        this.IsNull = value is null;
        return this;
    }
}
