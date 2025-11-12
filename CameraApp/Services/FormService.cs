using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using CameraApp.Models;
using CameraApp.Config;
using CameraApp.Exceptions;

namespace CameraApp.Services;

public class FormService : IFormService
{
    private readonly HttpClient _httpClient;
    private readonly IAuthService _authService;

    public FormService(HttpClient httpClient, IAuthService authService)
    {
        _httpClient = httpClient;
        _authService = authService;
    }

    public async Task<FormResponse> GetFormsAsync(FormFilter filter)
    {
        try
        {
                       
            var queryString = filter.BuildQueryString();
            var url = $"{ApiConfig.BaseUrl}{ApiConfig.Endpoints.Forms}?{queryString}";
            
            System.Diagnostics.Debug.WriteLine($"[FormService] GetFormsAsync URL: {url}");
            System.Diagnostics.Debug.WriteLine($"[FormService] CategoryId: {filter.CategoryId}, StatusFormId: {filter.StatusFormId}");
            
            var response = await _httpClient.GetAsync(url);
            
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<FormResponse>(jsonResponse, GetJsonOptions()) ?? new FormResponse();
            }
            
            await HandleErrorResponseAsync(response);
            return new FormResponse();
        }
        catch (ApiException)
        {
            // Re-throw API exceptions para que possam ser tratadas pelos ViewModels
            throw;
        }
        catch (Exception ex)
        {
            // Log error e converte para ApiException
            Console.WriteLine($"Error getting forms: {ex.Message}");
            throw new ApiException($"Erro inesperado ao carregar formulários: {ex.Message}", ex);
        }
    }

    public async Task<Form?> GetFormByIdAsync(string id)
    {
        try
        {
            
            var url = $"{ApiConfig.BaseUrl}{ApiConfig.Endpoints.Forms}/{id}";
            var response = await _httpClient.GetAsync(url);
            
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Form>(jsonResponse, GetJsonOptions());
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
            Console.WriteLine($"Error getting form by id: {ex.Message}");
            throw new ApiException($"Erro ao buscar formulário: {ex.Message}", ex);
        }
    }

    public async Task<Form?> CreateFormAsync(Form form)
    {
        try
        {
            
            var json = JsonSerializer.Serialize(form, GetJsonOptions());
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync($"{ApiConfig.BaseUrl}{ApiConfig.Endpoints.Forms}", content);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Form>(jsonResponse, GetJsonOptions());
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
            Console.WriteLine($"Error creating form: {ex.Message}");
            throw new ApiException($"Erro ao criar formulário: {ex.Message}", ex);
        }
    }

    public async Task<Form?> UpdateFormAsync(string id, Form form)
    {
        try
        {
            
            var json = JsonSerializer.Serialize(form, GetJsonOptions());
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var url = $"{ApiConfig.BaseUrl}{ApiConfig.Endpoints.Forms}/{id}";
            var response = await _httpClient.PutAsync(url, content);
            
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Form>(jsonResponse, GetJsonOptions());
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
            Console.WriteLine($"Error updating form: {ex.Message}");
            throw new ApiException($"Erro ao atualizar formulário: {ex.Message}", ex);
        }
    }

    public async Task<bool> DeleteFormAsync(string id)
    {
        try
        {
             
            var url = $"{ApiConfig.BaseUrl}{ApiConfig.Endpoints.Forms}/{id}";
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
            Console.WriteLine($"Error deleting form: {ex.Message}");
            throw new ApiException($"Erro ao excluir formulário: {ex.Message}", ex);
        }
    }


    private static JsonSerializerOptions GetJsonOptions()
    {
        return new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    private async Task HandleErrorResponseAsync(HttpResponseMessage response)
    {
        // Se a resposta foi bem-sucedida, não há erro para tratar
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        try
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            
            System.Diagnostics.Debug.WriteLine($"[FormService] HandleErrorResponseAsync - Status: {response.StatusCode}");
            System.Diagnostics.Debug.WriteLine($"[FormService] Error Content: {errorContent}");
            
            // Tenta deserializar como ApiError (formato TOTVS)
            if (!string.IsNullOrEmpty(errorContent))
            {
                try
                {
                    var apiError = JsonSerializer.Deserialize<ApiError>(errorContent, GetJsonOptions());
                    if (apiError != null && !string.IsNullOrEmpty(apiError.Code))
                    {                        
                        // Para outros erros, lança a ApiException com os detalhes
                        throw new ApiException(apiError, (int)response.StatusCode);
                    }
                }
                catch (JsonException ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[FormService] Erro ao deserializar ApiError: {ex.Message}");
                    // Continua para o tratamento genérico abaixo
                }
            }
            
            // Se não conseguiu deserializar como ApiError, cria erro genérico
            var errorMessage = $"Erro na API: {(int)response.StatusCode} - {response.ReasonPhrase}";
            
            if (!string.IsNullOrEmpty(errorContent))
            {
                errorMessage += $"\nDetalhes: {errorContent}";
            }
            
            System.Diagnostics.Debug.WriteLine($"[FormService] Lançando ApiException genérica: {errorMessage}");
            throw new ApiException(errorMessage, (int)response.StatusCode);
        }
        catch (ApiException)
        {
            // Re-throw ApiException para que seja tratada pelos ViewModels
            throw;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[FormService] Erro inesperado em HandleErrorResponseAsync: {ex.Message}");
            throw new ApiException($"Erro inesperado ao processar resposta da API: {ex.Message}", (int)response.StatusCode);
        }
    }
}