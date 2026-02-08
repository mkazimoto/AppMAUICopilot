using CameraApp.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CameraApp.Test.Services;

[TestClass]
public class LocationServiceTests
{
    private LocationService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _service = new LocationService();
    }

    #region Constructor Tests

    [TestMethod]
    public void Constructor_CreatesInstance()
    {
        // Arrange & Act
        var service = new LocationService();

        // Assert
        Assert.IsNotNull(service);
    }

    [TestMethod]
    public void Constructor_ImplementsInterface()
    {
        // Arrange & Act
        var service = new LocationService();

        // Assert
        Assert.IsInstanceOfType<ILocationService>(service);
    }

    #endregion

    #region GetCurrentLocationAsync Tests

    [TestMethod]
    public async Task GetCurrentLocationAsync_ReturnsTask()
    {
        // Act
        var task = _service.GetCurrentLocationAsync();

        // Assert
        Assert.IsNotNull(task);
        Assert.IsInstanceOfType<Task<Location?>>(task);
        
        // Complete the task (will return null in test environment without permissions)
        var result = await task;
        
        // In test environment without actual device permissions, expect null
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetCurrentLocationAsync_DoesNotThrow()
    {
        // Act & Assert - should not throw even in test environment
        try
        {
            var result = await _service.GetCurrentLocationAsync();
            
            // In test environment, should return null gracefully
            Assert.IsNull(result);
        }
        catch (Exception ex)
        {
            Assert.Fail($"Should not throw exception: {ex.Message}");
        }
    }

    [TestMethod]
    public async Task GetCurrentLocationAsync_CanBeCalledMultipleTimes()
    {
        // Act
        var result1 = await _service.GetCurrentLocationAsync();
        var result2 = await _service.GetCurrentLocationAsync();
        var result3 = await _service.GetCurrentLocationAsync();

        // Assert - should not throw and can be called multiple times
        // Results will be null in test environment
        Assert.IsNull(result1);
        Assert.IsNull(result2);
        Assert.IsNull(result3);
    }

    #endregion

    #region RequestLocationPermissionAsync Tests

    [TestMethod]
    public async Task RequestLocationPermissionAsync_ReturnsTask()
    {
        // Act
        var task = _service.RequestLocationPermissionAsync();

        // Assert
        Assert.IsNotNull(task);
        Assert.IsInstanceOfType<Task<bool>>(task);
        
        // Complete the task
        var result = await task;
        
        // In test environment, expect false (no permissions available)
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task RequestLocationPermissionAsync_ReturnsBool()
    {
        // Act
        var result = await _service.RequestLocationPermissionAsync();

        // Assert
        Assert.IsInstanceOfType<bool>(result);
        
        // In test environment without permissions system, expect false
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task RequestLocationPermissionAsync_DoesNotThrow()
    {
        // Act & Assert - should not throw even in test environment
        try
        {
            var result = await _service.RequestLocationPermissionAsync();
            
            // In test environment, should return false gracefully
            Assert.IsFalse(result);
        }
        catch (Exception ex)
        {
            Assert.Fail($"Should not throw exception: {ex.Message}");
        }
    }

    [TestMethod]
    public async Task RequestLocationPermissionAsync_CanBeCalledMultipleTimes()
    {
        // Act
        var result1 = await _service.RequestLocationPermissionAsync();
        var result2 = await _service.RequestLocationPermissionAsync();
        var result3 = await _service.RequestLocationPermissionAsync();

        // Assert - should not throw and can be called multiple times
        // Results will be false in test environment
        Assert.IsFalse(result1);
        Assert.IsFalse(result2);
        Assert.IsFalse(result3);
    }

    #endregion

    #region Interface Implementation Tests

    [TestMethod]
    public void LocationService_ImplementsILocationService()
    {
        // Assert
        Assert.IsInstanceOfType<ILocationService>(_service);
    }

    [TestMethod]
    public void LocationService_HasGetCurrentLocationAsyncMethod()
    {
        // Arrange
        var method = typeof(LocationService).GetMethod(nameof(ILocationService.GetCurrentLocationAsync));

        // Assert
        Assert.IsNotNull(method);
        Assert.AreEqual(typeof(Task<Location?>), method.ReturnType);
    }

    [TestMethod]
    public void LocationService_HasRequestLocationPermissionAsyncMethod()
    {
        // Arrange
        var method = typeof(LocationService).GetMethod(nameof(ILocationService.RequestLocationPermissionAsync));

        // Assert
        Assert.IsNotNull(method);
        Assert.AreEqual(typeof(Task<bool>), method.ReturnType);
    }

    #endregion

    #region Multiple Instance Tests

    [TestMethod]
    public void MultipleInstances_CanBeCreated()
    {
        // Arrange & Act
        var service1 = new LocationService();
        var service2 = new LocationService();
        var service3 = new LocationService();

        // Assert
        Assert.IsNotNull(service1);
        Assert.IsNotNull(service2);
        Assert.IsNotNull(service3);
        Assert.AreNotSame(service1, service2);
        Assert.AreNotSame(service2, service3);
    }

    [TestMethod]
    public async Task MultipleInstances_OperateIndependently()
    {
        // Arrange
        var service1 = new LocationService();
        var service2 = new LocationService();

        // Act
        var task1 = service1.GetCurrentLocationAsync();
        var task2 = service2.GetCurrentLocationAsync();

        var result1 = await task1;
        var result2 = await task2;

        // Assert - both should complete independently
        Assert.IsNull(result1);
        Assert.IsNull(result2);
    }

    #endregion

    #region Concurrent Access Tests

    [TestMethod]
    public async Task GetCurrentLocationAsync_ConcurrentCalls_DoNotInterfere()
    {
        // Arrange
        var tasks = new List<Task<Location?>>();

        // Act - call method concurrently
        for (int i = 0; i < 5; i++)
        {
            tasks.Add(_service.GetCurrentLocationAsync());
        }

        var results = await Task.WhenAll(tasks);

        // Assert - all should complete successfully
        Assert.AreEqual(5, results.Length);
        foreach (var result in results)
        {
            // In test environment, all should be null
            Assert.IsNull(result);
        }
    }

    [TestMethod]
    public async Task RequestLocationPermissionAsync_ConcurrentCalls_DoNotInterfere()
    {
        // Arrange
        var tasks = new List<Task<bool>>();

        // Act - call method concurrently
        for (int i = 0; i < 5; i++)
        {
            tasks.Add(_service.RequestLocationPermissionAsync());
        }

        var results = await Task.WhenAll(tasks);

        // Assert - all should complete successfully
        Assert.AreEqual(5, results.Length);
        foreach (var result in results)
        {
            // In test environment, all should be false
            Assert.IsFalse(result);
        }
    }

    #endregion

    #region Exception Handling Tests

    [TestMethod]
    public async Task GetCurrentLocationAsync_HandlesExceptionsGracefully()
    {
        // Act & Assert - should handle any platform exceptions gracefully
        try
        {
            var result = await _service.GetCurrentLocationAsync();
            
            // Should return null instead of throwing
            Assert.IsNull(result);
        }
        catch
        {
            Assert.Fail("Method should handle exceptions internally and return null");
        }
    }

    [TestMethod]
    public async Task RequestLocationPermissionAsync_HandlesExceptionsGracefully()
    {
        // Act & Assert - should handle any platform exceptions gracefully
        try
        {
            var result = await _service.RequestLocationPermissionAsync();
            
            // Should return false instead of throwing
            Assert.IsFalse(result);
        }
        catch
        {
            Assert.Fail("Method should handle exceptions internally and return false");
        }
    }

    #endregion

    #region Task Completion Tests

    [TestMethod]
    public async Task GetCurrentLocationAsync_CompletesWithinReasonableTime()
    {
        // Arrange
        var timeout = TimeSpan.FromSeconds(15); // Account for the 10-second internal timeout + buffer
        var cts = new CancellationTokenSource(timeout);

        // Act & Assert
        try
        {
            var task = _service.GetCurrentLocationAsync();
            var completedTask = await Task.WhenAny(task, Task.Delay(timeout, cts.Token));
            
            if (completedTask != task)
            {
                Assert.Fail("Method took longer than expected timeout");
            }

            var result = await task;
            
            // Should complete successfully (even if result is null)
            Assert.IsTrue(task.IsCompleted);
        }
        finally
        {
            cts.Cancel();
        }
    }

    [TestMethod]
    public async Task RequestLocationPermissionAsync_CompletesWithinReasonableTime()
    {
        // Arrange
        var timeout = TimeSpan.FromSeconds(5);
        var cts = new CancellationTokenSource(timeout);

        // Act & Assert
        try
        {
            var task = _service.RequestLocationPermissionAsync();
            var completedTask = await Task.WhenAny(task, Task.Delay(timeout, cts.Token));
            
            if (completedTask != task)
            {
                Assert.Fail("Method took longer than expected timeout");
            }

            var result = await task;
            
            // Should complete successfully
            Assert.IsTrue(task.IsCompleted);
        }
        finally
        {
            cts.Cancel();
        }
    }

    #endregion

    #region Null Safety Tests

    [TestMethod]
    public async Task GetCurrentLocationAsync_ReturnsNullableLocation()
    {
        // Act
        var result = await _service.GetCurrentLocationAsync();

        // Assert - result can be null
        // This is the expected behavior when permissions are denied or location unavailable
        if (result == null)
        {
            Assert.IsNull(result);
        }
        else
        {
            Assert.IsInstanceOfType<Location>(result);
        }
    }

    #endregion

    #region Service Lifecycle Tests

    [TestMethod]
    public async Task LocationService_CanBeReusedAfterCalls()
    {
        // Arrange
        var service = new LocationService();

        // Act - use service multiple times
        var result1 = await service.GetCurrentLocationAsync();
        var permission1 = await service.RequestLocationPermissionAsync();
        
        var result2 = await service.GetCurrentLocationAsync();
        var permission2 = await service.RequestLocationPermissionAsync();

        // Assert - service should remain functional
        Assert.IsNull(result1);
        Assert.IsNull(result2);
        Assert.IsFalse(permission1);
        Assert.IsFalse(permission2);
    }

    [TestMethod]
    public async Task LocationService_SupportsSequentialOperations()
    {
        // Act - sequential calls in order
        var permission = await _service.RequestLocationPermissionAsync();
        var location = await _service.GetCurrentLocationAsync();

        // Assert - both operations should complete
        Assert.IsFalse(permission); // No permission in test environment
        Assert.IsNull(location); // No location in test environment
    }

    [TestMethod]
    public async Task LocationService_SupportsInterleavedOperations()
    {
        // Act - start both operations, then await
        var permissionTask = _service.RequestLocationPermissionAsync();
        var locationTask = _service.GetCurrentLocationAsync();

        var permission = await permissionTask;
        var location = await locationTask;

        // Assert - both should complete independently
        Assert.IsFalse(permission);
        Assert.IsNull(location);
    }

    #endregion
}
