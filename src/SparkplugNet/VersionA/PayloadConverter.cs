// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PayloadConverter.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A helper class for the payload conversions from internal ProtoBuf model to external data and vice versa for version A.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.VersionA;

/// <summary>
/// A helper class for the payload conversions from internal ProtoBuf model to external data and vice versa for version A.
/// </summary>
internal static class PayloadConverter
{
    /// <summary>
    /// Gets the version A payload converted from the ProtoBuf payload.
    /// </summary>
    /// <param name="payload">The <see cref="VersionAProtoBuf.ProtoBufPayload"/>.</param>
    /// <returns>The <see cref="VersionAData.Payload"/>.</returns>
    public static VersionAData.Payload ConvertVersionAPayload(VersionAProtoBuf.ProtoBufPayload payload)
        => new()
        {
            Body = payload.Body,
            Metrics = payload.Metrics.Select(ConvertVersionAMetric).ToList(),
            Position = new VersionAData.KuraPosition
            {
                Timestamp = payload.Position?.Timestamp,
                Altitude = payload.Position?.Altitude,
                Heading = payload.Position?.Heading,
                Latitude = payload.Position?.Latitude ?? default,
                Longitude = payload.Position?.Longitude ?? default,
                Precision = payload.Position?.Precision,
                Satellites = payload.Position?.Satellites,
                Speed = payload.Position?.Speed,
                Status = payload.Position?.Status
            },
            Timestamp = payload.Timestamp
        };

    /// <summary>
    /// Gets the ProtoBuf payload converted from the version A payload.
    /// </summary>
    /// <param name="payload">The <see cref="VersionAData.Payload"/>.</param>
    /// <returns>The <see cref="VersionAProtoBuf.ProtoBufPayload"/>.</returns>
    public static VersionAProtoBuf.ProtoBufPayload ConvertVersionAPayload(VersionAData.Payload payload)
        => new()
        {
            Body = payload.Body,
            Metrics = payload.Metrics.Select(ConvertVersionAMetric).ToList(),
            Position = new VersionAProtoBuf.ProtoBufPayload.KuraPosition
            {
                Timestamp = payload.Position?.Timestamp,
                Altitude = payload.Position?.Altitude,
                Heading = payload.Position?.Heading,
                Latitude = payload.Position?.Latitude ?? default,
                Longitude = payload.Position?.Longitude ?? default,
                Precision = payload.Position?.Precision,
                Satellites = payload.Position?.Satellites,
                Speed = payload.Position?.Speed,
                Status = payload.Position?.Status
            },
            Timestamp = payload.Timestamp
        };

    /// <summary>
    /// Gets the ProtoBuf metric converted from the version A metric.
    /// </summary>
    /// <param name="metric">The <see cref="VersionAData.KuraMetric"/>.</param>
    /// <returns>The <see cref="VersionAProtoBuf.ProtoBufPayload.KuraMetric"/>.</returns>
    public static VersionAProtoBuf.ProtoBufPayload.KuraMetric ConvertVersionAMetric(VersionAData.KuraMetric metric)
    {
        var newMetric = new VersionAProtoBuf.ProtoBufPayload.KuraMetric
        {
            Name = metric.Name,
            DataType = ConvertVersionADataType(metric.DataType)
        };

        switch (newMetric.DataType)
        {
            case VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType.Bool:
                newMetric.BooleanValue = metric.Value.ConvertOrDefaultTo<bool>();
                break;
            case VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType.Bytes:
                newMetric.BytesValue = metric.Value.ConvertOrDefaultTo<byte[]>();
                break;
            case VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType.Double:
                newMetric.DoubleValue = metric.Value.ConvertOrDefaultTo<double>();
                break;
            case VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType.Float:
                newMetric.FloatValue = metric.Value.ConvertOrDefaultTo<float>();
                break;
            case VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType.Int32:
                newMetric.IntValue = metric.Value.ConvertOrDefaultTo<int>();
                break;
            case VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType.Int64:
                newMetric.LongValue = metric.Value.ConvertOrDefaultTo<long>();
                break;
            case VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType.String:
                newMetric.StringValue = metric.Value.ConvertOrDefaultTo<string>();
                break;
        }

        return newMetric;
    }

    /// <summary>
    /// Gets the ProtoBuf metric converted from the version A metric.
    /// </summary>
    /// <param name="metric">The <see cref="VersionAProtoBuf.ProtoBufPayload.KuraMetric"/>.</param>
    /// <returns>The <see cref="VersionAData.KuraMetric"/>.</returns>
    public static VersionAData.KuraMetric ConvertVersionAMetric(VersionAProtoBuf.ProtoBufPayload.KuraMetric metric)
    {
        var newMetric = new VersionAData.KuraMetric()
        {
            Name = metric.Name
        };

        switch (newMetric.DataType)
        {
            case VersionADataTypeEnum.Boolean:
                newMetric.SetValue(VersionADataTypeEnum.Boolean, metric.BooleanValue);
                break;
            case VersionADataTypeEnum.Bytes:
                newMetric.SetValue(VersionADataTypeEnum.Bytes, metric.BytesValue);
                break;
            case VersionADataTypeEnum.Double:
                newMetric.SetValue(VersionADataTypeEnum.Double, metric.DoubleValue);
                break;
            case VersionADataTypeEnum.Float:
                newMetric.SetValue(VersionADataTypeEnum.Float, metric.FloatValue);
                break;
            case VersionADataTypeEnum.Int32:
                newMetric.SetValue(VersionADataTypeEnum.Int32, metric.IntValue);
                break;
            case VersionADataTypeEnum.Int64:
                newMetric.SetValue(VersionADataTypeEnum.Int64, metric.LongValue);
                break;
            case VersionADataTypeEnum.String:
                newMetric.SetValue(VersionADataTypeEnum.String, metric.StringValue);
                break;
        }

        return newMetric;
    }

    /// <summary>
    /// Gets the version A data type from the version A ProtoBuf value type.
    /// </summary>
    /// <param name="type">The <see cref="VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType"/>.</param>
    /// <returns>The <see cref="VersionAData.DataType"/>.</returns>
    public static VersionADataTypeEnum ConvertVersionADataType(VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType type)
        => type switch
        {
            VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType.Bool => VersionADataTypeEnum.Boolean,
            VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType.Bytes => VersionADataTypeEnum.Bytes,
            VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType.Double => VersionADataTypeEnum.Double,
            VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType.Float => VersionADataTypeEnum.Float,
            VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType.Int32 => VersionADataTypeEnum.Int32,
            VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType.Int64 => VersionADataTypeEnum.Int64,
            VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType.String => VersionADataTypeEnum.String,
            _ => VersionADataTypeEnum.String
        };

    /// <summary>
    /// Gets the version A ProtoBuf value type from the version A data type.
    /// </summary>
    /// <param name="type">The <see cref="VersionAData.DataType"/>.</param>
    /// <returns>The <see cref="VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType"/>.</returns>
    public static VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType ConvertVersionADataType(VersionADataTypeEnum type)
        => type switch
        {
            VersionADataTypeEnum.Boolean => VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType.Bool,
            VersionADataTypeEnum.Bytes => VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType.Bytes,
            VersionADataTypeEnum.Double => VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType.Double,
            VersionADataTypeEnum.Float => VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType.Float,
            VersionADataTypeEnum.Int32 => VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType.Int32,
            VersionADataTypeEnum.Int64 => VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType.Int64,
            VersionADataTypeEnum.String => VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType.String,
            _ => VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType.String
        };
}
