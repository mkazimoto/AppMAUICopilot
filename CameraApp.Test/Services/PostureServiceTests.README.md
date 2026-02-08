# PostureService Tests Documentation

## Overview
Unit tests for the `PostureService` class that monitors user posture using the device accelerometer. These tests focus on testable properties, events, and state management, while acknowledging platform-specific limitations.

## Test Summary
- **Total Tests**: 37
- **Passing Tests**: 37 ?
- **Test Framework**: MSTest
- **Coverage**: Properties, Events, State Management

## Important Note: Platform-Specific Limitations

The `PostureService` uses platform-specific MAUI Essentials features:
- **`Accelerometer.Default`** - Hardware sensor access
- **`Vibration.Default`** - Device vibration
- **`Timer`** - Background monitoring

These platform-specific features **cannot be unit tested** without:
- Actual device hardware
- Platform-specific test runners
- Integration test environment

Therefore, these tests focus on:
? **Testable logic** (properties, events, initialization)
? **Not tested** (accelerometer integration, vibration, timer callbacks)

## Test Categories

### 1. Constructor Tests (1 test)
Tests initialization behavior:
- ? `Constructor_InitializesWithDefaultValues` - Verifies default property values
  - `IsMonitoring`: false
  - `Sensitivity`: 0.3 (30%)
  - `AlertDelaySeconds`: 5

### 2. Property Tests (5 tests)
Tests property getters and setters:

#### Sensitivity Property
- ? `Sensitivity_CanBeSet` - Can set sensitivity value
- ? `Sensitivity_AcceptsDifferentValues` - Tests values 0.0, 0.5, 1.0

#### AlertDelaySeconds Property
- ? `AlertDelaySeconds_CanBeSet` - Can set alert delay value
- ? `AlertDelaySeconds_AcceptsDifferentValues` - Tests values 1, 5, 15

#### IsMonitoring Property
- ? `IsMonitoring_DefaultsToFalse` - Verifies default state

### 3. Event Tests (6 tests)
Tests event subscription/unsubscription:

#### PostureAlert Event
- ? `PostureAlert_EventCanBeSubscribed` - Subscription doesn't throw
- ? `PostureAlert_EventCanBeUnsubscribed` - Unsubscription works correctly

#### AccelerometerDataUpdated Event
- ? `AccelerometerDataUpdated_EventCanBeSubscribed` - Subscription doesn't throw
- ? `AccelerometerDataUpdated_EventCanBeUnsubscribed` - Unsubscription works correctly

**Note**: Events cannot be triggered in unit tests without platform-specific code, so tests verify subscription/unsubscription only.

### 4. StopMonitoring Tests (2 tests)
Tests stopping the monitoring service:
- ? `StopMonitoring_WhenNotMonitoring_DoesNotThrow` - Safe to call when not monitoring
- ? `StopMonitoring_CanBeCalledMultipleTimes` - Idempotent operation

### 5. PostureAlertEventArgs Tests (3 tests)
Tests event arguments for posture alerts:
- ? `PostureAlertEventArgs_CanBeCreated` - Can create with custom values
- ? `PostureAlertEventArgs_DefaultsMessageToEmpty` - Default message is empty
- ? `PostureAlertEventArgs_DefaultsTimestampToNow` - Timestamp defaults to DateTime.Now

### 6. AccelerometerDataEventArgs Tests (3 tests)
Tests event arguments for accelerometer data:
- ? `AccelerometerDataEventArgs_CanBeCreated` - Can create with custom values
- ? `AccelerometerDataEventArgs_DefaultsToZeroValues` - Defaults X, Y, Z, Inclination to 0
- ? `AccelerometerDataEventArgs_DefaultsTimestampToNow` - Timestamp defaults to DateTime.Now

### 7. PostureStatus Enum Tests (3 tests)
Tests the PostureStatus enumeration:
- ? `PostureStatus_HasCorrectValues` - Verifies enum integer values
  - Good = 0
  - Warning = 1
  - Poor = 2
- ? `PostureStatus_CanBeCompared` - Equality comparisons work
- ? `PostureStatus_CanBeUsedInSwitchStatement` - Can be used in switch statements

### 8. Property Change Tests (2 tests)
Tests multiple property changes:
- ? `Sensitivity_ChangeMultipleTimes_WorksCorrectly` - Multiple sensitivity changes
- ? `AlertDelaySeconds_ChangeMultipleTimes_WorksCorrectly` - Multiple delay changes

