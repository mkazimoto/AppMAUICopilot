using CameraApp.Services;
using CameraApp.ViewModels;
using NSubstitute;
using Microsoft.Maui.Devices.Sensors;
using CommunityToolkit.Mvvm.Input;

namespace CameraApp.Test.ViewModels;

[TestClass]
public class MapPageViewModelTests
{
    private ILocationService _locationService = null!;
    private MapPageViewModel _viewModel = null!;

    [TestInitialize]
    public void Setup()
    {
        _locationService = Substitute.For<ILocationService>();
        _viewModel = new MapPageViewModel(_locationService);
    }

    #region Constructor Tests

    [TestMethod]
    public void Constructor_InitializesWithDefaultValues()
    {
        // Assert
        Assert.IsFalse(_viewModel.IsLoading);
        Assert.IsFalse(_viewModel.HasLocation);
        Assert.AreEqual(string.Empty, _viewModel.LocationText);
        Assert.AreEqual(string.Empty, _viewModel.LastUpdateText);
    }

    [TestMethod]
    public void Constructor_GeneratesDefaultMap()
    {
        // Assert
        Assert.IsFalse(string.IsNullOrEmpty(_viewModel.MapUrl));
        Assert.IsTrue(_viewModel.MapUrl.StartsWith("data:text/html"));
        Assert.IsTrue(_viewModel.MapUrl.Contains("-14.2350")); // Brazil center latitude
        Assert.IsTrue(_viewModel.MapUrl.Contains("-51.9253")); // Brazil center longitude
    }

    [TestMethod]
    public void Constructor_WithValidService_DoesNotThrow()
    {
        // Act & Assert - should not throw
        var viewModel = new MapPageViewModel(_locationService);
        Assert.IsNotNull(viewModel);
    }

    #endregion

    #region GetLocationAsync Tests

    [TestMethod]
    public async Task GetLocationAsync_WhenLocationRetrieved_UpdatesProperties()
    {
        // Arrange
        var mockLocation = CreateMockLocation(-23.550520, -46.633308); // São Paulo coordinates
        _locationService.GetCurrentLocationAsync().Returns(Task.FromResult<Location?>(mockLocation));

        // Act
        await InvokeGetLocationAsync();

        // Assert
        Assert.IsTrue(_viewModel.HasLocation);
        Assert.IsTrue(_viewModel.LocationText.Contains("Lat:"));
        Assert.IsTrue(_viewModel.LocationText.Contains("Lng:"));
        Assert.IsTrue(_viewModel.LocationText.Contains("-23"));
        Assert.IsTrue(_viewModel.LocationText.Contains("-46"));
        Assert.IsFalse(string.IsNullOrEmpty(_viewModel.LastUpdateText));
        Assert.IsTrue(_viewModel.LastUpdateText.Contains("Atualizado em:"));
        Assert.IsFalse(_viewModel.IsLoading);
    }

    [TestMethod]
    public async Task GetLocationAsync_WhenLocationRetrieved_UpdatesMapUrl()
    {
        // Arrange
        var mockLocation = CreateMockLocation(-23.550520, -46.633308);
        _locationService.GetCurrentLocationAsync().Returns(Task.FromResult<Location?>(mockLocation));
        var initialMapUrl = _viewModel.MapUrl;

        // Act
        await InvokeGetLocationAsync();

        // Assert
        Assert.AreNotEqual(initialMapUrl, _viewModel.MapUrl);
        Assert.IsTrue(_viewModel.MapUrl.Contains("-23.550520"));
        Assert.IsTrue(_viewModel.MapUrl.Contains("-46.633308"));
    }

    [TestMethod]
    public async Task GetLocationAsync_WhenServiceReturnsNull_ClearsLocationData()
    {
        // Arrange
        _locationService.GetCurrentLocationAsync().Returns(Task.FromResult<Location?>(null));

        // Act
        try
        {
            await InvokeGetLocationAsync();
        }
        catch (NullReferenceException)
        {
            // Shell.Current.DisplayAlert will throw NullReferenceException in test environment
            // This is expected behavior when service returns null
        }

        // Assert
        Assert.IsFalse(_viewModel.HasLocation);
        Assert.AreEqual(string.Empty, _viewModel.LocationText);
        Assert.AreEqual(string.Empty, _viewModel.LastUpdateText);
        Assert.IsFalse(_viewModel.IsLoading);
    }

