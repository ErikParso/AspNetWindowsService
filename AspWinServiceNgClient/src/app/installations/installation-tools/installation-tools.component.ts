import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { NewInstallationComponent } from '../new-installation/new-installation.component';
import { Store } from '@ngrx/store';
import { State } from 'src/app/app.reducer';
import * as actions from '../installations.actions';
import { Observable } from 'rxjs';
import { ClientInstallationInfo } from '../models/clientInstallationInfo';
import { currentInstallationSelector, latestClientVersionSelector } from '../instalations.reducer';

@Component({
  selector: 'app-installation-tools',
  templateUrl: './installation-tools.component.html',
  styleUrls: ['./installation-tools.component.css']
})
export class InstallationToolsComponent implements OnInit {

  public currentInstallation$: Observable<ClientInstallationInfo>;
  public latestClientVersion$: Observable<string>;

  constructor(
    public dialog: MatDialog,
    public store: Store<State>) { }

  ngOnInit() {
    this.currentInstallation$ = this.store.select(currentInstallationSelector);
    this.latestClientVersion$ = this.store.select(latestClientVersionSelector);
  }

  runApplication(row: ClientInstallationInfo) {
    this.store.dispatch(new actions.RunClientAction(row.clientName));
  }

  updateClient(row: ClientInstallationInfo) {
    this.store.dispatch(actions.updateClient({payload: row}));
  }

  deleteClient(row: ClientInstallationInfo) {
    this.store.dispatch(actions.deleteClient({payload: row}));
  }

  addNewClient() {
    const dialogRef = this.dialog.open(NewInstallationComponent, {
      width: '80%', maxWidth: '500px'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.store.dispatch(actions.installNewClient({
          payload: {
            clientName: result.clientName,
            installDir: result.installDir,
            applicationServer: result.applicationServer,
            installationProcessId: 'TODO_GENERATE_GUID'
          }
        }));
      }
    });
  }

  workInProgress(row: ClientInstallationInfo) {
    return row.version === 'installing' || row.version === 'updating' || row.version === 'deleting';
  }
}
