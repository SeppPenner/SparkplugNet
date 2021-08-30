// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PayloadConverter.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A helper class for the payload conversions from internal ProtoBuf model to external data and vice versa.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Core
{
    using System.Linq;

    using SparkplugNet.VersionA.ProtoBuf;

    using VersionAData = VersionA.Data;
    using VersionAProtoBuf = VersionA.ProtoBuf;
    using VersionBData = VersionB.Data;
    using VersionBProtoBuf = VersionB.ProtoBuf;

    /// <summary>
    /// A helper class for the payload conversions from internal ProtoBuf model to external data and vice versa.
    /// </summary>
    // ReSharper disable once StyleCop.SA1650
    internal static class PayloadConverter
    {
        /// <summary>
        /// Gets the version A payload converted from the ProtoBuf payload.
        /// </summary>
        /// <param name="payload">The <see cref="ProtoBufPayload"/>.</param>
        /// <returns>The <see cref="VersionAData.Payload"/>.</returns>
        // ReSharper disable once StyleCop.SA1650
        public static VersionAData.Payload ConvertVersionAPayload(ProtoBufPayload payload)
        {
            return new VersionAData.Payload
            {
                Body = payload.Body,
                Metrics = payload.Metrics.Select(m => new VersionAData.KuraMetric
                {
                    Type = ConvertVersionADataType(m.Type),
                    BoolValue = m.BoolValue,
                    BytesValue = m.BytesValue,
                    DoubleValue = m.DoubleValue,
                    FloatValue = m.FloatValue,
                    IntValue = m.IntValue,
                    LongValue = m.LongValue,
                    Name = m.Name,
                    StringValue = m.StringValue
                }).ToList(),
                Position = new VersionAData.KuraPosition
                {
                    Timestamp = payload.Position.Timestamp,
                    Altitude = payload.Position.Altitude,
                    Heading = payload.Position.Heading,
                    Latitude = payload.Position.Latitude,
                    Longitude = payload.Position.Longitude,
                    Precision = payload.Position.Precision,
                    Satellites = payload.Position.Satellites,
                    Speed = payload.Position.Speed,
                    Status = payload.Position.Status
                },
                Timestamp = payload.Timestamp
            };
        }

        /// <summary>
        /// Gets the ProtoBuf payload converted from the version A payload.
        /// </summary>
        /// <param name="payload">The <see cref="VersionAData.Payload"/>.</param>
        /// <returns>The <see cref="VersionAProtoBuf.ProtoBufPayload"/>.</returns>
        // ReSharper disable once StyleCop.SA1650
        public static ProtoBufPayload ConvertVersionAPayload(VersionAData.Payload payload)
        {
            return new ProtoBufPayload
            {
                Body = payload.Body,
                Metrics = payload.Metrics.Select(m => new ProtoBufPayload.KuraMetric
                {
                    Type = ConvertVersionADataType(m.Type),
                    BoolValue = m.BoolValue,
                    BytesValue = m.BytesValue,
                    DoubleValue = m.DoubleValue,
                    FloatValue = m.FloatValue,
                    IntValue = m.IntValue,
                    LongValue = m.LongValue,
                    Name = m.Name,
                    StringValue = m.StringValue
                }).ToList(),
                Position = new ProtoBufPayload.KuraPosition
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
        }

        /// <summary>
        /// Gets the version B payload converted from the ProtoBuf payload.
        /// </summary>
        /// <param name="payload">The <see cref="VersionBProtoBuf.ProtoBufPayload"/>.</param>
        /// <returns>The <see cref="VersionBData.Payload"/>.</returns>
        // ReSharper disable once StyleCop.SA1650
        public static VersionBData.Payload ConvertVersionBPayload(VersionBProtoBuf.ProtoBufPayload payload)
        {
            return new VersionBData.Payload
            {
                Body = payload.Body,
                Details = payload.Details,
                Metrics = payload.Metrics.Select(ConvertVersionBMetric).ToList(),
                Seq = payload.Seq,
                Timestamp = payload.Timestamp,
                Uuid = payload.Uuid
            };
        }

        /// <summary>
        /// Gets the ProtoBuf payload converted from the version B payload.
        /// </summary>
        /// <param name="payload">The <see cref="VersionBData.Payload"/>.</param>
        /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload"/>.</returns>
        // ReSharper disable once StyleCop.SA1650
        public static VersionBProtoBuf.ProtoBufPayload ConvertVersionBPayload(VersionBData.Payload payload)
        {
            return new VersionBProtoBuf.ProtoBufPayload
            {
                Body = payload.Body,
                Details = payload.Details,
                Metrics = payload.Metrics.Select(ConvertVersionBMetric).ToList(),
                Seq = payload.Seq,
                Timestamp = payload.Timestamp,
                Uuid = payload.Uuid
            };
        }

        /// <summary>
        /// Gets the version A data type from the version A ProtoBuf value type.
        /// </summary>
        /// <param name="type">The <see cref="ProtoBufPayload.KuraMetric.ValueType"/>.</param>
        /// <returns>The <see cref="VersionAData.DataType"/>.</returns>
        // ReSharper disable once StyleCop.SA1650
        private static VersionAData.DataType ConvertVersionADataType(ProtoBufPayload.KuraMetric.ValueType type)
        {
            return type switch
            {
                ProtoBufPayload.KuraMetric.ValueType.Bool => VersionAData.DataType.Bool,
                ProtoBufPayload.KuraMetric.ValueType.Bytes => VersionAData.DataType.Bytes,
                ProtoBufPayload.KuraMetric.ValueType.Double => VersionAData.DataType.Double,
                ProtoBufPayload.KuraMetric.ValueType.Float => VersionAData.DataType.Float,
                ProtoBufPayload.KuraMetric.ValueType.Int32 => VersionAData.DataType.Int32,
                ProtoBufPayload.KuraMetric.ValueType.Int64 => VersionAData.DataType.Int64,
                ProtoBufPayload.KuraMetric.ValueType.String => VersionAData.DataType.String,
                _ => VersionAData.DataType.String
            };
        }

        /// <summary>
        /// Gets the version A ProtoBuf value type from the version A data type.
        /// </summary>
        /// <param name="type">The <see cref="VersionAData.DataType"/>.</param>
        /// <returns>The <see cref="ProtoBufPayload.KuraMetric.ValueType"/>.</returns>
        // ReSharper disable once StyleCop.SA1650
        private static ProtoBufPayload.KuraMetric.ValueType ConvertVersionADataType(VersionAData.DataType type)
        {
            return type switch
            {
                VersionAData.DataType.Bool => ProtoBufPayload.KuraMetric.ValueType.Bool,
                VersionAData.DataType.Bytes => ProtoBufPayload.KuraMetric.ValueType.Bytes,
                VersionAData.DataType.Double => ProtoBufPayload.KuraMetric.ValueType.Double,
                VersionAData.DataType.Float => ProtoBufPayload.KuraMetric.ValueType.Float,
                VersionAData.DataType.Int32 => ProtoBufPayload.KuraMetric.ValueType.Int32,
                VersionAData.DataType.Int64 => ProtoBufPayload.KuraMetric.ValueType.Int64,
                VersionAData.DataType.String => ProtoBufPayload.KuraMetric.ValueType.String,
                _ => ProtoBufPayload.KuraMetric.ValueType.String
            };
        }

        /// <summary>
        /// Gets the version B data set from the version B ProtoBuf data set.
        /// </summary>
        /// <param name="dataSet">The <see cref="VersionBProtoBuf.ProtoBufPayload.DataSet"/>.</param>
        /// <returns>The <see cref="VersionBData.DataSet"/>.</returns>
        private static VersionBData.DataSet ConvertVersionBDataSet(VersionBProtoBuf.ProtoBufPayload.DataSet dataSet)
        {
            return new VersionBData.DataSet
            {
                Details = dataSet.Details,
                Columns = dataSet.Columns,
                NumOfColumns = dataSet.NumOfColumns,
                Rows = dataSet.Rows.Select(ConvertVersionBRow).ToList(),
                Types = dataSet.Types
            };
        }

        /// <summary>
        /// Gets the version B ProtoBuf data set from the version B data set.
        /// </summary>
        /// <param name="dataSet">The <see cref="VersionBData.DataSet"/>.</param>
        /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload.DataSet"/>.</returns>
        private static VersionBProtoBuf.ProtoBufPayload.DataSet ConvertVersionBDataSet(VersionBData.DataSet dataSet)
        {
            return new VersionBProtoBuf.ProtoBufPayload.DataSet
            {
                Details = dataSet.Details,
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
        /// <returns>The <see cref="VersionBData.DataSetValue"/>.</returns>
        private static VersionBData.DataSetValue ConvertVersionBDataSetValue(VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue dataSetValue)
        {
            return new VersionBData.DataSetValue
            {
                DoubleValue = dataSetValue.DoubleValue,
                BooleanValue = dataSetValue.BooleanValue,
                ExtensionValue = new VersionBData.DataSetValueExtension
                {
                    Details = dataSetValue.ExtensionValue.Details
                },
                FloatValue = dataSetValue.FloatValue,
                IntValue = dataSetValue.IntValue,
                LongValue = dataSetValue.LongValue,
                StringValue = dataSetValue.StringValue
            };
        }

        /// <summary>
        /// Gets the version B ProtoBuf data set value from the version B data set value.
        /// </summary>
        /// <param name="dataSetValue">The <see cref="VersionBData.DataSetValue"/>.</param>
        /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue"/>.</returns>
        private static VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue ConvertVersionBDataSetValue(VersionBData.DataSetValue dataSetValue)
        {
            return new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
            {
                DoubleValue = dataSetValue.DoubleValue,
                BooleanValue = dataSetValue.BooleanValue,
                ExtensionValue = new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.DataSetValueExtension
                {
                    Details = dataSetValue.ExtensionValue.Details
                },
                FloatValue = dataSetValue.FloatValue,
                IntValue = dataSetValue.IntValue,
                LongValue = dataSetValue.LongValue,
                StringValue = dataSetValue.StringValue
            };
        }

        /// <summary>
        /// Gets the version B row from the version B ProtoBuf row.
        /// </summary>
        /// <param name="row">The <see cref="VersionBProtoBuf.ProtoBufPayload.DataSet.Row"/>.</param>
        /// <returns>The <see cref="VersionBData.Row"/>.</returns>
        private static VersionBData.Row ConvertVersionBRow(VersionBProtoBuf.ProtoBufPayload.DataSet.Row row)
        {
            return new VersionBData.Row
            {
                Details = row.Details,
                Elements = row.Elements.Select(ConvertVersionBDataSetValue).ToList()
            };
        }

        /// <summary>
        /// Gets the version B ProtoBuf row from the version B row.
        /// </summary>
        /// <param name="row">The <see cref="VersionBData.Row"/>.</param>
        /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload.DataSet.Row"/>.</returns>
        private static VersionBProtoBuf.ProtoBufPayload.DataSet.Row ConvertVersionBRow(VersionBData.Row row)
        {
            return new VersionBProtoBuf.ProtoBufPayload.DataSet.Row
            {
                Details = row.Details,
                Elements = row.Elements.Select(ConvertVersionBDataSetValue).ToList()
            };
        }

        /// <summary>
        /// Gets the version B meta data from the version B ProtoBuf meta data.
        /// </summary>
        /// <param name="metaData">The <see cref="VersionBProtoBuf.ProtoBufPayload.MetaData"/>.</param>
        /// <returns>The <see cref="VersionBData.MetaData"/>.</returns>
        private static VersionBData.MetaData ConvertVersionBMetaData(VersionBProtoBuf.ProtoBufPayload.MetaData metaData)
        {
            return new VersionBData.MetaData
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
        }

        /// <summary>
        /// Gets the version B ProtoBuf meta data from the version B meta data.
        /// </summary>
        /// <param name="metaData">The <see cref="VersionBData.MetaData"/>.</param>
        /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload.MetaData"/>.</returns>
        private static VersionBProtoBuf.ProtoBufPayload.MetaData ConvertVersionBMetaData(VersionBData.MetaData metaData)
        {
            return new VersionBProtoBuf.ProtoBufPayload.MetaData
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
        }

        /// <summary>
        /// Gets the version B template from the version B ProtoBuf template.
        /// </summary>
        /// <param name="template">The <see cref="VersionBProtoBuf.ProtoBufPayload.Template"/>.</param>
        /// <returns>The <see cref="VersionBData.Template"/>.</returns>
        private static VersionBData.Template ConvertVersionBTemplate(VersionBProtoBuf.ProtoBufPayload.Template template)
        {
            return new VersionBData.Template
            {
                Metrics = template.Metrics.Select(ConvertVersionBMetric).ToList(),
                Details = template.Details,
                IsDefinition = template.IsDefinition,
                Parameters = template.Parameters.Select(ConvertVersionBParameter).ToList(),
                TemplateRef = template.TemplateRef,
                Version = template.Version
            };
        }

        /// <summary>
        /// Gets the version B ProtoBuf template from the version B template.
        /// </summary>
        /// <param name="template">The <see cref="VersionBData.Template"/>.</param>
        /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload.Template"/>.</returns>
        private static VersionBProtoBuf.ProtoBufPayload.Template ConvertVersionBTemplate(VersionBData.Template template)
        {
            return new VersionBProtoBuf.ProtoBufPayload.Template
            {
                Metrics = template.Metrics.Select(ConvertVersionBMetric).ToList(),
                Details = template.Details,
                IsDefinition = template.IsDefinition,
                Parameters = template.Parameters.Select(ConvertVersionBParameter).ToList(),
                TemplateRef = template.TemplateRef,
                Version = template.Version
            };
        }

        /// <summary>
        /// Gets the version B parameter from the version B ProtoBuf parameter.
        /// </summary>
        /// <param name="parameter">The <see cref="VersionBProtoBuf.ProtoBufPayload.Template.Parameter"/>.</param>
        /// <returns>The <see cref="VersionBData.Parameter"/>.</returns>
        private static VersionBData.Parameter ConvertVersionBParameter(VersionBProtoBuf.ProtoBufPayload.Template.Parameter parameter)
        {
            return new VersionBData.Parameter
            {
                DoubleValue = parameter.DoubleValue,
                BooleanValue = parameter.BooleanValue,
                ExtensionValue = new VersionBData.ParameterValueExtension
                {
                    Extensions = parameter.ExtensionValue.Extensions
                },
                FloatValue = parameter.FloatValue,
                IntValue = parameter.IntValue,
                LongValue = parameter.LongValue,
                Name = parameter.Name,
                StringValue = parameter.StringValue,
                Type = parameter.Type
            };
        }

        /// <summary>
        /// Gets the version B ProtoBuf parameter from the version B parameter.
        /// </summary>
        /// <param name="parameter">The <see cref="VersionBData.Parameter"/>.</param>
        /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload.Template.Parameter"/>.</returns>
        private static VersionBProtoBuf.ProtoBufPayload.Template.Parameter ConvertVersionBParameter(VersionBData.Parameter parameter)
        {
            return new VersionBProtoBuf.ProtoBufPayload.Template.Parameter
            {
                DoubleValue = parameter.DoubleValue,
                BooleanValue = parameter.BooleanValue,
                ExtensionValue = new VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ParameterValueExtension
                {
                    Extensions = parameter.ExtensionValue.Extensions
                },
                FloatValue = parameter.FloatValue,
                IntValue = parameter.IntValue,
                LongValue = parameter.LongValue,
                Name = parameter.Name,
                StringValue = parameter.StringValue,
                Type = parameter.Type
            };
        }

        /// <summary>
        /// Gets the version B metric from the version B ProtoBuf metric.
        /// </summary>
        /// <param name="metric">The <see cref="VersionBProtoBuf.ProtoBufPayload.Metric"/>.</param>
        /// <returns>The <see cref="VersionBData.Metric"/>.</returns>
        private static VersionBData.Metric ConvertVersionBMetric(VersionBProtoBuf.ProtoBufPayload.Metric metric)
        {
            return new VersionBData.Metric
            {
                DoubleValue = metric.DoubleValue,
                Alias = metric.Alias,
                BooleanValue = metric.BooleanValue,
                BytesValue = metric.BytesValue,
                DataSetValue = ConvertVersionBDataSet(metric.DatasetValue),
                DataType = metric.Datatype,
                ExtensionValue = new VersionBData.MetricValueExtension
                {
                    Details = metric.ExtensionValue.Details
                },
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
                TemplateValue = ConvertVersionBTemplate(metric.TemplateValue)
            };
        }

        /// <summary>
        /// Gets the version B ProtoBuf metric from the version B metric.
        /// </summary>
        /// <param name="metric">The <see cref="VersionBData.Metric"/>.</param>
        /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload.Metric"/>.</returns>
        private static VersionBProtoBuf.ProtoBufPayload.Metric ConvertVersionBMetric(VersionBData.Metric metric)
        {
            return new VersionBProtoBuf.ProtoBufPayload.Metric
            {
                DoubleValue = metric.DoubleValue,
                Alias = metric.Alias,
                BooleanValue = metric.BooleanValue,
                BytesValue = metric.BytesValue,
                DatasetValue = ConvertVersionBDataSet(metric.DataSetValue),
                Datatype = metric.DataType,
                ExtensionValue = new VersionBProtoBuf.ProtoBufPayload.Metric.MetricValueExtension
                {
                    Details = metric.ExtensionValue.Details
                },
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
                TemplateValue = ConvertVersionBTemplate(metric.TemplateValue)
            };
        }

        /// <summary>
        /// Gets the version B property set list from the version B ProtoBuf property set list.
        /// </summary>
        /// <param name="propertySetList">The <see cref="VersionBProtoBuf.ProtoBufPayload.PropertySetList"/>.</param>
        /// <returns>The <see cref="VersionBData.PropertySetList"/>.</returns>
        private static VersionBData.PropertySetList ConvertVersionBPropertySetList(VersionBProtoBuf.ProtoBufPayload.PropertySetList propertySetList)
        {
            return new VersionBData.PropertySetList
            {
                Details = propertySetList.Details,
                PropertySets = propertySetList.Propertysets.Select(ConvertVersionBPropertySet).ToList()
            };
        }

        /// <summary>
        /// Gets the version B ProtoBuf property set list from the version B property set list.
        /// </summary>
        /// <param name="propertySetList">The <see cref="VersionBData.PropertySetList"/>.</param>
        /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload.PropertySetList"/>.</returns>
        private static VersionBProtoBuf.ProtoBufPayload.PropertySetList ConvertVersionBPropertySetList(VersionBData.PropertySetList propertySetList)
        {
            return new VersionBProtoBuf.ProtoBufPayload.PropertySetList
            {
                Details = propertySetList.Details,
                Propertysets = propertySetList.PropertySets.Select(ConvertVersionBPropertySet).ToList()
            };
        }

        /// <summary>
        /// Gets the version B property set from the version B ProtoBuf property set.
        /// </summary>
        /// <param name="propertySet">The <see cref="VersionBProtoBuf.ProtoBufPayload.PropertySet"/>.</param>
        /// <returns>The <see cref="VersionBData.PropertySet"/>.</returns>
        private static VersionBData.PropertySet ConvertVersionBPropertySet(VersionBProtoBuf.ProtoBufPayload.PropertySet propertySet)
        {
            return new VersionBData.PropertySet
            {
                Details = propertySet.Details,
                Keys = propertySet.Keys,
                Values = propertySet.Values.Select(ConvertVersionBPropertyValue).ToList()
            };
        }

        /// <summary>
        /// Gets the version B ProtoBuf property set from the version B property set.
        /// </summary>
        /// <param name="propertySet">The <see cref="VersionBData.PropertySet"/>.</param>
        /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload.PropertySet"/>.</returns>
        private static VersionBProtoBuf.ProtoBufPayload.PropertySet ConvertVersionBPropertySet(VersionBData.PropertySet propertySet)
        {
            return new VersionBProtoBuf.ProtoBufPayload.PropertySet
            {
                Details = propertySet.Details,
                Keys = propertySet.Keys,
                Values = propertySet.Values.Select(ConvertVersionBPropertyValue).ToList()
            };
        }

        /// <summary>
        /// Gets the version B property value from the version B ProtoBuf property value.
        /// </summary>
        /// <param name="propertyValue">The <see cref="VersionBProtoBuf.ProtoBufPayload.PropertyValue"/>.</param>
        /// <returns>The <see cref="VersionBData.PropertyValue"/>.</returns>
        private static VersionBData.PropertyValue ConvertVersionBPropertyValue(VersionBProtoBuf.ProtoBufPayload.PropertyValue propertyValue)
        {
            return new VersionBData.PropertyValue
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
                IsNull = propertyValue.IsNull,
                LongValue = propertyValue.LongValue,
                PropertySetValue = ConvertVersionBPropertySet(propertyValue.PropertysetValue),
                StringValue = propertyValue.StringValue,
                Type = propertyValue.Type
            };
        }

        /// <summary>
        /// Gets the version B ProtoBuf property value from the version B property value.
        /// </summary>
        /// <param name="propertyValue">The <see cref="VersionBData.PropertyValue"/>.</param>
        /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload.PropertyValue"/>.</returns>
        private static VersionBProtoBuf.ProtoBufPayload.PropertyValue ConvertVersionBPropertyValue(VersionBData.PropertyValue propertyValue)
        {
            return new VersionBProtoBuf.ProtoBufPayload.PropertyValue
            {
                DoubleValue = propertyValue.DoubleValue,
                PropertysetsValue = ConvertVersionBPropertySetList(propertyValue.PropertySetsValue),
                BooleanValue = propertyValue.BooleanValue,
                ExtensionValue = new VersionBProtoBuf.ProtoBufPayload.PropertyValue.PropertyValueExtension
                {
                    Details = propertyValue.ExtensionValue.Details
                },
                FloatValue = propertyValue.FloatValue,
                IntValue = propertyValue.IntValue,
                IsNull = propertyValue.IsNull,
                LongValue = propertyValue.LongValue,
                PropertysetValue = ConvertVersionBPropertySet(propertyValue.PropertySetValue),
                StringValue = propertyValue.StringValue,
                Type = propertyValue.Type
            };
        }
    }
}
