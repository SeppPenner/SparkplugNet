// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataType.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   The externally used Sparkplug A data type enumeration.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.VersionA.Data;

/// <summary>
/// The externally used Sparkplug A data type enumeration.
/// </summary>
public enum DataType
{
    /// <summary>
    /// The double data type.
    /// </summary>
    Double = 0,

    /// <summary>
    /// The float data type.
    /// </summary>
    Float = 1,

    /// <summary>
    /// The 64 bit integer data type.
    /// </summary>
    Int64 = 2,

    /// <summary>
    /// The 32 bit integer data type.
    /// </summary>
    Int32 = 3,

    /// <summary>
    /// The boolean data type.
    /// </summary>
    Boolean = 4,

    /// <summary>
    /// The string data type.
    /// </summary>
    String = 5,

    /// <summary>
    /// The bytes data type.
    /// </summary>
    Bytes = 6
}
