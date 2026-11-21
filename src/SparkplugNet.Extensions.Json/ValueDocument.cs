// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueDocument.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   JSON documents for Sparkplug B values.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Extensions.Json;

/// <summary>
/// The JSON representation of a Sparkplug B value.
/// </summary>
/// <typeparam name="TJson">The type used to hold the value before or after JSON conversion.</typeparam>
internal class ValueDocument<TJson>
{
    /// <summary>
    /// Gets or sets the Sparkplug data type.
    /// </summary>
    public DataType DataType { get; set; }

    /// <summary>
    /// Gets or sets the JSON value.
    /// </summary>
    public TJson Value { get; set; } = default!;
}

/// <summary>
/// The JSON representation of a named Sparkplug B value.
/// </summary>
/// <typeparam name="TJson">The type used to hold the JSON value.</typeparam>
internal class NamedValueDocument<TJson> : ValueDocument<TJson>
{
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// The JSON representation of a Sparkplug B metric.
/// </summary>
/// <typeparam name="TJson">The type used to hold the JSON value.</typeparam>
internal sealed class MetricDocument<TJson> : NamedValueDocument<TJson>
{
    /// <summary>
    /// Gets or sets the alias.
    /// </summary>
    public ulong? Alias { get; set; }

    /// <summary>
    /// Gets or sets the timestamp in Unix milliseconds.
    /// </summary>
    public ulong? Timestamp { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the metric is historical.
    /// </summary>
    public bool? IsHistorical { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the metric is transient.
    /// </summary>
    public bool? IsTransient { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the metric is null.
    /// </summary>
    public bool IsNull { get; set; }

    /// <summary>
    /// Gets or sets the metadata.
    /// </summary>
    public MetaData? MetaData { get; set; }

    /// <summary>
    /// Gets or sets the properties.
    /// </summary>
    public PropertySet? Properties { get; set; }
}

/// <summary>
/// The JSON representation of a Sparkplug B property value.
/// </summary>
/// <typeparam name="TJson">The type used to hold the JSON value.</typeparam>
internal sealed class PropertyValueDocument<TJson> : ValueDocument<TJson>
{
    /// <summary>
    /// Gets or sets a value indicating whether the property value is null.
    /// </summary>
    public bool? IsNull { get; set; }
}
