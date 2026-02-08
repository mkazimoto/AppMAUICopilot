using CameraApp.Services;
using CameraApp.ViewModels;
using NSubstitute;

namespace CameraApp.Test.ViewModels;

[TestClass]
public class CameraPageViewModelTests
{
    private ICameraService _cameraService = null!;
    private CameraPageViewModel _viewModel = null!;

    [TestInitialize]
    public void Setup()
    {
        _cameraService = Substitute.For<ICameraService>();
        _viewModel = new CameraPageViewModel(_cameraService);
    }

    #region Constructor Tests

    [TestMethod]
    public void Constructor_InitializesWithDefaultValues()
    {
        // Assert
        Assert.IsNull(_viewModel.PhotoPath);
        Assert.IsFalse(_viewModel.HasPhoto);
    }

    [TestMethod]
    public void Constructor_WithValidService_DoesNotThrow()
    {
        // Act & Assert - should not throw
        var viewModel = new CameraPageViewModel(_cameraService);
        Assert.IsNotNull(viewModel);
    }

    #endregion

    #region TakePhotoCommand Tests

    [TestMethod]
    public async Task TakePhotoCommand_WhenPhotoTaken_UpdatesPhotoPathAndHasPhoto()
    {
        // Arrange
        const string expectedPath = "/path/to/photo.jpg";
        _cameraService.TakePhotoAsync().Returns(Task.FromResult<string?>(expectedPath));

        // Act
        await _viewModel.TakePhotoCommand.ExecuteAsync(null);

        // Assert
        Assert.AreEqual(expectedPath, _viewModel.PhotoPath);
        Assert.IsTrue(_viewModel.HasPhoto);
        await _cameraService.Received(1).TakePhotoAsync();
    }

    [TestMethod]
    public async Task TakePhotoCommand_WhenServiceReturnsNull_DoesNotUpdateProperties()
    {
        // Arrange
        _cameraService.TakePhotoAsync().Returns(Task.FromResult<string?>(null));

        // Act
        await _viewModel.TakePhotoCommand.ExecuteAsync(null);

        // Assert
        Assert.IsNull(_viewModel.PhotoPath);
        Assert.IsFalse(_viewModel.HasPhoto);
        await _cameraService.Received(1).TakePhotoAsync();
    }

    [TestMethod]
    public async Task TakePhotoCommand_WhenServiceReturnsEmptyString_DoesNotUpdateProperties()
    {
        // Arrange
        _cameraService.TakePhotoAsync().Returns(Task.FromResult<string?>(string.Empty));

        // Act
        await _viewModel.TakePhotoCommand.ExecuteAsync(null);

        // Assert
        Assert.IsNull(_viewModel.PhotoPath);
        Assert.IsFalse(_viewModel.HasPhoto);
        await _cameraService.Received(1).TakePhotoAsync();
    }

    [TestMethod]
    public async Task TakePhotoCommand_WhenCalledMultipleTimes_UpdatesWithLatestPhoto()
    {
        // Arrange
        const string firstPath = "/path/to/photo1.jpg";
        const string secondPath = "/path/to/photo2.jpg";
        
        _cameraService.TakePhotoAsync().Returns(
            Task.FromResult<string?>(firstPath),
            Task.FromResult<string?>(secondPath));

        // Act
        await _viewModel.TakePhotoCommand.ExecuteAsync(null);
        var firstPhotoPath = _viewModel.PhotoPath;
        
        await _viewModel.TakePhotoCommand.ExecuteAsync(null);
        var secondPhotoPath = _viewModel.PhotoPath;

        // Assert
        Assert.AreEqual(firstPath, firstPhotoPath);
        Assert.AreEqual(secondPath, secondPhotoPath);
        Assert.IsTrue(_viewModel.HasPhoto);
        await _cameraService.Received(2).TakePhotoAsync();
    }

