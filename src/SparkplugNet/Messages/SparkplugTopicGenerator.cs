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
    public class SparkplugTopicGenerator
    {
        /// <summary>
        /// Gets the subscription topic for an application.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <param name="nameSpace">The namespace.</param>
        /// <returns>The subscription topic <see cref="string"/> for an application.</returns>
        public string GetApplicationSubscribeTopic(SparkplugVersion version, SparkplugNamespace nameSpace)
        {
            if (version is SparkplugVersion.V22)
            {
                return $"{nameSpace.GetDescription()}/#";
            }

            throw new ArgumentOutOfRangeException(nameof(version));
        }
    }
}