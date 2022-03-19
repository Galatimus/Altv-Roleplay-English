using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using Altv_Roleplay.Handler;
using Altv_Roleplay.models;
using System;
using System.Collections.Generic;

namespace Altv_Roleplay.Model
{
    class ServerBank
    {
        public static List<Server_Banks> ServerBank_ = new List<Server_Banks>();

        public static void CreateNewBank(IPlayer client, int maxmoney, Position pos, string zoneName)
        {
            if (client == null || !client.Exists) return;
            var ServerBankData = new Server_Banks
            {
                posX = pos.X,
                posY = pos.Y,
                posZ = pos.Z,
                zoneName = zoneName
            };

            try
            {
                ServerBank_.Add(ServerBankData);
                using (gtaContext db = new gtaContext())
                {
                    db.Server_Banks.Add(ServerBankData);
                    db.SaveChanges();
                }

                HUDHandler.SendNotification(client, 2, 5000, $"ATM in der Zone ({ServerBankData.zoneName}) an deiner Position erstellt.");
/*
                foreach (IPlayer player in Alt.GetAllPlayers())
                {
                    if (player == null || !player.Exists) return;
                    player.EmitLocked("Client:ServerBlips:AddNewBlip", "Bankautomat", 2, 0.8, true, 277, pos.X, pos.Y, pos.Z);
                }*/
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }
    }
}
