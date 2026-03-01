using System.ComponentModel;
using System.Globalization;
using CameraApp.Resources.Strings;

namespace CameraApp.Services;

public class LocalizationResourceManager : INotifyPropertyChanged
{
  public static LocalizationResourceManager Instance { get; } = new();

  public event PropertyChangedEventHandler? PropertyChanged;

  private LocalizationResourceManager()
  {
    SetCulture(CultureInfo.CurrentUICulture);
  }

  public string this[string key] => AppResources.ResourceManager.GetString(key, AppResources.Culture) ?? key;

  public void SetCulture(CultureInfo culture)
  {
    AppResources.Culture = culture;
    CultureInfo.CurrentUICulture = culture;
    CultureInfo.CurrentCulture = culture;

    if (Application.Current is not null)
    {
      Application.Current.MainPage?.Dispatcher.Dispatch(() =>
      {
        if (Application.Current.MainPage is not null)
        {
          Application.Current.MainPage.FlowDirection = culture.TextInfo.IsRightToLeft
            ? FlowDirection.RightToLeft
            : FlowDirection.LeftToRight;
        }
      });
    }

    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
  }
}
