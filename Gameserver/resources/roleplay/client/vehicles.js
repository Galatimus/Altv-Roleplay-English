import * as alt from 'alt';
import * as game from 'natives';

alt.onServer("Client:Vehicles:ToggleDoorState", (veh, doorid, state) => {
    toggleDoor(veh, parseInt(doorid), state);
});

alt.on("gameEntityCreate", (entity) => {
    if (entity instanceof alt.Vehicle) {
        if (!entity.hasStreamSyncedMeta("IsVehicleCardealer")) return;
        if (entity.getStreamSyncedMeta("IsVehicleCardealer") == true) {
            game.freezeEntityPosition(entity.scriptID, true);
            game.setEntityInvincible(entity.scriptID, true);
        }
    }
});

function toggleDoor(vehicle, doorid, state) {
    if (state) {
        game.setVehicleDoorOpen(vehicle.scriptID, doorid, false, false);
    } else {
        game.setVehicleDoorShut(vehicle.scriptID, doorid, false);
    }
}

