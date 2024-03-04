// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugPayloadConverterTestVersionA.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class to test the <see cref="VersionB.PayloadConverter"/> class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Tests.Payloads;

/// <summary>
/// A class to test the <see cref="VersionB.PayloadConverter"/> class.
/// </summary>
[TestClass]
public sealed class SparkplugPayloadConverterTestVersionB
{
    /// <summary>
    /// Tests the Sparkplug payload converter for converting a version B payload from Proto.
    /// </summary>
    [TestMethod]
    public void TestConvertVersionBPayloadFromProto()
    {
        var timestamp = new DateTimeOffset(2019, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var dateTime = new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var dateTime2 = new DateTimeOffset(2022, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var dateTime3 = new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var dateTime4 = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var bodyData = new byte[] { 1, 2, 3, 4 };
        var metrics = new List<VersionBProtoBufPayload.Metric>
        {
            new()
            {
                Name = "Test1",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 1,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBData.DataType.Int8,
                IntValue = 1
            },
            new()
            {
                Name = "Test2",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 2,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBData.DataType.Int16,
                IntValue = 2
            },
            new()
            {
                Name = "Test3",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 3,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBData.DataType.Int32,
                IntValue = 3
            },
            new()
            {
                Name = "Test4",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 4,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBData.DataType.Int64,
                LongValue = 4
            },
            new()
            {
                Name = "Test5",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 5,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBData.DataType.UInt8,
                IntValue = 5
            },
            new()
            {
                Name = "Test6",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 6,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBData.DataType.UInt16,
                IntValue = 6
            },
            new()
            {
                Name = "Test7",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 7,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBData.DataType.UInt32,
                LongValue = 7
            },
            new()
            {
                Name = "Test8",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 8,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBData.DataType.UInt64,
                LongValue = 8
            },
            new()
            {
                Name = "Test9",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 9,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBData.DataType.Float,
                FloatValue = 9
            },
            new()
            {
                Name = "Test10",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 10,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBData.DataType.Double,
                DoubleValue = 10
            },
            new()
            {
                Name = "Test11",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 11,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBData.DataType.Boolean,
                BooleanValue = true
            },
            new()
            {
                Name = "Test12",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 12,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBData.DataType.String,
                StringValue = "12"
            },
            new()
            {
                Name = "Test13",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 13,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBData.DataType.DateTime,
                LongValue = 13
            },
            new()
            {
                Name = "Test14",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 14,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBData.DataType.Text,
                StringValue = "14"
            },
            new()
            {
                Name = "Test15",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 15,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBData.DataType.Uuid,
                StringValue = "c609f36e-92f9-4103-92f0-bf9d95c18be9"
            },
            new()
            {
                Name = "Test16",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 16,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBData.DataType.DataSet,
                DataSetValue = new VersionBProtoBufPayload.DataSet
                {
                    Columns = ["Test1", "Test2"],
                    NumberOfColumns = 2,
                    Rows =
                    [
                        new VersionBProtoBufPayload.DataSet.Row
                        {
                            Elements =
                            [
                                new()
                                {
                                    IntValue = 1
                                },
                                new()
                                {
                                    FloatValue = 2
                                }
                            ]
                        }
                    ],
                    Types = [1, 9]
                }
            },
            new()
            {
                Name = "Test17",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 17,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBData.DataType.Bytes,
                BytesValue = [1, 2, 3, 4]
            },
            new()
            {
                Name = "Test18",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 18,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                MetaData = new VersionBProtoBufPayload.MetaData
                {
                    ContentType = "application/json",
                    Description = "Test",
                    FileName = "Test.json",
                    FileType = "json",
                    IsMultiPart = true,
                    Md5 = "md5",
                    Seq = 1,
                    Size = 1
                },
                DataType = (uint?)VersionBData.DataType.File,
                BytesValue = [1, 2, 3, 4]
            },
            new()
            {
                Name = "Test19",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 19,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBData.DataType.Template,
                TemplateValue = new VersionBProtoBufPayload.Template
                {
                    IsDefinition = true,
                    Version = "1.0",
                    TemplateRef = "TestRef1",
                    Parameters =
                    [
                        new()
                        {
                            Name = "Test1",
                            DataType = (uint?)VersionBData.DataType.Int8,
                            IntValue = 1
                        },
                        new()
                        {
                            DataType = (uint?)VersionBData.DataType.Float,
                            Name = "Test2",
                            FloatValue = 2
                        }
                    ],
                    Metrics =
                    [
                        new()
                        {
                            Name = "Test1",
                            Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                            Alias = 1,
                            IsHistorical = true,
                            IsTransient = true,
                            IsNull = false,
                            DataType = (uint?)VersionBData.DataType.Int16,
                            IntValue = 1
                        },
                        new()
                        {
                            Name = "Test2",
                            Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                            Alias = 2,
                            IsHistorical = true,
                            IsTransient = true,
                            IsNull = false,
                            DataType = (uint?)VersionBData.DataType.Double,
                            DoubleValue = 2
                        }
                    ]
                }
            },
            new()
            {
                Name = "Test20",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 20,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBData.DataType.PropertySet,
                PropertySetValue = new VersionBProtoBufPayload.PropertySet
                {
                    Keys = ["Test1", "Test2"],
                    Values =
                    [
                        new()
                        {
                            IsNull = false,
                            DataType = (uint?)VersionBData.DataType.Int8,
                            IntValue = 1
                        },
                        new()
                        {
                            IsNull = false,
                            DataType = (uint?)VersionBData.DataType.Int64,
                            LongValue = 2
                        }
                    ]
                }
            },
            // Todo: How to handle this?!
            //new()
            //{
            //    Name = "Test21",
            //    Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
            //    Alias = 21,
            //    IsHistorical = true,
            //    IsTransient = true,
            //    IsNull = false,
            //    DataType = (uint?)VersionBData.DataType.PropertySetList,
            //    PropertySetListValue = "c609f36e-92f9-4103-92f0-bf9d95c18be9"
            //},
            new()
            {
                Name = "Test22",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 22,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBData.DataType.Int8Array,
                BytesValue = [1, 2, 3, 4]
            },
            new()
            {
                Name = "Test23",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 23,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBData.DataType.Int16Array,
                BytesValue = [1, 0, 2, 0, 3, 0, 4, 0]
            },
            new()
            {
                Name = "Test24",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 24,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBData.DataType.Int32Array,
                BytesValue = [1, 0, 0, 0, 2, 0, 0, 0, 3, 0, 0, 0, 4, 0, 0, 0]
            },
            new()
            {
                Name = "Test25",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 25,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBData.DataType.Int64Array,
                BytesValue = [1, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 4, 0, 0, 0, 0, 0, 0, 0]
            },
            new()
            {
                Name = "Test26",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 26,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBData.DataType.UInt8Array,
                BytesValue = [1, 2, 3, 4]
            },
            new()
            {
                Name = "Test27",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 27,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBData.DataType.UInt16Array,
                BytesValue = [1, 0, 2, 0, 3, 0, 4, 0]
            },
            new()
            {
                Name = "Test28",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 28,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBData.DataType.UInt32Array,
                BytesValue = [1, 0, 0, 0, 2, 0, 0, 0, 3, 0, 0, 0, 4, 0, 0, 0]
            },
            new()
            {
                Name = "Test29",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 29,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBData.DataType.UInt64Array,
                BytesValue = [1, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 4, 0, 0, 0, 0, 0, 0, 0]
            },
            new()
            {
                Name = "Test30",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 30,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBData.DataType.FloatArray,
                BytesValue = [0x00, 0x00, 0x80, 0x3F, 0x00, 0x00, 0x00, 0x40, 0x00, 0x00, 0x40, 0x40, 0x00, 0x00, 0x80, 0x40]
            },
            new()
            {
                Name = "Test31",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 31,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBData.DataType.DoubleArray,
                BytesValue = [0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xf0, 0x3f, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x40, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x08, 0x40, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x40]
            },
            new()
            {
                Name = "Test32",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 32,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBData.DataType.BooleanArray,
                BytesValue = [0x04, 0x00, 0x00, 0x00, 0xa0]
            },
            new()
            {
                Name = "Test33",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 33,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBData.DataType.StringArray,
                BytesValue = [0x31, 0x00, 0x32, 0x00, 0x33, 0x00, 0x34, 0x00]
            },
            new()
            {
                Name = "Test34",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 34,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBData.DataType.DateTimeArray,
                BytesValue = [0x00, 0xE8, 0x66, 0x5E, 0x6F, 0x01, 0x00, 0x00,
                              0x00, 0x9C, 0xEF, 0x12, 0x7E, 0x01, 0x00, 0x00, 
                              0x00, 0xC8, 0xA0, 0x6A, 0x85, 0x01, 0x00, 0x00, 
                              0x00, 0xF4, 0x51, 0xC2, 0x8C, 0x01, 0x00, 0x00]
            }
        };
        var convertedMetrics = new List<VersionBData.Metric>
        {
            new("Test1", VersionBData.DataType.Int8, (sbyte)1, timestamp)
            {
                Alias = 1,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test2", VersionBData.DataType.Int16, (short)2, timestamp)
            {
                Alias = 2,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test3", VersionBData.DataType.Int32, (int)3, timestamp)
            {
                Alias = 3,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test4", VersionBData.DataType.Int64, (long)4, timestamp)
            {
                Alias = 4,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test5", VersionBData.DataType.UInt8, (byte)5, timestamp)
            {
                Alias = 5,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test6", VersionBData.DataType.UInt16, (ushort)6, timestamp)
            {
                Alias = 6,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test7", VersionBData.DataType.UInt32, (uint)7, timestamp)
            {
                Alias = 7,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test8", VersionBData.DataType.UInt64, (ulong)8, timestamp)
            {
                Alias = 8,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test9", VersionBData.DataType.Float, 9f, timestamp)
            {
                Alias = 9,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test10", VersionBData.DataType.Double, 10.0, timestamp)
            {
                Alias = 10,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test11", VersionBData.DataType.Boolean, true, timestamp)
            {
                Alias = 11,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test12", VersionBData.DataType.String, "12", timestamp)
            {
                Alias = 12,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test13", VersionBData.DataType.DateTime, timestamp.DateTime, timestamp)
            {
                Alias = 13,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test14", VersionBData.DataType.Text, "14", timestamp)
            {
                Alias = 14,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test15", VersionBData.DataType.Uuid, "c609f36e-92f9-4103-92f0-bf9d95c18be9", timestamp)
            {
                Alias = 15,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test16", VersionBData.DataType.DataSet, new VersionBData.DataSet
                {
                    Columns = ["Test1", "Test2"],
                    NumberOfColumns = 2,
                    Rows =
                    [
                        new VersionBData.Row
                        {
                            Elements =
                            [
                                new(VersionBData.DataType.Int8, 1)
                                {
                                },
                                new(VersionBData.DataType.Float, 2.0f)
                                {
                                }
                            ]
                        }
                    ],
                    Types = [1, 9]
                }, timestamp)
            {
                Alias = 16,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test17", VersionBData.DataType.Bytes, new byte[] { 1, 2, 3, 4 }, timestamp)
            {
                Alias = 17,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test18", VersionBData.DataType.File, new byte[] { 1, 2, 3, 4 }, timestamp)
            {
                Alias = 18,
                IsHistorical = true,
                IsTransient = true,
                MetaData = new VersionBData.MetaData
                {
                    ContentType = "application/json",
                    Description = "Test",
                    FileName = "Test.json",
                    FileType = "json",
                    IsMultiPart = true,
                    Md5 = "md5",
                    Seq = 1,
                    Size = 1
                }
            },
            new("Test19", VersionBData.DataType.Template,
                new VersionBData.Template
                {
                    IsDefinition = true,
                    Version = "1.0",
                    TemplateRef = "TestRef1",
                    Parameters =
                    [
                        new("Test1", VersionBData.DataType.Int8, 1)
                        {
                        },
                        new("Test2", VersionBData.DataType.Float, 2.0f)
                        {
                        }
                    ],
                    Metrics =
                    [
                        new("Test1", VersionBData.DataType.Int16, 1, timestamp)
                        {
                            Alias = 1,
                            IsHistorical = true,
                            IsTransient = true
                        },
                        new("Test2", VersionBData.DataType.Double, 2.0, timestamp)
                        {
                            Alias = 2,
                            IsHistorical = true,
                            IsTransient = true
                        }
                    ]
                }, timestamp)
            {
                Alias = 19,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test20", VersionBData.DataType.PropertySet, new VersionBData.PropertySet
                {
                    Keys = ["Test1", "Test2"],
                    Values =
                    [
                        new(VersionBData.DataType.Int8, 1)
                        {
                        },
                        new(VersionBData.DataType.Int64, (long)2)
                        {
                        }
                    ]
                }, timestamp)
            {
                Alias = 20,
                IsHistorical = true,
                IsTransient = true
            },
            // Todo: How to handle this?!
            //new()
            //{
            //    Name = "Test21",
            //    Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
            //    Alias = 21,
            //    IsHistorical = true,
            //    IsTransient = true,
            //    IsNull = false,
            //    DataType = VersionBData.DataType.PropertySetList,
            //    PropertySetListValue = "c609f36e-92f9-4103-92f0-bf9d95c18be9"
            //},
            new("Test22", VersionBData.DataType.Int8Array, new sbyte[] { 1, 2, 3, 4 }, timestamp)
            {
                Alias = 22,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test23", VersionBData.DataType.Int16Array, new short[] { 1, 2, 3, 4 }, timestamp)
            {
                Alias = 23,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test24", VersionBData.DataType.Int32Array, new int[] { 1, 2, 3, 4 }, timestamp)
            {
                Alias = 24,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test25", VersionBData.DataType.Int64Array, new long[] { 1, 2, 3, 4 }, timestamp)
            {
                Alias = 25,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test26", VersionBData.DataType.UInt8Array, new byte[] { 1, 2, 3, 4 }, timestamp)
            {
                Alias = 26,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test27", VersionBData.DataType.UInt16Array, new ushort[] { 1, 2, 3, 4 }, timestamp)
            {
                Alias = 27,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test28", VersionBData.DataType.UInt32Array, new uint[] { 1, 2, 3, 4 }, timestamp)
            {
                Alias = 28,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test29", VersionBData.DataType.UInt64Array, new ulong[] { 1, 2, 3, 4 }, timestamp)
            {
                Alias = 29,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test30", VersionBData.DataType.FloatArray, new float[] { 1, 2, 3, 4 }, timestamp)
            {
                Alias = 30,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test31", VersionBData.DataType.DoubleArray, new double[] { 1, 2, 3, 4 }, timestamp)
            {
                Alias = 31,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test32", VersionBData.DataType.BooleanArray, new bool[] { true, false, true, false }, timestamp)
            {
                Alias = 32,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test33", VersionBData.DataType.StringArray, new string[] { "1", "2", "3", "4" }, timestamp)
            {
                Alias = 33,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test34", VersionBData.DataType.DateTimeArray, new DateTimeOffset[] { dateTime, dateTime2, dateTime3, dateTime4 }, timestamp)
            {
                Alias = 34,
                IsHistorical = true,
                IsTransient = true
            }
        };
        var oldPayload = new VersionBProtoBufPayload
        {
            Body = bodyData,
            Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
            Seq = 1,
            Uuid = "477a41e5-f0ba-4b98-9522-95d44861d993",
            Metrics = metrics
        };
        var payload = VersionB.PayloadConverter.ConvertVersionBPayload(oldPayload);
        Assert.IsNotNull(payload);
        CollectionAssert.AreEqual(bodyData, payload.Body);
        Assert.AreEqual((ulong)timestamp.ToUnixTimeMilliseconds(), payload.Timestamp);
        Assert.AreEqual((ulong)1, payload.Seq);
        Assert.AreEqual("477a41e5-f0ba-4b98-9522-95d44861d993", payload.Uuid);
        Assert.AreEqual(convertedMetrics.Count, payload.Metrics.Count);

        var count = 0;

        foreach (var metric in payload.Metrics)
        {
            EqualityHelper.MetricEquals(convertedMetrics[count++], metric);
        }
    }

    /// <summary>
    /// Tests the Sparkplug payload converter for converting a version B payload to Proto.
    /// </summary>
    [TestMethod]
    public void TestConvertVersionBPayloadToProto()
    {
        var timestamp = new DateTimeOffset(2019, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var dateTime = new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var dateTime2 = new DateTimeOffset(2022, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var dateTime3 = new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var dateTime4 = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var bodyData = new byte[] { 1, 2, 3, 4 };
        var metrics = new List<VersionBData.Metric>
        {
            new("Test1", VersionBData.DataType.Int8, (sbyte)1, timestamp)
            {
                Alias = 1,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test2", VersionBData.DataType.Int16, (short)2, timestamp)
            {
                Alias = 2,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test3", VersionBData.DataType.Int32, 3, timestamp)
            {
                Alias = 3,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test4", VersionBData.DataType.Int64, (long)4, timestamp)
            {
                Alias = 4,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test5", VersionBData.DataType.UInt8, (byte)5, timestamp)
            {
                Alias = 5,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test6", VersionBData.DataType.UInt16, (ushort)6, timestamp)
            {
                Alias = 6,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test7", VersionBData.DataType.UInt32, (uint)7, timestamp)
            {
                Alias = 7,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test8", VersionBData.DataType.UInt64, (ulong)8, timestamp)
            {
                Alias = 8,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test9", VersionBData.DataType.Float, 9.0f, timestamp)
            {
                Alias = 9,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test10", VersionBData.DataType.Double, 10.0, timestamp)
            {
                Alias = 10,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test11", VersionBData.DataType.Boolean, true, timestamp)
            {
                Alias = 11,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test12", VersionBData.DataType.String, "12", timestamp)
            {
                Alias = 12,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test13", VersionBData.DataType.DateTime, timestamp.DateTime, timestamp)
            {
                Alias = 13,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test14", VersionBData.DataType.Text, "14", timestamp)
            {
                Alias = 14,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test15", VersionBData.DataType.Uuid, "c609f36e-92f9-4103-92f0-bf9d95c18be9", timestamp)
            {
                Alias = 15,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test16", VersionBData.DataType.DataSet, new VersionBData.DataSet
                {
                    Columns = ["Test1", "Test2"],
                    NumberOfColumns = 2,
                    Rows =
                    [
                        new VersionBData.Row
                        {
                            Elements =
                            [
                                new(VersionBData.DataType.Int16, 1)
                                {
                                },
                                new(VersionBData.DataType.Float, 2.0f)
                                {
                                }
                            ]
                        }
                    ],
                    Types = [1, 9]
                }, timestamp)
            {
                Alias = 16,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test17", VersionBData.DataType.Bytes, new byte[] { 1, 2, 3, 4 }, timestamp)
            {
                Alias = 17,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test18", VersionBData.DataType.File, new byte[] { 1, 2, 3, 4 }, timestamp)
            {
                Alias = 18,
                IsHistorical = true,
                IsTransient = true,
                MetaData = new VersionBData.MetaData
                {
                    ContentType = "application/json",
                    Description = "Test",
                    FileName = "Test.json",
                    FileType = "json",
                    IsMultiPart = true,
                    Md5 = "md5",
                    Seq = 1,
                    Size = 1
                }
            },
            new("Test19", VersionBData.DataType.Template, new VersionBData.Template
                {
                    IsDefinition = true,
                    Version = "1.0",
                    TemplateRef = "TestRef1",
                    Parameters =
                    [
                        new("Test1", VersionBData.DataType.Int8, (sbyte)1)
                        {
                        },
                        new("Test2", VersionBData.DataType.Float, 2.0f)
                        {
                        }
                    ],
                    Metrics =
                    [
                        new("Test1", VersionBData.DataType.Int16, (short)1)
                        {
                            Alias = 1,
                            IsHistorical = true,
                            IsTransient = true,
                            Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds()
                        },
                        new("Test2", VersionBData.DataType.Double, 2.0)
                        {
                            Alias = 2,
                            IsHistorical = true,
                            IsTransient = true,
                            Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds()
                        }
                    ]
                }, timestamp)
            {
                Alias = 19,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test20", VersionBData.DataType.PropertySet, new VersionBData.PropertySet
                {
                    Keys = ["Test1", "Test2"],
                    Values =
                    [
                        new(VersionBData.DataType.Int8, (sbyte)1)
                        {
                        },
                        new(VersionBData.DataType.Int64, (long)2)
                        {
                        }
                    ]
                }, timestamp)
            {
                Alias = 20,
                IsHistorical = true,
                IsTransient = true
            },
            // Todo: How to handle this?!
            //new()
            //{
            //    Name = "Test21",
            //    Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
            //    Alias = 21,
            //    IsHistorical = true,
            //    IsTransient = true,
            //    IsNull = false,
            //    DataType = VersionBData.DataType.PropertySetList,
            //    PropertySetListValue = "c609f36e-92f9-4103-92f0-bf9d95c18be9"
            //},
            new("Test22", VersionBData.DataType.Int8Array, new sbyte [] { 1, 2, 3, 4 }, timestamp)
            {
                Alias = 22,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test23", VersionBData.DataType.Int16Array, new short [] { 1, 2, 3, 4 }, timestamp)
            {
                Alias = 23,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test24", VersionBData.DataType.Int32Array, new int [] { 1, 2, 3, 4 }, timestamp)
            {
                Alias = 24,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test25", VersionBData.DataType.Int64Array, new long [] { 1, 2, 3, 4 }, timestamp)
            {
                Alias = 25,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test26", VersionBData.DataType.UInt8Array, new byte [] { 1, 2, 3, 4 }, timestamp)
            {
                Alias = 26,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test27", VersionBData.DataType.UInt16Array, new ushort [] { 1, 2, 3, 4 }, timestamp)
            {
                Alias = 27,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test28", VersionBData.DataType.UInt32Array, new uint [] { 1, 2, 3, 4 }, timestamp)
            {
                Alias = 28,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test29", VersionBData.DataType.UInt64Array, new ulong [] { 1, 2, 3, 4 }, timestamp)
            {
                Alias = 29,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test30", VersionBData.DataType.FloatArray, new float [] { 1, 2, 3, 4 }, timestamp)
            {
                Alias = 30,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test31", VersionBData.DataType.DoubleArray, new double [] { 1, 2, 3, 4 }, timestamp)
            {
                Alias = 31,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test32", VersionBData.DataType.BooleanArray, new bool [] { true, false, true, false }, timestamp)
            {
                Alias = 32,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test33", VersionBData.DataType.StringArray, new string [] { "1", "2", "3", "4" }, timestamp)
            {
                Alias = 33,
                IsHistorical = true,
                IsTransient = true
            },
            new("Test34", VersionBData.DataType.DateTimeArray, new DateTimeOffset [] { dateTime, dateTime2, dateTime3, dateTime4 }, timestamp)
            {
                Alias = 34,
                IsHistorical = true,
                IsTransient = true
            }
        };
        var convertedMetrics = new List<VersionBProtoBufPayload.Metric>
        {
            new()
            {
                Name = "Test1",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 1,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBProtoBuf.DataType.Int8,
                IntValue = 1
            },
            new()
            {
                Name = "Test2",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 2,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBProtoBuf.DataType.Int16,
                IntValue = 2
            },
            new()
            {
                Name = "Test3",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 3,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBProtoBuf.DataType.Int32,
                IntValue = 3
            },
            new()
            {
                Name = "Test4",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 4,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBProtoBuf.DataType.Int64,
                LongValue = 4
            },
            new()
            {
                Name = "Test5",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 5,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBProtoBuf.DataType.UInt8,
                IntValue = 5
            },
            new()
            {
                Name = "Test6",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 6,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBProtoBuf.DataType.UInt16,
                IntValue = 6
            },
            new()
            {
                Name = "Test7",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 7,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBProtoBuf.DataType.UInt32,
                LongValue = 7
            },
            new()
            {
                Name = "Test8",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 8,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBProtoBuf.DataType.UInt64,
                LongValue = 8
            },
            new()
            {
                Name = "Test9",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 9,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBProtoBuf.DataType.Float,
                FloatValue = 9
            },
            new()
            {
                Name = "Test10",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 10,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBProtoBuf.DataType.Double,
                DoubleValue = 10
            },
            new()
            {
                Name = "Test11",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 11,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBProtoBuf.DataType.Boolean,
                BooleanValue = true
            },
            new()
            {
                Name = "Test12",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 12,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBProtoBuf.DataType.String,
                StringValue = "12"
            },
            new()
            {
                Name = "Test13",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 13,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBProtoBuf.DataType.DateTime,
                LongValue = (ulong)timestamp.ToUnixTimeMilliseconds()
            },
            new()
            {
                Name = "Test14",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 14,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBProtoBuf.DataType.Text,
                StringValue = "14"
            },
            new()
            {
                Name = "Test15",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 15,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBProtoBuf.DataType.Uuid,
                StringValue = "c609f36e-92f9-4103-92f0-bf9d95c18be9"
            },
            new()
            {
                Name = "Test16",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 16,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBProtoBuf.DataType.DataSet,
                DataSetValue = new VersionBProtoBufPayload.DataSet
                {
                    Columns = ["Test1", "Test2"],
                    NumberOfColumns = 2,
                    Rows =
                    [
                        new VersionBProtoBufPayload.DataSet.Row
                        {
                            Elements =
                            [
                                new()
                                {
                                    IntValue = 1
                                },
                                new()
                                {
                                    FloatValue = 2
                                }
                            ]
                        }
                    ],
                    Types = [1, 9]
                }
            },
            new()
            {
                Name = "Test17",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 17,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBProtoBuf.DataType.Bytes,
                BytesValue = [1, 2, 3, 4]
            },
            new()
            {
                Name = "Test18",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 18,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                MetaData = new VersionBProtoBufPayload.MetaData
                {
                    ContentType = "application/json",
                    Description = "Test",
                    FileName = "Test.json",
                    FileType = "json",
                    IsMultiPart = true,
                    Md5 = "md5",
                    Seq = 1,
                    Size = 1
                },
                DataType = (uint?)VersionBProtoBuf.DataType.File,
                BytesValue = [1, 2, 3, 4]
            },
            new()
            {
                Name = "Test19",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 19,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBProtoBuf.DataType.Template,
                TemplateValue = new VersionBProtoBufPayload.Template
                {
                    IsDefinition = true,
                    Version = "1.0",
                    TemplateRef = "TestRef1",
                    Parameters =
                    [
                        new()
                        {
                            Name = "Test1",
                            DataType = (uint?)VersionBProtoBuf.DataType.Int8,
                            IntValue = 1,
                        },
                        new()
                        {
                            DataType = (uint?)VersionBProtoBuf.DataType.Float,
                            Name = "Test2",
                            FloatValue = 2
                        }
                    ],
                    Metrics =
                    [
                        new()
                        {
                            Name = "Test1",
                            Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                            Alias = 1,
                            IsHistorical = true,
                            IsTransient = true,
                            IsNull = false,
                            DataType = (uint?)VersionBProtoBuf.DataType.Int16,
                            IntValue = 1
                        },
                        new()
                        {
                            Name = "Test2",
                            Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                            Alias = 2,
                            IsHistorical = true,
                            IsTransient = true,
                            IsNull = false,
                            DataType = (uint?)VersionBProtoBuf.DataType.Double,
                            DoubleValue = 2
                        }
                    ]
                }
            },
            new()
            {
                Name = "Test20",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 20,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBProtoBuf.DataType.PropertySet,
                PropertySetValue = new VersionBProtoBufPayload.PropertySet
                {
                    Keys = ["Test1", "Test2"],
                    Values =
                    [
                        new()
                        {
                            IsNull = false,
                            DataType = (uint?)VersionBProtoBuf.DataType.Int8,
                            IntValue = 1
                        },
                        new()
                        {
                            IsNull = false,
                            DataType = (uint?)VersionBProtoBuf.DataType.Int64,
                            LongValue = 2
                        }
                    ]
                }
            },
            // Todo: How to handle this?!
            //new()
            //{
            //    Name = "Test21",
            //    Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
            //    Alias = 21,
            //    IsHistorical = true,
            //    IsTransient = true,
            //    IsNull = false,
            //    DataType = (uint?)VersionBProtoBuf.DataType.PropertySetList,
            //    PropertySetListValue = "c609f36e-92f9-4103-92f0-bf9d95c18be9"
            //},
            new()
            {
                Name = "Test22",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 22,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBProtoBuf.DataType.Int8Array,
                BytesValue = [ 1, 2, 3, 4 ]
            },
            new()
            {
                Name = "Test23",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 23,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBProtoBuf.DataType.Int16Array,
                BytesValue = [1, 0, 2, 0, 3, 0, 4, 0]
            },
            new()
            {
                Name = "Test24",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 24,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBProtoBuf.DataType.Int32Array,
                BytesValue = [1, 0, 0, 0, 2, 0, 0, 0, 3, 0, 0, 0, 4, 0, 0, 0]
            },
            new()
            {
                Name = "Test25",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 25,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBProtoBuf.DataType.Int64Array,
                BytesValue = [1, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 4, 0, 0, 0, 0, 0, 0, 0]
            },
            new()
            {
                Name = "Test26",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 26,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBProtoBuf.DataType.UInt8Array,
                BytesValue = [1, 2, 3, 4]
            },
            new()
            {
                Name = "Test27",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 27,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBProtoBuf.DataType.UInt16Array,
                BytesValue = [1, 0, 2, 0, 3, 0, 4, 0]
            },
            new()
            {
                Name = "Test28",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 28,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBProtoBuf.DataType.UInt32Array,
                BytesValue = [1, 0, 0, 0, 2, 0, 0, 0, 3, 0, 0, 0, 4, 0, 0, 0]
            },
            new()
            {
                Name = "Test29",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 29,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBProtoBuf.DataType.UInt64Array,
                BytesValue = [1, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 4, 0, 0, 0, 0, 0, 0, 0]
            },
            new()
            {
                Name = "Test30",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 30,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBProtoBuf.DataType.FloatArray,
                BytesValue = [0x00, 0x00, 0x80, 0x3F, 0x00, 0x00, 0x00, 0x40, 0x00, 0x00, 0x40, 0x40, 0x00, 0x00, 0x80, 0x40]
            },
            new()
            {
                Name = "Test31",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 31,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBProtoBuf.DataType.DoubleArray,
                BytesValue = [0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xf0, 0x3f, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x40, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x08, 0x40, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x40]
            },
            new()
            {
                Name = "Test32",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 32,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBProtoBuf.DataType.BooleanArray,
                BytesValue = [0x04, 0x00, 0x00, 0x00, 0xa0]
            },
            new()
            {
                Name = "Test33",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 33,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBProtoBuf.DataType.StringArray,
                BytesValue = [0x31, 0x00, 0x32, 0x00, 0x33, 0x00, 0x34, 0x00]
            },
            new()
            {
                Name = "Test34",
                Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
                Alias = 34,
                IsHistorical = true,
                IsTransient = true,
                IsNull = false,
                DataType = (uint?)VersionBProtoBuf.DataType.DateTimeArray,
                BytesValue = [0x00, 0xE8, 0x66, 0x5E, 0x6F, 0x01, 0x00, 0x00,
                              0x00, 0x9C, 0xEF, 0x12, 0x7E, 0x01, 0x00, 0x00, 
                              0x00, 0xC8, 0xA0, 0x6A, 0x85, 0x01, 0x00, 0x00, 
                              0x00, 0xF4, 0x51, 0xC2, 0x8C, 0x01, 0x00, 0x00]
            }
        };
        var oldPayload = new VersionBData.Payload
        {
            Body = bodyData,
            Timestamp = (ulong)timestamp.ToUnixTimeMilliseconds(),
            Seq = 1,
            Uuid = "477a41e5-f0ba-4b98-9522-95d44861d993",
            Metrics = metrics
        };
        var payload = VersionB.PayloadConverter.ConvertVersionBPayload(oldPayload);
        Assert.IsNotNull(payload);
        CollectionAssert.AreEqual(bodyData, payload.Body);
        Assert.AreEqual((ulong)timestamp.ToUnixTimeMilliseconds(), payload.Timestamp);
        Assert.AreEqual((ulong)1, payload.Seq);
        Assert.AreEqual("477a41e5-f0ba-4b98-9522-95d44861d993", payload.Uuid);
        Assert.AreEqual(convertedMetrics.Count, payload.Metrics.Count);

        var count = 0;

        foreach (var metric in payload.Metrics)
        {
            EqualityHelper.MetricEquals(convertedMetrics[count++], metric);
        }
    }
}
