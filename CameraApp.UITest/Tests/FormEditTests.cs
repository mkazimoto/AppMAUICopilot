namespace CameraApp.UITest.Tests;

public class FormEditTests(AppiumFixture fixture) : IClassFixture<AppiumFixture>
{
    private readonly AppiumDriver _driver = fixture.Driver;

    private FormEditPageObject NavigateToNewForm()
    {
        var login = new LoginPageObject(_driver);
        if (login.IsVisible()) login.EnterUsername("user@test.com").EnterPassword("senha123").TapLogin();
        var list = new FormListPageObject(_driver);
        Assert.True(list.IsVisible());
        list.TapAdd();
        var page = new FormEditPageObject(_driver);
        Assert.True(page.IsVisible());
        return page;
    }

    [Fact]
    public void FormEdit_WithValidData_SavesSuccessfully()
    {
        NavigateToNewForm().SetTitle("Formulário de Teste").SetDescription("Descrição de teste").TapSave();
        Assert.True(new FormListPageObject(_driver).IsVisible());
    }

    [Fact]
    public void FormEdit_WithEmptyTitle_ShowsValidationError()
    {
        var page = NavigateToNewForm();
        page.SetDescription("Só descrição").TapSave();
        Assert.True(page.HasErrorMessage());
    }

    [Fact]
    public void FormEdit_TapCancel_ReturnsToFormList()
    {
        NavigateToNewForm().TapCancel();
        Assert.True(new FormListPageObject(_driver).IsVisible());
    }

    [Fact]
    public void FormEdit_TapCamera_OpensCameraPage()
    {
        NavigateToNewForm().TapCamera();
        Assert.True(new CameraPageObject(_driver).IsVisible());
    }

    [Fact]
    public void FormEdit_TitleField_AcceptsInput()
    {
        var page = NavigateToNewForm();
        page.SetTitle("Meu Formulário");
        Assert.Equal("Meu Formulário", page.GetTitleText());
    }
}
