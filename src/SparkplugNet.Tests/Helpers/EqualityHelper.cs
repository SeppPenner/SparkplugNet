// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EqualityHelper.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A helper class for the tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Tests.Helpers;

/// <summary>
/// A helper class for the tests.
/// </summary>
internal static class EqualityHelper
{
    /// <summary>
    /// Tests the two given metrics for equality.
    /// </summary>
    /// <param name="expectedMetric">The expected metric.</param>
    /// <param name="newMetric">The new metric.</param>
    public static void MetricEquals(VersionBProtoBufPayload.Metric expectedMetric, VersionBProtoBufPayload.Metric newMetric)
    {
        Assert.AreEqual(expectedMetric.Alias, newMetric.Alias);
        Assert.AreEqual(expectedMetric.BooleanValue, newMetric.BooleanValue);
        ByteArrayEquals(expectedMetric.BytesValue, newMetric.BytesValue);
        DataSetEquals(expectedMetric.DataSetValue, newMetric.DataSetValue);
        Assert.AreEqual(expectedMetric.DataType, newMetric.DataType);
        Assert.AreEqual(expectedMetric.DoubleValue, newMetric.DoubleValue);
        // Todo: How to handle extension value?
        //Assert.AreEqual(expectedMetric.ExtensionValue, newMetric.ExtensionValue);
        Assert.AreEqual(expectedMetric.FloatValue, newMetric.FloatValue);
        Assert.AreEqual(expectedMetric.IntValue, newMetric.IntValue);
        Assert.AreEqual(expectedMetric.IsHistorical, newMetric.IsHistorical);
        Assert.AreEqual(expectedMetric.IsNull, newMetric.IsNull);
        Assert.AreEqual(expectedMetric.IsTransient, newMetric.IsTransient);

        if (expectedMetric.DataType == (uint)VersionBProtoBuf.DataType.DateTime)
        {
            var expectedValue = DateTimeOffset.FromUnixTimeMilliseconds((long)expectedMetric.LongValue).DateTime;
            var newValue = DateTimeOffset.FromUnixTimeMilliseconds((long)newMetric.LongValue).DateTime;
            Assert.AreEqual(expectedValue, newValue);
        }
        else
        {
            Assert.AreEqual(expectedMetric.LongValue, newMetric.LongValue);
        }

        MetaDataEquals(expectedMetric.MetaData, newMetric.MetaData);
        Assert.AreEqual(expectedMetric.Name, newMetric.Name);
        PropertySetEquals(expectedMetric.PropertySetValue, newMetric.PropertySetValue);
        Assert.AreEqual(expectedMetric.StringValue, newMetric.StringValue);
        TemplateEquals(expectedMetric.TemplateValue, newMetric.TemplateValue);
        Assert.AreEqual(expectedMetric.Timestamp, newMetric.Timestamp);
    }

