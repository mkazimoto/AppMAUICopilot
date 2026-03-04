using System.Text.Json.Serialization;

namespace CameraApp.Models;

/// <summary>
/// Represents a form entity in the system.
/// </summary>
public class Form : BaseEntity
{
    /// <summary>
    /// Gets or sets the numeric identifier of the form.
    /// </summary>
    /// <value>The form identifier assigned by the API.</value>
    [JsonPropertyName("formId")]
    public int FormId { get; set; }

    /// <summary>
    /// Gets or sets the display title of the form.
    /// </summary>
    /// <value>The form title. The default is an empty string.</value>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the identifier of the category this form belongs to.
    /// </summary>
    /// <value>The category identifier.</value>
    [JsonPropertyName("categoryId")]
    public int CategoryId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the form's current status.
    /// </summary>
    /// <value>The status identifier.</value>
    [JsonPropertyName("statusFormId")]
    public int StatusFormId { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates whether the form uses a sequential script.
    /// </summary>
    /// <value><see langword="true" /> if questions must be answered in sequence; otherwise, <see langword="false" />. The default is <see langword="false" />.</value>
    [JsonPropertyName("sequentialScript")]
    public bool SequentialScript { get; set; }

    /// <summary>
    /// Gets or sets the total achievable score for the form.
    /// </summary>
    /// <value>The maximum score value.</value>
    [JsonPropertyName("totalScore")]
    public int TotalScore { get; set; }
}