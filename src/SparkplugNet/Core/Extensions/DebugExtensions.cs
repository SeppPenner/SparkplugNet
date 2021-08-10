// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DebugExtensions.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class that contains debug extension methods for any data type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Core.Extensions
{
    using Newtonsoft.Json;

    /// <summary>
    /// A class that contains debug extension methods for any data type.
    /// </summary>
    internal static class DebugExtensions
    {
        internal static string ToJson(this object value)
        {
            return JsonConvert.SerializeObject(value, Formatting.Indented);
        }
    }
}