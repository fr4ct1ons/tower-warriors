using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ScreenshotTaker : MonoBehaviour
{
    [Tooltip("Name of the screenshot. The screenshot will be saved in the project's root folder." +
             "By using / it is possible to create any folders. Do NOT add any extensions such as png or jpg.")]
    [SerializeField] private string screenshotName;

    [ContextMenu("Take screenshot")]
    public void TakeScreenshot()
    {
        ScreenCapture.CaptureScreenshot(screenshotName + ".png");
    }
}
