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
using AltV.Net.Enums;
using Altv_Roleplay.Factories;
using Altv_Roleplay.Model;
using Altv_Roleplay.Utils;

namespace Altv_Roleplay.Handler
{
    class InventoryHandler : IScript
    {
        [AsyncClientEvent("Server:Inventory:RequestInventoryItems")]
        public static async Task RequestInventoryItems(IPlayer player)
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                if (player == null || !player.Exists) return;
                int charId = User.GetPlayerOnline(player);
                if (charId <= 0) return;
                if (!CharactersTablet.HasCharacterTutorialEntryFinished(charId, "openInventory"))
                {
                    CharactersTablet.SetCharacterTutorialEntryState(charId, "openInventory", true);
                    HUDHandler.SendNotification(player, 1, 5000, "Erfolg freigeschaltet: Inventar öffnen.");
                }
                string invArray = CharactersInventory.GetCharacterInventory(User.GetPlayerOnline(player));
                player.EmitLocked("Client:Inventory:AddInventoryItems", invArray, Characters.GetCharacterBackpackSize(Characters.GetCharacterBackpack(User.GetPlayerOnline(player))), 0);
                if (stopwatch.Elapsed.Milliseconds > 30) Alt.Log($"{charId} - Inventaraufruf benötigte {stopwatch.Elapsed.Milliseconds}ms");
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [AsyncClientEvent("Server:Inventory:closeCEF")]
        public async Task CloseInventoryCEF(IPlayer player)
        {
            if (player == null || !player.Exists) return;
            player.EmitLocked("Client:Inventory:closeCEF");
        }