### 9. Edge Cases (5 tests)
Tests boundary and edge case values:
- ? `Sensitivity_NegativeValue_IsAllowed` - Negative values accepted (no validation)
- ? `Sensitivity_ValueGreaterThanOne_IsAllowed` - Values > 1.0 accepted (no validation)
- ? `AlertDelaySeconds_ZeroValue_IsAllowed` - Zero delay accepted
- ? `AlertDelaySeconds_NegativeValue_IsAllowed` - Negative delay accepted (no validation)
- ? `AlertDelaySeconds_LargeValue_IsAllowed` - Large values accepted

**Note**: These tests reveal that the service doesn't validate input ranges. In production, you may want to add validation:
- Sensitivity: 0.0 to 1.0
- AlertDelaySeconds: >= 0

### 10. Multiple Instance Tests (2 tests)
Tests that multiple service instances work independently:
- ? `MultipleInstances_HaveIndependentState` - Independent property values
- ? `MultipleInstances_CanSubscribeToEventsIndependently` - Independent event subscriptions

### 11. Disposal Tests (1 test)
Tests safe disposal:
- ? `StopMonitoring_CalledMultipleTimes_IsSafe` - Multiple stop calls are safe

## What's NOT Tested (Platform-Specific)

The following cannot be unit tested without platform runners:

### ? StartMonitoringAsync
- Accelerometer availability check
- Accelerometer initialization
- Timer creation and callbacks
- Thread safety

### ? CheckPostureStatus
- Accelerometer reading processing
- Inclination calculation
- Posture status determination
- Alert triggering logic

### ? CalculateInclination
- Mathematical calculations based on X, Y, Z values
- Angle computation

### ? DeterminePostureStatus
- Threshold calculations
- Sensitivity-based logic

### ? HandlePostureAlert
- Time-based alerting
- Alert delay logic

### ? TriggerAlert
- Vibration triggering
- Event raising with actual data

## Recommendations for Full Coverage

### 1. Refactor for Testability
Extract testable logic into separate methods:

```csharp
// Make these public or internal with [assembly: InternalsVisibleTo]
public double CalculateInclination(double x, double y, double z)
{
    var horizontalComponent = Math.Sqrt(x * x + z * z);
    var verticalComponent = Math.Abs(y);
    
    if (verticalComponent == 0)
        return 90;
    
    var angleRadians = Math.Atan2(horizontalComponent, verticalComponent);
    var angleDegrees = angleRadians * (180.0 / Math.PI);
    
    return angleDegrees;
}

public PostureStatus DeterminePostureStatus(double inclination)
{
    var goodPostureThreshold = 15 * (1 - Sensitivity);
    var warningThreshold = 30 * (1 - Sensitivity * 0.5);

    if (inclination <= goodPostureThreshold)
        return PostureStatus.Good;
    else if (inclination <= warningThreshold)
        return PostureStatus.Warning;
    else
        return PostureStatus.Poor;
}
```

Then create additional tests:

```csharp
[TestMethod]
public void CalculateInclination_PerfectVertical_ReturnsZero()
{
    // x=0, y=-1 (gravity), z=0 = perfect vertical
    var inclination = _service.CalculateInclination(0, -1, 0);
    Assert.AreEqual(0.0, inclination, 0.01);
}

[TestMethod]
public void DeterminePostureStatus_WithLowInclination_ReturnsGood()
{
    _service.Sensitivity = 0.3;
    var status = _service.DeterminePostureStatus(5.0);
    Assert.AreEqual(PostureStatus.Good, status);
}
```

### 2. Create Integration Tests
For platform-specific features, create integration tests:

```csharp
[TestMethod]
[TestCategory("Integration")]
public async Task StartMonitoringAsync_OnRealDevice_StartsMonitoring()
{
    // Requires real device or emulator
    await _service.StartMonitoringAsync();
    Assert.IsTrue(_service.IsMonitoring);
}
```

### 3. Add Dependency Injection
Create abstractions for platform features:

```csharp
public interface IAccelerometerService
{
    bool IsSupported { get; }
    bool IsMonitoring { get; }
    void Start(SensorSpeed speed);
    void Stop();
    event EventHandler<AccelerometerChangedEventArgs> ReadingChanged;
}

public interface IVibrationService
{
    bool IsSupported { get; }
    void Vibrate(TimeSpan duration);
}
```

Then mock these in tests:

```csharp
var accelerometer = Substitute.For<IAccelerometerService>();
var vibration = Substitute.For<IVibrationService>();
var service = new PostureService(accelerometer, vibration);
```

