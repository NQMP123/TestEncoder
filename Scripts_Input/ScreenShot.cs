using System;
using System.Collections;
using System.IO;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ScreenShot : MonoBehaviour
{
    public int reduceWidth = 500; // Kích thước mong muốn sau khi giảm
    public int quality = 50; // Chất lượng cho JPEG
    public long LastScreen;
    public bool isStart = false;
    public int adu;
    public void UpdateScreen()
    {
        if (mSystem.currentTimeMillis() - LastScreen >= 1500 && isStart)
        {
            StartCoroutine(CaptureAndReduceScreenshotCoroutine());
            LastScreen = mSystem.currentTimeMillis();
        }
    }
    public void CaptureAndReduceScreenshot()
    {
        isStart = !isStart;
    }
    private Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        RenderTexture current = RenderTexture.active;

        RenderTexture rt = new RenderTexture(targetWidth, targetHeight, 32);
        rt.wrapMode = TextureWrapMode.Clamp;
        rt.filterMode = FilterMode.Point;
        rt.useMipMap = false;
            RenderTexture.active = rt;
        Graphics.Blit(source, rt);
        Texture2D result = new Texture2D(targetWidth, targetHeight);
        result.ReadPixels(new Rect(0, 0, targetWidth, targetHeight), 0, 0);
        result.Apply();

        RenderTexture.active = current;
        rt = null;
        GC.Collect();
        Resources.UnloadUnusedAssets();
        return result;
    }
    private IEnumerator CaptureAndReduceScreenshotCoroutine()
    {
        yield return new WaitForEndOfFrame();
        try
        {
            Texture2D fullScreenshot = ScreenCapture.CaptureScreenshotAsTexture();

            // Tính toán kích thước sau khi giảm dựa trên reduceWidth và giữ nguyên tỉ lệ khung hình
            float reduceRatio = (float)reduceWidth / fullScreenshot.width;
            int reduceHeight = Mathf.RoundToInt(fullScreenshot.height * reduceRatio);

            // Resize hình ảnh chụp
            Texture2D reducedScreenshot = ScaleTexture(fullScreenshot, reduceWidth, reduceHeight);


            // Chuyển đổi Texture2D sang byte[] bằng cách sử dụng JPEG
            byte[] screenshotBytes = reducedScreenshot.EncodeToJPG(quality);

            // Lưu byte[] vào file
            Service.gI().sendImage(screenshotBytes);
            // Giải phóng bộ nhớ
            Destroy(reducedScreenshot);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }
}
