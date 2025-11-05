namespace CameraApp.Services;

public interface ILocationService
{
    Task<Location?> GetCurrentLocationAsync();
    Task<bool> RequestLocationPermissionAsync();
}