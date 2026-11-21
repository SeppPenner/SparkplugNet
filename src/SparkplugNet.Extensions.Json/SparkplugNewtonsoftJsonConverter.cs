// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugNewtonsoftJsonConverter.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   The Newtonsoft.Json converter for Sparkplug B values.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Extensions.Json;

/// <summary>
/// Converts Sparkplug B metrics and their values using Newtonsoft.Json.
/// </summary>
public sealed class SparkplugNewtonsoftJsonConverter : JsonNet.JsonConverter
{
    /// <inheritdoc />
    public override bool CanConvert(Type objectType) => SparkplugValueJson.GetDocumentType<JToken>(objectType) is not null;

    /// <inheritdoc />
    public override object? ReadJson(
        JsonNet.JsonReader reader,
        Type objectType,
        object? existingValue,
        JsonNet.JsonSerializer serializer)
    {
        if (reader.TokenType == JsonNet.JsonToken.Null)
        {
            return null;
        }

        var documentType = SparkplugValueJson.GetDocumentType<JToken>(objectType)!;
        var document = (ValueDocument<JToken>)ReadObject(reader).ToObject(documentType, serializer)!;
        var hasValue = document.Value is { Type: not (JTokenType.Null or JTokenType.Undefined) };

        try
        {
            return SparkplugValueJson.Read(document, hasValue, type => document.Value.ToObject(type, serializer));
        }
        catch (Exception exception) when (exception is FormatException or NotImplementedException or InvalidOperationException or OverflowException)
        {
            throw new JsonNet.JsonSerializationException($"Invalid Sparkplug {objectType.Name} value of type {document.DataType}.", exception);
        }
    }

    /// <inheritdoc />
    public override void WriteJson(JsonNet.JsonWriter writer, object? value, JsonNet.JsonSerializer serializer)
    {
        if (value is null)
        {
            writer.WriteNull();
            return;
        }

        var document = SparkplugValueJson.Write((ValueBaseVersionB)value);
        serializer.Serialize(writer, document, document.GetType());
    }

    /// <summary>
    /// Reads the raw object without treating date-like strings as dates before seeing the declared data type.
    /// </summary>
    /// <param name="reader">The JSON reader positioned at the object.</param>
    /// <returns>The raw JSON object.</returns>
    private static JObject ReadObject(JsonNet.JsonReader reader)
    {
        var dateParseHandling = reader.DateParseHandling;
        try
        {
            reader.DateParseHandling = JsonNet.DateParseHandling.None;
            return JObject.Load(reader);
        }
        finally
        {
            reader.DateParseHandling = dateParseHandling;
        }
    }
}
