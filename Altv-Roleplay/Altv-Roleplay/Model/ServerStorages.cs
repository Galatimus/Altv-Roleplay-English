using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using Altv_Roleplay.Factories;
using Altv_Roleplay.Handler;
using Altv_Roleplay.models;
using Altv_Roleplay.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Altv_Roleplay.Model
{
    class ServerStorages
    {
        public static List<Server_Storages> ServerStorages_ = new List<Server_Storages>();

        public static string GetFreeStorages()
        {
            return JsonConvert.SerializeObject(ServerStorages_.ToList().Where(x => x.owner == 0 && x.secondOwner == 0).Select(x => new
            {
                x.id,
                pos = x.entryPos,
                x.maxSize,
                x.price,
            }).OrderBy(x => x.id).ToList());
        }
        public static string GetAccountStorages(int accId)
        {
            return JsonConvert.SerializeObject(ServerStorages_.ToList().Where(x => x.owner == accId).Select(x => new
            {
                x.id,
                x.maxSize,
                x.price,
                pos = x.entryPos,
            }).OrderBy(x => x.id).ToList());
        }

        public static float GetWeight(int storageId)
        {
            if (!ExistStorage(storageId)) return 0f;
            List<Server_Storage_Item> items = GetStorageItems(storageId);
            float weight = 0f;
            foreach (var item in items.ToList())
            {
                var sItem = ServerItems.ServerItems_.ToList().FirstOrDefault(x => x.itemName == item.name);
                if (sItem == null) continue;
                weight += sItem.itemWeight * item.amount;
            }
            return weight;
        }

        public static int GetOwner(int storageId)
        {
            Server_Storages storage = ServerStorages_.ToList().FirstOrDefault(x => x.id == storageId);
            if (storage != null) return storage.owner;
            return 0;
        }
        public static int isfaction(int storageId)
        {
            Server_Storages storage = ServerStorages_.ToList().FirstOrDefault(x => x.id == storageId);
            if (storage != null) return storage.isfaction;
            return 0;
        }

        public static int GetPrice(int storageId)
        {
            Server_Storages storage = ServerStorages_.ToList().FirstOrDefault(x => x.id == storageId);
            if (storage != null) return storage.price;
            return 0;
        }
        public static int GetFactionid(int storageId)
        {
            Server_Storages storage = ServerStorages_.ToList().FirstOrDefault(x => x.id == storageId);
            if (storage != null) return storage.factionid;
            return 0;
        }
        public static int Getid(int factionid)
        {
            Server_Storages storage = ServerStorages_.ToList().FirstOrDefault(x => x.factionid == factionid);
            if (storage != null) return storage.id;
            return 0;
        }

        public static int GetSecondOwner(int storageId)
        {
            Server_Storages storage = ServerStorages_.ToList().FirstOrDefault(x => x.id == storageId);
            if (storage != null) return storage.secondOwner;
            return 0;
        }

        public static Position GetEntryPos(int storageId)
        {
            Server_Storages storage = ServerStorages_.ToList().FirstOrDefault(x => x.id == storageId);
            if (storage != null) return storage.entryPos;
            return new Position(0, 0, 0);
        }

        public static float GetMaxSize(int storageId)
        {
            Server_Storages storage = ServerStorages_.ToList().FirstOrDefault(x => x.id == storageId);
            if (storage != null) return storage.maxSize;
            return 0f;
        }

        public static List<Server_Storage_Item> GetStorageItems(int storageId)
        {
            Server_Storages storage = ServerStorages_.ToList().FirstOrDefault(x => x.id == storageId);
            if (storage != null) return storage.items;
            return new List<Server_Storage_Item>();
        }

        public static string GetStorageItemJSON(int storageId)
        {
            Server_Storages storage = ServerStorages_.ToList().FirstOrDefault(x => x.id == storageId);
            if (storage == null) return "[]";
            return System.Text.Json.JsonSerializer.Serialize(storage.items.ToList().Select(x => new
            {
                itemName = x.name,
                itemAmount = x.amount,
                pic = ServerItems.ReturnItemPicSRC(x.name),
            }).ToList());
        }

        public static bool ExistStorageItem(int storageId, string itemName)
        {
            Server_Storages storage = ServerStorages_.ToList().FirstOrDefault(x => x.id == storageId);
            if (storage != null && storage.items.Exists(x => x.name == itemName)) return true;
            return false;
        }

        public static bool ExistStorage(int storageId)
        {
            return ServerStorages_.ToList().Exists(x => x.id == storageId);
        }

        public static int GetItemAmount(int storageId, string itemName)
        {
            if (!ExistStorageItem(storageId, itemName)) return 0;
            List<Server_Storage_Item> items = GetStorageItems(storageId);
            Server_Storage_Item item = items.FirstOrDefault(x => x.name == itemName);
            if (item != null) return item.amount;
            return 0;
        }

        public static void SetOwner(int storageId, int ownerId)
        {
            try
            {
                Server_Storages storage = ServerStorages_.FirstOrDefault(x => x.id == storageId);
                if (storage == null) return;
                storage.owner = ownerId;

                using (gtaContext db = new gtaContext())
                {
                    db.Server_Storages.Update(storage);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void SetSecondOwner(int storageId, int secondOwnerId)
        {
            try
            {
                Server_Storages storage = ServerStorages_.FirstOrDefault(x => x.id == storageId);
                if (storage == null) return;
                storage.secondOwner = secondOwnerId;

                using (gtaContext db = new gtaContext())
                {
                    db.Server_Storages.Update(storage);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void SetStorageLocked(int storageId, bool isLocked)
        {
            Server_Storages storage = ServerStorages_.FirstOrDefault(x => x.id == storageId);
            if (storage != null)
                storage.isLocked = isLocked;
        }

        public static void RemoveItemAmount(int storageId, string itemName, int itemAmount)
        {
            try
            {
                if (!ExistStorageItem(storageId, itemName)) return;
                Server_Storages storage = ServerStorages_.FirstOrDefault(x => x.id == storageId);
                List<Server_Storage_Item> curItems = storage.items;
                if (storage == null || curItems == null || curItems.Count() == 0) return;
                Server_Storage_Item curItem = curItems.FirstOrDefault(x => x.name == itemName);
                if (curItem == null) return;
                curItem.amount -= itemAmount;

                if (curItem.amount <= 0)
                    curItems.Remove(curItem);

                using (gtaContext db = new gtaContext())
                {
                    db.Server_Storages.Update(storage);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void AddItem(int storageId, string itemName, int itemAmount)
        {
            try
            {
                Server_Storages storage = ServerStorages_.FirstOrDefault(x => x.id == storageId);
                List<Server_Storage_Item> curItems = storage.items;
                if (storage == null || curItems == null) return;
                if (ExistStorageItem(storageId, itemName))
                {
                    // Update
                    Server_Storage_Item curItem = curItems.FirstOrDefault(x => x.name == itemName);
                    if (curItem == null) return;
                    curItem.amount += itemAmount;
                    using (gtaContext db = new gtaContext())
                    {
                        db.Server_Storages.Update(storage);
                        db.SaveChanges();
                    }
                }
                else
                {
                    // Add
                    Server_Storage_Item newItem = new Server_Storage_Item
                    {
                        name = itemName,
                        amount = itemAmount
                    };

                    curItems.Add(newItem);
                    using (gtaContext db = new gtaContext())
                    {
                        db.Server_Storages.Update(storage);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}