// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PayloadConverter.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A helper class for the payload conversions from internal ProtoBuf model to external data and vice versa for version B.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.VersionB;

/// <summary>
/// A helper class for the payload conversions from internal ProtoBuf model to external data and vice versa for version B.
/// </summary>
internal static class PayloadConverter
{
    /// <summary>
    /// Gets the version B payload converted from the ProtoBuf payload.
    /// </summary>
    /// <param name="protoPayload">The <see cref="VersionBProtoBuf.ProtoBufPayload"/>.</param>
    /// <returns>The <see cref="Payload"/>.</returns>
    public static Payload ConvertVersionBPayload(VersionBProtoBuf.ProtoBufPayload protoPayload)
        => new()
        {
            Body = protoPayload.Body,
            Metrics = protoPayload.Metrics.Select(ConvertVersionBMetric).ToList(),
            Seq = protoPayload.Seq,
            Timestamp = protoPayload.Timestamp,
            Uuid = protoPayload.Uuid ?? string.Empty
        };

    /// <summary>
    /// Gets the ProtoBuf payload converted from the version B payload.
    /// </summary>
    /// <param name="payload">The <see cref="Payload"/>.</param>
    /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload"/>.</returns>
    public static VersionBProtoBuf.ProtoBufPayload ConvertVersionBPayload(Payload payload)
        => new()
        {
            Body = payload.Body,
            Metrics = payload.Metrics.Select(ConvertVersionBMetric).ToList(),
            Seq = payload.Seq,
            Timestamp = payload.Timestamp,
            Uuid = payload.Uuid
        };

    /// <summary>
    /// Gets the version B metric from the version B ProtoBuf metric.
    /// </summary>
    /// <param name="protoMetric">The <see cref="VersionBProtoBuf.ProtoBufPayload.Metric"/>.</param>
    /// <returns>The <see cref="Metric"/>.</returns>
    public static Metric ConvertVersionBMetric(VersionBProtoBuf.ProtoBufPayload.Metric protoMetric)
    {
        var metric = new Metric()
        {
            Alias = protoMetric.Alias,
            IsHistorical = protoMetric.IsHistorical,
            IsTransient = protoMetric.IsTransient,
            MetaData = ConvertVersionBMetaData(protoMetric.MetaData),
            Name = protoMetric.Name ?? string.Empty,
            Timestamp = protoMetric.Timestamp
        };

        var dataType = ConvertVersionBDataType((VersionBProtoBuf.DataType?)protoMetric.DataType);

        switch (dataType)
        {
            case VersionBDataTypeEnum.Int8:
                metric.SetValue(VersionBDataTypeEnum.Int8, protoMetric.IntValue);
                break;
            case VersionBDataTypeEnum.Int16:
                metric.SetValue(VersionBDataTypeEnum.Int16, protoMetric.IntValue);
                break;
            case VersionBDataTypeEnum.Int32:
                metric.SetValue(VersionBDataTypeEnum.Int32, protoMetric.IntValue);
                break;
            case VersionBDataTypeEnum.UInt8:
                metric.SetValue(VersionBDataTypeEnum.UInt8, protoMetric.IntValue);
                break;
            case VersionBDataTypeEnum.UInt16:
                metric.SetValue(VersionBDataTypeEnum.UInt16, protoMetric.IntValue);
                break;
            case VersionBDataTypeEnum.Int64:
                metric.SetValue(VersionBDataTypeEnum.Int64, protoMetric.LongValue);
                break;
            case VersionBDataTypeEnum.UInt32:
                metric.SetValue(VersionBDataTypeEnum.UInt32, protoMetric.LongValue);
                break;
            case VersionBDataTypeEnum.UInt64:
                metric.SetValue(VersionBDataTypeEnum.UInt64, protoMetric.LongValue);
                break;
            case VersionBDataTypeEnum.DateTime:
                metric.SetValue(VersionBDataTypeEnum.DateTime, protoMetric.LongValue);
                break;
            case VersionBDataTypeEnum.Float:
                metric.SetValue(VersionBDataTypeEnum.Float, protoMetric.FloatValue);
                break;
            case VersionBDataTypeEnum.Double:
                metric.SetValue(VersionBDataTypeEnum.Double, protoMetric.DoubleValue);
                break;
            case VersionBDataTypeEnum.Boolean:
                metric.SetValue(VersionBDataTypeEnum.Boolean, protoMetric.BooleanValue);
                break;
            case VersionBDataTypeEnum.String:
                metric.SetValue(VersionBDataTypeEnum.String, protoMetric.StringValue);
                break;
            case VersionBDataTypeEnum.Text:
                metric.SetValue(VersionBDataTypeEnum.Text, protoMetric.StringValue);
                break;
            case VersionBDataTypeEnum.Uuid:
                metric.SetValue(VersionBDataTypeEnum.Uuid, protoMetric.StringValue);
                break;
            case VersionBDataTypeEnum.Bytes:
                metric.SetValue(VersionBDataTypeEnum.Bytes, protoMetric.BytesValue);
                break;
            case VersionBDataTypeEnum.File:
                metric.SetValue(VersionBDataTypeEnum.File, protoMetric.BytesValue);
                break;
            case VersionBDataTypeEnum.DataSet:
                metric.SetValue(VersionBDataTypeEnum.DataSet, ConvertVersionBDataSet(protoMetric.DataSetValue));
                break;
            case VersionBDataTypeEnum.Template:
                metric.SetValue(VersionBDataTypeEnum.Template, ConvertVersionBTemplate(protoMetric.TemplateValue));
                break;
            case VersionBDataTypeEnum.PropertySet:
                metric.SetValue(VersionBDataTypeEnum.PropertySet, ConvertVersionBPropertySet(protoMetric.PropertySetValue));
                break;
            case VersionBDataTypeEnum.Int8Array:
                metric.SetValue(VersionBDataTypeEnum.Int8Array, protoMetric.BytesValue);
                break;
            case VersionBDataTypeEnum.Int16Array:
                metric.SetValue(VersionBDataTypeEnum.Int16Array, protoMetric.BytesValue);
                break;
            case VersionBDataTypeEnum.Int32Array:
                metric.SetValue(VersionBDataTypeEnum.Int32Array, protoMetric.BytesValue);
                break;
            case VersionBDataTypeEnum.Int64Array:
                metric.SetValue(VersionBDataTypeEnum.Int64Array, protoMetric.BytesValue);
                break;
            case VersionBDataTypeEnum.UInt8Array:
                metric.SetValue(VersionBDataTypeEnum.UInt8Array, protoMetric.BytesValue);
                break;
            case VersionBDataTypeEnum.UInt16Array:
                metric.SetValue(VersionBDataTypeEnum.UInt16Array, protoMetric.BytesValue);
                break;
            case VersionBDataTypeEnum.UInt32Array:
                metric.SetValue(VersionBDataTypeEnum.UInt32Array, protoMetric.BytesValue);
                break;
            case VersionBDataTypeEnum.UInt64Array:
                metric.SetValue(VersionBDataTypeEnum.UInt64Array, protoMetric.BytesValue);
                break;
            case VersionBDataTypeEnum.FloatArray:
                metric.SetValue(VersionBDataTypeEnum.FloatArray, protoMetric.BytesValue);
                break;
            case VersionBDataTypeEnum.DoubleArray:
                metric.SetValue(VersionBDataTypeEnum.DoubleArray, protoMetric.BytesValue);
                break;
            case VersionBDataTypeEnum.BooleanArray:
                metric.SetValue(VersionBDataTypeEnum.BooleanArray, protoMetric.BytesValue);
                break;
            case VersionBDataTypeEnum.StringArray:
                metric.SetValue(VersionBDataTypeEnum.StringArray, protoMetric.BytesValue);
                break;
            case VersionBDataTypeEnum.DateTimeArray:
                metric.SetValue(VersionBDataTypeEnum.DateTimeArray, protoMetric.BytesValue);
                break;
            case VersionBDataTypeEnum.PropertySetList:
            case VersionBDataTypeEnum.Unknown:
            default:
                throw new ArgumentOutOfRangeException(nameof(protoMetric.DataType), protoMetric.DataType, "Unknown metric data type");
        }

        return metric;
    }

