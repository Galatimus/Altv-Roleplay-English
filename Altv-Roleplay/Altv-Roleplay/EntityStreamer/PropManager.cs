using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using Altv_Roleplay.Factories;
using Altv_Roleplay.Model;
using Altv_Roleplay.Services;
using Altv_Roleplay.Utils;

namespace Altv_Roleplay.Handler
{
    class DynastyHandler : IScript
    {
        [AsyncClientEvent("Server:Dynasty:buyStorage")]
        public async Task buyStorage(ClassicPlayer player, int storageId)
        {
            if (player == null || !player.Exists || User.GetPlayerOnline(player) <= 0 || !ServerStorages.ExistStorage(storageId) || ServerStorages.GetOwner(storageId) != 0) return;
            int price = ServerStorages.GetPrice(storageId);
            int charId = User.GetPlayerOnline(player);
            if (!CharactersInventory.ExistCharacterItem(charId, "Bargeld", "inventory")) { HUDHandler.SendNotification(player, 3, 5000, "[LaVie Lagersystem] <br><br> Du hast kein Bargeld dabei."); return; }
            if (CharactersInventory.GetCharacterItemAmount(charId, "Bargeld", "inventory") < price) { HUDHandler.SendNotification(player, 3, 5000, "[LaVie Lagersystem] <br><br> Du hast nicht genügend Bargeld dabei."); return; }
            CharactersInventory.RemoveCharacterItemAmount(charId, "Bargeld", price, "inventory");
            ServerStorages.SetOwner(storageId, User.GetPlayerOnline(player));
            ServerStorages.SetSecondOwner(storageId, 0);
            HUDHandler.SendNotification(player, 2, 1500, $"[LaVie Lagersystem] <br><br> Du hast die Lagerhalle {storageId} für {price}$ gekauft.");
        }

        [AsyncClientEvent("Server:Dynasty:sellStorage")]
        public async Task sellStorage(ClassicPlayer player, int storageId)
        {
            if (player == null || !player.Exists || User.GetPlayerOnline(player) <= 0 || !ServerStorages.ExistStorage(storageId) || ServerStorages.GetOwner(storageId) != User.GetPlayerOnline(player)) return;
            int price = ServerStorages.GetPrice(storageId) / 2;
            int charId = User.GetPlayerOnline(player);
            CharactersInventory.AddCharacterItem(charId, "Bargeld", price, "inventory");
            ServerStorages.SetOwner(storageId, 0);
            ServerStorages.SetSecondOwner(storageId, 0);
            ServerStorages.SetStorageLocked(storageId, true);
            HUDHandler.SendNotification(player, 2, 1500, $"[LaVie Lagersystem] <br><br> Du hast die Lagerhalle {storageId} für {price}$ verkauft.");
        }
    }
}