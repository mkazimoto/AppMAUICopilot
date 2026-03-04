namespace CameraApp.Services;

/// <summary>
/// Defines operations for capturing and selecting photos using the device camera or media library.
/// </summary>
public interface ICameraService
{
    /// <summary>
    /// Launches the device camera to capture a new photo.
    /// </summary>
    /// <returns>The local file path of the captured photo; <see langword="null" /> if the capture was cancelled or failed.</returns>
    Task<string?> TakePhotoAsync();

    /// <summary>
    /// Opens the device photo picker to select an existing photo.
    /// </summary>
    /// <returns>The local file path of the selected photo; <see langword="null" /> if the selection was cancelled or failed.</returns>
    Task<string?> PickPhotoAsync();
}