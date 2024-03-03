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
}
