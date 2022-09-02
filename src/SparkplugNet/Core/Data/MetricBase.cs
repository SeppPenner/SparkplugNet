namespace SparkplugNet.Core.Data
{
    using System;

    /// <summary>
    /// Base class for Metrics
    /// </summary>
    /// <typeparam name="DataTypeEnum">The type of the ata type enum.</typeparam>
    /// <seealso cref="SparkplugNet.Core.Data.IMetric" />
    public abstract class MetricBase<DataTypeEnum> : IMetric
        where DataTypeEnum : Enum
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [DefaultValue("")]
        public virtual string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public abstract object? Value { get; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public DataTypeEnum Type { get; set; } = default(DataTypeEnum)!;

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="dataType">Type of the data.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public abstract MetricBase<DataTypeEnum> SetValue(DataTypeEnum dataType, object value);

        /// <summary>
        /// Converts the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objValue">The object value.</param>
        /// <returns></returns>
        protected T? ConvertValue<T>(object objValue)
        {
            if (objValue == null)
            {
                return default(T);
            }
            else if (objValue is T)
            {
                return (T)objValue;
            }
            else
            {
                return (T)Convert.ChangeType(objValue, typeof(T));
            }
        }
    }
}