    /// <summary>
    /// Tests the two given metrics for equality.
    /// </summary>
    /// <param name="expectedMetric">The expected metric.</param>
    /// <param name="newMetric">The new metric.</param>
    public static void MetricEquals(VersionBData.Metric expectedMetric, VersionBData.Metric newMetric)
    {
        Assert.AreEqual(expectedMetric.Alias, newMetric.Alias);

        switch (newMetric.DataType)
        {
            case VersionBData.DataType.DateTime:
                Assert.AreEqual(expectedMetric.Value, newMetric.Value);
                break;
            case VersionBData.DataType.DataSet:
                var expectedDataSet = expectedMetric.Value as VersionBData.DataSet;
                var newDataSet = newMetric.Value as VersionBData.DataSet;

                if (expectedDataSet is not null && newDataSet is not null)
                {
                    DataSetEquals(expectedDataSet, newDataSet);
                }
                else if (expectedDataSet is null && newDataSet is null)
                {
                    // Do nothing here.
                }
                else
                {
                    Assert.Fail("The data set values are not equal.");
                }

                break;
            case VersionBData.DataType.Bytes:
            case VersionBData.DataType.File:
                ByteArrayEquals(expectedMetric.Value, newMetric.Value);
                break;
            case VersionBData.DataType.Template:
                var expectedTemplate = expectedMetric.Value as VersionBData.Template;
                var newTemplate = newMetric.Value as VersionBData.Template;

                if (expectedTemplate is not null && newTemplate is not null)
                {
                    TemplateEquals(expectedTemplate, newTemplate);
                }
                else if (expectedTemplate is null && newTemplate is null)
                {
                    // Do nothing here.
                }
                else
                {
                    Assert.Fail("The template values are not equal.");
                }

                break;

            case VersionBData.DataType.PropertySet:
                var expectedPropertySet = expectedMetric.Value as VersionBData.PropertySet;
                var newPropertySet = newMetric.Value as VersionBData.PropertySet;

                if (expectedPropertySet is not null && newPropertySet is not null)
                {
                    PropertySetEquals(expectedPropertySet, newPropertySet);
                }
                else if (expectedPropertySet is null && newPropertySet is null)
                {
                    // Do nothing here.
                }
                else
                {
                    Assert.Fail("The property set values are not equal.");
                }

                break;
            case VersionBData.DataType.Int8Array:
            case VersionBData.DataType.Int16Array:
            case VersionBData.DataType.Int32Array:
            case VersionBData.DataType.Int64Array:
            case VersionBData.DataType.UInt8Array:
            case VersionBData.DataType.UInt16Array:
            case VersionBData.DataType.UInt32Array:
            case VersionBData.DataType.UInt64Array:
            case VersionBData.DataType.FloatArray:
            case VersionBData.DataType.DoubleArray:
            case VersionBData.DataType.BooleanArray:
            case VersionBData.DataType.StringArray:
            case VersionBData.DataType.DateTimeArray:
                ByteArrayEquals(expectedMetric.Value, newMetric.Value);
                break;
            default:
                Assert.AreEqual(expectedMetric.Value, newMetric.Value);
                break;
        }

        Assert.AreEqual(expectedMetric.DataType, newMetric.DataType);
        Assert.AreEqual(expectedMetric.IsHistorical, newMetric.IsHistorical);
        Assert.AreEqual(expectedMetric.IsNull, newMetric.IsNull);
        Assert.AreEqual(expectedMetric.IsTransient, newMetric.IsTransient);
        MetaDataEquals(expectedMetric.MetaData, newMetric.MetaData);
        Assert.AreEqual(expectedMetric.Name, newMetric.Name);
        Assert.AreEqual(expectedMetric.Timestamp, newMetric.Timestamp);
    }

    /// <summary>
    /// Tests two given byte arrays for equality.
    /// </summary>
    /// <param name="expectedArray">The expected byte array.</param>
    /// <param name="newArray">The new byte array.</param>
    /// <exception cref="InvalidCastException">Thrown if the array type is not supported.</exception>
    private static void ByteArrayEquals(byte[] expectedArray, byte[] newArray)
    {
        if (expectedArray is not null && newArray is not null)
        {
            if (newArray.GetType() == typeof(sbyte[]))
            {
                var convertedExpectedArray = expectedArray.Select(b => (sbyte)b).ToArray();
                CollectionAssert.AreEqual(convertedExpectedArray, newArray);
            }
            else if (newArray.GetType() == typeof(short[]))
            {
                var convertedExpectedArray = expectedArray.Select(b => (short)b).ToArray();
                CollectionAssert.AreEqual(convertedExpectedArray, newArray);
            }
            else if (newArray.GetType() == typeof(int[]))
            {
                var convertedExpectedArray = expectedArray.Select(b => (short)b).ToArray();
                CollectionAssert.AreEqual(convertedExpectedArray, newArray);
            }
            else if (newArray.GetType() == typeof(long[]))
            {
                var convertedExpectedArray = expectedArray.Select(b => (short)b).ToArray();
                CollectionAssert.AreEqual(convertedExpectedArray, newArray);
            }
            else if (newArray.GetType() == typeof(byte[]))
            {
                CollectionAssert.AreEqual(expectedArray, newArray);
            }
            else if (newArray.GetType() == typeof(ushort[]))
            {
                var convertedExpectedArray = expectedArray.Select(b => (short)b).ToArray();
                CollectionAssert.AreEqual(convertedExpectedArray, newArray);
            }
            else if (newArray.GetType() == typeof(uint[]))
            {
                var convertedExpectedArray = expectedArray.Select(b => (short)b).ToArray();
                CollectionAssert.AreEqual(convertedExpectedArray, newArray);
            }
            else if (newArray.GetType() == typeof(ulong[]))
            {
                var convertedExpectedArray = expectedArray.Select(b => (short)b).ToArray();
                CollectionAssert.AreEqual(convertedExpectedArray, newArray);
            }
            else
            {
                throw new InvalidCastException("The array type is not supported.");
            }
        }
        else if (expectedArray is null && newArray is null)
        {
            // Do nothing here.
        }
        else
        {
            Assert.Fail("The byte array values are not equal.");
        }
    }

