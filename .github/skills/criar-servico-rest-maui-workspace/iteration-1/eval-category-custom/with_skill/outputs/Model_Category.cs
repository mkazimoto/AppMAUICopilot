using System.Text.Json.Serialization;

namespace CameraApp.Models;

/// <summary>
/// Represents a category entity.
/// </summary>
public class Category : BaseEntity
{
    /// <summary>
    /// Gets or sets the name of the category.
    /// </summary>
    /// <value>The category name. The default is an empty string.</value>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the category.
    /// </summary>
    /// <value>The category description, or <see langword="null" /> if not provided.</value>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the status of the category.
    /// </summary>
    /// <value>
    /// <c>"ativo"</c> for active categories; <c>"inativo"</c> for inactive ones.
    /// The default is an empty string.
    /// </value>
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
}
