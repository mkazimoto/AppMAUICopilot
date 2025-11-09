using CameraApp.Models;

namespace CameraApp.Services;

public interface IFormService
{
    /// <summary>
    /// Obtém formulários com filtros e paginação
    /// </summary>
    Task<FormResponse> GetFormsAsync(FormFilter filter);
    
    Task<Form?> GetFormByIdAsync(string id);
    Task<Form?> CreateFormAsync(Form form);
    Task<Form?> UpdateFormAsync(string id, Form form);
    Task<bool> DeleteFormAsync(string id);
}