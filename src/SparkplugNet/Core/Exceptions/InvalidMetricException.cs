// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvalidMetricException.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   The exception that is thrown if an invalid metric is specified.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Core.Exceptions;

/// <inheritdoc cref="Exception"/>
/// <summary>
/// The exception that is thrown if an invalid metric is specified.
/// </summary>
/// <seealso cref="Exception"/>
public sealed class InvalidMetricException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidMetricException"/> class.
    /// </summary>
    public InvalidMetricException() : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidMetricException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public InvalidMetricException(string? message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidMetricException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic)
    /// if no inner exception is specified.</param>
    public InvalidMetricException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
