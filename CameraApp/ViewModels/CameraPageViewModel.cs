using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CameraApp.Services;

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
        var photoPath = await _cameraService.TakePhotoAsync();

        if (!string.IsNullOrEmpty(photoPath))
        {
            PhotoPath = photoPath;
            HasPhoto = true;
        }
    }

    [RelayCommand]
    private async Task PickPhotoAsync()
    {
        var photoPath = await _cameraService.PickPhotoAsync();

        if (!string.IsNullOrEmpty(photoPath))
        {
            PhotoPath = photoPath;
            HasPhoto = true;
        }
    }

    [RelayCommand]
    private void ClearPhoto()
    {
        PhotoPath = null;
        HasPhoto = false;
    }
}