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

    using SparkplugNet.Enumerations;
    using SparkplugNet.Extensions;

    /// <summary>
    /// The Sparkplug message generator.
    /// </summary>
    internal class SparkplugMessageGenerator
    {
        /// <summary>
        /// The topic generator.
        /// </summary>
        private readonly SparkplugTopicGenerator topicGenerator = new SparkplugTopicGenerator();

        /// <summary>
        /// Gets a STATE message.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <param name="nameSpace">The namespace.</param>
        /// <param name="scadaHostIdentifier">The SCADA host identifier.</param>
        /// <param name="online">A value indicating whether the message sender is online or not.</param>
        /// <returns>A new STATE <see cref="MqttApplicationMessage"/>.</returns>
        internal MqttApplicationMessage GetSparkplugStateMessage(SparkplugVersion version, SparkplugNamespace nameSpace, string scadaHostIdentifier, bool online)
        {
            if (!scadaHostIdentifier.IsIdentifierValid())
            {
                throw new ArgumentException(nameof(scadaHostIdentifier));
            }

            return nameSpace switch
            {
                SparkplugNamespace.VersionA => GetSparkplugStateMessageA(version, scadaHostIdentifier, online),
                SparkplugNamespace.VersionB => GetSparkplugStateMessageB(version, scadaHostIdentifier, online),
                _ => throw new ArgumentOutOfRangeException(nameof(nameSpace))
            };
        }

        /// <summary>
        /// Creates a message (Except STATE messages).
        /// </summary>
        /// <param name="version">The version.</param>
        /// <param name="nameSpace">The namespace.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="messageType">The message type.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="deviceIdentifier">The device identifier.</param>
        /// <returns>A new <see cref="MqttApplicationMessage"/>.</returns>
        internal MqttApplicationMessage CreateSparkplugMessage(
            SparkplugVersion version,
            SparkplugNamespace nameSpace,
            string groupIdentifier,
            SparkplugMessageType messageType,
            string edgeNodeIdentifier,
            string? deviceIdentifier)
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

            if (messageType == SparkplugMessageType.StateMessage)
            {
                throw new InvalidOperationException(nameof(messageType));
            }

            return nameSpace switch
            {
                SparkplugNamespace.VersionA => this.GenerateNamespaceAMessage(version, nameSpace, groupIdentifier, messageType, edgeNodeIdentifier, deviceIdentifier),
                SparkplugNamespace.VersionB => this.GenerateNamespaceBMessage(version, nameSpace, groupIdentifier, messageType, edgeNodeIdentifier, deviceIdentifier),
                _ => throw new ArgumentOutOfRangeException(nameof(nameSpace))
            };
        }

        /// <summary>
        /// Gets a STATE message with namespace version A.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <param name="scadaHostIdentifier">The SCADA host identifier.</param>
        /// <param name="online">A value indicating whether the message sender is online or not.</param>
        /// <returns>A new STATE <see cref="MqttApplicationMessage"/>.</returns>
        private MqttApplicationMessage GetSparkplugStateMessageA(SparkplugVersion version, string scadaHostIdentifier, bool online)
        {
            if (version is SparkplugVersion.V22)
            {
                return new MqttApplicationMessageBuilder()
                    .WithTopic(this.topicGenerator.GetSparkplugStateMessageTopic(version, scadaHostIdentifier))
                    .WithPayload(online ? "ONLINE" : "OFFLINE")
                    .WithAtLeastOnceQoS()
                    .WithRetainFlag()
                    .Build();
            }

            throw new ArgumentOutOfRangeException(nameof(version));
        }

        /// <summary>
        /// Gets a STATE message with namespace version B.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <param name="scadaHostIdentifier">The SCADA host identifier.</param>
        /// <param name="online">A value indicating whether the message sender is online or not.</param>
        /// <returns>A new STATE <see cref="MqttApplicationMessage"/>.</returns>
        private static MqttApplicationMessage GetSparkplugStateMessageB(SparkplugVersion version, string scadaHostIdentifier, bool online)
        {
            if (version is SparkplugVersion.V22)
            {
                // Todo: Add version B payload here.
                throw new InvalidOperationException();
            }

            throw new ArgumentOutOfRangeException(nameof(version));
        }

        /// <summary>
        /// Creates a message (Except STATE messages) with namespace version A.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <param name="nameSpace">The namespace.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="messageType">The message type.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="deviceIdentifier">The device identifier.</param>
        /// <returns>A new <see cref="MqttApplicationMessage"/>.</returns>
        private MqttApplicationMessage GenerateNamespaceAMessage(
            SparkplugVersion version,
            SparkplugNamespace nameSpace,
            string groupIdentifier,
            SparkplugMessageType messageType,
            string edgeNodeIdentifier,
            string? deviceIdentifier)
        {
            if (version is SparkplugVersion.V22)
            {
                var topic = this.topicGenerator.GetTopic(version, nameSpace, groupIdentifier, messageType, edgeNodeIdentifier, deviceIdentifier);

                var payload = this.GetPayloadNamespaceA(messageType);

                return new MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithPayload(payload)
                    .WithAtLeastOnceQoS()
                    .WithRetainFlag()
                    .Build();
            }

            throw new ArgumentOutOfRangeException(nameof(version));
        }

        /// <summary>
        /// Creates a message (Except STATE messages) with namespace version B.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <param name="nameSpace">The namespace.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="messageType">The message type.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="deviceIdentifier">The device identifier.</param>
        /// <returns>A new <see cref="MqttApplicationMessage"/>.</returns>
        private MqttApplicationMessage GenerateNamespaceBMessage(
            SparkplugVersion version,
            SparkplugNamespace nameSpace,
            string groupIdentifier,
            SparkplugMessageType messageType,
            string edgeNodeIdentifier,
            string? deviceIdentifier)
        {
            if (version is SparkplugVersion.V22)
            {
                var topic = this.topicGenerator.GetTopic(version, nameSpace, groupIdentifier, messageType, edgeNodeIdentifier, deviceIdentifier);

                var payload = this.GetPayloadNamespaceB(messageType);

                return new MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithPayload(payload)
                    .WithAtLeastOnceQoS()
                    .WithRetainFlag()
                    .Build();
            }

            throw new ArgumentOutOfRangeException(nameof(version));
        }

        /// <summary>
        /// Gets the payload for a namespace A message.
        /// </summary>
        /// <param name="messageType">The message type.</param>
        /// <returns>The payload <see cref="string"/> for a namespace A message.</returns>
        private string GetPayloadNamespaceA(SparkplugMessageType messageType)
        {
            // Todo: Check payload
            return messageType switch
            {
                SparkplugMessageType.NodeBirth => "ONLINE",
                SparkplugMessageType.NodeDeath => "OFFLINE",
                SparkplugMessageType.DeviceBirth => "ONLINE",
                SparkplugMessageType.DeviceDeath => "OFFLINE",
                SparkplugMessageType.NodeData => string.Empty,
                SparkplugMessageType.DeviceData => string.Empty,
                SparkplugMessageType.NodeCommand => string.Empty,
                SparkplugMessageType.DeviceCommand => string.Empty,
                SparkplugMessageType.StateMessage => throw new InvalidOperationException(nameof(SparkplugMessageType.StateMessage)),
                _ => throw new ArgumentOutOfRangeException(nameof(messageType))
            };
        }

        /// <summary>
        /// Gets the payload for a namespace B message.
        /// </summary>
        /// <param name="messageType">The message type.</param>
        /// <returns>The payload <see cref="string"/> for a namespace A message.</returns>
        private string GetPayloadNamespaceB(SparkplugMessageType messageType)
        {
            // Todo: Check payload
            return messageType switch
            {
                SparkplugMessageType.NodeBirth => "ONLINE",
                SparkplugMessageType.NodeDeath => "OFFLINE",
                SparkplugMessageType.DeviceBirth => "ONLINE",
                SparkplugMessageType.DeviceDeath => "OFFLINE",
                SparkplugMessageType.NodeData => string.Empty,
                SparkplugMessageType.DeviceData => string.Empty,
                SparkplugMessageType.NodeCommand => string.Empty,
                SparkplugMessageType.DeviceCommand => string.Empty,
                SparkplugMessageType.StateMessage => throw new InvalidOperationException(nameof(SparkplugMessageType.StateMessage)),
                _ => throw new ArgumentOutOfRangeException(nameof(messageType))
            };
        }
    }
}