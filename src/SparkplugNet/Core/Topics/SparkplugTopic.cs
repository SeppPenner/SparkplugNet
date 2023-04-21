// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugTopic.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   The Sparkplug class for the topic.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Core.Topics;

/// <summary>
/// The Sparkplug class for the topic.
/// </summary>
public class SparkplugTopic
{
    /// <summary>
    /// The namespace for Sparkplug B.
    /// </summary>
    public const string NamespaceSparkplugB = "spBv1.0";

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
    /// Gets the namespace.
    /// </summary>
    public SparkplugNamespace Namespace { get; }

    /// <summary>
    /// Gets the group identifier.
    /// </summary>
    public virtual string? GroupIdentifier { get; }

    /// <inheritdoc cref="object"/>
    public override string ToString()
    {
        if (string.IsNullOrWhiteSpace(this.GroupIdentifier))
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
    /// <returns>The namespace string.</returns>
    /// <exception cref="FormatException">Thrown if the namespace is unknown.</exception>
    protected static string GetNamespace(SparkplugNamespace @namespace)
    {
        return @namespace switch
        {
            SparkplugNamespace.VersionB => NamespaceSparkplugB,
            _ => throw new FormatException($"Namespace ${@namespace} is unknown!"),
        };
    }

    /// <summary>
    /// Tries to get the namespace.
    /// </summary>
    /// <param name="namespace">The namespace.</param>
    /// <param name="sparkplugNamespace">The Sparkplug namespace.</param>
    /// <returns>A value indicating whether the namespace was valid or not.</returns>
    protected static bool TryGetNamespace(string @namespace, out SparkplugNamespace sparkplugNamespace)
    {
        switch (@namespace)
        {
            case NamespaceSparkplugB:
                sparkplugNamespace = SparkplugNamespace.VersionB;
                return true;
            default:
                sparkplugNamespace = default;
                return false;
        }
    }
}
