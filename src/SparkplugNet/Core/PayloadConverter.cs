// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PayloadConverter.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A helper class for the payload conversions from internal ProtoBuf model to external data and vice versa.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Core;

/// <summary>
/// A helper class for the payload conversions from internal ProtoBuf model to external data and vice versa.
/// </summary>
internal static class PayloadConverter
{
    /// <summary>
    /// Gets the version B payload converted from the ProtoBuf payload.
    /// </summary>
    /// <param name="payload">The <see cref="VersionBProtoBuf.ProtoBufPayload"/>.</param>
    /// <returns>The <see cref="VersionBData.Payload"/>.</returns>
    public static VersionBData.Payload ConvertVersionBPayload(VersionBProtoBuf.ProtoBufPayload payload)
        => new VersionBData.Payload
        {
            Body = payload.Body,
            Details = payload.Details,
            Metrics = payload.Metrics.Select(ConvertVersionBMetric).ToList(),
            Seq = payload.Seq,
            Timestamp = payload.Timestamp,
            Uuid = payload.Uuid
        };

    /// <summary>
    /// Gets the ProtoBuf payload converted from the version B payload.
    /// </summary>
    /// <param name="payload">The <see cref="VersionBData.Payload"/>.</param>
    /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload"/>.</returns>
    public static VersionBProtoBuf.ProtoBufPayload ConvertVersionBPayload(VersionBData.Payload payload)
        => new VersionBProtoBuf.ProtoBufPayload
        {
            Body = payload.Body,
            Details = payload.Details,
            Metrics = payload.Metrics.Select(ConvertVersionBMetric).ToList(),
            Seq = payload.Seq,
            Timestamp = payload.Timestamp,
            Uuid = payload.Uuid
        };

    /// <summary>
    /// Gets the version B data type from the version B ProtoBuf value type for data set values.
    /// </summary>
    /// <param name="type">The <see cref="VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase"/>.</param>
    /// <returns>The <see cref="VersionBDataTypeEnum"/>.</returns>
    public static VersionBDataTypeEnum ConvertVersionBDataTypeDataSetValue(VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase type)
        => type switch
        {
            VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.None => VersionBDataTypeEnum.Unknown,
            VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.IntValue => VersionBDataTypeEnum.Int32,
            VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.UintValue => VersionBDataTypeEnum.UInt32,
            VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.LongValue => VersionBDataTypeEnum.Int64,
            VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.UlongValue => VersionBDataTypeEnum.UInt64,
            VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.FloatValue => VersionBDataTypeEnum.Float,
            VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.DoubleValue => VersionBDataTypeEnum.Double,
            VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.BooleanValue => VersionBDataTypeEnum.Boolean,
            VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.StringValue => VersionBDataTypeEnum.String,
            VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.ExtensionValue => VersionBDataTypeEnum.Unknown,
            _ => VersionBDataTypeEnum.String
        };

    /// <summary>
    /// Gets the version B ProtoBuf value type from the version B data type for data set values.
    /// </summary>
    /// <param name="type">The <see cref="VersionBDataTypeEnum"/>.</param>
    /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase"/>.</returns>
    public static VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase ConvertVersionBDataTypeDataSetValue(VersionBDataTypeEnum type)
        => type switch
        {
            VersionBDataTypeEnum.Unknown => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.ExtensionValue,
            VersionBDataTypeEnum.Int8 => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.IntValue,
            VersionBDataTypeEnum.Int16 => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.IntValue,
            VersionBDataTypeEnum.Int32 => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.IntValue,
            VersionBDataTypeEnum.Int64 => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.LongValue,
            VersionBDataTypeEnum.UInt8 => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.UintValue,
            VersionBDataTypeEnum.UInt16 => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.UintValue,
            VersionBDataTypeEnum.UInt32 => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.UintValue,
            VersionBDataTypeEnum.UInt64 => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.UlongValue,
            VersionBDataTypeEnum.Float => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.FloatValue,
            VersionBDataTypeEnum.Double => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.DoubleValue,
            VersionBDataTypeEnum.Boolean => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.BooleanValue,
            VersionBDataTypeEnum.String => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.StringValue,
            VersionBDataTypeEnum.DateTime => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.UlongValue,
            VersionBDataTypeEnum.Text => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.StringValue,
            VersionBDataTypeEnum.Uuid => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.StringValue,
            VersionBDataTypeEnum.DataSet => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.None,
            VersionBDataTypeEnum.Bytes => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.ExtensionValue,
            VersionBDataTypeEnum.File => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.ExtensionValue,
            VersionBDataTypeEnum.Template => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.None,
            VersionBDataTypeEnum.PropertySet => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.None,
            VersionBDataTypeEnum.PropertySetList => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.None,
            _ => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.None
        };

