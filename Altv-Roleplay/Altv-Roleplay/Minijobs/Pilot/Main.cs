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

namespace Altv_Roleplay.Minijobs.Pilot
{
    public class Main : IScript
    {
        public partial class Minijob_Spots
        {
            public int id { get; set; }
            public IColShape depositShape { get; set; }
        }
        public static List<Minijob_Spots> MinijobSpots_ = new List<Minijob_Spots>();
        public static ClassicColshape startJobShape = (ClassicColshape)Alt.CreateColShapeSphere(Constants.Positions.Minijob_Pilot_StartPos, 2f);

        public static void Initialize()
        {
            Alt.Log("Lade Minijob: Pilot...");
            Alt.OnColShape += ColShapeEnterExitHandler;
            Alt.OnPlayerEnterVehicle += PlayerEnterVehicle;
            Alt.OnPlayerLeaveVehicle += PlayerExitVehicle;
            Alt.OnPlayerDisconnect += PlayerDisconnectedHandler;

            var data = new Server_Peds { model = "s_m_m_pilot_01", posX = startJobShape.Position.X, posY = startJobShape.Position.Y, posZ = startJobShape.Position.Z - 1, rotation =  -24f};
            ServerPeds.ServerPeds_.Add(data);
            var markerData = new Server_Markers { type = 33, posX = Constants.Positions.Minijob_Pilot_VehOutPos.X, posY = Constants.Positions.Minijob_Pilot_VehOutPos.Y, posZ = Constants.Positions.Minijob_Pilot_VehOutPos.Z + 0.25f, scaleX = 1, scaleY = 1, scaleZ = 1, red = 46, green = 133, blue = 232, alpha = 150, bobUpAndDown = true };
            ServerBlips.ServerMarkers_.Add(markerData);

            startJobShape.Radius = 2f;

            MinijobSpots_.Add(new Minijob_Spots() {id = 1, depositShape = Alt.CreateColShapeSphere(new Position((float)-991.3714, (float)-3147.745, (float)14.873291), 2.5f) }); //International Airport
            MinijobSpots_.Add(new Minijob_Spots() {id = 2, depositShape = Alt.CreateColShapeSphere(new Position((float)2011.5601806640625, (float)4743.40966796875, (float)41.199241638183594), 2.5f) }); //Grapeseed
            MinijobSpots_.Add(new Minijob_Spots() {id = 3, depositShape = Alt.CreateColShapeSphere(new Position((float)1303.2792, (float)3075.178, (float)41.37805), 2.5f) }); //Sandy Shores
            Alt.Log("Minijob: Pilot geladen...");

            foreach(var item in MinijobSpots_)
            {
                ((ClassicColshape)item.depositShape).Radius = 2.5f;
            }
        }

