using System;

public class CreateCharScr : mScreen, IActionListener
{
    public static CreateCharScr instance;

    private PopUp p;

    public static bool isCreateChar = false;

    public static TField tAddName;

    public static int indexGender;

    public static int indexHair;

    public static int selected;

    public static int[][] hairID = new int[3][]
    {
        new int[3] { 127, 127, 127 },
        new int[3] { 128, 128, 128 },
        new int[3] { 126, 126, 126 }
    };

    public static int[] defaultLeg = new int[3] { 1106, 1116, 1111 };

    public static int[] defaultBody = new int[3] { 1105, 1115, 1110 };

    private int yButton;

    private int disY;

    private int[] bgID = new int[3] { 0, 4, 8 };

    public int yBegin;

    private int curIndex;

    private int cx = 168;

    private int cy = 350;

    private int dy = 45;

    private int cp1;

    private int cf;

    public CreateCharScr()
    {
        try
        {
            if (!GameCanvas.lowGraphic)
            {
                loadMapFromResource(new sbyte[3] { 39, 40, 41 });
            }
            loadMapTableFromResource(new sbyte[3] { 39, 40, 41 });
        }
        catch (Exception ex)
        {
            Cout.LogError("Tao char loi " + ex.ToString());
        }
        if (GameCanvas.w <= 200)
        {
            GameScr.setPopupSize(128, 100);
            GameScr.popupX = (GameCanvas.w - 128) / 2;
            GameScr.popupY = 10;
            cy += 15;
            dy -= 15;
        }
        indexGender = 1;
        tAddName = new TField();
        tAddName.width = ((GameCanvas.loginScr.tfUser.width /2) + 25);
        if (GameCanvas.w < 200)
        {
            tAddName.width = 60;
        }
        tAddName.height = mScreen.ITEM_HEIGHT + 2;
        if (GameCanvas.w < 200)
        {
            tAddName.x = GameScr.popupX + 45;
            tAddName.y = GameCanvas.h - tAddName.y - 80;
        }
        else
        {
            tAddName.x = (GameCanvas.w / 2 - tAddName.width / 2) - 30;
            tAddName.y = GameCanvas.h - tAddName.y - 30;
        }
        if (!GameCanvas.isTouch)
        {
            tAddName.isFocus = true;
        }
        tAddName.setIputType(TField.INPUT_TYPE_ANY);
        tAddName.showSubTextField = false;
        tAddName.strInfo = mResources.char_name;
        if (tAddName.getText().Equals("@"))
        {
            tAddName.setText(GameCanvas.loginScr.tfUser.getText().Substring(0, GameCanvas.loginScr.tfUser.getText().IndexOf("@")));
        }
        tAddName.name =mResources.char_name;
        try
        { tAddName.setTFImg(); }
        catch (Exception e){ UnityEngine.Debug.LogError(e.Message); }
        indexGender = 1;
        indexHair = 0;
        center = new Command("OK", this, 8000, null);
        center.x = tAddName.x + tAddName.imgTf2.getWidth() + 5;
        center.y = tAddName.y;
        left = new Command(mResources.BACK, this, 8001, null);
        if (!GameCanvas.isTouch)
        {
            right = tAddName.cmdClear;
        }
        yBegin = tAddName.y;
    }

    public static CreateCharScr gI()
    {
        if (instance == null)
        {
            instance = new CreateCharScr();
        }
        return instance;
    }

    public static void init()
    {
    }

    public static void loadMapFromResource(sbyte[] mapID)
    {
        Res.outz("newwwwwwwwww =============");
        DataInputStream dataInputStream = null;
        for (int i = 0; i < mapID.Length; i++)
        {
            dataInputStream = MyStream.readFile("/mymap/" + mapID[i]);
            MapTemplate.tmw[i] = (ushort)dataInputStream.read();
            MapTemplate.tmh[i] = (ushort)dataInputStream.read();
            Cout.LogError("Thong TIn : " + MapTemplate.tmw[i] + "::" + MapTemplate.tmh[i]);
            MapTemplate.maps[i] = new int[dataInputStream.available()];
            Cout.LogError("lent= " + MapTemplate.maps[i].Length);
            for (int j = 0; j < MapTemplate.tmw[i] * MapTemplate.tmh[i]; j++)
            {
                MapTemplate.maps[i][j] = dataInputStream.read();
            }
            MapTemplate.types[i] = new int[MapTemplate.maps[i].Length];
        }
    }

