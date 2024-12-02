// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugTopicGenerator.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   The Sparkplug topic generator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Core.Messages;

/// <summary>
/// The Sparkplug topic generator.
/// </summary>
internal static class SparkplugTopicGenerator
{
    /// <summary>
    /// Gets the wildcard namespace subscription topic.
    /// </summary>
    /// <param name="nameSpace">The namespace.</param>
    /// <returns>The wildcard namespace subscription topic <see cref="string"/>.</returns>
    internal static string GetWildcardNamespaceSubscribeTopic(SparkplugNamespace nameSpace)
    {
        return $"{nameSpace.GetDescription()}/#";
    }

    /// <summary>
    /// Gets the node command subscription topic.
    /// </summary>
    /// <param name="nameSpace">The namespace.</param>
    /// <param name="groupIdentifier">The group identifier.</param>
    /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
    /// <returns>The node command subscription topic <see cref="string"/>.</returns>
    internal static string GetNodeCommandSubscribeTopic(SparkplugNamespace nameSpace, string groupIdentifier, string edgeNodeIdentifier)
    {
        return $"{nameSpace.GetDescription()}/{groupIdentifier}/{SparkplugMessageType.NodeCommand.GetDescription()}/{edgeNodeIdentifier}";
    }

    /// <summary>
    /// Gets the wildcard device command subscription topic.
    /// </summary>
    /// <param name="nameSpace">The namespace.</param>
    /// <param name="groupIdentifier">The group identifier.</param>
    /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
    /// <returns>The wildcard device command subscription topic <see cref="string"/>.</returns>
    internal static string GetWildcardDeviceCommandSubscribeTopic(SparkplugNamespace nameSpace, string groupIdentifier, string edgeNodeIdentifier)
    {
        return $"{nameSpace.GetDescription()}/{groupIdentifier}/{SparkplugMessageType.DeviceCommand.GetDescription()}/{edgeNodeIdentifier}/#";
    }

    /// <summary>
    /// Gets the device command subscription topic.
    /// </summary>
    /// <param name="nameSpace">The namespace.</param>
    /// <param name="groupIdentifier">The group identifier.</param>
    /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
    /// <param name="deviceIdentifier">The device identifier.</param>
    /// <returns>The wildcard device command subscription topic <see cref="string"/>.</returns>
    internal static string GetDeviceCommandSubscribeTopic(SparkplugNamespace nameSpace, string groupIdentifier, string edgeNodeIdentifier, string deviceIdentifier)
    {
        return $"{nameSpace.GetDescription()}/{groupIdentifier}/{SparkplugMessageType.DeviceCommand.GetDescription()}/{edgeNodeIdentifier}/{deviceIdentifier}";
    }

    /// <summary>
    /// Gets the topic (Except STATE messages).
    /// </summary>
    /// <param name="nameSpace">The namespace.</param>
    /// <param name="groupIdentifier">The group identifier.</param>
    /// <param name="messageType">The message type.</param>
    /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
    /// <param name="deviceIdentifier">The device identifier. (Optional)</param>
    /// <returns>The topic as <see cref="string"/>.</returns>
    internal static string GetTopic(
        SparkplugNamespace nameSpace,
        string groupIdentifier,
        SparkplugMessageType messageType,
        string edgeNodeIdentifier,
        string? deviceIdentifier)
    {
        return string.IsNullOrWhiteSpace(deviceIdentifier)
                   ? $"{nameSpace.GetDescription()}/{groupIdentifier}/{messageType.GetDescription()}/{edgeNodeIdentifier}"
                   : $"{nameSpace.GetDescription()}/{groupIdentifier}/{messageType.GetDescription()}/{edgeNodeIdentifier}/{deviceIdentifier}";
    }

    /// <summary>
    /// Gets the STATE message topic.
    /// </summary>
    /// <param name="scadaHostIdentifier">The SCADA host identifier.</param>
    /// <param name="specificationVersion">The Sparkplug specification version.</param>
    /// <returns>The STATE message topic as <see cref="string"/>.</returns>
    internal static string GetSparkplugStateMessageTopic(string scadaHostIdentifier, SparkplugSpecificationVersion specificationVersion)
    {
        return specificationVersion switch
        {
            SparkplugSpecificationVersion.Version22 => $"{SparkplugMessageType.StateMessage.GetDescription()}/{scadaHostIdentifier}",
            SparkplugSpecificationVersion.Version30 => $"{SparkplugTopic.NamespaceSparkplugB}/{SparkplugMessageType.StateMessage.GetDescription()}/{scadaHostIdentifier}",
            _ => throw new NotImplementedException("Unknown Sparkplug specification version")
        };
    }
}
