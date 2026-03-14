using System.Text.Json;
using CameraApp.Config;
using CameraApp.Exceptions;
using CameraApp.Models;

namespace CameraApp.Services;

/// <summary>
/// Provides CRUD operations for <see cref="Employee" /> entities against the employees API endpoint.
/// </summary>
public class EmployeeService : BaseService<Employee>, IEmployeeService
{
    /// <inheritdoc/>
    protected override string EndpointPath => ApiConfig.Endpoints.Employees;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmployeeService" /> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client used to communicate with the API.</param>
    /// <param name="authService">The authentication service for token management.</param>
    public EmployeeService(HttpClient httpClient, IAuthService authService)
        : base(httpClient, authService)
    {
    }
}
