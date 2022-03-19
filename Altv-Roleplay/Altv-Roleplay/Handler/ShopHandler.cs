using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using Altv_Roleplay.Factories;
using Altv_Roleplay.Model;
using Altv_Roleplay.models;
using Altv_Roleplay.Services;
using Altv_Roleplay.Utils;
using Newtonsoft.Json;


namespace Altv_Roleplay.Handler
{
    class ShopHandler : IScript
    {

        #region Shops
        [AsyncClientEvent("Server:Shop:buyItem")]
        public async Task buyShopItem(IPlayer player, int shopId, int amount, string itemname)
        {
            if (player == null || !player.Exists || shopId <= 0 || amount <= 0 || itemname == "") return;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            if (player.HasPlayerHandcuffs() || player.HasPlayerRopeCuffs()) { HUDHandler.SendNotification(player, 3, 5000, "Wie willst du das mit Handschellen/Fesseln machen?"); return; }
            if (!player.Position.IsInRange(ServerShops.GetShopPosition(shopId), 3f)) { HUDHandler.SendNotification(player, 3, 5000, $"Du bist zu weit vom Shop entfernt."); return; }
            int charId = User.GetPlayerOnline(player);
            if (charId == 0) return;
            if (ServerShops.GetShopNeededLicense(shopId) != "None" && !Characters.HasCharacterPermission(charId, ServerShops.GetShopNeededLicense(shopId))) { HUDHandler.SendNotification(player, 3, 5000, $"Du hast hier keinen Zugriff drauf."); return; }
            float itemWeight = ServerItems.GetItemWeight(itemname) * amount;
            float invWeight = CharactersInventory.GetCharacterItemWeight(charId, "inventory");
            float backpackWeight = CharactersInventory.GetCharacterItemWeight(charId, "backpack");
            int itemPrice = ServerShopsItems.GetShopItemPrice(shopId, itemname) * amount;
            int shopFaction = ServerShops.GetShopFaction(shopId);
            if (ServerShopsItems.GetShopItemAmount(shopId, itemname) < amount) { HUDHandler.SendNotification(player, 3, 5000, $"Soviele Gegenstände hat der Shop nicht auf Lager."); return; }
            if (invWeight + itemWeight > 15f && backpackWeight + itemWeight > Characters.GetCharacterBackpackSize(Characters.GetCharacterBackpack(charId))) { HUDHandler.SendNotification(player, 3, 5000, $"Du hast nicht genug Platz in deinen Taschen."); return; }
            if (itemname.Contains("schluessel"))
            {
                CharactersInventory.AddCharacterItem(charId, itemname, amount, "schluessel");
                HUDHandler.SendNotification(player, 2, 5000, $"Du hast {itemname} ({amount}x) für {itemPrice} gekauft (Lagerort: Schluesselbund).");
                stopwatch.Stop();
                return;
            }
            if (invWeight + itemWeight <= 15f)
            {
                if (shopFaction > 0 && shopFaction != 0)
                {
                    if (!ServerFactions.IsCharacterInAnyFaction(charId)) { HUDHandler.SendNotification(player, 3, 2500, "Du hast hier keinen Zugriff drauf [CODE1-2]."); return; }
                    if (ServerFactions.GetCharacterFactionId(charId) != shopFaction) { HUDHandler.SendNotification(player, 3, 2500, $"Du hast hier keinen Zugriff drauf (Gefordert: {shopFaction} - Deine: {ServerFactions.GetCharacterFactionId(charId)}."); return; }
                    if (ServerFactions.GetFactionBankMoney(shopFaction) < itemPrice) { HUDHandler.SendNotification(player, 3, 2500, "Die Frakton hat nicht genügend Geld auf dem Fraktionskonto."); return; }
                    CharactersInventory.RemoveCharacterItemAmount(charId, "Bargeld", itemPrice, "inventory");
                    LoggingService.NewFactionLog(shopFaction, charId, 0, "shop", $"{Characters.GetCharacterName(charId)} hat {itemname} ({amount}x) für {itemPrice}$ erworben.");
                }
                else
                {
                    if (!CharactersInventory.ExistCharacterItem(charId, "Bargeld", "inventory") || CharactersInventory.GetCharacterItemAmount(charId, "Bargeld", "inventory") < itemPrice)
                    {
                        HUDHandler.SendNotification(player, 3, 2500, "Du hast nicht genügend Geld dabei.");
                        return;
                    }
                    CharactersInventory.RemoveCharacterItemAmount(charId, "Bargeld", itemPrice, "inventory");
                }

                CharactersInventory.AddCharacterItem(charId, itemname, amount, "inventory");
                HUDHandler.SendNotification(player, 2, 5000, $"Du hast {itemname} ({amount}x) für {itemPrice} gekauft (Lagerort: Inventar).");
                stopwatch.Stop();
                if (stopwatch.Elapsed.Milliseconds > 30) Alt.Log($"{charId} - buyShopItem benötigte {stopwatch.Elapsed.Milliseconds}ms");
                return;
            }

            if (Characters.GetCharacterBackpack(charId) != -2 && backpackWeight + itemWeight <= Characters.GetCharacterBackpackSize(Characters.GetCharacterBackpack(charId)))
            {
                if (shopFaction > 0 && shopFaction != 0)
                {
                    if (!ServerFactions.IsCharacterInAnyFaction(charId)) { HUDHandler.SendNotification(player, 3, 2500, "Du hast hier keinen Zugriff drauf [CODE1]."); return; }
                    if (ServerFactions.GetCharacterFactionId(charId) != shopFaction) { HUDHandler.SendNotification(player, 3, 2500, $"Du hast hier keinen Zugriff drauf (Gefordert: {shopFaction} - Deine: {ServerFactions.GetCharacterFactionId(charId)}."); return; }
                    if (ServerFactions.GetFactionBankMoney(shopFaction) < itemPrice) { HUDHandler.SendNotification(player, 3, 2500, "Die Frakton hat nicht genügend Geld auf dem Fraktionskonto."); return; }
                    ServerFactions.SetFactionBankMoney(shopFaction, ServerFactions.GetFactionBankMoney(shopFaction) - itemPrice);
                    LoggingService.NewFactionLog(shopFaction, charId, 0, "shop", $"{Characters.GetCharacterName(charId)} hat {itemname} ({amount}x) für {itemPrice}$ erworben.");
                }
                else
                {
                    if (!CharactersInventory.ExistCharacterItem(charId, "Bargeld", "inventory") || CharactersInventory.GetCharacterItemAmount(charId, "Bargeld", "inventory") < itemPrice)
                    {
                        HUDHandler.SendNotification(player, 3, 2500, "Du hast nicht genügend Geld dabei.");
                        return;
                    }
                    CharactersInventory.RemoveCharacterItemAmount(charId, "Bargeld", itemPrice, "inventory");
                }

                CharactersInventory.AddCharacterItem(charId, itemname, amount, "backpack");
                HUDHandler.SendNotification(player, 2, 5000, $"Du hast {itemname} ({amount}x) für {itemPrice} gekauft (Lagerort: Rucksack / Tasche).");
                stopwatch.Stop();
                if (stopwatch.Elapsed.Milliseconds > 30) Alt.Log($"{charId} - buyShopItem benötigte {stopwatch.Elapsed.Milliseconds}ms");
                return;
            }
        }

