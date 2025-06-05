//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace RongThanCustom
//{
//    public static class RongThanFactory
//    {
//        public static bool FindItem(int templateID)
//        {
//            for (int i = 0; i < global::Char.myCharz().arrItemBag.Length; i++)
//            {
//                Item item = global::Char.myCharz().arrItemBag[i];
//                bool flag = item != null && (int)item.template.id == templateID;
//                if (flag)
//                {
//                    return true;
//                }
//            }
//            return false;
//        }
//        public static void teleportMyChar(IMapObject obj)
//        {
//            teleportMyChar(obj.getX(), obj.getY());
//        }
//        public static void teleportMyChar(int x, int y)
//        {
//            Char.myCharz().currentMovePoint = null;
//            Char.myCharz().cx = x;
//            Char.myCharz().cy = y;
//            Service.gI().charMove();

//            if (isUsingTDLT())
//                return;

//            Char.myCharz().cx = x;
//            Char.myCharz().cy = y + 1;
//            Service.gI().charMove();
//            Char.myCharz().cx = x;
//            Char.myCharz().cy = y;
//            Service.gI().charMove();
//        }
//        public static readonly short ID_ICON_ITEM_TDLT = 4387;
//        public static bool isUsingTDLT() =>
//          ItemTime.isExistItem(ID_ICON_ITEM_TDLT);
//        public static Char findCharInMap(string name)
//        {
//            for (int i = 0; i < GameScr.vCharInMap.size(); i++)
//            {
//                Char @char = (Char)GameScr.vCharInMap.elementAt(i);
//                if (@char.getNameWithoutClanTag() == name)
//                    return @char;
//            }
//            return null;
//        }
//        public static bool isBoss(this Char @char)
//        {
//            return !@char.IsPet() && @char.cName != "Trọng tài" && char.IsUpper(getNameWithoutClanTag(@char)[0]);
//        }

//        public static bool isPet(this Char @char)
//        {
//            return @char.IsPet() || @char.IsMiniPet() || @char.cName.StartsWith("#") || @char.cName.StartsWith("$");
//        }
//        public static string getNameWithoutClanTag(this Char @char, bool enableRichText = false)
//        {
//            string name = @char.cName.Remove(0, @char.cName.IndexOf(']') + 1).TrimStart(' ', '#', '$');
//            if (enableRichText)
//            {
//                if (isPet(@char))
//                    name = $"<color=cyan>{name}</color>";
//                else if (isBoss(@char))
//                    name = $"<color=red><size={7 * mGraphics.zoomLevel}>{name}</size></color>";
//                else
//                    name = $"<color=yellow>{name}</color>";
//            }
//            return name;
//        }
//        public static void UseItem(int templateId)
//        {
//            for (int i = 0; i < global::Char.myCharz().arrItemBag.Length; i++)
//            {
//                Item item = global::Char.myCharz().arrItemBag[i];
//                bool flag = item != null && (int)item.template.id == templateId;
//                if (flag)
//                {
//                    Service.gI().useItem(0, 1, (sbyte)item.indexUI, -1);
//                    break;
//                }
//            }
//        }
//    }
//}
