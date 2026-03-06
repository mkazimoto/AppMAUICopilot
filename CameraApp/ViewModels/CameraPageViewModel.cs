using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CameraApp.Services;
using CameraApp.Exceptions;

namespace CameraApp.ViewModels;

/// <summary>
/// Provides properties and commands for capturing and managing photos on the camera page.
/// </summary>
public partial class CameraPageViewModel : ObservableObject
{
    private readonly ICameraService _cameraService;

    [ObservableProperty]
    private string? _photoPath;

    [ObservableProperty]
    private bool _hasPhoto;

    /// <summary>
    /// Initializes a new instance of the <see cref="CameraPageViewModel" /> class.
    /// </summary>
    /// <param name="cameraService">The camera service used to capture and pick photos.</param>
    public CameraPageViewModel(ICameraService cameraService)
    {
        _cameraService = cameraService;
    }

    [RelayCommand]
    private async Task TakePhotoAsync()
    {
        try
        {
            var photoPath = await _cameraService.TakePhotoAsync();

            if (!string.IsNullOrEmpty(photoPath))
            {
                PhotoPath = photoPath;
                HasPhoto = true;
            }
        }
        catch (CameraException ex)
        {
            await ShowErrorAsync(LocalizationResourceManager.Instance["Camera_CaptureError"].ToString() ?? "Error capturing photo", ex.Message);
        }
        catch (Exception ex)
        {
            await ShowErrorAsync(LocalizationResourceManager.Instance["Camera_UnexpectedError"].ToString() ?? "Unexpected error", ex.Message);
        }
    }

    [RelayCommand]
    private async Task PickPhotoAsync()
    {
        try
        {
            var photoPath = await _cameraService.PickPhotoAsync();

            if (!string.IsNullOrEmpty(photoPath))
            {
                PhotoPath = photoPath;
                HasPhoto = true;
            }
        }
        catch (CameraException ex)
        {
            await ShowErrorAsync(LocalizationResourceManager.Instance["Camera_PickError"].ToString() ?? "Error selecting photo", ex.Message);
        }
        catch (Exception ex)
        {
            await ShowErrorAsync(LocalizationResourceManager.Instance["Camera_UnexpectedError"].ToString() ?? "Unexpected error", ex.Message);
        }
    }

    private static async Task ShowErrorAsync(string title, string message)
    {
        var mainPage = Application.Current?.Windows?.FirstOrDefault()?.Page;
        if (mainPage != null)
        {
            await mainPage.DisplayAlert(title, message, "OK");
        }
    }

    [RelayCommand]
    private void ClearPhoto()
    {
        PhotoPath = null;
        HasPhoto = false;
    }
}