    /// <summary>
    /// Gets the version B data type from the version B ProtoBuf value type for metrics.
    /// </summary>
    /// <param name="type">The <see cref="VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase"/>.</param>
    /// <returns>The <see cref="VersionBDataTypeEnum"/>.</returns>
    public static VersionBDataTypeEnum ConvertVersionBDataTypeMetric(VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase type)
        => type switch
        {
            VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.None => VersionBDataTypeEnum.Unknown,
            VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.IntValue => VersionBDataTypeEnum.Int32,
            VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.UintValue => VersionBDataTypeEnum.UInt32,
            VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.LongValue => VersionBDataTypeEnum.Int64,
            VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.UlongValue => VersionBDataTypeEnum.UInt64,
            VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.FloatValue => VersionBDataTypeEnum.Float,
            VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.DoubleValue => VersionBDataTypeEnum.Double,
            VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.BooleanValue => VersionBDataTypeEnum.Boolean,
            VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.StringValue => VersionBDataTypeEnum.String,
            VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.BytesValue => VersionBDataTypeEnum.Bytes,
            VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.DatasetValue => VersionBDataTypeEnum.DataSet,
            VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.TemplateValue => VersionBDataTypeEnum.Template,
            VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.ExtensionValue => VersionBDataTypeEnum.Unknown,
            _ => VersionBDataTypeEnum.String
        };

    /// <summary>
    /// Gets the version B ProtoBuf value type from the version B data type for metrics.
    /// </summary>
    /// <param name="type">The <see cref="VersionBDataTypeEnum"/>.</param>
    /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase"/>.</returns>
    public static VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase ConvertVersionBDataTypeMetric(VersionBDataTypeEnum type)
        => type switch
        {
            VersionBDataTypeEnum.Unknown => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.ExtensionValue,
            VersionBDataTypeEnum.Int8 => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.IntValue,
            VersionBDataTypeEnum.Int16 => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.IntValue,
            VersionBDataTypeEnum.Int32 => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.IntValue,
            VersionBDataTypeEnum.Int64 => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.LongValue,
            VersionBDataTypeEnum.UInt8 => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.UintValue,
            VersionBDataTypeEnum.UInt16 => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.UintValue,
            VersionBDataTypeEnum.UInt32 => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.UintValue,
            VersionBDataTypeEnum.UInt64 => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.UlongValue,
            VersionBDataTypeEnum.Float => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.FloatValue,
            VersionBDataTypeEnum.Double => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.DoubleValue,
            VersionBDataTypeEnum.Boolean => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.BooleanValue,
            VersionBDataTypeEnum.String => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.StringValue,
            VersionBDataTypeEnum.DateTime => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.UlongValue,
            VersionBDataTypeEnum.Text => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.StringValue,
            VersionBDataTypeEnum.Uuid => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.StringValue,
            VersionBDataTypeEnum.DataSet => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.None,
            VersionBDataTypeEnum.Bytes => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.ExtensionValue,
            VersionBDataTypeEnum.File => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.ExtensionValue,
            VersionBDataTypeEnum.Template => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.None,
            VersionBDataTypeEnum.PropertySet => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.None,
            VersionBDataTypeEnum.PropertySetList => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.None,
            _ => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.None
        };

    /// <summary>
    /// Gets the version B data type from the version B ProtoBuf value type for parameters.
    /// </summary>
    /// <param name="type">The <see cref="VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase"/>.</param>
    /// <returns>The <see cref="VersionBDataTypeEnum"/>.</returns>
    public static VersionBDataTypeEnum ConvertVersionBDataTypeParameter(VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase type)
        => type switch
        {
            VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.None => VersionBDataTypeEnum.Unknown,
            VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.IntValue => VersionBDataTypeEnum.Int32,
            VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.UintValue => VersionBDataTypeEnum.UInt32,
            VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.LongValue => VersionBDataTypeEnum.Int64,
            VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.UlongValue => VersionBDataTypeEnum.UInt64,
            VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.FloatValue => VersionBDataTypeEnum.Float,
            VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.DoubleValue => VersionBDataTypeEnum.Double,
            VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.BooleanValue => VersionBDataTypeEnum.Boolean,
            VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.StringValue => VersionBDataTypeEnum.String,
            VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.ExtensionValue => VersionBDataTypeEnum.Unknown,
            _ => VersionBDataTypeEnum.String
        };

