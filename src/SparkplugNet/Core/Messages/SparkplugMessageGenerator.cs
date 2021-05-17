// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugMessageGenerator.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   The Sparkplug message generator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Core.Messages
{
    using System;

    using MQTTnet;

    using SparkplugNet.Core.Enumerations;
    using SparkplugNet.Core.Extensions;

    /// <summary>
    /// The Sparkplug message generator.
    /// </summary>
    public class SparkplugMessageGenerator
    {
        /// <summary>
        /// The topic generator.
        /// </summary>
        private readonly SparkplugTopicGenerator topicGenerator = new ();

        /// <summary>
        /// Gets a STATE message.
        /// </summary>
        /// <param name="nameSpace">The namespace.</param>
        /// <param name="scadaHostIdentifier">The SCADA host identifier.</param>
        /// <param name="online">A value indicating whether the message sender is online or not.</param>
        /// <returns>A new STATE <see cref="MqttApplicationMessage"/>.</returns>
        public MqttApplicationMessage GetSparkplugStateMessage(SparkplugNamespace nameSpace, string scadaHostIdentifier, bool online)
        {
            if (!scadaHostIdentifier.IsIdentifierValid())
            {
                throw new ArgumentException(nameof(scadaHostIdentifier));
            }

            return nameSpace switch
            {
                SparkplugNamespace.VersionA => this.GetSparkplugStateMessageA(scadaHostIdentifier, online),
                SparkplugNamespace.VersionB => this.GetSparkplugStateMessageB(scadaHostIdentifier, online),
                _ => throw new ArgumentOutOfRangeException(nameof(nameSpace))
            };
        }

        /// <summary>
        /// Creates a message (Except STATE messages).
        /// </summary>
        /// <param name="nameSpace">The namespace.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="messageType">The message type.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="deviceIdentifier">The device identifier.</param>
        /// <returns>A new <see cref="MqttApplicationMessage"/>.</returns>
        public MqttApplicationMessage CreateSparkplugMessage(
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
                SparkplugNamespace.VersionA => this.GenerateNamespaceAMessage(nameSpace, groupIdentifier, messageType, edgeNodeIdentifier, deviceIdentifier),
                SparkplugNamespace.VersionB => this.GenerateNamespaceBMessage(nameSpace, groupIdentifier, messageType, edgeNodeIdentifier, deviceIdentifier),
                _ => throw new ArgumentOutOfRangeException(nameof(nameSpace))
            };
        }

        /// <summary>
        /// Gets a STATE message with namespace version A.
        /// </summary>
        /// <param name="scadaHostIdentifier">The SCADA host identifier.</param>
        /// <param name="online">A value indicating whether the message sender is online or not.</param>
        /// <returns>A new STATE <see cref="MqttApplicationMessage"/>.</returns>
        private MqttApplicationMessage GetSparkplugStateMessageA(string scadaHostIdentifier, bool online)
        {
            return new MqttApplicationMessageBuilder()
                .WithTopic(this.topicGenerator.GetSparkplugStateMessageTopic(scadaHostIdentifier))
                .WithPayload(online ? "ONLINE" : "OFFLINE")
                .WithAtLeastOnceQoS()
                .WithRetainFlag()
                .Build();
        }

        /// <summary>
        /// Gets a STATE message with namespace version B.
        /// </summary>
        /// <param name="scadaHostIdentifier">The SCADA host identifier.</param>
        /// <param name="online">A value indicating whether the message sender is online or not.</param>
        /// <returns>A new STATE <see cref="MqttApplicationMessage"/>.</returns>
        private MqttApplicationMessage GetSparkplugStateMessageB(string scadaHostIdentifier, bool online)
        {
            return new MqttApplicationMessageBuilder()
                .WithTopic(this.topicGenerator.GetSparkplugStateMessageTopic(scadaHostIdentifier))
                .WithPayload(online ? "ONLINE" : "OFFLINE")
                .WithAtLeastOnceQoS()
                .WithRetainFlag()
                .Build();
        }

        /// <summary>
        /// Creates a message (Except STATE messages) with namespace version A.
        /// </summary>
        /// <param name="nameSpace">The namespace.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="messageType">The message type.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="deviceIdentifier">The device identifier.</param>
        /// <returns>A new <see cref="MqttApplicationMessage"/>.</returns>
        private MqttApplicationMessage GenerateNamespaceAMessage(
            SparkplugNamespace nameSpace,
            string groupIdentifier,
            SparkplugMessageType messageType,
            string edgeNodeIdentifier,
            string? deviceIdentifier)
        {
            var topic = this.topicGenerator.GetTopic(nameSpace, groupIdentifier, messageType, edgeNodeIdentifier, deviceIdentifier);

            var payload = this.GetPayloadNamespaceA(messageType);

            return new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .WithAtLeastOnceQoS()
                .WithRetainFlag()
                .Build();
        }

        /// <summary>
        /// Creates a message (Except STATE messages) with namespace version B.
        /// </summary>
        /// <param name="nameSpace">The namespace.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="messageType">The message type.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="deviceIdentifier">The device identifier.</param>
        /// <returns>A new <see cref="MqttApplicationMessage"/>.</returns>
        private MqttApplicationMessage GenerateNamespaceBMessage(
            SparkplugNamespace nameSpace,
            string groupIdentifier,
            SparkplugMessageType messageType,
            string edgeNodeIdentifier,
            string? deviceIdentifier)
        {
            var topic = this.topicGenerator.GetTopic(nameSpace, groupIdentifier, messageType, edgeNodeIdentifier, deviceIdentifier);

            var payload = this.GetPayloadNamespaceB(messageType);

            return new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .WithAtLeastOnceQoS()
                .WithRetainFlag()
                .Build();
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