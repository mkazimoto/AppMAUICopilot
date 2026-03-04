using CameraApp.Services;
using CameraApp.ViewModels;
using Microsoft.Maui.Dispatching;
using Moq;

namespace CameraApp.Test.ViewModels;

public class PosturePageViewModelTests
{
    private readonly Mock<IPostureService> _postureServiceMock;
    private readonly Mock<IDispatcher> _dispatcherMock;

    public PosturePageViewModelTests()
    {
        _postureServiceMock = new Mock<IPostureService>();
        _postureServiceMock.SetupGet(s => s.Sensitivity).Returns(0.5);
        _postureServiceMock.SetupGet(s => s.AlertDelaySeconds).Returns(10);
        _postureServiceMock.SetupGet(s => s.IsMonitoring).Returns(false);
        _postureServiceMock.SetupAdd(s => s.PostureAlert += It.IsAny<EventHandler<PostureAlertEventArgs>>());
        _postureServiceMock.SetupAdd(s => s.AccelerometerDataUpdated += It.IsAny<EventHandler<AccelerometerDataEventArgs>>());

        _dispatcherMock = new Mock<IDispatcher>();
        // Invoke the action synchronously so tests can assert on UI state immediately
        _dispatcherMock
            .Setup(d => d.Dispatch(It.IsAny<Action>()))
            .Callback<Action>(action => action());
    }

    private PosturePageViewModel CreateSut() =>
        new(_postureServiceMock.Object, _dispatcherMock.Object);

    // ── Constructor ──────────────────────────────────────────────────────────

    [Fact]
    public void Constructor_InitializesSensitivityFromService()
    {
        var sut = CreateSut();

        Assert.Equal(0.5, sut.Sensitivity);
    }

    [Fact]
    public void Constructor_InitializesAlertDelayFromService()
    {
        var sut = CreateSut();

        Assert.Equal(10, sut.AlertDelay);
    }

    [Fact]
    public void Constructor_SetsDefaultStatusMessage()
    {
        var sut = CreateSut();

        Assert.Equal("Pronto para monitorar", sut.StatusMessage);
    }

    [Fact]
    public void Constructor_SetsIsMonitoringFalse()
    {
        var sut = CreateSut();

        Assert.False(sut.IsMonitoring);
    }

    [Fact]
    public void Constructor_SubscribesToPostureAlertEvent()
    {
        var sut = CreateSut();

        _postureServiceMock.VerifyAdd(
            s => s.PostureAlert += It.IsAny<EventHandler<PostureAlertEventArgs>>(),
            Times.Once);
    }

    [Fact]
    public void Constructor_SubscribesToAccelerometerDataUpdatedEvent()
    {
        var sut = CreateSut();

        _postureServiceMock.VerifyAdd(
            s => s.AccelerometerDataUpdated += It.IsAny<EventHandler<AccelerometerDataEventArgs>>(),
            Times.Once);
    }

    // ── StartMonitoringAsync ─────────────────────────────────────────────────

    [Fact]
    public async Task StartMonitoringAsync_SetsServiceSensitivityBeforeStarting()
    {
        var sut = CreateSut();
        sut.Sensitivity = 0.7;

        await sut.StartMonitoringCommand.ExecuteAsync(null);

        _postureServiceMock.VerifySet(s => s.Sensitivity = 0.7, Times.Once);
    }

    [Fact]
    public async Task StartMonitoringAsync_SetsServiceAlertDelayBeforeStarting()
    {
        var sut = CreateSut();
        sut.AlertDelay = 15;

        await sut.StartMonitoringCommand.ExecuteAsync(null);

        _postureServiceMock.VerifySet(s => s.AlertDelaySeconds = 15, Times.Once);
    }

    [Fact]
    public async Task StartMonitoringAsync_CallsServiceStartMonitoringAsync()
    {
        var sut = CreateSut();

        await sut.StartMonitoringCommand.ExecuteAsync(null);

        _postureServiceMock.Verify(s => s.StartMonitoringAsync(), Times.Once);
    }

    [Fact]
    public async Task StartMonitoringAsync_SetsIsMonitoringTrue()
    {
        var sut = CreateSut();

        await sut.StartMonitoringCommand.ExecuteAsync(null);

        Assert.True(sut.IsMonitoring);
    }

    [Fact]
    public async Task StartMonitoringAsync_SetsStatusMessageMonitoramentoAtivo()
    {
        var sut = CreateSut();

        await sut.StartMonitoringCommand.ExecuteAsync(null);

        Assert.Equal("Monitoramento ativo", sut.StatusMessage);
    }

