using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using Altv_Roleplay.Factories;
using Altv_Roleplay.models;
using Altv_Roleplay.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altv_Roleplay.Minijobs.Müllmann
{
    class Model
    {
        public static List<Server_Minijob_Garbage_Spots> ServerMinijobGarbageSpots_ = new List<Server_Minijob_Garbage_Spots>();

        public static void CreateMinijobGarbageSpot(int id, int routeId, int spotId, Position position)
        {
            var spotData = new Server_Minijob_Garbage_Spots
            {
                id = id,
                routeId = routeId,
                spotId = spotId,
                posX = position.X,
                posY = position.Y,
                posZ = position.Z,
                destinationColshape = Alt.CreateColShapeSphere(position, 1.5f)
            };
            ServerMinijobGarbageSpots_.Add(spotData);

            foreach(var item in ServerMinijobGarbageSpots_)
            {
                ((ClassicColshape)item.destinationColshape).Radius = 1.5f;
            }
        }

        public static int GetMinijobGarbageMaxRouteSpots(int routeId)
        {
            try
            {
                if (routeId <= 0) return 0;
                var route = ServerMinijobGarbageSpots_.Where(x => x.routeId == routeId).ToList();
                if (route != null) return route.Count();
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return 0;
        }

        public static int GetMinijobGarbageMaxRoutes()
        {
            try
            {
                var uniqueRoutes = ServerMinijobGarbageSpots_.GroupBy(x => x.routeId).Select(x => x.FirstOrDefault()).ToList();
                if (!uniqueRoutes.Any()) return 0;
                return uniqueRoutes.Count();
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
            return 0;
        }

        public static Server_Minijob_Garbage_Spots GetCharacterMinijobNextSpot(IPlayer player)
        {
            if (player == null || !player.Exists) return null;
            var spot = ServerMinijobGarbageSpots_.FirstOrDefault(x => x.routeId == (int)player.GetPlayerCurrentMinijobRouteId() && x.spotId == (int)player.GetPlayerCurrentMinijobActionCount());
            if (spot != null) return spot;
            return null;
        }
    }
}
