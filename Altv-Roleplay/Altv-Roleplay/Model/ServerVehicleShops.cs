using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using Altv_Roleplay.models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Altv_Roleplay.Model
{
    class ServerVehicleShops
    {
        public static List<Server_Vehicle_Shops> ServerVehicleShops_ = new List<Server_Vehicle_Shops>();
        public static List<Server_Vehicle_Shops_Items> ServerVehicleShopsItems_ = new List<Server_Vehicle_Shops_Items>();

        public static string GetVehicleShopItems(int vehShopId)
        {
            if (vehShopId <= 0) return "undefined";

            var items = ServerVehicleShopsItems_.Where(x => x.shopId == vehShopId && x.isOnlyOnlineAvailable == false).Select(x => new
            {
                name = ServerVehicles.GetVehicleNameOnHash(x.hash),
                manufactor = ServerVehicles.GetVehicleManufactorOnHash(x.hash),
                fueltype = ServerVehicles.GetVehicleFuelTypeOnHash(x.hash),
                maxfuel = ServerVehicles.GetVehicleFuelLimitOnHash(x.hash),
                trunkcapacity = ServerVehicles.GetVehicleTrunkCapacityOnHash(x.hash),
                seats = ServerVehicles.GetVehicleMaxSeatsOnHash(x.hash),
                hash = x.hash.ToString(),
                price = x.price,
            }).ToList();

            return JsonConvert.SerializeObject(items);
        }

        public static string GetTabletVehicleStoreItems()
        {
            var items = ServerVehicleShopsItems_.Where(x => x.isOnlyOnlineAvailable == true).Select(x => new
            {
                x.shopId,
                name = ServerVehicles.GetVehicleNameOnHash(x.hash),
                manufactor = ServerVehicles.GetVehicleManufactorOnHash(x.hash),
                fueltype = ServerVehicles.GetVehicleFuelTypeOnHash(x.hash),
                fuellimit = ServerVehicles.GetVehicleFuelLimitOnHash(x.hash),
                storage = ServerVehicles.GetVehicleTrunkCapacityOnHash(x.hash),
                seats = ServerVehicles.GetVehicleMaxSeatsOnHash(x.hash),
                hash = x.hash.ToString(),
                x.price,
            }).OrderBy(x => x.name).ToList();

            return JsonConvert.SerializeObject(items);
        }


        public static void SetVehiclePrice(ulong hash, int price)
        {
            var vehs = ServerVehicleShopsItems_.FirstOrDefault(v => v.hash == hash);
            Alt.Log($"HASH >> {hash}");
            if (vehs != null)
            {
                vehs.price = price;

                Alt.Log($"price >> {price}");

                using (gtaContext db = new gtaContext())
                {
                    db.Server_Vehicle_Shops_Items.Update(vehs);
                    db.SaveChanges();
                Alt.Log($"ReplacePrice >> DONE");
                }
            }
        }

        public static int GetVehicleShopPrice(int shopId, ulong hash)
        {
            return ServerVehicleShopsItems_.FirstOrDefault(x => x.shopId == shopId && x.hash == hash)?.price ?? 999999;
        }
        public static int GetVehicleShopPrice2(long hash)
        {
/*            return ServerVehicleShopsItems_.FirstOrDefault(x => x.hash == hash)?.price ?? 0;
*/            if (hash <= 0) return -1;
            var shopItem = ServerVehicleShopsItems_.FirstOrDefault(x => (long)x.hash == hash);
            if (shopItem == null) return -1;
            return shopItem.price;
        }
        public static int GetVehicleShopId(int shopId)
        {
            return ServerVehicleShopsItems_.FirstOrDefault(x => x.shopId == shopId)?.id ?? 0;
        }

        public static Position GetVehicleShopOutPosition(int shopId)
        {
            Position pos = new Position(0, 0, 0);
            var shop = ServerVehicleShops_.FirstOrDefault(x => x.id == shopId);
            if(shop != null) { pos = new Position(shop.parkOutX, shop.parkOutY, shop.parkOutZ); }
            return pos;
        }
        public static Position GetVehicleShopSellPosition(int shopId)
        {
            Position pos = new Position(0, 0, 0);
            var shop = ServerVehicleShops_.FirstOrDefault(x => x.id == shopId);
            if(shop != null) { pos = new Position(shop.sellX, shop.sellY, shop.sellZ); }
            return pos;
        }

        public static Rotation GetVehicleShopOutRotation(int shopId)
        {
            Rotation rot = new Rotation(0, 0, 0);
            var shop = ServerVehicleShops_.FirstOrDefault(x => x.id == shopId);
            if(shop != null) { rot = new Rotation(shop.parkOutRotX, shop.parkOutRotY, shop.parkOutRotZ); }
            return rot;
        }
    }
}
