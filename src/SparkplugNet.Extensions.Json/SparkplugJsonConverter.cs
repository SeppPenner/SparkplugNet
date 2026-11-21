// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugJsonConverter.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   The System.Text.Json converter for Sparkplug B values.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Extensions.Json;

/// <summary>
/// Converts Sparkplug B metrics and their values using System.Text.Json.
/// </summary>
public sealed class SparkplugJsonConverter : JsonConverterFactory
{
    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert) => SparkplugValueJson.GetDocumentType<JsonElement>(typeToConvert) is not null;

    /// <inheritdoc />
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        if (typeToConvert == typeof(Metric))
        {
            return new ValueConverter<Metric>();
        }

        if (typeToConvert == typeof(Parameter))
        {
            return new ValueConverter<Parameter>();
        }

        if (typeToConvert == typeof(PropertyValue))
        {
            return new ValueConverter<PropertyValue>();
        }

        if (typeToConvert == typeof(DataSetValue))
        {
            return new ValueConverter<DataSetValue>();
        }

        throw new ArgumentException($"Unsupported Sparkplug model type {typeToConvert}.", nameof(typeToConvert));
    }

    /// <summary>
    /// Converts the selected Sparkplug value type.
    /// </summary>
    /// <typeparam name="TValue">The Sparkplug value type.</typeparam>
    private sealed class ValueConverter<TValue> : JsonConverter<TValue> where TValue : ValueBaseVersionB
    {
        /// <inheritdoc />
        public override TValue Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var documentType = SparkplugValueJson.GetDocumentType<JsonElement>(typeToConvert)!;
            var document = (ValueDocument<JsonElement>)JsonSerializer.Deserialize(ref reader, documentType, options)!;
            var hasValue = document.Value.ValueKind is not (JsonValueKind.Null or JsonValueKind.Undefined);

            try
            {
                return (TValue)SparkplugValueJson.Read(document, hasValue, type => document.Value.Deserialize(type, options));
            }
            catch (Exception exception) when (exception is FormatException or NotImplementedException or InvalidOperationException)
            {
                throw new JsonException($"Invalid Sparkplug {typeToConvert.Name} value of type {document.DataType}.", exception);
            }
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, TValue value, JsonSerializerOptions options)
        {
            var document = SparkplugValueJson.Write(value);
            JsonSerializer.Serialize(writer, document, document.GetType(), options);
        }
    }
}
