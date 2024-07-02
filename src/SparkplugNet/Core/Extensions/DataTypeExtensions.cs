// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataTypeExtensions.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class that contains data type extension methods.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Core.Extensions;

/// <summary>
/// A class that contains data type extension methods.
/// </summary>
internal static class DataTypeExtensions
{
    /// <summary>
    /// Converts the value to type T.
    /// </summary>
    /// <typeparam name="T">The type parameter.</typeparam>
    /// <param name="objValue">The object value.</param>
    /// <returns>The converted value.</returns>
    internal static T? ConvertTo<T>(this object? objValue)
    {
        if (objValue is null)
        {
            return default;
        }

        // Check whether we need to convert from signed to unsigned.
        var needToConvertFromSignedToUnsigned =
            (objValue is sbyte sbyteVal && sbyteVal < 0) ||
            (objValue is short shortVal && shortVal < 0) ||
            (objValue is int intVal && intVal < 0) ||
            (objValue is long longVal && longVal < 0);

        if (needToConvertFromSignedToUnsigned)
        {
            return unchecked((T)objValue);
        }

        // Check whether we need to convert from unsigned to signed.
        // Check for uint values first.
        if (objValue is uint uintVal)
        {
            var needToConvertFromUnsignedIntToSigned =
                (uintVal > sbyte.MaxValue && typeof(T) == typeof(sbyte)) ||
                (uintVal > short.MaxValue && typeof(T) == typeof(short)) ||
                (uintVal > int.MaxValue && typeof(T) == typeof(int));

            if (needToConvertFromUnsignedIntToSigned)
            {
                var intValue = unchecked((int)uintVal);
                return (T)Convert.ChangeType(intValue, typeof(T));
            }
        }

        // Check for ulong values second.
        if (objValue is ulong ulongVal)
        {
            var needToConvertFromUnsignedLongToSigned =
                (ulongVal > (ulong)sbyte.MaxValue && typeof(T) == typeof(sbyte)) ||
                (ulongVal > (ulong)short.MaxValue && typeof(T) == typeof(short)) ||
                (ulongVal > int.MaxValue && typeof(T) == typeof(int)) ||
                (ulongVal > long.MaxValue && typeof(T) == typeof(long));

            if (needToConvertFromUnsignedLongToSigned)
            {
                var intValue = unchecked((long)ulongVal);
                return (T)Convert.ChangeType(intValue, typeof(T));
            }
        }

        // Regular case.
        return objValue is T valueAsT
            ? valueAsT
            : (T)Convert.ChangeType(objValue, typeof(T));
    }

    /// <summary>
    /// Converts the value to type T or uses a default value.
    /// </summary>
    /// <typeparam name="T">The type parameter.</typeparam>
    /// <param name="objValue">The object value.</param>
    /// <returns>The converted or default value.</returns>
    internal static T ConvertOrDefaultTo<T>(this object? objValue)
    {
        T? value = objValue.ConvertTo<T>();

        return value is not null ? value
            : typeof(T) == typeof(string) ? (T)(object)string.Empty
            : typeof(T) == typeof(byte[]) ? (T)(object)Array.Empty<byte>()
            : typeof(T) == typeof(DataSet) ? (T)(object)new DataSet()
            : typeof(T) == typeof(Template) ? (T)(object)new Template()
            : typeof(T) == typeof(PropertySet) ? (T)(object)new PropertySet()
            : (T)(object)new(); // Last resort
    }
}
