using System.Net;
using System.Net.Http.Json;
using CameraApp.Models;
using CameraApp.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Maui.Storage;

namespace CameraApp.Test.Services;

/// <summary>
/// Unit tests for <see cref="AuthService" />.
/// </summary>
public class AuthServiceTests
{
    private readonly Mock<ISecureStorage> _secureStorageMock = new();

    private AuthService CreateSut(HttpMessageHandler handler) =>
        new(new HttpClient(handler),
            NullLogger<AuthService>.Instance,
            _secureStorageMock.Object);

    private static HttpResponseMessage OkJson<T>(T payload) =>
        new(HttpStatusCode.OK) { Content = JsonContent.Create(payload) };

    private static AuthToken MakeToken(bool expired = false) => new()
    {
        AccessToken = "access-123",
        RefreshToken = "refresh-456",
        TokenType = "Bearer",
        ExpiresIn = 3600,
        ExpiresAt = expired ? DateTime.Now.AddSeconds(-1) : DateTime.Now.AddHours(1)
    };

    // ── LoginAsync ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task LoginAsync_WhenCredentialsValid_ReturnsToken()
    {
        // Arrange
        _secureStorageMock.Setup(s => s.SetAsync(It.IsAny<string>(), It.IsAny<string>()))
                          .Returns(Task.CompletedTask);
        var sut = CreateSut(new FakeHttpMessageHandler(OkJson(MakeToken())));

        // Act
        var result = await sut.LoginAsync("user", "pass");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("access-123", result.AccessToken);
        Assert.Equal("refresh-456", result.RefreshToken);
    }

    [Fact]
    public async Task LoginAsync_WhenCredentialsValid_SetsIsAuthenticated()
    {
        // Arrange
        _secureStorageMock.Setup(s => s.SetAsync(It.IsAny<string>(), It.IsAny<string>()))
                          .Returns(Task.CompletedTask);
        var sut = CreateSut(new FakeHttpMessageHandler(OkJson(MakeToken())));

        // Act
        await sut.LoginAsync("user", "pass");

        // Assert
        Assert.True(sut.IsAuthenticated);
        Assert.Equal("access-123", sut.CurrentToken);
    }

    [Fact]
    public async Task LoginAsync_WhenCredentialsValid_SavesThreeKeysToSecureStorage()
    {
        // Arrange
        _secureStorageMock.Setup(s => s.SetAsync(It.IsAny<string>(), It.IsAny<string>()))
                          .Returns(Task.CompletedTask);
        var sut = CreateSut(new FakeHttpMessageHandler(OkJson(MakeToken())));

        // Act
        await sut.LoginAsync("user", "pass");

        // Assert
        _secureStorageMock.Verify(s => s.SetAsync("access_token", "access-123"), Times.Once);
        _secureStorageMock.Verify(s => s.SetAsync("refresh_token", "refresh-456"), Times.Once);
        _secureStorageMock.Verify(s => s.SetAsync("token_expires_at", It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WhenCredentialsValid_FiresAuthenticationChangedWithTrue()
    {
        // Arrange
        _secureStorageMock.Setup(s => s.SetAsync(It.IsAny<string>(), It.IsAny<string>()))
                          .Returns(Task.CompletedTask);
        var sut = CreateSut(new FakeHttpMessageHandler(OkJson(MakeToken())));
        bool? eventArg = null;
        sut.AuthenticationChanged += (_, e) => eventArg = e;

        // Act
        await sut.LoginAsync("user", "pass");

        // Assert
        Assert.True(eventArg);
    }

    [Fact]
    public async Task LoginAsync_WithServiceAlias_SendsRequestAndReturnsToken()
    {
        // Arrange
        _secureStorageMock.Setup(s => s.SetAsync(It.IsAny<string>(), It.IsAny<string>()))
                          .Returns(Task.CompletedTask);
        var sut = CreateSut(new FakeHttpMessageHandler(OkJson(MakeToken())));

        // Act
        var result = await sut.LoginAsync("user", "pass", serviceAlias: "svc-alias");

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task LoginAsync_WhenHttpReturnsUnauthorized_ReturnsNull()
    {
        // Arrange
        var sut = CreateSut(new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.Unauthorized)));

        // Act
        var result = await sut.LoginAsync("user", "wrong");

        // Assert
        Assert.Null(result);
        Assert.False(sut.IsAuthenticated);
    }

    [Fact]
    public async Task LoginAsync_WhenHttpThrows_ReturnsNull()
    {
        // Arrange
        var sut = CreateSut(new ThrowingHttpMessageHandler());

        // Act
        var result = await sut.LoginAsync("user", "pass");

        // Assert
        Assert.Null(result);
        Assert.False(sut.IsAuthenticated);
    }

    [Fact]
    public async Task LoginAsync_WhenHttpReturnsNullBody_ReturnsNull()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create<AuthToken?>(null)
        };
        var sut = CreateSut(new FakeHttpMessageHandler(response));

        // Act
        var result = await sut.LoginAsync("user", "pass");

        // Assert
        Assert.Null(result);
    }

