using System;
using System.Collections.Generic;
using UnityEngine.Rendering;
public class ModPanelbyNQMP 
{
    public class Mod 
    {
        public string name;
        public bool isStart;
        public Mod(string name)
        {
            this.name = name;
        }
    }
    public static bool isShow;
    public static bool isInit;
    public static List<Mod> mods = new List<Mod>();
    public static void init()
    {
        //mods.Add();
    }
    public static void paint(mGraphics g)
    {
        if (!isInit)
        {
            init();
            isInit = true;
            return;
        }
        PopUp.paintPopUp(g, 50, 30, 400, 400, -1, true);
    }

}