    /// <summary>
    /// Tests two given byte arrays for equality.
    /// </summary>
    /// <param name="expectedArray">The expected byte array.</param>
    /// <param name="newArray">The new byte array.</param>
    private static void ByteArrayEquals(object? expectedArray, object? newArray)
    {
        var expectedArrayAsByteArray = expectedArray as byte[];
        var newArrayAsByteArray = newArray as byte[];

        if (expectedArrayAsByteArray is not null && newArrayAsByteArray is not null)
        {
            ByteArrayEquals(expectedArrayAsByteArray, newArrayAsByteArray);
        }
        else if (expectedArrayAsByteArray is null && newArrayAsByteArray is null)
        {
            // Do nothing here.
        }
        else
        {
            Assert.Fail("The byte array values are not equal.");
        }
    }

    /// <summary>
    /// Tests the two given data sets for equality.
    /// </summary>
    /// <param name="expectedDataSet">The expected data set.</param>
    /// <param name="newDataSet">The new data set.</param>
    private static void DataSetEquals(VersionBProtoBufPayload.DataSet? expectedDataSet, VersionBProtoBufPayload.DataSet? newDataSet)
    {
        if (expectedDataSet is not null && newDataSet is not null)
        {
            CollectionAssert.AreEqual(expectedDataSet.Columns, newDataSet.Columns);
            Assert.AreEqual(expectedDataSet.NumberOfColumns, newDataSet.NumberOfColumns);

            // Check the rows.
            var rowIndex = 0;

            foreach (var row in newDataSet.Rows)
            {
                var expectedRow = expectedDataSet.Rows[rowIndex];
                var elementIndex = 0;

                // Check the elements in the row.
                foreach (var element in row.Elements)
                {
                    var expectedElement = expectedRow.Elements[elementIndex];
                    DataSetValueEquals(expectedElement, element);
                    elementIndex++;
                }

                rowIndex++;
            }

            CollectionAssert.AreEqual(expectedDataSet.Types, newDataSet.Types);
        }
        else if (expectedDataSet is null && newDataSet is null)
        {
            // Do nothing here.
        }
        else
        {
            Assert.Fail("The data set values are not equal.");
        }
    }

    /// <summary>
    /// Tests the two given data sets for equality.
    /// </summary>
    /// <param name="expectedDataSet">The expected data set.</param>
    /// <param name="newDataSet">The new data set.</param>
    private static void DataSetEquals(VersionBData.DataSet expectedDataSet, VersionBData.DataSet newDataSet)
    {
        CollectionAssert.AreEqual(expectedDataSet.Columns, newDataSet.Columns);
        Assert.AreEqual(expectedDataSet.NumberOfColumns, newDataSet.NumberOfColumns);

        // Check the rows.
        var rowIndex = 0;

        foreach (var row in newDataSet.Rows)
        {
            var expectedRow = expectedDataSet.Rows[rowIndex];
            var elementIndex = 0;

            // Check the elements in the row.
            foreach (var element in row.Elements)
            {
                var expectedElement = expectedRow.Elements[elementIndex];
                DataSetValueEquals(expectedElement, element);
                elementIndex++;
            }

            rowIndex++;
        }

        CollectionAssert.AreEqual(expectedDataSet.Types, newDataSet.Types);
    }

    /// <summary>
    /// Tests the two given data set values for equality.
    /// </summary>
    /// <param name="expectedDataSetValue">The expected data set value.</param>
    /// <param name="newDataSetValue">The new data set value.</param>
    private static void DataSetValueEquals(VersionBProtoBufPayload.DataSet.DataSetValue expectedDataSetValue, VersionBProtoBufPayload.DataSet.DataSetValue newDataSetValue)
    {
        Assert.AreEqual(expectedDataSetValue.BooleanValue, newDataSetValue.BooleanValue);
        Assert.AreEqual(expectedDataSetValue.DoubleValue, newDataSetValue.DoubleValue);
        // Todo: How to handle extension value?
        //Assert.AreEqual(expectedDataSetValue.ExtensionValue, newDataSetValue.ExtensionValue);
        Assert.AreEqual(expectedDataSetValue.FloatValue, newDataSetValue.FloatValue);
        Assert.AreEqual(expectedDataSetValue.IntValue, newDataSetValue.IntValue);
        Assert.AreEqual(expectedDataSetValue.LongValue, newDataSetValue.LongValue);
        Assert.AreEqual(expectedDataSetValue.StringValue, newDataSetValue.StringValue);
    }

