
//using System.Reflection;
//using UnityEngine;

//namespace RongThanCustom
//{
//    public class RongThanGraphics
//    {
//        public static long GetLastTimePress()
//        {
//            return (long)typeof(GameCanvas).GetField("lastTimePress", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
//        }
//        public static void fillRect(int x, int y, int w, int h, Color color, bool scale = true)
//        {
//            if (scale)
//            {
//                x *= mGraphics.zoomLevel;
//                y *= mGraphics.zoomLevel;
//            }
//            if (w < 0 || h < 0)
//                return;
//            Texture2D texture2D = new Texture2D(1, 1);
//            texture2D.SetPixel(0, 0, color);
//            texture2D.Apply();
//            GUI.DrawTexture(new Rect(x, y, w, h), texture2D);
//        }
//        public static int getWidth(GUIStyle gUIStyle, string s)
//        {
//            return (int)(gUIStyle.CalcSize(new GUIContent(s)).x * 1.025f / mGraphics.zoomLevel);
//        }

//        public static int getHeight(GUIStyle gUIStyle, string content)
//        {
//            return (int)gUIStyle.CalcSize(new GUIContent(content)).y / mGraphics.zoomLevel;
//        }
//        public static void updateKey()
//        {
//            if (GameCanvas.isPointerHoldIn(GameCanvas.w - GameScr.imgChat.getWidth() - LoadImage.bag.getWidth() * 2, GameScr.imgChat.getHeight() - LoadImage.bag.getHeight() + LoadImage.bag.getHeight() / 9, LoadImage.bag.getWidth(), LoadImage.bag.getHeight()) && GameCanvas.isPointerClick && GameCanvas.isPointerJustRelease)
//            {
//                mScreen.keyTouch = 102;
//                NewInventory.gI().isShow = true;
//                GameCanvas.clearAllPointerEvent();
//            }
//            else
//            {
//                mScreen.keyTouch = -1;
//            }
//            if (GameCanvas.isPointerHoldIn(GameScr.xHP - LoadImage.bongtai.getWidth() * 3 / 2, GameScr.yHP, LoadImage.bongtai.getWidth(), LoadImage.bongtai.getHeight()) && GameCanvas.isPointerClick && GameCanvas.isPointerJustRelease)
//            {
//                mScreen.keyTouch = 101;
//                if (RongThanFactory.FindItem(921))
//                {
//                    RongThanFactory.UseItem(921);
//                }
//                else
//                {
//                    RongThanFactory.UseItem(454);
//                }
//                GameCanvas.clearAllPointerEvent();
//            }
//            else
//            {
//                mScreen.keyTouch = -1;
//            }
//        }
//        public static void Paint(mGraphics g)
//        {
//            if (Char.isLoadingMap || GameCanvas.panel.isShow || GameCanvas.menu.showMenu || ChatTextField.gI().isShow)
//            {
//                return;
//            }
//            g.drawImage((mScreen.keyMouse != 102) ? LoadImage.bag : LoadImage.bagfocus, GameCanvas.w - GameScr.imgChat.getWidth() - LoadImage.bag.getWidth() * 2, GameScr.imgChat.getHeight() - LoadImage.bag.getHeight() + LoadImage.bag.getHeight() / 9);
//            if (RongThanFactory.FindItem(921) || RongThanFactory.FindItem(454))
//                    g.drawImage((mScreen.keyTouch != 101) ? LoadImage.bongtai : LoadImage.bongtai_focus, GameScr.xHP - LoadImage.bongtai.getWidth() * 3 / 2, GameScr.yHP);
                
            
//        }
//    }
//}
