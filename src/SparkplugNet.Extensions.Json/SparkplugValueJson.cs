// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugValueJson.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   Shared mapping between Sparkplug B values and JSON documents.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Extensions.Json;

/// <summary>
/// Maps Sparkplug B values to and from their JSON representation.
/// </summary>
internal static class SparkplugValueJson
{
    /// <summary>
    /// Finds the document type for a Sparkplug value, or returns null for other models.
    /// </summary>
    /// <typeparam name="TJson">The type used to hold the JSON value.</typeparam>
    /// <param name="modelType">The Sparkplug model type.</param>
    /// <returns>The document type, if supported.</returns>
    internal static Type? GetDocumentType<TJson>(Type modelType) => modelType switch
    {
        Type type when type == typeof(Metric) => typeof(MetricDocument<TJson>),
        Type type when type == typeof(Parameter) => typeof(NamedValueDocument<TJson>),
        Type type when type == typeof(PropertyValue) => typeof(PropertyValueDocument<TJson>),
        Type type when type == typeof(DataSetValue) => typeof(ValueDocument<TJson>),
        _ => null
    };

    /// <summary>
    /// Reconstructs a Sparkplug value once its data type and raw JSON value are known.
    /// </summary>
    /// <typeparam name="TJson">The type used to hold the JSON value.</typeparam>
    /// <param name="document">The input document.</param>
    /// <param name="hasValue">Whether the document contains a non-null JSON value.</param>
    /// <param name="readValue">Reads the JSON value as the selected CLR type.</param>
    /// <returns>The populated Sparkplug model.</returns>
    internal static ValueBaseVersionB Read<TJson>(
        ValueDocument<TJson> document,
        bool hasValue,
        Func<Type, object?> readValue)
    {
        ValueBaseVersionB model = document switch
        {
            MetricDocument<TJson> metric => new Metric
            {
                Name = metric.Name ?? string.Empty,
                Alias = metric.Alias,
                Timestamp = metric.Timestamp,
                IsHistorical = metric.IsHistorical,
                IsTransient = metric.IsTransient,
                MetaData = metric.MetaData,
                Properties = metric.Properties
            },
            NamedValueDocument<TJson> parameter => new Parameter { Name = parameter.Name ?? string.Empty },
            PropertyValueDocument<TJson> => new PropertyValue(),
            _ => new DataSetValue()
        };

        var isNull = document is MetricDocument<TJson> { IsNull: true } or PropertyValueDocument<TJson> { IsNull: true };
        var value = hasValue && !isNull ? readValue(GetValueType(model, document.DataType)) : null;
        value = value switch
        {
            DateTimeOffset time => time.ToUniversalTime(),
            DateTimeOffset[] times => times.Select(time => time.ToUniversalTime()).ToArray(),
            _ => value
        };

        if (model is Metric || document.DataType != DataType.Unknown || value is not null)
        {
            model.SetValue(document.DataType, value);
        }

        return model;
    }

    /// <summary>
    /// Creates the JSON document for a Sparkplug value.
    /// </summary>
    /// <param name="value">The Sparkplug value to serialize.</param>
    /// <returns>The corresponding JSON document.</returns>
    internal static ValueDocument<object?> Write(ValueBaseVersionB value)
    {
        ValueDocument<object?> document = value switch
        {
            Metric metric => new MetricDocument<object?>
            {
                Name = metric.Name,
                Alias = metric.Alias,
                Timestamp = metric.Timestamp,
                IsHistorical = metric.IsHistorical,
                IsTransient = metric.IsTransient,
                IsNull = metric.IsNull,
                MetaData = metric.MetaData,
                Properties = metric.Properties
            },
            Parameter parameter => new NamedValueDocument<object?> { Name = parameter.Name },
            PropertyValue property => new PropertyValueDocument<object?> { IsNull = property.IsNull },
            _ => new ValueDocument<object?>()
        };

        document.DataType = value.DataType;
        document.Value = value is PropertyValue { IsNull: true } ? null : value.Value;
        return document;
    }

    /// <summary>
    /// Gets the CLR type supported by the public Sparkplug model for the requested data type.
    /// </summary>
    /// <param name="model">The Sparkplug model.</param>
    /// <param name="dataType">The Sparkplug data type.</param>
    /// <returns>The CLR type to deserialize.</returns>
    private static Type GetValueType(ValueBaseVersionB model, DataType dataType) => dataType switch
    {
        DataType.Int8 => typeof(sbyte),
        DataType.Int16 => typeof(short),
        DataType.Int32 => typeof(int),
        DataType.Int64 => typeof(long),
        DataType.UInt8 => typeof(byte),
        DataType.UInt16 => typeof(ushort),
        DataType.UInt32 => typeof(uint),
        DataType.UInt64 => typeof(ulong),
        DataType.Float => typeof(float),
        DataType.Double => typeof(double),
        DataType.Boolean => typeof(bool),
        DataType.String or DataType.Text => typeof(string),
        DataType.DateTime => typeof(DateTimeOffset),
        DataType.Uuid => typeof(Guid),
        DataType.PropertySet when model is Metric or PropertyValue => typeof(PropertySet),
        DataType.PropertySetList when model is PropertyValue => typeof(PropertySetList),
        _ when model is Metric => GetMetricValueType(dataType),
        _ => throw new FormatException($"Data type {dataType} is not supported for {model.GetType().Name}.")
    };

    /// <summary>
    /// Gets the CLR type for non-scalar Sparkplug metric values.
    /// </summary>
    /// <param name="dataType">The Sparkplug data type.</param>
    /// <returns>The CLR type to deserialize.</returns>
    private static Type GetMetricValueType(DataType dataType) => dataType switch
    {
        DataType.DataSet => typeof(DataSet),
        DataType.Bytes or DataType.File or DataType.UInt8Array => typeof(byte[]),
        DataType.Template => typeof(Template),
        DataType.Int8Array => typeof(sbyte[]),
        DataType.Int16Array => typeof(short[]),
        DataType.Int32Array => typeof(int[]),
        DataType.Int64Array => typeof(long[]),
        DataType.UInt16Array => typeof(ushort[]),
        DataType.UInt32Array => typeof(uint[]),
        DataType.UInt64Array => typeof(ulong[]),
        DataType.FloatArray => typeof(float[]),
        DataType.DoubleArray => typeof(double[]),
        DataType.BooleanArray => typeof(bool[]),
        DataType.StringArray => typeof(string[]),
        DataType.DateTimeArray => typeof(DateTimeOffset[]),
        _ => throw new FormatException($"Data type {dataType} is not supported for Metric.")
    };
}