    /// <summary>
    /// Gets the version B ProtoBuf metric from the version B metric.
    /// </summary>
    /// <param name="metric">The <see cref="Metric"/>.</param>
    /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload.Metric"/>.</returns>
    public static VersionBProtoBuf.ProtoBufPayload.Metric ConvertVersionBMetric(Metric metric)
    {
        var protoMetric = new VersionBProtoBuf.ProtoBufPayload.Metric()
        {
            Alias = metric.Alias,
            DataType = (uint?)ConvertVersionBDataType(metric.DataType),
            IsHistorical = metric.IsHistorical,
            IsNull = metric.IsNull,
            IsTransient = metric.IsTransient,
            MetaData = ConvertVersionBMetaData(metric.MetaData),
            Name = metric.Name,
            Timestamp = metric.Timestamp
        };

        switch (metric.DataType)
        {
            case VersionBDataTypeEnum.Int8:
            case VersionBDataTypeEnum.Int16:
            case VersionBDataTypeEnum.Int32:
            case VersionBDataTypeEnum.UInt8:
            case VersionBDataTypeEnum.UInt16:
                protoMetric.IntValue = metric.Value.ConvertOrDefaultTo<uint>();
                break;
            case VersionBDataTypeEnum.Int64:
            case VersionBDataTypeEnum.UInt32:
            case VersionBDataTypeEnum.UInt64:
            case VersionBDataTypeEnum.DateTime:
                protoMetric.LongValue = metric.Value.ConvertOrDefaultTo<ulong>();
                break;
            case VersionBDataTypeEnum.Float:
                protoMetric.FloatValue = metric.Value.ConvertOrDefaultTo<uint>();
                break;
            case VersionBDataTypeEnum.Double:
                protoMetric.DoubleValue = metric.Value.ConvertOrDefaultTo<double>();
                break;
            case VersionBDataTypeEnum.Boolean:
                protoMetric.BooleanValue = metric.Value.ConvertOrDefaultTo<bool>();
                break;
            case VersionBDataTypeEnum.String:
            case VersionBDataTypeEnum.Text:
            case VersionBDataTypeEnum.Uuid:
                protoMetric.StringValue = metric.Value.ConvertOrDefaultTo<string>();
                break;
            case VersionBDataTypeEnum.Bytes:
            case VersionBDataTypeEnum.File:
                protoMetric.BytesValue = metric.Value.ConvertOrDefaultTo<byte[]>();
                break;
            case VersionBDataTypeEnum.DataSet:
                protoMetric.DataSetValue = ConvertVersionBDataSet(metric.Value.ConvertOrDefaultTo<DataSet>());
                break;
            case VersionBDataTypeEnum.Template:
                protoMetric.TemplateValue = ConvertVersionBTemplate(metric.Value.ConvertOrDefaultTo<Template>());
                break;
            case VersionBDataTypeEnum.PropertySet:
                protoMetric.PropertySetValue = ConvertVersionBPropertySet(metric.Value.ConvertOrDefaultTo<PropertySet>());
                break;
            case VersionBDataTypeEnum.Int8Array:
            case VersionBDataTypeEnum.Int16Array:
            case VersionBDataTypeEnum.Int32Array:
            case VersionBDataTypeEnum.Int64Array:
            case VersionBDataTypeEnum.UInt8Array:
            case VersionBDataTypeEnum.UInt16Array:
            case VersionBDataTypeEnum.UInt32Array:
            case VersionBDataTypeEnum.UInt64Array:
            case VersionBDataTypeEnum.FloatArray:
            case VersionBDataTypeEnum.DoubleArray:
            case VersionBDataTypeEnum.BooleanArray:
            case VersionBDataTypeEnum.StringArray:
            case VersionBDataTypeEnum.DateTimeArray:
                protoMetric.BytesValue = metric.ConvertOrDefaultTo<byte[]>();
                break;
            case VersionBDataTypeEnum.PropertySetList:
            case VersionBDataTypeEnum.Unknown:
            default:
                throw new ArgumentOutOfRangeException(nameof(metric.DataType), metric.DataType, "Unknown metric data type");
        }

        return protoMetric;
    }

