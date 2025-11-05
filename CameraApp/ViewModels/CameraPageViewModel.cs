using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CameraApp.Services;

namespace CameraApp.ViewModels;

public partial class CameraPageViewModel : ObservableObject
{
    private readonly ICameraService _cameraService;

    [ObservableProperty]
    private string? _photoPath;

    [ObservableProperty]
    private bool _hasPhoto;

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