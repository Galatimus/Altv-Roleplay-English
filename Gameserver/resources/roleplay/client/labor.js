import * as alt from 'alt';
import * as game from 'natives';


export let laborBrowser = null;
let isLaborCEFOpened = false;
let isStorageCEFOpened = false;
let isDynasty8CEFOpened = false;
// let laborBrowser = null;

alt.onServer('Client:HUD:CreateCEF', () => {
    if (laborBrowser == null) {
        laborBrowser = new alt.WebView("http://resource/client/cef/labor/index.html");
        laborBrowser.focus();

        laborBrowser.on("Client:Labor:switchItemToInventory", (name, amount) => {
            alt.emitServer("Server:Labor:switchItemToInventory", name, parseInt(amount));
        });

        laborBrowser.on("Client:Labor:switchItemToLabor", (name, amount) => {
            alt.emitServer("Server:Labor:switchItemToLabor", name, parseInt(amount));
        });

        laborBrowser.on("Client:Labor:destroy", () => {
            isLaborCEFOpened = false;
            alt.showCursor(false);
            game.freezeEntityPosition(alt.Player.local.scriptID, false);
            alt.toggleGameControls(true);
            laborBrowser.unfocus();
            alt.emitServer("Server:CEF:setCefStatus", false);
        });

        // Storage
        laborBrowser.on("Client:Storage:switchItemToStorage", (storageType, identifierId, name, amount) => {
            alt.emitServer("Server:Storage:switchItemToStorage", parseInt(identifierId), name, parseInt(amount));
        });

        laborBrowser.on("Client:Storage:switchItemToInventory", (storageType, identifierId, name, amount) => {
                alt.emitServer("Server:Storage:switchItemToInventory", parseInt(identifierId), name, parseInt(amount));
        });

        laborBrowser.on("Client:Storage:destroy", () => {
            isStorageCEFOpened = false;
            alt.showCursor(false);
            game.freezeEntityPosition(alt.Player.local.scriptID, false);
            alt.toggleGameControls(true);
            laborBrowser.unfocus();
            alt.emitServer("Server:CEF:setCefStatus", false);
        });

        //Dynasty8
        laborBrowser.on("Client:Utilities:locatePos", (x, y) => {
            game.setNewWaypoint(x, y);
        });

        laborBrowser.on("Client:Dynasty:buyStorage", (storageId) => {
            alt.emitServer("Server:Dynasty:buyStorage", parseInt(storageId));
        });

        laborBrowser.on("Client:Dynasty:sellStorage", (storageId) => {
            alt.emitServer("Server:Dynasty:sellStorage", parseInt(storageId));
        });

        laborBrowser.on("Client:Dynasty8:destroy", () => {
            isDynasty8CEFOpened = false;
            alt.showCursor(false);
            game.freezeEntityPosition(alt.Player.local.scriptID, false);
            alt.toggleGameControls(true);
            laborBrowser.unfocus();
            alt.emitServer("Server:CEF:setCefStatus", false);
        });
    }
});

alt.onServer("Client:Dynasty8:create", (type, myItems, freeItems) => {
    if (laborBrowser == null || isDynasty8CEFOpened) return;
    isDynasty8CEFOpened = true;
    laborBrowser.emit("CEF:Dynasty8:openDynasty8HUD", type, myItems, freeItems);
    alt.showCursor(true);
    alt.toggleGameControls(false);
    laborBrowser.focus();
    alt.emitServer("Server:CEF:setCefStatus", true);
});

alt.onServer("Client:Storage:openStorage", (type, id, invItems, storageItems) => {
    if (laborBrowser == null || isStorageCEFOpened) return;
    isLaborCEFOpened = true;
    laborBrowser.emit("CEF:Storage:openStorage", type, id, invItems, storageItems);
    alt.showCursor(true);
    alt.toggleGameControls(false);
    laborBrowser.focus();
    alt.emitServer("Server:CEF:setCefStatus", true);
});

alt.onServer("Client:Labor:openLabor", (invItems, laborItems) => {
    if (laborBrowser == null || isLaborCEFOpened) return;
    isLaborCEFOpened = true;
    laborBrowser.emit("CEF:Labor:openLabor", invItems, laborItems);
    alt.showCursor(true);
    alt.toggleGameControls(false);
    laborBrowser.focus();
    alt.emitServer("Server:CEF:setCefStatus", true);
});
