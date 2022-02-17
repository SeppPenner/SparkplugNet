// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugMessageType.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   The Sparkplug message type enumeration.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Core.Enumerations;

/// <summary>
/// The Sparkplug message type enumeration.
/// </summary>
public enum SparkplugMessageType
{
    /// <summary>
    /// The node birth message type.
    /// </summary>
    [Description("NBIRTH")]
    NodeBirth,

    /// <summary>
    /// The death birth message type.
    /// </summary>
    [Description("NDEATH")]
    NodeDeath,

    /// <summary>
    /// The device birth message type.
    /// </summary>
    [Description("DBIRTH")]
    DeviceBirth,

    /// <summary>
    /// The device death message type.
    /// </summary>
    [Description("DDEATH")]
    DeviceDeath,

    /// <summary>
    /// The node data message type.
    /// </summary>
    [Description("NDATA")]
    NodeData,

    /// <summary>
    /// The device data message type.
    /// </summary>
    [Description("DDATA")]
    DeviceData,

    /// <summary>
    /// The node command message type.
    /// </summary>
    [Description("NCMD")]
    NodeCommand,

    /// <summary>
    /// The device command message type.
    /// </summary>
    [Description("DCMD")]
    DeviceCommand,

    /// <summary>
    /// The state message message type.
    /// </summary>
    [Description("STATE")]
    StateMessage
}
