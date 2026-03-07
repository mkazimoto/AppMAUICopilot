namespace CameraApp.UITest;

public class AppiumFixture : IDisposable
{
    private const string AppiumServerUri = "http://localhost:4723/";
    private const string ApkRelativePath =
        @"..\CameraApp\bin\Debug\net10.0-android\com.companyname.cameraapp-Signed.apk";

    public AppiumDriver Driver { get; }

    public AppiumFixture()
    {
        var options = new AppiumOptions();
        options.PlatformName = "Android";
        options.App = Path.GetFullPath(ApkRelativePath);
        options.AddAdditionalAppiumOption("appium:automationName", "UIAutomator2");
        options.AddAdditionalAppiumOption("appium:newCommandTimeout", 120);
        options.AddAdditionalAppiumOption("appium:noReset", false);

        Driver = new AndroidDriver(new Uri(AppiumServerUri), options);
        Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
    }

    public void Dispose() => Driver?.Quit();
}
