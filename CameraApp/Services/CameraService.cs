using Microsoft.Maui.Media;

namespace CameraApp.Services;

/// <summary>
/// Provides camera and photo picker operations backed by the MAUI <see cref="MediaPicker" />.
/// </summary>
public class CameraService : ICameraService
{
    /// <summary>
    /// Launches the device camera to capture a new photo and saves it to the local cache directory.
    /// </summary>
    /// <returns>The local file path of the captured photo; <see langword="null" /> if the capture was cancelled, the device does not support capture, or an error occurred.</returns>
    public async Task<string?> TakePhotoAsync()
    {
        try
        {
            if (MediaPicker.Default.IsCaptureSupported)
            {
                var photo = await MediaPicker.Default.CapturePhotoAsync();

                if (photo != null)
                {
                    // Salvar em local storage
                    var localFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);

                    using var sourceStream = await photo.OpenReadAsync();
                    using var localFileStream = File.OpenWrite(localFilePath);

                    await sourceStream.CopyToAsync(localFileStream);

                    return localFilePath;
                }
            }
            else
            {
                var mainPage = Application.Current?.Windows?.FirstOrDefault()?.Page;
                if (mainPage != null)
                    await mainPage.DisplayAlertAsync("Erro", "Câmera não suportada neste dispositivo", "OK");
            }
        }
        catch (Exception ex)
        {
            var mainPage = Application.Current?.Windows?.FirstOrDefault()?.Page;
            if (mainPage != null)
                await mainPage.DisplayAlertAsync("Erro", $"Erro ao tirar foto: {ex.Message}", "OK");
        }

        return null;
    }

    /// <summary>
    /// Opens the device photo picker and saves the selected photo to the local cache directory.
    /// </summary>
    /// <returns>The local file path of the selected photo; <see langword="null" /> if the selection was cancelled or an error occurred.</returns>
    public async Task<string?> PickPhotoAsync()
    {
        try
        {
            var photos = await MediaPicker.Default.PickPhotosAsync();
            var photo = photos?.FirstOrDefault();

            if (photo != null)
            {
                var localFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);

                using var sourceStream = await photo.OpenReadAsync();
                using var localFileStream = File.OpenWrite(localFilePath);

                await sourceStream.CopyToAsync(localFileStream);

                return localFilePath;
            }
        }
        catch (Exception ex)
        {
            var mainPage = Application.Current?.Windows?.FirstOrDefault()?.Page;
            if (mainPage != null)
                await mainPage.DisplayAlertAsync("Erro", $"Erro ao selecionar foto: {ex.Message}", "OK");
        }

        return null;
    }
}