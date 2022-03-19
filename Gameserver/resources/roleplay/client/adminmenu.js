import * as alt from 'alt';
import * as game from 'natives';

export let adminMenuBrowser = null;
export let browserReady = false;
let adminmenu_isopened = false;
let adminmenu_isfocused = false;
let adminmenu_latestonlineactionarray = [];

let spectate_lastpos = null;
let spectate_camera = null;
let spectate_interval = null;

let nametags_tick = null;

let playerblips_allblips = [];
let playerblips_blip = {};
let playerblips_interval = null;

alt.onServer("Client:HUD:CreateCEF", () => {
    if (adminMenuBrowser == null) {
        adminMenuBrowser = new alt.WebView("http://resource/client/cef/adminmenu/index.html");
    }

    adminMenuBrowser.on("Client:AdminMenu:DoAction", (menuaction, info, addinfo, inputvalue) => {
        alt.emitServer("Server:AdminMenu:DoAction", menuaction, info, addinfo, inputvalue);
    });

    adminMenuBrowser.on("Client:AdminMenu:RequestAllOnlinePlayers", () => {
        alt.emitServer("Server:AdminMenu:RequestAllOnlinePlayers");
    });

    adminMenuBrowser.on("Client:AdminMenu:GetPlayerMeta", (username, tarray) => {
        adminmenu_latestonlineactionarray = tarray;
        alt.emitServer("Server:AdminMenu:GetPlayer", "GetPlayerMeta", username, "none");
    });

    adminMenuBrowser.on("Client:AdminMenu:SetMeta", (username, selectedfield) => {
        alt.emitServer("Server:AdminMenu:GetPlayer", "SetMeta", username, selectedfield);
    });
    
    adminMenuBrowser.on("Client:AdminMenu:SetGameControls", (state) => {
        if (state) {
            alt.toggleGameControls(false);
            adminMenuBrowser.focus();
            alt.emitServer("Server:CEF:setCefStatus", true);
            adminmenu_isfocused = true;
        } else {
            alt.toggleGameControls(true);
            adminMenuBrowser.unfocus();
            alt.emitServer("Server:CEF:setCefStatus", false);
            adminmenu_isfocused = false;
        }
    })
});

alt.on('keydown', (key) => {
    if (key === 38) {
        if (!adminmenu_isopened) return;
        adminMenuBrowser.emit("CEF:AdminMenu:ChangeSelectedField", "up");
    } else if (key === 40) {
        if (!adminmenu_isopened) return;
        adminMenuBrowser.emit("CEF:AdminMenu:ChangeSelectedField", "down");
    } else if (key === 13) {
        if (!adminmenu_isopened) return;
        adminMenuBrowser.emit("CEF:AdminMenu:ChangeSelectedMenu");
    } else if (key === 8) { // BACKSPACE
        if (!adminmenu_isopened || adminmenu_isfocused) return;
        adminMenuBrowser.emit("CEF:AdminMenu:ReturnSelectedMenu");
    } else if (key === 120) {
        if (adminmenu_isopened) {
            alt.emitServer("Server:AdminMenu:CloseMenu");
        } else {
            alt.emitServer("Server:AdminMenu:OpenMenu");
        }
    }
});

alt.on('keyup', (key) => {
});

alt.onServer("Client:Adminmenu:OpenMenu", () => {
    if (adminMenuBrowser != null) {
        adminmenu_isopened = true;
        adminMenuBrowser.emit("CEF:AdminMenu:OpenMenu");
        alt.emitServer("Server:CEF:setCefStatus", true);
    }
});

alt.onServer("Client:Adminmenu:CloseMenu", () => {
    if (!adminmenu_isopened) return;
    adminmenu_isopened = false;
    adminMenuBrowser.emit("CEF:AdminMenu:CloseMenu");
    alt.emitServer("Server:CEF:setCefStatus", false);
});

alt.onServer("Client:Adminmenu:ReceiveMeta", (GetPlayerPlayer) => {
    if (!adminmenu_isopened) return;
    var newarray = [];
    var count = 0;

    adminmenu_latestonlineactionarray.forEach(a => {
        var tempmeta = GetPlayerPlayer.getMeta(a);
        if (tempmeta == undefined) { tempmeta = false; GetPlayerPlayer.setMeta(a, false); }
        if (tempmeta != undefined && tempmeta) newarray.push(`yaes${tempmeta}${count}`);
        else if (tempmeta != undefined && !tempmeta) newarray.push(`yeno${tempmeta}${count}`);
        count++;
    });

    adminMenuBrowser.emit("CEF:Adminmenu:ReceiveMeta", newarray)
});

