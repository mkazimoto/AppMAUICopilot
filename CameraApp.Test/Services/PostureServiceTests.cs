using System.Numerics;
using CameraApp.Services;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Extensions.Logging;
using Moq;

namespace CameraApp.Test.Services;

/// <summary>
/// Unit tests for <see cref="PostureService" />.
/// </summary>
public class PostureServiceTests : IDisposable
{
    // Default Sensitivity = 0.3  →  goodThreshold = 10.5°,  warningThreshold = 25.5°
    private readonly Mock<IAccelerometer> _accelerometerMock = new();
    private readonly Mock<IVibration> _vibrationMock = new();
    private readonly Mock<ILogger<PostureService>> _loggerMock = new();
    private DateTime _fakeNow = new(2026, 1, 1, 12, 0, 0);
    private readonly PostureService _sut;

    public PostureServiceTests()
    {
        _accelerometerMock.Setup(a => a.IsSupported).Returns(true);
        _accelerometerMock.Setup(a => a.IsMonitoring).Returns(false);
        _vibrationMock.Setup(v => v.IsSupported).Returns(true);

        _sut = new PostureService(_accelerometerMock.Object, _vibrationMock.Object, _loggerMock.Object);
    }

    public void Dispose() => _sut.StopMonitoring();

    // ── CalculateInclination ──────────────────────────────────────────────────

    [Theory]
    [InlineData(0.0, -1.0, 0.0, 0.0)]        // perfectly vertical → 0°
    [InlineData(0.0, 0.0, 0.0, 90.0)]        // y = 0 special-case → 90°
    [InlineData(1.0, 0.0, 0.0, 90.0)]        // fully horizontal → 90°
    [InlineData(1.0, -1.0, 0.0, 45.0)]       // 45° tilt (atan2(1,1))
    [InlineData(0.0, -0.866, 0.5, 30.0)]     // 30° tilt (atan2(0.5, 0.866))
    public void CalculateInclination_WithKnownValues_ReturnsExpectedDegrees(
        double x, double y, double z, double expectedDegrees)
    {
        var result = _sut.CalculateInclination(x, y, z);

        Assert.Equal(expectedDegrees, result, precision: 0); // 0 decimal places
    }

    [Fact]
    public void CalculateInclination_WhenYIsNegativeOne_ReturnsZero()
    {
        var result = _sut.CalculateInclination(0, -1, 0);

        Assert.Equal(0.0, result, precision: 5);
    }

    // ── DeterminePostureStatus — default Sensitivity = 0.3 ───────────────────
    // goodThreshold = 15 × (1 − 0.3) = 10.5°
    // warningThreshold = 30 × (1 − 0.15) = 25.5°

    [Theory]
    [InlineData(0.0, PostureStatus.Good)]
    [InlineData(10.0, PostureStatus.Good)]
    [InlineData(10.5, PostureStatus.Good)]
    [InlineData(11.0, PostureStatus.Warning)]
    [InlineData(25.5, PostureStatus.Warning)]
    [InlineData(26.0, PostureStatus.Poor)]
    [InlineData(60.0, PostureStatus.Poor)]
    public void DeterminePostureStatus_WithDefaultSensitivity_ReturnsExpectedStatus(
        double inclination, PostureStatus expected)
    {
        var result = _sut.DeterminePostureStatus(inclination);

        Assert.Equal(expected, result);
    }

    // Sensitivity = 0.0  →  goodThreshold = 15.0°,  warningThreshold = 30.0°
    [Theory]
    [InlineData(14.9, PostureStatus.Good)]
    [InlineData(15.0, PostureStatus.Good)]
    [InlineData(15.1, PostureStatus.Warning)]
    [InlineData(30.0, PostureStatus.Warning)]
    [InlineData(30.1, PostureStatus.Poor)]
    public void DeterminePostureStatus_WithZeroSensitivity_UsesWidestThresholds(
        double inclination, PostureStatus expected)
    {
        _sut.Sensitivity = 0.0;

        var result = _sut.DeterminePostureStatus(inclination);

        Assert.Equal(expected, result);
    }

