// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugMessageTopic.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   The Sparkplug class for the message topic.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Core.Topics;

using System.Diagnostics.CodeAnalysis;

/// <inheritdoc cref="SparkplugTopic"/>
/// <summary>
/// The Sparkplug class for the message topic.
/// </summary>
/// <seealso cref="SparkplugTopic"/>
public class SparkplugMessageTopic : SparkplugTopic
{
    /// <summary>
    /// The message types.
    /// </summary>
    private static readonly Dictionary<SparkplugMessageType, string> messageTypes =
        Enum.GetValues(typeof(SparkplugMessageType)).Cast<SparkplugMessageType>().ToDictionary(msg => msg, msg => msg.GetDescription());

    /// <summary>
    /// The messages types as string.
    /// </summary>
    private static readonly Dictionary<string, SparkplugMessageType> messageTypeFromString = messageTypes.ToDictionary(x => x.Value, x => x.Key);

    /// <inheritdoc cref="SparkplugTopic"/>
    /// <summary>
    /// Initializes a new instance of the <see cref="SparkplugMessageTopic"/> class.
    /// </summary>
    /// <param name="namespace">The namespace.</param>
    /// <param name="groupIdentifier">The group identifier.</param>
    /// <param name="messageType">Type of the message.</param>
    /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
    /// <param name="deviceIdentifier">The device identifier.</param>
    /// <exception cref="ArgumentNullException">Thrown if the group or edge node identifier is null or empty.</exception>
    /// <seealso cref="SparkplugTopic"/>
    public SparkplugMessageTopic(SparkplugNamespace @namespace, string groupIdentifier, SparkplugMessageType messageType, string edgeNodeIdentifier, string? deviceIdentifier)
        : base(@namespace, groupIdentifier)
    {
        if (string.IsNullOrWhiteSpace(groupIdentifier))
        {
            throw new ArgumentNullException(nameof(groupIdentifier));
        }

        if (string.IsNullOrWhiteSpace(edgeNodeIdentifier))
        {
            throw new ArgumentNullException(nameof(edgeNodeIdentifier));
        }

        this.MessageType = messageType;
        this.EdgeNodeIdentifier = edgeNodeIdentifier;
        this.DeviceIdentifier = deviceIdentifier;
    }

    /// <summary>
    /// Gets the group identifier.
    /// </summary>
    public override string GroupIdentifier => base.GroupIdentifier!;

    /// <summary>
    /// Gets the type of the message.
    /// </summary>
    public SparkplugMessageType MessageType { get; }

    /// <summary>
    /// Gets the edge node identifier.
    /// </summary>
    public string EdgeNodeIdentifier { get; }

    /// <summary>
    /// Gets the device identifier.
    /// </summary>
    public string? DeviceIdentifier { get; }

    /// <inheritdoc cref="object"/>
    public override string ToString()
    {
        if (string.IsNullOrWhiteSpace(this.DeviceIdentifier))
        {
            return string.Concat(GetNamespace(this.Namespace), "/", this.GroupIdentifier, "/", messageTypes[this.MessageType], "/", this.EdgeNodeIdentifier);
        }
        else
        {
            return string.Concat(GetNamespace(this.Namespace), "/", this.GroupIdentifier, "/", messageTypes[this.MessageType], "/", this.EdgeNodeIdentifier, "/", this.DeviceIdentifier);
        }
    }

    /// <summary>
    /// Parses the specified topic.
    /// </summary>
    /// <param name="topic">The topic.</param>
    /// <returns>The message topic.</returns>
    /// <exception cref="FormatException">Thrown if the topic couldn't be parsed.</exception>
    public static SparkplugMessageTopic Parse(string topic)
    {
        if (TryParse(topic, out var parsedTopic, true))
        {
#if NETSTANDARD2_1_OR_GREATER
            return parsedTopic;
#else
            return parsedTopic!;
#endif
        }
        else
        {
            throw new FormatException($"{topic} could not be parsed");
        }
    }

    /// <summary>
    /// Tries to parse the topic.
    /// </summary>
    /// <param name="topic">The topic.</param>
    /// <param name="parsedTopic">The parsed topic.</param>
    /// <returns>A value indicating whether the topic can be parsed or not.</returns>
#if NETSTANDARD2_1_OR_GREATER
    public static bool TryParse(string topic, [NotNullWhen(true)] out SparkplugMessageTopic? parsedTopic)
#else
    public static bool TryParse(string topic, out SparkplugMessageTopic? parsedTopic)
#endif
    {
        return TryParse(topic, out parsedTopic, false);
    }

    /// <summary>
    /// Tries to parse the topic.
    /// </summary>
    /// <param name="topic">The topic.</param>
    /// <param name="parsedTopic">The parsed topic.</param>
    /// <param name="throwError">A value indicating whether errors should be thrown.</param>
    /// <returns>A value indicating whether the topic can be parsed or not.</returns>
    /// <exception cref="FormatException">Thrown if the namespace, message type or topic couldn't be parsed.</exception>
    private static bool TryParse(string topic, out SparkplugMessageTopic? parsedTopic, bool throwError)
    {
        string[] splitTopics = topic.Split('/');

        if (splitTopics.Length == 4 || splitTopics.Length == 5)
        {
            if (!TryGetNamespace(splitTopics[0], out var @namespace))
            {
                if (throwError)
                {
                    throw new FormatException($"The namespace {splitTopics[0]} is unknown in topic: {topic}.");
                }
            }
            else
            {
                string group = splitTopics[1];

                if (!messageTypeFromString.TryGetValue(splitTopics[2], out var msgType))
                {
                    if (throwError)
                    {
                        throw new FormatException($"The message type {splitTopics[2]} is unknown in topic: {topic}.");
                    }
                }
                else
                {
                    string edge = splitTopics[3];
                    string? device;

                    if (splitTopics.Length == 5)
                    {
                        device = splitTopics[4];
                    }
                    else
                    {
                        device = null;
                    }

                    parsedTopic = new SparkplugMessageTopic(@namespace, group, msgType, edge, device);
                    return true;
                }
            }
        }

        if (throwError)
        {
            throw new FormatException($"The topic {topic} has an invalid format, it should consist of 4 or 5 parts.");
        }
        else
        {
            parsedTopic = null;
            return false;
        }
    }
}