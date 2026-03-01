using System.Globalization;
using System.Resources;

namespace CameraApp.Resources.Strings;

public static class AppResources
{
  private static readonly ResourceManager ResourceManagerInstance = new("CameraApp.Resources.Strings.AppResources", typeof(AppResources).Assembly);
  private static CultureInfo? resourceCulture;

  public static ResourceManager ResourceManager => ResourceManagerInstance;

  public static CultureInfo? Culture
  {
    get => resourceCulture;
    set => resourceCulture = value;
  }
}
