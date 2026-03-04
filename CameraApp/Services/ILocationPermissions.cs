namespace CameraApp.Services;

/// <summary>
/// Abstracts the MAUI <see cref="Permissions" /> static API for <see cref="Permissions.LocationWhenInUse" /> to enable unit testing.
/// </summary>
public interface ILocationPermissions
{
    /// <summary>Checks the current <see cref="Permissions.LocationWhenInUse" /> permission status.</summary>
    Task<PermissionStatus> CheckStatusAsync();

    /// <summary>Requests the <see cref="Permissions.LocationWhenInUse" /> permission from the user.</summary>
    Task<PermissionStatus> RequestAsync();
}

/// <summary>
/// Default implementation that delegates to the MAUI static <see cref="Permissions" /> API.
/// </summary>
internal sealed class DefaultLocationPermissions : ILocationPermissions
{
    /// <inheritdoc />
    public Task<PermissionStatus> CheckStatusAsync()
        => Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

    /// <inheritdoc />
    public Task<PermissionStatus> RequestAsync()
        => Permissions.RequestAsync<Permissions.LocationWhenInUse>();
}
