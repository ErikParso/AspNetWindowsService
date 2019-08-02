"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var electron_1 = require("electron");
var path = require("path");
var url = require("url");
var electron = require('electron');
var win;
var tray;
electron_1.app.on('ready', createWindow);
electron_1.app.on('activate', function () {
    if (win === null) {
        createWindow();
    }
});
electron_1.app.on('ready', function () {
    console.log(__dirname);
    tray = new electron_1.Tray(path.join(__dirname, "/../../dist/AspWinServiceNgClient/assets/green.ico"));
    tray.setToolTip('Asp Windows Servcie Angular-Electron Client.');
    tray.on('click', function () {
        win.isVisible() ? win.hide() : win.show();
    });
});
function createWindow() {
    var display = electron.screen.getPrimaryDisplay();
    var screenWidth = display.workAreaSize.width;
    var screenHeight = display.workAreaSize.height;
    var browserWidth = 400;
    var browserHeight = 550;
    var margin = 10;
    win = new electron_1.BrowserWindow({
        width: browserWidth,
        height: browserHeight,
        x: screenWidth - browserWidth - margin,
        y: screenHeight - browserHeight - margin,
        frame: false,
        resizable: false,
        alwaysOnTop: true,
        skipTaskbar: true,
        webPreferences: {
            nodeIntegration: true
        }
    });
    win.loadURL(url.format({
        pathname: path.join(__dirname, "/../../dist/AspWinServiceNgClient/index.html"),
        protocol: 'file:',
        slashes: true,
    }));
    win.on('closed', function () {
        win = null;
    });
    // win.hide();
}
//# sourceMappingURL=main.js.map