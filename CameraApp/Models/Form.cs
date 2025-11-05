using System.Text.Json.Serialization;

namespace CameraApp.Models;

public class Form
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

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

    [JsonPropertyName("recCreatedBy")]
    public string RecCreatedBy { get; set; } = string.Empty;

    [JsonPropertyName("recCreatedOn")]
    public DateTime RecCreatedOn { get; set; }

    [JsonPropertyName("recModifiedBy")]
    public string RecModifiedBy { get; set; } = string.Empty;

    [JsonPropertyName("recModifiedOn")]
    public DateTime RecModifiedOn { get; set; }
}

public class FormResponse
{
    [JsonPropertyName("hasNext")]
    public bool HasNext { get; set; }

    [JsonPropertyName("items")]
    public List<Form> Items { get; set; } = new();
}