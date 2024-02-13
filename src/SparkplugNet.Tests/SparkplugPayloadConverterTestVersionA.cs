// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugMessageGeneratorTest.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class to test the <see cref="VersionA.PayloadConverter"/> class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Tests;

/// <summary>
/// A class to test the <see cref="VersionA.PayloadConverter"/> class.
/// </summary>
[TestClass]
public class SparkplugPayloadConverterTestVersionA
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
                Type = VersionAProtoBufPayload.KuraMetric.ValueType.Double,
                DoubleValue = 1.0
            },
            new()
            {
                Name = "Test2",
                Type = VersionAProtoBufPayload.KuraMetric.ValueType.Float,
                FloatValue = 2.0f
            },
            new()
            {
                Name = "Test3",
                Type = VersionAProtoBufPayload.KuraMetric.ValueType.Int64,
                LongValue = 3
            },
            new()
            {
                Name = "Test4",
                Type = VersionAProtoBufPayload.KuraMetric.ValueType.Int32,
                IntValue = 4
            },
            new()
            {
                Name = "Test5",
                Type = VersionAProtoBufPayload.KuraMetric.ValueType.Bool,
                BoolValue = true
            },
            new()
            {
                Name = "Test6",
                Type = VersionAProtoBufPayload.KuraMetric.ValueType.String,
                StringValue = "6"
            },
            new()
            {
                Name = "Test7",
                Type = VersionAProtoBufPayload.KuraMetric.ValueType.Bytes,
                BytesValue = [7, 8, 9, 10]
            }
        };
        var convertedMetrics = new List<VersionA.Data.KuraMetric>
        {
            new()
            {
                Name = "Test1",
                DataType = VersionA.Data.DataType.Double,
                DoubleValue = 1.0
            },
            new()
            {
                Name = "Test2",
                DataType = VersionA.Data.DataType.Float,
                FloatValue = 2.0f
            },
            new()
            {
                Name = "Test3",
                DataType = VersionA.Data.DataType.Int64,
                LongValue = 3
            },
            new()
            {
                Name = "Test4",
                DataType = VersionA.Data.DataType.Int32,
                IntValue = 4
            },
            new()
            {
                Name = "Test5",
                DataType = VersionA.Data.DataType.Boolean,
                BooleanValue = true
            },
            new()
            {
                Name = "Test6",
                DataType = VersionA.Data.DataType.String,
                StringValue = "6"
            },
            new()
            {
                Name = "Test7",
                DataType = VersionA.Data.DataType.Bytes,
                BytesValue = [7, 8, 9, 10]
            }
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
        var payload = VersionA.PayloadConverter.ConvertVersionAPayload(oldPayload);
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
        CollectionAssert.AreEqual(convertedMetrics, payload.Metrics);
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
        var metrics = new List<VersionA.Data.KuraMetric>
        {
            new()
            {
                Name = "Test1",
                DataType = VersionA.Data.DataType.Double,
                DoubleValue = 1.0
            },
            new()
            {
                Name = "Test2",
                DataType = VersionA.Data.DataType.Float,
                FloatValue = 2.0f
            },
            new()
            {
                Name = "Test3",
                DataType = VersionA.Data.DataType.Int64,
                LongValue = 3
            },
            new()
            {
                Name = "Test4",
                DataType = VersionA.Data.DataType.Int32,
                IntValue = 4
            },
            new()
            {
                Name = "Test5",
                DataType = VersionA.Data.DataType.Boolean,
                BooleanValue = true
            },
            new()
            {
                Name = "Test6",
                DataType = VersionA.Data.DataType.String,
                StringValue = "6"
            },
            new()
            {
                Name = "Test7",
                DataType = VersionA.Data.DataType.Bytes,
                BytesValue = [7, 8, 9, 10]
            }
        };
        var convertedMetrics = new List<VersionA.Data.KuraMetric>
        {
            new()
            {
                Name = "Test1",
                DataType = VersionA.Data.DataType.Double,
                DoubleValue = 1.0
            },
            new()
            {
                Name = "Test2",
                DataType = VersionA.Data.DataType.Float,
                FloatValue = 2.0f
            },
            new()
            {
                Name = "Test3",
                DataType = VersionA.Data.DataType.Int64,
                LongValue = 3
            },
            new()
            {
                Name = "Test4",
                DataType = VersionA.Data.DataType.Int32,
                IntValue = 4
            },
            new()
            {
                Name = "Test5",
                DataType = VersionA.Data.DataType.Boolean,
                BooleanValue = true
            },
            new()
            {
                Name = "Test6",
                DataType = VersionA.Data.DataType.String,
                StringValue = "6"
            },
            new()
            {
                Name = "Test7",
                DataType = VersionA.Data.DataType.Bytes,
                BytesValue = [7, 8, 9, 10]
            }
        };
        var oldPayload = new VersionA.Data.Payload
        {
            Timestamp = dateTime.ToUnixTimeMilliseconds(),
            Position = new VersionA.Data.KuraPosition
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
        var payload = VersionA.PayloadConverter.ConvertVersionAPayload(oldPayload);
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
        CollectionAssert.AreEqual(convertedMetrics, payload.Metrics);
        Assert.IsNotNull(payload.Body);
        CollectionAssert.AreEqual(bodyData, payload.Body);
    }
}
