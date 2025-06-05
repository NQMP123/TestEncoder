using System;
using System.Collections.Generic;

public class TamBao
{

    public static void RequestMessage(Message msg)
    {
        byte type = msg.reader().readUnsignedByte();
        if (type == 0)
        {
            listItem.Clear();
            idNeed = msg.reader().readInt();
            int size = msg.reader().readInt();
            for (int i = 0; i < size; i++)
            {
                int id = msg.reader().readInt();
                int color = msg.reader().readInt();
                int quantity = msg.reader().readInt();
                listItem.Add(new ItemTamBao(id, color, quantity));
            }
            isShow = true;
        }
        if (type == 1)
        {
            listNhan.Clear();
            int size = msg.reader().readInt();
            for (int i = 0; i < size; i++)
            {
                int id = msg.reader().readInt();
                int color = msg.reader().readInt();
                int quantity = msg.reader().readInt();
                listNhan.Add(new ItemTamBao(id, color, quantity));
            }
            isNhan = true;
        }

    }
    public static int SoLanDaQuay = 0;
    public static int[] MocThuong = new int[]
    {
        30,100,500,1000,5000,10000
    };

    public static int w => GameCanvas.w;
    public static int h => GameCanvas.h;
    public static List<ItemTamBao> listItem = new List<ItemTamBao>();
    public static List<ItemTamBao> listNhan = new List<ItemTamBao>();

    static int idNeed = 457;
    public static bool isNhan = true;
    public static Image khung;
    public static Image background;

    public static Image chucMung;
    public static Image btnDong;
    public static Image btnReRoll;
    public static Image imgSelect;
    public static Image[] bg = new Image[5];
    public static Image[] effr = new Image[4];
    public static Image[] effy = new Image[4];
    public static Image quay;
    public static Item item = new Item();
    public static Command cmdRollx1 = new Command("", (int)(w * 0.25), (int)(h * 0.85));
    public static Command cmdRollx10 = new Command("", (int)(w * 0.7), (int)(h * 0.85));
    public static Command btnExit = new Command("", (int)(w * 0.85), (int)(h * 0.085));
    public static bool isQuay = false;
    public static long lastQuay;
    public static int soLuot;
    public static Command btnBoquaHieuUng = new Command("[X]Hiệu ứng", (int)(w * 0.85), (int)(h * 0.5));
    public class ItemTamBao
    {
        public int id;
        public int color;
        public int quantity;
        public ItemTamBao()
        {

        }
        public ItemTamBao(int id, int color, int quantity)
        {
            this.id = id;
            this.color = color;
            this.quantity = quantity;
        }
    }

