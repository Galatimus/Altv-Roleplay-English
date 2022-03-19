import * as alt from 'alt';
import * as game from 'natives';

let loginBrowser = null;
let loginCam = null;
let loginPedHandle = null;
let loginModelHash = null;

alt.onServer('Client:Login:CreateCEF', () => {
    if (loginBrowser == null) {
        loginCam = game.createCamWithParams('DEFAULT_SCRIPTED_CAMERA', 3280, 5220, 26, 0, 0, 240, 50, true, 2);
        game.setCamActive(loginCam, true);
        game.renderScriptCams(true, false, 0, true, false, 0);
        game.freezeEntityPosition(alt.Player.local.scriptID, true);
        alt.showCursor(true);
        alt.toggleGameControls(false);
        loginBrowser = new alt.WebView("http://resource/client/cef/login/index.html");
        loginBrowser.focus();
        loginBrowser.on("Client:Login:cefIsReady", () => {
            alt.setTimeout(() => {   
                if (alt.LocalStorage.get("username")) {
                    loginBrowser.emit("CEF:Login:setStorage", alt.LocalStorage.get("username"), alt.LocalStorage.get("password"));
                }
                loginBrowser.emit("CEF:Login:showArea", "login");
            }, 2000);
        });//DONE

        loginBrowser.on("Client:Login:sendLoginDataToServer", (name, password) => {
            alt.emitServer("Server:Login:ValidateLoginCredentials", name, password);
        });//DONE

        loginBrowser.on("Client:Register:sendRegisterDataToServer", (name, email, password, passwordrepeat) => {
            alt.emitServer("Server:Register:RegisterNewPlayer", name, email, password, passwordrepeat);
        });//DONE

        loginBrowser.on("Client:Charcreator:OpenCreator", () => {
            alt.emitServer("Server:Charcreator:CreateCEF");
            destroyLoginBrowser();
        });//DONE

        loginBrowser.on("Client:Login:DestroyCEF", () => {
            destroyLoginBrowser();
        });//DONE

        loginBrowser.on("Client:Charselector:KillCharacter", (charid) => {
            alt.emitServer("Server:Charselector:KillCharacter", charid);
        });//DONE

        loginBrowser.on("Client:Charselector:PreviewCharacter", (charid) => {
            alt.emitServer("Server:Charselector:PreviewCharacter", charid);
        });//DONE

        loginBrowser.on("Client:Charselector:spawnChar", (charid, spawnstr) => {
            alt.emitServer("Server:Charselector:spawnChar", spawnstr, charid);
        });//DONE
    }
});//DONE

alt.onServer("Client:SpawnArea:setCharSkin", (facefeaturearray, headblendsarray, headoverlayarray) => {
    let facefeatures = JSON.parse(facefeaturearray);
    let headblends = JSON.parse(headblendsarray);
    let headoverlays = JSON.parse(headoverlayarray);

    game.setPedHeadBlendData(alt.Player.local.scriptID, parseInt(headblends[0]), parseInt(headblends[1]), 0, parseInt(headblends[2]), parseInt(headblends[5]), 0, parseFloat(headblends[3]), parseInt(headblends[4]), 0, false);
    game.setPedHeadOverlayColor(alt.Player.local.scriptID, 1, 1, parseInt(headoverlays[2][1]), 1);
    game.setPedHeadOverlayColor(alt.Player.local.scriptID, 2, 1, parseInt(headoverlays[2][2]), 1);
    game.setPedHeadOverlayColor(alt.Player.local.scriptID, 5, 2, parseInt(headoverlays[2][5]), 1);
    game.setPedHeadOverlayColor(alt.Player.local.scriptID, 8, 2, parseInt(headoverlays[2][8]), 1);
    game.setPedHeadOverlayColor(alt.Player.local.scriptID, 10, 1, parseInt(headoverlays[2][10]), 1);
    game.setPedEyeColor(alt.Player.local.scriptID, parseInt(headoverlays[0][14]));
    game.setPedHeadOverlay(alt.Player.local.scriptID, 0, parseInt(headoverlays[0][0]), parseInt(headoverlays[1][0]));
    game.setPedHeadOverlay(alt.Player.local.scriptID, 1, parseInt(headoverlays[0][1]), parseFloat(headoverlays[1][1]));
    game.setPedHeadOverlay(alt.Player.local.scriptID, 2, parseInt(headoverlays[0][2]), parseFloat(headoverlays[1][2]));
    game.setPedHeadOverlay(alt.Player.local.scriptID, 3, parseInt(headoverlays[0][3]), parseInt(headoverlays[1][3]));
    game.setPedHeadOverlay(alt.Player.local.scriptID, 4, parseInt(headoverlays[0][4]), parseInt(headoverlays[1][4]));
    game.setPedHeadOverlay(alt.Player.local.scriptID, 5, parseInt(headoverlays[0][5]), parseInt(headoverlays[1][5]));
    game.setPedHeadOverlay(alt.Player.local.scriptID, 6, parseInt(headoverlays[0][6]), parseInt(headoverlays[1][6]));
    game.setPedHeadOverlay(alt.Player.local.scriptID, 7, parseInt(headoverlays[0][7]), parseInt(headoverlays[1][7]));
    game.setPedHeadOverlay(alt.Player.local.scriptID, 8, parseInt(headoverlays[0][8]), parseInt(headoverlays[1][8]));
    game.setPedHeadOverlay(alt.Player.local.scriptID, 9, parseInt(headoverlays[0][9]), parseInt(headoverlays[1][9]));
    game.setPedHeadOverlay(alt.Player.local.scriptID, 10, parseInt(headoverlays[0][10]), parseInt(headoverlays[1][10]));
    game.setPedComponentVariation(alt.Player.local.scriptID, 2, parseInt(headoverlays[0][13]), 0, 0);
    game.setPedHairColor(alt.Player.local.scriptID, parseInt(headoverlays[2][13]), parseInt(headoverlays[1][13]));

    for (let i = 0; i < 20; i++) {
        game.setPedFaceFeature(alt.Player.local.scriptID, i, parseFloat(facefeatures[i]));
    }
});//DONE

