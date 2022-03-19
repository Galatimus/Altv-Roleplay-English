using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using Altv_Roleplay.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using Altv_Roleplay.Utils;
using Altv_Roleplay.Factories;
using System.Linq;
using System.Globalization;
using Altv_Roleplay.Services;
using AltV.Net.Elements.Refs;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using AltV.Net.Async;
using Altv_Roleplay.Database;

namespace Altv_Roleplay.Handler
{
    class TimerHandler
    {
        public static async void OnCheckTimer(object sender, ElapsedEventArgs e)
        {
            try
            {
                Console.WriteLine($"Timer - Thread = {Thread.CurrentThread.ManagedThreadId}");
                Stopwatch stopwatch = new Stopwatch();

                //CheckIfUserIsBanned
                stopwatch.Start();
                foreach (IPlayer player in Alt.GetAllPlayers().ToList())
                {
                    if (player == null) continue;
                    using (var playerReference = new PlayerRef(player))
                    {
                        if (!playerReference.Exists) return;
                        if (player == null || !player.Exists) continue;
                        lock (player)
                        {
                            if (player == null || !player.Exists) continue;
                            if (player.Dimension != 10000 && Characters.GetCharacterAccountId((int)player.GetCharacterMetaId()) == 0) player.kickWithMessage("Fehler #1339 erkannt");
                            if (player.Dimension == 0) { if (User.GetPlayerOnline(player) <= 0 || User.GetPlayerSocialclubIdbyAccId(User.GetPlayerAccountId(player)) != player.SocialClubId || User.GetPlayerHardwareIdbyAccId(User.GetPlayerAccountId(player)) != player.HardwareIdHash) player.kickWithMessage("Fehler #1338 erkannt"); }
                        }
                    }
                }
                stopwatch.Stop();
                Alt.Log($"OnCheckTimer: Player Foreach benötigte: {stopwatch.Elapsed}");

            }
            catch (Exception ex)
            {
                Alt.Log($"{ex}");
            }
        }
        