    /// <summary>
    /// Gets the version B ProtoBuf value type from the version B data type for parameters.
    /// </summary>
    /// <param name="type">The <see cref="VersionBDataTypeEnum"/>.</param>
    /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase"/>.</returns>
    public static VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase ConvertVersionBDataTypeParameter(VersionBDataTypeEnum type)
        => type switch
        {
            VersionBDataTypeEnum.Unknown => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.ExtensionValue,
            VersionBDataTypeEnum.Int8 => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.IntValue,
            VersionBDataTypeEnum.Int16 => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.IntValue,
            VersionBDataTypeEnum.Int32 => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.IntValue,
            VersionBDataTypeEnum.Int64 => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.LongValue,
            VersionBDataTypeEnum.UInt8 => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.UintValue,
            VersionBDataTypeEnum.UInt16 => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.UintValue,
            VersionBDataTypeEnum.UInt32 => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.UintValue,
            VersionBDataTypeEnum.UInt64 => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.UlongValue,
            VersionBDataTypeEnum.Float => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.FloatValue,
            VersionBDataTypeEnum.Double => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.DoubleValue,
            VersionBDataTypeEnum.Boolean => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.BooleanValue,
            VersionBDataTypeEnum.String => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.StringValue,
            VersionBDataTypeEnum.DateTime => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.UlongValue,
            VersionBDataTypeEnum.Text => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.StringValue,
            VersionBDataTypeEnum.Uuid => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.StringValue,
            VersionBDataTypeEnum.DataSet => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.None,
            VersionBDataTypeEnum.Bytes => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.ExtensionValue,
            VersionBDataTypeEnum.File => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.ExtensionValue,
            VersionBDataTypeEnum.Template => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.None,
            VersionBDataTypeEnum.PropertySet => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.None,
            VersionBDataTypeEnum.PropertySetList => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.None,
            _ => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.None
        };

    /// <summary>
    /// Gets the version B data type from the version B ProtoBuf value type for property values.
    /// </summary>
    /// <param name="type">The <see cref="VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase"/>.</param>
    /// <returns>The <see cref="VersionBDataTypeEnum"/>.</returns>
    public static VersionBDataTypeEnum ConvertVersionBDataTypePropertyValue(VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase type)
        => type switch
        {
            VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.None => VersionBDataTypeEnum.Unknown,
            VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.IntValue => VersionBDataTypeEnum.Int32,
            VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.UintValue => VersionBDataTypeEnum.UInt32,
            VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.LongValue => VersionBDataTypeEnum.Int64,
            VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.UlongValue => VersionBDataTypeEnum.UInt64,
            VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.FloatValue => VersionBDataTypeEnum.Float,
            VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.DoubleValue => VersionBDataTypeEnum.Double,
            VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.BooleanValue => VersionBDataTypeEnum.Boolean,
            VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.StringValue => VersionBDataTypeEnum.String,
            VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.ExtensionValue => VersionBDataTypeEnum.Unknown,
            _ => VersionBDataTypeEnum.String
        };

    /// <summary>
    /// Gets the version B ProtoBuf value type from the version B data type for property values.
    /// </summary>
    /// <param name="type">The <see cref="VersionBDataTypeEnum"/>.</param>
    /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase"/>.</returns>
    public static VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase ConvertVersionBDataTypePropertyValue(VersionBDataTypeEnum type)
        => type switch
        {
            VersionBDataTypeEnum.Unknown => VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.ExtensionValue,
            VersionBDataTypeEnum.Int8 => VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.IntValue,
            VersionBDataTypeEnum.Int16 => VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.IntValue,
            VersionBDataTypeEnum.Int32 => VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.IntValue,
            VersionBDataTypeEnum.Int64 => VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.LongValue,
            VersionBDataTypeEnum.UInt8 => VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.UintValue,
            VersionBDataTypeEnum.UInt16 => VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.UintValue,
            VersionBDataTypeEnum.UInt32 => VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.UintValue,
            VersionBDataTypeEnum.UInt64 => VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.UlongValue,
            VersionBDataTypeEnum.Float => VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.FloatValue,
            VersionBDataTypeEnum.Double => VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.DoubleValue,
            VersionBDataTypeEnum.Boolean => VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.BooleanValue,
            VersionBDataTypeEnum.String => VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.StringValue,
            VersionBDataTypeEnum.DateTime => VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.UlongValue,
            VersionBDataTypeEnum.Text => VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.StringValue,
            VersionBDataTypeEnum.Uuid => VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.StringValue,
            VersionBDataTypeEnum.DataSet => VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.None,
            VersionBDataTypeEnum.Bytes => VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.ExtensionValue,
            VersionBDataTypeEnum.File => VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.ExtensionValue,
            VersionBDataTypeEnum.Template => VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.None,
            VersionBDataTypeEnum.PropertySet => VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.None,
            VersionBDataTypeEnum.PropertySetList => VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.None,
            _ => VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.None
        };

    /// <summary>
    /// Gets the version B data set from the version B ProtoBuf data set.
    /// </summary>
    /// <param name="dataSet">The <see cref="VersionBProtoBuf.ProtoBufPayload.DataSet"/>.</param>
    /// <returns>The <see cref="VersionBData.DataSet"/>.</returns>
    private static VersionBData.DataSet ConvertVersionBDataSet(VersionBProtoBuf.ProtoBufPayload.DataSet dataSet)
        => new VersionBData.DataSet
        {
            Details = dataSet.Details,
            Columns = dataSet.Columns,
            NumOfColumns = dataSet.NumOfColumns,
            Rows = dataSet.Rows.Select(ConvertVersionBRow).ToList(),
            Types = dataSet.Types
        };

