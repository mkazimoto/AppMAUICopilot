namespace CameraApp.Services;

/// <summary>
/// Provides geolocation operations using the MAUI <see cref="Geolocation" /> API.
/// </summary>
public class LocationService : ILocationService
{
    /// <summary>
    /// Requests location permission if needed, then retrieves the device's current geographic location.
    /// </summary>
    /// <returns>The current <see cref="Location" />; <see langword="null" /> if permission was denied or the location could not be determined.</returns>
    public async Task<Location?> GetCurrentLocationAsync()
    {
        try
        {
            var hasPermission = await RequestLocationPermissionAsync();
            if (!hasPermission)
            {
                return null;
            }

            var request = new GeolocationRequest
            {
                DesiredAccuracy = GeolocationAccuracy.Medium,
                Timeout = TimeSpan.FromSeconds(10)
            };

            var location = await Geolocation.GetLocationAsync(request);
            return location;
        }
        catch (Exception ex)
        {
            // Log the error
            System.Diagnostics.Debug.WriteLine($"Erro ao obter localização: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Checks the current location permission status and requests it from the user if not already granted.
    /// </summary>
    /// <returns><see langword="true" /> if the <see cref="Permissions.LocationWhenInUse" /> permission is granted; otherwise, <see langword="false" />.</returns>
    public async Task<bool> RequestLocationPermissionAsync()
    {
        try
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }

            return status == PermissionStatus.Granted;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erro ao solicitar permissão de localização: {ex.Message}");
            return false;
        }
    }
}