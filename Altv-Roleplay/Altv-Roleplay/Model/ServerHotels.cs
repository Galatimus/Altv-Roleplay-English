using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using Altv_Roleplay.models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altv_Roleplay.Model
{
    class ServerHotels
    {
        public static List<Server_Hotels> ServerHotels_ = new List<Server_Hotels>();
        public static List<Server_Hotels_Apartments> ServerHotelsApartments_ = new List<Server_Hotels_Apartments>();
        public static List<Server_Hotels_Storage> ServerHotelsStorage_ = new List<Server_Hotels_Storage>();

        internal static void RequestHotelApartmentItems(IPlayer player, int hotelId)
        {
            try
            {
                if (player == null || !player.Exists || hotelId <= 0) return;
                var items = ServerHotelsApartments_.Where(x => x.hotelId == hotelId).Select(x => new
                {
                    x.hotelId,
                    apartmentId = x.id,
                    x.interiorId,
                    x.maxRentHours,
                    x.rentPrice,
                    x.ownerId,
                    ownerName = GetApartmentOwnerName(hotelId, x.id),
                }).ToList();

                var itemCount = (int)items.Count;
                var iterations = Math.Floor((decimal)(itemCount / 20));
                var rest = itemCount % 20;

                for(var i = 0; i < iterations; i++)
                {
                    var skip = i * 20;
                    if (player == null || !player.Exists) continue;
                    player.EmitLocked("Client:Hotel:setApartmentItems", JsonConvert.SerializeObject(items.Skip(skip).Take(20).ToList()));
                }

                if (player == null || !player.Exists) return;
                if (rest != 0) player.EmitLocked("Client:Hotel:setApartmentItems", JsonConvert.SerializeObject(items.Skip((int)iterations * 20).ToList()));
                player.EmitLocked("Client:Hotel:openCEF", GetHotelName(hotelId));
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        private static string GetHotelName(int hotelId)
        {
            try
            {
                if (hotelId <= 0) return "Fehler";
                var hotel = ServerHotels_.FirstOrDefault(x => x.id == hotelId);
                if(hotel != null)
                {
                    return hotel.name;
                }
                return "Fehler";
            }
            catch (Exception e)
            {
                Alt.Log($"{e}"); 
                return "Fehler";
            }
        }

        private static string GetApartmentOwnerName(int hotelId, int apartmentId)
        {
            string name = "-/-";
            try
            {
                if (hotelId <= 0 || apartmentId <= 0) return name;
                var hotelApartment = ServerHotelsApartments_.FirstOrDefault(x => x.hotelId == hotelId && x.id == apartmentId);
                if(hotelApartment != null)
                {
                    if (hotelApartment.ownerId <= 0) return name;
                    name = Characters.GetCharacterName(hotelApartment.ownerId);
                    return name;
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return name;
        }

        public static bool ExistHotelApartment(int hotelId, int apartmentId)
        {
            try
            {
                if (hotelId <= 0 || apartmentId <= 0) return false;
                var hotelApartment = ServerHotelsApartments_.FirstOrDefault(x => x.hotelId == hotelId && x.id == apartmentId);
                if(hotelApartment != null)
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return false;
        }

        public static int GetApartmentOwner(int hotelId, int apartmentId)
        {
            try
            {
                if (hotelId <= 0 || apartmentId == 0) return 0;
                var hotelApartment = ServerHotelsApartments_.FirstOrDefault(x => x.hotelId == hotelId && x.id == apartmentId);
                if(hotelApartment != null)
                {
                    return hotelApartment.ownerId;
                }
            }
            catch(Exception e)
            {
                Alt.Log($"{e}");
            }
            return 0;
        }

        public static int GetApartmentPrice(int hotelId, int apartmentId)
        {
            try
            {
                if (hotelId <= 0 || apartmentId <= 0) return 999999999;
                var hotelApartment = ServerHotelsApartments_.FirstOrDefault(x => x.hotelId == hotelId && x.id == apartmentId);
                if(hotelApartment != null)
                {
                    return hotelApartment.rentPrice;
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return 999999999;
        }

        public static bool HasCharacterAnApartment(int charId)
        {
            try
            {
                if (charId <= 0) return false;
                var hotelApartment = ServerHotelsApartments_.FirstOrDefault(x => x.ownerId == charId);
                if(hotelApartment != null)
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return false;
        }

        public static string GetCharacterRentedHotelName(int charId)
        {
            try
            {
                if (charId <= 0) return "Fehler";
                var hotelApartment = ServerHotelsApartments_.FirstOrDefault(x => x.ownerId == charId);
                if(hotelApartment != null)
                {
                    var hotel = ServerHotels_.FirstOrDefault(x => x.id == hotelApartment.hotelId);
                    if (hotel != null) return hotel.name;
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return "Fehler";
        }

        public static void SetApartmentOwner(int hotelId, int apartmentId, int newOwner)
        {
            try
            {
                if (hotelId <= 0 || apartmentId <= 0) return;
                var hotelApartment = ServerHotelsApartments_.FirstOrDefault(x => x.hotelId == hotelId && x.id == apartmentId);
                if(hotelApartment != null)
                {
                    hotelApartment.ownerId = newOwner;
                    hotelApartment.isLocked = true;
                    hotelApartment.lastRent = DateTime.Now;
                    using (gtaContext db = new gtaContext())
                    {
                        db.Server_Hotels_Apartments.Update(hotelApartment);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static int GetApartmentRentHours(int hotelId, int apartmentId)
        {
            try
            {
                if (hotelId <= 0 || apartmentId <= 0) return 0;
                var hotelApartment = ServerHotelsApartments_.FirstOrDefault(x => x.hotelId == hotelId && x.id == apartmentId);
                if(hotelApartment != null)
                {
                    return hotelApartment.maxRentHours;
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return 0;
        }

        public static Position GetHotelPosition(int hotelId, int apartmentId)
        {
            Position pos = new Position(0, 0, 0);
            try
            {
                if (hotelId <= 0 || apartmentId <= 0) return pos;
                var hotelApartment = ServerHotelsApartments_.FirstOrDefault(x => x.hotelId == hotelId && x.id == apartmentId);
                if(hotelApartment != null)
                {
                    var hotel = ServerHotels_.FirstOrDefault(x => x.id == hotelId);
                    if (hotel != null) return new Position(hotel.posX, hotel.posY, hotel.posZ);
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return pos;
        }

        public static int GetHotelIdByApartmentId(int apartmentId)
        {
            try
            {
                if (apartmentId <= 0) return 0;
                var hotel = ServerHotelsApartments_.FirstOrDefault(x => x.id == apartmentId);
                if(hotel != null)
                {
                    return hotel.hotelId;
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return 0;
        }

        public static int GetCharacterApartmentId(int charId)
        {
            try
            {
                if (charId <= 0) return 0;
                var apartment = ServerHotelsApartments_.FirstOrDefault(x => x.ownerId == charId);
                if (apartment != null) return apartment.id;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return 0;
        }

        public static string GetServerHotelStorageItems(int apartmentId)
        {
            if (apartmentId <= 0) return "[]";
            var items = ServerHotelsStorage_.Where(x => x.apartmentId == apartmentId).Select(x => new
            {
                x.id,
                x.apartmentId,
                x.itemName,
                x.amount,
                itemPicName = ServerItems.ReturnItemPicSRC(x.itemName),
            }).ToList();
            return JsonConvert.SerializeObject(items);
        }

        public static void AddServerHotelStorageItem(int apartmentId, string itemName, int itemAmount)
        {
            if (apartmentId <= 0 || itemName == "" || itemAmount <= 0) return;

            var itemData = new Server_Hotels_Storage
            {
                apartmentId = apartmentId,
                itemName = itemName,
                amount = itemAmount
            };

            try
            {
                var hasItem = ServerHotelsStorage_.FirstOrDefault(i => i.apartmentId == apartmentId && i.itemName == itemName);
                if (hasItem != null)
                {
                    //Item existiert, itemAmount erhöhen
                    hasItem.amount += itemAmount;
                    using (gtaContext db = new gtaContext())
                    {
                        var dbitem = db.Server_Hotels_Storages.FirstOrDefault(i => i.apartmentId == apartmentId && i.itemName == itemName);
                        if (dbitem != null)
                        {
                            dbitem.amount = dbitem.amount += itemAmount;
                        }
                        db.SaveChanges();
                    }
                }
                else
                {
                    //Existiert nicht, Item neu adden
                    ServerHotelsStorage_.Add(itemData);
                    using (gtaContext db = new gtaContext())
                    {
                        db.Server_Hotels_Storages.Add(itemData);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static bool ExistServerHotelStorageItem(int apartmentId, string itemName)
        {
            try
            {
                if (apartmentId <= 0 || itemName == "") return false;
                var storageData = ServerHotelsStorage_.FirstOrDefault(x => x.apartmentId == apartmentId && x.itemName == itemName);
                if (storageData != null) return true;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return false;
        }

        public static int GetServerHotelStorageItemAmount(int apartmentId, string itemName)
        {
            try
            {
                if (apartmentId <= 0 || itemName == "") return 0;
                var item = ServerHotelsStorage_.FirstOrDefault(x => x.apartmentId == apartmentId && x.itemName == itemName);
                if (item != null) return item.amount;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return 0;
        }

        public static void RemoveServerHotelStorageItemAmount(int apartmentId, string itemName, int itemAmount)
        {
            try
            {
                if (apartmentId <= 0 || itemName == "" || itemAmount == 0) return;
                var item = ServerHotelsStorage_.FirstOrDefault(i => i.apartmentId == apartmentId && i.itemName == itemName);
                if (item != null)
                {
                    using (gtaContext db = new gtaContext())
                    {
                        int prevAmount = item.amount;
                        item.amount -= itemAmount;
                        if (item.amount > 0)
                        {
                            db.Server_Hotels_Storages.Update(item);
                            db.SaveChanges();
                        }
                        else
                            RemoveServerHotelStorageItem(apartmentId, itemName);
                    }
                }
            }
            catch (Exception _) { Alt.Log($"{_}"); }
        }

        public static void RemoveServerHotelStorageItem(int apartmentId, string itemName)
        {
            try
            {
                var item = ServerHotelsStorage_.FirstOrDefault(i => i.apartmentId == apartmentId && i.itemName == itemName);
                if (item != null)
                {
                    ServerHotelsStorage_.Remove(item);
                    using (gtaContext db = new gtaContext())
                    {
                        db.Server_Hotels_Storages.Remove(item);
                        db.SaveChanges();
                    }
                }
            }
            catch(Exception e)
            {
                Alt.Log($"{e}");
            }
        }
        public static float GetHotelStorageItemWeight(int apartmentId)
        {
            var item = ServerHotelsStorage_.Where(i => i.apartmentId == apartmentId);
            float invWeight = 0f;
            foreach (Server_Hotels_Storage i in item)
            {
                string iName = ServerItems.ReturnNormalItemName(i.itemName);
                var serverItem = ServerItems.ServerItems_.FirstOrDefault(si => si.itemName == iName);
                if (serverItem != null)
                {
                    invWeight += serverItem.itemWeight * i.amount;
                }
            }
            return invWeight;
        }

        public static int GetApartmentInteriorId(int apartmentId)
        {
            try
            {
                if (apartmentId <= 0) return 0;
                var apartment = ServerHotelsApartments_.FirstOrDefault(x => x.id == apartmentId);
                if (apartment != null) return apartment.interiorId;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return 0;
        }
    }
}
