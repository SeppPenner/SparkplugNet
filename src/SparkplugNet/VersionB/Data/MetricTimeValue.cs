// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MetricTimeValue.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class to store metric time values and perform operations on them.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.VersionB.Data;

/// <summary>
/// A class to store metric time values and perform operations on them.
/// </summary>
public static class MetricTimeValue
{
    /// <summary>
    /// Gets the UTC time in milliseconds since the unix epoch.
    /// </summary>
    /// <param name="value">The UTC time.</param>
    /// <exception cref="InvalidOperationException">Thrown if the value is in UTC or out of range.</exception>
    /// <returns>The milliseconds.</returns>
    public static ulong GetMilliSeconds(object value)
    {
        switch (value)
        {
            case DateTime dateTime:
                return (dateTime.Kind != DateTimeKind.Utc)
                    ? throw new InvalidOperationException("DateTime value must be in UTC")
                    : (ulong)new DateTimeOffset(dateTime).ToUnixTimeMilliseconds();

            case DateTimeOffset dateTimeOffset:
                return (dateTimeOffset.Offset != TimeSpan.Zero)
                    ? throw new InvalidOperationException("DateTime value must be in UTC")
                    : (ulong)dateTimeOffset.ToUnixTimeMilliseconds();

            case ulong:
            case long:
                var longValue = value.ConvertTo<long>();

                if (longValue > DateTimeOffset.MaxValue.ToUnixTimeMilliseconds()
                    || (longValue < DateTimeOffset.MinValue.ToUnixTimeMilliseconds()))
                {
                    throw new InvalidOperationException("Value out of range");
                }

                return (ulong)longValue;
            default:
                throw new InvalidOperationException($"Value {value} is not a valid time value");
        }
    }

    /// <summary>
    /// Gets the <see cref="DateTimeOffset"/> from the long value.
    /// </summary>
    /// <returns>The <see cref="DateTimeOffset"/> value.</returns>
    public static DateTimeOffset GetDateTimeOffset(ulong value)
    {
        return DateTimeOffset.FromUnixTimeMilliseconds((long)GetMilliSeconds(value));
    }
}