    /// <summary>
    /// Gets the version B data type from the version B ProtoBuf data type.
    /// </summary>
    /// <param name="protoDataType">The <see cref="VersionBProtoBuf.DataType"/>.</param>
    /// <returns>The <see cref="VersionBDataTypeEnum"/>.</returns>
    public static VersionBDataTypeEnum ConvertVersionBDataType(VersionBProtoBuf.DataType? protoDataType)
        => protoDataType switch
        {
            VersionBProtoBuf.DataType.Unknown => VersionBDataTypeEnum.Unknown,
            VersionBProtoBuf.DataType.Int8 => VersionBDataTypeEnum.Int8,
            VersionBProtoBuf.DataType.Int16 => VersionBDataTypeEnum.Int16,
            VersionBProtoBuf.DataType.Int32 => VersionBDataTypeEnum.Int32,
            VersionBProtoBuf.DataType.Int64 => VersionBDataTypeEnum.Int64,
            VersionBProtoBuf.DataType.UInt8 => VersionBDataTypeEnum.UInt8,
            VersionBProtoBuf.DataType.UInt16 => VersionBDataTypeEnum.UInt16,
            VersionBProtoBuf.DataType.UInt32 => VersionBDataTypeEnum.UInt32,
            VersionBProtoBuf.DataType.UInt64 => VersionBDataTypeEnum.UInt64,
            VersionBProtoBuf.DataType.Float => VersionBDataTypeEnum.Float,
            VersionBProtoBuf.DataType.Double => VersionBDataTypeEnum.Double,
            VersionBProtoBuf.DataType.Boolean => VersionBDataTypeEnum.Boolean,
            VersionBProtoBuf.DataType.String => VersionBDataTypeEnum.String,
            VersionBProtoBuf.DataType.DateTime => VersionBDataTypeEnum.DateTime,
            VersionBProtoBuf.DataType.Text => VersionBDataTypeEnum.Text,
            VersionBProtoBuf.DataType.Uuid => VersionBDataTypeEnum.Uuid,
            VersionBProtoBuf.DataType.DataSet => VersionBDataTypeEnum.DataSet,
            VersionBProtoBuf.DataType.Bytes => VersionBDataTypeEnum.Bytes,
            VersionBProtoBuf.DataType.File => VersionBDataTypeEnum.File,
            VersionBProtoBuf.DataType.Template => VersionBDataTypeEnum.Template,
            VersionBProtoBuf.DataType.PropertySet => VersionBDataTypeEnum.PropertySet,
            VersionBProtoBuf.DataType.PropertySetList => VersionBDataTypeEnum.PropertySetList,
            VersionBProtoBuf.DataType.Int8Array => VersionBDataTypeEnum.Int8Array,
            VersionBProtoBuf.DataType.Int16Array => VersionBDataTypeEnum.Int16Array,
            VersionBProtoBuf.DataType.Int32Array => VersionBDataTypeEnum.Int32Array,
            VersionBProtoBuf.DataType.Int64Array => VersionBDataTypeEnum.Int64Array,
            VersionBProtoBuf.DataType.UInt8Array => VersionBDataTypeEnum.UInt8Array,
            VersionBProtoBuf.DataType.UInt16Array => VersionBDataTypeEnum.UInt16Array,
            VersionBProtoBuf.DataType.UInt32Array => VersionBDataTypeEnum.UInt32Array,
            VersionBProtoBuf.DataType.UInt64Array => VersionBDataTypeEnum.UInt64Array,
            VersionBProtoBuf.DataType.FloatArray => VersionBDataTypeEnum.FloatArray,
            VersionBProtoBuf.DataType.DoubleArray => VersionBDataTypeEnum.DoubleArray,
            VersionBProtoBuf.DataType.BooleanArray => VersionBDataTypeEnum.BooleanArray,
            VersionBProtoBuf.DataType.StringArray => VersionBDataTypeEnum.StringArray,
            VersionBProtoBuf.DataType.DateTimeArray => VersionBDataTypeEnum.DateTimeArray,
            _ => VersionBDataTypeEnum.Unknown
        };

    /// <summary>
    /// Gets the version B ProtoBuf data type from the version B data type.
    /// </summary>
    /// <param name="dataType">The <see cref="VersionBDataTypeEnum"/>.</param>
    /// <returns>The <see cref="VersionBProtoBuf.DataType"/>.</returns>
    public static VersionBProtoBuf.DataType ConvertVersionBDataType(VersionBDataTypeEnum? dataType)
        => dataType switch
        {
            VersionBDataTypeEnum.Unknown => VersionBProtoBuf.DataType.Unknown,
            VersionBDataTypeEnum.Int8 => VersionBProtoBuf.DataType.Int8,
            VersionBDataTypeEnum.Int16 => VersionBProtoBuf.DataType.Int16,
            VersionBDataTypeEnum.Int32 => VersionBProtoBuf.DataType.Int32,
            VersionBDataTypeEnum.Int64 => VersionBProtoBuf.DataType.Int64,
            VersionBDataTypeEnum.UInt8 => VersionBProtoBuf.DataType.UInt8,
            VersionBDataTypeEnum.UInt16 => VersionBProtoBuf.DataType.UInt16,
            VersionBDataTypeEnum.UInt32 => VersionBProtoBuf.DataType.UInt32,
            VersionBDataTypeEnum.UInt64 => VersionBProtoBuf.DataType.UInt64,
            VersionBDataTypeEnum.Float => VersionBProtoBuf.DataType.Float,
            VersionBDataTypeEnum.Double => VersionBProtoBuf.DataType.Double,
            VersionBDataTypeEnum.Boolean => VersionBProtoBuf.DataType.Boolean,
            VersionBDataTypeEnum.String => VersionBProtoBuf.DataType.String,
            VersionBDataTypeEnum.DateTime => VersionBProtoBuf.DataType.DateTime,
            VersionBDataTypeEnum.Text => VersionBProtoBuf.DataType.Text,
            VersionBDataTypeEnum.Uuid => VersionBProtoBuf.DataType.Uuid,
            VersionBDataTypeEnum.DataSet => VersionBProtoBuf.DataType.DataSet,
            VersionBDataTypeEnum.Bytes => VersionBProtoBuf.DataType.Bytes,
            VersionBDataTypeEnum.File => VersionBProtoBuf.DataType.File,
            VersionBDataTypeEnum.Template => VersionBProtoBuf.DataType.Template,
            VersionBDataTypeEnum.PropertySet => VersionBProtoBuf.DataType.PropertySet,
            VersionBDataTypeEnum.PropertySetList => VersionBProtoBuf.DataType.PropertySetList,
            VersionBDataTypeEnum.Int8Array => VersionBProtoBuf.DataType.Int8Array,
            VersionBDataTypeEnum.Int16Array => VersionBProtoBuf.DataType.Int16Array,
            VersionBDataTypeEnum.Int32Array => VersionBProtoBuf.DataType.Int32Array,
            VersionBDataTypeEnum.Int64Array => VersionBProtoBuf.DataType.Int64Array,
            VersionBDataTypeEnum.UInt8Array => VersionBProtoBuf.DataType.UInt8Array,
            VersionBDataTypeEnum.UInt16Array => VersionBProtoBuf.DataType.UInt16Array,
            VersionBDataTypeEnum.UInt32Array => VersionBProtoBuf.DataType.UInt32Array,
            VersionBDataTypeEnum.UInt64Array => VersionBProtoBuf.DataType.UInt64Array,
            VersionBDataTypeEnum.FloatArray => VersionBProtoBuf.DataType.FloatArray,
            VersionBDataTypeEnum.DoubleArray => VersionBProtoBuf.DataType.DoubleArray,
            VersionBDataTypeEnum.BooleanArray => VersionBProtoBuf.DataType.BooleanArray,
            VersionBDataTypeEnum.StringArray => VersionBProtoBuf.DataType.StringArray,
            VersionBDataTypeEnum.DateTimeArray => VersionBProtoBuf.DataType.DateTimeArray,
            _ => VersionBProtoBuf.DataType.Unknown
        };

