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
using System;
using System.Collections.Generic;
using System.Numerics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altv_Roleplay.Database;

namespace Altv_Roleplay.Handler
{
    public class Commands : IScript
    {


        [Command("vehpos")]
        public void vehPos(IPlayer player)
        {
            if (player == null || !player.Exists || !player.IsInVehicle) return;
            if (player.AdminLevel() <= 8) { HUDHandler.SendNotification(player, 4, 5000, "Keine Rechte."); return; }
            HUDHandler.SendNotification(player, 4, 60000, $"{player.Vehicle.Position.ToString()}");
        }

        [Command("testveh")]
        public void CMD_testtest(IPlayer player)
        {
            VehicleHandler.testtesttest(player);
        }

        //Erstelle Lagerhalle
        [Command("createstorage")]
        public void CMD(IPlayer player, int weight, int price)
        {
            if (player == null || !player.Exists) return;
            models.Server_Storages st = new models.Server_Storages
            {
                entryPos = player.Position,
                items = new List<models.Server_Storage_Item>(),
                owner = 0,
                secondOwner = 0,
                maxSize = weight,
                price = price
            };

            Model.ServerStorages.ServerStorages_.Add(st);

            using (gtaContext gta = new gtaContext())
            {
                gta.Server_Storages.Add(st);
                gta.SaveChanges();
            }
            EntityStreamer.BlipStreamer.CreateStaticBlip("Lagerhalle", 0, 0.5f, true, 50, player.Position, 0);
            EntityStreamer.MarkerStreamer.Create(EntityStreamer.MarkerTypes.MarkerTypeVerticalCylinder, new Vector3(player.Position.X, player.Position.Y, player.Position.Z - 1), new System.Numerics.Vector3(1), color: new AltV.Net.Data.Rgba(180, 50, 50, 100), dimension: 0, streamRange: 100);
            HUDHandler.SendNotification(player, 4, 5000, "[LaVie Lagersystem] <br><br> Erfolgreich erstellt und eingerichtet");
        }
        