    /// <summary>
    /// Gets the version B ProtoBuf data set from the version B data set.
    /// </summary>
    /// <param name="dataSet">The <see cref="VersionBData.DataSet"/>.</param>
    /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload.DataSet"/>.</returns>
    private static VersionBProtoBuf.ProtoBufPayload.DataSet ConvertVersionBDataSet(VersionBData.DataSet dataSet)
        => new VersionBProtoBuf.ProtoBufPayload.DataSet
        {
            Details = dataSet.Details,
            Columns = dataSet.Columns,
            NumOfColumns = dataSet.NumOfColumns,
            Rows = dataSet.Rows.Select(ConvertVersionBRow).ToList(),
            Types = dataSet.Types
        };

    /// <summary>
    /// Gets the version B data set value from the version B ProtoBuf data set value.
    /// </summary>
    /// <param name="dataSetValue">The <see cref="VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue"/>.</param>
    /// <returns>The <see cref="VersionBData.DataSetValue"/>.</returns>
    private static VersionBData.DataSetValue ConvertVersionBDataSetValue(VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue dataSetValue)
        => new VersionBData.DataSetValue
        {
            DoubleValue = dataSetValue.DoubleValue,
            BooleanValue = dataSetValue.BooleanValue,
            ExtensionValue = new VersionBData.DataSetValueExtension
            {
                Details = dataSetValue.ExtensionValue.Details
            },
            FloatValue = dataSetValue.FloatValue,
            IntValue = dataSetValue.IntValue,
            UIntValue = dataSetValue.UintValue,
            LongValue = dataSetValue.LongValue,
            ULongValue = dataSetValue.UlongValue,
            StringValue = dataSetValue.StringValue,
            DataType = ConvertVersionBDataTypeDataSetValue(dataSetValue.ValueCase)
        };

    /// <summary>
    /// Gets the version B ProtoBuf data set value from the version B data set value.
    /// </summary>
    /// <param name="dataSetValue">The <see cref="VersionBData.DataSetValue"/>.</param>
    /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue"/>.</returns>
    private static VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue ConvertVersionBDataSetValue(VersionBData.DataSetValue dataSetValue)
        => dataSetValue.DataType switch
        {
            VersionBDataTypeEnum.Int8
            or VersionBDataTypeEnum.Int16
            or VersionBDataTypeEnum.Int32 => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
            {
                IntValue = dataSetValue.IntValue
            },
            VersionBDataTypeEnum.Int64 => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
            {
                LongValue = dataSetValue.LongValue
            },
            VersionBDataTypeEnum.UInt8
            or VersionBDataTypeEnum.UInt16
            or VersionBDataTypeEnum.UInt32 => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
            {
                UintValue = dataSetValue.UIntValue
            },
            VersionBDataTypeEnum.UInt64
            or VersionBDataTypeEnum.DateTime => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
            {
                UlongValue = dataSetValue.ULongValue
            },
            VersionBDataTypeEnum.Float => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
            {
                FloatValue = dataSetValue.FloatValue
            },
            VersionBDataTypeEnum.Double => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
            {
                DoubleValue = dataSetValue.DoubleValue
            },
            VersionBDataTypeEnum.Boolean => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
            {
                BooleanValue = dataSetValue.BooleanValue
            },
            VersionBDataTypeEnum.String
            or VersionBDataTypeEnum.Text
            or VersionBDataTypeEnum.Uuid => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
            {
                StringValue = dataSetValue.StringValue
            },
            VersionBDataTypeEnum.Unknown
            or VersionBDataTypeEnum.DataSet
            or VersionBDataTypeEnum.Bytes
            or VersionBDataTypeEnum.File
            or VersionBDataTypeEnum.Template
            or VersionBDataTypeEnum.PropertySet
            or VersionBDataTypeEnum.PropertySetList
            or _ => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
            {
                ExtensionValue = new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.DataSetValueExtension
                {
                    Details = dataSetValue.ExtensionValue.Details
                }
            }
        };

    /// <summary>
    /// Gets the version B row from the version B ProtoBuf row.
    /// </summary>
    /// <param name="row">The <see cref="VersionBProtoBuf.ProtoBufPayload.DataSet.Row"/>.</param>
    /// <returns>The <see cref="VersionBData.Row"/>.</returns>
    private static VersionBData.Row ConvertVersionBRow(VersionBProtoBuf.ProtoBufPayload.DataSet.Row row)
        => new VersionBData.Row
        {
            Details = row.Details,
            Elements = row.Elements.Select(ConvertVersionBDataSetValue).ToList()
        };

