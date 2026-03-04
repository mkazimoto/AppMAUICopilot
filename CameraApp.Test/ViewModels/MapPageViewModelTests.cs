using CameraApp.Services;
using CameraApp.ViewModels;
using Moq;

namespace CameraApp.Test.ViewModels;

public class MapPageViewModelTests
{
    private readonly Mock<ILocationService> _locationServiceMock;

    public MapPageViewModelTests()
    {
        _locationServiceMock = new Mock<ILocationService>();
    }

    private MapPageViewModel CreateSut() =>
        new(_locationServiceMock.Object);

    // ── Initial state ────────────────────────────────────────────────────────

    [Fact]
    public void InitialState_IsLoading_IsFalse()
    {
        var sut = CreateSut();
        Assert.False(sut.IsLoading);
    }

    [Fact]
    public void InitialState_HasLocation_IsFalse()
    {
        var sut = CreateSut();
        Assert.False(sut.HasLocation);
    }

    [Fact]
    public void InitialState_LocationText_IsEmpty()
    {
        var sut = CreateSut();
        Assert.Equal(string.Empty, sut.LocationText);
    }

    [Fact]
    public void InitialState_LastUpdateText_IsEmpty()
    {
        var sut = CreateSut();
        Assert.Equal(string.Empty, sut.LastUpdateText);
    }

    [Fact]
    public void InitialState_MapUrl_ContainsDefaultMapHtml()
    {
        var sut = CreateSut();
        Assert.StartsWith("data:text/html;charset=utf-8,", sut.MapUrl);
    }

    [Fact]
    public void InitialState_MapUrl_IsCenteredOnBrazil()
    {
        var sut = CreateSut();
        // Default map uses Brazil center coordinates
        Assert.Contains("-14.2350", Uri.UnescapeDataString(sut.MapUrl));
        Assert.Contains("-51.9253", Uri.UnescapeDataString(sut.MapUrl));
    }

    // ── GetLocationAsync – success ───────────────────────────────────────────

    [Fact]
    public async Task GetLocationAsync_WhenLocationReturned_SetsHasLocationTrue()
    {
        // Arrange
        _locationServiceMock
            .Setup(s => s.GetCurrentLocationAsync())
            .ReturnsAsync(new Location(10.123456, 20.654321));
        var sut = CreateSut();

        // Act
        await sut.GetLocationCommand.ExecuteAsync(null);

        // Assert
        Assert.True(sut.HasLocation);
    }

    [Fact]
    public async Task GetLocationAsync_WhenLocationReturned_SetsFormattedLocationText()
    {
        // Arrange
        const double lat = 10.123456, lng = 20.654321;
        _locationServiceMock
            .Setup(s => s.GetCurrentLocationAsync())
            .ReturnsAsync(new Location(lat, lng));
        var sut = CreateSut();

        // Act
        await sut.GetLocationCommand.ExecuteAsync(null);

        // Assert – use the same culture-sensitive format the ViewModel uses
        Assert.Equal($"Lat: {lat:F6}, Lng: {lng:F6}", sut.LocationText);
    }

    [Fact]
    public async Task GetLocationAsync_WhenLocationReturned_SetsLastUpdateText()
    {
        // Arrange
        _locationServiceMock
            .Setup(s => s.GetCurrentLocationAsync())
            .ReturnsAsync(new Location(10.0, 20.0));
        var sut = CreateSut();

        // Act
        await sut.GetLocationCommand.ExecuteAsync(null);

        // Assert
        Assert.StartsWith("Atualizado em:", sut.LastUpdateText);
    }

    [Fact]
    public async Task GetLocationAsync_WhenLocationReturned_UpdatesMapUrlWithCoordinates()
    {
        // Arrange
        _locationServiceMock
            .Setup(s => s.GetCurrentLocationAsync())
            .ReturnsAsync(new Location(10.123456, 20.654321));
        var sut = CreateSut();

        // Act
        await sut.GetLocationCommand.ExecuteAsync(null);

        // Assert
        var decoded = Uri.UnescapeDataString(sut.MapUrl);
        Assert.StartsWith("data:text/html;charset=utf-8,", sut.MapUrl);
        Assert.Contains("10.123456", decoded);
        Assert.Contains("20.654321", decoded);
    }

    [Fact]
    public async Task GetLocationAsync_WhenLocationReturned_IsLoadingFalseAfterCompletion()
    {
        // Arrange
        _locationServiceMock
            .Setup(s => s.GetCurrentLocationAsync())
            .ReturnsAsync(new Location(10.0, 20.0));
        var sut = CreateSut();

        // Act
        await sut.GetLocationCommand.ExecuteAsync(null);

        // Assert
        Assert.False(sut.IsLoading);
    }

    [Fact]
    public async Task GetLocationAsync_AlwaysCallsLocationService()
    {
        // Arrange
        _locationServiceMock
            .Setup(s => s.GetCurrentLocationAsync())
            .ReturnsAsync((Location?)null);
        var sut = CreateSut();

        // Act
        await sut.GetLocationCommand.ExecuteAsync(null);

        // Assert
        _locationServiceMock.Verify(s => s.GetCurrentLocationAsync(), Times.Once);
    }

