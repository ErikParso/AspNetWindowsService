import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Store } from '@ngrx/store';
import { State } from 'src/app/app.reducer';
import * as actions from '../installations.actions';
import { Observable } from 'rxjs';
import { ClientInstallationInfo } from '../models/clientInstallationInfo';
import * as reducer from '../instalations.reducer';
import { UUID } from 'angular2-uuid';
import { CurrentProcess, CurrentProcessResult } from '../models/current-process';
import { ClientStateService, ClientState } from '../client-state.service';
import { NewClientWizardComponent } from '../new-client-wizard/new-client-wizard.component';

@Component({
  selector: 'app-installation-tools',
  templateUrl: './installation-tools.component.html',
  styleUrls: ['./installation-tools.component.css']
})
export class InstallationToolsComponent implements OnInit {

  public currentInstallation$: Observable<ClientInstallationInfo>;
  public currentProcesses$: Observable<CurrentProcess[]>;

  constructor(
    public dialog: MatDialog,
    public store: Store<State>,
    public clientStateService: ClientStateService) { }

  ngOnInit() {
    this.currentInstallation$ = this.store.select(reducer.currentInstallationSelector);
    this.currentProcesses$ = this.store.select(reducer.currentProcessesSelector);
  }

  runApplication(row: ClientInstallationInfo) {
    this.store.dispatch(new actions.RunClientAction(row.clientName));
  }

  updateClient(row: ClientInstallationInfo) {
    this.store.dispatch(actions.updateClient({ payload: { clientId: row.clientId, updateProcessId: UUID.UUID() } }));
  }

  deleteClient(row: ClientInstallationInfo) {
    this.store.dispatch(actions.deleteClient({ payload: { clientId: row.clientId, deleteProcessId: UUID.UUID() } }));
  }

  addNewClient() {
    const dialogRef = this.dialog.open(NewClientWizardComponent, {
      width: '80%', maxWidth: '500px'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.store.dispatch(actions.installNewClient({
          payload: {
            clientId: UUID.UUID(),
            clientName: result.clientName,
            language: result.language,
            installDir: result.installDir,
            applicationServer: result.applicationServer,
            installationProcessId: UUID.UUID()
          }
        }));
      }
    });
  }

  canRunActions(row: ClientInstallationInfo, currentProcesses: CurrentProcess[]) {
    const clientState = this.clientStateService.getClientState(row, currentProcesses);
    return clientState === ClientState.ready ||
      clientState === ClientState.upgradeSuccessfull ||
      clientState === ClientState.installationSuccessfull ||
      clientState === ClientState.upgradeAvailable;
  }
}
