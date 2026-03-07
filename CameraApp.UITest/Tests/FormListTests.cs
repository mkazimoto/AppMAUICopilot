namespace CameraApp.UITest.Tests;

public class FormListTests(AppiumFixture fixture) : IClassFixture<AppiumFixture>
{
    private readonly AppiumDriver _driver = fixture.Driver;

    private FormListPageObject NavigateToFormList()
    {
        var login = new LoginPageObject(_driver);
        if (login.IsVisible()) login.EnterUsername("user@test.com").EnterPassword("senha123").TapLogin();
        var page = new FormListPageObject(_driver);
        Assert.True(page.IsVisible());
        return page;
    }

    [Fact] public void FormList_OnLoad_DisplaysCollection() => Assert.True(NavigateToFormList().IsVisible());

    [Fact]
    public void FormList_Search_FiltersResults()
    {
        var page = NavigateToFormList();
        var initial = page.GetItemCount();
        page.SearchFor("Formulário");
        Assert.True(page.GetItemCount() <= initial);
    }

    [Fact]
    public void FormList_Search_WithNoMatch_ShowsEmptyState()
    {
        var page = NavigateToFormList();
        page.SearchFor("xyzABCDEF_SemResultado_12345");
        Assert.True(page.IsEmpty() || page.GetItemCount() == 0);
    }

    [Fact]
    public void FormList_TapFilter_OpensAdvancedFilters()
    {
        NavigateToFormList().TapFilter();
        Assert.True(new AdvancedFiltersPageObject(_driver).IsVisible());
    }

    [Fact]
    public void FormList_TapAdd_OpensFormEdit()
    {
        NavigateToFormList().TapAdd();
        Assert.True(new FormEditPageObject(_driver).IsVisible());
    }

    [Fact]
    public void FormList_TapItem_OpensFormEdit()
    {
        var page = NavigateToFormList();
        if (page.GetItemCount() == 0) return;
        page.TapItemAtIndex(0);
        Assert.True(new FormEditPageObject(_driver).IsVisible());
    }
}