    // ── GetLocationAsync – null result ───────────────────────────────────────

    [Fact]
    public async Task GetLocationAsync_WhenNullReturned_HasLocationIsFalse()
    {
        // Arrange
        _locationServiceMock
            .Setup(s => s.GetCurrentLocationAsync())
            .ReturnsAsync((Location?)null);
        var sut = CreateSut();

        // Act
        await sut.GetLocationCommand.ExecuteAsync(null);

        // Assert
        Assert.False(sut.HasLocation);
    }

    [Fact]
    public async Task GetLocationAsync_WhenNullReturned_ClearsLocationText()
    {
        // Arrange
        _locationServiceMock
            .Setup(s => s.GetCurrentLocationAsync())
            .ReturnsAsync((Location?)null);
        var sut = CreateSut();

        // Act
        await sut.GetLocationCommand.ExecuteAsync(null);

        // Assert
        Assert.Equal(string.Empty, sut.LocationText);
    }

    [Fact]
    public async Task GetLocationAsync_WhenNullReturned_ClearsLastUpdateText()
    {
        // Arrange
        _locationServiceMock
            .Setup(s => s.GetCurrentLocationAsync())
            .ReturnsAsync((Location?)null);
        var sut = CreateSut();

        // Act
        await sut.GetLocationCommand.ExecuteAsync(null);

        // Assert
        Assert.Equal(string.Empty, sut.LastUpdateText);
    }

    [Fact]
    public async Task GetLocationAsync_WhenNullReturned_IsLoadingFalseAfterCompletion()
    {
        // Arrange
        _locationServiceMock
            .Setup(s => s.GetCurrentLocationAsync())
            .ReturnsAsync((Location?)null);
        var sut = CreateSut();

        // Act
        await sut.GetLocationCommand.ExecuteAsync(null);

        // Assert
        Assert.False(sut.IsLoading);
    }

    // ── GetLocationAsync – exception ─────────────────────────────────────────

    [Fact]
    public async Task GetLocationAsync_WhenServiceThrows_HasLocationIsFalse()
    {
        // Arrange
        _locationServiceMock
            .Setup(s => s.GetCurrentLocationAsync())
            .ThrowsAsync(new Exception("GPS unavailable"));
        var sut = CreateSut();

        // Act
        await sut.GetLocationCommand.ExecuteAsync(null);

        // Assert
        Assert.False(sut.HasLocation);
    }

    [Fact]
    public async Task GetLocationAsync_WhenServiceThrows_IsLoadingFalseAfterCompletion()
    {
        // Arrange
        _locationServiceMock
            .Setup(s => s.GetCurrentLocationAsync())
            .ThrowsAsync(new Exception("GPS unavailable"));
        var sut = CreateSut();

        // Act
        await sut.GetLocationCommand.ExecuteAsync(null);

        // Assert
        Assert.False(sut.IsLoading);
    }

    [Theory]
    [InlineData(typeof(Exception))]
    [InlineData(typeof(TimeoutException))]
    [InlineData(typeof(InvalidOperationException))]
    public async Task GetLocationAsync_WhenAnyExceptionThrown_IsLoadingFalse(Type exceptionType)
    {
        // Arrange
        var exception = (Exception)Activator.CreateInstance(exceptionType, "test error")!;
        _locationServiceMock
            .Setup(s => s.GetCurrentLocationAsync())
            .ThrowsAsync(exception);
        var sut = CreateSut();

        // Act
        await sut.GetLocationCommand.ExecuteAsync(null);

        // Assert
        Assert.False(sut.IsLoading);
    }

    // ── ResetMap ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task ResetMap_WhenHasLocation_MapUrlContainsCurrentCoordinates()
    {
        // Arrange
        _locationServiceMock
            .Setup(s => s.GetCurrentLocationAsync())
            .ReturnsAsync(new Location(55.676098, 12.568337));
        var sut = CreateSut();
        await sut.GetLocationCommand.ExecuteAsync(null);
        var mapUrlAfterGet = sut.MapUrl;

        // Simulate a hypothetical URL change, then reset
        sut.MapUrl = "about:blank";

        // Act
        sut.ResetMapCommand.Execute(null);

        // Assert – URL is restored to the location-based map
        Assert.Equal(mapUrlAfterGet, sut.MapUrl);
    }

    [Fact]
    public void ResetMap_WhenNoLocation_MapUrlContainsDefaultBrazilCoordinates()
    {
        // Arrange
        var sut = CreateSut();
        sut.MapUrl = "about:blank"; // Overwrite to confirm it is actually reset

        // Act
        sut.ResetMapCommand.Execute(null);

        // Assert
        var decoded = Uri.UnescapeDataString(sut.MapUrl);
        Assert.StartsWith("data:text/html;charset=utf-8,", sut.MapUrl);
        Assert.Contains("-14.2350", decoded);
        Assert.Contains("-51.9253", decoded);
    }
}
