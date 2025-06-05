using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HairMod
{
    public class ClickHandler
    {
        private ArrayList a;
        public static int MenuSelected;
        public static int cmdX;
        public static int cmdY;
        public int size()
        {
            if (a == null)
            {
                return 0;
            }
            return a.Count;
        }
        public static void paint(mGraphics g)
        {
            if (ClickController.isClick() && !TamBao.isShow)
            {
                if(Char.myCharz().taskMaint.taskId == 0) 
                { return; }
                if (!GameCanvas.panel.isShow && GameCanvas.currentDialog == null && ChatPopup.currChatPopup == null && ChatPopup.serverChatPopUp == null)
                {
                    int x = (GameCanvas.w / 2) - 120;
                    ClickController.paintClick(g,x, GameCanvas.h-75, "M");
                    if (GameCanvas.isPointerHoldIn(x, GameCanvas.h - 75, 18, 30) && GameCanvas.isPointerClick && GameCanvas.isPointerJustRelease)
                    {
                        mScreen.keyTouch = 90;
                        Service.gI().openUIZone();
                        GameCanvas.clearAllPointerEvent();
                    }
                    x += 30;



                    ClickController.paintClick(g, x, GameCanvas.h - 75, "A");
                    if (GameCanvas.isPointerHoldIn(x, GameCanvas.h - 75, 18, 30) && GameCanvas.isPointerClick && GameCanvas.isPointerJustRelease)
                    {
                        mScreen.keyTouch = 92;
                        ZamasuMain.aaMod.isAk = !ZamasuMain.aaMod.isAk;
                        GameScr.info1.addInfo("Đã " + (ZamasuMain.aaMod.isAk ? "bật" : "tắt") + " tự động đánh.", 0);
                        GameCanvas.clearAllPointerEvent();
                    }
                    x += 30;

                    ClickController.paintClick(g, x, GameCanvas.h - 75, "E");
                    if (GameCanvas.isPointerHoldIn(x, GameCanvas.h - 75, 18, 30) && GameCanvas.isPointerClick && GameCanvas.isPointerJustRelease)
                    {
                        mScreen.keyTouch = 92;
                        ZamasuMain.aaMod.isAutoHoiSinh = !ZamasuMain.aaMod.isAutoHoiSinh;
                        GameScr.info1.addInfo("Đã " + (ZamasuMain.aaMod.isAutoHoiSinh ? "bật" : "tắt") + " tự động hồi sinh.", 0);

                        GameCanvas.clearAllPointerEvent();
                    }

                    x += 30;



                    ClickController.paintClick(g, x, GameCanvas.h - 75, "G");
                    if (GameCanvas.isPointerHoldIn(x, GameCanvas.h - 75, 18, 30) && GameCanvas.isPointerClick && GameCanvas.isPointerJustRelease)
                    {
                        mScreen.keyTouch = 93;
                        bool flag10 = global::Char.myCharz().charFocus == null;
                        if (flag10)
                        {
                            GameScr.info1.addInfo("Vui Lòng Chọn Mục Tiêu!", 0);
                        }
                        else
                        {
                            Service.gI().giaodich(0, global::Char.myCharz().charFocus.charID, -1, -1);
                            GameScr.info1.addInfo("Đã Gửi Lời Mời Giao Dịch Đến: " + global::Char.myCharz().charFocus.cName, 0);
                        }
                        GameCanvas.clearAllPointerEvent();
                    }
                    x += 30;

                    ClickController.paintClick(g, x, GameCanvas.h - 75, "S");
                    if (GameCanvas.isPointerHoldIn(x, GameCanvas.h - 75, 18, 30) && GameCanvas.isPointerClick && GameCanvas.isPointerJustRelease)
                    {
                        ZamasuMain.aaMod.isAutoFocus = !ZamasuMain.aaMod.isAutoFocus;
                        GameScr.info1.addInfo("Đã " + (ZamasuMain.aaMod.isAutoFocus ? "bật" : "tắt") + " tự động Focus vào Boss.", 0);
                        GameCanvas.clearAllPointerEvent();
                    }
                    x += 30;

                    ClickController.paintClick(g, x, GameCanvas.h - 75, "X");
                    if (GameCanvas.isPointerHoldIn(x, GameCanvas.h - 75, 18, 30) && GameCanvas.isPointerClick && GameCanvas.isPointerJustRelease)
                    {
                        ZamasuMain.ShowCommand();
                        GameCanvas.clearAllPointerEvent();
                    }

                    x += 30;
                    ClickController.paintClick(g, x, GameCanvas.h - 75, "T");
                    if (GameCanvas.isPointerHoldIn(x, GameCanvas.h - 75, 18, 30) && GameCanvas.isPointerClick && GameCanvas.isPointerJustRelease)
                    {
                        AutoTrain.ShowMenu();
                        GameCanvas.clearAllPointerEvent();
                    }

                    x += 30;
                    ClickController.paintClick(g, x, GameCanvas.h - 75, "N");
                    if (GameCanvas.isPointerHoldIn(x, GameCanvas.h - 75, 18, 30) && GameCanvas.isPointerClick && GameCanvas.isPointerJustRelease)
                    {
                        AutoNhat.ShowMenu();
                        GameCanvas.clearAllPointerEvent();
                    }

             
                    ClickController.paintClick(g, 10, GameCanvas.h-125, "J");
                    if (GameCanvas.isPointerHoldIn(10, GameCanvas.h - 125, 18, 30) && GameCanvas.isPointerClick && GameCanvas.isPointerJustRelease)
                    {
                        mScreen.keyTouch = 93;
                        LoadMap.LoadMapLeft();
                        GameCanvas.clearAllPointerEvent();
                    }
             
                    ClickController.paintClick(g, 40,GameCanvas.h - 125, "K");
                    if (GameCanvas.isPointerHoldIn(40, GameCanvas.h - 125, 18, 30) && GameCanvas.isPointerClick && GameCanvas.isPointerJustRelease)
                    {
                        mScreen.keyTouch = 95;
                        LoadMap.LoadMapCenter();
                        GameCanvas.clearAllPointerEvent();
                    }
                    
                    ClickController.paintClick(g, 70, GameCanvas.h - 125, "L");
                    if (GameCanvas.isPointerHoldIn(70, GameCanvas.h - 125, 18, 30) && GameCanvas.isPointerClick && GameCanvas.isPointerJustRelease)
                    {
                        mScreen.keyTouch = 94;
                        LoadMap.LoadMapRight();
                        GameCanvas.clearAllPointerEvent();
                    }
                    if (SmallImage.imgNew[1088] == null)
                    {
                        SmallImage.createImage(1088);
                        return;
                    }
                    int w = GameScr.imgNut.getWidth();
                    int h = GameScr.imgNut.getHeight();
                    g.drawImage(GameScr.imgNut, GameCanvas.w-85, GameCanvas.h - 85);
                    SmallImage.drawSmallImage(g, 1088, GameCanvas.w - 85+w/2, GameCanvas.h - 85+h/2, 0, mGraphics.VCENTER | mGraphics.HCENTER);

                    if (GameCanvas.isPointerHoldIn(GameCanvas.w - 85, GameCanvas.h - 85, w, h) && GameCanvas.isPointerClick && GameCanvas.isPointerJustRelease)
                    {
                        mScreen.keyTouch = 91;
                        ZamasuMain.UseItem(193);
                        ZamasuMain.UseItem(194);
                        GameCanvas.clearAllPointerEvent();
                    }
                    if (SmallImage.imgNew[3896] == null)
                    {
                        SmallImage.createImage(3896);
                        return;
                    }
                    g.drawImage(GameScr.imgNut, GameCanvas.w-40, GameCanvas.h - 120);
                    SmallImage.drawSmallImage(g, 3896, GameCanvas.w - 40 + w / 2, GameCanvas.h - 120 + h / 2, 0, mGraphics.VCENTER | mGraphics.HCENTER);
                    if (GameCanvas.isPointerHoldIn(GameCanvas.w - 40, GameCanvas.h - 120, w, h) && GameCanvas.isPointerClick && GameCanvas.isPointerJustRelease)
                    {
                        mScreen.keyTouch = 93;
                        ZamasuMain.UseItem(454);
                        ZamasuMain.UseItem(921);
                        GameCanvas.clearAllPointerEvent();
                    }

                }
            }
        }
    }
}
