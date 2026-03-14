using System.Text.Json.Serialization;

namespace CameraApp.Models;

/// <summary>
/// Represents a product entity in the system.
/// </summary>
public class Product : BaseEntity
{
    /// <summary>
    /// Gets or sets the display name of the product.
    /// </summary>
    /// <value>The product name. The default is an empty string.</value>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the product.
    /// </summary>
    /// <value>The product description, or <see langword="null" /> if not provided.</value>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the unit price of the product.
    /// </summary>
    /// <value>The price in the system's default currency.</value>
    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    /// <summary>
    /// Gets or sets the available stock quantity.
    /// </summary>
    /// <value>The number of units in stock.</value>
    [JsonPropertyName("stock")]
    public int Stock { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the product is active.
    /// </summary>
    /// <value><see langword="true" /> if the product is active; otherwise, <see langword="false" />.</value>
    [JsonPropertyName("active")]
    public bool Active { get; set; } = true;
}
