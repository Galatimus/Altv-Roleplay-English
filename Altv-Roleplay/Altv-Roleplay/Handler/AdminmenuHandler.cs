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
using Altv_Roleplay.Services;
using Altv_Roleplay.Utils;
using Newtonsoft.Json.Linq;

namespace Altv_Roleplay.Handler
{
    class AdminmenuHandler : IScript
    {

        [AsyncClientEvent("Server:AdminMenu:OpenMenu")]
        public void AdminmenuOpenMenu(IPlayer player)
        {
            try
            {
                if (player.AdminLevel() != 0) player.EmitLocked("Client:Adminmenu:OpenMenu");
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [AsyncClientEvent("Server:AdminMenu:CloseMenu")]
        public void AdminmenuCloseMenu(IPlayer player)
        {
            try
            {
                if (player.AdminLevel() != 0) player.EmitLocked("Client:Adminmenu:CloseMenu");
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [AsyncClientEvent("Server:AdminMenu:DoAction")]
        public void AdminmenuDoAction(IPlayer player, string action, string info, string addinfo, string inputvalue)
        {
            try
            {
                if (player.AdminLevel() != 0)
                {
                    switch (action)
                    {
                        // PLAYER
                        case "noclip":
                            player.EmitLocked("Client:AdminMenu:Noclip", info);

                            var text = "";
                            if (info == "on") text = " hat **NoClip** angemacht.";
                            else text = " hat **NoClip** ausgemacht.";
                            //.SendEmbed("adminmenu", "Adminmenu Logs", Characters.GetCharacterName((int)player.GetCharacterMetaId()) + text);
                            break;
                        case "unsichtbar":
                            if (player.Visible) player.Visible = false;
                            else player.Visible = true;

                            if (info == "on") text = " hat sich **unsichtbar** gemacht.";
                            else text = " hat sich **sichtbar** gemacht.";
                            //.SendEmbed("adminmenu", "Adminmenu Logs", Characters.GetCharacterName((int)player.GetCharacterMetaId()) + text);
                            break;
                        case "adminoutfit":
                            SetAdminClothes(player, info);

                            if (info == "on") text = " hat sich das **Adminoutfit angezogen**.";
                            else text = " hat sich das **Adminoutfit angezogen**.";
                            //.SendEmbed("adminmenu", "Adminmenu Logs", Characters.GetCharacterName((int)player.GetCharacterMetaId()) + text);
                            break;
                        case "godmode":
                            player.EmitLocked("Client:AdminMenu:Godmode", info);

                            if (info == "on") text = " hat den **Godmode** angemacht.";
                            else text = " hat den **Godmode** ausgemacht.";
                            //.SendEmbed("adminmenu", "Adminmenu Logs", Characters.GetCharacterName((int)player.GetCharacterMetaId()) + text);
                            break;
                        case "heilen":
                            player.Health = 200;

                            //.SendEmbed("adminmenu", "Adminmenu Logs", Characters.GetCharacterName((int)player.GetCharacterMetaId()) + "hat sich * *geheilt * *.");
                            break;
                        case "wiederbeleben":
                            DeathHandler.revive(player);
                            Alt.Emit("SaltyChat:SetPlayerAlive", player, true);

                            //.SendEmbed("adminmenu", "Adminmenu Logs", Characters.GetCharacterName((int)player.GetCharacterMetaId()) + "hat sich **wiederbelebt**.");
                            break;
                        // ONLINE
                        case "spieler_kicken":
                            var kickedPlayer = Alt.GetAllPlayers().ToList().FirstOrDefault(x => User.GetPlayerUsername(Characters.GetCharacterAccountId((int)x.GetCharacterMetaId())) == addinfo);
                            if (string.IsNullOrWhiteSpace(inputvalue))
                            {
                                kickedPlayer.kickWithMessage("Du wurdest von einem Teammitglied gekickt.");
                                //.SendEmbed("adminmenu", "Adminmenu Logs", Characters.GetCharacterName((int)player.GetCharacterMetaId()) + " hat " + Characters.GetCharacterName((int)kickedPlayer.GetCharacterMetaId()) + " **gekickt**.");
                            }
                            else
                            {
                                kickedPlayer.kickWithMessage("Du wurdest von einem Teammitglied gekickt. Grund: " + inputvalue);
                                //.SendEmbed("adminmenu", "Adminmenu Logs", Characters.GetCharacterName((int)player.GetCharacterMetaId()) + " hat " + Characters.GetCharacterName((int)kickedPlayer.GetCharacterMetaId()) + " **gekickt**. Grund: " + inputvalue);
                            }
                            break;
                        case "spieler_bannen":
                            var bannedPlayer = Alt.GetAllPlayers().ToList().FirstOrDefault(x => User.GetPlayerUsername(Characters.GetCharacterAccountId((int)x.GetCharacterMetaId())) == addinfo);
                            if (string.IsNullOrWhiteSpace(inputvalue))
                            {
                                User.SetPlayerBanned(Characters.GetCharacterAccountId((int)bannedPlayer.GetCharacterMetaId()), true, $"Gebannt von {Characters.GetCharacterName(User.GetPlayerOnline(player))}");
                                if (bannedPlayer != null) bannedPlayer.Kick("Du wurdest von einem Teammitglied gebannt.");
                                //.SendEmbed("adminmenu", "Adminmenu Logs", Characters.GetCharacterName((int)player.GetCharacterMetaId()) + " hat " + Characters.GetCharacterName((int)bannedPlayer.GetCharacterMetaId()) + " **gebannt**.");
                            }
                            else
                            {
                                User.SetPlayerBanned(Characters.GetCharacterAccountId((int)bannedPlayer.GetCharacterMetaId()), true, $"Gebannt von {Characters.GetCharacterName(User.GetPlayerOnline(player))}. Grund: " + inputvalue);
                                if (bannedPlayer != null) bannedPlayer.Kick("Du wurdest von einem Teammitglied gebannt. Grund: " + inputvalue);
                                //.SendEmbed("adminmenu", "Adminmenu Logs", Characters.GetCharacterName((int)player.GetCharacterMetaId()) + " hat " + Characters.GetCharacterName((int)bannedPlayer.GetCharacterMetaId()) + " **gebannt**. Grund: " + inputvalue);
                            }
                            break;
                        case "spieler_einfrieren":
                            var kickedPlayerr = Alt.GetAllPlayers().ToList().FirstOrDefault(x => User.GetPlayerUsername(Characters.GetCharacterAccountId((int)x.GetCharacterMetaId())) == addinfo);
                            Alt.EmitAllClients("Client:AdminMenu:SetFreezed", kickedPlayerr, info);

                            //.SendEmbed("adminmenu", "Adminmenu Logs", Characters.GetCharacterName((int)player.GetCharacterMetaId()) + " hat " + Characters.GetCharacterName((int)kickedPlayerr.GetCharacterMetaId()) + " **eingefroren**.");
                            break;
                        case "spieler_spectaten":
                            var spectatedPlayer = Alt.GetAllPlayers().ToList().FirstOrDefault(x => User.GetPlayerUsername(Characters.GetCharacterAccountId((int)x.GetCharacterMetaId())) == addinfo);
                            player.Emit("Client:AdminMenu:Spectate", spectatedPlayer, info);

                            if (info == "on") { text = " **spectated** nun " + Characters.GetCharacterName((int)spectatedPlayer.GetCharacterMetaId()); Alt.EmitAllClients("Client:AdminMenu:SetInvisible", player, "on"); }
                            else { text = " hat aufgehört, " + Characters.GetCharacterName((int)spectatedPlayer.GetCharacterMetaId()) + " **spectaten**."; Alt.EmitAllClients("Client:AdminMenu:SetInvisible", player, "off"); }
                            //.SendEmbed("adminmenu", "Adminmenu Logs", Characters.GetCharacterName((int)player.GetCharacterMetaId()) + text);
                            break;
                        case "spieler_heilen":
                            var HealedPlayer = Alt.GetAllPlayers().ToList().FirstOrDefault(x => User.GetPlayerUsername(Characters.GetCharacterAccountId((int)x.GetCharacterMetaId())) == addinfo);
                            HealedPlayer.Health = 200;

                            //.SendEmbed("adminmenu", "Adminmenu Logs", Characters.GetCharacterName((int)player.GetCharacterMetaId()) + " hat sich zu " + Characters.GetCharacterName((int)HealedPlayer.GetCharacterMetaId()) + " **geheilt**.");
                            break;
                        case "spieler_wiederbeleben":
                            var RevivedPlayer = Alt.GetAllPlayers().ToList().FirstOrDefault(x => User.GetPlayerUsername(Characters.GetCharacterAccountId((int)x.GetCharacterMetaId())) == addinfo);
                            DeathHandler.revive(RevivedPlayer);
                            Alt.Emit("SaltyChat:SetPlayerAlive", RevivedPlayer, true);

                            //.SendEmbed("adminmenu", "Adminmenu Logs", Characters.GetCharacterName((int)player.GetCharacterMetaId()) + " hat " + Characters.GetCharacterName((int)RevivedPlayer.GetCharacterMetaId()) + " zu sich **wiederbelebt**.");
                            break;
                        case "tp_zu_spieler":
                            var TeleportToPlayer = Alt.GetAllPlayers().ToList().FirstOrDefault(x => User.GetPlayerUsername(Characters.GetCharacterAccountId((int)x.GetCharacterMetaId())) == addinfo);
                            player.Position = new Position(TeleportToPlayer.Position.X, TeleportToPlayer.Position.Y, TeleportToPlayer.Position.Z);

                            //.SendEmbed("adminmenu", "Adminmenu Logs", Characters.GetCharacterName((int)player.GetCharacterMetaId()) + " hat sich zu " + Characters.GetCharacterName((int)TeleportToPlayer.GetCharacterMetaId()) + " **teleportiert**.");
                            break;
                        case "spieler_zu_mir_tp":
                            var TeleportPlayer = Alt.GetAllPlayers().ToList().FirstOrDefault(x => User.GetPlayerUsername(Characters.GetCharacterAccountId((int)x.GetCharacterMetaId())) == addinfo);
                            TeleportPlayer.Position = new Position(player.Position.X, player.Position.Y, player.Position.Z);

                            //.SendEmbed("adminmenu", "Adminmenu Logs", Characters.GetCharacterName((int)player.GetCharacterMetaId()) + " hat " + Characters.GetCharacterName((int)TeleportPlayer.GetCharacterMetaId()) + " zu sich **teleportiert**.");
                            break;
                        case "item_geben":
                            if (string.IsNullOrWhiteSpace(inputvalue)) break;
                            if (!ServerItems.ExistItem(ServerItems.ReturnNormalItemName(inputvalue))) { HUDHandler.SendNotification(player, 4, 5000, $"Itemname nicht gefunden: {inputvalue}"); break; }
                            var GiveItemPlayer = Alt.GetAllPlayers().ToList().FirstOrDefault(x => User.GetPlayerUsername(Characters.GetCharacterAccountId((int)x.GetCharacterMetaId())) == addinfo);

                            CharactersInventory.AddCharacterItem((int)GiveItemPlayer.GetCharacterMetaId(), inputvalue, 1, "inventory");
                            //.SendEmbed("adminmenu", "Adminmenu Logs", Characters.GetCharacterName((int)player.GetCharacterMetaId()) + " hat " + Characters.GetCharacterName((int)GiveItemPlayer.GetCharacterMetaId()) + " das **Item " + inputvalue + " gegeben**.");
                            break;
                        case "adminlevel_geben":
                            if (string.IsNullOrWhiteSpace(inputvalue)) break;
                            var GiveAdminPlayer = Alt.GetAllPlayers().ToList().FirstOrDefault(x => User.GetPlayerUsername(Characters.GetCharacterAccountId((int)x.GetCharacterMetaId())) == addinfo);
                            if (!Int32.TryParse(inputvalue, out int newinputvaluea)) { HUDHandler.SendNotification(player, 4, 5000, $"Du musst eine Zahl angeben"); break; }
                            if (player.AdminLevel() <= newinputvaluea) { HUDHandler.SendNotification(player, 4, 5000, $"Du darfst dieses Adminlevel nicht vergeben"); break; }
                            User.SetPlayerAdminLevel(Characters.GetCharacterAccountId((int)GiveAdminPlayer.GetCharacterMetaId()), newinputvaluea);

                            //.SendEmbed("adminmenu", "Adminmenu Logs", Characters.GetCharacterName((int)player.GetCharacterMetaId()) + " hat " + Characters.GetCharacterName((int)GiveAdminPlayer.GetCharacterMetaId()) + " **Adminlevel " + newinputvaluea + "** zugewiesen.");
                            break;
                        // MISC
                        case "zum_wegpunkt":
                            player.Emit("Client:AdminMenu:GetWaypointInfo");
                            break;
                        case "nametags":
                            player.Emit("Client:Adminmenu:ToggleNametags", info);
                            break;
                        case "spieler_auf_der_karte_anzeigen":
                            player.Emit("Client:Adminmenu:TogglePlayerBlips", info);
                            break;
                        // FARHEUG
                        case "fahrzeug_spawnen":
                            if (string.IsNullOrWhiteSpace(inputvalue)) break;
                            try
                            {
                                IVehicle veh = Alt.CreateVehicle(inputvalue, player.Position, player.Rotation);
                                veh.EngineOn = true;
                                veh.LockState = VehicleLockState.Unlocked;
                                player.Emit("Client:Utilities:setIntoVehicle", veh);
                                //.SendEmbed("adminmenu", "Adminmenu Logs", Characters.GetCharacterName((int)player.GetCharacterMetaId()) + " hat sich das **Fahrzeug " + inputvalue + " gespawnt**");
                                break;
                            }
                            catch (Exception)
                            {
                                HUDHandler.SendNotification(player, 4, 2000, "Dieses Fahrzeug existiert nicht");
                                break;
                            }
                        case "reparieren":
                            if (player.Vehicle == null || !player.Vehicle.Exists) break;
                            ServerVehicles.SetVehicleEngineHealthy(player.Vehicle, true);
                            Alt.EmitAllClients("Client:Utilities:repairVehicle", player.Vehicle);
                            //.SendEmbed("adminmenu", "Adminmenu Logs", Characters.GetCharacterName((int)player.GetCharacterMetaId()) + " hat sein **Fahrzeug repariert**");
                            break;
                        case "fahrzeug_löschen":
                            if (player.Vehicle == null || !player.Vehicle.Exists) break;
                            player.Vehicle.Remove();

                            //.SendEmbed("adminmenu", "Adminmenu Logs", Characters.GetCharacterName((int)player.GetCharacterMetaId()) + " hat sein **Fahrzeug gelöscht**");
                            break;
                        case "fahrzeugmotor_an_ausschalten":
                            if (player.Vehicle == null || !player.Vehicle.Exists) break;
                            if (!player.Vehicle.EngineOn) player.Vehicle.EngineOn = true;
                            else player.Vehicle.EngineOn = false;
                            break;
                        // PEDS
                        case "zurücksetzen":
                            if (!Characters.GetCharacterGender((int)player.GetCharacterMetaId())) player.Model = Alt.Hash("mp_m_freemode_01");
                            else player.Model = Alt.Hash("mp_f_freemode_01");

                            Characters.SetCharacterCorrectClothes(player);

                            //.SendEmbed("adminmenu", "Adminmenu Logs", Characters.GetCharacterName((int)player.GetCharacterMetaId()) + " hat sein **Outfit zurückgesetzt**");
                            break;
                        case "a_c_boar":
                            player.Model = Alt.Hash(action);
                            //.SendEmbed("adminmenu", "Adminmenu Logs", Characters.GetCharacterName((int)player.GetCharacterMetaId()) + " hat sein **Model zu " + action + " geändert**");
                            break;
                        case "a_c_cat_01":
                            player.Model = Alt.Hash(action);
                            //.SendEmbed("adminmenu", "Adminmenu Logs", Characters.GetCharacterName((int)player.GetCharacterMetaId()) + " hat sein **Model zu " + action + " geändert**");
                            break;
                        case "a_c_chimp":
                            player.Model = Alt.Hash(action);
                            //.SendEmbed("adminmenu", "Adminmenu Logs", Characters.GetCharacterName((int)player.GetCharacterMetaId()) + " hat sein **Model zu " + action + " geändert**");
                            break;
                        case "a_c_chop":
                            player.Model = Alt.Hash(action);
                            //.SendEmbed("adminmenu", "Adminmenu Logs", Characters.GetCharacterName((int)player.GetCharacterMetaId()) + " hat sein **Model zu " + action + " geändert**");
                            break;
                        case "a_c_cow":
                            player.Model = Alt.Hash(action);
                            //.SendEmbed("adminmenu", "Adminmenu Logs", Characters.GetCharacterName((int)player.GetCharacterMetaId()) + " hat sein **Model zu " + action + " geändert**");
                            break;
                        case "a_c_coyote":
                            player.Model = Alt.Hash(action);
                            //.SendEmbed("adminmenu", "Adminmenu Logs", Characters.GetCharacterName((int)player.GetCharacterMetaId()) + " hat sein **Model zu " + action + " geändert**");
                            break;
                        case "a_c_crow":
                            player.Model = Alt.Hash(action);
                            //.SendEmbed("adminmenu", "Adminmenu Logs", Characters.GetCharacterName((int)player.GetCharacterMetaId()) + " hat sein **Model zu " + action + " geändert**");
                            break;
                        case "a_c_husky":
                            player.Model = Alt.Hash(action);
                            //.SendEmbed("adminmenu", "Adminmenu Logs", Characters.GetCharacterName((int)player.GetCharacterMetaId()) + " hat sein **Model zu " + action + " geändert**");
                            break;
                        case "a_c_mtlion":
                            player.Model = Alt.Hash(action);
                            //.SendEmbed("adminmenu", "Adminmenu Logs", Characters.GetCharacterName((int)player.GetCharacterMetaId()) + " hat sein **Model zu " + action + " geändert**");
                            break;
                        case "a_c_poodle":
                            player.Model = Alt.Hash(action);
                            //.SendEmbed("adminmenu", "Adminmenu Logs", Characters.GetCharacterName((int)player.GetCharacterMetaId()) + " hat sein **Model zu " + action + " geändert**");
                            break;
                        case "a_c_pug":
                            player.Model = Alt.Hash(action);
                            //.SendEmbed("adminmenu", "Adminmenu Logs", Characters.GetCharacterName((int)player.GetCharacterMetaId()) + " hat sein **Model zu " + action + " geändert**");
                            break;
                        case "a_c_rabbit_01":
                            player.Model = Alt.Hash(action);
                            //.SendEmbed("adminmenu", "Adminmenu Logs", Characters.GetCharacterName((int)player.GetCharacterMetaId()) + " hat sein **Model zu " + action + " geändert**");
                            break;
                        case "a_c_retriever":
                            player.Model = Alt.Hash(action);
                            //.SendEmbed("adminmenu", "Adminmenu Logs", Characters.GetCharacterName((int)player.GetCharacterMetaId()) + " hat sein **Model zu " + action + " geändert**");
                            break;
                        case "a_c_shepherd":
                            player.Model = Alt.Hash(action);
                            //.SendEmbed("adminmenu", "Adminmenu Logs", Characters.GetCharacterName((int)player.GetCharacterMetaId()) + " hat sein **Model zu " + action + " geändert**");
                            break;
                        case "a_c_westy":
                            player.Model = Alt.Hash(action);
                            //.SendEmbed("adminmenu", "Adminmenu Logs", Characters.GetCharacterName((int)player.GetCharacterMetaId()) + " hat sein **Model zu " + action + " geändert**");
                            break;
                        case "other":
                            if (string.IsNullOrWhiteSpace(inputvalue)) break;
                            try
                            {
                                player.Model = Alt.Hash(inputvalue);
                                //.SendEmbed("adminmenu", "Adminmenu Logs", Characters.GetCharacterName((int)player.GetCharacterMetaId()) + " hat sein **Model zu " + inputvalue + " geändert**");
                                break;
                            }
                            catch (Exception)
                            {
                                HUDHandler.SendNotification(player, 4, 2000, "Dieses Model existiert nicht");
                                break;
                            }
                        // SERVER
                        case "ankündigung":
                            if (string.IsNullOrWhiteSpace(inputvalue)) break;
                            foreach (var client in Alt.GetAllPlayers())
                            {
                                if (client == null || !client.Exists) continue;
                                HUDHandler.SendNotification(client, 4, 5000, inputvalue);
                            }
                            //.SendEmbed("adminmenu", "Adminmenu Logs", Characters.GetCharacterName((int)player.GetCharacterMetaId()) + " hat eine Ankündigung gemacht: **" + inputvalue + "**");
                            break;
                        case "whitelist":
                            if (string.IsNullOrWhiteSpace(inputvalue)) break;
                            if (!User.ExistPlayerName(inputvalue)) { HUDHandler.SendNotification(player, 4, 3000, $"Benutzername {inputvalue} wurde nicht gefunden"); break; }
                            if (User.IsPlayerWhitelisted(inputvalue)) { HUDHandler.SendNotification(player, 4, 3000, $"Spieler {inputvalue} ist bereits gewhitelisted"); break; }
                            User.SetPlayerWhitelistState(User.GetPlayerAccountIdByUsername(inputvalue), true);
                            HUDHandler.SendNotification(player, 1, 3000, inputvalue + " wurde erfolgreich gewhitelisted");

                            //.SendEmbed("adminmenu", "Adminmenu Logs", Characters.GetCharacterName((int)player.GetCharacterMetaId()) + " hat **" + inputvalue + " gewhitelistet**");
                            break;
                        case "fahrzeug_einparken":
                            if (string.IsNullOrWhiteSpace(inputvalue)) break;
                            var vehicle = Alt.GetAllVehicles().ToList().FirstOrDefault(x => x != null && x.Exists && x.HasVehicleId() && (int)x.GetVehicleId() > 0 && x.NumberplateText.ToLower() == inputvalue.ToLower());
                            if (vehicle == null) { HUDHandler.SendNotification(player, 4, 2000, $"Fahrzeug mit dem Kennzeichen {inputvalue} existiert nicht"); break; }
                            ServerVehicles.SetVehicleInGarage(vehicle, true, 1);
                            HUDHandler.SendNotification(player, 1, 5000, $"Fahrzeug mit dem Kennzeichen {inputvalue} in die Pillbox Garage eingeparkt");

                            //.SendEmbed("adminmenu", "Adminmenu Logs", Characters.GetCharacterName((int)player.GetCharacterMetaId()) + " hat das **Fahrzeug mit dem Kennzeichen " + inputvalue + " eingeparkt**");
                            break;
                        case "alle_fahrzeuge_einparken":
                            int count = 0;
                            foreach (var veh in Alt.GetAllVehicles().ToList().Where(x => x != null && x.Exists && x.HasVehicleId()))
                            {
                                if (veh == null || !veh.Exists || !veh.HasVehicleId()) continue;
                                int currentGarageId = ServerVehicles.GetVehicleGarageId(veh);
                                if (currentGarageId <= 0) continue;
                                ServerVehicles.SetVehicleInGarage(veh, true, currentGarageId);
                                count++;
                            }

                            HUDHandler.SendNotification(player, 1, 3500, $"{count} Fahrzeuge eingeparkt");
                            //.SendEmbed("adminmenu", "Adminmenu Logs", Characters.GetCharacterName((int)player.GetCharacterMetaId()) + " hat ** alle Fahrzeuge (" + count + ") eingeparkt**");
                            break;
                        case "fahrzeuginhaber_finden":
                            if (string.IsNullOrWhiteSpace(inputvalue)) break;
                            var fvehicle = Alt.GetAllVehicles().ToList().FirstOrDefault(x => x != null && x.Exists && x.HasVehicleId() && (int)x.GetVehicleId() > 0 && x.NumberplateText.ToLower() == inputvalue.ToLower());
                            if (fvehicle == null) { HUDHandler.SendNotification(player, 4, 2000, $"Fahrzeug mit dem Kennzeichen {inputvalue} existiert nicht"); break; }
                            var ownerId = ServerVehicles.GetVehicleOwner(fvehicle);
                            var findvehownermsg = $"AccId: " + User.GetPlayerByCharId(ownerId).playerid + " | CharId: " + ownerId + " | Benutzername: " + User.GetPlayerUsername(User.GetPlayerByCharId(ownerId).playerid) + " | Name: " + Characters.GetCharacterName(ownerId);
                            HUDHandler.SendNotification(player, 1, 10000, findvehownermsg);
                            //.SendEmbed("adminmenu", "Adminmenu Logs", Characters.GetCharacterName((int)player.GetCharacterMetaId()) + " nach dem Besitzer des Fahrzeuges mit dem Kennzeichen " + inputvalue + " gesucht.\nResultat: **" + findvehownermsg + "**");
                            break;
                        case "hardwareid_zurücksetzen":
                            if (string.IsNullOrWhiteSpace(inputvalue)) break;
                            if (!User.ExistPlayerName(inputvalue)) { HUDHandler.SendNotification(player, 4, 3000, $"Benutzername {inputvalue} wurde nicht gefunden"); break; }
                            User.ResetPlayerHardwareID(User.GetPlayerAccountIdByUsername(inputvalue));
                            HUDHandler.SendNotification(player, 1, 3000, "Hardware-ID von " + inputvalue + " wurde erfolgreich zurückgesetzt");
                            //.SendEmbed("adminmenu", "Adminmenu Logs", Characters.GetCharacterName((int)player.GetCharacterMetaId()) + " hat die **Hardware-ID von " + inputvalue + " wurde zurückgesetzt**");
                            break;
                        case "socialclubid_zurücksetzen":
                            if (string.IsNullOrWhiteSpace(inputvalue)) break;
                            if (!User.ExistPlayerName(inputvalue)) { HUDHandler.SendNotification(player, 4, 3000, $"Benutzername {inputvalue} wurde nicht gefunden"); break; }
                            User.ResetPlayerSocialID(User.GetPlayerAccountIdByUsername(inputvalue));
                            HUDHandler.SendNotification(player, 1, 3000, "Socialclub-ID von " + inputvalue + " wurde erfolgreich zurückgesetzt");
                            //.SendEmbed("adminmenu", "Adminmenu Logs", Characters.GetCharacterName((int)player.GetCharacterMetaId()) + " hat die **Socialclub-ID von " + inputvalue + " wurde zurückgesetzt**");
                            break;
                        // DEFAULT
                        default:
                            Console.WriteLine("Missing Adminmenu Action: " + action);
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [AsyncClientEvent("Server:AdminMenu:RequestAllOnlinePlayers")]
        public void RequestAllOnlinePlayers(IPlayer player)
        {
            try
            {
                if (player.AdminLevel() != 0)
                {
                    try
                    {
                        dynamic array = new JArray() as dynamic;
                        dynamic entry = new JObject();

                        foreach (var plr in Alt.GetAllPlayers().Where(p => p != null && p.Exists))
                        {
                            if (Characters.GetCharacterAccountId((int)plr.GetCharacterMetaId()) == 0 || (int)plr.GetCharacterMetaId() == 0 || Characters.GetCharacterName((int)plr.GetCharacterMetaId()) == "SYSTEM" || User.GetPlayerUsername(Characters.GetCharacterAccountId((int)plr.GetCharacterMetaId())) == "Undefined") continue;
                            entry = new JObject();
                            entry.accid = Characters.GetCharacterAccountId((int)plr.GetCharacterMetaId());
                            entry.charid = (int)plr.GetCharacterMetaId();
                            entry.fullname = Characters.GetCharacterName((int)plr.GetCharacterMetaId());
                            entry.username = User.GetPlayerUsername(Characters.GetCharacterAccountId((int)plr.GetCharacterMetaId()));
                            array.Add(entry);
                        }

                        player.Emit("Client:AdminMenu:SendAllOnlinePlayers", array.ToString());
                    }
                    catch (Exception e)
                    {
                        Alt.Log($"{e}");
                    }
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [AsyncClientEvent("Server:AdminMenu:TeleportWaypoint")]
        public void TeleportWaypoint(IPlayer player, int x, int y, int z)
        {
            try
            {
                if (player.AdminLevel() != 0)
                {
                    try
                    {
                        player.Position = new Position(x, y, z);
                        //.SendEmbed("adminmenu", "Adminmenu Logs", Characters.GetCharacterName((int)player.GetCharacterMetaId()) + " hat sich **zum Wegpunkt teleportiert**.");
                    }
                    catch (Exception e)
                    {
                        Alt.Log($"{e}");
                    }
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [AsyncClientEvent("Server:AdminMenu:GetPlayer")]
        public void GetPlayer(IPlayer player, string reason, string username, string other)
        {
            try
            {
                if (player.AdminLevel() != 0)
                {
                    var GetPlayerPlayer = Alt.GetAllPlayers().ToList().FirstOrDefault(x => User.GetPlayerUsername(Characters.GetCharacterAccountId((int)x.GetCharacterMetaId())) == username);
                    if (reason == "GetPlayerMeta") player.Emit("Client:Adminmenu:ReceiveMeta", GetPlayerPlayer);
                    else if (reason == "SetMeta") player.Emit("Client:Adminmenu:SetMetaDef", GetPlayerPlayer, other);
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static void SetAdminClothes(IPlayer player, string info)
        {
            if (info == "on")
            {
                if (!Characters.GetCharacterGender((int)player.GetCharacterMetaId()))
                {
                    //Männlich
                    player.SetClothes(1, 135, 2, 2);
                    player.SetClothes(4, 114, 2, 2);
                    player.SetClothes(6, 78, 2, 2);
                    player.SetClothes(3, 3, 0, 2);
                    player.SetClothes(11, 287, 2, 2);
                    player.SetClothes(8, 1, 99, 2);

                }
                else
                {
                    //Weiblich
                    player.SetClothes(1, 135, 2, 2);
                    player.SetClothes(11, 300, 2, 2);
                    player.SetClothes(4, 121, 2, 2);
                    player.SetClothes(3, 8, 0, 2);
                    player.SetClothes(8, 1, 99, 2);
                    player.SetClothes(6, 82, 2, 2);

                }
            }
            else if (info == "off")
            {
                Characters.SetCharacterCorrectClothes(player);
            }
        }
    }
}
