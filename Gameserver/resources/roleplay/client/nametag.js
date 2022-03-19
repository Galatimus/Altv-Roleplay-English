import * as alt from 'alt';
import * as game from 'natives';
let isNameTagVisible = false;

alt.everyTick(() => {
    if (!isNameTagVisible || isNameTagVisible == undefined) return;
    let players = alt.Player.all;
    if (players.length > 0) {
        let localPlayer = alt.Player.local;
        let playerPos = game.getEntityCoords(localPlayer.scriptID);

        for (var i = 0; i < players.length; i++) {
            var player = players[i];
            if (!player.hasStreamSyncedMeta("sharedUsername")) continue;
            let playerPos2 = game.getEntityCoords(player.scriptID);
            let distance = game.getDistanceBetweenCoords(playerPos.x, playerPos.y, playerPos.z, playerPos2.x, playerPos2.y, playerPos2.z, true);

            if (distance <= 40.0 && `${player.getStreamSyncedMeta("sharedUsername")}` != `${localPlayer.getStreamSyncedMeta("sharedUsername")}`) {
                let scale = distance / (40 * 40.0);
                if (scale < 0.3)
                    scale = 0.3;

                let screenPos = game.getScreenCoordFromWorldCoord(playerPos2.x, playerPos2.y, playerPos2.z + 1);
                drawText(`${player.getStreamSyncedMeta("sharedUsername")}`, screenPos[1], screenPos[2] - 0.030, scale, 255, 255, 255, 175, true);
            }
        }
    }
});

alt.on('keyup', (key) => {
    if (alt.Player.local.getSyncedMeta("ADMINLEVEL") <= 0) return;
    if (key == 121) { //F10 
        isNameTagVisible = !isNameTagVisible;
    }
});

function drawText(text, x, y, scale, r, g, b, a, outline) {
    game.setTextFont(0);
    game.setTextProportional(0);
    game.setTextScale(scale, scale);
    game.setTextColour(r, g, b, a);
    game.setTextDropShadow(0, 0, 0, 0, 255);
    game.setTextEdge(2, 0, 0, 0, 255);
    game.setTextCentre(1);
    game.setTextDropShadow();

    if (outline) game.setTextOutline();

    game.beginTextCommandDisplayText("STRING");
    game.addTextComponentSubstringUnk(text);
    game.endTextCommandDisplayText(x, y, 0);
};