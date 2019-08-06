import { app, BrowserWindow, Tray } from 'electron';
import * as path from 'path';
import * as url from 'url';

const electron = require('electron');

let win: BrowserWindow;
let tray: Tray;

app.on('ready', createWindow);

app.on('activate', () => {
    if (win === null) {
        createWindow();
    }
});

app.on('ready', () => {
    console.log(__dirname);
    tray = new Tray(
        path.join(__dirname, `/../../dist/AspWinServiceNgClient/assets/green.ico`)
    );
    tray.setToolTip('Asp Windows Servcie Angular-Electron Client.');

    tray.on('click', () => {
        win.isVisible() ? win.hide() : win.show();
    });
});

function createWindow() {
    const display = electron.screen.getPrimaryDisplay();
    const screenWidth = display.workAreaSize.width;
    const screenHeight = display.workAreaSize.height;
    const browserWidth = 400;
    const browserHeight = 550;
    const margin = 10;

    win = new BrowserWindow({
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

    win.hide();

    win.loadURL(
        url.format({
            pathname: path.join(__dirname, `/../../dist/AspWinServiceNgClient/index.html`),
            protocol: 'file:',
            slashes: true,
        })
    );

    win.on('closed', () => {
        win = null;
    });

    // win.hide();
}