    static bool isInit;
    static void init()
    {
       
        khung = mSystem.loadImage("/quayTamBao/khung.png");
        background = mSystem.loadImage("/quayTamBao/background.png");
        btnExit.img = mSystem.loadImage("/quayTamBao/btnExit.png");
        cmdRollx1.img = mSystem.loadImage("/quayTamBao/btnRollx1.png");
        cmdRollx10.img = mSystem.loadImage("/quayTamBao/btnRollx10.png");
        imgSelect = mSystem.loadImage("/quayTamBao/select.png");
        btnDong = mSystem.loadImage("/quayTamBao/btnDong.png");
        btnReRoll = mSystem.loadImage("/quayTamBao/btnReRoll.png");
        quay = mSystem.loadImage("/quayTamBao/quay.png");
        chucMung = mSystem.loadImage("/quayTamBao/chucMung.png");
        for (int i = 0; i < 5; i++)
        {
            bg[i] = mSystem.loadImage("/quayTamBao/bg" + i + ".png");
        }
        for (int i = 0; i < 4; i++)
        {
            effr[i] = mSystem.loadImage("/quayTamBao/effr" + i + ".png");
        }
        for (int i = 0; i < 4; i++)
        {
            effy[i] = mSystem.loadImage("/quayTamBao/effy" + i + ".png");
        }
    }
    static bool boquahieuung;
    public static void update()
    {
        if (!isShow)
        {
            return;
        }
        if (cmdRollx1.isPointerPressInside() && !isQuay)
        {
            lastQuay = mSystem.currentTimeMillis();
            isQuay = true;
            Service.gI().sendStartTamBao(1);
        }
        if (cmdRollx10.isPointerPressInside() && !isQuay)
        {
            lastQuay = mSystem.currentTimeMillis();
            isQuay = true;
            Service.gI().sendStartTamBao(10);
        }
        if (btnExit.isPointerPressInside())
        {
            isShow = false;
            GameScr.isHaveSelectSkill = !false;
        }
        if (btnBoquaHieuUng.isPointerPressInside())
        {
           boquahieuung = !boquahieuung;
            btnBoquaHieuUng.caption = boquahieuung ? "Hiệu Ứng" : "[X]Hiệu Ứng";

        }
    }
    public static void paintNhanThuong(mGraphics g, List<ItemTamBao> listItemz)
    {

        int y = GameCanvas.hh;
        g.drawImageScale(chucMung, GameCanvas.hw - (int)(w*0.068), (int)(h * 0.067), (int)(w * 0.2), (int)(h * 0.15), 0);
        for (int i = 0; i < listItemz.Count; i++)
        {
            int x = GameCanvas.w / 2;
            x = i % 2 == 0 ? x + ((i+1) * (int)(w * 0.035)) : x - ((i+1) * (int)(w * 0.035));
            int id = listItemz[i].id;
            g.drawImageScale(bg[listItemz[i].color % 5], x - (int)(w * 0.022), y - (int)(h * 0.04), (int)(w * 0.05), (int)(w * 0.05), 0);
            if (listItemz[i].color > 1)
            {
                g.drawImageScale(effy[count % 4], x - (int)(w * 0.032), y - (int)(h * 0.068), (int)(w * 0.075), (int)(w * 0.075), 0);
            }
            SmallImage.drawSmallImage(g, ItemTemplates.get((short)id).iconID, x, y, 0, 3);
            if (listItemz[i].quantity > 1) mFont.tahoma_7b_white.drawString(g, "x" + listItemz[i].quantity, x + (int)(w * 0.0117), y, mFont.CENTER);
        }
        mFont.tahoma_7b_red.drawString(g, "Nhấn bất kì để trở về", w / 2, (int)(h * 0.85), mFont.CENTER);
        if (mSystem.currentTimeMillis() - last > 50)
        {
            count++;
            last = mSystem.currentTimeMillis();
        }
        if (GameCanvas.isPointerDown)
        {
            isQuay = false;
        }

    }
    static int findItemQuantity(int id)
    {
        foreach (var item in Char.myCharz().arrItemBag)
        {
            if (item != null && item.template.id == id)
            {
                return item.quantity;
            }
        }
        return 0;
    }

