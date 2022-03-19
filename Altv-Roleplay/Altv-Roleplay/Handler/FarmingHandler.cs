using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using AltV.Net.Resources.Chat.Api;
using Altv_Roleplay.Factories;
using Altv_Roleplay.Model;
using Altv_Roleplay.models;
using Altv_Roleplay.Utils;

namespace Altv_Roleplay.Handler
{
    class FarmingHandler : IScript
    {
        internal static async void FarmFieldAction(IPlayer player, string itemName, int itemMinAmount, int itemMaxAmount, string animation, int duration)
        {
            if (player == null || !player.Exists || itemName == "" || itemMinAmount == 0 || itemMaxAmount == 0 || animation == "") return;
            int charId = User.GetPlayerOnline(player); 
            if (charId <= 0) return;
            if (player.HasPlayerHandcuffs() || player.HasPlayerRopeCuffs()) { HUDHandler.SendNotification(player, 3, 5000, "Wie willst du das mit Handschellen/Fesseln machen?"); return; }
            if (itemName.Contains("Eisenerz"))
            {
                if (!CharactersInventory.ExistCharacterItem(charId, "Spitzhacke", "inventory"))
                {
                    HUDHandler.SendNotification(player, 4, 5000, $"Dir Fehlt folgendes Item: *Spitzhacke*");
                }
            }

            InventoryHandler.InventoryAnimation(player, animation, duration);
            await Task.Delay(duration + 1250);
            lock (player)
            {
                player.SetPlayerFarmingActionMeta("None");
            }
                int rndItemAmount = new Random().Next(itemMinAmount, itemMaxAmount);
                //Doppelte Menge aufsammeln
                if (Characters.IsCharacterFastFarm(charId)) rndItemAmount += 1;
                float itemWeight = ServerItems.GetItemWeight(itemName) * rndItemAmount;
                float invWeight = CharactersInventory.GetCharacterItemWeight(charId, "inventory");
                float backpackWeight = CharactersInventory.GetCharacterItemWeight(charId, "backpack");
                if (invWeight + itemWeight > 15f || backpackWeight + itemWeight > Characters.GetCharacterBackpackSize(Characters.GetCharacterBackpack(charId))) { HUDHandler.SendNotification(player, 3, 5000, $"Du hast nicht genug Platz in deinen Taschen."); return; }

            if (invWeight + itemWeight <= 15f)
                {
                    HUDHandler.SendNotification(player, 2, 5000, $"Du hast {itemName} ({rndItemAmount}x) gesammelt (Lagerort: Inventar).");
                    CharactersInventory.AddCharacterItem(charId, itemName, rndItemAmount, "inventory");
                    return;
                }

            if (Characters.GetCharacterBackpack(charId) != -2 && backpackWeight + itemWeight <= Characters.GetCharacterBackpackSize(Characters.GetCharacterBackpack(charId)))
            {
                    HUDHandler.SendNotification(player, 2, 5000, $"Du hast {itemName} ({rndItemAmount}x) gesammelt (Lagerort: Rucksack / Tasche).");
                    CharactersInventory.AddCharacterItem(charId, itemName, rndItemAmount, "backpack");
                    return;
                }
        }

