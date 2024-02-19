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
[Obsolete("Sparkplug version A is obsolete since version 3 of the specification, use version B where possible.")]
public class Payload
{
    /// <summary>
    /// Gets or sets the timestamp.
    /// </summary>
    public long? Timestamp { get; set; }

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
