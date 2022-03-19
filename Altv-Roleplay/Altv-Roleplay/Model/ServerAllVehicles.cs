using AltV.Net;
using Altv_Roleplay.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altv_Roleplay.Model
{
    class ServerAllVehicles
    {
        public static List<Server_All_Vehicles> ServerAllVehicles_ = new List<Server_All_Vehicles>();

        public static int GetVehicleTaxes(ulong hash)
        {
            int tax = 0;
            if (hash <= 0) return tax;
            var veh = ServerAllVehicles_.FirstOrDefault(x => x.hash == hash);
            if(veh != null)
            {
                tax = veh.tax;
            }
            return tax;
        }

        public static string GetVehicleNameOnHash(ulong hash)
        {
            string vehName = "undefined";
            if (hash == 0) return vehName;
            var vehs = ServerAllVehicles_.FirstOrDefault(x => x.hash == hash);
            if(vehs != null)
            {
                vehName = vehs.name;
            }
            return vehName;
        }

        public static int GetVehicleClass(ulong hash)
        {
            try
            {
                if (hash <= 0) return 0;
                var vehs = ServerAllVehicles_.FirstOrDefault(x => x.hash == hash);
                if(vehs != null)
                {
                    return vehs.vehClass;
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return 0;
        }
    }
}