        private static void PlayerDisconnectedHandler(IPlayer player, string reason)
        {
            try
            {
                if (player == null || !player.Exists) return;
                int charId = User.GetPlayerOnline(player);
                if (charId <= 0) return;
                foreach (var veh in Alt.GetAllVehicles().Where(x => x.NumberplateText == $"PL-{charId}").ToList())
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

        private static void PlayerExitVehicle(IVehicle vehicle, IPlayer player, byte seat)
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
                if (player.GetPlayerCurrentMinijob() != "Pilot") return;
                if (player.GetPlayerCurrentMinijobStep() == "DRIVE_BACK_TO_START" && vehicle.Position.IsInRange(Constants.Positions.Minijob_Pilot_VehOutPos, 10f))
                {
                    var model = vehicle.Model;
                    foreach (var veh in Alt.GetAllVehicles().Where(x => x.NumberplateText == $"PL-{charId}").ToList()) {
                        if (veh == null || !veh.Exists) continue;
                        ServerVehicles.RemoveVehiclePermanently(veh);
                        veh.Remove(); 
                    }
                    player.SetPlayerCurrentMinijob("None");
                    player.SetPlayerCurrentMinijobRouteId(0);
                    player.SetPlayerCurrentMinijobStep("None");
                    player.SetPlayerCurrentMinijobActionCount(0);
                    int rnd = 0;
                    int rndExp = 0;
                    switch(model)
                    {
                        case 2621610858: //Velum
                            rnd = new Random().Next(250, 500);
                            rndExp = new Random().Next(1, 5);
                            break;
                        case 1341619767: //Vestra
                            rnd = new Random().Next(450, 650);
                            rndExp = new Random().Next(3, 8);
                            break;
                        case 2999939664: //Nimbus
                            rnd = new Random().Next(550, 850);
                            rndExp = new Random().Next(5, 11);
                            break;
                    }
                    if (!CharactersBank.HasCharacterBankMainKonto(charId)) { HUDHandler.SendNotification(player, 3, 5000, $"Dein Gehalt i.H.v. {rnd}$ konnte nicht überwiesen werden da du kein Hauptkonto hast."); return; }
                    int accNumber = CharactersBank.GetCharacterBankMainKonto(charId);
                    if (accNumber <= 0) return;
                    CharactersMinijobs.IncreaseCharacterMinijobEXP(charId, "Pilot", rndExp);
                    CharactersBank.SetBankAccountMoney(accNumber, CharactersBank.GetBankAccountMoney(accNumber) + rnd);
                    ServerBankPapers.CreateNewBankPaper(accNumber, DateTime.Now.ToString("d", CultureInfo.CreateSpecificCulture("de-DE")), DateTime.Now.ToString("t", CultureInfo.CreateSpecificCulture("de-DE")), "Eingehende Überweisung", "San Andreas Flights", "Minijob Gehalt", $"+{rnd}$", "Online Banking");
                    HUDHandler.SendNotification(player, 2, 5000, $"Du hast den Minijob erfolgreich abgeschlossen. Dein Gehalt i.H.v. {rnd}$ wurde dir auf dein Hauptkonto überwiesen. Du hast {rndExp}EXP dazu bekommen.");
                    player.EmitLocked("Client:Minijob:RemoveJobMarker");
                    return;
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
                if (player.GetPlayerCurrentMinijob() != "Pilot") return;
                if(player.GetPlayerCurrentMinijobStep() == "FirstStepInVehicle")
                {
                    player.SetPlayerCurrentMinijobStep("DRIVE_TO_DESTINATION");
                    player.SetPlayerCurrentMinijobActionCount(1);
                    HUDHandler.SendNotification(player, 1, 2500, "Fahre zum ersten Punkt und warte dort 15 Sekunden.");
                    player.EmitLocked("Client:Minijob:CreateJobMarker", "Pilot: Checkpoint", 3, 514, 30, MinijobSpots_[0].depositShape.Position.X, MinijobSpots_[0].depositShape.Position.Y, MinijobSpots_[0].depositShape.Position.Z, true);
                    return;
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        private static void ColShapeEnterExitHandler(IColShape colShape, IEntity targetEntity, bool state)
        {
            try
            {
                if (colShape == null) return;
                if (!colShape.Exists) return;
                IPlayer client = targetEntity as IPlayer;
                if (client == null || !client.Exists) return;
                if(colShape == startJobShape && state)
                {
                    if(client.GetPlayerCurrentMinijob() == "Pilot") { HUDHandler.SendNotification(client, 1, 2500, "Drücke E um den Piloten Minijob zu beenden."); }
                    else if(client.GetPlayerCurrentMinijob() == "None") { HUDHandler.SendNotification(client, 1, 2500, "Drücke E um den Piloten Minijob zu starten."); }
                    else if(client.GetPlayerCurrentMinijob() != "None") { HUDHandler.SendNotification(client, 3, 25000, "Du bist bereits in einem Minijob."); }
                    return;
                }

                if (client.GetPlayerCurrentMinijob() != "Pilot") return;
                if(colShape == MinijobSpots_[0].depositShape && state && client.IsInVehicle)
                {
                    if (client.GetPlayerCurrentMinijobStep() != "DRIVE_TO_DESTINATION" || client.GetPlayerCurrentMinijobActionCount() != 1) return;
                    client.SetPlayerCurrentMinijobActionCount(2);
                    client.EmitLocked("Client:Minijob:RemoveJobMarkerWithFreeze", 15000);
                    HUDHandler.SendNotification(client, 2, 15000, "Am Checkpoint angekommen, warte 15 Sekunden - fliege anschließend zum nächsten Flughafen.");
                    client.EmitLocked("Client:Minijob:CreateJobMarker", "Grapeseed Airport", 3, 514, 30, MinijobSpots_[1].depositShape.Position.X, MinijobSpots_[1].depositShape.Position.Y, MinijobSpots_[1].depositShape.Position.Z, true);
                    return;
                }
                else if(colShape == MinijobSpots_[1].depositShape && state && client.IsInVehicle)
                {
                    if (client.GetPlayerCurrentMinijobStep() != "DRIVE_TO_DESTINATION" || client.GetPlayerCurrentMinijobActionCount() != 2) return;
                    client.EmitLocked("Client:Minijob:RemoveJobMarkerWithFreeze", 15000);
                    client.SetPlayerCurrentMinijobActionCount(3);
                    HUDHandler.SendNotification(client, 2, 15000, "Am Checkpoint angekommen, warte 15 Sekunden - fliege anschließend zum nächsten Flughafen.");
                    client.EmitLocked("Client:Minijob:CreateJobMarker", "Sandy Shores Airport", 3, 514, 30, MinijobSpots_[2].depositShape.Position.X, MinijobSpots_[2].depositShape.Position.Y, MinijobSpots_[2].depositShape.Position.Z, true);
                    return;
                }
                else if(colShape == MinijobSpots_[2].depositShape && state && client.IsInVehicle)
                {
                    if (client.GetPlayerCurrentMinijobStep() != "DRIVE_TO_DESTINATION" || client.GetPlayerCurrentMinijobActionCount() != 3) return;
                    client.EmitLocked("Client:Minijob:RemoveJobMarkerWithFreeze", 15000);
                    HUDHandler.SendNotification(client, 2, 15000, "Am Checkpoint angekommen, warte 15 Sekunden - fliege anschließend zurück zum International Airport und gebe dein Flugzeug ab.");
                    client.SetPlayerCurrentMinijobActionCount(0);
                    client.SetPlayerCurrentMinijobStep("DRIVE_BACK_TO_START");
                    client.EmitLocked("Client:Minijob:CreateJobMarker", "Pilot: Flugzeugabgabe", 3, 514, 30, Constants.Positions.Minijob_Pilot_VehOutPos.X, Constants.Positions.Minijob_Pilot_VehOutPos.Y, Constants.Positions.Minijob_Pilot_VehOutPos.Z, true);
                    return;
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [AsyncClientEvent("Server:MinijobPilot:StartJob")]
        public async Task StartMiniJob(IPlayer player, int level)
        {
            try
            {
                if (player == null || !player.Exists || level <= 0) return;
                int charId = User.GetPlayerOnline(player);
                if (charId <= 0) return;
                if (player.GetPlayerCurrentMinijob() != "None") return;                
                foreach(var veh in Alt.GetAllVehicles().ToList())
                {
                    if (veh == null || !veh.Exists) continue;
                    if(veh.Position.IsInRange(Constants.Positions.Minijob_Pilot_VehOutPos, 8f)) { HUDHandler.SendNotification(player, 3, 5000, "Der Hangar ist blockiert."); return; }
                }
                switch(level)
                {
                    case 1:
                        ServerVehicles.CreateVehicle(2621610858, charId, 2, 0, false, 0, Constants.Positions.Minijob_Pilot_VehOutPos, Constants.Positions.Minijob_Pilot_VehOutRot, $"PL-{charId}", 255, 255, 255);
                        break;
                    case 2:
                        if(CharactersMinijobs.GetCharacterMinijobEXP(charId, "Pilot") < 50) { HUDHandler.SendNotification(player, 3, 5000, $"Du hast nicht die nötigen EXP für diese Stufe (50 EXP - du hast {CharactersMinijobs.GetCharacterMinijobEXP(charId, "Pilot")}EXP)."); return; }
                        ServerVehicles.CreateVehicle(1341619767, charId, 2, 0, false, 0, Constants.Positions.Minijob_Pilot_VehOutPos, Constants.Positions.Minijob_Pilot_VehOutRot, $"PL-{charId}", 255, 255, 255);
                        break;
                    case 3:
                        if (CharactersMinijobs.GetCharacterMinijobEXP(charId, "Pilot") < 100) { HUDHandler.SendNotification(player, 3, 5000, $"Du hast nicht die nötigen EXP für diese Stufe (100 EXP - du hast {CharactersMinijobs.GetCharacterMinijobEXP(charId, "Pilot")}EXP)."); return; }
                        ServerVehicles.CreateVehicle(2999939664, charId, 2, 0, false, 0, Constants.Positions.Minijob_Pilot_VehOutPos, Constants.Positions.Minijob_Pilot_VehOutRot, $"PL-{charId}", 255, 255, 255);
                        break;
                }
                player.SetPlayerCurrentMinijob("Pilot");
                player.SetPlayerCurrentMinijobStep("FirstStepInVehicle");
                player.SetPlayerCurrentMinijobActionCount(0);
                player.EmitLocked("Client:Minijob:RemoveJobMarker");
                HUDHandler.SendNotification(player, 1, 2500, "Du hast den Minijob begonnen. Wir haben dir ein Flugzeug im Hangar abgestellt, steige ein.");
                return;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        internal static void TryStartMinijob(IPlayer player)
        {
            try
            {
                if (player == null || !player.Exists || !((ClassicColshape)startJobShape).IsInRange((ClassicPlayer)player)) return;
                int charId = User.GetPlayerOnline(player);
                if (charId <= 0) return;
                if(player.GetPlayerCurrentMinijob() == "Pilot")
                {
                    //Job abbrechen
                    foreach(var veh in Alt.GetAllVehicles().Where(x => x.NumberplateText == $"PL-{charId}").ToList())
                    {
                        if (veh == null || !veh.Exists) continue;
                        ServerVehicles.RemoveVehiclePermanently(veh);
                        veh.Remove();
                    }
                    HUDHandler.SendNotification(player, 2, 1500, "Du hast den Minijob: Pilot beendet.");
                    player.SetPlayerCurrentMinijob("None");
                    player.SetPlayerCurrentMinijobRouteId(0);
                    player.SetPlayerCurrentMinijobStep("None");
                    player.SetPlayerCurrentMinijobActionCount(0);
                    return;
                }
                else if(player.GetPlayerCurrentMinijob() == "None")
                {
                    //Levelauswahl anzeigen
                    if(!CharactersMinijobs.ExistCharacterMinijobEntry(charId, "Pilot"))
                    {
                        CharactersMinijobs.CreateCharacterMinijobEntry(charId, "Pilot");
                    }
                    player.EmitLocked("Client:MinijobPilot:openCEF");
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
