using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using CameraApp.Models;
using CameraApp.Config;
using CameraApp.Exceptions;

namespace CameraApp.Services;

/// <summary>
/// Provides a generic base implementation of REST CRUD operations with authentication and error handling.
/// </summary>
/// <typeparam name="T">The entity type, which must derive from <see cref="BaseEntity" />.</typeparam>
public abstract class BaseService<T> : IBaseService<T> where T : BaseEntity
{
    /// <summary>The HTTP client used to send requests to the API.</summary>
    protected readonly HttpClient _httpClient;
    /// <summary>The authentication service used to verify and refresh access tokens.</summary>
    protected readonly IAuthService _authService;

    /// <summary>
    /// Gets the relative API endpoint path for the entity type.
    /// </summary>
    protected abstract string EndpointPath { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseService{T}" /> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client used to communicate with the API.</param>
    /// <param name="authService">The authentication service for token management.</param>
    protected BaseService(HttpClient httpClient, IAuthService authService)
    {
        _httpClient = httpClient;
        _authService = authService;
    }

    /// <summary>
    /// Retrieves a paginated list of entities from the API.
    /// </summary>
    /// <param name="page">The one-based page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A paginated response containing the requested entities.</returns>
    /// <exception cref="Exceptions.ApiException">An error occurred while calling the API.</exception>
    public virtual async Task<PaginatedResponse<T>> GetAllAsync(int page = 1, int pageSize = 10)
    {
        try
        {

            var url = $"{ApiConfig.BaseUrl}{EndpointPath}?page={page}&pagesize={pageSize}";
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();

                // Tenta deserializar como resposta paginada do TOTVS (ApiResponse<T>)
                try
                {
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<T>>(jsonResponse, ApiConfig.GetJsonOptions());
                    if (apiResponse != null)
                    {
                        return new PaginatedResponse<T>
                        {
                            Items = apiResponse.Items ?? new List<T>(),
                            Page = page,
                            PageSize = pageSize,
                            TotalCount = apiResponse.Items?.Count ?? 0
                        };
                    }
                }
                catch
                {
                    // Se não for formato paginado, tenta como lista simples
                    var items = JsonSerializer.Deserialize<List<T>>(jsonResponse, ApiConfig.GetJsonOptions());
                    return new PaginatedResponse<T>
                    {
                        Items = items ?? new List<T>(),
                        Page = page,
                        PageSize = pageSize,
                        TotalCount = items?.Count ?? 0
                    };
                }

                return new PaginatedResponse<T>();
            }

            await HandleErrorResponseAsync(response);
            return new PaginatedResponse<T>();
        }
        catch (ApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting {typeof(T).Name} list: {ex.Message}");
            throw new ApiException($"Erro ao carregar lista de {typeof(T).Name}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Retrieves a single entity by its identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <returns>The matching entity; <see langword="null" /> if not found.</returns>
    /// <exception cref="Exceptions.ApiException">An error occurred while calling the API.</exception>
    public virtual async Task<T?> GetByIdAsync(string id)
    {
        try
        {

            var url = $"{ApiConfig.BaseUrl}{EndpointPath}/{id}";
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(jsonResponse, ApiConfig.GetJsonOptions());
            }

            await HandleErrorResponseAsync(response);
            return null;
        }
        catch (ApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting {typeof(T).Name} by id: {ex.Message}");
            throw new ApiException($"Erro ao buscar {typeof(T).Name}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Creates a new entity on the server.
    /// </summary>
    /// <param name="entity">The entity to create.</param>
    /// <returns>The created entity with server-assigned values; <see langword="null" /> if creation failed.</returns>
    /// <exception cref="Exceptions.ApiException">An error occurred while calling the API.</exception>
    public virtual async Task<T?> CreateAsync(T entity)
    {
        try
        {

            var json = JsonSerializer.Serialize(entity, ApiConfig.GetJsonOptions());
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{ApiConfig.BaseUrl}{EndpointPath}", content);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(jsonResponse, ApiConfig.GetJsonOptions());
            }

            await HandleErrorResponseAsync(response);
            return null;
        }
        catch (ApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating {typeof(T).Name}: {ex.Message}");
            throw new ApiException($"Erro ao criar {typeof(T).Name}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Updates an existing entity on the server.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to update.</param>
    /// <param name="entity">The entity containing the updated values.</param>
    /// <returns>The updated entity; <see langword="null" /> if the update failed.</returns>
    /// <exception cref="Exceptions.ApiException">An error occurred while calling the API.</exception>
    public virtual async Task<T?> UpdateAsync(string id, T entity)
    {
        try
        {

            var json = JsonSerializer.Serialize(entity, ApiConfig.GetJsonOptions());
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var url = $"{ApiConfig.BaseUrl}{EndpointPath}/{id}";
            var response = await _httpClient.PutAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(jsonResponse, ApiConfig.GetJsonOptions());
            }

            await HandleErrorResponseAsync(response);
            return null;
        }
        catch (ApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating {typeof(T).Name}: {ex.Message}");
            throw new ApiException($"Erro ao atualizar {typeof(T).Name}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Deletes an entity from the server.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to delete.</param>
    /// <returns><see langword="true" /> if the deletion succeeded; otherwise, <see langword="false" />.</returns>
    /// <exception cref="Exceptions.ApiException">An error occurred while calling the API.</exception>
    public virtual async Task<bool> DeleteAsync(string id)
    {
        try
        {

            var url = $"{ApiConfig.BaseUrl}{EndpointPath}/{id}";
            var response = await _httpClient.DeleteAsync(url);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            await HandleErrorResponseAsync(response);
            return false;
        }
        catch (ApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting {typeof(T).Name}: {ex.Message}");
            throw new ApiException($"Erro ao excluir {typeof(T).Name}: {ex.Message}", ex);
        }
    }

    #region Protected Helper Methods

    /// <summary>
    /// Processes a non-successful HTTP response and throws an <see cref="Exceptions.ApiException" /> with details.
    /// </summary>
    /// <param name="response">The non-successful HTTP response to process.</param>
    /// <exception cref="Exceptions.ApiException">Always thrown with details from the response body.</exception>
    protected virtual async Task HandleErrorResponseAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        try
        {
            var errorContent = await response.Content.ReadAsStringAsync();

            System.Diagnostics.Debug.WriteLine($"[BaseService<{typeof(T).Name}>] Error - Status: {response.StatusCode}");
            System.Diagnostics.Debug.WriteLine($"[BaseService<{typeof(T).Name}>] Error Content: {errorContent}");

            // Tenta deserializar como ApiError (formato TOTVS)
            if (!string.IsNullOrEmpty(errorContent))
            {
                try
                {
                    var apiError = JsonSerializer.Deserialize<ApiError>(errorContent, ApiConfig.GetJsonOptions());
                    if (apiError != null && !string.IsNullOrEmpty(apiError.Code))
                    {
                        throw new ApiException(apiError, (int)response.StatusCode);
                    }
                }
                catch (JsonException ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[BaseService<{typeof(T).Name}>] Erro ao deserializar ApiError: {ex.Message}");
                }
            }

            // Se não conseguiu deserializar como ApiError, cria erro genérico
            var errorMessage = $"Erro na API: {(int)response.StatusCode} - {response.ReasonPhrase}";

            if (!string.IsNullOrEmpty(errorContent))
            {
                errorMessage += $"\nDetalhes: {errorContent}";
            }

            throw new ApiException(errorMessage, (int)response.StatusCode);
        }
        catch (ApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ApiException($"Erro inesperado ao processar resposta da API: {ex.Message}", (int)response.StatusCode);
        }
    }


    #endregion
}
