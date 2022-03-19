using AltV.Net;
using Altv_Roleplay.models;
using System;
using System.Collections.Generic;
using Altv_Roleplay.Model;
using System.Text;
using System.Linq;

namespace Altv_Roleplay.Model
{
    class ServerItems
    {
        public static List<Server_Items> ServerItems_ = new List<Server_Items>(); 
        public static List<Server_Teleports> ServerTeleports_ = new List<Server_Teleports>();

        public static string ReturnNormalItemName(string itemName)
        {
            try
            {
                var normalName = itemName; 
                if(itemName.Contains("♂")) { normalName = itemName.Replace("♂", "-M-"); }
                else if(itemName.Contains("♀")) { normalName = itemName.Replace("♀", "-W-"); }
                else if(itemName.Contains("Ausweis")) { normalName = "Ausweis"; }
                else if(itemName.Contains("EC Karte")) { normalName = "EC Karte"; }
                else if(itemName.Contains("Fahrzeugschluessel")) { normalName = "Fahrzeugschluessel"; }
                else if(itemName.Contains("Generalschluessel")) { normalName = "Generalschluessel"; }
                return normalName;
            }
            catch(Exception e)
            {
                Alt.Log($"{e}");
            }
            return "";
        }

        public static string ReturnItemPicSRC(string itemName)
        {
            try
            {
                itemName = ReturnNormalItemName(itemName);
                var item = ServerItems_.ToList().FirstOrDefault(i => i.itemName == itemName);
                if (item != null) return item.itemPicSRC;
            }
            catch(Exception e)
            {
                Alt.Log($"{e}");
            }
            return "";
        }

        public static bool ExistItem(string itemName)
        {
            try
            {
                itemName = ReturnNormalItemName(itemName);
                var item = ServerItems_.ToList().FirstOrDefault(i => i.itemName == itemName);
                if (item != null) return true;
            }
            catch(Exception e)
            {
                Alt.Log($"{e}");
            }
            return false;
        }

        public static string GetItemType(string itemName)
        {
            try
            {
                itemName = ReturnNormalItemName(itemName);
                var item = ServerItems_.ToList().FirstOrDefault(i => i.itemName == itemName);
                if (item != null) return item.itemType;
            }
            catch(Exception e)
            {
                Alt.Log($"{e}");
            }
            return "";
        }

        public static string GetItemDescription(string itemName)
        {
            try
            {
                itemName = ReturnNormalItemName(itemName);
                var item = ServerItems_.ToList().FirstOrDefault(i => i.itemName == itemName);
                if (item != null) return item.itemDescription;
            }
            catch(Exception e)
            {
                Alt.Log($"{e}");
            }
            return "";
        }

        public static float GetItemWeight(string itemName)
        {
            try
            {
                itemName = ReturnNormalItemName(itemName);
                var item = ServerItems_.ToList().FirstOrDefault(i => i.itemName == itemName);
                if (item != null) return item.itemWeight;
            }
            catch(Exception e)
            {
                Alt.Log($"{e}");
            }
            return 0f;
        }

        public static bool IsItemDesire(string itemName)
        {
            try
            {
                itemName = ReturnNormalItemName(itemName);
                var item = ServerItems_.ToList().FirstOrDefault(i => i.itemName == itemName);
                if (item != null) return item.isItemDesire;
            }
            catch(Exception e)
            {
                Alt.Log($"{e}");
            }
            return false;
        }

        public static int GetItemDesireFood(string itemName)
        {
            try
            {
                itemName = ReturnNormalItemName(itemName);
                var item = ServerItems_.ToList().FirstOrDefault(i => i.itemName == itemName);
                if (item != null) return item.itemDesireFood;
            }
            catch(Exception e)
            {
                Alt.Log($"{e}");
            }
            return 0;
        }

        public static int GetItemDesireDrink(string itemName)
        {
            try
            {
                itemName = ReturnNormalItemName(itemName);
                var item = ServerItems_.ToList().FirstOrDefault(i => i.itemName == itemName);
                if (item != null) return item.itemDesireDrink;
            }
            catch(Exception e)
            {
                Alt.Log($"{e}");
            }
            return 0;
        }

        public static bool hasItemAnimation(string itemName)
        {
            try
            {
                itemName = ReturnNormalItemName(itemName);
                var item = ServerItems_.ToList().FirstOrDefault(i => i.itemName == itemName);
                if (item != null) return item.hasItemAnimation;
            }
            catch(Exception e)
            {
                Alt.Log($"{e}");
            }
            return false;
        }

        public static string GetItemAnimationName(string itemName)
        {
            try
            {
                itemName = ReturnNormalItemName(itemName);
                var item = ServerItems_.ToList().FirstOrDefault(i => i.itemName == itemName);
                if (item != null) return item.itemAnimationName;
            }
            catch(Exception e)
            {
                Alt.Log($"{e}");
            }
            return "";
        }

        public static bool IsItemDroppable(string itemName)
        {
            try
            {
                itemName = ReturnNormalItemName(itemName);
                var item = ServerItems_.ToList().FirstOrDefault(i => i.itemName == itemName);
                if (item != null) return item.isItemDroppable;
            }
            catch(Exception e)
            {
                Alt.Log($"{e}");
            }
            return false;
        }

        public static bool IsItemGiveable(string itemName)
        {
            try
            {
                itemName = ReturnNormalItemName(itemName);
                var item = ServerItems_.ToList().FirstOrDefault(i => i.itemName == itemName);
                if (item != null) return item.isItemGiveable;
            }
            catch(Exception e)
            {
                Alt.Log($"{e}");
            }
            return false;
        }

        public static bool IsItemUseable(string itemName)
        {
            try
            {
                itemName = ReturnNormalItemName(itemName);
                var item = ServerItems_.ToList().FirstOrDefault(i => i.itemName == itemName);
                if (item != null) return item.isItemUseable;
            }
            catch(Exception e)
            {
                Alt.Log($"{e}");
            }
            return false;
        }

        public static string GetClothesItemType(string itemName)
        {
            try
            {
                itemName = ReturnNormalItemName(itemName);
                var item = ServerItems_.ToList().FirstOrDefault(i => i.itemName == itemName);
                if (item != null) return item.ClothesType;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return "n";
        }
    }
}