    /// <summary>
    /// Gets the version B ProtoBuf row from the version B row.
    /// </summary>
    /// <param name="row">The <see cref="VersionBData.Row"/>.</param>
    /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload.DataSet.Row"/>.</returns>
    private static VersionBProtoBuf.ProtoBufPayload.DataSet.Row ConvertVersionBRow(VersionBData.Row row)
        => new VersionBProtoBuf.ProtoBufPayload.DataSet.Row
        {
            Details = row.Details,
            Elements = row.Elements.Select(ConvertVersionBDataSetValue).ToList()
        };

    /// <summary>
    /// Gets the version B meta data from the version B ProtoBuf meta data.
    /// </summary>
    /// <param name="metaData">The <see cref="VersionBProtoBuf.ProtoBufPayload.MetaData"/>.</param>
    /// <returns>The <see cref="VersionBData.MetaData"/>.</returns>
    private static VersionBData.MetaData? ConvertVersionBMetaData(VersionBProtoBuf.ProtoBufPayload.MetaData? metaData)
        => metaData is null
        ? null
        : new VersionBData.MetaData
        {
            Seq = metaData.Seq,
            Details = metaData.Details,
            ContentType = metaData.ContentType,
            Description = metaData.Description,
            FileName = metaData.FileName,
            FileType = metaData.FileType,
            IsMultiPart = metaData.IsMultiPart,
            Md5 = metaData.Md5,
            Size = metaData.Size
        };

    /// <summary>
    /// Gets the version B ProtoBuf meta data from the version B meta data.
    /// </summary>
    /// <param name="metaData">The <see cref="VersionBData.MetaData"/>.</param>
    /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload.MetaData"/>.</returns>
    private static VersionBProtoBuf.ProtoBufPayload.MetaData? ConvertVersionBMetaData(VersionBData.MetaData? metaData)
        => metaData is null
        ? null
        : new VersionBProtoBuf.ProtoBufPayload.MetaData
        {
            Seq = metaData.Seq,
            Details = metaData.Details,
            ContentType = metaData.ContentType,
            Description = metaData.Description,
            FileName = metaData.FileName,
            FileType = metaData.FileType,
            IsMultiPart = metaData.IsMultiPart,
            Md5 = metaData.Md5,
            Size = metaData.Size
        };

    /// <summary>
    /// Gets the version B template from the version B ProtoBuf template.
    /// </summary>
    /// <param name="template">The <see cref="VersionBProtoBuf.ProtoBufPayload.Template"/>.</param>
    /// <returns>The <see cref="VersionBData.Template"/>.</returns>
    private static VersionBData.Template? ConvertVersionBTemplate(VersionBProtoBuf.ProtoBufPayload.Template? template)
        => template is null
        ? null
        : new VersionBData.Template
        {
            Metrics = template.Metrics.Select(ConvertVersionBMetric).ToList(),
            Details = template.Details,
            IsDefinition = template.IsDefinition,
            Parameters = template.Parameters.Select(ConvertVersionBParameter).ToList(),
            TemplateRef = template.TemplateRef,
            Version = template.Version
        };

    /// <summary>
    /// Gets the version B ProtoBuf template from the version B template.
    /// </summary>
    /// <param name="template">The <see cref="VersionBData.Template"/>.</param>
    /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload.Template"/>.</returns>
    private static VersionBProtoBuf.ProtoBufPayload.Template? ConvertVersionBTemplate(VersionBData.Template? template)
        => template is null
        ? null
        : new VersionBProtoBuf.ProtoBufPayload.Template
        {
            Metrics = template.Metrics.Select(ConvertVersionBMetric).ToList(),
            Details = template.Details,
            IsDefinition = template.IsDefinition,
            Parameters = template.Parameters.Select(ConvertVersionBParameter).ToList(),
            TemplateRef = template.TemplateRef,
            Version = template.Version
        };

    /// <summary>
    /// Gets the version B parameter from the version B ProtoBuf parameter.
    /// </summary>
    /// <param name="parameter">The <see cref="VersionBProtoBuf.ProtoBufPayload.Template.Parameter"/>.</param>
    /// <returns>The <see cref="VersionBData.Parameter"/>.</returns>
    private static VersionBData.Parameter ConvertVersionBParameter(VersionBProtoBuf.ProtoBufPayload.Template.Parameter parameter)
        => new VersionBData.Parameter
        {
            DoubleValue = parameter.DoubleValue,
            BooleanValue = parameter.BooleanValue,
            ExtensionValue = new VersionBData.ParameterValueExtension
            {
                Extensions = parameter.ExtensionValue.Extensions
            },
            FloatValue = parameter.FloatValue,
            IntValue = parameter.IntValue,
            UIntValue = parameter.UintValue,
            LongValue = parameter.LongValue,
            ULongValue = parameter.UlongValue,
            Name = parameter.Name,
            StringValue = parameter.StringValue,
            ValueCase = parameter.Type,
            DataType = ConvertVersionBDataTypeParameter(parameter.ValueCase)
        };