        #region switch
        [AsyncClientEvent("Server:Inventory:switchItemToDifferentInv")]
        public async Task switchItemToDifferentInv(ClassicPlayer player, string itemname, int itemAmount, string fromContainer, string toContainer)
        {
            try
            {
                if (player == null || !player.Exists || itemname == "" || itemAmount <= 0 || fromContainer == "" || toContainer == "" || User.GetPlayerOnline(player) == 0) return;
                if (player.HasPlayerHandcuffs() || player.HasPlayerRopeCuffs()) { HUDHandler.SendNotification(player, 3, 5000, "Wie willst du das mit Handschellen/Fesseln machen?"); return; }
                int charId = player.CharacterId;
                string normalName = ServerItems.ReturnNormalItemName(itemname);
                float itemWeight = ServerItems.GetItemWeight(itemname) * itemAmount;
                float invWeight = CharactersInventory.GetCharacterItemWeight(charId, "inventory");
                float backpackWeight = CharactersInventory.GetCharacterItemWeight(charId, "backpack");
                if (!CharactersInventory.ExistCharacterItem(charId, itemname, fromContainer)) return;

                if (toContainer == "inventory") { if (invWeight + itemWeight > 15f) { HUDHandler.SendNotification(player, 3, 5000, $"Soviel Platz hast du im Inventar nicht."); return; } }
                else if (toContainer == "backpack") { if (backpackWeight + itemWeight > Characters.GetCharacterBackpackSize(Characters.GetCharacterBackpack(charId))) { HUDHandler.SendNotification(player, 3, 5000, $"Soviel Platz hast du in deinen Taschen / deinem Rucksack nicht."); return; } }

                if (CharactersInventory.GetCharacterItemAmount(charId, itemname, fromContainer) < itemAmount) { HUDHandler.SendNotification(player, 4, 5000, "Die angegebene Item-Anzahl ist größer als die Anzahl der Items die du mit dir trägst."); return; }
                if (itemname == "Rucksack" || itemname == "Tasche" || normalName == "Ausweis" || normalName == "Bargeld" || normalName == "Smartphone" || normalName == "EC Karte" || normalName == "Fahrzeugschluessel") { HUDHandler.SendNotification(player, 3, 5000, "Diesen Gegenstand kannst du nicht in deinen Rucksack / deine Tache legen."); return; }
                CharactersInventory.RemoveCharacterItemAmount(charId, itemname, itemAmount, fromContainer);
                CharactersInventory.AddCharacterItem(charId, itemname, itemAmount, toContainer);
                HUDHandler.SendNotification(player, 2, 5000, $"Der Gegenstand {itemname} ({itemAmount}x) wurde erfolgreich vom ({fromContainer}) in ({toContainer}) verschoben.");
                RequestInventoryItems(player);
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }
        #endregion switch

        #region use
        [AsyncClientEvent("Server:Inventory:UseItem")]
        public async Task UseItem(ClassicPlayer player, string itemname, int itemAmount, string fromContainer)
        {
            try
            {
                string ECData = null,
                    CarKeyData = null;

                if (itemname == "Schutzweste" && itemAmount > 1) { HUDHandler.SendNotification(player, 4, 3500, "Du kannst nur 1 Schutzweste nutzen!"); return; }
                if (player == null || !player.Exists || itemname == "" || itemAmount <= 0 || fromContainer == "" || User.GetPlayerOnline(player) == 0) return;
                if (player.HasPlayerHandcuffs() || player.HasPlayerRopeCuffs()) { HUDHandler.SendNotification(player, 3, 5000, "Wie willst du das mit Handschellen/Fesseln machen?"); return; }
                if (ServerItems.IsItemUseable(ServerItems.ReturnNormalItemName(itemname)) == false) { HUDHandler.SendNotification(player, 4, 5000, $"Dieser Gegenstand ist nicht benutzbar ({itemname})!"); return; }
                int charId = player.CharacterId;

                float itemWeight = ServerItems.GetItemWeight(itemname) * itemAmount;
                float invWeight = CharactersInventory.GetCharacterItemWeight(charId, "inventory");
                float backpackWeight = CharactersInventory.GetCharacterItemWeight(charId, "backpack");
                if (charId <= 0 || CharactersInventory.ExistCharacterItem(charId, itemname, fromContainer) == false) return;
                if (CharactersInventory.GetCharacterItemAmount(charId, itemname, fromContainer) < itemAmount) { HUDHandler.SendNotification(player, 4, 5000, $"Die angegeben zu nutzende Anzahl ist nicht vorhanden ({itemname})!"); return; }
                if (itemname.Contains("EC Karte")) { string[] SplittedItemName = itemname.Split(' '); ECData = itemname.Replace("EC Karte ", ""); itemname = "EC Karte"; }
                else if (itemname.Contains("Fahrzeugschluessel")) { string[] SplittedItemName = itemname.Split(' '); CarKeyData = itemname.Replace("Fahrzeugschluessel ", ""); itemname = "Autoschluessel"; }

                if (ServerItems.IsItemDesire(itemname))
                {
                    CharactersInventory.RemoveCharacterItemAmount(charId, itemname, itemAmount, fromContainer);
                    Characters.SetCharacterHunger(charId, Characters.GetCharacterHunger(charId) + ServerItems.GetItemDesireFood(itemname) * itemAmount);
                    Characters.SetCharacterThirst(charId, Characters.GetCharacterThirst(charId) + ServerItems.GetItemDesireDrink(itemname) * itemAmount);
                    player.EmitLocked("Client:HUD:UpdateDesire", Characters.GetCharacterArmor(charId), Characters.GetCharacterHealth(charId), Characters.GetCharacterHunger(charId), Characters.GetCharacterThirst(charId)); //HUD updaten
                }
                else if (itemname == "Beamtenschutzweste")
                {
                    CharactersInventory.RemoveCharacterItemAmount(charId, "Beamtenschutzweste", 1, fromContainer);
                    Characters.SetCharacterArmor(charId, 100);
                    player.Armor = 100;
                    if (Characters.GetCharacterGender(charId)) player.EmitLocked("Client:SpawnArea:setCharClothes", 9, 17, 2);
                    else player.EmitLocked("Client:SpawnArea:setCharClothes", 9, 57, 0); // Schutzweste
                }
                else if (itemname == "Verband")
                {
                    CharactersInventory.RemoveCharacterItemAmount(charId, "Verband", itemAmount, fromContainer);
                    Characters.SetCharacterHealth(charId, +10);
                    player.Health = +10;
                    player.EmitLocked("Client:HUD:UpdateDesire", Characters.GetCharacterArmor(charId), Characters.GetCharacterHealth(charId), Characters.GetCharacterHunger(charId), Characters.GetCharacterThirst(charId)); //HUD updaten
                }
                else if (itemname == "Reparaturkit")
                {
                    CharactersInventory.RemoveCharacterItemAmount(charId, "Verband", itemAmount, fromContainer);
                }
                else if (itemname == "Schweissgerät")
                {
                    var atmPos = ServerATM.ServerATM_.FirstOrDefault(x => player.Position.IsInRange(new Position(x.posX, x.posY, x.posZ), 1f));
                    int usingAccountNumber = Convert.ToInt32(ECData);
                    if (atmPos != null)
                    {
                        _ = RobberyHandler.breakUpATM(player, new Position(atmPos.posX, atmPos.posY, atmPos.posZ));
                        return;
                    }

                    if (player.Position.IsInRange(Handler.RobberyHandler.bankRobPosition, 2f))
                    {
                        RobberyHandler.breakUpBank(player);
                        return;
                    }
                    else { HUDHandler.SendNotification(player, 3, 2000, "Das kannst du hier nicht benutzen."); return; }
                }
                else if (itemname == "Einreisegeschenk")
                {
                    CharactersInventory.RemoveCharacterItem(charId, "Einreisegeschenk", fromContainer);
                    int rnd = new Random().Next(7500, 12500);
                    CharactersInventory.AddCharacterItem(charId, "Bargeld", rnd, "inventory");
                    HUDHandler.SendNotification(player, 2, 4000, $"Glückwunsch, du hast hast aus dem Geschenk {rnd}$ bekommen!");
                }

                else if (itemname == "Laptop")
                {
                    if (Characters.IsCharacterLaptopEquipped(charId))
                    {
                        player.EmitLocked("Client:Laptop:activateLaptop", false);
                        HUDHandler.SendNotification(player, 4, 2500, "Laptop ausgeschaltet");
                        Characters.SetCharacterLaptopEquipped(charId, false);
                    }
                    else if (!Characters.IsCharacterLaptopEquipped(charId))
                    {
                        player.EmitLocked("Client:Laptop:activateLaptop", true);
                        HUDHandler.SendNotification(player, 4, 2500, "Laptop angeschaltet");
                        Characters.SetCharacterLaptopEquipped(charId, true);
                    }
                }
                else if (itemname == "Schutzweste")
                {
                    CharactersInventory.RemoveCharacterItemAmount(charId, "Schutzweste", itemAmount, fromContainer);
                    Characters.SetCharacterArmor(charId, 100);
                    player.Armor = 100;
                    player.EmitLocked("Client:HUD:UpdateDesire", Characters.GetCharacterArmor(player.Armor), Characters.GetCharacterHealth(charId), Characters.GetCharacterHunger(charId), Characters.GetCharacterThirst(charId)); //HUD updaten
                }
                if (itemname == "Rucksack" || itemname == "Tasche" || itemname == "Armytasche")
                {
                    if (fromContainer == "backpack") { HUDHandler.SendNotification(player, 3, 5000, "Kleidungen & Taschen können nicht aus dem Rucksack aus benutzt werden."); return; }
                    if (Characters.GetCharacterBackpack(charId) == 66)
                    {
                        if (itemname == "Rucksack")
                        {
                            if (CharactersInventory.GetCharacterBackpackItemCount(charId) == 0)
                            {
                                var zwei = "-2";
                                Characters.SetCharacterBackpack(player, zwei);
                                HUDHandler.SendNotification(player, 2, 5000, "Du hast deinen Rucksack ausgezogen.");
                                player.EmitLocked("Client:SpawnArea:setCharClothes", 5, 0, 0);
                            }
                            else { HUDHandler.SendNotification(player, 4, 5000, "Du hast zuviele Sachen im Rucksack, du kannst deinen Rucksack nicht ablegen."); }
                        }
                        else
                        {
                            HUDHandler.SendNotification(player, 3, 5000, "Du hast bereits eine Tasche angelegt, lege diese vorher ab um deinen Rucksack anzulegen.");
                        }
                    }
                    else if (Characters.GetCharacterBackpack(charId) == 45)
                    {
                        if (itemname == "Tasche")
                        {
                            if (CharactersInventory.GetCharacterBackpackItemCount(charId) == 0)
                            {
                                Characters.SetCharacterBackpack(player, "-2");
                                player.EmitLocked("Client:SpawnArea:setCharClothes", 5, 0, 0);
                                HUDHandler.SendNotification(player, 2, 5000, "Du hast deine Tasche ausgezogen.");
                            }
                            else { HUDHandler.SendNotification(player, 4, 5000, "Du hast zuviele Sachen in deiner Tasche, du kannst deine Tasche nicht ablegen."); }
                        }
                        else
                        {
                            HUDHandler.SendNotification(player, 3, 5000, "Du hast bereits einen Rucksack angelegt, lege diesen vorher ab um deine Tasche anzulegen.");
                        }
                    }
                    else if (Characters.GetCharacterBackpack(charId) == 86)
                    {
                        if (itemname == "Armytasche")
                        {
                            if (CharactersInventory.GetCharacterBackpackItemCount(charId) == 0)
                            {
                                Characters.SetCharacterBackpack(player, "-2");
                                player.EmitLocked("Client:SpawnArea:setCharClothes", 5, 0, 0);
                                HUDHandler.SendNotification(player, 2, 5000, "Du hast deine Armytasche ausgezogen.");
                            }
                            else { HUDHandler.SendNotification(player, 4, 5000, "Du hast zuviele Sachen in deiner Armytasche, du kannst deine Armytasche nicht ablegen."); }
                        }
                        else
                        {
                            HUDHandler.SendNotification(player, 3, 5000, "Du hast bereits einen Rucksack oder erine Tasche angelegt, lege diesen vorher ab um deine Armytasche anzulegen.");
                        }
                    }
                    else if (Characters.GetCharacterBackpack(charId) == -2)
                    {
                        if (itemname == "Tasche")
                        {
                            Characters.SetCharacterBackpack(player, "Tasche");
                            HUDHandler.SendNotification(player, 2, 5000, "Du hast deine Tasche angezogen.");
                            //player.EmitLocked("Client:SpawnArea:setCharClothes", 5, 44, 0);
                        }
                        else if (itemname == "Rucksack")
                        {
                            Characters.SetCharacterBackpack(player, "Rucksack");
                            HUDHandler.SendNotification(player, 2, 5000, "Du hast deinen Rucksack angezogen");
                            //player.EmitLocked("Client:SpawnArea:setCharClothes", 5, 45, 0);
                        }
                        else if (itemname == "Armytasche")
                        {
                            Characters.SetCharacterBackpack(player, "Armytasche");
                            HUDHandler.SendNotification(player, 2, 5000, "Du hast deinen Armytasche angezogen");
                            //player.EmitLocked("Client:SpawnArea:setCharClothes", 5, 45, 0);
                        }
                    }
                }
                else if (itemname == "EC Karte")
                {
                    var atmPos = ServerATM.ServerATM_.FirstOrDefault(x => player.Position.IsInRange(new Position(x.posX, x.posY, x.posZ), 1f));
                    if (atmPos == null || player.IsInVehicle) { HUDHandler.SendNotification(player, 3, 5000, "Du bist an keinem ATM oder sitzt in einem Auto."); return; }
                    int usingAccountNumber = Convert.ToInt32(ECData);
                    if (CharactersBank.GetBankAccountLockStatus(usingAccountNumber)) { if (CharactersInventory.ExistCharacterItem(charId, "EC Karte " + usingAccountNumber, "inventory")) { CharactersInventory.RemoveCharacterItemAmount(charId, "EC Karte " + usingAccountNumber, 1, "inventory"); } HUDHandler.SendNotification(player, 3, 5000, $"Ihre EC Karte wurde einzogen da diese gesperrt ist."); return; }
                    player.EmitLocked("Client:ATM:BankATMcreateCEF", CharactersBank.GetBankAccountPIN(usingAccountNumber), usingAccountNumber, atmPos.zoneName);
                }
                else if (ServerItems.GetItemType(itemname) == "weapon")
                {
                    if (itemname.Contains("Munitionsbox"))
                    {
                        string wName = itemname.Replace(" Munitionsbox", "");
                        CharactersInventory.RemoveCharacterItemAmount(charId, itemname, itemAmount, fromContainer);
                        CharactersInventory.AddCharacterItem(charId, $"{wName} Munition", 30 * itemAmount, fromContainer);
                    }
                    else if (itemname.Contains("Munition")) { WeaponHandler.EquipCharacterWeapon(player, "Ammo", itemname, itemAmount, fromContainer); }
                    else { WeaponHandler.EquipCharacterWeapon(player, "Weapon", itemname, 0, fromContainer); }
                }
                else if (itemname == "Brecheisen")
                {
                    var house = ServerHouses.ServerHouses_.FirstOrDefault(x => x.ownerId > 0 && x.isLocked && ((ClassicColshape)x.entranceShape).IsInRange((ClassicPlayer)player));
                    if (house != null)
                    {
                        HouseHandler.BreakIntoHouse(player, house.id);
                        return;
                    }
                }
                else if (itemname == "Verbandskasten")
                {
                    CharactersInventory.RemoveCharacterItemAmount(charId, "Verbandskasten", itemAmount, fromContainer);
                    Characters.SetCharacterHealth(charId, 200);
                    player.Health = 200;
                    player.EmitLocked("Client:HUD:UpdateDesire", Characters.GetCharacterArmor(charId), Characters.GetCharacterHealth(charId), Characters.GetCharacterHunger(charId), Characters.GetCharacterThirst(charId)); //HUD updaten
                }
                else if (itemname == "Bandage")
                {
                    CharactersInventory.RemoveCharacterItemAmount(charId, "Bandage", itemAmount, fromContainer);
                    int rnd = new Random().Next(10, 120);
                    Characters.SetCharacterHealth(charId, rnd);
                    player.Health = (ushort)rnd;
                    player.EmitLocked("Client:HUD:UpdateDesire", Characters.GetCharacterArmor(charId), Characters.GetCharacterHealth(charId), Characters.GetCharacterHunger(charId), Characters.GetCharacterThirst(charId)); //HUD updaten
                }
                else if (itemname == "Weste")
                {
                    CharactersInventory.RemoveCharacterItemAmount(charId, "Weste", itemAmount, fromContainer);
                    Characters.SetCharacterArmor(charId, 100);
                    player.Armor = 100;
                    if (Characters.GetCharacterGender(charId)) player.EmitLocked("Client:SpawnArea:setCharClothes", 9, 17, 2);
                    else player.EmitLocked("Client:SpawnArea:setCharClothes", 9, 15, 2);
                    player.EmitLocked("Client:HUD:UpdateDesire", Characters.GetCharacterArmor(charId), Characters.GetCharacterHealth(charId), Characters.GetCharacterHunger(charId), Characters.GetCharacterThirst(charId)); //HUD updaten
                }
                else if (itemname == "Benzinkanister")
                {
                    foreach (IVehicle veh in Alt.GetAllVehicles())
                    {
                        /*                        !player.Position.IsInRange(targetPlayer.Position, 5f)
                        */
                        if (player.Position.IsInRange(veh.Position, 5f))
                        {

                            if ((ServerVehicles.GetVehicleFuel(veh) + 15.0f) >= ServerVehicles.GetVehicleFuelLimitOnHash(veh.Model))
                            {
                                HUDHandler.SendNotification(player, 4, 3500, "Das Fahrzeug ist zu voll zum Tanken");
                                return;
                            }
                            else if (player != null)
                            {
                                ulong vehID = veh.GetVehicleId();
                                if (charId <= 0 || vehID <= 0 || player.IsInVehicle) { HUDHandler.SendNotification(player, 4, 3500, "Das Fahrzeug ist zu voll zum Tanken"); return; }
                                var newfuel = ServerVehicles.GetVehicleFuel(veh) + 15.0f;
                                ServerVehicles.SetVehicleFuel(veh, newfuel);
                                CharactersInventory.RemoveCharacterItem(charId, "Benzinkanister", fromContainer);
                                HUDHandler.SendNotification(player, 3, 10000, "Fahrzeug wird befüllt, bitte warten.");
                                await Task.Delay(10000);
                                HUDHandler.SendNotification(player, 2, 3500, "Fahrzeug erfolgreich getankt");
                                CharactersInventory.AddCharacterItem(charId, "Leerer Benzinkanister", 1, "inventory");
                            }
                        }
                        //Kein Fahrzeug
                        else if (veh == null)
                        {
                            HUDHandler.SendNotification(player, 4, 3500, "Du bist nicht in der nähe eines Fahrzeuges.");
                            return;
                        }
                    }
                }
                else if (itemname == "Leerer Benzinkanister")
                {
                    var fuelSpot = ServerFuelStations.ServerFuelStationSpots_.FirstOrDefault(x => player.Position.IsInRange(new Position(x.posX, x.posY, x.posZ), 2.5f));
                    if (fuelSpot == null) { HUDHandler.SendNotification(player, 4, 5000, "Benzinkanister können nur an einer Tanke befüllt werden"); return; }
                    int fuelStationId = ServerFuelStations.GetFuelSpotParentStation(fuelSpot.id);
                    if (fuelStationId == 0) { HUDHandler.SendNotification(player, 4, 5000, "Ein unerwarteter Fehler ist aufgetreten. [FEHLERCODE: 002]"); return; }
                    int availableLiter = ServerFuelStations.GetFuelStationAvailableLiters(fuelStationId);
                    if (availableLiter < 1) { HUDHandler.SendNotification(player, 4, 5000, "Diese Tankstelle hat keinen Treibstoff mehr auf Lager."); return; }
                    if (CharactersInventory.ExistCharacterItem(charId, "Benzinkanister", "inventory")) { HUDHandler.SendNotification(player, 4, 3500, "Du hast bereits einen vollen Benzinkanister."); return; }
                    if (invWeight + itemWeight > 15f && backpackWeight + itemWeight > Characters.GetCharacterBackpackSize(Characters.GetCharacterBackpack(charId))) { HUDHandler.SendNotification(player, 3, 5000, $"Du hast nicht genug Platz in deinen Taschen."); return; }

                    else
                    {
                        if (invWeight + itemWeight <= 15f)
                        {
                            HUDHandler.SendNotification(player, 2, 10000, "Benzinkanister wird befüllt, bitte warten.");
                            CharactersInventory.RemoveCharacterItem(charId, "Leerer Benzinkanister", fromContainer);
                            await Task.Delay(10000);
                            CharactersInventory.RemoveCharacterItemAmount(charId, "Bargeld", 125, "inventory"); //15$ Benzin * 15L Benzin = 125$
                            CharactersInventory.AddCharacterItem(charId, "Benzinkanister", 1, "inventory");
                            HUDHandler.SendNotification(player, 2, 10000, "Benzinkanister ist nun voll.");
                        }
                        else
                        {
                            HUDHandler.SendNotification(player, 2, 10000, "Benzinkanister wird befüllt, bitte warten.");
                            CharactersInventory.RemoveCharacterItem(charId, "Leerer Benzinkanister", fromContainer);
                            await Task.Delay(10000);
                            CharactersInventory.RemoveCharacterItemAmount(charId, "Bargeld", 125, "inventory"); //15$ Benzin * 15L Benzin = 125$
                            CharactersInventory.AddCharacterItem(charId, "Benzinkanister", 1, "backpack");
                            HUDHandler.SendNotification(player, 2, 10000, "Benzinkanister ist nun voll.");
                        }
                    }
                }
                else if (itemname == "Pedalo")
                {
                    HUDHandler.SendNotification(player, 1, 3500, "Bruder muss los..");
                    player.EmitLocked("Client:Ragdoll:SetPedToRagdoll", true, 0); //Ragdoll setzen
                    player.EmitLocked("Client:Ragdoll:SetPedToRagdoll", false, 0); //Ragdoll setzen
                }
                else if (itemname == "Kokain")
                {
                    CharactersInventory.RemoveCharacterItemAmount(charId, "Kokain", itemAmount, fromContainer);
                    HUDHandler.SendNotification(player, 2, 2000, "Du hast Koks gezogen du bist nun 15 Minuten effektiver.");
                    player.EmitLocked("Client:Inventory:PlayEffect", "DrugsMichaelAliensFight", 900000);
                    Characters.SetCharacterFastFarm(charId, true, 15);

                }
                else if (itemname == "Joint")
                {
                    if (player.Armor >= 60) { HUDHandler.SendNotification(player, 3, 2000, "Weiter kannst du dich nicht selbst heilen."); return; }
                    CharactersInventory.RemoveCharacterItemAmount(charId, "Joint", itemAmount, fromContainer);
                    Characters.SetCharacterArmor(charId, Characters.GetCharacterArmor(charId) + 10);
                    player.Armor = +10;
                    player.EmitLocked("Client:HUD:UpdateDesire", Characters.GetCharacterArmor(charId), Characters.GetCharacterHealth(charId), Characters.GetCharacterHunger(charId), Characters.GetCharacterThirst(charId)); //HUD updaten
                }
                else if (itemname == "Smartphone")
                {
                    /*                    Alt.Log("Phone benutzt.");
                    */
                    if (Characters.IsCharacterPhoneEquipped(charId))
                    {
                        /*                        Alt.Log("Phone benutzt2.");
                        */
                        player.EmitLocked("Client:Smartphone:equipPhone", false, Characters.GetCharacterPhonenumber(charId), Characters.IsCharacterPhoneFlyModeEnabled(charId));
                        HUDHandler.SendNotification(player, 2, 1500, "Smartphone ausgeschaltet.");
                        Alt.Emit("Server:Smartphone:leaveRadioFrequence", player);
                    }
                    else
                    {
                        /*                        Alt.Log("Phone benutzt3.");
                        */
                        player.EmitLocked("Client:Smartphone:equipPhone", true, Characters.GetCharacterPhonenumber(charId), Characters.IsCharacterPhoneFlyModeEnabled(charId));
                        HUDHandler.SendNotification(player, 2, 1500, "Smartphone eingeschaltet.");
                    }
                    Characters.SetCharacterPhoneEquipped(charId, !Characters.IsCharacterPhoneEquipped(charId));
                    SmartphoneHandler.RequestLSPDIntranet((ClassicPlayer)player);
                }

                else
                {
                    Console.WriteLine(itemname);
                }

                if (ServerItems.hasItemAnimation(ServerItems.ReturnNormalItemName(itemname))) { InventoryAnimation(player, ServerItems.GetItemAnimationName(ServerItems.ReturnNormalItemName(itemname)), 0); }

                RequestInventoryItems(player);
                //HUDHandler.SendNotification(player, 2, 5000, $"DEBUG: Der Gegenstand {itemname} ({itemAmount}) wurde erfolgreich aus ({fromContainer}) benutzt.");
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }
        #endregion use

        #region drop
        [AsyncClientEvent("Server:Inventory:DropItem")]
        public async Task DropItem(ClassicPlayer player, string itemname, int itemAmount, string fromContainer)
        {
            try
            {
                if (player == null || !player.Exists || itemname == "" || itemAmount <= 0 || fromContainer == "" || User.GetPlayerOnline(player) == 0) return;
                if (player.HasPlayerHandcuffs() || player.HasPlayerRopeCuffs()) { HUDHandler.SendNotification(player, 3, 5000, "Wie willst du das mit Handschellen/Fesseln machen?"); return; }
                string normalItemName = ServerItems.ReturnNormalItemName(itemname);
                if (ServerItems.IsItemDroppable(itemname) == false) { HUDHandler.SendNotification(player, 4, 5000, $"Diesen Gegenstand kannst du nicht wegwerfen ({itemname})."); return; }
                int charId = player.CharacterId;
                if (charId <= 0 || CharactersInventory.ExistCharacterItem(charId, itemname, fromContainer) == false) return;
                if (CharactersInventory.GetCharacterItemAmount(charId, itemname, fromContainer) < itemAmount) { HUDHandler.SendNotification(player, 4, 5000, $"Die angegebene wegzuwerfende Anzahl ist nicht vorhanden ({itemname})."); return; }
                if (itemname == "Smartphone")
                {
                    if (Characters.IsCharacterPhoneEquipped(charId)) { HUDHandler.SendNotification(player, 3, 2500, "Du musst dein Handy erst ausschalten / ablegen."); return; }
                }
                else if (itemname == "Rucksack")
                {
                    if (Characters.GetCharacterBackpack(charId) == 31)
                    {
                        if (CharactersInventory.GetCharacterItemAmount(charId, "Rucksack", "inventory") == itemAmount)
                        {
                            HUDHandler.SendNotification(player, 3, 5000, "Du musst deinen Rucksack erst ablegen, bevor du diesen wegwerfen kannst.");
                            return;
                        }
                        else
                        {
                            CharactersInventory.RemoveCharacterItemAmount(charId, itemname, itemAmount, fromContainer);
                            InventoryAnimation(player, "drop", 0);
                            HUDHandler.SendNotification(player, 2, 5000, $"Der Gegenstand {itemname} ({itemAmount}) wurde erfolgreich weggeworfen ({fromContainer}).");
                            return;
                        }
                    }
                    else
                    {
                        CharactersInventory.RemoveCharacterItemAmount(charId, itemname, itemAmount, fromContainer);
                        InventoryAnimation(player, "drop", 0);
                        HUDHandler.SendNotification(player, 2, 5000, $"Der Gegenstand {itemname} ({itemAmount}) wurde erfolgreich weggeworfen ({fromContainer}).");
                        return;
                    }
                }
                else if (itemname == "Tasche")
                {
                    if (Characters.GetCharacterBackpack(charId) == 45)
                    {
                        if (CharactersInventory.GetCharacterItemAmount(charId, "Tasche", "inventory") == itemAmount)
                        {
                            HUDHandler.SendNotification(player, 3, 5000, "Du musst zuerst deine Tasche ablegen, bevor du diese wegwerfen kannst.");
                            return;
                        }
                        else
                        {
                            CharactersInventory.RemoveCharacterItemAmount(charId, itemname, itemAmount, fromContainer);
                            InventoryAnimation(player, "drop", 0);
                            HUDHandler.SendNotification(player, 2, 5000, $"Der Gegenstand {itemname} ({itemAmount}) wurde erfolgreich weggeworfen ({fromContainer}).");
                            RequestInventoryItems(player);
                            return;
                        }
                    }
                    else
                    {
                        CharactersInventory.RemoveCharacterItemAmount(charId, itemname, itemAmount, fromContainer);
                        InventoryAnimation(player, "drop", 0);
                        HUDHandler.SendNotification(player, 2, 5000, $"Der Gegenstand {itemname} ({itemAmount}) wurde erfolgreich weggeworfen ({fromContainer}).");
                        RequestInventoryItems(player);
                        return;
                    }
                }
                else if (ServerItems.GetItemType(itemname) == "weapon")
                {
                    if ((string)Characters.GetCharacterWeapon(player, "PrimaryWeapon") == normalItemName || (string)Characters.GetCharacterWeapon(player, "SecondaryWeapon") == normalItemName || (string)Characters.GetCharacterWeapon(player, "SecondaryWeapon2") == normalItemName || (string)Characters.GetCharacterWeapon(player, "FistWeapon") == normalItemName)
                    {
                        if (CharactersInventory.GetCharacterItemAmount(charId, normalItemName, fromContainer) == itemAmount) { HUDHandler.SendNotification(player, 3, 5000, "Du musst zuerst deine Waffe ablegen."); return; }
                    }
                }
                CharactersInventory.RemoveCharacterItemAmount(charId, itemname, itemAmount, fromContainer);
                InventoryAnimation(player, "drop", 0);
                HUDHandler.SendNotification(player, 2, 5000, $"Der Gegenstand {itemname} ({itemAmount}) wurde erfolgreich weggeworfen ({fromContainer}).");
                RequestInventoryItems(player);
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }
        #endregion

        #region give
        [AsyncClientEvent("Server:Inventory:GiveItem")]
        public async Task GiveItem(ClassicPlayer player, string itemname, int itemAmount, string fromContainer, int targetPlayerId)
        {
            try
            {
                if (player == null || !player.Exists || itemname == "" || itemAmount <= 0 || fromContainer == "" || targetPlayerId == 0) return;
                player.EmitLocked("Client:Inventory:closeCEF");
                if (player.HasPlayerHandcuffs() || player.HasPlayerRopeCuffs()) { HUDHandler.SendNotification(player, 3, 5000, "Wie willst du das mit Handschellen/Fesseln machen?"); return; }
                if (!ServerItems.IsItemGiveable(itemname)) { HUDHandler.SendNotification(player, 4, 5000, $"Diesen Gegenstand kannst du nicht weggeben ({itemname})."); return; }
                int charId = player.CharacterId;
                if (charId <= 0 || !CharactersInventory.ExistCharacterItem(charId, itemname, fromContainer)) return;
                if (CharactersInventory.GetCharacterItemAmount(charId, itemname, fromContainer) < itemAmount) { HUDHandler.SendNotification(player, 4, 5000, $"Die angegebene Anzahl ist nicht vorhanden ({itemname})."); return; }
                if (CharactersInventory.IsItemActive(player, itemname)) { HUDHandler.SendNotification(player, 4, 5000, $"Ausgerüstete Gegenstände können nicht abgegeben werden."); return; }
                float itemWeight = ServerItems.GetItemWeight(itemname) * itemAmount;
                float invWeight = CharactersInventory.GetCharacterItemWeight(targetPlayerId, "inventory");
                float backpackWeight = CharactersInventory.GetCharacterItemWeight(targetPlayerId, "backpack");
                var targetPlayer = Alt.GetAllPlayers().ToList().FirstOrDefault(x => x.GetCharacterMetaId() == (ulong)targetPlayerId);
                if (targetPlayer == null) return;
                if (!player.Position.IsInRange(targetPlayer.Position, 5f)) { HUDHandler.SendNotification(player, 4, 5000, "Fehler: Die Person ist zu weit entfernt."); return; }
                if (invWeight + itemWeight > 15f && backpackWeight + itemWeight > Characters.GetCharacterBackpackSize(Characters.GetCharacterBackpack(targetPlayerId))) { HUDHandler.SendNotification(player, 3, 5000, $"Der Spieler hat nicht genug Platz in seinen Taschen."); return; }
                var plate = itemname.Replace("Kaufvertrag ", "");
                int vehid = ServerVehicles.GetVehicleIdByPlate(plate);
                if (itemname.Contains("Kaufvertrag") && ServerVehicles.GetVehicleFactionId2(vehid) == 0)
                {
                    if (invWeight + itemWeight <= 15f)
                    {
                        HUDHandler.SendNotification(targetPlayer, 1, 5000, $"Du bist nun Besitzer vom Fahrzeug mit dem Kennzeichen:{plate}.");
                        HUDHandler.SendNotification(player, 2, 5000, $"Du hast einem Spieler den Gegenstand {itemname} ({itemAmount}x) gegeben.");
                        CharactersInventory.AddCharacterItem(targetPlayerId, itemname, itemAmount, "inventory");
                        CharactersInventory.RemoveCharacterItemAmount(charId, itemname, itemAmount, fromContainer);
                        ServerVehicles.SetVehicleNewOwner(targetPlayerId, plate);
                        InventoryAnimation(player, "give", 0);
                        return;
                    }

                    if (Characters.GetCharacterBackpack(targetPlayerId) != -2 && backpackWeight + itemWeight <= Characters.GetCharacterBackpackSize(Characters.GetCharacterBackpack(targetPlayerId)))
                    {
                        HUDHandler.SendNotification(targetPlayer, 1, 5000, $"Du bist nun Besitzer vom Fahrzeug mit dem Kennzeichen:{plate}.");
                        HUDHandler.SendNotification(player, 2, 5000, $"Du hast einem Spieler den Gegenstand {itemname} ({itemAmount}x) gegeben.");
                        CharactersInventory.AddCharacterItem(targetPlayerId, itemname, itemAmount, "backpack");
                        CharactersInventory.RemoveCharacterItemAmount(charId, itemname, itemAmount, fromContainer);
                        ServerVehicles.SetVehicleNewOwner(targetPlayerId, plate);
                        InventoryAnimation(player, "give", 0);
                        return;
                    }
                }
                else if (itemname.Contains("Generalschluessel"))
                {

                    if (invWeight + itemWeight <= 15f)
                    {
                        HUDHandler.SendNotification(targetPlayer, 1, 5000, $"Eine Person hat dir den Gegenstand {itemname} ({itemAmount}x) gegeben.");
                        HUDHandler.SendNotification(player, 2, 5000, $"Du hast einem Spieler den Gegenstand {itemname} ({itemAmount}x) gegeben.");
                        CharactersInventory.AddCharacterItem(targetPlayerId, itemname, itemAmount, "schluessel");
                        CharactersInventory.RemoveCharacterItemAmount(charId, itemname, itemAmount, fromContainer);
                        InventoryAnimation(player, "give", 0);
                        return;
                    }

                    if (Characters.GetCharacterBackpack(targetPlayerId) != -2 && backpackWeight + itemWeight <= Characters.GetCharacterBackpackSize(Characters.GetCharacterBackpack(targetPlayerId)))
                    {
                        HUDHandler.SendNotification(targetPlayer, 1, 5000, $"Eine Person hat dir den Gegenstand {itemname} ({itemAmount}x) gegeben.");
                        HUDHandler.SendNotification(player, 2, 5000, $"Du hast einem Spieler den Gegenstand {itemname} ({itemAmount}x) gegeben.");
                        CharactersInventory.AddCharacterItem(targetPlayerId, itemname, itemAmount, "schluessel");
                        CharactersInventory.RemoveCharacterItemAmount(charId, itemname, itemAmount, fromContainer);
                        InventoryAnimation(player, "give", 0);
                        return;
                    }
                }
                else if (itemname.Contains("Fahrzeugschluessel"))
                {

                    if (invWeight + itemWeight <= 15f)
                    {
                        HUDHandler.SendNotification(targetPlayer, 1, 5000, $"Eine Person hat dir den Gegenstand {itemname} ({itemAmount}x) gegeben.");
                        HUDHandler.SendNotification(player, 2, 5000, $"Du hast einem Spieler den Gegenstand {itemname} ({itemAmount}x) gegeben.");
                        CharactersInventory.AddCharacterItem(targetPlayerId, itemname, itemAmount, "schluessel");
                        CharactersInventory.RemoveCharacterItemAmount(charId, itemname, itemAmount, fromContainer);
                        InventoryAnimation(player, "give", 0);
                        return;
                    }

                    if (Characters.GetCharacterBackpack(targetPlayerId) != -2 && backpackWeight + itemWeight <= Characters.GetCharacterBackpackSize(Characters.GetCharacterBackpack(targetPlayerId)))
                    {
                        HUDHandler.SendNotification(targetPlayer, 1, 5000, $"Eine Person hat dir den Gegenstand {itemname} ({itemAmount}x) gegeben.");
                        HUDHandler.SendNotification(player, 2, 5000, $"Du hast einem Spieler den Gegenstand {itemname} ({itemAmount}x) gegeben.");
                        CharactersInventory.AddCharacterItem(targetPlayerId, itemname, itemAmount, "schluessel");
                        CharactersInventory.RemoveCharacterItemAmount(charId, itemname, itemAmount, fromContainer);
                        InventoryAnimation(player, "give", 0);
                        return;
                    }
                }
                else if (itemname.Contains("Handschellenschluessel"))
                {

                    if (invWeight + itemWeight <= 15f)
                    {
                        HUDHandler.SendNotification(targetPlayer, 1, 5000, $"Eine Person hat dir den Gegenstand {itemname} ({itemAmount}x) gegeben.");
                        HUDHandler.SendNotification(player, 2, 5000, $"Du hast einem Spieler den Gegenstand {itemname} ({itemAmount}x) gegeben.");
                        CharactersInventory.AddCharacterItem(targetPlayerId, itemname, itemAmount, "schluessel");
                        CharactersInventory.RemoveCharacterItemAmount(charId, itemname, itemAmount, fromContainer);
                        InventoryAnimation(player, "give", 0);
                        return;
                    }

                    if (Characters.GetCharacterBackpack(targetPlayerId) != -2 && backpackWeight + itemWeight <= Characters.GetCharacterBackpackSize(Characters.GetCharacterBackpack(targetPlayerId)))
                    {
                        HUDHandler.SendNotification(targetPlayer, 1, 5000, $"Eine Person hat dir den Gegenstand {itemname} ({itemAmount}x) gegeben.");
                        HUDHandler.SendNotification(player, 2, 5000, $"Du hast einem Spieler den Gegenstand {itemname} ({itemAmount}x) gegeben.");
                        CharactersInventory.AddCharacterItem(targetPlayerId, itemname, itemAmount, "schluessel");
                        CharactersInventory.RemoveCharacterItemAmount(charId, itemname, itemAmount, fromContainer);
                        InventoryAnimation(player, "give", 0);
                        return;
                    }
                }
                else
                {

                    if (invWeight + itemWeight <= 15f)
                    {
                        HUDHandler.SendNotification(targetPlayer, 1, 5000, $"Eine Person hat dir den Gegenstand {itemname} ({itemAmount}x) gegeben.");
                        HUDHandler.SendNotification(player, 2, 5000, $"Du hast einem Spieler den Gegenstand {itemname} ({itemAmount}x) gegeben.");
                        CharactersInventory.AddCharacterItem(targetPlayerId, itemname, itemAmount, "inventory");
                        CharactersInventory.RemoveCharacterItemAmount(charId, itemname, itemAmount, fromContainer);
                        InventoryAnimation(player, "give", 0);
                        return;
                    }

                    if (Characters.GetCharacterBackpack(targetPlayerId) != -2 && backpackWeight + itemWeight <= Characters.GetCharacterBackpackSize(Characters.GetCharacterBackpack(targetPlayerId)))
                    {
                        HUDHandler.SendNotification(targetPlayer, 1, 5000, $"Eine Person hat dir den Gegenstand {itemname} ({itemAmount}x) gegeben.");
                        HUDHandler.SendNotification(player, 2, 5000, $"Du hast einem Spieler den Gegenstand {itemname} ({itemAmount}x) gegeben.");
                        CharactersInventory.AddCharacterItem(targetPlayerId, itemname, itemAmount, "inventory");
                        CharactersInventory.RemoveCharacterItemAmount(charId, itemname, itemAmount, fromContainer);
                        InventoryAnimation(player, "give", 0);
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }
        #endregion

        #region take
        [AsyncClientEvent("Server:PlayerSearch:TakeItem")]
        public async Task PlayerSearchTakeItem(ClassicPlayer player, int givenTargetCharId, string itemName, string itemLocation, int itemAmount)
        {
            try
            {
                if (player == null || !player.Exists || givenTargetCharId <= 0 || itemName == "" || itemAmount <= 0 || itemLocation == "") return;
                int charId = player.CharacterId;
                if (charId <= 0) return;
                if (player.HasPlayerHandcuffs() || player.HasPlayerRopeCuffs()) { HUDHandler.SendNotification(player, 3, 5000, "Wie willst du das mit Handschellen/Fesseln machen?"); return; }
                var targetPlayer = Alt.GetAllPlayers().ToList().FirstOrDefault(x => x.GetCharacterMetaId() == (ulong)givenTargetCharId);
                int targetCharId = (int)targetPlayer.GetCharacterMetaId();
                if (targetCharId != givenTargetCharId) return;
                if (targetPlayer == null || !targetPlayer.Exists) return;
                if (!player.Position.IsInRange(targetPlayer.Position, 3f)) { HUDHandler.SendNotification(player, 3, 5000, "Fehler: Du bist zuweit vom Spieler entfernt."); return; }
                if (!targetPlayer.HasPlayerHandcuffs() && !targetPlayer.HasPlayerRopeCuffs()) { HUDHandler.SendNotification(player, 3, 5000, "Der Spieler ist nicht gefesselt."); return; }
                if (!ServerItems.IsItemDroppable(itemName) || !ServerItems.IsItemGiveable(itemName)) { HUDHandler.SendNotification(player, 3, 5000, "Fehler: Diesen Gegenstand kannst du nicht entfernen."); return; }
                if (!CharactersInventory.ExistCharacterItem(targetCharId, itemName, itemLocation)) { HUDHandler.SendNotification(player, 4, 5000, "Fehler: Dieser Gegenstand existiert nicht mehr."); return; }
                if (CharactersInventory.IsItemActive(targetPlayer, itemName)) { HUDHandler.SendNotification(player, 3, 5000, "Ausgerüstete Gegenstände können nicht entwendet werden."); return; }
                if (CharactersInventory.GetCharacterItemAmount(targetCharId, itemName, itemLocation) < itemAmount) { HUDHandler.SendNotification(player, 3, 5000, "Fehler: Soviele Gegenstände hat der Spieler davon nicht."); return; }
                float itemWeight = ServerItems.GetItemWeight(itemName) * itemAmount;
                float invWeight = CharactersInventory.GetCharacterItemWeight(charId, "inventory");
                float backpackWeight = CharactersInventory.GetCharacterItemWeight(charId, "backpack");
                if (itemName.Contains("Kaufvertrag")) { HUDHandler.SendNotification(player, 4, 3500, "Du kannst einem Spieler den Kaufvertrag nicht abnehmen."); return; }
                if (itemName.Contains("schluessel")) { HUDHandler.SendNotification(player, 4, 3500, "Du kannst einem Spieler keine Schluessel abnehmen."); return; }
                if (invWeight + itemWeight > 15f && backpackWeight + itemWeight > Characters.GetCharacterBackpackSize(Characters.GetCharacterBackpack(charId))) { HUDHandler.SendNotification(player, 3, 5000, $"Du hast nicht genug Platz in deinen Taschen."); return; }
                CharactersInventory.RemoveCharacterItemAmount(targetCharId, itemName, itemAmount, itemLocation);
                if (invWeight + itemWeight <= 15f || itemName == "Bargeld" || itemWeight == 0f)
                {
                    HUDHandler.SendNotification(player, 2, 5000, $"Du hast der Person {itemName} ({itemAmount}x) entwendet. (Lagerort: Inventar).");
                    HUDHandler.SendNotification(targetPlayer, 2, 5000, $"Dir wurde der Gegenstand {itemName} ({itemAmount}x) aus dem Inventar entwendet.");
                    CharactersInventory.AddCharacterItem(charId, itemName, itemAmount, "inventory");
                    return;
                }

                if (Characters.GetCharacterBackpack(charId) != -2 && backpackWeight + itemWeight <= Characters.GetCharacterBackpackSize(Characters.GetCharacterBackpack(charId)))
                {
                    HUDHandler.SendNotification(player, 2, 5000, $"Du hast der Person {itemName} ({itemAmount}x) entwendet. (Lagerort: Rucksack/Tasche).");
                    HUDHandler.SendNotification(targetPlayer, 2, 5000, $"Dir wurde der Gegenstand {itemName} ({itemAmount}x) aus dem Rucksack entwendet.");
                    CharactersInventory.AddCharacterItem(charId, itemName, itemAmount, "backpack");
                    return;
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }
        #endregion

        #region animation
        internal static void InventoryAnimation(IPlayer player, string Animation, int duration)
        {
            if (player == null || !player.Exists || player.IsInVehicle || Animation == "") return;
            if (Animation == "eat") player.EmitLocked("Client:Inventory:PlayAnimation", "mp_player_inteat@burger", "mp_player_int_eat_burger", 3500, 49, false);
            else if (Animation == "drink") player.EmitLocked("Client:Inventory:PlayAnimation", "amb@world_human_drinking@beer@male@idle_a", "idle_c", 3500, 49, false);
            else if (Animation == "drop") player.EmitLocked("Client:Inventory:PlayAnimation", "anim@narcotics@trash", "drop_front", 500, 1, false);
            else if (Animation == "give") player.EmitLocked("Client:Inventory:PlayAnimation", "anim@narcotics@trash", "drop_front", 500, 1, false); //ToDo: Give Animation raussuchen
            else if (Animation == "farmPickup") player.EmitLocked("Client:Inventory:PlayAnimation", "pickup_object", "pickup_low", duration, 1, false);
            else if (Animation == "handcuffs") player.EmitLocked("Client:Inventory:PlayAnimation", "mp_arresting", "sprint", duration, 49, false);
            else if (Animation == "revive") player.EmitLocked("Client:Inventory:PlayAnimation", "missheistfbi3b_ig8_2", "cpr_loop_paramedic", duration, 1, false);
            else if (Animation == "weste") player.EmitLocked("Client:Inventory:PlayAnimation", "anim@heists@narcotics@funding@gang_idle", "gang_chatting_idle01", 3000, 49, false);
            else if (Animation == "Kokain") player.EmitLocked("Client:Inventory:PlayAnimation", "anim@heists@narcotics@funding@gang_idle", "gang_chatting_idle01", 2000, 49, false);
            else if (Animation == "verband") player.EmitLocked("Client:Inventory:PlayAnimation", "anim@heists@narcotics@funding@gang_idle", "gang_chatting_idle01", 3000, 49, false);
            //Sheytan
            else if (Animation == "repair") player.EmitLocked("Client:Inventory:PlayAnimation", "mini@repair", "fixing_a_ped", duration, 1, false);
        }

        internal static void StopAnimation(IPlayer player, string animDict, string animName)
        {
            if (player == null || !player.Exists) return;
            player.EmitLocked("Client:Inventory:StopAnimation", animDict, animName);
        }

        //ToDo: Nach Charaktererstellung erste Kleidung setzen & ins Inventar geben
        //ToDo: neue Essensanimation raussuchen
        #endregion
    }
}