    [TestMethod]
    public async Task GetLocationAsync_SetsIsLoadingTrueWhileProcessing()
    {
        // Arrange
        var tcs = new TaskCompletionSource<Location?>();
        _locationService.GetCurrentLocationAsync().Returns(tcs.Task);

        // Act
        var task = InvokeGetLocationAsync();

        // Assert - IsLoading should be true while processing
        await Task.Delay(50); // Give it time to set IsLoading
        Assert.IsTrue(_viewModel.IsLoading);

        // Complete the task
        tcs.SetResult(CreateMockLocation(0, 0));
        await task;

        Assert.IsFalse(_viewModel.IsLoading);
    }

    [TestMethod]
    public async Task GetLocationAsync_WhenExceptionThrown_SetsHasLocationToFalse()
    {
        // Arrange
        _locationService.GetCurrentLocationAsync().Returns<Location?>(_ => throw new InvalidOperationException("GPS not available"));

        // Act
        try
        {
            await InvokeGetLocationAsync();
        }
        catch (NullReferenceException)
        {
            // Shell.Current.DisplayAlert will throw NullReferenceException in test environment
            // This is expected behavior in unit tests
        }

        // Assert
        Assert.IsFalse(_viewModel.HasLocation);
        Assert.IsFalse(_viewModel.IsLoading);
    }

    [TestMethod]
    public async Task GetLocationAsync_AlwaysSetsIsLoadingToFalse()
    {
        // Arrange - Test with successful call
        _locationService.GetCurrentLocationAsync().Returns(Task.FromResult<Location?>(CreateMockLocation(0, 0)));

        // Act
        await InvokeGetLocationAsync();

        // Assert
        Assert.IsFalse(_viewModel.IsLoading);

        // Note: Cannot test exception case with Shell.Current.DisplayAlert in unit tests
        // Shell.Current is null in test environment
    }

    [TestMethod]
    public async Task GetLocationAsync_FormatsCoordinatesCorrectly()
    {
        // Arrange
        var mockLocation = CreateMockLocation(-23.123456789, -46.987654321);
        _locationService.GetCurrentLocationAsync().Returns(Task.FromResult<Location?>(mockLocation));

        // Act
        await InvokeGetLocationAsync();

        // Assert - Should contain formatted coordinates with Lat:/Lng: labels
        Assert.IsTrue(_viewModel.HasLocation);
        Assert.IsTrue(_viewModel.LocationText.Contains("Lat:"));
        Assert.IsTrue(_viewModel.LocationText.Contains("Lng:"));
        Assert.IsTrue(_viewModel.LocationText.Length > 10); // Has actual coordinate data
    }

    [TestMethod]
    public async Task GetLocationAsync_CanBeCalledMultipleTimes()
    {
        // Arrange
        var location1 = CreateMockLocation(-23.550520, -46.633308);
        var location2 = CreateMockLocation(-22.906847, -43.172896); // Rio de Janeiro
        
        _locationService.GetCurrentLocationAsync().Returns(
            Task.FromResult<Location?>(location1),
            Task.FromResult<Location?>(location2));

        // Act
        await InvokeGetLocationAsync();
        var firstLocationText = _viewModel.LocationText;
        
        await InvokeGetLocationAsync();
        var secondLocationText = _viewModel.LocationText;

        // Assert
        Assert.AreNotEqual(firstLocationText, secondLocationText);
        Assert.IsTrue(secondLocationText.Contains("-22.9") || secondLocationText.Contains("-22,9"));
        Assert.IsTrue(secondLocationText.Contains("-43.1") || secondLocationText.Contains("-43,1"));
        await _locationService.Received(2).GetCurrentLocationAsync();
    }

    #endregion

    #region ResetMap Tests

    [TestMethod]
    public async Task ResetMap_WhenHasLocation_RestoresCurrentLocation()
    {
        // Arrange
        var mockLocation = CreateMockLocation(-23.550520, -46.633308);
        _locationService.GetCurrentLocationAsync().Returns(Task.FromResult<Location?>(mockLocation));
        await InvokeGetLocationAsync();
        
        var mapUrlAfterLocation = _viewModel.MapUrl;
        
        // Modify the map URL to simulate user interaction
        _viewModel.MapUrl = "modified_url";

        // Act
        InvokeResetMap();

        // Assert
        Assert.AreNotEqual("modified_url", _viewModel.MapUrl);
        Assert.IsTrue(_viewModel.MapUrl.Contains("-23.550520"));
        Assert.IsTrue(_viewModel.MapUrl.Contains("-46.633308"));
    }

    [TestMethod]
    public void ResetMap_WhenNoLocation_RestoresDefaultMap()
    {
        // Arrange
        _viewModel.MapUrl = "modified_url";

        // Act
        InvokeResetMap();

        // Assert
        Assert.AreNotEqual("modified_url", _viewModel.MapUrl);
        Assert.IsTrue(_viewModel.MapUrl.Contains("-14.2350")); // Brazil center
        Assert.IsTrue(_viewModel.MapUrl.Contains("-51.9253"));
    }

