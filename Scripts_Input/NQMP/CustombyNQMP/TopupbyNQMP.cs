using System;
using System.Collections.Generic;
using UnityEngine;

public class TopupbyNQMP : mScreen
{

    int width = 350;
    int height = 200;
    int x => (GameCanvas.w / 2) - width / 2;
    int y => (GameCanvas.h / 2) - height / 2;
    byte[] bytes;
    ComboBoxbyNQMP loaiThe;
    ComboBoxbyNQMP menhGia;
    int currentIndex;
    public bool isShow;
    public string[] description = new string[2]
    {
        "Hệ thống nạp thẻ tự động - Tỉ lệ nạp 1.0\n" +
        "Vui lòng nhập đúng mã thẻ và mệnh giá\n" +
        "Thẻ sai mệnh giá sẽ bị mất thẻ và không được cộng tiền",

        "Hệ thống nạp ngân hàng tự động - Tỉ lệ nạp 1.2\n" +
        "Khi chuyển khoản thành công sẽ được cộng tiền\nsau 1-5p\n" +
        "Hãy nhập đúng nội dung , sai nội dung sẽ mất"
    };

    string[] strings = new string[3] { "Thẻ cào", "Ngân hàng", "Lịch sử" };
    int popUpWidth = 50;
    int popUpHeight = 20;
    TField[][] fields = new TField[2][] { new TField[2], new TField[2] };
    Command btnTopup = new Command("Gửi yêu cầu", -1);
    Command btnExit = new Command();
    Image imgQR = null;
    string[] infoBanking = new string[3];

    private Scroll scrollHistory;
    private int historyItemHeight = 45;
    private void initHistoryScroll()
    {
        if (scrollHistory == null)
        {
            scrollHistory = new Scroll();
        }

        // Tính toán lại chiều cao cần thiết cho tất cả các mục
        int totalContentHeight = (historyTopups.Count + 1) * historyItemHeight;
        int visibleHeight = height - 50; // Trừ đi phần tiêu đề và phần hướng dẫn ở dưới

        // Thiết lập style cho scroll
        scrollHistory.setStyle(
            historyTopups.Count,  // số lượng item
            historyItemHeight,    // chiều cao mỗi item
            x + 10,               // vị trí x
            y + 35,               // vị trí y 
            width - 20,           // chiều rộng vùng scroll
            visibleHeight,        // chiều cao vùng scroll
            true,                 // sử dụng scrollUpDown
            1                     // số item trên mỗi dòng
        );

        // Đảm bảo cmyLim được thiết lập đúng
        if (totalContentHeight > visibleHeight)
        {
            scrollHistory.cmyLim = totalContentHeight - visibleHeight;
        }
        else
        {
            scrollHistory.cmyLim = 0;
        }

        // Reset vị trí về đầu
        scrollHistory.cmtoY = 0;
        scrollHistory.cmy = 0;

        Debug.LogError("Scroll initialized: items=" + historyTopups.Count +
                      ", cmyLim=" + scrollHistory.cmyLim +
                      ", height=" + visibleHeight +
                      ", totalHeight=" + totalContentHeight);
    }