    /// <summary>
    /// Gets the version B meta data from the version B ProtoBuf meta data.
    /// </summary>
    /// <param name="protoMetaData">The <see cref="VersionBProtoBuf.ProtoBufPayload.MetaData"/>.</param>
    /// <returns>The <see cref="MetaData"/>.</returns>
    public static MetaData? ConvertVersionBMetaData(VersionBProtoBuf.ProtoBufPayload.MetaData? protoMetaData)
        => protoMetaData is null
        ? null
        : new MetaData
        {
            Seq = protoMetaData.Seq,
            ContentType = protoMetaData.ContentType ?? string.Empty,
            Description = protoMetaData.Description ?? string.Empty,
            FileName = protoMetaData.FileName ?? string.Empty,
            FileType = protoMetaData.FileType ?? string.Empty,
            IsMultiPart = protoMetaData.IsMultiPart,
            Md5 = protoMetaData.Md5 ?? string.Empty,
            Size = protoMetaData.Size
        };

    /// <summary>
    /// Gets the version B ProtoBuf meta data from the version B meta data.
    /// </summary>
    /// <param name="metaData">The <see cref="MetaData"/>.</param>
    /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload.MetaData"/>.</returns>
    public static VersionBProtoBuf.ProtoBufPayload.MetaData? ConvertVersionBMetaData(MetaData? metaData)
        => metaData is null
        ? null
        : new VersionBProtoBuf.ProtoBufPayload.MetaData
        {
            Seq = metaData.Seq,
            ContentType = metaData.ContentType,
            Description = metaData.Description,
            FileName = metaData.FileName,
            FileType = metaData.FileType,
            IsMultiPart = metaData.IsMultiPart,
            Md5 = metaData.Md5,
            Size = metaData.Size
        };

    /// <summary>
    /// Gets the version B data set from the version B ProtoBuf data set.
    /// </summary>
    /// <param name="protoDataSet">The <see cref="VersionBProtoBuf.ProtoBufPayload.DataSet"/>.</param>
    /// <returns>The <see cref="DataSet"/>.</returns>
    public static DataSet? ConvertVersionBDataSet(VersionBProtoBuf.ProtoBufPayload.DataSet? protoDataSet)
    {
        if (protoDataSet is null)
        {
            return null;
        }

        var rows = new List<Row>();
        var index = 0;

        foreach (var row in protoDataSet.Rows)
        {
            rows.Add(new Row
            {
                Elements = row.Elements.Select(e => ConvertVersionBDataSetValue(e, protoDataSet.Types[index])).ToList()
            });

            index++;
        }

        return new DataSet
        {
            Columns = protoDataSet.Columns,
            NumberOfColumns = protoDataSet.NumberOfColumns ?? 0,
            Rows = rows,
            Types = protoDataSet.Types
        };
    }

    /// <summary>
    /// Gets the version B ProtoBuf data set from the version B data set.
    /// </summary>
    /// <param name="dataSet">The <see cref="DataSet"/>.</param>
    /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload.DataSet"/>.</returns>
    public static VersionBProtoBuf.ProtoBufPayload.DataSet? ConvertVersionBDataSet(DataSet? dataSet)
    {
        if (dataSet is null)
        {
            return null;
        }

        var rows = new List<VersionBProtoBuf.ProtoBufPayload.DataSet.Row>();
        var index = 0;

        foreach (var row in dataSet.Rows)
        {
            rows.Add(new VersionBProtoBuf.ProtoBufPayload.DataSet.Row
            {
                Elements = row.Elements.Select(e => ConvertVersionBDataSetValue(e, dataSet.Types[index])).ToList()
            });

            index++;
        }

        return new VersionBProtoBuf.ProtoBufPayload.DataSet
        {
            Columns = dataSet.Columns,
            NumberOfColumns = dataSet.NumberOfColumns,
            Rows = rows,
            Types = dataSet.Types
        };
    }

    /// <summary>
    /// Gets the version B template from the version B ProtoBuf template.
    /// </summary>
    /// <param name="protoTemplate">The <see cref="VersionBProtoBuf.ProtoBufPayload.Template"/>.</param>
    /// <returns>The <see cref="Template"/>.</returns>
    public static Template? ConvertVersionBTemplate(VersionBProtoBuf.ProtoBufPayload.Template? protoTemplate)
        => protoTemplate is null
        ? null
        : new Template
        {
            Metrics = protoTemplate.Metrics.Select(ConvertVersionBMetric).ToList(),
            IsDefinition = protoTemplate.IsDefinition,
            Parameters = protoTemplate.Parameters.Select(ConvertVersionBParameter).ToList(),
            TemplateRef = protoTemplate.TemplateRef ?? string.Empty,
            Version = protoTemplate.Version ?? string.Empty
        };

    /// <summary>
    /// Gets the version B ProtoBuf template from the version B template.
    /// </summary>
    /// <param name="template">The <see cref="Template"/>.</param>
    /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload.Template"/>.</returns>
    public static VersionBProtoBuf.ProtoBufPayload.Template? ConvertVersionBTemplate(Template? template)
        => template is null
        ? null
        : new VersionBProtoBuf.ProtoBufPayload.Template
        {
            Metrics = template.Metrics.Select(ConvertVersionBMetric).ToList(),
            IsDefinition = template.IsDefinition,
            Parameters = template.Parameters.Select(ConvertVersionBParameter).ToList(),
            TemplateRef = template.TemplateRef,
            Version = template.Version
        };

