namespace SparkplugNet.VersionB.Data;

using System.Runtime.CompilerServices;

/// <summary>
/// Class to store metric time values. and perform operations on them.
/// </summary>
public static class MetricTimeValue
{
    /// <summary>
    /// Return the UTC time in milliseconds Since Unix Epoch
    /// </summary>
    /// <param name="value">UTC Time</param>
    /// <exception cref="InvalidOperationException"></exception>
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
            
            case ulong :
            case long:
                long longValue = value.ConvertTo<long>();
                if (longValue> DateTimeOffset.MaxValue.ToUnixTimeMilliseconds() 
                    || (longValue < DateTimeOffset.MinValue.ToUnixTimeMilliseconds()))
                {
                    throw new InvalidOperationException("Value out of range");
                }
                return (ulong) longValue;
            default:
                throw new InvalidOperationException($"Value {value} is not a valid time value");
        }
    }
    
    /// <summary>
    /// Returns the DateTimeOffset date time value
    /// </summary>
    /// <returns>DateTimeOffset</returns>
    public static DateTimeOffset GetDateTimeOffset(ulong value)
    {
        return DateTimeOffset.FromUnixTimeMilliseconds((long)GetMilliSeconds(value));
    }
}

