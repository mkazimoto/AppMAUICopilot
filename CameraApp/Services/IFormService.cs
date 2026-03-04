using CameraApp.Models;

namespace CameraApp.Services;

/// <summary>
/// Defines CRUD operations for managing <see cref="Form" /> entities via the API.
/// </summary>
public interface IFormService
{
    /// <summary>
    /// Retrieves a filtered, paginated list of forms.
    /// </summary>
    /// <param name="filter">The filter and pagination criteria to apply.</param>
    /// <returns>A paginated response containing matching forms.</returns>
    Task<FormResponse> GetFormsAsync(FormFilter filter);

    /// <summary>
    /// Retrieves a single form by its identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the form.</param>
    /// <returns>The matching form; <see langword="null" /> if not found.</returns>
    Task<Form?> GetFormByIdAsync(string id);

    /// <summary>
    /// Creates a new form on the server.
    /// </summary>
    /// <param name="form">The form data to create.</param>
    /// <returns>The created form with server-assigned values; <see langword="null" /> if creation failed.</returns>
    Task<Form?> CreateFormAsync(Form form);

    /// <summary>
    /// Updates an existing form on the server.
    /// </summary>
    /// <param name="id">The unique identifier of the form to update.</param>
    /// <param name="form">The updated form data.</param>
    /// <returns>The updated form; <see langword="null" /> if the update failed.</returns>
    Task<Form?> UpdateFormAsync(string id, Form form);

    /// <summary>
    /// Deletes a form from the server.
    /// </summary>
    /// <param name="id">The unique identifier of the form to delete.</param>
    /// <returns><see langword="true" /> if the deletion succeeded; otherwise, <see langword="false" />.</returns>
    Task<bool> DeleteFormAsync(string id);
}