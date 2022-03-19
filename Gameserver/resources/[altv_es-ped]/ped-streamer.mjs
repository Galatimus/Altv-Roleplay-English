import * as alt from 'alt';
import * as natives from 'natives';


async function loadModel(model) {
    return new Promise(resolve => {

        if (!natives.isModelValid(model))
            return resolve(false);

        if (natives.hasModelLoaded(model))
            return resolve(true);

        natives.requestModel(model);

        let interval = alt.setInterval(() => {
                if (natives.hasModelLoaded(model)) {
                    resolve(true);
                    alt.clearInterval(interval);
                }
            },
            5);
    });
}

class PedStreamer {
    constructor() {
        this.peds = {};
    }

    async addPed(entityId, Model, entityType, pos, Rot) {
        this.removePed(+entityId);
        this.clearPed(+entityId);

        loadModel(natives.getHashKey(Model)).then(() => {
            let handle = natives.createPed(4, natives.getHashKey(Model), pos.x, pos.y, pos.z, Rot.z, false, true);
            natives.setEntityInvincible(handle, true);
            natives.disablePedPainAudio(handle, true);
            natives.freezeEntityPosition(handle, true);
            natives.taskSetBlockingOfNonTemporaryEvents(handle, true);
            let ped = { handle: handle, entityId: entityId, Model: Model, entityType: entityType, pos: pos, Rot: Rot };
            this.peds[entityId] = ped;
            this.setRotation(+entityId, Rot);
        });
    }

    getPed(entityId, entityType) {
        if (this.peds.hasOwnProperty(entityId)) {
            return this.peds[entityId];
        } else return null;
    }

    async restorePed(entityId, entityType) {
        if (this.peds.hasOwnProperty(entityId)) {
            let ped = this.peds[entityId];
            loadModel(natives.getHashKey(ped.Model)).then(() => {
                this.peds[entityId].handle = natives.createPed(4, natives.getHashKey(ped.Model), ped.pos.x, ped.pos.y, ped.pos.z, ped.Rot.z, false, true);
                natives.setEntityInvincible(ped.handle, true);
                natives.disablePedPainAudio(ped.handle, true);
                natives.freezeEntityPosition(ped.handle, true);
                natives.taskSetBlockingOfNonTemporaryEvents(ped.handle, true);
                this.setRotation(+entityId, ped.Rot);
            });
        }
    }

    removePed(entityId, entityType) {
        if (this.peds.hasOwnProperty(entityId)) {
            natives.deletePed(this.peds[entityId].handle);
            this.peds[entityId].handle = null;
        }
    }

    clearPed(entityId, entityType) {
        if (this.peds.hasOwnProperty(entityId)) {
            natives.deletePed(this.peds[entityId].handle);
            delete this.peds[entityId];
        }
    }

    clearAllPeds() {
        for (var ped in this.peds) {
            natives.deletePed(this.peds[ped].handle);
        }
        this.peds = {};
    }


    setRotation(entityId, rot) {
        if (this.peds.hasOwnProperty(entityId)) {
            natives.setEntityRotation(this.peds[entityId].handle, rot.x, rot.y, rot.z, 0, true);
            this.peds[entityId].Rot = rot;
        }
    }

    setPosition(entityId, pos) {
        if (this.peds.hasOwnProperty(entityId)) {
            natives.setEntityCoordsNoOffset(this.peds[entityId].handle, pos.x, pos.y, pos.z, true, true, true);
            this.peds[entityId].pos = pos;
        }
    }

    async setModel(entityId, model) {
        if (this.peds.hasOwnProperty(entityId)) {
            this.peds[entityId].Model = model;
        }
    }
}

export const pedStreamer = new PedStreamer();

alt.on("resourceStop", () => {
    pedStreamer.clearAllPeds();
});