using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using Altv_Roleplay.Model;
using Altv_Roleplay.models;
using Altv_Roleplay.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altv_Roleplay.Handler
{
    class GarageHandler : IScript
    {
        internal static void OpenGarageCEF(IPlayer player, int garageId)
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                if (player == null || !player.Exists || garageId == 0) return;
                int charId = User.GetPlayerOnline(player);
                if (charId <= 0) return;
                var garageInfo = ServerGarages.ServerGarages_.FirstOrDefault(x => x.id == garageId);
                var garageSlots = ServerGarages.ServerGarageSlots_.Where(x => x.garageId == garageId).ToList();
                if (garageInfo == null || !garageSlots.Any()) return;
                if (!player.Position.IsInRange(new Position(garageInfo.posX, garageInfo.posY, garageInfo.posZ), 2f)) return;
                var charFaction = ServerFactions.GetCharacterFactionId(charId);
                var factionCut = ServerFactions.GetFactionShortName(charFaction);
                bool charFactionDuty = ServerFactions.IsCharacterInFactionDuty(charId);
                var inString = "";
                var outString = "";
                var garageName = "";
                //0 Auto | 1 Boot | 2 Flugzeug | 3 Heli | 4 LKW
                if (garageInfo.type == 0) { garageName = $"Fahrzeuggarage: {garageInfo.name}"; }
                else if (garageInfo.type == 1) { garageName = $"Bootsgarage: {garageInfo.name}"; }
                else if (garageInfo.type == 2) { garageName = $"Flugzeuggarage: {garageInfo.name}"; }
                else if (garageInfo.type == 3) { garageName = $"Heligarage: {garageInfo.name}"; }
                else if (garageInfo.type == 4) { garageName = $"LkwGarage: {garageInfo.name}"; }

                if (garageInfo.name.Contains("Fraktion"))
                {
                    if (charFaction <= 0) { HUDHandler.SendNotification(player, 4, 5000, $"Keine Berechtigung."); return; }
                    var gFactionCut = garageInfo.name.Split(" ")[1]; //Fraktion LSPD Mission Row  <- Beispiel
                    if (gFactionCut != factionCut) { HUDHandler.SendNotification(player, 4, 5000, $"Keine Berechtigung [002]."); return; }
                    /*if(!charFactionDuty) { HUDHandler.SendNotification(player, 4, 5000, $"Keine Berechtigung (nicht im Dienst)."); return; }*/
                    //inString = GetGarageParkInString(player, garageSlots, charId, garageId, true, factionCut, charFaction);
                    //outString = GetGarageParkOutString(player, garageId, charId, true, factionCut);
                    inString = GetGarageParkInString(player, garageSlots, charId, garageId, false, "Zivilist", charFaction); //Array von Fahrzeugen die um die Garage rum zum Einparken stehen
                    outString = GetGarageParkOutString(player, garageId, charId, false, "Zivilist");
                    player.EmitLocked("Client:Garage:OpenGarage", garageId, garageName, inString, outString);
                    stopwatch.Stop();
                    if (stopwatch.Elapsed.Milliseconds > 30) Alt.Log($"{charId} - OpenGarageCEF benötigte {stopwatch.Elapsed.Milliseconds}ms");
                    return;
                }

                inString = GetGarageParkInString(player, garageSlots, charId, garageId, false, "Zivilist", charFaction); //Array von Fahrzeugen die um die Garage rum zum Einparken stehen
                outString = GetGarageParkOutString(player, garageId, charId, false, "Zivilist");
                Global.mGlobal.VirtualAPI.TriggerClientEventSafe(player, "Client:Garage:OpenGarage", garageId, garageName, inString, outString);


                stopwatch.Stop();
                if (stopwatch.Elapsed.Milliseconds > 30)  Alt.Log($"{charId} - OpenGarageCEF benötigte {stopwatch.Elapsed.Milliseconds}ms");
            }
            catch(Exception e)
            {
                Alt.Log($"{e}");
            }
        }

        public static string GetGarageParkInString(IPlayer player, IReadOnlyCollection<Server_Garage_Slots> garageSlots, int charId, int garageId, bool isFaction, string factionShort, int factionId)
        {
            if (player == null || !player.Exists || !garageSlots.Any() || garageId == 0 || charId == 0) return "undefined";
            List<IVehicle> vehicles = null;
            if (isFaction == false) { vehicles = Alt.GetAllVehicles().Where(x => x != null && x.Exists && x.HasVehicleId() && x.GetVehicleId() > 0 && x.Position.IsInRange(player.Position, 50f)).ToList(); } 
            else if(isFaction == true) { vehicles = Alt.GetAllVehicles().Where(x => x != null && x.Exists && x.HasVehicleId() && x.GetVehicleId() > 0 && x.Position.IsInRange(player.Position, 50f) && ServerVehicles.GetVehicleFactionId(x) == factionId && x.NumberplateText.Contains(factionShort)).ToList(); }
            int garageType = ServerGarages.GetGarageType(garageId);
            if (garageType == -1) return "undefined";
            dynamic array = new JArray() as dynamic;
            dynamic entry = new JObject();
            foreach(var veh in vehicles)
            {
                bool hasKey = false,
                    isOwner = ServerVehicles.GetVehicleOwner(veh) == charId;
                if (isFaction) hasKey = CharactersInventory.ExistCharacterItem(charId, $"Fahrzeugschluessel {factionShort}", "schluessel");
                else if (!isFaction) hasKey = CharactersInventory.ExistCharacterItem(charId, $"Fahrzeugschluessel {veh.NumberplateText}", "schluessel");
                if (!isOwner && !hasKey) continue;
                entry = new JObject();
                entry.vehid = veh.GetVehicleId();
                entry.plate = veh.NumberplateText;
                entry.hash = veh.Model;
                entry.name = ServerVehicles.GetVehicleNameOnHash(veh.Model);
                array.Add(entry);
            }

            //foreach (var slot in garageSlots.Where(x => x.garageId == garageId))
            //{
            //    var pos = new Position(slot.posX, slot.posY, slot.posZ);
            //    var entity = vehicles.OrderBy(x => x.Position.Distance(pos)).ToList().FirstOrDefault();
            //    if(entity == null) { continue; }
            //    var distance = entity.Position.Distance(pos);
            //    if (distance >= 1.5f) { continue; }
            //    var vehicle = ServerVehicles.ServerVehicles_.Where(x => x.isInGarage == false).ToList().FirstOrDefault(x => string.Equals(x.plate, entity.NumberplateText, StringComparison.CurrentCultureIgnoreCase));
            //    if(vehicle == null) { continue; }
            //    var vehicleData = ServerAllVehicles.ServerAllVehicles_.FirstOrDefault(x => x.hash == entity.Model);
            //    if (vehicleData == null) { continue; }
            //    if(vehicleData.vehClass != garageType) { continue; }
            //    bool hasKey = false;
            //    if (isFaction == false) { hasKey = CharactersInventory.ExistCharacterItem(charId, "Fahrzeugschluessel " + entity.NumberplateText, "inventory"); }
            //    else if(isFaction == true) { hasKey = CharactersInventory.ExistCharacterItem(charId, "Fahrzeugschluessel " + factionShort, "inventory"); }
            //    bool isOwner = vehicle.charid == charId;
            //    if(!hasKey && !isOwner) { continue; }

            //    entry = new JObject();
            //    entry.vehid = vehicle.id;
            //    entry.plate = entity.NumberplateText;
            //    entry.hash = entity.Model;
            //    entry.name = vehicleData.name;
            //    array.Add(entry);
            //}
            return array.ToString();
        }

        public static string GetGarageParkOutString(IPlayer player, int garageId, int charId, bool isFaction, string factionShort)
        {
            try
            {
                if (player == null || !player.Exists || garageId == 0 || charId == 0) return "undefined";
                List<Server_Vehicles> inGarageVehs = null;
                if (isFaction == false) { inGarageVehs = ServerVehicles.ServerVehicles_.Where(x => x.isInGarage == true && x.garageId == garageId).ToList(); }
                else if (isFaction == true) { inGarageVehs = ServerVehicles.ServerVehicles_.Where(x => x.isInGarage == true && x.garageId == garageId && x.plate.Contains(factionShort)).ToList(); }
                dynamic array = new JArray() as dynamic;
                dynamic entry = new JObject();

                foreach (var vehicle in inGarageVehs)
                {
                    bool hasKey = false;
                    if (isFaction == false) { hasKey = CharactersInventory.ExistCharacterItem(charId, "Fahrzeugschluessel " + vehicle.plate, "schluessel"); }
                    else if (isFaction == true) { hasKey = CharactersInventory.ExistCharacterItem(charId, "Fahrzeugschluessel " + factionShort, "schluessel"); }
                    bool isOwner = vehicle.charid == charId;
                    if (!hasKey && !isOwner) continue;

                    entry = new JObject();
                    entry.vehid = vehicle.id;
                    entry.plate = vehicle.plate;
                    entry.hash = vehicle.hash;
                    entry.name = ServerAllVehicles.GetVehicleNameOnHash(vehicle.hash);
                    array.Add(entry);
                }

                return array.ToString();
            }
            catch(Exception e)
            {
                Alt.Log($"{e}");
            }
            return "[]";
        }

        [AsyncClientEvent("Server:Garage:DoAction")]
        public async Task DoGarageAction(IPlayer player, int garageid, string action, int vehID)
        {
            try
            {
                if (player == null || !player.Exists || action == "" || vehID <= 0 || garageid <= 0) return;
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                int charId = User.GetPlayerOnline(player);
                if (charId <= 0) return;
                var vehicle = Alt.GetAllVehicles().ToList().FirstOrDefault(x => x.GetVehicleId() == (ulong)vehID);
                if (action == "storage")
                {
                    //Fahrzeug einparken
                    if (vehicle == null) return;
                    if (!vehicle.Position.IsInRange(player.Position, 50f)) return;

                    //var gData = ServerGarages.ServerGarageSlots_.FirstOrDefault(x => vehicle.Position.IsInRange(new Position(x.posX, x.posY, x.posZ), 15f) && x.garageId == garageid);
                    //Position garagePos = new Position(gData.posX, gData.posY, gData.posZ);
                    //Alt.Log($"StorageVeh: {gData} - {gData.garageId} - {garagePos} - {garagePos.ToString()}");
                    //if (garagePos == null || garagePos == new Position(0, 0, 0)) return;
                    if (vehicle.BodyHealth < 990)
                    {
                        HUDHandler.SendNotification(player, 2, 5000, $"Das Fahrzeug konnte nicht eingeparkt werden, da es beschädigt ist!<br>Lasse es zuvor von einem Mechaniker Reparieren.");
                    }
                    else
                    {
                        ServerVehicles.SetVehicleInGarage(vehicle, true, garageid);
                        HUDHandler.SendNotification(player, 2, 5000, $"Fahrzeug erfolgreich eingeparkt.");
                    }
                }
                else if (action == "take")
                {
                    //Fahrzeug ausparken
                    Position outPos = new Position(0, 0, 0);
                    int curPid = 1;
                    bool slotAreFree = true;
                    foreach (var x in ServerGarages.ServerGarageSlots_.Where(x => x.garageId == garageid))
                    {
                        foreach (var veh in Alt.GetAllVehicles().ToList())
                        {
                            if (veh.Position.IsInRange(ServerGarages.GetGarageSlotPosition(garageid, curPid), 2f))
                            {
                                slotAreFree = false;
                                curPid++;
                                break;
                            }
                            else { slotAreFree = true; }
                        }
                        if (slotAreFree) break;
                    }
                    if (!slotAreFree) { HUDHandler.SendNotification(player, 4, 5000, $"Es ist kein Parkplatz mehr frei."); return; }
                    if (vehicle != null) { HUDHandler.SendNotification(player, 4, 5000, $"Ein unerwarteter Fehler ist aufgetreten. [GARAGE-002]"); return; }
                    var finalVeh = ServerVehicles.ServerVehicles_.FirstOrDefault(v => v.id == vehID);
                    if (finalVeh == null) { HUDHandler.SendNotification(player, 4, 5000, $"Ein unerwarteter Fehler ist aufgetreten. [GARAGE-001]"); return; }
                    var altVeh = await AltAsync.Do(() => Alt.CreateVehicle((uint)finalVeh.hash, ServerGarages.GetGarageSlotPosition(garageid, curPid), (ServerGarages.GetGarageSlotRotation(garageid, curPid))));
                    altVeh.LockState = VehicleLockState.Locked;
                    altVeh.EngineOn = false;
                    altVeh.NumberplateText = finalVeh.plate;
                    altVeh.SetVehicleId((ulong)finalVeh.id);
                    altVeh.SetVehicleTrunkState(false);
                    ServerVehicles.SetVehicleModsCorrectly(altVeh);
                    ServerVehicles.SetVehicleInGarage(altVeh, false, garageid);
                }

                if (!CharactersTablet.HasCharacterTutorialEntryFinished(charId, "useGarage"))
                {
                    CharactersTablet.SetCharacterTutorialEntryState(charId, "useGarage", true);
                    HUDHandler.SendNotification(player, 1, 2500, "Erfolg freigeschaltet: Keine Schäden");
                }
                stopwatch.Stop();

                if (stopwatch.Elapsed.Milliseconds > 30) Alt.Log($"{charId} - DoGarageAction benötigte {stopwatch.Elapsed.Milliseconds}ms");
            }
            catch(Exception e)
            {
                Alt.Log($"{e}");
            }
        }
    }
}
