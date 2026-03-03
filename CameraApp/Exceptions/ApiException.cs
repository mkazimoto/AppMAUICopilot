using CameraApp.Models;

namespace CameraApp.Exceptions;

/// <summary>
/// Represents an exception thrown when an API request returns an error response.
/// </summary>
public class ApiException : Exception
{
    /// <summary>
    /// Gets the structured API error associated with this exception.
    /// </summary>
    /// <value>The <see cref="ApiError" /> describing the error returned by the API.</value>
    public ApiError ApiError { get; }

    /// <summary>
    /// Gets the HTTP status code returned by the API.
    /// </summary>
    /// <value>The HTTP status code, or <c>0</c> if not available.</value>
    public int StatusCode { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiException" /> class with a structured <see cref="ApiError" />.
    /// </summary>
    /// <param name="apiError">The structured error object returned by the API.</param>
    /// <param name="statusCode">The HTTP status code associated with the error.</param>
    public ApiException(ApiError apiError, int statusCode = 0)
        : base(apiError.GetDisplayMessage())
    {
        ApiError = apiError;
        StatusCode = statusCode;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiException" /> class with an error message.
    /// </summary>
    /// <param name="message">The error message that describes the exception.</param>
    /// <param name="statusCode">The HTTP status code associated with the error.</param>
    public ApiException(string message, int statusCode = 0)
        : base(message)
    {
        ApiError = new ApiError { Message = message };
        StatusCode = statusCode;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiException" /> class with an error message and an inner exception.
    /// </summary>
    /// <param name="message">The error message that describes the exception.</param>
    /// <param name="innerException">The exception that is the cause of this exception.</param>
    /// <param name="statusCode">The HTTP status code associated with the error.</param>
    public ApiException(string message, Exception innerException, int statusCode = 0)
        : base(message, innerException)
    {
        ApiError = new ApiError { Message = message };
        StatusCode = statusCode;
    }
}