using System;

internal class ComboBox
{
    Action[] commandAction;
    string[] commandList;
    public bool openBox;
    public bool focus;
    public int x, w, h;
    public int[] y;
    public int length;
    int selected = 0;

    internal ComboBox(Action[] commandAction, params string[] commandList)
    {
        length = commandList.Length;
        y = new int[length];
        this.commandAction = commandAction;
        this.commandList = commandList;
    }
    public void update()
    {
        if (this.isPressesInside())
        {
            this.Invoke();
        }
    }

    public void paint(mGraphics g)
    {
        mFont f = selected == 0 ? mFont.tahoma_7b_dark : mFont.tahoma_7b_green2;
        PopUp.paintPopUp(g, x, y[0], w, h, selected == 0 && focus ? 1 : 0, true);
        f.drawString(g, commandList[0], x + w / 2, y[0] + h / 2 - f.getHeight() / 2, 3);
        g.drawRegion(Mob.imgHP, 0, 6, 9, 6, openBox ? 5 : 0, x + w - 10, y[0] + h / 2, 3);
        if (openBox)
        {
            for (int i = 1; i < length; i++)
            {
                f = i == selected ? mFont.tahoma_7b_dark : mFont.tahoma_7b_green2;
                PopUp.paintPopUp(g, x, y[i], w, h, i == selected && focus ? 1 : 0, true);
                f.drawString(g, commandList[i], x + w / 2, y[i] + h / 2 - f.getHeight() / 2, 3);
            }
        }
    }

    public bool isPressesInside()
    {
        focus = false;
        for (int i = 0; i < y.Length; i++)
        {
            if (!openBox && i > 0)
                continue;

            if (GameCanvas.isPointerHoldIn(x, y[i], w, h))
            {
                selected = i;
                if (GameCanvas.isPointerDown)
                {
                    focus = true;
                }
                if (GameCanvas.isPointerClick && GameCanvas.isPointerJustRelease)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void Invoke()
    {
        //    GameCanvas.clearAllPointerEvent();
        //GameCanvas.isPointerDown = (GameCanvas.isPointerJustRelease = (GameCanvas.isPointerClick = false));
        if (selected == 0)
        {
            openBox = !openBox;
        }
        else if (openBox)
        {
            commandAction[selected]?.Invoke();
            if (selected > 0)
            {
                string tempCommand = commandList[0];
                commandList[0] = commandList[selected];
                commandList[selected] = tempCommand;
                Action tempAction = commandAction[0];
                commandAction[0] = commandAction[selected];
                commandAction[selected] = tempAction;
            }

            openBox = false;
            selected = 0;
        }
    }
}