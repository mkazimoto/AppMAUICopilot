using CameraApp.Config;

namespace CameraApp.Services;

/// <summary>
/// Provides generic CRUD operations for <see cref="Models.Form" /> entities, inheriting all base REST behavior.
/// </summary>
public class FormServiceGeneric : BaseService<Models.Form>
{
    /// <summary>
    /// Gets the relative API endpoint path for forms.
    /// </summary>
    protected override string EndpointPath => "/api/forms";

    /// <summary>
    /// Initializes a new instance of the <see cref="FormServiceGeneric" /> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client used to communicate with the API.</param>
    /// <param name="authService">The authentication service for token management.</param>
    public FormServiceGeneric(HttpClient httpClient, IAuthService authService)
        : base(httpClient, authService)
    {
    }

    // Métodos específicos de formulário podem ser adicionados aqui
    // Por exemplo:

    /// <summary>
    /// Retrieves a paginated list of forms belonging to a specific category.
    /// </summary>
    /// <param name="categoryId">The category identifier to filter by.</param>
    /// <param name="page">The one-based page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A paginated response containing forms in the specified category.</returns>
    /// <exception cref="Exceptions.ApiException">An error occurred while calling the API.</exception>
    public async Task<PaginatedResponse<Models.Form>> GetByCategoryAsync(int categoryId, int page = 1, int pageSize = 10)
    {
        try
        {
            var url = $"{Config.ApiConfig.BaseUrl}{EndpointPath}?$filter=categoryId eq {categoryId}&page={page}&pagesize={pageSize}";
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var apiResponse = System.Text.Json.JsonSerializer.Deserialize<Models.ApiResponse<Models.Form>>(jsonResponse, ApiConfig.GetJsonOptions());

                return new PaginatedResponse<Models.Form>
                {
                    Items = apiResponse?.Items ?? new List<Models.Form>(),
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = apiResponse?.Items?.Count ?? 0
                };
            }

            await HandleErrorResponseAsync(response);
            return new PaginatedResponse<Models.Form>();
        }
        catch (Exceptions.ApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exceptions.ApiException($"Erro ao filtrar formulários por categoria: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Obtém formulários por status
    /// </summary>
    public async Task<PaginatedResponse<Models.Form>> GetByStatusAsync(int statusId, int page = 1, int pageSize = 10)
    {
        try
        {
            var url = $"{Config.ApiConfig.BaseUrl}{EndpointPath}?$filter=statusFormId eq {statusId}&page={page}&pagesize={pageSize}";
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var apiResponse = System.Text.Json.JsonSerializer.Deserialize<Models.ApiResponse<Models.Form>>(jsonResponse, ApiConfig.GetJsonOptions());

                return new PaginatedResponse<Models.Form>
                {
                    Items = apiResponse?.Items ?? new List<Models.Form>(),
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = apiResponse?.Items?.Count ?? 0
                };
            }

            await HandleErrorResponseAsync(response);
            return new PaginatedResponse<Models.Form>();
        }
        catch (Exceptions.ApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exceptions.ApiException($"Erro ao filtrar formulários por status: {ex.Message}", ex);
        }
    }
}