    // ── RefreshTokenAsync ──────────────────────────────────────────────────────

    [Fact]
    public async Task RefreshTokenAsync_WhenRefreshValid_ReturnsNewToken()
    {
        // Arrange
        _secureStorageMock.Setup(s => s.SetAsync(It.IsAny<string>(), It.IsAny<string>()))
                          .Returns(Task.CompletedTask);
        var sut = CreateSut(new FakeHttpMessageHandler(OkJson(MakeToken())));

        // Act
        var result = await sut.RefreshTokenAsync("refresh-456");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("access-123", result.AccessToken);
    }

    [Fact]
    public async Task RefreshTokenAsync_WhenRefreshValid_UpdatesSecureStorage()
    {
        // Arrange
        _secureStorageMock.Setup(s => s.SetAsync(It.IsAny<string>(), It.IsAny<string>()))
                          .Returns(Task.CompletedTask);
        var sut = CreateSut(new FakeHttpMessageHandler(OkJson(MakeToken())));

        // Act
        await sut.RefreshTokenAsync("refresh-456");

        // Assert
        _secureStorageMock.Verify(s => s.SetAsync("access_token", "access-123"), Times.Once);
        _secureStorageMock.Verify(s => s.SetAsync("refresh_token", "refresh-456"), Times.Once);
        _secureStorageMock.Verify(s => s.SetAsync("token_expires_at", It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task RefreshTokenAsync_WhenRefreshValid_FiresAuthenticationChangedWithTrue()
    {
        // Arrange
        _secureStorageMock.Setup(s => s.SetAsync(It.IsAny<string>(), It.IsAny<string>()))
                          .Returns(Task.CompletedTask);
        var sut = CreateSut(new FakeHttpMessageHandler(OkJson(MakeToken())));
        bool? eventArg = null;
        sut.AuthenticationChanged += (_, e) => eventArg = e;

        // Act
        await sut.RefreshTokenAsync("refresh-456");

        // Assert
        Assert.True(eventArg);
    }

    [Fact]
    public async Task RefreshTokenAsync_WhenHttpFails_ReturnsNull()
    {
        // Arrange
        var sut = CreateSut(new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.BadRequest)));

        // Act
        var result = await sut.RefreshTokenAsync("bad-refresh");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task RefreshTokenAsync_WhenHttpThrows_ReturnsNull()
    {
        // Arrange
        var sut = CreateSut(new ThrowingHttpMessageHandler());

        // Act
        var result = await sut.RefreshTokenAsync("refresh-456");

        // Assert
        Assert.Null(result);
    }

    // ── LogoutAsync ────────────────────────────────────────────────────────────

    [Fact]
    public async Task LogoutAsync_AfterLogin_ClearsIsAuthenticated()
    {
        // Arrange — log in first
        _secureStorageMock.Setup(s => s.SetAsync(It.IsAny<string>(), It.IsAny<string>()))
                          .Returns(Task.CompletedTask);
        var sut = CreateSut(new FakeHttpMessageHandler(OkJson(MakeToken())));
        await sut.LoginAsync("user", "pass");
        Assert.True(sut.IsAuthenticated);

        // Act
        await sut.LogoutAsync();

        // Assert
        Assert.False(sut.IsAuthenticated);
        Assert.Null(sut.CurrentToken);
    }

    [Fact]
    public async Task LogoutAsync_CallsRemoveAllOnSecureStorage()
    {
        // Arrange
        var sut = CreateSut(new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)));

        // Act
        await sut.LogoutAsync();

        // Assert
        _secureStorageMock.Verify(s => s.RemoveAll(), Times.Once);
    }

