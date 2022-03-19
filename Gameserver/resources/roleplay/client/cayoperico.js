import alt from 'alt-client';
import * as native from 'natives';

let player = alt.Player.local;
let pos = { x: 4895.28, y: -5744.58, z: 26.351 };
let loaded = false;

alt.setInterval(load_island, 1000)

function load_island() {
    let dist = native.getDistanceBetweenCoords(pos.x, pos.y, pos.z, player.pos.x, player.pos.y, player.pos.z, false);
    if (dist <= 2000 && !loaded) {
        native.setIslandHopperEnabled('HeistIsland', true);
        native.setScenarioGroupEnabled('Heist_Island_Peds', true);
        native.setAudioFlag("PlayerOnDLCHeist4Island", true);
        native.setAmbientZoneListStatePersistent("AZL_DLC_Hei4_Island_Zones", true, true);
        native.setAmbientZoneListStatePersistent("AZL_DLC_Hei4_Island_Disabled_Zones", false, true);
        loaded = true;
    } 
    else if (dist > 2000 && loaded) {
        native.setIslandHopperEnabled('HeistIsland', false);
        native.setScenarioGroupEnabled("Heist_Island_Peds", false);
        native.setAudioFlag("PlayerOnDLCHeist4Island", false);
        native.setAmbientZoneListStatePersistent("AZL_DLC_Hei4_Island_Zones", false, false);
        native.setAmbientZoneListStatePersistent("AZL_DLC_Hei4_Island_Disabled_Zones", false, false);
        loaded = false;
    }
}

alt.on("connectionComplete",()=>{
    new alt.PointBlip(6500, -6500, 20).alpha = 0;

    native.doorControl(alt.hash("h4_prop_h4_gate_r_03a"), 4981.012, -5712.747, 20.78103, true, 0, 0, -10);
    native.doorControl(alt.hash("h4_prop_h4_gate_l_03a"), 4984.134, -5709.249, 20.78103, true, 0, 0, 10);
    native.doorControl(alt.hash("h4_prop_h4_gate_r_03a"), 4990.681, -5715.106, 20.78103, true, 0, 0, -10);
    native.doorControl(alt.hash("h4_prop_h4_gate_l_03a"), 4987.587, -5718.635, 20.78103, true, 0, 0, 10);

    alt.setInterval(() => {
        native.setRadarAsExteriorThisFrame()
        native.setRadarAsInteriorThisFrame(alt.hash("h4_fake_islandx"), 4700.0, -5145.0, 0, 0)
    }, 1)
});