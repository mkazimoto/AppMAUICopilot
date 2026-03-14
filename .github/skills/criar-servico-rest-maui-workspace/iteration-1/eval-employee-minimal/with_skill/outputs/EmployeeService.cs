namespace CameraApp.Services;

/// <summary>
/// Provides CRUD operations for <see cref="Models.Employee"/> entities.
/// </summary>
public class EmployeeService : BaseService<Models.Employee>, IEmployeeService
{
    /// <inheritdoc/>
    protected override string EndpointPath => Config.ApiConfig.Endpoints.Employees;

    /// <summary>
    /// Initializes a new instance of <see cref="EmployeeService"/>.
    /// </summary>
    public EmployeeService(HttpClient httpClient, IAuthService authService)
        : base(httpClient, authService) { }

    // Adicione métodos específicos aqui se necessário
}