    // Sensitivity = 1.0  →  goodThreshold = 0°,  warningThreshold = 15.0°
    [Theory]
    [InlineData(0.0, PostureStatus.Good)]
    [InlineData(0.1, PostureStatus.Warning)]
    [InlineData(15.0, PostureStatus.Warning)]
    [InlineData(15.1, PostureStatus.Poor)]
    public void DeterminePostureStatus_WithMaxSensitivity_UsesNarrowestThresholds(
        double inclination, PostureStatus expected)
    {
        _sut.Sensitivity = 1.0;

        var result = _sut.DeterminePostureStatus(inclination);

        Assert.Equal(expected, result);
    }

    // ── StartMonitoringAsync ──────────────────────────────────────────────────

    [Fact]
    public async Task StartMonitoringAsync_WhenAccelerometerNotSupported_ThrowsInvalidOperationException()
    {
        _accelerometerMock.Setup(a => a.IsSupported).Returns(false);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.StartMonitoringAsync());

        Assert.IsType<NotSupportedException>(ex.InnerException);
    }

    [Fact]
    public async Task StartMonitoringAsync_WhenCalled_SetsIsMonitoringTrue()
    {
        await _sut.StartMonitoringAsync();

        Assert.True(_sut.IsMonitoring);
    }

    [Fact]
    public async Task StartMonitoringAsync_WhenCalled_StartsAccelerometerAndSubscribesToEvents()
    {
        await _sut.StartMonitoringAsync();

        _accelerometerMock.Verify(a => a.Start(SensorSpeed.Default), Times.Once);
        _accelerometerMock.VerifyAdd(a => a.ReadingChanged += It.IsAny<EventHandler<AccelerometerChangedEventArgs>>(), Times.Once);
    }

    [Fact]
    public async Task StartMonitoringAsync_WhenCalledTwice_StartsAccelerometerOnlyOnce()
    {
        await _sut.StartMonitoringAsync();
        await _sut.StartMonitoringAsync(); // second call should no-op

        _accelerometerMock.Verify(a => a.Start(It.IsAny<SensorSpeed>()), Times.Once);
    }

    // ── StopMonitoring ────────────────────────────────────────────────────────

    [Fact]
    public void StopMonitoring_WhenNotMonitoring_DoesNotCallAccelerometer()
    {
        Assert.False(_sut.IsMonitoring);

        _sut.StopMonitoring();

        _accelerometerMock.Verify(a => a.Stop(), Times.Never);
    }

    [Fact]
    public async Task StopMonitoring_AfterStarting_SetsIsMonitoringFalse()
    {
        await _sut.StartMonitoringAsync();

        _sut.StopMonitoring();

        Assert.False(_sut.IsMonitoring);
    }

    [Fact]
    public async Task StopMonitoring_WhenAccelerometerIsRunning_StopsAndUnsubscribes()
    {
        _accelerometerMock.Setup(a => a.IsMonitoring).Returns(true);
        await _sut.StartMonitoringAsync();

        _sut.StopMonitoring();

        _accelerometerMock.Verify(a => a.Stop(), Times.Once);
        _accelerometerMock.VerifyRemove(a => a.ReadingChanged -= It.IsAny<EventHandler<AccelerometerChangedEventArgs>>(), Times.Once);
    }

    // ── ProcessReading ────────────────────────────────────────────────────────

    // Upright reading: (0, -1, 0)  →  inclination 0°  →  Good
    private static readonly Vector3 GoodPostureReading = new(0f, -1f, 0f);

    // Severe tilt: (0.8, -0.6, 0)  →  atan2(0.8, 0.6) ≈ 53°  →  Poor
    private static readonly Vector3 PoorPostureReading = new(0.8f, -0.6f, 0f);

    [Fact]
    public void ProcessReading_RaisesAccelerometerDataUpdated_WithCorrectCoordinates()
    {
        AccelerometerDataEventArgs? received = null;
        _sut.AccelerometerDataUpdated += (_, e) => received = e;

        _sut.ProcessReading(GoodPostureReading);

        Assert.NotNull(received);
        Assert.Equal(GoodPostureReading.X, received.X);
        Assert.Equal(GoodPostureReading.Y, received.Y);
        Assert.Equal(GoodPostureReading.Z, received.Z);
    }

    [Fact]
    public void ProcessReading_GoodPostureReading_RaisesEventWithGoodStatus()
    {
        AccelerometerDataEventArgs? received = null;
        _sut.AccelerometerDataUpdated += (_, e) => received = e;

        _sut.ProcessReading(GoodPostureReading);

        Assert.NotNull(received);
        Assert.Equal(PostureStatus.Good, received.Status);
    }

    [Fact]
    public void ProcessReading_PoorPostureReading_RaisesEventWithPoorStatus()
    {
        AccelerometerDataEventArgs? received = null;
        _sut.AccelerometerDataUpdated += (_, e) => received = e;

        _sut.ProcessReading(PoorPostureReading);

        Assert.NotNull(received);
        Assert.Equal(PostureStatus.Poor, received.Status);
    }

    [Fact]
    public void ProcessReading_FirstPoorPostureReading_DoesNotImmediatelyRaiseAlert()
    {
        var alerts = new List<PostureAlertEventArgs>();
        _sut.PostureAlert += (_, e) => alerts.Add(e);

        _sut.ProcessReading(PoorPostureReading); // starts the poor-posture clock

        Assert.Empty(alerts);
    }

    [Fact]
    public void ProcessReading_PoorPostureBeforeDelayExpires_DoesNotRaiseAlert()
    {
        _sut.AlertDelaySeconds = 5;
        var alerts = new List<PostureAlertEventArgs>();
        _sut.PostureAlert += (_, e) => alerts.Add(e);

        _sut.ProcessReading(PoorPostureReading);       // sets poor-posture start time
        _fakeNow = _fakeNow.AddSeconds(4);             // advance clock — still within delay
        _sut.ProcessReading(PoorPostureReading);

        Assert.Empty(alerts);
    }

    [Fact]
    public void ProcessReading_PoorPostureAfterDelayExpires_RaisesPostureAlert()
    {
        _sut.AlertDelaySeconds = 5;
        var alerts = new List<PostureAlertEventArgs>();
        _sut.PostureAlert += (_, e) => alerts.Add(e);

        _sut.ProcessReading(PoorPostureReading);       // sets poor-posture start time
        _fakeNow = _fakeNow.AddSeconds(6);             // advance clock past delay
        _sut.ProcessReading(PoorPostureReading);

        Assert.Single(alerts);
        Assert.Equal(PostureStatus.Poor, alerts[0].Status);
        Assert.Contains("Inclinação:", alerts[0].Message);
    }

    [Fact]
    public void ProcessReading_AlertFired_VibratesDevice()
    {
        _sut.AlertDelaySeconds = 5;

        _sut.ProcessReading(PoorPostureReading);
        _fakeNow = _fakeNow.AddSeconds(6);
        _sut.ProcessReading(PoorPostureReading);

        _vibrationMock.Verify(v => v.Vibrate(TimeSpan.FromMilliseconds(500)), Times.Once);
    }

    [Fact]
    public void ProcessReading_AlertFired_DoesNotVibrate_WhenVibrationNotSupported()
    {
        _vibrationMock.Setup(v => v.IsSupported).Returns(false);
        _sut.AlertDelaySeconds = 5;

        _sut.ProcessReading(PoorPostureReading);
        _fakeNow = _fakeNow.AddSeconds(6);
        _sut.ProcessReading(PoorPostureReading);

        _vibrationMock.Verify(v => v.Vibrate(It.IsAny<TimeSpan>()), Times.Never);
    }

    [Fact]
    public void ProcessReading_WhenPostureImproves_DoesNotRaiseAlert()
    {
        _sut.AlertDelaySeconds = 5;
        var alerts = new List<PostureAlertEventArgs>();
        _sut.PostureAlert += (_, e) => alerts.Add(e);

        _sut.ProcessReading(PoorPostureReading);       // starts poor-posture timer
        _sut.ProcessReading(GoodPostureReading);       // resets poor-posture state

        _fakeNow = _fakeNow.AddSeconds(6);
        _sut.ProcessReading(PoorPostureReading);       // poor-posture clock restarts here

        Assert.Empty(alerts);
    }

    [Fact]
    public void ProcessReading_RepeatedAlerts_ResetsTimerAfterEachAlert()
    {
        _sut.AlertDelaySeconds = 5;
        var alerts = new List<PostureAlertEventArgs>();
        _sut.PostureAlert += (_, e) => alerts.Add(e);

        _sut.ProcessReading(PoorPostureReading);
        _fakeNow = _fakeNow.AddSeconds(6);
        _sut.ProcessReading(PoorPostureReading); // first alert

        _fakeNow = _fakeNow.AddSeconds(6);
        _sut.ProcessReading(PoorPostureReading); // second alert

        Assert.Equal(2, alerts.Count);
    }
}