    /// <summary>
    /// Gets the version B ProtoBuf parameter from the version B parameter.
    /// </summary>
    /// <param name="parameter">The <see cref="VersionBData.Parameter"/>.</param>
    /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload.Template.Parameter"/>.</returns>
    private static VersionBProtoBuf.ProtoBufPayload.Template.Parameter ConvertVersionBParameter(VersionBData.Parameter parameter)
    {
        VersionBProtoBuf.ProtoBufPayload.Template.Parameter pbTplParam = new()
        {
            Name = parameter.Name,
            Type = parameter.ValueCase
        };

        switch (parameter.DataType)
        {
            case VersionBDataTypeEnum.Int8:
            case VersionBDataTypeEnum.Int16:
            case VersionBDataTypeEnum.Int32:
                pbTplParam.IntValue = parameter.IntValue;
                break;
            case VersionBDataTypeEnum.Int64:
                pbTplParam.LongValue = parameter.LongValue;
                break;
            case VersionBDataTypeEnum.UInt8:
            case VersionBDataTypeEnum.UInt16:
            case VersionBDataTypeEnum.UInt32:
                pbTplParam.UintValue = parameter.UIntValue;
                break;
            case VersionBDataTypeEnum.UInt64:
            case VersionBDataTypeEnum.DateTime:
                pbTplParam.UlongValue = parameter.ULongValue;
                break;
            case VersionBDataTypeEnum.Float:
                pbTplParam.FloatValue = parameter.FloatValue;
                break;
            case VersionBDataTypeEnum.Double:
                pbTplParam.DoubleValue = parameter.DoubleValue;
                break;
            case VersionBDataTypeEnum.Boolean:
                pbTplParam.BooleanValue = parameter.BooleanValue;
                break;
            case VersionBDataTypeEnum.String:
            case VersionBDataTypeEnum.Text:
            case VersionBDataTypeEnum.Uuid:
                pbTplParam.StringValue = parameter.StringValue;
                break;
            case VersionBDataTypeEnum.Unknown:
            case VersionBDataTypeEnum.DataSet:
            case VersionBDataTypeEnum.Bytes:
            case VersionBDataTypeEnum.File:
            case VersionBDataTypeEnum.Template:
            case VersionBDataTypeEnum.PropertySet:
            case VersionBDataTypeEnum.PropertySetList:
            default:
                pbTplParam.ExtensionValue = new VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ParameterValueExtension
                {
                    Extensions = parameter.ExtensionValue.Extensions
                };
                pbTplParam.StringValue = parameter.StringValue;
                break;
        }

        return pbTplParam;
    }

    /// <summary>
    /// Gets the version B metric from the version B ProtoBuf metric.
    /// </summary>
    /// <param name="metric">The <see cref="VersionBProtoBuf.ProtoBufPayload.Metric"/>.</param>
    /// <returns>The <see cref="VersionBData.Metric"/>.</returns>
    private static VersionBData.Metric ConvertVersionBMetric(VersionBProtoBuf.ProtoBufPayload.Metric metric)
        => new VersionBData.Metric
        {
            DoubleValue = metric.DoubleValue,
            Alias = metric.Alias,
            BooleanValue = metric.BooleanValue,
            BytesValue = metric.BytesValue,
            DataSetValue = ConvertVersionBDataSet(metric.DatasetValue),
            ValueCase = metric.Datatype,
            ExtensionValue = (metric.ExtensionValue is not null) ? new VersionBData.MetricValueExtension
            {
                Details = metric.ExtensionValue.Details
            } : null,
            FloatValue = metric.FloatValue,
            IntValue = metric.IntValue,
            UIntValue = metric.UintValue,
            IsHistorical = metric.IsHistorical,
            IsNull = metric.IsNull,
            IsTransient = metric.IsTransient,
            LongValue = metric.LongValue,
            ULongValue = metric.UlongValue,
            Metadata = ConvertVersionBMetaData(metric.Metadata),
            Name = metric.Name,
            Properties = ConvertVersionBPropertySet(metric.Properties),
            StringValue = metric.StringValue,
            Timestamp = metric.Timestamp,
            TemplateValue = ConvertVersionBTemplate(metric.TemplateValue),
            DataType = ConvertVersionBDataTypeMetric(metric.ValueCase)
        };

