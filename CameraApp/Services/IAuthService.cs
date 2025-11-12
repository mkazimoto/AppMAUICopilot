using CameraApp.Models;

namespace CameraApp.Services
{
    public interface IAuthService
    {
        Task<AuthToken?> LoginAsync(string username, string password, string? serviceAlias = null);
        Task<AuthToken?> RefreshTokenAsync(string refreshToken);
        Task LogoutAsync();
        Task<bool> TryRestoreTokenAsync();

        bool IsAuthenticated { get; }
        string? CurrentToken { get; }
        event EventHandler<bool> AuthenticationChanged;
    }
}