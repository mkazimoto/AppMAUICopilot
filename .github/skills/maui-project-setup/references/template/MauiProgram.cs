using Microsoft.Extensions.Logging;

namespace MyApp; // Replace 'MyApp' with the actual namespace

public static class MauiProgram {
    public static MauiApp CreateMauiApp() {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

#if DEBUG
    	builder.Logging.AddDebug();
#endif

        // Services
        builder.Services.AddSingleton<INavigationService, NavigationService>();

        // ViewModels
        builder.Services.AddTransient<MainViewModel>();

        // Views
        builder.Services.AddTransient<MainPage>();

        return builder.Build();
    }
}