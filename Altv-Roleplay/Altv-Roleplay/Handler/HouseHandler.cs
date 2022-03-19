using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using AltV.Net;
using AltV.Net.Elements.Entities;
using Altv_Roleplay.Model;
using Altv_Roleplay.Utils;
using Altv_Roleplay.models;
using Newtonsoft.Json;
using AltV.Net.Data;
using System.Threading.Tasks;
using System.Linq;
using AltV.Net.Enums;
using AltV.Net.Async;

namespace Altv_Roleplay.Handler
{
    class HouseHandler : IScript
    {
        #region allgemein
        internal static void openEntranceCEF(IPlayer player, int houseId)
        {
            try
            {
                if (player == null || !player.Exists || houseId <= 0) return;
                int charId = User.GetPlayerOnline(player);
                if (charId <= 0) return;
                player.EmitLocked("Client:HouseEntrance:openCEF", charId, ServerHouses.GetHouseInformationArray(houseId), ServerHouses.IsCharacterRentedInHouse(charId, houseId));
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [AsyncClientEvent("Server:House:BuyHouse")]
        public async Task BuyHouse(IPlayer player, int houseId)
        {
            try
            {
                if (player == null || houseId <= 0 || !player.Exists) return;
                int charId = User.GetPlayerOnline(player);
                if (charId <= 0) return;
                if (!ServerHouses.ExistHouse(houseId)) return;
                if(ServerHouses.GetHouseOwner(houseId) > 0) { HUDHandler.SendNotification(player, 3, 5000, "Fehler: Dieses Haus gehört bereits jemanden."); return; }
                if (!CharactersBank.HasCharacterBankMainKonto(charId)) { HUDHandler.SendNotification(player, 3, 5000, "Du hast noch kein Hauptkonto in der Bank festgelegt."); return; }
                int accNumber = CharactersBank.GetCharacterBankMainKonto(charId);
                if (accNumber <= 0) return;
                if (CharactersBank.GetBankAccountLockStatus(accNumber)) { HUDHandler.SendNotification(player, 3, 5000, "Dein Bankkonto ist gesperrt."); return; }
                if (CharactersBank.GetBankAccountMoney(accNumber) < ServerHouses.GetHousePrice(houseId)) { HUDHandler.SendNotification(player, 3, 5000, $"Soviel Geld hast du auf deinem Konto nicht ({ServerHouses.GetHousePrice(houseId)}$) - du hast {CharactersBank.GetBankAccountMoney(accNumber)}$"); return; }
                CharactersBank.SetBankAccountMoney(accNumber, CharactersBank.GetBankAccountMoney(accNumber) - ServerHouses.GetHousePrice(houseId));
                ServerBankPapers.CreateNewBankPaper(accNumber, DateTime.Now.ToString("d", CultureInfo.CreateSpecificCulture("de-DE")), DateTime.Now.ToString("t", CultureInfo.CreateSpecificCulture("de-DE")), "Ausgehende Überweisung", "Dynasty8", $"Hauskauf: {ServerHouses.GetHouseStreet(houseId)}", $"-{ServerHouses.GetHousePrice(houseId)}$", "Bankeinzug");
                ServerHouses.SetHouseOwner(houseId, charId);
                HUDHandler.SendNotification(player, 2, 5000, $"Sie haben sich das Haus mit der Adresse '{ServerHouses.GetHouseStreet(houseId)}' erfolgreich gekauft (Kosten: {ServerHouses.GetHousePrice(houseId)}$).");
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }
        internal static void LockHouse(IPlayer player, int houseId)
        {
            try
            {
                if (player == null || houseId <= 0 || !player.Exists) return;
                int charId = (int)player.GetCharacterMetaId();
                if (charId <= 0) return;
                if (!ServerHouses.ExistHouse(houseId)) return;
                if(ServerHouses.GetHouseOwner(houseId) != charId && !ServerHouses.IsCharacterRentedInHouse(charId, houseId)) { HUDHandler.SendNotification(player, 3, 5000, "Dieses Haus gehört nicht dir und / oder du bist nicht eingemietet."); return; }
                bool lockState = ServerHouses.IsHouseLocked(houseId);
                ServerHouses.SetHouseLocked(houseId, !lockState);
                if(lockState) { HUDHandler.SendNotification(player, 2, 2500, "Du hast das Haus aufgeschlossen."); }
                else { HUDHandler.SendNotification(player, 3, 2500, "Du hast das Haus abgeschlossen."); }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }
        

        [AsyncClientEvent("Server:House:EnterHouse")]
        public async Task EnterHouse(IPlayer player, int houseId)
        {
            try
            {
                if (player == null || houseId <= 0 || !player.Exists) return;
                int charId = (int)player.GetCharacterMetaId();
                if (charId <= 0) return;
                if (!ServerHouses.ExistHouse(houseId)) return;
                if (!player.Position.IsInRange(ServerHouses.GetHouseEntrance(houseId), 2f)) return;
                if(ServerHouses.GetHouseOwner(houseId) <= 0) { HUDHandler.SendNotification(player, 4, 5000, "Dieses Haus gehört Niemanden."); return; }
                if(ServerHouses.IsHouseLocked(houseId)) { HUDHandler.SendNotification(player, 4, 5000, "Das Haus ist abgeschlossen."); return; }
                int interiorId = ServerHouses.GetHouseInteriorId(houseId);
                if (interiorId <= 0) return;
                Position targetPos = ServerHouses.GetInteriorExitPosition(interiorId);
                if (targetPos == new Position(0, 0, 0)) return;
                player.Position = targetPos;
                player.Dimension = 10000 + houseId;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        internal static void LeaveHouse(IPlayer player, int interiorId)
        {
            try
            {
                if (player == null || interiorId <= 0 || !player.Exists) return;
                int charId = (int)player.GetCharacterMetaId();
                if (charId <= 0) return;
                if (player.Dimension <= 0 || player.Dimension - 10000 <= 0) return;
                int houseId = player.Dimension - 10000;
                if (houseId <= 0) return;
                if (!ServerHouses.ExistHouse(houseId)) return;
                Position housePos = ServerHouses.GetHouseEntrance(houseId);
                if (housePos == new Position(0, 0, 0)) return;
                player.Position = housePos;
                player.Dimension = 0;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }
        #endregion

        #region something
        internal static void openManageCEF(IPlayer player)
        {
            try
            {
                if (player == null || !player.Exists) return;
                int charId = (int)player.GetCharacterMetaId();
                if (charId <= 0) return;
                int dimension = player.Dimension;
                if (dimension <= 10000) return;
                int houseId = dimension - 10000;
                if (houseId <= 0 || !ServerHouses.ExistHouse(houseId) || ServerHouses.GetHouseOwner(houseId) != charId) return;
                var houseInfo = ServerHouses.GetHouseInformationArray(houseId);
                var renterInfo = ServerHouses.GetHouseRenterArray(houseId);
                if (houseInfo == "[]") return;
                player.EmitLocked("Client:HouseManage:openCEF", houseInfo, renterInfo);
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        internal static void openStorage(IPlayer player)
        {
            try
            {
                if (player == null || !player.Exists) return;
                int charId = (int)player.GetCharacterMetaId();
                if (charId <= 0) return;
                int dimension = player.Dimension;
                if (dimension <= 10000) return;
                int houseId = dimension - 10000;
                if (houseId <= 0 || !ServerHouses.ExistHouse(houseId)) return;
                if(!ServerHouses.HasHouseStorageUpgrade(houseId)) { HUDHandler.SendNotification(player, 4, 5000, "Dieses Haus besitzt noch keinen ausgebauten Lagerplatz."); return; }
                int interiorId = ServerHouses.GetHouseInteriorId(houseId);
                if (interiorId <= 0) return;
                if (!player.Position.IsInRange(ServerHouses.GetInteriorStoragePosition(interiorId), 2f)) return;
                var houseStorageContent = ServerHouses.GetServerHouseStorageItems(houseId); //Haus Inventar
                var characterInvArray = CharactersInventory.GetCharacterInventory(charId); //Spieler Inventar
                player.EmitLocked("Client:FactionStorage:openCEF", charId, houseId, "house", characterInvArray, houseStorageContent);
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }
        #endregion

        #region storage
        [AsyncClientEvent("Server:HouseStorage:StorageItem")]
        public async Task StorageItem(IPlayer player, int houseId, string itemName, int itemAmount, string fromContainer)
        {
            try
            {
                if (player == null || !player.Exists || houseId <= 0 || itemName == "" || itemName == "undefined" || itemAmount <= 0 || fromContainer == "none" || fromContainer == "") return;
                int charId = (int)player.GetCharacterMetaId();
                if (charId <= 0) return;
                if (!ServerHouses.ExistHouse(houseId)) return;
                if (!ServerHouses.HasHouseStorageUpgrade(houseId)) { HUDHandler.SendNotification(player, 4, 5000, "Dieses Haus besitzt noch keinen ausgebauten Lagerplatz."); return; }
                if (player.Dimension - 10000 <= 0 || player.Dimension - 10000 != houseId) return;
                if (player.HasPlayerHandcuffs() || player.HasPlayerRopeCuffs()) { HUDHandler.SendNotification(player, 3, 5000, "Wie willst du das mit Handschellen/Fesseln machen?"); return; }
                if (!CharactersInventory.ExistCharacterItem(charId, itemName, fromContainer)) { HUDHandler.SendNotification(player, 4, 5000, "Fehler: Diesen Gegenstand besitzt du nicht."); return; }
                if (CharactersInventory.GetCharacterItemAmount(charId, itemName, fromContainer) < itemAmount) { HUDHandler.SendNotification(player, 4, 5000, "Fehler: Du hast nicht genügend Gegenstände davon dabei."); return; }
                if (CharactersInventory.IsItemActive(player, itemName)) { HUDHandler.SendNotification(player, 4, 5000, "Fehler: Ausgerüstete Gegenstände können nicht umgelagert werden."); return; }
                float storageLimit = ServerHouses.GetInteriorStorageLimit(ServerHouses.GetHouseInteriorId(houseId));
                float itemWeight = ServerItems.GetItemWeight(itemName) * itemAmount;
                if (ServerHouses.GetHouseStorageItemWeight(houseId) >= storageLimit || (ServerHouses.GetHouseStorageItemWeight(houseId) + itemWeight >= storageLimit))
                {
                    HUDHandler.SendNotification(player, 3, 5000, $"Fehler: Soviel passt in das Hauslager nicht rein (maximal {storageLimit}kg Lagerplatz).");
                    return;
                }
                CharactersInventory.RemoveCharacterItemAmount(charId, itemName, itemAmount, fromContainer);
                ServerHouses.AddServerHouseStorageItem(houseId, itemName, itemAmount);
                HUDHandler.SendNotification(player, 2, 5000, $"Der Gegenstand wurde erfolgreich eingelagert ({itemName} - {itemAmount}x).");
                //ToDo: Log Eintrag
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }


        [AsyncClientEvent("Server:HouseStorage:TakeItem")]
        public async Task TakeItem(IPlayer player, int houseId, string itemName, int itemAmount)
        {
            try
            {
                if (player == null || !player.Exists || houseId <= 0 | itemAmount <= 0 || itemName == "" || itemName == "undefined") return;
                int charId = (int)player.GetCharacterMetaId();
                if (charId <= 0) return;
                if (!ServerHouses.ExistHouse(houseId)) return;
                if (!ServerHouses.HasHouseStorageUpgrade(houseId)) { HUDHandler.SendNotification(player, 4, 5000, "Dieses Haus besitzt noch keinen ausgebauten Lagerplatz."); return; }
                if (player.Dimension - 10000 <= 0 || player.Dimension - 10000 != houseId) return;
                if (player.HasPlayerHandcuffs() || player.HasPlayerRopeCuffs()) { HUDHandler.SendNotification(player, 3, 5000, "Wie willst du das mit Handschellen/Fesseln machen?"); return; }
                if (!ServerHouses.ExistServerHouseStorageItem(houseId, itemName)) { HUDHandler.SendNotification(player, 4, 5000, "Fehler: Der Gegenstand existiert im Hauslager nicht."); return; }
                if (ServerHouses.GetServerHouseStorageItemAmount(houseId, itemName) < itemAmount) { HUDHandler.SendNotification(player, 4, 5000, "Fehler: Soviele Gegenstände sind nicht im Hauslager."); return; }
                float itemWeight = ServerItems.GetItemWeight(itemName) * itemAmount;
                float invWeight = CharactersInventory.GetCharacterItemWeight(charId, "inventory");
                var itemType = ServerItems.GetItemType(itemName);
                float backpackWeight = CharactersInventory.GetCharacterItemWeight(charId, "backpack");
                if (invWeight + itemWeight > 15f && backpackWeight + itemWeight > Characters.GetCharacterBackpackSize(Characters.GetCharacterBackpack(charId))) { HUDHandler.SendNotification(player, 3, 5000, $"Du hast nicht genug Platz in deinen Taschen."); return; }
               ServerHouses.RemoveServerHouseStorageItemAmount(houseId, itemName, itemAmount);
                //ToDo: Log Eintrag
                if(itemName.Contains("Fahrzeugschluessel"))
                {
                    HUDHandler.SendNotification(player, 2, 5000, $"Du hast {itemName} ({itemAmount}x) aus dem Hauslager genommen (Lagerort: Schluesselbund).");
                    CharactersInventory.AddCharacterItem(charId, itemName, itemAmount, "schluessel");
                    return;
                } 
                if(itemName.Contains("Handschellenschluessel"))
                {
                    HUDHandler.SendNotification(player, 2, 5000, $"Du hast {itemName} ({itemAmount}x) aus dem Hauslager genommen (Lagerort: Schluesselbund).");
                    CharactersInventory.AddCharacterItem(charId, itemName, itemAmount, "schluessel");
                    return;
                } 
                if(itemName.Contains("Generalschluessel"))
                {
                    HUDHandler.SendNotification(player, 2, 5000, $"Du hast {itemName} ({itemAmount}x) aus dem Hauslager genommen (Lagerort: Schluesselbund).");
                    CharactersInventory.AddCharacterItem(charId, itemName, itemAmount, "schluessel");
                    return;
                } else
                {
                    if (invWeight + itemWeight <= 15f)
                    {
                        HUDHandler.SendNotification(player, 2, 5000, $"Du hast {itemName} ({itemAmount}x) aus dem Hauslager genommen (Lagerort: Inventar).");
                        CharactersInventory.AddCharacterItem(charId, itemName, itemAmount, "inventory");
                        return;
                    }

                    if (Characters.GetCharacterBackpack(charId) != -2 && backpackWeight + itemWeight <= Characters.GetCharacterBackpackSize(Characters.GetCharacterBackpack(charId)))
                    {
                        HUDHandler.SendNotification(player, 2, 5000, $"Du hast {itemName} ({itemAmount}x) aus dem Hauslager genommen (Lagerort: Rucksack / Tasche).");
                        CharactersInventory.AddCharacterItem(charId, itemName, itemAmount, "backpack");
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

        internal static async void BreakIntoHouse(IPlayer player, int houseId)
        {
            //Funktion: um in andere Häuser einzubrechen
            try
            {
                if (player == null || !player.Exists || houseId <= 0 || player.Dimension < 0 || player.Dimension > 0 || player.IsInVehicle) return;
                int charId = (int)player.GetCharacterMetaId();
                if (charId <= 0) return;
                if (player.HasPlayerHandcuffs() || player.HasPlayerRopeCuffs()) { HUDHandler.SendNotification(player, 3, 5000, "Wie willst du das mit Handschellen/Fesseln machen?"); return; }
                if (!ServerHouses.ExistHouse(houseId) || ServerHouses.GetHouseOwner(houseId) <= 0 || !ServerHouses.IsHouseLocked(houseId)) return;
                if (!CharactersInventory.ExistCharacterItem(charId, "Brecheisen", "inventory") && !CharactersInventory.ExistCharacterItem(charId, "Brecheisen", "backpack")) return;
                if(ServerFactions.GetFactionDutyMemberCount(2) < 6) { HUDHandler.SendNotification(player, 3, 5000, "Es sind nicht genügend Beamte im Dienst (6)."); return; }
                if(!player.IsPlayerUsingCrowbar())
                {
                    int houseOwner = ServerHouses.GetHouseOwner(houseId);
                    if (houseOwner <= 0) return;
                    //Aufbrechen Starten
                    if (DateTime.Now.Subtract(Convert.ToDateTime(Characters.GetCharacterLastLogin(houseOwner))).TotalHours >= 48) { HUDHandler.SendNotification(player, 4, 5000, "Fehler: Der Hausbesitzer war in den letzten 48 Stunden nicht online."); return; }
                    Position curPos = player.Position;
                    int duration = 300000;
                    var houseOwnerPlayer = Alt.GetAllPlayers().ToList().FirstOrDefault(x => x != null && x.Exists && x.GetCharacterMetaId() == (ulong)houseOwner);
                    if (ServerHouses.HasHouseAlarmUpgrade(houseId))
                    {
                        ServerFactions.createFactionDispatch(player, 2, $"Hauseinbruch: {ServerHouses.GetHouseStreet(houseId)}", $"Ein Einbruch in ein Haus wurde gemeldet - ein Dispatch wurde dazu in der Notrufverwaltung angelegt.");
                        if (houseOwnerPlayer != null && (CharactersInventory.ExistCharacterItem(houseOwner, "Tablet", "inventory") || CharactersInventory.ExistCharacterItem(houseOwner, "Tablet", "backpack")))
                        {
                            HUDHandler.SendNotification(houseOwnerPlayer, 3, 3500, $"Jemand bricht in dein Haus ein: {ServerHouses.GetHouseStreet(houseId)}");
                            HUDHandler.SendNotification(houseOwnerPlayer, 3, 3500, $"Jemand bricht in dein Haus ein: {ServerHouses.GetHouseStreet(houseId)}");
                            HUDHandler.SendNotification(houseOwnerPlayer, 3, 3500, $"Jemand bricht in dein Haus ein: {ServerHouses.GetHouseStreet(houseId)}");
                        }
                    }
                    player.GiveWeapon(WeaponModel.Crowbar, 1, true);
                    player.SetPlayerUsingCrowbar(true);
                    //ToDo: Animation
                    HUDHandler.SendNotification(player, 1, duration, "Aufbrechen des Hauses begonnen (5 Minuten)...");
                    await Task.Delay(duration);
                    if (player == null || !player.Exists) return;
                    if(!player.Position.IsInRange(curPos, 3f)) { HUDHandler.SendNotification(player, 3, 5000, "Aufbrechen abgebrochen, du bist zu weit entfernt."); player.SetPlayerUsingCrowbar(false); player.RemoveWeapon(WeaponModel.Crowbar); return; }
                    if (!player.IsPlayerUsingCrowbar()) return;
                    player.RemoveWeapon(WeaponModel.Crowbar);
                    ServerHouses.SetHouseLocked(houseId, false);
                    HUDHandler.SendNotification(player, 2, 2500, "Haus aufgebrochen, beeil dich.");                    
                    player.SetPlayerUsingCrowbar(false);
                    return;
                } 
                else
                {
                    //Einbruch: Abbrechen
                    player.EmitLocked("Client:Inventory:StopAnimation");
                    HUDHandler.SendNotification(player, 2, 1500, "Du hast den Einbruch abgebrochen.");
                    player.SetPlayerUsingCrowbar(false);
                    return;
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [AsyncClientEvent("Server:House:setMainHouse")]
        public async Task setMainHouse(IPlayer player, int houseId)
        {
            try
            {
                if (player == null || !player.Exists || houseId <= 0) return;
                int charId = (int)player.GetCharacterMetaId();
                if (charId <= 0 || !ServerHouses.ExistHouse(houseId)) return;
                if (!ServerHouses.ExistHouse(houseId) || ServerHouses.GetHouseOwner(houseId) != charId) { HUDHandler.SendNotification(player, 4, 3500, "Fehler: Dieses Haus gehört nicht dir."); return; }
                Characters.SetCharacterStreet(charId, ServerHouses.GetHouseStreet(houseId));
                HUDHandler.SendNotification(player, 2, 2500, $"Du hast dich erfolgreich auf die Adresse '{ServerHouses.GetHouseStreet(houseId)} gemeldet.");
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [AsyncClientEvent("Server:House:SellHouse")]
        public async Task SellHouse(IPlayer player, int houseId)
        {
            try
            {
                if (player == null || !player.Exists || houseId <= 0) return;
                int charId = (int)player.GetCharacterMetaId();
                if (charId <= 0) return;
                if(!ServerHouses.ExistHouse(houseId) || ServerHouses.GetHouseOwner(houseId) != charId) { HUDHandler.SendNotification(player, 4, 3500, "Fehler: Dieses Haus gehört nicht dir."); return; }
                if(!CharactersBank.HasCharacterBankMainKonto(charId)) { HUDHandler.SendNotification(player, 3, 2500, "Du besitzt kein Haupt-Bankkonto"); return; }
                int accNumber = CharactersBank.GetCharacterBankMainKonto(charId);
                int housePrice = ServerHouses.GetHousePrice(houseId) / 2;
                if (!CharactersBank.ExistBankAccountNumber(accNumber)) return;
                ServerHouses.SetHouseLocked(houseId, true);
                ServerHouses.SetHouseOwner(houseId, 0);
                CharactersBank.SetBankAccountMoney(accNumber, CharactersBank.GetBankAccountMoney(accNumber) + housePrice);
                HUDHandler.SendNotification(player, 2, 2500, $"Haus erfolgreich für {housePrice}$ (50% Kaufpreis) verkauft.");
                ServerBankPapers.CreateNewBankPaper(accNumber, DateTime.Now.ToString("d", CultureInfo.CreateSpecificCulture("de-DE")), DateTime.Now.ToString("t", CultureInfo.CreateSpecificCulture("de-DE")), "Eingehende Überweisung", "Immobilienmakler", $"Hausverkauf: {ServerHouses.GetHouseStreet(houseId)}", $"+{housePrice}$", "Online-Banking");
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [AsyncClientEvent("Server:HouseManage:DepositMoney")]
        public async Task DepositMoney(IPlayer player, int houseId, int money)
        {
            if (player == null || !player.Exists || houseId <= 0 || money <= 0) return;
            int charId = (int)player.GetCharacterMetaId();
            if (charId <= 0) return;
            int dimension = player.Dimension;
            if (dimension <= 10000) return;
            int dhouseId = dimension - 10000;
            if (dhouseId <= 0 || dhouseId != houseId || !ServerHouses.ExistHouse(houseId)) return;
            if (ServerHouses.GetHouseOwner(houseId) != charId) { HUDHandler.SendNotification(player, 4, 5000, "Fehler: Du bist nicht der Hausbesitzer."); return; }
            if(!CharactersInventory.ExistCharacterItem(charId, "Bargeld", "inventory") || CharactersInventory.GetCharacterItemAmount(charId, "Bargeld", "inventory") < money) { HUDHandler.SendNotification(player, 4, 3500, $"Fehler: Du hast nicht genügend Geld dabei ({money}$)."); return; }
            CharactersInventory.RemoveCharacterItemAmount(charId, "Bargeld", money, "inventory");
            ServerHouses.SetHouseBankMoney(houseId, ServerHouses.GetHouseBankMoney(houseId) + money);
            HUDHandler.SendNotification(player, 2, 2500, $"Du hast erfolgreich {money}$ in den Tresor gelagert.");
        }

        [AsyncClientEvent("Server:HouseManage:WithdrawMoney")]
        public async Task WithdrawMoney(IPlayer player, int houseId, int money)
        {
            try
            {
                if (player == null || !player.Exists || houseId <= 0 || money <= 0) return;
                int charId = (int)player.GetCharacterMetaId();
                if (charId <= 0) return;
                int dimension = player.Dimension;
                if (dimension <= 10000) return;
                int dhouseId = dimension - 10000;
                if (dhouseId <= 0 || dhouseId != houseId || !ServerHouses.ExistHouse(houseId)) return;
                if (ServerHouses.GetHouseOwner(houseId) != charId) { HUDHandler.SendNotification(player, 4, 5000, "Fehler: Du bist nicht der Hausbesitzer."); return; }
                if(ServerHouses.GetHouseBankMoney(houseId) < money) { HUDHandler.SendNotification(player, 4, 5000, $"Fehler: Soviel Geld ist nicht im Tresor (Aktueller Stand: {ServerHouses.GetHouseBankMoney(houseId)}$)."); return; }
                ServerHouses.SetHouseBankMoney(houseId, ServerHouses.GetHouseBankMoney(houseId) - money);
                CharactersInventory.AddCharacterItem(charId, "Bargeld", money, "inventory");
                HUDHandler.SendNotification(player, 2, 2500, $"Du hast erfolgreich {money}$ aus dem Tresor entnommen.");
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [AsyncClientEvent("Server:HouseManage:BuyUpgrade")]
        public async Task BuyUpgrade(IPlayer player, int houseId, string upgrade)
        {
            try
            {
                if (player == null || !player.Exists || houseId <= 0) return;
                if (upgrade != "alarm" && upgrade != "storage" && upgrade != "bank") return;
                int charId = (int)player.GetCharacterMetaId();
                if (charId <= 0) return; 
                int dimension = player.Dimension;
                if (dimension <= 10000) return;
                int dhouseId = dimension - 10000;
                if (dhouseId <= 0 || dhouseId != houseId || !ServerHouses.ExistHouse(houseId)) return;
                if (ServerHouses.GetHouseOwner(houseId) != charId) { HUDHandler.SendNotification(player, 4, 5000, "Fehler: Du bist nicht der Hausbesitzer."); return; }
                switch(upgrade)
                {
                    case "alarm":
                        if(ServerHouses.HasHouseAlarmUpgrade(houseId)) { HUDHandler.SendNotification(player, 4, 2500, "Dein Haus besitzt bereits eine Alarmanlage."); return; }
                        if (!ServerHouses.HasHouseBankUpgrade(houseId)) { HUDHandler.SendNotification(player, 4, 2500, "Du hast noch keinen Tresor ausgebaut in dem genügend Geld ist (500$)."); return; }
                        if(ServerHouses.GetHouseBankMoney(houseId) < 500) { HUDHandler.SendNotification(player, 4, 2500, "Dein Haustresor verfügt nicht über die Kosten (500$)."); return; }
                        ServerHouses.SetHouseBankMoney(houseId, ServerHouses.GetHouseBankMoney(houseId) - 500);
                        ServerHouses.SetHouseUpgradeState(houseId, "alarm", true);
                        HUDHandler.SendNotification(player, 2, 2500, $"Du hast das Hausupgrade 'Alarmanlage' erfolgreich erworben.");
                        return;
                    case "storage":
                        if (ServerHouses.HasHouseStorageUpgrade(houseId)) { HUDHandler.SendNotification(player, 4, 2500, "Dein Haus besitzt bereits eine Lagermöglichkeit."); return; }
                        if (!ServerHouses.HasHouseBankUpgrade(houseId)) { HUDHandler.SendNotification(player, 4, 2500, "Du hast noch keinen Tresor ausgebaut in dem genügend Geld ist (1500$)."); return; }
                        if (ServerHouses.GetHouseBankMoney(houseId) < 1500) { HUDHandler.SendNotification(player, 4, 2500, "Dein Haustresor verfügt nicht über die Kosten (1500$)."); return; }
                        ServerHouses.SetHouseBankMoney(houseId, ServerHouses.GetHouseBankMoney(houseId) - 1500);
                        ServerHouses.SetHouseUpgradeState(houseId, "storage", true);
                        HUDHandler.SendNotification(player, 2, 2500, $"Du hast das Hausupgrade 'Lagerraum' erfolgreich erworben.");
                        return;
                    case "bank":
                        if (ServerHouses.HasHouseBankUpgrade(houseId)) { HUDHandler.SendNotification(player, 4, 2500, "Dein Haus besitzt bereits einen Tersor."); return; }
                        if(!CharactersBank.HasCharacterBankMainKonto(charId)) { HUDHandler.SendNotification(player, 4, 5000, "Du besitzt noch kein Hauptkonto in deiner Bank."); return; }
                        int accNumber = CharactersBank.GetCharacterBankMainKonto(charId);
                        if (accNumber <= 0) return;
                        if(CharactersBank.GetBankAccountMoney(accNumber) < 250) { HUDHandler.SendNotification(player, 4, 5000, "Dein Hauptkonto ist nicht ausreichend gedeckt (250$)."); return; }
                        CharactersBank.SetBankAccountMoney(accNumber, CharactersBank.GetBankAccountMoney(accNumber) - 250);
                        ServerHouses.SetHouseUpgradeState(houseId, "bank", true);
                        HUDHandler.SendNotification(player, 2, 2500, $"Du hast das Hausupgrade 'Tresor' erfolgreich erworben.");
                        return;
                }                
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [AsyncClientEvent("Server:HouseManage:RemoveRenter")]
        public async Task RemoveRenter(IPlayer player, int houseId, int renterId)
        {
            try
            {
                if (player == null || !player.Exists || houseId <= 0 || renterId <= 0) return;
                int charId = (int)player.GetCharacterMetaId();
                if (charId <= 0) return;
                int dimension = player.Dimension;
                if (dimension <= 10000) return;
                int dhouseId = dimension - 10000;
                if (dhouseId <= 0 || dhouseId != houseId || !ServerHouses.ExistHouse(houseId)) return;
                if (ServerHouses.GetHouseOwner(houseId) != charId) { HUDHandler.SendNotification(player, 4, 5000, "Fehler: Du bist nicht der Hausbesitzer."); return; }
                if(!ServerHouses.IsCharacterRentedInAnyHouse(renterId) || !ServerHouses.IsCharacterRentedInHouse(renterId, houseId)) { HUDHandler.SendNotification(player, 4, 5000, "Fehler: Dieser Spieler ist nicht in deinem Haus eingemietet."); return; }
                ServerHouses.RemoveServerHouseRenter(houseId, renterId);
                HUDHandler.SendNotification(player, 2, 2000, $"Du hast den Mieter {Characters.GetCharacterName(renterId)} erfolgreich gekündigt.");
                foreach(var renterPlayer in Alt.GetAllPlayers().ToList().Where(x => x != null && x.Exists && x.GetCharacterMetaId() == (ulong)renterId)) { HUDHandler.SendNotification(renterPlayer, 3, 2000, $"Dein Mietvertrag in dem Haus '{ServerHouses.GetHouseStreet(houseId)}' wurde gekündigt."); break; }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [AsyncClientEvent("Server:HouseManage:setRentState")]
        public async Task setRentState(IPlayer player, int houseId, string rentState)
        {
            try
            {
                if (player == null || !player.Exists || houseId <= 0) return;
                if (rentState != "true" && rentState != "false") return;
                int charId = (int)player.GetCharacterMetaId();
                if (charId <= 0) return;
                bool rentBool = rentState == "true";
                int dimension = player.Dimension;
                if (dimension <= 10000) return;
                int dhouseId = dimension - 10000;
                if (dhouseId <= 0 || dhouseId != houseId || !ServerHouses.ExistHouse(houseId)) return;
                if (ServerHouses.GetHouseOwner(houseId) != charId) { HUDHandler.SendNotification(player, 4, 5000, "Fehler: Du bist nicht der Hausbesitzer."); return; }
                ServerHouses.SetHouseRentState(houseId, rentBool);
                if(rentBool) { HUDHandler.SendNotification(player, 2, 2500, $"Du hast den Mietstatus auf 'Mieter zulassen' gestellt."); return; }
                else { HUDHandler.SendNotification(player, 2, 2500, $"Du hast den Mietstatus auf 'Mieter nicht zulassen' gestellt."); return; }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [AsyncClientEvent("Server:HouseManage:setRentPrice")]
        public async Task setRentPrice(IPlayer player, int houseId, int rentPrice)
        {
            try
            {
                if (player == null || !player.Exists || houseId <= 0 || rentPrice <= 0) return;
                int charId = (int)player.GetCharacterMetaId();
                if (charId <= 0) return;
                if(rentPrice > 1000) { HUDHandler.SendNotification(player, 4, 5000, "Fehler: Die Miete darf einen Wert von 1.000$ nicht überschreiten."); return; }
                int dimension = player.Dimension;
                if (dimension <= 10000) return;
                int dhouseId = dimension - 10000;
                if (dhouseId <= 0 || dhouseId != houseId || !ServerHouses.ExistHouse(houseId)) return;
                if(ServerHouses.GetHouseOwner(houseId) != charId) { HUDHandler.SendNotification(player, 4, 5000, "Fehler: Du bist nicht der Hausbesitzer."); return; }
                ServerHouses.SetHouseRentPrice(houseId, rentPrice);
                HUDHandler.SendNotification(player, 2, 3000, $"Du hast den Mietpreis auf {rentPrice}$ festgelegt. Die Miete wird jede 7 Tage von Mietbeginn eines Mieters abgebucht.");
                if(!ServerHouses.HasHouseBankUpgrade(houseId)) { HUDHandler.SendNotification(player, 3, 5000, "Dein Haus besitzt keinen Tresor. Mieteinnahmen werden erst gesammelt sofern dieser ausgebaut ist."); }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [AsyncClientEvent("Server:House:RentHouse")]
        public async Task RentHouse(IPlayer player, int houseId)
        {
            try
            {
                if (player == null || !player.Exists || houseId <= 0) return;
                int charId = (int)player.GetCharacterMetaId();
                if (charId <= 0) return;
                if (!ServerHouses.ExistHouse(houseId)) return;
                if (ServerHouses.GetHouseOwner(houseId) <= 0 || ServerHouses.GetHouseOwner(houseId) == charId) return;
                if(ServerHouses.IsCharacterRentedInAnyHouse(charId)) { HUDHandler.SendNotification(player, 4, 5000, "Fehler: Du bist bereits in einem anderen Haus eingemietet."); return; }
                if(ServerHouses.IsCharacterRentedInHouse(charId, houseId)) { HUDHandler.SendNotification(player, 4, 5000, "Fehler: In diesem Haus bist du bereits eingemietet."); return; }
                if(!CharactersBank.HasCharacterBankMainKonto(charId)) { HUDHandler.SendNotification(player, 4, 5000, "Fehler: Du besitzt kein Haupt-Bankkonto."); return; }
                int accNumber = CharactersBank.GetCharacterBankMainKonto(charId);
                int ownerBankNumber = CharactersBank.GetCharacterBankMainKonto(ServerHouses.GetHouseOwner(houseId));
                int rentPrice = ServerHouses.GetHouseRentPrice(houseId);
                if (accNumber <= 0 || rentPrice <= 0 || ownerBankNumber <= 0) return;
                if (CharactersBank.GetBankAccountLockStatus(accNumber)) { HUDHandler.SendNotification(player, 4, 5000, "Fehler: Dein Hauptkonto ist gesperrt."); return; }
                if(CharactersBank.GetBankAccountMoney(accNumber) < rentPrice) { HUDHandler.SendNotification(player, 4, 5000, "Fehler: Dein Hauptkonto ist nicht ausreichend gedeckt."); return; }
                CharactersBank.SetBankAccountMoney(accNumber, CharactersBank.GetBankAccountMoney(accNumber) - rentPrice);
                CharactersBank.SetBankAccountMoney(ownerBankNumber, CharactersBank.GetBankAccountMoney(ownerBankNumber) + rentPrice);
                ServerHouses.AddServerHouseRenter(houseId, charId);
                ServerBankPapers.CreateNewBankPaper(accNumber, DateTime.Now.ToString("d", CultureInfo.CreateSpecificCulture("de-DE")), DateTime.Now.ToString("t", CultureInfo.CreateSpecificCulture("de-DE")), "Ausgehende Überweisung", $"{ownerBankNumber}", "Mietvertrag", $"-{rentPrice}$", "Bankeinzug");
                ServerBankPapers.CreateNewBankPaper(ownerBankNumber, DateTime.Now.ToString("d", CultureInfo.CreateSpecificCulture("de-DE")), DateTime.Now.ToString("t", CultureInfo.CreateSpecificCulture("de-DE")), "Eingehende Überweisung", $"{accNumber}", "Mietvertrag", $"+{rentPrice}$", "Bankeinzug");
                HUDHandler.SendNotification(player, 2, 5000, $"Du hast dich erfolgreich eingemietet, die Miete beträgt {rentPrice}$ täglich welche von deinem Hauptkonto abgezogen werden.");
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [AsyncClientEvent("Server:House:UnrentHouse")]
        public async Task UnrentHouse(IPlayer player, int houseId)
        {
            try
            {
                if (player == null || !player.Exists || houseId <= 0) return;
                int charId = (int)player.GetCharacterMetaId();
                if (charId <= 0) return;
                if (!ServerHouses.ExistHouse(houseId) || ServerHouses.GetHouseOwner(houseId) <= 0) return;
                if (!ServerHouses.IsCharacterRentedInAnyHouse(charId) || !ServerHouses.IsCharacterRentedInHouse(charId, houseId)) return;
                ServerHouses.RemoveServerHouseRenter(houseId, charId);
                HUDHandler.SendNotification(player, 2, 3000, "Du hast dich aus dem Haus ausgemietet.");
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }
    }
}
