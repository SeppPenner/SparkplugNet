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
    /// Gets the ProtoBuf payload converted from the version A payload.
    /// </summary>
    /// <param name="payload">The <see cref="VersionAData.Payload"/>.</param>
    /// <returns>The <see cref="VersionAProtoBuf.ProtoBufPayload"/>.</returns>
    public static VersionAProtoBuf.ProtoBufPayload ConvertVersionAPayload(VersionAData.Payload payload)
    {
        return new VersionAProtoBuf.ProtoBufPayload
        {
            Body = payload.Body,
            Metrics = payload.Metrics.Select(m => new VersionAProtoBuf.ProtoBufPayload.KuraMetric
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
    }

    /// <summary>
    /// Gets the version B payload converted from the ProtoBuf payload.
    /// </summary>
    /// <param name="payload">The <see cref="VersionBProtoBuf.ProtoBufPayload"/>.</param>
    /// <returns>The <see cref="VersionBData.Payload"/>.</returns>
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
    /// <param name="type">The <see cref="VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType"/>.</param>
    /// <returns>The <see cref="VersionAData.DataType"/>.</returns>
    public static VersionAData.DataType ConvertVersionADataType(VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType type)
    {
        return type switch
        {
            VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType.Bool => VersionAData.DataType.Bool,
            VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType.Bytes => VersionAData.DataType.Bytes,
            VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType.Double => VersionAData.DataType.Double,
            VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType.Float => VersionAData.DataType.Float,
            VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType.Int32 => VersionAData.DataType.Int32,
            VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType.Int64 => VersionAData.DataType.Int64,
            VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType.String => VersionAData.DataType.String,
            _ => VersionAData.DataType.String
        };
    }

    /// <summary>
    /// Gets the version A ProtoBuf value type from the version A data type.
    /// </summary>
    /// <param name="type">The <see cref="VersionAData.DataType"/>.</param>
    /// <returns>The <see cref="VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType"/>.</returns>
    public static VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType ConvertVersionADataType(VersionAData.DataType type)
    {
        return type switch
        {
            VersionAData.DataType.Bool => VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType.Bool,
            VersionAData.DataType.Bytes => VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType.Bytes,
            VersionAData.DataType.Double => VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType.Double,
            VersionAData.DataType.Float => VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType.Float,
            VersionAData.DataType.Int32 => VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType.Int32,
            VersionAData.DataType.Int64 => VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType.Int64,
            VersionAData.DataType.String => VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType.String,
            _ => VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType.String
        };
    }

    /// <summary>
    /// Gets the version B data type from the version B ProtoBuf value type for data set values.
    /// </summary>
    /// <param name="type">The <see cref="VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase"/>.</param>
    /// <returns>The <see cref="VersionBDataTypeEnum"/>.</returns>
    public static VersionBDataTypeEnum ConvertVersionBDataTypeDataSetValue(VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase type)
    {
        return type switch
        {
            VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.None => VersionBDataTypeEnum.Unknown,
            VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.IntValue => VersionBDataTypeEnum.Int32,
            VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.LongValue => VersionBDataTypeEnum.Int64,
            VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.FloatValue => VersionBDataTypeEnum.Float,
            VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.DoubleValue => VersionBDataTypeEnum.Double,
            VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.BooleanValue => VersionBDataTypeEnum.Boolean,
            VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.StringValue => VersionBDataTypeEnum.String,
            VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.ExtensionValue => VersionBDataTypeEnum.Unknown,
            _ => VersionBDataTypeEnum.String
        };
    }

    /// <summary>
    /// Gets the version B ProtoBuf value type from the version B data type for data set values.
    /// </summary>
    /// <param name="type">The <see cref="VersionBDataTypeEnum"/>.</param>
    /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase"/>.</returns>
    public static VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase ConvertVersionBDataTypeDataSetValue(VersionBDataTypeEnum type)
    {
        return type switch
        {
            VersionBDataTypeEnum.Unknown => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.ExtensionValue,
            VersionBDataTypeEnum.Int8 => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.IntValue,
            VersionBDataTypeEnum.Int16 => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.IntValue,
            VersionBDataTypeEnum.Int32 => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.IntValue,
            VersionBDataTypeEnum.Int64 => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.LongValue,
            VersionBDataTypeEnum.UInt8 => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.IntValue,
            VersionBDataTypeEnum.UInt16 => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.IntValue,
            VersionBDataTypeEnum.UInt32 => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.IntValue,
            VersionBDataTypeEnum.UInt64 => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.LongValue,
            VersionBDataTypeEnum.Float => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.FloatValue,
            VersionBDataTypeEnum.Double => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.DoubleValue,
            VersionBDataTypeEnum.Boolean => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.BooleanValue,
            VersionBDataTypeEnum.String => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.StringValue,
            VersionBDataTypeEnum.DateTime => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.LongValue,
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
    }

    /// <summary>
    /// Gets the version B data type from the version B ProtoBuf value type for metrics.
    /// </summary>
    /// <param name="type">The <see cref="VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase"/>.</param>
    /// <returns>The <see cref="VersionBDataTypeEnum"/>.</returns>
    public static VersionBDataTypeEnum ConvertVersionBDataTypeMetric(VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase type)
    {
        return type switch
        {
            VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.None => VersionBDataTypeEnum.Unknown,
            VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.IntValue => VersionBDataTypeEnum.Int32,
            VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.LongValue => VersionBDataTypeEnum.Int64,
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
    }

    /// <summary>
    /// Gets the version B ProtoBuf value type from the version B data type for metrics.
    /// </summary>
    /// <param name="type">The <see cref="VersionBDataTypeEnum"/>.</param>
    /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase"/>.</returns>
    public static VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase ConvertVersionBDataTypeMetric(VersionBDataTypeEnum type)
    {
        return type switch
        {
            VersionBDataTypeEnum.Unknown => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.ExtensionValue,
            VersionBDataTypeEnum.Int8 => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.IntValue,
            VersionBDataTypeEnum.Int16 => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.IntValue,
            VersionBDataTypeEnum.Int32 => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.IntValue,
            VersionBDataTypeEnum.Int64 => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.LongValue,
            VersionBDataTypeEnum.UInt8 => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.IntValue,
            VersionBDataTypeEnum.UInt16 => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.IntValue,
            VersionBDataTypeEnum.UInt32 => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.IntValue,
            VersionBDataTypeEnum.UInt64 => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.LongValue,
            VersionBDataTypeEnum.Float => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.FloatValue,
            VersionBDataTypeEnum.Double => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.DoubleValue,
            VersionBDataTypeEnum.Boolean => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.BooleanValue,
            VersionBDataTypeEnum.String => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.StringValue,
            VersionBDataTypeEnum.DateTime => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.LongValue,
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
    }

    /// <summary>
    /// Gets the version B data type from the version B ProtoBuf value type for parameters.
    /// </summary>
    /// <param name="type">The <see cref="VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase"/>.</param>
    /// <returns>The <see cref="VersionBDataTypeEnum"/>.</returns>
    public static VersionBDataTypeEnum ConvertVersionBDataTypeParameter(VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase type)
    {
        return type switch
        {
            VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.None => VersionBDataTypeEnum.Unknown,
            VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.IntValue => VersionBDataTypeEnum.Int32,
            VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.LongValue => VersionBDataTypeEnum.Int64,
            VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.FloatValue => VersionBDataTypeEnum.Float,
            VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.DoubleValue => VersionBDataTypeEnum.Double,
            VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.BooleanValue => VersionBDataTypeEnum.Boolean,
            VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.StringValue => VersionBDataTypeEnum.String,
            VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.ExtensionValue => VersionBDataTypeEnum.Unknown,
            _ => VersionBDataTypeEnum.String
        };
    }

    /// <summary>
    /// Gets the version B ProtoBuf value type from the version B data type for parameters.
    /// </summary>
    /// <param name="type">The <see cref="VersionBDataTypeEnum"/>.</param>
    /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase"/>.</returns>
    public static VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase ConvertVersionBDataTypeParameter(VersionBDataTypeEnum type)
    {
        return type switch
        {
            VersionBDataTypeEnum.Unknown => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.ExtensionValue,
            VersionBDataTypeEnum.Int8 => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.IntValue,
            VersionBDataTypeEnum.Int16 => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.IntValue,
            VersionBDataTypeEnum.Int32 => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.IntValue,
            VersionBDataTypeEnum.Int64 => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.LongValue,
            VersionBDataTypeEnum.UInt8 => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.IntValue,
            VersionBDataTypeEnum.UInt16 => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.IntValue,
            VersionBDataTypeEnum.UInt32 => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.IntValue,
            VersionBDataTypeEnum.UInt64 => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.LongValue,
            VersionBDataTypeEnum.Float => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.FloatValue,
            VersionBDataTypeEnum.Double => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.DoubleValue,
            VersionBDataTypeEnum.Boolean => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.BooleanValue,
            VersionBDataTypeEnum.String => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.StringValue,
            VersionBDataTypeEnum.DateTime => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.LongValue,
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
    }

    /// <summary>
    /// Gets the version B data type from the version B ProtoBuf value type for property values.
    /// </summary>
    /// <param name="type">The <see cref="VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase"/>.</param>
    /// <returns>The <see cref="VersionBDataTypeEnum"/>.</returns>
    public static VersionBDataTypeEnum ConvertVersionBDataTypePropertyValue(VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase type)
    {
        return type switch
        {
            VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.None => VersionBDataTypeEnum.Unknown,
            VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.IntValue => VersionBDataTypeEnum.Int32,
            VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.LongValue => VersionBDataTypeEnum.Int64,
            VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.FloatValue => VersionBDataTypeEnum.Float,
            VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.DoubleValue => VersionBDataTypeEnum.Double,
            VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.BooleanValue => VersionBDataTypeEnum.Boolean,
            VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.StringValue => VersionBDataTypeEnum.String,
            VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.ExtensionValue => VersionBDataTypeEnum.Unknown,
            _ => VersionBDataTypeEnum.String
        };
    }

    /// <summary>
    /// Gets the version B ProtoBuf value type from the version B data type for property values.
    /// </summary>
    /// <param name="type">The <see cref="VersionBDataTypeEnum"/>.</param>
    /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase"/>.</returns>
    public static VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase ConvertVersionBDataTypePropertyValue(VersionBDataTypeEnum type)
    {
        return type switch
        {
            VersionBDataTypeEnum.Unknown => VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.ExtensionValue,
            VersionBDataTypeEnum.Int8 => VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.IntValue,
            VersionBDataTypeEnum.Int16 => VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.IntValue,
            VersionBDataTypeEnum.Int32 => VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.IntValue,
            VersionBDataTypeEnum.Int64 => VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.LongValue,
            VersionBDataTypeEnum.UInt8 => VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.IntValue,
            VersionBDataTypeEnum.UInt16 => VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.IntValue,
            VersionBDataTypeEnum.UInt32 => VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.IntValue,
            VersionBDataTypeEnum.UInt64 => VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.LongValue,
            VersionBDataTypeEnum.Float => VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.FloatValue,
            VersionBDataTypeEnum.Double => VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.DoubleValue,
            VersionBDataTypeEnum.Boolean => VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.BooleanValue,
            VersionBDataTypeEnum.String => VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.StringValue,
            VersionBDataTypeEnum.DateTime => VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.LongValue,
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
            StringValue = dataSetValue.StringValue,
            ValueCase = ConvertVersionBDataTypeDataSetValue(dataSetValue.ValueCase)
        };
    }

    /// <summary>
    /// Gets the version B ProtoBuf data set value from the version B data set value.
    /// </summary>
    /// <param name="dataSetValue">The <see cref="VersionBData.DataSetValue"/>.</param>
    /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue"/>.</returns>
    private static VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue ConvertVersionBDataSetValue(VersionBData.DataSetValue dataSetValue)
    {
        return dataSetValue.ValueCase switch
        {
            VersionBDataTypeEnum.Unknown => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
            {
                ExtensionValue = new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.DataSetValueExtension
                {
                    Details = dataSetValue.ExtensionValue.Details
                }
            },
            VersionBDataTypeEnum.Int8 => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
            {
                IntValue = dataSetValue.IntValue
            },
            VersionBDataTypeEnum.Int16 => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
            {
                IntValue = dataSetValue.IntValue
            },
            VersionBDataTypeEnum.Int32 => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
            {
                IntValue = dataSetValue.IntValue
            },
            VersionBDataTypeEnum.Int64 => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
            {
                LongValue = dataSetValue.LongValue
            },
            VersionBDataTypeEnum.UInt8 => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
            {
                IntValue = dataSetValue.IntValue
            },
            VersionBDataTypeEnum.UInt16 => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
            {
                IntValue = dataSetValue.IntValue
            },
            VersionBDataTypeEnum.UInt32 => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
            {
                IntValue = dataSetValue.IntValue
            },
            VersionBDataTypeEnum.UInt64 => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
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
            VersionBDataTypeEnum.String => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
            {
                StringValue = dataSetValue.StringValue
            },
            VersionBDataTypeEnum.DateTime => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
            {
                LongValue = dataSetValue.LongValue
            },
            VersionBDataTypeEnum.Text => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
            {
                StringValue = dataSetValue.StringValue
            },
            VersionBDataTypeEnum.Uuid => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
            {
                StringValue = dataSetValue.StringValue
            },
            VersionBDataTypeEnum.DataSet => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
            {
                ExtensionValue = new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.DataSetValueExtension
                {
                    Details = dataSetValue.ExtensionValue.Details
                }
            },
            VersionBDataTypeEnum.Bytes => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
            {
                ExtensionValue = new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.DataSetValueExtension
                {
                    Details = dataSetValue.ExtensionValue.Details
                }
            },
            VersionBDataTypeEnum.File => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
            {
                ExtensionValue = new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.DataSetValueExtension
                {
                    Details = dataSetValue.ExtensionValue.Details
                }
            },
            VersionBDataTypeEnum.Template => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
            {
                ExtensionValue = new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.DataSetValueExtension
                {
                    Details = dataSetValue.ExtensionValue.Details
                }
            },
            VersionBDataTypeEnum.PropertySet => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
            {
                ExtensionValue = new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.DataSetValueExtension
                {
                    Details = dataSetValue.ExtensionValue.Details
                }
            },
            VersionBDataTypeEnum.PropertySetList => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
            {
                ExtensionValue = new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.DataSetValueExtension
                {
                    Details = dataSetValue.ExtensionValue.Details
                }
            },
            _ => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
            {
                ExtensionValue = new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.DataSetValueExtension
                {
                    Details = dataSetValue.ExtensionValue.Details
                }
            }
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
    private static VersionBData.MetaData? ConvertVersionBMetaData(VersionBProtoBuf.ProtoBufPayload.MetaData? metaData)
    {
        if (metaData is null)
        {
            return null;
        }

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
    private static VersionBProtoBuf.ProtoBufPayload.MetaData? ConvertVersionBMetaData(VersionBData.MetaData? metaData)
    {
        if (metaData is not null)
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
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Gets the version B template from the version B ProtoBuf template.
    /// </summary>
    /// <param name="template">The <see cref="VersionBProtoBuf.ProtoBufPayload.Template"/>.</param>
    /// <returns>The <see cref="VersionBData.Template"/>.</returns>
    private static VersionBData.Template? ConvertVersionBTemplate(VersionBProtoBuf.ProtoBufPayload.Template? template)
    {
        if (template is null)
        {
            return null;
        }

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
    private static VersionBProtoBuf.ProtoBufPayload.Template? ConvertVersionBTemplate(VersionBData.Template? template)
    {
        if (template is null)
        {
            return null;
        }

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
            Type = parameter.Type,
            ValueCase = ConvertVersionBDataTypeParameter(parameter.ValueCase)
        };
    }

    /// <summary>
    /// Gets the version B ProtoBuf parameter from the version B parameter.
    /// </summary>
    /// <param name="parameter">The <see cref="VersionBData.Parameter"/>.</param>
    /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload.Template.Parameter"/>.</returns>
    private static VersionBProtoBuf.ProtoBufPayload.Template.Parameter ConvertVersionBParameter(VersionBData.Parameter parameter)
    {
        return parameter.ValueCase switch
        {
            VersionBDataTypeEnum.Unknown => new VersionBProtoBuf.ProtoBufPayload.Template.Parameter
            {
                ExtensionValue = new VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ParameterValueExtension
                {
                    Extensions = parameter.ExtensionValue.Extensions
                },
                Name = parameter.Name,
                Type = parameter.Type
            },
            VersionBDataTypeEnum.Int8 => new VersionBProtoBuf.ProtoBufPayload.Template.Parameter
            {
                IntValue = parameter.IntValue,
                Name = parameter.Name,
                Type = parameter.Type
            },
            VersionBDataTypeEnum.Int16 => new VersionBProtoBuf.ProtoBufPayload.Template.Parameter
            {
                IntValue = parameter.IntValue,
                Name = parameter.Name,
                Type = parameter.Type
            },
            VersionBDataTypeEnum.Int32 => new VersionBProtoBuf.ProtoBufPayload.Template.Parameter
            {
                IntValue = parameter.IntValue,
                Name = parameter.Name,
                Type = parameter.Type
            },
            VersionBDataTypeEnum.Int64 => new VersionBProtoBuf.ProtoBufPayload.Template.Parameter
            {
                LongValue = parameter.LongValue,
                Name = parameter.Name,
                Type = parameter.Type
            },
            VersionBDataTypeEnum.UInt8 => new VersionBProtoBuf.ProtoBufPayload.Template.Parameter
            {
                IntValue = parameter.IntValue,
                Name = parameter.Name,
                Type = parameter.Type
            },
            VersionBDataTypeEnum.UInt16 => new VersionBProtoBuf.ProtoBufPayload.Template.Parameter
            {
                IntValue = parameter.IntValue,
                Name = parameter.Name,
                Type = parameter.Type
            },
            VersionBDataTypeEnum.UInt32 => new VersionBProtoBuf.ProtoBufPayload.Template.Parameter
            {
                IntValue = parameter.IntValue,
                Name = parameter.Name,
                Type = parameter.Type
            },
            VersionBDataTypeEnum.UInt64 => new VersionBProtoBuf.ProtoBufPayload.Template.Parameter
            {
                LongValue = parameter.LongValue,
                Name = parameter.Name,
                Type = parameter.Type
            },
            VersionBDataTypeEnum.Float => new VersionBProtoBuf.ProtoBufPayload.Template.Parameter
            {
                FloatValue = parameter.FloatValue,
                Name = parameter.Name,
                Type = parameter.Type
            },
            VersionBDataTypeEnum.Double => new VersionBProtoBuf.ProtoBufPayload.Template.Parameter
            {
                DoubleValue = parameter.DoubleValue,
                Name = parameter.Name,
                Type = parameter.Type
            },
            VersionBDataTypeEnum.Boolean => new VersionBProtoBuf.ProtoBufPayload.Template.Parameter
            {
                BooleanValue = parameter.BooleanValue,
                Name = parameter.Name,
                Type = parameter.Type
            },
            VersionBDataTypeEnum.String => new VersionBProtoBuf.ProtoBufPayload.Template.Parameter
            {
                StringValue = parameter.StringValue,
                Name = parameter.Name,
                Type = parameter.Type
            },
            VersionBDataTypeEnum.DateTime => new VersionBProtoBuf.ProtoBufPayload.Template.Parameter
            {
                LongValue = parameter.LongValue,
                Name = parameter.Name,
                Type = parameter.Type
            },
            VersionBDataTypeEnum.Text => new VersionBProtoBuf.ProtoBufPayload.Template.Parameter
            {
                StringValue = parameter.StringValue,
                Name = parameter.Name,
                Type = parameter.Type
            },
            VersionBDataTypeEnum.Uuid => new VersionBProtoBuf.ProtoBufPayload.Template.Parameter
            {
                StringValue = parameter.StringValue,
                Name = parameter.Name,
                Type = parameter.Type
            },
            VersionBDataTypeEnum.DataSet => new VersionBProtoBuf.ProtoBufPayload.Template.Parameter
            {
                ExtensionValue = new VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ParameterValueExtension
                {
                    Extensions = parameter.ExtensionValue.Extensions
                },
                Name = parameter.Name,
                StringValue = parameter.StringValue,
                Type = parameter.Type
            },
            VersionBDataTypeEnum.Bytes => new VersionBProtoBuf.ProtoBufPayload.Template.Parameter
            {
                ExtensionValue = new VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ParameterValueExtension
                {
                    Extensions = parameter.ExtensionValue.Extensions
                },
                Name = parameter.Name,
                StringValue = parameter.StringValue,
                Type = parameter.Type
            },
            VersionBDataTypeEnum.File => new VersionBProtoBuf.ProtoBufPayload.Template.Parameter
            {
                ExtensionValue = new VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ParameterValueExtension
                {
                    Extensions = parameter.ExtensionValue.Extensions
                },
                Name = parameter.Name,
                StringValue = parameter.StringValue,
                Type = parameter.Type
            },
            VersionBDataTypeEnum.Template => new VersionBProtoBuf.ProtoBufPayload.Template.Parameter
            {
                ExtensionValue = new VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ParameterValueExtension
                {
                    Extensions = parameter.ExtensionValue.Extensions
                },
                Name = parameter.Name,
                StringValue = parameter.StringValue,
                Type = parameter.Type
            },
            VersionBDataTypeEnum.PropertySet => new VersionBProtoBuf.ProtoBufPayload.Template.Parameter
            {
                ExtensionValue = new VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ParameterValueExtension
                {
                    Extensions = parameter.ExtensionValue.Extensions
                },
                Name = parameter.Name,
                StringValue = parameter.StringValue,
                Type = parameter.Type
            },
            VersionBDataTypeEnum.PropertySetList => new VersionBProtoBuf.ProtoBufPayload.Template.Parameter
            {
                ExtensionValue = new VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ParameterValueExtension
                {
                    Extensions = parameter.ExtensionValue.Extensions
                },
                Name = parameter.Name,
                StringValue = parameter.StringValue,
                Type = parameter.Type
            },
            _ => new VersionBProtoBuf.ProtoBufPayload.Template.Parameter
            {
                ExtensionValue = new VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ParameterValueExtension
                {
                    Extensions = parameter.ExtensionValue.Extensions
                },
                Name = parameter.Name,
                StringValue = parameter.StringValue,
                Type = parameter.Type
            }
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
            ExtensionValue = (metric.ExtensionValue is not null) ? new VersionBData.MetricValueExtension
            {
                Details = metric.ExtensionValue.Details
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
            Type = ConvertVersionBDataTypeMetric(metric.ValueCase)
        };
    }

    /// <summary>
    /// Gets the version B ProtoBuf metric from the version B metric.
    /// </summary>
    /// <param name="metric">The <see cref="VersionBData.Metric"/>.</param>
    /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload.Metric"/>.</returns>
    private static VersionBProtoBuf.ProtoBufPayload.Metric ConvertVersionBMetric(VersionBData.Metric metric)
    {
        return metric.Type switch
        {
            VersionBDataTypeEnum.Unknown => new VersionBProtoBuf.ProtoBufPayload.Metric
            {
                Alias = metric.Alias,
                Datatype = metric.DataType,
                ExtensionValue = (metric.ExtensionValue is not null) ? new VersionBProtoBuf.ProtoBufPayload.Metric.MetricValueExtension
                {
                    Details = metric.ExtensionValue.Details
                } : null,
                IsHistorical = metric.IsHistorical,
                IsNull = metric.IsNull,
                IsTransient = metric.IsTransient,
                Metadata = ConvertVersionBMetaData(metric.Metadata),
                Name = metric.Name,
                Properties = ConvertVersionBPropertySet(metric.Properties),
                Timestamp = metric.Timestamp
            },
            VersionBDataTypeEnum.Int8 => new VersionBProtoBuf.ProtoBufPayload.Metric
            {
                Alias = metric.Alias,
                Datatype = metric.DataType,
                IntValue = metric.IntValue,
                IsHistorical = metric.IsHistorical,
                IsNull = metric.IsNull,
                IsTransient = metric.IsTransient,
                Metadata = ConvertVersionBMetaData(metric.Metadata),
                Name = metric.Name,
                Properties = ConvertVersionBPropertySet(metric.Properties),
                Timestamp = metric.Timestamp
            },
            VersionBDataTypeEnum.Int16 => new VersionBProtoBuf.ProtoBufPayload.Metric
            {
                Alias = metric.Alias,
                Datatype = metric.DataType,
                IntValue = metric.IntValue,
                IsHistorical = metric.IsHistorical,
                IsNull = metric.IsNull,
                IsTransient = metric.IsTransient,
                Metadata = ConvertVersionBMetaData(metric.Metadata),
                Name = metric.Name,
                Properties = ConvertVersionBPropertySet(metric.Properties),
                Timestamp = metric.Timestamp
            },
            VersionBDataTypeEnum.Int32 => new VersionBProtoBuf.ProtoBufPayload.Metric
            {
                Alias = metric.Alias,
                Datatype = metric.DataType,
                IntValue = metric.IntValue,
                IsHistorical = metric.IsHistorical,
                IsNull = metric.IsNull,
                IsTransient = metric.IsTransient,
                Metadata = ConvertVersionBMetaData(metric.Metadata),
                Name = metric.Name,
                Properties = ConvertVersionBPropertySet(metric.Properties),
                Timestamp = metric.Timestamp
            },
            VersionBDataTypeEnum.Int64 => new VersionBProtoBuf.ProtoBufPayload.Metric
            {
                Alias = metric.Alias,
                Datatype = metric.DataType,
                IsHistorical = metric.IsHistorical,
                IsNull = metric.IsNull,
                IsTransient = metric.IsTransient,
                LongValue = metric.LongValue,
                Metadata = ConvertVersionBMetaData(metric.Metadata),
                Name = metric.Name,
                Properties = ConvertVersionBPropertySet(metric.Properties),
                Timestamp = metric.Timestamp
            },
            VersionBDataTypeEnum.UInt8 => new VersionBProtoBuf.ProtoBufPayload.Metric
            {
                Alias = metric.Alias,
                Datatype = metric.DataType,
                IntValue = metric.IntValue,
                IsHistorical = metric.IsHistorical,
                IsNull = metric.IsNull,
                IsTransient = metric.IsTransient,
                Metadata = ConvertVersionBMetaData(metric.Metadata),
                Name = metric.Name,
                Properties = ConvertVersionBPropertySet(metric.Properties),
                Timestamp = metric.Timestamp
            },
            VersionBDataTypeEnum.UInt16 => new VersionBProtoBuf.ProtoBufPayload.Metric
            {
                Alias = metric.Alias,
                Datatype = metric.DataType,
                IntValue = metric.IntValue,
                IsHistorical = metric.IsHistorical,
                IsNull = metric.IsNull,
                IsTransient = metric.IsTransient,
                Metadata = ConvertVersionBMetaData(metric.Metadata),
                Name = metric.Name,
                Properties = ConvertVersionBPropertySet(metric.Properties),
                Timestamp = metric.Timestamp
            },
            VersionBDataTypeEnum.UInt32 => new VersionBProtoBuf.ProtoBufPayload.Metric
            {
                Alias = metric.Alias,
                Datatype = metric.DataType,
                IntValue = metric.IntValue,
                IsHistorical = metric.IsHistorical,
                IsNull = metric.IsNull,
                IsTransient = metric.IsTransient,
                Metadata = ConvertVersionBMetaData(metric.Metadata),
                Name = metric.Name,
                Properties = ConvertVersionBPropertySet(metric.Properties),
                Timestamp = metric.Timestamp
            },
            VersionBDataTypeEnum.UInt64 => new VersionBProtoBuf.ProtoBufPayload.Metric
            {
                Alias = metric.Alias,
                Datatype = metric.DataType,
                IsHistorical = metric.IsHistorical,
                IsNull = metric.IsNull,
                IsTransient = metric.IsTransient,
                LongValue = metric.LongValue,
                Metadata = ConvertVersionBMetaData(metric.Metadata),
                Name = metric.Name,
                Properties = ConvertVersionBPropertySet(metric.Properties),
                Timestamp = metric.Timestamp
            },
            VersionBDataTypeEnum.Float => new VersionBProtoBuf.ProtoBufPayload.Metric
            {
                Alias = metric.Alias,
                Datatype = metric.DataType,
                FloatValue = metric.FloatValue,
                IsHistorical = metric.IsHistorical,
                IsNull = metric.IsNull,
                IsTransient = metric.IsTransient,
                Metadata = ConvertVersionBMetaData(metric.Metadata),
                Name = metric.Name,
                Properties = ConvertVersionBPropertySet(metric.Properties),
                Timestamp = metric.Timestamp
            },
            VersionBDataTypeEnum.Double => new VersionBProtoBuf.ProtoBufPayload.Metric
            {
                DoubleValue = metric.DoubleValue,
                Alias = metric.Alias,
                Datatype = metric.DataType,
                IsHistorical = metric.IsHistorical,
                IsNull = metric.IsNull,
                IsTransient = metric.IsTransient,
                Metadata = ConvertVersionBMetaData(metric.Metadata),
                Name = metric.Name,
                Properties = ConvertVersionBPropertySet(metric.Properties),
                Timestamp = metric.Timestamp
            },
            VersionBDataTypeEnum.Boolean => new VersionBProtoBuf.ProtoBufPayload.Metric
            {
                Alias = metric.Alias,
                BooleanValue = metric.BooleanValue,
                Datatype = metric.DataType,
                IsHistorical = metric.IsHistorical,
                IsNull = metric.IsNull,
                IsTransient = metric.IsTransient,
                Metadata = ConvertVersionBMetaData(metric.Metadata),
                Name = metric.Name,
                Properties = ConvertVersionBPropertySet(metric.Properties),
                Timestamp = metric.Timestamp
            },
            VersionBDataTypeEnum.String => new VersionBProtoBuf.ProtoBufPayload.Metric
            {
                Alias = metric.Alias,
                Datatype = metric.DataType,
                IsHistorical = metric.IsHistorical,
                IsNull = metric.IsNull,
                IsTransient = metric.IsTransient,
                Metadata = ConvertVersionBMetaData(metric.Metadata),
                Name = metric.Name,
                Properties = ConvertVersionBPropertySet(metric.Properties),
                StringValue = metric.StringValue,
                Timestamp = metric.Timestamp
            },
            VersionBDataTypeEnum.DateTime => new VersionBProtoBuf.ProtoBufPayload.Metric
            {
                Alias = metric.Alias,
                Datatype = metric.DataType,
                IsHistorical = metric.IsHistorical,
                IsNull = metric.IsNull,
                IsTransient = metric.IsTransient,
                LongValue = metric.LongValue,
                Metadata = ConvertVersionBMetaData(metric.Metadata),
                Name = metric.Name,
                Properties = ConvertVersionBPropertySet(metric.Properties),
                Timestamp = metric.Timestamp
            },
            VersionBDataTypeEnum.Text => new VersionBProtoBuf.ProtoBufPayload.Metric
            {
                Alias = metric.Alias,
                Datatype = metric.DataType,
                IsHistorical = metric.IsHistorical,
                IsNull = metric.IsNull,
                IsTransient = metric.IsTransient,
                Metadata = ConvertVersionBMetaData(metric.Metadata),
                Name = metric.Name,
                Properties = ConvertVersionBPropertySet(metric.Properties),
                StringValue = metric.StringValue,
                Timestamp = metric.Timestamp
            },
            VersionBDataTypeEnum.Uuid => new VersionBProtoBuf.ProtoBufPayload.Metric
            {
                Alias = metric.Alias,
                Datatype = metric.DataType,
                IsHistorical = metric.IsHistorical,
                IsNull = metric.IsNull,
                IsTransient = metric.IsTransient,
                Metadata = ConvertVersionBMetaData(metric.Metadata),
                Name = metric.Name,
                Properties = ConvertVersionBPropertySet(metric.Properties),
                StringValue = metric.StringValue,
                Timestamp = metric.Timestamp
            },
            VersionBDataTypeEnum.DataSet => new VersionBProtoBuf.ProtoBufPayload.Metric
            {
                Alias = metric.Alias,
                DatasetValue = ConvertVersionBDataSet(metric.DataSetValue),
                Datatype = metric.DataType,
                IsHistorical = metric.IsHistorical,
                IsNull = metric.IsNull,
                IsTransient = metric.IsTransient,
                Metadata = ConvertVersionBMetaData(metric.Metadata),
                Name = metric.Name,
                Properties = ConvertVersionBPropertySet(metric.Properties),
                Timestamp = metric.Timestamp
            },
            VersionBDataTypeEnum.Bytes => new VersionBProtoBuf.ProtoBufPayload.Metric
            {
                Alias = metric.Alias,
                Datatype = metric.DataType,
                ExtensionValue = (metric.ExtensionValue is not null) ? new VersionBProtoBuf.ProtoBufPayload.Metric.MetricValueExtension
                {
                    Details = metric.ExtensionValue.Details
                } : null,
                IsHistorical = metric.IsHistorical,
                IsNull = metric.IsNull,
                IsTransient = metric.IsTransient,
                Metadata = ConvertVersionBMetaData(metric.Metadata),
                Name = metric.Name,
                Properties = ConvertVersionBPropertySet(metric.Properties),
                Timestamp = metric.Timestamp
            },
            VersionBDataTypeEnum.File => new VersionBProtoBuf.ProtoBufPayload.Metric
            {
                Alias = metric.Alias,
                Datatype = metric.DataType,
                ExtensionValue = (metric.ExtensionValue is not null) ? new VersionBProtoBuf.ProtoBufPayload.Metric.MetricValueExtension
                {
                    Details = metric.ExtensionValue.Details
                } : null,
                IsHistorical = metric.IsHistorical,
                IsNull = metric.IsNull,
                IsTransient = metric.IsTransient,
                Metadata = ConvertVersionBMetaData(metric.Metadata),
                Name = metric.Name,
                Properties = ConvertVersionBPropertySet(metric.Properties),
                Timestamp = metric.Timestamp
            },
            VersionBDataTypeEnum.Template => new VersionBProtoBuf.ProtoBufPayload.Metric
            {
                Alias = metric.Alias,
                Datatype = metric.DataType,
                IsHistorical = metric.IsHistorical,
                IsNull = metric.IsNull,
                IsTransient = metric.IsTransient,
                Metadata = ConvertVersionBMetaData(metric.Metadata),
                Name = metric.Name,
                Properties = ConvertVersionBPropertySet(metric.Properties),
                Timestamp = metric.Timestamp,
                TemplateValue = ConvertVersionBTemplate(metric.TemplateValue)
            },
            VersionBDataTypeEnum.PropertySet => new VersionBProtoBuf.ProtoBufPayload.Metric
            {
                Alias = metric.Alias,
                Datatype = metric.DataType,
                ExtensionValue = (metric.ExtensionValue is not null) ? new VersionBProtoBuf.ProtoBufPayload.Metric.MetricValueExtension
                {
                    Details = metric.ExtensionValue.Details
                } : null,
                IsHistorical = metric.IsHistorical,
                IsNull = metric.IsNull,
                IsTransient = metric.IsTransient,
                Metadata = ConvertVersionBMetaData(metric.Metadata),
                Name = metric.Name,
                Properties = ConvertVersionBPropertySet(metric.Properties),
                Timestamp = metric.Timestamp
            },
            VersionBDataTypeEnum.PropertySetList => new VersionBProtoBuf.ProtoBufPayload.Metric
            {
                Alias = metric.Alias,
                Datatype = metric.DataType,
                ExtensionValue = (metric.ExtensionValue is not null) ? new VersionBProtoBuf.ProtoBufPayload.Metric.MetricValueExtension
                {
                    Details = metric.ExtensionValue.Details
                } : null,
                IsHistorical = metric.IsHistorical,
                IsNull = metric.IsNull,
                IsTransient = metric.IsTransient,
                Metadata = ConvertVersionBMetaData(metric.Metadata),
                Name = metric.Name,
                Properties = ConvertVersionBPropertySet(metric.Properties),
                Timestamp = metric.Timestamp
            },
            _ => new VersionBProtoBuf.ProtoBufPayload.Metric
            {
                Alias = metric.Alias,
                Datatype = metric.DataType,
                ExtensionValue = (metric.ExtensionValue is not null) ? new VersionBProtoBuf.ProtoBufPayload.Metric.MetricValueExtension
                {
                    Details = metric.ExtensionValue.Details
                } : null,
                IsHistorical = metric.IsHistorical,
                IsNull = metric.IsNull,
                IsTransient = metric.IsTransient,
                Metadata = ConvertVersionBMetaData(metric.Metadata),
                Name = metric.Name,
                Properties = ConvertVersionBPropertySet(metric.Properties),
                Timestamp = metric.Timestamp
            }
        };
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
    {
        if (propertySetList is null)
        {
            return null;
        }

        return new VersionBProtoBuf.ProtoBufPayload.PropertySetList
        {
            Details = propertySetList.Details,
            Propertysets = propertySetList.PropertySets.Select(ConvertVersionBPropertySet).ToNonNullList()
        };
    }

    /// <summary>
    /// Gets the version B property set from the version B ProtoBuf property set.
    /// </summary>
    /// <param name="propertySet">The <see cref="VersionBProtoBuf.ProtoBufPayload.PropertySet"/>.</param>
    /// <returns>The <see cref="VersionBData.PropertySet"/>.</returns>
    private static VersionBData.PropertySet? ConvertVersionBPropertySet(VersionBProtoBuf.ProtoBufPayload.PropertySet? propertySet)
    {
        if (propertySet is null)
        {
            return null;
        }

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
    private static VersionBProtoBuf.ProtoBufPayload.PropertySet? ConvertVersionBPropertySet(VersionBData.PropertySet? propertySet)
    {
        if (propertySet is null)
        {
            return null;
        }

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
            Type = propertyValue.Type,
            ValueCase = ConvertVersionBDataTypePropertyValue(propertyValue.ValueCase)
        };
    }

    /// <summary>
    /// Gets the version B ProtoBuf property value from the version B property value.
    /// </summary>
    /// <param name="propertyValue">The <see cref="VersionBData.PropertyValue"/>.</param>
    /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload.PropertyValue"/>.</returns>
    private static VersionBProtoBuf.ProtoBufPayload.PropertyValue ConvertVersionBPropertyValue(VersionBData.PropertyValue propertyValue)
    {
        return propertyValue.ValueCase switch
        {
            VersionBDataTypeEnum.Unknown => new VersionBProtoBuf.ProtoBufPayload.PropertyValue
            {
                ExtensionValue = new VersionBProtoBuf.ProtoBufPayload.PropertyValue.PropertyValueExtension
                {
                    Details = propertyValue.ExtensionValue.Details
                },
                IsNull = propertyValue.IsNull,
                Type = propertyValue.Type
            },
            VersionBDataTypeEnum.Int8 => new VersionBProtoBuf.ProtoBufPayload.PropertyValue
            {
                IntValue = propertyValue.IntValue,
                IsNull = propertyValue.IsNull,
                Type = propertyValue.Type
            },
            VersionBDataTypeEnum.Int16 => new VersionBProtoBuf.ProtoBufPayload.PropertyValue
            {
                IntValue = propertyValue.IntValue,
                IsNull = propertyValue.IsNull,
                Type = propertyValue.Type
            },
            VersionBDataTypeEnum.Int32 => new VersionBProtoBuf.ProtoBufPayload.PropertyValue
            {
                IntValue = propertyValue.IntValue,
                IsNull = propertyValue.IsNull,
                Type = propertyValue.Type
            },
            VersionBDataTypeEnum.Int64 => new VersionBProtoBuf.ProtoBufPayload.PropertyValue
            {
                IsNull = propertyValue.IsNull,
                LongValue = propertyValue.LongValue,
                Type = propertyValue.Type
            },
            VersionBDataTypeEnum.UInt8 => new VersionBProtoBuf.ProtoBufPayload.PropertyValue
            {
                IntValue = propertyValue.IntValue,
                IsNull = propertyValue.IsNull,
                Type = propertyValue.Type
            },
            VersionBDataTypeEnum.UInt16 => new VersionBProtoBuf.ProtoBufPayload.PropertyValue
            {
                IntValue = propertyValue.IntValue,
                IsNull = propertyValue.IsNull,
                Type = propertyValue.Type
            },
            VersionBDataTypeEnum.UInt32 => new VersionBProtoBuf.ProtoBufPayload.PropertyValue
            {
                IntValue = propertyValue.IntValue,
                IsNull = propertyValue.IsNull,
                Type = propertyValue.Type
            },
            VersionBDataTypeEnum.UInt64 => new VersionBProtoBuf.ProtoBufPayload.PropertyValue
            {
                IsNull = propertyValue.IsNull,
                LongValue = propertyValue.LongValue,
                Type = propertyValue.Type
            },
            VersionBDataTypeEnum.Float => new VersionBProtoBuf.ProtoBufPayload.PropertyValue
            {
                FloatValue = propertyValue.FloatValue,
                IsNull = propertyValue.IsNull,
                Type = propertyValue.Type
            },
            VersionBDataTypeEnum.Double => new VersionBProtoBuf.ProtoBufPayload.PropertyValue
            {
                DoubleValue = propertyValue.DoubleValue,
                IsNull = propertyValue.IsNull,
                Type = propertyValue.Type
            },
            VersionBDataTypeEnum.Boolean => new VersionBProtoBuf.ProtoBufPayload.PropertyValue
            {
                BooleanValue = propertyValue.BooleanValue,
                IsNull = propertyValue.IsNull,
                Type = propertyValue.Type
            },
            VersionBDataTypeEnum.String => new VersionBProtoBuf.ProtoBufPayload.PropertyValue
            {
                IsNull = propertyValue.IsNull,
                StringValue = propertyValue.StringValue,
                Type = propertyValue.Type
            },
            VersionBDataTypeEnum.DateTime => new VersionBProtoBuf.ProtoBufPayload.PropertyValue
            {
                IsNull = propertyValue.IsNull,
                LongValue = propertyValue.LongValue,
                Type = propertyValue.Type
            },
            VersionBDataTypeEnum.Text => new VersionBProtoBuf.ProtoBufPayload.PropertyValue
            {
                IsNull = propertyValue.IsNull,
                StringValue = propertyValue.StringValue,
                Type = propertyValue.Type
            },
            VersionBDataTypeEnum.Uuid => new VersionBProtoBuf.ProtoBufPayload.PropertyValue
            {
                IsNull = propertyValue.IsNull,
                StringValue = propertyValue.StringValue,
                Type = propertyValue.Type
            },
            VersionBDataTypeEnum.DataSet => new VersionBProtoBuf.ProtoBufPayload.PropertyValue
            {
                ExtensionValue = new VersionBProtoBuf.ProtoBufPayload.PropertyValue.PropertyValueExtension
                {
                    Details = propertyValue.ExtensionValue.Details
                },
                IsNull = propertyValue.IsNull,
                Type = propertyValue.Type
            },
            VersionBDataTypeEnum.Bytes => new VersionBProtoBuf.ProtoBufPayload.PropertyValue
            {
                ExtensionValue = new VersionBProtoBuf.ProtoBufPayload.PropertyValue.PropertyValueExtension
                {
                    Details = propertyValue.ExtensionValue.Details
                },
                IsNull = propertyValue.IsNull,
                Type = propertyValue.Type
            },
            VersionBDataTypeEnum.File => new VersionBProtoBuf.ProtoBufPayload.PropertyValue
            {
                ExtensionValue = new VersionBProtoBuf.ProtoBufPayload.PropertyValue.PropertyValueExtension
                {
                    Details = propertyValue.ExtensionValue.Details
                },
                IsNull = propertyValue.IsNull,
                Type = propertyValue.Type
            },
            VersionBDataTypeEnum.Template => new VersionBProtoBuf.ProtoBufPayload.PropertyValue
            {
                ExtensionValue = new VersionBProtoBuf.ProtoBufPayload.PropertyValue.PropertyValueExtension
                {
                    Details = propertyValue.ExtensionValue.Details
                },
                IsNull = propertyValue.IsNull,
                Type = propertyValue.Type
            },
            VersionBDataTypeEnum.PropertySet => new VersionBProtoBuf.ProtoBufPayload.PropertyValue
            {
                IsNull = propertyValue.IsNull,
                PropertysetValue = ConvertVersionBPropertySet(propertyValue.PropertySetValue),
                Type = propertyValue.Type
            },
            VersionBDataTypeEnum.PropertySetList => new VersionBProtoBuf.ProtoBufPayload.PropertyValue
            {
                PropertysetsValue = ConvertVersionBPropertySetList(propertyValue.PropertySetsValue),
                IsNull = propertyValue.IsNull,
                Type = propertyValue.Type
            },
            _ => new VersionBProtoBuf.ProtoBufPayload.PropertyValue
            {
                ExtensionValue = new VersionBProtoBuf.ProtoBufPayload.PropertyValue.PropertyValueExtension
                {
                    Details = propertyValue.ExtensionValue.Details
                },
                IsNull = propertyValue.IsNull,
                Type = propertyValue.Type
            }
        };
    }
}
