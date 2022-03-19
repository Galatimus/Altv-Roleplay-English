using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using Altv_Roleplay.Factories;
using Altv_Roleplay.Model;
using Altv_Roleplay.models;
using Altv_Roleplay.Utils;

namespace Altv_Roleplay.Handler
{
    class LaborHandler : IScript
    {
        public static async Task openLabor(ClassicPlayer player)
        {
            if (player == null || !player.Exists || User.GetPlayerOnline(player) <= 0 || player.Dimension <= 0 || !ServerFactions.IsCharacterInAnyFaction(User.GetPlayerOnline(player)) || !ServerFactions.existFaction(player.Dimension)) return;
            int factionId = player.Dimension;
            player.Emit("Client:Labor:openLabor", CharactersInventory.GetCharacterInventory(User.GetPlayerOnline(player)), ServerFactions.GetLaborItemsFromPlayer(factionId, User.GetPlayerOnline(player)));
        }

        [AsyncClientEvent("Server:Labor:switchItemToInventory")]
        public async Task switchItemToInventory(ClassicPlayer player, string itemName, int itemAmount)
        {
            // Labor -> Inventory
            if (player == null || !player.Exists || User.GetPlayerOnline(player) <= 0 || player.Dimension <= 0 || !ServerFactions.IsCharacterInAnyFaction(User.GetPlayerOnline(player)) || !ServerFactions.existFaction(player.Dimension) || itemName.Length <= 0 || itemAmount <= 0) return;
            int factionId = player.Dimension;
            if (itemAmount > ServerFactions.GetLaborItemAmount(factionId, User.GetPlayerOnline(player), itemName))
            {
                HUDHandler.SendNotification(player, 4, 1500, "Soviele Gegenstände sind nicht im Labor.");
                return;
            }

            float itemWeight = ServerItems.GetItemWeight(itemName) * itemAmount;
            if (CharactersInventory.GetCharacterItemWeight(User.GetPlayerOnline(player), "inventory") + itemWeight > 15f)
            {
                HUDHandler.SendNotification(player, 4, 1500, "Du hast nicht genügend Platz für diese Gegenstände.");
                return;
            }

            ServerFactions.RemoveLaborItemAmount(factionId, User.GetPlayerOnline(player), itemName, itemAmount);
            CharactersInventory.AddCharacterItem(User.GetPlayerOnline(player), itemName, itemAmount, "inventory");
            HUDHandler.SendNotification(player, 2, 2500, $"Du hast {itemAmount}x {itemName} aus dem Labor entnommen.");
        }

        [AsyncClientEvent("Server:Labor:switchItemToLabor")]
        public async Task switchItemToLabor(ClassicPlayer player, string itemName, int itemAmount)
        {
            if (player == null || !player.Exists || User.GetPlayerOnline(player) <= 0 || player.Dimension <= 0 || !ServerFactions.IsCharacterInAnyFaction(User.GetPlayerOnline(player)) || !ServerFactions.existFaction(player.Dimension) || itemName.Length <= 0 || itemAmount <= 0) return;
            int factionId = player.Dimension;

            if (itemName != "Batteriezellen" && itemName != "Hanfsamenpulver" && itemName != "Dünger" && itemName != "Batteriezellen" && itemName != "Ephedrinpulver" && itemName != "Toilettenreiniger")
            {
                HUDHandler.SendNotification(player, 2, 1500, $"Fehler: Diesen Gegenstand kannst du nicht einlagern ({itemName}).");
                return;
            }

            if (itemAmount > CharactersInventory.GetCharacterItemAmount(User.GetPlayerOnline(player), itemName, "inventory"))
            {
                HUDHandler.SendNotification(player, 4, 1500, "Soviele Gegenstände hast du nicht im Inventar. (Das Item soll nicht im Rucksack/Tasche sein)");
                return;
            }

            if ((itemName == "Batteriezellen" && ServerFactions.GetLaborItemAmount(factionId, User.GetPlayerOnline(player), "Batteriezellen") + itemAmount > 50) || (itemName == "Hanfsamenpulver" && ServerFactions.GetLaborItemAmount(factionId, User.GetPlayerOnline(player), "Hanfsamenpulver") + itemAmount > 50) || (itemName == "Dünger" && ServerFactions.GetLaborItemAmount(factionId, User.GetPlayerOnline(player), "Dünger") + itemAmount > 50) || (itemName == "Ephedrinpulver" && ServerFactions.GetLaborItemAmount(factionId, User.GetPlayerOnline(player), "Ephedrinpulver") + itemAmount > 50) || (itemName == "Toilettenreiniger" && ServerFactions.GetLaborItemAmount(factionId, User.GetPlayerOnline(player), "Toilettenreiniger") + itemAmount > 50))
            {
                HUDHandler.SendNotification(player, 2, 1500, $"Soviele Gegenstände kannst du nicht einlagern, maximal 50 von jedem (aktuell: {ServerFactions.GetLaborItemAmount(factionId, User.GetPlayerOnline(player), itemName)}x {itemName}).");
                return;
            }


            ServerFactions.AddLaborItem(factionId, User.GetPlayerOnline(player), itemName, itemAmount);
            CharactersInventory.RemoveCharacterItemAmount(User.GetPlayerOnline(player), itemName, itemAmount, "inventory");
            HUDHandler.SendNotification(player, 2, 2500, $"Du hast {itemAmount}x {itemName} in das Labor gelegt.");
        }
    }
}