alt.onServer("Client:SpawnArea:setCharClothes", (componentId, drawableId, textureId) => {
    //alt.log(`Component: ${componentId} - drawable: ${drawableId} - texture: ${textureId}`);
    game.setPedComponentVariation(alt.Player.local.scriptID, parseInt(componentId), parseInt(drawableId), parseInt(textureId), 0);
});

alt.onServer("Client:SpawnArea:setCharAccessory", (componentId, drawableId, textureId) => {
    game.setPedPropIndex(alt.Player.local.scriptID, componentId, drawableId, textureId, false);
});

alt.onServer("Client:SpawnArea:clearCharAccessory", (componentId) => {
    game.clearPedProp(alt.Player.local.scriptID, componentId);
});

alt.onServer("Client:Charselector:ViewCharacter", (gender, facefeaturearray, headblendsarray, headoverlayarray) => {
    spawnCharSelectorPed(gender, facefeaturearray, headblendsarray, headoverlayarray);
});//DONE

alt.onServer("Client:Login:SaveLoginCredentialsToStorage", (name, password) => {
    alt.LocalStorage.set('username', name);
    alt.LocalStorage.set('password', password);
    alt.LocalStorage.save();
});//DONE

alt.onServer("Client:Login:showError", (msg) => {
    if (loginBrowser != null) {
        loginBrowser.emit("CEF:Login:showError", msg);
    }
});//DONE

alt.onServer("Client:Login:showArea", (area) => {
    if (loginBrowser != null) {
        loginBrowser.emit("CEF:Login:showArea", area);
        if (area == "charselect") {
            if (loginCam != null) {
                game.renderScriptCams(false, false, 0, true, false, 0);
                game.setCamActive(loginCam, false);
                game.destroyCam(loginCam, true);
                loginCam = null;
            }
            game.setEntityAlpha(alt.Player.local.scriptID, 0, 0);
            loginCam = game.createCamWithParams('DEFAULT_SCRIPTED_CAMERA', 402.7, -1003, -98.6, 0, 0, 358, 18, true, 2);
            game.setCamActive(loginCam, true);
            game.renderScriptCams(true, false, 0, true, false, 0);
        }
    }
});//DONE

alt.onServer("Client:Charselector:sendCharactersToCEF", (chars) => {
    if (loginBrowser != null) {
        loginBrowser.emit("CEF:Charselector:sendCharactersToCEF", chars)
    }
});//DONE

let destroyLoginBrowser = function() {
    if (loginBrowser != null) {
        loginBrowser.destroy();
    }
    loginBrowser = null;
    game.renderScriptCams(false, false, 0, true, false, 0);
    game.setCamActive(loginCam, false);
    if (loginCam != null) {
        game.destroyCam(loginCam, true);
    }
    if (loginPedHandle != null) {
        game.deletePed(loginPedHandle);
        loginPedHandle = null;
    }
    loginCam = null;
    alt.showCursor(false);
    alt.toggleGameControls(true);
    game.freezeEntityPosition(alt.Player.local.scriptID, false);
    game.setEntityAlpha(alt.Player.local.scriptID, 255, 0);
}//DONE

