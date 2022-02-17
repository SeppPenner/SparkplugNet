// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MetaData.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   The externally used Sparkplug B meta data class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.VersionB.Data;

/// <summary>
/// The externally used Sparkplug B meta data class.
/// </summary>
public class MetaData
{
    /// <summary>
    /// Gets or sets a value indicating whether the meta data is a multi part data set or not.
    /// </summary>
    public bool IsMultiPart { get; set; }

    /// <summary>
    /// Gets or sets the content type.
    /// </summary>
    [DefaultValue("")]
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the size.
    /// </summary>
    public ulong Size { get; set; }

    /// <summary>
    /// Gets or sets the SEQ number.
    /// </summary>
    public ulong Seq { get; set; }

    /// <summary>
    /// Gets or sets the file name.
    /// </summary>
    [DefaultValue("")]
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the file type.
    /// </summary>
    [DefaultValue("")]
    public string FileType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the MD5 hash.
    /// </summary>
    [DefaultValue("")]
    public string Md5 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    [DefaultValue("")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the details.
    /// </summary>
    public List<byte> Details { get; set; } = new();
}
