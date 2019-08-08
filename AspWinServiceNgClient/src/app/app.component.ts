import { Component, OnInit } from '@angular/core';
import { ElectronService } from 'ngx-electron';
import { ChildProcessService } from 'ngx-childprocess';
import { Observable } from 'rxjs';
import { Store, select } from '@ngrx/store';
import { State, localVersionSelector, loadVersions, latestVersionSelector } from './app.reducer';
import { AssociationsService } from './associations.service';
import { MatDialog } from '@angular/material/dialog';
import { MessageBoxComponent } from './shared/message-box/message-box.component';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {

  public localVersion$: Observable<string>;
  public latestVersion$: Observable<string>;

  constructor(
    public associationsService: AssociationsService,
    public electronService: ElectronService,
    public childProcessService: ChildProcessService,
    private store: Store<State>,
    public dialog: MatDialog) { }

  ngOnInit(): void {
    console.log(this.electronService.remote.process.argv);

    const startupFile = this.getStartupFile();
    const uriScheme = this.getUriScheme();

    if (startupFile) {
      this.associationsService.openFileInClient(startupFile)
        .subscribe(() => this.electronService.remote.getCurrentWindow().close());
    }

    if (uriScheme) {
      this.associationsService.startClientFromUri(uriScheme)
        .subscribe(() => this.electronService.remote.getCurrentWindow().close());
    }

    this.localVersion$ = this.store.select(localVersionSelector);
    this.latestVersion$ = this.store.select(latestVersionSelector);
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

  private getStartupFile(): string {
    const filePathRegex = new RegExp('.+\\.[a-zA-Z0-9_]+$');
    return this.electronService.isElectronApp &&
      this.electronService.remote.process.argv.length === 2 &&
      filePathRegex.exec(this.electronService.remote.process.argv[1])
      ? this.electronService.remote.process.argv[1]
      : null;
  }

  private getUriScheme(): string {
    return this.electronService.isElectronApp &&
      this.electronService.remote.process.argv.length === 2 &&
      this.electronService.remote.process.argv[1].startsWith('heliosgreenservice:')
      ? this.electronService.remote.process.argv[1]
      : null;
  }

  showUpgradeInfo() {
    const dialogRef = this.dialog.open(MessageBoxComponent, {
      width: '80%', maxWidth: '500px',
      data: {
        title: 'Upgrade available',
        message: `There is an upgrade available for this Client Manager.
         Do you want to download and install new version ? Client Manager will close.`,
        yesNo: true
      }
    });

    dialogRef.afterClosed().subscribe((data) => {
      if (data) {
        this.electronService.remote.getCurrentWindow().close();
      }
    });
  }
}
