namespace SparkplugNet.Core.Extensions
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Linq extentions
    /// </summary>
    public static class EnumerableExtentions
    {
        /// <summary>
        /// Converts to nonnulllist.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        public static List<T> ToNonNullList<T>(this IEnumerable<T?> target) => target.Where(t => t != null).Select(t => t!).ToList();
    }
}
