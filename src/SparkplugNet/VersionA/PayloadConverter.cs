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
            Type = ConvertVersionADataType(metric.DataType)
        };

        switch (newMetric.Type)
        {
            case VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType.Bool:
                newMetric.BoolValue = metric.BooleanValue;
                break;
            case VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType.Bytes:
                newMetric.BytesValue = metric.BytesValue;
                break;
            case VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType.Double:
                newMetric.DoubleValue = metric.DoubleValue;
                break;
            case VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType.Float:
                newMetric.FloatValue = metric.FloatValue;
                break;
            case VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType.Int32:
                newMetric.IntValue = metric.IntValue;
                break;
            case VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType.Int64:
                newMetric.LongValue = metric.LongValue;
                break;
            case VersionAProtoBuf.ProtoBufPayload.KuraMetric.ValueType.String:
                newMetric.StringValue = metric.StringValue;
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
        var newMetric = new VersionAData.KuraMetric
        {
            Name = metric.Name,
            DataType = ConvertVersionADataType(metric.Type)
        };

        switch (newMetric.DataType)
        {
            case VersionAData.DataType.Boolean:
                newMetric.BooleanValue = metric.BoolValue ?? default;
                break;
            case VersionAData.DataType.Bytes:
                newMetric.BytesValue = metric.BytesValue ?? Array.Empty<byte>();
                break;
            case VersionAData.DataType.Double:
                newMetric.DoubleValue = metric.DoubleValue ?? default;
                break;
            case VersionAData.DataType.Float:
                newMetric.FloatValue = metric.FloatValue ?? default;
                break;
            case VersionAData.DataType.Int32:
                newMetric.IntValue = metric.IntValue ?? default;
                break;
            case VersionAData.DataType.Int64:
                newMetric.LongValue = metric.LongValue ?? default;
                break;
            case VersionAData.DataType.String:
                newMetric.StringValue = metric.StringValue ?? string.Empty;
                break;
        }

        return newMetric;
    }

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
}
