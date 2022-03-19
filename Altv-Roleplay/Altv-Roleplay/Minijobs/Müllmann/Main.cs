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

namespace Altv_Roleplay.Minijobs.Müllmann
{
    public class Main
    {
        public static ClassicColshape startJobShape = (ClassicColshape)Alt.CreateColShapeSphere(Constants.Positions.Minijob_Müllmann_StartPos, 3f);
        
        public static void Initialize()
        {
            Alt.Log("Lade Minijob: Müllmann...");
            Alt.OnColShape += ColshapeEnterExitHandler;
            Alt.OnPlayerEnterVehicle += PlayerEnterVehicle;
            Alt.OnPlayerLeaveVehicle += PlayerExitVehicle;
            Alt.OnPlayerDisconnect += PlayerDisconnectedHandler;

            var data = new Server_Peds { model = "", posX = startJobShape.Position.X, posY = startJobShape.Position.Y, posZ = startJobShape.Position.Z, rotation = -0.9688125252723694f };
            ServerPeds.ServerPeds_.Add(data);
            var markerData = new Server_Markers { type = 33, posX = Constants.Positions.Minijob_Müllmann_VehOutPos.X, posY = Constants.Positions.Minijob_Müllmann_VehOutPos.Y, posZ = Constants.Positions.Minijob_Müllmann_VehOutPos.Z + 1, alpha = 150, bobUpAndDown = true, scaleX = 1, scaleY = 1, scaleZ = 1, red = 46, green = 133, blue = 232 };
            ServerBlips.ServerMarkers_.Add(markerData);
            Alt.Log("Minijob: Müllmann geladen...");

            startJobShape.Radius = 3f;
        }

