namespace CameraApp.Exceptions;

/// <summary>
/// Represents an exception that occurs during camera or photo picker operations.
/// </summary>
public class CameraException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CameraException" /> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public CameraException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CameraException" /> class with a specified error message and a reference to the inner exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public CameraException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
