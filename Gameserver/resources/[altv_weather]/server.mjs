import * as alt from 'alt';
import Weather from './weather';

let weatherSync = new Weather("1d78d94d4720a0aa56c41d495e45daf2", "Peine", "DE");

alt.on('consoleCommand', (msg) => {
    switch (msg) {
        case "startWeather":
            weatherSync.startSync();
            break;
        case "stopWeather":
            weatherSync.stopSync();
            break;
        case "currentTemp":
            weatherSync.getTemp();
            break;
        case "currentData":
            weatherSync.getCurrentData();
            break;
        default:
            break;
    }
});