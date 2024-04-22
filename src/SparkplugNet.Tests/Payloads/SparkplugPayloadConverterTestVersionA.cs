// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugPayloadConverterTestVersionA.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class to test the <see cref="VersionA.PayloadConverter"/> class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Tests.Payloads;

/// <summary>
/// A class to test the <see cref="VersionAMain.PayloadConverter"/> class.
/// </summary>
[TestClass]
public sealed class SparkplugPayloadConverterTestVersionA
{
    /// <summary>
    /// Tests the Sparkplug payload converter for converting a version A payload from Proto.
    /// </summary>
    [TestMethod]
    public void TestConvertVersionAPayloadFromProto()
    {
        var dateTime = new DateTimeOffset(2021, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var dateTime2 = new DateTimeOffset(2022, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var bodyData = new byte[] { 1, 2, 3, 4 };
        var metrics = new List<VersionAProtoBufPayload.KuraMetric>
        {
            new()
            {
                Name = "Test1",
                DataType = VersionAProtoBufPayload.KuraMetric.ValueType.Double,
                DoubleValue = 1.0
            },
            new()
            {
                Name = "Test2",
                DataType = VersionAProtoBufPayload.KuraMetric.ValueType.Float,
                FloatValue = 2.0f
            },
            new()
            {
                Name = "Test3",
                DataType = VersionAProtoBufPayload.KuraMetric.ValueType.Int64,
                LongValue = 3
            },
            new()
            {
                Name = "Test4",
                DataType = VersionAProtoBufPayload.KuraMetric.ValueType.Int32,
                IntValue = 4
            },
            new()
            {
                Name = "Test5",
                DataType = VersionAProtoBufPayload.KuraMetric.ValueType.Bool,
                BooleanValue = true
            },
            new()
            {
                Name = "Test6",
                DataType = VersionAProtoBufPayload.KuraMetric.ValueType.String,
                StringValue = "6"
            },
            new()
            {
                Name = "Test7",
                DataType = VersionAProtoBufPayload.KuraMetric.ValueType.Bytes,
                BytesValue = [7, 8, 9, 10]
            }
        };
        var convertedMetrics = new List<VersionAData.KuraMetric>
        {
            new("Test1", VersionAData.DataType.Double, 1.0),
            new("Test2", VersionAData.DataType.Float, 2.0f),
            new("Test3", VersionAData.DataType.Int64, 3),
            new("Test4", VersionAData.DataType.Int32, 4),
            new("Test5", VersionAData.DataType.Boolean, true),
            new("Test6", VersionAData.DataType.String, "6"),
            new("Test7", VersionAData.DataType.Bytes, new byte[] { 7, 8, 9, 10 })
        };
        var oldPayload = new VersionAProtoBufPayload
        {
            Timestamp = dateTime.ToUnixTimeMilliseconds(),
            Position = new VersionAProtoBufPayload.KuraPosition
            {
                Heading = 1.0,
                Latitude = 2.0,
                Longitude = 3.0,
                Precision = 4.0,
                Speed = 5.0,
                Timestamp = dateTime2.ToUnixTimeMilliseconds(),
                Altitude = 7.0,
                Status = 8,
                Satellites = 9
            },
            Metrics = metrics,
            Body = bodyData
        };
        var payload = VersionAMain.PayloadConverter.ConvertVersionAPayload(oldPayload);
        Assert.IsNotNull(payload);
        Assert.AreEqual(dateTime.ToUnixTimeMilliseconds(), payload.Timestamp);
        Assert.IsNotNull(payload.Position);
        Assert.AreEqual(1.0, payload.Position.Heading);
        Assert.AreEqual(2.0, payload.Position.Latitude);
        Assert.AreEqual(3.0, payload.Position.Longitude);
        Assert.AreEqual(4.0, payload.Position.Precision);
        Assert.AreEqual(5.0, payload.Position.Speed);
        Assert.AreEqual(dateTime2.ToUnixTimeMilliseconds(), payload.Position.Timestamp);
        Assert.AreEqual(7.0, payload.Position.Altitude);
        Assert.AreEqual(8, payload.Position.Status);
        Assert.AreEqual(9, payload.Position.Satellites);
        Assert.IsNotNull(payload.Metrics);
        Assert.AreEqual(convertedMetrics.Count, payload.Metrics.Count);

        var count = 0;

        foreach (var metric in payload.Metrics)
        {
            MetricEquals(convertedMetrics[count++], metric);
        }

        Assert.IsNotNull(payload.Body);
        CollectionAssert.AreEqual(bodyData, payload.Body);
    }

    /// <summary>
    /// Tests the Sparkplug payload converter for converting a version A payload to Proto.
    /// </summary>
    [TestMethod]
    public void TestConvertVersionAPayloadToProto()
    {
        var dateTime = new DateTimeOffset(2021, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var dateTime2 = new DateTimeOffset(2022, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var bodyData = new byte[] { 1, 2, 3, 4 };
        var metrics = new List<VersionAData.KuraMetric>
        {
            new("Test1", VersionAData.DataType.Double, 1.0),
            new("Test2", VersionAData.DataType.Float, 2.0f),
            new("Test3", VersionAData.DataType.Int64, 3),
            new("Test4", VersionAData.DataType.Int32, 4),
            new("Test5", VersionAData.DataType.Boolean, true),
            new("Test6", VersionAData.DataType.String, "6"),
            new("Test7", VersionAData.DataType.Bytes, new byte[] {7, 8, 9, 10 })
        };
        var convertedMetrics = new List<VersionAProtoBufPayload.KuraMetric>
        {
            new()
            {
                Name = "Test1",
                DataType = VersionAProtoBufPayload.KuraMetric.ValueType.Double,
                DoubleValue = 1.0
            },
            new()
            {
                Name = "Test2",
                DataType = VersionAProtoBufPayload.KuraMetric.ValueType.Float,
                FloatValue = 2.0f
            },
            new()
            {
                Name = "Test3",
                DataType = VersionAProtoBufPayload.KuraMetric.ValueType.Int64,
                LongValue = 3
            },
            new()
            {
                Name = "Test4",
                DataType = VersionAProtoBufPayload.KuraMetric.ValueType.Int32,
                IntValue = 4
            },
            new()
            {
                Name = "Test5",
                DataType = VersionAProtoBufPayload.KuraMetric.ValueType.Bool,
                BooleanValue = true
            },
            new()
            {
                Name = "Test6",
                DataType = VersionAProtoBufPayload.KuraMetric.ValueType.String,
                StringValue = "6"
            },
            new()
            {
                Name = "Test7",
                DataType = VersionAProtoBufPayload.KuraMetric.ValueType.Bytes,
                BytesValue = [7, 8, 9, 10]
            }
        };
        var oldPayload = new VersionAData.Payload
        {
            Timestamp = dateTime.ToUnixTimeMilliseconds(),
            Position = new VersionAData.KuraPosition
            {
                Heading = 1.0,
                Latitude = 2.0,
                Longitude = 3.0,
                Precision = 4.0,
                Speed = 5.0,
                Timestamp = dateTime2.ToUnixTimeMilliseconds(),
                Altitude = 7.0,
                Status = 8,
                Satellites = 9
            },
            Metrics = metrics,
            Body = bodyData
        };
        var payload = VersionAMain.PayloadConverter.ConvertVersionAPayload(oldPayload);
        Assert.IsNotNull(payload);
        Assert.AreEqual(dateTime.ToUnixTimeMilliseconds(), payload.Timestamp);
        Assert.IsNotNull(payload.Position);
        Assert.AreEqual(1.0, payload.Position.Heading);
        Assert.AreEqual(2.0, payload.Position.Latitude);
        Assert.AreEqual(3.0, payload.Position.Longitude);
        Assert.AreEqual(4.0, payload.Position.Precision);
        Assert.AreEqual(5.0, payload.Position.Speed);
        Assert.AreEqual(dateTime2.ToUnixTimeMilliseconds(), payload.Position.Timestamp);
        Assert.AreEqual(7.0, payload.Position.Altitude);
        Assert.AreEqual(8, payload.Position.Status);
        Assert.AreEqual(9, payload.Position.Satellites);
        Assert.IsNotNull(payload.Metrics);
        Assert.AreEqual(convertedMetrics.Count, payload.Metrics.Count);

        var count = 0;

        foreach (var metric in payload.Metrics)
        {
            MetricEquals(convertedMetrics[count++], metric);
        }

        Assert.IsNotNull(payload.Body);
        CollectionAssert.AreEqual(bodyData, payload.Body);
    }

    /// <summary>
    /// Tests the Sparkplug payload converter for converting a version A payload from Proto (With negative values).
    /// </summary>
    [TestMethod]
    public void TestConvertVersionAPayloadFromProtoWithNegativeValues()
    {
        var dateTime = new DateTimeOffset(2021, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var dateTime2 = new DateTimeOffset(2022, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var bodyData = new byte[] { 1, 2, 3, 4 };
        var metrics = new List<VersionAProtoBufPayload.KuraMetric>
        {
            new()
            {
                Name = "Test1",
                DataType = VersionAProtoBufPayload.KuraMetric.ValueType.Double,
                DoubleValue = -1.0
            }
        };
            var convertedMetrics = new List<VersionAData.KuraMetric>
        {
            new("Test1", VersionAData.DataType.Double, -1.0)
        };
        var oldPayload = new VersionAProtoBufPayload
        {
            Timestamp = dateTime.ToUnixTimeMilliseconds(),
            Position = new VersionAProtoBufPayload.KuraPosition
            {
                Heading = 1.0,
                Latitude = 2.0,
                Longitude = 3.0,
                Precision = 4.0,
                Speed = 5.0,
                Timestamp = dateTime2.ToUnixTimeMilliseconds(),
                Altitude = 7.0,
                Status = 8,
                Satellites = 9
            },
            Metrics = metrics,
            Body = bodyData
        };
        var payload = VersionAMain.PayloadConverter.ConvertVersionAPayload(oldPayload);
        Assert.IsNotNull(payload);
        Assert.AreEqual(dateTime.ToUnixTimeMilliseconds(), payload.Timestamp);
        Assert.IsNotNull(payload.Position);
        Assert.AreEqual(1.0, payload.Position.Heading);
        Assert.AreEqual(2.0, payload.Position.Latitude);
        Assert.AreEqual(3.0, payload.Position.Longitude);
        Assert.AreEqual(4.0, payload.Position.Precision);
        Assert.AreEqual(5.0, payload.Position.Speed);
        Assert.AreEqual(dateTime2.ToUnixTimeMilliseconds(), payload.Position.Timestamp);
        Assert.AreEqual(7.0, payload.Position.Altitude);
        Assert.AreEqual(8, payload.Position.Status);
        Assert.AreEqual(9, payload.Position.Satellites);
        Assert.IsNotNull(payload.Metrics);
        Assert.AreEqual(convertedMetrics.Count, payload.Metrics.Count);

        var count = 0;

        foreach (var metric in payload.Metrics)
        {
            MetricEquals(convertedMetrics[count++], metric);
        }

        Assert.IsNotNull(payload.Body);
        CollectionAssert.AreEqual(bodyData, payload.Body);
    }

    /// <summary>
    /// Tests the Sparkplug payload converter for converting a version A payload to Proto (With negative values).
    /// </summary>
    [TestMethod]
    public void TestConvertVersionAPayloadToProtoWithNegativeValues()
    {
        var dateTime = new DateTimeOffset(2021, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var dateTime2 = new DateTimeOffset(2022, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var bodyData = new byte[] { 1, 2, 3, 4 };
        var metrics = new List<VersionAData.KuraMetric>
        {
            new("Test1", VersionAData.DataType.Float, -2.0f)
        };
            var convertedMetrics = new List<VersionAProtoBufPayload.KuraMetric>
        {
            new()
            {
                Name = "Test1",
                DataType = VersionAProtoBufPayload.KuraMetric.ValueType.Float,
                FloatValue = -2.0f
            }
        };
        var oldPayload = new VersionAData.Payload
        {
            Timestamp = dateTime.ToUnixTimeMilliseconds(),
            Position = new VersionAData.KuraPosition
            {
                Heading = 1.0,
                Latitude = 2.0,
                Longitude = 3.0,
                Precision = 4.0,
                Speed = 5.0,
                Timestamp = dateTime2.ToUnixTimeMilliseconds(),
                Altitude = 7.0,
                Status = 8,
                Satellites = 9
            },
            Metrics = metrics,
            Body = bodyData
        };
        var payload = VersionAMain.PayloadConverter.ConvertVersionAPayload(oldPayload);
        Assert.IsNotNull(payload);
        Assert.AreEqual(dateTime.ToUnixTimeMilliseconds(), payload.Timestamp);
        Assert.IsNotNull(payload.Position);
        Assert.AreEqual(1.0, payload.Position.Heading);
        Assert.AreEqual(2.0, payload.Position.Latitude);
        Assert.AreEqual(3.0, payload.Position.Longitude);
        Assert.AreEqual(4.0, payload.Position.Precision);
        Assert.AreEqual(5.0, payload.Position.Speed);
        Assert.AreEqual(dateTime2.ToUnixTimeMilliseconds(), payload.Position.Timestamp);
        Assert.AreEqual(7.0, payload.Position.Altitude);
        Assert.AreEqual(8, payload.Position.Status);
        Assert.AreEqual(9, payload.Position.Satellites);
        Assert.IsNotNull(payload.Metrics);
        Assert.AreEqual(convertedMetrics.Count, payload.Metrics.Count);

        var count = 0;

        foreach (var metric in payload.Metrics)
        {
            MetricEquals(convertedMetrics[count++], metric);
        }

        Assert.IsNotNull(payload.Body);
        CollectionAssert.AreEqual(bodyData, payload.Body);
    }

    /// <summary>
    /// Tests the two given metrics for equality.
    /// </summary>
    /// <param name="expected">The expected metric.</param>
    /// <param name="newMetric">The new metric.</param>
    private static void MetricEquals(VersionAData.KuraMetric expected, VersionAData.KuraMetric newMetric)
    {
        if (expected.Value is not null && expected.Value.GetType() == typeof(byte[]))
        {
            CollectionAssert.AreEqual((byte[]?)expected.Value, (byte[]?)newMetric.Value);
        }
        else
        {
            Assert.AreEqual(expected.Value, newMetric.Value);
        }

        Assert.AreEqual(expected.DataType, newMetric.DataType);
        Assert.AreEqual(expected.Name, newMetric.Name);
    }

    /// <summary>
    /// Tests the two given metrics for equality.
    /// </summary>
    /// <param name="expected">The expected metric.</param>
    /// <param name="newMetric">The new metric.</param>
    private static void MetricEquals(VersionAProtoBufPayload.KuraMetric expected, VersionAProtoBufPayload.KuraMetric newMetric)
    {
        Assert.AreEqual(expected.BooleanValue, newMetric.BooleanValue);
        CollectionAssert.AreEqual(expected.BytesValue, newMetric.BytesValue);
        Assert.AreEqual(expected.DataType, newMetric.DataType);
        Assert.AreEqual(expected.DoubleValue, newMetric.DoubleValue);
        Assert.AreEqual(expected.FloatValue, newMetric.FloatValue);
        Assert.AreEqual(expected.IntValue, newMetric.IntValue);
        Assert.AreEqual(expected.LongValue, newMetric.LongValue);
        Assert.AreEqual(expected.Name, newMetric.Name);
        Assert.AreEqual(expected.StringValue, newMetric.StringValue);
    }
}
