import { Component, OnInit } from '@angular/core';
import { ClientInstallationInfo } from '../models/clientInstallationInfo';
import { Store } from '@ngrx/store';
import { State } from 'src/app/app.reducer';
import * as reducer from '../instalations.reducer';
import * as actions from '../installations.actions';
import { Observable } from 'rxjs';
import { MatDialog } from '@angular/material/dialog';
import { CurrentProcessComponent, CurrenProcessDialogData } from '../current-process/current-process.component';
import { CurrentProcess, CurrentProcessResult, CurrentProcessType } from '../models/current-process';
import { ClientState, ClientStateService } from '../client-state.service';

@Component({
  selector: 'app-installation-list',
  templateUrl: './installation-list.component.html',
  styleUrls: ['./installation-list.component.scss']
})
export class InstallationListComponent implements OnInit {

  public installations$: Observable<ClientInstallationInfo[]>;
  public currentInstallation$: Observable<string>;
  public currentProcesses$: Observable<CurrentProcess[]>;

  public clientStates = ClientState;

  displayedColumns: string[] = ['clientName', 'version']; // 'installDir'

  constructor(
    private store: Store<State>,
    public dialog: MatDialog,
    public clientStateService: ClientStateService) { }

  ngOnInit() {
    this.installations$ = this.store.select(reducer.allInstallationsSelector);
    this.currentInstallation$ = this.store.select(reducer.currentInstallationIdSelector);
    this.currentProcesses$ = this.store.select(reducer.currentProcessesSelector);

    this.store.dispatch(actions.loadInstallations());
  }

  setCurrentInstallation(row: ClientInstallationInfo) {
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