    /// <summary>
    /// Tests the two given data set values for equality.
    /// </summary>
    /// <param name="expectedDataSetValue">The expected data set value.</param>
    /// <param name="newDataSetValue">The new data set value.</param>
    private static void DataSetValueEquals(VersionBData.DataSetValue expectedDataSetValue, VersionBData.DataSetValue newDataSetValue)
    {
        Assert.AreEqual(expectedDataSetValue.DataType, newDataSetValue.DataType);
        Assert.AreEqual(expectedDataSetValue.Value, newDataSetValue.Value);
    }

    /// <summary>
    /// Tests the two given meta data for equality.
    /// </summary>
    /// <param name="expectedMetaData">The expected meta data.</param>
    /// <param name="newMetaData">The new meta data.</param>
    private static void MetaDataEquals(VersionBProtoBufPayload.MetaData? expectedMetaData, VersionBProtoBufPayload.MetaData? newMetaData)
    {
        if (expectedMetaData is not null && newMetaData is not null)
        {
            Assert.AreEqual(expectedMetaData.IsMultiPart, newMetaData.IsMultiPart);
            Assert.AreEqual(expectedMetaData.ContentType, newMetaData.ContentType);
            Assert.AreEqual(expectedMetaData.Size, newMetaData.Size);
            Assert.AreEqual(expectedMetaData.Seq, newMetaData.Seq);
            Assert.AreEqual(expectedMetaData.FileName, newMetaData.FileName);
            Assert.AreEqual(expectedMetaData.FileType, newMetaData.FileType);
            Assert.AreEqual(expectedMetaData.Md5, newMetaData.Md5);
            Assert.AreEqual(expectedMetaData.Description, newMetaData.Description);
        }
        else if (expectedMetaData is null && newMetaData is null)
        {
            // Do nothing here.
        }
        else
        {
            Assert.Fail("The meta data values are not equal.");
        }
    }

    /// <summary>
    /// Tests the two given meta data for equality.
    /// </summary>
    /// <param name="expectedMetaData">The expected meta data.</param>
    /// <param name="newMetaData">The new meta data.</param>
    private static void MetaDataEquals(VersionBData.MetaData? expectedMetaData, VersionBData.MetaData? newMetaData)
    {
        if (expectedMetaData is not null && newMetaData is not null)
        {
            Assert.AreEqual(expectedMetaData.IsMultiPart, newMetaData.IsMultiPart);
            Assert.AreEqual(expectedMetaData.ContentType, newMetaData.ContentType);
            Assert.AreEqual(expectedMetaData.Size, newMetaData.Size);
            Assert.AreEqual(expectedMetaData.Seq, newMetaData.Seq);
            Assert.AreEqual(expectedMetaData.FileName, newMetaData.FileName);
            Assert.AreEqual(expectedMetaData.FileType, newMetaData.FileType);
            Assert.AreEqual(expectedMetaData.Md5, newMetaData.Md5);
            Assert.AreEqual(expectedMetaData.Description, newMetaData.Description);
        }
        else if (expectedMetaData is null && newMetaData is null)
        {
            // Do nothing here.
        }
        else
        {
            Assert.Fail("The meta data values are not equal.");
        }
    }

    /// <summary>
    /// Tests the two given property sets for equality.
    /// </summary>
    /// <param name="expectedPropertySetValue">The expected property set.</param>
    /// <param name="newPropertySetValue">The new property set.</param>
    private static void PropertySetEquals(VersionBProtoBufPayload.PropertySet? expectedPropertySetValue, VersionBProtoBufPayload.PropertySet? newPropertySetValue)
    {
        if (expectedPropertySetValue is not null && newPropertySetValue is not null)
        {
            CollectionAssert.AreEqual(expectedPropertySetValue.Keys, newPropertySetValue.Keys);

            var propertyIndex = 0;

            foreach (var propertyValue in newPropertySetValue.Values)
            {
                var expectedPropertyValue = expectedPropertySetValue.Values[propertyIndex];
                PropertyValueEquals(expectedPropertyValue, propertyValue);
                propertyIndex++;
            }
        }
        else if (expectedPropertySetValue is null && newPropertySetValue is null)
        {
            // Do nothing here.
        }
        else
        {
            Assert.Fail("The property set values are not equal.");
        }
    }

