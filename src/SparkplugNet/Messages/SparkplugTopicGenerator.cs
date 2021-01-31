// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugTopicGenerator.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   The Sparkplug topic generator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Messages
{
    using System;

    using SparkplugNet.Enumerations;
    using SparkplugNet.Extensions;

    /// <summary>
    /// The Sparkplug topic generator.
    /// </summary>
    internal class SparkplugTopicGenerator
    {
        /// <summary>
        /// Gets the wildcard namespace subscription topic.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <param name="nameSpace">The namespace.</param>
        /// <returns>The wildcard namespace subscription topic <see cref="string"/></returns>
        internal string GetWildcardNamespaceSubscribeTopic(SparkplugVersion version, SparkplugNamespace nameSpace)
        {
            if (version is SparkplugVersion.V22)
            {
                return $"{nameSpace.GetDescription()}/#";
            }

            throw new ArgumentOutOfRangeException(nameof(version));
        }

        /// <summary>
        /// Gets the node command subscription topic.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <param name="nameSpace">The namespace.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <returns>The node command subscription topic <see cref="string"/>.</returns>
        internal string GetNodeCommandSubscribeTopic(SparkplugVersion version, SparkplugNamespace nameSpace, string groupIdentifier, string edgeNodeIdentifier)
        {
            if (version is SparkplugVersion.V22)
            {
                return $"{nameSpace.GetDescription()}/{groupIdentifier}/{SparkplugMessageType.NodeCommand.GetDescription()}/{edgeNodeIdentifier}";
            }

            throw new ArgumentOutOfRangeException(nameof(version));
        }

        /// <summary>
        /// Gets the wildcard device command subscription topic.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <param name="nameSpace">The namespace.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <returns>The wildcard device command subscription topic <see cref="string"/>.</returns>
        internal string GetWildcardDeviceCommandSubscribeTopic(SparkplugVersion version, SparkplugNamespace nameSpace, string groupIdentifier, string edgeNodeIdentifier)
        {
            if (version is SparkplugVersion.V22)
            {
                return $"{nameSpace.GetDescription()}/{groupIdentifier}/{SparkplugMessageType.DeviceCommand.GetDescription()}/{edgeNodeIdentifier}/#";
            }

            throw new ArgumentOutOfRangeException(nameof(version));
        }

        /// <summary>
        /// Gets state subscription topic.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <param name="scadaHostIdentifier">The SCADA host identifier.</param>
        /// <returns>The state subscription topic <see cref="string"/></returns>
        internal string GetStateSubscribeTopic(SparkplugVersion version, string scadaHostIdentifier)
        {
            if (version is SparkplugVersion.V22)
            {
                return $"{SparkplugMessageType.StateMessage.GetDescription()}/{scadaHostIdentifier}";
            }

            throw new ArgumentOutOfRangeException(nameof(version));
        }

        /// <summary>
        /// Gets the topic (Except STATE messages).
        /// </summary>
        /// <param name="version">The version.</param>
        /// <param name="nameSpace">The namespace.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="messageType">The message type.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="deviceIdentifier">The device identifier. (Optional)</param>
        /// <returns>The topic as <see cref="string"/>.</returns>
        internal string GetTopic(
            SparkplugVersion version,
            SparkplugNamespace nameSpace,
            string groupIdentifier,
            SparkplugMessageType messageType,
            string edgeNodeIdentifier,
            string? deviceIdentifier)
        {
            if (version is SparkplugVersion.V22)
            {
                return deviceIdentifier is null
                           ? $"{nameSpace.GetDescription()}/{groupIdentifier}/{messageType.GetDescription()}/{edgeNodeIdentifier}"
                           : $"{nameSpace.GetDescription()}/{groupIdentifier}/{messageType.GetDescription()}/{edgeNodeIdentifier}/{deviceIdentifier}";
            }

            throw new ArgumentOutOfRangeException(nameof(version));
        }

        /// <summary>
        /// Gets the STATE message topic.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <param name="scadaHostIdentifier">The SCADA host identifier.</param>
        /// <returns>The STATE message topic as <see cref="string"/>.</returns>
        internal string GetSparkplugStateMessageTopic(
            SparkplugVersion version,
            string scadaHostIdentifier)
        {
            if (version is SparkplugVersion.V22)
            {
                return $"{SparkplugMessageType.StateMessage.GetDescription()}/{scadaHostIdentifier}";
            }

            throw new ArgumentOutOfRangeException(nameof(version));
        }
    }
}