        private static void PlayerDisconnectedHandler(IPlayer player, string reason)
        {
            try
            {
                if (player == null || !player.Exists) return;
                int charId = User.GetPlayerOnline(player);
                if (charId <= 0) return;
                foreach (var veh in Alt.GetAllVehicles().Where(x => x.NumberplateText == $"MM-{charId}").ToList())
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
                if (player.GetPlayerCurrentMinijob() != "Müllmann") return;
                if (player.GetPlayerCurrentMinijobStep() != "DRIVE_BACK_TO_START") return;
                if (!vehicle.Position.IsInRange(Constants.Positions.Minijob_Müllmann_VehOutPos, 8f)) return;
                player.EmitLocked("Client:Minijob:RemoveJobMarker");
                foreach(var veh in Alt.GetAllVehicles().Where(x => x.NumberplateText == $"MM-{charId}").ToList())
                {
                    if (veh == null || !veh.Exists) continue;
                    ServerVehicles.RemoveVehiclePermanently(veh);
                    await Task.Delay(5000);
                    veh.Remove();
                }
                player.SetPlayerCurrentMinijob("None");
                player.SetPlayerCurrentMinijobStep("None");
                player.SetPlayerCurrentMinijobActionCount(0);
                player.SetPlayerCurrentMinijobRouteId(0);
                int rnd = new Random().Next(500, 850);
                if (!CharactersBank.HasCharacterBankMainKonto(charId)) { HUDHandler.SendNotification(player, 3, 5000, $"Dein Gehalt i.H.v. {rnd}$ konnte nicht überwiesen werden da du kein Hauptkonto hast."); return; }
                int accNumber = CharactersBank.GetCharacterBankMainKonto(charId);
                if (accNumber <= 0) return;
                CharactersBank.SetBankAccountMoney(accNumber, CharactersBank.GetBankAccountMoney(accNumber) + rnd);
                ServerBankPapers.CreateNewBankPaper(accNumber, DateTime.Now.ToString("d", CultureInfo.CreateSpecificCulture("de-DE")), DateTime.Now.ToString("t", CultureInfo.CreateSpecificCulture("de-DE")), "Eingehende Überweisung", "Arbeitgeber: Müllmann", "Minijob Gehalt", $"+{rnd}$", "Online Banking");
                HUDHandler.SendNotification(player, 2, 5000, $"Du hast den Minijob erfolgreich abgeschlossen. Dein Gehalt i.H.v. {rnd}$ wurde dir auf dein Hauptkonto überwiesen.");
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
                if (player.GetPlayerCurrentMinijob() != "Müllmann") return;
                if (player.GetPlayerCurrentMinijobStep() == "None") return;
                if(player.GetPlayerCurrentMinijobStep() == "FirstStepInVehicle")
                {
                    HUDHandler.SendNotification(player, 1, 2500, "Fahre zum ersten Zielort um den Müll abzuholen, dieser wurde auf deinem GPS markiert.");
                    var spot = Model.GetCharacterMinijobNextSpot(player);
                    if (spot == null) return;
                    player.SetPlayerCurrentMinijobStep("PICKUP_TRASH");
                    player.EmitLocked("Client:Minijob:CreateJobMarker", "Minijob: Müll abholen", 3, 514, 1, spot.posX, spot.posY, spot.posZ, false);
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
                if(colShape == startJobShape && state)
                {
                    if (client.GetPlayerCurrentMinijob() == "Müllmann") { HUDHandler.SendNotification(client, 1, 2500, "Drücke E um den Müllmann Minijob zu beenden."); }
                    else if (client.GetPlayerCurrentMinijob() == "None") { HUDHandler.SendNotification(client, 1, 2500, "Drücke E um den Müllmann Minijob zu starten."); }
                    else if (client.GetPlayerCurrentMinijob() != "None") { HUDHandler.SendNotification(client, 3, 25000, "Du bist bereits in einem Minijob."); }
                    return;
                }

                if (client.GetPlayerCurrentMinijob() != "Müllmann") return;
                if (client.GetPlayerCurrentMinijobRouteId() <= 0) return;
                if (client.GetPlayerCurrentMinijobActionCount() <= 0) return;
                if(client.GetPlayerCurrentMinijobStep() == "PICKUP_TRASH" && state && !client.IsInVehicle)
                {
                    var spot = Model.GetCharacterMinijobNextSpot(client);
                    if (spot == null) return;
                    if (colShape != spot.destinationColshape) return;
                    var personalThrowCol = Alt.GetAllColShapes().Where(x => x.Exists && x != null).ToList().FirstOrDefault(x => x != null && x.Exists && x.GetColShapeName() == "GarbageMinijobThrowInVehicle" && x.GetColShapeId() == (ulong)charId);
                    if (personalThrowCol != null && personalThrowCol.Exists) personalThrowCol.Remove();
                    //ToDo: Objeklt in Hand geben
                    InventoryHandler.InventoryAnimation(client, "farmPickup", 1100);
                    var veh = Alt.GetAllVehicles().ToList().FirstOrDefault(x => x.Exists && x.NumberplateText == $"MM-{charId}");
                    if (veh == null || !veh.Exists) return;
                    HUDHandler.SendNotification(client, 1, 1200, "Mülltonne geleert, werfe den Müll in den Wagen.");
                    DegreeRotation vehRot = veh.Rotation;
                    Position MMThrowPos = veh.Position.getPositionInBackOfPosition(vehRot.Yaw, -5.5f);
                    client.EmitLocked("Client:Minijob:RemoveJobMarker");
                    client.SetPlayerCurrentMinijobStep("THROW_TRASH_IN_VEHICLE");
                    client.EmitLocked("Client:Minijob:CreateJobMarker", "Minijob: Müll einladen", 3, 514, 22, MMThrowPos.X, MMThrowPos.Y, MMThrowPos.Z, true);
                    ClassicColshape throwCol = (ClassicColshape)Alt.CreateColShapeSphere(MMThrowPos, 2.5f);
                    throwCol.SetColShapeName("GarbageMinijobThrowInVehicle");
                    throwCol.SetColShapeId((ulong)charId);
                    throwCol.Radius = 2.5f;
                    return;
                }
                else if(client.GetPlayerCurrentMinijobStep() == "THROW_TRASH_IN_VEHICLE" && state && !client.IsInVehicle)
                {
                    var personalThrowCol = Alt.GetAllColShapes().Where(x => x.Exists && x != null).ToList().FirstOrDefault(x => x != null && x.Exists && x.GetColShapeName() == "GarbageMinijobThrowInVehicle" && x.GetColShapeId() == (ulong)charId);
                    if (personalThrowCol == null || !personalThrowCol.Exists) return;
                    if (!((ClassicColshape)personalThrowCol).IsInRange((ClassicPlayer)client)) return;
                    client.EmitLocked("Client:Minijob:RemoveJobMarker");
                    //ToDo: Objekt aus Hand entfernen
                    InventoryHandler.InventoryAnimation(client, "farmPickup", 1100);
                    int maxSpots = Model.GetMinijobGarbageMaxRouteSpots((int)client.GetPlayerCurrentMinijobRouteId()); 
                    if ((int)client.GetPlayerCurrentMinijobActionCount() < maxSpots)
                    {
                        //neuer Punkt
                        client.SetPlayerCurrentMinijobActionCount(client.GetPlayerCurrentMinijobActionCount() + 1);
                        var spot = Model.GetCharacterMinijobNextSpot(client);
                        if (spot == null) return;
                        client.SetPlayerCurrentMinijobStep("PICKUP_TRASH");
                        client.EmitLocked("Client:Minijob:CreateJobMarker", "Minijob: Müll abholen", 3, 514, 1, spot.posX, spot.posY, spot.posZ, false);
                        HUDHandler.SendNotification(client, 1, 2500, "Fahre zum nächsten Zielort um den Müll abzuholen, dieser wurde auf deinem GPS markiert.");
                        Alt.Log($"Aktueller Spot || Route: {spot.routeId} || SpotID: {spot.spotId}");
                    } 
                    else if((int)client.GetPlayerCurrentMinijobActionCount() >= maxSpots)
                    {
                        //zurueck zum Depot
/*                        HUDHandler.SendNotification(client, 1, 2222, "VERSUCH: Colshape entfernen [003]");
*/                        if (personalThrowCol != null && personalThrowCol.Exists) personalThrowCol.Remove();
                        HUDHandler.SendNotification(client, 1, 6000, "Alles aufgesammelt mein Jung. Zurück zur Mülldeponie - das Zeug abgeben, stell das Fahrzeug einfach dort ab wo du es bekommen hast.");
                        client.SetPlayerCurrentMinijobStep("DRIVE_BACK_TO_START");
                        client.EmitLocked("Client:Minijob:CreateJobMarker", "Minijob: Fahrzeug abgeben", 3, 514, 30, Constants.Positions.Minijob_Müllmann_VehOutPos.X, Constants.Positions.Minijob_Müllmann_VehOutPos.Y, Constants.Positions.Minijob_Müllmann_VehOutPos.Z, false);
                    }
                    return;
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        internal static void StartMinijob(IPlayer player)
        {
            try
            {
                if (player == null || !player.Exists) return;
                if (!((ClassicColshape)startJobShape).IsInRange((ClassicPlayer)player)) return;
                int charId = User.GetPlayerOnline(player);
                if (charId <= 0) return;
                if(player.GetPlayerCurrentMinijob() == "Müllmann")
                {
                    //Job abbrechen
                    foreach(var veh in Alt.GetAllVehicles().Where(x => x.NumberplateText == $"MM-{charId}").ToList())
                    {
                        if (veh == null || !veh.Exists) continue;
                        ServerVehicles.RemoveVehiclePermanently(veh);
                        veh.Remove();
                    }
                    var personalThrowCol = Alt.GetAllColShapes().Where(x => x.Exists && x != null).ToList().FirstOrDefault(x => x.GetColShapeName() == "GarbageMinijobThrowInVehicle" && x.GetColShapeId() == (ulong)charId);
                    if (personalThrowCol != null && personalThrowCol.Exists) personalThrowCol.Remove();
                    HUDHandler.SendNotification(player, 2, 1500, "Du hast den Minijob: Müllmann beendet.");
                    player.SetPlayerCurrentMinijob("None");
                    player.SetPlayerCurrentMinijobStep("None");
                    player.SetPlayerCurrentMinijobRouteId(0);
                    player.SetPlayerCurrentMinijobActionCount(0);
                    player.EmitLocked("Client:Minijob:RemoveJobMarker");
                    return;
                }
                else if(player.GetPlayerCurrentMinijob() == "None")
                {
                    //Job annehmen
                    foreach(var veh in Alt.GetAllVehicles().ToList())
                    {
                        if (veh == null || !veh.Exists) continue;
                        if(veh.Position.IsInRange(Constants.Positions.Minijob_Müllmann_VehOutPos, 5f)) { HUDHandler.SendNotification(player, 3, 5000, "Der Ausparkpunkt ist belegt."); return; }
                    }
                    ServerVehicles.CreateVehicle(3039269212, charId, 2, 0, false, 0, Constants.Positions.Minijob_Müllmann_VehOutPos, Constants.Positions.Minijob_Müllmann_VehOutRot, $"MM-{charId}", 255, 255, 255);
                    var generatorId = new Random();
                    int routeId = generatorId.Next(1, Model.GetMinijobGarbageMaxRoutes());
                    player.SetPlayerCurrentMinijob("Müllmann");
                    player.SetPlayerCurrentMinijobStep("FirstStepInVehicle");
                    player.SetPlayerCurrentMinijobRouteId((ulong)routeId);
                    player.SetPlayerCurrentMinijobActionCount(1);
                    HUDHandler.SendNotification(player, 1, 2500, "Du hast den Minijob begonnen. Wir haben dir ein Fahrzeug zur Verfügung gestellt, steige in dies ein um zu beginnen.");
                    Alt.Log($"Max Routes: {Model.GetMinijobGarbageMaxRoutes()}"); //2
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