    [TestMethod]
    public void ResetMap_CanBeCalledMultipleTimes()
    {
        // Act & Assert - should not throw
        InvokeResetMap();
        InvokeResetMap();
        InvokeResetMap();
        
        Assert.IsTrue(_viewModel.MapUrl.Contains("-14.2350"));
    }

    #endregion

    #region MapUrl Generation Tests

    [TestMethod]
    public void MapUrl_ContainsLeafletReferences()
    {
        // Assert
        Assert.IsTrue(_viewModel.MapUrl.Contains("leaflet"));
        Assert.IsTrue(_viewModel.MapUrl.Contains("openstreetmap"));
    }

    [TestMethod]
    public void MapUrl_IsProperlyEncoded()
    {
        // Assert
        Assert.IsTrue(_viewModel.MapUrl.StartsWith("data:text/html;charset=utf-8,"));
        Assert.IsFalse(_viewModel.MapUrl.Contains(" ")); // Should be URL encoded
    }

    [TestMethod]
    public async Task MapUrl_WithLocation_ContainsMarkerAndCircle()
    {
        // Arrange
        var mockLocation = CreateMockLocation(-23.550520, -46.633308);
        _locationService.GetCurrentLocationAsync().Returns(Task.FromResult<Location?>(mockLocation));

        // Act
        await InvokeGetLocationAsync();
        var decodedUrl = Uri.UnescapeDataString(_viewModel.MapUrl);

        // Assert
        Assert.IsTrue(decodedUrl.Contains("marker"));
        Assert.IsTrue(decodedUrl.Contains("circle"));
        Assert.IsTrue(decodedUrl.Contains("Sua Localização"));
    }

    [TestMethod]
    public void MapUrl_Default_DoesNotContainLocationMarker()
    {
        // Arrange
        var decodedUrl = Uri.UnescapeDataString(_viewModel.MapUrl);

        // Assert - Check that the marker is not actually added to the map (not in the executable code)
        Assert.IsFalse(decodedUrl.Contains("L.marker"));
        Assert.IsFalse(decodedUrl.Contains("Sua Localização"));
    }

    #endregion

    #region Property Change Notification Tests

