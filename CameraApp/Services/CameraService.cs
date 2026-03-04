using Microsoft.Maui.Media;
using Microsoft.Maui.Storage;

namespace CameraApp.Services;

/// <summary>
/// Abstraction for copying a <see cref="FileResult" /> to a local path.
/// Exists solely to make <see cref="CameraService" /> unit-testable without real platform I/O.
/// </summary>
public interface IPhotoCopier
{
    /// <summary>Reads the source photo and writes it to <paramref name="destPath" />.</summary>
    Task CopyAsync(FileResult photo, string destPath);
}

/// <summary>Production implementation that uses the MAUI <see cref="FileResult" /> stream API.</summary>
public sealed class PhotoCopier : IPhotoCopier
{
    public async Task CopyAsync(FileResult photo, string destPath)
    {
        using var sourceStream = await photo.OpenReadAsync();
        using var localFileStream = File.OpenWrite(destPath);
        await sourceStream.CopyToAsync(localFileStream);
    }
}

/// <summary>
/// Provides camera and photo picker operations backed by the MAUI <see cref="IMediaPicker" />.
/// </summary>
public class CameraService : ICameraService
{
    private readonly IMediaPicker _mediaPicker;
    private readonly IFileSystem _fileSystem;
    private readonly IPhotoCopier _photoCopier;

    /// <summary>
    /// Initializes a new instance of <see cref="CameraService" />.
    /// </summary>
    /// <param name="mediaPicker">The media picker implementation (use <see cref="MediaPicker.Default" /> in production).</param>
    /// <param name="fileSystem">The file-system abstraction (use <see cref="FileSystem.Current" /> in production).</param>
    /// <param name="photoCopier">Strategy for copying a <see cref="FileResult" /> to a local path.</param>
    public CameraService(IMediaPicker mediaPicker, IFileSystem fileSystem, IPhotoCopier photoCopier)
    {
        _mediaPicker = mediaPicker;
        _fileSystem = fileSystem;
        _photoCopier = photoCopier;
    }

    /// <summary>
    /// Launches the device camera to capture a new photo and saves it to the local cache directory.
    /// </summary>
    /// <returns>The local file path of the captured photo; <see langword="null" /> if the capture was cancelled, the device does not support capture, or an error occurred.</returns>
    public async Task<string?> TakePhotoAsync()
    {
        try
        {
            if (_mediaPicker.IsCaptureSupported)
            {
                var photo = await _mediaPicker.CapturePhotoAsync();

                if (photo != null)
                {
                    // Salvar em local storage
                    var localFilePath = Path.Combine(_fileSystem.CacheDirectory, photo.FileName);

                    await _photoCopier.CopyAsync(photo, localFilePath);

                    return localFilePath;
                }
            }
        }
        catch (Exception ex)
        {
            var mainPage = Application.Current?.Windows?.FirstOrDefault()?.Page;
            if (mainPage != null)
                await mainPage.DisplayAlertAsync("Erro", $"Erro ao tirar foto: {ex.Message}", "OK");
        }

        return null;
    }

    /// <summary>
    /// Opens the device photo picker and saves the selected photo to the local cache directory.
    /// </summary>
    /// <returns>The local file path of the selected photo; <see langword="null" /> if the selection was cancelled or an error occurred.</returns>
    public async Task<string?> PickPhotoAsync()
    {
        try
        {
            var photos = await _mediaPicker.PickPhotosAsync();
            var photo = photos?.FirstOrDefault();

            if (photo != null)
            {
                var localFilePath = Path.Combine(_fileSystem.CacheDirectory, photo.FileName);

                await _photoCopier.CopyAsync(photo, localFilePath);

                return localFilePath;
            }
        }
        catch (Exception ex)
        {
            var mainPage = Application.Current?.Windows?.FirstOrDefault()?.Page;
            if (mainPage != null)
                await mainPage.DisplayAlertAsync("Erro", $"Erro ao selecionar foto: {ex.Message}", "OK");
        }

        return null;
    }
}