    /// <summary>
    /// Gets the version B property set from the version B ProtoBuf property set.
    /// </summary>
    /// <param name="protoPropertySet">The <see cref="VersionBProtoBuf.ProtoBufPayload.PropertySet"/>.</param>
    /// <returns>The <see cref="PropertySet"/>.</returns>
    public static PropertySet? ConvertVersionBPropertySet(VersionBProtoBuf.ProtoBufPayload.PropertySet? protoPropertySet)
        => protoPropertySet is null
        ? null
        : new PropertySet
        {
            Keys = protoPropertySet.Keys,
            Values = protoPropertySet.Values.Select(ConvertVersionBPropertyValue).ToList()
        };

    /// <summary>
    /// Gets the version B ProtoBuf property set from the version B property set.
    /// </summary>
    /// <param name="propertySet">The <see cref="PropertySet"/>.</param>
    /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload.PropertySet"/>.</returns>
    public static VersionBProtoBuf.ProtoBufPayload.PropertySet? ConvertVersionBPropertySet(PropertySet? propertySet)
        => propertySet is null
        ? null
        : new VersionBProtoBuf.ProtoBufPayload.PropertySet
        {
            Keys = propertySet.Keys,
            Values = propertySet.Values.Select(ConvertVersionBPropertyValue).ToList()
        };

    /// <summary>
    /// Gets the version B data set value from the version B ProtoBuf data set value.
    /// </summary>
    /// <param name="protoDataSetValue">The <see cref="VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue"/>.</param>
    /// <param name="protoDataType">The data type.</param>
    /// <returns>The <see cref="DataSetValue"/>.</returns>
    public static DataSetValue ConvertVersionBDataSetValue(VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue protoDataSetValue, uint protoDataType)
        => (VersionBDataTypeEnum?)protoDataType switch
        {
            VersionBDataTypeEnum.Int8 => new DataSetValue(VersionBDataTypeEnum.Int8, protoDataSetValue.IntValue),
            VersionBDataTypeEnum.Int16 => new DataSetValue(VersionBDataTypeEnum.Int16, protoDataSetValue.IntValue),
            VersionBDataTypeEnum.Int32 => new DataSetValue(VersionBDataTypeEnum.Int32, protoDataSetValue.IntValue),
            VersionBDataTypeEnum.UInt8 => new DataSetValue(VersionBDataTypeEnum.UInt8, protoDataSetValue.IntValue),
            VersionBDataTypeEnum.UInt16 => new DataSetValue(VersionBDataTypeEnum.UInt16, protoDataSetValue.IntValue),
            VersionBDataTypeEnum.Int64 => new DataSetValue(VersionBDataTypeEnum.Int64, protoDataSetValue.LongValue),
            VersionBDataTypeEnum.UInt32 => new DataSetValue(VersionBDataTypeEnum.UInt32, protoDataSetValue.LongValue),
            VersionBDataTypeEnum.UInt64 => new DataSetValue(VersionBDataTypeEnum.UInt64, protoDataSetValue.LongValue),
            VersionBDataTypeEnum.DateTime => new DataSetValue(VersionBDataTypeEnum.DateTime, protoDataSetValue.LongValue),
            VersionBDataTypeEnum.Float => new DataSetValue(VersionBDataTypeEnum.Float, protoDataSetValue.FloatValue),
            VersionBDataTypeEnum.Double => new DataSetValue(VersionBDataTypeEnum.Double, protoDataSetValue.DoubleValue),
            VersionBDataTypeEnum.Boolean => new DataSetValue(VersionBDataTypeEnum.Boolean, protoDataSetValue.BooleanValue),
            VersionBDataTypeEnum.String => new DataSetValue(VersionBDataTypeEnum.String, protoDataSetValue.StringValue),
            VersionBDataTypeEnum.Text => new DataSetValue(VersionBDataTypeEnum.Text, protoDataSetValue.StringValue),
            VersionBDataTypeEnum.Uuid => new DataSetValue(VersionBDataTypeEnum.Uuid, protoDataSetValue.StringValue),
            VersionBDataTypeEnum.Bytes
             or VersionBDataTypeEnum.File
             or VersionBDataTypeEnum.DataSet
             or VersionBDataTypeEnum.Template
             or VersionBDataTypeEnum.PropertySet
             or VersionBDataTypeEnum.PropertySetList
             or VersionBDataTypeEnum.Int8Array
             or VersionBDataTypeEnum.Int16Array
             or VersionBDataTypeEnum.Int32Array
             or VersionBDataTypeEnum.Int64Array
             or VersionBDataTypeEnum.UInt8Array
             or VersionBDataTypeEnum.UInt16Array
             or VersionBDataTypeEnum.UInt32Array
             or VersionBDataTypeEnum.UInt64Array
             or VersionBDataTypeEnum.FloatArray
             or VersionBDataTypeEnum.DoubleArray
             or VersionBDataTypeEnum.BooleanArray
             or VersionBDataTypeEnum.StringArray
             or VersionBDataTypeEnum.DateTimeArray
             or _ => throw new ArgumentOutOfRangeException(nameof(protoDataType), (VersionBDataTypeEnum?)protoDataType, "Unknown data set value data type")
        };

