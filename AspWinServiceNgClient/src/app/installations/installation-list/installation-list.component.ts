import { Component, OnInit } from '@angular/core';
import { ClientInstallationInfo } from '../models/clientInstallationInfo';
import { Store } from '@ngrx/store';
import { State } from 'src/app/app.reducer';
import * as reducer from '../instalations.reducer';
import * as actions from '../installations.actions';
import { Observable } from 'rxjs';
import { MatDialog } from '@angular/material/dialog';
import { MessageBoxComponent } from 'src/app/shared/message-box/message-box.component';
import { CurrentProcessComponent, CurrenProcessDialogData } from '../current-process/current-process.component';

@Component({
  selector: 'app-installation-list',
  templateUrl: './installation-list.component.html',
  styleUrls: ['./installation-list.component.scss']
})
export class InstallationListComponent implements OnInit {

  public installations$: Observable<ClientInstallationInfo[]>;
  public latestClientVersion$: Observable<string>;
  public currentInstallations$: Observable<ClientInstallationInfo>;

  displayedColumns: string[] = ['clientName', 'version']; // 'installDir'

  constructor(
    private store: Store<State>,
    public dialog: MatDialog) { }

  ngOnInit() {
    this.installations$ = this.store.select(reducer.allInstallationsSelector);
    this.latestClientVersion$ = this.store.select(reducer.latestClientVersionSelector);
    this.currentInstallations$ = this.store.select(reducer.currentInstallationSelector);

    this.store.dispatch(actions.loadInstallations());
    this.store.dispatch(actions.loadLatestClientVersion());
  }

  setCurrentInstallation(row: ClientInstallationInfo) {
    this.store.dispatch(actions.setCurrentInstallation({ payload: row }));

    if (row.currentProcessId && row.currentProcessId.length) {
      this.dialog.open(CurrentProcessComponent, {
        width: '80%', maxWidth: '500px',
        data: {
          currentProcessId: row.currentProcessId
        } as CurrenProcessDialogData
      });
    }
  }

  errorClick(row: ClientInstallationInfo) {
    const dialogRef = this.dialog.open(MessageBoxComponent, {
      width: '80%', maxWidth: '500px',
      data: {
        title: 'Installation failed',
        message: `Client ${row.clientName} installation to dorectory ${row.installDir} failed. ${row.errorMessage}`
      }
    });

    dialogRef.afterClosed().subscribe(() => {
      this.store.dispatch(actions.removeCleint({ payload: row }));
    });
  }

  workInProgress(row: ClientInstallationInfo) {
    return row.version === 'installing' || row.version === 'updating' || row.version === 'deleting';
  }
}