        internal static void openShop(IPlayer player, Server_Shops shopPos)
        {
            try
            {
                if (player == null || !player.Exists) return;
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                int charId = User.GetPlayerOnline(player);
                if (charId <= 0) return;

                if (shopPos.faction > 0 && shopPos.faction != 0)
                {
                    if (!ServerFactions.IsCharacterInAnyFaction(charId)) { HUDHandler.SendNotification(player, 3, 2500, "Kein Zugriff [1]"); return; }
                    if (ServerFactions.GetCharacterFactionId(charId) != shopPos.faction) { HUDHandler.SendNotification(player, 3, 2500, $"Kein Zugriff [{shopPos.faction} - {ServerFactions.GetCharacterFactionId(charId)}]"); return; }
                }

                if (shopPos.neededLicense != "None" && !Characters.HasCharacterPermission(charId, shopPos.neededLicense))
                {
                    HUDHandler.SendNotification(player, 3, 5000, $"Du hast hier keinen Zugriff drauf.");
                    stopwatch.Stop();
                    if (stopwatch.Elapsed.Milliseconds > 30) Alt.Log($"{charId} - openShop benötigte {stopwatch.Elapsed.Milliseconds}ms");
                    return;
                }

                if (shopPos.isOnlySelling == false)
                {
                    Global.mGlobal.VirtualAPI.TriggerClientEventSafe(player, "Client:Shop:shopCEFCreateCEF", ServerShopsItems.GetShopShopItems(shopPos.shopId), shopPos.shopId, shopPos.isOnlySelling);
                    stopwatch.Stop();
                    if (stopwatch.Elapsed.Milliseconds > 30) Alt.Log($"{charId} - openShop benötigte {stopwatch.Elapsed.Milliseconds}ms");
                    return;
                }
                else if (shopPos.isOnlySelling == true)
                {
                    Global.mGlobal.VirtualAPI.TriggerClientEventSafe(player, "Client:Shop:shopCEFCreateCEF", ServerShopsItems.GetShopSellItems(charId, shopPos.shopId), shopPos.shopId, shopPos.isOnlySelling);
                    stopwatch.Stop();
                    if (stopwatch.Elapsed.Milliseconds > 30) Alt.Log($"{charId} - openShop benötigte {stopwatch.Elapsed.Milliseconds}ms");
                    return;
                }
                stopwatch.Stop();
                if (stopwatch.Elapsed.Milliseconds > 30) Alt.Log($"{charId} - openShop benötigte {stopwatch.Elapsed.Milliseconds}ms");
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [AsyncClientEvent("Server:Shop:robShop")]
        public async Task robShop(ClassicPlayer player, int shopId)
        {
            try
            {
                if (player == null || !player.Exists || player.CharacterId <= 0 || shopId <= 0) return;
                if (player.HasPlayerHandcuffs() || player.HasPlayerRopeCuffs()) { HUDHandler.SendNotification(player, 3, 5000, "Wie willst du das mit Handschellen/Fesseln machen?"); return; }
                if (!player.Position.IsInRange(ServerShops.GetShopPosition(shopId), 3f)) { HUDHandler.SendNotification(player, 3, 5000, "Du bist zu weit entfernt."); return; }
                if (player.isRobbingAShop)
                {
                    HUDHandler.SendNotification(player, 4, 2500, "Du raubst bereits einen Shop aus.");
                    return;
                }

                if (ServerFactions.GetFactionDutyMemberCount(1) < 6)
                {
                    HUDHandler.SendNotification(player, 3, 2500, "Es sind weniger als 6 Polizisten im Staat.");
                    return;
                }

                if (ServerShops.IsShopRobbedNow(shopId))
                {
                    HUDHandler.SendNotification(player, 3, 2500, "Dieser Shop wird bereits ausgeraubt.");
                    return;
                }

                ServerFactions.AddNewFactionDispatch(0, 2, $"Aktiver Shopraub", player.Position);
                ServerFactions.AddNewFactionDispatch(0, 12, $"Aktiver Shopraub", player.Position);

                foreach (var p in Alt.GetAllPlayers().Where(x => x != null && x.Exists && x.GetCharacterMetaId() > 0).ToList())
                {
                    if (!ServerFactions.IsCharacterInAnyFaction((int)p.GetCharacterMetaId()) || !ServerFactions.IsCharacterInFactionDuty((int)p.GetCharacterMetaId()) || ServerFactions.GetCharacterFactionId((int)p.GetCharacterMetaId()) != 2 && ServerFactions.GetCharacterFactionId((int)p.GetCharacterMetaId()) != 1) continue;
                    HUDHandler.SendNotification(p, 1, 9500, "Ein stiller Alarm wurde ausgelöst.");
                }


                ServerShops.SetShopRobbedNow(shopId, true);
                player.isRobbingAShop = true;
                //HUDHandler.SendNotification(player, 1, 2500, "Du raubst den Laden nun aus - warte 8 Minuten um das Geld zu erhalten.");
                HUDHandler.SendNotification(player, 1, 2500, "Du raubst den Laden nun aus - warte 30 Sekunden um das Geld zu erhalten.");
                //await Task.Delay(480000);
                await Task.Delay(30000);
                ServerShops.SetShopRobbedNow(shopId, false);
                if (player == null || !player.Exists) return;
                player.isRobbingAShop = false;
                if (!player.Position.IsInRange(ServerShops.GetShopPosition(shopId), 12f))
                {
                    HUDHandler.SendNotification(player, 3, 5000, "Du bist zu weit entfernt, der Raub wurde abgebrochen.");
                    return;
                }

                int amount = new Random().Next(6000, 9000);
                HUDHandler.SendNotification(player, 2, 2500, $"Shop ausgeraubt - du erhälst {amount}$.");
                CharactersInventory.AddCharacterItem(player.CharacterId, "Bargeld", amount, "inventory");
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }


        [AsyncClientEvent("Server:Shop:sellItem")]
        public async Task sellShopItem(IPlayer player, int shopId, int amount, string itemname)
        {
            if (player == null || !player.Exists || shopId <= 0 || amount <= 0 || itemname == "") return;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            if (player.HasPlayerHandcuffs() || player.HasPlayerRopeCuffs()) { HUDHandler.SendNotification(player, 3, 5000, "Wie willst du das mit Handschellen/Fesseln machen?"); return; }
            if (!player.Position.IsInRange(ServerShops.GetShopPosition(shopId), 3f)) { HUDHandler.SendNotification(player, 3, 5000, "Du bist zu weit entfernt."); return; }
            int charId = User.GetPlayerOnline(player);
            if (charId == 0) return;
            if (ServerShops.GetShopNeededLicense(shopId) != "None" && !Characters.HasCharacterPermission(charId, ServerShops.GetShopNeededLicense(shopId))) { HUDHandler.SendNotification(player, 3, 5000, "Du hast hier keinen Zugriff drauf."); return; }
            if (!CharactersInventory.ExistCharacterItem(charId, itemname, "inventory") && !CharactersInventory.ExistCharacterItem(charId, itemname, "backpack")) { HUDHandler.SendNotification(player, 3, 5000, "Diesen Gegenstand besitzt du nicht."); return; }
            int itemSellPrice = ServerShopsItems.GetShopItemPrice(shopId, itemname); //Verkaufpreis pro Item
            int invItemAmount = CharactersInventory.GetCharacterItemAmount(charId, itemname, "inventory"); //Anzahl an Items im Inventar
            int backpackItemAmount = CharactersInventory.GetCharacterItemAmount(charId, itemname, "backpack"); //Anzahl an Items im Rucksack
            if (invItemAmount + backpackItemAmount < amount) { HUDHandler.SendNotification(player, 3, 5000, "Soviele Gegenstände hast du nicht zum Verkauf dabei."); return; }


            var removeFromInventory = Math.Min(amount, invItemAmount);
            if (removeFromInventory > 0)
            {
                CharactersInventory.RemoveCharacterItemAmount(charId, itemname, removeFromInventory, "inventory");
            }

            var itemsLeft = amount - removeFromInventory;
            if (itemsLeft > 0)
            {
                CharactersInventory.RemoveCharacterItemAmount(charId, itemname, itemsLeft, "backpack");
            }

            HUDHandler.SendNotification(player, 2, 5000, $"Du hast {amount}x {itemname} für {itemSellPrice * amount}$ verkauft.");
            CharactersInventory.AddCharacterItem(charId, "Bargeld", amount * itemSellPrice, "inventory");
            stopwatch.Stop();
            if (stopwatch.Elapsed.Milliseconds > 30) Alt.Log($"{charId} - sellShopItem benötigte {stopwatch.Elapsed.Milliseconds}ms");
        }
        #endregion

        #region VehicleShop

        internal static void OpenVehicleShop(IPlayer player, string shopname, int shopId)
        {
            if (player == null || !player.Exists || shopId <= 0) return;
            var array = ServerVehicleShops.GetVehicleShopItems(shopId);
            player.EmitLocked("Client:VehicleShop:OpenCEF", shopId, shopname, array);
        }

        [AsyncClientEvent("Server:VehicleShop:BuyVehicle")]
        public async Task BuyVehicle(IPlayer player, int shopid, string hash)
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                if (player == null || !player.Exists || shopid <= 0 || hash == "") return;
                ulong fHash = Convert.ToUInt64(hash);
                int charId = User.GetPlayerOnline(player);
                if (charId == 0 || fHash == 0) return;
                int Price = ServerVehicleShops.GetVehicleShopPrice(shopid, fHash);
                bool PlaceFree = true;
                float itemWeight = ServerItems.GetItemWeight("Kennzeichen") * 1;
                float invWeight = CharactersInventory.GetCharacterItemWeight(charId, "inventory");
                float backpackWeight = CharactersInventory.GetCharacterItemWeight(charId, "backpack");
                Position ParkOut = ServerVehicleShops.GetVehicleShopOutPosition(shopid);
                Rotation RotOut = ServerVehicleShops.GetVehicleShopOutRotation(shopid);
                foreach (IVehicle veh in Alt.GetAllVehicles().ToList()) { if (veh.Position.IsInRange(ParkOut, 2f)) { PlaceFree = false; break; } }
                if (!PlaceFree) { HUDHandler.SendNotification(player, 3, 5000, $"Der Ausladepunkt ist belegt."); return; }
                int rnd = new Random().Next(100000, 999999);
                int rnd2 = new Random().Next(1000, 9999);
                int rnd3 = new Random().Next(100, 999);
                if (ServerVehicles.ExistServerVehiclePlate($"LS{rnd}")) { BuyVehicle(player, shopid, hash); return; }
                if (invWeight + itemWeight > 15f && backpackWeight + itemWeight > Characters.GetCharacterBackpackSize(Characters.GetCharacterBackpack(charId))) { HUDHandler.SendNotification(player, 3, 5000, $"Du hast nicht genug Platz in deinen Taschen."); return; }
                if (!CharactersInventory.ExistCharacterItem(charId, "Bargeld", "inventory") || CharactersInventory.GetCharacterItemAmount(charId, "Bargeld", "inventory") < Price) { HUDHandler.SendNotification(player, 4, 5000, $"Du hast nicht genügend Bargeld dabei ({Price}$)."); return; }
                if (hash == "2046537925" && ServerFactions.GetCharacterFactionRank(charId) == 16 && ServerFactions.GetCharacterFactionId(charId) == 1)
                {
                    HUDHandler.SendNotification(player, 2, 3500, "Dieses Fahrzeug ist kostenlos!");
                }
                else
                {
                    CharactersInventory.RemoveCharacterItemAmount(charId, "Bargeld", Price, "inventory");
                }
                if (shopid == 6)
                {
                    if (hash == "2046537925" && ServerFactions.GetCharacterFactionRank(charId) >= 15)
                    {
                        ServerVehicles.CreateVehicle(fHash, charId, 0, 6, false, 1, ParkOut, RotOut, $"LSPD{rnd3}", 255, 255, 255);
/*                        ServerVehicles.CreateVehicle(fHash, charId, Vtyp, fID, false, gID, ParkOut, RotOut, $"LSPD{rnd3}", 255, 255, 255);
*/                        HUDHandler.SendNotification(player, 2, 5000, $"Fahrzeug erfolgreich gekauft. Kennzeichen: LSPD{rnd3}");
                    }
                    else
                    {
                        ServerVehicles.CreateVehicle(fHash, charId, 0, 0, false, 1, ParkOut, RotOut, $"LSPD{rnd3}", 255, 255, 255);
                        CharactersInventory.AddCharacterItem(charId, $"Fahrzeugschluessel LSPD{rnd3}", 2, "schluessel");
                        HUDHandler.SendNotification(player, 2, 5000, $"Fahrzeug erfolgreich gekauft. Kennzeichen: LSPD{rnd3}");
                    }
                }
                else if (shopid == 7 && ServerFactions.GetCharacterFactionRank(charId) == 16 && ServerFactions.GetCharacterFactionId(charId) == 1)
                {
                    ServerVehicles.CreateVehicle(fHash, charId, 0, 1, false, 2, ParkOut, RotOut, $"LSPD{rnd3}", 255, 255, 255);
                    CharactersInventory.AddCharacterItem(charId, $"Fahrzeugschluessel LSPD{rnd3}", 2, "schluessel");
                    HUDHandler.SendNotification(player, 2, 5000, $"Fahrzeug erfolgreich gekauft. Kennzeichen: LSPD{rnd3}");
                }
                else if (shopid == 7 && ServerFactions.GetCharacterFactionRank(charId) == 15 && ServerFactions.GetCharacterFactionId(charId) == 1)
                {
                    ServerVehicles.CreateVehicle(fHash, charId, 0, 1, false, 2, ParkOut, RotOut, $"LSPD{rnd3}", 255, 255, 255);
                    CharactersInventory.AddCharacterItem(charId, $"Fahrzeugschluessel LSPD{rnd3}", 2, "schluessel");
                    HUDHandler.SendNotification(player, 2, 5000, $"Fahrzeug erfolgreich gekauft. Kennzeichen: LSPD{rnd3}");
                }
                else if (shopid == 8)
                {
                    ServerVehicles.CreateVehicle(fHash, charId, 0, 4, false, 4, ParkOut, RotOut, $"MD{rnd3}", 255, 255, 255);
                    CharactersInventory.AddCharacterItem(charId, $"Fahrzeugschluessel MD{rnd3}", 2, "schluessel");
                    HUDHandler.SendNotification(player, 2, 5000, $"Fahrzeug erfolgreich gekauft. Kennzeichen: MD{rnd3}");
                }
                else if (shopid == 9)
                {
                    ServerVehicles.CreateVehicle(fHash, charId, 0, 4, false, 5, ParkOut, RotOut, $"MD{rnd3}", 255, 255, 255);
                    CharactersInventory.AddCharacterItem(charId, $"Fahrzeugschluessel MD{rnd3}", 2, "schluessel");
                    HUDHandler.SendNotification(player, 2, 5000, $"Fahrzeug erfolgreich gekauft. Kennzeichen: MD{rnd3}");
                }
                else if (shopid == 10)
                {
                    ServerVehicles.CreateVehicle(fHash, charId, 0, 5, false, 8, ParkOut, RotOut, $"LSC{rnd3}", 255, 255, 255);
                    CharactersInventory.AddCharacterItem(charId, $"Fahrzeugschluessel LSC{rnd3}", 2, "schluessel");
                    HUDHandler.SendNotification(player, 2, 5000, $"Fahrzeug erfolgreich gekauft. Kennzeichen: LSC{rnd3}");
                }
                else if (shopid == 23)
                {
                    ServerVehicles.CreateVehicle(fHash, charId, 0, 6, false, 9, ParkOut, RotOut, $"LSF{rnd3}", 255, 255, 255);
                    CharactersInventory.AddCharacterItem(charId, $"Fahrzeugschluessel LSF{rnd3}", 2, "schluessel");
                    HUDHandler.SendNotification(player, 2, 5000, $"Fahrzeug erfolgreich gekauft. Kennzeichen: LSF{rnd3}");
                }
                else
                {
                    ServerVehicles.CreateVehicle(fHash, charId, 0, 0, false, 10, ParkOut, RotOut, $"LS{rnd}", 255, 255, 255);
                    CharactersInventory.AddCharacterItem(charId, $"Fahrzeugschluessel LS{rnd}", 2, "schluessel");
                    HUDHandler.SendNotification(player, 2, 5000, $"Fahrzeug erfolgreich gekauft. Kennzeichen: LS{rnd}");
                    
                }
                if (!CharactersTablet.HasCharacterTutorialEntryFinished(charId, "buyVehicle"))
                {
                    CharactersTablet.SetCharacterTutorialEntryState(charId, "buyVehicle", true);
                    HUDHandler.SendNotification(player, 1, 2500, "Erfolg freigeschaltet: Mobilität");
                }
                stopwatch.Stop();
                if (stopwatch.Elapsed.Milliseconds > 30) Alt.Log($"{charId} - BuyVehicle benötigte {stopwatch.Elapsed.Milliseconds}ms");
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [AsyncClientEvent("Server:VehicleShop:BuyVehicleBank")]
        public async Task BuyVehicleBank(IPlayer player, int shopid, string hash)
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                if (player == null || !player.Exists || shopid <= 0 || hash == "") return;
                ulong fHash = Convert.ToUInt64(hash);
                int charId = User.GetPlayerOnline(player);
                if (charId == 0 || fHash == 0) return;
                int Price = ServerVehicleShops.GetVehicleShopPrice(shopid, fHash);
                bool PlaceFree = true;
                float itemWeight = ServerItems.GetItemWeight("Kennzeichen") * 1;
                float invWeight = CharactersInventory.GetCharacterItemWeight(charId, "inventory");
                float backpackWeight = CharactersInventory.GetCharacterItemWeight(charId, "backpack");
                Position ParkOut = ServerVehicleShops.GetVehicleShopOutPosition(shopid);
                Rotation RotOut = ServerVehicleShops.GetVehicleShopOutRotation(shopid);
                foreach (IVehicle veh in Alt.GetAllVehicles().ToList()) { if (veh.Position.IsInRange(ParkOut, 2f)) { PlaceFree = false; break; } }
                if (!PlaceFree) { HUDHandler.SendNotification(player, 3, 5000, $"Der Ausladepunkt ist belegt."); return; }
                int rnd = new Random().Next(100000, 999999);
                int rnd2 = new Random().Next(1000, 9999);
                int rnd3 = new Random().Next(100, 999);
                var accNUM = CharactersBank.GetCharacterBankMainKonto(charId);
                if (ServerVehicles.ExistServerVehiclePlate($"LS{rnd}")) { BuyVehicleBank(player, shopid, hash); return; }
                if (CharactersBank.GetBankAccountMoney(accNUM) != Price) { HUDHandler.SendNotification(player, 4, 3500, "Du hast nicht genug Geld auf dem Konto"); return; }
                if (hash == "2046537925" && ServerFactions.GetCharacterFactionRank(charId) == 16 && ServerFactions.GetCharacterFactionId(charId) == 1)
                {
                    HUDHandler.SendNotification(player, 2, 3500, "Dieses Fahrzeug ist kostenlos!");
                }
                else
                {
                    CharactersBank.SetBankAccountMoney(accNUM, CharactersBank.GetBankAccountMoney(accNUM) - Price);
                }
                if (shopid == 1)
                {
                    if (hash == "2046537925" && ServerFactions.GetCharacterFactionRank(charId) >= 15)
                    {
                        ServerVehicles.CreateVehicle(fHash, charId, 0, 1, false, 1, ParkOut, RotOut, $"LSPD{rnd3}", 255, 255, 255);
                        HUDHandler.SendNotification(player, 2, 5000, $"Fahrzeug erfolgreich gekauft. Kennzeichen: LSPD{rnd3}");
                    }
                    else
                    {
                        ServerVehicles.CreateVehicle(fHash, charId, 0, 0, false, 1, ParkOut, RotOut, $"LSPD{rnd3}", 255, 255, 255);
                        CharactersInventory.AddCharacterItem(charId, $"Fahrzeugschluessel LSPD{rnd3}", 2, "schluessel");
                        HUDHandler.SendNotification(player, 2, 5000, $"Fahrzeug erfolgreich gekauft. Kennzeichen: LSPD{rnd3}");
                    }
                }
                else if (shopid == 2 && ServerFactions.GetCharacterFactionRank(charId) == 16 && ServerFactions.GetCharacterFactionId(charId) == 1)
                {
                    ServerVehicles.CreateVehicle(fHash, charId, 0, 1, false, 2, ParkOut, RotOut, $"LSPD{rnd3}", 255, 255, 255);
                    CharactersInventory.AddCharacterItem(charId, $"Fahrzeugschluessel LSPD{rnd3}", 2, "schluessel");
                    HUDHandler.SendNotification(player, 2, 5000, $"Fahrzeug erfolgreich gekauft. Kennzeichen: LSPD{rnd3}");
                }
                else if (shopid == 2 && ServerFactions.GetCharacterFactionRank(charId) == 15 && ServerFactions.GetCharacterFactionId(charId) == 1)
                {
                    ServerVehicles.CreateVehicle(fHash, charId, 0, 1, false, 2, ParkOut, RotOut, $"LSPD{rnd3}", 255, 255, 255);
                    CharactersInventory.AddCharacterItem(charId, $"Fahrzeugschluessel LSPD{rnd3}", 2, "schluessel");
                    HUDHandler.SendNotification(player, 2, 5000, $"Fahrzeug erfolgreich gekauft. Kennzeichen: LSPD{rnd3}");
                }
                else if (shopid == 3)
                {
                    ServerVehicles.CreateVehicle(fHash, charId, 0, 4, false, 4, ParkOut, RotOut, $"MD{rnd3}", 255, 255, 255);
                    CharactersInventory.AddCharacterItem(charId, $"Fahrzeugschluessel MD{rnd3}", 2, "schluessel");
                    HUDHandler.SendNotification(player, 2, 5000, $"Fahrzeug erfolgreich gekauft. Kennzeichen: MD{rnd3}");
                }
                else if (shopid == 4)
                {
                    ServerVehicles.CreateVehicle(fHash, charId, 0, 4, false, 5, ParkOut, RotOut, $"MD{rnd3}", 255, 255, 255);
                    CharactersInventory.AddCharacterItem(charId, $"Fahrzeugschluessel MD{rnd3}", 2, "schluessel");
                    HUDHandler.SendNotification(player, 2, 5000, $"Fahrzeug erfolgreich gekauft. Kennzeichen: MD{rnd3}");
                }
                else if (shopid == 5)
                {
                    ServerVehicles.CreateVehicle(fHash, charId, 0, 5, false, 8, ParkOut, RotOut, $"LSC{rnd3}", 255, 255, 255);
                    CharactersInventory.AddCharacterItem(charId, $"Fahrzeugschluessel LSC{rnd3}", 2, "schluessel");
                    HUDHandler.SendNotification(player, 2, 5000, $"Fahrzeug erfolgreich gekauft. Kennzeichen: LSC{rnd3}");
                }
                else if (shopid == 6)
                {
                    ServerVehicles.CreateVehicle(fHash, charId, 0, 6, false, 9, ParkOut, RotOut, $"LSF{rnd3}", 255, 255, 255);
                    CharactersInventory.AddCharacterItem(charId, $"Fahrzeugschluessel LSF{rnd3}", 2, "schluessel");
                    HUDHandler.SendNotification(player, 2, 5000, $"Fahrzeug erfolgreich gekauft. Kennzeichen: LSF{rnd3}");
                }
                else if (shopid == 7)
                {
                    ServerVehicles.CreateVehicle(fHash, charId, 0, 9, false, 7, ParkOut, RotOut, $"VUC{rnd3}", 255, 255, 255);
                    CharactersInventory.AddCharacterItem(charId, $"Fahrzeugschluessel VUC{rnd3}", 2, "schluessel");
                    HUDHandler.SendNotification(player, 2, 5000, $"Fahrzeug erfolgreich gekauft. Kennzeichen: VUC{rnd3}");
                }
                else
                {
                    ServerVehicles.CreateVehicle(fHash, charId, 0, 0, false, 10, ParkOut, RotOut, $"LS-{rnd2}", 255, 255, 255);
                    CharactersInventory.AddCharacterItem(charId, $"Fahrzeugschluessel LS-{rnd2}", 2, "schluessel");
                    HUDHandler.SendNotification(player, 2, 5000, $"Fahrzeug erfolgreich gekauft. Kennzeichen: LS-{rnd2}");
                    if (invWeight + itemWeight < 15f)
                    {
                        CharactersInventory.AddCharacterItem(charId, $"Kennzeichen LS-{rnd2}", 1, "inventory");
                    }
                    if (backpackWeight + itemWeight < Characters.GetCharacterBackpackSize(Characters.GetCharacterBackpack(charId)))
                    {
                        CharactersInventory.AddCharacterItem(charId, $"Kennzeichen LS-{rnd2}", 1, "backpack");
                    }
                    if (invWeight + itemWeight < 15f)
                    {
                        CharactersInventory.AddCharacterItem(charId, $"Kaufvertrag LS-{rnd2}", 1, "inventory");
                    }
                    if (backpackWeight + itemWeight < Characters.GetCharacterBackpackSize(Characters.GetCharacterBackpack(charId)))
                    {
                        CharactersInventory.AddCharacterItem(charId, $"Kaufvertrag LS-{rnd2}", 1, "backpack");
                    }
                }
                if (!CharactersTablet.HasCharacterTutorialEntryFinished(charId, "buyVehicle"))
                {
                    CharactersTablet.SetCharacterTutorialEntryState(charId, "buyVehicle", true);
                    HUDHandler.SendNotification(player, 1, 2500, "Erfolg freigeschaltet: Mobilität");
                }
                stopwatch.Stop();
                if (stopwatch.Elapsed.Milliseconds > 30) Alt.Log($"{charId} - BuyVehicle benötigte {stopwatch.Elapsed.Milliseconds}ms");
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        internal static void SellVehicle(IPlayer player, string shopname, int shopId)
        {
            IVehicle veh = player.Vehicle;
            if (player == null && !player.Exists && player.GetCharacterMetaId() == 0 && Characters.GetCharacterAccountId((int)player.GetCharacterMetaId()) == 0 && player.IsInVehicle == false) return;
            if (veh == null && !veh.Exists) return;
            var hash = ServerVehicles.GetVehicleHashById((int)veh.GetVehicleId()); ////////////////////
            ulong fHash = Convert.ToUInt64(hash);
            int price = ServerVehicleShops.GetVehicleShopPrice2((long)fHash)/100*80; //FIXEN AMK!! --> D O N E
            if (ServerVehicleShops.GetVehicleShopId(shopId) != 1000)
            {
                
                if (ServerVehicles.GetVehicleKM(veh) >= 151) { HUDHandler.SendNotification(player, 4, 7500, $"Der Kilometerstand des Fahrzeugs ist höher als 150Km <br>Kilometerstand: {ServerVehicles.GetVehicleKM(veh)}"); return; }
                CharactersInventory.AddCharacterItem((int)player.GetCharacterMetaId(), "Bargeld", price, "inventory");
                HUDHandler.SendNotification(player, 2, 7500, $"Du hast das Fahrzeug {ServerVehicles.GetVehicleNameOnHash(hash)} verkauft für: <br> Summe: {price}$ <br> Kilometerstand: {ServerVehicles.GetVehicleKM(veh)}Km <br><br>Verkaufsort: {shopname}");
                CharactersInventory.RemoveCharacterItem2((int)player.GetCharacterMetaId(), $"Fahrzeugschluessel {veh.NumberplateText}");
                ServerVehicles.RemoveVehiclePermanently(veh); 
                Alt.RemoveVehicle(veh);
            } else
            {
                if (player.AdminLevel() <= 7) { HUDHandler.SendNotification(player, 4, 4000, "Das Fahrzeug kannst du nicht verkaufen!"); return; }
                if (ServerVehicles.GetVehicleKM(veh) >= 151) { HUDHandler.SendNotification(player, 4, 7500, $"Der Kilometerstand des Fahrzeugs ist höher als 150Km <br>Kilometerstand: {ServerVehicles.GetVehicleKM(veh)}"); return; }
                
                HUDHandler.SendNotification(player, 2, 7500, $"Du hast das Fahrzeug {ServerVehicles.GetVehicleNameOnHash(hash)} verkauft für: <br> Preis: 0$ <br> Kilometerstand: {ServerVehicles.GetVehicleKM(veh)}Km <br><br>Verkaufsort: {shopname}");
                CharactersInventory.RemoveCharacterItem2((int)player.GetCharacterMetaId(), $"Fahrzeugschluessel {veh.NumberplateText}");
                ServerVehicles.RemoveVehiclePermanently(veh);
                Alt.RemoveVehicle(veh);
            }

            Alt.Log($"Fahrzeugname: {ServerAllVehicles.GetVehicleNameOnHash(hash)} | Fahrzeughash:{fHash} | Preis: {price} | Spieler: {Characters.GetCharacterName((int)player.GetCharacterMetaId())}");
        }
        #endregion

        #region Clothes Shop
        public static void openClothesShop(ClassicPlayer player, int id)
        {
            try
            {
                if (player == null || !player.Exists || player.CharacterId <= 0 || !ServerClothesShops.ExistClothesShop(id)) return;

                if (!player.HasData("clothesMenuOpen")) player.SetData("clothesMenuOpen", true);
                else
                {
                    Characters.SetCharacterCorrectClothes(player);
                    player.DeleteData("clothesMenuOpen");
                }

                player.EmitLocked("Client:Clothesstore:OpenMenu", Convert.ToInt32(Characters.GetCharacterGender(player.CharacterId)) + 1);
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [AsyncClientEvent("Server:Clothesstore:BuyCloth")]
        public async Task buyClothesShopItem(ClassicPlayer player, int clothId, bool isProp)
        {
            try
            {
                if (player == null || !player.Exists || player.CharacterId <= 0 || !ServerClothes.ExistClothes(clothId, Convert.ToInt32(Characters.GetCharacterGender(player.CharacterId)))) return;

                await Characters.SwitchCharacterClothes(player, clothId, isProp);

                if (CharactersClothes.ExistCharacterClothes(player.CharacterId, clothId)) HUDHandler.SendNotification(player, 2, 1500, $"Du hast das Kleidungsstück angezogen.");
                else
                {
                    int price = ServerClothesShops.GetClothesPrice(player, clothId, isProp);
                    if (!CharactersInventory.ExistCharacterItem(player.CharacterId, "Bargeld", "inventory") || CharactersInventory.GetCharacterItemAmount(player.CharacterId, "Bargeld", "inventory") < price) HUDHandler.SendNotification(player, 2, 1500, $"Du hast nicht genug Geld, um dieses Kleidungsstück zu kaufen. (${price})"); ;
                    CharactersInventory.RemoveCharacterItemAmount(player.CharacterId, "Bargeld", price, "inventory");
                    HUDHandler.SendNotification(player, 2, 1500, $"Du hast dir das Kleidungsstück für ${price} gekauft.");
                    CharactersClothes.CreateCharacterOwnedClothes(player.CharacterId, clothId);
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [AsyncClientEvent("Server:Clothesstore:SetPerfectTorso")]
        public async Task ClothesshopSetPerfectTorso(ClassicPlayer player, int BestTorsoDrawable, int BestTorsoTexture)
        {
            try
            {
                int clothId = ServerClothes.GetClothesId(3, BestTorsoDrawable, BestTorsoTexture, Convert.ToInt32(Characters.GetCharacterGender(player.CharacterId)));
                if (player == null || !player.Exists || player.CharacterId <= 0 || !ServerClothes.ExistClothes(clothId, Convert.ToInt32(Characters.GetCharacterGender(player.CharacterId)))) return;

                await Characters.SwitchCharacterClothes(player, clothId, false);
                CharactersClothes.CreateCharacterOwnedClothes(player.CharacterId, clothId);
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [AsyncClientEvent("Server:Clothesstore:CloseClothesshop")]
        public void CloseClothesshop(ClassicPlayer player)
        {
            try
            {
                if (!player.Exists || player == null || (int)player.GetCharacterMetaId() == 0) return;
                int charid = (int)player.GetCharacterMetaId();

                //WAFFEN CHANGE - AMMO
                //player.EmitLocked("Client:WeaponAmmoChange:ComingRespond", (ulong)wHash);
                string secondaryWeapon2REMOVE = (string)Characters.GetCharacterWeapon(player, "SecondaryWeapon2");
                string secondaryWeaponREMOVE = (string)Characters.GetCharacterWeapon(player, "SecondaryWeapon");
                string primaryWeaponREMOVE = (string)Characters.GetCharacterWeapon(player, "PrimaryWeapon");
                if (primaryWeaponREMOVE != null)
                {
                    var hash = WeaponHandler.GetWeaponModelByName(primaryWeaponREMOVE);
                    player.EmitLocked("Client:WeaponAmmoChange:ComingTimer", (ulong)hash);
                }
                else if (secondaryWeaponREMOVE != null)
                {
                    var hash = WeaponHandler.GetWeaponModelByName(secondaryWeaponREMOVE);
                    player.EmitLocked("Client:WeaponAmmoChange:ComingTimer", (ulong)hash);
                }
                else if (secondaryWeapon2REMOVE != null)
                {
                    var hash = WeaponHandler.GetWeaponModelByName(secondaryWeapon2REMOVE);
                    player.EmitLocked("Client:WeaponAmmoChange:ComingTimer", (ulong)hash);
                }

                player.RemoveAllWeapons();
                string FistWeaponRemove = (string)Characters.GetCharacterWeapon(player, "FistWeapon");

                if (secondaryWeaponREMOVE != "None") //Wenn nur Sekündär1
                {
                    int ammodiS1 = (int)Characters.GetCharacterWeapon(player, "SecondaryAmmo");
                    player.GiveWeapon(WeaponHandler.GetWeaponModelByName(secondaryWeaponREMOVE), ammodiS1, false);
                }

                if (secondaryWeapon2REMOVE != "None") //Wenn nur Sekündär2
                {
                    int ammodiS1 = (int)Characters.GetCharacterWeapon(player, "SecondaryAmmo2");
                    player.GiveWeapon(WeaponHandler.GetWeaponModelByName(secondaryWeapon2REMOVE), ammodiS1, false);
                }

                if (primaryWeaponREMOVE != "None") //Wenn nur Primär1
                {
                    int ammodiS1 = (int)Characters.GetCharacterWeapon(player, "PrimaryAmmo");
                    player.GiveWeapon(WeaponHandler.GetWeaponModelByName(primaryWeaponREMOVE), ammodiS1, false);
                }

                if (FistWeaponRemove != "None") //Wenn nur FAUST
                {
                    player.GiveWeapon(WeaponHandler.GetWeaponModelByName(FistWeaponRemove), 0, false);
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        #endregion

        #region Tattoo Shop
        internal static async void openTattooShop(ClassicPlayer player, Server_Tattoo_Shops tattooShop)
        {
            if (player == null || !player.Exists || player.CharacterId <= 0 || player.IsCefOpen() || tattooShop == null) return;
            await LoginHandler.setCefStatus(player, true);
            int gender = Convert.ToInt32(Characters.GetCharacterGender(player.CharacterId));
            player.EmitAsync("Client:TattooShop:openShop", gender, tattooShop.id, CharactersTattoos.GetAccountOwnTattoos(player.CharacterId)); //fix TODO
        }

        [AsyncClientEvent("Server:TattooShop:buyTattoo")]
        public async Task ClientEvent_buyTattoo(ClassicPlayer player, int shopId, int tattooId)
        {
            try
            {
                if (player == null || !player.Exists || player.CharacterId <= 0 || shopId <= 0 || tattooId <= 0 || !ServerTattoos.ExistTattoo(tattooId) || CharactersTattoos.ExistAccountTattoo(player.CharacterId, tattooId) || !ServerTattooShops.ExistTattooShop(shopId)) return;
                int price = ServerTattoos.GetTattooPrice(tattooId);
                if (!CharactersInventory.ExistCharacterItem(player.CharacterId, "Bargeld", "inventory") || CharactersInventory.GetCharacterItemAmount(player.CharacterId, "Bargeld", "inventory") < price)
                {
                    HUDHandler.SendNotification(player, 4, 5000, $"Fehler: Du hast nicht genügend Geld dabei ({price}$).");
                    return;
                }
                CharactersInventory.RemoveCharacterItemAmount(player.CharacterId, "Bargeld", price, "inventory");
                ServerTattooShops.SetTattooShopBankMoney(shopId, ServerTattooShops.GetTattooShopBank(shopId) + price);
                CharactersTattoos.CreateNewEntry(player.CharacterId, tattooId);
                HUDHandler.SendNotification(player, 2, 1500, $"Du hast das Tattoo '{ServerTattoos.GetTattooName(tattooId)}' für {price}$ gekauft.");
                player.updateTattoos();
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e}");
            }
        }

        [AsyncClientEvent("Server:TattooShop:deleteTattoo")]
        public async Task ClientEvent_deleteTattoo(ClassicPlayer player, int tattooId)
        {
            try
            {
                if (player == null || !player.Exists || player.CharacterId <= 0 || tattooId <= 0 || !CharactersTattoos.ExistAccountTattoo(player.CharacterId, tattooId)) return;
                CharactersTattoos.RemoveAccountTattoo(player.CharacterId, tattooId);
                HUDHandler.SendNotification(player, 2, 1500, $"Du hast das Tattoo '{ServerTattoos.GetTattooName(tattooId)}' erfolgreich entfernen lassen.");
                player.updateTattoos();
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e}");
            }
        }
        #endregion

        #region Schluessedienst
        public static void openschluesselShop(ClassicPlayer player)
        {
            try
            {
                if (player == null || !player.Exists || player.CharacterId <= 0) return;

                if (!player.HasData("schluesselMenuOpen")) player.SetData("schluesselMenuOpen", true);
                else
                {
                    player.DeleteData("schluesselMenuOpen");
                }

                player.EmitLocked("Client:schluesseldienst:OpenMenu");
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [AsyncClientEvent("Server:schluesseldienst:ReCreateSchluessel")]
        public async Task ReCreateSchluessel(ClassicPlayer player, string plate)
        {
            try
            {

                //HUDHandler.SendNotification(player, 2, 4000, "SERVERSIDE1");
                if (player == null || !player.Exists || player.CharacterId <= 0 || plate == null)
                {
                    HUDHandler.SendNotification(player, 2, 4000, "Irgendwas == 0");
                    return;
                }
                //HUDHandler.SendNotification(player, 2, 4000, "SERVERSIDE2");


                int price = 750; //Preis zum nachmachen
                int vehid = ServerVehicles.GetVehicleIdByPlate(plate);
                //HUDHandler.SendNotification(player, 2, 4000, "SERVERSIDE3");

                if (CharactersInventory.GetCharacterItemAmount(player.CharacterId, "Bargeld", "inventory") <= price)
                {
                    HUDHandler.SendNotification(player, 4, 4000, "Du hast nicht gegnug Geld dabei, um einen Schlüssel nachzumachen");
                    return;
                }

                if (CharactersInventory.ExistCharacterItem(player.CharacterId, "Schluesselrohling", "inventory") == false && CharactersInventory.ExistCharacterItem(player.CharacterId, "Schluesselrohling", "backpack") == false)
                {
                    HUDHandler.SendNotification(player, 4, 4000, "Du benötigst Schlüsselrohlinge um einen Schlüssel nachzumachen");
                    return;
                }

                await Task.Delay(120000);

                if (ServerVehicles.GetVehicleOwnerById(vehid) == User.GetPlayerOnline(player))
                {
                    CharactersInventory.AddCharacterItem(User.GetPlayerOnline(player), $"Fahrzeugschluessel {plate}", 1, "schluessel");
                    CharactersInventory.RemoveCharacterItemAmount(player.CharacterId, "Bargeld", price, "inventory");
                    CharactersInventory.RemoveCharacterItemAmount2(player.CharacterId, "Schluesselrohling", 1);
                    HUDHandler.SendNotification(player, 2, 4000, "Fahrzeugschluessel erfolgreich nachgemacht");
                }
                else if(ServerVehicles.GetVehicleOwnerById(vehid) != User.GetPlayerOnline(player))
                {
                    HUDHandler.SendNotification(player, 4, 4000, "Du bist nicht der Besitzer des Fahrzeuges");
                    return;
                }
                //HUDHandler.SendNotification(player, 2, 4000, "SERVERSIDE4");

            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }


        [AsyncClientEvent("Server:schluesseldienst:DeleteSchluesselCEF")]
        public async Task DeleteSchluesselCEF(ClassicPlayer player, string plate)
        {
            try
            {
                //HUDHandler.SendNotification(player, 2, 4000, "SERVERSIDE1");
                if (player == null || !player.Exists || player.CharacterId <= 0 || plate == null)
                {
                    HUDHandler.SendNotification(player, 2, 4000, "Irgendwas == 0");
                    return;
                }
                //HUDHandler.SendNotification(player, 2, 4000, "SERVERSIDE2");

                int price = 750; //Preis zum nachmachen
                int owner = ServerVehicles.GetVehicleIdByPlate(plate);

                if (CharactersInventory.GetCharacterItemAmount(player.CharacterId, "Bargeld", "inventory") <= price)
                {
                    HUDHandler.SendNotification(player, 4, 4000, "Du hast nicht gegnug Geld dabei, um das Schloss zu tauschen");
                    return;
                }

                if (ServerVehicles.GetVehicleOwnerById(owner) == player.CharacterId)
                {
                    var itemname = $"Fahrzeugschluessel {plate}";
                    var items = CharactersInventory.CharactersInventory_.ToList().FirstOrDefault(i => i.itemName == itemname);
                    /*  var AllKeys = CharactersInventory.CharactersInventory_.ToList().Where(x => x.itemName == );*/
                    await Task.Delay(240000);
                    CharactersInventory.RemoveItem(itemname, "schluessel");
                    CharactersInventory.RemoveCharacterItemAmount(player.CharacterId, "Bargeld", price, "inventory");
                    CharactersInventory.AddCharacterItem(player.CharacterId, $"Fahrzeugschluessel {plate}", 2, "schluessel");
                    HUDHandler.SendNotification(player, 2, 4000, "Fahrzeugschluessel erfolgreich ersetzt");
                }
                else
                {
                    HUDHandler.SendNotification(player, 4, 4000, "Du bist nicht der Besitzer des Fahrzeuges");
                    return;
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }
        #endregion

        #region waschstrasse
        internal static async Task usewaschstrasse(IPlayer player)
        {
            try
            {

                IVehicle veh = player.Vehicle;
                if (player == null && !player.Exists && player.GetCharacterMetaId() == 0 && Characters.GetCharacterAccountId((int)player.GetCharacterMetaId()) == 0 && player.IsInVehicle == false) return;
                if (veh == null && !veh.Exists) return;

                var charId = (int)player.GetCharacterMetaId();
                var WashPrice = 750; //HIER ÄNDERN
                var zeit = 25000;    //HIER ÄNDERN
                bool engineState = ServerVehicles.GetVehicleEngineState(veh);
                int rnd1 = new Random().Next(50, 100);

                if (CharactersInventory.GetCharacterItemAmount2(charId, "Bargeld") <= WashPrice) { HUDHandler.SendNotification(player, 4, 5000, $"Du hast nicht genug Bargeld! {WashPrice}"); return; }
                if (veh.EngineOn == true) { HUDHandler.SendNotification(player, 4, 5000, $"Der Motor muss dafür aus sein! <br>Motor = {engineState}"); return; }
                HUDHandler.SendNotification(player, 4, zeit, $"Das Fahrzeug wird gereinigt (Zeit: {zeit / 1000}s)");
                await Task.Delay(zeit);
                if (rnd1 == 1) { HUDHandler.SendNotification(player, 4, 5000, $"Während des Waschens ist Wasser in den Motor gekommen! <br>Damage = {veh.DamageData}"); return; }
                if (rnd1 == 1) { HUDHandler.SendNotification(player, 4, 5000, $"Bitte Rufe einen Abschlepper, damit dein Fahrzeug repariert werden kann! <br>Damage = {veh.DamageData}"); return; }
                if (rnd1 == 1) { ServerVehicles.SetVehicleEngineHealthy(veh, false); return; }
                else
                {
                    HUDHandler.SendNotification(player, 4, 5000, $"Das Waschen ist abgeschlossen und das Fahrzeug ist wieder sauber!");
                    Alt.EmitAllClients("Client:Utilities:repairVehicle", veh);
                    CharactersInventory.RemoveCharacterItemAmount2(charId, "Bargeld", WashPrice);
                }

            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }
        #endregion
    }
}