    /// <summary>
    /// Gets the version B ProtoBuf metric from the version B metric.
    /// </summary>
    /// <param name="metric">The <see cref="VersionBData.Metric"/>.</param>
    /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload.Metric"/>.</returns>
    private static VersionBProtoBuf.ProtoBufPayload.Metric ConvertVersionBMetric(VersionBData.Metric metric)
    {
        VersionBProtoBuf.ProtoBufPayload.Metric pbMetric = new()
        {
            Alias = metric.Alias,
            Datatype = metric.ValueCase,
            IsHistorical = metric.IsHistorical,
            IsNull = metric.IsNull,
            IsTransient = metric.IsTransient,
            Metadata = ConvertVersionBMetaData(metric.Metadata),
            Name = metric.Name,
            Properties = ConvertVersionBPropertySet(metric.Properties),
            Timestamp = metric.Timestamp
        };

        switch (metric.DataType)
        {
            case VersionBDataTypeEnum.Int8:
            case VersionBDataTypeEnum.Int16:
            case VersionBDataTypeEnum.Int32:
                pbMetric.IntValue = metric.IntValue;
                break;
            case VersionBDataTypeEnum.Int64:
                pbMetric.LongValue = metric.LongValue;
                break;
            case VersionBDataTypeEnum.UInt8:
            case VersionBDataTypeEnum.UInt16:
            case VersionBDataTypeEnum.UInt32:
                pbMetric.UintValue = metric.UIntValue;
                break;
            case VersionBDataTypeEnum.UInt64:
            case VersionBDataTypeEnum.DateTime:
                pbMetric.UlongValue = metric.ULongValue;
                break;
            case VersionBDataTypeEnum.Float:
                pbMetric.FloatValue = metric.FloatValue;
                break;
            case VersionBDataTypeEnum.Double:
                pbMetric.DoubleValue = metric.DoubleValue;
                break;
            case VersionBDataTypeEnum.Boolean:
                pbMetric.BooleanValue = metric.BooleanValue;
                break;
            case VersionBDataTypeEnum.String:
            case VersionBDataTypeEnum.Text:
            case VersionBDataTypeEnum.Uuid:
                pbMetric.StringValue = metric.StringValue;
                break;
            case VersionBDataTypeEnum.DataSet:
                pbMetric.DatasetValue = ConvertVersionBDataSet(metric.DataSetValue);
                break;
            case VersionBDataTypeEnum.Template:
                pbMetric.TemplateValue = ConvertVersionBTemplate(metric.TemplateValue);
                break;
            case VersionBDataTypeEnum.Bytes:
                pbMetric.BytesValue = metric.BytesValue;
                break;
            case VersionBDataTypeEnum.Unknown:
            case VersionBDataTypeEnum.File:
            case VersionBDataTypeEnum.PropertySet:
            case VersionBDataTypeEnum.PropertySetList:
            default:
                pbMetric.ExtensionValue = (metric.ExtensionValue == null) ? null :
                    new VersionBProtoBuf.ProtoBufPayload.Metric.MetricValueExtension { Details = metric.ExtensionValue.Details };
                break;
        }

        return pbMetric;
    }

    /// <summary>
    /// Gets the version B property set list from the version B ProtoBuf property set list.
    /// </summary>
    /// <param name="propertySetList">The <see cref="VersionBProtoBuf.ProtoBufPayload.PropertySetList"/>.</param>
    /// <returns>The <see cref="VersionBData.PropertySetList"/>.</returns>
    private static VersionBData.PropertySetList? ConvertVersionBPropertySetList(VersionBProtoBuf.ProtoBufPayload.PropertySetList? propertySetList)
    {
        if (propertySetList is null)
        {
            return null;
        }

        if (propertySetList.Propertysets is null)
        {
            throw new ArgumentNullException(nameof(propertySetList), "Propertysets is not set");
        }

        return new VersionBData.PropertySetList
        {
            Details = propertySetList.Details,
            PropertySets = propertySetList.Propertysets.Select(ConvertVersionBPropertySet).ToNonNullList()
        };
    }

    /// <summary>
    /// Gets the version B ProtoBuf property set list from the version B property set list.
    /// </summary>
    /// <param name="propertySetList">The <see cref="VersionBData.PropertySetList"/>.</param>
    /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload.PropertySetList"/>.</returns>
    private static VersionBProtoBuf.ProtoBufPayload.PropertySetList? ConvertVersionBPropertySetList(VersionBData.PropertySetList? propertySetList)
        => propertySetList is null
        ? null
        : new VersionBProtoBuf.ProtoBufPayload.PropertySetList
        {
            Details = propertySetList.Details,
            Propertysets = propertySetList.PropertySets.Select(ConvertVersionBPropertySet).ToNonNullList()
        };

    /// <summary>
    /// Gets the version B property set from the version B ProtoBuf property set.
    /// </summary>
    /// <param name="propertySet">The <see cref="VersionBProtoBuf.ProtoBufPayload.PropertySet"/>.</param>
    /// <returns>The <see cref="VersionBData.PropertySet"/>.</returns>
    private static VersionBData.PropertySet? ConvertVersionBPropertySet(VersionBProtoBuf.ProtoBufPayload.PropertySet? propertySet)
        => propertySet is null
        ? null
        : new VersionBData.PropertySet
        {
            Details = propertySet.Details,
            Keys = propertySet.Keys,
            Values = propertySet.Values.Select(ConvertVersionBPropertyValue).ToList()
        };

