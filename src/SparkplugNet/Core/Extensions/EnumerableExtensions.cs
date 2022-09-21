// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableExtensions.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class that contains enumerable extension methods.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Core.Extensions;

/// <summary>
/// A class that contains enumerable extension methods.
/// </summary>
internal static class EnumerableExtensions
{
    /// <summary>
    /// Converts an <see cref="IEnumerable{T}"/> to a not null list.
    /// </summary>
    /// <typeparam name="T">The type parameter.</typeparam>
    /// <param name="target">The target.</param>
    /// <returns>A not null <see cref="List{T}"/>.</returns>
    internal static List<T> ToNonNullList<T>(this IEnumerable<T?> target) => target.Where(t => t is not null).Select(t => t!).ToList();
}
