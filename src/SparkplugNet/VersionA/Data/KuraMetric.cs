// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KuraMetric.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   The externally used Sparkplug A Kura metric class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.VersionA.Data;

/// <summary>
/// The externally used Sparkplug A Kura metric class.
/// </summary>
public class KuraMetric
{
    /// <summary>
    /// The double value.
    /// </summary>
    private double? doubleValue;

    /// <summary>
    /// The float value.
    /// </summary>
    private float? floatValue;

    /// <summary>
    /// The long value.
    /// </summary>
    private long? longValue;

    /// <summary>
    /// The integer value.
    /// </summary>
    private int? intValue;

    /// <summary>
    /// The boolean value.
    /// </summary>
    private bool? boolValue;

    /// <summary>
    /// The string value.
    /// </summary>
    private string? stringValue;

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type.
    /// </summary>
    public DataType Type { get; set; }

    /// <summary>
    /// Gets or sets the double value.
    /// </summary>
    public double? DoubleValue
    {
        get => this.doubleValue.GetValueOrDefault();
        set => this.doubleValue = value;
    }

    /// <summary>
    /// Gets or sets the float value.
    /// </summary>
    public float? FloatValue
    {
        get => this.floatValue.GetValueOrDefault();
        set => this.floatValue = value;
    }

    /// <summary>
    /// Gets or sets the long value.
    /// </summary>
    public long? LongValue
    {
        get => this.longValue.GetValueOrDefault();
        set => this.longValue = value;
    }

    /// <summary>
    /// Gets or sets the integer value.
    /// </summary>
    public int? IntValue
    {
        get => this.intValue.GetValueOrDefault();
        set => this.intValue = value;
    }

    /// <summary>
    /// Gets or sets the boolean value.
    /// </summary>
    public bool? BoolValue
    {
        get => this.boolValue.GetValueOrDefault();
        set => this.boolValue = value;
    }

    /// <summary>
    /// Gets or sets the string value.
    /// </summary>
    [DefaultValue("")]
    public string? StringValue
    {
        get => this.stringValue ?? string.Empty;
        set => this.stringValue = value;
    }

    /// <summary>
    /// Gets or sets the bytes value.
    /// </summary>
    public byte[]? BytesValue { get; set; }
}