alt.onServer("Client:Adminmenu:SetMetaDef", (GetPlayerPlayer, field) => {
    if (!GetPlayerPlayer.getMeta(field)) GetPlayerPlayer.setMeta(field, true);
    else if (GetPlayerPlayer.getMeta(field)) GetPlayerPlayer.setMeta(field, false);
});

alt.onServer("Client:Adminmenu:CloseMenu", () => {
    if (!adminmenu_isopened) return;
    adminmenu_isopened = false;
    adminMenuBrowser.emit("CEF:AdminMenu:CloseMenu");
    alt.emitServer("Server:CEF:setCefStatus", false);
});

alt.onServer("Client:AdminMenu:SendAllOnlinePlayers", (AllOnlinePlayerArray) => {
    if (!adminmenu_isopened) return;
    adminMenuBrowser.emit("CEF:AdminMenu:AllOnlinePlayerArray", AllOnlinePlayerArray);
});

alt.onServer("Client:AdminMenu:GetWaypointInfo", () => {
    if (!adminmenu_isopened) return;
    const waypoint = game.getFirstBlipInfoId(8);
    if (game.doesBlipExist(waypoint)) {
        let coords = game.getBlipInfoIdCoord(waypoint);
        game.setEntityCoords(alt.Player.local.scriptID, coords.x, coords.y, 150);
        alt.setTimeout(() => {
            const [found, groundZ] = game.getGroundZFor3dCoord(coords.x, coords.y, coords.z + 100, false, false);
            coords = new alt.Vector3(coords.x, coords.y, groundZ + 1);
            alt.emitServer('Server:AdminMenu:TeleportWaypoint', coords.x, coords.y, coords.z);  
        }, 50);
    }
});

alt.onServer("Client:AdminMenu:Noclip", (info) => {
    if (info == "on") NoClip.start();
    else if (info == "off") NoClip.stop();
});

alt.onServer("Client:AdminMenu:Godmode", (info) => {
    if (info == "on") game.setPlayerInvincible(alt.Player.local.scriptID, true);
    else if (info == "off") game.setPlayerInvincible(alt.Player.local.scriptID, false);
});

alt.onServer("Client:AdminMenu:Spectate", (target, info) => {
    if (info == "on") {
        spectate_lastpos = game.getEntityCoords(alt.Player.local.scriptID);
        game.setEntityCoords(alt.Player.local.scriptID, target.pos.x, target.pos.y, target.pos.z - 3);
        alt.setTimeout(() => {
            spectate_camera = game.createCam("DEFAULT_SCRIPTED_CAMERA", true);
            //spectate_camera = game.createCamWithParams('DEFAULT_SCRIPTED_CAMERA', target.pos.x, target.pos.y, target.pos.z, 0, 0, 240, 50, true, 2);
            game.setCamActive(spectate_camera, true);
            game.freezeEntityPosition(alt.Player.local.scriptID, true);
            game.attachCamToEntity(spectate_camera, target.scriptID, 0, -2.0, 1.0, true);
            game.renderScriptCams(true, false, 0, true, false, 0);
            
            spectate_interval = alt.everyTick(() => {
                game.pointCamAtCoord(spectate_camera, target.pos.x, target.pos.y, target.pos.z);
                game.setEntityCoords(alt.Player.local.scriptID, target.pos.x, target.pos.y, target.pos.z - 3);
            });
        }, 30);
    } else if (info == "off") {
        if (spectate_camera == null) return;
        alt.clearEveryTick(spectate_interval);
        game.setCamActive(spectate_camera, false);
        game.renderScriptCams(false, false, 0, false, false, 0);
        game.destroyCam(spectate_camera, true);
        game.setEntityCoords(alt.Player.local.scriptID, spectate_lastpos.x, spectate_lastpos.y, spectate_lastpos.z);
        game.freezeEntityPosition(alt.Player.local.scriptID, false);
        spectate_lastpos, spectate_camera = null;
    }
});

alt.onServer("Client:AdminMenu:SetFreezed", (target, info) => {
    if (info == "on") {
        if (target == alt.Player.local) alt.toggleGameControls(false);
        game.freezeEntityPosition(target.scriptID, true);
    } else if (info == "off") {
        if (target == alt.Player.local) alt.toggleGameControls(true);
        game.freezeEntityPosition(target.scriptID, false);
    }
});

