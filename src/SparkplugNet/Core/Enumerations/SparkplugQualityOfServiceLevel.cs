// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugQualityOfServiceLevel.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   The Sparkplug quality of service level enumeration.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Core.Enumerations;

/// <summary>
/// The Sparkplug quality of service level enumeration.
/// </summary>
public enum SparkplugQualityOfServiceLevel
{
    /// <summary>
    /// The at most once Sparkplug quality of service level.
    /// </summary>
    AtMostOnce = 0x00,

    /// <summary>
    /// The at least once Sparkplug quality of service level.
    /// </summary>
    AtLeastOnce = 0x01
}
