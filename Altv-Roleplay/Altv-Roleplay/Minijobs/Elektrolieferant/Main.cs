using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using Altv_Roleplay.Factories;
using Altv_Roleplay.Handler;
using Altv_Roleplay.Model;
using Altv_Roleplay.models;
using Altv_Roleplay.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altv_Roleplay.Minijobs.Elektrolieferant
{
    public class Main
    {       
        public partial class Minijob_Spots
        {
            public int id { get; set; }
            public Position pos { get; set; }
            public Position depositPos { get; set; }
            public IColShape depositColshape { get; set; }
        }
        public static List<Minijob_Spots> MinijobSpots_ = new List<Minijob_Spots>();
        public static ClassicColshape startJobShape = (ClassicColshape)Alt.CreateColShapeSphere(Constants.Positions.Minijob_Elektrolieferent_StartPos, 2f);
        

        public static void Initialize()
        {
            Alt.Log("Lade Minijob: Elektrolieferant...");
            Alt.OnColShape += ColshapeEnterExitHandler;
            Alt.OnPlayerEnterVehicle += PlayerEnterVehicle;
            Alt.OnPlayerLeaveVehicle += PlayerExitVehicle;
            Alt.OnPlayerDisconnect += PlayerDisconnectedHandler;

            var data = new Server_Peds { model = "s_m_y_dockwork_01", posX = startJobShape.Position.X, posY = startJobShape.Position.Y, posZ = startJobShape.Position.Z - 1, rotation = -24.444355010986328f };
            ServerPeds.ServerPeds_.Add(data);

            var MarkerData = new Server_Markers { type = 39, posX = Constants.Positions.Minijob_Elektrolieferant_VehOutPos.X, posY = Constants.Positions.Minijob_Elektrolieferant_VehOutPos.Y, posZ = (Constants.Positions.Minijob_Elektrolieferant_VehOutPos.Z + 0.25f), scaleX = 1, scaleY = 1, scaleZ = 1, red = 46, green = 133, blue = 232, alpha = 150, bobUpAndDown = true };
            ServerBlips.ServerMarkers_.Add(MarkerData);

            MinijobSpots_.Add(new Minijob_Spots() { id = 1, pos = new Position((float)974.2866821289062, (float)7.489353656768799, (float)80.36315155029297), depositPos = new Position((float)974.350341796875, (float)12.931292533874512, (float)81.04092407226562), depositColshape = Alt.CreateColShapeSphere(new Position((float)974.350341796875, (float)12.931292533874512, (float)81.04092407226562), 2f) }); //Casino
            MinijobSpots_.Add(new Minijob_Spots() { id = 2, pos = new Position((float)-1052.0967, (float)-249.11209, (float)37.064453), depositPos = new Position((float)-1041.5845947265625, (float)-241.25299072265625, (float)37.95166015625), depositColshape = Alt.CreateColShapeSphere(new Position((float)-1041.5845947265625, (float)-241.25299072265625, (float)37.95166015625), 2f) }); //Lifeinvader
            MinijobSpots_.Add(new Minijob_Spots() { id = 3, pos = new Position((float)-532.53625, (float)-889.25275, (float)24.106934), depositPos = new Position((float)-537.14306640625, (float)-886.7233276367188, (float)25.197668075561523), depositColshape = Alt.CreateColShapeSphere(new Position((float)-537.14306640625, (float)-886.7233276367188, (float)25.197668075561523), 2f) }); //Weazel News
            MinijobSpots_.Add(new Minijob_Spots() { id = 4, pos = new Position((float)-1398.3956, (float)-463.14725, (float)33.694458), depositPos = new Position((float)-1371.060546875, (float)-460.3683776855469, (float)34.4775390625), depositColshape = Alt.CreateColShapeSphere(new Position((float)-1371.060546875, (float)-460.3683776855469, (float)34.4775390625), 2f) }); //Maze Bank
            MinijobSpots_.Add(new Minijob_Spots() { id = 5, pos = new Position((float)-1152.0396, (float)-204.94945, (float)37.182373), depositPos = new Position((float)-1139.5904541015625, (float)-199.96868896484375, (float)37.96001052856445), depositColshape = Alt.CreateColShapeSphere(new Position((float)-1139.5904541015625, (float)-199.96868896484375, (float)37.96001052856445), 2f) }); //Crastenburg Hotel
            MinijobSpots_.Add(new Minijob_Spots() { id = 6, pos = new Position((float)-199.8989, (float)-1381.1736, (float)30.476196), depositPos = new Position((float)-229.72915649414062, (float)-1377.137939453125, (float)31.25824737548828), depositColshape = Alt.CreateColShapeSphere(new Position((float)-229.72915649414062, (float)-1377.137939453125, (float)31.25824737548828), 2f) }); //Glass Heroes
            Alt.Log("Minijob: Elektrolieferant geladen...");

            startJobShape.Radius = 2f;

            foreach(var item in MinijobSpots_)
            {
                ((ClassicColshape)item.depositColshape).Radius = 2f;
            }
        }

        private static void PlayerDisconnectedHandler(IPlayer player, string reason)
        {
            try
            {
                if (player == null || !player.Exists) return;
                int charId = User.GetPlayerOnline(player);
                if (charId <= 0) return;
                foreach(var veh in Alt.GetAllVehicles().Where(x => x.NumberplateText == $"EL-{charId}").ToList())
                {
                    if (veh == null || !veh.Exists) continue;
                    ServerVehicles.RemoveVehiclePermanently(veh);
                    veh.Remove();
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        private static void PlayerEnterVehicle(IVehicle vehicle, IPlayer player, byte seat)
        {
            try
            {
                if (player == null || vehicle == null || !player.Exists || !vehicle.Exists) return;
                int charId = User.GetPlayerOnline(player);
                if (charId <= 0) return;
                if (ServerVehicles.GetVehicleType(vehicle) != 2) return;
                if (ServerVehicles.GetVehicleOwner(vehicle) != charId) return;
                if (player.GetPlayerCurrentMinijob() == "None") return;
                if (player.GetPlayerCurrentMinijobStep() == "None") return;
                if (player.GetPlayerCurrentMinijob() != "Elektrolieferant") return;
                if(player.GetPlayerCurrentMinijobStep() == "FirstStepInVehicle")
                {
                    player.SetPlayerCurrentMinijobStep("DELIVER_TO_DESTINATION");
                    player.SetPlayerCurrentMinijobActionCount(1);
                    HUDHandler.SendNotification(player, 1, 2500, "Fahre zum ersten Unternehmen um die Elektrozellen abzuliefern.");
                    player.EmitLocked("Client:Minijob:CreateJobMarker", "Diamond Casino", 3, 514, 30, MinijobSpots_[0].pos.X, MinijobSpots_[0].pos.Y, MinijobSpots_[0].pos.Z, true);
                    return;
                }
                else if(player.GetPlayerCurrentMinijobStep() == "DELIVER_TO_DESTINATION" && player.GetPlayerCurrentMinijobActionCount() == 2)
                {
                    HUDHandler.SendNotification(player, 1, 2500, "Fahre zum nächsten Unternehmen um die Elektrozellen abzuliefern.");
                    player.EmitLocked("Client:Minijob:CreateJobMarker", "Lifeinvader", 3, 514, 30, MinijobSpots_[1].pos.X, MinijobSpots_[1].pos.Y, MinijobSpots_[1].pos.Z, true);
                    return;
                }
                else if (player.GetPlayerCurrentMinijobStep() == "DELIVER_TO_DESTINATION" && player.GetPlayerCurrentMinijobActionCount() == 3)
                {
                    HUDHandler.SendNotification(player, 1, 2500, "Fahre zum nächsten Unternehmen um die Elektrozellen abzuliefern.");
                    player.EmitLocked("Client:Minijob:CreateJobMarker", "Weazel News", 3, 514, 30, MinijobSpots_[2].pos.X, MinijobSpots_[2].pos.Y, MinijobSpots_[2].pos.Z, true);
                    return;
                }
                else if (player.GetPlayerCurrentMinijobStep() == "DELIVER_TO_DESTINATION" && player.GetPlayerCurrentMinijobActionCount() == 4)
                {
                    HUDHandler.SendNotification(player, 1, 2500, "Fahre zum nächsten Unternehmen um die Elektrozellen abzuliefern.");
                    player.EmitLocked("Client:Minijob:CreateJobMarker", "Maze Bank", 3, 514, 30, MinijobSpots_[3].pos.X, MinijobSpots_[3].pos.Y, MinijobSpots_[3].pos.Z, true);
                    return;
                }
                else if (player.GetPlayerCurrentMinijobStep() == "DELIVER_TO_DESTINATION" && player.GetPlayerCurrentMinijobActionCount() == 5)
                {
                    HUDHandler.SendNotification(player, 1, 2500, "Fahre zum nächsten Unternehmen um die Elektrozellen abzuliefern.");
                    player.EmitLocked("Client:Minijob:CreateJobMarker", "Crastenburg Hotel", 3, 514, 30, MinijobSpots_[4].pos.X, MinijobSpots_[4].pos.Y, MinijobSpots_[4].pos.Z, true);
                    return;
                }
                else if (player.GetPlayerCurrentMinijobStep() == "DELIVER_TO_DESTINATION" && player.GetPlayerCurrentMinijobActionCount() == 6)
                {
                    HUDHandler.SendNotification(player, 1, 2500, "Fahre zum nächsten Unternehmen um die Elektrozellen abzuliefern.");
                    player.EmitLocked("Client:Minijob:CreateJobMarker", "Glass Heroes", 3, 514, 30, MinijobSpots_[5].pos.X, MinijobSpots_[5].pos.Y, MinijobSpots_[5].pos.Z, true);
                    return;
                }
                else if(player.GetPlayerCurrentMinijobStep() == "DRIVE_BACK_TO_START")
                {
                    HUDHandler.SendNotification(player, 1, 2500, "Fahre zurück zum Department of Water and Power und stelle dein Fahrzeug ab.");
                    player.EmitLocked("Client:Minijob:CreateJobMarker", "Department of Water and Power: Fahrzeugabgabe", 3, 514, 39, Constants.Positions.Minijob_Elektrolieferant_VehOutPos.X, Constants.Positions.Minijob_Elektrolieferant_VehOutPos.Y, Constants.Positions.Minijob_Elektrolieferant_VehOutPos.Z, true);
                    return;
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        private static async void PlayerExitVehicle(IVehicle vehicle, IPlayer player, byte seat)
        {
            try
            {
                if (player == null || vehicle == null || !player.Exists || !vehicle.Exists) return;
                int charId = User.GetPlayerOnline(player);
                if (charId <= 0) return;
                if (ServerVehicles.GetVehicleType(vehicle) != 2) return;
                if (ServerVehicles.GetVehicleOwner(vehicle) != charId) return;
                if (player.GetPlayerCurrentMinijob() == "None") return;
                if (player.GetPlayerCurrentMinijobStep() == "None") return;
                if (player.GetPlayerCurrentMinijob() == "Elektrolieferant" && player.GetPlayerCurrentMinijobStep() == "DELIVER_TO_DESTINATION")
                {
                    if (player.GetPlayerCurrentMinijobActionCount() == 1 && vehicle.Position.IsInRange(new Position(MinijobSpots_[0].pos.X, MinijobSpots_[0].pos.Y, MinijobSpots_[0].pos.Z), 5f))
                    {
                        player.EmitLocked("Client:Minijob:RemoveJobMarker");
                        player.EmitLocked("Client:Minijob:CreateJobMarker", "Diamond Casino", 3, 514, 1, MinijobSpots_[0].depositPos.X, MinijobSpots_[0].depositPos.Y, MinijobSpots_[0].depositPos.Z - 1, false);
                        HUDHandler.SendNotification(player, 1, 5000, "Begebe dich zur Tür und gebe das Paket ab.");
                        return;
                    }
                    else if (player.GetPlayerCurrentMinijobActionCount() == 2 && vehicle.Position.IsInRange(new Position(MinijobSpots_[1].pos.X, MinijobSpots_[1].pos.Y, MinijobSpots_[1].pos.Z), 5f)) {
                        player.EmitLocked("Client:Minijob:RemoveJobMarker");
                        player.EmitLocked("Client:Minijob:CreateJobMarker", "Lifeinvader", 3, 514, 1, MinijobSpots_[1].depositPos.X, MinijobSpots_[1].depositPos.Y, MinijobSpots_[1].depositPos.Z - 1, false);
                        HUDHandler.SendNotification(player, 1, 5000, "Begebe dich zur Tür und gebe das Paket ab.");
                        return;
                    }
                    else if (player.GetPlayerCurrentMinijobActionCount() == 3 && vehicle.Position.IsInRange(new Position(MinijobSpots_[2].pos.X, MinijobSpots_[2].pos.Y, MinijobSpots_[2].pos.Z), 5f))
                    {
                        player.EmitLocked("Client:Minijob:RemoveJobMarker");
                        player.EmitLocked("Client:Minijob:CreateJobMarker", "Weazel News", 3, 514, 1, MinijobSpots_[2].depositPos.X, MinijobSpots_[2].depositPos.Y, MinijobSpots_[2].depositPos.Z - 1, false);
                        HUDHandler.SendNotification(player, 1, 5000, "Begebe dich zur Tür und gebe das Paket ab.");
                        return;
                    }
                    else if (player.GetPlayerCurrentMinijobActionCount() == 4 && vehicle.Position.IsInRange(new Position(MinijobSpots_[3].pos.X, MinijobSpots_[3].pos.Y, MinijobSpots_[3].pos.Z), 5f))
                    {
                        player.EmitLocked("Client:Minijob:RemoveJobMarker");
                        player.EmitLocked("Client:Minijob:CreateJobMarker", "Maze Bank", 3, 514, 1, MinijobSpots_[3].depositPos.X, MinijobSpots_[3].depositPos.Y, MinijobSpots_[3].depositPos.Z - 1, false);
                        HUDHandler.SendNotification(player, 1, 5000, "Begebe dich zur Tür und gebe das Paket ab.");
                        return;
                    }
                    else if (player.GetPlayerCurrentMinijobActionCount() == 5 && vehicle.Position.IsInRange(new Position(MinijobSpots_[4].pos.X, MinijobSpots_[4].pos.Y, MinijobSpots_[4].pos.Z), 5f))
                    {
                        player.EmitLocked("Client:Minijob:RemoveJobMarker");
                        player.EmitLocked("Client:Minijob:CreateJobMarker", "Crastenburg Hotel", 3, 514, 1, MinijobSpots_[4].depositPos.X, MinijobSpots_[4].depositPos.Y, MinijobSpots_[4].depositPos.Z - 1, false);
                        HUDHandler.SendNotification(player, 1, 5000, "Begebe dich zur Tür und gebe das Paket ab.");
                        return;
                    }
                    else if (player.GetPlayerCurrentMinijobActionCount() == 6 && vehicle.Position.IsInRange(new Position(MinijobSpots_[5].pos.X, MinijobSpots_[5].pos.Y, MinijobSpots_[5].pos.Z), 5f))
                    {
                        player.EmitLocked("Client:Minijob:RemoveJobMarker");
                        player.EmitLocked("Client:Minijob:CreateJobMarker", "Glass Heroes", 3, 514, 1, MinijobSpots_[5].depositPos.X, MinijobSpots_[5].depositPos.Y, MinijobSpots_[5].depositPos.Z - 1, false);
                        HUDHandler.SendNotification(player, 1, 5000, "Begebe dich zur Tür und gebe das Paket ab.");
                        return;
                    }                    
                }
                else if (player.GetPlayerCurrentMinijob() == "Elektrolieferant" && player.GetPlayerCurrentMinijobStep() == "DRIVE_BACK_TO_START" && vehicle.Position.IsInRange(Constants.Positions.Minijob_Elektrolieferant_VehOutPos, 8f))
                {
                    player.EmitLocked("Client:Minijob:RemoveJobMarker");
                    foreach (var veh in Alt.GetAllVehicles().Where(x => x.NumberplateText == $"EL-{charId}").ToList()) {
                        if (veh == null || !veh.Exists) continue;
                        ServerVehicles.RemoveVehiclePermanently(veh);
                        veh.Remove(); 
                    }
                    player.SetPlayerCurrentMinijob("None");
                    player.SetPlayerCurrentMinijobStep("None");
                    player.SetPlayerCurrentMinijobActionCount(0);
                    int rnd = new Random().Next(500, 750);
                    if (!CharactersBank.HasCharacterBankMainKonto(charId)) { HUDHandler.SendNotification(player, 3, 5000, $"Dein Gehalt i.H.v. {rnd}$ konnte nicht überwiesen werden da du kein Hauptkonto hast."); return; }
                    int accNumber = CharactersBank.GetCharacterBankMainKonto(charId);
                    if (accNumber <= 0) return;
                    CharactersBank.SetBankAccountMoney(accNumber, CharactersBank.GetBankAccountMoney(accNumber) + rnd);
                    ServerBankPapers.CreateNewBankPaper(accNumber, DateTime.Now.ToString("d", CultureInfo.CreateSpecificCulture("de-DE")), DateTime.Now.ToString("t", CultureInfo.CreateSpecificCulture("de-DE")), "Eingehende Überweisung", "Department of Water and Power", "Minijob Gehalt", $"+{rnd}$", "Online Banking");
                    HUDHandler.SendNotification(player, 2, 5000, $"Du hast den Minijob erfolgreich abgeschlossen. Dein Gehalt i.H.v. {rnd}$ wurde dir auf dein Hauptkonto überwiesen.");
                    return;
                }
            }
            catch(Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        private static void ColshapeEnterExitHandler(IColShape colShape, IEntity targetEntity, bool state)
        {
            try
            {
                if (colShape == null) return;
                if (!colShape.Exists) return;
                IPlayer client = targetEntity as IPlayer;
                if (client == null || !client.Exists) return;
                if (colShape == startJobShape && state)
                {
                    if (client.GetPlayerCurrentMinijob() == "Elektrolieferant") { HUDHandler.SendNotification(client, 1, 2500, "Drücke E um den Elektrolieferanten Beruf der Firma 'Department of Water and Power' zu beenden."); }
                    else if (client.GetPlayerCurrentMinijob() == "None") { HUDHandler.SendNotification(client, 1, 2500, "Drücke E um den Elektrolieferanten Beruf der Firma 'Department of Water and Power' zu starten."); }
                    else if (client.GetPlayerCurrentMinijob() != "None") { HUDHandler.SendNotification(client, 3, 25000, "Du bist bereits in einem Minijob."); }
                    return;
                }

                if (client.GetPlayerCurrentMinijob() != "Elektrolieferant") return;
                if (colShape == MinijobSpots_[0].depositColshape && state && !client.IsInVehicle)
                {
                    if (client.GetPlayerCurrentMinijobStep() != "DELIVER_TO_DESTINATION" && client.GetPlayerCurrentMinijobActionCount() != 1) return;
                    client.EmitLocked("Client:Minijob:RemoveJobMarker");
                    client.SetPlayerCurrentMinijobActionCount(2);
                    HUDHandler.SendNotification(client, 2, 2500, "Erfolgreich abgegeben, steige wieder in dein Fahrzeug.");
                    return;
                }
                else if (colShape == MinijobSpots_[1].depositColshape && state && !client.IsInVehicle)
                {
                    if (client.GetPlayerCurrentMinijobStep() != "DELIVER_TO_DESTINATION" && client.GetPlayerCurrentMinijobActionCount() != 2) return;
                    client.EmitLocked("Client:Minijob:RemoveJobMarker");
                    client.SetPlayerCurrentMinijobActionCount(3);
                    HUDHandler.SendNotification(client, 2, 2500, "Erfolgreich abgegeben, steige wieder in dein Fahrzeug.");
                    return;
                }
                else if (colShape == MinijobSpots_[2].depositColshape && state && !client.IsInVehicle)
                {
                    if (client.GetPlayerCurrentMinijobStep() != "DELIVER_TO_DESTINATION" && client.GetPlayerCurrentMinijobActionCount() != 3) return;
                    client.EmitLocked("Client:Minijob:RemoveJobMarker");
                    client.SetPlayerCurrentMinijobActionCount(4);
                    HUDHandler.SendNotification(client, 2, 2500, "Erfolgreich abgegeben, steige wieder in dein Fahrzeug.");
                    return;
                }
                else if (colShape == MinijobSpots_[3].depositColshape && state && !client.IsInVehicle)
                {
                    if (client.GetPlayerCurrentMinijobStep() != "DELIVER_TO_DESTINATION" && client.GetPlayerCurrentMinijobActionCount() != 4) return;
                    client.EmitLocked("Client:Minijob:RemoveJobMarker");
                    client.SetPlayerCurrentMinijobActionCount(5);
                    HUDHandler.SendNotification(client, 2, 2500, "Erfolgreich abgegeben, steige wieder in dein Fahrzeug.");
                    return;
                }
                else if (colShape == MinijobSpots_[4].depositColshape && state && !client.IsInVehicle)
                {
                    if (client.GetPlayerCurrentMinijobStep() != "DELIVER_TO_DESTINATION" && client.GetPlayerCurrentMinijobActionCount() != 5) return;
                    client.EmitLocked("Client:Minijob:RemoveJobMarker");
                    client.SetPlayerCurrentMinijobActionCount(6);
                    HUDHandler.SendNotification(client, 2, 2500, "Erfolgreich abgegeben, steige wieder in dein Fahrzeug.");
                    return;
                }
                else if (colShape == MinijobSpots_[5].depositColshape && state && !client.IsInVehicle)
                {
                    if (client.GetPlayerCurrentMinijobStep() != "DELIVER_TO_DESTINATION" && client.GetPlayerCurrentMinijobActionCount() != 6) return;
                    client.EmitLocked("Client:Minijob:RemoveJobMarker");
                    client.SetPlayerCurrentMinijobStep("DRIVE_BACK_TO_START");
                    client.SetPlayerCurrentMinijobActionCount(0);
                    HUDHandler.SendNotification(client, 2, 2500, "Erfolgreich abgegeben, steige wieder in dein Fahrzeug.");
                    return;
                }
            }
            catch(Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        internal static void StartMinijob(IPlayer player)
        {
            try
            {
                if (player == null || !player.Exists || !((ClassicColshape)startJobShape).IsInRange((ClassicPlayer)player)) return;
                int charId = User.GetPlayerOnline(player);
                if (charId <= 0) return;
                if(player.GetPlayerCurrentMinijob() == "Elektrolieferant")
                {
                    //Job abbrechen
                    foreach(var veh in Alt.GetAllVehicles().Where(x => x.NumberplateText == $"EL-{charId}").ToList()) {
                        if (veh == null || !veh.Exists) continue;
                        ServerVehicles.RemoveVehiclePermanently(veh);
                        veh.Remove(); 
                    }
                    HUDHandler.SendNotification(player, 2, 1500, "Du hast den Minijob: Elektrolieferant beendet.");
                    player.SetPlayerCurrentMinijob("None");
                    player.SetPlayerCurrentMinijobStep("None");
                    player.SetPlayerCurrentMinijobActionCount(0);
                    player.SetPlayerCurrentMinijobRouteId(0);
                    return;
                }
                else if(player.GetPlayerCurrentMinijob() == "None")
                {
                    //Job annehmen
                    foreach(var veh in Alt.GetAllVehicles().ToList()) {
                        if (veh == null || !veh.Exists) continue;
                        if(veh.Position.IsInRange(Constants.Positions.Minijob_Elektrolieferant_VehOutPos, 5f)) { HUDHandler.SendNotification(player, 3, 5000, "Der Ausparkpunkt links vom Haupteingang ist blockiert."); return; }
                    }

                    ServerVehicles.CreateVehicle(2307837162, charId, 2, 0, false, 0, Constants.Positions.Minijob_Elektrolieferant_VehOutPos, Constants.Positions.Minijob_Elektrolieferant_VehOutRot, $"EL-{charId}", 255, 255, 255);
                    player.SetPlayerCurrentMinijob("Elektrolieferant");
                    player.SetPlayerCurrentMinijobStep("FirstStepInVehicle");
                    HUDHandler.SendNotification(player, 1, 2500, "Du hast den Minijob begonnen. Wir haben dir ein Fahrzeug mit Energiezellen links vom Haupteingang abgestellt.");
                    return;
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }
    }
}
