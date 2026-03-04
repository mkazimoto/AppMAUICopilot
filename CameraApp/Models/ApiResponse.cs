using System.Text.Json.Serialization;

namespace CameraApp.Models;

/// <summary>
/// Represents a generic paginated API response containing a list of items.
/// </summary>
/// <remarks>Compatible with the TOTVS API response format.</remarks>
/// <typeparam name="T">The type of entity returned in the list.</typeparam>
public class ApiResponse<T> where T : BaseEntity
{
    /// <summary>
    /// Gets or sets a value that indicates whether more items are available on the next page.
    /// </summary>
    /// <value><see langword="true" /> if a subsequent page exists; otherwise, <see langword="false" />. The default is <see langword="false" />.</value>
    [JsonPropertyName("hasNext")]
    public bool HasNext { get; set; }

    /// <summary>
    /// Gets or sets the list of items returned by the API.
    /// </summary>
    /// <value>The collection of items for the current page.</value>
    [JsonPropertyName("items")]
    public List<T> Items { get; set; } = new();

    /// <summary>
    /// Gets the number of items in the current page.
    /// </summary>
    /// <value>The count of items in <see cref="Items" />.</value>
    [JsonIgnore]
    public int Count => Items?.Count ?? 0;

    /// <summary>
    /// Gets a value that indicates whether the response contains no items.
    /// </summary>
    /// <value><see langword="true" /> if <see cref="Count" /> is zero; otherwise, <see langword="false" />.</value>
    [JsonIgnore]
    public bool IsEmpty => Count == 0;
}

/// <summary>
/// Represents a paginated API response for <see cref="Form" /> entities.
/// </summary>
/// <remarks>Maintained for backward compatibility with existing code.</remarks>
public class FormResponse : ApiResponse<Form>
{
}