        [Command("vscp")]
        public void vehPrice(IPlayer player, string car, int price)
        {
            try
            {
                if (player.AdminLevel() <= 8) { HUDHandler.SendNotification(player, 4, 5000, "Keine Rechte."); return; }
                if (car == null) { HUDHandler.SendNotification(player, 4, 5000, "Fahrzeugname nicht angegeben!"); return; }
                IVehicle testVehicle = Alt.CreateVehicle(car, player.Position, player.Rotation);

                if (testVehicle == null)
                {
                    HUDHandler.SendNotification(player, 2, 5000, $"Falsches Fahrzeug.");
                }
                else
                {
                    testVehicle.Remove();
                    uint hashedCar = Alt.Hash(car);
                    HUDHandler.SendNotification(player, 2, 20000, $"Fahrzeugname: {car} || Fahrzeughash: {hashedCar} || Neuer Preis: {price}$");
                    ServerVehicleShops.SetVehiclePrice(hashedCar, price);
                }

            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        //OoC Messages
        [Command("ooc", true)]
        public void oocCMD(IPlayer player, string msg)
        {
            if (player == null || !player.Exists) return;
            if (msg == null) { HUDHandler.SendNotification(player, 4, 4000, "Wie wäre es mit einer Nachricht? (Fehlt)"); return; }

            foreach (var client in Alt.GetAllPlayers())
            {
                var name = Characters.GetCharacterName((int)player.GetCharacterMetaId());
                if (client == null || !client.Exists) continue;
                var range = 5; //Change OOC Range!!
                if (client.Position.Distance(player.Position) <= range)
                {
                    HUDHandler.SendNotification((IPlayer)client, 2, 5000, $"[{(int)player.GetCharacterMetaId()}] {name}: \n " + msg);
/*                    HelperMethods.send//("OOC", $"{player.Name} Nachricht: {msg}", "red");
*/                }
            }
        }

        [Command("LSPD", true)]
        public void LSPDCMD(IPlayer player, string msg)
        {
            if (player == null || !player.Exists) return;
            if (!ServerFactions.IsCharacterInAnyFaction((int)player.GetCharacterMetaId())) { HUDHandler.SendNotification(player, 4, 4000, "Du bist in keiner Fraktion"); return; }
            if (ServerFactions.GetCharacterFactionId((int)player.GetCharacterMetaId()) != 1) { HUDHandler.SendNotification(player, 4, 4000, "Du gehörst nicht zum LSPD"); return; }


            foreach (var client in Alt.GetAllPlayers())
            {
                if (client == null || !client.Exists) continue;
                HUDHandler.SendNotification((IPlayer)client, 4, 5000, "Das L.S.P.D informiert: \n" + msg);
            }
/*            HelperMethods.send//("LSPD USERMAIL", $"{player.accountName} hast eine LSPD Rundmail geschickt: {msg}", "red");
*/        }
        
        [Command("report", true)]
        public void REPORTCMD(IPlayer player, string msg)
        {
            if (player == null || !player.Exists) return;


            foreach (var client in Alt.GetAllPlayers().Where(x => x != null && x.Exists && x.AdminLevel() >= 1))
            {
                if (client == null || !client.Exists) continue;
                HUDHandler.SendNotification((IPlayer)client, 4, 5000, $"[SUPPORT] {Characters.GetCharacterName((int)player.GetCharacterMetaId())} (ID: {(int)player.GetCharacterMetaId()}) benötigt Support: {msg}");
                //.SendEmbed("Command", "Command",$" {Characters.GetCharacterName((int)player.GetCharacterMetaId())}  {msg}");
            }
/*            HelperMethods.send//("LSPD USERMAIL", $"{player.accountName} hast eine LSPD Rundmail geschickt: {msg}", "red");
*/        }

        [Command("id")]
        public static void CMD_ID(IPlayer player)
        {
            if (player == null) { HUDHandler.SendNotification(player, 4, 3000, "Fehler 002 >> player == null // Spieler nicht gefunden"); HUDHandler.SendNotification(player, 4, 3000, "Fehler 002 >> Sollte der Fehler bleiben, bitte melde das einem Admin!"); return; }
            HUDHandler.SendNotification(player, 2, 10000, $"Deine ID = {player.Id}");
        }

        [Command("license")]
        public void licenseCMD(IPlayer player, int charId, string licenseshort)
        {
            if (licenseshort == null || !player.Exists) return;
            if (player.AdminLevel() <= 8) { HUDHandler.SendNotification(player, 4, 5000, "Keine Rechte."); return; }
            if (!player.HasData("isAduty")) { HUDHandler.SendNotification(player, 4, 5000, "Nicht im (/am) Admindienst."); return; }
            if (charId <= 0) return;
            Characters.AddCharacterPermission(charId, licenseshort);
            HUDHandler.SendNotification(player, 2, 3500, $"Du hast die License {licenseshort} vergeben an: {charId}");
        }

        [Command("setAtm")]
        public void SetATM_CMD(IPlayer player, string name)
        {
            if (player == null || !player.Exists) return;
            if (player.AdminLevel() <= 8) { HUDHandler.SendNotification(player, 4, 5000, "Keine Rechte."); return; }
            if (!player.HasData("isAduty")) { HUDHandler.SendNotification(player, 4, 5000, "Nicht im (/am) Admindienst."); return; }
            ulong charId = player.GetCharacterMetaId();
            if (charId <= 0) return;
            ServerATM.CreateNewATM(player, 0, player.Position, name);
            HUDHandler.SendNotification(player, 2, 2500, $"ATM erfolgreich gesetzt: {name}");
        }

        [Command("money")]
        public void GiveItemCMD(IPlayer player, int itemAmount)
        {
            if (player == null || !player.Exists) return;
            if (player.AdminLevel() <= 8) { HUDHandler.SendNotification(player, 4, 5000, "Keine Rechte."); return; }
            if (!player.HasData("isAduty")) { HUDHandler.SendNotification(player, 4, 5000, "Nicht im (/am) Admindienst."); return; }
            ulong charId = player.GetCharacterMetaId();
            if (charId <= 0) return;
            CharactersInventory.AddCharacterItem((int)charId, "Bargeld", itemAmount, "inventory");
            HUDHandler.SendNotification(player, 2, 5000, $"{itemAmount}$ erhalten (Bargeld).");
        }

        [Command("ReloadDB")]
        public static void CMD_RELOAD(IPlayer player, int ID)
        {
            if (player == null || !player.Exists) return;
            if (player.AdminLevel() <= 8) { HUDHandler.SendNotification(player, 4, 5000, "Keine Rechte."); return; }
            if (!player.HasData("isAduty")) { HUDHandler.SendNotification(player, 4, 5000, "Nicht im (/am) Admindienst."); return; }
            ulong charId = player.GetCharacterMetaId();
            if (charId <= 0) return;

            // 1 = Accounts
            if (ID == 1)
            {
                DatabaseHandler.LoadAllPlayers();
                HUDHandler.SendNotification(player, 2, 5000, "Accounts neugeladen!");
            }
            // 2 = Characters
            if (ID == 2)
            {
                DatabaseHandler.LoadAllPlayerCharacters();
                HUDHandler.SendNotification(player, 2, 5000, "Characters neugeladen!");
            }
            // 3 = GARAGE
            if (ID == 3)
            {
                DatabaseHandler.LoadAllGarages();
                DatabaseHandler.LoadAllGarageSlots();
                HUDHandler.SendNotification(player, 2, 5000, "Garagen neugeladen");
            }
            // 4 = VEHICLESHOP
            if (ID == 4)
            {
                DatabaseHandler.LoadAllVehicleShops();
                DatabaseHandler.LoadAllVehicleShopItems();
                HUDHandler.SendNotification(player, 2, 5000, "Fahrzeugshops neugeladen!");
            }
            // 5 = SHOPS
            if (ID == 5)
            {
                DatabaseHandler.LoadAllServerShops();
                DatabaseHandler.LoadAllServerShopItems();
                HUDHandler.SendNotification(player, 2, 5000, "Supermarket neugeladen!");
            }
            // 6 = Clothesshops
            if (ID == 6)
            {
                DatabaseHandler.LoadAllClothesShops();
                HUDHandler.SendNotification(player, 2, 5000, "Kleidungsshops neugeladen");
            }
            // 7 = Tankstellen
            if (ID == 7)
            {
                DatabaseHandler.LoadAllServerFuelStations();
                DatabaseHandler.LoadALlServerFuelStationSpots();
                HUDHandler.SendNotification(player, 2, 5000, "Tankstellen neugeladen!");
            }
            // reNew
            if (ID <= 0)
            {
                DatabaseHandler.RenewAll();
                HUDHandler.SendNotification(player, 2, 5000, "ReNew CharactersEntrys DONE");
            }
        }

        [Command("setBank")]
        public void CMD_SETBANK(IPlayer player, string zoneName)
        {
            if (player == null || !player.Exists) return;
            if (player.AdminLevel() <= 8) { HUDHandler.SendNotification(player, 4, 5000, "Keine Rechte."); return; }
            if (!player.HasData("isAduty")) { HUDHandler.SendNotification(player, 4, 5000, "Nicht im (/am) Admindienst."); return; }
            ulong charId = player.GetCharacterMetaId();
            if (charId <= 0) return;
            ServerBank.CreateNewBank(player, 0, player.Position, zoneName);
            HUDHandler.SendNotification(player, 2, 3500, $"Du hast die Bank {zoneName} erfolgreich erstellt!");
        }

        [Command("getaccountidbymail")]
        public static void CMD_getAccountIdByMail(ClassicPlayer player, string mail)
        {
            try
            {
                if (player == null || !player.Exists || player.CharacterId <= 0 || player.AdminLevel() <= 0) return;
                if (!player.HasData("isAduty")) { HUDHandler.SendNotification(player, 4, 5000, "Nicht im (/am) Admindienst."); return; }
                var accEntry = User.Player.ToList().FirstOrDefault(x => x.Email == mail);
                if (accEntry == null) return;
                HUDHandler.SendNotification(player, 2, 5000, $"Spieler-ID der E-Mail {mail} lautet: {accEntry.playerid} - Spielername: {accEntry.playerName}");
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [Command("trace")]
        public static void trace_CMD(IPlayer player)
        {
            try
            {
                if (player == null || !player.Exists || player.AdminLevel() <= 8) return;
                if (!player.HasData("isAduty")) { HUDHandler.SendNotification(player, 4, 5000, "Nicht im (/am) Admindienst."); return; }
                AltTrace.Start("FabiansDebugKasten");
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [Command("stoptrace")]
        public static void stopTrace_CMD(IPlayer player)
        {
            try
            {
                if (player == null || !player.Exists || player.AdminLevel() <= 8) return;
                if (!player.HasData("isAduty")) { HUDHandler.SendNotification(player, 4, 5000, "Nicht im (/am) Admindienst."); return; }
                AltTrace.Stop();
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [Command("kick")]
        public static void cmd_KICK(IPlayer player, int charId)
        {
            try
            {
                if (player == null || !player.Exists || charId <= 0 || player.AdminLevel() <= 1) return;
                var targetP = Alt.GetAllPlayers().ToList().FirstOrDefault(x => x != null && x.Exists && User.GetPlayerOnline(x) == charId);
                if (targetP == null) return;
                targetP.Kick("");
                HUDHandler.SendNotification(player, 4, 5000, $"Spieler mit Char-ID {charId} Erfolgreich gekickt.");
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [Command("ban")]
        public static void cmd_BAn(IPlayer player, int accId)
        {
            try
            {
                if (player == null || !player.Exists || accId <= 0 || player.AdminLevel() <= 2) return;
                if (!player.HasData("isAduty")) { HUDHandler.SendNotification(player, 4, 5000, "Nicht im (/am) Admindienst."); return; }
                User.SetPlayerBanned(accId, true, $"Gebannt von {Characters.GetCharacterName(User.GetPlayerOnline(player))}");
                var targetP = Alt.GetAllPlayers().ToList().FirstOrDefault(x => x != null && x.Exists && User.GetPlayerAccountId(x) == accId);
                if (targetP != null) targetP.Kick("");
                HUDHandler.SendNotification(player, 4, 5000, $"Spieler mit ID {accId} Erfolgreich gebannt.");
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [Command("unban")]
        public static void CMD_Unban(ClassicPlayer player, int accId)
        {
            try
            {
                if (player == null || !player.Exists || player.CharacterId <= 0 || accId <= 0 || player.AdminLevel() <= 3) return;
                if (!player.HasData("isAduty")) { HUDHandler.SendNotification(player, 4, 5000, "Nicht im (/am) Admindienst."); return; }
                User.SetPlayerBanned(accId, false, "");
                HUDHandler.SendNotification(player, 4, 5000, $"Spieler mit ID {accId} Erfolgreich entbannt.");
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [Command("changeprice")]
        public static void cmd_ChangeP(IPlayer player, int shopId, int itemId, int newPrice)
        {
            try
            {
                if (player == null || !player.Exists || shopId <= 0 || itemId <= 0 || newPrice < 0 || player.AdminLevel() <= 8) return;
                if (!player.HasData("isAduty")) { HUDHandler.SendNotification(player, 4, 5000, "Nicht im (/am) Admindienst."); return; }
                var shopItem = ServerShopsItems.ServerShopsItems_.FirstOrDefault(x => x != null && x.shopId == shopId && x.id == itemId);
                if (shopItem == null) return;
                shopItem.itemPrice = newPrice;
                using (gtaContext db = new gtaContext())
                {
                    db.Server_Shops_Items.Update(shopItem);
                    db.SaveChanges();
                }
                HUDHandler.SendNotification(player, 4, 5000, "Preis geändert.");
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [Command("announce", true)]
        public void announceCMD(IPlayer player, string msg)
        {
            try
            {
                if (player == null || !player.Exists) return;
                if (player.AdminLevel() <= 8) { HUDHandler.SendNotification(player, 4, 5000, "Keine Rechte."); return; }
                if (!player.HasData("isAduty")) { HUDHandler.SendNotification(player, 4, 5000, "Nicht im (/am) Admindienst."); return; }

                foreach (var client in Alt.GetAllPlayers())
                {
                    if (client == null || !client.Exists) continue;
                    HUDHandler.SendNotification(client, 4, 10000, msg);
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [Command("support", true)]
        public void supportCMD(IPlayer player, string msg)
        {
            try
            {
                if (player == null || !player.Exists || User.GetPlayerOnline(player) <= 0) return;
                if (!player.HasData("isAduty")) { HUDHandler.SendNotification(player, 4, 5000, "Nicht im (/am) Admindienst."); return; }
                foreach (var admin in Alt.GetAllPlayers().Where(x => x != null && x.Exists && x.AdminLevel() > 0))
                {
                    //admin.SendChatMessage($"[SUPPORT] {Characters.GetCharacterName(User.GetPlayerOnline(player))} (ID: {User.GetPlayerOnline(player)}) benötigt Support: {msg}");
                    HUDHandler.SendNotification(admin, 4, 15000, $"[SUPPORT] {Characters.GetCharacterName(User.GetPlayerOnline(player))} (ID: {User.GetPlayerOnline(player)}) benötigt Support: {msg}");
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [Command("report", true)]
        public void reportCMD(IPlayer player, string msg)
        {
            try
            {
                if (player == null || !player.Exists || User.GetPlayerOnline(player) <= 0) return;
                foreach (var admin in Alt.GetAllPlayers().Where(x => x != null && x.Exists && x.AdminLevel() > 5))
                {
                    //admin.SendChatMessage($"[SUPPORT] {Characters.GetCharacterName(User.GetPlayerOnline(player))} (ID: {User.GetPlayerOnline(player)}) benötigt Support: {msg}");
                    HUDHandler.SendNotification(admin, 4, 15000, $"[REPORT] {Characters.GetCharacterName(User.GetPlayerOnline(player))} (ID: {User.GetPlayerOnline(player)}) benötigt Support: {msg}");
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [Command("changegender")]
        public void cMD(IPlayer player, int gender)
        {
            if (player.AdminLevel() <= 8) { HUDHandler.SendNotification(player, 4, 5000, "Keine Rechte."); return; }
            if (!player.HasData("isAduty")) { HUDHandler.SendNotification(player, 4, 5000, "Nicht im (/am) Admindienst."); return; }
            switch (gender)
            {
                case 0: player.Model = 1885233650; break;
                case 1: player.Model = 2627665880; break;
            }
        }

        [Command("car")]
        public void heyCMD(IPlayer player, string model)
        {
            if (player == null || !player.Exists) return;
            if (player.AdminLevel() <= 1) { HUDHandler.SendNotification(player, 4, 5000, "Keine Rechte."); return; }
            if (!player.HasData("isAduty")) { HUDHandler.SendNotification(player, 4, 5000, "Nicht im (/am) Admindienst."); return; }
            if (player.Vehicle != null && player.Vehicle.Exists) player.Vehicle.Remove();
            IVehicle veh = Alt.CreateVehicle(model, new Position(player.Position.X + 2f, player.Position.Y, player.Position.Z), player.Rotation);
            veh.EngineOn = true;
            veh.LockState = VehicleLockState.Unlocked;
            veh.SetNumberplateTextAsync("Admin");
            veh.PrimaryColor = 55;
            veh.SecondaryColor = 55;
            veh.DashboardColor = 55;
            veh.WindowTint = 3;
            veh.WheelColor = 55;
            veh.PearlColor = 3;
            player.EmitLocked("Client:HUD:setIntoVehicle", veh); //Setze Spieler in Fahrzeug (First try :D)
        }

        [Command("carHash")]
        public static void CMD_GetCarHash(IPlayer player, string car)
        {
            try
            {
                if (player.AdminLevel() <= 8) { HUDHandler.SendNotification(player, 4, 5000, "Keine Rechte."); return; }
                if (!player.HasData("isAduty")) { HUDHandler.SendNotification(player, 4, 5000, "Nicht im (/am) Admindienst."); return; }
                if (car == null) { HUDHandler.SendNotification(player, 4, 5000, "Fahrzeugname nicht angegeben!"); return; }
                IVehicle testVehicle = Alt.CreateVehicle(car, player.Position, player.Rotation);

                if (testVehicle == null)
                {
                    HUDHandler.SendNotification(player, 2, 5000, $"Falsches Fahrzeug.");
                }
                else
                {
                    testVehicle.Remove();
                    uint hashedCar = Alt.Hash(car);
                    HUDHandler.SendNotification(player, 2, 20000, $"Fahrzeugname: {car} || Fahrzeughash: {hashedCar}");
                    StreamWriter hashFile;
                    if (!File.Exists("SavedCoords.txt"))
                    {
                        hashFile = new StreamWriter("SavedHashed.txt");
                    }
                    else
                    {
                        hashFile = File.AppendText("SavedHashed.txt");
                    }
                    HUDHandler.SendNotification(player, 4, 8000, "Die SavedHashed.txt datei wurde überarbeitet!");
                    hashFile.WriteLine("| " + car + " | " + "Saved hash: " + hashedCar);
                    hashFile.Close();
                }

            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [Command("makeAdmin")]
        public static void CMD_Giveadmin(IPlayer player, int accId, int adminLevel)
        {
            if (player.AdminLevel() <= 8) { HUDHandler.SendNotification(player, 4, 5000, "Keine Rechte."); return; }
            if (!player.HasData("isAduty")) { HUDHandler.SendNotification(player, 4, 5000, "Nicht im (/am) Admindienst."); return; }
            try
            {
                if (player == null || !player.Exists) return;
                User.SetPlayerAdminLevel(accId, adminLevel);
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [Command("tppos")]
        public void TpPosCMD(IPlayer player, float X, float Y, float Z)
        {
            try
            {
                if (player == null || !player.Exists) return;
                if (player.AdminLevel() <= 2) { HUDHandler.SendNotification(player, 4, 5000, "Keine Rechte."); return; }
                if (!player.HasData("isAduty")) { HUDHandler.SendNotification(player, 4, 5000, "Nicht im (/am) Admindienst."); return; }
                player.Position = new Position(X, Y, Z);
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [Command("dv")]
        public void CMD_DeleteVehicle(IPlayer player, float range = 2)
        {
            if (!player.HasData("isAduty")) { HUDHandler.SendNotification(player, 4, 5000, "Nicht im (/am) Admindienst."); return; }
            foreach (IVehicle veh in Alt.GetAllVehicles())
            {
                if (veh.Position.Distance(player.Position) <= range)
                {
                    Alt.RemoveVehicle(veh);
                }
            }
        }

        [Command("parkvehicle")]
        public static void CMD_parkVehicleById(IPlayer player, int vehId)
        {
            try
            {
                if (player == null || !player.Exists || player.AdminLevel() <= 8 || vehId <= 0) return;
                if (!player.HasData("isAduty")) { HUDHandler.SendNotification(player, 4, 5000, "Nicht im (/am) Admindienst."); return; }
                var vehicle = Alt.GetAllVehicles().ToList().FirstOrDefault(x => x != null && x.Exists && x.HasVehicleId() && (int)x.GetVehicleId() == vehId);
                if (vehicle == null) return;
                ServerVehicles.SetVehicleInGarage(vehicle, true, 25);
                HUDHandler.SendNotification(player, 4, 5000, $"Fahrzeug {vehId} in Garage 1(Pillbox) eingeparkt");
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [Command("parkallvehicles")]
        public static void CMD_ParkALlVehs(IPlayer player)
        {
            try
            {
                if (player == null || !player.Exists || player.AdminLevel() <= 8) return;
                if (!player.HasData("isAduty")) { HUDHandler.SendNotification(player, 4, 5000, "Nicht im (/am) Admindienst."); return; }
                int count = 0;
                foreach (var veh in Alt.GetAllVehicles().ToList().Where(x => x != null && x.Exists && x.HasVehicleId()))
                {
                    if (veh == null || !veh.Exists || !veh.HasVehicleId()) continue;
                    int currentGarageId = ServerVehicles.GetVehicleGarageId(veh);
                    if (currentGarageId <= 0) continue;
                    ServerVehicles.SetVehicleInGarage(veh, true, currentGarageId);
                    count++;
                }

                HUDHandler.SendNotification(player, 4, 5000, $"{count} Fahrzeuge eingeparkt.");
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [Command("parkvehiclekz", true)]
        public static void CMD_parkVehicle(IPlayer player, string plate)
        {
            try
            {
                if (player == null || !player.Exists || player.AdminLevel() <= 1 || string.IsNullOrWhiteSpace(plate)) return;
                if (!player.HasData("isAduty")) { HUDHandler.SendNotification(player, 4, 5000, "Nicht im (/am) Admindienst."); return; }
                var vehicle = Alt.GetAllVehicles().ToList().FirstOrDefault(x => x != null && x.Exists && x.HasVehicleId() && (int)x.GetVehicleId() > 0 && x.NumberplateText.ToLower() == plate.ToLower());
                if (vehicle == null) return;
                ServerVehicles.SetVehicleInGarage(vehicle, true, 25);
                HUDHandler.SendNotification(player, 4, 5000, $"Fahrzeug mit dem Kennzeichen {plate} in Garage 1 (Pillbox) eingeparkt");
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

/*        [Command("whitelist")]
        public void WhitelistCMD(IPlayer player, int targetAccId)
        {
            try
            {
                if (player == null || !player.Exists || targetAccId <= 0 || player.GetCharacterMetaId() <= 0) return;
                if (player.AdminLevel() <= 0) { HUDHandler.SendNotification(player, 4, 5000, "Keine Rechte."); return; }
                if (!User.ExistPlayerById(targetAccId)) { HUDHandler.SendNotification(player, 4, 5000, $"Diese ID existiert nicht {targetAccId}"); return; }
                if (User.IsPlayerWhitelisted(targetAccId)) { HUDHandler.SendNotification(player, 4, 5000, "Der Spieler ist bereits gewhitelisted."); return; }
                User.SetPlayerWhitelistState(targetAccId, true);
                HUDHandler.SendNotification(player, 4, 5000, $"Du hast den Spieler {targetAccId} gewhitelistet.");
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

*/        [Command("revive")]
        public void ReviveTargetCMD(IPlayer player, int targetId)
        {
            if (player == null || !player.Exists) return;
            if (player.AdminLevel() <= 2) { HUDHandler.SendNotification(player, 4, 5000, "Keine Rechte."); return; }
            if (!player.HasData("isAduty")) { HUDHandler.SendNotification(player, 4, 5000, "Nicht im (/am) Admindienst."); return; }
            string charName = Characters.GetCharacterName(targetId);
            if (!Characters.ExistCharacterName(charName)) return;
            var tp = Alt.GetAllPlayers().FirstOrDefault(x => x != null && x.Exists && x.GetCharacterMetaId() == (ulong)targetId);
            if (tp != null)
            {
                tp.Health = 200;
                DeathHandler.revive(tp);
                Alt.Emit("SaltyChat:SetPlayerAlive", tp, true);
                HUDHandler.SendNotification(player, 4, 5000, $"Du hast den Spieler {charName} wiederbelebt.");
                return;
            }
            HUDHandler.SendNotification(player, 4, 5000, $"Der Spieler {charName} ist nicht online.");
        }

        [Command("faction")]
        public void FactionCMD(IPlayer player, int charId, int id)
        {
            try
            {
                if (player == null || !player.Exists || player.GetCharacterMetaId() <= 0) return;
                if (player.AdminLevel() <= 8) { HUDHandler.SendNotification(player, 4, 5000, "Keine Rechte."); return; }
                if (!player.HasData("isAduty")) { HUDHandler.SendNotification(player, 4, 5000, "Nicht im (/am) Admindienst."); return; }
                if (ServerFactions.IsCharacterInAnyFaction(charId))
                {
                    ServerFactions.RemoveServerFactionMember(ServerFactions.GetCharacterFactionId(charId), charId);
                }

                ServerFactions.CreateServerFactionMember(id, charId, ServerFactions.GetFactionMaxRankCount(id), charId);
                HUDHandler.SendNotification(player, 4, 5000, $"Done.");
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [Command("giveitem")]
        public void GiveItemCMD(IPlayer player, string itemName, int itemAmount)
        {
            if (player == null || !player.Exists) return;
            if (player.AdminLevel() <= 8) { HUDHandler.SendNotification(player, 4, 5000, "Keine Rechte."); return; }
            if (!player.HasData("isAduty")) { HUDHandler.SendNotification(player, 4, 5000, "Nicht im (/am) Admindienst."); return; }
            if (!ServerItems.ExistItem(ServerItems.ReturnNormalItemName(itemName))) { HUDHandler.SendNotification(player, 4, 5000, $"Itemname nicht gefunden: {itemName}"); return; }
            ulong charId = player.GetCharacterMetaId();
            if (charId <= 0) return;
            CharactersInventory.AddCharacterItem((int)charId, itemName, itemAmount, "inventory");
            HUDHandler.SendNotification(player, 2, 5000, $"Gegenstand '{itemName}' ({itemAmount}x) erhalten.");
        }

        [Command("tp", false)]
        public void GotoCMD(IPlayer player, int targetId)
        {
            if (player.AdminLevel() <= 1) { HUDHandler.SendNotification(player, 4, 5000, "Keine Rechte."); return; }
            if (!player.HasData("isAduty")) { HUDHandler.SendNotification(player, 4, 5000, "Nicht im (/am) Admindienst."); return; }
            try
            {
                if (player == null || !player.Exists) return;
                if (targetId <= 0 || targetId.ToString().Length <= 0)
                {
                    HUDHandler.SendNotification(player, 4, 5000, "Benutzung: /goto charId");
                    return;
                }
                string targetCharName = Characters.GetCharacterName(targetId);
                if (targetCharName.Length <= 0)
                {
                    HUDHandler.SendNotification(player, 3, 5000, $"Warnung: Die angegebene Character-ID wurde nicht gefunden ({targetId}).");
                    return;
                }
                if (!Characters.ExistCharacterName(targetCharName))
                {
                    HUDHandler.SendNotification(player, 3, 5000, $"Warnung: Der angegebene Charaktername wurde nicht gefunden ({targetCharName} - ID: {targetId}).");
                    return;
                }
                var targetPlayer = Alt.GetAllPlayers().FirstOrDefault(x => x != null && x.Exists && x.GetCharacterMetaId() == (ulong)targetId);
                if (targetPlayer == null || !targetPlayer.Exists) { HUDHandler.SendNotification(player, 4, 5000, "Fehler: Spieler ist nicht online."); return; }
                HUDHandler.SendNotification(targetPlayer, 1, 5000, $"{Characters.GetCharacterName((int)player.GetCharacterMetaId())} hat sich zu dir teleportiert.");
                HUDHandler.SendNotification(player, 2, 5000, $"Du hast dich zu dem Spieler {Characters.GetCharacterName((int)targetPlayer.GetCharacterMetaId())} teleportiert.");
                player.Position = targetPlayer.Position + new Position(0, 0, 1);
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [Command("gethere", false)]
        public void GetHereCMD(IPlayer player, int targetId)
        {
            if (player.AdminLevel() <= 1) { HUDHandler.SendNotification(player, 4, 5000, "Keine Rechte."); return; }
            if (!player.HasData("isAduty")) { HUDHandler.SendNotification(player, 4, 5000, "Nicht im (/am) Admindienst."); return; }
            try
            {
                if (player == null || !player.Exists) return;
                if (targetId <= 0 || targetId.ToString().Length <= 0)
                {
                    HUDHandler.SendNotification(player, 4, 5000, "Benutzung: /gethere charId");
                    return;
                }
                string targetCharName = Characters.GetCharacterName(targetId);
                if (targetCharName.Length <= 0)
                {
                    HUDHandler.SendNotification(player, 3, 5000, $"Warnung: Die angegebene Character-ID wurde nicht gefunden ({targetId}).");
                    return;
                }
                if (!Characters.ExistCharacterName(targetCharName))
                {
                    HUDHandler.SendNotification(player, 3, 5000, $"Warnung: Der angegebene Charaktername wurde nicht gefunden ({targetCharName} - ID: {targetId}).");
                    return;
                }
                var targetPlayer = Alt.GetAllPlayers().FirstOrDefault(x => x != null && x.Exists && x.GetCharacterMetaId() == (ulong)targetId);
                if (targetPlayer == null || !targetPlayer.Exists) { HUDHandler.SendNotification(player, 4, 5000, "Fehler: Spieler ist nicht online."); return; }
                HUDHandler.SendNotification(targetPlayer, 1, 5000, $"{Characters.GetCharacterName((int)player.GetCharacterMetaId())} hat dich zu Ihm teleportiert.");
                HUDHandler.SendNotification(player, 2, 5000, $"Du hast den Spieler {Characters.GetCharacterName((int)targetPlayer.GetCharacterMetaId())} zu dir teleportiert.");
                targetPlayer.Position = player.Position + new Position(0, 0, 1);
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }


        [Command("pos")]
        public static void PosCMD(IPlayer player, string coordName)
        {
            if (!player.HasData("isAduty")) { HUDHandler.SendNotification(player, 4, 5000, "Nicht im (/am) Admindienst."); return; }
            if (coordName == null)
            {
                HUDHandler.SendNotification(player, 4, 5000, "Kein Namen angegeben!");
                return;
            }
            if (player.AdminLevel() <= 0)
            {
                HUDHandler.SendNotification(player, 4, 5000, "Keine Rechte.");
                return;
            }

            Position playerPosGet = player.Position;
            Rotation playerRotGet = player.Rotation;

            HUDHandler.SendNotification(player, 4, 8000, $"{coordName}: {playerPosGet.ToString()} - {playerRotGet.ToString()}");
            StreamWriter coordsFile;
            if (!File.Exists("SavedCoords.txt"))
            {
                coordsFile = new StreamWriter("SavedCoords.txt");
            }
            else
            {
                coordsFile = File.AppendText("SavedCoords.txt");
            }
            HUDHandler.SendNotification(player, 4, 8000, "Die SavedCoords.txt datei wurde überarbeitet!");
            coordsFile.WriteLine("| " + coordName + " | " + "Saved Coordenates: " + playerPosGet.ToString() + " Saved Rotation: " + playerRotGet.ToString());
            coordsFile.Close();
        }

        [Command("am", true)]
        public void AdutyCMD(IPlayer player)
        {
            if (player == null || !player.Exists) return;
            if (player.AdminLevel() <= 2) { HUDHandler.SendNotification(player, 4, 5000, "Keine Rechte."); return; }

            if (player.HasData("isAduty"))
            {
                player.DeleteData("isAduty");
                Characters.SetCharacterCorrectClothes(player);
                HUDHandler.SendNotification(player, 4, 5000, $"Du bist nun nicht mehr als Admin unterwegs. (F9 Deaktiviert)");


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
            else
            {
                player.SetData("isAduty", true);
                if (!Characters.GetCharacterGender((int)player.GetCharacterMetaId()))
                {
                    //MÃ¤nnlich
                    player.EmitLocked("Client:SpawnArea:setCharClothes", 1, 135, 10);
                    player.EmitLocked("Client:SpawnArea:setCharClothes", 4, 114, 10);
                    player.EmitLocked("Client:SpawnArea:setCharClothes", 6, 78, 10);
                    player.EmitLocked("Client:SpawnArea:setCharClothes", 3, 3, 0);
                    player.EmitLocked("Client:SpawnArea:setCharClothes", 5, 0, 0);
                    player.EmitLocked("Client:SpawnArea:setCharClothes", 7, 0, 0);
                    player.EmitLocked("Client:SpawnArea:setCharClothes", 8, 15, 0); 
                    player.EmitLocked("Client:SpawnArea:setCharAccessory", 0, 11, 0); //hut
                    player.EmitLocked("Client:SpawnArea:setCharClothes", 9, 0, 0);
                    player.EmitLocked("Client:SpawnArea:setCharClothes", 11, 287, 10);
                    player.EmitLocked("Client:SpawnArea:setCharClothes", 8, 1, 99);
                }
                else
                {
                    //Weiblich
                    player.EmitLocked("Client:SpawnArea:setCharClothes", 1, 135, 4);
                    player.EmitLocked("Client:SpawnArea:setCharClothes", 11, 300, 4);
                    player.EmitLocked("Client:SpawnArea:setCharClothes", 4, 121, 4);
                    player.EmitLocked("Client:SpawnArea:setCharClothes", 5, 0, 0);
                    player.EmitLocked("Client:SpawnArea:setCharClothes", 9, 0, 0);
                    player.EmitLocked("Client:SpawnArea:setCharAccessory", 0, 57, 0); //hut
                    player.EmitLocked("Client:SpawnArea:setCharClothes", 7, 0, 0);
                    player.EmitLocked("Client:SpawnArea:setCharClothes", 8, 14, 0);
                    player.EmitLocked("Client:SpawnArea:setCharClothes", 3, 8, 0);
                    player.EmitLocked("Client:SpawnArea:setCharClothes", 8, 1, 99);
                    player.EmitLocked("Client:SpawnArea:setCharClothes", 6, 82, 4);
                }
                HUDHandler.SendNotification(player, 4, 5000, $"Du bist nun als Admin unterwegs. (F9 aktiviert)");
            }
        }

    }
}
