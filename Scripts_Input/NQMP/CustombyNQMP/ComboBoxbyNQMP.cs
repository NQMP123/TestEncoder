using System;
using System.Collections.Generic;
using UnityEngine;

public class ComboBoxbyNQMP
{
    public class ItemComboBox
    {
        public string captions;
        public object p;
        public ItemComboBox(string caption, object p = null)
        {
            this.captions = caption;
            this.p = p;
        }

        public override string ToString()
        {
            return captions;
        }
    }
    // Constants
    private const int ITEM_HEIGHT = 20;
    private const int PADDING = 5;
    int maxHeight => height + (Math.Min(maxVisibleItems, items.Count) * ITEM_HEIGHT);

    // Animation Constants
    private const float ANIMATION_SPEED = 5f; // Animation speed (higher = faster)
    private const float ANIMATION_DELAY = 0.5f; // Delay before closing (seconds)

    // Properties
    private bool isDropdownOpen = false;
    private bool isAnimating = false;
    private float animationProgress = 0f; // Animation progress (0 = closed, 1 = fully open)
    private float animationTimer = 0f; // Timer for animation
    private bool pendingClose = false; // Flag to mark pending close request

    private int selectedIndex = -1;
    private ItemComboBox selectedItem = null;
    private List<ItemComboBox> items = new List<ItemComboBox>();

    // UI Properties
    private int x, y, width, height;
    private int maxVisibleItems = 5;
    private bool wasTouched = false;

    // Scroll properties
    private Scroll scroll = new Scroll();
    private bool hasScrollInitialized = false;

    // Styling
    private string captions;

    public bool isShow = false;
    public Action action = null;
    /// <summary>
    /// Constructor for ComboBox
    /// </summary>
    public ComboBoxbyNQMP(int x, int y, int width, int height, int maxVisibleItems = 5, string captions = "Select value")
    {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
        this.maxVisibleItems = maxVisibleItems;
        this.captions = captions;
    }

    /// <summary>
    /// Add an item to ComboBox
    /// </summary>
    public void AddItem(ItemComboBox item)
    {
        if (item != null)
        {
            items.Add(item);
            UpdateScrollConfig();
        }
    }

    /// <summary>
    /// Add an item to ComboBox from caption
    /// </summary>
    public void AddItem(string caption,object p = null)
    {
        ItemComboBox item = new ItemComboBox(caption);
        item.p = p;
        AddItem(item);
    }

    /// <summary>
    /// Clear all items
    /// </summary>
    public void ClearItems()
    {
        items.Clear();
        selectedIndex = -1;
        selectedItem = null;
        hasScrollInitialized = false;
    }

    /// <summary>
    /// Update scroll configuration based on items count
    /// </summary>
    private void UpdateScrollConfig()
    {
        if (items.Count > maxVisibleItems)
        {
            // Initialize scroll when needed
            int dropdownHeight = maxVisibleItems * ITEM_HEIGHT;
            scroll.setStyle(
                items.Count,        // Number of items
                ITEM_HEIGHT,        // Item height
                x,                  // x position
                y + height,         // y position (below the main ComboBox)
                width,              // width
                dropdownHeight,     // height (visible items only)
                true,               // styleUPDOWN
                1                   // ITEM_PER_LINE
            );
            hasScrollInitialized = true;
        }
    }

    /// <summary>
    /// Update ComboBox state each frame
    /// </summary>
    public void update()
    {
        // Update animation
        UpdateAnimation(Time.deltaTime);

        // If scroll is initialized and dropdown is open, update scroll position
        if (hasScrollInitialized && (isDropdownOpen || isAnimating) && animationProgress > 0)
        {
            scroll.updatecm();
        }

        // Reset touch state for next frame
        wasTouched = false;
    }

