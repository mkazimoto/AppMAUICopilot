using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CameraApp.ViewModels;

public partial class RichTextEditorViewModel : ObservableObject
{
    [ObservableProperty]
    private string title = string.Empty;

    [ObservableProperty]
    private string htmlContent = string.Empty;

    [ObservableProperty]
    private bool isSaving;

    [ObservableProperty]
    private string pageTitle = "Novo Conteúdo";

    [ObservableProperty]
    private bool isEditMode;

    /// <summary>Set by the view to retrieve current HTML from the WebView before saving.</summary>
    public Func<Task<string>>? GetEditorContentAsync { get; set; }

    /// <summary>Set by the view to clear the WebView editor content.</summary>
    public Func<Task>? ClearEditorAsync { get; set; }

    public void LoadForEdit(string id, string existingTitle, string existingHtml)
    {
        IsEditMode = true;
        PageTitle = "Editar Conteúdo";
        Title = existingTitle;
        HtmlContent = existingHtml;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (GetEditorContentAsync != null)
            HtmlContent = await GetEditorContentAsync();

        if (string.IsNullOrWhiteSpace(Title))
        {
            await Shell.Current.DisplayAlertAsync("Atenção", "O título é obrigatório.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(HtmlContent) || HtmlContent == "<p><br></p>")
        {
            await Shell.Current.DisplayAlertAsync("Atenção", "O conteúdo não pode estar vazio.", "OK");
            return;
        }

        IsSaving = true;
        try
        {
            await Task.Delay(500);
            await Shell.Current.DisplayAlertAsync("Sucesso", "Conteúdo salvo com sucesso!", "OK");
            await Shell.Current.GoToAsync("..");
        }
        finally
        {
            IsSaving = false;
        }
    }

    [RelayCommand]
    private async Task ClearAsync()
    {
        bool confirm = await Shell.Current.DisplayAlertAsync(
            "Confirmação",
            "Deseja limpar todo o conteúdo do editor?",
            "Limpar",
            "Cancelar");

        if (confirm)
        {
            if (ClearEditorAsync != null)
                await ClearEditorAsync();
            HtmlContent = string.Empty;
        }
    }
}
