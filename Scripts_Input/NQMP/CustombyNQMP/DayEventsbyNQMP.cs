using UnityEngine;
using System;
using System.Collections.Generic;
public class DayEventsbyNQMP : mScreen
{

    public void addDayEvent(DayEvent dayEvent)
    {
        if (dayEvent != null)
        {
            // Kiểm tra xem đã có sự kiện có cùng id hay chưa
            bool exists = false;
            for (int i = 0; i < dayEvents.Count; i++)
            {
                if (dayEvents[i].id == dayEvent.id)
                {
                    // Nếu có rồi thì cập nhật thay vì thêm mới
                    dayEvents[i] = dayEvent;
                    exists = true;
                    break;
                }
            }

            // Nếu chưa có thì thêm mới
            if (!exists)
            {
                dayEvents.Add(dayEvent);
            }

            // Cập nhật lại scroll sau khi thêm/sửa sự kiện
            initScroll();
        }
    }
    private void processEventList(Message msg)
    {
        try
        {
            // Xóa danh sách cũ nếu cần
            // dayEvents.Clear(); // Bỏ comment nếu muốn xóa hoàn toàn danh sách cũ

            // Đọc số lượng sự kiện
            byte count = msg.reader().readUnsignedByte();

            for (int i = 0; i < count; i++)
            {
                // Đọc thông tin sự kiện
                int id = msg.reader().readInt();
                string title = msg.reader().readUTF();
                string description = msg.reader().readUTF();

                // Tạo đối tượng sự kiện mới
                DayEvent dayEvent = new DayEvent(title, description);
                dayEvent.id = id;

                // Thêm vào danh sách
                addDayEvent(dayEvent);
            }

            // Cập nhật lại giao diện
            updateItemScroll();

            // Nếu chưa có tab nào được chọn và có sự kiện, chọn tab đầu tiên
            if (currentTab == 0 && dayEvents.Count > 0)
            {
                currentTab = 0;
            }

            Debug.LogError("Received " + count + " events from server");
        }
        catch (Exception ex)
        {
            Debug.LogError("Error in processEventList: " + ex.ToString());
        }
    }

