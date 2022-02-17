// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugBase.cs" company="Hämmer Electronics">
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
    public static byte[]? Serialize<T>(T? record) where T : class
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
    public static T? Deserialize<T>(byte[]? data) where T : class
    {
        if (null == data)
        {
            return null;
        }

        using var stream = new MemoryStream(data);
        return Serializer.Deserialize<T>(stream);
    }
}
