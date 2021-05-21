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
    using System;
    using System.Diagnostics;
    using Newtonsoft.Json;

    /// <summary>
    /// A class that contains debug extension methods for any data type.
    /// </summary>
    internal static class DebugExtensions
    {
        internal static void ToOutputWindowJson(this object value, string logTitle)
        {
            Debug.WriteLine($"{DateTime.Now} - {logTitle}:");
            Debug.WriteLine(JsonConvert.SerializeObject(value, Formatting.Indented));
            Debug.WriteLine("\n");
        }
    }
}