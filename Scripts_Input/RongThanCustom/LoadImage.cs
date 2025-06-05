using MODMANHHDC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RongThanCustom
{
    public class LoadImage
    {
        public static Image CreateScrBgr,ServerListBgr;
        public static Image bag,bagfocus;
        public static Image img_soil;
        public static Image btnback, btnback1;
        public static Image btnok, btnok1;
        public static Image btnselect, btnselect1;
        public static Image inputTF;
        public static Image char0, char1, char2;
        public static Image aura0, aura1, aura2;
        public static Image imgMay;
        public static Image imgVong,imgVong1,imgVong2;
        public static Image bongtai,bongtai_focus;
        public static Image imgloading,imgwait;
        static LoadImage()
        {
            imgloading = Image.createImage("res/RongThan/screen/loading");
            imgwait = Image.createImage("res/RongThan/screen/img_wait");
            bongtai = Image.createImage("res/RongThan/gamescr/btc2");
            bongtai_focus = Image.createImage("res/RongThan/gamescr/btc2_1");
            ServerListBgr = Image.createImage("res/RongThan/screen/bgr");
            char0 = Image.createImage("res/RongThan/character/char0");
            char1 = Image.createImage("res/RongThan/character/char1");
            char2 = Image.createImage("res/RongThan/character/char2");
            aura0 = Image.createImage("res/RongThan/aura/aura0");
            aura1 = Image.createImage("res/RongThan/aura/aura1");
            aura2 = Image.createImage("res/RongThan/aura/aura2");
            imgMay = Image.createImage("res/RongThan/screen/imgMay");
            imgVong = Image.createImage("res/RongThan/screen/imgVong0");
            imgVong1 = Image.createImage("res/RongThan/screen/imgVong1");
            imgVong2 = Image.createImage("res/RongThan/screen/imgVong2");
            CreateScrBgr = Image.createImage("res/RongThan/createchar/background");
            bag = Image.createImage("res/RongThan/gamescr/bag");
            bagfocus = Image.createImage("res/RongThan/gamescr/bag1");
            img_soil = Image.createImage("res/RongThan/createchar/img_soil");
            btnback = Image.createImage("res/RongThan/createchar/btnback");
            btnback1 = Image.createImage("res/RongThan/createchar/btnback1");
            btnok = Image.createImage("res/RongThan/createchar/btnok");
            btnok1 = Image.createImage("res/RongThan/createchar/btnok1");
            btnselect = Image.createImage("res/RongThan/createchar/btn1");
            btnselect1 = Image.createImage("res/RongThan/createchar/btn2");
            inputTF = Image.createImage("res/RongThan/createchar/input");
            if(mGraphics.zoomLevel != 2)
            {
                resizeImage(CreateScrBgr);
                resizeImage(bag);
                resizeImage(bagfocus);
                resizeImage(img_soil);
                resizeImage(btnback);
                resizeImage(btnback1);
                resizeImage(btnok);
                resizeImage(btnok1);
                resizeImage(btnselect);
                resizeImage(btnselect1);
                resizeImage(inputTF);
                resizeImageBgr(ServerListBgr);
                resizeImage(char0);
                resizeImage(char1);
                resizeImage(char2);
                resizeImage(aura0);
                resizeImage(aura1);
                resizeImage(aura2);
                resizeImage(imgMay);
                resizeImage(imgVong);
                resizeImage(imgVong1);
                resizeImage(imgVong2);
                resizeImage(bongtai);
                resizeImage(bongtai_focus);
                resizeImage(imgloading);
                resizeImage(imgwait);
            }
        }
        public static void resizeImageBgr(Image img)
        {
            int h = GameCanvas.h ;

            int wx1 = img.w ;//vi la hinh anh x2 nen chuyen size ve x1 xong tinh len

            int w = wx1 * mGraphics.zoomLevel;

            img.texture = myCommand.Resize(img.texture, w, h * mGraphics.zoomLevel);
            img.w = img.texture.width;
            img.h = img.texture.height;
            Image.setTextureQuality(img.texture);
        }

        static void resizeImage(Image img)
        {
            int wx1 = img.w / 2;
            int hx1 = img.h / 2;
            int w = wx1 * mGraphics.zoomLevel;
            int h = hx1 * mGraphics.zoomLevel;
            img.texture = myCommand.Resize(img.texture, w, h);
            img.w = img.texture.width;
            img.h = img.texture.height;
            Image.setTextureQuality(img.texture);
        }
    }
}
