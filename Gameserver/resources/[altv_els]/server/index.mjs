import * as alt from 'alt-server';

alt.onClient("Server:Sirens:ForwardSirenMute", (player, vehicleID, status) => {
    alt.emitClient(null, "Client:Sirens:MuteSirens", vehicleID, status);
});

function disableRadioForCurrentVehicleOfPlayer(vehicle){
    alt.emitClient(player, "Client:Sirens:DisableRadio", vehicle.id);
}

alt.onClient("Server:Sirens:UpdateSoundState", (player,vehId,state,index) => {
    alt.emitClient(null, "Client:Sirens:UpdateSoundState", vehId, state,index);
});

alt.onClient("Server:Sirens:UpdateHonkState", (player,status, vehicleId) => {
    alt.emitClient(null, "Client:Sirens:UpdateHonkState", status, vehicleId);
});