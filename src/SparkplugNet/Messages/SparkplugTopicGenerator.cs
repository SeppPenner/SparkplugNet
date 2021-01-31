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
        /// Gets the subscription topic for an application.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <param name="nameSpace">The namespace.</param>
        /// <returns>The subscription topic <see cref="string"/> for an application.</returns>
        internal string GetApplicationSubscribeTopic(SparkplugVersion version, SparkplugNamespace nameSpace)
        {
            if (version is SparkplugVersion.V22)
            {
                return $"{nameSpace.GetDescription()}/#";
            }

            throw new ArgumentOutOfRangeException(nameof(version));
        }

        /// <summary>
        /// Gets the SparkPlug topic (Except STATE messages).
        /// </summary>
        /// <param name="version">The version.</param>
        /// <param name="nameSpace">The Sparkplug namespace.</param>
        /// <param name="groupIdentifier">The Sparkplug group identifier.</param>
        /// <param name="messageType">The Sparkplug message type.</param>
        /// <param name="edgeNodeIdentifier">The Sparkplug edge node identifier.</param>
        /// <param name="deviceIdentifier">The Sparkplug device identifier. (Optional)</param>
        /// <returns>The Sparkplug topic as <see cref="string"/>.</returns>
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
        /// Gets the Sparkplug STATE message topic.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <param name="scadaHostIdentifier">The SCADA host identifier.</param>
        /// <returns>The Sparkplug STATE message topic as <see cref="string"/>.</returns>
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