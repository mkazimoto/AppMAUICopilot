namespace CameraApp.UITest.Pages;

public class AdvancedFiltersPageObject(AppiumDriver driver)
{
    public const string DateFromEntryId = "AdvancedFiltersPage_DateFromEntry";
    public const string DateToEntryId   = "AdvancedFiltersPage_DateToEntry";
    public const string ApplyButtonId   = "AdvancedFiltersPage_ApplyButton";
    public const string ClearButtonId   = "AdvancedFiltersPage_ClearButton";

    private AppiumElement DateFromEntry => driver.FindElement(MobileBy.AccessibilityId(DateFromEntryId));
    private AppiumElement DateToEntry   => driver.FindElement(MobileBy.AccessibilityId(DateToEntryId));
    private AppiumElement ApplyButton   => driver.FindElement(MobileBy.AccessibilityId(ApplyButtonId));
    private AppiumElement ClearButton   => driver.FindElement(MobileBy.AccessibilityId(ClearButtonId));

    public AdvancedFiltersPageObject SetDateFrom(string date) { DateFromEntry.Clear(); DateFromEntry.SendKeys(date); return this; }
    public AdvancedFiltersPageObject SetDateTo(string date)   { DateToEntry.Clear(); DateToEntry.SendKeys(date); return this; }
    public void TapApply() => ApplyButton.Click();
    public void TapClear() => ClearButton.Click();
    public bool IsVisible() => driver.FindElements(MobileBy.AccessibilityId(ApplyButtonId)).Count > 0;
}
