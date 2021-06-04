// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugADataType.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   The Sparkplug A data type enumeration.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Core.Enumerations
{
    /// <summary>
    /// The Sparkplug A data type enumeration.
    /// </summary>
    public enum SparkplugADataType
    {
        /// <summary>
        /// Unknown placeholder for future expansion
        /// </summary>
        Double = 0,

        /// <summary>
        /// Int8
        /// </summary>
        Float = 1,

        /// <summary>
        /// Int16
        /// </summary>
        Int64 = 2,

        /// <summary>
        /// Int32
        /// </summary>
        Int32 = 3,

        /// <summary>
        /// Int64
        /// </summary>
        Bool = 4,

        /// <summary>
        /// UInt8
        /// </summary>
        String = 5,

        /// <summary>
        /// UInt16
        /// </summary>
        Bytes = 6
    }
}