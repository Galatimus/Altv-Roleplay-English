using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using Altv_Roleplay.Handler;
using Altv_Roleplay.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altv_Roleplay.Model
{
    class ServerShops
    {
        public static List<Server_Shops> ServerShops_ = new List<Server_Shops>();

        public static void CreateServerShop(IPlayer client, int shopid, string name, Position pos)
        {
            if (client == null || !client.Exists) return;
            var ServerShopData = new Server_Shops
            {
                shopId = shopid,
                name = name,
                posX = pos.X,
                posY = pos.Y,
                posZ = pos.Z,
                pedX = 0f,
                pedY = 0f,
                pedZ = 0f,
                pedRot = 0f,
                pedModel = "",
                neededLicense = "None",
                isOnlySelling = false
            };

            try
            {
                ServerShops_.Add(ServerShopData);

                using (gtaContext db = new gtaContext())
                {
                    db.Server_Shops.Add(ServerShopData);
                    db.SaveChanges();
                }

                ServerShopsItems.CreateServerShopItem(client, shopid, "Bagel", 20, 5);
                ServerShopsItems.CreateServerShopItem(client, shopid, "BonBon", 20, 2);
                ServerShopsItems.CreateServerShopItem(client, shopid, "Chips", 20, 4);
                ServerShopsItems.CreateServerShopItem(client, shopid, "Donut", 20, 2);
                ServerShopsItems.CreateServerShopItem(client, shopid, "Eis", 20, 1);
                ServerShopsItems.CreateServerShopItem(client, shopid, "HotDog", 20, 5);
                ServerShopsItems.CreateServerShopItem(client, shopid, "Müsli-Riegel", 20, 2);
                ServerShopsItems.CreateServerShopItem(client, shopid, "Sandwich", 20, 4);
                ServerShopsItems.CreateServerShopItem(client, shopid, "Schokolade", 20, 3);
                ServerShopsItems.CreateServerShopItem(client, shopid, "Wrap", 20, 4);


                HUDHandler.SendNotification(client, 2, 5000, $"Shop mit dem Namen ({ServerShopData.name}) an deiner Position erstellt.");
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static int GetShopFaction(int shopId)
        {
            try
            {
                var shop = ServerShops_.ToList().FirstOrDefault(x => x.shopId == shopId);
                if (shop != null) return shop.faction;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return 0;
        }

        public static void SetShopRobbedNow(int id, bool isRobbedNow)
        {
            try
            {
                var shop = ServerShops_.FirstOrDefault(x => x.shopId == id);
                if (shop == null) return;
                shop.isRobbedNow = isRobbedNow;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static bool IsShopRobbedNow(int shopId)
        {
            try
            {
                var shop = ServerShops_.ToList().FirstOrDefault(x => x.shopId == shopId);
                if (shop != null) return shop.isRobbedNow;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return false;
        }

        public static Position GetShopPosition(int shopId)
        {
            if (shopId <= 0) return new Position(0, 0, 0);
            var shop = ServerShops_.FirstOrDefault(x => x.shopId == shopId);
            if (shop == null) return new Position(0, 0, 0);
            return new Position(shop.posX, shop.posY, shop.posZ);
        }

        public static string GetShopNeededLicense(int shopId)
        {
            if (shopId <= 0) return "";
            var shop = ServerShops_.FirstOrDefault(x => x.shopId == shopId);
            if(shop != null)
            {
                return shop.neededLicense;
            }
            return "";
        }

        public static string GetShopName(int shopId)
        {
            if (shopId <= 0) return "";
            var shop = ServerShops_.FirstOrDefault(x => x.shopId == shopId);
            if(shop != null)
            {
                return shop.name;
            }
            return "";
        }
    }
}
