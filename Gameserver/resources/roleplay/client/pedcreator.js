import * as alt from 'alt';
import * as game from 'natives';

alt.onServer('Client:Pedcreator:spawnPed', (pedArray) => {
    pedArray = JSON.parse(pedArray);
    for (var i in pedArray) {
        spawnPed(pedArray[i].model, pedArray[i].posX, pedArray[i].posY, pedArray[i].posZ, pedArray[i].rotation);
    }
});

function spawnPed(model, x, y, z, rotation) {
    let modelHash = game.getHashKey(model);
    new Promise((resolve, reject) => {
        if (game.hasModelLoaded(modelHash)) {
            resolve();
        }
        game.requestModel(modelHash);
        const timer = alt.setInterval(() => {
            if (game.hasModelLoaded(modelHash)) {
                alt.clearInterval(timer);
                resolve();
            }
        }, 10);
    }).then(() => {
        let pedHandle = game.createPed(4, modelHash, x, y, z, rotation, false, true);
        game.setEntityInvincible(pedHandle, true);
        game.disablePedPainAudio(pedHandle, true);
        game.freezeEntityPosition(pedHandle, true);
        game.taskSetBlockingOfNonTemporaryEvents(pedHandle, true);
    });
}