    [Fact]
    public async Task StartMonitoringAsync_ResetsAlertCount()
    {
        var sut = CreateSut();
        sut.AlertCount = 5;

        await sut.StartMonitoringCommand.ExecuteAsync(null);

        Assert.Equal(0, sut.AlertCount);
    }

    [Fact]
    public async Task StartMonitoringAsync_OnException_SetsStatusMessageWithError()
    {
        _postureServiceMock
            .Setup(s => s.StartMonitoringAsync())
            .ThrowsAsync(new InvalidOperationException("sensor not available"));
        var sut = CreateSut();

        await sut.StartMonitoringCommand.ExecuteAsync(null);

        Assert.Contains("sensor not available", sut.StatusMessage);
    }

    [Fact]
    public async Task StartMonitoringAsync_OnException_IsMonitoringRemainsFalse()
    {
        _postureServiceMock
            .Setup(s => s.StartMonitoringAsync())
            .ThrowsAsync(new InvalidOperationException("sensor not available"));
        var sut = CreateSut();

        await sut.StartMonitoringCommand.ExecuteAsync(null);

        Assert.False(sut.IsMonitoring);
    }

    // ── StopMonitoring ───────────────────────────────────────────────────────

    [Fact]
    public void StopMonitoring_CallsServiceStopMonitoring()
    {
        var sut = CreateSut();

        sut.StopMonitoringCommand.Execute(null);

        _postureServiceMock.Verify(s => s.StopMonitoring(), Times.Once);
    }

    [Fact]
    public void StopMonitoring_SetsIsMonitoringFalse()
    {
        var sut = CreateSut();

        sut.StopMonitoringCommand.Execute(null);

        Assert.False(sut.IsMonitoring);
    }

    [Fact]
    public void StopMonitoring_SetsStatusMessageMonitoramentoParado()
    {
        var sut = CreateSut();

        sut.StopMonitoringCommand.Execute(null);

        Assert.Equal("Monitoramento parado", sut.StatusMessage);
    }

    [Fact]
    public void StopMonitoring_ResetsAccelerometerValues()
    {
        var sut = CreateSut();

        sut.StopMonitoringCommand.Execute(null);

        Assert.Equal(0, sut.AccelerometerX);
        Assert.Equal(0, sut.AccelerometerY);
        Assert.Equal(0, sut.AccelerometerZ);
        Assert.Equal(0, sut.Inclination);
    }

    [Fact]
    public void StopMonitoring_SetsPostureStatusToDesconhecido()
    {
        var sut = CreateSut();

        sut.StopMonitoringCommand.Execute(null);

        Assert.Equal("Desconhecido", sut.PostureStatus);
    }

    // ── ResetStats ───────────────────────────────────────────────────────────

    [Fact]
    public void ResetStats_ClearsAlertCount()
    {
        var sut = CreateSut();
        sut.AlertCount = 7;

        sut.ResetStatsCommand.Execute(null);

        Assert.Equal(0, sut.AlertCount);
    }

    [Fact]
    public void ResetStats_ClearsLastAlertMessage()
    {
        var sut = CreateSut();
        sut.LastAlertMessage = "Some alert";

        sut.ResetStatsCommand.Execute(null);

        Assert.Equal("", sut.LastAlertMessage);
    }

    [Fact]
    public void ResetStats_ResetsLastAlertTimeToMinValue()
    {
        var sut = CreateSut();
        sut.LastAlertTime = DateTime.Now;

        sut.ResetStatsCommand.Execute(null);

        Assert.Equal(DateTime.MinValue, sut.LastAlertTime);
    }

    // ── OnSensitivityChanged ─────────────────────────────────────────────────

    [Fact]
    public void OnSensitivityChanged_WhenMonitoring_UpdatesServiceSensitivity()
    {
        _postureServiceMock.SetupGet(s => s.IsMonitoring).Returns(true);
        var sut = CreateSut();

        sut.Sensitivity = 0.8;

        _postureServiceMock.VerifySet(s => s.Sensitivity = 0.8, Times.Once);
    }

    [Fact]
    public void OnSensitivityChanged_WhenNotMonitoring_DoesNotUpdateServiceSensitivity()
    {
        _postureServiceMock.SetupGet(s => s.IsMonitoring).Returns(false);
        var sut = CreateSut();

        sut.Sensitivity = 0.9;

        _postureServiceMock.VerifySet(s => s.Sensitivity = 0.9, Times.Never);
    }

