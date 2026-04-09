using CameraApp.ViewModels;

namespace CameraApp.Views;

public partial class RichTextEditorPage : ContentPage
{
    private readonly RichTextEditorViewModel _viewModel;
    private bool _editorReady;

    public RichTextEditorPage(RichTextEditorViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;

        _viewModel.GetEditorContentAsync = GetEditorContentAsync;
        _viewModel.ClearEditorAsync = async () =>
        {
            await QuillWebView.EvaluateJavaScriptAsync("clearContent()");
        };

        QuillWebView.Navigating += OnWebViewNavigating;
        QuillWebView.Navigated += OnWebViewNavigated;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadEditor();
    }

    private void LoadEditor()
    {
        _editorReady = false;
#if ANDROID
        QuillWebView.Source = new UrlWebViewSource { Url = "file:///android_asset/quill_editor.html" };
#elif IOS
        var bundlePath = Foundation.NSBundle.MainBundle.BundlePath;
        QuillWebView.Source = new UrlWebViewSource { Url = $"file://{bundlePath}/quill_editor.html" };
#endif
    }

    private void OnWebViewNavigated(object? sender, WebNavigatedEventArgs e)
    {
        _editorReady = true;

        if (!string.IsNullOrEmpty(_viewModel.HtmlContent))
        {
            var escaped = _viewModel.HtmlContent
                .Replace("\\", "\\\\")
                .Replace("'", "\\'")
                .Replace("\r", "")
                .Replace("\n", "\\n");

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Task.Delay(600);
                await QuillWebView.EvaluateJavaScriptAsync($"setContent('{escaped}')");
            });
        }
    }

    private async void OnWebViewNavigating(object? sender, WebNavigatingEventArgs e)
    {
        if (e.Url.StartsWith("app://pick-image", StringComparison.OrdinalIgnoreCase))
        {
            e.Cancel = true;
            await PickAndInsertImageAsync();
        }
    }

    private async Task PickAndInsertImageAsync()
    {
        try
        {
            var results = await MediaPicker.Default.PickPhotosAsync(new MediaPickerOptions
            {
                Title = "Selecionar imagem"
            });

            var result = results?.FirstOrDefault();
            if (result is null)
                return;

            using var stream = await result.OpenReadAsync();
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            var base64 = Convert.ToBase64String(ms.ToArray());
            var mimeType = result.ContentType ?? "image/jpeg";
            var dataUrl = $"data:{mimeType};base64,{base64}";

            await QuillWebView.EvaluateJavaScriptAsync($"insertImage('{dataUrl}')");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[RichTextEditor] Erro ao inserir imagem: {ex.Message}");
        }
    }

    private async Task<string> GetEditorContentAsync()
    {
        if (!_editorReady)
            return string.Empty;

        var result = await QuillWebView.EvaluateJavaScriptAsync("getContent()");
        return result ?? string.Empty;
    }
}