function spawnCharSelectorPed(gender, facefeaturearray, headblendsarray, headoverlayarray) {
    let facefeatures = JSON.parse(facefeaturearray);
    let headblends = JSON.parse(headblendsarray);
    let headoverlays = JSON.parse(headoverlayarray);

    if (loginPedHandle != null) {
        game.deletePed(loginPedHandle);
        loginPedHandle = null;
    }

    if (gender == true) {
        loginModelHash = game.getHashKey('mp_f_freemode_01');
        game.requestModel(loginModelHash);
    } else if (gender == false) {
        loginModelHash = game.getHashKey('mp_m_freemode_01');
        game.requestModel(loginModelHash);
    }
    alt.setTimeout(function() {
        if (game.hasModelLoaded(loginModelHash)) {
            loginPedHandle = game.createPed(4, loginModelHash, 402.778, -996.9758, -100.01465, 0, false, true);
            game.setEntityAlpha(loginPedHandle, 255, 0);
            game.setEntityHeading(loginPedHandle, 180.0);
            game.setEntityInvincible(loginPedHandle, true);
            game.disablePedPainAudio(loginPedHandle, true);
            game.freezeEntityPosition(loginPedHandle, true);
            game.taskSetBlockingOfNonTemporaryEvents(loginPedHandle, true);

            game.setPedHeadBlendData(loginPedHandle, headblends[0], headblends[1], 0, headblends[2], headblends[5], 0, headblends[3], headblends[4], 0, 0);
            game.setPedHeadOverlayColor(loginPedHandle, 1, 1, parseInt(headoverlays[2][1]), 1);
            game.setPedHeadOverlayColor(loginPedHandle, 2, 1, parseInt(headoverlays[2][2]), 1);
            game.setPedHeadOverlayColor(loginPedHandle, 5, 2, parseInt(headoverlays[2][5]), 1);
            game.setPedHeadOverlayColor(loginPedHandle, 8, 2, parseInt(headoverlays[2][8]), 1);
            game.setPedHeadOverlayColor(loginPedHandle, 10, 1, parseInt(headoverlays[2][10]), 1);
            game.setPedEyeColor(loginPedHandle, parseInt(headoverlays[0][14]));
            game.setPedHeadOverlay(loginPedHandle, 0, parseInt(headoverlays[0][0]), parseInt(headoverlays[1][0]));
            game.setPedHeadOverlay(loginPedHandle, 1, parseInt(headoverlays[0][1]), parseFloat(headoverlays[1][1]));
            game.setPedHeadOverlay(loginPedHandle, 2, parseInt(headoverlays[0][2]), parseFloat(headoverlays[1][2]));
            game.setPedHeadOverlay(loginPedHandle, 3, parseInt(headoverlays[0][3]), parseInt(headoverlays[1][3]));
            game.setPedHeadOverlay(loginPedHandle, 4, parseInt(headoverlays[0][4]), parseInt(headoverlays[1][4]));
            game.setPedHeadOverlay(loginPedHandle, 5, parseInt(headoverlays[0][5]), parseInt(headoverlays[1][5]));
            game.setPedHeadOverlay(loginPedHandle, 6, parseInt(headoverlays[0][6]), parseInt(headoverlays[1][6]));
            game.setPedHeadOverlay(loginPedHandle, 7, parseInt(headoverlays[0][7]), parseInt(headoverlays[1][7]));
            game.setPedHeadOverlay(loginPedHandle, 8, parseInt(headoverlays[0][8]), parseInt(headoverlays[1][8]));
            game.setPedHeadOverlay(loginPedHandle, 9, parseInt(headoverlays[0][9]), parseInt(headoverlays[1][9]));
            game.setPedHeadOverlay(loginPedHandle, 10, parseInt(headoverlays[0][10]), parseInt(headoverlays[1][10]));
            game.setPedComponentVariation(loginPedHandle, 2, parseInt(headoverlays[0][13]), 0, 0);
            game.setPedHairColor(loginPedHandle, parseInt(headoverlays[2][13]), parseInt(headoverlays[1][13]));

            for (let i = 0; i < 20; i++) {                
                game.setPedFaceFeature(loginPedHandle, i, parseFloat(facefeatures[i]));
            }
        }
    }, 200);
}//DONE

alt.on('connectionComplete', () => {
    loadallIPLsAndInteriors();
    alt.setStat('stamina', 50);
    alt.setStat('strength', 50);
    alt.setStat('lung_capacity', 100);
    alt.setStat('wheelie_ability', 100);
    alt.setStat('flying_ability', 100);
    alt.setStat('shooting_ability', 100);
    alt.setStat('stealth_ability', 100);
});//DONE

