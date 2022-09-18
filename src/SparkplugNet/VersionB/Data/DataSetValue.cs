// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataSetValue.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   The externally used Sparkplug B data set value class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.VersionB.Data;

/// <summary>
/// The externally used Sparkplug B data set value class.
/// </summary>
public class DataSetValue
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
    /// The extension value.
    /// </summary>
    private DataSetValueExtension? extensionValue;

    /// <summary>
    /// Gets or sets the integer value.
    /// </summary>
    public uint IntValue
    {
        get => this.integerValue ?? default;
        set
        {
            this.integerValue = value;
            this.ValueCase = VersionBDataTypeEnum.Int32;
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
            this.ValueCase = VersionBDataTypeEnum.Int64;
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
            this.ValueCase = VersionBDataTypeEnum.Float;
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
            this.ValueCase = VersionBDataTypeEnum.Double;
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
            this.ValueCase = VersionBDataTypeEnum.Boolean;
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
            this.ValueCase = VersionBDataTypeEnum.String;
        }
    }

    /// <summary>
    /// Gets or sets the extension value.
    /// </summary>
    public DataSetValueExtension ExtensionValue
    {
        get => this.extensionValue ?? new();
        set
        {
            this.extensionValue = value;
            this.ValueCase = VersionBDataTypeEnum.Unknown;
        }
    }

    /// <summary>
    /// Gets or sets the value case.
    /// </summary>
    public VersionBDataTypeEnum ValueCase { get; set; }
}
