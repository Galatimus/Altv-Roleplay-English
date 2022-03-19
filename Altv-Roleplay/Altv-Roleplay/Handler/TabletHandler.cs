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
using Altv_Roleplay.Model;
using Altv_Roleplay.Services;
using Altv_Roleplay.Utils;
using Newtonsoft.Json;

namespace Altv_Roleplay.Handler
{
    class TabletHandler : IScript
    {
        [AsyncClientEvent("Server:Tablet:openCEF")]
        public async Task openCEF(IPlayer player)
        {
            try
            {
                if (player == null || !player.Exists) return;
                int charId = User.GetPlayerOnline(player);
                if (charId <= 0) return;
                if (player.HasPlayerHandcuffs() || player.HasPlayerRopeCuffs()) { HUDHandler.SendNotification(player, 4, 5000, "Wie willst du das mit Handschellen/Fesseln machen?"); return; }
                if (!CharactersInventory.ExistCharacterItem(charId, "Tablet", "inventory") && !CharactersInventory.ExistCharacterItem(charId, "Tablet", "backpack")) { HUDHandler.SendNotification(player, 3, 5000, "Du besitzt kein Tablet."); return; }
                player.EmitLocked("Client:Tablet:createCEF");
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [AsyncClientEvent("Server:Tablet:RequestTabletData")]
        public async Task RequestTabletData(IPlayer player)
        {
            try
            {
                if (player == null || !player.Exists) return;
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                int charId = User.GetPlayerOnline(player);
                if (charId == 0) return;
                RefreshTabletData(player, true);
                stopwatch.Stop();
                Alt.Log($"{charId} - RequestTabletData benötigte {stopwatch.Elapsed.Milliseconds}ms");
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static void RefreshTabletData(IPlayer player, bool openTablet)
        {
            try
            {
                if (player == null || !player.Exists) return;
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                int charId = User.GetPlayerOnline(player);
                if (charId <= 0) return;
                if (!CharactersTablet.HasCharacterTutorialEntryFinished(charId, "openTablet"))
                {
                    CharactersTablet.SetCharacterTutorialEntryState(charId, "openTablet", true);
                    HUDHandler.SendNotification(player, 1, 5000, "Erfolg abgeschlossen: Tablet öffnen.");
                }
                var appArray = CharactersTablet.GetCharacterTabletApps(charId);
                var vehicleStoreArray = ServerVehicleShops.GetTabletVehicleStoreItems();
                Global.mGlobal.VirtualAPI.TriggerClientEventSafe(player, "Client:Tablet:setTabletHomeAppData", CharactersTablet.GetCharacterTabletApps(charId));
                Global.mGlobal.VirtualAPI.TriggerClientEventSafe(player, "Client:Tablet:SetVehicleStoreAppContent", vehicleStoreArray);
                Global.mGlobal.VirtualAPI.TriggerClientEventSafe(player, "Client:Tablet:SetTutorialAppContent", CharactersTablet.GetCharacterTabletTutorialEntrys(charId));

                if (CharactersTablet.HasCharacterTabletApp(charId, "banking"))
                {
                    var bankingAppOwnerArray = CharactersTablet.GetCharacterTabletBankingAppOwnerInfo(charId);
                    var bankPaperArray = ServerBankPapers.GetTabletBankAccountBankPaper(CharactersBank.GetCharacterBankMainKonto(charId));
                    Global.mGlobal.VirtualAPI.TriggerClientEventSafe(player, "Client:Tablet:SetBankingAppContent", bankingAppOwnerArray, bankPaperArray);
                }

                if (CharactersTablet.HasCharacterTabletApp(charId, "events"))
                {
                    var eventArray = CharactersTablet.GetServerTabletEvents();
                    Global.mGlobal.VirtualAPI.TriggerClientEventSafe(player, "Client:Tablet:SetEventsAppContent", eventArray);
                }

                if (CharactersTablet.HasCharacterTabletApp(charId, "notices"))
                {
                    var notesArray = CharactersTablet.GetCharacterTabletNotes(charId);
                    Global.mGlobal.VirtualAPI.TriggerClientEventSafe(player, "Client:Tablet:NotesAppAddNotesContent", notesArray);
                }

                if (CharactersTablet.HasCharacterTabletApp(charId, "vehicles"))
                {
                    var vehicleArray = CharactersTablet.GetCharacterTabletVehicles(charId);
                    Global.mGlobal.VirtualAPI.TriggerClientEventSafe(player, "Client:Tablet:SetVehiclesAppContent", vehicleArray);
                }

                if (ServerCompanys.IsCharacterInAnyServerCompany(charId))
                {
                    var memberArray = ServerCompanys.GetServerCompanyMembers(ServerCompanys.GetCharacterServerCompanyId(charId));
                    var companyInfos = ServerCompanys.GetServerCompanyInfos(ServerCompanys.GetCharacterServerCompanyId(charId));
                    Global.mGlobal.VirtualAPI.TriggerClientEventSafe(player, "Client:Tablet:SetCompanyAppContent", ServerCompanys.GetCharacterServerCompanyId(charId), companyInfos, memberArray);
                }

                if (ServerFactions.IsCharacterInAnyFaction(charId))
                {
                    int charRank = ServerFactions.GetCharacterFactionRank(charId);
                    if (charRank >= ServerFactions.GetFactionMaxRankCount(ServerFactions.GetCharacterFactionId(charId)) - 2)
                    {
                        var infoArray = ServerFactions.GetServerFactionManagerInfos(ServerFactions.GetCharacterFactionId(charId));
                        var memberArray = ServerFactions.GetServerFactionMembers(ServerFactions.GetCharacterFactionId(charId));
                        var rankArray = ServerFactions.GetServerFactionRanks(ServerFactions.GetCharacterFactionId(charId));
                        Global.mGlobal.VirtualAPI.TriggerClientEventSafe(player, "Client:Tablet:SetFactionManagerAppContent", ServerFactions.GetCharacterFactionId(charId), infoArray, memberArray, rankArray);
                    }

                    if (ServerFactions.IsCharacterInFactionDuty(charId) && ServerFactions.GetCharacterFactionId(charId) > 0)
                    {
                        int factionDutyMemberAmount = ServerFactions.GetFactionDutyMemberCount(ServerFactions.GetCharacterFactionId(charId));
                        string factionShort = ServerFactions.GetFactionShortName(ServerFactions.GetCharacterFactionId(charId));
                        Global.mGlobal.VirtualAPI.TriggerClientEventSafe(player, "Client:Tablet:SetFactionAppContent", factionDutyMemberAmount, ServerVehicles.GetAllParkedOutFactionVehicles(factionShort));

                        string dispatches = ServerFactions.GetFactionDispatches(ServerFactions.GetCharacterFactionId(charId));
                        if (dispatches != "[]") Global.mGlobal.VirtualAPI.TriggerClientEventSafe(player, "Client:Tablet:setDispatches", ServerFactions.GetCharacterFactionId(charId), dispatches);
                    }
                }

                if (openTablet) Global.mGlobal.VirtualAPI.TriggerClientEventSafe(player, "Client:Tablet:finaly");
                stopwatch.Stop();
                Alt.Log($"{charId} - RefreshTabletData benötigte {stopwatch.Elapsed.Milliseconds}ms");
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [AsyncClientEvent("Server:Tablet:AppStoreInstallUninstallApp")]
        public async Task AppStoreInstallUninstallApp(IPlayer player, string appName, bool isInstalling)
        {
            try
            {
                if (player == null || !player.Exists || appName == "" || appName == "undefined") return;
                int charId = User.GetPlayerOnline(player);
                if (charId == 0) return;
                if (isInstalling == true)
                {
                    if (!CharactersBank.HasCharacterBankMainKonto(charId)) { HUDHandler.SendNotification(player, 4, 5000, "Du hast noch kein Hauptkonto in deiner Bankfiliale festgelegt."); return; }
                    int accountNumber = CharactersBank.GetCharacterBankMainKonto(charId);
                    int appPrice = CharactersTablet.GetServerTabletAppPrice(appName);
                    if (CharactersBank.GetBankAccountLockStatus(accountNumber)) { HUDHandler.SendNotification(player, 3, 5000, $"Dieses Bankkonto ist gesperrt und kann nicht weiter benutzt werden."); return; }
                    if (appPrice > CharactersBank.GetBankAccountMoney(accountNumber)) { HUDHandler.SendNotification(player, 3, 5000, $"Du hast nicht genügend Geld auf deinem Konto ({appPrice}$)."); return; }
                    DateTime dateTime = DateTime.Now;
                    CharactersBank.SetBankAccountMoney(accountNumber, (CharactersBank.GetBankAccountMoney(accountNumber) - appPrice));
                    ServerBankPapers.CreateNewBankPaper(accountNumber, dateTime.ToString("dd.MM.yyyy"), dateTime.ToString("HH.mm"), "Ausgehende Überweisung", "Tablet App-Store", $"App-Kauf: {appName}", $"-{appPrice}$", "Online");
                    CharactersTablet.ChangeCharacterTabletAppInstallState(charId, appName, true);
                    HUDHandler.SendNotification(player, 2, 5000, $"Sie haben die App erfolgreich für {appPrice}$ installiert.");
                }
                else
                {
                    CharactersTablet.ChangeCharacterTabletAppInstallState(charId, appName, false);
                    HUDHandler.SendNotification(player, 2, 5000, $"Sie haben die App erfolgreich deinstalliert.");
                }
                RefreshTabletData(player, false);
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [AsyncClientEvent("Server:Tablet:BankingAppNewTransaction")]
        public async Task BankingAppNewTransaction(IPlayer player, int targetAccountNumber, string transactionMessage, int moneyAmount)
        {
            try
            {
                if (player == null || targetAccountNumber == 0 || moneyAmount < 1) return;
                int charId = User.GetPlayerOnline(player);
                if (charId == 0) return;
                if (!CharactersBank.HasCharacterBankMainKonto(charId)) { HUDHandler.SendNotification(player, 4, 5000, "Du hast noch kein Hauptkonto in deiner Bankfiliale festgelegt."); return; }
                int ownAccountNumber = CharactersBank.GetCharacterBankMainKonto(charId);
                if (CharactersBank.GetBankAccountLockStatus(ownAccountNumber)) { HUDHandler.SendNotification(player, 3, 5000, $"Dieses Bankkonto ist gesperrt und kann nicht weiter benutzt werden."); return; }
                if (CharactersBank.GetBankAccountMoney(ownAccountNumber) < moneyAmount) { HUDHandler.SendNotification(player, 3, 5000, $"Du hast nicht genügend Geld auf deinem Konto ({moneyAmount}$)."); return; }
                DateTime dateTime = DateTime.Now;
                CharactersBank.SetBankAccountMoney(ownAccountNumber, (CharactersBank.GetBankAccountMoney(ownAccountNumber) - moneyAmount));
                CharactersBank.SetBankAccountMoney(targetAccountNumber, (CharactersBank.GetBankAccountMoney(targetAccountNumber) + moneyAmount));
                ServerBankPapers.CreateNewBankPaper(ownAccountNumber, dateTime.ToString("dd.MM.yyyy"), dateTime.ToString("HH.mm"), "Ausgehende Überweisung", $"{targetAccountNumber}", $"{transactionMessage}", $"-{moneyAmount}$", "Online Banking");
                ServerBankPapers.CreateNewBankPaper(targetAccountNumber, dateTime.ToString("dd.MM.yyyy"), dateTime.ToString("HH.mm"), "Eingehende Überweisung", $"{ownAccountNumber}", $"{transactionMessage}", $"+{moneyAmount}$", "Online Banking");
                HUDHandler.SendNotification(player, 2, 5000, $"Sie haben erfolgreich eine Überweisung i.H.v. {moneyAmount}$ an die Kontonummer {targetAccountNumber} getätigt.");
                RefreshTabletData(player, false);
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [AsyncClientEvent("Server:Tablet:EventsAppNewEntry")]
        public async Task EventsAppNewEntry(IPlayer player, string title, string callNumber, string eventDate, string eventTime, string location, string eventType, string information)
        {
            try
            {
                if (player == null || !player.Exists || title == "" || title == "undefined" || eventDate == "" || eventTime == "" || location == "" || location == "undefined" || information == "") return;
                int charId = User.GetPlayerOnline(player);
                if (charId == 0) return;
                if (!CharactersBank.HasCharacterBankMainKonto(charId)) { HUDHandler.SendNotification(player, 4, 5000, "Du hast noch kein Hauptkonto in deiner Bankfiliale festgelegt."); return; }
                int ownAccountNumber = CharactersBank.GetCharacterBankMainKonto(charId);
                if (CharactersBank.GetBankAccountLockStatus(ownAccountNumber)) { HUDHandler.SendNotification(player, 3, 5000, $"Dieses Bankkonto ist gesperrt und kann nicht weiter benutzt werden."); return; }
                if (CharactersBank.GetBankAccountMoney(ownAccountNumber) < 250) { HUDHandler.SendNotification(player, 3, 5000, $"Du hast nicht genügend Geld auf deinem Konto (250$)."); return; }
                CharactersBank.SetBankAccountMoney(ownAccountNumber, (CharactersBank.GetBankAccountMoney(ownAccountNumber) - 250));
                DateTime dateTime = DateTime.Now;
                ServerBankPapers.CreateNewBankPaper(ownAccountNumber, dateTime.ToString("dd.MM.yyyy"), dateTime.ToString("HH.mm"), "Ausgehende Überweisung", $"Event Dienstleister", $"Event-Eintrag: {eventDate}", $"-250$", "Online Banking");
                CharactersTablet.CreateServerTabletEvent(charId, title, callNumber, eventDate, eventTime, location, eventType, information);
                HUDHandler.SendNotification(player, 2, 5000, $"Sie haben erfolgreich ein Event für eine Gebühr von 250$ eingetragen. Die Anzeigedauer beträgt 7 Tage.");
                RefreshTabletData(player, false);
                foreach (IPlayer client in Alt.GetAllPlayers().ToList())
                {
                    if (client == null || !client.Exists) continue;
                    HUDHandler.SendNotification(client, 1, 5000, "Ein neues Event wurde eingetragen, checke die Events-App im Tablet!");
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [AsyncClientEvent("Server:Tablet:VehicleStoreBuyVehicle")]
        public async Task VehicleStoreBuyVehicle(IPlayer player, string hash, int shopId, string color)
        {
            try
            {
                if (player == null || !player.Exists || hash == "" || hash == "undefined" || shopId <= 0 || color == "" || color == "undefined") return;
                ulong fHash = Convert.ToUInt64(hash);
                int charId = User.GetPlayerOnline(player);
                if (charId == 0 || fHash == 0) return;
                int Price = ServerVehicleShops.GetVehicleShopPrice(shopId, fHash);
                int rnd = new Random().Next(100000, 999999); 
                int color_R = 0,
                     color_G = 0,
                     color_B = 0;

                if (ServerVehicles.ExistServerVehiclePlate($"LS{rnd}")) { VehicleStoreBuyVehicle(player, hash, shopId, color); return; }
                if (!CharactersBank.HasCharacterBankMainKonto(charId)) { HUDHandler.SendNotification(player, 4, 5000, "Du hast noch kein Hauptkonto in deiner Bankfiliale festgelegt."); return; }
                int bankAccountNumber = CharactersBank.GetCharacterBankMainKonto(charId);
                if (CharactersBank.GetBankAccountLockStatus(bankAccountNumber)) { HUDHandler.SendNotification(player, 3, 5000, $"Dieses Bankkonto ist gesperrt und kann nicht weiter benutzt werden."); return; }
                if (CharactersBank.GetBankAccountMoney(bankAccountNumber) < Price) { HUDHandler.SendNotification(player, 3, 5000, $"Du hast nicht genügend Geld auf deinem Konto ({CharactersBank.GetBankAccountMoney(bankAccountNumber)}$ / {Price}$)."); return; }
                switch (color)
                {
                    case "schwarz": color_R = 0; color_G = 0; color_B = 0; break;
                    case "weiß": color_R = 255; color_G = 255; color_B = 255; break;
                    case "blau": color_R = 0; color_G = 0; color_B = 255; break;
                    case "rot": color_R = 255; color_G = 0; color_B = 0; break;
                    case "grün": color_R = 0; color_G = 255; color_B = 0; break;
                    case "gelb": color_R = 255; color_G = 255; color_B = 0; break;

                }

                DateTime dateTime = DateTime.Now;
                CharactersBank.SetBankAccountMoney(bankAccountNumber, (CharactersBank.GetBankAccountMoney(bankAccountNumber) - Price));
                ServerBankPapers.CreateNewBankPaper(bankAccountNumber, dateTime.ToString("dd.MM.yyyy"), dateTime.ToString("HH.mm"), "Ausgehende Überweisung", "Online Fahrzeugshop", $"Fahrzeugkauf: {ServerVehicles.GetVehicleNameOnHash(fHash)}", $"-{Price}", "Online Banking");
                ServerVehicles.CreateVehicle(fHash, charId, 0, 0, true, 1, new Position(0, 0, 0), new Rotation(0, 0, 0), $"NL{rnd}", color_R, color_G, color_B);
                CharactersInventory.AddCharacterItem(charId, $"Fahrzeugschluessel LS{rnd}", 2, "schluessel");
                HUDHandler.SendNotification(player, 2, 5000, $"Fahrzeug '{ServerVehicles.GetVehicleNameOnHash(fHash)}' erfolgreich für {Price}$ erworben.<br>Lieferort: La Mesa Fahrzeuggarage.");
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [AsyncClientEvent("Server:Tablet:NotesAppNewNote")]
        public async Task NotesAppNewNote(IPlayer player, string title, string text, string color)
        {
            try
            {
                if (player == null || !player.Exists || title == "" || text == "" || color == "") return;
                int charId = User.GetPlayerOnline(player);
                if (charId == 0) return;
                CharactersTablet.CreateServerTabletNote(charId, title, text, color);
                HUDHandler.SendNotification(player, 2, 5000, "Neue Notiz erfolgreich angelegt.");
                RefreshTabletData(player, false);
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [AsyncClientEvent("Server:Tablet:NotesAppDeleteNote")]
        public async Task NotesAppDeleteNote(IPlayer player, int noteId)
        {
            try
            {
                if (player == null || !player.Exists || noteId == 0) return;
                int charId = User.GetPlayerOnline(player);
                if (charId == 0) return;
                CharactersTablet.DeleteServerTabletNote(charId, noteId);
                RefreshTabletData(player, false);
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [AsyncClientEvent("Server:Tablet:DeleteFactionDispatch")]
        public async Task DeleteFactionDispatch(IPlayer player, int factionId, int senderId)
        {
            try
            {
                if (player == null || !player.Exists || factionId <= 0 || senderId <= 0) return;
                int charId = (int)player.GetCharacterMetaId();
                if (charId <= 0) return;
                if (!ServerFactions.ExistDispatch(factionId, senderId)) return;
                ServerFactions.RemoveDispatch(factionId, senderId);
                HUDHandler.SendNotification(player, 2, 2500, "Dispatch erfolgreich entfernt.");
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [AsyncClientEvent("Server:Tablet:CompanyAppInviteNewMember")]
        public async Task CompanyAppInviteNewMember(IPlayer player, string targetCharName, int companyId)
        {
            try
            {
                if (targetCharName == "" || targetCharName == "undefined" || companyId <= 0 || !player.Exists || player == null) return;
                int charId = User.GetPlayerOnline(player);
                if (charId == 0) return;
                if (ServerCompanys.GetCharacterServerCompanyId(charId) != companyId) { HUDHandler.SendNotification(player, 4, 5000, "Ein unerwarteter Fehler ist aufgetreten [COMPANY-INVITE-001]"); return; }
                if (ServerCompanys.GetCharacterServerCompanyRank(charId) < 1) { HUDHandler.SendNotification(player, 4, 5000, "Du hast keine Berechtigung dafür."); return; }
                if (!Characters.ExistCharacterName(targetCharName)) { HUDHandler.SendNotification(player, 3, 5000, $"Der eingegebene Name wurde nicht gefunden ({targetCharName})."); return; }
                int targetCharId = Characters.GetCharacterIdFromCharName(targetCharName);
                if (targetCharId <= 0) { HUDHandler.SendNotification(player, 4, 5000, "Ein unerwarteter Fehler ist aufgetreten [COMPANY-INVITE-002]"); return; }
                if (ServerCompanys.IsCharacterInAnyServerCompany(targetCharId)) { HUDHandler.SendNotification(player, 3, 5000, $"Die angegebene Person ist bereits in einem Unternehmen."); return; }
                var targetPlayer = Alt.GetAllPlayers().ToList().FirstOrDefault(x => x.GetCharacterMetaId() == (ulong)targetCharId);
                if (targetPlayer == null || !targetPlayer.Exists) { HUDHandler.SendNotification(player, 4, 5000, "Der angegebene Spieler ist nicht in deiner Nähe oder ihr seid nicht an dem Verwaltungspunkt des Unternehmens. [FEHLER: COMPANY-INVITE-003]"); return; }
                if (!player.Position.IsInRange(ServerCompanys.GetServerCompanyPosition(companyId), 5f) || !targetPlayer.Position.IsInRange(ServerCompanys.GetServerCompanyPosition(companyId), 5f)) { HUDHandler.SendNotification(player, 3, 5000, "Der angegebene Spieler ist nicht in deiner Nähe oder ihr seid nicht an dem Verwaltungspunkt des Unternehmens. [FEHLER: COMPANY-INVITE-004]"); return; }
                ServerCompanys.CreateServerCompanyMember(companyId, targetCharId, 0);
                string licName = ServerCompanys.GetServerCompanyGivenLicense(companyId);
                if (licName != "None" && !Characters.HasCharacterPermission(targetCharId, licName)) { Characters.AddCharacterPermission(targetCharId, licName); }
                HUDHandler.SendNotification(player, 2, 8000, $"Du hast die Person '{targetCharName}' erfolgreich als Mitarbeiter (Rang 0) eingestellt. Dieser hat nun Zugriff auf alle Funktionen des Unternehmens (bspw. exklusive Shops).");
                HUDHandler.SendNotification(targetPlayer, 1, 5000, $"Du wurdest erfolgreich als Mitarbeiter in das Unternehmen '{ServerCompanys.GetServerCompanyName(companyId)}' eingestellt.");
                RefreshTabletData(player, false);
                LoggingService.NewCompanyLog(companyId, charId, targetCharId, "invite", $"{Characters.GetCharacterName(charId)} ({charId}) hat {Characters.GetCharacterName(targetCharId)} ({targetCharId}) in das Unternehmen eingestellt.");
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [AsyncClientEvent("Server:Tablet:FactionManagerAppInviteNewMember")]
        public async Task FactionManagerAppInviteNewMember(IPlayer player, string targetCharName, int dienstnummer, int factionId)
        {
            try
            {
                if (targetCharName == "" || targetCharName == "undefined" || factionId <= 0 || !player.Exists || player == null) return;
                int charId = User.GetPlayerOnline(player);
                if (charId == 0) return;
                if (ServerFactions.GetCharacterFactionId(charId) != factionId) { HUDHandler.SendNotification(player, 4, 5000, "Ein unerwarteter Fehler ist aufgetreten [FACTION-INVITE-001]"); return; }
                if (ServerFactions.GetCharacterFactionRank(charId) < ServerFactions.GetFactionMaxRankCount(factionId) - 2) { HUDHandler.SendNotification(player, 4, 5000, "Dazu hast du keine Berechtigung."); return; }
                if (!Characters.ExistCharacterName(targetCharName)) { HUDHandler.SendNotification(player, 3, 5000, $"Der eingegebene Name wurde nicht gefunden ({targetCharName})."); return; }
                int targetCharId = Characters.GetCharacterIdFromCharName(targetCharName);
                if (targetCharId <= 0) { HUDHandler.SendNotification(player, 4, 5000, "Ein unerwarteter Fehler ist aufgetreten [FACTION-INVITE-002]"); return; }
                if (ServerFactions.IsCharacterInAnyFaction(targetCharId)) { HUDHandler.SendNotification(player, 3, 5000, $"Die angegebene Person ist bereits in einer Fraktion."); return; }
                if (ServerFactions.ExistFactionServiceNumber(factionId, dienstnummer)) { HUDHandler.SendNotification(player, 3, 5000, "Die eingegebene Dienstnummer ist bereits vergeben."); return; }
                var targetPlayer = Alt.GetAllPlayers().ToList().FirstOrDefault(x => x.GetCharacterMetaId() == (ulong)targetCharId);
                if (targetPlayer == null || !targetPlayer.Exists) { HUDHandler.SendNotification(player, 4, 5000, "Der angegebene Spieler ist nicht in deiner Nähe oder ihr seid nicht an dem Verwaltungspunkt der Fraktion. [FEHLER: FACTION-INVITE-003]"); return; }
                if (!player.Position.IsInRange(targetPlayer.Position, 5f)) { HUDHandler.SendNotification(player, 3, 5000, "Der angegebene Spieler ist nicht in deiner Nähe oder ihr seid nicht an dem Verwaltungspunkt der Fraktion. [FEHLER: FACTION-INVITE-004]"); return; }
                ServerFactions.CreateServerFactionMember(factionId, targetCharId, 1, dienstnummer);
                Characters.SetCharacterJob(charId, ServerFactions.GetFactionShortName(factionId));
                HUDHandler.SendNotification(player, 2, 8000, $"Du hast die Person '{targetCharName}' erfolgreich als Mitarbeiter (Rang 1) eingestellt. Dieser hat nun Zugriff auf alle Funktionen der Fraktion.");
                HUDHandler.SendNotification(targetPlayer, 1, 5000, $"Du wurdest erfolgreich als Mitarbeiter (Rang 1) in die Fraktion '{ServerFactions.GetFactionFullName(factionId)}' eingestellt.");
                RefreshTabletData(player, false);
                LoggingService.NewFactionLog(factionId, charId, targetCharId, "invite", $"{Characters.GetCharacterName(charId)} ({charId}) hat {Characters.GetCharacterName(targetCharId)} ({targetCharId}) in die Fraktion eingestellt.");
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [AsyncClientEvent("Server:Tablet:CompanyAppLeaveCompany")]
        public async Task CompanyAppLeaveCompany(IPlayer player)
        {
            try
            {
                if (player == null || !player.Exists) return;
                int charId = User.GetPlayerOnline(player);
                if (charId == 0) return;
                if (!ServerCompanys.IsCharacterInAnyServerCompany(charId)) { HUDHandler.SendNotification(player, 4, 5000, "Du bist in keinem Unternehmen welches du verlassen kannst."); return; }
                if (ServerCompanys.GetCharacterServerCompanyRank(charId) > 1) { HUDHandler.SendNotification(player, 4, 5000, "Du kannst als Geschäftsführer dein Unternehmen nicht einfach verlassen, tue dies beim Gewerbeamt."); return; }
                int currentCompany = ServerCompanys.GetCharacterServerCompanyId(charId);
                if (currentCompany <= 0) { HUDHandler.SendNotification(player, 4, 5000, "Ein unerwarteter Fehler ist aufgetreten. [COMPANY-LEAVE-COMPANY-001]"); return; }
                string companyLicName = ServerCompanys.GetServerCompanyGivenLicense(currentCompany);
                if (companyLicName != "None" && Characters.HasCharacterPermission(charId, companyLicName)) { Characters.RemoveCharacterPermission(charId, companyLicName); }
                CharactersTablet.ChangeCharacterTabletAppInstallState(charId, "company", false);
                ServerCompanys.RemoveServerCompanyMember(currentCompany, charId);
                HUDHandler.SendNotification(player, 2, 5000, "Du hast das Unternehmen erfolgreich verlassen.");
                RefreshTabletData(player, false);
                LoggingService.NewCompanyLog(currentCompany, charId, 0, "leavecompany", $"{Characters.GetCharacterName(charId)} ({charId}) hat das Unternehmen verlassen.");
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [AsyncClientEvent("Server:Tablet:CompanyAppRankAction")]
        public async Task CompanyAppRankAction(IPlayer player, int rankId, int targetCharId)
        {
            try
            {
                if (player == null || !player.Exists || targetCharId <= 0) return;
                int charId = User.GetPlayerOnline(player);
                if (charId <= 0) return;
                if (!ServerCompanys.IsCharacterInAnyServerCompany(charId)) { HUDHandler.SendNotification(player, 4, 5000, "Du bist in keinem Unternehmen welches du verwalten kannst."); return; }
                int companyId = ServerCompanys.GetCharacterServerCompanyId(charId);
                if (companyId <= 0) { HUDHandler.SendNotification(player, 4, 5000, "Ein unerwarteter Fehler ist aufgetreten. [COMPANY-RANK-ACTION-001]"); return; }
                if (ServerCompanys.GetCharacterServerCompanyRank(charId) < 1) { HUDHandler.SendNotification(player, 4, 5000, "Dafür hast du keine Berechtigung."); return; }
                if (!ServerCompanys.IsCharacterInAnyServerCompany(targetCharId)) { HUDHandler.SendNotification(player, 4, 5000, "Ein unerwarteter Fehler ist aufgetreten. [COMPANY-RANK-ACTION-002]"); return; }
                if (ServerCompanys.GetCharacterServerCompanyRank(targetCharId) > 1) { HUDHandler.SendNotification(player, 3, 5000, "Diese Person kannst du nicht verwalten."); return; }
                if (ServerCompanys.GetCharacterServerCompanyRank(charId) == 1 && ServerCompanys.GetCharacterServerCompanyRank(targetCharId) >= 1) { HUDHandler.SendNotification(player, 3, 5000, "Dieser Person kannst du diesen Rang nicht setzen."); return; }
                if (ServerCompanys.GetCharacterServerCompanyRank(targetCharId) == rankId) { HUDHandler.SendNotification(player, 3, 5000, "Die Person besitzt diesen Rang bereits."); return; }
                if (ServerCompanys.GetCharacterServerCompanyRank(charId) == 1 && ServerCompanys.GetCharacterServerCompanyRank(targetCharId) == 0 && (rankId > 0 && rankId < 5)) { HUDHandler.SendNotification(player, 3, 5000, "Dieser Person kannst du diesen Rang nicht setzen."); return; }
                var targetPlayer = Alt.GetAllPlayers().ToList().FirstOrDefault(x => x.GetCharacterMetaId() == (ulong)targetCharId);
                if (rankId == 1337)
                {
                    //Entlasse Spieler aus Unternehmen.
                    string companyLicName = ServerCompanys.GetServerCompanyGivenLicense(companyId);
                    if (companyLicName != "None" && Characters.HasCharacterPermission(targetCharId, companyLicName)) { Characters.RemoveCharacterPermission(targetCharId, companyLicName); }
                    CharactersTablet.ChangeCharacterTabletAppInstallState(targetCharId, "company", false);
                    ServerCompanys.RemoveServerCompanyMember(companyId, targetCharId);
                    RefreshTabletData(player, false);
                    HUDHandler.SendNotification(player, 2, 5000, $"Du hast die Person {Characters.GetCharacterName(targetCharId)} aus deinem Unternehmen entlassen.");
                    LoggingService.NewCompanyLog(companyId, charId, targetCharId, "uninvite", $"{Characters.GetCharacterName(charId)} ({charId}) hat die Person {Characters.GetCharacterName(targetCharId)} ({targetCharId}) aus dem Unternehmen entlassen.");
                    if (targetPlayer != null && targetPlayer.Exists)
                    {
                        RefreshTabletData(targetPlayer, false);
                        HUDHandler.SendNotification(targetPlayer, 1, 5000, $"Du wurdest aus dem Unternehmen {ServerCompanys.GetServerCompanyName(companyId)} entlassen.");
                    }
                    return;
                }

                ServerCompanys.ChangeServerCompanyMemberRank(companyId, targetCharId, rankId);
                RefreshTabletData(player, false);
                HUDHandler.SendNotification(player, 2, 5000, $"Du hast der Person {Characters.GetCharacterName(targetCharId)} den Rang {ServerCompanys.GetServerCompanyRankName(rankId)} gesetzt.");
                LoggingService.NewCompanyLog(companyId, charId, targetCharId, "rankchange", $"{Characters.GetCharacterName(charId)} ({charId}) hat den Unternehmensrang von {Characters.GetCharacterName(targetCharId)} ({targetCharId}) auf Rang {rankId} ({ServerCompanys.GetServerCompanyRankName(rankId)}) geändert.");
                if (targetPlayer != null && targetPlayer.Exists)
                {
                    RefreshTabletData(targetPlayer, false);
                    HUDHandler.SendNotification(targetPlayer, 1, 5000, $"Im Unternehmen '{ServerCompanys.GetServerCompanyName(companyId)}' wurde dir der Rang {ServerCompanys.GetServerCompanyRankName(rankId)} gesetzt.");
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [AsyncClientEvent("Server:Tablet:FactionManagerRankAction")]
        public async Task FactionManagerRankAction(IPlayer player, string action, int targetCharId)
        {
            try
            {
                if (player == null || !player.Exists || targetCharId <= 0) return;
                if (action != "rankup" && action != "rankdown" && action != "remove") { HUDHandler.SendNotification(player, 3, 5000, "Ein unerwarteter Fehler ist aufgetreten. [FACTION-RANKACTION-001]"); return; }
                int charId = User.GetPlayerOnline(player);
                if (charId <= 0) return;
                if (charId == targetCharId) { HUDHandler.SendNotification(player, 3, 5000, "Du kannst dich nicht selbst verwalten."); return; }
                if (!ServerFactions.IsCharacterInAnyFaction(charId)) { HUDHandler.SendNotification(player, 4, 5000, "Du bist in keiner Fraktion um dies zu tun."); return; }
                if (!ServerFactions.IsCharacterInAnyFaction(targetCharId)) { HUDHandler.SendNotification(player, 4, 5000, "Die ausgewählte Person ist in keiner Fraktion dessen Rang du verwalten kannst."); return; }
                if (ServerFactions.GetCharacterFactionId(charId) != ServerFactions.GetCharacterFactionId(targetCharId)) { HUDHandler.SendNotification(player, 4, 5000, "Die ausgewählte Person ist in keiner Fraktion dessen Rang du verwalten kannst."); return; }
                if (ServerFactions.GetCharacterFactionRank(charId) < ServerFactions.GetFactionMaxRankCount(ServerFactions.GetCharacterFactionId(charId)) - 2) { HUDHandler.SendNotification(player, 3, 5000, "Du bist nicht dazu berechtigt."); return; }
                var targetPlayer = Alt.GetAllPlayers().ToList().FirstOrDefault(x => x.GetCharacterMetaId() == (ulong)targetCharId);
                int factionId = ServerFactions.GetCharacterFactionId(charId);
                int currentTargetRank = ServerFactions.GetCharacterFactionRank(targetCharId);
                int currentActorRank = ServerFactions.GetCharacterFactionRank(charId);
                if (factionId <= 0) return;
                if (action == "remove")
                {
                    if (currentActorRank <= currentTargetRank) { HUDHandler.SendNotification(player, 3, 5000, "Dazu hast du keine Berechtigung."); return; }
                    ServerFactions.RemoveServerFactionMember(factionId, targetCharId);
                    HUDHandler.SendNotification(player, 2, 5000, $"Du hast die Person {Characters.GetCharacterName(targetCharId)} erfolgreich aus der Fraktion {ServerFactions.GetFactionFullName(factionId)} entlassen.");
                    RefreshTabletData(player, false);
                    if (targetPlayer == null || !targetPlayer.Exists) return;
                    HUDHandler.SendNotification(targetPlayer, 1, 5000, $"Du wurdest aus der Fraktion {ServerFactions.GetFactionFullName(factionId)} entlassen.");
                    LoggingService.NewFactionLog(factionId, charId, targetCharId, "uninvite", $"{Characters.GetCharacterName(charId)} ({charId}) hat {Characters.GetCharacterName(targetCharId)} ({targetCharId}) aus der Fraktion entlassen.");
                    return;
                }
                else if (action == "rankup")
                {
                    if (currentTargetRank + 1 >= ServerFactions.GetFactionMaxRankCount(factionId)) { HUDHandler.SendNotification(player, 3, 5000, "Auf den Rang der Fraktionsleiter kann nur das Bürgeramt befördern."); return; }
                    if (currentTargetRank + 1 >= currentActorRank) { HUDHandler.SendNotification(player, 3, 5000, "Diese Person kannst du nicht weiter befördern"); return; }
                    if (currentTargetRank == ServerFactions.GetFactionMaxRankCount(factionId)) { HUDHandler.SendNotification(player, 3, 5000, "Diesen Rang gibt es nicht."); return; }
                    if (currentActorRank <= currentTargetRank) { HUDHandler.SendNotification(player, 3, 5000, "Diese Person kannst du nicht verwalten."); return; }
                    if (currentActorRank == ServerFactions.GetFactionMaxRankCount(factionId) - 1 && currentTargetRank >= ServerFactions.GetFactionMaxRankCount(factionId) - 1) { HUDHandler.SendNotification(player, 3, 5000, "Diese Person kannst du nicht verwalten."); return; }
                    if (currentActorRank == ServerFactions.GetFactionMaxRankCount(factionId) - 2 && currentTargetRank >= ServerFactions.GetFactionMaxRankCount(factionId) - 2) { HUDHandler.SendNotification(player, 3, 5000, "Diese Person kannst du nicht verwalten."); return; }
                    ServerFactions.SetCharacterFactionRank(targetCharId, currentTargetRank + 1);
                    HUDHandler.SendNotification(player, 2, 5000, $"Sie haben den Rang von der Person {Characters.GetCharacterName(targetCharId)} von {currentTargetRank} auf {currentTargetRank + 1} geändert.");
                    RefreshTabletData(player, false);
                    if (targetPlayer == null || !targetPlayer.Exists) return;
                    HUDHandler.SendNotification(targetPlayer, 1, 5000, $"Ihr Rang in der Fraktion {ServerFactions.GetFactionFullName(factionId)} wurde von {currentTargetRank} auf {currentTargetRank + 1} geändert.");
                    LoggingService.NewFactionLog(factionId, charId, targetCharId, "rankchange", $"{Characters.GetCharacterName(charId)} ({charId}) hat den Rang von {Characters.GetCharacterName(targetCharId)} von Rang {currentTargetRank} auf Rang {currentTargetRank + 1} geändert.");
                    return;
                }
                else if (action == "rankdown")
                {
                    if (currentTargetRank - 1 <= 0) { HUDHandler.SendNotification(player, 4, 5000, "Weiter runter geht es nicht."); return; }
                    if (currentActorRank <= currentTargetRank) { HUDHandler.SendNotification(player, 3, 5000, "Diese Person kannst du nicht verwalten."); return; }
                    if (currentActorRank == ServerFactions.GetFactionMaxRankCount(factionId) - 1 && currentTargetRank >= ServerFactions.GetFactionMaxRankCount(factionId) - 1) { HUDHandler.SendNotification(player, 3, 5000, "Diese Person kannst du nicht verwalten."); return; }
                    if (currentActorRank == ServerFactions.GetFactionMaxRankCount(factionId) - 2 && currentTargetRank >= ServerFactions.GetFactionMaxRankCount(factionId) - 2) { HUDHandler.SendNotification(player, 3, 5000, "Diese Person kannst du nicht verwalten."); return; }
                    ServerFactions.SetCharacterFactionRank(targetCharId, currentTargetRank - 1);
                    HUDHandler.SendNotification(player, 2, 5000, $"Sie haben den Rang von der Person {Characters.GetCharacterName(targetCharId)} von {currentTargetRank} auf {currentTargetRank - 1} geändert.");
                    RefreshTabletData(player, false);
                    if (targetPlayer == null || !targetPlayer.Exists) return;
                    HUDHandler.SendNotification(targetPlayer, 1, 5000, $"Ihr Rang in der Fraktion {ServerFactions.GetFactionFullName(factionId)} wurde von {currentTargetRank} auf {currentTargetRank - 1} geändert.");
                    LoggingService.NewFactionLog(factionId, charId, targetCharId, "rankchange", $"{Characters.GetCharacterName(charId)} ({charId}) hat den Rang von {Characters.GetCharacterName(targetCharId)} von Rang {currentTargetRank} auf Rang {currentTargetRank - 1} geändert.");
                    return;
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [AsyncClientEvent("Server:Tablet:FactionManagerSetRankPaycheck")]
        public async Task FactionManagerSetRankPaycheck(IPlayer player, int rankId, int paycheck)
        {
            try
            {
                if (player == null || !player.Exists || rankId <= 0 || paycheck <= 0) return;
                if (paycheck > 20000) { HUDHandler.SendNotification(player, 3, 5000, "Fehler: Das Gehalt kann nur maximal auf 20.000$ gesetzt werden."); return; }
                int charId = User.GetPlayerOnline(player);
                if (charId <= 0) return;
                if (!ServerFactions.IsCharacterInAnyFaction(charId)) { HUDHandler.SendNotification(player, 4, 5000, "Du bist in keiner Fraktion dessen Gehalt du festlegen kannst."); return; }
                int factionId = ServerFactions.GetCharacterFactionId(charId);
                if (factionId <= 0) return;
                int currentPaycheck = ServerFactions.GetFactionRankPaycheck(factionId, rankId);
                if (currentPaycheck == paycheck) { return; }
                int charRank = ServerFactions.GetCharacterFactionRank(charId);
                if (charRank <= 0) return;
                if (charRank < ServerFactions.GetFactionMaxRankCount(factionId) - 2) { HUDHandler.SendNotification(player, 4, 5000, "Dazu hast du keine Berechtigungen."); return; }
                if (!ServerFactions.ExistServerFactionRankOnId(factionId, rankId)) { HUDHandler.SendNotification(player, 4, 5000, "Ein unerwarteter Fehler ist aufgetreten. [FACTION-RANKPAYCHECK-001]"); return; }
                ServerFactions.SetFactionRankPaycheck(factionId, rankId, paycheck);
                HUDHandler.SendNotification(player, 2, 5000, $"Du hast das Gehalt des Ranges '{ServerFactions.GetFactionRankName(factionId, rankId)}' der Fraktion '{ServerFactions.GetFactionFullName(factionId)}' von {currentPaycheck}$ auf {paycheck}$ gesetzt.");
                RefreshTabletData(player, false);
                LoggingService.NewFactionLog(factionId, charId, 0, "rankpaycheck", $"{Characters.GetCharacterName(charId)} ({charId}) hat das Gehalt des Ranges '{ServerFactions.GetFactionRankName(factionId, rankId)}' von {currentPaycheck}$ auf {paycheck}$ gesetzt.");
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [AsyncClientEvent("Server:Tablet:sendDispatchToFaction")]
        public async Task sendDispatchToFaction(IPlayer player, int factionId, string msg)
        {
            try
            {
                if (player == null || !player.Exists || factionId <= 0 || msg.Length <= 0) return;
                int charId = (int)player.GetCharacterMetaId();
                if (charId <= 0) return;
                if (Characters.IsCharacterUnconscious(charId)) return;
                if (!CharactersInventory.ExistCharacterItem(charId, "Tablet", "inventory") && !CharactersInventory.ExistCharacterItem(charId, "Tablet", "backpack")) return;
                if (player.HasPlayerHandcuffs() || player.HasPlayerRopeCuffs()) { HUDHandler.SendNotification(player, 4, 3500, "Fehler: Du bist gefesselt."); return; }
                if (factionId == 6) { HUDHandler.SendNotification(player, 4, 3500, "Fehler: Fahrschule nicht existent [LSF-6]"); return; }
/*                if (ServerFactions.ExistDispatchBySender(charId)) { HUDHandler.SendNotification(player, 3, 2500, "Du hast bereits einen Dispatch offen."); return; }
*/                ServerFactions.AddNewFactionDispatch(charId, factionId, msg, player.Position);
                HUDHandler.SendNotification(player, 2, 2500, "Notruf erfolgreich gesendet.");
                foreach (var p in Alt.GetAllPlayers().Where(x => x != null && x.Exists && x.GetCharacterMetaId() > 0).ToList())
                {
                    if (p == null || !p.Exists) continue;
                    if (!ServerFactions.IsCharacterInAnyFaction((int)p.GetCharacterMetaId()) || !ServerFactions.IsCharacterInFactionDuty((int)p.GetCharacterMetaId()) || ServerFactions.GetCharacterFactionId((int)p.GetCharacterMetaId()) != factionId) continue;
                    p.EmitLocked("Client:Tablet:sendDispatchSound", "../utils/sounds/dispatch.mp3");
                    p.Emit("Client:Tablet:sendDispatchSound", "../utils/sounds/dispatch.mp3");
                    HUDHandler.SendNotification(p, 1, 9500, "Ein neuer Notruf ist eingegangen.");
                    if (factionId == 6)
                    {
                        ServerFactions.RemoveDispatch(6, charId);
                    }
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }
    }
}