    public void paintHistory(mGraphics g)
    {
        // Vẽ tiêu đề
        mFont.tahoma_8b.drawString(g, "LỊCH SỬ NẠP TIỀN", (GameCanvas.w / 2), y + 25, mFont.CENTER);

        // Khởi tạo scroll nếu chưa có hoặc có sự thay đổi về số lượng item
        if (scrollHistory == null || scrollHistory.nITEM != historyTopups.Count)
        {
            initHistoryScroll();
        }

        // Vẽ background cho thanh tiêu đề
        g.setColor(0x5c95ff);
        g.fillRect(scrollHistory.xPos, scrollHistory.yPos, scrollHistory.width, 20);

        // Vẽ các tiêu đề cột
        mFont.tahoma_7b_white.drawString(g, "ID", scrollHistory.xPos + 15, scrollHistory.yPos + 4, mFont.LEFT);
        mFont.tahoma_7b_white.drawString(g, "Loại", scrollHistory.xPos + 60, scrollHistory.yPos + 4, mFont.LEFT);
        mFont.tahoma_7b_white.drawString(g, "Mệnh giá", scrollHistory.xPos + 130, scrollHistory.yPos + 4, mFont.LEFT);
        mFont.tahoma_7b_white.drawString(g, "Trạng thái", scrollHistory.xPos + scrollHistory.width - 20, scrollHistory.yPos + 4, mFont.RIGHT);

        // Tính toán vị trí bắt đầu và kết thúc để vẽ các item
        int startIndex = scrollHistory.cmy / historyItemHeight;
        int endIndex = startIndex + (scrollHistory.height / historyItemHeight) + 2; // +2 để tránh hiện tượng "nhấp nháy"

        if (endIndex > historyTopups.Count)
        {
            endIndex = historyTopups.Count;
        }

        // Thiết lập clip cho vùng scroll
        g.setClip(scrollHistory.xPos, scrollHistory.yPos + 20, scrollHistory.width, scrollHistory.height - 20);

        // Vẽ các item lịch sử
        for (int i = startIndex; i < endIndex; i++)
        {
            int yPaint = scrollHistory.yPos + 20 + (i * historyItemHeight) - scrollHistory.cmy;

            if (i < historyTopups.Count)
            {
                HistoryTopupItem item = historyTopups[i];

                // Vẽ background cho item với màu xen kẽ để dễ đọc
                if (i % 2 == 0)
                {
                    g.setColor(0xf0f0f0);
                }
                else
                {
                    g.setColor(0xffffff);
                }
                g.fillRect(scrollHistory.xPos, yPaint, scrollHistory.width, historyItemHeight - 1);

                // Vẽ đường ngăn cách giữa các item
                g.setColor(0xdddddd);
                g.drawLine(scrollHistory.xPos, yPaint + historyItemHeight - 1,
                           scrollHistory.xPos + scrollHistory.width, yPaint + historyItemHeight - 1);

                // Xác định các thông tin cần hiển thị
                string typeStr = item.type == 0 ? "Thẻ cào" : "Ngân hàng";
                string statusStr;
                int statusColor;

                switch (item.status)
                {
                    case 0:
                        statusStr = "Đang xử lý";
                        statusColor = 0x0066ff; // màu xanh dương
                        break;
                    case 1:
                        statusStr = "Thành công";
                        statusColor = 0x00b300; // màu xanh lá
                        break;
                    case 2:
                        statusStr = "Thất bại";
                        statusColor = 0xff0000; // màu đỏ
                        break;
                    default:
                        statusStr = "Không xác định";
                        statusColor = 0x666666; // màu xám
                        break;
                }

                // Vẽ ID
                mFont.tahoma_7b_dark.drawString(g, "#" + item.id, scrollHistory.xPos + 15, yPaint + 8, mFont.LEFT);

                // Vẽ loại nạp
                mFont.tahoma_7.drawString(g, typeStr, scrollHistory.xPos + 60, yPaint + 8, mFont.LEFT);

                // Vẽ mệnh giá và thực nhận
                string amountStr = GameCanvas.getMoneys(item.amount);
                string realAmountStr = GameCanvas.getMoneys(item.realAmount);

                mFont.tahoma_7.drawString(g, amountStr, scrollHistory.xPos + 130, yPaint + 8, mFont.LEFT);

                // Vẽ thông tin thực nhận và ngày
                mFont.tahoma_7_green.drawString(g, realAmountStr, scrollHistory.xPos + 130, yPaint + 25, mFont.LEFT);
                mFont.tahoma_7_grey.drawString(g, "Ngày: " + item.dateTime, scrollHistory.xPos + 15, yPaint + 25, mFont.LEFT);

                // Vẽ trạng thái với màu tương ứng
                g.setColor(statusColor);
                g.fillRect(scrollHistory.xPos + scrollHistory.width - 80, yPaint + 5, 70, 16);

                mFont.tahoma_7b_white.drawString(g, statusStr,
                    scrollHistory.xPos + scrollHistory.width - 45, yPaint + 7, mFont.CENTER);

                // Vẽ nút xem chi tiết (làm theo dạng link)
                mFont.tahoma_7b_blue.drawString(g, "Chi tiết",
                    scrollHistory.xPos + scrollHistory.width - 15, yPaint + 25, mFont.RIGHT);
            }
        }

        // Bỏ clip để vẽ tiếp các phần khác
        g.setClip(0, 0, GameCanvas.w, GameCanvas.h);

        // Vẽ thông báo nếu không có dữ liệu
        if (historyTopups.Count == 0)
        {
            g.setColor(0xffffff);
            g.fillRect(scrollHistory.xPos, scrollHistory.yPos + 20, scrollHistory.width, scrollHistory.height - 20);

            mFont.tahoma_7b_dark.drawString(g, "Chưa có lịch sử nạp tiền",
                scrollHistory.xPos + scrollHistory.width / 2, scrollHistory.yPos + scrollHistory.height / 2, mFont.CENTER);
        }

        // Vẽ phần hướng dẫn cuộn
        g.setColor(0xaaaaaa);
        g.fillRect(x, (y + height) - 20, width, 20);
        g.setColor(0xffffff);

        if (historyTopups.Count > 0)
        {
            mFont.tahoma_7b_dark.drawString(g, "↑ Vuốt để cuộn ↓",
                GameCanvas.w / 2, (y + height) - 15, mFont.CENTER);
        }
    }

