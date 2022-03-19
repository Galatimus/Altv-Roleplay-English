import * as alt from 'alt';
import * as game from 'natives';

export let clothesStoreBrowser = null;

let opened = false;

alt.onServer("Client:HUD:CreateCEF", () => {
    if (clothesStoreBrowser == null) {
        clothesStoreBrowser = new alt.WebView("http://resource/client/cef/clothesstore/index.html");

        clothesStoreBrowser.on('Client:Clothesstore:PreviewCloth', (isProp, previewComponentId, previewDrawableId, previewTextureId) => {
            if (!isProp) game.setPedComponentVariation(alt.Player.local.scriptID, parseInt(previewComponentId), parseInt(previewDrawableId), parseInt(previewTextureId), 0); 
            else game.setPedPropIndex(alt.Player.local.scriptID, parseInt(previewComponentId), parseInt(previewDrawableId), parseInt(previewTextureId), false);
        });

        clothesStoreBrowser.on('Client:Clothesstore:BuyCloth', (clothId, isProp) => {
            alt.emitServer("Server:Clothesstore:BuyCloth", clothId, isProp);
        });

        clothesStoreBrowser.on('Client:Clothesstore:SetPerfectTorso', (BestTorsoDrawable, BestTorsoTexture) => {
            alt.emitServer("Server:Clothesstore:SetPerfectTorso", BestTorsoDrawable, BestTorsoTexture);
        });
    }
});

alt.onServer("Client:Clothesstore:OpenMenu", (gender) => {
    if (!opened) {
        opened = true
        alt.showCursor(true);
        alt.toggleGameControls(false);
        clothesStoreBrowser.focus();
    } else {
        opened = false;
        alt.showCursor(false);
        alt.toggleGameControls(true);
        clothesStoreBrowser.unfocus();
    }

    clothesStoreBrowser.emit("CEF:Clothesstore:OpenMenu", gender);
});

