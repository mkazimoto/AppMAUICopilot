using System.Text.Json.Serialization;

namespace CameraApp.Models;

/// <summary>
/// Represents the active/inactive status of a category.
/// </summary>
public enum CategoryStatus
{
    /// <summary>The category is active and available for use.</summary>
    Active,

    /// <summary>The category is inactive and not available for use.</summary>
    Inactive
}

/// <summary>
/// Represents a category entity returned by the <c>/api/v1/categories</c> endpoint.
/// </summary>
public class Category : BaseEntity
{
    /// <summary>
    /// Gets or sets the display name of the category.
    /// </summary>
    /// <value>The category name. The default is an empty string.</value>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional description of the category.
    /// </summary>
    /// <value>The description, or <see langword="null" /> if not provided.</value>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the status of the category.
    /// </summary>
    /// <value>Either <see cref="CategoryStatus.Active" /> or <see cref="CategoryStatus.Inactive" />. The default is <see cref="CategoryStatus.Active" />.</value>
    [JsonPropertyName("status")]
    public CategoryStatus Status { get; set; } = CategoryStatus.Active;
}

/// <summary>
/// Represents the non-standard paginated response returned by the <c>/api/v1/categories</c> endpoint.
/// </summary>
/// <remarks>
/// The categories endpoint uses a custom envelope with <c>data</c>, <c>total</c>, <c>page</c>, and
/// <c>pageSize</c> fields instead of the standard <c>items</c> / <c>hasNext</c> format used by other
/// TOTVS endpoints.
/// </remarks>
public class CategoryPagedResponse
{
    /// <summary>
    /// Gets or sets the list of categories on the current page.
    /// </summary>
    /// <value>The collection of <see cref="Category" /> items for this page.</value>
    [JsonPropertyName("data")]
    public List<Category> Data { get; set; } = new();

    /// <summary>
    /// Gets or sets the total number of records matching the query across all pages.
    /// </summary>
    /// <value>The total record count.</value>
    [JsonPropertyName("total")]
    public int Total { get; set; }

    /// <summary>
    /// Gets or sets the current page number (1-based).
    /// </summary>
    /// <value>The one-based current page index.</value>
    [JsonPropertyName("page")]
    public int Page { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of items per page used for this request.
    /// </summary>
    /// <value>The page size.</value>
    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; }

    /// <summary>
    /// Gets the total number of pages available.
    /// </summary>
    /// <value>Calculated from <see cref="Total" /> and <see cref="PageSize" />.</value>
    [JsonIgnore]
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)Total / PageSize) : 0;

    /// <summary>
    /// Gets a value indicating whether a next page exists.
    /// </summary>
    /// <value><see langword="true" /> if <see cref="Page" /> is less than <see cref="TotalPages" />; otherwise, <see langword="false" />.</value>
    [JsonIgnore]
    public bool HasNext => Page < TotalPages;
}
