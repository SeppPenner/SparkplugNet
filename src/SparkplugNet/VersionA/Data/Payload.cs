// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Payload.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   TThe externally used Sparkplug A payload class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.VersionA.Data;

/// <summary>
/// The externally used Sparkplug A payload class.
/// </summary>
public class Payload
{
    /// <summary>
    /// The timestamp.
    /// </summary>
    private long? timestamp;

    /// <summary>
    /// Gets or sets the timestamp.
    /// </summary>
    public long? Timestamp
    {
        get => this.timestamp.GetValueOrDefault();
        set => this.timestamp = value;
    }

    /// <summary>
    /// Gets or sets the position.
    /// </summary>
    public KuraPosition? Position { get; set; }

    /// <summary>
    /// Gets or sets the metrics.
    /// </summary>
    public List<KuraMetric> Metrics { get; set; } = new();

    /// <summary>
    /// Gets or sets the body.
    /// </summary>
    public byte[] Body { get; set; } = Array.Empty<byte>();
}
