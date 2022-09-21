// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Payload.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   The externally used Sparkplug B payload class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.VersionB.Data;

/// <summary>
/// The externally used Sparkplug B payload class.
/// </summary>
public class Payload
{
    /// <summary>
    /// Gets or sets the timestamp.
    /// </summary>
    public ulong Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the metrics.
    /// </summary>
    public List<Metric> Metrics { get; set; } = new();

    /// <summary>
    /// Gets or sets the SEQ number.
    /// </summary>
    public ulong Seq { get; set; }

    /// <summary>
    /// Gets or sets the UUID.
    /// </summary>
    [DefaultValue("")]
    public string Uuid { get; set; } = string.Empty;

    /// <summary>
    /// Get sor sets the body.
    /// </summary>
    public byte[]? Body { get; set; }

    /// <summary>
    /// Gets or sets the details.
    /// </summary>
    public List<byte> Details { get; set; } = new();
}
