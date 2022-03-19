import * as alt from 'alt';
import * as game from 'natives';

let webview,
    vehicle,
    possibleMods,
    installedMods,
    showcasedMods,
    currentPrimaryColors,
    currentSecondaryColors,
    currentUnderfloorColors,
    currentTyreSmokeColors,
    currentPrimaryPaintType,
    currentSecondaryPaintType,
    paintTypeIds = [0, 0, 15,21,118,120],
    viewModeActive,
    paintTypes = ["Normal", "", "Pearl", "Matt", "Metallic", "Chrom"];

alt.onServer("Client:Tuningmenu:OpenMenu", (veh, pMods, iMods, modPrices) => {
    if (webview) {
        webview.destroy();
	    webview = undefined;
    }

    webview = new alt.WebView('http://resource/client/cef/tuningmenu/index.html');
    webview.focus();
    alt.emitServer("Server:CEF:setCefStatus", true);
    alt.showCursor(true);
    alt.toggleGameControls(false);

    vehicle = veh;
    possibleMods = pMods;
    installedMods = iMods;
    showcasedMods = iMods;

    currentPrimaryColors = [parseInt(installedMods[56]), parseInt(installedMods[57]), parseInt(installedMods[58])];
    currentSecondaryColors = [parseInt(installedMods[60]), parseInt(installedMods[61]), parseInt(installedMods[62])];
    currentUnderfloorColors = [parseInt(installedMods[82]), parseInt(installedMods[83]), parseInt(installedMods[84])];
    currentTyreSmokeColors = [parseInt(installedMods[85]), parseInt(installedMods[86]), parseInt(installedMods[87])];
    currentPrimaryPaintType = installedMods[55];
    currentSecondaryPaintType = installedMods[59];

    setTimeout(() => {
        webview.emit("CEF:Tuningmenu:OpenMenu", pMods, iMods, modPrices);
    }, 500);

    webview.on("Client:Tuningmenu:CloseMenu", () => {
        webview.unfocus();
        webview.destroy();
	    webview = undefined;
        alt.emitServer("Server:CEF:setCefStatus", false);
        alt.emitServer("Server:Tuning:resetToNormal", vehicle);
        resetToNormal(vehicle, installedMods)
        alt.showCursor(false);
        alt.toggleGameControls(true);
        vehicle = undefined;

        possibleMods = undefined;
        installedMods = undefined;
        showcasedMods = undefined;
        currentPrimaryColors = undefined;
        currentSecondaryColors = undefined;
        currentUnderfloorColors = undefined;
        currentPrimaryPaintType = undefined;
        currentSecondaryPaintType = undefined;
    }); 

    webview.on("Client:Tuningmenu:ShowcaseColor", (cat, colorR, colorG, colorB) => {
        if (!vehicle) return;
        if (cat == "primary") game.setVehicleCustomPrimaryColour(vehicle, parseInt(colorR), parseInt(colorG), parseInt(colorB));
        else if (cat == "secondary") game.setVehicleCustomSecondaryColour(vehicle, parseInt(colorR), parseInt(colorG), parseInt(colorB));
        else if (cat == "underfloor") game.setVehicleNeonLightsColour(vehicle, parseInt(colorR), parseInt(colorG), parseInt(colorB));
        else if (cat == "tyresmoke") game.setVehicleTyreSmokeColor(vehicle, parseInt(colorR), parseInt(colorG), parseInt(colorB));
    });

    webview.on("Client:Tuningmenu:ShowcasePaintType", (cat, val) => {
        if (!vehicle) return;
        
        alt.log("val: " + val);
        alt.log("new: " + paintTypeIds.indexOf(parseInt(val)));
        if (cat == "primary") {
            game.setVehicleModColor1(vehicle, parseInt(paintTypeIds.indexOf(parseInt(val))), 0, parseInt(showcasedMods[63]));
            alt.setTimeout(() => {
                game.setVehicleCustomPrimaryColour(vehicle, parseInt(currentPrimaryColors[0]), parseInt(currentPrimaryColors[1]), parseInt(currentPrimaryColors[2]));
            }, 50);
        } else if (cat == "secondary") {
            game.setVehicleModColor2(vehicle, parseInt(paintTypeIds.indexOf(parseInt(val))), 0);
            alt.setTimeout(() => {
                game.setVehicleCustomSecondaryColour(vehicle, parseInt(currentSecondaryColors[0]), parseInt(currentSecondaryColors[1]), parseInt(currentSecondaryColors[2]));
            }, 50);
        }
    });

    webview.on("Client:Tuningmenu:ShowcasePart", (type, index) => {
        if (!vehicle) return;

        showcasedMods[type] = index;

        if (type == 18 || type == 20 ||type == 22) {
            if (index == 0) game.toggleVehicleMod(vehicle, type, false);
            else game.toggleVehicleMod(vehicle, type, true);
        } else if (type < 49) game.setVehicleMod(vehicle, parseInt(type), parseInt(index) - 1, false);
        else if (type == 49) game.setVehicleLivery(vehicle, parseInt(index) - 1);
        else if (type == 50) {
            game.setVehicleWheelType(vehicle, parseInt(index));
            game.setVehicleMod(vehicle, 23, parseInt(showcasedMods[51] - 1), false);
        }
        else if (type == 51) game.setVehicleMod(vehicle, 23, parseInt(index) - 1, false);
        else if (type == 52) game.setVehicleExtraColours(vehicle, parseInt(showcasedMods[63]), parseInt(index));
        else if (type == 53) game.setVehicleWindowTint(vehicle, parseInt(index));
        else if (type == 54) game.setVehicleNumberPlateTextIndex(vehicle, parseInt(index));
        else if (type == 63) game.setVehicleExtraColours(vehicle, parseInt(index), parseInt(showcasedMods[52]));
        else if (type == 80) game.setVehicleInteriorColor(vehicle, parseInt(index));
        else if (type == 81) {
            if (index == 0) for (let i = 0; i < 4; i++) game.setVehicleNeonLightEnabled(vehicle, i, false);
            else for (let i = 0; i < 4; i++) game.setVehicleNeonLightEnabled(vehicle, i, true);
        } else if (type == 88) game.setVehicleXenonLightsColor(vehicle, parseInt(index));
    });

    webview.on("Client:Tuningmenu:EquipTuneItem", (type, index) => {
        if (!vehicle) return;

        installedMods[type] = index;
        
        alt.emitServer("Server:Tuningmenu:EquipTuneItem", vehicle, type, index)
    });

    webview.on("Client:Tuningmenu:EquipRGBTuneItem", (type, colorR, colorG, colorB, paintType) => {
        if (!vehicle) return;
        
        if (type == 100) {
            installedMods[55] = paintTypeIds.indexOf(paintType);
            installedMods[56] = colorR;
            installedMods[57] = colorG;
            installedMods[58] = colorB;

            currentPrimaryColors = [parseInt(colorR), parseInt(colorG), parseInt(colorB)];
        } else if (type == 200) {
            installedMods[59] = paintTypeIds.indexOf(paintType);
            installedMods[60] = colorR;
            installedMods[61] = colorG;
            installedMods[62] = colorB;

            currentSecondaryColors = [parseInt(colorR), parseInt(colorG), parseInt(colorB)];
        } else if (type == 300) {
            installedMods[82] = colorR;
            installedMods[83] = colorG;
            installedMods[84] = colorB;

            currentUnderfloorColors = [parseInt(colorR), parseInt(colorG), parseInt(colorB)];
        } else if (type == 400) {
            installedMods[85] = colorR;
            installedMods[86] = colorG;
            installedMods[87] = colorB;

            currentTyreSmokeColors = [parseInt(colorR), parseInt(colorG), parseInt(colorB)];
        }

        alt.emitServer("Server:Tuningmenu:EquipRGBTuneItem", vehicle, type, colorR, colorG, colorB, paintType == 0 ? 0 : paintType)
    });
});

