// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataSet.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   The externally used Sparkplug B data set class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.VersionB.Data;

/// <summary>
/// The externally used Sparkplug B data set class.
/// </summary>
public sealed class DataSet
{
    /// <summary>
    /// Initializes the DataSet
    /// </summary>
    public DataSet()
    {
    }

    /// <summary>
    /// Initializes the DataSet using a Dictionary with Column names as key
    /// and DataType as value
    /// </summary>
    /// <param name="entries"></param>
    public DataSet(IDictionary<string, VersionBDataTypeEnum> entries)
    {
        this.NumberOfColumns = (ulong)entries.Count;
        this.Columns = [.. entries.Keys];
        this.Types = entries.Values.Select(type => (uint)type).ToArray();
    }

    /// <summary>
    /// Gets or sets the number of columns. A value of zero lets the encoder use the schema width.
    /// </summary>
    public ulong NumberOfColumns { get; set; }

    /// <summary>
    /// Gets or sets the column names. If empty, the encoder uses names such as Column1 and Column2.
    /// </summary>
    public List<string> Columns { get; set; } = [];

    /// <summary>
    /// Gets or sets the column types. If empty, the encoder infers them from the first row.
    /// </summary>
    public uint[] Types { get; set; } = [];

    /// <summary>
    /// Gets or sets the rows.
    /// </summary>
    public List<Row> Rows { get; set; } = [];
}