    /// <summary>
    /// Gets the version B ProtoBuf data set value from the version B data set value.
    /// </summary>
    /// <param name="dataSetValue">The <see cref="DataSetValue"/>.</param>
    /// <param name="dataType">The data type.</param>
    /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue"/>.</returns>
    public static VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue ConvertVersionBDataSetValue(DataSetValue dataSetValue, uint dataType)
        => (VersionBDataTypeEnum)dataType switch
        {
            VersionBDataTypeEnum.Int8
             or VersionBDataTypeEnum.Int16
             or VersionBDataTypeEnum.Int32
             or VersionBDataTypeEnum.UInt8
             or VersionBDataTypeEnum.UInt16 => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
             {
                 IntValue = dataSetValue.Value.ConvertOrDefaultTo<uint>()
             },
            VersionBDataTypeEnum.Int64
             or VersionBDataTypeEnum.UInt32
             or VersionBDataTypeEnum.UInt64
             or VersionBDataTypeEnum.DateTime => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
             {
                 LongValue = dataSetValue.Value.ConvertOrDefaultTo<ulong>()
             },
            VersionBDataTypeEnum.Float => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
            {
                FloatValue = dataSetValue.Value.ConvertOrDefaultTo<float>()
            },
            VersionBDataTypeEnum.Double => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
            {
                DoubleValue = dataSetValue.Value.ConvertOrDefaultTo<double>()
            },
            VersionBDataTypeEnum.Boolean => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
            {
                BooleanValue = dataSetValue.Value.ConvertOrDefaultTo<bool>()
            },
            VersionBDataTypeEnum.String
             or VersionBDataTypeEnum.Text
             or VersionBDataTypeEnum.Uuid => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
             {
                 StringValue = dataSetValue.Value.ConvertOrDefaultTo<string>()
             },
            VersionBDataTypeEnum.Bytes
             or VersionBDataTypeEnum.File
             or VersionBDataTypeEnum.DataSet
             or VersionBDataTypeEnum.Template
             or VersionBDataTypeEnum.PropertySet
             or VersionBDataTypeEnum.PropertySetList
             or VersionBDataTypeEnum.Int8Array
             or VersionBDataTypeEnum.Int16Array
             or VersionBDataTypeEnum.Int32Array
             or VersionBDataTypeEnum.Int64Array
             or VersionBDataTypeEnum.UInt8Array
             or VersionBDataTypeEnum.UInt16Array
             or VersionBDataTypeEnum.UInt32Array
             or VersionBDataTypeEnum.UInt64Array
             or VersionBDataTypeEnum.FloatArray
             or VersionBDataTypeEnum.DoubleArray
             or VersionBDataTypeEnum.BooleanArray
             or VersionBDataTypeEnum.StringArray
             or VersionBDataTypeEnum.DateTimeArray
             or _ => throw new ArgumentOutOfRangeException(nameof(dataSetValue.DataType), dataSetValue.DataType, "Unknown data set value data type")
        };

    /// <summary>
    /// Gets the version B parameter from the version B ProtoBuf parameter.
    /// </summary>
    /// <param name="protoParameter">The <see cref="VersionBProtoBuf.ProtoBufPayload.Template.Parameter"/>.</param>
    /// <returns>The <see cref="Parameter"/>.</returns>
    public static Parameter ConvertVersionBParameter(VersionBProtoBuf.ProtoBufPayload.Template.Parameter protoParameter)
    => (VersionBDataTypeEnum?)protoParameter.DataType switch
    {
        VersionBDataTypeEnum.Int8 => new Parameter(protoParameter.Name ?? string.Empty, VersionBDataTypeEnum.Int8, protoParameter.IntValue),
        VersionBDataTypeEnum.Int16 => new Parameter(protoParameter.Name ?? string.Empty, VersionBDataTypeEnum.Int16, protoParameter.IntValue),
        VersionBDataTypeEnum.Int32 => new Parameter(protoParameter.Name ?? string.Empty, VersionBDataTypeEnum.Int32, protoParameter.IntValue),
        VersionBDataTypeEnum.UInt8 => new Parameter(protoParameter.Name ?? string.Empty, VersionBDataTypeEnum.UInt8, protoParameter.IntValue),
        VersionBDataTypeEnum.UInt16 => new Parameter(protoParameter.Name ?? string.Empty, VersionBDataTypeEnum.UInt16, protoParameter.IntValue),
        VersionBDataTypeEnum.Int64 => new Parameter(protoParameter.Name ?? string.Empty, VersionBDataTypeEnum.Int64, protoParameter.LongValue),
        VersionBDataTypeEnum.UInt32 => new Parameter(protoParameter.Name ?? string.Empty, VersionBDataTypeEnum.UInt32, protoParameter.LongValue),
        VersionBDataTypeEnum.UInt64 => new Parameter(protoParameter.Name ?? string.Empty, VersionBDataTypeEnum.UInt64, protoParameter.LongValue),
        VersionBDataTypeEnum.DateTime => new Parameter(protoParameter.Name ?? string.Empty, VersionBDataTypeEnum.DateTime, protoParameter.LongValue),
        VersionBDataTypeEnum.Float => new Parameter(protoParameter.Name ?? string.Empty, VersionBDataTypeEnum.Float, protoParameter.FloatValue),
        VersionBDataTypeEnum.Double => new Parameter(protoParameter.Name ?? string.Empty, VersionBDataTypeEnum.Double, protoParameter.DoubleValue),
        VersionBDataTypeEnum.Boolean => new Parameter(protoParameter.Name ?? string.Empty, VersionBDataTypeEnum.Boolean, protoParameter.BooleanValue),
        VersionBDataTypeEnum.String => new Parameter(protoParameter.Name ?? string.Empty, VersionBDataTypeEnum.String, protoParameter.StringValue),
        VersionBDataTypeEnum.Text => new Parameter(protoParameter.Name ?? string.Empty, VersionBDataTypeEnum.Text, protoParameter.StringValue),
        VersionBDataTypeEnum.Uuid => new Parameter(protoParameter.Name ?? string.Empty, VersionBDataTypeEnum.Uuid, protoParameter.StringValue),
        VersionBDataTypeEnum.Bytes
         or VersionBDataTypeEnum.File
         or VersionBDataTypeEnum.DataSet
         or VersionBDataTypeEnum.Template
         or VersionBDataTypeEnum.PropertySet
         or VersionBDataTypeEnum.PropertySetList
         or VersionBDataTypeEnum.Int8Array
         or VersionBDataTypeEnum.Int16Array
         or VersionBDataTypeEnum.Int32Array
         or VersionBDataTypeEnum.Int64Array
         or VersionBDataTypeEnum.UInt8Array
         or VersionBDataTypeEnum.UInt16Array
         or VersionBDataTypeEnum.UInt32Array
         or VersionBDataTypeEnum.UInt64Array
         or VersionBDataTypeEnum.FloatArray
         or VersionBDataTypeEnum.DoubleArray
         or VersionBDataTypeEnum.BooleanArray
         or VersionBDataTypeEnum.StringArray
         or VersionBDataTypeEnum.DateTimeArray
         or _ => throw new ArgumentOutOfRangeException(nameof(protoParameter), (VersionBDataTypeEnum?)protoParameter.DataType, "Unknown parameter data type")
    };

