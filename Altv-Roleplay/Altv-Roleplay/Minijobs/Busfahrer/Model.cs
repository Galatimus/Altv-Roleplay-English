using Altv_Roleplay.models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using Altv_Roleplay.Model;
using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using Altv_Roleplay.Utils;
using Altv_Roleplay.Factories;

namespace Altv_Roleplay.Minijobs.Busfahrer
{
    class Model
    {
        public static List<Server_Minijob_Busdriver_Routes> ServerMinijobBusdriverRoutes_ = new List<Server_Minijob_Busdriver_Routes>();
        public static List<Server_Minijob_Busdriver_Spots> ServerMinijobBusdriverSpots_ = new List<Server_Minijob_Busdriver_Spots>();

        public static string GetAvailableRoutes(int charId)
        {
            if (charId <= 0) return "[]";
            var items = ServerMinijobBusdriverRoutes_.Where(x => CharactersMinijobs.GetCharacterMinijobEXP(charId, "Busfahrer") >= x.neededExp).Select(x => new
            {
                x.id,
                x.routeId,
                x.routeName,
                x.hash,
                x.neededExp,
                x.givenExp,
                x.paycheck,
                x.neededTime,
            }).ToList();

            return JsonConvert.SerializeObject(items);
        }

        public static bool ExistRoute(int routeId)
        {
            try
            {
                if (routeId <= 0) return false;
                var route = ServerMinijobBusdriverRoutes_.FirstOrDefault(x => x.routeId == routeId);
                if (route != null) return true;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return false;
        }

        public static ulong GetRouteVehicleHash(int routeId)
        {
            try
            {
                if (routeId <= 0) return 0;
                var route = ServerMinijobBusdriverRoutes_.FirstOrDefault(x => x.routeId == routeId);
                if (route != null) return route.hash;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return 0;
        }

        public static int GetRouteNeededEXP(int routeId)
        {
            try
            {
                if (routeId <= 0) return 0;
                var route = ServerMinijobBusdriverRoutes_.FirstOrDefault(x => x.routeId == routeId);
                if (route != null) return route.neededExp;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return 0;
        }

        public static int GetRouteGivenEXP(int routeId)
        {
            try
            {
                if (routeId <= 0) return 0;
                var route = ServerMinijobBusdriverRoutes_.FirstOrDefault(x => x.routeId == routeId);
                if (route != null) return route.givenExp;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return 0;
        }

        public static int GetRouteGivenMoney(int routeId)
        {
            try
            {
                if (routeId <= 0) return 0;
                var route = ServerMinijobBusdriverRoutes_.FirstOrDefault(x => x.routeId == routeId);
                if (route != null) return route.paycheck;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return 0;
        }

        internal static void CreateMinijobRouteSpot(int id, int routeId, int spotId, Position position)
        {
            try
            {
                var spotData = new Server_Minijob_Busdriver_Spots
                {
                    id = id,
                    routeId = routeId,
                    spotId = spotId,
                    posX = position.X,
                    posY = position.Y,
                    posZ = position.Z,
                    destinationColshape = Alt.CreateColShapeSphere(position, 3f)
                };
                ServerMinijobBusdriverSpots_.Add(spotData);

                foreach(var item in ServerMinijobBusdriverSpots_)
                {
                    ((ClassicColshape)item.destinationColshape).Radius = 3f;
                }
            }
            catch(Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static Server_Minijob_Busdriver_Spots GetCharacterMinijobNextSpot(IPlayer player)
        {
            if (player == null || !player.Exists) return null;
            var spot = ServerMinijobBusdriverSpots_.FirstOrDefault(x => x.routeId == (int)player.GetPlayerCurrentMinijobRouteId() && x.spotId == (int)player.GetPlayerCurrentMinijobActionCount());
            if (spot != null) return spot;
            return null;
        }

        public static int GetMinijobMaxRouteSpots(int routeId)
        {
            try
            {
                if (routeId <= 0) return 0;
                var route = ServerMinijobBusdriverSpots_.Where(x => x.routeId == routeId).ToList();
                if (route != null) return route.Count();
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return 0;
        }
    }
}