    private void processEventDetail(Message msg)
    {
        try
        {
            // Đọc ID của sự kiện
            int eventId = msg.reader().readInt();

            // Tìm sự kiện tương ứng
            DayEvent targetEvent = null;
            int eventIndex = -1;

            for (int i = 0; i < dayEvents.Count; i++)
            {
                if (dayEvents[i].id == eventId)
                {
                    targetEvent = dayEvents[i];
                    eventIndex = i;
                    break;
                }
            }

            if (targetEvent == null)
            {
                Debug.LogError("Cannot find event with id: " + eventId);
                return;
            }

            // Đọc số lượng items trong sự kiện
            byte itemCount = msg.reader().readUnsignedByte();
            targetEvent.items = new DayEvent.DayEventItem[itemCount];

            for (int i = 0; i < itemCount; i++)
            {
                // Đọc thông tin của từng item
                int index = msg.reader().readByte();
                string info1 = msg.reader().readUTF();
                string info2 = msg.reader().readUTF();
                bool canReceive = msg.reader().readBoolean();
                bool isReceived = msg.reader().readBoolean();

                // Tạo item mới
                DayEvent.DayEventItem item = new DayEvent.DayEventItem(
                    index, info1, info2, null, canReceive, isReceived);

                // Đọc số lượng phần thưởng
                byte rewardCount = msg.reader().readUnsignedByte();
                if (rewardCount > 0)
                {
                    item.itemList = new Item[rewardCount];

                    for (int j = 0; j < rewardCount; j++)
                    {
                        // Đọc thông tin phần thưởng (ID template và số lượng)
                        short templateId = msg.reader().readShort();
                        int quantity = msg.reader().readInt();

                        // Tạo item phần thưởng và thêm vào danh sách
                        Item reward = new Item();
                        reward.template = ItemTemplates.get(templateId);
                        reward.quantity = quantity;

                        item.itemList[j] = reward;
                    }
                }

                // Thêm item vào sự kiện
                targetEvent.items[i] = item;
            }

            // Cập nhật sự kiện trong danh sách
            dayEvents[eventIndex] = targetEvent;

            // Nếu đang xem sự kiện này, cập nhật lại scroll
            if (currentTab == eventIndex)
            {
                updateItemScroll();
            }

            Debug.LogError("Received details for event id: " + eventId + " with " + itemCount + " items");
        }
        catch (Exception ex)
        {
            Debug.LogError("Error in processEventDetail: " + ex.ToString());
        }
    }
    public void requestMessage(Message msg)
    {
        try
        {
            // Đọc loại message
            byte type = msg.reader().readUnsignedByte();

            switch (type)
            {
                case 0: // Danh sách sự kiện
                    processEventList(msg);
                    break;

                case 1: // Thông tin chi tiết một sự kiện
                    processEventDetail(msg);
                    break;

                case 2: // Kết quả nhận quà
                    processRewardResult(msg);
                    break;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error in DayEventsbyNQMP.requestMessage: " + ex.ToString());
        }
    }
    private void processRewardResult(Message msg)
    {
        try
        {
            // Đọc thông tin kết quả
            byte tabIndex = msg.reader().readUnsignedByte();
            byte itemIndex = msg.reader().readUnsignedByte();
            byte result = msg.reader().readUnsignedByte();

            // Xử lý kết quả
            if (tabIndex < dayEvents.Count && dayEvents[tabIndex].items != null)
            {
                for (int i = 0; i < dayEvents[tabIndex].items.Length; i++)
                {
                    if (dayEvents[tabIndex].items[i] != null && dayEvents[tabIndex].items[i].index == itemIndex)
                    {
                        if (result == 1) // Thành công
                        {
                            dayEvents[tabIndex].items[i].isRecieve = true;
                            dayEvents[tabIndex].items[i].canRecieve = false;
                            GameCanvas.startOKDlg("Nhận quà thành công!");
                        }
                        else // Thất bại
                        {
                            string message = "Nhận quà thất bại!";
                            if (result == 2)
                                message = "Bạn đã nhận quà này rồi!";
                            else if (result == 3)
                                message = "Bạn chưa đủ điều kiện để nhận quà!";

                            GameCanvas.startOKDlg(message);
                        }
                        break;
                    }
                }
            }

            Debug.LogError("Reward result: tab=" + tabIndex + ", item=" + itemIndex + ", result=" + result);
        }
        catch (Exception ex)
        {
            Debug.LogError("Error in processRewardResult: " + ex.ToString());
        }
    }


    int width = 400;
    int height = 250;
    int wTabEvent = 75;
    int space = 2;

    int x => (GameCanvas.w / 2) - width / 2;
    int y => (GameCanvas.h / 2) - height / 2;
    Command btnExit = new Command();
    Scroll scrollTab;
    Scroll itemTab;
    public byte currentTab;
    public static DayEventsbyNQMP instance;
    public DayEventsbyNQMP()
    {
        // Khởi tạo các tab và dữ liệu mẫu
        dayEvents.Add(new DayEvent("Đăng nhập\n7 ngày", "Đăng nhập liên tục trong vòng 7 ngày , ngày nào cũng có quà"));
        dayEvents.Add(new DayEvent("Quà nạp\n7 ngày", "Nạp liên tục 50.000 trong vòng mỗi ngày , ngày nào cũng có quà"));
        dayEvents.Add(new DayEvent("Quà tích nạp", "Tích lũy nạp tiền , nạp tiên càng nhiều , quà càng lớn"));
        dayEvents.Add(new DayEvent("Ưu đãi\nThẻ Tuần", "Tích lũy nạp tiền , nạp tiên càng nhiều , quà càng lớn"));
        dayEvents.Add(new DayEvent("Ưu đãi\nThẻ Tuần 2", "Tích lũy nạp tiền , nạp tiên càng nhiều , quà càng lớn"));

        // Thêm dữ liệu mẫu cho các tab
        dayEvents[0].items = DayEvent.example;
        dayEvents[0].items[0].itemList = DayEvent.DayEventItem.example();
        dayEvents[0].items[1].itemList = DayEvent.DayEventItem.example();

        // Thêm dữ liệu mẫu cho tab 1-4 (để kiểm tra scroll)
        for (int i = 1; i < 5; i++)
        {
            dayEvents[i].items = new DayEvent.DayEventItem[10]; // Tạo nhiều item để test scroll
            for (int j = 0; j < 10; j++)
            {
                dayEvents[i].items[j] = new DayEvent.DayEventItem(
                    j + 1,
                    $"Nhiệm vụ {j + 1}",
                    $"Thông tin thêm {j + 1}",
                    null,
                    j % 3 == 0, // Một số có thể nhận
                    j % 4 == 0  // Một số đã nhận
                );

                if (j < 5) // Chỉ thêm items cho 5 dòng đầu tiên để test
                {
                    dayEvents[i].items[j].itemList = DayEvent.DayEventItem.example();
                }
            }
        }

        // Khởi tạo scroll
        initScroll();

        // Khởi tạo nút exit
        btnExit.img = GameCanvas.loadImage("/mainImage/myTexture2dbtX.png");
        btnExit.x = x + width - btnExit.img.getWidth() - 5;
        btnExit.y = y + btnExit.img.getHeight() / 2;
    }
    public static DayEventsbyNQMP gI()
    {
        instance ??= new DayEventsbyNQMP();

        return instance;
    }
    public void initScroll()
    {
        // Khởi tạo scroll cho tab bên trái
        int hTab = 40;
        int totalTabHeight = (hTab + space) * dayEvents.Count;

        scrollTab = new Scroll();
        scrollTab.setStyle(
            dayEvents.Count,     // Số lượng item
            hTab + space,        // Chiều cao mỗi item
            x,                   // Vị trí x
            y,                   // Vị trí y
            wTabEvent,           // Chiều rộng vùng scroll
            height,              // Chiều cao vùng scroll
            true,                // Sử dụng scrollUpDown
            1                    // Số item trên mỗi dòng
        );

        // Thiết lập giới hạn scroll
        if (totalTabHeight > height)
        {
            scrollTab.cmyLim = totalTabHeight - height;
        }
        else
        {
            scrollTab.cmyLim = 0;
        }

        // Khởi tạo scroll cho phần item bên phải
        updateItemScroll();
    }

    // Cập nhật scroll cho phần items của tab hiện tại
    public void updateItemScroll()
    {
        itemTab = new Scroll();
        if (currentTab < dayEvents.Count && dayEvents[currentTab].items != null)
        {
            int totalItemHeight = dayEvents[currentTab].items.Length * itemEventHeight;

            itemTab.setStyle(
                dayEvents[currentTab].items.Length,  // Số lượng item
                itemEventHeight,                     // Chiều cao mỗi item
                x + wTabEvent + space,               // Vị trí x
                y + 30,                              // Vị trí y 
                width - wTabEvent - space,           // Chiều rộng vùng scroll
                height - 30,                         // Chiều cao vùng scroll
                true,                                // Sử dụng scrollUpDown
                1                                    // Số item trên mỗi dòng
            );

            // Thiết lập giới hạn scroll
            if (totalItemHeight > (height - 30))
            {
                itemTab.cmyLim = totalItemHeight - (height - 30);
            }
            else
            {
                itemTab.cmyLim = 0;
            }
        }
    }
    public override void switchToMe()
    {
        GameScr.isPaintOther = true;
        base.switchToMe();
    }
    public override void paint(mGraphics g)
    {
        GameScr.gI().paint(g);
        PopUp.paintPopUp(g, x, y, width, height, 0, true);

        paintEvents(g);
        paintTabEvent(g);
        btnExit.paint(g);
    }
    public override void updateKey()
    {
        bool isReset = false;
        base.updateKey();

        // Cập nhật scroll cho tab
        ScrollResult tabResult = scrollTab.updateKey();
        if (tabResult.isFinish && tabResult.selected >= 0 && tabResult.selected < dayEvents.Count)
        {
            if (currentTab != tabResult.selected)
            {
                currentTab = (byte)tabResult.selected;
                updateItemScroll(); // Cập nhật lại scroll cho items
            }
            isReset = true;
        }

        // Cập nhật scroll cho items

        itemTab?.updateKey();

        if (GameCanvas.isPointerHoldIn(x + wTabEvent + space, y, width - wTabEvent - space, height))
        {
            if (GameCanvas.isPointerClick && GameCanvas.isPointerJustRelease)
            {
                int hTab = 40;
                int yStart = y - scrollTab.cmy;
                for (int i = 0; i < dayEvents.Count; i++)
                {
                    var abc = dayEvents[i];
                    if (abc  != null)
                    {
                        int yTab = yStart + (hTab + space) * i;
                        {
                            if (GameCanvas.isPointerHoldIn(x, yTab, wTabEvent, hTab))
                            {
                                currentTab = (byte)i;
                                isReset = true;
                                break;
                            }

                        }
                    }
                }
            }
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
    public override void update()
    {
        GameScr.gI().update();
        scrollTab?.updatecm();
        itemTab?.updatecm();
        var events = dayEvents[currentTab];
        if (events != null && events.items != null)
        {
            for (int i = 0; i < events.items.Length; i++)
            {
                var e = events.items[i];
                if (e != null && e.btnRecieve != null)
                {
                    try
                    {
                        if (e.btnRecieve.isPointerPressInside())
                        {
                            Debug.LogError("sendEvent : " + currentTab + " - " + (byte)e.index);
                            Service.gI().sendEvent(currentTab, (byte)e.index);
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex.ToString());
                    }
                }
            }
        }
    }



    public void paintEvents(mGraphics g)
    {
        // Vẽ khung ngoài
        PopUp.paintPopUp(g, x, y, wTabEvent, height, 0, true);
        PopUp.paintPopUp(g, x + wTabEvent + space, y, width - wTabEvent - space, height, 0, true);

        // Thiết lập vùng cắt cho scroll tab bên trái
        g.setClip(x, y, wTabEvent, height);

        int hTab = 40;
        int yStart = y - scrollTab.cmy;

        // Vẽ từng tab
        for (int i = 0; i < dayEvents.Count; i++)
        {
            var events = dayEvents[i];
            if (events != null)
            {
                int yTab = yStart + (hTab + space) * i;

                // Chỉ vẽ tab nếu nó nằm trong vùng hiển thị
                if (yTab + hTab >= y && yTab <= y + height)
                {
                    PopUp.paintPopUp(g, x, yTab, wTabEvent, hTab, currentTab == i ? 1 : 0, true);
                    mFont.tahoma_7b_dark.drawString(g, events.title, x + wTabEvent / 2, yTab + 7, mFont.CENTER);
                }
            }
        }

        // Bỏ clip
        g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
    }
    public int itemEventHeight = 65;
    public void paintTabEvent(mGraphics g)
    {
        var events = dayEvents[currentTab];
        if (events != null)
        {
            // Vẽ phần mô tả ở trên đầu
            mFont.tahoma_7b_dark.drawString(g, events.description, x + wTabEvent + space + ((width - wTabEvent - space) / 2), y + 10, mFont.CENTER);

            // Vẽ khung cho phần danh sách items
            PopUp.paintPopUp(g, x + wTabEvent + space, y + 30, width - wTabEvent - space, height - 30, 0, true);

            if (events.items != null && events.items.Length > 0)
            {
                // Thiết lập vùng cắt cho phần danh sách items
                g.setClip(x + wTabEvent + space, y + 30, width - wTabEvent - space, height - 30);

                // Tính vị trí bắt đầu vẽ
                int yStart = y + 30 - itemTab.cmy;

                // Vẽ từng item
                for (int i = 0; i < events.items.Length; i++)
                {
                    var item = events.items[i];
                    if (item != null)
                    {
                        int yItem = yStart + i * itemEventHeight;

                        // Chỉ vẽ item nếu nó nằm trong vùng hiển thị
                        if (yItem + itemEventHeight >= y + 30 && yItem <= y + height)
                        {
                            PopUp.paintPopUp(g, x + wTabEvent + space, yItem, width - wTabEvent - space, itemEventHeight, 0, true);
                            mFont.tahoma_7b_dark.drawString(g, item.info1, x + wTabEvent + space + 5, yItem + 5, mFont.LEFT);
                            mFont.tahoma_7.drawString(g, item.info2, x + wTabEvent + space + 5, yItem + 15, mFont.LEFT);

                            if (item.isRecieve)
                            {
                                mFont.tahoma_7_green.drawString(g, "Đã nhận", x + width - 10, yItem + 5, mFont.RIGHT);
                            }
                            else if (!item.canRecieve)
                            {
                                mFont.tahoma_7_red.drawString(g, "Chưa đủ điều kiện", x + width - 10, yItem + 5, mFont.RIGHT);
                            }
                            else
                            {
                                item.btnRecieve.x = (x + width - 10) - item.btnRecieve.w;
                                item.btnRecieve.y = yItem + 5;
                                item.btnRecieve.paint(g);
                            }

                            try
                            {
                                if (item.itemList != null)
                                {
                                    int xItem = x + wTabEvent + space + 5;
                                    foreach (var _item in item.itemList)
                                    {
                                        if (_item == null) continue;

                                        g.setColor(11837316, 1.5f);
                                        g.fillRect(xItem, yItem + 30, 30, 30, 4);

                                        g.setColor(9993045, 0.8f);
                                        g.fillRect(xItem + 2, yItem + 32, 26, 26, 4);

                                        SmallImage.drawSmallImage(g, _item.template.iconID, xItem + 15, yItem + 45, 0, mGraphics.VCENTER | mGraphics.HCENTER);
                                        if (_item.quantity > 1)
                                        {
                                            mFont.tahoma_7_yellow.drawString(g, NinjaUtil.getMoneys(_item.quantity), xItem + 30, yItem + 30 - mFont.tahoma_7b_yellow.getHeight(), mFont.RIGHT, mFont.tahoma_7b_dark);
                                        }

                                        xItem += 37;
                                    }
                                }
                            }
                            catch (Exception e) { Debug.LogError(e); }
                        }
                    }
                }

                // Bỏ clip
                g.setClip(0, 0, GameCanvas.w, GameCanvas.h);

                // Hiển thị chỉ dẫn cuộn nếu cần
                if (itemTab.cmyLim > 0)
                {
                    mFont.tahoma_7_grey.drawString(g, "↑ Vuốt để cuộn ↓",
                        x + wTabEvent + space + (width - wTabEvent - space) / 2,
                        y + height - 15, mFont.CENTER);
                }
            }
            else
            {
                mFont.tahoma_7b_dark.drawString(g, "Không có sự kiện nào",
                    x + wTabEvent + space + (width - wTabEvent - space) / 2,
                    y + 30 + (height - 30) / 2, mFont.CENTER);
            }
        }
    }


    public List<DayEvent> dayEvents = new List<DayEvent>();


    public class DayEvent
    {
        public int id;
        public string title;
        public string description;
        public DayEventItem[] items;
        public DayEvent(string title, string description = "", DayEventItem[] items = null)
        {
            this.title = title;
            this.items = items;
            this.description = description;
        }

        public static DayEventItem[] example = new DayEventItem[7]
        {
            new DayEventItem(1,"Đăng nhập 1 ngày","Đã đăng nhập 2 ngày",null,false,true),
            new DayEventItem(2,"Đăng nhập 2 ngày","Đã đăng nhập 2 ngày",null,true,false),
            new DayEventItem(3,"Đăng nhập 3 ngày","Đã đăng nhập 2 ngày",null,false,false),
            new DayEventItem(4,"Đăng nhập 4 ngày", "Đã đăng nhập 2 ngày", null, false, false),
            new DayEventItem(5,"Đăng nhập 5 ngày","Đã đăng nhập 2 ngày",null,false,false),
            new DayEventItem(6,"Đăng nhập 6 ngày","Đã đăng nhập 2 ngày",null,false,false),
            new DayEventItem(7,"Đăng nhập 7 ngày","Đã đăng nhập 2 ngày",null,false,false)
        };
        public class DayEventItem
        {

            public static Item[] example()
            {
                return new Item[5]
                {
                    Char.myCharz().arrItemBag[0],
                    Char.myCharz().arrItemBag[1],
                    Char.myCharz().arrItemBag[2],
                    Char.myCharz().arrItemBag[3],
                    Char.myCharz().arrItemBag[4],
                };
            }
            public int index;
            public string info1;
            public string info2;
            public Item[] itemList;
            public bool canRecieve;
            public bool isRecieve;
            public Command btnRecieve = new Command("Nhận quà", -1);
            public DayEventItem()
            {

            }
            public DayEventItem(int index, string info1, string info2, Item[] itemList, bool canReceive, bool isRecieve)
            {
                this.index = index;
                this.info1 = info1;
                this.info2 = info2;
                this.itemList = itemList;
                this.canRecieve = canReceive;
                this.isRecieve = isRecieve;
                btnRecieve.p = this;
            }
        }
    }

}