    /// <summary>
    /// Update animation state
    /// </summary>
    private void UpdateAnimation(float deltaTime)
    {
        if (isAnimating)
        {
            if (isDropdownOpen)
            {
                // Opening dropdown
                animationProgress += deltaTime * ANIMATION_SPEED;
                if (animationProgress >= 1f)
                {
                    animationProgress = 1f;
                    isAnimating = false;
                }
            }
            else
            {
                if (pendingClose)
                {
                    // Wait before beginning close animation
                    animationTimer += deltaTime;
                    if (animationTimer >= ANIMATION_DELAY)
                    {
                        pendingClose = false;
                        animationTimer = 0f;
                    }
                }
                else
                {
                    // Closing dropdown
                    animationProgress -= deltaTime * ANIMATION_SPEED;
                    if (animationProgress <= 0f)
                    {
                        animationProgress = 0f;
                        isAnimating = false;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Open dropdown with animation
    /// </summary>
    private void OpenDropdown()
    {
        isDropdownOpen = true;
        isAnimating = true;
        pendingClose = false;
        animationTimer = 0f;
        // If closing, continue from current position
        // If fully closed, start from 0
        if (animationProgress <= 0f)
        {
            animationProgress = 0.01f;
        }
    }

    /// <summary>
    /// Close dropdown with animation
    /// </summary>
    private void CloseDropdown()
    {
        isDropdownOpen = false;
        isAnimating = true;
        pendingClose = true;
        animationTimer = 0f;
    }

    /// <summary>
    /// Handle keyboard/touch input for ComboBox
    /// </summary>
    public void updateKey()
    {
        bool isReset = false;

        // Handle main ComboBox button click
        if (GameCanvas.isPointerHoldIn(x, y, width, height))
        {
            if (!isDropdownOpen && GameCanvas.isPointerClick)
            {
                OpenDropdown();
                isReset = true;
            }
        }

        // Handle scroll and item selection when dropdown is open
        if ((isDropdownOpen || isAnimating) && animationProgress > 0)
        {
            // If scroll is initialized (items > maxVisibleItems), handle scroll input
            if (hasScrollInitialized)
            {
                ScrollResult result = scroll.updateKey();
                wasTouched = result.isDowning;

                // Handle item selection from scroll
                if (result.isFinish && result.selected >= 0 && result.selected < items.Count)
                {
                    SetSelectedIndex(result.selected);
                    CloseDropdown();
                    isReset = true;
                }
            }
            else
            {
                // No scroll needed, handle direct item selection
                int dropdownHeight = items.Count * ITEM_HEIGHT;
                if (GameCanvas.isPointerHoldIn(x, y + height, width, dropdownHeight))
                {
                    if (GameCanvas.isPointerClick)
                    {
                        int index = (GameCanvas.py - (y + height)) / ITEM_HEIGHT;
                        if (index >= 0 && index < items.Count)
                        {
                            SetSelectedIndex(index);
                            CloseDropdown();
                            isReset = true;
                        }
                    }
                }
            }

            // Close dropdown when clicking outside
            if (!GameCanvas.isPointerHoldIn(x, y, width, maxHeight) &&
                GameCanvas.isPointerClick)
            {
                CloseDropdown();
                isReset = true;
            }
        }

        if (isReset)
        {
            GameCanvas.clearAllPointerEvent();
        }
    }

    /// <summary>
    /// Draw ComboBox on screen
    /// </summary>
    public void paint(mGraphics g)
    {
        // Draw ComboBox main area
        g.setClip(x, y, width, height);
        PopUp.paintPopUp(g, x, y, width, height, 0, true);
        string displayText = selectedItem != null ? selectedItem.captions : this.captions;
        mFont.tahoma_7b_dark.drawString(g, displayText, x + PADDING, (y + height / 2) - 5, mFont.LEFT);

        // Draw dropdown arrow (rotated when open)
        if (!isDropdownOpen && animationProgress <= 0)
            g.drawRegion(Mob.imgHP, 0, 6, 9, 6, 0, (x + width) - (Mob.imgHP.getWidth() + 3), y + height / 2, 3);
        else
            g.drawRegion(Mob.imgHP, 0, 0, 9, 6, 6, (x + width) - (Mob.imgHP.getWidth() + 3), y + height / 2, 3);

        // If dropdown is animating or fully open, draw items
        if ((isDropdownOpen || isAnimating) && items.Count > 0 && animationProgress > 0)
        {
            int visibleItems = Math.Min(items.Count, maxVisibleItems);
            int fullDropdownHeight = visibleItems * ITEM_HEIGHT;
            int currentDropdownHeight = Mathf.RoundToInt(fullDropdownHeight * animationProgress);

            if (currentDropdownHeight <= 0)
            {
                g.reset();
                return;
            }

            // Set clip for dropdown
            g.setClip(x, y + height, width, currentDropdownHeight);

            // Draw dropdown background
            PopUp.paintPopUp(g, x, y + height, width, currentDropdownHeight, -1, true);

            // Draw items with scroll offset
            int startY = y + height;
            int scrollOffset = hasScrollInitialized ? scroll.cmy : 0;

            // Calculate first visible item based on scroll position
            int firstVisibleItem = scrollOffset / ITEM_HEIGHT;
            int maxDrawItems = Math.Min(visibleItems + 1, items.Count - firstVisibleItem);

            // Draw visible items
            for (int i = 0; i < maxDrawItems; i++)
            {
                int itemIndex = firstVisibleItem + i;
                if (itemIndex < 0 || itemIndex >= items.Count) continue;

                int itemY = startY + (i * ITEM_HEIGHT) - (scrollOffset % ITEM_HEIGHT);
                if (itemY + ITEM_HEIGHT < startY || itemY > startY + currentDropdownHeight) continue;

                // Draw item background
                PopUp.paintPopUp(g, x, itemY, width, ITEM_HEIGHT, selectedIndex == itemIndex ? 1 : 0, true);

                // Draw item text
                mFont font = (selectedIndex != itemIndex) ? mFont.tahoma_7 : mFont.tahoma_7b_dark;
                font.drawString(g, items[itemIndex].captions, x + PADDING + 3, itemY + ITEM_HEIGHT / 2 - 5, mFont.LEFT);
            }
        }

        g.reset();
    }

    /// <summary>
    /// Set selected item by index
    /// </summary>
    public void SetSelectedIndex(int index)
    {
        if (index >= 0 && index < items.Count)
        {
            selectedIndex = index;
            selectedItem = items[index];
            action?.Invoke();
        }
    }

    /// <summary>
    /// Set selected item
    /// </summary>
    public void SetSelectedItem(ItemComboBox item)
    {
        selectedIndex = items.IndexOf(item);
        if (selectedIndex >= 0)
        {
            selectedItem = item;
        }
    }

    /// <summary>
    /// Get current selected index
    /// </summary>
    public int GetSelectedIndex()
    {
        return selectedIndex;
    }

    /// <summary>
    /// Get current selected item
    /// </summary>
    public ItemComboBox GetSelectedItem()
    {
        return selectedItem;
    }

    /// <summary>
    /// Check if dropdown is open
    /// </summary>
    public bool IsDropdownOpen()
    {
        return isDropdownOpen;
    }

    /// <summary>
    /// Set position and size of ComboBox
    /// </summary>
    public void SetBounds(int x, int y, int width, int height)
    {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;

        // Update scroll configuration if needed
        if (hasScrollInitialized)
        {
            UpdateScrollConfig();
        }
    }

    /// <summary>
    /// Set maximum visible items in dropdown
    /// </summary>
    public void SetMaxVisibleItems(int max)
    {
        this.maxVisibleItems = max > 0 ? max : 1;

        // Update scroll configuration if needed
        if (items.Count > 0)
        {
            UpdateScrollConfig();
        }
    }
}