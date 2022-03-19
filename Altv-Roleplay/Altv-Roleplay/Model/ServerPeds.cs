using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using Altv_Roleplay.Handler;
using Altv_Roleplay.models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Altv_Roleplay.Model
{
    class ServerPeds
    {
        public static List<Server_Peds> ServerPeds_ = new List<Server_Peds>();

        public static void CreateServerPed(IPlayer client, string model, Position pos, float rotation)
        {
            if (client == null || !client.Exists) return;
            var ServerPedData = new Server_Peds
            {
                model = model,
                posX = pos.X,
                posY = pos.Y,
                posZ = pos.Z,
                rotation = rotation
            };

            try
            {
                ServerPeds_.Add(ServerPedData);

                using (gtaContext db = new gtaContext())
                {
                    db.Server_Peds.Add(ServerPedData);
                    db.SaveChanges();
                }

                HUDHandler.SendNotification(client, 2, 5000, $"Ped mit dem Model ({ServerPedData.model}) an deiner Position erstellt.");
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static string GetAllServerPeds()
        {
            var items = ServerPeds_.Select(x => new
            {
                model = x.model,
                posX = x.posX,
                posY = x.posY,
                posZ = x.posZ,
                rotation = x.rotation,
            }).ToList();

            return JsonConvert.SerializeObject(items);
        }
    }
}
