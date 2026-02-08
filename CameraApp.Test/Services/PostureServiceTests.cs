using CameraApp.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CameraApp.Test.Services;

[TestClass]
public class PostureServiceTests
{
    private PostureService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _service = new PostureService();
    }

    #region Constructor Tests

    [TestMethod]
    public void Constructor_InitializesWithDefaultValues()
    {
        // Assert
        Assert.IsFalse(_service.IsMonitoring);
        Assert.AreEqual(0.3, _service.Sensitivity);
        Assert.AreEqual(5, _service.AlertDelaySeconds);
    }

    #endregion

    #region Property Tests

    [TestMethod]
    public void Sensitivity_CanBeSet()
    {
        // Arrange
        var newSensitivity = 0.5;

        // Act
        _service.Sensitivity = newSensitivity;

        // Assert
        Assert.AreEqual(newSensitivity, _service.Sensitivity);
    }

    [TestMethod]
    public void Sensitivity_AcceptsDifferentValues()
    {
        // Arrange & Act & Assert
        _service.Sensitivity = 0.0;
        Assert.AreEqual(0.0, _service.Sensitivity);

        _service.Sensitivity = 0.5;
        Assert.AreEqual(0.5, _service.Sensitivity);

        _service.Sensitivity = 1.0;
        Assert.AreEqual(1.0, _service.Sensitivity);
    }

    [TestMethod]
    public void AlertDelaySeconds_CanBeSet()
    {
        // Arrange
        var newDelay = 10;

        // Act
        _service.AlertDelaySeconds = newDelay;

        // Assert
        Assert.AreEqual(newDelay, _service.AlertDelaySeconds);
    }

    [TestMethod]
    public void AlertDelaySeconds_AcceptsDifferentValues()
    {
        // Arrange & Act & Assert
        _service.AlertDelaySeconds = 1;
        Assert.AreEqual(1, _service.AlertDelaySeconds);

        _service.AlertDelaySeconds = 5;
        Assert.AreEqual(5, _service.AlertDelaySeconds);

        _service.AlertDelaySeconds = 15;
        Assert.AreEqual(15, _service.AlertDelaySeconds);
    }

    [TestMethod]
    public void IsMonitoring_DefaultsToFalse()
    {
        // Assert
        Assert.IsFalse(_service.IsMonitoring);
    }

    #endregion

    #region Event Tests

    [TestMethod]
    public void PostureAlert_EventCanBeSubscribed()
    {
        // Arrange
        var eventRaised = false;
        _service.PostureAlert += (sender, args) => eventRaised = true;

        // Act - we can't easily trigger the event without platform-specific code
        // But we can verify subscription doesn't throw
        
        // Assert
        Assert.IsFalse(eventRaised); // Event not triggered in test environment
    }

    [TestMethod]
    public void AccelerometerDataUpdated_EventCanBeSubscribed()
    {
        // Arrange
        var eventRaised = false;
        _service.AccelerometerDataUpdated += (sender, args) => eventRaised = true;

        // Act - we can't easily trigger the event without platform-specific code
        // But we can verify subscription doesn't throw
        
        // Assert
        Assert.IsFalse(eventRaised); // Event not triggered in test environment
    }

    [TestMethod]
    public void PostureAlert_EventCanBeUnsubscribed()
    {
        // Arrange
        var eventRaised = false;
        EventHandler<PostureAlertEventArgs> handler = (sender, args) => eventRaised = true;
        
        _service.PostureAlert += handler;
        _service.PostureAlert -= handler;

        // Act & Assert - should not throw
        Assert.IsFalse(eventRaised);
    }

    [TestMethod]
    public void AccelerometerDataUpdated_EventCanBeUnsubscribed()
    {
        // Arrange
        var eventRaised = false;
        EventHandler<AccelerometerDataEventArgs> handler = (sender, args) => eventRaised = true;
        
        _service.AccelerometerDataUpdated += handler;
        _service.AccelerometerDataUpdated -= handler;

        // Act & Assert - should not throw
        Assert.IsFalse(eventRaised);
    }

    #endregion

    #region StopMonitoring Tests

    [TestMethod]
    public void StopMonitoring_WhenNotMonitoring_DoesNotThrow()
    {
        // Arrange
        Assert.IsFalse(_service.IsMonitoring);

        // Act & Assert - should not throw
        _service.StopMonitoring();
        Assert.IsFalse(_service.IsMonitoring);
    }

    [TestMethod]
    public void StopMonitoring_CanBeCalledMultipleTimes()
    {
        // Arrange
        Assert.IsFalse(_service.IsMonitoring);

        // Act & Assert - should not throw
        _service.StopMonitoring();
        _service.StopMonitoring();
        _service.StopMonitoring();
        Assert.IsFalse(_service.IsMonitoring);
    }

    #endregion

    #region PostureAlertEventArgs Tests

    [TestMethod]
    public void PostureAlertEventArgs_CanBeCreated()
    {
        // Arrange & Act
        var eventArgs = new PostureAlertEventArgs
        {
            Message = "Test message",
            Status = PostureStatus.Poor,
            Timestamp = DateTime.Now
        };

        // Assert
        Assert.AreEqual("Test message", eventArgs.Message);
        Assert.AreEqual(PostureStatus.Poor, eventArgs.Status);
        Assert.IsTrue(eventArgs.Timestamp <= DateTime.Now);
    }

    [TestMethod]
    public void PostureAlertEventArgs_DefaultsMessageToEmpty()
    {
        // Arrange & Act
        var eventArgs = new PostureAlertEventArgs();

        // Assert
        Assert.AreEqual(string.Empty, eventArgs.Message);
    }

    [TestMethod]
    public void PostureAlertEventArgs_DefaultsTimestampToNow()
    {
        // Arrange & Act
        var before = DateTime.Now;
        var eventArgs = new PostureAlertEventArgs();
        var after = DateTime.Now;

        // Assert
        Assert.IsTrue(eventArgs.Timestamp >= before);
        Assert.IsTrue(eventArgs.Timestamp <= after);
    }

    #endregion

    #region AccelerometerDataEventArgs Tests

    [TestMethod]
    public void AccelerometerDataEventArgs_CanBeCreated()
    {
        // Arrange & Act
        var eventArgs = new AccelerometerDataEventArgs
        {
            X = 0.5,
            Y = -0.8,
            Z = 0.3,
            Inclination = 25.5,
            Status = PostureStatus.Warning,
            Timestamp = DateTime.Now
        };

        // Assert
        Assert.AreEqual(0.5, eventArgs.X);
        Assert.AreEqual(-0.8, eventArgs.Y);
        Assert.AreEqual(0.3, eventArgs.Z);
        Assert.AreEqual(25.5, eventArgs.Inclination);
        Assert.AreEqual(PostureStatus.Warning, eventArgs.Status);
        Assert.IsTrue(eventArgs.Timestamp <= DateTime.Now);
    }

    [TestMethod]
    public void AccelerometerDataEventArgs_DefaultsToZeroValues()
    {
        // Arrange & Act
        var eventArgs = new AccelerometerDataEventArgs();

        // Assert
        Assert.AreEqual(0.0, eventArgs.X);
        Assert.AreEqual(0.0, eventArgs.Y);
        Assert.AreEqual(0.0, eventArgs.Z);
        Assert.AreEqual(0.0, eventArgs.Inclination);
    }

    [TestMethod]
    public void AccelerometerDataEventArgs_DefaultsTimestampToNow()
    {
        // Arrange & Act
        var before = DateTime.Now;
        var eventArgs = new AccelerometerDataEventArgs();
        var after = DateTime.Now;

        // Assert
        Assert.IsTrue(eventArgs.Timestamp >= before);
        Assert.IsTrue(eventArgs.Timestamp <= after);
    }

    #endregion

    #region PostureStatus Enum Tests

    [TestMethod]
    public void PostureStatus_HasCorrectValues()
    {
        // Assert
        Assert.AreEqual(0, (int)PostureStatus.Good);
        Assert.AreEqual(1, (int)PostureStatus.Warning);
        Assert.AreEqual(2, (int)PostureStatus.Poor);
    }

    [TestMethod]
    public void PostureStatus_CanBeCompared()
    {
        // Arrange
        var good = PostureStatus.Good;
        var warning = PostureStatus.Warning;
        var poor = PostureStatus.Poor;

        // Assert
        Assert.IsTrue(good == PostureStatus.Good);
        Assert.IsTrue(warning == PostureStatus.Warning);
        Assert.IsTrue(poor == PostureStatus.Poor);
        Assert.IsFalse(good == warning);
        Assert.IsFalse(warning == poor);
    }

    [TestMethod]
    public void PostureStatus_CanBeUsedInSwitchStatement()
    {
        // Arrange
        var status = PostureStatus.Good;
        var result = string.Empty;

        // Act
        switch (status)
        {
            case PostureStatus.Good:
                result = "Good";
                break;
            case PostureStatus.Warning:
                result = "Warning";
                break;
            case PostureStatus.Poor:
                result = "Poor";
                break;
        }

        // Assert
        Assert.AreEqual("Good", result);
    }

    #endregion

    #region Property Change Tests

    [TestMethod]
    public void Sensitivity_ChangeMultipleTimes_WorksCorrectly()
    {
        // Arrange & Act
        _service.Sensitivity = 0.1;
        Assert.AreEqual(0.1, _service.Sensitivity);

        _service.Sensitivity = 0.5;
        Assert.AreEqual(0.5, _service.Sensitivity);

        _service.Sensitivity = 0.9;
        Assert.AreEqual(0.9, _service.Sensitivity);
    }

    [TestMethod]
    public void AlertDelaySeconds_ChangeMultipleTimes_WorksCorrectly()
    {
        // Arrange & Act
        _service.AlertDelaySeconds = 1;
        Assert.AreEqual(1, _service.AlertDelaySeconds);

        _service.AlertDelaySeconds = 10;
        Assert.AreEqual(10, _service.AlertDelaySeconds);

        _service.AlertDelaySeconds = 15;
        Assert.AreEqual(15, _service.AlertDelaySeconds);
    }

    #endregion

    #region Edge Cases

    [TestMethod]
    public void Sensitivity_NegativeValue_IsAllowed()
    {
        // Note: The service doesn't validate the range, so negative values are technically possible
        // In a production scenario, you might want to add validation
        
        // Arrange & Act
        _service.Sensitivity = -0.5;

        // Assert
        Assert.AreEqual(-0.5, _service.Sensitivity);
    }

    [TestMethod]
    public void Sensitivity_ValueGreaterThanOne_IsAllowed()
    {
        // Note: The service doesn't validate the range
        // In a production scenario, you might want to add validation (0.0 to 1.0)
        
        // Arrange & Act
        _service.Sensitivity = 1.5;

        // Assert
        Assert.AreEqual(1.5, _service.Sensitivity);
    }

    [TestMethod]
    public void AlertDelaySeconds_ZeroValue_IsAllowed()
    {
        // Arrange & Act
        _service.AlertDelaySeconds = 0;

        // Assert
        Assert.AreEqual(0, _service.AlertDelaySeconds);
    }

    [TestMethod]
    public void AlertDelaySeconds_NegativeValue_IsAllowed()
    {
        // Note: The service doesn't validate the range
        // In a production scenario, you might want to prevent negative values
        
        // Arrange & Act
        _service.AlertDelaySeconds = -5;

        // Assert
        Assert.AreEqual(-5, _service.AlertDelaySeconds);
    }

    [TestMethod]
    public void AlertDelaySeconds_LargeValue_IsAllowed()
    {
        // Arrange & Act
        _service.AlertDelaySeconds = 1000;

        // Assert
        Assert.AreEqual(1000, _service.AlertDelaySeconds);
    }

    #endregion

    #region Multiple Instance Tests

    [TestMethod]
    public void MultipleInstances_HaveIndependentState()
    {
        // Arrange
        var service1 = new PostureService();
        var service2 = new PostureService();

        // Act
        service1.Sensitivity = 0.2;
        service1.AlertDelaySeconds = 3;

        service2.Sensitivity = 0.8;
        service2.AlertDelaySeconds = 10;

        // Assert
        Assert.AreEqual(0.2, service1.Sensitivity);
        Assert.AreEqual(3, service1.AlertDelaySeconds);
        Assert.AreEqual(0.8, service2.Sensitivity);
        Assert.AreEqual(10, service2.AlertDelaySeconds);
    }

    [TestMethod]
    public void MultipleInstances_CanSubscribeToEventsIndependently()
    {
        // Arrange
        var service1 = new PostureService();
        var service2 = new PostureService();
        
        var service1EventRaised = false;
        var service2EventRaised = false;

        service1.PostureAlert += (sender, args) => service1EventRaised = true;
        service2.PostureAlert += (sender, args) => service2EventRaised = true;

        // Assert - events can be subscribed independently
        Assert.IsFalse(service1EventRaised);
        Assert.IsFalse(service2EventRaised);
    }

    #endregion

    #region Disposal Tests

    [TestMethod]
    public void StopMonitoring_CalledMultipleTimes_IsSafe()
    {
        // Arrange
        var service = new PostureService();

        // Act & Assert - should not throw
        service.StopMonitoring();
        service.StopMonitoring();
        service.StopMonitoring();
        
        Assert.IsFalse(service.IsMonitoring);
    }

    #endregion
}
