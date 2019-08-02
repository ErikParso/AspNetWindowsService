import { Component, OnInit } from '@angular/core';
import { ElectronService } from 'ngx-electron';
import { ChildProcessService } from 'ngx-childprocess';
import { Observable } from 'rxjs';
import { Store, select } from '@ngrx/store';
import { State, localVersionSelector, loadVersions } from './app.reducer';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {

  public version$: Observable<string>;

  constructor(
    public electronService: ElectronService,
    public childProcessService: ChildProcessService,
    private store: Store<State>) { }

  ngOnInit(): void {
    this.version$ = this.store.pipe(select(localVersionSelector));
    this.store.dispatch(loadVersions());
  }

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
