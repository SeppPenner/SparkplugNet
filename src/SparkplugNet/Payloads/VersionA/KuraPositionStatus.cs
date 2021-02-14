// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KuraPositionStatus.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class that handles a Sparkplug version A payload position status.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Payloads.VersionA
{
    /// <summary>
    /// A class that handles a Sparkplug version A payload position status.
    /// </summary>
    public enum KuraPositionStatus
    {
        /// <summary>
        /// The no GPS response position status.
        /// </summary>
        NoGpsResponse = 1,

        /// <summary>
        /// The error in response position status.
        /// </summary>
        ErrorInResponse = 2,

        /// <summary>
        /// The valid position status.
        /// </summary>
        Valid = 4
    }
}