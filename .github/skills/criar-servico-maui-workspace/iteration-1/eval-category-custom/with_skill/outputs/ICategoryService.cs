using CameraApp.Models;

namespace CameraApp.Services;

/// <summary>
/// Defines operations for managing <see cref="Category"/> entities via the API.
/// </summary>
public interface ICategoryService
{
    /// <summary>
    /// Retrieves a paginated list of all categories.
    /// </summary>
    /// <param name="page">The one-based page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A paginated response containing the requested categories.</returns>
    Task<PaginatedResponse<Category>> GetAllAsync(int page = 1, int pageSize = 10);

    /// <summary>
    /// Retrieves a paginated list of categories filtered by status.
    /// </summary>
    /// <param name="status">The status filter value, e.g. <c>"ativo"</c> or <c>"inativo"</c>.</param>
    /// <param name="page">The one-based page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A paginated response containing categories matching the given status.</returns>
    Task<PaginatedResponse<Category>> GetAllByStatusAsync(string status, int page = 1, int pageSize = 10);

    /// <summary>
    /// Retrieves a single category by its identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the category.</param>
    /// <returns>The matching category; <see langword="null" /> if not found.</returns>
    Task<Category?> GetByIdAsync(string id);

    /// <summary>
    /// Creates a new category on the server.
    /// </summary>
    /// <param name="entity">The category to create.</param>
    /// <returns>The created category with server-assigned values; <see langword="null" /> if creation failed.</returns>
    Task<Category?> CreateAsync(Category entity);

    /// <summary>
    /// Updates an existing category on the server.
    /// </summary>
    /// <param name="id">The unique identifier of the category to update.</param>
    /// <param name="entity">The category containing the updated values.</param>
    /// <returns>The updated category; <see langword="null" /> if the update failed.</returns>
    Task<Category?> UpdateAsync(string id, Category entity);

    /// <summary>
    /// Deletes a category from the server.
    /// </summary>
    /// <param name="id">The unique identifier of the category to delete.</param>
    /// <returns><see langword="true" /> if deletion succeeded; otherwise, <see langword="false" />.</returns>
    Task<bool> DeleteAsync(string id);
}
