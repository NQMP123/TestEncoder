using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LoginRewardPopup : mScreen
{
    private static LoginRewardPopup instance;
    public static LoginRewardPopup gI()
    {
        if (instance == null)
        {
            instance = new LoginRewardPopup();
        }
        return instance;
    }

    private const int POPUP_WIDTH = 400;
    private const int POPUP_HEIGHT = 500;
    private const int ITEM_SIZE = 80;
    private const int ITEM_SPACING = 20;
    private const int DAYS_COUNT = 7;

    private bool isShow;
    private int currentDay;
    private int[] receivedDays;
    private List<Item> rewards;
    private long lastLoginTime;
    private bool canReceiveToday;

    private Image imgExit;
    private int xExit;
    private int yExit;
    private int wExit;
    private int hExit;

    public LoginRewardPopup()
    {
        rewards = new List<Item>();
        receivedDays = new int[DAYS_COUNT];
        LoadRewards();
        LoadProgress();
        LoadImages();
    }

    private void LoadRewards()
    {
        // Day 1
        rewards.Add(new Item
        {
            template = ItemTemplates.get(1), // Gold
            quantity = 1000
        });

        // Day 2
        rewards.Add(new Item
        {
            template = ItemTemplates.get(2), // Diamond
            quantity = 50
        });

        // Day 3
        rewards.Add(new Item
        {
            template = ItemTemplates.get(3), // Energy
            quantity = 100
        });

        // Day 4
        rewards.Add(new Item
        {
            template = ItemTemplates.get(4), // Special Item
            quantity = 1
        });

        // Day 5
        rewards.Add(new Item
        {
            template = ItemTemplates.get(5), // Premium Currency
            quantity = 200
        });

        // Day 6
        rewards.Add(new Item
        {
            template = ItemTemplates.get(6), // Rare Item
            quantity = 1
        });

        // Day 7
        rewards.Add(new Item
        {
            template = ItemTemplates.get(7), // Legendary Item
            quantity = 1
        });
    }

    private void LoadProgress()
    {
        // Load saved progress from Rms
        string savedProgress = Rms.loadRmsString("login_reward_progress");
        if (!string.IsNullOrEmpty(savedProgress))
        {
            string[] parts = savedProgress.Split(',');
            for (int i = 0; i < Math.Min(parts.Length, DAYS_COUNT); i++)
            {
                receivedDays[i] = int.Parse(parts[i]);
            }
        }

        // Load last login time
        string lastLogin = Rms.loadRmsString("last_login_time");
        if (!string.IsNullOrEmpty(lastLogin))
        {
            lastLoginTime = long.Parse(lastLogin);
        }

        CheckCanReceiveToday();
    }

    private void SaveProgress()
    {
        string progress = string.Join(",", receivedDays);
        Rms.saveRmsString("login_reward_progress", progress);
        Rms.saveRmsString("last_login_time", lastLoginTime.ToString());
    }

    private void CheckCanReceiveToday()
    {
        long currentTime = mSystem.currentTimeMillis();
        long timeDiff = currentTime - lastLoginTime;
        long oneDay = 24 * 60 * 60 * 1000; // 24 hours in milliseconds

        canReceiveToday = timeDiff >= oneDay;
    }

    private void LoadImages()
    {
        imgExit = GameCanvas.loadImage("/mainImage/myTexture2dbtX.png");
        wExit = 30;
        hExit = 30;
    }

    public void Show()
    {
        isShow = true;
        CheckCanReceiveToday();
    }

    public void Hide()
    {
        isShow = false;
    }

    public override void paint(mGraphics g)
    {
        if (!isShow)
            return;

        // Cache dimensions
        int screenWidth = GameCanvas.w;
        int screenHeight = GameCanvas.h;
        int popupX = (screenWidth - POPUP_WIDTH) / 2;
        int popupY = (screenHeight - POPUP_HEIGHT) / 2;

        // Draw popup background using PopUp.paintPopUp
        PopUp.paintPopUp(g, popupX, popupY, POPUP_WIDTH, POPUP_HEIGHT, 0, false);

        // Draw title
        mFont.tahoma_7b_white.drawString(g, "ĐĂNG NHẬP NHẬN QUÀ", popupX + POPUP_WIDTH / 2, popupY + 30, mFont.CENTER);
        mFont.tahoma_7_yellow.drawString(g, "Đăng nhập liên tục 7 ngày để nhận quà hấp dẫn", popupX + POPUP_WIDTH / 2, popupY + 50, mFont.CENTER);

        // Draw days in a grid layout
        int startX = popupX + 40;
        int startY = popupY + 100;
        int itemsPerRow = 4;
        int currentRow = 0;
        int currentCol = 0;

        for (int i = 0; i < DAYS_COUNT; i++)
        {
            int x = startX + (currentCol * (ITEM_SIZE + ITEM_SPACING));
            int y = startY + (currentRow * (ITEM_SIZE + ITEM_SPACING));

            // Draw day background using PopUp.paintPopUp
            PopUp.paintPopUp(g, x - 5, y - 5, ITEM_SIZE + 10, ITEM_SIZE + 10, 0, false);

            // Draw day number
            mFont.tahoma_7b_white.drawString(g, "NGÀY " + (i + 1), x + ITEM_SIZE / 2, y - 15, mFont.CENTER);

            // Draw item icon
            if (i < rewards.Count)
            {
                // Draw item background
                g.setColor(0x222222);
                g.fillRect(x + 10, y + 10, ITEM_SIZE - 20, ITEM_SIZE - 20);

                // Draw item icon
                SmallImage.drawSmallImage(g, rewards[i].template.iconID, x + ITEM_SIZE / 2, y + ITEM_SIZE / 2, 0, 0);

                // Draw quantity
                mFont.tahoma_7b_yellow.drawString(g, "x" + rewards[i].quantity, x + ITEM_SIZE / 2, y + ITEM_SIZE - 10, mFont.CENTER);
            }

            // Draw status text
            if (receivedDays[i] == 1)
            {
                mFont.tahoma_7b_green.drawString(g, "ĐÃ NHẬN", x + ITEM_SIZE / 2, y + ITEM_SIZE + 15, mFont.CENTER);
            }
            else if (i == currentDay && canReceiveToday)
            {
                mFont.tahoma_7b_yellow.drawString(g, "NHẬN NGAY", x + ITEM_SIZE / 2, y + ITEM_SIZE + 15, mFont.CENTER);
            }
            else if (i > currentDay)
            {
                mFont.tahoma_7b_yellow.drawString(g, "KHÓA", x + ITEM_SIZE / 2, y + ITEM_SIZE + 15, mFont.CENTER);
            }

            currentCol++;
            if (currentCol >= itemsPerRow)
            {
                currentCol = 0;
                currentRow++;
            }
        }

        // Draw exit button
        xExit = popupX + POPUP_WIDTH - 40;
        yExit = popupY + 20;
        if (imgExit != null)
        {
            g.drawImage(imgExit, xExit, yExit, 0);
        }
        else
        {
            g.setColor(0xFF0000);
            g.fillRect(xExit, yExit, wExit, hExit);
            mFont.tahoma_7b_white.drawString(g, "X", xExit + wExit / 2, yExit + hExit / 2, mFont.CENTER);
        }

        // Draw progress bar
        int progressBarWidth = POPUP_WIDTH - 80;
        int progressBarHeight = 10;
        int progressBarX = popupX + 40;
        int progressBarY = popupY + POPUP_HEIGHT - 50;

        // Draw progress bar background
        g.setColor(0x333333);
        g.fillRect(progressBarX, progressBarY, progressBarWidth, progressBarHeight);

        // Draw progress
        int receivedCount = receivedDays.Count(d => d == 1);
        int progressWidth = (int)(progressBarWidth * (receivedCount / (float)DAYS_COUNT));
        g.setColor(0x00FF00);
        g.fillRect(progressBarX, progressBarY, progressWidth, progressBarHeight);

        // Draw progress text
        mFont.tahoma_7_white.drawString(g, "Tiến độ: " + receivedCount + "/" + DAYS_COUNT, popupX + POPUP_WIDTH / 2, progressBarY + 20, mFont.CENTER);
    }

    public override void update()
    {
        if (!isShow)
            return;

        // Update current day
        long currentTime = mSystem.currentTimeMillis();
        if (currentTime - lastLoginTime >= 24 * 60 * 60 * 1000)
        {
            CheckCanReceiveToday();
        }
    }

    public override void updateKey()
    {
        if (!isShow)
            return;

        if (GameCanvas.isPointerClick)
        {
            // Check exit button
            if (GameCanvas.isPointerHoldIn(xExit, yExit, wExit, hExit))
            {
                Hide();
                return;
            }

            // Check reward items
            int screenWidth = GameCanvas.w;
            int screenHeight = GameCanvas.h;
            int popupX = (screenWidth - POPUP_WIDTH) / 2;
            int popupY = (screenHeight - POPUP_HEIGHT) / 2;
            int startX = popupX + 40;
            int startY = popupY + 100;
            int itemsPerRow = 4;
            int currentRow = 0;
            int currentCol = 0;

            for (int i = 0; i < DAYS_COUNT; i++)
            {
                int x = startX + (currentCol * (ITEM_SIZE + ITEM_SPACING));
                int y = startY + (currentRow * (ITEM_SIZE + ITEM_SPACING));

                if (GameCanvas.isPointerHoldIn(x, y, ITEM_SIZE, ITEM_SIZE))
                {
                    if (i == currentDay && canReceiveToday && receivedDays[i] == 0)
                    {
                        ReceiveReward(i);
                    }
                    break;
                }

                currentCol++;
                if (currentCol >= itemsPerRow)
                {
                    currentCol = 0;
                    currentRow++;
                }
            }
        }
    }

    private void ReceiveReward(int day)
    {
        if (day < 0 || day >= DAYS_COUNT || day >= rewards.Count)
            return;

        // Add reward to inventory
        Item reward = rewards[day];
        Char.myCharz().arrItemBag = addItemToBag(Char.myCharz().arrItemBag, reward);

        // Mark as received
        receivedDays[day] = 1;
        currentDay = (day + 1) % DAYS_COUNT;
        lastLoginTime = mSystem.currentTimeMillis();
        canReceiveToday = false;

        // Save progress
        SaveProgress();

        // Show received message
        GameScr.startFlyText("Nhận quà thành công!", Char.myCharz().cx, Char.myCharz().cy - 50, 0, -1, mFont.RED);
    }

    private Item[] addItemToBag(Item[] bag, Item item)
    {
        // Check if bag is full
        if (bag.Length >= 50)
        {
            GameScr.startFlyText("Túi đồ đã đầy!", Char.myCharz().cx, Char.myCharz().cy - 50, 0, -1, mFont.RED);
            return bag;
        }

        // Add item to bag
        Item[] newBag = new Item[bag.Length + 1];
        Array.Copy(bag, newBag, bag.Length);
        newBag[bag.Length] = item;
        return newBag;
    }
}