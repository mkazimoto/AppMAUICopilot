using CameraApp.Services;
using Microsoft.Maui.Devices.Sensors;

namespace CameraApp.Test.Services;

/// <summary>
/// Unit tests for <see cref="LocationService" />.
/// </summary>
public class LocationServiceTests
{
    private readonly Mock<IGeolocation> _geolocationMock = new();
    private readonly Mock<ILocationPermissions> _permissionsMock = new();
    private readonly LocationService _sut;

    public LocationServiceTests()
    {
        _sut = new LocationService(_geolocationMock.Object, _permissionsMock.Object);
    }

    // ── GetCurrentLocationAsync ───────────────────────────────────────────────

    [Fact]
    public async Task GetCurrentLocationAsync_WhenPermissionAlreadyGranted_ReturnsLocation()
    {
        // Arrange
        var expected = new Location(48.8566, 2.3522);
        _permissionsMock.Setup(p => p.CheckStatusAsync()).ReturnsAsync(PermissionStatus.Granted);
        _geolocationMock
            .Setup(g => g.GetLocationAsync(It.IsAny<GeolocationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _sut.GetCurrentLocationAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expected.Latitude, result.Latitude);
        Assert.Equal(expected.Longitude, result.Longitude);
        _permissionsMock.Verify(p => p.RequestAsync(), Times.Never);
    }

    [Fact]
    public async Task GetCurrentLocationAsync_WhenPermissionDeniedAfterRequest_ReturnsNull()
    {
        // Arrange
        _permissionsMock.Setup(p => p.CheckStatusAsync()).ReturnsAsync(PermissionStatus.Unknown);
        _permissionsMock.Setup(p => p.RequestAsync()).ReturnsAsync(PermissionStatus.Denied);

        // Act
        var result = await _sut.GetCurrentLocationAsync();

        // Assert
        Assert.Null(result);
        _geolocationMock.Verify(
            g => g.GetLocationAsync(It.IsAny<GeolocationRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetCurrentLocationAsync_WhenPermissionGrantedAfterRequest_ReturnsLocation()
    {
        // Arrange
        var expected = new Location(-23.5505, -46.6333);
        _permissionsMock.Setup(p => p.CheckStatusAsync()).ReturnsAsync(PermissionStatus.Unknown);
        _permissionsMock.Setup(p => p.RequestAsync()).ReturnsAsync(PermissionStatus.Granted);
        _geolocationMock
            .Setup(g => g.GetLocationAsync(It.IsAny<GeolocationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _sut.GetCurrentLocationAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expected.Latitude, result.Latitude);
        Assert.Equal(expected.Longitude, result.Longitude);
    }

    [Fact]
    public async Task GetCurrentLocationAsync_WhenGeolocationReturnsNull_ReturnsNull()
    {
        // Arrange
        _permissionsMock.Setup(p => p.CheckStatusAsync()).ReturnsAsync(PermissionStatus.Granted);
        _geolocationMock
            .Setup(g => g.GetLocationAsync(It.IsAny<GeolocationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Location?)null);

        // Act
        var result = await _sut.GetCurrentLocationAsync();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetCurrentLocationAsync_WhenGeolocationThrows_ReturnsNull()
    {
        // Arrange
        _permissionsMock.Setup(p => p.CheckStatusAsync()).ReturnsAsync(PermissionStatus.Granted);
        _geolocationMock
            .Setup(g => g.GetLocationAsync(It.IsAny<GeolocationRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new FeatureNotSupportedException("GPS not supported"));

        // Act
        var result = await _sut.GetCurrentLocationAsync();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetCurrentLocationAsync_WhenPermissionsCheckThrows_ReturnsNull()
    {
        // Arrange
        _permissionsMock.Setup(p => p.CheckStatusAsync()).ThrowsAsync(new Exception("Platform error"));

        // Act
        var result = await _sut.GetCurrentLocationAsync();

        // Assert
        Assert.Null(result);
        _geolocationMock.Verify(
            g => g.GetLocationAsync(It.IsAny<GeolocationRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Theory]
    [InlineData(PermissionStatus.Denied)]
    [InlineData(PermissionStatus.Restricted)]
    [InlineData(PermissionStatus.Disabled)]
    public async Task GetCurrentLocationAsync_WhenPermissionNotGrantedAfterRequest_ReturnsNull(
        PermissionStatus deniedStatus)
    {
        // Arrange
        _permissionsMock.Setup(p => p.CheckStatusAsync()).ReturnsAsync(PermissionStatus.Unknown);
        _permissionsMock.Setup(p => p.RequestAsync()).ReturnsAsync(deniedStatus);

        // Act
        var result = await _sut.GetCurrentLocationAsync();

        // Assert
        Assert.Null(result);
        _geolocationMock.Verify(
            g => g.GetLocationAsync(It.IsAny<GeolocationRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    // ── RequestLocationPermissionAsync ────────────────────────────────────────

    [Fact]
    public async Task RequestLocationPermissionAsync_WhenAlreadyGranted_ReturnsTrueWithoutRequest()
    {
        // Arrange
        _permissionsMock.Setup(p => p.CheckStatusAsync()).ReturnsAsync(PermissionStatus.Granted);

        // Act
        var result = await _sut.RequestLocationPermissionAsync();

        // Assert
        Assert.True(result);
        _permissionsMock.Verify(p => p.RequestAsync(), Times.Never);
    }

    [Fact]
    public async Task RequestLocationPermissionAsync_WhenNotGranted_RequestsAndReturnsTrue()
    {
        // Arrange
        _permissionsMock.Setup(p => p.CheckStatusAsync()).ReturnsAsync(PermissionStatus.Unknown);
        _permissionsMock.Setup(p => p.RequestAsync()).ReturnsAsync(PermissionStatus.Granted);

        // Act
        var result = await _sut.RequestLocationPermissionAsync();

        // Assert
        Assert.True(result);
        _permissionsMock.Verify(p => p.RequestAsync(), Times.Once);
    }

    [Theory]
    [InlineData(PermissionStatus.Denied)]
    [InlineData(PermissionStatus.Restricted)]
    [InlineData(PermissionStatus.Disabled)]
    public async Task RequestLocationPermissionAsync_WhenRequestDenied_ReturnsFalse(PermissionStatus status)
    {
        // Arrange
        _permissionsMock.Setup(p => p.CheckStatusAsync()).ReturnsAsync(PermissionStatus.Unknown);
        _permissionsMock.Setup(p => p.RequestAsync()).ReturnsAsync(status);

        // Act
        var result = await _sut.RequestLocationPermissionAsync();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task RequestLocationPermissionAsync_WhenCheckThrows_ReturnsFalse()
    {
        // Arrange
        _permissionsMock.Setup(p => p.CheckStatusAsync()).ThrowsAsync(new Exception("Platform error"));

        // Act
        var result = await _sut.RequestLocationPermissionAsync();

        // Assert
        Assert.False(result);
    }
}