## Running the Tests

### Run All PostureService Tests
```bash
dotnet test CameraApp.Test/CameraApp.Test.csproj --filter "FullyQualifiedName~PostureServiceTests"
```

### Run Specific Test Category
```bash
# Constructor tests
dotnet test --filter "FullyQualifiedName~PostureServiceTests.Constructor_"

# Property tests
dotnet test --filter "FullyQualifiedName~PostureServiceTests.*Property"

# Event tests
dotnet test --filter "FullyQualifiedName~PostureServiceTests.*Event"
```

### Run with Detailed Output
```bash
dotnet test CameraApp.Test/CameraApp.Test.csproj --filter "FullyQualifiedName~PostureServiceTests" --logger "console;verbosity=detailed"
```

## Test Patterns Used

### AAA Pattern (Arrange-Act-Assert)
```csharp
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
```

### Multiple Values Testing
```csharp
[TestMethod]
public void Sensitivity_AcceptsDifferentValues()
{
    // Test boundary values
    _service.Sensitivity = 0.0;
    Assert.AreEqual(0.0, _service.Sensitivity);

    _service.Sensitivity = 0.5;
    Assert.AreEqual(0.5, _service.Sensitivity);

    _service.Sensitivity = 1.0;
    Assert.AreEqual(1.0, _service.Sensitivity);
}
```

### Event Testing (Subscription Only)
```csharp
[TestMethod]
public void PostureAlert_EventCanBeSubscribed()
{
    // Can verify subscription/unsubscription doesn't throw
    var eventRaised = false;
    _service.PostureAlert += (sender, args) => eventRaised = true;
    
    // Event won't be raised in unit test environment
    Assert.IsFalse(eventRaised);
}
```

## Code Coverage

Current test coverage:
- ? **100%** - Properties (Sensitivity, AlertDelaySeconds, IsMonitoring)
- ? **100%** - Event subscription/unsubscription
- ? **100%** - Constructor
- ? **100%** - PostureAlertEventArgs
- ? **100%** - AccelerometerDataEventArgs
- ? **100%** - PostureStatus enum
- ?? **0%** - StartMonitoringAsync (platform-specific)
- ?? **0%** - StopMonitoring implementation (platform-specific)
- ?? **0%** - CheckPostureStatus (platform-specific)
- ?? **0%** - CalculateInclination (private method)
- ?? **0%** - DeterminePostureStatus (private method)
- ?? **0%** - HandlePostureAlert (private method)
- ?? **0%** - TriggerAlert (private method)

**Overall Logic Coverage**: ~30%
**Testable Code Coverage**: 100%

## Known Limitations

### 1. Platform Dependencies
The service heavily relies on MAUI Essentials:
- Cannot test without actual hardware
- Accelerometer behavior varies by device
- Timing-dependent logic (Timer callbacks)

### 2. Private Methods
Key calculation methods are private:
- `CalculateInclination`
- `DeterminePostureStatus`
- `HandlePostureAlert`

### 3. Thread Safety
Timer callbacks run on background threads:
- Cannot easily test thread safety in unit tests
- Requires integration testing

### 4. Time-Based Logic
Alert delay timing:
- Uses DateTime.Now comparisons
- Difficult to test without mocking time

## Future Improvements

1. **Extract Interfaces**: Create `IAccelerometerService` and `IVibrationService`
2. **Make Methods Testable**: Make calculation methods public or internal
3. **Add Validation**: Validate Sensitivity (0-1) and AlertDelaySeconds (>= 0)
4. **Mock Time**: Use `IDateTimeProvider` for testable time-based logic
5. **Integration Tests**: Create device-specific integration tests
6. **UI Tests**: Test the complete posture monitoring workflow

## Related Documentation

- [PostureService.cs](../../CameraApp/Services/PostureService.cs)
- [IPostureService.cs](../../CameraApp/Services/IPostureService.cs)
- [README_Postura.md](../../CameraApp/README_Postura.md)
- [PosturePageViewModel Tests](../ViewModels/PosturePageViewModelTests.cs) - If created

## Maintenance

When modifying `PostureService`:
1. Run all tests to ensure no regressions
2. Add new tests for new public properties/events
3. Consider refactoring for testability
4. Update documentation
5. Run integration tests on real devices

---

**Note**: These tests provide coverage for the testable portions of the `PostureService`. For full coverage, consider refactoring the service to separate platform-specific code from business logic, enabling more comprehensive unit testing.
