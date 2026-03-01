using System.ComponentModel;
using System.Globalization;
using CameraApp.Resources.Strings;

namespace CameraApp.Services;

/// <summary>
/// Gerenciador de localização que permite troca de idioma em tempo de execução.
/// Expõe as strings de recursos via indexador para uso em bindings XAML.
/// </summary>
/// <remarks>
/// Uso em XAML (binding dinâmico):
/// <code>
/// xmlns:services="clr-namespace:CameraApp.Services"
/// Text="{Binding [Login_SignIn], Source={x:Static services:LocalizationResourceManager.Instance}}"
/// </code>
///
/// Uso em XAML (estático, não atualiza ao mudar idioma):
/// <code>
/// xmlns:resx="clr-namespace:CameraApp.Resources.Strings"
/// Text="{x:Static resx:AppResources.Login_SignIn}"
/// </code>
///
/// Uso em C#:
/// <code>
/// string text = LocalizationResourceManager.Instance["Login_SignIn"];
/// </code>
/// </remarks>
public class LocalizationResourceManager : INotifyPropertyChanged
{
    private static readonly Lazy<LocalizationResourceManager> _instance =
        new(() => new LocalizationResourceManager());

    /// <summary>
    /// Instância singleton do gerenciador de localização.
    /// </summary>
    public static LocalizationResourceManager Instance => _instance.Value;

    private LocalizationResourceManager()
    {
        // Inicializa com a cultura atual do dispositivo
        SetCulture(CultureInfo.CurrentUICulture);
    }

    /// <summary>
    /// Obtém a string localizada pela chave.
    /// Retorna a chave entre colchetes se não encontrada.
    /// </summary>
    public string this[string key] =>
        AppResources.ResourceManager.GetString(key, AppResources.Culture) ?? $"[{key}]";

    /// <summary>
    /// Cultura atualmente ativa.
    /// </summary>
    public CultureInfo CurrentCulture => AppResources.Culture ?? CultureInfo.CurrentUICulture;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Idiomas suportados pelo aplicativo.
    /// </summary>
    public static IReadOnlyList<CultureInfo> SupportedCultures { get; } =
    [
        new CultureInfo("pt"),
        new CultureInfo("en"),
        new CultureInfo("es"),
    ];

    /// <summary>
    /// Altera o idioma da interface em tempo de execução.
    /// Todos os bindings que referenciam esta instância serão atualizados automaticamente.
    /// </summary>
    /// <param name="culture">A nova cultura a ser aplicada.</param>
    public void SetCulture(CultureInfo culture)
    {
        AppResources.Culture = culture;
        CultureInfo.CurrentUICulture = culture;
        CultureInfo.CurrentCulture = culture;

        // Notifica todos os bindings para releitura das strings
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
    }

    /// <summary>
    /// Altera o idioma usando o código de cultura (ex: "pt", "en", "es").
    /// </summary>
    /// <param name="cultureCode">Código da cultura.</param>
    public void SetCulture(string cultureCode) =>
        SetCulture(new CultureInfo(cultureCode));
}