    // ── OnAlertDelayChanged ──────────────────────────────────────────────────

    [Fact]
    public void OnAlertDelayChanged_WhenMonitoring_UpdatesServiceAlertDelay()
    {
        _postureServiceMock.SetupGet(s => s.IsMonitoring).Returns(true);
        var sut = CreateSut();

        sut.AlertDelay = 20;

        _postureServiceMock.VerifySet(s => s.AlertDelaySeconds = 20, Times.Once);
    }

    [Fact]
    public void OnAlertDelayChanged_WhenNotMonitoring_DoesNotUpdateServiceAlertDelay()
    {
        _postureServiceMock.SetupGet(s => s.IsMonitoring).Returns(false);
        var sut = CreateSut();

        sut.AlertDelay = 20;

        _postureServiceMock.VerifySet(s => s.AlertDelaySeconds = 20, Times.Never);
    }

    // ── AccelerometerDataUpdated event ───────────────────────────────────────

    [Fact]
    public void AccelerometerDataUpdated_UpdatesAccelerometerProperties()
    {
        var sut = CreateSut();
        var args = new AccelerometerDataEventArgs
        {
            X = 1.1,
            Y = 2.2,
            Z = 3.3,
            Inclination = 45.0,
            Status = PostureStatus.Good
        };

        _postureServiceMock.Raise(
            s => s.AccelerometerDataUpdated += null,
            _postureServiceMock.Object,
            args);

        Assert.Equal(1.1, sut.AccelerometerX);
        Assert.Equal(2.2, sut.AccelerometerY);
        Assert.Equal(3.3, sut.AccelerometerZ);
        Assert.Equal(45.0, sut.Inclination);
    }

    [Theory]
    [InlineData(PostureStatus.Good, "Boa Postura")]
    [InlineData(PostureStatus.Warning, "Atenção")]
    [InlineData(PostureStatus.Poor, "Postura Ruim")]
    public void AccelerometerDataUpdated_SetsCorrectPostureStatus(
        PostureStatus status, string expectedText)
    {
        var sut = CreateSut();
        var args = new AccelerometerDataEventArgs
        {
            X = 0,
            Y = 0,
            Z = 0,
            Inclination = 0,
            Status = status
        };

        _postureServiceMock.Raise(
            s => s.AccelerometerDataUpdated += null,
            _postureServiceMock.Object,
            args);

        Assert.Equal(expectedText, sut.PostureStatus);
    }

    // ── PostureAlert event ───────────────────────────────────────────────────

    [Fact]
    public void PostureAlert_SetsLastAlertMessage()
    {
        var sut = CreateSut();
        var timestamp = new DateTime(2024, 6, 1, 12, 0, 0);
        var args = new PostureAlertEventArgs { Message = "Corrija sua postura", Timestamp = timestamp };

        _postureServiceMock.Raise(
            s => s.PostureAlert += null,
            _postureServiceMock.Object,
            args);

        Assert.Equal("Corrija sua postura", sut.LastAlertMessage);
    }

    [Fact]
    public void PostureAlert_SetsLastAlertTime()
    {
        var sut = CreateSut();
        var timestamp = new DateTime(2024, 6, 1, 12, 0, 0);
        var args = new PostureAlertEventArgs { Message = "Corrija sua postura", Timestamp = timestamp };

        _postureServiceMock.Raise(
            s => s.PostureAlert += null,
            _postureServiceMock.Object,
            args);

        Assert.Equal(timestamp, sut.LastAlertTime);
    }

    [Fact]
    public void PostureAlert_IncrementsAlertCount()
    {
        var sut = CreateSut();
        var args = new PostureAlertEventArgs { Message = "Alert", Timestamp = DateTime.Now };

        _postureServiceMock.Raise(s => s.PostureAlert += null, _postureServiceMock.Object, args);
        _postureServiceMock.Raise(s => s.PostureAlert += null, _postureServiceMock.Object, args);

        Assert.Equal(2, sut.AlertCount);
    }

    [Fact]
    public void PostureAlert_UpdatesStatusMessageWithAlertPrefix()
    {
        var sut = CreateSut();
        var args = new PostureAlertEventArgs { Message = "Mensagem de alerta", Timestamp = DateTime.Now };

        _postureServiceMock.Raise(
            s => s.PostureAlert += null,
            _postureServiceMock.Object,
            args);

        Assert.Equal("Alerta: Mensagem de alerta", sut.StatusMessage);
    }
}