    public void loadMapTableFromResource(sbyte[] mapID)
    {
        if (GameCanvas.lowGraphic)
        {
            return;
        }
        DataInputStream dataInputStream = null;
        try
        {
            for (int i = 0; i < mapID.Length; i++)
            {
                dataInputStream = MyStream.readFile("/mymap/mapTable" + mapID[i]);
                Cout.LogError("mapTable : " + mapID[i]);
                short num = dataInputStream.readShort();
                MapTemplate.vCurrItem[i] = new MyVector();
                Res.outz("nItem= " + num);
                for (int j = 0; j < num; j++)
                {
                    short id = dataInputStream.readShort();
                    short num2 = dataInputStream.readShort();
                    short num3 = dataInputStream.readShort();
                    if (TileMap.getBIById(id) != null)
                    {
                        BgItem bIById = TileMap.getBIById(id);
                        BgItem bgItem = new BgItem();
                        bgItem.id = id;
                        bgItem.idImage = bIById.idImage;
                        bgItem.dx = bIById.dx;
                        bgItem.dy = bIById.dy;
                        bgItem.x = num2 * TileMap.size;
                        bgItem.y = num3 * TileMap.size;
                        bgItem.layer = bIById.layer;
                        MapTemplate.vCurrItem[i].addElement(bgItem);
                        if (!BgItem.imgNew.containsKey(bgItem.idImage + string.Empty))
                        {
                            try
                            {
                                Image image = GameCanvas.loadImage("/mapBackGround/" + bgItem.idImage + ".png");
                                if (image == null)
                                {
                                    BgItem.imgNew.put(bgItem.idImage + string.Empty, Image.createRGBImage(new int[1], 1, 1, true));
                                    Service.gI().getBgTemplate(bgItem.idImage);
                                }
                                else
                                {
                                    BgItem.imgNew.put(bgItem.idImage + string.Empty, image);
                                }
                            }
                            catch (Exception)
                            {
                                Image image2 = GameCanvas.loadImage("/mapBackGround/" + bgItem.idImage + ".png");
                                if (image2 == null)
                                {
                                    image2 = Image.createRGBImage(new int[1], 1, 1, true);
                                    Service.gI().getBgTemplate(bgItem.idImage);
                                }
                                BgItem.imgNew.put(bgItem.idImage + string.Empty, image2);
                            }
                            BgItem.vKeysLast.addElement(bgItem.idImage + string.Empty);
                        }
                        if (!BgItem.isExistKeyNews(bgItem.idImage + string.Empty))
                        {
                            BgItem.vKeysNew.addElement(bgItem.idImage + string.Empty);
                        }
                        bgItem.changeColor();
                    }
                    else
                    {
                        Res.outz("item null");
                    }
                }
            }
        }
        catch (Exception ex2)
        {
            Cout.println("LOI TAI loadMapTableFromResource" + ex2.ToString());
        }
    }

    public override void switchToMe()
    {
        LoginScr.isContinueToLogin = false;
        GameCanvas.menu.showMenu = false;
        GameCanvas.endDlg();
        base.switchToMe();
        indexGender = Res.random(0, 3);
        indexHair = Res.random(0, 3);
        doChangeMap();
        Char.isLoadingMap = false;
        tAddName.setFocusWithKb(true);
    }

    public void doChangeMap()
    {
        TileMap.maps = new int[MapTemplate.maps[indexGender].Length];
        for (int i = 0; i < MapTemplate.maps[indexGender].Length; i++)
        {
            TileMap.maps[i] = MapTemplate.maps[indexGender][i];
        }
        TileMap.types = MapTemplate.types[indexGender];
        TileMap.pxh = MapTemplate.pxh[indexGender];
        TileMap.pxw = MapTemplate.pxw[indexGender];
        TileMap.tileID = MapTemplate.pxw[indexGender];
        TileMap.tmw = MapTemplate.tmw[indexGender];
        TileMap.tmh = MapTemplate.tmh[indexGender];
        TileMap.tileID = bgID[indexGender] + 1;
        TileMap.loadMainTile();
        TileMap.loadTileCreatChar();
        GameCanvas.loadBG(bgID[indexGender]);
        GameScr.loadCamera(false, cx, cy);
    }

    public override void keyPress(int keyCode)
    {
        tAddName.keyPressed(keyCode);
    }

