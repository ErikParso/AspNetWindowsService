import { Component, OnInit } from '@angular/core';
import { ElectronService } from 'ngx-electron';
import { ChildProcessService } from 'ngx-childprocess';
import { Observable } from 'rxjs';
import { Store } from '@ngrx/store';
import * as reducer from './app.reducer';
import { AssociationsService } from './associations.service';
import { MatDialog } from '@angular/material/dialog';
import { MessageBoxComponent } from './shared/message-box/message-box.component';
import { VersionService } from './version.service';
import { UpgradeInfo } from './upgradeInfo.enum';
import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {

  public upgradeState = UpgradeInfo;

  public localVersion$: Observable<string>;
  public latestVersion$: Observable<string>;
  public upgradeState$: Observable<UpgradeInfo>;

  constructor(
    public versionService: VersionService,
    public associationsService: AssociationsService,
    public electronService: ElectronService,
    public childProcessService: ChildProcessService,
    private store: Store<reducer.State>,
    public dialog: MatDialog,
    private router: Router) { }

  ngOnInit(): void {
    if (this.electronService.isElectronApp) {
      console.log(this.electronService.remote.process.argv);
    }

    // const startupFile = this.getStartupFile();
    const startupFile = 'C:\\Users\\eparso\\Desktop\\install.hegi';
    const uriScheme = this.getUriScheme();

    if (startupFile && startupFile.endsWith('.hegi')) {
      this.store.dispatch(reducer.setStartupFile({ payload: startupFile }));
      if (this.electronService.isElectronApp) {
        this.electronService.remote.getCurrentWindow().show();
      }
    } else if (startupFile) {
      this.store.dispatch(reducer.setStartupFile({ payload: startupFile }));
      this.associationsService.openFileInClient(startupFile)
        .subscribe(() => this.electronService.remote.getCurrentWindow().close());
    } else if (uriScheme) {
      this.associationsService.startClientFromUri(uriScheme)
        .subscribe(() => this.electronService.remote.getCurrentWindow().close());
    }

    this.localVersion$ = this.store.select(reducer.localVersionSelector);
    this.latestVersion$ = this.store.select(reducer.latestVersionSelector);
    this.upgradeState$ = this.store.select(reducer.upgradeStateSelector);

    this.store.dispatch(reducer.loadVersions());
    this.store.dispatch(reducer.loadCurrentUserInfo());
  }

  hideWindow() {
    this.electronService.remote.getCurrentWindow().hide();
  }

  closeWindow() {
    this.electronService.remote.getCurrentWindow().close();
  }

  showUpgradeInfo() {
    const dialogRef = this.dialog.open(MessageBoxComponent, {
      width: '80%', maxWidth: '500px',
      data: {
        title: 'Upgrade available',
        message: `There is an upgrade available for this Client Manager.
         Do you want to download and install new version ? Client Manager will restart.`,
        yesNo: true
      }
    });

    dialogRef.afterClosed().subscribe((data) => {
      if (data) {
        this.store.dispatch(reducer.downloadUpgradePackage());
      }
    });
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
}
