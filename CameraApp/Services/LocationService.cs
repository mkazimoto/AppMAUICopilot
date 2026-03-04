using Microsoft.Maui.Devices.Sensors;

namespace CameraApp.Services;

/// <summary>
/// Provides geolocation operations using the MAUI <see cref="IGeolocation" /> API.
/// </summary>
public class LocationService : ILocationService
{
    private readonly IGeolocation _geolocation;
    private readonly ILocationPermissions _permissions;

    /// <summary>
    /// Initializes a new instance of <see cref="LocationService" /> using the default MAUI implementations.
    /// </summary>
    public LocationService()
        : this(Geolocation.Default, new DefaultLocationPermissions())
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="LocationService" /> with injectable dependencies.
    /// </summary>
    /// <param name="geolocation">The geolocation provider.</param>
    /// <param name="permissions">The location permissions provider.</param>
    public LocationService(IGeolocation geolocation, ILocationPermissions permissions)
    {
        _geolocation = geolocation;
        _permissions = permissions;
    }

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

            return await _geolocation.GetLocationAsync(request);
        }
        catch (Exception ex)
        {
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
            var status = await _permissions.CheckStatusAsync();

            if (status != PermissionStatus.Granted)
            {
                status = await _permissions.RequestAsync();
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