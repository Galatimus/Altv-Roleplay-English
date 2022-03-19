using AltV.Net;
using AltV.Net.Async;
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

namespace Altv_Roleplay.Minijobs.Busfahrer
{
    public class Main : IScript
    {
        public static ClassicColshape startJobShape = (ClassicColshape)Alt.CreateColShapeSphere(Constants.Positions.Minijob_Busdriver_StartPos, 2f);

        public static void Initialize()
        {
            Alt.Log("Lade Minijob: Busfahrer...");
            Alt.OnColShape += ColshapeEnterExitHandler;
            Alt.OnPlayerEnterVehicle += PlayerEnterVehicle;
            Alt.OnPlayerLeaveVehicle += PlayerExitVehicle;

            var data = new Server_Peds { model = "cs_tom", posX = startJobShape.Position.X, posY = startJobShape.Position.Y, posZ = startJobShape.Position.Z - 1, rotation = -106.16410064697266f };
            ServerPeds.ServerPeds_.Add(data);
            var markerData = new Server_Markers { type = 39, posX = Constants.Positions.Minijob_Busdriver_VehOutPos.X, posY = Constants.Positions.Minijob_Busdriver_VehOutPos.Y, posZ = Constants.Positions.Minijob_Busdriver_VehOutPos.Z + 1, alpha = 150, bobUpAndDown = true, scaleX = 1, scaleY = 1, scaleZ = 1, red = 46, green = 133, blue = 232 };
            ServerBlips.ServerMarkers_.Add(markerData);
            Alt.Log("Minijob: Busfahrer geladen...");

            startJobShape.Radius = 2f;
        }