        public static async void OnFuelTimer(object sender, ElapsedEventArgs e)
        {
            try
            {
                //Console.WriteLine($"Timer - Thread = {Thread.CurrentThread.ManagedThreadId}");
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                foreach (IVehicle Veh in Alt.GetAllVehicles().ToList())
                {
                    if (Veh == null || !Veh.Exists) { continue; }
                    using (var vRef = new VehicleRef(Veh))
                    {
                        if (!vRef.Exists) continue;
                        lock (Veh)
                        {
                            if (Veh == null || !Veh.Exists) continue;
                            if (Veh.EngineOn == true) { ServerVehicles.SetVehicleFuel(Veh, ServerVehicles.GetVehicleFuel(Veh) - 0.125f); }
                            ServerVehicles.SaveVehiclePositionAndStates(Veh);
                            ServerVehicles.SaveVehiclePositionAndStates(Veh);
                            ServerVehicles.SaveVehiclePositionAndStates(Veh);
                            ServerVehicles.SaveVehiclePositionAndStates(Veh);
                            if (Veh.BodyHealth < 150)
                            {
                                ServerVehicles.SetVehicleEngineHealthy(Veh, false);
                                ServerVehicles.SetVehicleEngineState(Veh, false);
                                ServerVehicles.SaveVehiclePositionAndStates(Veh);
                                ServerVehicles.SetVehicleKM(Veh, ServerVehicles.GetVehicleKM(Veh));
                            }
                            ServerVehicles.SetVehicleKM(Veh, ServerVehicles.GetVehicleKM(Veh));
                            ServerVehicles.SetVehicleKM(Veh, ServerVehicles.GetVehicleKM(Veh));
                            ServerVehicles.SetVehicleKM(Veh, ServerVehicles.GetVehicleKM(Veh));
                            ServerVehicles.SetVehicleKM(Veh, ServerVehicles.GetVehicleKM(Veh));
                        }
                    }
                }

                stopwatch.Stop();
                //Alt.Log($"OnFuelTimer: VehicleFUEL AMK Foreach benötigte: {stopwatch.Elapsed}");

                stopwatch.Reset();

                stopwatch.Start();
                foreach (IPlayer player in Alt.GetAllPlayers().ToList())
                {
                    if (player == null) continue;
                    using (var playerReference = new PlayerRef(player))
                    {
                        if (!playerReference.Exists) return;
                        if (player == null || !player.Exists) continue;
                        lock (player)
                        {
                            if (player == null || !player.Exists) continue;
                            int charId2 = User.GetPlayerOnline(player);
                            if (charId2 != 0)
                            {
                                if (player.IsInVehicle)
                                {
                                    player.EmitLocked("Client:HUD:GetDistanceForVehicleKM"); 
                                    HUDHandler.SendInformationToVehicleHUD(player);
                                    Alt.Log("Vehicle KM-State-HUD updated!");
                                }
                            }

                        }
                        int charId = User.GetPlayerOnline(player);
                        player.EmitLocked("Client:HUD:UpdateDesire", Characters.GetCharacterArmor(charId), Characters.GetCharacterHealth(charId), Characters.GetCharacterHunger(charId), Characters.GetCharacterThirst(charId)); //HUD updaten
                    }
                }
                stopwatch.Stop();
            }
            catch (Exception ex)
            {
                Alt.Log($"{ex}");
            }
        }
        public static async void OnKilometerTimer(object sender, ElapsedEventArgs e)
        {
            try
            {
                //Console.WriteLine($"Timer - Thread = {Thread.CurrentThread.ManagedThreadId}");
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                foreach (IVehicle Veh in Alt.GetAllVehicles().ToList())
                {
                    if (Veh == null || !Veh.Exists) { continue; }
                    using (var vRef = new VehicleRef(Veh))
                    {
                        if (!vRef.Exists) continue;
                        lock (Veh)
                        {
                            if (Veh == null || !Veh.Exists) continue;
                            ServerVehicles.SaveVehiclePositionAndStates(Veh);
                            Alt.Log("Vehicle KM-State saved!");
                        }
                    }

                }

                stopwatch.Stop();
                //Alt.Log($"OnKilometerTimer: Vehicle Foreach benötigte: {stopwatch.Elapsed}");

                stopwatch.Reset();
                stopwatch.Start();
                foreach (IPlayer player in Alt.GetAllPlayers().ToList())
                {
                    if (player == null) continue;
                    using (var playerReference = new PlayerRef(player))
                    {
                        if (!playerReference.Exists) return;
                        if (player == null || !player.Exists) continue;
                        lock (player)
                        {
                            if (player == null || !player.Exists) continue;
                            int charId2 = User.GetPlayerOnline(player);
                            if (charId2 != 0)
                            {
                                if (player.IsInVehicle)
                                {
                                    player.EmitLocked("Client:HUD:GetDistanceForVehicleKM"); HUDHandler.SendInformationToVehicleHUD(player);
                                    Alt.Log("Vehicle KM-State-HUD updated!");
                                }
                            }

                        }
                        int charId = User.GetPlayerOnline(player);
                        player.EmitLocked("Client:HUD:UpdateDesire", Characters.GetCharacterArmor(charId), Characters.GetCharacterHealth(charId), Characters.GetCharacterHunger(charId), Characters.GetCharacterThirst(charId)); //HUD updaten
                    }
                }
                stopwatch.Stop();
                //Alt.Log($"OnEntityTimer: KILOMETER AMK Foreach benötigte: {stopwatch.Elapsed}");
            }
            catch (Exception ex)
            {
                Alt.Log($"{ex}");
            }
        }
        public static async void OnHUDTimer(object sender, ElapsedEventArgs e)
        {
            try
            {
                //Console.WriteLine($"Timer - Thread = {Thread.CurrentThread.ManagedThreadId}");
                Stopwatch stopwatch = new Stopwatch();

                stopwatch.Start();
                foreach (IPlayer player in Alt.GetAllPlayers().ToList())
                {
                    if (player == null) continue;
                    using (var playerReference = new PlayerRef(player))
                    {
                        if (!playerReference.Exists) return;
                        if (player == null || !player.Exists) continue;
                        lock (player)
                        {
                            if (player == null || !player.Exists) continue;
                            int charId2 = User.GetPlayerOnline(player);
                            if (charId2 > 0)
                            {
                                Characters.SetCharacterHealth(charId2, player.Health);
                                Characters.SetCharacterArmor(charId2, player.Armor);
                                //player.CurrentAmmo
                            }

                        }
                        int charId = User.GetPlayerOnline(player);
                        player.EmitLocked("Client:HUD:UpdateDesire", Characters.GetCharacterArmor(charId), Characters.GetCharacterHealth(charId), Characters.GetCharacterHunger(charId), Characters.GetCharacterThirst(charId)); //HUD updaten
                        player.EmitLocked("Client:HUD:updateMoney", CharactersInventory.GetCharacterItemAmount(User.GetPlayerOnline(player), "Bargeld", "inventory"));
                    }
                }
                stopwatch.Stop();
                //Alt.Log($"OnEntityTimer: KILOMETER AMK Foreach benötigte: {stopwatch.Elapsed}");
            }
            catch (Exception ex)
            {
                Alt.Log($"{ex}");
            }
        }
        public static async void OnEntityTimer(object sender, ElapsedEventArgs e)
        {
            try
            {
                Console.WriteLine($"Timer - Thread = {Thread.CurrentThread.ManagedThreadId}");
                Stopwatch stopwatch = new Stopwatch();
                /*stopwatch.Start();
                foreach (IVehicle Veh in Alt.GetAllVehicles().ToList())
                {
                    if (Veh == null || !Veh.Exists) { continue; }
                    using (var vRef = new VehicleRef(Veh))
                    {
                        if (!vRef.Exists) continue;
                        lock (Veh)
                        {
                            if (Veh == null || !Veh.Exists) continue;
                            ulong vehID = Veh.GetVehicleId();
                            if (vehID <= 0) { continue; }
                            ServerVehicles.SaveVehiclePositionAndStates(Veh);
*//*                            if (Veh.EngineOn == true) { ServerVehicles.SetVehicleFuel(Veh, ServerVehicles.GetVehicleFuel(Veh) - 0.01f); }
*//*                        }
                    }
                }

                stopwatch.Stop();
                Alt.Log($"OnEntityTimer: Vehicle Foreach benötigte: {stopwatch.Elapsed}");

                stopwatch.Reset();*/
                stopwatch.Start();
                foreach (IPlayer player in Alt.GetAllPlayers().ToList())
                {
                    if (player == null) continue;
                    using (var playerReference = new PlayerRef(player))
                    {
                        if (!playerReference.Exists) return;
                        if (player == null || !player.Exists) continue;
                        lock (player)
                        {
                            if (player == null || !player.Exists) continue;
                            int charId = User.GetPlayerOnline(player);
                            if (charId > 0)
                            {
                                Characters.SetCharacterLastPosition(charId, player.Position, player.Dimension);
                                if (User.IsPlayerBanned(player)) { player.kickWithMessage($"Du bist gebannt. (Grund: {User.GetPlayerBanReason(player)})."); }
                                Characters.SetCharacterHealth(charId, player.Health);
                                Characters.SetCharacterArmor(charId, player.Armor);
/*                                WeatherHandler.SetRealTime(player);
*//*                                if (player.IsInVehicle) { player.EmitLocked("Client:HUD:GetDistanceForVehicleKM"); HUDHandler.SendInformationToVehicleHUD(player); }
*/                                Characters.IncreaseCharacterPaydayTime(charId);

                                if (Characters.IsCharacterUnconscious(charId))
                                {
                                    int unconsciousTime = Characters.GetCharacterUnconsciousTime(charId);
                                    if (unconsciousTime > 0) { Characters.SetCharacterUnconscious(charId, true, unconsciousTime - 1); }
                                    else if (unconsciousTime <= 0)
                                    {
                                        Characters.SetCharacterUnconscious(charId, false, 0);
                                        DeathHandler.closeDeathscreen(player);
                                        player.Spawn(new Position(355.54285f, -596.33405f, 28.75768f));
                                        player.Health = player.MaxHealth;
                                    }
                                }

                                if (Characters.IsCharacterFastFarm(charId))
                                {
                                    int fastFarmTime = Characters.GetCharacterFastFarmTime(charId);
                                    if (fastFarmTime > 0) Characters.SetCharacterFastFarm(charId, true, fastFarmTime - 1);
                                    else if (fastFarmTime <= 0) Characters.SetCharacterFastFarm(charId, false, 0);
                                }

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


                                if (Characters.IsCharacterInJail(charId))
                                {
                                    int jailTime = Characters.GetCharacterJailTime(charId);
                                    if (jailTime > 0) Characters.SetCharacterJailTime(charId, true, jailTime - 1);
                                    else if (jailTime <= 0)
                                    {
                                        if (CharactersWanteds.HasCharacterWanteds(charId))
                                        {
                                            int jailTimes = CharactersWanteds.GetCharacterWantedFinalJailTime(charId);
                                            int jailPrice = CharactersWanteds.GetCharacterWantedFinalJailPrice(charId);
                                            if (CharactersBank.HasCharacterBankMainKonto(charId))
                                            {
                                                int accNumber = CharactersBank.GetCharacterBankMainKonto(charId);
                                                int bankMoney = CharactersBank.GetBankAccountMoney(accNumber);
                                                CharactersBank.SetBankAccountMoney(accNumber, bankMoney - jailPrice);
                                                HUDHandler.SendNotification(player, 1, 7500, $"Durch deine Inhaftierung wurden dir {jailPrice}$ vom Konto abgezogen.");
                                            }
                                            HUDHandler.SendNotification(player, 1, 7500, $"Du sitzt nun für {jailTimes} Minuten im Gefängnis.");
                                            Characters.SetCharacterJailTime(charId, true, jailTimes);
                                            CharactersWanteds.RemoveCharacterWanteds(charId);
                                            player.Position = new Position(1691.4594f, 2565.7056f, 45.556763f);
                                            if (Characters.GetCharacterGender(charId) == false)
                                            {
                                                player.EmitLocked("Client:SpawnArea:setCharClothes", 11, 5, 0);
                                                player.EmitLocked("Client:SpawnArea:setCharClothes", 3, 5, 0);
                                                player.EmitLocked("Client:SpawnArea:setCharClothes", 4, 7, 15);
                                                player.EmitLocked("Client:SpawnArea:setCharClothes", 6, 7, 0);
                                                player.EmitLocked("Client:SpawnArea:setCharClothes", 8, 1, 88);
                                            }
                                            else
                                            {

                                            }
                                        }
                                        else
                                        {
                                            Characters.SetCharacterJailTime(charId, false, 0);
                                            Characters.SetCharacterCorrectClothes(player);
                                            player.Position = new Position(1846.022f, 2585.8945f, 45.657f);
                                            HUDHandler.SendNotification(player, 1, 2500, "Du wurdest aus dem Gefängnis entlassen.");
                                        }
                                    }
                                }

                                if (Characters.GetCharacterPaydayTime(charId) >= 60)
                                {
                                    Characters.IncreaseCharacterPlayTimeHours(charId);
                                    Characters.ResetCharacterPaydayTime(charId);
                                    if (CharactersBank.HasCharacterBankMainKonto(charId))
                                    {
                                        int accountNumber = CharactersBank.GetCharacterBankMainKonto(charId);
                                        if (!ServerFactions.IsCharacterInAnyFaction(charId) || ServerFactions.GetCharacterFactionId(charId) == 0)
                                        {
                                            CharactersBank.SetBankAccountMoney(accountNumber, CharactersBank.GetBankAccountMoney(accountNumber) + 200); //250$ Stütze
                                            ServerBankPapers.CreateNewBankPaper(accountNumber, DateTime.Now.ToString("d", CultureInfo.CreateSpecificCulture("de-DE")), DateTime.Now.ToString("t", CultureInfo.CreateSpecificCulture("de-DE")), "Eingehende Überweisung", "Staat", "Arbeitslosengeld", "+200$", "Unbekannt");
                                        }

                                        if (!Characters.IsCharacterCrimeFlagged(charId) && Characters.GetCharacterJob(charId) != "None" && DateTime.Now.Subtract(Convert.ToDateTime(Characters.GetCharacterLastJobPaycheck(charId))).TotalHours >= 12 && !ServerFactions.IsCharacterInAnyFaction(charId))
                                        {
                                            if (Characters.GetCharacterJobHourCounter(charId) >= ServerJobs.GetJobNeededHours(Characters.GetCharacterJob(charId)) - 1)
                                            {
                                                int jobCheck = ServerJobs.GetJobPaycheck(Characters.GetCharacterJob(charId));
                                                Characters.SetCharacterLastJobPaycheck(charId, DateTime.Now);
                                                Characters.ResetCharacterJobHourCounter(charId);
                                                CharactersBank.SetBankAccountMoney(accountNumber, CharactersBank.GetBankAccountMoney(accountNumber) + jobCheck);
                                                ServerBankPapers.CreateNewBankPaper(accountNumber, DateTime.Now.ToString("d", CultureInfo.CreateSpecificCulture("de-DE")), DateTime.Now.ToString("t", CultureInfo.CreateSpecificCulture("de-DE")), "Eingehende Überweisung", "Arbeitsamt", $"Gehalt: {Characters.GetCharacterJob(charId)}", $"+{jobCheck}$", "Unbekannt");
                                                HUDHandler.SendNotification(player, 1, 5000, $"Gehalt erhalten (Beruf: {Characters.GetCharacterJob(charId)} | Gehalt: {jobCheck}$)");
                                            }
                                            else { Characters.IncreaseCharacterJobHourCounter(charId); }
                                        }

                                        if (ServerFactions.IsCharacterInAnyFaction(charId) && ServerFactions.IsCharacterInFactionDuty(charId))
                                        {
                                            int factionid = ServerFactions.GetCharacterFactionId(charId);
                                            int factionPayCheck = ServerFactions.GetFactionRankPaycheck(factionid, ServerFactions.GetCharacterFactionRank(charId));
                                            if (ServerFactions.GetFactionBankMoney(factionid) >= factionPayCheck)
                                            {
                                                ServerFactions.SetFactionBankMoney(factionid, ServerFactions.GetFactionBankMoney(factionid) - factionPayCheck);
                                                CharactersBank.SetBankAccountMoney(accountNumber, CharactersBank.GetBankAccountMoney(accountNumber) + factionPayCheck);
                                                HUDHandler.SendNotification(player, 1, 5000, $"Du hast deinen Lohn i.H.v. {factionPayCheck}$ erhalten ({ServerFactions.GetFactionRankName(factionid, ServerFactions.GetCharacterFactionRank(charId))})");
                                                ServerBankPapers.CreateNewBankPaper(accountNumber, DateTime.Now.ToString("d", CultureInfo.CreateSpecificCulture("de-DE")), DateTime.Now.ToString("t", CultureInfo.CreateSpecificCulture("de-DE")), "Eingehende Überweisung", $"{ServerFactions.GetFactionFullName(factionid)}", $"Gehalt: {ServerFactions.GetFactionRankName(factionid, ServerFactions.GetCharacterFactionRank(charId))}", $"+{factionPayCheck}$", "Dauerauftrag");
                                                LoggingService.NewFactionLog(factionid, charId, 0, "paycheck", $"{Characters.GetCharacterName(charId)} hat seinen Lohn i.H.v. {factionPayCheck}$ erhalten ({ServerFactions.GetFactionRankName(factionid, ServerFactions.GetCharacterFactionRank(charId))}).");
                                            }
                                            else
                                            {
                                                HUDHandler.SendNotification(player, 3, 5000, $"Deine Fraktion hat nicht genügend Geld um dich zu bezahlen ({factionPayCheck}$).");
                                            }
                                        }

                                        var playerVehicles = ServerVehicles.ServerVehicles_.Where(x => x.id > 0 && x.charid == charId && x.plate.Contains("NL"));
                                        int taxMoney = 0;
                                        foreach (var i in playerVehicles)
                                        {
                                            if (!i.plate.Contains("NL")) continue;
                                            taxMoney += ServerAllVehicles.GetVehicleTaxes(i.hash);
                                        }

                                        if (playerVehicles != null && taxMoney > 0)
                                        {
                                            if (CharactersBank.GetBankAccountMoney(accountNumber) < taxMoney) { HUDHandler.SendNotification(player, 3, 5000, $"Deine Fahrzeugsteuern konnten nicht abgebucht werden ({taxMoney}$)"); }
                                            else
                                            {
                                                CharactersBank.SetBankAccountMoney(accountNumber, CharactersBank.GetBankAccountMoney(accountNumber) - taxMoney);
                                                ServerBankPapers.CreateNewBankPaper(accountNumber, DateTime.Now.ToString("d", CultureInfo.CreateSpecificCulture("de-DE")), DateTime.Now.ToString("t", CultureInfo.CreateSpecificCulture("de-DE")), "Ausgehende Überweisung", "Zulassungsamt", $"Fahrzeugsteuer", $"-{taxMoney}$", "Bankeinzug");
                                                HUDHandler.SendNotification(player, 1, 5000, $"Du hast deine Fahrzeugsteuern i.H.v. {taxMoney}$ bezahlt.");
                                            }
                                        }
                                    }
                                    else { HUDHandler.SendNotification(player, 3, 5000, $"Dein Einkommen konnte nicht überwiesen werden da du kein Hauptkonto hast."); }
                                }
                            }
                        }
                    }
                }
                stopwatch.Stop();
                Alt.Log($"OnEntityTimer: Player Foreach benötigte: {stopwatch.Elapsed}");
            }
            catch (Exception ex)
            {
                Alt.Log($"{ex}");
            }
        }
        internal static void VehicleAutomaticParkFetch(object sender, ElapsedEventArgs e)
        {
            try
            {
                foreach (var hotelApartment in ServerHotels.ServerHotelsApartments_.Where(x => x.ownerId > 0))
                {
                    if (hotelApartment == null) continue;
                    if (DateTime.Now.Subtract(Convert.ToDateTime(hotelApartment.lastRent)).TotalHours >= hotelApartment.maxRentHours)
                    {
                        int oldOwnerId = hotelApartment.ownerId;
                        ServerHotels.SetApartmentOwner(hotelApartment.hotelId, hotelApartment.id, 0);
                        foreach (IPlayer players in Alt.GetAllPlayers().ToList().Where(x => x != null && x.Exists && User.GetPlayerOnline(x) == oldOwnerId))
                        {
                            HUDHandler.SendNotification(players, 1, 5000, "Deine Mietdauer im Hotel ist ausgelaufen, dein Zimmer wurde gekündigt");
                        }
                    }
                }
            }
            catch (Exception ex) { Alt.Log($"{ex}"); }
        }
        internal static void OnDesireTimer(object sender, ElapsedEventArgs e)
        {
            Alt.Log("OnDesireTimer Timer aufgerufen");
            foreach (IPlayer player in Alt.GetAllPlayers().ToList())
            {
                if (player == null || Characters.IsCharacterAnimal(((ClassicPlayer)player).CharacterId)) continue; using (var pRef = new PlayerRef(player))
                {
                    if (!pRef.Exists) return;
                    lock (player)
                    {
                        if (player.Exists && User.GetPlayerOnline(player) != 0)
                        {
                            int charId = User.GetPlayerOnline(player);
                            int random = new Random().Next(1, 1);
                            if (Characters.GetCharacterHunger(User.GetPlayerOnline(player)) > 0)
                            {
                                Characters.SetCharacterHunger(charId, (Characters.GetCharacterHunger(charId) - random));
                                if (Characters.GetCharacterHunger(charId) < 0) { Characters.SetCharacterHunger(charId, 0); }
                            }
                            else
                            {
                                player.Health = (ushort)(player.Health - 3);
                                Characters.SetCharacterHealth(charId, player.Health);
                                HUDHandler.SendNotification(player, 1, 5000, $"Du hast Hunger.");
                            }

                            if (Characters.GetCharacterHealth(User.GetPlayerOnline(player)) <= 15)
                            {
                                HUDHandler.SendNotification(player, 1, 5000, $"Su solltest einen Arzt besuchen!");
                            }

                            if (Characters.GetCharacterThirst(User.GetPlayerOnline(player)) > 0)
                            {
                                Characters.SetCharacterThirst(charId, (Characters.GetCharacterThirst(charId) - random));
                                if (Characters.GetCharacterThirst(charId) < 0) { Characters.SetCharacterThirst(charId, 0); }
                            }
                            else
                            {
                                player.Health = (ushort)(player.Health - 5);
                                Characters.SetCharacterHealth(charId, player.Health);
                                HUDHandler.SendNotification(player, 1, 5000, $"Du hast Durst.");
                            }


                            Alt.Log($"Essen/Durst Anzeige update: {Characters.GetCharacterArmor(charId)} | {Characters.GetCharacterHealth(charId)} | {Characters.GetCharacterHunger(charId)} | {Characters.GetCharacterThirst(charId)}");
                            player.EmitLocked("Client:HUD:UpdateDesire", Characters.GetCharacterArmor(charId), Characters.GetCharacterHealth(charId), Characters.GetCharacterHunger(charId), Characters.GetCharacterThirst(charId)); //HUD updaten
                            player.EmitLocked("Client:HUD:updateMoney", CharactersInventory.GetCharacterItemAmount(User.GetPlayerOnline(player), "Bargeld", "inventory"));

                        }
                    }
                }
            }
        }
        internal static void LaborTimer(Object sender, ElapsedEventArgs a)
        {
            try
            {
                Console.WriteLine("test");
                if (DateTime.Now.Minute != 0 && DateTime.Now.Minute != 15 && DateTime.Now.Minute != 30 && DateTime.Now.Minute != 45) return;
                Console.WriteLine("yes");
                foreach (var faction in ServerFactions.ServerFactions_.ToList())
                {
                    if (faction.id < 6 || faction.id == 12) continue;
                    foreach (var factionMember in ServerFactions.ServerFactionMembers_.ToList().Where(x => x.factionId == faction.id))
                    {
                        if (ServerFactions.GetLaborItemAmount(factionMember.factionId, factionMember.charId, "Batteriezellen") >= 10 && ServerFactions.GetLaborItemAmount(factionMember.factionId, factionMember.charId, "Hanfsamenpulver") >= 10 && ServerFactions.GetLaborItemAmount(factionMember.factionId, factionMember.charId, "Dünger") >= 10)
                        {
                            ServerFactions.RemoveLaborItemAmount(factionMember.factionId, factionMember.charId, "Batteriezellen", 10);
                            ServerFactions.RemoveLaborItemAmount(factionMember.factionId, factionMember.charId, "Hanfsamenpulver", 10);
                            ServerFactions.RemoveLaborItemAmount(factionMember.factionId, factionMember.charId, "Dünger", 10);
                            ServerFactions.AddLaborItem(factionMember.factionId, factionMember.charId, "Cannabiskiste", 1);
                        }

                        if (ServerFactions.GetLaborItemAmount(factionMember.factionId, factionMember.charId, "Batteriezellen") >= 10 && ServerFactions.GetLaborItemAmount(factionMember.factionId, factionMember.charId, "Ephedrinpulver") >= 10 && ServerFactions.GetLaborItemAmount(factionMember.factionId, factionMember.charId, "Toilettenreiniger") >= 10)
                        {
                            ServerFactions.RemoveLaborItemAmount(factionMember.factionId, factionMember.charId, "Batteriezellen", 10);
                            ServerFactions.RemoveLaborItemAmount(factionMember.factionId, factionMember.charId, "Ephedrinpulver", 10);
                            ServerFactions.RemoveLaborItemAmount(factionMember.factionId, factionMember.charId, "Toilettenreiniger", 10);
                            ServerFactions.AddLaborItem(factionMember.factionId, factionMember.charId, "Methkiste", 1);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
