import * as alt from 'alt';
import * as game from 'natives';

let charcreatorBrowser = null;
let charcreatorCam = null;
let pedHandle = null;
let modelHash = null;

alt.onServer('Client:Charcreator:CreateCEF', (player) => {
    if (charcreatorBrowser == null) {
        game.freezeEntityPosition(game.playerPedId(), true);
        game.triggerScreenblurFadeOut(0);
        alt.showCursor(true);
        alt.toggleGameControls(false);
        game.setEntityAlpha(alt.Player.local.scriptID, 0, 0);
        game.triggerScreenblurFadeIn(1);
        charcreatorCam = game.createCamWithParams('DEFAULT_SCRIPTED_CAMERA', 402.7, -1003, -98.6, 0, 0, 358, 18, true, 2);
        game.setCamActive(charcreatorCam, true);
        game.renderScriptCams(true, false, 0, true, false, 0);
        charcreatorBrowser = new alt.WebView("http://resource/client/cef/charcreator/index.html");
        charcreatorBrowser.focus();

        charcreatorBrowser.on("Client:Charcreator:ChangeGender", (gender) => {
            game.triggerScreenblurFadeOut(0);
            charcreatorBrowser.emit("CEF:Charcreator:showArea", "creatorarea");

            if (gender == 0 || gender == false) {
                spawnCreatorPed(false);
            } else if (gender == 1 || gender == true) {
                spawnCreatorPed(true);
            }
        });//DONE

        charcreatorBrowser.on("Client:Charcreator:cefIsReady", () => {
            alt.setTimeout(function() {
                charcreatorBrowser.emit("CEF:Charcreator:showArea", "sexarea");
            }, 1000);
        });//DONE

        charcreatorBrowser.on("Client:Charcreator:SetRotation", (rot) => {
            game.setEntityHeading(pedHandle, rot);
        });//DONE

        charcreatorBrowser.on("Client:Charcreator:UpdateFaceFeature", (facefeaturesdata) => {
            let facefeatures = JSON.parse(facefeaturesdata);

            for (let i = 0; i < 20; i++) {
                game.setPedFaceFeature(pedHandle, i, parseFloat(facefeatures[i]));
            }
        });//DONE

        charcreatorBrowser.on("Client:Charcreator:UpdateHeadBlends", (headblendsdata) => {
            let headblends = JSON.parse(headblendsdata);
            game.setPedHeadBlendData(pedHandle, parseInt(headblends[0]), parseInt(headblends[1]), 0, parseInt(headblends[2]), parseInt(headblends[5]), 0, parseFloat(headblends[3]), parseInt(headblends[4]), 0, true);
        });//DONE

        charcreatorBrowser.on("Client:Charcreator:UpdateHeadOverlays", (headoverlaysarray) => {
            let headoverlays = JSON.parse(headoverlaysarray);
            game.setPedHeadOverlayColor(pedHandle, 1, 1, parseInt(headoverlays[2][1]), 1);
            game.setPedHeadOverlayColor(pedHandle, 2, 1, parseInt(headoverlays[2][2]), 1);
            game.setPedHeadOverlayColor(pedHandle, 5, 2, parseInt(headoverlays[2][5]), 1);
            game.setPedHeadOverlayColor(pedHandle, 8, 2, parseInt(headoverlays[2][8]), 1);
            game.setPedHeadOverlayColor(pedHandle, 10, 1, parseInt(headoverlays[2][10]), 1);
            game.setPedEyeColor(pedHandle, parseInt(headoverlays[0][14]));
            game.setPedHeadOverlay(pedHandle, 0, parseInt(headoverlays[0][0]), parseInt(headoverlays[1][0]));
            game.setPedHeadOverlay(pedHandle, 1, parseInt(headoverlays[0][1]), parseFloat(headoverlays[1][1]));
            game.setPedHeadOverlay(pedHandle, 2, parseInt(headoverlays[0][2]), parseFloat(headoverlays[1][2]));
            game.setPedHeadOverlay(pedHandle, 3, parseInt(headoverlays[0][3]), parseInt(headoverlays[1][3]));
            game.setPedHeadOverlay(pedHandle, 4, parseInt(headoverlays[0][4]), parseInt(headoverlays[1][4]));
            game.setPedHeadOverlay(pedHandle, 5, parseInt(headoverlays[0][5]), parseInt(headoverlays[1][5]));
            game.setPedHeadOverlay(pedHandle, 6, parseInt(headoverlays[0][6]), parseInt(headoverlays[1][6]));
            game.setPedHeadOverlay(pedHandle, 7, parseInt(headoverlays[0][7]), parseInt(headoverlays[1][7]));
            game.setPedHeadOverlay(pedHandle, 8, parseInt(headoverlays[0][8]), parseInt(headoverlays[1][8]));
            game.setPedHeadOverlay(pedHandle, 9, parseInt(headoverlays[0][9]), parseInt(headoverlays[1][9]));
            game.setPedHeadOverlay(pedHandle, 10, parseInt(headoverlays[0][10]), parseInt(headoverlays[1][10]));
            game.setPedComponentVariation(pedHandle, 2, parseInt(headoverlays[0][13]), 0, 0);
            game.setPedHairColor(pedHandle, parseInt(headoverlays[2][13]), parseInt(headoverlays[1][13]));
        });

        charcreatorBrowser.on("Client:Charcreator:SaveCharacter", (vorname, nachname, birthdate, gender, facefeaturesarray, headblendsdataarray, headoverlaysarray, clothesarray) => {
            game.clearPedProp(game.playerPedId(), 0);
            game.clearPedProp(game.playerPedId(), 1);
            game.clearPedProp(game.playerPedId(), 2);
            game.clearPedProp(game.playerPedId(), 6);
            game.clearPedProp(game.playerPedId(), 7);
            alt.emitServer("Server:Charcreator:CreateCharacter", vorname + " " + nachname, birthdate, gender, facefeaturesarray, headblendsdataarray, headoverlaysarray);
        });
    }
});//DONE

