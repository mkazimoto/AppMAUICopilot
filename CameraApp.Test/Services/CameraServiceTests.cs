using CameraApp.Services;
using Microsoft.Maui.Media;
using Microsoft.Maui.Storage;
using Moq;

namespace CameraApp.Test.Services;

/// <summary>
/// Unit tests for <see cref="CameraService" />.
/// </summary>
public class CameraServiceTests
{
    private readonly Mock<IMediaPicker> _mediaPickerMock = new();
    private readonly Mock<IFileSystem> _fileSystemMock = new();
    private readonly Mock<IPhotoCopier> _photoCopierMock = new();
    private readonly string _cacheDir = Path.GetTempPath();
    private readonly CameraService _sut;

    public CameraServiceTests()
    {
        _fileSystemMock.Setup(fs => fs.CacheDirectory).Returns(_cacheDir);
        _photoCopierMock.Setup(c => c.CopyAsync(It.IsAny<FileResult>(), It.IsAny<string>()))
                        .Returns(Task.CompletedTask);
        _sut = new CameraService(_mediaPickerMock.Object, _fileSystemMock.Object, _photoCopierMock.Object);
    }

    // ── TakePhotoAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task TakePhotoAsync_WhenCaptureSupported_AndPhotoTaken_ReturnsFilePath()
    {
        // Arrange
        var fileName = "take_photo.jpg";
        var fileResult = new FileResult(fileName);

        _mediaPickerMock.Setup(mp => mp.IsCaptureSupported).Returns(true);
        _mediaPickerMock.Setup(mp => mp.CapturePhotoAsync(It.IsAny<MediaPickerOptions?>()))
                        .ReturnsAsync(fileResult);

        var expectedPath = Path.Combine(_cacheDir, fileName);

        // Act
        var result = await _sut.TakePhotoAsync();

        // Assert
        Assert.Equal(expectedPath, result);
        _photoCopierMock.Verify(c => c.CopyAsync(fileResult, expectedPath), Times.Once);
    }

    [Fact]
    public async Task TakePhotoAsync_WhenCaptureNotSupported_ReturnsNull()
    {
        // Arrange
        _mediaPickerMock.Setup(mp => mp.IsCaptureSupported).Returns(false);

        // Act
        var result = await _sut.TakePhotoAsync();

        // Assert
        Assert.Null(result);
        _mediaPickerMock.Verify(mp => mp.CapturePhotoAsync(It.IsAny<MediaPickerOptions?>()), Times.Never);
    }

    [Fact]
    public async Task TakePhotoAsync_WhenUserCancels_ReturnsNull()
    {
        // Arrange
        _mediaPickerMock.Setup(mp => mp.IsCaptureSupported).Returns(true);
        _mediaPickerMock.Setup(mp => mp.CapturePhotoAsync(It.IsAny<MediaPickerOptions?>()))
                        .ReturnsAsync((FileResult?)null);

        // Act
        var result = await _sut.TakePhotoAsync();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task TakePhotoAsync_WhenExceptionThrown_ReturnsNull()
    {
        // Arrange
        _mediaPickerMock.Setup(mp => mp.IsCaptureSupported).Returns(true);
        _mediaPickerMock.Setup(mp => mp.CapturePhotoAsync(It.IsAny<MediaPickerOptions?>()))
                        .ThrowsAsync(new Exception("Camera hardware error"));

        // Act
        var result = await _sut.TakePhotoAsync();

        // Assert
        Assert.Null(result);
    }

    // ── PickPhotoAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task PickPhotoAsync_WhenPhotoSelected_ReturnsFilePath()
    {
        // Arrange
        var fileName = "picked_photo.jpg";
        var fileResult = new FileResult(fileName);

        _mediaPickerMock.Setup(mp => mp.PickPhotosAsync(It.IsAny<MediaPickerOptions?>()))
                        .Returns(Task.FromResult(new List<FileResult> { fileResult }));

        var expectedPath = Path.Combine(_cacheDir, fileName);

        // Act
        var result = await _sut.PickPhotoAsync();

        // Assert
        Assert.Equal(expectedPath, result);
        _photoCopierMock.Verify(c => c.CopyAsync(fileResult, expectedPath), Times.Once);
    }

    [Fact]
    public async Task PickPhotoAsync_WhenUserCancels_ReturnsNull()
    {
        // Arrange
        _mediaPickerMock.Setup(mp => mp.PickPhotosAsync(It.IsAny<MediaPickerOptions?>()))
                        .Returns(Task.FromResult<List<FileResult>>(null!));

        // Act
        var result = await _sut.PickPhotoAsync();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task PickPhotoAsync_WhenEmptyListReturned_ReturnsNull()
    {
        // Arrange
        _mediaPickerMock.Setup(mp => mp.PickPhotosAsync(It.IsAny<MediaPickerOptions?>()))
                        .Returns(Task.FromResult(new List<FileResult>()));

        // Act
        var result = await _sut.PickPhotoAsync();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task PickPhotoAsync_WhenExceptionThrown_ReturnsNull()
    {
        // Arrange
        _mediaPickerMock.Setup(mp => mp.PickPhotosAsync(It.IsAny<MediaPickerOptions?>()))
                        .ThrowsAsync(new Exception("Picker error"));

        // Act
        var result = await _sut.PickPhotoAsync();

        // Assert
        Assert.Null(result);
    }
}