    private void updateHistoryScroll()
    {
        if (scrollHistory != null)
        {
            // Cập nhật animation cuộn
            scrollHistory.updatecm();

            // Kiểm tra xem có cần khởi tạo lại scroll không
            if (scrollHistory.nITEM != historyTopups.Count)
            {
                initHistoryScroll();
            }

            // Xử lý sự kiện cuộn
            ScrollResult result = scrollHistory.updateKey();

            // Kiểm tra lại giới hạn cuộn
            if (scrollHistory.cmy < 0)
            {
                scrollHistory.cmy = 0;
                scrollHistory.cmtoY = 0;
            }

            if (scrollHistory.cmy > scrollHistory.cmyLim)
            {
                scrollHistory.cmy = scrollHistory.cmyLim;
                scrollHistory.cmtoY = scrollHistory.cmyLim;
            }

            // Xử lý khi người dùng nhấn vào một mục cụ thể
            if (result.isFinish && result.selected >= 0 && result.selected < historyTopups.Count)
            {
                // Xử lý khi người dùng nhấn vào một mục cụ thể trong lịch sử
                HistoryTopupItem selectedItem = historyTopups[result.selected];
                GameCanvas.startOKDlg("Chi tiết nạp tiền #" + selectedItem.id +
                                   "\nLoại: " + (selectedItem.type == 0 ? "Thẻ cào" : "Ngân hàng") +
                                   "\nMệnh giá: " + GameCanvas.getMoneys(selectedItem.amount) +
                                   "\nThực nhận: " + GameCanvas.getMoneys(selectedItem.realAmount) +
                                   "\nNgày: " + selectedItem.dateTime +
                                   "\nTrạng thái: " + (selectedItem.status == 0 ? "Đang xử lý" :
                                                     (selectedItem.status == 1 ? "Thành công" : "Thất bại")));
            }
        }
    }



    public static TopupbyNQMP instance;

    public static TopupbyNQMP gI()
    {
        instance ??= new TopupbyNQMP();
        return instance;
    }

    public override void switchToMe()
    {
        GameScr.isPaintOther = true;
        base.switchToMe();
    }

