using System.Text.Json.Serialization;

namespace CameraApp.Models;

/// <summary>
/// Represents an API error response containing a structured error code and message.
/// </summary>
public class ApiError
{
    /// <summary>
    /// Gets or sets the error code returned by the API.
    /// </summary>
    /// <value>The machine-readable error code. The default is an empty string.</value>
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the human-readable error message.
    /// </summary>
    /// <value>The short error message suitable for display. The default is an empty string.</value>
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the detailed error message with additional context.
    /// </summary>
    /// <value>The extended description of the error. The default is an empty string.</value>
    [JsonPropertyName("detailedMessage")]
    public string DetailedMessage { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the URL to the help documentation for this error.
    /// </summary>
    /// <value>The URL pointing to further information about the error. The default is an empty string.</value>
    [JsonPropertyName("helpUrl")]
    public string HelpUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets additional structured error details.
    /// </summary>
    /// <value>An optional object containing extra contextual information. The default is <see langword="null" />.</value>
    [JsonPropertyName("details")]
    public object? Details { get; set; }

    /// <summary>
    /// Returns the most informative available error message for display purposes.
    /// </summary>
    /// <returns>The <see cref="DetailedMessage" /> combined with <see cref="Message" /> when available; otherwise, <see cref="Message" /> alone.</returns>
    public string GetDisplayMessage()
    {
        if (!string.IsNullOrEmpty(DetailedMessage))
            return $"{Message}\n\nDetalhes: {DetailedMessage}";

        return Message;
    }

    /// <summary>
    /// Returns a formatted string containing the error code and message.
    /// </summary>
    /// <returns>A multi-line string with the error code and message.</returns>
    public string GetFullErrorInfo()
    {
        var info = $"Código: {Code}\nMensagem: {Message}";

        // if (!string.IsNullOrEmpty(DetailedMessage))
        //     info += $"\nDetalhes: {DetailedMessage}";

        // if (!string.IsNullOrEmpty(HelpUrl))
        //     info += $"\nAjuda: {HelpUrl}";

        return info;
    }
}