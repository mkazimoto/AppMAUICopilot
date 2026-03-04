using CameraApp.Models;
using CameraApp.Services;
using CameraApp.ViewModels;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace CameraApp.Test.ViewModels;

public class LoginViewModelTests
{
    private readonly Mock<IAuthService> _authServiceMock;

    public LoginViewModelTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _authServiceMock.SetupAdd(s => s.AuthenticationChanged += It.IsAny<EventHandler<bool>>());
        _authServiceMock.Setup(s => s.IsAuthenticated).Returns(false);
        // TryRestoreTokenAsync is only called when the service is the concrete AuthService,
        // so the mock instance safely bypasses that code path.
    }

    private LoginViewModel CreateSut() =>
        new(_authServiceMock.Object, NullLogger<LoginViewModel>.Instance);

    // ── Initial state ────────────────────────────────────────────────────────

    [Fact]
    public void InitialState_IsLoading_IsFalse()
    {
        var sut = CreateSut();
        Assert.False(sut.IsLoading);
    }

    [Fact]
    public void InitialState_IsAuthenticated_MatchesService()
    {
        _authServiceMock.Setup(s => s.IsAuthenticated).Returns(false);
        var sut = CreateSut();
        Assert.False(sut.IsAuthenticated);
    }

    [Fact]
    public void InitialState_LoginButtonText_IsEntrar()
    {
        var sut = CreateSut();
        Assert.Equal("Entrar", sut.LoginButtonText);
    }

    // ── CanLogin ─────────────────────────────────────────────────────────────

    [Fact]
    public void CanLogin_WhenUsernameAndPasswordSet_IsTrue()
    {
        var sut = CreateSut();
        sut.Username = "user";
        sut.Password = "pass";
        Assert.True(sut.CanLogin);
    }

    [Fact]
    public void CanLogin_WhenUsernameEmpty_IsFalse()
    {
        var sut = CreateSut();
        sut.Username = string.Empty;
        sut.Password = "pass";
        Assert.False(sut.CanLogin);
    }

    [Fact]
    public void CanLogin_WhenPasswordEmpty_IsFalse()
    {
        var sut = CreateSut();
        sut.Username = "user";
        sut.Password = string.Empty;
        Assert.False(sut.CanLogin);
    }

    [Fact]
    public void CanLogin_WhenIsLoading_IsFalse()
    {
        var sut = CreateSut();
        sut.Username = "user";
        sut.Password = "pass";
        sut.IsLoading = true;
        Assert.False(sut.CanLogin);
    }

    // ── ClearCredentials ─────────────────────────────────────────────────────

    [Fact]
    public void ClearCredentialsCommand_ClearsAllFields()
    {
        var sut = CreateSut();
        sut.Username = "user";
        sut.Password = "pass";
        sut.ServiceAlias = "alias";
        sut.ErrorMessage = "some error";

        sut.ClearCredentialsCommand.Execute(null);

        Assert.Equal(string.Empty, sut.Username);
        Assert.Equal(string.Empty, sut.Password);
        Assert.Equal(string.Empty, sut.ServiceAlias);
        Assert.Equal(string.Empty, sut.ErrorMessage);
    }

    // ── LoginAsync validation ─────────────────────────────────────────────────

    [Fact]
    public async Task LoginAsync_WhenUsernameEmpty_SetsErrorMessage()
    {
        var sut = CreateSut();
        sut.Username = string.Empty;
        sut.Password = "pass";

        await sut.LoginCommand.ExecuteAsync(null);

        Assert.Equal("Por favor, informe o nome de usuário.", sut.ErrorMessage);
        Assert.False(sut.IsLoading);
    }

    [Fact]
    public async Task LoginAsync_WhenPasswordEmpty_SetsErrorMessage()
    {
        var sut = CreateSut();
        sut.Username = "user";
        sut.Password = string.Empty;

        await sut.LoginCommand.ExecuteAsync(null);

        Assert.Equal("Por favor, informe a senha.", sut.ErrorMessage);
        Assert.False(sut.IsLoading);
    }

    [Fact]
    public async Task LoginAsync_WhenServiceReturnsNull_SetsInvalidCredentialsMessage()
    {
        _authServiceMock
            .Setup(s => s.LoginAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string?>()))
            .ReturnsAsync((AuthToken?)null);

        var sut = CreateSut();
        sut.Username = "user";
        sut.Password = "wrongpass";

        await sut.LoginCommand.ExecuteAsync(null);

        Assert.Equal("Credenciais inválidas. Verifique o usuário e senha.", sut.ErrorMessage);
        Assert.False(sut.IsLoading);
    }

    [Fact]
    public async Task LoginAsync_WhenServiceThrows_SetsGenericErrorMessage()
    {
        _authServiceMock
            .Setup(s => s.LoginAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string?>()))
            .ThrowsAsync(new HttpRequestException("Network error"));

        var sut = CreateSut();
        sut.Username = "user";
        sut.Password = "pass";

        await sut.LoginCommand.ExecuteAsync(null);

        Assert.Equal("Erro ao fazer login. Tente novamente.", sut.ErrorMessage);
        Assert.False(sut.IsLoading);
    }

    [Fact]
    public async Task LoginAsync_WhenAlreadyLoading_DoesNotCallService()
    {
        var sut = CreateSut();
        sut.Username = "user";
        sut.Password = "pass";
        sut.IsLoading = true;

        await sut.LoginCommand.ExecuteAsync(null);

        _authServiceMock.Verify(
            s => s.LoginAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string?>()),
            Times.Never);
    }

    // ── Computed UI properties ────────────────────────────────────────────────

    [Fact]
    public void ShowLoginForm_WhenNotAuthenticated_IsTrue()
    {
        _authServiceMock.Setup(s => s.IsAuthenticated).Returns(false);
        var sut = CreateSut();
        Assert.True(sut.ShowLoginForm);
    }

    [Fact]
    public void ShowLogoutSection_WhenNotAuthenticated_IsFalse()
    {
        _authServiceMock.Setup(s => s.IsAuthenticated).Returns(false);
        var sut = CreateSut();
        Assert.False(sut.ShowLogoutSection);
    }
}
