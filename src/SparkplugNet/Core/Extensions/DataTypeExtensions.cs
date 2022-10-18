namespace SparkplugNet.Core.Extensions
{
    using SparkplugNet.VersionB.Data;

    /// <summary>
    /// A class that contains data type extension methods.
    /// </summary>
    internal static class DataTypeExtensions
    {
        /// <summary>
        /// Converts the value to type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objValue">The object value.</param>
        /// <returns>The converted value.</returns>
        public static T? ConvertTo<T>(this object? objValue)
        {
            if (objValue == null)
                return default;

            return objValue is T valueAsT
                ? valueAsT
                : (T)Convert.ChangeType(objValue, typeof(T));
        }

        /// <summary>
        /// Converts the value to type T or uses a default value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objValue"></param>
        /// <returns>The converted or default value.</returns>
        public static T ConvertOrDefaultTo<T>(this object? objValue)
        {
            T? value = objValue.ConvertTo<T>();

            return value != null ? value
                : typeof(T) == typeof(string) ? (T)(object)string.Empty
                : typeof(T) == typeof(byte[]) ? (T)(object)Array.Empty<byte>()
                : typeof(T) == typeof(DataSet) ? (T)(object)new DataSet()
                : typeof(T) == typeof(Template) ? (T)(object)new Template()
                : typeof(T) == typeof(PropertySet) ? (T)(object)new PropertySet()
                : (T)(object)new(); // Last Resort
        }
    }
}