    /// <summary>
    /// Gets the version B ProtoBuf parameter from the version B parameter.
    /// </summary>
    /// <param name="parameter">The <see cref="Parameter"/>.</param>
    /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload.Template.Parameter"/>.</returns>
    public static VersionBProtoBuf.ProtoBufPayload.Template.Parameter ConvertVersionBParameter(Parameter parameter)
    {
        var protoParameter = new VersionBProtoBuf.ProtoBufPayload.Template.Parameter()
        {
            Name = parameter.Name,
            DataType = (uint?)ConvertVersionBDataType(parameter.DataType)
        };

        switch (parameter.DataType)
        {
            case VersionBDataTypeEnum.Int8:
            case VersionBDataTypeEnum.Int16:
            case VersionBDataTypeEnum.Int32:
            case VersionBDataTypeEnum.UInt8:
            case VersionBDataTypeEnum.UInt16:
                protoParameter.IntValue = parameter.Value.ConvertOrDefaultTo<uint>();
                break;
            case VersionBDataTypeEnum.Int64:
            case VersionBDataTypeEnum.UInt32:
            case VersionBDataTypeEnum.UInt64:
            case VersionBDataTypeEnum.DateTime:
                protoParameter.LongValue = parameter.Value.ConvertOrDefaultTo<ulong>();
                break;
            case VersionBDataTypeEnum.Float:
                protoParameter.FloatValue = parameter.Value.ConvertOrDefaultTo<float>();
                break;
            case VersionBDataTypeEnum.Double:
                protoParameter.DoubleValue = parameter.Value.ConvertOrDefaultTo<double>();
                break;
            case VersionBDataTypeEnum.Boolean:
                protoParameter.BooleanValue = parameter.Value.ConvertOrDefaultTo<bool>();
                break;
            case VersionBDataTypeEnum.String:
            case VersionBDataTypeEnum.Text:
            case VersionBDataTypeEnum.Uuid:
                protoParameter.StringValue = parameter.Value.ConvertOrDefaultTo<string>();
                break;
            case VersionBDataTypeEnum.Bytes:
            case VersionBDataTypeEnum.File:
            case VersionBDataTypeEnum.DataSet:
            case VersionBDataTypeEnum.Template:
            case VersionBDataTypeEnum.PropertySet:
            case VersionBDataTypeEnum.PropertySetList:
            case VersionBDataTypeEnum.Int8Array:
            case VersionBDataTypeEnum.Int16Array:
            case VersionBDataTypeEnum.Int32Array:
            case VersionBDataTypeEnum.Int64Array:
            case VersionBDataTypeEnum.UInt8Array:
            case VersionBDataTypeEnum.UInt16Array:
            case VersionBDataTypeEnum.UInt32Array:
            case VersionBDataTypeEnum.UInt64Array:
            case VersionBDataTypeEnum.FloatArray:
            case VersionBDataTypeEnum.DoubleArray:
            case VersionBDataTypeEnum.BooleanArray:
            case VersionBDataTypeEnum.StringArray:
            case VersionBDataTypeEnum.DateTimeArray:
            case VersionBDataTypeEnum.Unknown:
            default:
                throw new ArgumentOutOfRangeException(nameof(parameter.DataType), parameter.DataType, "Unknown parameter data type");
        }

        return protoParameter;
    }

    /// <summary>
    /// Gets the version B property value from the version B ProtoBuf property value.
    /// </summary>
    /// <param name="protoPropertyValue">The <see cref="VersionBProtoBuf.ProtoBufPayload.PropertyValue"/>.</param>
    /// <returns>The <see cref="PropertyValue"/>.</returns>
    public static PropertyValue ConvertVersionBPropertyValue(VersionBProtoBuf.ProtoBufPayload.PropertyValue protoPropertyValue)
        => (VersionBDataTypeEnum?)protoPropertyValue.DataType switch
        {
            VersionBDataTypeEnum.Int8 => new PropertyValue(VersionBDataTypeEnum.Int8, protoPropertyValue.IntValue),
            VersionBDataTypeEnum.Int16 => new PropertyValue(VersionBDataTypeEnum.Int16, protoPropertyValue.IntValue),
            VersionBDataTypeEnum.Int32 => new PropertyValue(VersionBDataTypeEnum.Int32, protoPropertyValue.IntValue),
            VersionBDataTypeEnum.UInt8 => new PropertyValue(VersionBDataTypeEnum.UInt8, protoPropertyValue.IntValue),
            VersionBDataTypeEnum.UInt16 => new PropertyValue(VersionBDataTypeEnum.UInt16, protoPropertyValue.IntValue),
            VersionBDataTypeEnum.Int64 => new PropertyValue(VersionBDataTypeEnum.Int64, protoPropertyValue.LongValue),
            VersionBDataTypeEnum.UInt32 => new PropertyValue(VersionBDataTypeEnum.UInt32, protoPropertyValue.LongValue),
            VersionBDataTypeEnum.UInt64 => new PropertyValue(VersionBDataTypeEnum.UInt64, protoPropertyValue.LongValue),
            VersionBDataTypeEnum.DateTime => new PropertyValue(VersionBDataTypeEnum.DateTime, protoPropertyValue.LongValue),
            VersionBDataTypeEnum.Float => new PropertyValue(VersionBDataTypeEnum.Float, protoPropertyValue.FloatValue),
            VersionBDataTypeEnum.Double => new PropertyValue(VersionBDataTypeEnum.Double, protoPropertyValue.DoubleValue),
            VersionBDataTypeEnum.Boolean => new PropertyValue(VersionBDataTypeEnum.Boolean, protoPropertyValue.BooleanValue),
            VersionBDataTypeEnum.String => new PropertyValue(VersionBDataTypeEnum.String, protoPropertyValue.StringValue),
            VersionBDataTypeEnum.Text => new PropertyValue(VersionBDataTypeEnum.Text, protoPropertyValue.StringValue),
            VersionBDataTypeEnum.Uuid => new PropertyValue(VersionBDataTypeEnum.Uuid, protoPropertyValue.StringValue),
            VersionBDataTypeEnum.PropertySet => new PropertyValue(VersionBDataTypeEnum.PropertySet, ConvertVersionBPropertySet(protoPropertyValue.PropertySetValue)),
            VersionBDataTypeEnum.PropertySetList => new PropertyValue(VersionBDataTypeEnum.PropertySetList, ConvertVersionBPropertySetList(protoPropertyValue.PropertySetListValue)),
            VersionBDataTypeEnum.Bytes
             or VersionBDataTypeEnum.File
             or VersionBDataTypeEnum.DataSet
             or VersionBDataTypeEnum.Template
             or VersionBDataTypeEnum.Int8Array
             or VersionBDataTypeEnum.Int16Array
             or VersionBDataTypeEnum.Int32Array
             or VersionBDataTypeEnum.Int64Array
             or VersionBDataTypeEnum.UInt8Array
             or VersionBDataTypeEnum.UInt16Array
             or VersionBDataTypeEnum.UInt32Array
             or VersionBDataTypeEnum.UInt64Array
             or VersionBDataTypeEnum.FloatArray
             or VersionBDataTypeEnum.DoubleArray
             or VersionBDataTypeEnum.BooleanArray
             or VersionBDataTypeEnum.StringArray
             or VersionBDataTypeEnum.DateTimeArray
             or _ => throw new ArgumentOutOfRangeException(nameof(protoPropertyValue), (VersionBDataTypeEnum?)protoPropertyValue.DataType, "Unknown property value data type")
        };

