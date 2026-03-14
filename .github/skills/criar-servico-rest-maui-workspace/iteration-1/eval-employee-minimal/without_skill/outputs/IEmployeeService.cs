using CameraApp.Models;

namespace CameraApp.Services;

/// <summary>
/// Defines CRUD operations for managing <see cref="Employee" /> entities via the API.
/// </summary>
public interface IEmployeeService
{
    /// <summary>
    /// Retrieves a paginated list of employees.
    /// </summary>
    /// <param name="page">The one-based page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A paginated response containing the requested employees.</returns>
    Task<PaginatedResponse<Employee>> GetAllAsync(int page = 1, int pageSize = 10);

    /// <summary>
    /// Retrieves a single employee by their identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the employee.</param>
    /// <returns>The matching employee; <see langword="null" /> if not found.</returns>
    Task<Employee?> GetByIdAsync(string id);

    /// <summary>
    /// Creates a new employee on the server.
    /// </summary>
    /// <param name="employee">The employee data to create.</param>
    /// <returns>The created employee with server-assigned values; <see langword="null" /> if creation failed.</returns>
    Task<Employee?> CreateAsync(Employee employee);

    /// <summary>
    /// Updates an existing employee on the server.
    /// </summary>
    /// <param name="id">The unique identifier of the employee to update.</param>
    /// <param name="employee">The updated employee data.</param>
    /// <returns>The updated employee; <see langword="null" /> if the update failed.</returns>
    Task<Employee?> UpdateAsync(string id, Employee employee);

    /// <summary>
    /// Deletes an employee from the server.
    /// </summary>
    /// <param name="id">The unique identifier of the employee to delete.</param>
    /// <returns><see langword="true" /> if the deletion succeeded; otherwise, <see langword="false" />.</returns>
    Task<bool> DeleteAsync(string id);
}
