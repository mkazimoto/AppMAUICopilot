using CameraApp.Models;

namespace CameraApp.Services;

/// <summary>
/// Defines CRUD operations and status-based filtering for <see cref="Category" /> entities.
/// </summary>
public interface ICategoryService : IBaseService<Category>
{
    /// <summary>
    /// Retrieves a paginated list of categories filtered by their active/inactive status.
    /// </summary>
    /// <param name="status">The <see cref="CategoryStatus" /> value used to filter results.</param>
    /// <param name="page">The one-based page number to retrieve. Default is <c>1</c>.</param>
    /// <param name="pageSize">The number of items per page. Default is <c>10</c>.</param>
    /// <returns>
    /// A <see cref="CategoryPagedResponse" /> containing the filtered categories and pagination metadata.
    /// </returns>
    /// <exception cref="CameraApp.Exceptions.ApiException">An error occurred while calling the API.</exception>
    Task<CategoryPagedResponse> GetByStatusAsync(CategoryStatus status, int page = 1, int pageSize = 10);
}
