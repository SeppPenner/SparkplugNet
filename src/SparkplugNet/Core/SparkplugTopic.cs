namespace SparkplugNet.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The base sparkplug Topic
    /// </summary>
    public class SparkplugTopic
    {
        /// <summary>
        /// The namespace sparkplug a
        /// </summary>
        public const string NamespaceSparkplugA = "spAv1.0";
        /// <summary>
        /// The namespace sparkplug b
        /// </summary>
        public const string NamespaceSparkplugB = "spBv1.0";

        /// <summary>
        /// Gets the namespace.
        /// </summary>
        /// <value>
        /// The namespace.
        /// </value>
        public SparkplugNamespace Namespace { get; }
        /// <summary>
        /// Gets the group identifier.
        /// </summary>
        /// <value>
        /// The group identifier.
        /// </value>
        public virtual string? GroupIdentifier { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SparkplugTopic"/> class.
        /// </summary>
        /// <param name="namespace">The namespace.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        public SparkplugTopic(SparkplugNamespace @namespace, string? groupIdentifier = null)
        {
            this.Namespace = @namespace;
            this.GroupIdentifier = groupIdentifier;
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(this.GroupIdentifier))
            {
                return string.Concat(GetNamespace(this.Namespace), "/#");
            }
            else
            {
                return string.Concat(GetNamespace(this.Namespace), "/", this.GroupIdentifier);
            }
        }

        /// <summary>
        /// Gets the namespace string.
        /// </summary>
        /// <param name="namespace">The namespace.</param>
        /// <returns></returns>
        /// <exception cref="System.FormatException">Namespace ${@namespace} is unknown!</exception>
        protected static string GetNamespace(SparkplugNamespace @namespace)
        {
            return @namespace switch
            {
                SparkplugNamespace.VersionA => NamespaceSparkplugA,
                SparkplugNamespace.VersionB => NamespaceSparkplugB,
                _ => throw new FormatException($"Namespace ${@namespace} is unknown!"),
            };
        }

        /// <summary>
        /// Tries to get the namespace.
        /// </summary>
        /// <param name="namepsace">The namepsace.</param>
        /// <param name="sparkplugNamespace">The sparkplug namespace.</param>
        /// <returns></returns>
        protected static bool TryGetNamespace(string @namepsace, out SparkplugNamespace sparkplugNamespace)
        {
            switch (@namepsace)
            {
                case NamespaceSparkplugA:
                    sparkplugNamespace = SparkplugNamespace.VersionA;
                    return true;
                case NamespaceSparkplugB:
                    sparkplugNamespace = SparkplugNamespace.VersionB;
                    return true;
                default:
                    sparkplugNamespace = default;
                    return false;
            }
        }
    }

    /// <summary>
    /// SparkplugMessageTopic
    /// </summary>
    /// <seealso cref="SparkplugNet.Core.SparkplugTopic" />
    public class SparkplugMessageTopic : SparkplugTopic
    {
        private static readonly Dictionary<SparkplugMessageType, string> _messageTypes =
            Enum.GetValues(typeof(SparkplugMessageType)).Cast<SparkplugMessageType>().ToDictionary(msg => msg, msg => msg.GetDescription());

        private static readonly Dictionary<string, SparkplugMessageType> _messageTypeFromString = _messageTypes.ToDictionary(x => x.Value, x => x.Key);
        /// <summary>
        /// Gets the group identifier.
        /// </summary>
        /// <value>
        /// The group identifier.
        /// </value>
        public override string GroupIdentifier => base.GroupIdentifier!;
        /// <summary>
        /// Gets the type of the message.
        /// </summary>
        /// <value>
        /// The type of the message.
        /// </value>
        public SparkplugMessageType MessageType { get; }
        /// <summary>
        /// Gets the edge node identifier.
        /// </summary>
        /// <value>
        /// The edge node identifier.
        /// </value>
        public string EdgeNodeIdentifier { get; }
        /// <summary>
        /// Gets the device identifier.
        /// </summary>
        /// <value>
        /// The device identifier.
        /// </value>
        public string? DeviceIdentifier { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SparkplugMessageTopic"/> class.
        /// </summary>
        /// <param name="namespace">The namespace.</param>
        /// <param name="groupIdentifier">The group identifier.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="edgeNodeIdentifier">The edge node identifier.</param>
        /// <param name="deviceIdentifier">The device identifier.</param>
        /// <exception cref="System.ArgumentNullException">
        /// groupIdentifier
        /// or
        /// edgeNodeIdentifier
        /// </exception>
        public SparkplugMessageTopic(SparkplugNamespace @namespace, string groupIdentifier, SparkplugMessageType messageType, string edgeNodeIdentifier, string? deviceIdentifier)
            : base(@namespace, groupIdentifier)
        {
            if (string.IsNullOrEmpty(groupIdentifier))
            {
                throw new ArgumentNullException(nameof(groupIdentifier));
            }

            if (string.IsNullOrEmpty(edgeNodeIdentifier))
            {
                throw new ArgumentNullException(nameof(edgeNodeIdentifier));
            }

            this.MessageType = messageType;
            this.EdgeNodeIdentifier = edgeNodeIdentifier;
            this.DeviceIdentifier = deviceIdentifier;
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(this.DeviceIdentifier))
            {
                return string.Concat(GetNamespace(this.Namespace), "/", this.GroupIdentifier, "/", _messageTypes[this.MessageType], "/", this.EdgeNodeIdentifier);
            }
            else
            {
                return string.Concat(GetNamespace(this.Namespace), "/", this.GroupIdentifier, "/", _messageTypes[this.MessageType], "/", this.EdgeNodeIdentifier, "/", this.DeviceIdentifier);
            }
        }

        /// <summary>
        /// Parses the specified topic.
        /// </summary>
        /// <param name="topic">The topic.</param>
        /// <returns></returns>
        /// <exception cref="System.FormatException">
        /// namespace {topics[0]} is unknown in topic: {topic}!
        /// or
        /// message type {topics[2]} is unknown in topic: {topic}!
        /// or
        /// </exception>
        public static SparkplugMessageTopic? Parse(string topic)
        {
            if (TryParse(topic, out var parsedTopic, true))
            {
                return parsedTopic;
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
        /// <returns></returns>
        public static bool TryParse(string topic, out SparkplugMessageTopic? parsedTopic)
        {
            return TryParse(topic, out parsedTopic, false);
        }
        private static bool TryParse(string topic, out SparkplugMessageTopic? parsedTopic, bool throwError)
        {
            string[] topicsSplitted = topic.Split('/');
            if (topicsSplitted.Length == 4 ||
                topicsSplitted.Length == 5)
            {
                if (!TryGetNamespace(topicsSplitted[0], out var @namespace))
                {
                    if (throwError)
                    {
                        throw new FormatException($"namespace {topicsSplitted[0]} is unknown in topic: {topic}!");
                    }
                }
                else
                {
                    string group = topicsSplitted[1];

                    if (!_messageTypeFromString.TryGetValue(topicsSplitted[2], out var msgType))
                    {
                        if (throwError)
                        {
                            throw new FormatException($"message type {topicsSplitted[2]} is unknown in topic: {topic}!");
                        }
                    }
                    else
                    {

                        string edge = topicsSplitted[3];
                        string? device;
                        if (topicsSplitted.Length == 5)
                        {
                            device = topicsSplitted[4];
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
                throw new FormatException($"{topic} has a invalid format, should consist of 4 or 5 parts!");
            }
            else
            {
                parsedTopic = null;
                return false;
            }
        }
    }
}
