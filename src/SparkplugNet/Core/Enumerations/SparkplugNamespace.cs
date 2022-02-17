// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugNamespace.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   The Sparkplug namespace enumeration.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Core.Enumerations;

/// <summary>
/// The Sparkplug namespace enumeration.
/// </summary>
public enum SparkplugNamespace
{
    /// <summary>
    /// The version A namespace.
    /// </summary>
    [Description("spAv1.0")]
    VersionA,

    /// <summary>
    /// The version B namespace.
    /// </summary>
    [Description("spBv1.0")]
    VersionB
}
