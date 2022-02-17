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
public class PropertyValue
{
    /// <summary>
    /// The integer value.
    /// </summary>
    private uint? integerValue;

    /// <summary>
    /// The long value.
    /// </summary>
    private ulong? longValue;

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
    /// The property set value.
    /// </summary>
    private PropertySet? propertySetValue;

    /// <summary>
    /// The property set list value.
    /// </summary>
    private PropertySetList? propertySetListValue;

    /// <summary>
    /// The extension value.
    /// </summary>
    private PropertyValueExtension? extensionValue;

    /// <summary>
    /// Gets or sets the type.
    /// </summary>
    public uint Type { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the property value is null or not.
    /// </summary>
    public bool IsNull { get; set; }

    /// <summary>
    /// Gets or sets the integer value.
    /// </summary>
    public uint IntValue
    {
        get => this.integerValue ?? default;
        set
        {
            this.integerValue = value;
            this.ValueCase = DataType.Int32;
        }
    }

    /// <summary>
    /// Gets or sets the long value.
    /// </summary>
    public ulong LongValue
    {
        get => this.longValue ?? default;
        set
        {
            this.longValue = value;
            this.ValueCase = DataType.Int64;
        }
    }

    /// <summary>
    /// Gets or sets the float value.
    /// </summary>
    public float FloatValue
    {
        get => this.floatValue ?? default;
        set
        {
            this.floatValue = value;
            this.ValueCase = DataType.Float;
        }
    }

    /// <summary>
    /// Gets or sets the double value.
    /// </summary>
    public double DoubleValue
    {
        get => this.doubleValue ?? default;
        set
        {
            this.doubleValue = value;
            this.ValueCase = DataType.Double;
        }
    }

    /// <summary>
    /// Gets or sets the boolean value.
    /// </summary>
    public bool BooleanValue
    {
        get => this.booleanValue ?? default;
        set
        {
            this.booleanValue = value;
            this.ValueCase = DataType.Boolean;
        }
    }

    /// <summary>
    /// Gets or sets the string value.
    /// </summary>
    [DefaultValue("")]
    public string StringValue
    {
        get => this.stringValue ?? string.Empty;
        set
        {
            this.stringValue = value;
            this.ValueCase = DataType.String;
        }
    }

    /// <summary>
    /// Gets or sets the property set value.
    /// </summary>
    public PropertySet PropertySetValue
    {
        get => this.propertySetValue ?? new ();
        set
        {
            this.propertySetValue = value;
            this.ValueCase = DataType.PropertySet;
        }
    }

    /// <summary>
    /// Gets or sets the property set list value.
    /// </summary>
    public PropertySetList PropertySetsValue
    {
        get => this.propertySetListValue ?? new ();
        set
        {
            this.propertySetListValue = value;
            this.ValueCase = DataType.PropertySet;
        }
    }

    /// <summary>
    /// Gets or sets the extension value.
    /// </summary>
    public PropertyValueExtension ExtensionValue
    {
        get => this.extensionValue ?? new ();
        set
        {
            this.extensionValue = value;
            this.ValueCase = DataType.Unknown;
        }
    }

    /// <summary>
    /// Gets or sets the value case.
    /// </summary>
    public DataType ValueCase { get; set; }
}