    public static bool isShow = false;
    public static void paint(mGraphics g)
    {
        if (!isShow)
        {
            return;
        }
        GameScr.isHaveSelectSkill = false;
        if (!isInit)
        {
            isInit = true;
            init();
        }
        g.drawImageScale(background, -1, -2, GameCanvas.w + 10, GameCanvas.h + 10, 0);
        g.drawImage(khung, 5, 5);
        mFont.tahoma_7_yellow.drawString(g, "Số lượng hiện có :" + findItemQuantity(idNeed), (int)(w * 0.1), (int)(h * 0.1), mFont.LEFT);
        if (isNhan &&( mSystem.currentTimeMillis() - lastQuay >= (boquahieuung ? 500 : 5000)) && isQuay)
        {
            paintNhanThuong(g, listNhan);
        }
        else
        {
            paintListItem(g, listItem);
        }
        mFont.tahoma_7b_white.drawString(g, ("Tốn :1"), (int)(w * 0.27), (int)(h * 0.8), mFont.LEFT);

        mFont.tahoma_7b_white.drawString(g, ("Tốn :10"), (int)(w * 0.72), (int)(h * 0.8), mFont.LEFT);

        SmallImage.drawSmallImage(g, ItemTemplates.get((short)idNeed).iconID, (int)(w * 0.28) + mFont.tahoma_7b_white.getWidth("Tốn :1     "), (int)(h * 0.82), 0, 3);
        SmallImage.drawSmallImage(g, ItemTemplates.get((short)idNeed).iconID, (int)(w * 0.73) + mFont.tahoma_7b_white.getWidth("Tốn :10     "), (int)(h * 0.82), 0, 3);
        btnExit.paint(g);
        cmdRollx1.paint(g);
        cmdRollx10.paint(g);
        btnBoquaHieuUng.paint(g);
    }
    public static byte count = 0;
    public static long lastSelect = mSystem.currentTimeMillis();
    public static long last = mSystem.currentTimeMillis();
    public static int speed = 30;
    public static List<ItemTamBao> listsort(int color)
    {
        List<ItemTamBao> list = new List<ItemTamBao>();
        foreach (var item in listItem)
        {
            if (item != null && item.color == color)
            {
                list.Add(item);
            }
        }
        return list;
    }
    static List<RandomXY> listrandom = new List<RandomXY>();
    static int maxWidth(List<ItemTamBao> list)
    {
        int result = 0;
        foreach (var item in list)
        {
            if (SmallImage.imgNew[ItemTemplates.get((short)item.id).iconID] != null)
            {
                Image img = SmallImage.imgNew[ItemTemplates.get((short)item.id).iconID].img;
                if (img.getWidth() > result)
                {
                    result = img.getWidth();
                }
            }
            else
            {
                SmallImage.createImage(ItemTemplates.get((short)item.id).iconID);
            }
        }
        return result;
    }
    static int maxHeight(List<ItemTamBao> list)
    {
        int result = 0;
        foreach (var item in list)
        {
            if (SmallImage.imgNew[ItemTemplates.get((short)item.id).iconID] != null)
            {
                Image img = SmallImage.imgNew[ItemTemplates.get((short)item.id).iconID].img;
                if (img.getHeight() > result)
                {
                    result = img.getHeight();
                }
            }
            else
            {
                SmallImage.createImage(ItemTemplates.get((short)item.id).iconID);
            }
        }
        return result;
    }
    public static void paintListItem(mGraphics g, List<ItemTamBao> listItemz)
    {
        int ystart = (int)(h * 0.175);
        listrandom.Clear();
        for (int i = 4; i >= 0; i--)
        {
            int cout = 0;
            int xstart = (w / 2) - (maxWidth(listsort(i)) / 2);
            foreach (var item in listsort(i))
            {
               
                if (SmallImage.imgNew[ItemTemplates.get((short)item.id).iconID] != null)
                {
                    Image img = SmallImage.imgNew[ItemTemplates.get((short)item.id).iconID].img;
                    g.drawImageScale(bg[item.color], xstart , ystart, maxWidth(listsort(i)) + 5, maxHeight(listsort(i))+5, 0);
                    listrandom.Add(new RandomXY(xstart, ystart));
                    int wid = maxWidth(listsort(i)) - img.getWidth();
                    int hei = maxHeight(listsort(i)) - img.getHeight();
                    g.drawImage(img,xstart + wid/2, ystart + hei/2);
                
                   

                    if (item.quantity > 1) mFont.tahoma_7b_white.drawString(g, "x" + item.quantity, xstart + maxWidth(listsort(i))-10, ystart + maxHeight(listsort(i))-5, mFont.LEFT);
                    if (item.color > 1)
                    {
                        g.drawImageScale(effy[count % 4], xstart-7, ystart-5, (int)(maxWidth(listsort(i)) +20),(int)( maxHeight(listsort(i))+17), 0);
                    }
                    cout++;
                    xstart = cout % 2 == 0 ? xstart -= cout * maxWidth(listsort(i)) : xstart += cout * maxWidth(listsort(i));
                    xstart = cout % 2 == 0 ? xstart -= cout * (int)(w*0.02) : xstart += cout *(int)(w * 0.02);
                }
                else
                {
                    SmallImage.createImage(ItemTemplates.get((short)item.id).iconID);
                }
              

            }
            ystart += (int)(h * 0.125);
        }
        if (mSystem.currentTimeMillis() - last > 50)
        {
            count++;
            last = mSystem.currentTimeMillis();
            if (count > 10)
            {
                count = 0;
            }
        }
        if (isQuay)
        {
            g.setColor(UnityEngine.Color.yellow);
            if (mSystem.currentTimeMillis() - lastRandom >= 300L)
            {
                getRandom = new Random().Next(0, listrandom.Count - 1);
                lastRandom = mSystem.currentTimeMillis();
            }
            var x = listrandom[getRandom]; 
            g.fillRect(x.x, x.y, (int)(w * 0.05), (int)(w * 0.05));
        }
    }
    static long lastRandom;
    static int getRandom;
    public class RandomXY
    {
        public int x;
        public int y;
        public RandomXY(int x, int y)
        {
            this.x = x; this.y = y;
        }
        public RandomXY()
        {

        }
    }
}