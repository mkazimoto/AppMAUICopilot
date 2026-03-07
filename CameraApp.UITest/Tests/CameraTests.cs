namespace CameraApp.UITest.Tests;

public class CameraTests(AppiumFixture fixture) : IClassFixture<AppiumFixture>
{
    private readonly AppiumDriver _driver = fixture.Driver;

    private CameraPageObject NavigateToCamera()
    {
        var login = new LoginPageObject(_driver);
        if (login.IsVisible()) login.EnterUsername("user@test.com").EnterPassword("senha123").TapLogin();
        var list = new FormListPageObject(_driver);
        Assert.True(list.IsVisible());
        list.TapAdd();
        var edit = new FormEditPageObject(_driver);
        Assert.True(edit.IsVisible());
        edit.TapCamera();
        var page = new CameraPageObject(_driver);
        Assert.True(page.IsVisible());
        return page;
    }

    [Fact] public void Camera_OnOpen_ShowsCaptureButton() => Assert.True(NavigateToCamera().IsVisible());

    [Fact]
    public void Camera_TapClose_ReturnsToFormEdit()
    {
        NavigateToCamera().TapClose();
        Assert.True(new FormEditPageObject(_driver).IsVisible());
    }

    [Fact]
    public void Camera_TapCapture_ShowsPreviewAndConfirm()
    {
        var page = NavigateToCamera();
        page.TapCapture();
        Assert.True(page.IsConfirmVisible());
        Assert.True(page.IsPreviewVisible());
    }

    [Fact]
    public void Camera_TapRetake_ReturnsToLivePreview()
    {
        var page = NavigateToCamera();
        page.TapCapture();
        Assert.True(page.IsConfirmVisible());
        page.TapRetake();
        Assert.True(page.IsVisible());
        Assert.False(page.IsConfirmVisible());
    }

    [Fact]
    public void Camera_TapConfirm_ReturnsToFormEdit()
    {
        var page = NavigateToCamera();
        page.TapCapture();
        Assert.True(page.IsConfirmVisible());
        page.TapConfirm();
        Assert.True(new FormEditPageObject(_driver).IsVisible());
    }
}
