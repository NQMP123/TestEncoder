using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InfoTop
{
    public static string nameTop = "NQMP SIEU CAP VIPPRO";
    public static List<Top> listTop = new List<Top> {
    };

    public static List<int> mapPaint = new List<int>();
    private static double getWidthRect()
    {
        Top top = null;
        int charlenght = 0;
        foreach(Top top2  in listTop) 
        {
            if (top2 != null)
            {
                if (top2.name.Length > charlenght)
                {
                    charlenght = top2.name.Length;
                    top = top2;
                }
            }
        }
        mFont m = top.name.Equals(Char.myCharz().cName) ? mFont.tahoma_7_yellow : mFont.tahoma_7_white;

        return m.getWidth("          " + top.name + "          ") *1.2;
    }
    public static void paint(mGraphics g)
    {
        if (listTop.Count == 0)
        {
            return;
        }
        if (!mapPaint.Contains(TileMap.mapID))
        {
            return;
        }

        try
        {
            int i = 1;
            int y = (int)(GameCanvas.h * 0.3);
            mFont.tahoma_7b_red.drawString(g, nameTop, GameCanvas.getX(20), y - 20, mFont.LEFT);
            foreach (Top top in listTop)
            {
                g.setColor(202020, 0.5f);
                g.fillRect(GameCanvas.getX(15), y, (int)getWidthRect(), 15);
                mFont m = top.name.Equals(Char.myCharz().cName) ? mFont.tahoma_7_yellow : mFont.tahoma_7_white;
                m.drawString(g, i.ToString(), GameCanvas.getX(20), y, mFont.LEFT);
                m.drawString(g, top.name, GameCanvas.getX(70), y, mFont.LEFT);
                m.drawString(g, top.point.ToString(), GameCanvas.getX(70) + m.getWidth(top.name + "          "), y, mFont.LEFT);
                i += 1;
                y += 15;
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }

    }
    public class Top
    {
        public string name;
        public int point;

        public Top()
        {
        }

        public Top(string name, int point)
        {
            this.name = name;
            this.point = point;
        }
    }
}

