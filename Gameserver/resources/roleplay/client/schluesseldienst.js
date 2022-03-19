import * as alt from 'alt';
import * as game from 'natives';

export let schluesseldienstBrowser = null;

let opened = false;

alt.onServer("Client:HUD:CreateCEF", () => {
    if (schluesseldienstBrowser == null) {
        schluesseldienstBrowser = new alt.WebView("http://resource/client/cef/schluesseldienst/index.html");

        schluesseldienstBrowser.on('Client:schluesseldienst:ReCreateSchluessel', (plate) => {
            alt.emitServer("Server:schluesseldienst:ReCreateSchluessel", plate);
        });

        schluesseldienstBrowser.on('Client:schluesseldienst:DeleteSchluesselCEF', (plate) => {
            alt.emitServer("Server:schluesseldienst:DeleteSchluesselCEF", plate);
        });

        schluesseldienstBrowser.on("Client:schluesseldienst:destroyCEF", () => {
            closeJobcenterCEF();
        });
    }
});

alt.onServer("Client:schluesseldienst:OpenMenu", () => {
    if (!opened) {
        opened = true
        alt.showCursor(true);
        alt.toggleGameControls(false);
        schluesseldienstBrowser.focus();
    } else {
        opened = false;
        alt.showCursor(false);
        alt.toggleGameControls(true);
        schluesseldienstBrowser.unfocus();
    }

    schluesseldienstBrowser.emit("CEF:schluesseldienst:OpenMenu");
});

let closeJobcenterCEF = function() {
    alt.emitServer("Server:CEF:setCefStatus", false);
    game.freezeEntityPosition(alt.Player.local.scriptID, false);
    alt.showCursor(false);
    alt.toggleGameControls(true);
    schluesseldienstBrowser.unfocus();
    opened = false;
}
