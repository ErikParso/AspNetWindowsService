import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Store } from '@ngrx/store';
import { State } from 'src/app/reactive-state/app.reducer';
import * as actions from '../reactive-state/installations/installations.actions';
import { Observable } from 'rxjs';
import { ClientInfo } from '../models/client-info';
import * as reducer from '../reactive-state/installations/instalations.reducer';
import { UUID } from 'angular2-uuid';
import { CurrentProcess, CurrentProcessResult } from '../models/current-process';
import { ClientStateService, ClientState } from '../services/client-state.service';
import { NewClientWizardComponent } from '../new-client-wizard/new-client-wizard.component';
import { UpgradeInfo } from '../models/upgrade-info.enum';

@Component({
  selector: 'app-installation-tools',
  templateUrl: './installation-tools.component.html',
  styleUrls: ['./installation-tools.component.css']
})
export class InstallationToolsComponent implements OnInit {

  public currentInstallation$: Observable<ClientInfo>;
  public currentProcesses$: Observable<CurrentProcess[]>;
  public upgradeInfo = UpgradeInfo;

  constructor(
    public dialog: MatDialog,
    public store: Store<State>,
    public clientStateService: ClientStateService) { }

  ngOnInit() {
    this.currentInstallation$ = this.store.select(reducer.currentInstallationSelector);
    this.currentProcesses$ = this.store.select(reducer.currentProcessesSelector);
  }

  runApplication(row: ClientInfo) {
    this.store.dispatch(actions.runClient({ payload: { clientId: row.clientId } }));
  }

  updateClient(row: ClientInfo) {
    this.store.dispatch(actions.updateClient({ payload: { clientId: row.clientId, updateProcessId: UUID.UUID() } }));
  }

  checkForUpgrades(row: ClientInfo) {
    this.store.dispatch(actions.getClientNeedUpgrade({ payload: { clientId: row.clientId } }));
  }

  deleteClient(row: ClientInfo) {
    this.store.dispatch(actions.deleteClient({ payload: { clientId: row.clientId, deleteProcessId: UUID.UUID() } }));
  }

  canRunActions(row: ClientInfo, currentProcesses: CurrentProcess[]) {
    const clientState = this.clientStateService.getClientState(row, currentProcesses);
    return clientState === ClientState.ready ||
      clientState === ClientState.upgradeSuccessfull ||
      clientState === ClientState.installationSuccessfull ||
      clientState === ClientState.upgradeAvailable ||
      clientState === ClientState.upgradeNotChecked ||
      clientState === ClientState.upgradeCheckFailed;
  }
}
