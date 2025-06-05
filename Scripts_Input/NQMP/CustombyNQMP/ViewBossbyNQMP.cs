using HairMod;
using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class ViewBossbyNQMP
{
    // Constants
    private const int ITEM_WIDTH = 90;
    private const int ITEM_HEIGHT = 30;
    private const int ITEM_SPACING = 3;

    // Static fields
    public static List<BossView> listboss = new List<BossView>();
    public static Scroll scroll = new Scroll();
    public static Command cmdExit = new Command();
    public static bool isShow = false;
    public static bool isInit = false;
    public static int select = 0;
    public static int w = GameCanvas.w;
    public static int h = GameCanvas.h;
    public static int cf = 0;
    public static long currentTimeRes;

    // BossView class
    public class BossView
    {
        public string name;
        public long timeRespawn;
        public int head;
        public int body;
        public int leg;
        public int[] listDrop;
        public int[] mapDrop;
        public long hp;

        public BossView() { }

        public BossView(string name, long timeRespawn, int[] listDrop, int[] mapDrop, long hp, int head, int body, int leg)
        {
            this.name = name;
            this.timeRespawn = timeRespawn;
            this.listDrop = listDrop;
            this.mapDrop = mapDrop;
            this.hp = hp;
            this.head = head;
            this.body = body;
            this.leg = leg;
        }
    }

    // Message handling
    public static void requestMessage(Message msg)
    {
        try
        {
            sbyte type = msg.reader().readByte();
            switch (type)
            {
                case 0:
                    listboss.Clear();
                    break;
                case 1:
                    AddBossFromMessage(msg);
                    break;
                case 2:
                    InitializeScroll();
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.Log("Error in requestMessage: " + e.Message);
        }
    }

    private static void AddBossFromMessage(Message msg)
    {
        BossView boss = new BossView
        {
            name = msg.reader().readUTF(),
            timeRespawn = msg.reader().readLong(),
            listDrop = new int[msg.reader().readInt()],
            mapDrop = new int[msg.reader().readInt()],
            hp = msg.reader().readLong(),
            head = msg.reader().readInt(),
            body = msg.reader().readInt(),
            leg = msg.reader().readInt()
        };

        Debug.LogError($"Boss{boss.name}  - {boss.head} - {boss.body} - {boss.leg}");

        for (int i = 0; i < boss.listDrop.Length; i++)
            boss.listDrop[i] = msg.reader().readInt();

        for (int i = 0; i < boss.mapDrop.Length; i++)
            boss.mapDrop[i] = msg.reader().readInt();

        listboss.Add(boss);
    }

    private static void InitializeScroll()
    {
        scroll.setStyle(listboss.Count, ITEM_WIDTH, (int)(w * 0.05), (int)(h * 0.1),
            ITEM_HEIGHT, (int)(h * 0.8), true, 1);
        isShow = true;
        init();
    }

    // Initialization
    public static void init()
    {
        try
        {
            cmdExit = new Command("", null, 1, null);
            cmdExit.img = GameCanvas.loadImage("/mainImage/myTexture2dbtX.png");
            cmdExit.x = (int)(w * 0.85);
            cmdExit.y = (int)(h * 0.1);
            isInit = true;
            scroll.setStyle(30, 100, (int)(w * 0.05), (int)(h * 0.1),
                ITEM_HEIGHT, (int)(h * 0.8), true, 1);
        }
        catch (Exception e)
        {
            Debug.Log("Error in init: " + e.Message);
        }
    }

    // Update methods
    public static void update()
    {
        if (!
            isShow) return;

        updateKey();
        scroll.updatecm();
        UpdateScrollPosition();
    }

    private static void UpdateScrollPosition()
    {
        if (scroll.cmyLim > 0)
        {
            ScrollResult result = scroll.updateKey();
            if (scroll.cmy < 0 && !result.isDowning)
                scroll.cmy -= scroll.cmy / 2;
            else if (scroll.cmy > scroll.cmyLim && !result.isDowning)
                scroll.cmy -= (scroll.cmy - scroll.cmyLim + 6) / 2;
        }
    }

    public static void updateKey()
    {
        if (!isShow) return;

        if (cmdExit.isPointerPressInside())
        {
            isShow = false;
            GameScr.isHaveSelectSkill = true;
            scroll.cmy = scroll.cmtoY = 0;
            scroll.clear();
            return;
        }

        UpdateItemSelection();
    }

    private static void UpdateItemSelection()
    {
        int xstart = (int)(w * 0.05);
        int yStart = (int)(h * 0.1);

        for (int i = 0; i < listboss.Count; i++)
        {
            var item = listboss[i];
            if (item != null && GameCanvas.isPointerHoldIn(xstart, yStart - scroll.cmtoY, ITEM_WIDTH, ITEM_HEIGHT))
            {
                if (GameCanvas.isPointerClick && !GameCanvas.isPointerMove)
                    select = i;
            }
            yStart += ITEM_HEIGHT + ITEM_SPACING;
        }
    }

    // Painting methods
    public static void paint(mGraphics g)
    {
        if (!isShow) return;
        if (!isInit) init();

        GameScr.isHaveSelectSkill = false;
        paintPopup(g);

        if (!paintList(g)) return;
        cmdExit.paint(g);
    }

    public static void paintPopup(mGraphics g)
    {
        PopUp.paintPopUp(g, (int)(w * 0.05), (int)(h * 0.1),
            (int)(w * 0.8), (int)(h * 0.8), 0, true);
    }

    public static bool paintList(mGraphics g)
    {
        try
        {
            if (listboss.Count == 0)
            {
                mFont.tahoma_7.drawString(g, "Không thể tìm thấy dử liệu Boss , hãy thử lại sau",
                    w / 2, h / 2, 3, mFont.tahoma_7b_blue);
                return false;
            }

            g.setClip((int)(w * 0.05), (int)(h * 0.1), (int)(w * 0.8), (int)(h * 0.8));
            g.translate(scroll.cmx, -scroll.cmy);

            PaintBossList(g);
            g.reset();
            paintCurrentBoss(g, select);
            return true;
        }
        catch (Exception e)
        {
            Debug.Log("Error in paintList: " + e.ToString());
            return false;
        }
    }

    private static void PaintBossList(mGraphics g)
    {
        int xstart = (int)(w * 0.05);
        int yStart = (int)(h * 0.1);

        for (int i = 0; i < listboss.Count; i++)
        {
            var item = listboss[i];
            if (item != null)
            {
                PopUp.paintPopUp(g, xstart, yStart, ITEM_WIDTH, ITEM_HEIGHT,
                    i != select ? 0 : 1, true);
                mFont.tahoma_7.drawString(g, item.name,
                    xstart + ITEM_WIDTH / 2, yStart + 3, 3);
                yStart += ITEM_HEIGHT + ITEM_SPACING;
            }
        }
    }

    public static BossView Boss(int select)
    {
        if (listboss.Count == 0 || listboss.Count <= select)
            return null;
        return listboss[select];
    }

    public static void paintCurrentBoss(mGraphics g, int select)
    {
        var boss = Boss(select);
        if (boss == null) return;

        PaintBossInfo(g, boss);
        PaintBossDrops(g, boss);
        UpdateBossRespawnTime(g,boss);
        PaintBossMaps(g, boss);
    }

    private static void PaintBossInfo(mGraphics g, BossView boss)
    {
        PopUp.paintPopUp(g, (int)(w * 0.23), (int)(h * 0.1),
            (int)(w * 0.62), (int)(h * 0.8), 0, true);
        PopUp.paintPopUp(g, (int)(w * 0.27), (int)(h * 0.15),
            (int)(w * 0.2), (int)(w * 0.175), 0, true);

        PaintBossCharacter(g, boss);
        PaintBossStats(g, boss);
    }

    private static void PaintBossCharacter(mGraphics g, BossView boss)
    {
        Part part = GameScr.parts[boss.head];
        Part part2 = GameScr.parts[boss.leg];
        Part part3 = GameScr.parts[boss.body];

        int cx = (int)(w * 0.35);
        int cy = (int)(h * 0.3262);

        SmallImage.drawSmallImage(g, part.pi[Char.CharInfo[cf][0][0]].id,
            cx + Char.CharInfo[cf][0][1] + part.pi[Char.CharInfo[cf][0][0]].dx,
            cy - Char.CharInfo[cf][0][2] + part.pi[Char.CharInfo[cf][0][0]].dy, 0, 0);

        SmallImage.drawSmallImage(g, part2.pi[Char.CharInfo[cf][1][0]].id,
            cx + Char.CharInfo[cf][1][1] + part2.pi[Char.CharInfo[cf][1][0]].dx,
            cy - Char.CharInfo[cf][1][2] + part2.pi[Char.CharInfo[cf][1][0]].dy, 0, 0);

        SmallImage.drawSmallImage(g, part3.pi[Char.CharInfo[cf][2][0]].id,
            cx + Char.CharInfo[cf][2][1] + part3.pi[Char.CharInfo[cf][2][0]].dx,
            cy - Char.CharInfo[cf][2][2] + part3.pi[Char.CharInfo[cf][2][0]].dy, 0, 0);
    }

    private static void PaintBossStats(mGraphics g, BossView boss)
    {
        mFont.tahoma_7.drawString(g, boss.name, (int)(w * 0.375), (int)(h * 0.45), mFont.CENTER);
        mFont.bigNumber_red.drawString(g, "HP : " + NinjaUtil.getMoneys(boss.hp),
            (int)(w * 0.375), (int)(h * 0.485), mFont.CENTER);
    }

    private static void PaintBossDrops(mGraphics g, BossView boss)
    {
        mFont.tahoma_7.drawString(g, "Vật Phẩm Rơi", (int)(w * 0.65), (int)(h * 0.15), mFont.CENTER);

        int xs = (int)(w * 0.55);
        int ys = (int)(h * 0.2);
        int status = 0;

        for (int i = 0; i < boss.listDrop.Length; i++)
        {
            ItemTemplate template = ItemTemplates.get(boss.listDrop[i]);
            if (template != null)
            {
                int iconid = template.iconID;
                if (SmallImage.imgNew[iconid] == null)
                {
                    SmallImage.createImage(iconid);
                    return;
                }

                PopUp.paintPopUp(g, xs, ys, 30, 30, 0, true);
                SmallImage.drawSmallImage(g, iconid, (xs + xs + 30) / 2, ((ys * 2) + 30) / 2, 0, 3);

                xs += 35;
                status++;

                if (status % 4 == 0)
                {
                    xs = (int)(w * 0.55);
                    ys += 35;
                }
            }
        }
    }

    private static void UpdateBossRespawnTime(mGraphics g, BossView boss)
    {
        if (mSystem.currentTimeMillis() - currentTimeRes >= 1000L)
        {
            boss.timeRespawn--;
            currentTimeRes = mSystem.currentTimeMillis();
        }

        string datarespawn = boss.timeRespawn <= 0 ?
            "Đã xuất hiện tại 1 trong các map , đang chờ tiêu diệt" :
            "Sẽ xuất hiện sau khoảng: " + boss.timeRespawn + " giây nữa";

        mFont.tahoma_7b_red.drawString(g, datarespawn, (int)(w * 0.5), (int)(h * 0.55), mFont.CENTER);
    }

    private static void PaintBossMaps(mGraphics g, BossView boss)
    {
        PopUp.paintPopUp(g, (int)(w * 0.23), (int)(h * 0.6),
            (int)(w * 0.62), (int)(h * 0.3), 0, true);

        mFont.tahoma_7b_white.drawString(g, "Các map mà Boss có thể xuất hiện:",
            (int)(w * 0.5), (int)(h * 0.625), mFont.CENTER, mFont.tahoma_7);

        string datamap = string.Empty;
        foreach (int map in boss.mapDrop)
        {
            try
            {
                datamap += TileMap.mapNames[map] + "[" + map + "] , ";
            }
            catch
            {
                Debug.Log("MapError :" + map);
            }
        }

        if (!string.IsNullOrEmpty(datamap))
        {
            datamap = datamap.Substring(0, datamap.Length - 3);
            string[] dataPaint = mFont.tahoma_7.splitFontArray(datamap, (int)(w * 0.6));
            int ypaint = (int)(h * 0.66);

            foreach (string datapaint in dataPaint)
            {
                mFont.tahoma_7.drawString(g, datapaint, (int)(w * 0.25), ypaint, mFont.LEFT);
                ypaint += 10;
            }
        }
    }

    public static int x(double value)
    {
        return GameCanvas.getX(value);
    }
    public static int y(double value)
    {
        return GameCanvas.getY(value);
    }

    public static Image ResizeTexture(Image image, int targetWidth, int targetHeight)
    {
        // Load the source texture from bytes
        Texture2D sourceTexture = image.texture;

        // Resize the texture
        Texture2D resizedTexture = ScaleTexture(sourceTexture, targetWidth, targetHeight);

        // Convert the resized texture to bytes
        Image img = Image.createImage(resizedTexture.EncodeToPNG());
        return img;
    }

    private static Texture2D ScaleTexture(Texture2D sourceTexture, int targetWidth, int targetHeight)
    {
        RenderTexture current = RenderTexture.active;

        RenderTexture rt = new RenderTexture(targetWidth, targetHeight, 32);
        rt.wrapMode = TextureWrapMode.Clamp;
        rt.filterMode = FilterMode.Point;
        rt.useMipMap = false;
        RenderTexture.active = rt;

        UnityEngine.Graphics.Blit(sourceTexture, rt);

        Texture2D result = new Texture2D(targetWidth, targetHeight);
        result.ReadPixels(new Rect(0, 0, targetWidth, targetHeight), 0, 0);
        result.Apply();

        RenderTexture.active = current;
        rt = null;
        GC.Collect();
        Resources.UnloadUnusedAssets(); 

        return result;
    }
}