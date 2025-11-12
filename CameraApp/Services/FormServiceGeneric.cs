using CameraApp.Config;

namespace CameraApp.Services;

/// <summary>
/// Serviço genérico para operações com formulários.
/// Herda todas as operações CRUD da classe BaseService.
/// </summary>
public class FormServiceGeneric : BaseService<Models.Form>
{
    /// <summary>
    /// Define o endpoint da API para formulários
    /// </summary>
    protected override string EndpointPath => "/api/forms";

    public FormServiceGeneric(HttpClient httpClient, IAuthService authService) 
        : base(httpClient, authService)
    {
    }

    // Métodos específicos de formulário podem ser adicionados aqui
    // Por exemplo:
    
    /// <summary>
    /// Obtém formulários por categoria
    /// </summary>
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
