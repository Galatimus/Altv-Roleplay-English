using AltV.Net;
using AltV.Net.Data;
using Altv_Roleplay.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altv_Roleplay.Model
{
    class ServerFuelStations
    {
        public static List<Server_Fuel_Stations> ServerFuelStations_ = new List<Server_Fuel_Stations>();
        public static List<Server_Fuel_Spots> ServerFuelStationSpots_ = new List<Server_Fuel_Spots>();

        public static string GetFuelStationName(int fuelId)
        {
            if (fuelId == 0) return "";
            var fs = ServerFuelStations_.FirstOrDefault(x => x.id == fuelId);
            if(fs != null)
            {
                return fs.name;
            }
            return "";
        }

        public static string GetFuelStationAvailableFuel(int fuelId)
        {
            if (fuelId == 0) return "";
            var fs = ServerFuelStations_.FirstOrDefault(x => x.id == fuelId);
            if(fs != null)
            {
                return fs.availableFuel;
            }
            return "";
        }

        public static void SetFuelStationBankMoney(int fuelId, int money)
        {
            try
            {
                if (fuelId == 0) return;
                var fs = ServerFuelStations_.FirstOrDefault(x => x.id == fuelId);
                if (fs != null)
                {
                    fs.bank = money;
                    using (gtaContext db = new gtaContext())
                    {
                        db.Server_Fuel_Stations.Update(fs);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static int GetFuelStationBankMoney(int fuelId)
        {
            if (fuelId == 0) return 0;
            var fs = ServerFuelStations_.FirstOrDefault(x => x.id == fuelId);
            if(fs != null) { return fs.bank; }
            return 0;
        }

        public static int GetFuelStationOwnerId(int fuelId)
        {
            if (fuelId == 0) return 0;
            var fs = ServerFuelStations_.FirstOrDefault(x => x.id == fuelId);
            if(fs != null)
            {
                return fs.owner;
            }
            return 0;
        }

        public static string GetFuelStationOwnerName(int ownerId)
        {
            if (ownerId == 0) return "Staat";
            var chars = Characters.PlayerCharacters.FirstOrDefault(x => x.charId == ownerId);
            if(chars != null)
            {
                return chars.charname;
            }
            return "Staat";
        }

        public static int GetFuelStationAvailableLiters(int fuelId)
        {
            if (fuelId == 0) return 0;
            var fs = ServerFuelStations_.FirstOrDefault(x => x.id == fuelId);
            if(fs != null)
            {
                return fs.availableLiters;
            }
            return 0;
        }

        public static void SetFuelStationAvailableLiters(int fuelId, int liter)
        {
            try
            {
                if (fuelId == 0) return;
                var fs = ServerFuelStations_.FirstOrDefault(x => x.id == fuelId);
                if (fs != null)
                {
                    fs.availableLiters = liter;
                    using (gtaContext db = new gtaContext())
                    {
                        db.Server_Fuel_Stations.Update(fs);
                        db.SaveChanges();
                    }
                }
            }
            catch(Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static int GetFuelSpotParentStation(int spotId)
        {
            if (spotId == 0) return 0;
            var spot = ServerFuelStationSpots_.FirstOrDefault(x => x.id == spotId);
            if(spot != null)
            {
                return spot.fuelStationId;
            }
            return 0;
        }

        public static Position GetFuelSpotPosition(int spotId)
        {
            if (spotId == 0) return new Position(0, 0, 0);
            var spot = ServerFuelStationSpots_.FirstOrDefault(x => x.id == spotId);
            if(spot != null)
            {
                return new Position(spot.posX, spot.posY, spot.posZ);
            }
            return new Position(0, 0, 0);
        }
    }
}
