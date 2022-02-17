// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Template.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   The externally used Sparkplug B template class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.VersionB.Data;

/// <summary>
/// The externally used Sparkplug B template class.
/// </summary>
public class Template
{
    /// <summary>
    /// Gets or sets the version.
    /// </summary>
    [DefaultValue("")]
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the metrics.
    /// </summary>
    public List<Metric> Metrics { get; set; } = new();

    /// <summary>
    /// Gets or sets the parameters.
    /// </summary>
    public List<Parameter> Parameters { get; set; } = new();

    /// <summary>
    /// Gets or sets the template reference.
    /// </summary>
    [DefaultValue("")]
    public string TemplateRef { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the template is a definition or not.
    /// </summary>
    public bool IsDefinition { get; set; }

    /// <summary>
    /// Gets or sets the details.
    /// </summary>
    public List<byte> Details { get; set; } = new();
}
