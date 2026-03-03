using CameraApp.Services;
using CameraApp.ViewModels;
using Moq;

namespace CameraApp.Test.ViewModels;

public class CameraPageViewModelTests
{
    private readonly Mock<ICameraService> _cameraServiceMock;

    public CameraPageViewModelTests()
    {
        _cameraServiceMock = new Mock<ICameraService>();
    }

    private CameraPageViewModel CreateSut() =>
        new(_cameraServiceMock.Object);

    // ── Initial state ────────────────────────────────────────────────────────

    [Fact]
    public void InitialState_PhotoPath_IsNull()
    {
        var sut = CreateSut();
        Assert.Null(sut.PhotoPath);
    }

    [Fact]
    public void InitialState_HasPhoto_IsFalse()
    {
        var sut = CreateSut();
        Assert.False(sut.HasPhoto);
    }

    // ── TakePhotoAsync ───────────────────────────────────────────────────────

    [Fact]
    public async Task TakePhotoAsync_WhenPhotoReturned_SetsPhotoPath()
    {
        // Arrange
        const string expectedPath = "/storage/photos/photo.jpg";
        _cameraServiceMock
            .Setup(s => s.TakePhotoAsync())
            .ReturnsAsync(expectedPath);
        var sut = CreateSut();

        // Act
        await sut.TakePhotoCommand.ExecuteAsync(null);

        // Assert
        Assert.Equal(expectedPath, sut.PhotoPath);
    }

    [Fact]
    public async Task TakePhotoAsync_WhenPhotoReturned_SetsHasPhotoTrue()
    {
        // Arrange
        _cameraServiceMock
            .Setup(s => s.TakePhotoAsync())
            .ReturnsAsync("/storage/photos/photo.jpg");
        var sut = CreateSut();

        // Act
        await sut.TakePhotoCommand.ExecuteAsync(null);

        // Assert
        Assert.True(sut.HasPhoto);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task TakePhotoAsync_WhenNullOrEmptyReturned_DoesNotSetPhotoPath(string? returnedPath)
    {
        // Arrange
        _cameraServiceMock
            .Setup(s => s.TakePhotoAsync())
            .ReturnsAsync(returnedPath);
        var sut = CreateSut();

        // Act
        await sut.TakePhotoCommand.ExecuteAsync(null);

        // Assert
        Assert.Null(sut.PhotoPath);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task TakePhotoAsync_WhenNullOrEmptyReturned_HasPhotoRemainsFlase(string? returnedPath)
    {
        // Arrange
        _cameraServiceMock
            .Setup(s => s.TakePhotoAsync())
            .ReturnsAsync(returnedPath);
        var sut = CreateSut();

        // Act
        await sut.TakePhotoCommand.ExecuteAsync(null);

        // Assert
        Assert.False(sut.HasPhoto);
    }

    // ── PickPhotoAsync ───────────────────────────────────────────────────────

    [Fact]
    public async Task PickPhotoAsync_WhenPhotoReturned_SetsPhotoPath()
    {
        // Arrange
        const string expectedPath = "/storage/photos/picked.jpg";
        _cameraServiceMock
            .Setup(s => s.PickPhotoAsync())
            .ReturnsAsync(expectedPath);
        var sut = CreateSut();

        // Act
        await sut.PickPhotoCommand.ExecuteAsync(null);

        // Assert
        Assert.Equal(expectedPath, sut.PhotoPath);
    }

    [Fact]
    public async Task PickPhotoAsync_WhenPhotoReturned_SetsHasPhotoTrue()
    {
        // Arrange
        _cameraServiceMock
            .Setup(s => s.PickPhotoAsync())
            .ReturnsAsync("/storage/photos/picked.jpg");
        var sut = CreateSut();

        // Act
        await sut.PickPhotoCommand.ExecuteAsync(null);

        // Assert
        Assert.True(sut.HasPhoto);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task PickPhotoAsync_WhenNullOrEmptyReturned_DoesNotSetPhotoPath(string? returnedPath)
    {
        // Arrange
        _cameraServiceMock
            .Setup(s => s.PickPhotoAsync())
            .ReturnsAsync(returnedPath);
        var sut = CreateSut();

        // Act
        await sut.PickPhotoCommand.ExecuteAsync(null);

        // Assert
        Assert.Null(sut.PhotoPath);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task PickPhotoAsync_WhenNullOrEmptyReturned_HasPhotoRemainsFalse(string? returnedPath)
    {
        // Arrange
        _cameraServiceMock
            .Setup(s => s.PickPhotoAsync())
            .ReturnsAsync(returnedPath);
        var sut = CreateSut();

        // Act
        await sut.PickPhotoCommand.ExecuteAsync(null);

        // Assert
        Assert.False(sut.HasPhoto);
    }

    // ── ClearPhoto ───────────────────────────────────────────────────────────

    [Fact]
    public void ClearPhoto_AfterTakingPhoto_ClearsPhotoPath()
    {
        // Arrange
        var sut = CreateSut();
        sut.PhotoPath = "/storage/photos/photo.jpg";
        sut.HasPhoto = true;

        // Act
        sut.ClearPhotoCommand.Execute(null);

        // Assert
        Assert.Null(sut.PhotoPath);
    }

    [Fact]
    public void ClearPhoto_AfterTakingPhoto_SetsHasPhotoFalse()
    {
        // Arrange
        var sut = CreateSut();
        sut.PhotoPath = "/storage/photos/photo.jpg";
        sut.HasPhoto = true;

        // Act
        sut.ClearPhotoCommand.Execute(null);

        // Assert
        Assert.False(sut.HasPhoto);
    }

    // ── Service interaction ──────────────────────────────────────────────────

    [Fact]
    public async Task TakePhotoAsync_AlwaysCallsCameraService()
    {
        // Arrange
        _cameraServiceMock
            .Setup(s => s.TakePhotoAsync())
            .ReturnsAsync((string?)null);
        var sut = CreateSut();

        // Act
        await sut.TakePhotoCommand.ExecuteAsync(null);

        // Assert
        _cameraServiceMock.Verify(s => s.TakePhotoAsync(), Times.Once);
    }

    [Fact]
    public async Task PickPhotoAsync_AlwaysCallsCameraService()
    {
        // Arrange
        _cameraServiceMock
            .Setup(s => s.PickPhotoAsync())
            .ReturnsAsync((string?)null);
        var sut = CreateSut();

        // Act
        await sut.PickPhotoCommand.ExecuteAsync(null);

        // Assert
        _cameraServiceMock.Verify(s => s.PickPhotoAsync(), Times.Once);
    }
}
