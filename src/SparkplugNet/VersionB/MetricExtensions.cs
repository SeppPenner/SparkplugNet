namespace SparkplugNet.VersionB
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using SparkplugNet.Core.Enumerations;

    /// <summary>
    /// SparkplugB Metric Extensions
    /// </summary>
    public static class MetricExtensions
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="metric">The metric.</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public static object GetValue(this Payload.Metric metric)
        {
            switch (metric.Datatype)
            {
                case (int)SparkplugBDataType.String:
                case (int)SparkplugBDataType.Text:
                case (int)SparkplugBDataType.Uuid:
                    return metric.StringValue;
                case (int)SparkplugBDataType.Int8:
                case (int)SparkplugBDataType.UInt8:
                case (int)SparkplugBDataType.Int16:
                case (int)SparkplugBDataType.UInt16:
                case (int)SparkplugBDataType.Int32:
                case (int)SparkplugBDataType.UInt32:
                    return metric.IntValue;
                case (int)SparkplugBDataType.Int64:
                case (int)SparkplugBDataType.UInt64:
                case (int)SparkplugBDataType.DateTime:
                    return metric.LongValue;
                case (int)SparkplugBDataType.Float:
                    return metric.FloatValue;
                case (int)SparkplugBDataType.Double:
                    return metric.DoubleValue;
                case (int)SparkplugBDataType.Boolean:
                    return metric.BytesValue;
                case (int)SparkplugBDataType.Bytes:
                case (int)SparkplugBDataType.File:
                    return metric.BytesValue;
                case (int)SparkplugBDataType.Dataset:
                    return metric.DatasetValue;
                case (int)SparkplugBDataType.Template:
                    return metric.TemplateValue;
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Sets the metric historical.
        /// </summary>
        /// <param name="metrics">The metrics.</param>
        /// <param name="metricName">Name of the metric.</param>
        /// <param name="timestamp">The timestamp.</param>
        /// <param name="value">The value.</param>
        public static void SetMetricHistorical(
            this List<Payload.Metric> metrics,
            string metricName,
            ulong timestamp,
            object value)
        {
            SetMetric(metrics, metricName, timestamp, value, isNull: false, isTransient: false, isHistorical: true);
        }

        /// <summary>
        /// Sets the metric transient.
        /// </summary>
        /// <param name="metrics">The metrics.</param>
        /// <param name="metricName">Name of the metric.</param>
        /// <param name="timestamp">The timestamp.</param>
        /// <param name="value">The value.</param>
        public static void SetMetricTransient(
            this List<Payload.Metric> metrics,
            string metricName,
            ulong timestamp,
            object value)
        {
            SetMetric(metrics, metricName, timestamp, value, isNull: false, isTransient: true, isHistorical: false);
        }

        /// <summary>
        /// Sets the metric null.
        /// </summary>
        /// <param name="metrics">The metrics.</param>
        /// <param name="metricName">Name of the metric.</param>
        /// <param name="timestamp">The timestamp.</param>
        /// <param name="value">The value.</param>
        public static void SetMetricNull(
            this List<Payload.Metric> metrics,
            string metricName,
            ulong timestamp,
            object value)
        {
            SetMetric(metrics, metricName, timestamp, value, isNull: true, isTransient: false, isHistorical: false);
        }

        /// <summary>
        /// Gets the changed metrics.
        /// </summary>
        /// <param name="metrics">The metrics.</param>
        /// <param name="timestamp">The timestamp.</param>
        /// <returns></returns>
        public static List<Payload.Metric> GetChangedMetrics(this List<Payload.Metric> metrics, ulong timestamp)
        {
            return metrics.Where(x => x.Timestamp == timestamp).ToList();
        }

        private static void SetMetric(
            IEnumerable<Payload.Metric> metrics,
            string metricName,
            ulong timestamp,
            object value,
            bool isNull = false,
            bool isTransient = false,
            bool isHistorical = false)
        {
            var metric = metrics.FirstOrDefault(x => string.Equals(x.Name, metricName, StringComparison.CurrentCultureIgnoreCase));
            if (metric == null)
            {
                return;
            }

            switch (metric.Datatype)
            {
                case (int)SparkplugBDataType.String:
                case (int)SparkplugBDataType.Text:
                case (int)SparkplugBDataType.Uuid:
                    try
                    {
                        var stringValue = value is null || isNull ? null : value.ToString();
                        if (metric.StringValue == stringValue) { return; }
                        metric.StringValue = stringValue;
                    }
                    catch (Exception e)
                    {
                        return;
                    }

                    break;
                case (int)SparkplugBDataType.Int8:
                case (int)SparkplugBDataType.UInt8:
                case (int)SparkplugBDataType.Int16:
                case (int)SparkplugBDataType.UInt16:
                case (int)SparkplugBDataType.Int32:
                case (int)SparkplugBDataType.UInt32:
                    try
                    {
                        var intValue = value is null || isNull ? 0 : Convert.ToUInt32(value);
                        if (intValue < 0) intValue = 0;
                        if (metric.IntValue == intValue) { return; }
                        metric.IntValue = intValue;
                    }
                    catch (Exception e)
                    {
                        return;
                    }

                    break;
                case (int)SparkplugBDataType.Int64:
                case (int)SparkplugBDataType.UInt64:
                case (int)SparkplugBDataType.DateTime:
                    try
                    {
                        var longValue = value is null || isNull ? 0 : value is DateTime time ? (ulong)((DateTimeOffset)time).ToUnixTimeMilliseconds() : Convert.ToUInt64(value);
                        if (metric.LongValue == longValue) { return; }
                        metric.LongValue = longValue;
                    }
                    catch (Exception e)
                    {
                        return;
                    }

                    break;
                case (int)SparkplugBDataType.Float:
                    try
                    {
                        var floatValue = value is null || isNull ? 0 : Convert.ToSingle(value);
                        if (Math.Abs(metric.FloatValue - floatValue) < .001) { return; }
                        metric.FloatValue = floatValue;
                    }
                    catch (Exception e)
                    {
                        return;
                    }

                    break;
                case (int)SparkplugBDataType.Double:
                    try
                    {
                        var doubleValue = value is null || isNull ? 0 : Convert.ToDouble(value);
                        if (Math.Abs(metric.DoubleValue - doubleValue) < .001) { return; }
                        metric.DoubleValue = doubleValue;
                    }
                    catch (Exception e)
                    {
                        return;
                    }

                    break;
                case (int)SparkplugBDataType.Boolean:
                    try
                    {
                        var booleanValue = value is null || isNull ? false : Convert.ToBoolean(value);
                        if (metric.BooleanValue == booleanValue) { return; }
                        metric.BooleanValue = booleanValue;
                    }
                    catch (Exception e)
                    {
                        return;
                    }

                    break;
                case (int)SparkplugBDataType.Bytes:
                case (int)SparkplugBDataType.File:
                    try
                    {
                        metric.BytesValue = value is null || isNull ? null : (byte[])value;
                    }
                    catch (Exception e)
                    {
                        return;
                    }
                    break;
                case (int)SparkplugBDataType.Dataset:
                    try
                    {
                        metric.DatasetValue = isNull ? null :(Payload.DataSet)value;
                    }
                    catch (Exception e)
                    {
                        return;
                    }

                    break;
                case (int)SparkplugBDataType.Template:
                    try
                    {
                        metric.TemplateValue = isNull ? null :(Payload.Template)value;

                    }
                    catch (Exception e)
                    {
                        return;
                    }

                    break;
                default:
                    throw new NotSupportedException();
            }

            // don't set unless value is unique
            metric.IsNull = isNull;
            metric.IsTransient = isTransient;
            metric.IsHistorical = isHistorical;
            metric.Timestamp = timestamp;
        }
    }
}
