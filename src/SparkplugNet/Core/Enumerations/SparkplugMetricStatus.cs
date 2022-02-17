// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugMetricStatus.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   The Sparkplug metric status enumeration.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Core.Enumerations;

/// <summary>
/// The Sparkplug metric status enumeration.
/// </summary>
public enum SparkplugMetricStatus
{
    /// <summary>
    /// The unknown Sparkplug connection status.
    /// </summary>
    Unknown,

    /// <summary>
    /// The online Sparkplug connection status.
    /// </summary>
    Online,

    /// <summary>
    /// The offline Sparkplug connection status.
    /// </summary>
    Offline
}
