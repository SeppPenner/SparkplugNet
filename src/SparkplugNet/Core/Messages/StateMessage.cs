// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StateMessage.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   The STATE message class to serialize the data.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Core.Messages;

/// <summary>
/// The STATE message class to serialize the data.
/// </summary>
public sealed record class StateMessage
{
    /// <summary>
    /// Gets or sets a value indicating whether the state is online or offline.
    /// </summary>
    [JsonPropertyName("online")]
    public bool Online { get; init; }

    /// <summary>
    /// Gets or sets the timestamp.
    /// </summary>
    [JsonPropertyName("timestamp")]
    public long Timestamp { get; init; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    /// <summary>
    /// Reads the required fields of a Sparkplug 3.0 STATE payload.
    /// </summary>
    /// <param name="payload">The JSON payload.</param>
    /// <param name="online">Whether the host is online.</param>
    /// <param name="timestamp">The UTC timestamp in milliseconds.</param>
    /// <returns>Whether both fields are present with their expected types.</returns>
    internal static bool TryReadJson(string payload, out bool online, out long timestamp)
    {
        online = false;
        timestamp = 0;

        try
        {
            using var document = JsonDocument.Parse(payload);
            var root = document.RootElement;
            if (root.ValueKind != JsonValueKind.Object ||
                !root.TryGetProperty("online", out var onlineValue) || onlineValue.ValueKind is not (JsonValueKind.True or JsonValueKind.False) ||
                !root.TryGetProperty("timestamp", out var timestampValue) || timestampValue.ValueKind != JsonValueKind.Number ||
                !timestampValue.TryGetInt64(out timestamp))
            {
                return false;
            }

            online = onlineValue.GetBoolean();
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }
}
