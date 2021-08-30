// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataSet.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   The externally used Sparkplug B data set class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.VersionB.Data
{
    using System.Collections.Generic;

    /// <summary>
    /// The externally used Sparkplug B data set class.
    /// </summary>
    public class DataSet
    {
        /// <summary>
        /// Gets or sets the number of columns.
        /// </summary>
        public ulong NumOfColumns { get; set; }

        /// <summary>
        /// Gets or sets the columns.
        /// </summary>
        public List<string> Columns { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the types.
        /// </summary>
        public uint[] Types { get; set; }

        /// <summary>
        /// Gets or sets the rows.
        /// </summary>
        public List<Row> Rows { get; set; } = new List<Row>();

        /// <summary>
        /// Gets or sets the details.
        /// </summary>
        public List<byte> Details { get; set; } = new List<byte>();
    }
}
