// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMetric.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   The interface for all metrics.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Core.Interfaces;

/// <summary>
/// The interface for all metrics.
/// </summary>
public interface IMetric
{
    /// <summary>
    /// Gets the name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the value.
    /// </summary>
    public object? Value { get; }
}
