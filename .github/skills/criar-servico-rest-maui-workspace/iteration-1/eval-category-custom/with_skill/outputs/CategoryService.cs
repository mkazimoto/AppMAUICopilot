using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using CameraApp.Config;
using CameraApp.Exceptions;
using CameraApp.Models;

namespace CameraApp.Services;

/// <summary>
/// Provides operations for <see cref="Category"/> entities against the categories API endpoint.
/// </summary>
/// <remarks>
/// Uses a custom implementation (not <c>BaseService&lt;T&gt;</c>) because the API returns
/// a non-standard pagination envelope — <c>{ "data": [...], "pagination": { "total": n,
/// "page": n, "limit": n } }</c> — which differs from the TOTVS <c>ApiResponse&lt;T&gt;</c>
/// format expected by the base class.
/// </remarks>
public class CategoryService : ICategoryService
{
    private readonly HttpClient _httpClient;
    private readonly IAuthService _authService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CategoryService"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client used to communicate with the API.</param>
    /// <param name="authService">The authentication service for token management.</param>
    public CategoryService(HttpClient httpClient, IAuthService authService)
    {
        _httpClient = httpClient;
        _authService = authService;
    }

    /// <inheritdoc/>
    /// <exception cref="ApiException">An error occurred while communicating with the API.</exception>
    public async Task<PaginatedResponse<Category>> GetAllAsync(int page = 1, int pageSize = 10)
    {
        try
        {
            var url = $"{ApiConfig.BaseUrl}{ApiConfig.Endpoints.Categories}?page={page}&limit={pageSize}";
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return DeserializePagedResponse(json, page, pageSize);
            }

            await HandleErrorResponseAsync(response);
            return new PaginatedResponse<Category>();
        }
        catch (ApiException) { throw; }
        catch (Exception ex)
        {
            throw new ApiException($"Erro ao carregar categorias: {ex.Message}", ex);
        }
    }

    /// <inheritdoc/>
    /// <exception cref="ApiException">An error occurred while communicating with the API.</exception>
    public async Task<PaginatedResponse<Category>> GetAllByStatusAsync(string status, int page = 1, int pageSize = 10)
    {
        try
        {
            var url = $"{ApiConfig.BaseUrl}{ApiConfig.Endpoints.Categories}?status={Uri.EscapeDataString(status)}&page={page}&limit={pageSize}";
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return DeserializePagedResponse(json, page, pageSize);
            }

            await HandleErrorResponseAsync(response);
            return new PaginatedResponse<Category>();
        }
        catch (ApiException) { throw; }
        catch (Exception ex)
        {
            throw new ApiException($"Erro ao filtrar categorias por status '{status}': {ex.Message}", ex);
        }
    }

    /// <inheritdoc/>
    /// <exception cref="ApiException">An error occurred while communicating with the API.</exception>
    public async Task<Category?> GetByIdAsync(string id)
    {
        try
        {
            var url = $"{ApiConfig.BaseUrl}{ApiConfig.Endpoints.Categories}/{id}";
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Category>(json, ApiConfig.GetJsonOptions());
            }

            await HandleErrorResponseAsync(response);
            return null;
        }
        catch (ApiException) { throw; }
        catch (Exception ex)
        {
            throw new ApiException($"Erro ao buscar categoria '{id}': {ex.Message}", ex);
        }
    }

    /// <inheritdoc/>
    /// <exception cref="ApiException">An error occurred while communicating with the API.</exception>
    public async Task<Category?> CreateAsync(Category entity)
    {
        try
        {
            var json = JsonSerializer.Serialize(entity, ApiConfig.GetJsonOptions());
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{ApiConfig.BaseUrl}{ApiConfig.Endpoints.Categories}", content);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Category>(jsonResponse, ApiConfig.GetJsonOptions());
            }

            await HandleErrorResponseAsync(response);
            return null;
        }
        catch (ApiException) { throw; }
        catch (Exception ex)
        {
            throw new ApiException($"Erro ao criar categoria: {ex.Message}", ex);
        }
    }

    /// <inheritdoc/>
    /// <exception cref="ApiException">An error occurred while communicating with the API.</exception>
    public async Task<Category?> UpdateAsync(string id, Category entity)
    {
        try
        {
            var json = JsonSerializer.Serialize(entity, ApiConfig.GetJsonOptions());
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var url = $"{ApiConfig.BaseUrl}{ApiConfig.Endpoints.Categories}/{id}";
            var response = await _httpClient.PutAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Category>(jsonResponse, ApiConfig.GetJsonOptions());
            }

            await HandleErrorResponseAsync(response);
            return null;
        }
        catch (ApiException) { throw; }
        catch (Exception ex)
        {
            throw new ApiException($"Erro ao atualizar categoria '{id}': {ex.Message}", ex);
        }
    }

    /// <inheritdoc/>
    /// <exception cref="ApiException">An error occurred while communicating with the API.</exception>
    public async Task<bool> DeleteAsync(string id)
    {
        try
        {
            var url = $"{ApiConfig.BaseUrl}{ApiConfig.Endpoints.Categories}/{id}";
            var response = await _httpClient.DeleteAsync(url);

            if (response.IsSuccessStatusCode)
                return true;

            await HandleErrorResponseAsync(response);
            return false;
        }
        catch (ApiException) { throw; }
        catch (Exception ex)
        {
            throw new ApiException($"Erro ao excluir categoria '{id}': {ex.Message}", ex);
        }
    }

    // ---------------------------------------------------------------------------
    // Private helpers
    // ---------------------------------------------------------------------------

    /// <summary>
    /// Deserializes the non-standard paginated API envelope into a <see cref="PaginatedResponse{T}"/>.
    /// </summary>
    /// <remarks>
    /// Expected envelope: <c>{ "data": [...], "pagination": { "total": n, "page": n, "limit": n } }</c>.
    /// Adjust <see cref="CategoryApiPagedResponse"/> fields if the real API shape differs.
    /// </remarks>
    private static PaginatedResponse<Category> DeserializePagedResponse(string json, int page, int pageSize)
    {
        var envelope = JsonSerializer.Deserialize<CategoryApiPagedResponse>(json, ApiConfig.GetJsonOptions());
        if (envelope is null)
            return new PaginatedResponse<Category>();

        return new PaginatedResponse<Category>
        {
            Items      = envelope.Data      ?? new List<Category>(),
            Page       = envelope.Pagination?.Page    ?? page,
            PageSize   = envelope.Pagination?.Limit   ?? pageSize,
            TotalCount = envelope.Pagination?.Total   ?? envelope.Data?.Count ?? 0
        };
    }

    private async Task HandleErrorResponseAsync(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        var apiError = string.IsNullOrEmpty(content)
            ? null
            : JsonSerializer.Deserialize<ApiError>(content, ApiConfig.GetJsonOptions());
        throw new ApiException(
            apiError?.Message ?? $"Erro HTTP {(int)response.StatusCode}",
            (int)response.StatusCode);
    }

    // ---------------------------------------------------------------------------
    // Private DTOs — non-standard pagination envelope
    // ---------------------------------------------------------------------------

    /// <summary>
    /// Maps the non-standard API pagination envelope returned by <c>/api/v1/categories</c>.
    /// </summary>
    /// <remarks>
    /// TODO: Adjust property names/types to match the actual API contract once confirmed.
    /// Expected JSON shape:
    /// <code>
    /// {
    ///   "data": [ { ... } ],
    ///   "pagination": { "total": 50, "page": 1, "limit": 10 }
    /// }
    /// </code>
    /// </remarks>
    private sealed class CategoryApiPagedResponse
    {
        /// <summary>Gets or sets the list of category items in the current page.</summary>
        [JsonPropertyName("data")]
        public List<Category>? Data { get; set; }

        /// <summary>Gets or sets the pagination metadata block.</summary>
        [JsonPropertyName("pagination")]
        public CategoryApiPagination? Pagination { get; set; }
    }

    /// <summary>
    /// Represents the pagination metadata block in the non-standard API response.
    /// </summary>
    private sealed class CategoryApiPagination
    {
        /// <summary>Gets or sets the total number of records across all pages.</summary>
        [JsonPropertyName("total")]
        public int Total { get; set; }

        /// <summary>Gets or sets the current page number (one-based).</summary>
        [JsonPropertyName("page")]
        public int Page { get; set; }

        /// <summary>Gets or sets the maximum number of items per page.</summary>
        [JsonPropertyName("limit")]
        public int Limit { get; set; }
    }
}
