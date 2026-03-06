using Microsoft.Extensions.Logging;
using Microsoft.Maui.Devices.Sensors;

namespace CameraApp.Services;

/// <summary>
/// Provides geolocation operations using the MAUI <see cref="IGeolocation" /> API.
/// </summary>
public class LocationService : ILocationService
{
    private readonly IGeolocation _geolocation;
    private readonly ILocationPermissions _permissions;
    private readonly ILogger<LocationService> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="LocationService" /> using the default MAUI implementations.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    public LocationService(ILogger<LocationService> logger)
        : this(Geolocation.Default, new DefaultLocationPermissions(), logger)
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="LocationService" /> with injectable dependencies.
    /// </summary>
    /// <param name="geolocation">The geolocation provider.</param>
    /// <param name="permissions">The location permissions provider.</param>
    /// <param name="logger">The logger instance.</param>
    public LocationService(IGeolocation geolocation, ILocationPermissions permissions, ILogger<LocationService> logger)
    {
        _geolocation = geolocation ?? throw new ArgumentNullException(nameof(geolocation));
        _permissions = permissions ?? throw new ArgumentNullException(nameof(permissions));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Requests location permission if needed, then retrieves the device's current geographic location.
    /// </summary>
    /// <returns>The current <see cref="Location" />; <see langword="null" /> if permission was denied or the location could not be determined.</returns>
    /// <exception cref="FeatureNotSupportedException">Thrown when geolocation is not supported on the device.</exception>
    /// <exception cref="FeatureNotEnabledException">Thrown when location services are disabled on the device.</exception>
    public async Task<Location?> GetCurrentLocationAsync()
    {
        try
        {
            var hasPermission = await RequestLocationPermissionAsync();
            if (!hasPermission)
            {
                _logger.LogWarning("Location permission was denied by the user");
                return null;
            }

            var request = new GeolocationRequest
            {
                DesiredAccuracy = GeolocationAccuracy.Medium,
                Timeout = TimeSpan.FromSeconds(10)
            };

            var location = await _geolocation.GetLocationAsync(request);
            if (location == null)
            {
                _logger.LogWarning("Unable to determine current location");
            }
            else
            {
                _logger.LogInformation("Location obtained successfully: Lat={Latitude}, Lon={Longitude}", 
                    location.Latitude, location.Longitude);
            }

            return location;
        }
        catch (FeatureNotSupportedException ex)
        {
            _logger.LogError(ex, "Geolocation feature is not supported on this device");
            throw;
        }
        catch (FeatureNotEnabledException ex)
        {
            _logger.LogError(ex, "Location services are disabled on the device");
            throw;
        }
        catch (PermissionException ex)
        {
            _logger.LogError(ex, "Location permission exception occurred");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while obtaining location");
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
            _logger.LogInformation("Current location permission status: {Status}", status);

            if (status != PermissionStatus.Granted)
            {
                _logger.LogInformation("Requesting location permission from user");
                status = await _permissions.RequestAsync();
                _logger.LogInformation("Location permission request result: {Status}", status);
            }

            return status == PermissionStatus.Granted;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while requesting location permission");
            return false;
        }
    }
}
