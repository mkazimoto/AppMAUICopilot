namespace CameraApp.Models;

/// <summary>
/// Serves as the base class for all system entities, providing common identity and audit fields.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier of the entity.
    /// </summary>
    /// <value>The entity identifier. The default is an empty string.</value>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date and time when the record was created.
    /// </summary>
    /// <value>The creation timestamp, or <see langword="null" /> if not set.</value>
    public DateTime? RecCreatedOn { get; set; }

    /// <summary>
    /// Gets or sets the name of the user who created the record.
    /// </summary>
    /// <value>The creator's username, or <see langword="null" /> if not set.</value>
    public string? RecCreatedBy { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the record was last modified.
    /// </summary>
    /// <value>The last modification timestamp, or <see langword="null" /> if not set.</value>
    public DateTime? RecModifiedOn { get; set; }

    /// <summary>
    /// Gets or sets the name of the user who last modified the record.
    /// </summary>
    /// <value>The modifier's username, or <see langword="null" /> if not set.</value>
    public string? RecModifiedBy { get; set; }
}
