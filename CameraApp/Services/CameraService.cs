using Microsoft.Maui.Media;

namespace CameraApp.Services;

public class CameraService : ICameraService
{
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
                    await mainPage.DisplayAlert("Erro", "Câmera não suportada neste dispositivo", "OK");
            }
        }
        catch (Exception ex)
        {
            var mainPage = Application.Current?.Windows?.FirstOrDefault()?.Page;
            if (mainPage != null)
                await mainPage.DisplayAlert("Erro", $"Erro ao tirar foto: {ex.Message}", "OK");
        }
        
        return null;
    }

    public async Task<string?> PickPhotoAsync()
    {
        try
        {
            var photo = await MediaPicker.Default.PickPhotoAsync();
            
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
                await mainPage.DisplayAlert("Erro", $"Erro ao selecionar foto: {ex.Message}", "OK");
        }
        
        return null;
    }
}