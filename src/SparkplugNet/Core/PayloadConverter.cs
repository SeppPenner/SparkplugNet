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
            Body = payload.Body ?? Array.Empty<byte>(),
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
    /// <returns>The <see cref="VersionBData.DataType"/>.</returns>
    public static VersionBData.DataType ConvertVersionBDataTypeDataSetValue(VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase type)
    {
        return type switch
        {
            VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.None => VersionBData.DataType.Unknown,
            VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.IntValue => VersionBData.DataType.Int32,
            VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.LongValue => VersionBData.DataType.Int64,
            VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.FloatValue => VersionBData.DataType.Float,
            VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.DoubleValue => VersionBData.DataType.Double,
            VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.BooleanValue => VersionBData.DataType.Boolean,
            VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.StringValue => VersionBData.DataType.String,
            VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.ExtensionValue => VersionBData.DataType.Unknown,
            _ => VersionBData.DataType.String
        };
    }

    /// <summary>
    /// Gets the version B ProtoBuf value type from the version B data type for data set values.
    /// </summary>
    /// <param name="type">The <see cref="VersionBData.DataType"/>.</param>
    /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase"/>.</returns>
    public static VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase ConvertVersionBDataTypeDataSetValue(VersionBData.DataType type)
    {
        return type switch
        {
            VersionBData.DataType.Unknown => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.ExtensionValue,
            VersionBData.DataType.Int8 => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.IntValue,
            VersionBData.DataType.Int16 => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.IntValue,
            VersionBData.DataType.Int32 => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.IntValue,
            VersionBData.DataType.Int64 => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.LongValue,
            VersionBData.DataType.UInt8 => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.IntValue,
            VersionBData.DataType.UInt16 => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.IntValue,
            VersionBData.DataType.UInt32 => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.IntValue,
            VersionBData.DataType.UInt64 => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.LongValue,
            VersionBData.DataType.Float => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.FloatValue,
            VersionBData.DataType.Double => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.DoubleValue,
            VersionBData.DataType.Boolean => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.BooleanValue,
            VersionBData.DataType.String => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.StringValue,
            VersionBData.DataType.DateTime => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.LongValue,
            VersionBData.DataType.Text => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.StringValue,
            VersionBData.DataType.Uuid => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.StringValue,
            VersionBData.DataType.DataSet => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.None,
            VersionBData.DataType.Bytes => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.ExtensionValue,
            VersionBData.DataType.File => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.ExtensionValue,
            VersionBData.DataType.Template => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.None,
            VersionBData.DataType.PropertySet => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.None,
            VersionBData.DataType.PropertySetList => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.None,
            _ => VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.ValueOneofCase.None
        };
    }

    /// <summary>
    /// Gets the version B data type from the version B ProtoBuf value type for metrics.
    /// </summary>
    /// <param name="type">The <see cref="VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase"/>.</param>
    /// <returns>The <see cref="VersionBData.DataType"/>.</returns>
    public static VersionBData.DataType ConvertVersionBDataTypeMetric(VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase type)
    {
        return type switch
        {
            VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.None => VersionBData.DataType.Unknown,
            VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.IntValue => VersionBData.DataType.Int32,
            VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.LongValue => VersionBData.DataType.Int64,
            VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.FloatValue => VersionBData.DataType.Float,
            VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.DoubleValue => VersionBData.DataType.Double,
            VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.BooleanValue => VersionBData.DataType.Boolean,
            VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.StringValue => VersionBData.DataType.String,
            VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.ExtensionValue => VersionBData.DataType.Unknown,
            _ => VersionBData.DataType.String
        };
    }

    /// <summary>
    /// Gets the version B ProtoBuf value type from the version B data type for metrics.
    /// </summary>
    /// <param name="type">The <see cref="VersionBData.DataType"/>.</param>
    /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase"/>.</returns>
    public static VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase ConvertVersionBDataTypeMetric(VersionBData.DataType type)
    {
        return type switch
        {
            VersionBData.DataType.Unknown => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.ExtensionValue,
            VersionBData.DataType.Int8 => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.IntValue,
            VersionBData.DataType.Int16 => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.IntValue,
            VersionBData.DataType.Int32 => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.IntValue,
            VersionBData.DataType.Int64 => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.LongValue,
            VersionBData.DataType.UInt8 => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.IntValue,
            VersionBData.DataType.UInt16 => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.IntValue,
            VersionBData.DataType.UInt32 => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.IntValue,
            VersionBData.DataType.UInt64 => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.LongValue,
            VersionBData.DataType.Float => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.FloatValue,
            VersionBData.DataType.Double => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.DoubleValue,
            VersionBData.DataType.Boolean => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.BooleanValue,
            VersionBData.DataType.String => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.StringValue,
            VersionBData.DataType.DateTime => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.LongValue,
            VersionBData.DataType.Text => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.StringValue,
            VersionBData.DataType.Uuid => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.StringValue,
            VersionBData.DataType.DataSet => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.None,
            VersionBData.DataType.Bytes => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.ExtensionValue,
            VersionBData.DataType.File => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.ExtensionValue,
            VersionBData.DataType.Template => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.None,
            VersionBData.DataType.PropertySet => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.None,
            VersionBData.DataType.PropertySetList => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.None,
            _ => VersionBProtoBuf.ProtoBufPayload.Metric.ValueOneofCase.None
        };
    }

    /// <summary>
    /// Gets the version B data type from the version B ProtoBuf value type for parameters.
    /// </summary>
    /// <param name="type">The <see cref="VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase"/>.</param>
    /// <returns>The <see cref="VersionBData.DataType"/>.</returns>
    public static VersionBData.DataType ConvertVersionBDataTypeParameter(VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase type)
    {
        return type switch
        {
            VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.None => VersionBData.DataType.Unknown,
            VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.IntValue => VersionBData.DataType.Int32,
            VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.LongValue => VersionBData.DataType.Int64,
            VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.FloatValue => VersionBData.DataType.Float,
            VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.DoubleValue => VersionBData.DataType.Double,
            VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.BooleanValue => VersionBData.DataType.Boolean,
            VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.StringValue => VersionBData.DataType.String,
            VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.ExtensionValue => VersionBData.DataType.Unknown,
            _ => VersionBData.DataType.String
        };
    }

    /// <summary>
    /// Gets the version B ProtoBuf value type from the version B data type for parameters.
    /// </summary>
    /// <param name="type">The <see cref="VersionBData.DataType"/>.</param>
    /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase"/>.</returns>
    public static VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase ConvertVersionBDataTypeParameter(VersionBData.DataType type)
    {
        return type switch
        {
            VersionBData.DataType.Unknown => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.ExtensionValue,
            VersionBData.DataType.Int8 => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.IntValue,
            VersionBData.DataType.Int16 => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.IntValue,
            VersionBData.DataType.Int32 => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.IntValue,
            VersionBData.DataType.Int64 => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.LongValue,
            VersionBData.DataType.UInt8 => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.IntValue,
            VersionBData.DataType.UInt16 => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.IntValue,
            VersionBData.DataType.UInt32 => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.IntValue,
            VersionBData.DataType.UInt64 => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.LongValue,
            VersionBData.DataType.Float => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.FloatValue,
            VersionBData.DataType.Double => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.DoubleValue,
            VersionBData.DataType.Boolean => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.BooleanValue,
            VersionBData.DataType.String => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.StringValue,
            VersionBData.DataType.DateTime => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.LongValue,
            VersionBData.DataType.Text => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.StringValue,
            VersionBData.DataType.Uuid => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.StringValue,
            VersionBData.DataType.DataSet => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.None,
            VersionBData.DataType.Bytes => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.ExtensionValue,
            VersionBData.DataType.File => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.ExtensionValue,
            VersionBData.DataType.Template => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.None,
            VersionBData.DataType.PropertySet => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.None,
            VersionBData.DataType.PropertySetList => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.None,
            _ => VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ValueOneofCase.None
        };
    }

    /// <summary>
    /// Gets the version B data type from the version B ProtoBuf value type for property values.
    /// </summary>
    /// <param name="type">The <see cref="VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase"/>.</param>
    /// <returns>The <see cref="VersionBData.DataType"/>.</returns>
    public static VersionBData.DataType ConvertVersionBDataTypePropertyValue(VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase type)
    {
        return type switch
        {
            VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.None => VersionBData.DataType.Unknown,
            VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.IntValue => VersionBData.DataType.Int32,
            VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.LongValue => VersionBData.DataType.Int64,
            VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.FloatValue => VersionBData.DataType.Float,
            VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.DoubleValue => VersionBData.DataType.Double,
            VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.BooleanValue => VersionBData.DataType.Boolean,
            VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.StringValue => VersionBData.DataType.String,
            VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.ExtensionValue => VersionBData.DataType.Unknown,
            _ => VersionBData.DataType.String
        };
    }

    /// <summary>
    /// Gets the version B ProtoBuf value type from the version B data type for property values.
    /// </summary>
    /// <param name="type">The <see cref="VersionBData.DataType"/>.</param>
    /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase"/>.</returns>
    public static VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase ConvertVersionBDataTypePropertyValue(VersionBData.DataType type)
    {
        return type switch
        {
            VersionBData.DataType.Unknown => VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.ExtensionValue,
            VersionBData.DataType.Int8 => VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.IntValue,
            VersionBData.DataType.Int16 => VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.IntValue,
            VersionBData.DataType.Int32 => VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.IntValue,
            VersionBData.DataType.Int64 => VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.LongValue,
            VersionBData.DataType.UInt8 => VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.IntValue,
            VersionBData.DataType.UInt16 => VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.IntValue,
            VersionBData.DataType.UInt32 => VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.IntValue,
            VersionBData.DataType.UInt64 => VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.LongValue,
            VersionBData.DataType.Float => VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.FloatValue,
            VersionBData.DataType.Double => VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.DoubleValue,
            VersionBData.DataType.Boolean => VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.BooleanValue,
            VersionBData.DataType.String => VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.StringValue,
            VersionBData.DataType.DateTime => VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.LongValue,
            VersionBData.DataType.Text => VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.StringValue,
            VersionBData.DataType.Uuid => VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.StringValue,
            VersionBData.DataType.DataSet => VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.None,
            VersionBData.DataType.Bytes => VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.ExtensionValue,
            VersionBData.DataType.File => VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.ExtensionValue,
            VersionBData.DataType.Template => VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.None,
            VersionBData.DataType.PropertySet => VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.None,
            VersionBData.DataType.PropertySetList => VersionBProtoBuf.ProtoBufPayload.PropertyValue.ValueOneofCase.None,
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
            VersionBData.DataType.Unknown => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
            {
                ExtensionValue = new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.DataSetValueExtension
                {
                    Details = dataSetValue.ExtensionValue.Details
                }
            },
            VersionBData.DataType.Int8 => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
            {
                IntValue = dataSetValue.IntValue
            },
            VersionBData.DataType.Int16 => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
            {
                IntValue = dataSetValue.IntValue
            },
            VersionBData.DataType.Int32 => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
            {
                IntValue = dataSetValue.IntValue
            },
            VersionBData.DataType.Int64 => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
            {
                LongValue = dataSetValue.LongValue
            },
            VersionBData.DataType.UInt8 => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
            {
                IntValue = dataSetValue.IntValue
            },
            VersionBData.DataType.UInt16 => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
            {
                IntValue = dataSetValue.IntValue
            },
            VersionBData.DataType.UInt32 => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
            {
                IntValue = dataSetValue.IntValue
            },
            VersionBData.DataType.UInt64 => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
            {
                LongValue = dataSetValue.LongValue
            },
            VersionBData.DataType.Float => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
            {
                FloatValue = dataSetValue.FloatValue
            },
            VersionBData.DataType.Double => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
            {
                DoubleValue = dataSetValue.DoubleValue
            },
            VersionBData.DataType.Boolean => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
            {
                BooleanValue = dataSetValue.BooleanValue
            },
            VersionBData.DataType.String => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
            {
                StringValue = dataSetValue.StringValue
            },
            VersionBData.DataType.DateTime => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
            {
                LongValue = dataSetValue.LongValue
            },
            VersionBData.DataType.Text => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
            {
                StringValue = dataSetValue.StringValue
            },
            VersionBData.DataType.Uuid => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
            {
                StringValue = dataSetValue.StringValue
            },
            VersionBData.DataType.DataSet => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
            {
                ExtensionValue = new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.DataSetValueExtension
                {
                    Details = dataSetValue.ExtensionValue.Details
                }
            },
            VersionBData.DataType.Bytes => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
            {
                ExtensionValue = new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.DataSetValueExtension
                {
                    Details = dataSetValue.ExtensionValue.Details
                }
            },
            VersionBData.DataType.File => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
            {
                ExtensionValue = new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.DataSetValueExtension
                {
                    Details = dataSetValue.ExtensionValue.Details
                }
            },
            VersionBData.DataType.Template => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
            {
                ExtensionValue = new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.DataSetValueExtension
                {
                    Details = dataSetValue.ExtensionValue.Details
                }
            },
            VersionBData.DataType.PropertySet => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
            {
                ExtensionValue = new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue.DataSetValueExtension
                {
                    Details = dataSetValue.ExtensionValue.Details
                }
            },
            VersionBData.DataType.PropertySetList => new VersionBProtoBuf.ProtoBufPayload.DataSet.DataSetValue
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
            VersionBData.DataType.Unknown => new VersionBProtoBuf.ProtoBufPayload.Template.Parameter
            {
                ExtensionValue = new VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ParameterValueExtension
                {
                    Extensions = parameter.ExtensionValue.Extensions
                },
                Name = parameter.Name,
                Type = parameter.Type
            },
            VersionBData.DataType.Int8 => new VersionBProtoBuf.ProtoBufPayload.Template.Parameter
            {
                IntValue = parameter.IntValue,
                Name = parameter.Name,
                Type = parameter.Type
            },
            VersionBData.DataType.Int16 => new VersionBProtoBuf.ProtoBufPayload.Template.Parameter
            {
                IntValue = parameter.IntValue,
                Name = parameter.Name,
                Type = parameter.Type
            },
            VersionBData.DataType.Int32 => new VersionBProtoBuf.ProtoBufPayload.Template.Parameter
            {
                IntValue = parameter.IntValue,
                Name = parameter.Name,
                Type = parameter.Type
            },
            VersionBData.DataType.Int64 => new VersionBProtoBuf.ProtoBufPayload.Template.Parameter
            {
                LongValue = parameter.LongValue,
                Name = parameter.Name,
                Type = parameter.Type
            },
            VersionBData.DataType.UInt8 => new VersionBProtoBuf.ProtoBufPayload.Template.Parameter
            {
                IntValue = parameter.IntValue,
                Name = parameter.Name,
                Type = parameter.Type
            },
            VersionBData.DataType.UInt16 => new VersionBProtoBuf.ProtoBufPayload.Template.Parameter
            {
                IntValue = parameter.IntValue,
                Name = parameter.Name,
                Type = parameter.Type
            },
            VersionBData.DataType.UInt32 => new VersionBProtoBuf.ProtoBufPayload.Template.Parameter
            {
                IntValue = parameter.IntValue,
                Name = parameter.Name,
                Type = parameter.Type
            },
            VersionBData.DataType.UInt64 => new VersionBProtoBuf.ProtoBufPayload.Template.Parameter
            {
                LongValue = parameter.LongValue,
                Name = parameter.Name,
                Type = parameter.Type
            },
            VersionBData.DataType.Float => new VersionBProtoBuf.ProtoBufPayload.Template.Parameter
            {
                FloatValue = parameter.FloatValue,
                Name = parameter.Name,
                Type = parameter.Type
            },
            VersionBData.DataType.Double => new VersionBProtoBuf.ProtoBufPayload.Template.Parameter
            {
                DoubleValue = parameter.DoubleValue,
                Name = parameter.Name,
                Type = parameter.Type
            },
            VersionBData.DataType.Boolean => new VersionBProtoBuf.ProtoBufPayload.Template.Parameter
            {
                BooleanValue = parameter.BooleanValue,
                Name = parameter.Name,
                Type = parameter.Type
            },
            VersionBData.DataType.String => new VersionBProtoBuf.ProtoBufPayload.Template.Parameter
            {
                StringValue = parameter.StringValue,
                Name = parameter.Name,
                Type = parameter.Type
            },
            VersionBData.DataType.DateTime => new VersionBProtoBuf.ProtoBufPayload.Template.Parameter
            {
                LongValue = parameter.LongValue,
                Name = parameter.Name,
                Type = parameter.Type
            },
            VersionBData.DataType.Text => new VersionBProtoBuf.ProtoBufPayload.Template.Parameter
            {
                StringValue = parameter.StringValue,
                Name = parameter.Name,
                Type = parameter.Type
            },
            VersionBData.DataType.Uuid => new VersionBProtoBuf.ProtoBufPayload.Template.Parameter
            {
                StringValue = parameter.StringValue,
                Name = parameter.Name,
                Type = parameter.Type
            },
            VersionBData.DataType.DataSet => new VersionBProtoBuf.ProtoBufPayload.Template.Parameter
            {
                ExtensionValue = new VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ParameterValueExtension
                {
                    Extensions = parameter.ExtensionValue.Extensions
                },
                Name = parameter.Name,
                StringValue = parameter.StringValue,
                Type = parameter.Type
            },
            VersionBData.DataType.Bytes => new VersionBProtoBuf.ProtoBufPayload.Template.Parameter
            {
                ExtensionValue = new VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ParameterValueExtension
                {
                    Extensions = parameter.ExtensionValue.Extensions
                },
                Name = parameter.Name,
                StringValue = parameter.StringValue,
                Type = parameter.Type
            },
            VersionBData.DataType.File => new VersionBProtoBuf.ProtoBufPayload.Template.Parameter
            {
                ExtensionValue = new VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ParameterValueExtension
                {
                    Extensions = parameter.ExtensionValue.Extensions
                },
                Name = parameter.Name,
                StringValue = parameter.StringValue,
                Type = parameter.Type
            },
            VersionBData.DataType.Template => new VersionBProtoBuf.ProtoBufPayload.Template.Parameter
            {
                ExtensionValue = new VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ParameterValueExtension
                {
                    Extensions = parameter.ExtensionValue.Extensions
                },
                Name = parameter.Name,
                StringValue = parameter.StringValue,
                Type = parameter.Type
            },
            VersionBData.DataType.PropertySet => new VersionBProtoBuf.ProtoBufPayload.Template.Parameter
            {
                ExtensionValue = new VersionBProtoBuf.ProtoBufPayload.Template.Parameter.ParameterValueExtension
                {
                    Extensions = parameter.ExtensionValue.Extensions
                },
                Name = parameter.Name,
                StringValue = parameter.StringValue,
                Type = parameter.Type
            },
            VersionBData.DataType.PropertySetList => new VersionBProtoBuf.ProtoBufPayload.Template.Parameter
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
            TemplateValue = ConvertVersionBTemplate(metric.TemplateValue),
            ValueCase = ConvertVersionBDataTypeMetric(metric.ValueCase)
        };
    }

    /// <summary>
    /// Gets the version B ProtoBuf metric from the version B metric.
    /// </summary>
    /// <param name="metric">The <see cref="VersionBData.Metric"/>.</param>
    /// <returns>The <see cref="VersionBProtoBuf.ProtoBufPayload.Metric"/>.</returns>
    private static VersionBProtoBuf.ProtoBufPayload.Metric ConvertVersionBMetric(VersionBData.Metric metric)
    {
        return metric.ValueCase switch
        {
            VersionBData.DataType.Unknown => new VersionBProtoBuf.ProtoBufPayload.Metric
            {
                Alias = metric.Alias,
                Datatype = metric.DataType,
                ExtensionValue = new VersionBProtoBuf.ProtoBufPayload.Metric.MetricValueExtension
                {
                    Details = metric.ExtensionValue.Details
                },
                IsHistorical = metric.IsHistorical,
                IsNull = metric.IsNull,
                IsTransient = metric.IsTransient,
                Metadata = ConvertVersionBMetaData(metric.Metadata),
                Name = metric.Name,
                Properties = ConvertVersionBPropertySet(metric.Properties),
                Timestamp = metric.Timestamp
            },
            VersionBData.DataType.Int8 => new VersionBProtoBuf.ProtoBufPayload.Metric
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
            VersionBData.DataType.Int16 => new VersionBProtoBuf.ProtoBufPayload.Metric
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
            VersionBData.DataType.Int32 => new VersionBProtoBuf.ProtoBufPayload.Metric
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
            VersionBData.DataType.Int64 => new VersionBProtoBuf.ProtoBufPayload.Metric
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
            VersionBData.DataType.UInt8 => new VersionBProtoBuf.ProtoBufPayload.Metric
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
            VersionBData.DataType.UInt16 => new VersionBProtoBuf.ProtoBufPayload.Metric
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
            VersionBData.DataType.UInt32 => new VersionBProtoBuf.ProtoBufPayload.Metric
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
            VersionBData.DataType.UInt64 => new VersionBProtoBuf.ProtoBufPayload.Metric
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
            VersionBData.DataType.Float => new VersionBProtoBuf.ProtoBufPayload.Metric
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
            VersionBData.DataType.Double => new VersionBProtoBuf.ProtoBufPayload.Metric
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
            VersionBData.DataType.Boolean => new VersionBProtoBuf.ProtoBufPayload.Metric
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
            VersionBData.DataType.String => new VersionBProtoBuf.ProtoBufPayload.Metric
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
            VersionBData.DataType.DateTime => new VersionBProtoBuf.ProtoBufPayload.Metric
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
            VersionBData.DataType.Text => new VersionBProtoBuf.ProtoBufPayload.Metric
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
            VersionBData.DataType.Uuid => new VersionBProtoBuf.ProtoBufPayload.Metric
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
            VersionBData.DataType.DataSet => new VersionBProtoBuf.ProtoBufPayload.Metric
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
            VersionBData.DataType.Bytes => new VersionBProtoBuf.ProtoBufPayload.Metric
            {
                Alias = metric.Alias,
                Datatype = metric.DataType,
                ExtensionValue = new VersionBProtoBuf.ProtoBufPayload.Metric.MetricValueExtension
                {
                    Details = metric.ExtensionValue.Details
                },
                IsHistorical = metric.IsHistorical,
                IsNull = metric.IsNull,
                IsTransient = metric.IsTransient,
                Metadata = ConvertVersionBMetaData(metric.Metadata),
                Name = metric.Name,
                Properties = ConvertVersionBPropertySet(metric.Properties),
                Timestamp = metric.Timestamp
            },
            VersionBData.DataType.File => new VersionBProtoBuf.ProtoBufPayload.Metric
            {
                Alias = metric.Alias,
                Datatype = metric.DataType,
                ExtensionValue = new VersionBProtoBuf.ProtoBufPayload.Metric.MetricValueExtension
                {
                    Details = metric.ExtensionValue.Details
                },
                IsHistorical = metric.IsHistorical,
                IsNull = metric.IsNull,
                IsTransient = metric.IsTransient,
                Metadata = ConvertVersionBMetaData(metric.Metadata),
                Name = metric.Name,
                Properties = ConvertVersionBPropertySet(metric.Properties),
                Timestamp = metric.Timestamp
            },
            VersionBData.DataType.Template => new VersionBProtoBuf.ProtoBufPayload.Metric
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
            VersionBData.DataType.PropertySet => new VersionBProtoBuf.ProtoBufPayload.Metric
            {
                Alias = metric.Alias,
                Datatype = metric.DataType,
                ExtensionValue = new VersionBProtoBuf.ProtoBufPayload.Metric.MetricValueExtension
                {
                    Details = metric.ExtensionValue.Details
                },
                IsHistorical = metric.IsHistorical,
                IsNull = metric.IsNull,
                IsTransient = metric.IsTransient,
                Metadata = ConvertVersionBMetaData(metric.Metadata),
                Name = metric.Name,
                Properties = ConvertVersionBPropertySet(metric.Properties),
                Timestamp = metric.Timestamp
            },
            VersionBData.DataType.PropertySetList => new VersionBProtoBuf.ProtoBufPayload.Metric
            {
                Alias = metric.Alias,
                Datatype = metric.DataType,
                ExtensionValue = new VersionBProtoBuf.ProtoBufPayload.Metric.MetricValueExtension
                {
                    Details = metric.ExtensionValue.Details
                },
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
                ExtensionValue = new VersionBProtoBuf.ProtoBufPayload.Metric.MetricValueExtension
                {
                    Details = metric.ExtensionValue.Details
                },
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
            VersionBData.DataType.Unknown => new VersionBProtoBuf.ProtoBufPayload.PropertyValue
            {
                ExtensionValue = new VersionBProtoBuf.ProtoBufPayload.PropertyValue.PropertyValueExtension
                {
                    Details = propertyValue.ExtensionValue.Details
                },
                IsNull = propertyValue.IsNull,
                Type = propertyValue.Type
            },
            VersionBData.DataType.Int8 => new VersionBProtoBuf.ProtoBufPayload.PropertyValue
            {
                IntValue = propertyValue.IntValue,
                IsNull = propertyValue.IsNull,
                Type = propertyValue.Type
            },
            VersionBData.DataType.Int16 => new VersionBProtoBuf.ProtoBufPayload.PropertyValue
            {
                IntValue = propertyValue.IntValue,
                IsNull = propertyValue.IsNull,
                Type = propertyValue.Type
            },
            VersionBData.DataType.Int32 => new VersionBProtoBuf.ProtoBufPayload.PropertyValue
            {
                IntValue = propertyValue.IntValue,
                IsNull = propertyValue.IsNull,
                Type = propertyValue.Type
            },
            VersionBData.DataType.Int64 => new VersionBProtoBuf.ProtoBufPayload.PropertyValue
            {
                IsNull = propertyValue.IsNull,
                LongValue = propertyValue.LongValue,
                Type = propertyValue.Type
            },
            VersionBData.DataType.UInt8 => new VersionBProtoBuf.ProtoBufPayload.PropertyValue
            {
                IntValue = propertyValue.IntValue,
                IsNull = propertyValue.IsNull,
                Type = propertyValue.Type
            },
            VersionBData.DataType.UInt16 => new VersionBProtoBuf.ProtoBufPayload.PropertyValue
            {
                IntValue = propertyValue.IntValue,
                IsNull = propertyValue.IsNull,
                Type = propertyValue.Type
            },
            VersionBData.DataType.UInt32 => new VersionBProtoBuf.ProtoBufPayload.PropertyValue
            {
                IntValue = propertyValue.IntValue,
                IsNull = propertyValue.IsNull,
                Type = propertyValue.Type
            },
            VersionBData.DataType.UInt64 => new VersionBProtoBuf.ProtoBufPayload.PropertyValue
            {
                IsNull = propertyValue.IsNull,
                LongValue = propertyValue.LongValue,
                Type = propertyValue.Type
            },
            VersionBData.DataType.Float => new VersionBProtoBuf.ProtoBufPayload.PropertyValue
            {
                FloatValue = propertyValue.FloatValue,
                IsNull = propertyValue.IsNull,
                Type = propertyValue.Type
            },
            VersionBData.DataType.Double => new VersionBProtoBuf.ProtoBufPayload.PropertyValue
            {
                DoubleValue = propertyValue.DoubleValue,
                IsNull = propertyValue.IsNull,
                Type = propertyValue.Type
            },
            VersionBData.DataType.Boolean => new VersionBProtoBuf.ProtoBufPayload.PropertyValue
            {
                BooleanValue = propertyValue.BooleanValue,
                IsNull = propertyValue.IsNull,
                Type = propertyValue.Type
            },
            VersionBData.DataType.String => new VersionBProtoBuf.ProtoBufPayload.PropertyValue
            {
                IsNull = propertyValue.IsNull,
                StringValue = propertyValue.StringValue,
                Type = propertyValue.Type
            },
            VersionBData.DataType.DateTime => new VersionBProtoBuf.ProtoBufPayload.PropertyValue
            {
                IsNull = propertyValue.IsNull,
                LongValue = propertyValue.LongValue,
                Type = propertyValue.Type
            },
            VersionBData.DataType.Text => new VersionBProtoBuf.ProtoBufPayload.PropertyValue
            {
                IsNull = propertyValue.IsNull,
                StringValue = propertyValue.StringValue,
                Type = propertyValue.Type
            },
            VersionBData.DataType.Uuid => new VersionBProtoBuf.ProtoBufPayload.PropertyValue
            {
                IsNull = propertyValue.IsNull,
                StringValue = propertyValue.StringValue,
                Type = propertyValue.Type
            },
            VersionBData.DataType.DataSet => new VersionBProtoBuf.ProtoBufPayload.PropertyValue
            {
                ExtensionValue = new VersionBProtoBuf.ProtoBufPayload.PropertyValue.PropertyValueExtension
                {
                    Details = propertyValue.ExtensionValue.Details
                },
                IsNull = propertyValue.IsNull,
                Type = propertyValue.Type
            },
            VersionBData.DataType.Bytes => new VersionBProtoBuf.ProtoBufPayload.PropertyValue
            {
                ExtensionValue = new VersionBProtoBuf.ProtoBufPayload.PropertyValue.PropertyValueExtension
                {
                    Details = propertyValue.ExtensionValue.Details
                },
                IsNull = propertyValue.IsNull,
                Type = propertyValue.Type
            },
            VersionBData.DataType.File => new VersionBProtoBuf.ProtoBufPayload.PropertyValue
            {
                ExtensionValue = new VersionBProtoBuf.ProtoBufPayload.PropertyValue.PropertyValueExtension
                {
                    Details = propertyValue.ExtensionValue.Details
                },
                IsNull = propertyValue.IsNull,
                Type = propertyValue.Type
            },
            VersionBData.DataType.Template => new VersionBProtoBuf.ProtoBufPayload.PropertyValue
            {
                ExtensionValue = new VersionBProtoBuf.ProtoBufPayload.PropertyValue.PropertyValueExtension
                {
                    Details = propertyValue.ExtensionValue.Details
                },
                IsNull = propertyValue.IsNull,
                Type = propertyValue.Type
            },
            VersionBData.DataType.PropertySet => new VersionBProtoBuf.ProtoBufPayload.PropertyValue
            {
                IsNull = propertyValue.IsNull,
                PropertysetValue = ConvertVersionBPropertySet(propertyValue.PropertySetValue),
                Type = propertyValue.Type
            },
            VersionBData.DataType.PropertySetList => new VersionBProtoBuf.ProtoBufPayload.PropertyValue
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
