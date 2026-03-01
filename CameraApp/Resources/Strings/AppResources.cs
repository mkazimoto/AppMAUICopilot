// Classe de acesso a recursos de localização - gerada manualmente para suporte ao VS Code.
// Em Visual Studio, este arquivo seria gerado automaticamente pelo ResXFileCodeGenerator.
// Após adicionar novas chaves ao .resx, adicionar as propriedades correspondentes aqui.
namespace CameraApp.Resources.Strings;

/// <summary>
/// Classe de acesso fortemente tipado aos recursos de localização do aplicativo.
/// </summary>
public static class AppResources
{
    private static System.Resources.ResourceManager? _resourceManager;
    private static System.Globalization.CultureInfo? _culture;

    /// <summary>
    /// Retorna o ResourceManager desta classe.
    /// </summary>
    public static System.Resources.ResourceManager ResourceManager =>
        _resourceManager ??= new System.Resources.ResourceManager(
            "CameraApp.Resources.Strings.AppResources",
            typeof(AppResources).Assembly);

    /// <summary>
    /// Substitui a cultura atual utilizada para todas as consultas de recursos.
    /// Defina como <c>null</c> para usar a cultura do thread atual.
    /// </summary>
    public static System.Globalization.CultureInfo? Culture
    {
        get => _culture;
        set => _culture = value;
    }

    // ── App geral ──────────────────────────────────────────────────────────
    public static string AppTitle => Get("AppTitle");
    public static string AppSubtitle => Get("AppSubtitle");

    // ── Login ──────────────────────────────────────────────────────────────
    public static string Login_AccessData => Get("Login_AccessData");
    public static string Login_User => Get("Login_User");
    public static string Login_UserPlaceholder => Get("Login_UserPlaceholder");
    public static string Login_Password => Get("Login_Password");
    public static string Login_PasswordPlaceholder => Get("Login_PasswordPlaceholder");
    public static string Login_ServiceAlias => Get("Login_ServiceAlias");
    public static string Login_ServiceAliasHint => Get("Login_ServiceAliasHint");
    public static string Login_Clear => Get("Login_Clear");
    public static string Login_SignIn => Get("Login_SignIn");
    public static string Login_Authenticated => Get("Login_Authenticated");
    public static string Login_AuthenticatedMessage => Get("Login_AuthenticatedMessage");
    public static string Login_SignOut => Get("Login_SignOut");
    public static string Login_Authenticating => Get("Login_Authenticating");

    // ── Lista de Formulários ───────────────────────────────────────────────
    public static string FormList_Title => Get("FormList_Title");
    public static string FormList_SearchPlaceholder => Get("FormList_SearchPlaceholder");
    public static string FormList_ActiveFilters => Get("FormList_ActiveFilters");
    public static string FormList_AdvancedFilters => Get("FormList_AdvancedFilters");
    public static string FormList_New => Get("FormList_New");
    public static string FormList_ClearFilters => Get("FormList_ClearFilters");
    public static string FormList_Category => Get("FormList_Category");
    public static string FormList_Status => Get("FormList_Status");
    public static string FormList_Score => Get("FormList_Score");
    public static string FormList_Sequential => Get("FormList_Sequential");
    public static string FormList_CreatedAt => Get("FormList_CreatedAt");
    public static string FormList_By => Get("FormList_By");
    public static string FormList_Empty => Get("FormList_Empty");

    // ── Edição de Formulário ───────────────────────────────────────────────
    public static string FormEdit_Subtitle => Get("FormEdit_Subtitle");
    public static string FormEdit_TitleField => Get("FormEdit_TitleField");
    public static string FormEdit_TitlePlaceholder => Get("FormEdit_TitlePlaceholder");
    public static string FormEdit_Category => Get("FormEdit_Category");
    public static string FormEdit_Status => Get("FormEdit_Status");
    public static string FormEdit_Score => Get("FormEdit_Score");
    public static string FormEdit_Save => Get("FormEdit_Save");
    public static string FormEdit_Cancel => Get("FormEdit_Cancel");

    // ── Mapa ───────────────────────────────────────────────────────────────
    public static string Map_CurrentLocation => Get("Map_CurrentLocation");
    public static string Map_LocationUnavailable => Get("Map_LocationUnavailable");
    public static string Map_UpdateLocation => Get("Map_UpdateLocation");
    public static string Map_ResetZoom => Get("Map_ResetZoom");

    // ── Postura ────────────────────────────────────────────────────────────
    public static string Posture_MonitoringStatus => Get("Posture_MonitoringStatus");
    public static string Posture_Start => Get("Posture_Start");
    public static string Posture_Stop => Get("Posture_Stop");
    public static string Posture_AccelerometerData => Get("Posture_AccelerometerData");
    public static string Posture_Settings => Get("Posture_Settings");
    public static string Posture_SensitivityHint => Get("Posture_SensitivityHint");
    public static string Posture_AlertTimeHint => Get("Posture_AlertTimeHint");
    public static string Posture_AlertStats => Get("Posture_AlertStats");
    public static string Posture_Reset => Get("Posture_Reset");
    public static string Posture_TotalAlerts => Get("Posture_TotalAlerts");
    public static string Posture_LastAlert => Get("Posture_LastAlert");
    public static string Posture_Message => Get("Posture_Message");
    public static string Posture_HowToUse => Get("Posture_HowToUse");

    // ── Câmera ─────────────────────────────────────────────────────────────
    public static string Camera_TakePhoto => Get("Camera_TakePhoto");
    public static string Camera_SelectPhoto => Get("Camera_SelectPhoto");
    public static string Camera_Send => Get("Camera_Send");

    // ── Geral ──────────────────────────────────────────────────────────────
    public static string Common_Loading => Get("Common_Loading");
    public static string Common_Error => Get("Common_Error");
    public static string Common_Success => Get("Common_Success");
    public static string Common_Cancel => Get("Common_Cancel");
    public static string Common_Ok => Get("Common_Ok");
    public static string Common_Yes => Get("Common_Yes");
    public static string Common_No => Get("Common_No");
    public static string Common_Save => Get("Common_Save");
    public static string Common_Delete => Get("Common_Delete");
    public static string Common_Edit => Get("Common_Edit");
    public static string Common_Back => Get("Common_Back");

    // ── Geral (adicional) ──────────────────────────────────────────────────
    public static string Common_Processing => Get("Common_Processing");

    // ── Edição de Formulário (adicional) ──────────────────────────────────
    public static string FormEdit_ScoreHint => Get("FormEdit_ScoreHint");
    public static string FormEdit_SequentialScript => Get("FormEdit_SequentialScript");
    public static string FormEdit_SequentialScriptHint => Get("FormEdit_SequentialScriptHint");
    public static string FormEdit_SystemInfo => Get("FormEdit_SystemInfo");
    public static string FormEdit_Delete => Get("FormEdit_Delete");

    // ── Postura (adicional) ────────────────────────────────────────────────
    public static string Posture_Inclination => Get("Posture_Inclination");
    public static string Posture_InstructionStep1 => Get("Posture_InstructionStep1");
    public static string Posture_InstructionStep2 => Get("Posture_InstructionStep2");
    public static string Posture_InstructionStep3 => Get("Posture_InstructionStep3");
    public static string Posture_InstructionStep4 => Get("Posture_InstructionStep4");

    // ── Helpers ────────────────────────────────────────────────────────────
    private static string Get(string key) =>
        ResourceManager.GetString(key, _culture) ?? $"[{key}]";
}
