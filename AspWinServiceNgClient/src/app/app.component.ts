import { Component } from '@angular/core';
import { ElectronService } from 'ngx-electron';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'AspWinServiceNgClient';

  constructor(public electronService: ElectronService) { }

  hideWindow() {
    this.electronService.remote.getCurrentWindow().hide();
  }

  closeWindow() {
    this.electronService.remote.getCurrentWindow().close();
  }
}
