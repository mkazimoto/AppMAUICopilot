namespace CameraApp.UITest.Pages;

public class FormEditPageObject(AppiumDriver driver)
{
    public const string TitleEntryId       = "FormEditPage_TitleEntry";
    public const string DescriptionEntryId = "FormEditPage_DescriptionEntry";
    public const string SaveButtonId       = "FormEditPage_SaveButton";
    public const string CancelButtonId     = "FormEditPage_CancelButton";
    public const string DeleteButtonId     = "FormEditPage_DeleteButton";
    public const string LoadingIndicatorId = "FormEditPage_LoadingIndicator";
    public const string ErrorMessageId     = "FormEditPage_ErrorMessage";
    public const string CameraButtonId     = "FormEditPage_CameraButton";

    private AppiumElement TitleEntry       => driver.FindElement(MobileBy.AccessibilityId(TitleEntryId));
    private AppiumElement DescriptionEntry => driver.FindElement(MobileBy.AccessibilityId(DescriptionEntryId));
    private AppiumElement SaveButton       => driver.FindElement(MobileBy.AccessibilityId(SaveButtonId));
    private AppiumElement CancelButton     => driver.FindElement(MobileBy.AccessibilityId(CancelButtonId));
    private AppiumElement CameraButton     => driver.FindElement(MobileBy.AccessibilityId(CameraButtonId));

    public FormEditPageObject SetTitle(string t)       { TitleEntry.Clear(); TitleEntry.SendKeys(t); return this; }
    public FormEditPageObject SetDescription(string d) { DescriptionEntry.Clear(); DescriptionEntry.SendKeys(d); return this; }
    public void TapSave()   => SaveButton.Click();
    public void TapCancel() => CancelButton.Click();
    public void TapCamera() => CameraButton.Click();
    public void TapDelete()
    {
        var b = driver.FindElements(MobileBy.AccessibilityId(DeleteButtonId));
        if (b.Count > 0) b[0].Click();
    }
    public bool IsVisible()       => driver.FindElements(MobileBy.AccessibilityId(SaveButtonId)).Count > 0;
    public bool IsLoading()       => driver.FindElements(MobileBy.AccessibilityId(LoadingIndicatorId)).Count > 0;
    public bool HasErrorMessage() => driver.FindElements(MobileBy.AccessibilityId(ErrorMessageId)).Count > 0;
    public string GetTitleText()    => TitleEntry.Text;
    public string GetErrorMessage() => driver.FindElement(MobileBy.AccessibilityId(ErrorMessageId)).Text;
}
