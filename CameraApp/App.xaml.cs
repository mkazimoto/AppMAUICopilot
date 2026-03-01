using CameraApp.Services;

namespace CameraApp;

public partial class App : Application
{
	private readonly IAuthService _authService;
	private readonly IThemeService _themeService;

	public App(IAuthService authService, IThemeService themeService)
	{
		InitializeComponent();
		_authService = authService;
		_themeService = themeService;
		_themeService.Initialize();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new AppShell());
	}

	protected override async void OnStart()
	{
		base.OnStart();
		
		// Tentar restaurar token do SecureStorage
		var tokenRestored = await _authService.TryRestoreTokenAsync();
		
		if (!tokenRestored)
		{
			// Se não conseguiu restaurar o token, ir para a tela de login
			await Shell.Current.GoToAsync("//LoginPage");
		}
	}
}