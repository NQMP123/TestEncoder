using UnityEngine;

    internal class Background
    {
        private static Texture2D textureBGR;
        public static Image imgBgr;
        public static void paintBG(mGraphics g)
        {
        textureBGR = Resources.Load<Texture2D>("/res/x2/bgr/background");
        Texture2D rsT = Resize(textureBGR,mGraphics.zoomLevel);
        imgBgr = Image.createImage(rsT.EncodeToPNG());
        g.drawImage(imgBgr, 0, 0);
        }
        static Texture2D Resize(Texture2D t, int zoom)
    {
        int width = t.width * zoom;
        int height = t.height * zoom;
        Texture2D resizei = new Texture2D(width,height);
        Color[] pixel = resizei.GetPixels(0);
        Color[] original = t.GetPixels(0);
        for(int i = 0; i < height; i++)
        {
            for(int j = 0; j < width; j++)
            {
                int orgX = Mathf.FloorToInt(j / (float)zoom);
                int orgY = Mathf.FloorToInt(i / (float)zoom);
                pixel[i * width + j] = original[orgY * t.width + orgX];
            }
        }
        resizei.SetPixels(pixel, 0);
        resizei.Apply();
        return resizei;
    }
    }

