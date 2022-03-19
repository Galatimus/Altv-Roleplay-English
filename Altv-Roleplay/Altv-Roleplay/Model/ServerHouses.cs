using AltV.Net;
using AltV.Net.Data;
using Altv_Roleplay.models;
using System;
using System.Collections.Generic;
using System.Text;
using Altv_Roleplay.Utils;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Altv_Roleplay.Factories;

namespace Altv_Roleplay.Model
{
    class ServerHouses
    {
        public static List<Server_Houses_Interiors> ServerHousesInteriors_ = new List<Server_Houses_Interiors>();
        public static List<Server_Houses> ServerHouses_ = new List<Server_Houses>();
        public static List<Server_Houses_Storage> ServerHousesStorage_ = new List<Server_Houses_Storage>();
        public static List<Server_Houses_Renter> ServerHousesRenter_ = new List<Server_Houses_Renter>();

        internal static void CreateHouse(int id, int interiorId, int ownerId, string street, int price, int maxRenters, int rentPrice, bool isRentable, bool hasStorage, bool hasAlarm, bool hasBank, Position entrancePos, int money)
        {
            try
            {
                var houseData = new Server_Houses
                {
                    id = id,
                    interiorId = interiorId,
                    ownerId = ownerId,
                    street = street,
                    price = price,
                    maxRenters = maxRenters,
                    rentPrice = rentPrice,
                    isRentable = isRentable,
                    hasStorage = hasStorage,
                    hasAlarm = hasAlarm,
                    hasBank = hasBank,
                    entranceX = entrancePos.X,
                    entranceY = entrancePos.Y,
                    entranceZ = entrancePos.Z,
                    money = money,
                    isLocked = true,
                    entranceShape = Alt.CreateColShapeSphere(entrancePos, 2f)
                };
                houseData.entranceShape.SetColShapeId((ulong)id);
                ((ClassicColshape)houseData.entranceShape).Radius = 2f;
                ServerHouses_.Add(houseData);

                var markerData = new Server_Markers
                {
                    type = 0,
                    bobUpAndDown = true,
                    scaleX = 1,
                    scaleY = 1,
                    scaleZ = 1,
                    alpha = 50,
                    posX = entrancePos.X,
                    posY = entrancePos.Y,
                    posZ = entrancePos.Z + 0.15f,
                    red = 242,
                    green = 58,
                    blue = 58
                };
                ServerBlips.ServerMarkers_.Add(markerData);
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static string GetAllCharacterHouses(int charId)
        {
            if (charId <= 0) return "[]";
            var items = ServerHouses_.Where(x => x.ownerId == charId).Select(x => new
            {
                x.id,
                x.street,
                x.price,
            }).ToList();
            return JsonConvert.SerializeObject(items);
        }

        public static string GetHouseInformationArray(int houseId)
        {
            if (houseId <= 0) return "[]";
            var items = ServerHouses_.Where(x => x.id == houseId).Select(x => new
            {
                x.id,
                x.ownerId,
                ownerName = Characters.GetCharacterName(x.ownerId),
                x.interiorId,
                renterCount = 0,
                x.maxRenters,
                x.rentPrice,
                x.street,
                x.isLocked,
                x.isRentable,
                x.hasAlarm,
                x.hasStorage,
                x.hasBank,
                x.price,
                x.money,
            }).ToList();

            return JsonConvert.SerializeObject(items);
        }

        public static string GetHouseRenterArray(int houseId)
        {
            if (houseId <= 0) return "[]";
            var items = ServerHousesRenter_.Where(x => x.houseId == houseId && x.charId > 0).Select(x => new
            {
                x.id,
                x.charId,
                x.houseId,
                renterName = Characters.GetCharacterName(x.charId),
            }).ToList();
            return JsonConvert.SerializeObject(items);
        }

        public static bool ExistHouse(int houseId)
        {
            try
            {
                if (houseId <= 0) return false;
                var house = ServerHouses_.FirstOrDefault(x => x.id == houseId);
                if (house != null) return true;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return false;
        }

        public static int GetHouseOwner(int houseId)
        {
            try
            {
                if (houseId <= 0) return 0;
                var house = ServerHouses_.FirstOrDefault(x => x.id == houseId);
                if (house != null) return house.ownerId;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return 0;
        }

        public static int GetHousePrice(int houseId)
        {
            try
            {
                if (houseId <= 0) return 99999999;
                var house = ServerHouses_.FirstOrDefault(x => x.id == houseId);
                if (house != null) return house.price;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return 99999999;
        }

        public static string GetHouseStreet(int houseId)
        {
            try
            {
                if (houseId <= 0) return "";
                var house = ServerHouses_.FirstOrDefault(x => x.id == houseId);
                if (house != null) return house.street;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return "";
        }

        public static bool IsHouseLocked(int houseId)
        {
            try
            {
                if (houseId <= 0) return true;
                var house = ServerHouses_.FirstOrDefault(x => x.id == houseId);
                if (house != null) return house.isLocked;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return true;
        }

        public static void SetHouseLocked(int houseId, bool isLocked)
        {
            try
            {
                if (houseId <= 0) return;
                var house = ServerHouses_.FirstOrDefault(x => x.id == houseId);
                if (house == null) return;
                house.isLocked = isLocked;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static void SetHouseOwner(int houseId, int newOwner)
        {
            try
            {
                if (houseId <= 0) return;
                var house = ServerHouses_.FirstOrDefault(x => x.id == houseId);
                if(house != null)
                {
                    house.ownerId = newOwner;
                    house.isLocked = true;
                    if(newOwner == 0) { house.isRentable = false; }
                    using (gtaContext db = new gtaContext())
                    {
                        db.Server_Houses.Update(house);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static Position GetHouseEntrance(int houseId)
        {
            Position pos = new Position(0, 0, 0);
            try
            {
                if (houseId <= 0) return pos;
                var house = ServerHouses_.FirstOrDefault(x => x.id == houseId);
                if(house != null)
                {
                    pos = new Position(house.entranceX, house.entranceY, house.entranceZ);
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return pos;
        }

        public static int GetMaxInteriorsCount()
        {
            try
            {
                return ServerHousesInteriors_.Count();
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return 0;
        }

        public static int GetHouseInteriorId(int houseId)
        {
            try
            {
                if (houseId <= 0) return 0;
                var house = ServerHouses_.FirstOrDefault(x => x.id == houseId);
                if (house != null) return house.interiorId;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return 0;
        }

        public static Position GetInteriorExitPosition(int interiorId)
        {
            Position pos = new Position(0, 0, 0);
            try
            {
                if (interiorId <= 0) return pos;
                var interior = ServerHousesInteriors_.FirstOrDefault(x => x.interiorId == interiorId);
                if(interior != null) pos = new Position(interior.exitX, interior.exitY, interior.exitZ);
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return pos;
        }

        public static Position GetInteriorStoragePosition(int interiorId)
        {
            Position pos = new Position(0, 0, 0);
            try
            {
                if (interiorId <= 0) return pos;
                var interior = ServerHousesInteriors_.FirstOrDefault(x => x.interiorId == interiorId);
                if (interior != null) pos = new Position(interior.storageX, interior.storageY, interior.storageZ);
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return pos;
        }

        public static Position GetInteriorManagePosition(int interiorId)
        {
            Position pos = new Position(0, 0, 0);
            try
            {
                if (interiorId <= 0) return pos;
                var interior = ServerHousesInteriors_.FirstOrDefault(x => x.interiorId == interiorId);
                if (interior != null) pos = new Position(interior.manageX, interior.manageY, interior.manageZ);
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return pos;
        }

        public static string GetServerHouseStorageItems(int houseId)
        {
            if (houseId <= 0) return "[]";
            var items = ServerHousesStorage_.Where(x => x.houseId == houseId).Select(x => new
            {
                x.id,
                x.houseId,
                x.itemName,
                amount = x.itemAmount,
                itemPicName = ServerItems.ReturnItemPicSRC(x.itemName),
            }).ToList();
            return JsonConvert.SerializeObject(items);
        }

        public static float GetHouseStorageItemWeight(int houseId)
        {
            float invWeight = 0f;
            try
            {
                if (houseId <= 0) return invWeight;
                var item = ServerHousesStorage_.Where(i => i.houseId == houseId);
                foreach (Server_Houses_Storage i in item)
                {
                    string iName = ServerItems.ReturnNormalItemName(i.itemName);
                    var serverItem = ServerItems.ServerItems_.FirstOrDefault(si => si.itemName == iName);
                    if (serverItem != null)
                    {
                        invWeight += serverItem.itemWeight * i.itemAmount;
                    }
                }
            }
            catch(Exception e)
            {
                Alt.Log($"{e}");
            }
            return invWeight;
        }

        public static float GetInteriorStorageLimit(int interiorId)
        {
            float limit = 0f;
            try
            {
                if (interiorId <= 0) return limit;
                var interior = ServerHousesInteriors_.FirstOrDefault(x => x.interiorId == interiorId);
                if (interior != null) return interior.storageLimit;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return limit;
        }

        public static void AddServerHouseStorageItem(int houseId, string itemName, int itemAmount)
        {
            if (houseId <= 0 || itemName == "" || itemAmount <= 0) return;

            var itemData = new Server_Houses_Storage
            {
                houseId = houseId,
                itemName = itemName,
                itemAmount = itemAmount
            };

            try
            {
                var hasItem = ServerHousesStorage_.FirstOrDefault(i => i.houseId == houseId && i.itemName == itemName);
                if (hasItem != null)
                {
                    //Item existiert, itemAmount erhöhen
                    hasItem.itemAmount += itemAmount;
                    using (gtaContext db = new gtaContext())
                    {
                        var dbitem = db.Server_Houses_Storages.FirstOrDefault(i => i.houseId == houseId && i.itemName == itemName);
                        if (dbitem != null)
                        {
                            dbitem.itemAmount = dbitem.itemAmount += itemAmount;
                        }
                        db.SaveChanges();
                    }
                }
                else
                {
                    //Existiert nicht, Item neu adden
                    ServerHousesStorage_.Add(itemData);
                    using (gtaContext db = new gtaContext())
                    {
                        db.Server_Houses_Storages.Add(itemData);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static bool ExistServerHouseStorageItem(int houseId, string itemName)
        {
            try
            {
                if (houseId <= 0 || itemName == "") return false;
                var storageData = ServerHousesStorage_.FirstOrDefault(x => x.houseId == houseId && x.itemName == itemName);
                if (storageData != null) return true;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return false;
        }

        public static int GetServerHouseStorageItemAmount(int houseId, string itemName)
        {
            try
            {
                if (houseId <= 0 || itemName == "") return 0;
                var item = ServerHousesStorage_.FirstOrDefault(x => x.houseId == houseId && x.itemName == itemName);
                if (item != null) return item.itemAmount;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return 0;
        }

        public static void RemoveServerHouseStorageItemAmount(int houseId, string itemName, int itemAmount)
        {
            try
            {
                if (houseId <= 0 || itemName == "" || itemAmount == 0) return;
                var item = ServerHousesStorage_.FirstOrDefault(i => i.houseId == houseId && i.itemName == itemName);
                if (item != null)
                {
                    using (gtaContext db = new gtaContext())
                    {
                        int prevAmount = item.itemAmount;
                        item.itemAmount -= itemAmount;
                        if (item.itemAmount > 0)
                        {
                            db.Server_Houses_Storages.Update(item);
                            db.SaveChanges();
                        }
                        else
                            RemoveServerHouseStorageItem(houseId, itemName);
                    }
                }
            }
            catch (Exception _) { Alt.Log($"{_}");}
        }

        public static void RemoveServerHouseStorageItem(int houseId, string itemName)
        {
            try
            {
                var item = ServerHousesStorage_.FirstOrDefault(i => i.houseId == houseId && i.itemName == itemName);
                if (item != null)
                {
                    ServerHousesStorage_.Remove(item);
                    using (gtaContext db = new gtaContext())
                    {
                        db.Server_Houses_Storages.Remove(item);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static bool HasHouseStorageUpgrade(int houseId)
        {
            try
            {
                if (houseId <= 0) return false;
                var house = ServerHouses_.FirstOrDefault(x => x.id == houseId);
                if (house != null) return house.hasStorage;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return false;
        }

        public static bool HasHouseAlarmUpgrade(int houseId)
        {
            try
            {
                if (houseId <= 0) return false;
                var house = ServerHouses_.FirstOrDefault(x => x.id == houseId);
                if (house != null) return house.hasAlarm;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return false;
        }

        public static void SetHouseUpgradeState(int houseId, string upgrade, bool state)
        {
            try
            {
                if (houseId <= 0) return;
                var house = ServerHouses_.FirstOrDefault(x => x.id == houseId);
                if(house != null)
                {
                    switch(upgrade)
                    {
                        case "alarm":
                            house.hasAlarm = state;
                            break;
                        case "storage":
                            house.hasStorage = state;
                            break;
                        case "bank":
                            house.hasBank = state;
                            break;
                    }

                    using (gtaContext db = new gtaContext())
                    {
                        db.Server_Houses.Update(house);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static void SetHouseBankMoney(int houseId, int money)
        {
            try
            {
                if (houseId <= 0) return;
                var house = ServerHouses_.FirstOrDefault(x => x.id == houseId);
                if(house != null)
                {
                    house.money = money;
                    using (gtaContext db = new gtaContext())
                    {
                        db.Server_Houses.Update(house);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static int GetHouseBankMoney(int houseId)
        {
            try
            {
                if (houseId <= 0) return 0;
                var house = ServerHouses_.FirstOrDefault(x => x.id == houseId);
                if (house != null) return house.money;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return 0;
        }

        public static void SetHouseRentState(int houseId, bool state)
        {
            try
            {
                if (houseId <= 0) return;
                var house = ServerHouses_.FirstOrDefault(x => x.id == houseId);
                if(house != null)
                {
                    house.isRentable = state;
                    using (gtaContext db = new gtaContext())
                    {
                        db.Server_Houses.Update(house);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static bool HasHouseBankUpgrade(int houseId)
        {
            try
            {
                if (houseId <= 0) return false;
                var house = ServerHouses_.FirstOrDefault(x => x.id == houseId);
                if (house != null) return house.hasBank;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return false;
        }

        public static bool IsCharacterRentedInHouse(int charId, int houseId)
        {
            try
            {
                if (charId <= 0 || houseId <= 0) return false;
                var renter = ServerHousesRenter_.FirstOrDefault(x => x.charId == charId && x.houseId == houseId);
                if (renter != null) return true;
            }
            catch(Exception e)
            {
                Alt.Log($"{e}");
            }
            return false;
        }

        public static bool IsCharacterRentedInAnyHouse(int charId)
        {
            try
            {
                if (charId <= 0) return false;
                var renter = ServerHousesRenter_.FirstOrDefault(x => x.charId == charId);
                if (renter != null) return true;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return false;
        }

        public static int GetHouseRentPrice(int houseId)
        {
            try
            {
                if (houseId <= 0) return 0;
                var house = ServerHouses_.FirstOrDefault(x => x.id == houseId);
                if (house != null) return house.rentPrice;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return 0;
        }

        public static void AddServerHouseRenter(int houseId, int renterId)
        {
            if (houseId <= 0 || renterId <= 0) return;

            var rentData = new Server_Houses_Renter
            {
                houseId = houseId,
                charId = renterId
            };

            try
            {
                var renter = ServerHousesRenter_.FirstOrDefault(i => i.charId == renterId && i.houseId == houseId);
                if (renter != null) return;
                //Existiert nicht, Mieter hinzufügen
                ServerHousesRenter_.Add(rentData);
                using (gtaContext db = new gtaContext())
                {
                    db.Server_Houses_Renters.Add(rentData);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static void RemoveServerHouseRenter(int houseId, int renterId)
        {
            try
            {
                var renter = ServerHousesRenter_.FirstOrDefault(i => i.houseId == houseId && i.charId == renterId);
                if (renter != null)
                {
                    ServerHousesRenter_.Remove(renter);
                    using (gtaContext db = new gtaContext())
                    {
                        db.Server_Houses_Renters.Remove(renter);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static bool ExistInteriorId(int interiorId)
        {
            try
            {
                if (interiorId <= 0) return false;
                var interior = ServerHousesInteriors_.FirstOrDefault(x => x.interiorId == interiorId);
                if (interior != null) return true;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return false;
        }

        public static void SetHouseRentPrice(int houseId, int rentPrice)
        {
            try
            {
                if (houseId <= 0 || rentPrice <= 0) return;
                var house = ServerHouses_.FirstOrDefault(x => x.id == houseId);
                if(house != null)
                {
                    house.rentPrice = rentPrice;
                    using (gtaContext db = new gtaContext())
                    {
                        db.Server_Houses.Update(house);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }
    }
}