    [Fact]
    public async Task LogoutAsync_FiresAuthenticationChangedWithFalse()
    {
        // Arrange
        var sut = CreateSut(new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)));
        bool? eventArg = null;
        sut.AuthenticationChanged += (_, e) => eventArg = e;

        // Act
        await sut.LogoutAsync();

        // Assert
        Assert.False(eventArg);
    }

    // ── TryRestoreTokenAsync ───────────────────────────────────────────────────

    [Fact]
    public async Task TryRestoreTokenAsync_WhenValidNonExpiredTokenStored_ReturnsTrue()
    {
        // Arrange
        var expiresAt = DateTime.Now.AddHours(1);
        _secureStorageMock.Setup(s => s.GetAsync("access_token")).ReturnsAsync("access-123");
        _secureStorageMock.Setup(s => s.GetAsync("refresh_token")).ReturnsAsync("refresh-456");
        _secureStorageMock.Setup(s => s.GetAsync("token_expires_at")).ReturnsAsync(expiresAt.ToString());
        var sut = CreateSut(new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)));

        // Act
        var result = await sut.TryRestoreTokenAsync();

        // Assert
        Assert.True(result);
        Assert.True(sut.IsAuthenticated);
        Assert.Equal("access-123", sut.CurrentToken);
    }

    [Fact]
    public async Task TryRestoreTokenAsync_WhenValidNonExpiredTokenStored_FiresAuthenticationChangedWithTrue()
    {
        // Arrange
        var expiresAt = DateTime.Now.AddHours(1);
        _secureStorageMock.Setup(s => s.GetAsync("access_token")).ReturnsAsync("access-123");
        _secureStorageMock.Setup(s => s.GetAsync("refresh_token")).ReturnsAsync("refresh-456");
        _secureStorageMock.Setup(s => s.GetAsync("token_expires_at")).ReturnsAsync(expiresAt.ToString());
        var sut = CreateSut(new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)));
        bool? eventArg = null;
        sut.AuthenticationChanged += (_, e) => eventArg = e;

        // Act
        await sut.TryRestoreTokenAsync();

        // Assert
        Assert.True(eventArg);
    }

    [Fact]
    public async Task TryRestoreTokenAsync_WhenExpiredTokenStored_AttemptsRefreshAndReturnsTrue()
    {
        // Arrange
        var expiredAt = DateTime.Now.AddSeconds(-1);
        _secureStorageMock.Setup(s => s.GetAsync("access_token")).ReturnsAsync("old-access");
        _secureStorageMock.Setup(s => s.GetAsync("refresh_token")).ReturnsAsync("refresh-456");
        _secureStorageMock.Setup(s => s.GetAsync("token_expires_at")).ReturnsAsync(expiredAt.ToString());
        _secureStorageMock.Setup(s => s.SetAsync(It.IsAny<string>(), It.IsAny<string>()))
                          .Returns(Task.CompletedTask);
        var sut = CreateSut(new FakeHttpMessageHandler(OkJson(MakeToken())));

        // Act
        var result = await sut.TryRestoreTokenAsync();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task TryRestoreTokenAsync_WhenExpiredTokenAndRefreshFails_ReturnsFalse()
    {
        // Arrange
        var expiredAt = DateTime.Now.AddSeconds(-1);
        _secureStorageMock.Setup(s => s.GetAsync("access_token")).ReturnsAsync("old-access");
        _secureStorageMock.Setup(s => s.GetAsync("refresh_token")).ReturnsAsync("refresh-456");
        _secureStorageMock.Setup(s => s.GetAsync("token_expires_at")).ReturnsAsync(expiredAt.ToString());
        var sut = CreateSut(new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.Unauthorized)));

        // Act
        var result = await sut.TryRestoreTokenAsync();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task TryRestoreTokenAsync_WhenNoTokenStored_ReturnsFalse()
    {
        // Arrange
        _secureStorageMock.Setup(s => s.GetAsync(It.IsAny<string>())).ReturnsAsync((string?)null);
        var sut = CreateSut(new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)));

        // Act
        var result = await sut.TryRestoreTokenAsync();

        // Assert
        Assert.False(result);
        Assert.False(sut.IsAuthenticated);
    }

    [Fact]
    public async Task TryRestoreTokenAsync_WhenSecureStorageThrows_ReturnsFalse()
    {
        // Arrange
        _secureStorageMock.Setup(s => s.GetAsync(It.IsAny<string>()))
                          .ThrowsAsync(new Exception("storage error"));
        var sut = CreateSut(new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)));

        // Act
        var result = await sut.TryRestoreTokenAsync();

        // Assert
        Assert.False(result);
    }

    // ── IsAuthenticated / CurrentToken ─────────────────────────────────────────

    [Fact]
    public void IsAuthenticated_Initially_ReturnsFalse()
    {
        var sut = CreateSut(new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)));
        Assert.False(sut.IsAuthenticated);
    }

    [Fact]
    public void CurrentToken_Initially_ReturnsNull()
    {
        var sut = CreateSut(new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)));
        Assert.Null(sut.CurrentToken);
    }
}

// ── Test helpers ───────────────────────────────────────────────────────────────

internal sealed class FakeHttpMessageHandler(HttpResponseMessage response) : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken) =>
        Task.FromResult(response);
}

internal sealed class ThrowingHttpMessageHandler : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken) =>
        throw new HttpRequestException("Simulated network failure");
}