alt.onServer("Client:Adminmenu:ToggleNametags", (info) => {
    if (info == "on") {
        nametags_tick = alt.everyTick(() => {
            for (let i = 0, n = alt.Player.all.length; i < n; i++) {
                let player = alt.Player.all[i];
                if (!player.valid) continue;

                /*if (player.scriptID === alt.Player.local.scriptID) {
                    continue;
                }*/
        
                const name = player.getSyncedMeta('NAME');
                if (!name) continue;
            
                if (!game.hasEntityClearLosToEntity(alt.Player.local.scriptID, player.scriptID, 17)) continue;
            
                let dist = distance2d(player.pos, alt.Player.local.pos);
                if (dist > 25) continue;

                const isChatting = player.getSyncedMeta('CHATTING');
                const pos = { ...game.getPedBoneCoords(player.scriptID, 12844, 0, 0, 0) };
                pos.z += 0.5;
            
                let scale = 1 - (0.8 * dist) / 25;
                let fontSize = 0.6 * scale;
            
                const lineHeight = game.getTextScaleHeight(fontSize, 4);
                const entity = player.vehicle ? player.vehicle.scriptID : player.scriptID;
                const vector = game.getEntityVelocity(entity);
                const frameTime = game.getFrameTime();
            
                game.setDrawOrigin(
                    pos.x + vector.x * frameTime,
                    pos.y + vector.y * frameTime,
                    pos.z + vector.z * frameTime,
                    0
                );
                game.beginTextCommandDisplayText('STRING');
                game.setTextFont(4);
                game.setTextScale(fontSize, fontSize);
                game.setTextProportional(true);
                game.setTextCentre(true);
                game.setTextColour(255, 255, 255, 255);
                game.setTextOutline();
                game.addTextComponentSubstringPlayerName(isChatting ? `${name}~r~*` : `${name}`);
                game.endTextCommandDisplayText(0, 0, 0);
            
                if (!game.isEntityDead(player.scriptID)) {
                    drawBarBackground(100, lineHeight, scale, 0.25, 0, 147, 29, 255);
                    drawBar(game.getEntityHealth(player.scriptID) - 100, lineHeight, scale, 0.25, 0, 224, 11, 255);
            
                    if (game.getPedArmour(player.scriptID) > 0) {
                        drawBarBackground(100, lineHeight, scale, 0.75, 0, 45, 142, 255);
                        drawBar(game.getPedArmour(player.scriptID), lineHeight, scale, 0.75, 0, 74, 234, 255);
                    }
                }
                game.clearDrawOrigin();
            }
        });
    } else if (info == "off") {
        alt.clearEveryTick(nametags_tick);
    }
});

alt.onServer("Client:Adminmenu:TogglePlayerBlips", (info) => {
    if (info == "on") { 
        for (let i = 0, n = alt.Player.all.length; i < n; i++) {
            let player = alt.Player.all[i];
            if (!player.valid) continue;

            const username = player.getSyncedMeta('NAME');
            if (!username) continue;

            playerblips_blip[player.scriptID] = new alt.PointBlip(player.pos.x, player.pos.y, player.pos.z);
            playerblips_blip[player.scriptID].scale = 0.9;
            playerblips_blip[player.scriptID].color = 4;
            playerblips_blip[player.scriptID].name = username;
            playerblips_blip[player.scriptID].dimension = player.dimension;
            playerblips_allblips.push(playerblips_blip[player.scriptID]);
        }

        playerblips_interval = alt.setInterval(() => {
            for (let i = 0, n = alt.Player.all.length; i < n; i++) {
                let player = alt.Player.all[i];
                if (playerblips_blip[player.scriptID] == undefined) {
                    playerblips_blip[player.scriptID] = new alt.PointBlip(player.pos.x, player.pos.y, player.pos.z);
                    playerblips_blip[player.scriptID].scale = 0.9;
                    playerblips_blip[player.scriptID].color = 4;
                    playerblips_blip[player.scriptID].name = username;
                    playerblips_blip[player.scriptID].dimension = player.dimension;
                    playerblips_allblips.push(playerblips_blip[player.scriptID]);
                }
                if (!player.valid || player.scriptID == undefined) {
                    playerblips_blip[player.scriptID].destroy();
                    continue;
                }

                playerblips_blip[player.scriptID].pos = new alt.Vector3(player.pos.x, player.pos.y, player.pos.z);
            }
        }, 500);
    } else if (info == "off") {
        if (playerblips_interval == null) return;
        alt.clearInterval(playerblips_interval);
        for (let i = 0, n = alt.Player.all.length; i < n; i++) {
            let player = alt.Player.all[i];
            if (playerblips_blip[player.scriptID] == undefined) continue;
            playerblips_blip[player.scriptID].destroy();
            const elementindex = playerblips_allblips.indexOf(5);
            if (elementindex > -1) elementindex.splice(index, 1);
        }
        playerblips_blip = {};
        playerblips_allblips.forEach(a => {
            if (a != undefined) a.destroy();
        });
    }
});