    /// <summary>
    /// Gets the version B ProtoBuf property value from the version B property value.
    /// </summary>
    /// <param name="propertyValue">The <see cref="PropertyValue"/>.</param>
    /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload.PropertyValue"/>.</returns>
    public static VersionBProtoBuf.ProtoBufPayload.PropertyValue ConvertVersionBPropertyValue(PropertyValue propertyValue)
    {
        var protoPropertyValue = new VersionBProtoBuf.ProtoBufPayload.PropertyValue()
        {
            IsNull = propertyValue.IsNull,
            DataType = (uint?)ConvertVersionBDataType(propertyValue.DataType)
        };

        switch (propertyValue.DataType)
        {
            case VersionBDataTypeEnum.Int8:
            case VersionBDataTypeEnum.Int16:
            case VersionBDataTypeEnum.Int32:
            case VersionBDataTypeEnum.UInt8:
            case VersionBDataTypeEnum.UInt16:
                protoPropertyValue.IntValue = propertyValue.Value.ConvertOrDefaultTo<uint>();
                break;
            case VersionBDataTypeEnum.Int64:
            case VersionBDataTypeEnum.UInt32:
            case VersionBDataTypeEnum.UInt64:
            case VersionBDataTypeEnum.DateTime:
                protoPropertyValue.LongValue = propertyValue.Value.ConvertOrDefaultTo<ulong>();
                break;
            case VersionBDataTypeEnum.Float:
                protoPropertyValue.FloatValue = propertyValue.Value.ConvertOrDefaultTo<float>();
                break;
            case VersionBDataTypeEnum.Double:
                protoPropertyValue.DoubleValue = propertyValue.Value.ConvertOrDefaultTo<double>();
                break;
            case VersionBDataTypeEnum.Boolean:
                protoPropertyValue.BooleanValue = propertyValue.Value.ConvertOrDefaultTo<bool>();
                break;
            case VersionBDataTypeEnum.String:
            case VersionBDataTypeEnum.Text:
            case VersionBDataTypeEnum.Uuid:
                protoPropertyValue.StringValue = propertyValue.Value.ConvertOrDefaultTo<string>();
                break;
            case VersionBDataTypeEnum.PropertySet:
                protoPropertyValue.PropertySetValue = ConvertVersionBPropertySet(propertyValue.Value.ConvertOrDefaultTo<PropertySet>());
                break;
            case VersionBDataTypeEnum.PropertySetList:
                protoPropertyValue.PropertySetListValue = ConvertVersionBPropertySetList(propertyValue.Value.ConvertOrDefaultTo<PropertySetList>());
                break;
            case VersionBDataTypeEnum.Bytes:
            case VersionBDataTypeEnum.File:
            case VersionBDataTypeEnum.DataSet:
            case VersionBDataTypeEnum.Template:
            case VersionBDataTypeEnum.Int8Array:
            case VersionBDataTypeEnum.Int16Array:
            case VersionBDataTypeEnum.Int32Array:
            case VersionBDataTypeEnum.Int64Array:
            case VersionBDataTypeEnum.UInt8Array:
            case VersionBDataTypeEnum.UInt16Array:
            case VersionBDataTypeEnum.UInt32Array:
            case VersionBDataTypeEnum.UInt64Array:
            case VersionBDataTypeEnum.FloatArray:
            case VersionBDataTypeEnum.DoubleArray:
            case VersionBDataTypeEnum.BooleanArray:
            case VersionBDataTypeEnum.StringArray:
            case VersionBDataTypeEnum.DateTimeArray:
            case VersionBDataTypeEnum.Unknown:
            default:
                throw new ArgumentOutOfRangeException(nameof(propertyValue.DataType), propertyValue.DataType, "Unknown property value data type");
        }

        return protoPropertyValue;
    }

    /// <summary>
    /// Gets the version B ProtoBuf property set list from the version B property set list.
    /// </summary>
    /// <param name="propertySetList">The <see cref="PropertySetList"/>.</param>
    /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload.PropertySetList"/>.</returns>
    public static VersionBProtoBuf.ProtoBufPayload.PropertySetList? ConvertVersionBPropertySetList(PropertySetList? propertySetList)
    {
        if (propertySetList is null)
        {
            return null;
        }

        if (propertySetList.PropertySets is null)
        {
            throw new ArgumentNullException(nameof(propertySetList), "Property sets are not set");
        }

        return new VersionBProtoBuf.ProtoBufPayload.PropertySetList
        {
            PropertySets = propertySetList.PropertySets.Select(ConvertVersionBPropertySet).ToNonNullList()
        };
    }

    /// <summary>
    /// Gets the version B property set list from the version B ProtoBuf property set list.
    /// </summary>
    /// <param name="protoPropertySetList">The <see cref="VersionBProtoBuf.ProtoBufPayload.PropertySetList"/>.</param>
    /// <returns>The <see cref="PropertySetList"/>.</returns>
    public static PropertySetList? ConvertVersionBPropertySetList(VersionBProtoBuf.ProtoBufPayload.PropertySetList? protoPropertySetList)
    {
        if (protoPropertySetList is null)
        {
            return null;
        }

        if (protoPropertySetList.PropertySets is null)
        {
            throw new ArgumentNullException(nameof(protoPropertySetList), "Property sets are not set");
        }

        return new PropertySetList
        {
            PropertySets = protoPropertySetList.PropertySets.Select(ConvertVersionBPropertySet).ToNonNullList()
        };
    }
}
