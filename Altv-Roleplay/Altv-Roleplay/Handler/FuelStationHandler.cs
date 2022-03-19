using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using Altv_Roleplay.Model;
using Altv_Roleplay.Utils;

namespace Altv_Roleplay.Handler
{
    class FuelStationHandler : IScript
    {
        [AsyncClientEvent("Server:FuelStation:FuelVehicleAction")]
        public async Task FuelVehicle(IPlayer player, int vID, int fuelstationId, string fueltype, int selectedLiterAmount, int selectedLiterPrice)
        {
            try
            {
                if (player == null || !player.Exists || vID == 0 || fuelstationId == 0 || fueltype == "" || selectedLiterAmount <= 0 || selectedLiterPrice == 0) return;
                ulong vehID = Convert.ToUInt64(vID);
                int charId = User.GetPlayerOnline(player);
                if (vehID <= 0 || charId <= 0) return;
                if (player.HasPlayerHandcuffs() || player.HasPlayerRopeCuffs()) { HUDHandler.SendNotification(player, 3, 5000, "Wie willst du das mit Handschellen/Fesseln machen?"); return; }
                var vehicle = Alt.GetAllVehicles().ToList().FirstOrDefault(x => x.GetVehicleId() == vehID);
                if (vehicle == null || !vehicle.Exists) { HUDHandler.SendNotification(player, 3, 5000, "Ein unerwarteter Fehler ist aufgetreten. [FEHLERCODE: FUEL-004]"); return; }
                if (ServerVehicles.GetVehicleType(vehicle) == 0)
                {
                    if (!CharactersInventory.ExistCharacterItem(charId, "Bargeld", "inventory")) { HUDHandler.SendNotification(player, 4, 5000, "Du hast nicht genügend Bargeld dabei."); return; }
                    if (CharactersInventory.GetCharacterItemAmount(charId, "Bargeld", "inventory") < (selectedLiterPrice * selectedLiterAmount)) { HUDHandler.SendNotification(player, 3, 5000, "Du hast nicht genügend Bargeld dabei."); return; }
                }
                if (!player.Position.IsInRange(vehicle.Position, 8f)) { HUDHandler.SendNotification(player, 4, 5000, "Du hast dich zu weit vom Fahrzeug entfernt."); return; }
                if (ServerVehicles.GetVehicleFuel(vehicle) >= ServerVehicles.GetVehicleFuelLimitOnHash(vehicle.Model)) { HUDHandler.SendNotification(player, 3, 5000, "Das Fahrzeug ist bereits voll getankt."); return; }
                var fuelStation = ServerFuelStations.ServerFuelStations_.FirstOrDefault(x => x.id == fuelstationId);
                if (fuelStation == null) { HUDHandler.SendNotification(player, 4, 5000, "Ein unerwarteter Fehler ist aufgetreten. [FEHLERCODE: FUEL-005]"); return; }
                int duration = 500 * selectedLiterAmount;
                HUDHandler.SendNotification(player, 1, duration, "Fahrzeug wird betankt, bitte warten..");
                await Task.Delay(duration);
                lock (player)
                {
                    if (!player.Position.IsInRange(vehicle.Position, 10f)) { HUDHandler.SendNotification(player, 4, 5000, "Du hast dich zu weit vom Fahrzeug entfernt."); return; }
                }
                float fuelVal = ServerVehicles.GetVehicleFuel(vehicle) + selectedLiterAmount;
                if (fuelVal > ServerVehicles.GetVehicleFuelLimitOnHash(vehicle.Model)) { fuelVal = ServerVehicles.GetVehicleFuelLimitOnHash(vehicle.Model); }
                if (ServerVehicles.GetVehicleType(vehicle) == 0)
                {
                    CharactersInventory.RemoveCharacterItemAmount(charId, "Bargeld", (selectedLiterPrice * selectedLiterAmount), "inventory");
                }
                ServerVehicles.SetVehicleFuel(vehicle, fuelVal);
                if (ServerVehicles.GetVehicleFuelTypeOnHash(vehicle.Model) != fueltype) { ServerVehicles.SetVehicleEngineState(vehicle, false); ServerVehicles.SetVehicleEngineHealthy(vehicle, false); return; }
                ServerFuelStations.SetFuelStationBankMoney(fuelstationId, ServerFuelStations.GetFuelStationBankMoney(fuelstationId) + (selectedLiterPrice * selectedLiterAmount));

                if (ServerFuelStations.GetFuelStationOwnerId(fuelstationId) != 0)
                {
                    ServerFuelStations.SetFuelStationAvailableLiters(fuelstationId, ServerFuelStations.GetFuelStationAvailableLiters(fuelstationId) - selectedLiterAmount);
                }
            }
            catch (Exception e)
            {
                Alt.Log($"{e}");
            }
        }
    }
}
