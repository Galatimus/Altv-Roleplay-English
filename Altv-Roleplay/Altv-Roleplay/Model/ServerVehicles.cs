using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using Altv_Roleplay.models;
using System;
using System.Collections.Generic;
using System.Text;
using AltV.Net.Enums;
using System.Linq;
using System.Globalization;
using Altv_Roleplay.Utils;
using Newtonsoft.Json;
using System.Threading.Tasks;
using AltV.Net.Async;
using Altv_Roleplay.Factories;

namespace Altv_Roleplay.Model
{
    class ServerVehicles
    {
        //this two
        public static List<Server_Vehicles> ServerVehiclesLocked_ { get { lock (ServerVehicles_) return ServerVehicles_.ToList(); } set => ServerVehicles_ = value; }
        public static List<Server_Vehicles> ServerVehicles_ = new List<Server_Vehicles>();
        public static List<Server_Vehicles_Mod> ServerVehiclesMod_ = new List<Server_Vehicles_Mod>();
        public static List<Server_Vehicle_Items> ServerVehicleTrunkItems_ = new List<Server_Vehicle_Items>();

        public static void CreateServerVehicle(int id, int charid, uint hash, int vehType, int factionid, float fuel, float km, bool engineState, bool isEngineHealthy, bool lockState, bool isInGarage, int garageId, Position position, Rotation rotation, string plate, DateTime lastUsage, DateTime buyDate)
        {
            var newVehicleData = new Server_Vehicles
            {
                id = id,
                charid = charid,
                hash = hash,
                vehType = vehType,
                faction = factionid,
                fuel = fuel,
                KM = km,
                engineState = engineState,
                isEngineHealthy = isEngineHealthy,
                lockState = lockState,
                isInGarage = isInGarage,
                garageId = garageId,
                posX = position.X,
                posY = position.Y,
                posZ = position.Z,
                rotX = rotation.Pitch,
                rotY = rotation.Roll,
                rotZ = rotation.Yaw,
                plate = plate,
                lastUsage = lastUsage,
                buyDate = buyDate
            };
            ServerVehicles_.Add(newVehicleData);

            if (isInGarage) return;
            IVehicle veh = Alt.CreateVehicle(hash, position, rotation);
            if (position == new Position(0, 0, 0)) { SetVehicleInGarage(veh, true, 10); return; }
            if (lockState == true) { veh.LockState = VehicleLockState.Locked; }
            else if (lockState == false) { veh.LockState = VehicleLockState.Unlocked; }
            veh.NumberplateText = plate;
            veh.EngineOn = engineState;
            veh.SetVehicleId((ulong)id);
            veh.SetVehicleTrunkState(false);
            SetVehicleModsCorrectly(veh);
        }

