import { Component, OnInit } from '@angular/core';
import { ClientInfo } from '../models/client-info';
import { Store } from '@ngrx/store';
import { State } from 'src/app/reactive-state/app.reducer';
import * as reducer from '../reactive-state/installations/instalations.reducer';
import * as actions from '../reactive-state/installations/installations.actions';
import { Observable } from 'rxjs';
import { MatDialog } from '@angular/material/dialog';
import { CurrentProcessComponent, CurrenProcessDialogData } from '../current-process/current-process.component';
import { CurrentProcess } from '../models/current-process';
import { ClientState, ClientStateService } from '../services/client-state.service';
import { HegiService } from '../services/hegi.service';
import { Router } from '@angular/router';
import { ElectronService } from 'ngx-electron';
import { UpgradeInfo } from '../models/upgrade-info.enum';

@Component({
  selector: 'app-installation-list',
  templateUrl: './installation-list.component.html',
  styleUrls: ['./installation-list.component.scss']
})
export class InstallationListComponent implements OnInit {

  public installations$: Observable<ClientInfo[]>;
  public currentInstallation$: Observable<string>;
  public currentProcesses$: Observable<CurrentProcess[]>;

  public clientStates = ClientState;
  public upgradeInfos = UpgradeInfo;

  displayedColumns: string[] = ['clientName', 'version']; // 'installDir'

  constructor(
    private store: Store<State>,
    public dialog: MatDialog,
    public clientStateService: ClientStateService,
    public hegiService: HegiService,
    public router: Router,
    public electronService: ElectronService) {
  }

  ngOnInit() {
    this.installations$ = this.store.select(reducer.allInstallationsSelector);
    this.currentInstallation$ = this.store.select(reducer.currentInstallationIdSelector);
    this.currentProcesses$ = this.store.select(reducer.currentProcessesSelector);
  }

  setCurrentInstallation(row: ClientInfo) {
    this.store.dispatch(actions.setCurrentInstallation({ payload: row.clientId }));

    if (row.currentProcessId && row.currentProcessId.length) {
      this.dialog.open(CurrentProcessComponent, {
        width: '80%', maxWidth: '500px',
        data: {
          currentProcessId: row.currentProcessId
        } as CurrenProcessDialogData
      });
    }
  }
}
