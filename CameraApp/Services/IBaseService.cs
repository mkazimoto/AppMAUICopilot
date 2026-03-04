using CameraApp.Models;

namespace CameraApp.Services;

/// <summary>
/// Defines generic CRUD operations for a REST service operating on a specific entity type.
/// </summary>
/// <typeparam name="T">The entity type, which must derive from <see cref="BaseEntity" />.</typeparam>
public interface IBaseService<T> where T : BaseEntity
{
    /// <summary>
    /// Retrieves a paginated list of entities.
    /// </summary>
    /// <param name="page">The one-based page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A paginated response containing the requested entities and pagination metadata.</returns>
    Task<PaginatedResponse<T>> GetAllAsync(int page = 1, int pageSize = 10);

    /// <summary>
    /// Retrieves a single entity by its identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <returns>The matching entity; <see langword="null" /> if not found.</returns>
    Task<T?> GetByIdAsync(string id);

    /// <summary>
    /// Creates a new entity on the server.
    /// </summary>
    /// <param name="entity">The entity to create.</param>
    /// <returns>The created entity with server-assigned values; <see langword="null" /> if creation failed.</returns>
    Task<T?> CreateAsync(T entity);

    /// <summary>
    /// Updates an existing entity on the server.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to update.</param>
    /// <param name="entity">The entity containing the updated values.</param>
    /// <returns>The updated entity; <see langword="null" /> if the update failed.</returns>
    Task<T?> UpdateAsync(string id, T entity);

    /// <summary>
    /// Deletes an entity from the server.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to delete.</param>
    /// <returns><see langword="true" /> if the deletion succeeded; otherwise, <see langword="false" />.</returns>
    Task<bool> DeleteAsync(string id);
}

/// <summary>
/// Represents a paginated API response for a specific entity type.
/// </summary>
/// <typeparam name="T">The entity type, which must derive from <see cref="BaseEntity" />.</typeparam>
public class PaginatedResponse<T> where T : BaseEntity
{
    /// <summary>
    /// Gets or sets the items on the current page.
    /// </summary>
    /// <value>The collection of entities for the current page.</value>
    public List<T> Items { get; set; } = new();

    /// <summary>
    /// Gets or sets the current page number.
    /// </summary>
    /// <value>The one-based page number.</value>
    public int Page { get; set; }

    /// <summary>
    /// Gets or sets the number of items per page.
    /// </summary>
    /// <value>The page size.</value>
    public int PageSize { get; set; }

    /// <summary>
    /// Gets or sets the total number of items available across all pages.
    /// </summary>
    /// <value>The total item count.</value>
    public int TotalCount { get; set; }

    /// <summary>
    /// Gets the total number of pages available.
    /// </summary>
    /// <value>The page count calculated from <see cref="TotalCount" /> and <see cref="PageSize" />.</value>
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;

    /// <summary>
    /// Gets a value that indicates whether a previous page exists.
    /// </summary>
    /// <value><see langword="true" /> if <see cref="Page" /> is greater than <c>1</c>; otherwise, <see langword="false" />.</value>
    public bool HasPreviousPage => Page > 1;

    /// <summary>
    /// Gets a value that indicates whether a next page exists.
    /// </summary>
    /// <value><see langword="true" /> if <see cref="Page" /> is less than <see cref="TotalPages" />; otherwise, <see langword="false" />.</value>
    public bool HasNextPage => Page < TotalPages;
}
