import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { NewInstallationComponent } from '../new-installation/new-installation.component';
import { Store } from '@ngrx/store';
import { State } from 'src/app/app.reducer';
import * as actions from '../installations.actions';

@Component({
  selector: 'app-installation-tools',
  templateUrl: './installation-tools.component.html',
  styleUrls: ['./installation-tools.component.css']
})
export class InstallationToolsComponent implements OnInit {

  constructor(
    public dialog: MatDialog,
    public store: Store<State>) { }

  ngOnInit() {
  }

  addNewClient() {
    const dialogRef = this.dialog.open(NewInstallationComponent, {
      width: '80%', maxWidth: '500px'
    });

    dialogRef.afterClosed().subscribe(result => {
      this.store.dispatch(actions.InstallNewClient({
        payload: { clientName: result.clientName, installDir: result.clientName }
      }));
    });
  }
}