        public static void openFarmingCEF(IPlayer player,string neededItem, string producedItem, int neededItemAmount, int producedItemAmount, int duration, string neededItemTWO, string neededItemTHREE, int neededItemTWOAmount, int neededItemTHREEAmount)
        {
            try
            {
                player.EmitLocked("Client:Farming:createCEF", neededItem, producedItem, neededItemAmount, producedItemAmount, duration, neededItemTWO, neededItemTHREE, neededItemTWOAmount, neededItemTHREEAmount);
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        internal static async void ProduceItem(IPlayer player, string neededItem, string producedItem, int neededItemAmount, int producedItemAmount, int duration, string neededItemTWO, string neededItemTHREE, int neededItemTWOAmount, int neededItemTHREEAmount)
        {
            try
            {
                if (player == null || !player.Exists || neededItem == "" || producedItem == "" || neededItemAmount == 0 || producedItemAmount == 0 || duration < 0) return;
                int charId = User.GetPlayerOnline(player);
                int hasItemSlot = 1;
                if (charId == 0) return;
                if (neededItemTWO != "none") hasItemSlot = 2;
                if (neededItemTHREE != "none") hasItemSlot = 3;

                if (producedItem.Contains("Schutzweste"))
                {
                    if (!CharactersInventory.ExistCharacterItem(charId, "Nadel und Faden", "inventory") || CharactersInventory.ExistCharacterItem(charId, "Nadel und Faden", "backpack"))
                    {
                        HUDHandler.SendNotification(player, 4, 5000, $"Dir Fehlt folgendes Item: *Nadel und Faden*");
                        return;
                    }
                }
                if (producedItem.Contains("Wasser"))
                {
                    if (!CharactersInventory.ExistCharacterItem(charId, "Trichter", "inventory") || CharactersInventory.ExistCharacterItem(charId, "Trichter", "backpack"))
                    {
                        HUDHandler.SendNotification(player, 4, 5000, $"Dir Fehlt folgendes Item: *Trichter*");
                        return;
                    }
                }

                if (!CharactersInventory.ExistCharacterItem(charId, neededItem, "inventory") && !CharactersInventory.ExistCharacterItem(charId, neededItem, "backpack")) { HUDHandler.SendNotification(player, 4, 5000, $"Du hast nicht die richtigen Gegenstände, um " + neededItem + " zu verarbeiten."); return;}; ; //Item existiert nicht, abbrechen.
                if (hasItemSlot == 2)
                {
                    if (!CharactersInventory.ExistCharacterItem(charId, neededItemTWO, "inventory") && !CharactersInventory.ExistCharacterItem(charId, neededItemTWO, "backpack")) { HUDHandler.SendNotification(player, 4, 5000, $"Du hast nicht die richtigen Gegenstände, um " + neededItem + " zu verarbeiten."); return; }; ; //Item existiert nicht, abbrechen.
                } else if (hasItemSlot == 3) {
                    if (!CharactersInventory.ExistCharacterItem(charId, neededItemTHREE, "inventory") && !CharactersInventory.ExistCharacterItem(charId, neededItemTHREE, "backpack")) { HUDHandler.SendNotification(player, 4, 5000, $"Du hast nicht die richtigen Gegenstände, um " + neededItem + " zu verarbeiten."); return; }; ; //Item existiert nicht, abbrechen.
                }
                float itemWeight = ServerItems.GetItemWeight(producedItem) * producedItemAmount;
                float invWeight = CharactersInventory.GetCharacterItemWeight(charId, "inventory");
                float backpackWeight = CharactersInventory.GetCharacterItemWeight(charId, "backpack");
                if (invWeight + itemWeight > 15f && backpackWeight + itemWeight > Characters.GetCharacterBackpackSize(Characters.GetCharacterBackpack(charId))) { HUDHandler.SendNotification(player, 3, 5000, $"Du hast nicht genug Platz in deinen Taschen."); return; }
                int invAmount = CharactersInventory.GetCharacterItemAmount(charId, neededItem, "inventory"); //Anzahl an neededItem im Inventar
                int backpackAmount = CharactersInventory.GetCharacterItemAmount(charId, neededItem, "backpack"); //Anzahl an neededItem im Rucksack
                int finalAmount = invAmount + backpackAmount; //Zusammengerechnete Anzahl von neededItems.
                int giveInvItems = 0;
                int giveBackItems = 0;
                int removeInvItems = 0;
                int removeBackItems = 0;
                int removeInvItemsTWO = 0;
                int removeInvItemsTHREE = 0;
                int removeBackItemsTWO = 0;
                int removeBackItemsTHREE = 0;
                if (invAmount <= 0 && backpackAmount <= 0){ HUDHandler.SendNotification(player, 3, 5000, $"Es ist ein Fehler aufgetreten."); return; } //Abbrechen wenn Anzahl von beiden 0 ist = existiert nicht.
                if (finalAmount < neededItemAmount || finalAmount < neededItemTWOAmount || finalAmount < neededItemTHREEAmount)  { HUDHandler.SendNotification(player, 3, 5000, $"Du hast nicht die nötigen Gegenstände dabei, um {neededItem} zu verarbeiten!"); return; } //Spieler hat nicht genug Gegenstände dabei.
                if (invAmount < neededItemAmount && backpackAmount < neededItemAmount || invAmount < neededItemTWOAmount && backpackAmount < neededItemTWOAmount || invAmount < neededItemTHREEAmount && backpackAmount < neededItemTHREEAmount) { HUDHandler.SendNotification(player, 3, 5000, $"Du benötigst mindestens {neededItemAmount} Gegenstände in der gleichen Tasche."); return; }
                player.SetPlayerFarmingActionMeta("produce");
                int InventoryNeededItemCount = 0;
                int BackpackNeededItemCount = 0;
                if (invAmount >= neededItemAmount)
                {
                    int availableNeededItems = invAmount / neededItemAmount;
                    giveInvItems = availableNeededItems * producedItemAmount;
                    removeInvItems = availableNeededItems * neededItemAmount;
                    InventoryNeededItemCount = availableNeededItems;
                }
                if (backpackAmount >= neededItemAmount)
                {
                    int availableNeededItems = backpackAmount / neededItemAmount;
                    giveBackItems = availableNeededItems * producedItemAmount;
                    removeBackItems = availableNeededItems * neededItemAmount;
                    BackpackNeededItemCount = availableNeededItems;
                }

                if (hasItemSlot >= 2)
                {
                    if (invAmount >= neededItemTWOAmount)
                    {
                        int availableNeededItems = invAmount / neededItemTWOAmount;
                        removeInvItemsTWO = InventoryNeededItemCount * neededItemTWOAmount;
                    }
                    if (backpackAmount >= neededItemTWOAmount)
                    {
                        int availableNeededItems = backpackAmount / neededItemTWOAmount;
                        removeBackItemsTWO = BackpackNeededItemCount * neededItemTWOAmount;
                    }
                }

                if (hasItemSlot >= 3)
                {
                    if (invAmount >= neededItemTHREEAmount)
                    {
                        int availableNeededItems = invAmount / neededItemTHREEAmount;
                        removeInvItemsTHREE = InventoryNeededItemCount * neededItemTHREEAmount;
                    }
                    if (backpackAmount >= neededItemTHREEAmount)
                    {
                        int availableNeededItems = backpackAmount / neededItemTHREEAmount;
                        removeBackItemsTHREE = BackpackNeededItemCount * neededItemTHREEAmount;
                    }
                }
                

                Position ProducerPos = player.Position;
                int finalDuration = (removeInvItems + removeBackItems) * duration;
                HUDHandler.SendNotification(player, 1, finalDuration, $"Verarbeitung von {neededItem} wurde gestartet...");
                await Task.Delay(finalDuration);
                // Amount welchen man am Abschluss vom verarbeiten hat sprich sollte da was weggelegt werden wird der kleiner als der InvAmount und somit hat jemand probiert zu Dupen
                int antiDupeInvAmount = CharactersInventory.GetCharacterItemAmount(charId, neededItem, "inventory"); //Anzahl an neededItem im Inventar beim verarbeiten
                int antiDupeBackpackAmount = CharactersInventory.GetCharacterItemAmount(charId, neededItem, "backpack"); //Anzahl an neededItem im Rucksack beim verarbeiten
                int antiDupeInvAmountTWO = CharactersInventory.GetCharacterItemAmount(charId, neededItem, "inventory"); //Anzahl an neededItem im Inventar beim verarbeiten
                int antiDupeBackpackAmountTWO = CharactersInventory.GetCharacterItemAmount(charId, neededItem, "backpack"); //Anzahl an neededItem im Rucksack beim verarbeiten
                int antiDupeInvAmountTHREE = CharactersInventory.GetCharacterItemAmount(charId, neededItem, "inventory"); //Anzahl an neededItem im Inventar beim verarbeiten
                int antiDupeBackpackAmountTHREE = CharactersInventory.GetCharacterItemAmount(charId, neededItem, "backpack"); //Anzahl an neededItem im Rucksack beim verarbeiten
                if (!player.Position.IsInRange(ProducerPos, 3f)) { HUDHandler.SendNotification(player, 4, 5000, $"Du hast dich zu weit entfernt."); player.SetPlayerFarmingActionMeta("None"); return; }
                if (antiDupeInvAmount < invAmount || antiDupeBackpackAmount < backpackAmount || antiDupeInvAmountTWO < invAmount || antiDupeBackpackAmountTWO < backpackAmount || antiDupeInvAmountTHREE < invAmount || antiDupeBackpackAmountTHREE < backpackAmount) { HUDHandler.SendNotification(player, 3, 5000, $"Du darfst nichts wegwerfen während du verarbeitest!"); return; }
                lock (player)
                {
                    player.SetPlayerFarmingActionMeta("None");
                    HUDHandler.SendNotification(player, 1, 2500, $"Verarbeitung von {neededItem} ist nun abgeschlossen.");
                }
                CharactersInventory.RemoveCharacterItemAmount(charId, neededItem, removeInvItems, "inventory");
                CharactersInventory.RemoveCharacterItemAmount(charId, neededItem, removeBackItems, "backpack");
                CharactersInventory.AddCharacterItem(charId, producedItem, giveInvItems, "inventory");
                CharactersInventory.AddCharacterItem(charId, producedItem, giveBackItems, "backpack");

                if (hasItemSlot == 2 && hasItemSlot != 3)
                {
                    CharactersInventory.RemoveCharacterItemAmount(charId, neededItemTWO, removeInvItemsTWO, "inventory");
                    CharactersInventory.RemoveCharacterItemAmount(charId, neededItemTWO, removeBackItemsTWO, "backpack");
                } else if (hasItemSlot != 2 && hasItemSlot == 3)
                {
                    CharactersInventory.RemoveCharacterItemAmount(charId, neededItemTHREE, removeInvItemsTHREE, "inventory");
                    CharactersInventory.RemoveCharacterItemAmount(charId, neededItemTHREE, removeBackItemsTHREE, "backpack");
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }        
    }
}
