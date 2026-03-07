namespace CameraApp.UITest.Tests;

public class LoginTests(AppiumFixture fixture) : IClassFixture<AppiumFixture>
{
    private readonly AppiumDriver _driver = fixture.Driver;

    [Fact]
    public void Login_WithValidCredentials_NavigatesToFormList()
    {
        var page = new LoginPageObject(_driver);
        page.EnterUsername("user@test.com").EnterPassword("senha123").TapLogin();
        Assert.True(new FormListPageObject(_driver).IsVisible());
    }

    [Fact]
    public void Login_WithEmptyCredentials_ShowsErrorMessage()
    {
        var page = new LoginPageObject(_driver);
        page.TapLogin();
        Assert.True(page.HasErrorMessage());
        Assert.NotEmpty(page.GetErrorMessage());
    }

    [Fact]
    public void Login_WithInvalidPassword_ShowsErrorMessage()
    {
        var page = new LoginPageObject(_driver);
        page.EnterUsername("user@test.com").EnterPassword("senhaErrada").TapLogin();
        Assert.True(page.HasErrorMessage());
    }

    [Fact]
    public void Login_WithInvalidEmail_ShowsErrorMessage()
    {
        var page = new LoginPageObject(_driver);
        page.EnterUsername("emailinvalido").EnterPassword("senha123").TapLogin();
        Assert.True(page.HasErrorMessage());
    }

    [Fact]
    public void Login_WhileLoading_ShowsLoadingIndicatorOrNavigates()
    {
        var page = new LoginPageObject(_driver);
        page.EnterUsername("user@test.com").EnterPassword("senha123").TapLogin();
        Assert.True(page.IsLoadingVisible() || new FormListPageObject(_driver).IsVisible());
    }
}
