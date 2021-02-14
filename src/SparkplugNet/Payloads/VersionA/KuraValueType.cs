// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KuraValueType.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class that handles a Sparkplug version A payload value type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Payloads.VersionA
{
    /// <summary>
    /// A class that handles a Sparkplug version A payload value type.
    /// </summary>
    // ReSharper disable StyleCop.SA1650
    public enum KuraValueType
    {
        /// <summary>
        /// The double value type.
        /// </summary>
        Double = 0,

        /// <summary>
        /// The float value type.
        /// </summary>
        Float = 1,

        /// <summary>
        /// The int64 value type.
        /// </summary>
        Int64 = 2,

        /// <summary>
        /// The int32 value type.
        /// </summary>
        Int32 = 3,

        /// <summary>
        /// The bool value type.
        /// </summary>
        Bool = 4,

        /// <summary>
        /// The string value type.
        /// </summary>
        String = 5,

        /// <summary>
        /// The bytes value type.
        /// </summary>
        Bytes = 6
    }
}