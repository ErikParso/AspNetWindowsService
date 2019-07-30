import { app, BrowserWindow } from 'electron';
import * as path from 'path';
import * as url from 'url';

let win: BrowserWindow;

app.on('ready', createWindow);

app.on('activate', () => {
    if (win === null) {
        createWindow();
    }
});

function createWindow() {
    win = new BrowserWindow({ width: 800, height: 600, frame: false });

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
}
