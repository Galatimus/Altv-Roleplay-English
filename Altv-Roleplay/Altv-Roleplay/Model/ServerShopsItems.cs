using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using Altv_Roleplay.Handler;
using Altv_Roleplay.models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altv_Roleplay.Model
{
    class ServerShopsItems
    {
        public static List<Server_Shops_Items> ServerShopsItems_ = new List<Server_Shops_Items>();

        public static void CreateServerShopItem(IPlayer client, int shopId, string itemName, int itemAmount, int itemPrice)
        {
            if (client == null || !client.Exists) return;
            var ServerShopItemData = new Server_Shops_Items
            {
                shopId = shopId,
                itemName = itemName,
                itemAmount = itemAmount,
                itemPrice = itemPrice,
                itemGender = 2
            };

            try
            {
                ServerShopsItems_.Add(ServerShopItemData);
                using (gtaContext db = new gtaContext())
                {
                    db.Server_Shops_Items.Add(ServerShopItemData);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static string GetShopShopItems(int shopId)
        {
            if (shopId == 0) return "";

            var items = ServerShopsItems_.Where(x => x.shopId == shopId).Select(x => new
            {
                itemname = x.itemName,
                itemprice = x.itemPrice,
                itempic = ServerItems.ReturnItemPicSRC(x.itemName),
                itemmaxamount = x.itemAmount,
            }).ToList();

            return JsonConvert.SerializeObject(items);
        }

        public static string GetShopSellItems(int charId, int shopId)
        {
            if (charId == 0 || shopId == 0) return "";
            var items = ServerShopsItems_.Where(x => x.shopId == shopId && (CharactersInventory.ExistCharacterItem(charId, x.itemName, "inventory") == true || CharactersInventory.ExistCharacterItem(charId, x.itemName, "backpack") == true)).Select(x => new
            {
                itemname = x.itemName,
                itemprice = x.itemPrice,
                itempic = ServerItems.ReturnItemPicSRC(x.itemName),
                itemmaxamount = CharactersInventory.GetCharacterItemAmount(charId, x.itemName, "inventory") + CharactersInventory.GetCharacterItemAmount(charId, x.itemName, "backpack"),
            }).ToList();

            return JsonConvert.SerializeObject(items);
        } 

        public static void RemoveShopItemAmount(int shopId, string itemName, int itemAmount)
        {
            if (shopId == 0 || itemName == "" || itemAmount == 0) return;
            var shopItem = ServerShopsItems_.FirstOrDefault(x => x.shopId == shopId && x.itemName == itemName);
            if (shopItem == null) return;
            int prevAmount = shopItem.itemAmount;
            shopItem.itemAmount -= itemAmount;
            using (gtaContext db = new gtaContext())
            {
                if (shopItem.itemAmount > 0)
                {
                    db.Server_Shops_Items.Update(shopItem);
                    db.SaveChanges();
                }
                else RemoveShopItem(shopId, itemName);
            }
        }

        public static void RemoveShopItem(int shopId, string itemName)
        {
            var shopItem = ServerShopsItems_.FirstOrDefault(i => i.shopId == shopId && i.itemName == itemName);
            if (shopItem == null) return;
            ServerShopsItems_.Remove(shopItem);
            using (gtaContext db = new gtaContext())
            {
                db.Server_Shops_Items.Remove(shopItem);
                db.SaveChanges();
            }
        }

        public static int GetShopItemPrice(int shopId, string itemname)
        {
            if (shopId <= 0 || itemname == "") return -1;
            var shopItem = ServerShopsItems_.FirstOrDefault(x => x.shopId == shopId && x.itemName == itemname);
            if (shopItem == null) return -1;
            return shopItem.itemPrice;
        }

        public static int GetShopItemAmount(int shopId, string itemname)
        {
            if (shopId <= 0 || itemname == "") return -1;
            var shopItem = ServerShopsItems_.FirstOrDefault(x => x.shopId == shopId && x.itemName == itemname);
            if (shopItem == null) return -1;
            return shopItem.itemAmount;
        }
    }
}