    public override void update()
    {
        cp1++;
        if (cp1 > 30)
        {
            cp1 = 0;
        }
        if (cp1 % 15 < 5)
        {
            cf = 0;
        }
        else
        {
            cf = 1;
        }
        tAddName.update();
        if (selected != 0)
        {
            tAddName.isFocus = false;
        }
    }
    string txtAdd = string.Empty;
    public override void updateKey()
    {
        if (GameCanvas.keyPressed[(!Main.isPC) ? 2 : 21])
        {
            selected--;
            if (selected < 0)
            {
                selected = mResources.MENUNEWCHAR.Length - 1;
            }
        }
        if (GameCanvas.keyPressed[(!Main.isPC) ? 8 : 22])
        {
            selected++;
            if (selected >= mResources.MENUNEWCHAR.Length)
            {
                selected = 0;
            }
        }
        if (selected == 0)
        {
            if (!GameCanvas.isTouch)
            {
                right = tAddName.cmdClear;
            }
            tAddName.update();
        }

        bool flag7 = CreateCharScr.selected == 1;
        if (flag7)
        {
            bool flag8 = GameCanvas.keyPressed[(!Main.isPC) ? 4 : 23];
            if (flag8)
            {
                CreateCharScr.indexGender--;
                bool flag9 = CreateCharScr.indexGender < 0;
                if (flag9)
                {
                    CreateCharScr.indexGender = mResources.MENUGENDER.Length - 1;
                }
            }
            bool flag10 = GameCanvas.keyPressed[(!Main.isPC) ? 6 : 24];
            if (flag10)
            {
                CreateCharScr.indexGender++;
                bool flag11 = CreateCharScr.indexGender > mResources.MENUGENDER.Length - 1;
                if (flag11)
                {
                    CreateCharScr.indexGender = 0;
                }
            }
            this.right = null;
        }

        bool isPointerJustRelease = GameCanvas.isPointerJustRelease;
        if (isPointerJustRelease)
        {
            int num = 110;
            int num2 = 60;
            int num3 = 78;
            bool flag17 = GameCanvas.w > GameCanvas.h;
            if (flag17)
            {
                num = 100;
                num2 = 40;
            }
            bool flag18 = GameCanvas.isPointerHoldIn(tAddName.x,tAddName.y, tAddName.width, tAddName.height);
            if (flag18)
            {
                CreateCharScr.selected = 0;
                CreateCharScr.tAddName.isFocus = true;
            }
            bool flag19 = GameCanvas.isPointerHoldIn(GameCanvas.w / 2 - 3 * num3 / 2, num - 30, num3 * 3, num2 + 5);
            if (flag19)
            {
                CreateCharScr.selected = 1;
                int num4 = CreateCharScr.indexGender;
                CreateCharScr.indexGender = (GameCanvas.px - (GameCanvas.w / 2 - 3 * num3 / 2)) / num3;
                bool flag20 = CreateCharScr.indexGender < 0;
                if (flag20)
                {
                    CreateCharScr.indexGender = 0;
                }
                bool flag21 = CreateCharScr.indexGender > mResources.MENUGENDER.Length - 1;
                if (flag21)
                {
                    CreateCharScr.indexGender = mResources.MENUGENDER.Length - 1;
                }
            }

        }


        if (!TouchScreenKeyboard.visible)
        {
            base.updateKey();
        }
        GameCanvas.clearKeyHold();
        GameCanvas.clearKeyPressed();
    }
    static Image bg;
    static Image soil, btn1, btn2, img1, img2;
    static int imgmove = 0;
    public override void paint(mGraphics g)
    {
        if (Char.isLoadingMap)
        {
            return;
        }
        int num444 = 78;
       
        if (bg == null)
        {
            bg = GameCanvas.loadImage("/bgr/background.png");
            return;
        }
        g.drawImage(bg, 0, 0);
        if (img1 == null)
        {
            img1 = GameCanvas.loadImage("/eff/imgMay.png");
            return;
        }
        if (img2 == null)
        {
            img2 = GameCanvas.loadImage("/eff/imgMay1.png");
            return;
        }
        g.drawImage(img2, GameCanvas.w - imgmove, GameCanvas.h - 270);
        g.drawImage(img2, GameCanvas.w - imgmove, GameCanvas.h - 120);
        imgmove++;
        if (GameCanvas.w - imgmove < -img2.getWidth())
        {
            imgmove = 0;
        }
        g.translate(-GameScr.cmx, -GameScr.cmy);
        if (!GameCanvas.lowGraphic)
        {
            for (int i = 0; i < MapTemplate.vCurrItem[indexGender].size(); i++)
            {
                BgItem bgItem = (BgItem)MapTemplate.vCurrItem[indexGender].elementAt(i);
                if (bgItem.idImage != -1 && bgItem.layer == 1)
                {
                    bgItem.paint(g);
                }
            }
        }
        int num = 30;
        if (GameCanvas.w == 128)
        {
            num = 20;
        }
        int num2 = hairID[indexGender][indexHair];
        int num3 = defaultLeg[indexGender];
        int num4 = defaultBody[indexGender];
        if (soil == null)
        {
            soil = GameCanvas.loadImage("/bgr/img_soil.png");
        }
        g.drawImage(soil, 250, GameCanvas.h - 16);
        g.drawImage(TileMap.bong, cx + 150, cy + dy - 100, 3);
        Part part = GameScr.parts[num2];
        Part part2 = GameScr.parts[num3];
        Part part3 = GameScr.parts[num4];
        SmallImage.drawSmallImage(g, part.pi[Char.CharInfo[cf][0][0]].id, cx + Char.CharInfo[cf][0][1] + part.pi[Char.CharInfo[cf][0][0]].dx + 150, cy - Char.CharInfo[cf][0][2] + part.pi[Char.CharInfo[cf][0][0]].dy + dy - 100, 0, 0);
        SmallImage.drawSmallImage(g, part2.pi[Char.CharInfo[cf][1][0]].id, cx + Char.CharInfo[cf][1][1] + part2.pi[Char.CharInfo[cf][1][0]].dx + 150, cy - Char.CharInfo[cf][1][2] + part2.pi[Char.CharInfo[cf][1][0]].dy + dy - 100, 0, 0);
        SmallImage.drawSmallImage(g, part3.pi[Char.CharInfo[cf][2][0]].id, cx + Char.CharInfo[cf][2][1] + part3.pi[Char.CharInfo[cf][2][0]].dx + 150, cy - Char.CharInfo[cf][2][2] + part3.pi[Char.CharInfo[cf][2][0]].dy + dy - 100, 0, 0);

        if (!GameCanvas.lowGraphic)
        {
            for (int j = 0; j < MapTemplate.vCurrItem[indexGender].size(); j++)
            {
                BgItem bgItem2 = (BgItem)MapTemplate.vCurrItem[indexGender].elementAt(j);
                if (bgItem2.idImage != -1 && bgItem2.layer == 3)
                {
                    bgItem2.paint(g);
                }
            }
        }
        g.translate(-g.getTranslateX(), -g.getTranslateY());
        if (GameCanvas.w > 200)
        {
            for (int l = 0; l < 3; l++)
            {
                int num5 = 78;
                bool flag13 = l != CreateCharScr.indexGender;
                if (btn1 == null)
                {
                    btn1 = GameCanvas.loadImage("/button/btn1.png");
                    return;
                }
                if (btn2 == null)
                {
                    btn2 = GameCanvas.loadImage("/button/btn2.png");
                    return;
                }


                g.drawImage(flag13 ? btn1 : btn2, GameCanvas.w / 2 - num5 + l * num5, this.yButton + 80, 3);

                if (!flag13)
                {
                    bool flag14 = CreateCharScr.selected == 1;
                    if (flag14)
                    {
                        g.drawRegion(GameScr.arrow, 0, 0, 13, 16, 4, GameCanvas.w / 2 - num5 + l * num5, this.yButton - 20 + ((GameCanvas.gameTick % 7 > 3) ? 1 : 0), StaticObj.VCENTER_HCENTER);
                    }
                }
                mFont.tahoma_7_white.drawString(g, mResources.MENUGENDER[l], GameCanvas.w / 2 - num5 + l * num5, this.yButton +75, mFont.CENTER);

            }
            try

            { tAddName.paint(g); }
            catch { }
        }
        
        g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
        mFont.tahoma_7b_red .drawString(g, "Bạn có thể nhập tên nhân vật từ 5 tới 20 ký tự , có thể đặt tên có dấu theo Telex", GameCanvas.w/2, tAddName.y - 30, mFont.CENTER) ;
        mFont.tahoma_7.drawString(g, "Bạn có thể coppy tên ở ngoài và dàn vào với tổ hợp phím Ctrl+V", GameCanvas.w / 2, tAddName.y - 20, mFont.CENTER);

        mFont.tahoma_7b_white.drawString(g, mResources.server + " " + LoginScr.serverName, 5, 5, 0, mFont.tahoma_7b_dark);
        if (!TouchScreenKeyboard.visible)
        {
            base.paint(g);
        }
    }

