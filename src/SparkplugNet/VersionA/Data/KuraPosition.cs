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
public class KuraPosition
{
    /// <summary>
    /// The altitude.
    /// </summary>
    private double? altitude;

    /// <summary>
    /// The precision.
    /// </summary>
    private double? precision;

    /// <summary>
    /// The heading.
    /// </summary>
    private double? heading;

    /// <summary>
    /// The speed.
    /// </summary>
    private double? speed;

    /// <summary>
    /// The timestamp.
    /// </summary>
    private long? timestamp;

    /// <summary>
    /// The satellites.
    /// </summary>
    private int? satellites;

    /// <summary>
    /// The status.
    /// </summary>
    private int? status;

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
    public double? Altitude
    {
        get => this.altitude.GetValueOrDefault();
        set => this.altitude = value;
    }

    /// <summary>
    /// Gets or sets the precision.
    /// </summary>
    public double? Precision
    {
        get => this.precision.GetValueOrDefault();
        set => this.precision = value;
    }

    /// <summary>
    /// Gets or sets the heading.
    /// </summary>
    public double? Heading
    {
        get => this.heading.GetValueOrDefault();
        set => this.heading = value;
    }

    /// <summary>
    /// Gets or sets the speed.
    /// </summary>
    public double? Speed
    {
        get => this.speed.GetValueOrDefault();
        set => this.speed = value;
    }

    /// <summary>
    /// Gets or sets the timestamp.
    /// </summary>
    public long? Timestamp
    {
        get => this.timestamp.GetValueOrDefault();
        set => this.timestamp = value;
    }

    /// <summary>
    /// Gets or sets the satellites.
    /// </summary>
    public int? Satellites
    {
        get => this.satellites.GetValueOrDefault();
        set => this.satellites = value;
    }

    /// <summary>
    /// Gets or sets the status.
    /// </summary>
    public int? Status
    {
        get => this.status.GetValueOrDefault();
        set => this.status = value;
    }
}
