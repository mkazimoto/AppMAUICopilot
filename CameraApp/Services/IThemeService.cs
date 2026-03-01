namespace CameraApp.Services
{
  public interface IThemeService
  {
    void Initialize();
    void ApplyTheme(AppTheme theme);
    void FollowSystemTheme();
  }
}