        public static void AddVehicleTrunkItem(int vehId, string itemName, int itemAmount, bool inGlovebox)
        {
            if (vehId <= 0 || itemName == "" || itemAmount <= 0) return;
            try
            {
                var itemData = new Server_Vehicle_Items
                {
                    vehId = vehId,
                    itemName = itemName,
                    itemAmount = itemAmount,
                    isInGlovebox = inGlovebox
                };

                var hasItem = ServerVehicleTrunkItems_.FirstOrDefault(i => i.vehId == vehId && i.itemName == itemName && i.isInGlovebox == inGlovebox);
                if (hasItem != null)
                {
                    //Item existiert, itemAmount erhöhen
                    hasItem.itemAmount += itemAmount;
                    using (gtaContext db = new gtaContext())
                    {
                        var dbitem = db.Server_Vehicle_Items.FirstOrDefault(i => i.vehId == vehId && i.itemName == itemName && i.isInGlovebox == inGlovebox);
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
                    ServerVehicleTrunkItems_.Add(itemData);
                    using (gtaContext db = new gtaContext())
                    {
                        db.Server_Vehicle_Items.Add(itemData);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static int GetVehicleIdByPlate(string plate)
        {
            try
            {
                var vehicle = ServerVehicles_.FirstOrDefault(x => x.plate == plate);
                if (vehicle != null)
                {
                    return vehicle.id;
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return 0;
        }

        public static int GetVehicleTrunkItemAmount(int vehId, string itemName, bool inGlovebox)
        {
            try
            {
                if (vehId <= 0 || itemName == "") return 0;
                var item = ServerVehicleTrunkItems_.FirstOrDefault(x => x.vehId == vehId && x.itemName == itemName && x.isInGlovebox == inGlovebox);
                if (item != null) return item.itemAmount;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return 0;
        }

        public static void RemoveVehicleTrunkItemAmount(int vehId, string itemName, int itemAmount, bool inGlovebox)
        {
            try
            {
                if (vehId <= 0 || itemName == "" || itemAmount == 0) return;
                var item = ServerVehicleTrunkItems_.FirstOrDefault(i => i.vehId == vehId && i.itemName == itemName && i.isInGlovebox == inGlovebox);
                if (item != null)
                {
                    int prevAmount = item.itemAmount;
                    item.itemAmount -= itemAmount;
                    using (gtaContext db = new gtaContext())
                    {
                        if (item.itemAmount > 0)
                        {
                            db.Server_Vehicle_Items.Update(item);
                            db.SaveChanges();
                        }
                        else
                            RemoveVehicleTrunkItem(vehId, itemName, inGlovebox);
                    }
                }
            }
            catch (Exception _) { Alt.Log($"{_}"); }
        }

        public static void RemoveVehicleTrunkItem(int vehId, string itemName, bool inGlovebox)
        {
            var item = ServerVehicleTrunkItems_.FirstOrDefault(i => i.vehId == vehId && i.itemName == itemName && i.isInGlovebox == inGlovebox);
            if (item != null)
            {
                ServerVehicleTrunkItems_.Remove(item);
                using (gtaContext db = new gtaContext())
                {
                    db.Server_Vehicle_Items.Remove(item);
                    db.SaveChanges();
                }
            }
        }

        public static bool ExistVehicleTrunkItem(int vehId, string itemName, bool isInGlovebox)
        {
            try
            {
                if (vehId <= 0 || itemName == "") return false;
                var trunkData = ServerVehicleTrunkItems_.FirstOrDefault(x => x.vehId == vehId && x.itemName == itemName && x.isInGlovebox == isInGlovebox);
                if (trunkData != null) return true;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return false;
        }

        public static string GetVehicleTrunkItems(int vehId, bool isInGlovebox)
        {
            var items = ServerVehicleTrunkItems_.ToList().Where(x => x != null && x.vehId == vehId && x.isInGlovebox == isInGlovebox).Select(x => new
            {
                x.id,
                x.vehId,
                x.itemName,
                x.itemAmount,
                itemPicName = ServerItems.ReturnItemPicSRC(x.itemName),
            }).ToList();
            return JsonConvert.SerializeObject(items);
        }

        public static void SetVehicleLockState(IVehicle veh, bool state)
        {
            if (veh == null || !veh.Exists) return;
            ulong vehID = veh.GetVehicleId();
            if (vehID == 0) return;
            var vehs = ServerVehicles_.FirstOrDefault(v => (ulong)v.id == vehID);
            if (vehs != null)
            {
                AltAsync.Do(() =>
                {
                    if (state == true) { veh.LockState = VehicleLockState.Locked; }
                    else if (state == false) { veh.LockState = VehicleLockState.Unlocked; }
                });
                vehs.lockState = state;

                using (gtaContext db = new gtaContext())
                {
                    db.Server_Vehicles.Update(vehs);
                    db.SaveChanges();
                }
            }
        }

        public static bool GetVehicleLockState(IVehicle veh)
        {
            ulong vehID = veh.GetVehicleId();
            if (veh == null || !veh.Exists || vehID == 0) return false;
            var vehs = ServerVehicles_.FirstOrDefault(v => (ulong)v.id == vehID);
            if (vehs != null)
            {
                return vehs.lockState;
            }
            return false;
        }

        public static void SetVehicleEngineState(IVehicle veh, bool state)
        {
            if (veh == null || !veh.Exists) return;
            ulong vehID = veh.GetVehicleId();
            if (vehID == 0) return;
            var vehs = ServerVehicles_.FirstOrDefault(v => (ulong)v.id == vehID);
            if (vehs != null)
            {
                AltAsync.Do(() =>
                {
                    veh.EngineOn = state;
                });
                vehs.engineState = state;
            }
        }

        public static bool GetVehicleEngineState(IVehicle veh)
        {
            ulong vehID = veh.GetVehicleId();
            if (veh == null || !veh.Exists || vehID == 0) return false;
            var vehs = ServerVehicles_.FirstOrDefault(v => (ulong)v.id == vehID);
            if (vehs != null)
            {
                return vehs.engineState;
            }
            return false;
        }

        public static Position GetVehiclePosition(IVehicle veh)
        {
            Position pos = new Position();
            ulong vehId = veh.GetVehicleId();
            var vehs = ServerVehicles_.FirstOrDefault(v => (ulong)v.id == vehId);
            if (vehs != null)
            {
                pos = new Position(vehs.posX, vehs.posY, vehs.posZ);
            }
            return pos;
        }

        public static Rotation GetVehicleRotation(IVehicle veh)
        {
            Rotation rot = new Rotation();
            ulong vehID = veh.GetVehicleId();
            var vehs = ServerVehicles_.FirstOrDefault(v => (ulong)v.id == vehID);
            if (vehs != null)
            {
                rot = new Rotation(vehs.rotX, vehs.rotY, vehs.rotZ);
            }
            return rot;
        }

        public static int GetVehicleOwner(IVehicle veh)
        {
            ulong vehID = veh.GetVehicleId();
            if (veh == null || !veh.Exists || vehID == 0) return 0;
            var vehs = ServerVehicles_.FirstOrDefault(v => (ulong)v.id == vehID);
            if (vehs != null)
            {
                return vehs.charid;
            }
            return 0;
        }

        public static ulong GetVehicleHashById(int vehId)
        {
            try
            {
                if (vehId <= 0) return 0;
                var vehs = ServerVehicles_.FirstOrDefault(x => x.id == vehId);
                if (vehs != null)
                {
                    return vehs.hash;
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return 0;
        }

        public static int GetVehicleOwnerById(int vehId)
        {
            try
            {
                if (vehId <= 0) return 0;
                var vehs = ServerVehicles_.FirstOrDefault(v => v.id == vehId);
                if (vehs != null)
                {
                    return vehs.charid;
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return 0;
        }

        public static int GetVehicleFactionId(IVehicle veh)
        {
            ulong vehID = veh.GetVehicleId();
            if (veh == null || !veh.Exists || vehID == 0) return 0;
            var vehs = ServerVehicles_.FirstOrDefault(v => (ulong)v.id == vehID);
            if (vehs != null)
            {
                return vehs.faction;
            }
            return 0;
        }

        public static int GetVehicleFactionId2(int id)
        {
            if (id == null) return 0;
            var vehs = ServerVehicles_.FirstOrDefault(v => v.id == id);
            if (vehs != null)
            {
                return vehs.faction;
            }
            return 0;
        }

        public static int GetVehicleType(IVehicle veh)
        {
            ulong vehID = veh.GetVehicleId();
            if (veh == null || !veh.Exists || vehID == 0) return -1;
            var vehs = ServerVehicles_.FirstOrDefault(v => (ulong)v.id == vehID);
            if (vehs != null)
            {
                return vehs.vehType;
            }
            return -1;
        }

        public static float GetVehicleFuel(IVehicle veh)
        {
            ulong vehID = veh.GetVehicleId();
            if (veh == null || !veh.Exists || vehID == 0) return 0;
            var vehs = ServerVehicles_.FirstOrDefault(v => (ulong)v.id == vehID);
            if (vehs != null)
            {
                return vehs.fuel;
            }
            return 0;
        }

        public static void SetVehicleFuel(IVehicle veh, float fuel)
        {
            try
            {
                if (veh == null || !veh.Exists) return;
                ulong vehID = veh.GetVehicleId();
                if (vehID == 0) return;
                var vehs = ServerVehicles_.FirstOrDefault(v => (ulong)v.id == vehID);
                if (vehs != null)
                {
                    vehs.fuel = fuel;
                    if (vehs.fuel <= 0)
                    {
                        vehs.fuel = 0f;
                        SetVehicleEngineState(veh, false);
                    }
                    else if (vehs.fuel > GetVehicleFuelLimitOnHash(veh.Model)) vehs.fuel = (float)GetVehicleFuelLimitOnHash(veh.Model);
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static float GetVehicleKM(IVehicle veh)
        {
            ulong vehID = veh.GetVehicleId();
            if (veh == null || !veh.Exists || vehID == 0) return 0;
            var vehs = ServerVehicles_.FirstOrDefault(v => (ulong)v.id == vehID);
            if (vehs != null)
            {
                return vehs.KM;
            }
            return 0;
        }

        public static void SetVehicleKM(IVehicle veh, float km)
        {
            try
            {
                if (veh == null || !veh.Exists) return;
                ulong vehID = veh.GetVehicleId();
                if (vehID == 0) return;
                var vehs = ServerVehicles_.FirstOrDefault(v => (ulong)v.id == vehID);
                if (vehs != null)
                {
                    vehs.KM = km;
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static void SetVehicleEngineHealthy(IVehicle veh, bool state)
        {
            try
            {
                if (veh == null || !veh.Exists) return;
                ulong vehID = veh.GetVehicleId();
                if (vehID == 0) return;
                var vehs = ServerVehicles_.FirstOrDefault(v => (ulong)v.id == vehID);
                if (vehs != null)
                {
                    vehs.isEngineHealthy = state;
                    using (gtaContext db = new gtaContext())
                    {
                        db.Server_Vehicles.Update(vehs);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static bool IsVehicleEngineHealthy(IVehicle veh)
        {
            try
            {
                if (veh == null || !veh.Exists) return false;
                ulong vehID = veh.GetVehicleId();
                if (vehID == 0) return false;
                var vehs = ServerVehicles_.FirstOrDefault(v => (ulong)v.id == vehID);
                if (vehs != null)
                {
                    return vehs.isEngineHealthy;
                }
                return false;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
                return false;
            }
        }

        public static string GetVehicleNameOnHash(ulong hash)
        {
            return ServerAllVehicles.ServerAllVehicles_.FirstOrDefault(x => x.hash == hash)?.name ?? "";
        }

        public static string GetVehicleManufactorOnHash(ulong hash)
        {
            return ServerAllVehicles.ServerAllVehicles_.FirstOrDefault(x => x.hash == hash)?.manufactor ?? "";
        }

        public static string GetVehicleFuelTypeOnHash(ulong hash)
        {
            return ServerAllVehicles.ServerAllVehicles_.FirstOrDefault(x => x.hash == hash)?.fuelType ?? "";
        }

        public static int GetVehicleFuelLimitOnHash(ulong hash)
        {
            return ServerAllVehicles.ServerAllVehicles_.FirstOrDefault(x => x.hash == hash)?.maxFuel ?? 0;
        }

        public static int GetVehicleTrunkCapacityOnHash(ulong hash)
        {
            return ServerAllVehicles.ServerAllVehicles_.FirstOrDefault(x => x.hash == hash)?.trunkCapacity ?? 0;
        }

        public static int GetVehicleMaxSeatsOnHash(ulong hash)
        {
            return ServerAllVehicles.ServerAllVehicles_.FirstOrDefault(x => x.hash == hash)?.seats ?? 0;
        }

        public static bool ExistServerVehiclePlate(string plate)
        {
            bool Exist = false;
            var dbveh = ServerVehicles_.ToList().FirstOrDefault(x => x.plate == plate);
            if (dbveh != null)
            {
                Exist = true;
            }
            return Exist;
        }

        public static DateTime GetVehicleBuyDate(int vehicleId)
        {
            DateTime dt = new DateTime(0001, 01, 01);
            if (vehicleId <= 0) return dt;
            var vehs = ServerVehicles_.FirstOrDefault(p => p.id == vehicleId);
            if (vehs != null)
            {
                dt = vehs.buyDate;
            }
            return dt;
        }

        public static void SetServerVehiclePlate(int vehID, string plate)
        {
            try
            {
                if (vehID <= 0 || plate == "") return;
                var vehicle = ServerVehicles_.FirstOrDefault(x => x.id == vehID);
                if (vehicle != null)
                {
                    vehicle.plate = plate;

                    using (gtaContext db = new gtaContext())
                    {
                        db.Server_Vehicles.Update(vehicle);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static bool IsVehicleInGarage(IVehicle veh)
        {
            ulong vehID = veh.GetVehicleId();
            if (veh == null || !veh.Exists || vehID == 0) return false;
            var vehs = ServerVehicles_.FirstOrDefault(v => (ulong)v.id == vehID);
            if (vehs != null)
            {
                return vehs.isInGarage;
            }
            return false;
        }

        public static int GetVehicleGarageId(IVehicle veh)
        {
            ulong vehID = veh.GetVehicleId();
            if (veh == null || !veh.Exists || vehID == 0) return 0;
            var vehs = ServerVehicles_.FirstOrDefault(v => (ulong)v.id == vehID);
            if (vehs != null)
            {
                return vehs.garageId;
            }
            return 0;
        }

        public static void SetVehicleInGarage(IVehicle veh, bool state, int garageId)
        {
            try
            {
                if (veh == null || !veh.Exists) return;
                ulong vehID = veh.GetVehicleId();
                if (vehID == 0) return;
                var dbVehicle = ServerVehicles_.FirstOrDefault(v => v.id == (int)veh.GetVehicleId());
                if (dbVehicle == null) return;
                dbVehicle.isInGarage = state;
                dbVehicle.lastUsage = DateTime.Now;
                if (state == true) { dbVehicle.garageId = garageId; dbVehicle.engineState = false; dbVehicle.lockState = true; veh.Remove(); }
                using (gtaContext db = new gtaContext())
                {
                    db.Server_Vehicles.Update(dbVehicle);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }
        public static void SetVehicleNewOwner(int newOwner, string plate)
        {
            int vehID = ServerVehicles.GetVehicleIdByPlate(plate);
            if (vehID == 0) return;
            var vehs = ServerVehicles_.FirstOrDefault(v => v.id == vehID);
            if (vehs != null)
            {
                AltAsync.Do(() =>
                {
                    vehs.charid = newOwner;
                });
                vehs.charid = newOwner;
                using (gtaContext db = new gtaContext())
                {
                    db.Server_Vehicles.Update(vehs);
                    db.SaveChanges();
                }
            }

        }

        public static float GetVehicleVehicleTrunkWeight(int vehId, bool searchOnlyGlovebox)
        {
            try
            {
                float invWeight = 0f;
                if (vehId <= 0) return invWeight;

                if (searchOnlyGlovebox)
                {
                    var gItem = ServerVehicleTrunkItems_.ToList().Where(i => i.vehId == vehId && i.isInGlovebox == true);
                    foreach (Server_Vehicle_Items i in gItem)
                    {
                        string iName = ServerItems.ReturnNormalItemName(i.itemName);
                        var serverItem = ServerItems.ServerItems_.ToList().FirstOrDefault(si => si.itemName == iName);
                        if (serverItem != null)
                        {
                            invWeight += serverItem.itemWeight * i.itemAmount;
                        }
                    }
                    return invWeight;
                }

                var item = ServerVehicleTrunkItems_.ToList().Where(i => i.vehId == vehId);
                foreach (Server_Vehicle_Items i in item)
                {
                    string iName = ServerItems.ReturnNormalItemName(i.itemName);
                    var serverItem = ServerItems.ServerItems_.ToList().FirstOrDefault(si => si.itemName == iName);
                    if (serverItem != null)
                    {
                        invWeight += serverItem.itemWeight * i.itemAmount;
                    }
                }
                return invWeight;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return 0f;
        }


        public static void SaveVehiclePositionAndStates(IVehicle veh)
        {
            try
            {

                
                ulong vehID = veh.GetVehicleId();
                if (veh == null || !veh.Exists || vehID == 0) return;
                var vehs = ServerVehicles_.FirstOrDefault(v => (ulong)v.id == vehID);
                if (vehs != null)
                {
                    vehs.posX = veh.Position.X;
                    vehs.posY = veh.Position.Y;
                    vehs.posZ = veh.Position.Z;
                    vehs.rotX = veh.Rotation.Pitch;
                    vehs.rotY = veh.Rotation.Roll;
                    vehs.rotZ = veh.Rotation.Yaw;
                    vehs.engineState = veh.EngineOn;
                    if (veh.LockState == VehicleLockState.Locked) { vehs.lockState = true; }
                    else if (veh.LockState == VehicleLockState.Unlocked) { vehs.lockState = false; }
                    vehs.KM = GetVehicleKM(veh);
                    vehs.fuel = GetVehicleFuel(veh);

                    using (gtaContext db = new gtaContext())
                    {
                        db.Server_Vehicles.Update(vehs);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static string GetAllParkedOutFactionVehicles(string factionShort)
        {
            if (factionShort == "") return "";

            var items = Alt.GetAllVehicles().ToList().Where(x => x.NumberplateText.Contains(factionShort)).Select(x => new
            {
                vehName = GetVehicleNameOnHash(x.Model),
                vehHash = x.Model,
                vehPlate = x.NumberplateText,
                vehPosX = x.Position.X,
                vehPosY = x.Position.Y,
            }).ToList();

            return JsonConvert.SerializeObject(items);
        }


        public static void AddVehicleModToList(params int[] args)
        {
            try
            {
                var vehMods = new Server_Vehicles_Mod
                {
                    id = args[0],
                    vehId = args[1],
                    colorPrimaryType = (byte)args[2],
                    colorSecondaryType = (byte)args[3],
                    spoiler = (byte)args[4],
                    front_bumper = (byte)args[5],
                    rear_bumper = (byte)args[6],
                    side_skirt = (byte)args[7],
                    exhaust = (byte)args[8],
                    frame = (byte)args[9],
                    grille = (byte)args[10],
                    hood = (byte)args[11],
                    fender = (byte)args[12],
                    right_fender = (byte)args[13],
                    roof = (byte)args[14],
                    engine = (byte)args[15],
                    brakes = (byte)args[16],
                    transmission = (byte)args[17],
                    horns = (byte)args[18],
                    suspension = (byte)args[19],
                    armor = (byte)args[20],
                    turbo = (byte)args[21],
                    xenon = (byte)args[22],
                    wheel_type = (byte)args[23],
                    wheels = (byte)args[24],
                    wheelcolor = (byte)args[25],
                    plate_holder = (byte)args[26],
                    trim_design = (byte)args[27],
                    ornaments = (byte)args[28],
                    dial_design = (byte)args[29],
                    steering_wheel = (byte)args[30],
                    shift_lever = (byte)args[31],
                    plaques = (byte)args[32],
                    hydraulics = (byte)args[33],
                    airfilter = (byte)args[34],
                    window_tint = (byte)args[35],
                    livery = (byte)args[36],
                    plate = (byte)args[37],
                    neon = (byte)args[38],
                    neon_r = (byte)args[39],
                    neon_g = (byte)args[40],
                    neon_b = (byte)args[41],
                    smoke_r = (byte)args[42],
                    smoke_g = (byte)args[43],
                    smoke_b = (byte)args[44],
                    colorPearl = (byte)args[45],
                    headlightColor = (byte)args[46],
                    colorPrimary_r = (byte)args[47],
                    colorPrimary_g = (byte)args[48],
                    colorPrimary_b = (byte)args[49],
                    colorSecondary_r = (byte)args[50],
                    colorSecondary_g = (byte)args[51],
                    colorSecondary_b = (byte)args[52],
                    back_wheels = (byte)args[53],
                    plate_vanity = (byte)args[54],
                    door_interior = (byte)args[55],
                    seats = (byte)args[56],
                    rear_shelf = (byte)args[57],
                    trunk = (byte)args[58],
                    engine_block = (byte)args[59],
                    strut_bar = (byte)args[60],
                    arch_cover = (byte)args[61],
                    antenna = (byte)args[62],
                    exterior_parts = (byte)args[63],
                    tank = (byte)args[64],
                    rear_hydraulics = (byte)args[65],
                    door = (byte)args[66],
                    plate_color = (byte)args[67],
                    interior_color = (byte)args[68],
                    smoke = (byte)args[69]

                };

                ServerVehiclesMod_.Add(vehMods);

                using (gtaContext db = new gtaContext())
                {
                    var vMod = db.Server_Vehicles_Mods.FirstOrDefault(v => v.id == vehMods.id && v.vehId == vehMods.vehId);
                    if (vMod == null)
                    {
                        db.Server_Vehicles_Mods.Add(vehMods);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        

        public static void SetVehicleModsCorrectly(IVehicle veh)
        {
            try
            {
                if (veh == null || !veh.Exists) return;
                veh.ModKit = 1;

                var mod = ServerVehiclesMod_.FirstOrDefault(x => x.vehId == (int)veh.GetVehicleId());
                if (mod != null)
                {
                    veh.SetMod(0, (byte)mod.spoiler);
                    veh.SetMod(1, (byte)mod.front_bumper);
                    veh.SetMod(2, (byte)mod.rear_bumper);
                    veh.SetMod(3, (byte)mod.side_skirt);
                    veh.SetMod(4, (byte)mod.exhaust);
                    veh.SetMod(5, (byte)mod.frame);
                    veh.SetMod(6, (byte)mod.grille);
                    veh.SetMod(7, (byte)mod.hood);
                    veh.SetMod(8, (byte)mod.fender);
                    veh.SetMod(9, (byte)mod.right_fender);
                    veh.SetMod(10, (byte)mod.roof);
                    veh.SetMod(11, (byte)mod.engine);
                    veh.SetMod(12, (byte)mod.brakes);
                    veh.SetMod(13, (byte)mod.transmission);
                    veh.SetMod(14, (byte)mod.horns);
                    veh.SetMod(15, (byte)mod.suspension);
                    veh.SetMod(16, (byte)mod.armor);
                    veh.SetMod(18, (byte)mod.turbo);
                    veh.SetMod(20, (byte)mod.smoke);
                    veh.SetMod(22, (byte)mod.xenon);
                    veh.SetWheels(0, (byte)mod.wheels);
                    //veh.SetWheels(0, (byte)mod.back_wheels); BACK WHEELS
                    veh.SetMod(25, (byte)mod.plate_holder);
                    veh.SetMod(26, (byte)mod.plate_vanity);
                    veh.SetMod(27, (byte)mod.trim_design);
                    veh.SetMod(28, (byte)mod.ornaments);
                    veh.SetMod(30, (byte)mod.dial_design);
                    veh.SetMod(31, (byte)mod.door_interior);
                    veh.SetMod(32, (byte)mod.seats);
                    veh.SetMod(33, (byte)mod.steering_wheel);
                    veh.SetMod(34, (byte)mod.shift_lever);
                    veh.SetMod(35, (byte)mod.plaques);
                    veh.SetMod(36, (byte)mod.rear_shelf);
                    veh.SetMod(37, (byte)mod.trunk);
                    veh.SetMod(38, (byte)mod.hydraulics);
                    veh.SetMod(39, (byte)mod.engine_block);
                    veh.SetMod(40, (byte)mod.airfilter);
                    veh.SetMod(41, (byte)mod.strut_bar);
                    veh.SetMod(42, (byte)mod.arch_cover);
                    veh.SetMod(43, (byte)mod.antenna);
                    veh.SetMod(44, (byte)mod.exterior_parts);
                    veh.SetMod(45, (byte)mod.tank);
                    veh.SetMod(46, (byte)mod.door);
                    veh.SetMod(47, (byte)mod.rear_hydraulics);
                    veh.SetMod(48, (byte)mod.livery);
                    veh.SetWheels((byte)mod.wheel_type, veh.WheelVariation);
                    veh.SetWheels(veh.WheelType, (byte)mod.wheels);
                    veh.WheelColor = (byte)mod.wheelcolor;
                    veh.WindowTint = (byte)mod.window_tint;
                    veh.NumberplateIndex = (byte)mod.plate_color;
                    veh.PrimaryColor = (byte)mod.colorPrimaryType;
                    veh.SecondaryColor = (byte)mod.colorSecondaryType;
                    veh.PearlColor = (byte)mod.colorPearl;
                    veh.InteriorColor = (byte)mod.interior_color;
                    veh.HeadlightColor = (byte)mod.headlightColor;
                    if (mod.neon == 0) veh.SetNeonActive(false, false, false, false);
                    else veh.SetNeonActive(true, true, true, true);

                    veh.PrimaryColorRgb = new Rgba((byte)mod.colorPrimary_r, (byte)mod.colorPrimary_g, (byte)mod.colorPrimary_b, 255);
                    veh.SecondaryColorRgb = new Rgba((byte)mod.colorSecondary_r, (byte)mod.colorSecondary_g, (byte)mod.colorSecondary_b, 255);
                    veh.NeonColor = new Rgba((byte)mod.neon_r, (byte)mod.neon_g, (byte)mod.neon_b, 255);
                    veh.TireSmokeColor = new Rgba((byte)mod.smoke_r, (byte)mod.smoke_g, (byte)mod.smoke_b, 255);
                }

            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static void InstallBoughtMod(IVehicle veh, int modType, int modId)
        {
            try
            {
                if (veh == null || !veh.Exists) return;
                if (veh.GetVehicleId() <= 0) return;
                var mod = ServerVehiclesMod_.FirstOrDefault(x => x.vehId == (int)veh.GetVehicleId());
                if (mod != null)
                {
                    veh.ModKit = 1;
                    switch (modType)
                    {
                        case 0: mod.spoiler = modId; veh.SetMod((byte)modType, (byte)modId); break;
                        case 1: mod.front_bumper = modId; veh.SetMod((byte)modType, (byte)modId); break;
                        case 2: mod.rear_bumper = modId; veh.SetMod((byte)modType, (byte)modId); break;
                        case 3: mod.side_skirt = modId; veh.SetMod((byte)modType, (byte)modId); break;
                        case 4: mod.exhaust = modId; veh.SetMod((byte)modType, (byte)modId); break;
                        case 5: mod.frame = modId; veh.SetMod((byte)modType, (byte)modId); break;
                        case 6: mod.grille = modId; veh.SetMod((byte)modType, (byte)modId); break;
                        case 7: mod.hood = modId; veh.SetMod((byte)modType, (byte)modId); break;
                        case 8: mod.fender = modId; veh.SetMod((byte)modType, (byte)modId); break;
                        case 9: mod.right_fender = modId; veh.SetMod((byte)modType, (byte)modId); break;
                        case 10: mod.roof = modId; veh.SetMod((byte)modType, (byte)modId); break;
                        case 11: mod.engine = modId; veh.SetMod((byte)modType, (byte)modId); break;
                        case 12: mod.brakes = modId; veh.SetMod((byte)modType, (byte)modId); break;
                        case 13: mod.transmission = modId; veh.SetMod((byte)modType, (byte)modId); break;
                        case 14: mod.horns = modId; veh.SetMod((byte)modType, (byte)modId); break;
                        case 15: mod.suspension = modId; veh.SetMod((byte)modType, (byte)modId); break;
                        case 16: mod.armor = modId; veh.SetMod((byte)modType, (byte)modId); break;
                        case 18: mod.turbo = modId; veh.SetMod((byte)modType, (byte)modId); break;
                        case 20: mod.smoke = modId; veh.SetMod((byte)modType, (byte)modId); break;
                        case 22: mod.xenon = modId; veh.SetMod((byte)modType, (byte)modId); break;
                        case 23: mod.wheels = modId; veh.SetWheels(0, (byte)modId); break;
                        case 132: mod.wheelcolor = modId; veh.WheelColor = (byte)modId; break;
                        //case 24: mod.back_wheels = modId; veh.SetWheels(0, (byte)modId); break; BACK WHEELS
                        case 25: mod.plate_holder = modId; veh.SetMod((byte)modType, (byte)modId); break;
                        case 26: mod.plate_vanity = modId; veh.SetMod((byte)modType, (byte)modId); break;
                        case 27: mod.trim_design = modId; veh.SetMod((byte)modType, (byte)modId); break;
                        case 28: mod.ornaments = modId; veh.SetMod((byte)modType, (byte)modId); break;
                        case 30: mod.dial_design = modId; veh.SetMod((byte)modType, (byte)modId); break;
                        case 31: mod.door_interior = modId; veh.SetMod((byte)modType, (byte)modId); break;
                        case 32: mod.seats = modId; veh.SetMod((byte)modType, (byte)modId); break;
                        case 33: mod.steering_wheel = modId; veh.SetMod((byte)modType, (byte)modId); break;
                        case 34: mod.shift_lever = modId; veh.SetMod((byte)modType, (byte)modId); break;
                        case 35: mod.plaques = modId; veh.SetMod((byte)modType, (byte)modId); break;
                        case 36: mod.rear_shelf = modId; veh.SetMod((byte)modType, (byte)modId); break;
                        case 37: mod.trunk = modId; veh.SetMod((byte)modType, (byte)modId); break;
                        case 38: mod.hydraulics = modId; veh.SetMod((byte)modType, (byte)modId); break;
                        case 39: mod.engine_block = modId; veh.SetMod((byte)modType, (byte)modId); break;
                        case 40: mod.airfilter = modId; veh.SetMod((byte)modType, (byte)modId); break;
                        case 41: mod.strut_bar = modId; veh.SetMod((byte)modType, (byte)modId); break;
                        case 42: mod.arch_cover = modId; veh.SetMod((byte)modType, (byte)modId); break;
                        case 43: mod.antenna = modId; veh.SetMod((byte)modType, (byte)modId); break;
                        case 44: mod.exterior_parts = modId; veh.SetMod((byte)modType, (byte)modId); break;
                        case 45: mod.tank = modId; veh.SetMod((byte)modType, (byte)modId); break;
                        case 46: mod.door = modId; veh.SetMod((byte)modType, (byte)modId); break;
                        case 47: mod.rear_hydraulics = modId; veh.SetMod((byte)modType, (byte)modId); break;
                        case 48: mod.livery = modId; veh.SetMod((byte)modType, (byte)modId); veh.Livery = (byte)modId; break;
                        case 50: mod.wheel_type = modId; veh.SetWheels((byte)modId, veh.WheelVariation); break;
                        case 51: mod.wheels = modId; veh.SetWheels(veh.WheelType, (byte)modId); break;
                        case 52: mod.wheelcolor = modId; veh.WheelColor = (byte)modId; break;
                        case 53: mod.window_tint = modId; veh.WindowTint = (byte)modId; break;
                        case 54: mod.plate_color = modId; veh.NumberplateIndex = (uint)modId; break;
                        case 55: mod.colorPrimaryType = modId; veh.PrimaryColor = (byte)modId; break;
                        case 59: mod.colorSecondary_b = modId; veh.SecondaryColor = (byte)modId; break;
                        case 63: mod.colorPearl = modId; veh.PearlColor = (byte)modId; break;
                        case 80: mod.interior_color = modId; veh.InteriorColor = (byte)modId; break;
                        case 81: mod.neon = modId; if (modId == 0) veh.SetNeonActive(false, false, false, false); else veh.SetNeonActive(true, true, true, true); break;
                        case 82: mod.neon_r = modId; veh.NeonColor = new Rgba((byte)mod.neon_r, (byte)mod.neon_g, (byte)mod.neon_b, 255); break;
                        case 83: mod.neon_g = modId; veh.NeonColor = new Rgba((byte)mod.neon_r, (byte)mod.neon_g, (byte)mod.neon_b, 255); break;
                        case 84: mod.neon_b = modId; veh.NeonColor = new Rgba((byte)mod.neon_r, (byte)mod.neon_g, (byte)mod.neon_b, 255); break;
                        case 85: mod.smoke_r = modId; veh.TireSmokeColor = new Rgba((byte)mod.smoke_r, (byte)mod.smoke_g, (byte)mod.smoke_b, 255); break;
                        case 86: mod.smoke_g = modId; veh.TireSmokeColor = new Rgba((byte)mod.smoke_r, (byte)mod.smoke_g, (byte)mod.smoke_b, 255); break;
                        case 87: mod.smoke_b = modId; veh.TireSmokeColor = new Rgba((byte)mod.smoke_r, (byte)mod.smoke_g, (byte)mod.smoke_b, 255); break;
                        case 88: mod.headlightColor = modId; veh.HeadlightColor = (byte)modId; break;

                    }

                    using (gtaContext db = new gtaContext())
                    {
                        db.Server_Vehicles_Mods.Update(mod);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static void InstallBoughtModRgb(IVehicle veh, int modType, int colorR, int colorG, int colorB)
        {
            try
            {
                if (veh == null || !veh.Exists) return;
                if (veh.GetVehicleId() <= 0) return;
                var mod = ServerVehiclesMod_.FirstOrDefault(x => x.vehId == (int)veh.GetVehicleId());
                if (mod != null)
                {
                    veh.ModKit = 1;
                    switch (modType)
                    {
                        case 100: mod.colorPrimary_r = colorR; mod.colorPrimary_g = colorG; mod.colorPrimary_b = colorB; veh.PrimaryColorRgb = new Rgba((byte)colorR, (byte)colorG, (byte)colorB, 255); break;
                        case 200: mod.colorSecondary_r = colorR; mod.colorSecondary_g = colorG; mod.colorSecondary_b = colorB; veh.SecondaryColorRgb = new Rgba((byte)colorR, (byte)colorG, (byte)colorB, 255); break;
                        case 300: mod.neon_r = colorR; mod.neon_g = colorG; mod.neon_b = colorB; veh.NeonColor = new Rgba((byte)colorR, (byte)colorG, (byte)colorB, 255); break;
                        case 400: mod.smoke_r = colorR; mod.smoke_g = colorG; mod.smoke_b = colorB; veh.TireSmokeColor = new Rgba((byte)colorR, (byte)colorG, (byte)colorB, 255); break;
                    }

                    using (gtaContext db = new gtaContext())
                    {
                        db.Server_Vehicles_Mods.Update(mod);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }


        public static void CreateVehicle(ulong hash, int charid, int vehtype, int faction, bool isInGarage, int garageId, Position pos, Rotation rot, string plate, int colorR, int colorG, int colorB)
        {
            try
            {
                var nVehicle = new Server_Vehicles
                {
                    charid = charid,
                    hash = hash,
                    vehType = vehtype,
                    faction = faction,
                    fuel = GetVehicleFuelLimitOnHash(hash),
                    KM = 0f,
                    engineState = false,
                    isEngineHealthy = true,
                    lockState = true,
                    isInGarage = isInGarage,
                    garageId = garageId,
                    posX = pos.X,
                    posY = pos.Y,
                    posZ = pos.Z,
                    rotX = rot.Pitch,
                    rotY = rot.Roll,
                    rotZ = rot.Yaw,
                    plate = plate,
                    lastUsage = DateTime.Now,
                    buyDate = DateTime.Now
                };
                ServerVehicles_.Add(nVehicle);

                using (gtaContext db = new gtaContext())
                {
                    db.Server_Vehicles.Add(nVehicle);
                    db.SaveChanges();
                }

                if (vehtype != 2) { AddVehicleModToList(nVehicle.id, nVehicle.id, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 255, 255, 255, 0, 0, 0, 0, 0, colorR, colorG, colorB, colorR, colorG, colorB, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0); }
                if (isInGarage) return;
                ClassicVehicle veh = (ClassicVehicle)Alt.CreateVehicle((uint)hash, pos, rot);
                veh.NumberplateText = plate;
                veh.LockState = VehicleLockState.Locked;
                veh.EngineOn = false;
                veh.SetVehicleId((ulong)nVehicle.id);
                veh.SetVehicleTrunkState(false);
                if (vehtype != 2) { SetVehicleModsCorrectly(veh); }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static void RemoveVehiclePermanently(IVehicle veh)
        {
            try
            {
                if (veh == null || !veh.Exists) return;
                ulong vehID; vehID = veh.GetVehicleId();
                if (vehID <= 0) return;
                var mod = ServerVehiclesMod_.FirstOrDefault(x => x.vehId == (int)vehID);
                var vehItems = ServerVehicleTrunkItems_.Where(x => x.vehId == (int)vehID).ToList();
                var vehDb = ServerVehicles_.FirstOrDefault(x => x.id == (int)vehID);
                using (gtaContext db = new gtaContext())
                {
                    foreach (var item in vehItems)
                    {
                        db.Server_Vehicle_Items.Remove(item);
                        ServerVehicleTrunkItems_.Remove(item);
                    }

                    if (mod != null)
                    {
                        db.Server_Vehicles_Mods.Remove(mod);
                        ServerVehiclesMod_.Remove(mod);
                    }

                    if (vehDb != null)
                    {
                        db.Server_Vehicles.Remove(vehDb);
                        ServerVehicles_.Remove(vehDb);
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }
    }
}