    /// <summary>
    /// Tests the two given property sets for equality.
    /// </summary>
    /// <param name="expectedPropertySetValue">The expected property set.</param>
    /// <param name="newPropertySetValue">The new property set.</param>
    private static void PropertySetEquals(VersionBData.PropertySet? expectedPropertySetValue, VersionBData.PropertySet? newPropertySetValue)
    {
        if (expectedPropertySetValue is not null && newPropertySetValue is not null)
        {
            CollectionAssert.AreEqual(expectedPropertySetValue.Keys, newPropertySetValue.Keys);

            var propertyIndex = 0;

            foreach (var propertyValue in newPropertySetValue.Values)
            {
                var expectedPropertyValue = expectedPropertySetValue.Values[propertyIndex];
                PropertyValueEquals(expectedPropertyValue, propertyValue);
                propertyIndex++;
            }
        }
        else if (expectedPropertySetValue is null && newPropertySetValue is null)
        {
            // Do nothing here.
        }
        else
        {
            Assert.Fail("The property set values are not equal.");
        }
    }

    /// <summary>
    /// Tests the two given property values for equality.
    /// </summary>
    /// <param name="expectedPropertyValue">The expected property value.</param>
    /// <param name="newPropertyValue">The new property value.</param>
    private static void PropertyValueEquals(VersionBProtoBufPayload.PropertyValue expectedPropertyValue, VersionBProtoBufPayload.PropertyValue newPropertyValue)
    {
        Assert.AreEqual(expectedPropertyValue.BooleanValue, newPropertyValue.BooleanValue);
        Assert.AreEqual(expectedPropertyValue.DataType, newPropertyValue.DataType);
        Assert.AreEqual(expectedPropertyValue.DoubleValue, newPropertyValue.DoubleValue);
        // Todo: How to handle extension value?
        //Assert.AreEqual(expectedDataSetValue.ExtensionValue, newDataSetValue.ExtensionValue);
        Assert.AreEqual(expectedPropertyValue.FloatValue, newPropertyValue.FloatValue);
        Assert.AreEqual(expectedPropertyValue.IntValue, newPropertyValue.IntValue);
        Assert.AreEqual(expectedPropertyValue.IsNull, newPropertyValue.IsNull);
        Assert.AreEqual(expectedPropertyValue.LongValue, newPropertyValue.LongValue);

        if (expectedPropertyValue.PropertySetListValue is not null && newPropertyValue.PropertySetListValue is not null)
        {
            var propertyIndex = 0;

            foreach (var propertySetValue in newPropertyValue.PropertySetListValue.PropertySets)
            {
                var expectedPropertySetValue = expectedPropertyValue.PropertySetListValue.PropertySets[propertyIndex];
                PropertySetEquals(expectedPropertySetValue, propertySetValue);
                propertyIndex++;
            }
        }
        else if (expectedPropertyValue.PropertySetListValue is null && newPropertyValue.PropertySetListValue is null)
        {
            // Do nothing here.
        }
        else
        {
            Assert.Fail("The property set list values are not equal.");
        }

        PropertySetEquals(expectedPropertyValue.PropertySetValue, newPropertyValue.PropertySetValue);
        Assert.AreEqual(expectedPropertyValue.StringValue, newPropertyValue.StringValue);
    }

    /// <summary>
    /// Tests the two given property values for equality.
    /// </summary>
    /// <param name="expectedPropertyValue">The expected property value.</param>
    /// <param name="newPropertyValue">The new property value.</param>
    private static void PropertyValueEquals(VersionBData.PropertyValue expectedPropertyValue, VersionBData.PropertyValue newPropertyValue)
    {
        Assert.AreEqual(expectedPropertyValue.DataType, newPropertyValue.DataType);
        Assert.AreEqual(expectedPropertyValue.IsNull, newPropertyValue.IsNull);
        Assert.AreEqual(expectedPropertyValue.Value, newPropertyValue.Value);
    }

