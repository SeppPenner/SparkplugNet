// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PayloadHelper.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A helper class for the payload classes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("SparkplugNet.Tests")]
namespace SparkplugNet.Core;

/// <summary>
/// A helper class for the payload classes.
/// </summary>
internal static class PayloadHelper
{
    /// <summary>
    /// Serializes the data from a proto payload.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="record">The record.</param>
    /// <returns>The <see cref="T:byte[]?"/> value as serialized data.</returns>
    internal static byte[]? Serialize<T>(T? record) where T : class
    {
        if (record is null)
        {
            return null;
        }

        using var stream = new MemoryStream();
        Serializer.Serialize(stream, record);
        return stream.ToArray();
    }

    /// <summary>
    /// Deserializes the data to a proto payload.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="data">The data.</param>
    /// <returns>The <see cref="T:T?"/> value as deserialized object.</returns>
    internal static T? Deserialize<T>(byte[]? data) where T : class
    {
        if (data is null)
        {
            return null;
        }

        using var stream = new MemoryStream(data);
        return Serializer.Deserialize<T>(stream);
    }

    // Todo: Adjust to callback via action / Func?
    /// <summary>
    /// The write bytes delegate.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="span">The data.</param>
    /// <param name="value">The value.</param>
    internal delegate void WriteBytes<T>(Span<byte> span, T value);

    /// <summary>
    /// Gets the bytes from an array.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="items">The items.</param>
    /// <param name="writeBytes">A function to write the bytes.</param>
    /// <returns>The byte array.</returns>
    internal static byte[] GetBytesFromArray<T>(T[] items, WriteBytes<T> writeBytes)
    {
        var itemSize = Marshal.SizeOf(typeof(T));
        var bytes = new byte[itemSize * items.Length];

        for (var i = 0; i < items.Length; i++)
        {
            writeBytes(bytes.AsSpan(i * itemSize), items[i]);
        };

        return bytes;
    }

    // Todo: Adjust to callback via action / Func?
    /// <summary>
    /// The read bytes delegate.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="span">The data.</param>
    /// <returns>The written data.</returns>
    internal delegate T ReadBytes<T>(ReadOnlySpan<byte> span);

    /// <summary>
    /// Gets an array of T from bytes.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="bytes">The bytes.</param>
    /// <param name="readBytes">A function to read the bytes.</param>
    /// <returns>An array of type <c>T</c>.</returns>
    /// <exception cref="ArgumentException">Thrown if the size of the array is invalid.</exception>
    internal static T[] GetArrayOfTFromBytes<T>(ReadOnlySpan<byte> bytes, ReadBytes<T> readBytes)
    {
        var itemSize = Marshal.SizeOf(typeof(T));

        if (bytes.Length % itemSize != 0)
        {
            throw new ArgumentException("The byte array is not a multiple of the item size");
        }

        var numberOfItems = bytes.Length / itemSize;
        var items = new T[numberOfItems];

        for (var i = 0; i < items.Length; i++)
        {
            items[i] = readBytes(bytes.Slice(i * itemSize, itemSize));
        };

        return items;
    }
}
