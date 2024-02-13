// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugMessageGeneratorTest.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class to test the <see cref="VersionB.PayloadConverter"/> class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Tests;

/// <summary>
/// A class to test the <see cref="VersionB.PayloadConverter"/> class.
/// </summary>
[TestClass]
public class SparkplugPayloadConverterTestVersionB
{
    /// <summary>
    /// Tests the Sparkplug payload converter for converting a version B payload from Proto.
    /// </summary>
    [TestMethod]
    public void TestConvertVersionBPayloadFromProto()
    {
        var dateTime = new DateTimeOffset(2021, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var dateTime2 = new DateTimeOffset(2022, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var bodyData = new byte[] { 1, 2, 3, 4 };
        var metrics = new List<VersionBProtoBufPayload.Metric>
        {
            new()
            {
                Name = "Test1",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 1,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = (uint?)VersionBDataTypeEnum.Int8,
                IntValue = 1
            },
            new()
            {
                Name = "Test2",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 2,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = (uint?)VersionBDataTypeEnum.Int16,
                IntValue = 2
            },
            new()
            {
                Name = "Test3",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 3,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = (uint?)VersionBDataTypeEnum.Int32,
                IntValue = 3
            },
            new()
            {
                Name = "Test4",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 4,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = (uint?)VersionBDataTypeEnum.Int64,
                LongValue = 4
            },
            new()
            {
                Name = "Test5",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 5,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = (uint?)VersionBDataTypeEnum.UInt8,
                IntValue = 5
            },
            new()
            {
                Name = "Test6",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 6,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = (uint?)VersionBDataTypeEnum.UInt16,
                IntValue = 6
            },
            new()
            {
                Name = "Test7",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 7,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = (uint?)VersionBDataTypeEnum.UInt32,
                LongValue = 7
            },
            new()
            {
                Name = "Test8",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 8,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = (uint?)VersionBDataTypeEnum.UInt64,
                LongValue = 8
            },
            new()
            {
                Name = "Test9",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 9,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = (uint?)VersionBDataTypeEnum.Float,
                FloatValue = 9
            },
            new()
            {
                Name = "Test10",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 10,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = (uint?)VersionBDataTypeEnum.Double,
                DoubleValue = 10
            },
            new()
            {
                Name = "Test11",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 11,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = (uint?)VersionBDataTypeEnum.Boolean,
                BooleanValue = true
            },
            new()
            {
                Name = "Test12",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 12,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = (uint?)VersionBDataTypeEnum.String,
                StringValue = "12"
            },
            new()
            {
                Name = "Test13",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 13,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = (uint?)VersionBDataTypeEnum.DateTime,
                LongValue = 13
            },
            new()
            {
                Name = "Test13",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 13,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = (uint?)VersionBDataTypeEnum.DateTime,
                LongValue = 13
            },
            new()
            {
                Name = "Test14",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 14,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = (uint?)VersionBDataTypeEnum.Text,
                StringValue = "14"
            },
            new()
            {
                Name = "Test15",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 15,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = (uint?)VersionBDataTypeEnum.Text,
                StringValue = "c609f36e-92f9-4103-92f0-bf9d95c18be9"
            },
            new()
            {
                Name = "Test16",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 16,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = (uint?)VersionBDataTypeEnum.DataSet,
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
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 17,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = (uint?)VersionBDataTypeEnum.Bytes,
                BytesValue = [1, 2, 3, 4]
            },
            new()
            {
                Name = "Test18",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 18,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
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
                DataType = (uint?)VersionBDataTypeEnum.File,
                BytesValue = [1, 2, 3, 4]
            },
            new()
            {
                Name = "Test19",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 19,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = (uint?)VersionBDataTypeEnum.Template,
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
                            DataType = (uint?)VersionBDataTypeEnum.Int8,
                            IntValue = 1
                        },
                        new()
                        {
                            DataType = (uint?)VersionBDataTypeEnum.Float,
                            Name = "Test2",
                            FloatValue = 2
                        }
                    ],
                    Metrics =
                    [
                        new()
                        {
                            Name = "Test1",
                            Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                            Alias = 1,
                            IsHistorical = true,
                            IsTransient = true,
                            IsNull = true,
                            DataType = (uint?)VersionBDataTypeEnum.Int16,
                            IntValue = 1
                        },
                        new()
                        {
                            Name = "Test2",
                            Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                            Alias = 2,
                            IsHistorical = true,
                            IsTransient = true,
                            IsNull = true,
                            DataType = (uint?)VersionBDataTypeEnum.Double,
                            DoubleValue = 1
                        }
                    ]
                }
            },
            new()
            {
                Name = "Test20",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 20,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = (uint?)VersionBDataTypeEnum.PropertySet,
                PropertySetValue = new VersionBProtoBufPayload.PropertySet
                {
                    Keys = ["Test1", "Test2"],
                    Values =
                    [
                        new()
                        {
                            IsNull = true,
                            DataType = (uint?)VersionBDataTypeEnum.Int8,
                            IntValue = 1
                        },
                        new()
                        {
                            IsNull = true,
                            DataType = (uint?)VersionBDataTypeEnum.Int64,
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
            //    IsNull = true,
            //    DataType = (uint?)VersionBDataTypeEnum.PropertySetList,
            //    PropertySetListValue = "c609f36e-92f9-4103-92f0-bf9d95c18be9"
            //},
            new()
            {
                Name = "Test22",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 22,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = (uint?)VersionBDataTypeEnum.Int8Array,
                BytesValue = [1, 2, 3, 4]
            },
            new()
            {
                Name = "Test23",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 23,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = (uint?)VersionBDataTypeEnum.Int16Array,
                BytesValue = [1, 2, 3, 4]
            },
            new()
            {
                Name = "Test24",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 24,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = (uint?)VersionBDataTypeEnum.Int32Array,
                BytesValue = [1, 2, 3, 4]
            },
            new()
            {
                Name = "Test25",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 25,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = (uint?)VersionBDataTypeEnum.Int64Array,
                BytesValue = [1, 2, 3, 4]
            },
            new()
            {
                Name = "Test26",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 26,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = (uint?)VersionBDataTypeEnum.UInt8Array,
                BytesValue = [1, 2, 3, 4]
            },
            new()
            {
                Name = "Test27",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 27,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = (uint?)VersionBDataTypeEnum.UInt16Array,
                BytesValue = [1, 2, 3, 4]
            },
            new()
            {
                Name = "Test28",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 28,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = (uint?)VersionBDataTypeEnum.UInt32Array,
                BytesValue = [1, 2, 3, 4]
            },
            new()
            {
                Name = "Test29",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 29,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = (uint?)VersionBDataTypeEnum.UInt64Array,
                BytesValue = [1, 2, 3, 4]
            },
            new()
            {
                Name = "Test30",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 30,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = (uint?)VersionBDataTypeEnum.FloatArray,
                BytesValue = [1, 2, 3, 4]
            },
            new()
            {
                Name = "Test31",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 31,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = (uint?)VersionBDataTypeEnum.DoubleArray,
                BytesValue = [1, 2, 3, 4]
            },
            new()
            {
                Name = "Test32",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 32,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = (uint?)VersionBDataTypeEnum.BooleanArray,
                BytesValue = [1, 2, 3, 4]
            },
            new()
            {
                Name = "Test33",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 33,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = (uint?)VersionBDataTypeEnum.StringArray,
                BytesValue = [1, 2, 3, 4]
            },
            new()
            {
                Name = "Test34",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 34,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = (uint?)VersionBDataTypeEnum.DateTimeArray,
                BytesValue = [1, 2, 3, 4]
            }
        };
        var convertedMetrics = new List<VersionB.Data.Metric>
        {
            new()
            {
                Name = "Test1",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 1,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = VersionBDataTypeEnum.Int8,
                IntValue = 1
            },
            new()
            {
                Name = "Test2",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 2,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = VersionBDataTypeEnum.Int16,
                IntValue = 2
            },
            new()
            {
                Name = "Test3",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 3,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = VersionBDataTypeEnum.Int32,
                IntValue = 3
            },
            new()
            {
                Name = "Test4",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 4,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = VersionBDataTypeEnum.Int64,
                LongValue = 4
            },
            new()
            {
                Name = "Test5",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 5,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = VersionBDataTypeEnum.UInt8,
                IntValue = 5
            },
            new()
            {
                Name = "Test6",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 6,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = VersionBDataTypeEnum.UInt16,
                IntValue = 6
            },
            new()
            {
                Name = "Test7",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 7,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = VersionBDataTypeEnum.UInt32,
                LongValue = 7
            },
            new()
            {
                Name = "Test8",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 8,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = VersionBDataTypeEnum.UInt64,
                LongValue = 8
            },
            new()
            {
                Name = "Test9",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 9,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = VersionBDataTypeEnum.Float,
                FloatValue = 9
            },
            new()
            {
                Name = "Test10",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 10,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = VersionBDataTypeEnum.Double,
                DoubleValue = 10
            },
            new()
            {
                Name = "Test11",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 11,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = VersionBDataTypeEnum.Boolean,
                BooleanValue = true
            },
            new()
            {
                Name = "Test12",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 12,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = VersionBDataTypeEnum.String,
                StringValue = "12"
            },
            new()
            {
                Name = "Test13",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 13,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = VersionBDataTypeEnum.DateTime,
                LongValue = 13
            },
            new()
            {
                Name = "Test13",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 13,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = VersionBDataTypeEnum.DateTime,
                LongValue = 13
            },
            new()
            {
                Name = "Test14",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 14,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = VersionBDataTypeEnum.Text,
                StringValue = "14"
            },
            new()
            {
                Name = "Test15",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 15,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = VersionBDataTypeEnum.Text,
                StringValue = "c609f36e-92f9-4103-92f0-bf9d95c18be9"
            },
            new()
            {
                Name = "Test16",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 16,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = VersionBDataTypeEnum.DataSet,
                DataSetValue = new VersionB.Data.DataSet
                {
                    Columns = ["Test1", "Test2"],
                    NumberOfColumns = 2,
                    Rows =
                    [
                        new VersionB.Data.Row
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
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 17,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = VersionBDataTypeEnum.Bytes,
                BytesValue = [1, 2, 3, 4]
            },
            new()
            {
                Name = "Test18",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 18,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                MetaData = new VersionB.Data.MetaData
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
                DataType = VersionBDataTypeEnum.File,
                BytesValue = [1, 2, 3, 4]
            },
            new()
            {
                Name = "Test19",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 19,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = VersionBDataTypeEnum.Template,
                TemplateValue = new VersionB.Data.Template
                {
                    IsDefinition = true,
                    Version = "1.0",
                    TemplateRef = "TestRef1",
                    Parameters =
                    [
                        new()
                        {
                            Name = "Test1",
                            DataType = VersionBDataTypeEnum.Int8,
                            IntValue = 1
                        },
                        new()
                        {
                            DataType = VersionBDataTypeEnum.Float,
                            Name = "Test2",
                            FloatValue = 2
                        }
                    ],
                    Metrics =
                    [
                        new()
                        {
                            Name = "Test1",
                            Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                            Alias = 1,
                            IsHistorical = true,
                            IsTransient = true,
                            IsNull = true,
                            DataType = VersionBDataTypeEnum.Int16,
                            IntValue = 1
                        },
                        new()
                        {
                            Name = "Test2",
                            Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                            Alias = 2,
                            IsHistorical = true,
                            IsTransient = true,
                            IsNull = true,
                            DataType = VersionBDataTypeEnum.Double,
                            DoubleValue = 1
                        }
                    ]
                }
            },
            new()
            {
                Name = "Test20",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 20,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = VersionBDataTypeEnum.PropertySet,
                PropertySetValue = new VersionB.Data.PropertySet
                {
                    Keys = ["Test1", "Test2"],
                    Values =
                    [
                        new()
                        {
                            IsNull = true,
                            DataType = VersionBDataTypeEnum.Int8,
                            IntValue = 1
                        },
                        new()
                        {
                            IsNull = true,
                            DataType = VersionBDataTypeEnum.Int64,
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
            //    IsNull = true,
            //    DataType = VersionBDataTypeEnum.PropertySetList,
            //    PropertySetListValue = "c609f36e-92f9-4103-92f0-bf9d95c18be9"
            //},
            new()
            {
                Name = "Test22",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 22,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = VersionBDataTypeEnum.Int8Array,
                BytesValue = [1, 2, 3, 4]
            },
            new()
            {
                Name = "Test23",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 23,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = VersionBDataTypeEnum.Int16Array,
                BytesValue = [1, 2, 3, 4]
            },
            new()
            {
                Name = "Test24",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 24,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = VersionBDataTypeEnum.Int32Array,
                BytesValue = [1, 2, 3, 4]
            },
            new()
            {
                Name = "Test25",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 25,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = VersionBDataTypeEnum.Int64Array,
                BytesValue = [1, 2, 3, 4]
            },
            new()
            {
                Name = "Test26",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 26,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = VersionBDataTypeEnum.UInt8Array,
                BytesValue = [1, 2, 3, 4]
            },
            new()
            {
                Name = "Test27",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 27,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = VersionBDataTypeEnum.UInt16Array,
                BytesValue = [1, 2, 3, 4]
            },
            new()
            {
                Name = "Test28",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 28,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = VersionBDataTypeEnum.UInt32Array,
                BytesValue = [1, 2, 3, 4]
            },
            new()
            {
                Name = "Test29",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 29,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = VersionBDataTypeEnum.UInt64Array,
                BytesValue = [1, 2, 3, 4]
            },
            new()
            {
                Name = "Test30",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 30,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = VersionBDataTypeEnum.FloatArray,
                BytesValue = [1, 2, 3, 4]
            },
            new()
            {
                Name = "Test31",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 31,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = VersionBDataTypeEnum.DoubleArray,
                BytesValue = [1, 2, 3, 4]
            },
            new()
            {
                Name = "Test32",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 32,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = VersionBDataTypeEnum.BooleanArray,
                BytesValue = [1, 2, 3, 4]
            },
            new()
            {
                Name = "Test33",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 33,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = VersionBDataTypeEnum.StringArray,
                BytesValue = [1, 2, 3, 4]
            },
            new()
            {
                Name = "Test34",
                Timestamp = (ulong)dateTime2.ToUnixTimeMilliseconds(),
                Alias = 34,
                IsHistorical = true,
                IsTransient = true,
                IsNull = true,
                DataType = VersionBDataTypeEnum.DateTimeArray,
                BytesValue = [1, 2, 3, 4]
            }
        };
        var oldPayload = new VersionBProtoBufPayload
        {
            Body = bodyData,
            Timestamp = (ulong)dateTime.ToUnixTimeMilliseconds(),
            Seq = 1,
            Uuid = "477a41e5-f0ba-4b98-9522-95d44861d993",
            Metrics = metrics
        };
        var payload = VersionB.PayloadConverter.ConvertVersionBPayload(oldPayload);
        Assert.IsNotNull(payload);
        CollectionAssert.AreEqual(bodyData, payload.Body);
        Assert.AreEqual((ulong)dateTime.ToUnixTimeMilliseconds(), payload.Timestamp);
        Assert.AreEqual((ulong)1, payload.Seq);
        Assert.AreEqual("477a41e5-f0ba-4b98-9522-95d44861d993", payload.Uuid);
        CollectionAssert.AreEqual(convertedMetrics, payload.Metrics);
    }

    /// <summary>
    /// Tests the Sparkplug payload converter for converting a version B payload to Proto.
    /// </summary>
    [TestMethod]
    public void TestConvertVersionBPayloadToProto()
    {
        var oldPayload = new VersionB.Data.Payload();
        var payload = VersionB.PayloadConverter.ConvertVersionBPayload(oldPayload);
        Assert.IsNotNull(payload);
    }

    /// <summary>
    /// Tests the Sparkplug payload converter for converting a version B data type property value to Proto with byte data in the metrics.
    /// Test code for https://github.com/SeppPenner/SparkplugNet/issues/30.
    /// </summary>
    [TestMethod]
    public void TestConvertVersionBMetricsWithByteValues()
    {
        var oldPayload = new VersionB.Data.Payload
        {
            Metrics =
            [
                new VersionB.Data.Metric()
                {
                    BytesValue = [1, 2, 3, 4]
                }
            ],
            Seq = 1,
            Timestamp = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };

        var payload = VersionB.PayloadConverter.ConvertVersionBPayload(oldPayload);

        Assert.IsNotNull(payload);
        Assert.IsNotNull(payload.Metrics);
        Assert.IsTrue(payload.Metrics.Count > 0);
        Assert.IsNotNull(payload.Metrics[0]);
        CollectionAssert.AreEqual(new byte[] { 1, 2, 3, 4 }, payload.Metrics[0].BytesValue);
        Assert.AreEqual((uint)17, payload.Metrics[0].DataType);
    }
}
