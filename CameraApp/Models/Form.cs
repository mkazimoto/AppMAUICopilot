using System.Text.Json.Serialization;

namespace CameraApp.Models;

/// <summary>
/// Representa um formulário no sistema
/// </summary>
public class Form : BaseEntity
{
    [JsonPropertyName("formId")]
    public int FormId { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("categoryId")]
    public int CategoryId { get; set; }

    [JsonPropertyName("statusFormId")]
    public int StatusFormId { get; set; }

    [JsonPropertyName("sequentialScript")]
    public bool SequentialScript { get; set; }

    [JsonPropertyName("totalScore")]
    public int TotalScore { get; set; }
}