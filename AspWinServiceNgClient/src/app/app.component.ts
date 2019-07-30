import { Component, OnInit, Input, EventEmitter } from '@angular/core';
import { ElectronService } from 'ngx-electron';
import { ChildProcessService } from 'ngx-childprocess';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'AspWinServiceNgClient';

  constructor(
    public electronService: ElectronService,
    public childProcessService: ChildProcessService) { }

  hideWindow() {
    this.electronService.remote.getCurrentWindow().hide();
  }

  closeWindow() {
    this.electronService.remote.getCurrentWindow().close();
  }

  runApplication() {
    const executablePath = 'C:\\ProgramData\\Asseco Solutions\\NorisWin32Clients\\Source99-E5\\NorisWin32.exe';
    const params = ['--incognito'];

    if (this.electronService.isElectronApp) {
      this.childProcessService.childProcess.execFile(executablePath, params, null, null);
    } else {
      // TODO: Find how to run .exe from browser and angular app.
    }
  }
}
