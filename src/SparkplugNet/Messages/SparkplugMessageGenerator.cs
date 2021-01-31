// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugMessageGenerator.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   The Sparkplug message generator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Messages
{
    using System;

    using MQTTnet;
    using MQTTnet.Protocol;

    using SparkplugNet.Enumerations;
    using SparkplugNet.Extensions;

    /// <summary>
    /// The Sparkplug message generator.
    /// </summary>
    public class SparkplugMessageGenerator
    {
        public MqttApplicationMessage GetSparkplugStateMessage(SparkplugVersion version, string scadaHostIdentifier, bool online)
        {
            if (!scadaHostIdentifier.IsIdentifierValid())
            {
                throw new ArgumentException(nameof(scadaHostIdentifier));
            }

            // Todo: Move to sub methods, check namespace a / b?
            if (version is SparkplugVersion.V22)
            {
                return new MqttApplicationMessageBuilder()
                    .WithTopic($"{SparkplugMessageType.StateMessage.GetDescription()}/{scadaHostIdentifier}")
                    .WithPayload(online ? "ONLINE": "OFFLINE")
                    .WithAtLeastOnceQoS()
                    .WithRetainFlag()
                    .Build();
            }

            throw new ArgumentOutOfRangeException(nameof(version));
        }

        public static MqttApplicationMessage CreateSparkplugMessage(
            SparkplugVersion version,
            SparkplugNamespace nameSpace,
            string groupIdentifier,
            SparkplugMessageType messageType,
            string edgeNodeIdentifier,
            string? deviceIdentifier,
            SparkplugQualityOfServiceLevel qualityOfServiceLevel)
        {
            if (!groupIdentifier.IsIdentifierValid())
            {
                throw new ArgumentException(nameof(groupIdentifier));
            }

            if (!edgeNodeIdentifier.IsIdentifierValid())
            {
                throw new ArgumentException(nameof(edgeNodeIdentifier));
            }

            // ReSharper disable once InvertIf
            if (deviceIdentifier != null)
            {
                if (!deviceIdentifier.IsIdentifierValid())
                {
                    throw new ArgumentException(nameof(deviceIdentifier));
                }
            }

            return nameSpace switch
            {
                SparkplugNamespace.VersionA => GenerateNamespaceAMessage(version, nameSpace, groupIdentifier, messageType, edgeNodeIdentifier, deviceIdentifier, qualityOfServiceLevel),
                SparkplugNamespace.VersionB => GenerateNamespaceBMessage(version, nameSpace, groupIdentifier, messageType, edgeNodeIdentifier, deviceIdentifier, qualityOfServiceLevel),
                _ => throw new ArgumentOutOfRangeException(nameof(nameSpace))
            };
        }

        private static MqttApplicationMessage GenerateNamespaceAMessage(
            SparkplugVersion version,
            SparkplugNamespace nameSpace,
            string groupIdentifier,
            SparkplugMessageType messageType,
            string edgeNodeIdentifier,
            string? deviceIdentifier,
            SparkplugQualityOfServiceLevel qualityOfServiceLevel)
        {
            if (version is SparkplugVersion.V22)
            {
                var topic = GetTopic(nameSpace, groupIdentifier, messageType, edgeNodeIdentifier, deviceIdentifier);

                // Todo: Check payload and retain flag based on message type
                return new MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithPayload("Hello World")
                    .WithQualityOfServiceLevel((MqttQualityOfServiceLevel)qualityOfServiceLevel)
                    .WithRetainFlag()
                    .Build();
            }
            
            throw new ArgumentOutOfRangeException(nameof(version));
        }

        private static MqttApplicationMessage GenerateNamespaceBMessage(
            SparkplugVersion version,
            SparkplugNamespace nameSpace,
            string groupIdentifier,
            SparkplugMessageType messageType,
            string edgeNodeIdentifier,
            string? deviceIdentifier,
            SparkplugQualityOfServiceLevel qualityOfServiceLevel)
        {
            if (version is SparkplugVersion.V22)
            {
                var topic = GetTopic(nameSpace, groupIdentifier, messageType, edgeNodeIdentifier, deviceIdentifier);

                // Todo: Check payload and retain flag based on message type
                return new MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithPayload("Hello World")
                    .WithQualityOfServiceLevel((MqttQualityOfServiceLevel)qualityOfServiceLevel)
                    .WithRetainFlag()
                    .Build();
            }

            throw new ArgumentOutOfRangeException(nameof(version));
        }

        /// <summary>
        /// Gets the SparkPlug topic.
        /// </summary>
        /// <param name="nameSpace">The Sparkplug namespace.</param>
        /// <param name="groupIdentifier">The Sparkplug group identifier.</param>
        /// <param name="messageType">The Sparkplug message type.</param>
        /// <param name="edgeNodeIdentifier">The Sparkplug edge node identifier.</param>
        /// <param name="deviceIdentifier">The Sparkplug device identifier. (Optional)</param>
        /// <returns>The Sparkplug topic as <see cref="string"/>.</returns>
        private static string GetTopic(
            SparkplugNamespace nameSpace,
            string groupIdentifier,
            SparkplugMessageType messageType,
            string edgeNodeIdentifier,
            string? deviceIdentifier)
        {
            return deviceIdentifier is null
                       ? $"{nameSpace.GetDescription()}/{groupIdentifier}/{messageType.GetDescription()}/{edgeNodeIdentifier}"
                       : $"{nameSpace.GetDescription()}/{groupIdentifier}/{messageType.GetDescription()}/{edgeNodeIdentifier}/{deviceIdentifier}";
        }
    }
}