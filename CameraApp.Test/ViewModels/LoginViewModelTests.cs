using CameraApp.Models;
using CameraApp.Services;
using CameraApp.ViewModels;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace CameraApp.Test.ViewModels;

[TestClass]
public class LoginViewModelTests
{
    private IAuthService _authService = null!;
    private ILogger<LoginViewModel> _logger = null!;
    private LoginViewModel _viewModel = null!;

    [TestInitialize]
    public void Setup()
    {
        _authService = Substitute.For<IAuthService>();
        _logger = Substitute.For<ILogger<LoginViewModel>>();
        _authService.IsAuthenticated.Returns(false);

        _viewModel = new LoginViewModel(_authService, _logger);
    }

    #region Constructor Tests

    [TestMethod]
    public void Constructor_WhenNotAuthenticated_InitializesDefaultState()
    {
        Assert.IsFalse(_viewModel.IsAuthenticated);
        Assert.AreEqual("Entrar", _viewModel.LoginButtonText);
        Assert.IsTrue(_viewModel.ShowLoginForm);
        Assert.IsFalse(_viewModel.ShowLogoutSection);
        Assert.IsFalse(_viewModel.IsLoading);
    }

    [TestMethod]
    public void Constructor_WhenAuthenticated_InitializesAuthenticatedState()
    {
        _authService.IsAuthenticated.Returns(true);

        var viewModel = new LoginViewModel(_authService, _logger);

        Assert.IsTrue(viewModel.IsAuthenticated);
        Assert.AreEqual("Sair", viewModel.LoginButtonText);
        Assert.IsFalse(viewModel.ShowLoginForm);
        Assert.IsTrue(viewModel.ShowLogoutSection);
    }

    #endregion

    #region LoginCommand Tests

