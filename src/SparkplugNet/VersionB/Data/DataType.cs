// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataType.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   The Sparkplug B data type enumeration.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.VersionB.Data;

/// <summary>
/// The Sparkplug B data type enumeration.
/// </summary>
public enum DataType
{
    /// <summary>
    /// The unknown data type, for future extension.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// The 8 bit integer data type.
    /// </summary>
    Int8 = 1,

    /// <summary>
    /// The 16 bit integer data type.
    /// </summary>
    Int16 = 2,

    /// <summary>
    /// The 32 bit integer data type.
    /// </summary>
    Int32 = 3,

    /// <summary>
    /// The 64 bit integer data type.
    /// </summary>
    Int64 = 4,

    /// <summary>
    /// The unsigned 8 bit integer data type.
    /// </summary>
    UInt8 = 5,

    /// <summary>
    /// The unsigned 16 bit integer data type.
    /// </summary>
    UInt16 = 6,

    /// <summary>
    /// The unsigned 32 bit integer data type.
    /// </summary>
    UInt32 = 7,

    /// <summary>
    /// The unsigned 64 bit integer data type.
    /// </summary>
    UInt64 = 8,

    /// <summary>
    /// The float data type.
    /// </summary>
    Float = 9,

    /// <summary>
    /// The double data type.
    /// </summary>
    Double = 10,

    /// <summary>
    /// The boolean data type.
    /// </summary>
    Boolean = 11,

    /// <summary>
    /// The string data type.
    /// </summary>
    String = 12,

    /// <summary>
    /// The date time data type.
    /// </summary>
    DateTime = 13,

    /// <summary>
    /// The text data type.
    /// </summary>
    Text = 14,

    /// <summary>
    /// The UUID data type.
    /// </summary>
    Uuid = 15,

    /// <summary>
    /// The data set data type.
    /// </summary>
    DataSet = 16,

    /// <summary>
    /// The bytes type.
    /// </summary>
    Bytes = 17,

    /// <summary>
    /// The file data type.
    /// </summary>
    File = 18,

    /// <summary>
    /// The template data type.
    /// </summary>
    Template = 19,

    /// <summary>
    /// The property set data type.
    /// </summary>
    PropertySet = 20,

    /// <summary>
    /// The property set list data type.
    /// </summary>
    PropertySetList = 21,

    /// <summary>
    /// The 8 bit integer array data type.
    /// </summary>
    Int8Array = 22,

    /// <summary>
    /// The 16 bit integer array data type.
    /// </summary>
    Int16Array = 23,

    /// <summary>
    /// The 32 bit integer array data type.
    /// </summary>
    Int32Array = 24,

    /// <summary>
    /// The 64 bit integer array data type.
    /// </summary>
    Int64Array = 25,

    /// <summary>
    /// The unsigned 8 bit integer array data type.
    /// </summary>
    UInt8Array = 26,

    /// <summary>
    /// The unsigned 16 bit integer array data type.
    /// </summary>
    UInt16Array = 27,

    /// <summary>
    /// The unsigned 32 bit integer array data type.
    /// </summary>
    UInt32Array = 28,

    /// <summary>
    /// The unsigned 64 bit integer array data type.
    /// </summary>
    UInt64Array = 29,

    /// <summary>
    /// The float array data type.
    /// </summary>
    FloatArray = 30,

    /// <summary>
    /// The double array data type.
    /// </summary>
    DoubleArray = 31,

    /// <summary>
    /// The boolean array data type.
    /// </summary>
    BooleanArray = 32,

    /// <summary>
    /// The string array data type.
    /// </summary>
    StringArray = 33,

    /// <summary>
    /// The date time array data type.
    /// </summary>
    DateTimeArray = 34
}
