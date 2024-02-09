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
    /// Gets the version A payload converted from the ProtoBuf payload.
    /// </summary>
    /// <param name="payload">The <see cref="VersionAProtoBuf.ProtoBufPayload"/>.</param>
    /// <returns>The <see cref="VersionAData.Payload"/>.</returns>
    public static VersionAData.Payload ConvertVersionAPayload(VersionAProtoBuf.ProtoBufPayload payload)
        => new VersionAData.Payload
        {
            Body = payload.Body,
            Metrics = payload.Metrics.Select(m => new VersionAData.KuraMetric
            {
                Name = m.Name,
                DataType = ConvertVersionADataType(m.Type),
                BooleanValue = m.BoolValue ?? default,
                BytesValue = m.BytesValue ?? Array.Empty<byte>(),
                DoubleValue = m.DoubleValue ?? default,
                FloatValue = m.FloatValue ?? default,
                IntValue = m.IntValue ?? default,
                LongValue = m.LongValue ?? default,
                StringValue = m.StringValue ?? string.Empty
            }).ToList(),
            Position = new VersionAData.KuraPosition
            {
                Timestamp = payload.Position?.Timestamp ?? default,
                Altitude = payload.Position?.Altitude ?? default,
                Heading = payload.Position?.Heading ?? default,
                Latitude = payload.Position?.Latitude ?? default,
                Longitude = payload.Position?.Longitude ?? default,
                Precision = payload.Position?.Precision ?? default,
                Satellites = payload.Position?.Satellites ?? default,
                Speed = payload.Position?.Speed ?? default,
                Status = payload.Position?.Status ?? default
            },
            Timestamp = payload.Timestamp
        };

    /// <summary>
    /// Gets the ProtoBuf payload converted from the version A payload.
    /// </summary>
    /// <param name="payload">The <see cref="VersionAData.Payload"/>.</param>
    /// <returns>The <see cref="VersionAProtoBuf.ProtoBufPayload"/>.</returns>
    public static VersionAProtoBuf.ProtoBufPayload ConvertVersionAPayload(VersionAData.Payload payload)
        => new VersionAProtoBuf.ProtoBufPayload
        {
            Body = payload.Body,
            Metrics = payload.Metrics.Select(m => new VersionAProtoBuf.ProtoBufPayload.KuraMetric
            {
                Type = ConvertVersionADataType(m.DataType),
                BoolValue = m.BooleanValue,
                BytesValue = m.BytesValue,
                DoubleValue = m.DoubleValue,
                FloatValue = m.FloatValue,
                IntValue = m.IntValue,
                LongValue = m.LongValue,
                Name = m.Name,
                StringValue = m.StringValue
            }).ToList(),
            Position = new VersionAProtoBuf.ProtoBufPayload.KuraPosition
            {
                Timestamp = payload.Position?.Timestamp ?? default,
                Altitude = payload.Position?.Altitude ?? default,
                Heading = payload.Position?.Heading ?? default,
                Latitude = payload.Position?.Latitude ?? default,
                Longitude = payload.Position?.Longitude ?? default,
                Precision = payload.Position?.Precision ?? default,
                Satellites = payload.Position?.Satellites ?? default,
                Speed = payload.Position?.Speed ?? default,
                Status = payload.Position?.Status ?? default
            },
            Timestamp = payload.Timestamp
        };

    /// <summary>
    /// Gets the version B payload converted from the ProtoBuf payload.
    /// </summary>
    /// <param name="payload">The <see cref="VersionBProtoBuf.ProtoBufPayload"/>.</param>
    /// <returns>The <see cref="Payload"/>.</returns>
    public static Payload ConvertVersionBPayload(VersionBProtoBuf.ProtoBufPayload payload)
        => new Payload
        {
            Body = payload.Body,
            Metrics = payload.Metrics.Select(ConvertVersionBMetric).ToList(),
            Seq = payload.Seq,
            Timestamp = payload.Timestamp,
            Uuid = payload.Uuid
        };

    /// <summary>
    /// Gets the ProtoBuf payload converted from the version B payload.
    /// </summary>
    /// <param name="payload">The <see cref="Payload"/>.</param>
    /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload"/>.</returns>
    public static VersionBProtoBuf.ProtoBufPayload ConvertVersionBPayload(Payload payload)
        => new VersionBProtoBuf.ProtoBufPayload
        {
            Body = payload.Body,
            Metrics = payload.Metrics.Select(ConvertVersionBMetric).ToList(),
            Seq = payload.Seq,
            Timestamp = payload.Timestamp,
            Uuid = payload.Uuid
        };

    /// <summary>
    /// Gets the version A data type from the version A ProtoBuf value type.
    /// </summary>
    /// <param name="type">The <see cref="VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType"/>.</param>
    /// <returns>The <see cref="VersionAData.DataType"/>.</returns>
    public static VersionAData.DataType ConvertVersionADataType(VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType type)
        => type switch
        {
            VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType.Bool => VersionAData.DataType.Boolean,
            VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType.Bytes => VersionAData.DataType.Bytes,
            VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType.Double => VersionAData.DataType.Double,
            VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType.Float => VersionAData.DataType.Float,
            VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType.Int32 => VersionAData.DataType.Int32,
            VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType.Int64 => VersionAData.DataType.Int64,
            VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType.String => VersionAData.DataType.String,
            _ => VersionAData.DataType.String
        };

    /// <summary>
    /// Gets the version A ProtoBuf value type from the version A data type.
    /// </summary>
    /// <param name="type">The <see cref="VersionAData.DataType"/>.</param>
    /// <returns>The <see cref="VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType"/>.</returns>
    public static VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType ConvertVersionADataType(VersionAData.DataType type)
        => type switch
        {
            VersionAData.DataType.Boolean => VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType.Bool,
            VersionAData.DataType.Bytes => VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType.Bytes,
            VersionAData.DataType.Double => VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType.Double,
            VersionAData.DataType.Float => VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType.Float,
            VersionAData.DataType.Int32 => VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType.Int32,
            VersionAData.DataType.Int64 => VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType.Int64,
            VersionAData.DataType.String => VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType.String,
            _ => VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType.String
        };

    /// <summary>
    /// Gets the version B data type from the version B ProtoBuf data type.
    /// </summary>
    /// <param name="type">The <see cref="VersionBProtoBuf.DataType"/>.</param>
    /// <returns>The <see cref="VersionBDataTypeEnum"/>.</returns>
    public static VersionBDataTypeEnum ConvertVersionBDataType(VersionBProtoBuf.DataType type)
        => type switch
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
    /// <param name="type">The <see cref="VersionBDataTypeEnum"/>.</param>
    /// <returns>The <see cref="VersionBProtoBuf.DataType"/>.</returns>
    public static VersionBProtoBuf.DataType ConvertVersionBDataType(VersionBDataTypeEnum type)
        => type switch
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
    /// Gets the version B data set from the version B ProtoBuf data set.
    /// </summary>
    /// <param name="dataSet">The <see cref="VersionBProtoBuf.ProtoBufPayload.DataSet"/>.</param>
    /// <returns>The <see cref="DataSet"/>.</returns>
    private static DataSet? ConvertVersionBDataSet(VersionBProtoBuf.ProtoBufPayload.DataSet? dataSet)
    {
        if (dataSet is null)
        {
            return null;
        }

        var rows = new List<Row>();
        var index = 0;

        foreach (var row in dataSet.Rows)
        {
            rows.Add(new Row
            {
                Elements = row.Elements.Select(e => ConvertVersionBDataSetValue(e, dataSet.Types[index])).ToList()
            });

            index++;
        }

        return new DataSet
        {
            Columns = dataSet.Columns,
            NumOfColumns = dataSet.NumOfColumns,
            Rows = rows,
            Types = dataSet.Types
        };
    }

    /// <summary>
    /// Gets the version B ProtoBuf data set from the version B data set.
    /// </summary>
    /// <param name="dataSet">The <see cref="DataSet"/>.</param>
    /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload.DataSet"/>.</returns>
    private static VersionBProtoBuf.ProtoBufPayload.DataSet? ConvertVersionBDataSet(DataSet? dataSet)
    {
        if (dataSet is null)
        {
            return null;
        }

        return new VersionBProtoBuf.ProtoBufPayload.DataSet
        {
            Columns = dataSet.Columns,
            NumOfColumns = dataSet.NumOfColumns,
            Rows = dataSet.Rows.Select(ConvertVersionBRow).ToList(),
            Types = dataSet.Types
        };
    }

    /// <summary>
    /// Gets the version B data set value from the version B ProtoBuf data set value.
    /// </summary>
    /// <param name="dataSetValue">The <see cref="VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue"/>.</param>
    /// <param name="dataType">The data type.</param>
    /// <returns>The <see cref="DataSetValue"/>.</returns>
    private static DataSetValue ConvertVersionBDataSetValue(
        VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue dataSetValue,
        uint dataType)
        => new DataSetValue
        {
            DoubleValue = dataSetValue.DoubleValue,
            BooleanValue = dataSetValue.BooleanValue,
            // Todo: How to handle this properly?
            ExtensionValue = new DataSetValueExtension
            {
            },
            FloatValue = dataSetValue.FloatValue,
            IntValue = dataSetValue.IntValue,
            LongValue = dataSetValue.LongValue,
            StringValue = dataSetValue.StringValue,
            DataType = ConvertVersionBDataType((VersionBProtoBuf.DataType)dataType)
        };

    /// <summary>
    /// Gets the version B ProtoBuf data set value from the version B data set value.
    /// </summary>
    /// <param name="dataSetValue">The <see cref="DataSetValue"/>.</param>
    /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue"/>.</returns>
    private static VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue ConvertVersionBDataSetValue(DataSetValue dataSetValue)
        => dataSetValue.DataType switch
        {
            VersionBDataTypeEnum.Int8
                or VersionBDataTypeEnum.Int16
                or VersionBDataTypeEnum.Int32
                or VersionBDataTypeEnum.UInt8
                or VersionBDataTypeEnum.UInt16
                or VersionBDataTypeEnum.UInt32 => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
                {
                    IntValue = dataSetValue.IntValue
                },
            VersionBDataTypeEnum.Int64
                or VersionBDataTypeEnum.UInt64
                or VersionBDataTypeEnum.DateTime => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
                {
                    LongValue = dataSetValue.LongValue
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
                or _ => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
                {
                    ExtensionValue = new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.DataSetValueExtension
                    {
                    }
                }
        };

    /// <summary>
    /// Gets the version B row from the version B ProtoBuf row.
    /// </summary>
    /// <param name="row">The <see cref="VersionBProtoBuf.ProtoBufPayload.DataSet.Row"/>.</param>
    /// <param name="dataType">The data type.</param>
    /// <returns>The <see cref="Row"/>.</returns>
    private static Row ConvertVersionBRow(VersionBProtoBuf.ProtoBufPayload.DataSet.Row row, uint dataType)
        => new Row
        {
            Elements = row.Elements.Select(e => ConvertVersionBDataSetValue(e, dataType)).ToList()
        };

    /// <summary>
    /// Gets the version B ProtoBuf row from the version B row.
    /// </summary>
    /// <param name="row">The <see cref="Row"/>.</param>
    /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload.DataSet.Row"/>.</returns>
    private static VersionBProtoBuf.ProtoBufPayload.DataSet.Row ConvertVersionBRow(Row row)
        => new VersionBProtoBuf.ProtoBufPayload.DataSet.Row
        {
            Elements = row.Elements.Select(ConvertVersionBDataSetValue).ToList()
        };

    /// <summary>
    /// Gets the version B meta data from the version B ProtoBuf meta data.
    /// </summary>
    /// <param name="metaData">The <see cref="VersionBProtoBuf.ProtoBufPayload.MetaData"/>.</param>
    /// <returns>The <see cref="MetaData"/>.</returns>
    private static MetaData? ConvertVersionBMetaData(VersionBProtoBuf.ProtoBufPayload.MetaData? metaData)
        => metaData is null
        ? null
        : new MetaData
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
    /// Gets the version B ProtoBuf meta data from the version B meta data.
    /// </summary>
    /// <param name="metaData">The <see cref="MetaData"/>.</param>
    /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload.MetaData"/>.</returns>
    private static VersionBProtoBuf.ProtoBufPayload.MetaData? ConvertVersionBMetaData(MetaData? metaData)
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
    /// Gets the version B template from the version B ProtoBuf template.
    /// </summary>
    /// <param name="template">The <see cref="VersionBProtoBuf.ProtoBufPayload.Template"/>.</param>
    /// <returns>The <see cref="Template"/>.</returns>
    private static Template? ConvertVersionBTemplate(VersionBProtoBuf.ProtoBufPayload.Template? template)
        => template is null
        ? null
        : new Template
        {
            Metrics = template.Metrics.Select(ConvertVersionBMetric).ToList(),
            IsDefinition = template.IsDefinition,
            Parameters = template.Parameters.Select(ConvertVersionBParameter).ToList(),
            TemplateRef = template.TemplateRef,
            Version = template.Version
        };

    /// <summary>
    /// Gets the version B ProtoBuf template from the version B template.
    /// </summary>
    /// <param name="template">The <see cref="Template"/>.</param>
    /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload.Template"/>.</returns>
    private static VersionBProtoBuf.ProtoBufPayload.Template? ConvertVersionBTemplate(Template? template)
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
    /// Gets the version B parameter from the version B ProtoBuf parameter.
    /// </summary>
    /// <param name="parameter">The <see cref="VersionBProtoBuf.ProtoBufPayload.Template.Parameter"/>.</param>
    /// <returns>The <see cref="Parameter"/>.</returns>
    private static Parameter ConvertVersionBParameter(VersionBProtoBuf.ProtoBufPayload.Template.Parameter parameter)
        => new Parameter
        {
            DoubleValue = parameter.DoubleValue,
            BooleanValue = parameter.BooleanValue,
            // Todo: How to handle this properly?
            ExtensionValue = new ParameterValueExtension
            {
            },
            FloatValue = parameter.FloatValue,
            IntValue = parameter.IntValue,
            LongValue = parameter.LongValue,
            Name = parameter.Name,
            StringValue = parameter.StringValue,
            ValueCase = parameter.Type,
            DataType = ConvertVersionBDataType((VersionBProtoBuf.DataType)parameter.Type)
        };

    /// <summary>
    /// Gets the version B ProtoBuf parameter from the version B parameter.
    /// </summary>
    /// <param name="parameter">The <see cref="Parameter"/>.</param>
    /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload.Template.Parameter"/>.</returns>
    private static VersionBProtoBuf.ProtoBufPayload.Template.Parameter ConvertVersionBParameter(Parameter parameter)
    {
        return new()
        {
            Name = parameter.Name,
            Type = parameter.ValueCase,
            BooleanValue = parameter.BooleanValue,
            DoubleValue = parameter.DoubleValue,
            // Todo: How to handle this properly?
            ExtensionValue = parameter.ExtensionValue is null ? null : new VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ParameterValueExtension { },
            FloatValue = parameter.FloatValue,
            IntValue = parameter.IntValue,
            LongValue = parameter.LongValue,
            StringValue = parameter.StringValue
        };
    }

    /// <summary>
    /// Gets the version B metric from the version B ProtoBuf metric.
    /// </summary>
    /// <param name="metric">The <see cref="VersionBProtoBuf.ProtoBufPayload.Metric"/>.</param>
    /// <returns>The <see cref="Metric"/>.</returns>
    private static Metric ConvertVersionBMetric(VersionBProtoBuf.ProtoBufPayload.Metric metric)
        => new Metric
        {
            DoubleValue = metric.DoubleValue,
            Alias = metric.Alias,
            BooleanValue = metric.BooleanValue,
            BytesValue = metric.BytesValue,
            DataSetValue = ConvertVersionBDataSet(metric.DatasetValue),
            ValueCase = metric.Datatype,
            // Todo: How to handle this properly?
            ExtensionValue = (metric.ExtensionValue is not null) ? new MetricValueExtension
            {
            } : null,
            FloatValue = metric.FloatValue,
            IntValue = metric.IntValue,
            IsHistorical = metric.IsHistorical,
            IsNull = metric.IsNull,
            IsTransient = metric.IsTransient,
            LongValue = metric.LongValue,
            Metadata = ConvertVersionBMetaData(metric.Metadata),
            Name = metric.Name,
            Properties = ConvertVersionBPropertySet(metric.Properties),
            StringValue = metric.StringValue,
            Timestamp = metric.Timestamp,
            TemplateValue = ConvertVersionBTemplate(metric.TemplateValue),
            DataType = ConvertVersionBDataType((VersionBProtoBuf.DataType)metric.Datatype)
        };

    /// <summary>
    /// Gets the version B ProtoBuf metric from the version B metric.
    /// </summary>
    /// <param name="metric">The <see cref="Metric"/>.</param>
    /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload.Metric"/>.</returns>
    private static VersionBProtoBuf.ProtoBufPayload.Metric ConvertVersionBMetric(Metric metric)
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
            case VersionBDataTypeEnum.UInt8:
            case VersionBDataTypeEnum.UInt16:
            case VersionBDataTypeEnum.UInt32:
                pbMetric.IntValue = metric.IntValue;
                break;
            case VersionBDataTypeEnum.Int64:
            case VersionBDataTypeEnum.UInt64:
            case VersionBDataTypeEnum.DateTime:
                pbMetric.LongValue = metric.LongValue;
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
                pbMetric.ExtensionValue = (metric.ExtensionValue is null)
                    ? null
                    : new VersionBProtoBuf.ProtoBufPayload.Metric.MetricValueExtension { };
                break;
        }

        return pbMetric;
    }

    /// <summary>
    /// Gets the version B property set list from the version B ProtoBuf property set list.
    /// </summary>
    /// <param name="propertySetList">The <see cref="VersionBProtoBuf.ProtoBufPayload.PropertySetList"/>.</param>
    /// <returns>The <see cref="PropertySetList"/>.</returns>
    private static PropertySetList? ConvertVersionBPropertySetList(VersionBProtoBuf.ProtoBufPayload.PropertySetList? propertySetList)
    {
        if (propertySetList is null)
        {
            return null;
        }

        if (propertySetList.Propertysets is null)
        {
            throw new ArgumentNullException(nameof(propertySetList), "Propertysets is not set");
        }

        return new PropertySetList
        {
            PropertySets = propertySetList.Propertysets.Select(ConvertVersionBPropertySet).ToNonNullList()
        };
    }

    /// <summary>
    /// Gets the version B ProtoBuf property set list from the version B property set list.
    /// </summary>
    /// <param name="propertySetList">The <see cref="PropertySetList"/>.</param>
    /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload.PropertySetList"/>.</returns>
    private static VersionBProtoBuf.ProtoBufPayload.PropertySetList? ConvertVersionBPropertySetList(PropertySetList? propertySetList)
        => propertySetList is null
        ? null
        : new VersionBProtoBuf.ProtoBufPayload.PropertySetList
        {
            Propertysets = propertySetList.PropertySets.Select(ConvertVersionBPropertySet).ToNonNullList()
        };

    /// <summary>
    /// Gets the version B property set from the version B ProtoBuf property set.
    /// </summary>
    /// <param name="propertySet">The <see cref="VersionBProtoBuf.ProtoBufPayload.PropertySet"/>.</param>
    /// <returns>The <see cref="PropertySet"/>.</returns>
    private static PropertySet? ConvertVersionBPropertySet(VersionBProtoBuf.ProtoBufPayload.PropertySet? propertySet)
        => propertySet is null
        ? null
        : new PropertySet
        {
            Keys = propertySet.Keys,
            Values = propertySet.Values.Select(ConvertVersionBPropertyValue).ToList()
        };

    /// <summary>
    /// Gets the version B ProtoBuf property set from the version B property set.
    /// </summary>
    /// <param name="propertySet">The <see cref="PropertySet"/>.</param>
    /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload.PropertySet"/>.</returns>
    private static VersionBProtoBuf.ProtoBufPayload.PropertySet? ConvertVersionBPropertySet(PropertySet? propertySet)
        => propertySet is null
        ? null
        : new VersionBProtoBuf.ProtoBufPayload.PropertySet
        {
            Keys = propertySet.Keys,
            Values = propertySet.Values.Select(ConvertVersionBPropertyValue).ToList()
        };

    /// <summary>
    /// Gets the version B property value from the version B ProtoBuf property value.
    /// </summary>
    /// <param name="propertyValue">The <see cref="VersionBProtoBuf.ProtoBufPayload.PropertyValue"/>.</param>
    /// <returns>The <see cref="PropertyValue"/>.</returns>
    private static PropertyValue ConvertVersionBPropertyValue(VersionBProtoBuf.ProtoBufPayload.PropertyValue propertyValue)
        => new PropertyValue
        {
            DoubleValue = propertyValue.DoubleValue,
            PropertySetsValue = ConvertVersionBPropertySetList(propertyValue.PropertysetsValue),
            BooleanValue = propertyValue.BooleanValue,
            // Todo: How to handle this properly?
            ExtensionValue = new PropertyValueExtension
            {
            },
            FloatValue = propertyValue.FloatValue,
            IntValue = propertyValue.IntValue,
            IsNull = propertyValue.IsNull,
            LongValue = propertyValue.LongValue,
            PropertySetValue = ConvertVersionBPropertySet(propertyValue.PropertysetValue),
            StringValue = propertyValue.StringValue,
            ValueCase = propertyValue.Type,
            DataType = ConvertVersionBDataType((VersionBProtoBuf.DataType)propertyValue.Type)
        };

    /// <summary>
    /// Gets the version B ProtoBuf property value from the version B property value.
    /// </summary>
    /// <param name="propertyValue">The <see cref="PropertyValue"/>.</param>
    /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload.PropertyValue"/>.</returns>
    private static VersionBProtoBuf.ProtoBufPayload.PropertyValue ConvertVersionBPropertyValue(PropertyValue propertyValue)
    {
        return new()
        {
            IsNull = propertyValue.IsNull,
            Type = propertyValue.ValueCase,
            BooleanValue = propertyValue.BooleanValue,
            DoubleValue = propertyValue.DoubleValue,
            // Todo: How to handle this properly?
            ExtensionValue = new VersionBProtoBuf.ProtoBufPayload.PropertyValue.PropertyValueExtension
            {
            },
            FloatValue = propertyValue.FloatValue,
            IntValue = propertyValue.IntValue,
            LongValue = propertyValue.LongValue,
            StringValue = propertyValue.StringValue,
            PropertysetValue = ConvertVersionBPropertySet(propertyValue.PropertySetValue),
            PropertysetsValue = ConvertVersionBPropertySetList(propertyValue.PropertySetsValue)
        };
    }
}
