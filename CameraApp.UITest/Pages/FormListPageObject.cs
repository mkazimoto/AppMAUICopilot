namespace CameraApp.UITest.Pages;

public class FormListPageObject(AppiumDriver driver)
{
    public const string FormsCollectionId   = "FormListPage_FormsCollection";
    public const string SearchEntryId       = "FormListPage_SearchEntry";
    public const string FilterButtonId      = "FormListPage_FilterButton";
    public const string AddButtonId         = "FormListPage_AddButton";
    public const string LoadingIndicatorId  = "FormListPage_LoadingIndicator";
    public const string EmptyStateMessageId = "FormListPage_EmptyStateMessage";

    private AppiumElement FormsCollection => driver.FindElement(MobileBy.AccessibilityId(FormsCollectionId));
    private AppiumElement SearchEntry     => driver.FindElement(MobileBy.AccessibilityId(SearchEntryId));
    private AppiumElement FilterButton    => driver.FindElement(MobileBy.AccessibilityId(FilterButtonId));
    private AppiumElement AddButton       => driver.FindElement(MobileBy.AccessibilityId(AddButtonId));

    public FormListPageObject SearchFor(string term) { SearchEntry.Clear(); SearchEntry.SendKeys(term); return this; }
    public void TapFilter() => FilterButton.Click();
    public void TapAdd()    => AddButton.Click();
    public void TapItemAtIndex(int index) => FormsCollection.FindElements(MobileBy.ClassName("android.widget.FrameLayout"))[index].Click();
    public bool IsVisible() => driver.FindElements(MobileBy.AccessibilityId(FormsCollectionId)).Count > 0;
    public bool IsLoading() => driver.FindElements(MobileBy.AccessibilityId(LoadingIndicatorId)).Count > 0;
    public bool IsEmpty()   => driver.FindElements(MobileBy.AccessibilityId(EmptyStateMessageId)).Count > 0;
    public int GetItemCount() => FormsCollection.FindElements(MobileBy.ClassName("android.widget.FrameLayout")).Count;
}
