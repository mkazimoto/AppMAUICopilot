using System.Text.Json.Serialization;

namespace CameraApp.Models;

public class ApiError
{
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("detailedMessage")]
    public string DetailedMessage { get; set; } = string.Empty;

    [JsonPropertyName("helpUrl")]
    public string HelpUrl { get; set; } = string.Empty;

    [JsonPropertyName("details")]
    public object? Details { get; set; }

    public string GetDisplayMessage()
    {
        if (!string.IsNullOrEmpty(DetailedMessage))
            return $"{Message}\n\nDetalhes: {DetailedMessage}";
        
        return Message;
    }

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