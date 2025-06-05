using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class newShop : IChatable
{

    public static string[] TabName;
    public static Command cmdBuyorSaleItem;
    public static Command cmdBuys;
    public static Command cmdExit;
    public static Command cmdBuyKyGui;
    public static Command cmdKyGuiVang;
    public static Command cmdHuyKyGui;
    public static Command cmdUpTop;
    public static Command cmdNhanTien;
    public static Command cmdPrePage;
    public static Command cmdNextPage;
    public static List<Item> itemkygui = new List<Item>();
    public static List<Item> itemkygui2 = new List<Item>();
    public static ChatTextField chatTField = new ChatTextField();
    public static Scroll scroll = new Scroll();
    public static newShop _Instance;


    public static newShop getInstance()
    {
        bool flag = newShop._Instance == null;
        if (flag)
        {
            newShop._Instance = new newShop();
        }
        return newShop._Instance;
    }
    public static Item[] itemkyguiTab(int tab)
    {
        List<Item> list = new List<Item>();
        foreach (Item item in itemkygui)
        {
            if (item.tab == tab)
            {
                list.Add(item);
            }
        }
        Item[] items = new Item[list.Count];
        for (int i = 0; i < items.Length; i++)
        {
            items[i] = list[i];
        }
        return items;
    }
    public static void init()
    {
        try
        {
            cmdBuyorSaleItem = new Command("Mua Vật Phẩm", null, 1, null);
            cmdBuyorSaleItem.x = (int)(GameCanvas.w * 0.5);
            cmdBuyorSaleItem.y = (int)(GameCanvas.h * 0.733);
            cmdBuys = new Command("Mua Nhiều", null, 1, null);
            cmdBuys.x = (int)(GameCanvas.w * 0.75);
            cmdBuys.y = (int)(GameCanvas.h * 0.733);
            cmdBuyKyGui = new Command("Mua Vật Phẩm", cmdBuyorSaleItem.x, cmdBuyorSaleItem.y);
            cmdKyGuiVang = new Command("Ký Gửi Thỏi Vàng", (int)(cmdBuyorSaleItem.x * 0.8), cmdBuyorSaleItem.y);
            cmdHuyKyGui = new Command("Hủy Ký Gửi", (int)(cmdBuyorSaleItem.x * 0.8), cmdBuyorSaleItem.y);
            cmdUpTop = new Command("Up Top", (int)(cmdBuyorSaleItem.x * 1.2), cmdBuyorSaleItem.y);
            cmdNhanTien = new Command("Nhận Tiền", cmdBuyorSaleItem.x, cmdBuyorSaleItem.y);
            cmdPrePage = new Command("Trước", (int)((GameCanvas.w) * 0.55), (int)(cmdBuyorSaleItem.y * 0.7));
            cmdNextPage = new Command("Sau", (int)((GameCanvas.w) * 0.75), (int)(cmdBuyorSaleItem.y * 0.7));
            cmdExit = new Command("", null, 1, null);
            cmdExit.img = imgExit;
            cmdExit.x = (int)(GameCanvas.w * 0.85);
            cmdExit.y = (int)(GameCanvas.h * 0.1);
            setScroll();
        }
        catch (Exception e) { Debug.Log(e.ToString()); }
    }
    static void setScroll()
    {
        if (currentTab < Char.myCharz().arrItemShop.Length)
        {
            scroll.setStyle(Char.myCharz().arrItemShop[currentTab].Length, 36, (int)(GameCanvas.w * 0.1), (int)(GameCanvas.h * 0.25), (int)(GameCanvas.w), (int)(GameCanvas.h * 0.3), true, 4);
        }
        else
        {
            scroll.setStyle(Char.myCharz().arrItemBag.Length, 36, (int)(GameCanvas.w * 0.1), (int)(GameCanvas.h * 0.25), (int)(GameCanvas.w), (int)(GameCanvas.h * 0.3), true, 4);

        }
    }
    public static Image imgExit = GameCanvas.loadImage("/mainImage/myTexture2dbtX.png");
    public static long LastBuy = 0L;
    public static void update()
    {
        if (newShop.isKyGui != 0 && mSystem.currentTimeMillis() - newShop.lastChatQuantity >= 250L && newShop.itemsell != null)
        {
            if (newShop.isKyGui == 1)
            {
                StartKGV(newShop.itemsell);
            }
            else
            {
                StartKGN(newShop.itemsell);
            }
            newShop.isKyGui = 0;
        }
        scroll.updatecm();
        if (currentTab == TabName.Length && cmdBuyorSaleItem.caption != "Bán Vật Phẩm")
        {
            cmdBuyorSaleItem.caption = "Bán Vật Phẩm";
        }
        if (currentTab < TabName.Length && cmdBuyorSaleItem.caption != "Mua Vật Phẩm")
        {
            cmdBuyorSaleItem.caption = "Mua Vật Phẩm";
        }
        if (itembuy != null && isbuys && mSystem.currentTimeMillis() - LastBuy >= 10L)
        {
                  if (maxbuy == 0)
            {
                itembuy = null;
                isbuys = false;
                maxbuy = 0;
                return;
            }
            LastBuy = mSystem.currentTimeMillis();
            if (CheckCanBuy(itembuy))
            {
                Service.gI().buyItem(3, itembuy.template.id, maxbuy);
                maxbuy = 0;
            }
            else
            {
                itembuy = null;
                isbuys = false;
                maxbuy = 0;
            }
        }
    }
    public static bool CheckCanBuy(Item item)
    {
        if (GameScr.gI().isBagFull())
        {
            Debug.Log("12347");
            return false;
        }
        if (item.buyCoin > 0 && Char.myCharz().xu < item.buyCoin)
        {
            Debug.Log("12348");
            return false;
        }
        if (item.buyGold > 0 && Char.myCharz().luong < item.buyCoin)
        {
            Debug.Log("12349");
            return false;
        }
        if (item.buySpec > 0 && findIconIteminBag(item.iconSpec) < item.buySpec)
        {
            Debug.Log("123410");
            return false;
        }
        return true;
    }
    static int findIconIteminBag(int idicon)
    {
        if (idicon == 861)
        {
            return Char.myCharz().luongKhoa;
        }
        foreach (Item item in Char.myCharz().arrItemBag)
        {
            if (item != null && item.template.iconID == idicon)
            {
                return item.quantity;
            }
        }
        return 0;
    }
    public static void updateKey()
    {
        if (scroll.cmyLim > 0)
        {
            ScrollResult result = scroll.updateKey();
            if (scroll.cmy < 0 && result.isDowning == false)
            {
                scroll.cmy -= scroll.cmy / 2;
            }
            else if (scroll.cmy > scroll.cmyLim && result.isDowning == false)
            {
                scroll.cmy -= (scroll.cmy - scroll.cmyLim + 6) / 2;
            }
        }
        updateKeyTouch();
        if (GameCanvas.newKeyAscii != -99)
        {
            int w = GameCanvas.w, h = GameCanvas.h;
            int itemWidth = 90;
            int itemsPerRow = (int)(w * 0.73) / (itemWidth + itemSpacing);
            Item[] itemshow = new Item[10];
            if (!isTypeKiGui())
            {
                itemWidth = 90;
                itemshow = currentTab == TabName.Length ? Char.myCharz().arrItemBag : Char.myCharz().arrItemShop[currentTab];
            }
            else
            {
                itemshow = new Item[itemTab((byte)currentTab).Count];
            }
            if (GameCanvas.newKeyAscii == -4)
            {
                if (selectItem + 1 < itemshow.Length)
                {
                    selectItem += 1;
                }

            }
            else if (GameCanvas.newKeyAscii == -3)
            {
                if (selectItem > 0)
                {
                    selectItem -= 1;
                }

            }
            else if (GameCanvas.newKeyAscii == -1)
            {
                if (selectItem >= itemsPerRow)
                {
                    selectItem -= itemsPerRow;
                }
            }
            else if (GameCanvas.newKeyAscii == -2)
            {
                if (selectItem + itemsPerRow < itemshow.Length)
                {
                    selectItem += itemsPerRow;
                }
            }
            GameCanvas.newKeyAscii = -99;
        }

    }
    public static int pointX;
    public static int pointY;
    public static void hide()
    {
        currentTab = 0;
        selectItem = 0;
        scroll.cmy = scroll.cmtoY = 0;
        scroll.clear();
        if (GameCanvas.panel.timeShow > 0)
        {
            GameCanvas.panel.isClose = false;
            return;
        }
        if (GameCanvas.panel.isTypeShop())
        {
            Char.myCharz().resetPartTemp();
        }
        GameScr.isPaint = true;
        TileMap.lastPlanetId = -1;
        if (Panel.imgMap != null)
        {
            Panel.imgMap.texture = null;
            Panel.imgMap = null;
        }
        mSystem.gcc();
        GameCanvas.panel.isClanOption = false;
        GameCanvas.panel.isClose = true;
        Hint.clickNpc();
        GameCanvas.panel2 = null;
        GameCanvas.clearAllPointerEvent();
        GameCanvas.clearKeyPressed();
        GameCanvas.isFocusPanel2 = false;
        GameCanvas.panel.pointerDownTime = (GameCanvas.panel.pointerDownFirstX = 0);
        GameCanvas.panel.pointerIsDowning = false;
        if ((Char.myCharz().cHP <= 0 || Char.myCharz().statusMe == 14 || Char.myCharz().statusMe == 5) && Char.myCharz().meDead)
        {
            Command center = new Command(mResources.DIES[0], 11038, GameScr.gI());
            GameScr.gI().center = center;
            Char.myCharz().cHP = 0;
        }
    }
    private static void updateTouchCMD()
    {
        if (!isTypeKiGui())
        {
            if (cmdBuyorSaleItem.isPointerPressInside())
            {
                GameCanvas.clearAllPointerEvent();
                if (currentTab < TabName.Length)
                {
                    Item item13 = Char.myCharz().arrItemShop[currentTab][selectItem];
                    Service.gI().buyItem(3, item13.template.id, 0);
                }
                else
                {

                    Service.gI().saleItem(0, 1, (short)selectItem);
                }
                return;
            }
        }
        else
        {

        }
        if (cmdExit.isPointerPressInside())
        {
            hide();
            GameCanvas.clearAllPointerEvent();
            return;
        }

    }
    public static void updateKeyTouch()
    {
        updateTouchCMD();
        for (int i = 0; i <= TabName.Length; i++)
        {
            if (GameCanvas.isPointerHoldIn((int)(GameCanvas.w * 0.10) + (65 * (i)), (int)(GameCanvas.h * 0.1), 60, 18) && GameCanvas.isPointerClick && !GameCanvas.isPointerMove)
            {
                int curtab = (pointX - (int)(GameCanvas.w * 0.10)) / 65;
                if (curtab != currentTab)
                {
                    currentTab = curtab;
                    selectItem = 0;
                    scroll.cmy = scroll.cmtoY = 0;
                    setScroll();
                    return;
                }
            }
        }
        if (GameCanvas.isPointerHoldIn((int)(GameCanvas.w * 0.1), (int)(GameCanvas.h * 0.2), (int)(GameCanvas.w * 0.8), (int)(GameCanvas.h * 0.35)))
        {
            if (GameCanvas.isPointerClick && !GameCanvas.isPointerMove)
            {
                int w = GameCanvas.w, h = GameCanvas.h;
                int xStart = (int)(w * 0.12), yStart = (int)(h * 0.2);
                int itemWidth = 90, itemHeight = 35;
                int itemsPerRow = (int)(w * 0.73) / (itemWidth + itemSpacing);
                int col = (GameCanvas.px - xStart) / (itemWidth + itemSpacing);
                int row = ((scroll.cmtoY + GameCanvas.py - yStart) / (itemHeight + itemSpacing));
                selectItem = row * itemsPerRow + col;
                int lenght = isTypeKiGui() ? itemTab((byte)currentTab).Count : Char.myCharz().arrItemShop[currentTab].Length;
                if (selectItem >= lenght)
                {
                    selectItem = lenght - 1;
                }
                else if (selectItem < 0)
                {
                    selectItem = 0;
                }
            }
        }
    }
    public static int currentTab;
    public static int selectItem;
    public static string[] getstrOption(Item item)
    {
        try
        {
            string[] result = new string[item.itemOption.Length];
            for (int i = 0; i < item.itemOption.Length; i++)
            {
                result[i] = item.itemOption[i].getOptionString();
            }
            return result;
        }
        catch
        {
            return new string[1] { string.Empty };
        }
    }
    private static string getStringItemKyGui(int type)
    {
        return type == 0 ? "Chưa ký gửi" : type == 1 ? "Đang Ký Gửi" : type == 2 ? "Đã bán " : "Quá Hạn Ký Gửi";
    }
    public static Item itemsell;
    public static Item itembuy;
    private static void BuysItem(Item item)
    {
        if (item == null)
        {
            return;
        }
        itembuy = item;
        ChatTextField.gI().strChat = "Nhập Số Lượng Muốn Mua";
        ChatTextField.gI().tfChat.name = "Số Lượng Muốn Mua";
        GameCanvas.panel.isShow = false;
        ChatTextField.gI().startChat2(new newShop(), string.Empty);
    }
    private static void StartKGN(Item item)
    {
        if (item == null)
        {
            return;
        }
        if (item.quantity == 1 || item.quantilyToBuy != 0)
        {
            ChatTextField.gI().strChat = "Ký Gửi Hồng Ngọc";
            ChatTextField.gI().tfChat.name = "Giá tiền muốn bán";
        }
        else
        {
            ChatTextField.gI().strChat = "Nhập Số Lượng ";
            ChatTextField.gI().tfChat.name = "Số Lượng";
        }
        if (item != null)
        {
            itemsell = item;
        }
        GameCanvas.panel.isShow = false;
        ChatTextField.gI().startChat2(new newShop(), string.Empty);
    }
    private static void StartKGV(Item item)
    {
        if (item == null)
        {
            return;
        }
        if (item.quantity == 1 || item.quantilyToBuy != 0)
        {
            ChatTextField.gI().strChat = "Ký Gửi Thỏi Vàng";
            ChatTextField.gI().tfChat.name = "Giá tiền muốn bán";
        }
        else
        {
            ChatTextField.gI().strChat = "Nhập Số Lượng";
            ChatTextField.gI().tfChat.name = "Số Lượng";
        }
        if (item != null)
        {
            itemsell = item;
        }
        GameCanvas.panel.isShow = false;
        ChatTextField.gI().startChat2(new newShop(), string.Empty);
    }
    private static string ProcessString(ref string data, int maxLength)
    {
        string result;
        int newlineIndex = data.IndexOf('\n');
        if (newlineIndex >= 0 && newlineIndex < maxLength)
        {
            result = data.Substring(0, newlineIndex);
            data = data.Substring(newlineIndex + 1);
            if (data.Length > 0 && data[0] == ',')
            {
                data = data.Substring(1);
            }
            return result;
        }
        string temp = data.Substring(0, System.Math.Min(data.Length, maxLength));
        int lastSpaceIndex = temp.LastIndexOf(' ');

        if (lastSpaceIndex == -1 || lastSpaceIndex == maxLength - 1)
        {
            result = temp;
        }
        else
        {
            result = temp.Substring(0, lastSpaceIndex);
        }

        data = data.Substring(result.Length);
        return result;
    }
    public static void paintInfoItem(mGraphics g, Item item)
    {
        string[] optionItem = item == null ? new string[0] : getstrOption(item);
        string dataoptions = string.Empty;
        foreach (string option in optionItem)
        {
            dataoptions += option + ",";
        }
        dataoptions = dataoptions.Substring(0, dataoptions.Length - 1);
        string itemName = "Tên Vật Phẩm: " + (item == null ? "Chưa xác định" : item.template.name);
        long price = item == null ? -1 : item.buyGold;
        long price2 = item == null ? -1 : item.buyCoin;
        long price3 = item == null ? -1 : item.buySpec;
        string strbuy = "Giá bán: " + (item == null ? "Chưa xác định" : price > 0 ? price.ToString() : price2 > 0 ? price2.ToString() : price3 > 0 ? price3.ToString() : "Không xác định");
        string quatity = "Số lượng : " + (item == null ? "Chưa xác định" : item.quantity == 0 ? 1 : item.quantity);
        mFont.tahoma_7_blue.drawString(g, itemName, (int)(GameCanvas.w * 0.12), (int)(GameCanvas.h * 0.6), mFont.LEFT);
        string[] descriptionLines = mFont.tahoma_7.splitFontArray("Mô tả : " + item.template.description,(int)(GameCanvas.w*0.3));
        int yPosition = (int)(GameCanvas.h * 0.6) + 10;
        foreach (string line in descriptionLines)
        {
            mFont.tahoma_7.drawString(g, line, (int)(GameCanvas.w * 0.12), yPosition, mFont.LEFT);
            yPosition += 10;
        }
        if (item == null)
        {
            return;
        }
        descriptionLines = mFont.tahoma_7.splitFontArray(dataoptions, (int)(GameCanvas.w * 0.3));
        foreach (string line in descriptionLines)
        {
            mFont.tahoma_7.drawString(g, line, (int)(GameCanvas.w * 0.12), yPosition, mFont.LEFT);
            yPosition += 10;
        }
        mFont.tahoma_7.drawString(g, strbuy, (int)(GameCanvas.w * 0.45), (int)(GameCanvas.h * 0.6), mFont.LEFT);
        mFont.tahoma_7.drawString(g, quatity, (int)(GameCanvas.w * 0.45), (int)(GameCanvas.h * 0.65), mFont.LEFT);

        if (item.buyCoin > 0 || item.buyGold > 0 || item.buySpec > 0)
        {
            if (isTypeKiGui())
            {
                if (item.buyGold > 0)
                {
                    SmallImage.drawSmallImage(g, 4028, (int)(GameCanvas.w * 0.45) + (int)(mFont.tahoma_7_white.getWidth(strbuy) * 1.25), (int)(GameCanvas.h * 0.6222), 0, 3);
                }
                if (item.buyCoin > 0)
                {
                    g.drawImage(Panel.imgLuongKhoa, (int)(GameCanvas.w * 0.45) + (int)(mFont.tahoma_7_white.getWidth(strbuy) * 1.25), (int)(GameCanvas.h * 0.6222), 3);
                }
            }
            else
            {
                if (item.buyCoin > 0)
                {
                    g.drawImage(Panel.imgXu, (int)(GameCanvas.w * 0.45) + (int)(mFont.tahoma_7_white.getWidth(strbuy) * 1.25), (int)(GameCanvas.h * 0.6222), 3);
                }
                if (item.buyGold > 0)
                {
                    g.drawImage(Panel.imgLuong, (int)(GameCanvas.w * 0.45) + (int)(mFont.tahoma_7_white.getWidth(strbuy) * 1.25), (int)(GameCanvas.h * 0.6222), 3);
                }
                if (item.buySpec > 0)
                {
                    SmallImage.drawSmallImage(g, item.iconSpec, (int)(GameCanvas.w * 0.45) + (int)(mFont.tahoma_7_white.getWidth(strbuy) * 1.25), (int)(GameCanvas.h * 0.6222), 0, 3);
                }
            }

        }
        if (isTypeKiGui())
        {
            if (currentTab == 4)
            {
                mFont.tahoma_7_white.drawString(g, "Trạng Thái :" + getStringItemKyGui(item.buyType), (int)(GameCanvas.w * 0.675), (int)(GameCanvas.h * 0.678), mFont.CENTER);
                if (item.buyType == 0)
                {
                    cmdKyGuiVang.paint(g);
                    if (cmdKyGuiVang.isPointerPressInside())
                    {
                        StartKGV(item);
                        GameCanvas.clearAllPointerEvent();
                    }
                }
                else if (item.buyType == 1)
                {
                    cmdUpTop.paint(g);
                    cmdHuyKyGui.paint(g);

                    if (cmdUpTop.isPointerPressInside())
                    {
                        Service.gI().upTopKyGui(item.itemId);
                        GameCanvas.clearAllPointerEvent();
                    }
                    else if (cmdHuyKyGui.isPointerPressInside())
                    {
                        Service.gI().huyKyGui(item.itemId);
                        GameCanvas.clearAllPointerEvent();
                    }
                }
                else if (item.buyType == 2)
                {
                    cmdNhanTien.paint(g);
                    if (cmdNhanTien.isPointerPressInside())
                    {
                        Service.gI().nhanTienKyGui(item.itemId);
                        GameCanvas.clearAllPointerEvent();
                    }
                }
                else if (item.buyType == 3)
                {
                    cmdHuyKyGui.paint(g);
                    if (cmdHuyKyGui.isPointerPressInside())
                    {
                        Service.gI().huyKyGui(item.itemId);
                        GameCanvas.clearAllPointerEvent();
                    }
                }
            }
            else
            {
                cmdBuyKyGui.paint(g);
                if (cmdBuyKyGui.isPointerPressInside())
                {
                    Service.gI().buyKyGui(item.itemId);
                    GameCanvas.clearAllPointerEvent();
                }
            }
        }

    }
    private static List<string> SplitStringIntoLines(string str, int maxLineLength)
    {
        List<string> lines = new List<string>();
        while (str.Length > maxLineLength)
        {
            int breakIndex = str.LastIndexOf(' ', maxLineLength);
            if (breakIndex == -1)
            {
                breakIndex = maxLineLength;
            }
            lines.Add(str.Substring(0, breakIndex));
            str = str.Substring(breakIndex).Trim();
        }
        lines.Add(str);
        return lines;
    }
    static string getSLThoiVang()
    {

        int soluong = 0;
        foreach (Item item in Char.myCharz().arrItemBag)
        {
            if (item != null && item.template.id == 457)
            {
                soluong += item.quantity;
            }
        }
        return soluong.ToString();
    }
    static Image thoivang = null;
    public static void paintInfoMoney(mGraphics g)
    {
        int x = (int)(cmdBuyorSaleItem.x * 0.85);
        int y = (int)(cmdBuyorSaleItem.y * 1.25);
        g.setColor(15196114);
        g.setClip((int)(GameCanvas.w * 0.1), (int)(GameCanvas.h * 0.1), (int)(GameCanvas.w * 0.8), (int)(GameCanvas.h * 0.8));
        PopUp.paintPopUp(g, (int)(x * 0.93), (int)(y * 0.94), 500, 13, 0, true);

        g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
        y -= 2;
        if (thoivang == null)
        {
            if (SmallImage.imgNew[4028] != null)
            {
                thoivang = SmallImage.imgNew[4028].img;
            }
            else
            {
                SmallImage.createImage(4028);
            }
            return;
        }
        g.drawImage(Panel.imgXu, x, y - 7, 3);

        x += Panel.imgXu.getWidth();

        mFont.tahoma_7.drawString(g, NinjaUtil.getMoneys(Char.myCharz().xu), x, y - 13, mFont.LEFT) ;

        x += (int)(mFont.tahoma_7.getWidth(NinjaUtil.getMoneys(Char.myCharz().xu)) * 1.5);

        g.drawImage(Panel.imgLuong, x, y - 8, 3);

        x += Panel.imgLuong.getWidth();

        mFont.tahoma_7_green.drawString(g, Char.myCharz().luongStr.Equals("") ? "0" : Char.myCharz().luongStr, x, y - 13, mFont.LEFT);

        x += (int)(mFont.tahoma_7b_green.getWidth(Char.myCharz().luongStr.Equals("") ? "0" : Char.myCharz().luongStr) * 1.5);

        g.drawImage(Panel.imgLuongKhoa, x, y - 8, 3);

        x += Panel.imgLuongKhoa.getWidth();

        mFont.tahoma_7_red.drawString(g, Char.myCharz().luongKhoaStr.Equals("") ? "0" : Char.myCharz().luongKhoaStr, x, y - 13, mFont.LEFT);

        x += (int)(mFont.tahoma_7b_green.getWidth(Char.myCharz().luongKhoaStr.Equals("") ? "0" : Char.myCharz().luongKhoaStr) * 1.7);
        g.drawImage(thoivang, x, y - 7, 3);
        x += thoivang.getWidth() - 5;
        mFont.tahoma_7.drawString(g, getSLThoiVang(), x, y - 13, mFont.LEFT);

    }
    private static void paintPopup(mGraphics g)
    {
        //g.setClip((int)(GameCanvas.w * 0.1), (int)(GameCanvas.h * 0.1), (int)(GameCanvas.w * 0.8), (int)(GameCanvas.h * 0.8));
        //g.setColor(9993045);
        //g.fillRect((int)(GameCanvas.w * 0.1), (int)(GameCanvas.h * 0.1), (int)(GameCanvas.w * 0.8), (int)(GameCanvas.h * 0.8));
        //g.setClip((int)(GameCanvas.w * 0.1), (int)(GameCanvas.h * 0.6), (int)(GameCanvas.w * 0.8), (int)(GameCanvas.h * 0.3));
        //g.setColor(11837316);
        //g.fillRect((int)(GameCanvas.w * 0.1), (int)(GameCanvas.h * 0.6), (int)(GameCanvas.w * 0.8), (int)(GameCanvas.h * 0.3));
        PopUp.paintPopUp(g, (int)(GameCanvas.w * 0.1), (int)(GameCanvas.h * 0.1), (int)(GameCanvas.w * 0.8), (int)(GameCanvas.h * 0.8),0, true   );
        PopUp.paintPopUp(g, (int)(GameCanvas.w * 0.1), (int)(GameCanvas.h * 0.6), (int)(GameCanvas.w * 0.8), (int)(GameCanvas.h * 0.3), 0, true);

        if (imgExit == null)
        {
            GameCanvas.loadImage("/mainImage/myTexture2dbtX.png");
            return;
        }
        g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
        cmdExit.paint(g);
    }
    private static void paintTab(mGraphics g)
    {
        g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
        for (int i = 0; i <= (!isTypeKiGui() ? TabName.Length : TabName.Length - 1); i++)
        {
            string strTab = i != TabName.Length ? TabName[i] : "Hành  Trang";
            int color = (i != currentTab) ? 0 : 1;
            PopUp.paintPopUp(g,(int)(GameCanvas.w * 0.10) + (65 * (i)), (int)(GameCanvas.h * 0.1), 60, 18,color,true);
            mFont.tahoma_7.drawString(g, strTab, (int)(GameCanvas.w * 0.12) + (65 * (i)), (int)(GameCanvas.h * 0.1), mFont.LEFT);
        }
        g.setColor(UnityEngine.Color.black);
    }
    private static List<Item> itemTab(byte tab)
    {
        List<Item> its = new List<Item>();
        foreach (Item it in itemkygui)
        {
            if (it != null && it.tab == tab)
            {
                its.Add(it);
            }
        }
        return its;
    }
    private static int itemSpacing = (mGraphics.zoomLevel - 1) * 3;
    private static void paintItemShop(mGraphics g)
    {

        int w = GameCanvas.w, h = GameCanvas.h;
        g.setClip((int)(w * 0.1), (int)(h * 0.15), (int)(w * 0.8), (int)(h * 0.43));
        g.translate(scroll.cmx, -scroll.cmy);

        int xStart = (int)(w * 0.12), yStart = (int)(h * 0.18);
        int itemWidth = 90, itemHeight = 35;
        int itemsPerRow = (int)(w * 0.73) / (itemWidth + itemSpacing);
        if (!isTypeKiGui())
        {
            Item[] items = Char.myCharz().arrItemShop[currentTab];
            for (int i = 0; i < items.Length; i++)
            {
                int row = i / itemsPerRow;
                int x = xStart + (i % itemsPerRow) * (itemWidth + itemSpacing);
                int y = yStart + row * (itemHeight + itemSpacing);
                int color = i != selectItem ? 0 : 1;
                PopUp.paintPopUp(g,x, y, itemWidth, itemHeight,color,true);
                if (items[i] != null)
                {
                    int xPaint = x + (int)(w * 0.025), yPaint = y + (int)(h * 0.05);
                    SmallImage.drawSmallImage(g, items[i].template.iconID, xPaint, yPaint, 0, 3);
                    if (items[i].newItem && GameCanvas.gameTick % 20 > 10)
                    {
                        g.drawImage(Panel.imgNew, xPaint, yPaint + 7, 3);
                    }
                    int xclone = x,yclone = y;
                    var data = mFont.tahoma_7_blue.splitFontArray(items[i].template.name,45);
                    foreach (var strs in data)
                    {
                        mFont.tahoma_7_blue.drawString(g, strs, xclone + 35, yclone+3, 0);
                        yclone += 9;
                    }
                    
                }
            }
        }
        else
        {

            List<Item> items = itemTab((byte)currentTab);
            for (int i = 0; i < items.Count; i++)
            {
                int row = i / itemsPerRow;
                int x = xStart + (i % itemsPerRow) * (itemWidth + itemSpacing);
                int y = yStart + row * (itemHeight + itemSpacing);

                int color = i != selectItem ? 0 : 1;
                PopUp.paintPopUp(g, x, y, itemWidth, itemHeight, color, true);
                if (items[i] != null)
                {

                    if (currentTab == 4)
                    {
                        if (items[i].buyType != 0)
                        {
                            if (items[i].buyType == 1)
                            {
                                g.setColor(15792007, 0.7f);
                            }
                            else if (items[i].buyType == 2)
                            {
                                g.setColor(6750054, 0.7f);
                            }
                            else if (items[i].buyType == 3)
                            {
                                g.setColor(16711680, 0.3f);
                            }
                            g.fillRect(x, y, itemWidth, itemHeight);
                        }
                    }
                    else if (items[i].isMe == 1)
                    {
                        g.setColor(15792007, 0.7f);
                        g.fillRect(x, y, itemWidth, itemHeight);
                    }

                    int xPaint = x + (int)(w * 0.028), yPaint = y + (int)(h * 0.045);
                    try
                    {
                        if (items[i].buyGold > 0)
                        {
                            mFont.tahoma_7b_yellow.drawString(g, NinjaUtil.getMoneys(items[i].buyGold), xPaint + 15, yPaint - 7, 0);
                            SmallImage.drawSmallImage(g, 4028, xPaint + 15 + (int)(mFont.tahoma_7b_yellow.getWidth(NinjaUtil.getMoneys(items[i].buyGold) + "  ") * 1.6), yPaint - 3, 0, 3);
                        }
                        int yclone = yPaint;
                        var data = mFont.tahoma_7_blue.splitFontArray(items[i].template.name, 50);
                        foreach (var strs in data)
                        {
                            mFont.tahoma_7_blue.drawString(g, strs, xPaint + 15, yclone, 0);
                            yclone += 9;
                        }
                        SmallImage.drawSmallImage(g, items[i].template.iconID, xPaint, yPaint, 0, 3);
                        mFont.tahoma_7.drawString(g, i.ToString(), xPaint - (int)(w * 0.0165), yPaint - 13, 2);
                       
                    }
                    catch (Exception e) { Debug.LogError(e.ToString()); }
                }
            }
        }
        g.reset();
    }
    private static void paintInventory(mGraphics g)
    {
        int w = GameCanvas.w, h = GameCanvas.h;
        g.setClip((int)(w * 0.1), (int)(h * 0.2), (int)(w * 0.8), (int)(h * 0.4));
        g.translate(scroll.cmx, -scroll.cmy);

        int xStart = (int)(w * 0.12), yStart = (int)(h * 0.2);
        int itemWidth = 90, itemHeight = 35;
        int itemsPerRow = (int)(w * 0.73) / (itemWidth + itemSpacing);
        Item[] items = Char.myCharz().arrItemBag;

        for (int i = 0; i < items.Length; i++)
        {
            int row = i / itemsPerRow;
            int x = xStart + (i % itemsPerRow) * (itemWidth + itemSpacing);
            int y = yStart + row * (itemHeight + itemSpacing);

            g.setColor(i != selectItem ? 15196114 : 16383818);
            PopUp.paintPopUp(g, x, y, itemWidth, itemHeight, i != selectItem ? 0 : 1, true);
        

            if (items[i] != null)
            {
                int xPaint = x + (int)(w * 0.028), yPaint = y + (int)(h * 0.05);
                try
                {
                    try
                    {
                        SmallImage.drawSmallImage(g, items[i].template.iconID, xPaint, yPaint - 5, 0, 3);
                    }
                    catch (Exception e){ Debug.LogError("Error : " + items[i].template.id); }
                    mFont.tahoma_7.drawString(g, items[i].quantity.ToString(), xPaint, yPaint+2, mFont.CENTER);
                    int yclone = yPaint;
                    var data = mFont.tahoma_7_blue.splitFontArray(items[i].template.name, 50);
                    foreach (var strs in data)
                    {
                        mFont.tahoma_7_blue.drawString(g, strs, xPaint + 15, yclone-5, 0);
                        yclone += 9;
                    }
                }
                catch(Exception e) { Debug.LogError(e.ToString()); }
            }
        }
        g.reset();
    }
    private static bool isTypeKiGui()
    {
        return GameCanvas.panel.type == 1 && GameCanvas.panel.typeShop == 2;
    }
    public static void paint(mGraphics g)
    {
        try
        {
            paintPopup(g);
            paintTab(g);
            paintInfoMoney(g);
            if (!isTypeKiGui())
            {
                cmdBuyorSaleItem.paint(g);
                if (currentTab < TabName.Length)
                {
                    cmdBuys.paint(g);
                    paintInfoItem(g, Char.myCharz().arrItemShop[currentTab][selectItem]);
                    paintItemShop(g);
                    if (cmdBuys.isPointerPressInside())
                    {
                        Item item13 = Char.myCharz().arrItemShop[currentTab][selectItem];
                        BuysItem(item13);
                    }
                }
                else if (currentTab == TabName.Length)
                {
                    try
                    { paintInfoItem(g, Char.myCharz().arrItemBag[selectItem]); }
                    catch { }
                    try { paintInventory(g); }
                    catch { }
                }
            }
            else
            {
                if (currentTab < TabName.Length)
                {
                    if (selectItem < itemTab((byte)currentTab).Count && itemTab((byte)currentTab).Count != 0)
                    {
                        try
                        { paintInfoItem(g, itemTab((byte)currentTab)[selectItem]); }
                        catch { Debug.Log(itemTab((byte)currentTab).Count); };
                    }
                    paintItemShop(g);
                }
            }
        }

        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }
    private static long lastChatQuantity;
    private static byte isKyGui;
    static int maxbuy;
    static bool isbuys;
    public void onChatFromMe(string text, string to)
    {
        bool flag = ChatTextField.gI().tfChat.getText() != null && !ChatTextField.gI().tfChat.getText().Equals(string.Empty) && !text.Equals(string.Empty) && text != null;
        if (flag)
        {
            if (ChatTextField.gI().strChat.Equals("Nhập Số Lượng") && itemsell != null)
            {
                itemsell.quantilyToBuy = int.Parse(ChatTextField.gI().tfChat.getText());
                isKyGui = 1;
                lastChatQuantity = mSystem.currentTimeMillis();
            }
            else if (ChatTextField.gI().strChat.Equals("Nhập Số Lượng ") && itemsell != null)
            {
                itemsell.quantilyToBuy = int.Parse(ChatTextField.gI().tfChat.getText());
                isKyGui = 2;
                lastChatQuantity = mSystem.currentTimeMillis();
            }
            else if (ChatTextField.gI().strChat.Equals("Ký Gửi Thỏi Vàng"))
            {
                int quantily = itemsell.quantilyToBuy == 0 ? 1 : itemsell.quantilyToBuy;
                int price = int.Parse(ChatTextField.gI().tfChat.getText());
                Service.gI().sellKyGui(itemsell.itemId, quantily, -1, price);
            }
            else if (ChatTextField.gI().strChat.Equals("Ký Gửi Hồng Ngọc"))
            {
                int quantily = itemsell.quantilyToBuy == 0 ? 1 : itemsell.quantilyToBuy;
                int price = int.Parse(ChatTextField.gI().tfChat.getText());
                Service.gI().sellKyGui(itemsell.itemId, quantily, price, -1);
            }
            else if (ChatTextField.gI().strChat.Equals("Nhập Số Lượng Muốn Mua"))
            {
                Debug.LogError("buys");
                maxbuy = int.Parse(ChatTextField.gI().tfChat.getText());
                isbuys = true;
            }
            else
            {
                Debug.LogError(ChatTextField.gI().strChat);
            }
        }
        else
        {
            Debug.LogError("NULL tfchat");
        }
        ResetChatTextField();
    }
    private static void ResetChatTextField()
    {
        ChatTextField.gI().strChat = "Chat";
        ChatTextField.gI().tfChat.name = "chat";
        ChatTextField.gI().isShow = false;
        ChatTextField.gI().parentScreen = GameScr.gI();
        GameCanvas.panel.isShow = true;
    }
    public void onCancelChat()
    {
        if (ChatTextField.gI().strChat.Equals("Ký Gửi Thỏi Vàng"))
        {
            itemsell = null;
        }
        else if (ChatTextField.gI().strChat.Equals("Ký Gửi Hồng Ngọc"))
        {
            itemsell = null;
        }
        else if (ChatTextField.gI().strChat.Equals("Nhập Số Lượng Muốn Mua"))
        {
            itembuy = null;
            maxbuy = 0;
            isbuys = false;
        }
    }
}
