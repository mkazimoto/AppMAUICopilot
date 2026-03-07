namespace CameraApp.UITest.Tests;

public class NavigationTests(AppiumFixture fixture) : IClassFixture<AppiumFixture>
{
    private readonly AppiumDriver _driver = fixture.Driver;

    [Fact]
    public void Navigation_AppLaunch_ShowsLoginPage()
        => Assert.True(new LoginPageObject(_driver).IsVisible());

    [Fact]
    public void Navigation_AfterLogin_ShowsFormList()
    {
        var login = new LoginPageObject(_driver);
        if (!login.IsVisible()) return;
        login.EnterUsername("user@test.com").EnterPassword("senha123").TapLogin();
        Assert.True(new FormListPageObject(_driver).IsVisible());
    }

    [Fact]
    public void Navigation_FormList_ToEdit_AndBack()
    {
        var login = new LoginPageObject(_driver);
        if (login.IsVisible()) login.EnterUsername("user@test.com").EnterPassword("senha123").TapLogin();
        var list = new FormListPageObject(_driver);
        Assert.True(list.IsVisible());
        list.TapAdd();
        var edit = new FormEditPageObject(_driver);
        Assert.True(edit.IsVisible());
        edit.TapCancel();
        Assert.True(list.IsVisible());
    }

    [Fact]
    public void Navigation_DeepFlow_FormList_Filter_Apply_BackToList()
    {
        var login = new LoginPageObject(_driver);
        if (login.IsVisible()) login.EnterUsername("user@test.com").EnterPassword("senha123").TapLogin();
        var list = new FormListPageObject(_driver);
        Assert.True(list.IsVisible());
        list.TapFilter();
        var filters = new AdvancedFiltersPageObject(_driver);
        Assert.True(filters.IsVisible());
        filters.TapApply();
        Assert.True(list.IsVisible());
    }

    [Fact]
    public void Navigation_CompleteFlow_Login_List_Add_Save_BackToList()
    {
        var login = new LoginPageObject(_driver);
        if (login.IsVisible()) login.EnterUsername("user@test.com").EnterPassword("senha123").TapLogin();
        var list = new FormListPageObject(_driver);
        Assert.True(list.IsVisible());
        var initial = list.GetItemCount();
        list.TapAdd();
        new FormEditPageObject(_driver)
            .SetTitle($"Teste {Guid.NewGuid():N}")
            .SetDescription("Criado via teste de navegação.")
            .TapSave();
        Assert.True(list.IsVisible());
        Assert.True(list.GetItemCount() >= initial);
    }
}
