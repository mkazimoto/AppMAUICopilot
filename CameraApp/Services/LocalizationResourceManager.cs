using System.ComponentModel;
using System.Globalization;
using CameraApp.Resources.Strings;

namespace CameraApp.Services;

/// <summary>
/// Manages runtime localization by exposing resource strings as bindable properties via an indexer.
/// </summary>
/// <remarks>
/// Usage in XAML (dynamic binding):
/// <code language="xaml">
/// xmlns:services="clr-namespace:CameraApp.Services"
/// Text="{Binding [Login_SignIn], Source={x:Static services:LocalizationResourceManager.Instance}}"
/// </code>
///
/// Usage in XAML (static, does not update on language change):
/// <code language="xaml">
/// xmlns:resx="clr-namespace:CameraApp.Resources.Strings"
/// Text="{x:Static resx:AppResources.Login_SignIn}"
/// </code>
///
/// Usage in C#:
/// <code language="csharp">
/// string text = LocalizationResourceManager.Instance["Login_SignIn"];
/// </code>
/// </remarks>
public class LocalizationResourceManager : INotifyPropertyChanged
{
    private static readonly Lazy<LocalizationResourceManager> _instance =
        new(() => new LocalizationResourceManager());

    /// <summary>
    /// Gets the shared singleton instance of <see cref="LocalizationResourceManager" />.
    /// </summary>
    public static LocalizationResourceManager Instance => _instance.Value;

    private LocalizationResourceManager()
    {
        // Inicializa com a cultura atual do dispositivo
        SetCulture(CultureInfo.CurrentUICulture);
    }

    /// <summary>
    /// Gets the localized string for the specified resource key.
    /// </summary>
    /// <value>The localized string; or the key surrounded by brackets if the key is not found.</value>
    public string this[string key] =>
        AppResources.ResourceManager.GetString(key, AppResources.Culture) ?? $"[{key}]";

    /// <summary>
    /// Gets the currently active culture.
    /// </summary>
    /// <value>The <see cref="CultureInfo" /> that is currently applied to resource lookups and the UI.</value>
    public CultureInfo CurrentCulture => AppResources.Culture ?? CultureInfo.CurrentUICulture;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets the list of cultures supported by the application.
    /// </summary>
    public static IReadOnlyList<CultureInfo> SupportedCultures { get; } =
    [
        new CultureInfo("pt"),
        new CultureInfo("en"),
        new CultureInfo("es"),
    ];

    /// <summary>
    /// Changes the active UI culture at runtime, triggering all XAML bindings referencing this instance to refresh.
    /// </summary>
    /// <param name="culture">The new culture to apply.</param>
    public void SetCulture(CultureInfo culture)
    {
        AppResources.Culture = culture;
        CultureInfo.CurrentUICulture = culture;
        CultureInfo.CurrentCulture = culture;

        // Notifica todos os bindings para releitura das strings
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
    }

    /// <summary>
    /// Changes the active UI culture using a culture code string.
    /// </summary>
    /// <param name="cultureCode">The culture code to apply (for example, <c>"pt"</c>, <c>"en"</c>, or <c>"es"</c>).</param>
    public void SetCulture(string cultureCode) =>
        SetCulture(new CultureInfo(cultureCode));
}