    public void perform(int idAction, object p)
    {
        switch (idAction)
        {
            case 8000:
                if (tAddName.getText().Equals(string.Empty))
                {
                    GameCanvas.startOKDlg(mResources.char_name_blank);
                    break;
                }
                if (tAddName.getText().Length < 5)
                {
                    GameCanvas.startOKDlg(mResources.char_name_short);
                    break;
                }
                if (tAddName.getText().Length > 20)
                {
                    GameCanvas.startOKDlg(mResources.char_name_long);
                    break;
                }
                InfoDlg.showWait();
                Service.gI().createChar(tAddName.getText(), indexGender, hairID[indexGender][indexHair]);
                break;
            case 8001:
                if (GameCanvas.loginScr.isLogin2)
                {
                    GameCanvas.startYesNoDlg(mResources.note, new Command(mResources.YES, this, 10019, null), new Command(mResources.NO, this, 10020, null));
                    break;
                }
                if (Main.isWindowsPhone)
                {
                    GameMidlet.isBackWindowsPhone = true;
                }
                Session_ME.gI().close();
                GameCanvas.serverScreen.switchToMe();
                break;
            case 10020:
                GameCanvas.endDlg();
                break;
            case 10019:
                Session_ME.gI().close();
                GameCanvas.endDlg();
                GameCanvas.serverScreen.switchToMe();
                break;
        }
    }
}