    [TestMethod]
    public async Task LoginCommand_WhenIsLoading_DoesNotCallService()
    {
        _viewModel.IsLoading = true;
        _viewModel.Username = "user";
        _viewModel.Password = "password";

        await _viewModel.LoginCommand.ExecuteAsync(null);

        await _authService.DidNotReceive().LoginAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string?>());
    }

    [TestMethod]
    public async Task LoginCommand_WhenUsernameIsEmpty_ShowsValidationError()
    {
        _viewModel.Username = string.Empty;
        _viewModel.Password = "password";

        await _viewModel.LoginCommand.ExecuteAsync(null);

        Assert.AreEqual("Por favor, informe o nome de usuário.", _viewModel.ErrorMessage);
        Assert.IsFalse(_viewModel.IsLoading);
        await _authService.DidNotReceive().LoginAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string?>());
    }

    [TestMethod]
    public async Task LoginCommand_WhenPasswordIsEmpty_ShowsValidationError()
    {
        _viewModel.Username = "user";
        _viewModel.Password = string.Empty;

        await _viewModel.LoginCommand.ExecuteAsync(null);

        Assert.AreEqual("Por favor, informe a senha.", _viewModel.ErrorMessage);
        Assert.IsFalse(_viewModel.IsLoading);
        await _authService.DidNotReceive().LoginAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string?>());
    }

    [TestMethod]
    public async Task LoginCommand_WhenAliasIsWhitespace_PassesNullAlias()
    {
        _viewModel.Username = "user";
        _viewModel.Password = "password";
        _viewModel.ServiceAlias = "   ";

        _authService.LoginAsync("user", "password", null)
            .Returns(Task.FromResult<AuthToken?>(null));

        await _viewModel.LoginCommand.ExecuteAsync(null);

        await _authService.Received(1).LoginAsync("user", "password", null);
        Assert.AreEqual("Credenciais inválidas. Verifique o usuário e senha.", _viewModel.ErrorMessage);
        Assert.IsFalse(_viewModel.IsLoading);
    }

    [TestMethod]
    public async Task LoginCommand_WhenAliasIsProvided_PassesAliasToService()
    {
        _viewModel.Username = "user";
        _viewModel.Password = "password";
        _viewModel.ServiceAlias = "srv-01";

        _authService.LoginAsync("user", "password", "srv-01")
            .Returns(Task.FromResult<AuthToken?>(null));

        await _viewModel.LoginCommand.ExecuteAsync(null);

        await _authService.Received(1).LoginAsync("user", "password", "srv-01");
        Assert.AreEqual("Credenciais inválidas. Verifique o usuário e senha.", _viewModel.ErrorMessage);
    }

    [TestMethod]
    public async Task LoginCommand_WhenServiceThrows_ShowsGenericError()
    {
        _viewModel.Username = "user";
        _viewModel.Password = "password";

        _authService.LoginAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string?>())
            .Returns(_ => Task.FromException<AuthToken?>(new InvalidOperationException("Service unavailable")));

        await _viewModel.LoginCommand.ExecuteAsync(null);

        Assert.AreEqual("Erro ao fazer login. Tente novamente.", _viewModel.ErrorMessage);
        Assert.IsFalse(_viewModel.IsLoading);
    }

    #endregion

    #region LogoutCommand Tests

    [TestMethod]
    public async Task LogoutCommand_WhenIsLoading_DoesNotCallService()
    {
        _viewModel.IsLoading = true;

        await _viewModel.LogoutCommand.ExecuteAsync(null);

        await _authService.DidNotReceive().LogoutAsync();
        Assert.IsTrue(_viewModel.IsLoading);
    }

    #endregion

    #region ClearCredentialsCommand Tests

    [TestMethod]
    public void ClearCredentialsCommand_ClearsCredentialFieldsAndError()
    {
        _viewModel.Username = "user";
        _viewModel.Password = "password";
        _viewModel.ServiceAlias = "alias";
        _viewModel.ErrorMessage = "Erro";

        _viewModel.ClearCredentialsCommand.Execute(null);

        Assert.AreEqual(string.Empty, _viewModel.Username);
        Assert.AreEqual(string.Empty, _viewModel.Password);
        Assert.AreEqual(string.Empty, _viewModel.ServiceAlias);
        Assert.AreEqual(string.Empty, _viewModel.ErrorMessage);
    }

    #endregion

    #region Property and Event Tests

    [TestMethod]
    public void CanLogin_ChangesWhenCredentialsAndLoadingChange()
    {
        Assert.IsFalse(_viewModel.CanLogin);

        _viewModel.Username = "user";
        Assert.IsFalse(_viewModel.CanLogin);

        _viewModel.Password = "password";
        Assert.IsTrue(_viewModel.CanLogin);

        _viewModel.IsLoading = true;
        Assert.IsFalse(_viewModel.CanLogin);

        _viewModel.IsLoading = false;
        Assert.IsTrue(_viewModel.CanLogin);
    }

    [TestMethod]
    public void AuthenticationChanged_WhenTrue_UpdatesAuthenticationStateAndMessage()
    {
        _authService.AuthenticationChanged += Raise.Event<EventHandler<bool>>(this, true);

        Assert.IsTrue(_viewModel.IsAuthenticated);
        Assert.AreEqual("Sair", _viewModel.LoginButtonText);
        Assert.IsFalse(_viewModel.ShowLoginForm);
        Assert.IsTrue(_viewModel.ShowLogoutSection);
        Assert.AreEqual("Login realizado com sucesso!", _viewModel.ErrorMessage);
        Assert.IsTrue(_viewModel.CanLogout);
    }

    [TestMethod]
    public void AuthenticationChanged_WhenFalse_UpdatesAuthenticationState()
    {
        _authService.IsAuthenticated.Returns(true);
        var viewModel = new LoginViewModel(_authService, _logger);

        _authService.AuthenticationChanged += Raise.Event<EventHandler<bool>>(this, false);

        Assert.IsFalse(viewModel.IsAuthenticated);
        Assert.AreEqual("Entrar", viewModel.LoginButtonText);
        Assert.IsTrue(viewModel.ShowLoginForm);
        Assert.IsFalse(viewModel.ShowLogoutSection);
        Assert.IsFalse(viewModel.CanLogout);
    }

    #endregion
}