alt.onServer("Client:Charcreator:DestroyCEF", () => {
    destroycharcreatorBrowser();
});

alt.onServer("Client:Charcreator:showError", (msg) => {
    if (charcreatorBrowser != null) {
        charcreatorBrowser.emit("CEF:Charcreator:showError", msg);
    }
});//DONE


alt.onServer("Client:Charcreator:showArea", (area) => {
    if (charcreatorBrowser != null) {
        charcreatorBrowser.emit("CEF:Charcreator:showArea", area);
    }
});

function spawnCreatorPed(gender) {
    if (gender == true) {
        modelHash = game.getHashKey('mp_f_freemode_01');
        game.requestModel(modelHash);
    } else if (gender == false) {
        modelHash = game.getHashKey('mp_m_freemode_01');
        game.requestModel(modelHash);
    }
    let interval = alt.setInterval(function() {
        if (game.hasModelLoaded(modelHash)) {
            alt.clearInterval(interval);
            pedHandle = game.createPed(4, modelHash, 402.778, -996.9758, -100.01465, 0, false, true);
            game.setEntityHeading(pedHandle, 180.0);
            game.setEntityInvincible(pedHandle, true);
            game.disablePedPainAudio(pedHandle, true);
            game.freezeEntityPosition(pedHandle, true);
            game.taskSetBlockingOfNonTemporaryEvents(pedHandle, true);
            game.setPedComponentVariation(pedHandle, 11, 15, 0, 0);
            game.setPedComponentVariation(pedHandle, 8, 15, 0, 0);
            game.setPedComponentVariation(pedHandle, 3, 15, 0, 0);
        }
    }, 0);
}//DONE

let destroycharcreatorBrowser = function() {
    if (charcreatorBrowser != null) {
        charcreatorBrowser.destroy();
    }
    charcreatorBrowser = null;    
    game.renderScriptCams(false, false, 0, true, false, 0);
    game.setCamActive(charcreatorCam, false);
    if (charcreatorCam != null) {
        game.destroyCam(charcreatorCam, true);
    }
    if (pedHandle != null) {
        game.deletePed(pedHandle);
        pedHandle = null;
    }
    charcreatorCam = null;
    alt.showCursor(false);
}//DONE