function loadallIPLsAndInteriors() {
    alt.removeIpl("rc12b_default"); //Pillbox Hill Hospital
    alt.removeIpl("rc12b_hospitalinterior"); //Pillbox Hill Hospital
    alt.removeIpl("rc12b_hospitalinterior_lod"); //Pillbox Hill Hospital
    alt.removeIpl("rc12b_destroyed"); //Pillbox Hill Hospital

    alt.requestIpl("hei_hw1_blimp_interior_v_apart_midspaz_milo");
    alt.requestIpl('canyonriver01');
    alt.requestIpl('chop_props');
    alt.requestIpl('FIBlobby');
    alt.removeIpl('FIBlobbyfake');
    alt.requestIpl('FBI_colPLUG');
    alt.requestIpl('FBI_repair');
    alt.requestIpl('v_tunnel_hole');
    alt.requestIpl('TrevorsMP');
    alt.requestIpl('TrevorsTrailer');
    alt.requestIpl('TrevorsTrailerTidy');
    alt.removeIpl('farm_burnt');
    alt.removeIpl('farm_burnt_lod');
    alt.removeIpl('farm_burnt_props');
    alt.removeIpl('farmint_cap');
    alt.removeIpl('farmint_cap_lod');
    alt.requestIpl('farm');
    alt.requestIpl('farmint');
    alt.requestIpl('farm_lod');
    alt.requestIpl('farm_props');
    alt.requestIpl('facelobby');
    alt.removeIpl('CS1_02_cf_offmission');
    alt.requestIpl('CS1_02_cf_onmission1');
    alt.requestIpl('CS1_02_cf_onmission2');
    alt.requestIpl('CS1_02_cf_onmission3');
    alt.requestIpl('CS1_02_cf_onmission4');
    alt.requestIpl('v_rockclub');
    alt.requestIpl('v_janitor');
    alt.removeIpl('hei_bi_hw1_13_door');
    alt.requestIpl('bkr_bi_hw1_13_int');
    game.removeIpl('ufo');
    game.removeIpl('ufo_lod');
    game.removeIpl('ufo_eye');
    alt.requestIpl('farm');
    alt.requestIpl('farm_lod');
    alt.requestIpl('farm_props');
    alt.requestIpl('farmint');
    alt.requestIpl('des_farmhs_startimap');
    alt.removeIpl('v_carshowroom');
    alt.removeIpl('shutter_open');
    alt.removeIpl('shutter_closed');
    alt.removeIpl('shr_int');
    alt.requestIpl('csr_afterMission');
    alt.requestIpl('v_carshowroom');
    alt.requestIpl('shr_int');
    alt.requestIpl('shutter_closed');
    alt.requestIpl('smboat');
    alt.requestIpl('smboat_distantlights');
    alt.requestIpl('smboat_lod');
    alt.requestIpl('smboat_lodlights');
    alt.requestIpl('cargoship');
    alt.requestIpl('railing_start');
    alt.removeIpl('sp1_10_fake_interior');
    alt.removeIpl('sp1_10_fake_interior_lod');
    alt.requestIpl('sp1_10_real_interior');
    alt.requestIpl('sp1_10_real_interior_lod');
    alt.removeIpl('id2_14_during_door');
    alt.removeIpl('id2_14_during1');
    alt.removeIpl('id2_14_during2');
    alt.removeIpl('id2_14_on_fire');
    alt.removeIpl('id2_14_post_no_int');
    alt.removeIpl('id2_14_pre_no_int');
    alt.removeIpl('id2_14_during_door');
    alt.requestIpl('id2_14_during1');
    alt.removeIpl('Coroner_Int_off');
    alt.requestIpl('coronertrash');
    alt.requestIpl('Coroner_Int_on');
    alt.removeIpl('bh1_16_refurb');
    alt.removeIpl('jewel2fake');
    alt.removeIpl('bh1_16_doors_shut');
    alt.requestIpl('refit_unload');
    alt.requestIpl('post_hiest_unload');
    alt.requestIpl('Carwash_with_spinners');
    alt.requestIpl('KT_CarWash');
    alt.requestIpl('ferris_finale_Anim');
    alt.removeIpl('ch1_02_closed');
    alt.requestIpl('ch1_02_open');
    alt.requestIpl('AP1_04_TriAf01');
    alt.requestIpl('CS2_06_TriAf02');
    alt.requestIpl('CS4_04_TriAf03');
    alt.removeIpl('scafstartimap');
    alt.requestIpl('scafendimap');
    alt.removeIpl('DT1_05_HC_REMOVE');
    alt.requestIpl('DT1_05_HC_REQ');
    alt.requestIpl('DT1_05_REQUEST');
    alt.requestIpl('FINBANK');
    alt.removeIpl('DT1_03_Shutter');
    alt.removeIpl('DT1_03_Gr_Closed');
    alt.requestIpl('golfflags');
    alt.requestIpl('airfield');
    alt.requestIpl('v_garages');
    alt.requestIpl('v_foundry');
    alt.requestIpl('hei_yacht_heist');
    alt.requestIpl('hei_yacht_heist_Bar');
    alt.requestIpl('hei_yacht_heist_Bedrm');
    alt.requestIpl('hei_yacht_heist_Bridge');
    alt.requestIpl('hei_yacht_heist_DistantLights');
    alt.requestIpl('hei_yacht_heist_enginrm');
    alt.requestIpl('hei_yacht_heist_LODLights');
    alt.requestIpl('hei_yacht_heist_Lounge');
    alt.requestIpl('hei_carrier');
    alt.requestIpl('hei_Carrier_int1');
    alt.requestIpl('hei_Carrier_int2');
    alt.requestIpl('hei_Carrier_int3');
    alt.requestIpl('hei_Carrier_int4');
    alt.requestIpl('hei_Carrier_int5');
    alt.requestIpl('hei_Carrier_int6');
    alt.requestIpl('hei_carrier_LODLights');
    alt.requestIpl('bkr_bi_id1_23_door');
    alt.requestIpl('lr_cs6_08_grave_closed');
    alt.requestIpl('hei_sm_16_interior_v_bahama_milo_');
    alt.requestIpl('CS3_07_MPGates');
    alt.requestIpl('cs5_4_trains');
    alt.requestIpl('v_lesters');
    alt.requestIpl('v_trevors');
    alt.requestIpl('v_michael');
    alt.requestIpl('v_comedy');
    alt.requestIpl('v_cinema');
    alt.requestIpl('V_Sweat');
    alt.requestIpl('V_35_Fireman');
    alt.requestIpl('redCarpet');
    alt.requestIpl('triathlon2_VBprops');
    alt.requestIpl('jetstegameurnel');
    alt.requestIpl('Jetsteal_ipl_grp1');
    alt.requestIpl('v_hospital');
    alt.requestIpl('bh1_47_joshhse_unburnt');


    alt.requestIpl("apa_v_mp_h_02_a");

    // CLOSE OPEN DOORS
    game.doorControl(3687927243, -1149.709, -1521.088, 10.78267, true, 0.0, 50.0, 0.0); // VESPUCCI HOUSE
    game.doorControl(520341586, -14.868921, -1441.1823, 31.193226, true, 0.0, 50.0, 0.0); // FRANKLIN'S OLD HOUSE
    game.doorControl(159994461, -816.716, 179.09796, 72.82738, true, 0.0, 50.0, 0.0); // MICHAELS HOUSE 
    game.doorControl(2608952911, -816.1068, 177.51086, 72.82738, true, 0.0, 50.0, 0.0); // MICHAELS HOUSE 
    game.doorControl(2731327123, -806.28174, 186.02461, 72.62405, true, 0.0, 50.0, 0.0); // MICHAELS HOUSE 
    game.doorControl(2840207166, -793.3943, 180.50746, 73.04045, true, 0.0, 50.0, 0.0); // MICHAELS HOUSE 
    game.doorControl(2840207166, -796.5657, 177.22139, 73.04045, true, 0.0, 50.0, 0.0); // MICHAELS HOUSE 
    game.doorControl(1245831483, -794.1853, 182.56801, 73.04045, true, 0.0, 50.0, 0.0); // MICHAELS HOUSE 
    game.doorControl(1245831483, -794.5051, 178.01237, 73.04045, true, 0.0, 50.0, 0.0); // MICHAELS HOUSE 
    game.doorControl(308207762, 7.518359, 539.5268, 176.17764, true, 0.0, 50.0, 0.0); // FRANKLIN'S NEW HOUSE
    game.doorControl(1145337974, 1273.8154, -1720.6969, 54.92143, true, 0.0, 50.0, 0.0); // LESTER'S HOUSE
    game.doorControl(132154435, 1972.769, 3815.366, 33.663258, true, 0.0, 50.0, 0.0); // TREVOR'S HOUSE
    
    alt.requestIpl('shr_int'); //Premium Deluxe Motorsports
    game.activateInteriorEntitySet(game.getInteriorAtCoordsWithType(-38.62, -1099.01, 27.31, 'v_carshowroom'), 'csr_beforeMission'); //Premium Deluxe Motorsports
    game.activateInteriorEntitySet(game.getInteriorAtCoordsWithType(-38.62, -1099.01, 27.31, 'v_carshowroom'), 'shutter_closed'); //Premium Deluxe Motorsports

    var nightClubGalaxyIntId = game.getInteriorAtCoords(345.4899597168, 294.95315551758, 98.191421508789); //Galaxy Nightclub
    //120834 Galaxy InteriorId
    game.pinInteriorInMemory(nightClubGalaxyIntId);
    game.activateInteriorEntitySet(nightClubGalaxyIntId, "Int01_ba_security_upgrade"); //Galaxy Nightclub
    game.activateInteriorEntitySet(nightClubGalaxyIntId, "Int01_ba_equipment_setup"); //Galaxy Nightclub
    game.deactivateInteriorEntitySet(nightClubGalaxyIntId, "Int01_ba_Style01"); //Galaxy Nightclub
    game.deactivateInteriorEntitySet(nightClubGalaxyIntId, "Int01_ba_Style02"); //Galaxy Nightclub
    game.activateInteriorEntitySet(nightClubGalaxyIntId, "Int01_ba_Style03"); //Galaxy Nightclub
    game.deactivateInteriorEntitySet(nightClubGalaxyIntId, "Int01_ba_style01_podium"); //Galaxy Nightclub
    game.deactivateInteriorEntitySet(nightClubGalaxyIntId, "Int01_ba_style02_podium"); //Galaxy Nightclub
    game.activateInteriorEntitySet(nightClubGalaxyIntId, "Int01_ba_style03_podium"); //Galaxy Nightclub
    game.activateInteriorEntitySet(nightClubGalaxyIntId, "int01_ba_lights_screen"); //Galaxy Nightclub
    game.activateInteriorEntitySet(nightClubGalaxyIntId, "Int01_ba_Screen"); //Galaxy Nightclub
    game.activateInteriorEntitySet(nightClubGalaxyIntId, "Int01_ba_bar_content"); //Galaxy Nightclub
    game.deactivateInteriorEntitySet(nightClubGalaxyIntId, "Int01_ba_booze_01"); //Galaxy Nightclub
    game.deactivateInteriorEntitySet(nightClubGalaxyIntId, "Int01_ba_booze_02"); //Galaxy Nightclub
    game.deactivateInteriorEntitySet(nightClubGalaxyIntId, "Int01_ba_booze_03"); //Galaxy Nightclub
    game.deactivateInteriorEntitySet(nightClubGalaxyIntId, "Int01_ba_dj01"); //Galaxy Nightclub
    game.deactivateInteriorEntitySet(nightClubGalaxyIntId, "Int01_ba_dj02"); //Galaxy Nightclub
    game.activateInteriorEntitySet(nightClubGalaxyIntId, "Int01_ba_dj03"); //Galaxy Nightclub
    game.deactivateInteriorEntitySet(nightClubGalaxyIntId, "Int01_ba_dj04"); //Galaxy Nightclub
    game.activateInteriorEntitySet(nightClubGalaxyIntId, "DJ_01_Lights_01"); //Galaxy Nightclub
    game.deactivateInteriorEntitySet(nightClubGalaxyIntId, "DJ_01_Lights_02"); //Galaxy Nightclub
    game.deactivateInteriorEntitySet(nightClubGalaxyIntId, "DJ_01_Lights_03"); //Galaxy Nightclub
    game.deactivateInteriorEntitySet(nightClubGalaxyIntId, "DJ_01_Lights_04"); //Galaxy Nightclub
    game.deactivateInteriorEntitySet(nightClubGalaxyIntId, "DJ_02_Lights_01"); //Galaxy Nightclub
    game.activateInteriorEntitySet(nightClubGalaxyIntId, "DJ_02_Lights_02"); //Galaxy Nightclub
    game.deactivateInteriorEntitySet(nightClubGalaxyIntId, "DJ_02_Lights_03"); //Galaxy Nightclub
    game.deactivateInteriorEntitySet(nightClubGalaxyIntId, "DJ_02_Lights_04"); //Galaxy Nightclub
    game.deactivateInteriorEntitySet(nightClubGalaxyIntId, "DJ_03_Lights_01"); //Galaxy Nightclub
    game.deactivateInteriorEntitySet(nightClubGalaxyIntId, "DJ_03_Lights_02"); //Galaxy Nightclub
    game.activateInteriorEntitySet(nightClubGalaxyIntId, "DJ_03_Lights_03"); //Galaxy Nightclub
    game.deactivateInteriorEntitySet(nightClubGalaxyIntId, "DJ_03_Lights_04"); //Galaxy Nightclub
    game.deactivateInteriorEntitySet(nightClubGalaxyIntId, "DJ_04_Lights_01"); //Galaxy Nightclub
    game.deactivateInteriorEntitySet(nightClubGalaxyIntId, "DJ_04_Lights_02"); //Galaxy Nightclub
    game.deactivateInteriorEntitySet(nightClubGalaxyIntId, "DJ_04_Lights_03"); //Galaxy Nightclub
    game.activateInteriorEntitySet(nightClubGalaxyIntId, "DJ_04_Lights_04"); //Galaxy Nightclub
    game.deactivateInteriorEntitySet(nightClubGalaxyIntId, "light_rigs_off"); //Galaxy Nightclub
    game.activateInteriorEntitySet(nightClubGalaxyIntId, "Int01_ba_lightgrid_01"); //Galaxy Nightclub
    game.deactivateInteriorEntitySet(nightClubGalaxyIntId, "Int01_ba_Clutter"); //Galaxy Nightclub
    game.activateInteriorEntitySet(nightClubGalaxyIntId, "Int01_ba_equipment_upgrade"); //Galaxy Nightclub
    game.activateInteriorEntitySet(nightClubGalaxyIntId, "Int01_ba_clubname_01"); //Galaxy Nightclub
    game.deactivateInteriorEntitySet(nightClubGalaxyIntId, "Int01_ba_clubname_02"); //Galaxy Nightclub
    game.deactivateInteriorEntitySet(nightClubGalaxyIntId, "Int01_ba_clubname_03"); //Galaxy Nightclub
    game.deactivateInteriorEntitySet(nightClubGalaxyIntId, "Int01_ba_clubname_04"); //Galaxy Nightclub
    game.deactivateInteriorEntitySet(nightClubGalaxyIntId, "Int01_ba_clubname_05"); //Galaxy Nightclub
    game.deactivateInteriorEntitySet(nightClubGalaxyIntId, "Int01_ba_clubname_06"); //Galaxy Nightclub
    game.deactivateInteriorEntitySet(nightClubGalaxyIntId, "Int01_ba_clubname_07"); //Galaxy Nightclub
    game.deactivateInteriorEntitySet(nightClubGalaxyIntId, "Int01_ba_clubname_08"); //Galaxy Nightclub
    game.deactivateInteriorEntitySet(nightClubGalaxyIntId, "Int01_ba_clubname_09"); //Galaxy Nightclub
    game.activateInteriorEntitySet(nightClubGalaxyIntId, "Int01_ba_dry_ice"); //Galaxy Nightclub
    game.deactivateInteriorEntitySet(nightClubGalaxyIntId, "Int01_ba_deliverytruck"); //Galaxy Nightclub
    game.deactivateInteriorEntitySet(nightClubGalaxyIntId, "Int01_ba_trophy04"); //Galaxy Nightclub
    game.deactivateInteriorEntitySet(nightClubGalaxyIntId, "Int01_ba_trophy05"); //Galaxy Nightclub
    game.deactivateInteriorEntitySet(nightClubGalaxyIntId, "Int01_ba_trophy07"); //Galaxy Nightclub
    game.deactivateInteriorEntitySet(nightClubGalaxyIntId, "Int01_ba_trophy09"); //Galaxy Nightclub
    game.deactivateInteriorEntitySet(nightClubGalaxyIntId, "Int01_ba_trophy08"); //Galaxy Nightclub
    game.deactivateInteriorEntitySet(nightClubGalaxyIntId, "Int01_ba_trophy11"); //Galaxy Nightclub
    game.deactivateInteriorEntitySet(nightClubGalaxyIntId, "Int01_ba_trophy10"); //Galaxy Nightclub
    game.deactivateInteriorEntitySet(nightClubGalaxyIntId, "Int01_ba_trophy03"); //Galaxy Nightclub
    game.deactivateInteriorEntitySet(nightClubGalaxyIntId, "Int01_ba_trophy01"); //Galaxy Nightclub
    game.deactivateInteriorEntitySet(nightClubGalaxyIntId, "Int01_ba_trophy02"); //Galaxy Nightclub
    game.deactivateInteriorEntitySet(nightClubGalaxyIntId, "Int01_ba_trad_lights"); //Galaxy Nightclub
    game.deactivateInteriorEntitySet(nightClubGalaxyIntId, "Int01_ba_Worklamps"); //Galaxy Nightclub
    game.refreshInterior(nightClubGalaxyIntId); //Galaxy Nightclub

    // Weed Labor
    alt.requestIpl("bkr_biker_interior_placement_interior_3_biker_dlc_int_ware02_milo");
    game.activateInteriorEntitySet(247297, "weed_upgrade_equip");
    game.activateInteriorEntitySet(247297, "weed_drying");
    game.activateInteriorEntitySet(247297, "weed_security_upgrade");
    game.activateInteriorEntitySet(247297, "weed_production");
    game.activateInteriorEntitySet(247297, "weed_set_up");
    game.activateInteriorEntitySet(247297, "weed_chairs");
    game.activateInteriorEntitySet(247297, "weed_growtha_stage3");
    game.activateInteriorEntitySet(247297, "weed_growthb_stage3");
    game.activateInteriorEntitySet(247297, "weed_growthc_stage3");
    game.activateInteriorEntitySet(247297, "weed_growthd_stage3");
    game.activateInteriorEntitySet(247297, "weed_growthe_stage3");
    game.activateInteriorEntitySet(247297, "weed_growthf_stage3");
    game.refreshInterior(247297);

    // Meth Labor
    alt.requestIpl("bkr_biker_interior_placement_interior_2_biker_dlc_int_ware01_milo");
    game.activateInteriorEntitySet(247041, "meth_lab_upgrade");
    game.activateInteriorEntitySet(247041, "meth_lab_production");
    game.activateInteriorEntitySet(247041, "meth_lab_security_high");
    game.activateInteriorEntitySet(247041, "meth_lab_setup");
    game.refreshInterior(247041);

    alt.requestIpl("tr_tuner_shop_burton");
    alt.requestIpl("tr_tuner_shop_strawberry");
    alt.requestIpl("tr_tuner_shop_rancho");
    alt.requestIpl("tr_tuner_shop_mission");
    alt.requestIpl("tr_tuner_shop_mesa");
    alt.requestIpl("tr_tuner_shop_burton");
    alt.requestIpl("tr_tuner_race_line");
    alt.requestIpl("tr_tuner_meetup");
    
    let TunerInteriorID = game.getInteriorAtCoords(-1350.0, 160.0, -100.0);
    if(game.isValidInterior(TunerInteriorID)) {
        
        game.deactivateInteriorEntitySet(TunerInteriorID, "entity_set_style_1"); // Default Design
        game.deactivateInteriorEntitySet(TunerInteriorID, "entity_set_style_2"); // White Design
        game.activateInteriorEntitySet(TunerInteriorID, "entity_set_style_3"); // Dark Design
        game.deactivateInteriorEntitySet(TunerInteriorID, "entity_set_style_4"); // Concrete Design
        game.deactivateInteriorEntitySet(TunerInteriorID, "entity_set_style_5"); // Home Design
        game.deactivateInteriorEntitySet(TunerInteriorID, "entity_set_style_6"); // Street Design
        game.deactivateInteriorEntitySet(TunerInteriorID, "entity_set_style_7"); // Japan Design
        game.deactivateInteriorEntitySet(TunerInteriorID, "entity_set_style_8"); // Color Design
        game.deactivateInteriorEntitySet(TunerInteriorID, "entity_set_style_9"); // Race Design
        
        
        game.activateInteriorEntitySet(TunerInteriorID, "entity_set_bedroom"); // With Bed room
        game.deactivateInteriorEntitySet(TunerInteriorID, "entity_set_bedroom_empty"); // Bed room is clean
        
        game.activateInteriorEntitySet(TunerInteriorID, "entity_set_table");  // nothing
        game.activateInteriorEntitySet(TunerInteriorID, "entity_set_thermal"); // nothing
        game.activateInteriorEntitySet(TunerInteriorID, "entity_set_tints"); // railing
        game.activateInteriorEntitySet(TunerInteriorID, "entity_set_train"); // nothing
        game.activateInteriorEntitySet(TunerInteriorID, "entity_set_laptop"); // nothing
        game.activateInteriorEntitySet(TunerInteriorID, "entity_set_lightbox"); // lights ceiling
        game.activateInteriorEntitySet(TunerInteriorID, "entity_set_plate"); // nothing
        game.activateInteriorEntitySet(TunerInteriorID, "entity_set_cabinets"); // nothing
        game.activateInteriorEntitySet(TunerInteriorID, "entity_set_chalkboard"); // panel at the top of two rooms in front
        game.deactivateInteriorEntitySet(TunerInteriorID, "entity_set_box_clutter"); // Box

        game.deactivateInteriorEntitySet(TunerInteriorID, "entity_set_car_lift_cutscene"); // Carlift Cutscene
        game.deactivateInteriorEntitySet(TunerInteriorID, "entity_set_car_lift_default"); // Carlift Default
        game.deactivateInteriorEntitySet(TunerInteriorID, "entity_set_car_lift_purchase"); // Carlift Purchase
        
        game.activateInteriorEntitySet(TunerInteriorID, "entity_set_scope"); // nothing
        game.activateInteriorEntitySet(TunerInteriorID, "entity_set_cut_seats"); // Seats in corner
        game.activateInteriorEntitySet(TunerInteriorID, "entity_set_def_table"); // nothing
        game.activateInteriorEntitySet(TunerInteriorID, "entity_set_container"); // nothing
        game.activateInteriorEntitySet(TunerInteriorID, "entity_set_virus"); // nothing
        game.activateInteriorEntitySet(TunerInteriorID, "entity_set_bombs"); // nothing

        game.refreshInterior(TunerInteriorID);
    }

    let CarMeetInteriorID = game.getInteriorAtCoords(-2000.0, 1113.211, -25.36243);
    if (game.isValidInterior(CarMeetInteriorID)) {
        game.activateInteriorEntitySet(CarMeetInteriorID, "entity_set_meet_crew"); // nothing
        game.activateInteriorEntitySet(CarMeetInteriorID, "entity_set_meet_lights"); // activate every light
        game.activateInteriorEntitySet(CarMeetInteriorID, "entity_set_meet_lights_cheap"); // activate every cheap light
        game.activateInteriorEntitySet(CarMeetInteriorID, "entity_set_player"); // nothing
        game.activateInteriorEntitySet(CarMeetInteriorID, "entity_set_test_crew"); // nothing
        game.activateInteriorEntitySet(CarMeetInteriorID, "entity_set_test_lights"); // activate every light on the test race
        game.activateInteriorEntitySet(CarMeetInteriorID, "entity_set_test_lights_cheap"); // activate every cheap light on the test race
        game.activateInteriorEntitySet(CarMeetInteriorID, "entity_set_time_trial"); // activate the white traces on the ground

        game.refreshInterior(CarMeetInteriorID);
    }
}