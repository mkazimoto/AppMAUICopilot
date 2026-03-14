using System.Text.Json;
using CameraApp.Config;
using CameraApp.Exceptions;
using CameraApp.Models;

namespace CameraApp.Services;

/// <summary>
/// Provides CRUD operations and status-based filtering for <see cref="Category" /> entities
/// against the <c>/api/v1/categories</c> endpoint, adapting its non-standard pagination format.
/// </summary>
public class CategoryService : BaseService<Category>, ICategoryService
{
    /// <inheritdoc/>
    protected override string EndpointPath => ApiConfig.Endpoints.Categories;

    /// <summary>
    /// Initializes a new instance of the <see cref="CategoryService" /> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client used to communicate with the API.</param>
    /// <param name="authService">The authentication service for token management.</param>
    public CategoryService(HttpClient httpClient, IAuthService authService)
        : base(httpClient, authService)
    {
    }

    /// <summary>
    /// Retrieves a paginated list of categories, deserializing the non-standard API envelope
    /// (<c>data</c>, <c>total</c>, <c>page</c>, <c>pageSize</c>) and mapping it to
    /// <see cref="PaginatedResponse{T}" />.
    /// </summary>
    /// <inheritdoc/>
    public override async Task<PaginatedResponse<Category>> GetAllAsync(int page = 1, int pageSize = 10)
    {
        try
        {
            var url = $"{ApiConfig.BaseUrl}{EndpointPath}?page={page}&pageSize={pageSize}";
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var paged = JsonSerializer.Deserialize<CategoryPagedResponse>(json, ApiConfig.GetJsonOptions());

                if (paged != null)
                {
                    return new PaginatedResponse<Category>
                    {
                        Items = paged.Data,
                        Page = paged.Page,
                        PageSize = paged.PageSize,
                        TotalCount = paged.Total
                    };
                }
            }

            await HandleErrorResponseAsync(response);
            return new PaginatedResponse<Category>();
        }
        catch (ApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting Category list: {ex.Message}");
            throw new ApiException($"Erro ao carregar categorias: {ex.Message}", ex);
        }
    }

    /// <inheritdoc/>
    public async Task<CategoryPagedResponse> GetByStatusAsync(
        CategoryStatus status,
        int page = 1,
        int pageSize = 10)
    {
        try
        {
            var statusParam = status == CategoryStatus.Active ? "active" : "inactive";
            var url = $"{ApiConfig.BaseUrl}{EndpointPath}?status={statusParam}&page={page}&pageSize={pageSize}";

            System.Diagnostics.Debug.WriteLine($"[CategoryService] GetByStatusAsync URL: {url}");

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<CategoryPagedResponse>(json, ApiConfig.GetJsonOptions())
                    ?? new CategoryPagedResponse();
            }

            await HandleErrorResponseAsync(response);
            return new CategoryPagedResponse();
        }
        catch (ApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting categories by status: {ex.Message}");
            throw new ApiException($"Erro ao filtrar categorias por status: {ex.Message}", ex);
        }
    }
}
