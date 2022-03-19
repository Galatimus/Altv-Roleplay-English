import * as alt from 'alt';
import * as game from 'natives';

var sirenStrings = ["VEHICLES_HORNS_SIREN_1", "VEHICLES_HORNS_SIREN_2", "VEHICLES_HORNS_POLICE_WARNING",
"RESIDENT_VEHICLES_SIREN_WAIL_01", "RESIDENT_VEHICLES_SIREN_WAIL_02", "RESIDENT_VEHICLES_SIREN_WAIL_03",
"RESIDENT_VEHICLES_SIREN_QUICK_01", "RESIDENT_VEHICLES_SIREN_QUICK_02", "RESIDENT_VEHICLES_SIREN_QUICK_03",
"VEHICLES_HORNS_AMBULANCE_WARNING",
"RESIDENT_VEHICLES_SIREN_FIRETRUCK_WAIL_01",
"RESIDENT_VEHICLES_SIREN_FIRETRUCK_QUICK_01"]

var sirenDict = {};

alt.everyTick(() => {
    if(alt.Player.local.vehicle == null){return;}
    if(game.getVehicleClass(alt.Player.local.vehicle.scriptID) != 18){return;}   // Emergency cars
    game.disableControlAction(1,86,true);
    game.disableControlAction(0, 14, true);
    if (game.isDisabledControlJustPressed(0, 14)){
        if(alt.Player.local.vehicle == null || game.getVehicleClass(alt.Player.local.vehicle.scriptID) != 18){return;}
        if(alt.Player.local.vehicle != null && alt.Player.local.scriptID != game.getPedInVehicleSeat(alt.Player.local.vehicle.scriptID, -1)){return;}
        var index = sirenDict[alt.Player.local.vehicle.id].index;
        if(index == sirenStrings.length-1){
            index = 0;
        }else{
            index++;
        }
        alt.emitServer("Server:Sirens:UpdateSoundState", alt.Player.local.vehicle.id,sirenDict[alt.Player.local.vehicle.id].state,index);
    }
});

alt.on("Client:Sirens:DisableRadio", () => {
    disableRadio();
})

function disableRadio(){
    if(alt.Player.local.vehicle != null){
        game.setVehicleRadioEnabled(alt.Player.local.vehicle.scriptID,false);
    }
}

function toggleEmergencyLight(){
    if(alt.Player.local.vehicle == null){
        return;
    }
    const vehicleID = alt.Player.local.vehicle.scriptID;
    if(game.isVehicleSirenOn(vehicleID)){
        game.setVehicleSiren(alt.Player.local.vehicle.scriptID, false);
        alt.emitServer("Server:Sirens:UpdateSoundState", alt.Player.local.vehicle.id,false,sirenDict[alt.Player.local.vehicle.id].index);
        game.setVehicleHasMutedSirens(alt.Player.local.vehicle.scriptID, true);
    }else{
        game.setVehicleSiren(alt.Player.local.vehicle.scriptID, true);
        game.setVehicleHasMutedSirens(alt.Player.local.vehicle.scriptID, true);
    }
}

alt.on('keydown', (key) => {
    if(alt.Player.local.vehicle == null || game.getVehicleClass(alt.Player.local.vehicle.scriptID) != 18){return;}
    if(alt.Player.local.vehicle != null && alt.Player.local.scriptID != game.getPedInVehicleSeat(alt.Player.local.vehicle.scriptID, -1)){return;}
    if (key === "Q".charCodeAt(0)) {
        disableRadio();
        toggleEmergencyLight();
    }
    if (key === 18) { // ALT
        if(alt.Player.local.vehicle == undefined || alt.Player.local.vehicle == null){return;}
        if(game.isVehicleSirenOn(alt.Player.local.vehicle.scriptID)){
            alt.emitServer("Server:Sirens:UpdateSoundState", alt.Player.local.vehicle.id,!sirenDict[alt.Player.local.vehicle.id].state,sirenDict[alt.Player.local.vehicle.id].index);
        }
    }
    if (key === "E".charCodeAt(0)){
        if(alt.Player.local.vehicle == undefined || alt.Player.local.vehicle == null){return;}
        alt.emitServer("Server:Sirens:UpdateHonkState", true, alt.Player.local.vehicle.id);
    }
});

alt.on('keyup', (key) => {
    if (key === "E".charCodeAt(0)){
        if(alt.Player.local.vehicle == undefined || alt.Player.local.vehicle == null){return;}
        alt.emitServer("Server:Sirens:UpdateHonkState", false, alt.Player.local.vehicle.id);
    }
});

alt.on("leftVehicle", (vehicle, seat) => {
    if(seat == -1){
        alt.emitServer("Server:Sirens:UpdateHonkState", false, vehicle.id);
    }
});

alt.on("gameEntityCreate", (entity) => {
    if(entity.type != 1){return;}
    if(game.getVehicleClass(entity.scriptID) != 18){return;}
    game.setVehicleHasMutedSirens(entity.scriptID, true);
    if(sirenDict[entity.id] == null || sirenDict[entity.id] == undefined){
        sirenDict[entity.id] = {id: entity.id, soundId: game.getSoundId(), state: false, index: 0, honkState: false, honkSoundId: game.getSoundId()};
    }
    game.stopSound(sirenDict[entity.id].soundId);
    if(sirenDict[entity.id].state){
        game.playSoundFromEntity(sirenDict[entity.id].soundId,sirenStrings[sirenDict[entity.id].index],entity.scriptID,0,0,0);
    }
});

alt.on("gameEntityDestroy", (entity) => {
    if(entity.type != 1){return;}
    if(sirenDict[entity.id] != null && sirenDict[entity.id] != undefined){
        game.releaseSoundId(sirenDict[entity.id].soundId);
        game.releaseSoundId(sirenDict[entity.id].honkSoundId);
        delete sirenDict[entity.id];
    }
});

alt.onServer("Client:Sirens:UpdateSoundState", (vehicleId,soundState,soundIndex) => {
    if(sirenDict[vehicleId] == null || sirenDict[vehicleId] == undefined){return;}
    sirenDict[vehicleId].state = soundState;
    sirenDict[vehicleId].index = soundIndex;
    const veh = alt.Vehicle.getByID(vehicleId);
    if(veh == null){return;}
    game.stopSound(sirenDict[vehicleId].soundId);
    if(soundState){
        game.playSoundFromEntity(sirenDict[vehicleId].soundId,sirenStrings[soundIndex],veh.scriptID,0,0,0);
    }
});

alt.onServer("Client:Sirens:UpdateHonkState", (status, vehicleId) => {
    if(sirenDict[vehicleId] == null || sirenDict[vehicleId] == undefined){return;}
    sirenDict[vehicleId].honkState = status;
    const veh = alt.Vehicle.getByID(vehicleId);
    if(veh == null){return;}
    game.stopSound(sirenDict[vehicleId].honkSoundId);
    if(status){
        game.playSoundFromEntity(sirenDict[vehicleId].honkSoundId,"SIRENS_AIRHORN",veh.scriptID,0,0,0);
    }
});