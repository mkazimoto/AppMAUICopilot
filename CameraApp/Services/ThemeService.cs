using CameraApp.Resources.Themes;

namespace CameraApp.Services
{
  public class ThemeService : IThemeService
  {
    private const string StylesDictionaryPath = "Resources/Styles/Styles.xaml";
    private bool _isInitialized;

    public void Initialize()
    {
      if (_isInitialized || Application.Current is null)
      {
        return;
      }

      Application.Current.RequestedThemeChanged += OnRequestedThemeChanged;
      _isInitialized = true;

      FollowSystemTheme();
    }

    public void ApplyTheme(AppTheme theme)
    {
      if (Application.Current is null)
      {
        return;
      }

      var forcedTheme = theme == AppTheme.Unspecified ? AppTheme.Light : theme;
      Application.Current.UserAppTheme = forcedTheme;
      ApplyThemeDictionary(forcedTheme);
    }

    public void FollowSystemTheme()
    {
      if (Application.Current is null)
      {
        return;
      }

      Application.Current.UserAppTheme = AppTheme.Unspecified;
      ApplyThemeDictionary(Application.Current.RequestedTheme);
    }

    private static void OnRequestedThemeChanged(object? sender, AppThemeChangedEventArgs e)
    {
      if (Application.Current?.UserAppTheme != AppTheme.Unspecified)
      {
        return;
      }

      ApplyThemeDictionary(e.RequestedTheme);
    }

    private static void ApplyThemeDictionary(AppTheme theme)
    {
      if (Application.Current is null)
      {
        return;
      }

      ResourceDictionary themeDictionary = theme == AppTheme.Dark
        ? new DarkTheme()
        : new LightTheme();

      var mergedDictionaries = Application.Current.Resources.MergedDictionaries;
      mergedDictionaries.Clear();
      mergedDictionaries.Add(themeDictionary);
      mergedDictionaries.Add(new ResourceDictionary
      {
        Source = new Uri(StylesDictionaryPath, UriKind.Relative)
      });
    }
  }
}
