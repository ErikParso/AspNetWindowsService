import { app, BrowserWindow, Tray } from 'electron';
import * as path from 'path';
import * as url from 'url';

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
    win = new BrowserWindow({ width: 800, height: 600, frame: false, webPreferences: { nodeIntegration: true } });

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
