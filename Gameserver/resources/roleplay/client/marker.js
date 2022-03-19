import * as alt from 'alt';
import * as game from 'natives';
var markers = [];

alt.onServer("Client:ServerMarkers:LoadAllMarkers", (markerArray) => {
    markerArray = JSON.parse(markerArray);

    for (var i in markerArray) {
        markers.push({
            type: markerArray[i].type,
            x: markerArray[i].posX,
            y: markerArray[i].posY,
            z: markerArray[i].posZ,
            scaleX: markerArray[i].scaleX,
            scaleY: markerArray[i].scaleY,
            scaleZ: markerArray[i].scaleZ,
            red: markerArray[i].red,
            green: markerArray[i].green,
            blue: markerArray[i].blue,
            alpha: markerArray[i].alpha,
            bobUpAndDown: markerArray[i].bobUpAndDown
        });
    }
});

alt.everyTick(() => {
    if (markers.length >= 1) {
        for (var i = 0; i < markers.length; i++) {
            game.drawRect(0, 0, 0, 0, 0, 0, 0, 0, 0);
            game.drawMarker(markers[i].type, markers[i].x, markers[i].y, markers[i].z, 0, 0, 0, 0, 0, 0, markers[i].scaleX, markers[i].scaleY, markers[i].scaleZ, markers[i].red, markers[i].green, markers[i].blue, markers[i].alpha, markers[i].bobUpAndDown, false, 2, false, undefined, undefined, false);
        }
    }
});