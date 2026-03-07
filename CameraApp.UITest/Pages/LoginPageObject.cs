namespace CameraApp.UITest.Pages;

public class LoginPageObject(AppiumDriver driver)
{
    public const string UsernameEntryId    = "LoginPage_UsernameEntry";
    public const string PasswordEntryId    = "LoginPage_PasswordEntry";
    public const string LoginButtonId      = "LoginPage_LoginButton";
    public const string LoadingIndicatorId = "LoginPage_LoadingIndicator";
    public const string ErrorMessageId     = "LoginPage_ErrorMessage";

    private AppiumElement UsernameEntry => driver.FindElement(MobileBy.AccessibilityId(UsernameEntryId));
    private AppiumElement PasswordEntry => driver.FindElement(MobileBy.AccessibilityId(PasswordEntryId));
    private AppiumElement LoginButton   => driver.FindElement(MobileBy.AccessibilityId(LoginButtonId));

    public LoginPageObject EnterUsername(string username) { UsernameEntry.Clear(); UsernameEntry.SendKeys(username); return this; }
    public LoginPageObject EnterPassword(string password) { PasswordEntry.Clear(); PasswordEntry.SendKeys(password); return this; }
    public void TapLogin() => LoginButton.Click();
    public bool IsVisible()        => driver.FindElements(MobileBy.AccessibilityId(LoginButtonId)).Count > 0;
    public bool IsLoadingVisible() => driver.FindElements(MobileBy.AccessibilityId(LoadingIndicatorId)).Count > 0;
    public bool HasErrorMessage()  => driver.FindElements(MobileBy.AccessibilityId(ErrorMessageId)).Count > 0;
    public string GetErrorMessage() => driver.FindElement(MobileBy.AccessibilityId(ErrorMessageId)).Text;
}