    public void init()
    {

        fields[0][0] = new TField();
        fields[0][0].x = x + 170;
        fields[0][0].y = y + 50;
        fields[0][0].width = 150;
        fields[0][0].height = mFont.tahoma_8b.getHeight() + 8;
        fields[0][0].setIputType(TField.INPUT_TYPE_NUMERIC);
        fields[0][0].name = "Mã Seri Của Thẻ";
        fields[0][1] = new TField();
        fields[0][1].x = x + 170;
        fields[0][1].y = y + 100;
        fields[0][1].width = 150;
        fields[0][1].height = mFont.tahoma_8b.getHeight() + 8;
        fields[0][1].setIputType(TField.INPUT_TYPE_NUMERIC);
        fields[0][1].name = "Mã Thẻ";
        loaiThe = new ComboBoxbyNQMP(x + 15, y + 50, 150, mFont.tahoma_8b.getHeight() + 12, captions: "Chọn loại thẻ");
        loaiThe.AddItem("VIETELL", "Vietell");
        loaiThe.AddItem("MOBIPHONE", "Mobiphone");
        loaiThe.AddItem("VINAPHONE", "Vinaphone");
        menhGia = new ComboBoxbyNQMP(x + 15, y + 100, 150, mFont.tahoma_8b.getHeight() + 12, 5, captions: "Chọn mệnh giá");
        menhGia.AddItem("10.000", 10000);
        menhGia.AddItem("20.000", 20000);
        menhGia.AddItem("30.000", 30000);
        menhGia.AddItem("50.000", 50000);
        menhGia.AddItem("100.000", 100000);
        menhGia.AddItem("200.000", 200000);
        menhGia.AddItem("300.000", 300000);
        menhGia.AddItem("500.000", 500000);
        menhGia.AddItem("1.000.000", 1000000);
        btnTopup.x = (GameCanvas.w / 2) - 35;
        btnTopup.y = (y + height) - btnTopup.h;
        btnExit.img = GameCanvas.loadImage("/mainImage/myTexture2dbtX.png");
        btnExit.x = x + width - btnExit.img.getWidth() - 5;
        btnExit.y = y + btnExit.img.getHeight() / 2;

        if (historyTopups.Count == 0)
        {
            for (int i = 0; i < 10; i++)
            {
                HistoryTopupItem item = new HistoryTopupItem();
                item.id = i + 1;
                item.type = (i % 2); // 0: Thẻ cào, 1: Ngân hàng
                item.status = (byte)(i % 3); // 0: Đang xử lý, 1: Thành công, 2: Thất bại
                item.dateTime = DateTime.Now.AddDays(-i).ToString("dd/MM/yyyy HH:mm");
                item.amount = (i + 1) * 10000;
                item.realAmount = item.status == 1 ? item.amount * (item.type == 0 ? 1 : 12) / 10 : 0;
                item.seri = "12345678901234";
                item.code = "9876543210123";
                historyTopups.Add(item);
            }
        }
        

    }
    public void requestMessage(Message msg)
    {
        try
        {
            sbyte type = msg.reader().readByte();
            if (type == 0)
            {
                TopupbyNQMP.gI().init();
                TopupbyNQMP.gI().switchToMe();
            }
            if (type == 1)
            {
                Debug.LogError("Start createImage");
                infoBanking[0] = msg.reader().readUTF();
                infoBanking[1] = msg.reader().readUTF();
                infoBanking[2] = msg.reader().readUTF();
                byte[] bytes = new byte[msg.reader().readInt()];
                Debug.LogError("byte Lenght " + bytes.Length);
                for (int i = 0; i < bytes.Length; i++)
                {
                    bytes[i] = msg.reader().readUnsignedByte();
                }
                imgQR = Image.createImage(bytes);
                GameCanvas.resizeImage(imgQR, 125, 125);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }
    public override void update()
    {

        GameScr.gI().update();
        if (currentIndex == 0)
        {
            fields[0][0]?.update();
            fields[0][0].isLogUpdate = true;
            fields[0][1]?.update();
            loaiThe?.update();
            menhGia?.update();
        }
        else if (currentIndex == 1)
        {
            menhGia?.update();
        }
        else if (currentIndex == 2)
        {
            updateHistoryScroll();
        }
    }
    public override void keyPress(int keyCode)
    {
        if (currentIndex == 0)
        {
            fields[0][0]?.keyPressed(keyCode);
            fields[0][1]?.keyPressed(keyCode);
        }
    }
    bool click => GameCanvas.isPointerClick && GameCanvas.isPointerJustRelease;
    public override void updateKey()
    {
        bool isReset = false;

        loaiThe.updateKey();
        menhGia.updateKey();
        if (GameCanvas.isPointerHoldIn(x, y, width, height))
        {
            for (int i = 0; i < strings.Length; i++)
            {
                int xP = x + 15 + ((popUpWidth + 3) * i);
                int yP = y + 5;
                if (GameCanvas.isPointerHoldIn(xP, yP, popUpWidth, popUpHeight) && click)
                {
                    currentIndex = i;
                    btnTopup.caption = currentIndex == 0 ? "Gửi thẻ" : "Tạo Đơn";
                    isReset = true;
                    break;
                }
            }
        }
        if (btnTopup.isPointerPressInside())
        {
            checkTopUp();
            isReset = true;
        }
        if (btnExit.isPointerPressInside())
        {
            GameScr.gI().switchToMe();
            isReset = true;
        }
        if (isReset)
        {
            GameCanvas.clearAllPointerEvent();
        }
    }

    public override void paint(mGraphics g)
    {
        GameScr.gI().paint(g);
        PopUp.paintPopUp(g, x, y, width, height, 0, true);
        btnExit.paint(g);
        for (int i = 0; i < strings.Length; i++)
        {
            string s = strings[i];
            PopUp.paintPopUp(g, x + 15 + ((popUpWidth + 3) * i), y + 5, popUpWidth, popUpHeight, currentIndex == i ? 1 : 0, true);
            mFont.tahoma_7b_dark.drawString(g, s, x + 15 + ((popUpWidth + 3) * i) + popUpWidth / 2, y + 10, mFont.CENTER);
        }
        if (currentIndex == 0)
        {
            paintNapThe(g);
            if (mSystem.currentTimeMillis() - lastTopUp >= 30000)
            { btnTopup.paint(g); }
            else
            {
                long time = (lastTopUp + 30000) - mSystem.currentTimeMillis();
                time /= 1000;
                mFont.tahoma_7b_dark.drawString(g, "Hãy đợi " + time + "s nữa", GameCanvas.w / 2, (y + height) - 30, mFont.CENTER);
            }
        }
        else if (currentIndex == 1)
        {
            paintBanking(g);
            if (mSystem.currentTimeMillis() - lastTopUp >= 30000)
            { btnTopup.paint(g); }
            else
            {
                long time = (lastTopUp + 30000) - mSystem.currentTimeMillis();
                time /= 1000;
                mFont.tahoma_7b_dark.drawString(g, "Hãy đợi " + time + "s nữa để có thể tạo QR mới", GameCanvas.w / 2, (y + height) - 30, mFont.CENTER);

            }
        }
        else if (currentIndex == 2)
        {
            paintHistory(g);
        }


    }


    public void paintNapThe(mGraphics g)
    {
        try
        {
            mFont.tahoma_7b_dark.drawString(g, "Nạp Thẻ Cào", (GameCanvas.w / 2), y + 25, mFont.CENTER);
            mFont.tahoma_7b_dark.drawString(g, description[0], x + 10, y + 130, mFont.LEFT);
            menhGia.paint(g);
            loaiThe.paint(g);

            fields[0][0]?.paint(g);
            fields[0][1]?.paint(g);
            g.setClip(0, 0, GameCanvas.w, GameCanvas.h);

        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }



    }
    public void paintBanking(mGraphics g)
    {
        mFont.tahoma_7b_dark.drawString(g, "Nạp Ngân Hàng", (GameCanvas.w / 2), y + 25, mFont.CENTER);
        if (imgQR != null)
        {
            g.drawImage(imgQR, x + width / 2 + 32, y + 37);
            mFont.tahoma_7b_dark.drawString(g, $"Số tài khoản:{infoBanking[0]}", x + 10, y + 125, mFont.LEFT);
            mFont.tahoma_7b_dark.drawString(g, $"Số tiền nạp :{infoBanking[1]}", x + 10, y + 140, mFont.LEFT);
            mFont.tahoma_7b_dark.drawString(g, $"Nội dung    :{infoBanking[2]}", x + 10, y + 155, mFont.LEFT);

        }
        menhGia.paint(g);
        PopUp.paintPopUp(g, x + width / 2 + 30, y + 35, 130, 130, 0, true);
        mFont.tahoma_7b_dark.drawString(g, "Mã QR", x + width / 2 + 95, y + 175, mFont.CENTER);
        mFont.tahoma_7b_dark.drawString(g, description[1], x + 10, y + 50, mFont.LEFT);
       

    }

    public bool IsNumeric(string input)
    {
        // Kiểm tra input có null hoặc rỗng không
        if (string.IsNullOrEmpty(input))
        {
            return false;
        }

        // Kiểm tra từng ký tự trong chuỗi
        foreach (char c in input)
        {
            if (!char.IsDigit(c))
            {
                return false;
            }
        }

        return true;
    }
    public bool checkValidCard(string seri, string mathe)
    {
        // Kiểm tra seri và mã thẻ có rỗng không
        if (string.IsNullOrEmpty(seri) || string.IsNullOrEmpty(mathe))
        {
            return false;
        }

        // Kiểm tra seri và mã thẻ chỉ chứa số
        if (!IsNumeric(seri) || !IsNumeric(mathe))
        {
            return false;
        }

        // Lấy loại thẻ đang được chọn
        int selectedCardType = loaiThe.GetSelectedIndex();

        // Kiểm tra theo loại thẻ
        switch (selectedCardType)
        {
            case 0:
                if ((seri.Length == 11 && mathe.Length == 13) ||
                    (seri.Length == 14 && mathe.Length == 15))
                {
                    return true;
                }
                break;

            case 1: // MOBIPHONE
                    // Mã thẻ cào có 12 số
                    // Dãy seri của thẻ cào có tất cả 15 số
                if (mathe.Length == 12 && seri.Length == 15)
                {
                    return true;
                }
                break;

            case 2: // VINAPHONE
                    // Mã thẻ cào có 12 hoặc 14 số
                    // Dãy seri của thẻ cào có tất cả 14 số
                if ((mathe.Length == 12 || mathe.Length == 14) && seri.Length == 14)
                {
                    return true;
                }
                break;
        }

        return false;
    }
    long lastTopUp;
    public void checkTopUp()
    {
        if (currentIndex == 0)
        {
            if (loaiThe.GetSelectedIndex() == -1)
            {
                GameCanvas.startOKDlg("Vui lòng chọn loại thẻ cần nạp");
                return;
            }
            if (menhGia.GetSelectedIndex() == -1)
            {
                GameCanvas.startOKDlg("Vui lòng chọn mệnh giá muốn nạp");
                return;
            }
            if (fields[0][0].getText() == null || fields[0][0].getText().Equals(string.Empty))
            {
                GameCanvas.startOKDlg("Mã seri không được để trống");
                return;
            }
            if (fields[0][1].getText() == null || fields[0][1].getText().Equals(string.Empty))
            {
                GameCanvas.startOKDlg("Mã thẻ không được để trống");
                return;
            }
            if (!IsNumeric(fields[0][0].getText()))
            {
                GameCanvas.startOKDlg("Mã seri chỉ được chứa số");
                return;
            }
            if (!IsNumeric(fields[0][1].getText()))
            {
                GameCanvas.startOKDlg("Mã thẻ chỉ được chứa số");
                return;
            }

            // Kiểm tra tính hợp lệ của thẻ
            string seri = fields[0][0].getText();
            string mathe = fields[0][1].getText();
            string loai = "";
            if (!checkValidCard(seri, mathe))
            {

                switch (loaiThe.GetSelectedIndex())
                {
                    case 0:
                        GameCanvas.startOKDlg("Thẻ Viettel không hợp lệ!\nSeri phải có 11 hoặc 14 số\nMã thẻ phải có 13 hoặc 15 số tương ứng");
                        return;
                    case 1:
                        GameCanvas.startOKDlg("Thẻ Mobiphone không hợp lệ!\nSeri phải có 15 số\nMã thẻ phải có 12 số");
                        return;
                    case 2:
                        GameCanvas.startOKDlg("Thẻ Vinaphone không hợp lệ!\nSeri phải có 14 số\nMã thẻ phải có 12 hoặc 14 số");
                        return;
                }
                return;
            }
            // Tiếp tục xử lý nạp thẻ...
            Service.gI().sendTopup(0, loaiThe.GetSelectedItem().p.ToString(), seri, mathe, int.Parse(menhGia.GetSelectedItem().p.ToString()));
            // Ở đây có thể gửi request đến server hoặc xử lý nạp thẻ
            lastTopUp = mSystem.currentTimeMillis();
            GameCanvas.startOKDlg("Gửi yêu cầu nạp thẻ thành công!");
        }
        if (currentIndex == 1)
        {
            if (menhGia.GetSelectedIndex() == -1)
            {
                GameCanvas.startOKDlg("Vui lòng chọn mệnh giá muốn nạp");
                return;
            }
            Service.gI().sendTopup(1, menhgia: int.Parse(menhGia.GetSelectedItem().p.ToString()));
            lastTopUp = mSystem.currentTimeMillis();
            // GameCanvas.startOKDlg("Gửi yêu cầu tạo mã QR thành công");
        }
    }
    public List<HistoryTopupItem> historyTopups = new List<HistoryTopupItem>();
    public class HistoryTopupItem
    {
        public int id;
        public int type;
        public byte status;
        public string dateTime;
        public int amount;
        public int realAmount;
        public string seri;
        public string code;
    }
}