    /// <summary>
    /// Tests the two given templates for equality.
    /// </summary>
    /// <param name="expectedTemplate">The expected template.</param>
    /// <param name="newTemplate">The new template.</param>
    private static void TemplateEquals(VersionBProtoBufPayload.Template? expectedTemplate, VersionBProtoBufPayload.Template? newTemplate)
    {
        if (expectedTemplate is not null && newTemplate is not null)
        {
            Assert.AreEqual(expectedTemplate.IsDefinition, newTemplate.IsDefinition);

            var metricIndex = 0;

            foreach (var expectedMetric in expectedTemplate.Metrics)
            {
                var newMetric = newTemplate.Metrics[metricIndex];
                MetricEquals(expectedMetric, newMetric);
                metricIndex++;
            }

            var parameterIndex = 0;

            foreach (var expectedParameter in expectedTemplate.Parameters)
            {
                var newParameter = newTemplate.Parameters[parameterIndex];
                ParameterEquals(expectedParameter, newParameter);
                parameterIndex++;
            }

            Assert.AreEqual(expectedTemplate.TemplateRef, newTemplate.TemplateRef);
            Assert.AreEqual(expectedTemplate.Version, newTemplate.Version);
        }
        else if (expectedTemplate is null && newTemplate is null)
        {
            // Do nothing here.
        }
        else
        {
            Assert.Fail("The template values are not equal.");
        }
    }

    /// <summary>
    /// Tests the two given templates for equality.
    /// </summary>
    /// <param name="expectedTemplate">The expected template.</param>
    /// <param name="newTemplate">The new template.</param>
    private static void TemplateEquals(VersionBData.Template expectedTemplate, VersionBData.Template newTemplate)
    {
        Assert.AreEqual(expectedTemplate.Version, newTemplate.Version);
        var metricIndex = 0;

        foreach (var expectedMetric in expectedTemplate.Metrics)
        {
            var newMetric = newTemplate.Metrics[metricIndex];
            MetricEquals(expectedMetric, newMetric);
            metricIndex++;
        }

        var parameterIndex = 0;

        foreach (var expectedParameter in expectedTemplate.Parameters)
        {
            var newParameter = newTemplate.Parameters[parameterIndex];
            ParameterEquals(expectedParameter, newParameter);
            parameterIndex++;
        }

        Assert.AreEqual(expectedTemplate.TemplateRef, newTemplate.TemplateRef);
        Assert.AreEqual(expectedTemplate.IsDefinition, newTemplate.IsDefinition);
    }

    /// <summary>
    /// Tests the two given parameters for equality.
    /// </summary>
    /// <param name="expectedParameter">The expected parameter.</param>
    /// <param name="newParameter">The new parameter.</param>
    private static void ParameterEquals(VersionBProtoBufPayload.Template.Parameter? expectedParameter, VersionBProtoBufPayload.Template.Parameter? newParameter)
    {
        if (expectedParameter is not null && newParameter is not null)
        {
            Assert.AreEqual(expectedParameter.BooleanValue, newParameter.BooleanValue);
            Assert.AreEqual(expectedParameter.DataType, newParameter.DataType);
            Assert.AreEqual(expectedParameter.DoubleValue, newParameter.DoubleValue);
            // Todo: How to handle extension value?
            //Assert.AreEqual(expectedParameter.ExtensionValue, newParameter.ExtensionValue);
            Assert.AreEqual(expectedParameter.FloatValue, newParameter.FloatValue);
            Assert.AreEqual(expectedParameter.IntValue, newParameter.IntValue);
            Assert.AreEqual(expectedParameter.LongValue, newParameter.LongValue);
            Assert.AreEqual(expectedParameter.Name, newParameter.Name);
            Assert.AreEqual(expectedParameter.StringValue, newParameter.StringValue);
        }
        else if (expectedParameter is null && newParameter is null)
        {
            // Do nothing here.
        }
        else
        {
            Assert.Fail("The parameter values are not equal.");
        }
    }

    /// <summary>
    /// Tests the two given parameters for equality.
    /// </summary>
    /// <param name="expectedParameter">The expected parameter.</param>
    /// <param name="newParameter">The new parameter.</param>
    private static void ParameterEquals(VersionBData.Parameter expectedParameter, VersionBData.Parameter newParameter)
    {
        Assert.AreEqual(expectedParameter.DataType, newParameter.DataType);
        Assert.AreEqual(expectedParameter.Name, newParameter.Name);
        Assert.AreEqual(expectedParameter.Value, newParameter.Value);
    }
}
