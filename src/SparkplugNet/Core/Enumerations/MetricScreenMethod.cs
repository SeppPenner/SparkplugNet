namespace SparkplugNet.Core.Enumerations;

/// <summary>
/// Represents methods for screening incoming messages.
/// </summary>
public enum MetricScreenMethod
{
    /// <summary>
    /// None: Allow all incoming metrics to be passed through
    /// </summary>
    None = 0,

    /// <summary>
    /// Filter: Filter the incoming metrics, ignores unknown metrics
    /// </summary>
    Filter = 1,
    
    /// <summary>
    /// Validate: Validate the incoming metrics, throws exception when unknown metrics are detected
    /// </summary>
    Validate = 2
}