    /// <summary>
    /// Gets the version B ProtoBuf property set from the version B property set.
    /// </summary>
    /// <param name="propertySet">The <see cref="VersionBData.PropertySet"/>.</param>
    /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload.PropertySet"/>.</returns>
    private static VersionBProtoBuf.ProtoBufPayload.PropertySet? ConvertVersionBPropertySet(VersionBData.PropertySet? propertySet)
        => propertySet is null
        ? null
        : new VersionBProtoBuf.ProtoBufPayload.PropertySet
        {
            Details = propertySet.Details,
            Keys = propertySet.Keys,
            Values = propertySet.Values.Select(ConvertVersionBPropertyValue).ToList()
        };

    /// <summary>
    /// Gets the version B property value from the version B ProtoBuf property value.
    /// </summary>
    /// <param name="propertyValue">The <see cref="VersionBProtoBuf.ProtoBufPayload.PropertyValue"/>.</param>
    /// <returns>The <see cref="VersionBData.PropertyValue"/>.</returns>
    private static VersionBData.PropertyValue ConvertVersionBPropertyValue(VersionBProtoBuf.ProtoBufPayload.PropertyValue propertyValue)
        => new VersionBData.PropertyValue
        {
            DoubleValue = propertyValue.DoubleValue,
            PropertySetsValue = ConvertVersionBPropertySetList(propertyValue.PropertysetsValue),
            BooleanValue = propertyValue.BooleanValue,
            ExtensionValue = new VersionBData.PropertyValueExtension
            {
                Details = propertyValue.ExtensionValue.Details
            },
            FloatValue = propertyValue.FloatValue,
            IntValue = propertyValue.IntValue,
            UIntValue = propertyValue.UintValue,
            IsNull = propertyValue.IsNull,
            LongValue = propertyValue.LongValue,
            ULongValue = propertyValue.UlongValue,
            PropertySetValue = ConvertVersionBPropertySet(propertyValue.PropertysetValue),
            StringValue = propertyValue.StringValue,
            ValueCase = propertyValue.Type,
            DataType = ConvertVersionBDataTypePropertyValue(propertyValue.ValueCase)
        };

    /// <summary>
    /// Gets the version B ProtoBuf property value from the version B property value.
    /// </summary>
    /// <param name="propertyValue">The <see cref="VersionBData.PropertyValue"/>.</param>
    /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload.PropertyValue"/>.</returns>
    private static VersionBProtoBuf.ProtoBufPayload.PropertyValue ConvertVersionBPropertyValue(VersionBData.PropertyValue propertyValue)
    {
        VersionBProtoBuf.ProtoBufPayload.PropertyValue pbPropValue = new()
        {
            IsNull = propertyValue.IsNull,
            Type = propertyValue.ValueCase
        };

        switch (propertyValue.DataType)
        {
            case VersionBDataTypeEnum.Int8:
            case VersionBDataTypeEnum.Int16:
            case VersionBDataTypeEnum.Int32:
                pbPropValue.IntValue = propertyValue.IntValue;
                break;
            case VersionBDataTypeEnum.Int64:
                pbPropValue.LongValue = propertyValue.LongValue;
                break;
            case VersionBDataTypeEnum.UInt8:
            case VersionBDataTypeEnum.UInt16:
            case VersionBDataTypeEnum.UInt32:
                pbPropValue.UintValue = propertyValue.UIntValue;
                break;
            case VersionBDataTypeEnum.UInt64:
            case VersionBDataTypeEnum.DateTime:
                pbPropValue.UlongValue = propertyValue.ULongValue;
                break;
            case VersionBDataTypeEnum.Float:
                pbPropValue.FloatValue = propertyValue.FloatValue;
                break;
            case VersionBDataTypeEnum.Double:
                pbPropValue.DoubleValue = propertyValue.DoubleValue;
                break;
            case VersionBDataTypeEnum.Boolean:
                pbPropValue.BooleanValue = propertyValue.BooleanValue;
                break;
            case VersionBDataTypeEnum.String:
            case VersionBDataTypeEnum.Text:
            case VersionBDataTypeEnum.Uuid:
                pbPropValue.StringValue = propertyValue.StringValue;
                break;
            case VersionBDataTypeEnum.PropertySet:
                pbPropValue.PropertysetValue = ConvertVersionBPropertySet(propertyValue.PropertySetValue);
                break;
            case VersionBDataTypeEnum.PropertySetList:
                pbPropValue.PropertysetsValue = ConvertVersionBPropertySetList(propertyValue.PropertySetsValue);
                break;
            case VersionBDataTypeEnum.DataSet:
            case VersionBDataTypeEnum.Template:
            case VersionBDataTypeEnum.Bytes:
            case VersionBDataTypeEnum.Unknown:
            case VersionBDataTypeEnum.File:
            default:
                pbPropValue.ExtensionValue = new VersionBProtoBuf.ProtoBufPayload.PropertyValue.PropertyValueExtension
                {
                    Details = propertyValue.ExtensionValue.Details
                };
                break;
        }

        return pbPropValue;
    }
}
