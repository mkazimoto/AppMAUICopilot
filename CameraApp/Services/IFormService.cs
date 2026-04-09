using CameraApp.Models;

namespace CameraApp.Services;

/// <summary>
/// Defines CRUD operations for managing <see cref="Form" /> entities via the API.
/// </summary>
public interface IFormService : IBaseService<Form>
{
    Task<FormResponse> GetFormsAsync(FormFilter filter);
    Task<Form?> GetFormByIdAsync(string id);
    Task<Form?> CreateFormAsync(Form form);
    Task<Form?> UpdateFormAsync(string id, Form form);
    Task<bool> DeleteFormAsync(string id);
}