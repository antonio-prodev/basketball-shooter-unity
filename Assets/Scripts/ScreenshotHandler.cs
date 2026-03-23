using UnityEngine;

public class ScreenshotHandler : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) // Press 'K' to capture
        {
            string fileName = "Screenshot_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
            ScreenCapture.CaptureScreenshot(fileName);
            Debug.Log("Screenshot saved as: " + fileName);
        }
    }
}