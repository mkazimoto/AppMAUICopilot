namespace CameraApp.UITest.Pages;

public class CameraPageObject(AppiumDriver driver)
{
    public const string CaptureButtonId    = "CameraPage_CaptureButton";
    public const string FlipCameraButtonId = "CameraPage_FlipCameraButton";
    public const string CloseButtonId      = "CameraPage_CloseButton";
    public const string PreviewImageId     = "CameraPage_PreviewImage";
    public const string ConfirmButtonId    = "CameraPage_ConfirmButton";
    public const string RetakeButtonId     = "CameraPage_RetakeButton";

    private AppiumElement CaptureButton => driver.FindElement(MobileBy.AccessibilityId(CaptureButtonId));
    private AppiumElement CloseButton   => driver.FindElement(MobileBy.AccessibilityId(CloseButtonId));
    private AppiumElement ConfirmButton => driver.FindElement(MobileBy.AccessibilityId(ConfirmButtonId));
    private AppiumElement RetakeButton  => driver.FindElement(MobileBy.AccessibilityId(RetakeButtonId));

    public void TapCapture() => CaptureButton.Click();
    public void TapClose()   => CloseButton.Click();
    public void TapConfirm() => ConfirmButton.Click();
    public void TapRetake()  => RetakeButton.Click();
    public void TapFlipCamera()
    {
        var f = driver.FindElements(MobileBy.AccessibilityId(FlipCameraButtonId));
        if (f.Count > 0) f[0].Click();
    }
    public bool IsVisible()        => driver.FindElements(MobileBy.AccessibilityId(CaptureButtonId)).Count > 0;
    public bool IsPreviewVisible() => driver.FindElements(MobileBy.AccessibilityId(PreviewImageId)).Count > 0;
    public bool IsConfirmVisible() => driver.FindElements(MobileBy.AccessibilityId(ConfirmButtonId)).Count > 0;
}
