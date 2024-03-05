namespace SparkplugNet.Tests.Metric;

using System.Runtime.CompilerServices;

/// <summary>
/// 
/// </summary>
[TestClass] 
public class MetricTimeTests
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="year"></param>
    /// <param name="month"></param>
    /// <param name="day"></param>
    /// <param name="hour"></param>
    /// <param name="minute"></param>
    /// <param name="second"></param>
    /// <param name="kind"></param>
    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException),"DateTime value must be in UTC")]
    [DataRow(2021, 1, 1, 0, 0, 0, DateTimeKind.Local)]
    [DataRow(2021, 1, 1, 0, 0, 0, DateTimeKind.Unspecified)]
    public void TestWithNonUtcDateTime(int year, int month, int day, int hour, int minute, int second, DateTimeKind kind)
    {
        var dt = new DateTime(year, month, day, hour, minute, second, kind);
        var metricTimeValue = SparkplugNet.VersionB.Data.MetricTimeValue.GetMilliSeconds(dt);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="year"></param>
    /// <param name="month"></param>
    /// <param name="day"></param>
    /// <param name="hour"></param>
    /// <param name="minute"></param>
    /// <param name="second"></param>
    /// <param name="kind"></param>
    [TestMethod]
    [DataRow(2021, 1, 1, 0, 0, 0, DateTimeKind.Utc)]
    public void TestWitUtcDateTime(int year, int month, int day, int hour, int minute, int second, DateTimeKind kind)
    {
        var dt = new DateTime(year, month, day, hour, minute, second, kind);
        var metricTimeValue = SparkplugNet.VersionB.Data.MetricTimeValue.GetMilliSeconds(dt);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="year"></param>
    /// <param name="month"></param>
    /// <param name="day"></param>
    /// <param name="hour"></param>
    /// <param name="minute"></param>
    /// <param name="second"></param>
    /// <param name="offsetSeconds"></param>
    [TestMethod]
    [DataRow(2021, 1, 1, 0, 0, 0, 0)]
    public void TestWitUtcDateTimeOffset(int year, int month, int day, int hour, int minute, int second, double offsetSeconds)
    {
        var dt = new DateTimeOffset(year, month, day, hour, minute, second, TimeSpan.FromSeconds(offsetSeconds));
        var metricTimeValue = SparkplugNet.VersionB.Data.MetricTimeValue.GetMilliSeconds(dt);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="year"></param>
    /// <param name="month"></param>
    /// <param name="day"></param>
    /// <param name="hour"></param>
    /// <param name="minute"></param>
    /// <param name="second"></param>
    /// <param name="offsetSeconds"></param>
    [TestMethod]
    [DataRow(2021, 1, 1, 0, 0, 0, 1)]
    [ExpectedException(typeof(InvalidOperationException),"DateTime value must be in UTC")]
    public void TestWitNonUtcDateTimeOffset(int year, int month, int day, int hour, int minute, int second, double offsetSeconds)
    {
        var dt = new DateTimeOffset(year, month, day, hour, minute, second, TimeSpan.FromMinutes(offsetSeconds));
        var metricTimeValue = SparkplugNet.VersionB.Data.MetricTimeValue.GetMilliSeconds(dt);
    }
    /// <summary>
    /// 
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException),"Value out of range")]
    public void TestWitOutOfRangeValue()
    {
        var dt = DateTimeOffset.MaxValue.ToUnixTimeMilliseconds() + 1;
        var metricTimeValue = SparkplugNet.VersionB.Data.MetricTimeValue.GetMilliSeconds(dt);
    }
}