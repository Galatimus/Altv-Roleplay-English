using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using Altv_Roleplay.Factories;
using Altv_Roleplay.Model;
using Altv_Roleplay.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Altv_Roleplay.Handler
{
    class HUDHandler : IScript
    {

        public static void CreateHUDBrowser(IPlayer client)
        {
            if (client == null || !client.Exists) return;
            client.EmitLocked("Client:HUD:CreateCEF", Characters.GetCharacterArmor(User.GetPlayerOnline(client)), Characters.GetCharacterHealth(User.GetPlayerOnline(client)), Characters.GetCharacterHunger(User.GetPlayerOnline(client)), Characters.GetCharacterThirst(User.GetPlayerOnline(client)), CharactersInventory.GetCharacterItemAmount2(User.GetPlayerOnline(client), "Bargeld"), CharactersInventory.GetCharacterItemAmount2(User.GetPlayerOnline(client), "Schwarzgeld"));
            client.EmitLocked("Client:HUD:UpdateDesire", Characters.GetCharacterArmor(User.GetPlayerOnline(client)), Characters.GetCharacterHealth(User.GetPlayerOnline(client)), Characters.GetCharacterHunger(User.GetPlayerOnline(client)), Characters.GetCharacterThirst(User.GetPlayerOnline(client))); //HUD updaten
            client.EmitLocked("Client:HUD:updateMoney", CharactersInventory.GetCharacterItemAmount(User.GetPlayerOnline(client), "Bargeld", "inventory"));
        }

        [AsyncScriptEvent(ScriptEventType.PlayerEnterVehicle)]
        public async Task OnPlayerEnterVehicle_Handler(IVehicle vehicle, IPlayer client, byte seat)
        {
            try
            {
                if (client == null || !client.Exists) return;
                client.EmitLocked("Client:HUD:updateHUDPosInVeh", true, ServerVehicles.GetVehicleFuel(vehicle), ServerVehicles.GetVehicleKM(vehicle));
                client.EmitLocked("Client:HUD:GetDistanceForVehicleKM");
            }
            catch(Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [AsyncScriptEvent(ScriptEventType.PlayerLeaveVehicle)]
        public async Task OnPlayerLeaveVehicle_Handler(IVehicle vehicle, IPlayer client, byte seat)
        {
            try
            {
                if (client == null || !client.Exists) return;
                client.EmitLocked("Client:HUD:updateHUDPosInVeh", false, 0, 0);
            }
            catch(Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static void SendInformationToVehicleHUD(IPlayer player) {
            if (player == null || !player.Exists) return;
            IVehicle Veh = player.Vehicle;
            if (!Veh.Exists) return;
            ulong vehID = Veh.GetVehicleId();
            if (vehID == 0) return;
            player.EmitLocked("Client:HUD:SetPlayerHUDVehicleInfos", ServerVehicles.GetVehicleFuel(Veh), ServerVehicles.GetVehicleKM(Veh));  
        }

        public static void SendNotification(IPlayer client, int type, int duration, string msg, int delay = 0) //1 Info | 2 Success | 3 Warning | 4 Error
        {
            try
            {
                if (client == null || !client.Exists) return;
                client.EmitLocked("Client:HUD:sendNotification", type, duration, msg, delay);
            }
            catch(Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [AsyncClientEvent("Server:Vehicle:UpdateVehicleKM")]
        public async Task UpdateVehicleKM(IPlayer player, float km)
        {
            //KM = bei 600 Meter = 600
            //600 / 1000 = 0,6   = 0,6km ?
            try
            {
                if (player == null || !player.Exists || km <= 0) return;
                if (!player.IsInVehicle || player.Vehicle == null) return;
                float fKM = km / 1000;
                fKM = fKM + ServerVehicles.GetVehicleKM(player.Vehicle);
                ServerVehicles.SetVehicleKM(player.Vehicle, fKM);
            }
            catch(Exception e)
            {
                Alt.Log($"{e}");
            }
        }
    }
}
