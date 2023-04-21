// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugGlobals.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class that contains global settings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Core;

/// <summary>
///  A class that contains global settings.
/// </summary>
public static class SparkplugGlobals
{
    /// <summary>
    /// Gets or sets a value indicating whether strict identifier checking is enabled or not.
    /// This prevents users from using the following chars in identifiers like <code>.,\@#$%^&amp;*()[]{}|!`~:;'"&lt;&gt;?</code>
    /// </summary>
    public static bool UseStrictIdentifierChecking { get; set; }
}
