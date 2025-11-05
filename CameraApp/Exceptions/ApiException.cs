using CameraApp.Models;

namespace CameraApp.Exceptions;

public class ApiException : Exception
{
    public ApiError ApiError { get; }
    public int StatusCode { get; }

    public ApiException(ApiError apiError, int statusCode = 0) 
        : base(apiError.GetDisplayMessage())
    {
        ApiError = apiError;
        StatusCode = statusCode;
    }

    public ApiException(string message, int statusCode = 0) 
        : base(message)
    {
        ApiError = new ApiError { Message = message };
        StatusCode = statusCode;
    }

    public ApiException(string message, Exception innerException, int statusCode = 0) 
        : base(message, innerException)
    {
        ApiError = new ApiError { Message = message };
        StatusCode = statusCode;
    }
}