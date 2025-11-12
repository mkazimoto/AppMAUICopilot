using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using CameraApp.Models;
using CameraApp.Config;
using CameraApp.Exceptions;

namespace CameraApp.Services;

/// <summary>
/// Implementação base genérica para serviços REST.
/// Fornece operações CRUD padrão com autenticação e tratamento de erros.
/// </summary>
/// <typeparam name="T">Tipo da entidade que herda de BaseEntity</typeparam>
public abstract class BaseService<T> : IBaseService<T> where T : BaseEntity
{
    protected readonly HttpClient _httpClient;
    protected readonly IAuthService _authService;
    protected abstract string EndpointPath { get; }

    protected BaseService(HttpClient httpClient, IAuthService authService)
    {
        _httpClient = httpClient;
        _authService = authService;
    }

    /// <summary>
    /// Obtém uma lista paginada de entidades
    /// </summary>
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
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<T>>(jsonResponse, GetJsonOptions());
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
                    var items = JsonSerializer.Deserialize<List<T>>(jsonResponse, GetJsonOptions());
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
    /// Obtém uma entidade por ID
    /// </summary>
    public virtual async Task<T?> GetByIdAsync(string id)
    {
        try
        {
            
            var url = $"{ApiConfig.BaseUrl}{EndpointPath}/{id}";
            var response = await _httpClient.GetAsync(url);
            
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(jsonResponse, GetJsonOptions());
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
    /// Cria uma nova entidade
    /// </summary>
    public virtual async Task<T?> CreateAsync(T entity)
    {
        try
        {
             
            var json = JsonSerializer.Serialize(entity, GetJsonOptions());
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync($"{ApiConfig.BaseUrl}{EndpointPath}", content);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(jsonResponse, GetJsonOptions());
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
    /// Atualiza uma entidade existente
    /// </summary>
    public virtual async Task<T?> UpdateAsync(string id, T entity)
    {
        try
        {
            
            var json = JsonSerializer.Serialize(entity, GetJsonOptions());
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var url = $"{ApiConfig.BaseUrl}{EndpointPath}/{id}";
            var response = await _httpClient.PutAsync(url, content);
            
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(jsonResponse, GetJsonOptions());
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
    /// Exclui uma entidade
    /// </summary>
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
    /// Trata respostas de erro da API
    /// </summary>
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
                    var apiError = JsonSerializer.Deserialize<ApiError>(errorContent, GetJsonOptions());
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

    /// <summary>
    /// Obtém opções de serialização JSON
    /// </summary>
    protected virtual JsonSerializerOptions GetJsonOptions()
    {
        return new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    #endregion
}
