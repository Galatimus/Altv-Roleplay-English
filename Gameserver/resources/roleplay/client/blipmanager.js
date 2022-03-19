import * as alt from 'alt';
import * as game from 'natives';

alt.onServer("Client:ServerBlips:LoadAllBlips", (blipArray) => {
    blipArray = JSON.parse(blipArray);

    for (var i in blipArray) {
        createBlip(blipArray[i].posX, blipArray[i].posY, blipArray[i].posZ, blipArray[i].sprite, blipArray[i].scale, blipArray[i].color, blipArray[i].shortRange, blipArray[i].name);
    }
});

alt.on("consoleCommand", (name, args) => {
    if (name == "rot") {
        alt.log(`Rotation: ${JSON.stringify(game.getEntityRotation(alt.Player.local.scriptID, 2))}`);
    }
});

alt.onServer("Client:ServerBlips:AddNewBlip", (name, color, scale, shortRange, sprite, X, Y, Z) => {
    createBlip(X, Y, Z, sprite, scale, color, shortRange, name);
});

alt.on('keyup', (key) => { //ToDo: entfernen
    if (key == 'N'.charCodeAt(0)) {
        var waypoint = game.getFirstBlipInfoId(8);

        if (game.doesBlipExist(waypoint)) {
            var coords = game.getBlipInfoIdCoord(waypoint);
            // alt.Player.local.pos = coords;
            var res = game.getGroundZFor3dCoord(coords.x, coords.y, coords.z + 100, undefined, undefined);
            var newZCoord = res + 1;
            alt.emitServer("ServerBlip:TpWayPoint", coords.x, coords.y, newZCoord);
        }
    }
});

function createBlip(X, Y, Z, sprite, scale, color, shortRange, name) {
    const blip = new alt.PointBlip(X, Y, Z);
    blip.sprite = sprite;
    blip.scale = scale;
    blip.color = color;
    blip.shortRange = shortRange;
    blip.name = name;
}