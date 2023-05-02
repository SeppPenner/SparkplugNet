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
public sealed class StateMessage
{
    /// <summary>
    /// Gets or sets a value indicating whether the state is online or offline.
    /// </summary>
    [JsonPropertyName("online")]
    public bool Online { get; set; }

    /// <summary>
    /// Gets or sets the timestamp.
    /// </summary>
    [JsonPropertyName("timestamp")]
    public long Timestamp { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
}
