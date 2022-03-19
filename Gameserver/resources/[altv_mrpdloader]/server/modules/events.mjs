import * as alt from 'alt';
import * as eventFuncs from './eventFunctions.mjs';

//
alt.on('playerConnect', eventFuncs.playerFirstJoin);

