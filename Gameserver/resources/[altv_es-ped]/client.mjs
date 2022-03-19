import * as alt from 'alt';

import { pedStreamer } from "./ped-streamer";

// when an object is streamed in
alt.onServer("entitySync:create", (entityId, entityType, position, currEntityData) => {
    if (currEntityData) {
        let data = currEntityData;
        if (data != undefined) {
            if (entityType == 6) {
                pedStreamer.addPed(+entityId, data.Model,
                    entityType, position, data.Rotation);
            }
        }
    }
    // this entity has streamed in before, fetch from cache
    else {
        if (entityType == 6) {
            pedStreamer.restorePed(+entityId);
        }
    }
});

// when an object is streamed out
alt.onServer("entitySync:remove", (entityId, entityType) => {
    if (entityType == 6) {
        pedStreamer.removePed(+entityId);
    }
});

// when a streamed in object changes position data
alt.onServer("entitySync:updatePosition", (entityId, entityType, position) => {
    if (entityType == 6) {
        pedStreamer.setPosition(+entityId, position);
    }
});

// when a streamed in object changes data
alt.onServer("entitySync:updateData", (entityId, entityType, newEntityData) => {
    if (entityType == 6) {
        if (newEntityData.hasOwnProperty("Rotation"))
            pedStreamer.setRotation(+entityId, newEntityData.Rotation);

        if (newEntityData.hasOwnProperty("Model"))
            pedStreamer.setModel(+entityId, newEntityData.Model);

    }
});

// when a streamed in object needs to be removed
alt.onServer("entitySync:clearCache", (entityId, entityType) => {
    if (entityType == 6) {
        pedStreamer.clearPed(+entityId);
    }
});