/* -------------------------- FUNCTIONS ---------------------------*/

function distance2d(vector1, vector2) {
    return Math.sqrt(Math.pow(vector1.x - vector2.x, 2) + Math.pow(vector1.y - vector2.y, 2));
}

function drawBar(value, lineHeight, scale, position, r, g, b, a) {
    const healthWidth = value * 0.0005 * scale;
    game.drawRect(
        (healthWidth - 100 * 0.0005 * scale) / 2,
        lineHeight + position * lineHeight,
        healthWidth,
        lineHeight / 4,
        r,
        g,
        b,
        a
    );
}

function drawBarBackground(value, lineHeight, scale, position, r, g, b, a) {
    const width = value * 0.0005 * scale;
    game.drawRect(0, lineHeight + position * lineHeight, width + 0.002, lineHeight / 3 + 0.002, 0, 0, 0, 255);
    game.drawRect(0, lineHeight + position * lineHeight, width, lineHeight / 3, r, g, b, a);
}

function addSpeedToVector(vector1, vector2, speed, lr = false) {
    return new alt.Vector3(
        vector1.x + vector2.x * speed,
        vector1.y + vector2.y * speed,
        lr === true ? vector1.z : vector1.z + vector2.z * speed
    );
}

function camVectorForward(camRot) {
    let rotInRad = {
        x: camRot.x * (Math.PI / 180),
        y: camRot.y * (Math.PI / 180),
        z: camRot.z * (Math.PI / 180) + Math.PI / 2,
    };

    let camDir = {
        x: Math.cos(rotInRad.z),
        y: Math.sin(rotInRad.z),
        z: Math.sin(rotInRad.x),
    };

    return camDir;
}

function camVectorRight(camRot) {
    let rotInRad = {
        x: camRot.x * (Math.PI / 180),
        y: camRot.y * (Math.PI / 180),
        z: camRot.z * (Math.PI / 180),
    };

    var camDir = {
        x: Math.cos(rotInRad.z),
        y: Math.sin(rotInRad.z),
        z: Math.sin(rotInRad.x),
    };

    return camDir;
}

function isVectorEqual(vector1, vector2) {
    return (
        vector1.x === vector2.x &&
        vector1.y === vector2.y &&
        vector1.z === vector2.z
    );
}

/* ------------------------- NOCLIP ------------------------- */

export default class NoClip {
    static enabled = false;
    static speed = 1.0;
    static everyTick = null;

    static start() {
        if (NoClip.enabled) return;
        NoClip.enabled = true;
        game.freezeEntityPosition(alt.Player.local.scriptID, true);
        this.everyTick = alt.everyTick(NoClip.keyHandler);
    }
    static stop() {
        if (!NoClip.enabled) return;
        NoClip.enabled = false;
        game.freezeEntityPosition(alt.Player.local.scriptID, false);
        alt.clearEveryTick(this.everyTick);
    }

    static KEYS = {
        FORWARD: 32,
        BACKWARD: 33,
        LEFT: 34,
        RIGHT: 35,
        UP: 22,
        DOWN: 36,
        SHIFT: 21,
    };
    static keyHandler() {
        let currentPos = alt.Player.local.pos;
        let speed = NoClip.speed;
        let rot = game.getGameplayCamRot(2);
        let dirForward = camVectorForward(rot);
        let dirRight = camVectorRight(rot);
        let zModifier = 0;

        if (game.isDisabledControlPressed(0, NoClip.KEYS.SHIFT)) speed = speed * 5;
        if (game.isDisabledControlPressed(0, NoClip.KEYS.FORWARD)) currentPos = addSpeedToVector(currentPos, dirForward, speed);
        if (game.isDisabledControlPressed(0, NoClip.KEYS.BACKWARD)) currentPos = addSpeedToVector(currentPos, dirForward, -speed);
        if (game.isDisabledControlPressed(0, NoClip.KEYS.LEFT)) currentPos = addSpeedToVector(currentPos, dirRight, -speed, true);
        if (game.isDisabledControlPressed(0, NoClip.KEYS.RIGHT)) currentPos = addSpeedToVector(currentPos, dirRight, speed, true);
        if (game.isDisabledControlPressed(0, NoClip.KEYS.UP)) zModifier += speed;
        if (game.isDisabledControlPressed(0, NoClip.KEYS.DOWN)) zModifier -= speed;

       if (!isVectorEqual(new alt.Vector3(currentPos.x, currentPos.y, currentPos.z + zModifier), alt.Player.local.pos)) game.setEntityCoords(alt.Player.local.scriptID, currentPos.x, currentPos.y, (currentPos.z + zModifier - 1), true, false, false, true);
    }
}