using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using CameraApp.ViewModels;
using CameraApp.Views;
using CameraApp.Services;
using CameraApp.Config;

namespace CameraApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		// Registrar serviços
		builder.Services.AddSingleton<HttpClient>(provider =>
		{
			var client = new HttpClient();
			client.Timeout = ApiConfig.RequestTimeout;
			return client;
		});
		
		builder.Services.AddTransient<AuthHttpHandler>();
		builder.Services.AddSingleton<ICameraService, CameraService>();
		builder.Services.AddSingleton<ILocationService, LocationService>();
		builder.Services.AddSingleton<IPostureService, PostureService>();
		builder.Services.AddSingleton<IAuthService, AuthService>();
		builder.Services.AddSingleton<IFormService, FormService>();
		
		// Registrar App
		builder.Services.AddSingleton<App>();
		
		// Registrar ViewModels
		builder.Services.AddTransient<CameraPageViewModel>();
		builder.Services.AddTransient<MapPageViewModel>();
		builder.Services.AddTransient<PosturePageViewModel>();
		builder.Services.AddTransient<LoginViewModel>();
		builder.Services.AddTransient<FormListViewModel>();
		builder.Services.AddTransient<FormEditViewModel>();
		
		// Registrar Views
		builder.Services.AddTransient<CameraPage>();
		builder.Services.AddTransient<MapPage>();
		builder.Services.AddTransient<PosturePage>();
		builder.Services.AddTransient<LoginPage>();
		builder.Services.AddTransient<FormListPage>();
		builder.Services.AddTransient<FormEditPage>();
		builder.Services.AddTransient<MainShell>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
