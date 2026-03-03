namespace CameraApp.Services;

/// <summary>
/// Defines operations for accessing the device's geolocation capabilities.
/// </summary>
public interface ILocationService
{
    /// <summary>
    /// Retrieves the device's current geographic location.
    /// </summary>
    /// <returns>The current <see cref="Location" />; <see langword="null" /> if the location could not be determined.</returns>
    Task<Location?> GetCurrentLocationAsync();

    /// <summary>
    /// Requests the user's permission to access the device location.
    /// </summary>
    /// <returns><see langword="true" /> if permission was granted; otherwise, <see langword="false" />.</returns>
    Task<bool> RequestLocationPermissionAsync();
}