alt.on('keydown', (key) => {
    if (key == 'O'.charCodeAt(0)) {
        if (!webview || !vehicle || viewModeActive) return;

        alt.showCursor(false);
        alt.toggleGameControls(true);
        viewModeActive = true;
        webview.unfocus();
        webview.emit("viewModeActive", true);
    }
});

alt.on('keyup', (key) => {
    if (key == 'O'.charCodeAt(0)) {
        if (!webview || !vehicle || !viewModeActive) return;

        alt.showCursor(true);
        alt.toggleGameControls(false);
        viewModeActive = false;
        webview.focus();
        webview.emit("viewModeActive", false);
    }
});

function resetToNormal(vehicle, mods) {
    let count = 0;
    mods.forEach(index => {
        let type = count;
        count++;
        
        if (type == 18 || type == 20 || type == 22) {
            if (index == 0) game.toggleVehicleMod(vehicle, type, false);
            else game.toggleVehicleMod(vehicle, type, true);
        } else if (type < 49) game.setVehicleMod(vehicle, parseInt(type), parseInt(index) - 1, false);
        else if (type == 49) game.setVehicleLivery(vehicle, parseInt(index) - 1);
        else if (type == 50) {
            alt.log("test1: " + index);
            game.setVehicleWheelType(vehicle, parseInt(index));
            game.setVehicleMod(vehicle, 23, parseInt(installedMods[51]) - 1, false);
        }
        else if (type == 51) { game.setVehicleMod(vehicle, 23, parseInt(index) - 1, false); alt.log("test4: " + index); }
        else if (type == 52) game.setVehicleExtraColours(vehicle, parseInt(installedMods[63]), parseInt(index));
        else if (type == 53) game.setVehicleWindowTint(vehicle, parseInt(index));
        else if (type == 54) game.setVehicleNumberPlateTextIndex(vehicle, parseInt(index));
        else if (type == 63) game.setVehicleExtraColours(vehicle, parseInt(index), parseInt(showcasedMods[52]));
        else if (type == 80) game.setVehicleInteriorColor(vehicle, parseInt(index));
        else if (type == 81) {
            if (index == 0) for (let i = 0; i < 4; i++) game.setVehicleNeonLightEnabled(vehicle, i, false);
            else for (let i = 0; i < 4; i++) game.setVehicleNeonLightEnabled(vehicle, i, true);
        } else if (type == 88) game.setVehicleXenonLightsColor(vehicle, parseInt(index));
    });

    game.setVehicleModColor1(vehicle, parseInt(paintTypes.indexOf(mods[55])), 0, parseInt(mods[63]));
    game.setVehicleModColor2(vehicle, parseInt(paintTypes.indexOf(mods[59])), 0);

    game.setVehicleCustomPrimaryColour(vehicle, parseInt(currentPrimaryColors[0]), parseInt(currentPrimaryColors[1]), parseInt(currentPrimaryColors[2]));
    game.setVehicleCustomSecondaryColour(vehicle, parseInt(currentSecondaryColors[0]), parseInt(currentSecondaryColors[1]), parseInt(currentSecondaryColors[2]));
    game.setVehicleNeonLightsColour(vehicle, parseInt(currentUnderfloorColors[0]), parseInt(currentUnderfloorColors[1]), parseInt(currentUnderfloorColors[2]));
    game.setVehicleTyreSmokeColor(vehicle, parseInt(currentTyreSmokeColors[0]), parseInt(currentTyreSmokeColors[1]), parseInt(currentTyreSmokeColors[2]));
}