    [TestMethod]
    public async Task TakePhotoCommand_WhenServiceThrowsException_PropagatesException()
    {
        // Arrange
        _cameraService.TakePhotoAsync().Returns<string?>(_ => throw new InvalidOperationException("Camera not available"));

        // Act & Assert
        await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () =>
            await _viewModel.TakePhotoCommand.ExecuteAsync(null));
    }

    [TestMethod]
    public void TakePhotoCommand_IsNotNull()
    {
        // Assert
        Assert.IsNotNull(_viewModel.TakePhotoCommand);
    }

    #endregion

    #region PickPhotoCommand Tests

    [TestMethod]
    public async Task PickPhotoCommand_WhenPhotoSelected_UpdatesPhotoPathAndHasPhoto()
    {
        // Arrange
        const string expectedPath = "/path/to/picked_photo.jpg";
        _cameraService.PickPhotoAsync().Returns(Task.FromResult<string?>(expectedPath));

        // Act
        await _viewModel.PickPhotoCommand.ExecuteAsync(null);

        // Assert
        Assert.AreEqual(expectedPath, _viewModel.PhotoPath);
        Assert.IsTrue(_viewModel.HasPhoto);
        await _cameraService.Received(1).PickPhotoAsync();
    }

    [TestMethod]
    public async Task PickPhotoCommand_WhenServiceReturnsNull_DoesNotUpdateProperties()
    {
        // Arrange
        _cameraService.PickPhotoAsync().Returns(Task.FromResult<string?>(null));

        // Act
        await _viewModel.PickPhotoCommand.ExecuteAsync(null);

        // Assert
        Assert.IsNull(_viewModel.PhotoPath);
        Assert.IsFalse(_viewModel.HasPhoto);
        await _cameraService.Received(1).PickPhotoAsync();
    }

    [TestMethod]
    public async Task PickPhotoCommand_WhenServiceReturnsEmptyString_DoesNotUpdateProperties()
    {
        // Arrange
        _cameraService.PickPhotoAsync().Returns(Task.FromResult<string?>(string.Empty));

        // Act
        await _viewModel.PickPhotoCommand.ExecuteAsync(null);

        // Assert
        Assert.IsNull(_viewModel.PhotoPath);
        Assert.IsFalse(_viewModel.HasPhoto);
        await _cameraService.Received(1).PickPhotoAsync();
    }

    [TestMethod]
    public async Task PickPhotoCommand_WhenCalledMultipleTimes_UpdatesWithLatestPhoto()
    {
        // Arrange
        const string firstPath = "/path/to/picked_photo1.jpg";
        const string secondPath = "/path/to/picked_photo2.jpg";
        
        _cameraService.PickPhotoAsync().Returns(
            Task.FromResult<string?>(firstPath),
            Task.FromResult<string?>(secondPath));

        // Act
        await _viewModel.PickPhotoCommand.ExecuteAsync(null);
        var firstPhotoPath = _viewModel.PhotoPath;
        
        await _viewModel.PickPhotoCommand.ExecuteAsync(null);
        var secondPhotoPath = _viewModel.PhotoPath;

        // Assert
        Assert.AreEqual(firstPath, firstPhotoPath);
        Assert.AreEqual(secondPath, secondPhotoPath);
        Assert.IsTrue(_viewModel.HasPhoto);
        await _cameraService.Received(2).PickPhotoAsync();
    }

    [TestMethod]
    public async Task PickPhotoCommand_WhenServiceThrowsException_PropagatesException()
    {
        // Arrange
        _cameraService.PickPhotoAsync().Returns<string?>(_ => throw new InvalidOperationException("Gallery not available"));

        // Act & Assert
        await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () =>
            await _viewModel.PickPhotoCommand.ExecuteAsync(null));
    }

    [TestMethod]
    public void PickPhotoCommand_IsNotNull()
    {
        // Assert
        Assert.IsNotNull(_viewModel.PickPhotoCommand);
    }

    #endregion

    #region ClearPhotoCommand Tests

    [TestMethod]
    public void ClearPhotoCommand_WhenPhotoExists_ClearsPhotoPathAndHasPhoto()
    {
        // Arrange
        _viewModel.PhotoPath = "/path/to/photo.jpg";
        _viewModel.HasPhoto = true;

        // Act
        _viewModel.ClearPhotoCommand.Execute(null);

        // Assert
        Assert.IsNull(_viewModel.PhotoPath);
        Assert.IsFalse(_viewModel.HasPhoto);
    }

    [TestMethod]
    public void ClearPhotoCommand_WhenNoPhoto_DoesNotThrow()
    {
        // Arrange
        _viewModel.PhotoPath = null;
        _viewModel.HasPhoto = false;

        // Act & Assert - should not throw
        _viewModel.ClearPhotoCommand.Execute(null);
        Assert.IsNull(_viewModel.PhotoPath);
        Assert.IsFalse(_viewModel.HasPhoto);
    }

    [TestMethod]
    public async Task ClearPhotoCommand_AfterTakingPhoto_ResetsState()
    {
        // Arrange
        _cameraService.TakePhotoAsync().Returns(Task.FromResult<string?>("/path/to/photo.jpg"));

        // Act
        await _viewModel.TakePhotoCommand.ExecuteAsync(null);
        Assert.IsTrue(_viewModel.HasPhoto); // Verify photo was set

        _viewModel.ClearPhotoCommand.Execute(null);

        // Assert
        Assert.IsNull(_viewModel.PhotoPath);
        Assert.IsFalse(_viewModel.HasPhoto);
    }

    [TestMethod]
    public async Task ClearPhotoCommand_AfterPickingPhoto_ResetsState()
    {
        // Arrange
        _cameraService.PickPhotoAsync().Returns(Task.FromResult<string?>("/path/to/picked_photo.jpg"));

        // Act
        await _viewModel.PickPhotoCommand.ExecuteAsync(null);
        Assert.IsTrue(_viewModel.HasPhoto); // Verify photo was set

        _viewModel.ClearPhotoCommand.Execute(null);

        // Assert
        Assert.IsNull(_viewModel.PhotoPath);
        Assert.IsFalse(_viewModel.HasPhoto);
    }

    [TestMethod]
    public void ClearPhotoCommand_IsNotNull()
    {
        // Assert
        Assert.IsNotNull(_viewModel.ClearPhotoCommand);
    }

    [TestMethod]
    public void ClearPhotoCommand_CanBeCalledMultipleTimes()
    {
        // Arrange
        _viewModel.PhotoPath = "/path/to/photo.jpg";
        _viewModel.HasPhoto = true;

        // Act
        _viewModel.ClearPhotoCommand.Execute(null);
        _viewModel.ClearPhotoCommand.Execute(null);
        _viewModel.ClearPhotoCommand.Execute(null);

        // Assert
        Assert.IsNull(_viewModel.PhotoPath);
        Assert.IsFalse(_viewModel.HasPhoto);
    }

    #endregion

    #region Property Change Notification Tests

    [TestMethod]
    public void PhotoPath_WhenChanged_RaisesPropertyChanged()
    {
        // Arrange
        var propertyChangedRaised = false;
        _viewModel.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(_viewModel.PhotoPath))
                propertyChangedRaised = true;
        };

        // Act
        _viewModel.PhotoPath = "/path/to/photo.jpg";

        // Assert
        Assert.IsTrue(propertyChangedRaised);
    }

    [TestMethod]
    public void HasPhoto_WhenChanged_RaisesPropertyChanged()
    {
        // Arrange
        var propertyChangedRaised = false;
        _viewModel.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(_viewModel.HasPhoto))
                propertyChangedRaised = true;
        };

        // Act
        _viewModel.HasPhoto = true;

        // Assert
        Assert.IsTrue(propertyChangedRaised);
    }

    #endregion

    #region Integration/Workflow Tests

    [TestMethod]
    public async Task Workflow_TakePhoto_ThenClear_ThenPickPhoto_WorksCorrectly()
    {
        // Arrange
        const string takenPhotoPath = "/path/to/taken_photo.jpg";
        const string pickedPhotoPath = "/path/to/picked_photo.jpg";
        
        _cameraService.TakePhotoAsync().Returns(Task.FromResult<string?>(takenPhotoPath));
        _cameraService.PickPhotoAsync().Returns(Task.FromResult<string?>(pickedPhotoPath));

        // Act - Take photo
        await _viewModel.TakePhotoCommand.ExecuteAsync(null);
        Assert.AreEqual(takenPhotoPath, _viewModel.PhotoPath);
        Assert.IsTrue(_viewModel.HasPhoto);

        // Act - Clear photo
        _viewModel.ClearPhotoCommand.Execute(null);
        Assert.IsNull(_viewModel.PhotoPath);
        Assert.IsFalse(_viewModel.HasPhoto);

        // Act - Pick photo
        await _viewModel.PickPhotoCommand.ExecuteAsync(null);
        Assert.AreEqual(pickedPhotoPath, _viewModel.PhotoPath);
        Assert.IsTrue(_viewModel.HasPhoto);

        // Assert
        await _cameraService.Received(1).TakePhotoAsync();
        await _cameraService.Received(1).PickPhotoAsync();
    }

    [TestMethod]
    public async Task Workflow_PickPhoto_ThenTakePhoto_ReplacesExistingPhoto()
    {
        // Arrange
        const string pickedPhotoPath = "/path/to/picked_photo.jpg";
        const string takenPhotoPath = "/path/to/taken_photo.jpg";
        
        _cameraService.PickPhotoAsync().Returns(Task.FromResult<string?>(pickedPhotoPath));
        _cameraService.TakePhotoAsync().Returns(Task.FromResult<string?>(takenPhotoPath));

        // Act - Pick photo
        await _viewModel.PickPhotoCommand.ExecuteAsync(null);
        Assert.AreEqual(pickedPhotoPath, _viewModel.PhotoPath);

        // Act - Take photo (should replace)
        await _viewModel.TakePhotoCommand.ExecuteAsync(null);

        // Assert
        Assert.AreEqual(takenPhotoPath, _viewModel.PhotoPath);
        Assert.IsTrue(_viewModel.HasPhoto);
    }

    #endregion
}