        private static async void PlayerExitVehicle(IVehicle vehicle, IPlayer player, byte seat)
        {
            try
            {
                if (player == null || !player.Exists) return;
                if (vehicle == null || !vehicle.Exists) return;
                int charId = User.GetPlayerOnline(player);
                if (charId <= 0) return;
                if (ServerVehicles.GetVehicleType(vehicle) != 2) return;
                if (ServerVehicles.GetVehicleOwner(vehicle) != charId) return;
                if (player.GetPlayerCurrentMinijob() != "Busfahrer") return;
                if (player.GetPlayerCurrentMinijobStep() != "DRIVE_BACK_TO_START") return;
                if (!vehicle.Position.IsInRange(Constants.Positions.Minijob_Busdriver_VehOutPos, 8f)) return;
                player.EmitLocked("Client:Minijob:RemoveJobMarker");
                foreach(var veh in Alt.GetAllVehicles().Where(x => x.NumberplateText == $"BUS-{charId}").ToList())
                {
                    if (veh == null || !veh.Exists) continue;
                    ServerVehicles.RemoveVehiclePermanently(veh);
                    await Task.Delay(5000);
                    veh.Remove();
                }
                int givenEXP = Model.GetRouteGivenEXP((int)player.GetPlayerCurrentMinijobRouteId());
                int givenMoney = Model.GetRouteGivenMoney((int)player.GetPlayerCurrentMinijobRouteId());
                player.SetPlayerCurrentMinijob("None");
                player.SetPlayerCurrentMinijobStep("None");
                player.SetPlayerCurrentMinijobActionCount(0);
                player.SetPlayerCurrentMinijobRouteId(0);
                CharactersMinijobs.IncreaseCharacterMinijobEXP(charId, "Busfahrer", givenEXP);
                if (!CharactersBank.HasCharacterBankMainKonto(charId)) { HUDHandler.SendNotification(player, 3, 5000, $"Dein Gehalt i.H.v. {givenMoney}$ konnte nicht überwiesen werden da du kein Hauptkonto hast. Du hast aber {givenEXP}EXP erhalten (du hast nun: {CharactersMinijobs.GetCharacterMinijobEXP(charId, "Busfahrer")}EXP)."); return; }
                int accNumber = CharactersBank.GetCharacterBankMainKonto(charId);
                if (accNumber <= 0) return;
                CharactersBank.SetBankAccountMoney(accNumber, CharactersBank.GetBankAccountMoney(accNumber) + givenMoney);
                ServerBankPapers.CreateNewBankPaper(accNumber, DateTime.Now.ToString("d", CultureInfo.CreateSpecificCulture("de-DE")), DateTime.Now.ToString("t", CultureInfo.CreateSpecificCulture("de-DE")), "Eingehende Überweisung", "Los Santos Transit", "Minijob Gehalt", $"+{givenMoney}$", "Online Banking");
                HUDHandler.SendNotification(player, 2, 5000, $"Du hast den Minijob erfolgreich abgeschlossen. Dein Gehalt i.H.v. {givenMoney}$ wurde dir auf dein Hauptkonto überwiesen. Du hast {givenEXP} erhalten (deine EXP: {CharactersMinijobs.GetCharacterMinijobEXP(charId, "Busfahrer")})");
                return;
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
                if (player.GetPlayerCurrentMinijob() != "Busfahrer") return;
                if (player.GetPlayerCurrentMinijobStep() == "None") return;
                if(player.GetPlayerCurrentMinijobStep() == "FirstStepInVehicle")
                {
                    var spot = Model.GetCharacterMinijobNextSpot(player);
                    if (spot == null) return; 
                    HUDHandler.SendNotification(player, 1, 25000, "Fahre zur ersten Haltestelle und warte dort 10 Sekunden.");
                    player.SetPlayerCurrentMinijobStep("DRIVE_TO_NEXT_STATION");
                    player.EmitLocked("Client:Minijob:CreateJobMarker", "Minijob: Haltestelle", 3, 80, 30, spot.posX, spot.posY, spot.posZ, false);
                    return;
                }
            }
            catch (Exception e)
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
                int charId = User.GetPlayerOnline(client);
                if (charId <= 0) return;
                if (colShape == startJobShape && state)
                {
                    if (client.GetPlayerCurrentMinijob() == "Busfahrer") { HUDHandler.SendNotification(client, 1, 2500, "Drücke E um den Busfahrer Minijob zu beenden."); }
                    else if (client.GetPlayerCurrentMinijob() == "None") { HUDHandler.SendNotification(client, 1, 2500, "Drücke E um den Busfahrer Minijob zu starten."); }
                    else if (client.GetPlayerCurrentMinijob() != "None") { HUDHandler.SendNotification(client, 3, 25000, "Du bist bereits in einem Minijob."); }
                    return;
                }

                if (client.GetPlayerCurrentMinijob() != "Busfahrer") return;
                if (client.GetPlayerCurrentMinijobRouteId() <= 0) return;
                if (client.GetPlayerCurrentMinijobActionCount() <= 0) return;
                if (client.GetPlayerCurrentMinijobStep() == "DRIVE_TO_NEXT_STATION" && state && client.IsInVehicle)
                {
                    var spot = Model.GetCharacterMinijobNextSpot(client);
                    if (spot == null) return;
                    if (colShape != spot.destinationColshape) return;
                    client.EmitLocked("Client:Minijob:RemoveJobMarkerWithFreeze", 10000);
                    int maxSpots = Model.GetMinijobMaxRouteSpots((int)client.GetPlayerCurrentMinijobRouteId());
                    if((int)client.GetPlayerCurrentMinijobActionCount() < maxSpots)
                    {
                        //neuer Punkt
                        client.SetPlayerCurrentMinijobActionCount(client.GetPlayerCurrentMinijobActionCount() + 1);
                        var newSpot = Model.GetCharacterMinijobNextSpot(client);
                        if (newSpot == null) return;
                        client.SetPlayerCurrentMinijobStep("DRIVE_TO_NEXT_STATION");
                        client.EmitLocked("Client:Minijob:CreateJobMarker", "Minijob: Haltestelle", 3, 80, 30, newSpot.posX, newSpot.posY, newSpot.posZ, false);
                        HUDHandler.SendNotification(client, 2, 10000, "An Haltestelle angekommen, warte 10 Sekunden und fahre anschließend zur nächsten Haltestelle.");
                        Alt.Log($"Aktueller Spot || Route: {newSpot.routeId} || SpotID: {newSpot.spotId}");
                        return;
                    }
                    else if((int)client.GetPlayerCurrentMinijobActionCount() >= maxSpots)
                    {
                        //zurueck zum Depot
                        HUDHandler.SendNotification(client, 2, 10000, "An Haltestelle angekommen, warte 10 Sekunden und fahre den Bus anschließend zurück zum Depot und stelle ihn dort ab, wo du ihn bekommen hast.");
                        client.SetPlayerCurrentMinijobStep("DRIVE_BACK_TO_START");
                        client.EmitLocked("Client:Minijob:CreateJobMarker", "Minijob: Busabgabe", 3, 515, 30, Constants.Positions.Minijob_Busdriver_VehOutPos.X, Constants.Positions.Minijob_Busdriver_VehOutPos.Y, Constants.Positions.Minijob_Busdriver_VehOutPos.Z, false);
                        return;
                    }
                }
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
                if (player.GetPlayerCurrentMinijob() == "Busfahrer")
                {
                    //Job abbrechen
                    foreach (var veh in Alt.GetAllVehicles().Where(x => x.NumberplateText == $"BUS-{charId}").ToList())
                    {
                        if (veh == null || !veh.Exists) continue;
                        ServerVehicles.RemoveVehiclePermanently(veh);
                        veh.Remove();
                    }
                    HUDHandler.SendNotification(player, 2, 1500, "Du hast den Minijob: Busfahrer beendet.");
                    player.SetPlayerCurrentMinijob("None");
                    player.SetPlayerCurrentMinijobRouteId(0);
                    player.SetPlayerCurrentMinijobStep("None");
                    player.SetPlayerCurrentMinijobActionCount(0);
                    player.EmitLocked("Client:Minijob:RemoveJobMarker");
                    return;
                }
                else if (player.GetPlayerCurrentMinijob() == "None")
                {
                    //Levelauswahl anzeigen
                    if (!CharactersMinijobs.ExistCharacterMinijobEntry(charId, "Busfahrer"))
                    {
                        CharactersMinijobs.CreateCharacterMinijobEntry(charId, "Busfahrer");
                    }
                    var availableRoutes = Model.GetAvailableRoutes(charId);
                    if (availableRoutes == "[]") return;
                    player.EmitLocked("Client:MinijobBusdriver:openCEF", availableRoutes);
                    return;
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        [AsyncClientEvent("Server:MinijobBusdriver:StartJob")]
        public async Task StartMiniJob(IPlayer player, int routeId)
        {
            try
            {
                if (player == null || !player.Exists || routeId <= 0) return;
                int charId = User.GetPlayerOnline(player);
                if (charId <= 0) return;
                if (player.GetPlayerCurrentMinijob() != "None") return;
                if (!Model.ExistRoute(routeId)) return;
                if(CharactersMinijobs.GetCharacterMinijobEXP(charId, "Busfahrer") < Model.GetRouteNeededEXP(routeId)) { HUDHandler.SendNotification(player, 3, 5000, $"Du hast nicht die benötigen EXP für diese Linie ({Model.GetRouteNeededEXP(routeId)}EXP - du hast {CharactersMinijobs.GetCharacterMinijobEXP(charId, "Busfahrer")}EXP)."); return; }
                foreach (var veh in Alt.GetAllVehicles().ToList())
                {
                    if (veh == null || !veh.Exists) continue;
                    if (veh.Position.IsInRange(Constants.Positions.Minijob_Busdriver_VehOutPos, 8f)) { HUDHandler.SendNotification(player, 3, 5000, "Der Ausparkpunkt ist blockiert."); return; }
                }
                ServerVehicles.CreateVehicle(Model.GetRouteVehicleHash(routeId), charId, 2, 0, false, 0, Constants.Positions.Minijob_Busdriver_VehOutPos, Constants.Positions.Minijob_Busdriver_VehOutRot, $"BUS-{charId}", 255, 255, 255); player.SetPlayerCurrentMinijob("Busfahrer");
                player.SetPlayerCurrentMinijobStep("FirstStepInVehicle");
                player.SetPlayerCurrentMinijobRouteId((ulong)routeId);
                player.SetPlayerCurrentMinijobActionCount(1);
                HUDHandler.SendNotification(player, 1, 2500, "Du hast den Minijob begonnen. Wir haben dir einen Bus am Tor ausgeparkt, steige ein.");
                return;
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }
    }
}
