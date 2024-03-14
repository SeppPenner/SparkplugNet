// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MetricTimeTests.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A test class for the metric times.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Tests.Metric;

/// <summary>
/// A test class for the metric times.
/// </summary>
[TestClass] 
public sealed class MetricTimeTests
{
    /// <summary>
    /// Tests the <see cref="VersionBData.MetricTimeValue"/> class with a non UTC <see cref="DateTime"/>s.
    /// </summary>
    /// <param name="year">The year.</param>
    /// <param name="month">The month.</param>
    /// <param name="day">The day.</param>
    /// <param name="hour">The hour.</param>
    /// <param name="minute">The minute.</param>
    /// <param name="second">The second.</param>
    /// <param name="kind">The kind.</param>
    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException), "DateTime value must be in UTC")]
    [DataRow(2021, 1, 1, 0, 0, 0, DateTimeKind.Local)]
    [DataRow(2021, 1, 1, 0, 0, 0, DateTimeKind.Unspecified)]
    public void TestWithNonUtcDateTime(int year, int month, int day, int hour, int minute, int second, DateTimeKind kind)
    {
        var dt = new DateTime(year, month, day, hour, minute, second, kind);
        _ = VersionBData.MetricTimeValue.GetMilliSeconds(dt);
    }

    /// <summary>
    /// Tests the <see cref="VersionBData.MetricTimeValue"/> class with UTC <see cref="DateTime"/>s.
    /// </summary>
    /// <param name="year">The year.</param>
    /// <param name="month">The month.</param>
    /// <param name="day">The day.</param>
    /// <param name="hour">The hour.</param>
    /// <param name="minute">The minute.</param>
    /// <param name="second">The second.</param>
    /// <param name="kind">The kind.</param>
    [TestMethod]
    [DataRow(2021, 1, 1, 0, 0, 0, DateTimeKind.Utc)]
    public void TestWitUtcDateTime(int year, int month, int day, int hour, int minute, int second, DateTimeKind kind)
    {
        var dt = new DateTime(year, month, day, hour, minute, second, kind);
        _ = VersionBData.MetricTimeValue.GetMilliSeconds(dt);
    }

    /// <summary>
    /// Tests the <see cref="VersionBData.MetricTimeValue"/> class with UTC <see cref="DateTimeOffset"/>s.
    /// </summary>
    /// <param name="year">The year.</param>
    /// <param name="month">The month.</param>
    /// <param name="day">The day.</param>
    /// <param name="hour">The hour.</param>
    /// <param name="minute">The minute.</param>
    /// <param name="second">The second.</param>
    /// <param name="offsetSeconds">The offset in seconds.</param>
    [TestMethod]
    [DataRow(2021, 1, 1, 0, 0, 0, 0)]
    public void TestWitUtcDateTimeOffset(int year, int month, int day, int hour, int minute, int second, double offsetSeconds)
    {
        var dt = new DateTimeOffset(year, month, day, hour, minute, second, TimeSpan.FromSeconds(offsetSeconds));
        _ = VersionBData.MetricTimeValue.GetMilliSeconds(dt);
    }

    /// <summary>
    /// Tests the <see cref="VersionBData.MetricTimeValue"/> class with non UTC <see cref="DateTimeOffset"/>s.
    /// </summary>
    /// <param name="year">The year.</param>
    /// <param name="month">The month.</param>
    /// <param name="day">The day.</param>
    /// <param name="hour">The hour.</param>
    /// <param name="minute">The minute.</param>
    /// <param name="second">The second.</param>
    /// <param name="offsetSeconds">The offset in seconds.</param>
    [TestMethod]
    [DataRow(2021, 1, 1, 0, 0, 0, 1)]
    [ExpectedException(typeof(InvalidOperationException), "DateTime value must be in UTC")]
    public void TestWitNonUtcDateTimeOffset(int year, int month, int day, int hour, int minute, int second, double offsetSeconds)
    {
        var dt = new DateTimeOffset(year, month, day, hour, minute, second, TimeSpan.FromMinutes(offsetSeconds));
        _ = VersionBData.MetricTimeValue.GetMilliSeconds(dt);
    }

    /// <summary>
    /// Tests the <see cref="VersionBData.MetricTimeValue"/> class with an out of range value.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException), "Value out of range")]
    public void TestWitOutOfRangeValue()
    {
        var dt = DateTimeOffset.MaxValue.ToUnixTimeMilliseconds() + 1;
        _ = VersionBData.MetricTimeValue.GetMilliSeconds(dt);
    }
}