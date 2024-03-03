// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KuraPosition.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   The externally used Sparkplug A Kura position class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.VersionA.Data;

/// <summary>
/// The externally used Sparkplug A Kura position class.
/// </summary>
[Obsolete("Sparkplug version A is obsolete since version 3 of the specification, use version B where possible.")]
public sealed class KuraPosition
{
    /// <summary>
    /// Gets or sets the latitude.
    /// </summary>
    public double Latitude { get; set; }

    /// <summary>
    /// Gets or sets the longitude.
    /// </summary>
    public double Longitude { get; set; }

    /// <summary>
    /// Gets or sets the altitude.
    /// </summary>
    public double? Altitude { get; set; }

    /// <summary>
    /// Gets or sets the precision.
    /// </summary>
    public double? Precision { get; set; }

    /// <summary>
    /// Gets or sets the heading.
    /// </summary>
    public double? Heading { get; set; }

    /// <summary>
    /// Gets or sets the speed.
    /// </summary>
    public double? Speed { get; set; }

    /// <summary>
    /// Gets or sets the timestamp.
    /// </summary>
    public long? Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the satellites.
    /// </summary>
    public int? Satellites { get; set; }

    /// <summary>
    /// Gets or sets the status.
    /// </summary>
    public int? Status { get; set; }
}
