using System.Collections.Generic;

public class ItemCombine
{
    public int IconID;
    public string name;
    public string info;

    public ItemCombine(int iconID, string info, string name)
    {
        this.info = info;
        this.IconID = iconID;
        this.name = name;
    }
    public static void Debug(ItemCombine item)
    {
        UnityEngine.Debug.Log("Icon id : " + item.IconID);
        UnityEngine.Debug.Log("name : " + item.name);
        UnityEngine.Debug.Log("info : " + item.info);
    }

    public static List<ItemCombine> listCombine = new List<ItemCombine>();
    public static void loadItemCombine(Message msg)
    {
        listCombine.Clear();
        if (msg.reader().available() > 0)
        {
            int size = msg.reader().readInt();
            for (int i = 0; i < size; i++)
            {
                int iconId = msg.reader().readInt();
                string name = msg.reader().readUTF();
                string info = msg.reader().readUTF();

                ItemCombine itemCombine = new ItemCombine(iconId, info, name);
                listCombine.Add(itemCombine);
                Debug(itemCombine);

            }
        }
    }

}