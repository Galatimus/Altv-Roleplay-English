import * as alt from 'alt';
import * as game from 'natives';

export default class Ragdoll {
    constructor(player) {
        this.player = player;
        this.ragdoll = false;

        alt.everyTick(() => {
            if (this.ragdoll) {
                this.doRagdoll();
            }
        });
    }

    /**
     * Start ragdoll
     */
    start() {
        this.doRagdoll();
    }

    /**
     * Stop ragdoll
     */
    stop() {
        this.setRagdoll(false);
        this.setShift(false);
    }

    /**
     * Set shift key pressed
     *
     * @param value
     */
    setShift(value) {
        this.shift = value;
    }

    /**
     * Set ragdoll key pressed
     *
     * @param value
     */
    setRagdoll(value) {
        this.ragdoll = value;
    }

    doRagdoll() {
        // is player in any vehicle?
        if (game.isPedInAnyVehicle(this.player.scriptID, false)) {

            // prevent ragdoll if vehicle isn't a bike
            if (!game.isPedOnAnyBike(this.player.scriptID)) {
                return;
            }

        } else {

            // player isn't in any vehicle. does it have any weapon?
            const currentWeapon = game.getSelectedPedWeapon(this.player.scriptID);

            // prevent if player has weapon and isn't jumping, climbing or falling
            if (this.ragdoll === false) {
                if (game.getWeaponClipSize(currentWeapon) > 0 && !game.isPedJumping(this.player.scriptID) && !game.isPlayerClimbing(this.player.scriptID) && !game.isPedFalling(this.player.scriptID)) {
                    return;
                }
            }

        }

        this.setRagdoll(true);
        game.setPedToRagdoll(this.player.scriptID, 1000, 1000, 0, false, false, false);
    }
}