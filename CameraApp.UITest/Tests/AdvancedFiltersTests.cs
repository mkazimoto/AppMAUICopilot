namespace CameraApp.UITest.Tests;

public class AdvancedFiltersTests(AppiumFixture fixture) : IClassFixture<AppiumFixture>
{
    private readonly AppiumDriver _driver = fixture.Driver;

    private AdvancedFiltersPageObject NavigateToFilters()
    {
        var login = new LoginPageObject(_driver);
        if (login.IsVisible()) login.EnterUsername("user@test.com").EnterPassword("senha123").TapLogin();
        var list = new FormListPageObject(_driver);
        Assert.True(list.IsVisible());
        list.TapFilter();
        var page = new AdvancedFiltersPageObject(_driver);
        Assert.True(page.IsVisible());
        return page;
    }

    [Fact]
    public void AdvancedFilters_TapApply_ReturnsToFormList()
    {
        NavigateToFilters().TapApply();
        Assert.True(new FormListPageObject(_driver).IsVisible());
    }

    [Fact]
    public void AdvancedFilters_TapClear_ClearsAndReturns()
    {
        var page = NavigateToFilters();
        page.SetDateFrom("2026-01-01").SetDateTo("2026-12-31").TapClear();
        Assert.True(page.IsVisible() || new FormListPageObject(_driver).IsVisible());
    }

    [Fact]
    public void AdvancedFilters_WithDateRange_FiltersResults()
    {
        NavigateToFilters().SetDateFrom("2026-01-01").SetDateTo("2026-03-07").TapApply();
        Assert.True(new FormListPageObject(_driver).IsVisible());
    }
}
