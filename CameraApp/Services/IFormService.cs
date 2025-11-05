using CameraApp.Models;

namespace CameraApp.Services;

public interface IFormService
{
    Task<FormResponse> GetFormsAsync(int page = 1, int pageSize = 10);
    Task<FormResponse> GetFormsByCategoryAsync(int categoryId, int page = 1, int pageSize = 10);
    Task<FormResponse> GetFormsByStatusAsync(int statusId, int page = 1, int pageSize = 10);
    Task<FormResponse> GetFormsByDateRangeAsync(DateTime startDate, DateTime endDate, int page = 1, int pageSize = 10);
    Task<Form?> GetFormByIdAsync(string id);
    Task<Form?> CreateFormAsync(Form form);
    Task<Form?> UpdateFormAsync(string id, Form form);
    Task<bool> DeleteFormAsync(string id);
}