    [TestMethod]
    public void IsLoading_WhenChanged_RaisesPropertyChanged()
    {
        // Arrange
        var propertyChangedRaised = false;
        _viewModel.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(_viewModel.IsLoading))
                propertyChangedRaised = true;
        };

        // Act
        _viewModel.IsLoading = true;

        // Assert
        Assert.IsTrue(propertyChangedRaised);
    }

    [TestMethod]
    public void HasLocation_WhenChanged_RaisesPropertyChanged()
    {
        // Arrange
        var propertyChangedRaised = false;
        _viewModel.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(_viewModel.HasLocation))
                propertyChangedRaised = true;
        };

        // Act
        _viewModel.HasLocation = true;

        // Assert
        Assert.IsTrue(propertyChangedRaised);
    }

    [TestMethod]
    public void LocationText_WhenChanged_RaisesPropertyChanged()
    {
        // Arrange
        var propertyChangedRaised = false;
        _viewModel.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(_viewModel.LocationText))
                propertyChangedRaised = true;
        };

        // Act
        _viewModel.LocationText = "Test Location";

        // Assert
        Assert.IsTrue(propertyChangedRaised);
    }

    [TestMethod]
    public void MapUrl_WhenChanged_RaisesPropertyChanged()
    {
        // Arrange
        var propertyChangedRaised = false;
        _viewModel.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(_viewModel.MapUrl))
                propertyChangedRaised = true;
        };

        // Act
        _viewModel.MapUrl = "new_url";

        // Assert
        Assert.IsTrue(propertyChangedRaised);
    }

    #endregion

    #region Integration/Workflow Tests

    [TestMethod]
    public async Task Workflow_GetLocation_ThenReset_WorksCorrectly()
    {
        // Arrange
        var mockLocation = CreateMockLocation(-23.550520, -46.633308);
        _locationService.GetCurrentLocationAsync().Returns(Task.FromResult<Location?>(mockLocation));

        // Act - Get location
        await InvokeGetLocationAsync();
        Assert.IsTrue(_viewModel.HasLocation);
        var locationMapUrl = _viewModel.MapUrl;

        // Act - Reset map
        InvokeResetMap();

        // Assert
        Assert.IsTrue(_viewModel.HasLocation); // Should still have location
        Assert.IsTrue(_viewModel.MapUrl.Contains("-23.550520")); // Should restore same location
    }

    [TestMethod]
    public async Task Workflow_MultipleLocationUpdates_MaintainsLatestLocation()
    {
        // Arrange
        var location1 = CreateMockLocation(-23.550520, -46.633308); // São Paulo
        var location2 = CreateMockLocation(-22.906847, -43.172896); // Rio
        var location3 = CreateMockLocation(-15.826691, -47.921822); // Brasília

        _locationService.GetCurrentLocationAsync().Returns(
            Task.FromResult<Location?>(location1),
            Task.FromResult<Location?>(location2),
            Task.FromResult<Location?>(location3));

        // Act
        await InvokeGetLocationAsync();
        await InvokeGetLocationAsync();
        await InvokeGetLocationAsync();

        // Assert - Should have the last location (Brasília)
        Assert.IsTrue(_viewModel.LocationText.Contains("-15.8") || _viewModel.LocationText.Contains("-15,8"));
        Assert.IsTrue(_viewModel.LocationText.Contains("-47.9") || _viewModel.LocationText.Contains("-47,9"));

        // Act - Reset should show Brasília
        InvokeResetMap();
        Assert.IsTrue(_viewModel.MapUrl.Contains("-15.8") || _viewModel.MapUrl.Contains("-15,8"));
    }

    #endregion

    #region Edge Cases

    [TestMethod]
    public async Task GetLocationAsync_WithZeroCoordinates_HandlesCorrectly()
    {
        // Arrange
        var mockLocation = CreateMockLocation(0, 0); // Null Island
        _locationService.GetCurrentLocationAsync().Returns(Task.FromResult<Location?>(mockLocation));

        // Act
        await InvokeGetLocationAsync();

        // Assert
        Assert.IsTrue(_viewModel.HasLocation);
        Assert.IsTrue(_viewModel.LocationText.Contains("Lat:"));
        Assert.IsTrue(_viewModel.LocationText.Contains("Lng:"));
    }

    [TestMethod]
    public async Task GetLocationAsync_WithExtremeCoordinates_HandlesCorrectly()
    {
        // Arrange
        var mockLocation = CreateMockLocation(90, 180); // Extreme valid coordinates
        _locationService.GetCurrentLocationAsync().Returns(Task.FromResult<Location?>(mockLocation));

        // Act
        await InvokeGetLocationAsync();

        // Assert
        Assert.IsTrue(_viewModel.HasLocation);
        Assert.IsTrue(_viewModel.LocationText.Contains("90"));
        Assert.IsTrue(_viewModel.LocationText.Contains("180"));
    }

    [TestMethod]
    public async Task GetLocationAsync_WithNegativeCoordinates_FormatsCorrectly()
    {
        // Arrange
        var mockLocation = CreateMockLocation(-90, -180);
        _locationService.GetCurrentLocationAsync().Returns(Task.FromResult<Location?>(mockLocation));

        // Act
        await InvokeGetLocationAsync();

        // Assert
        Assert.IsTrue(_viewModel.LocationText.Contains("-90"));
        Assert.IsTrue(_viewModel.LocationText.Contains("-180"));
    }

    #endregion

    #region Helper Methods

    private Location CreateMockLocation(double latitude, double longitude)
    {
        return new Location(latitude, longitude);
    }

    // Helper methods to invoke private methods via reflection or generated commands
    private async Task InvokeGetLocationAsync()
    {
        // Use reflection to invoke the GetLocationAsyncCommand.ExecuteAsync
        var commandProperty = _viewModel.GetType().GetProperty("GetLocationAsyncCommand");
        if (commandProperty != null)
        {
            var command = commandProperty.GetValue(_viewModel) as IAsyncRelayCommand;
            if (command != null)
            {
                await command.ExecuteAsync(null);
                return;
            }
        }

        // Fallback: invoke the private method directly via reflection
        var method = _viewModel.GetType().GetMethod("GetLocationAsync", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (method != null)
        {
            var task = method.Invoke(_viewModel, null) as Task;
            if (task != null)
            {
                await task;
            }
        }
    }

    private void InvokeResetMap()
    {
        // Use reflection to invoke the ResetMapCommand.Execute
        var commandProperty = _viewModel.GetType().GetProperty("ResetMapCommand");
        if (commandProperty != null)
        {
            var command = commandProperty.GetValue(_viewModel) as IRelayCommand;
            if (command != null)
            {
                command.Execute(null);
                return;
            }
        }

        // Fallback: invoke the private method directly via reflection
        var method = _viewModel.GetType().GetMethod("ResetMap", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        method?.Invoke(_viewModel, null);
    }

    #endregion
}
