using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShotManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        CaptureScreenShot();
    }
    void CaptureScreenShot()
    {
        GameObject screenshotObject = new GameObject("ScreenShotObject");
        ScreenShot screenShot = screenshotObject.AddComponent<ScreenShot>();
        screenShot.CaptureAndReduceScreenshot();
    }
}
