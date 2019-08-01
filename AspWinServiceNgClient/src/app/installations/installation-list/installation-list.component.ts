import { Component, OnInit } from '@angular/core';
import { ClientInstallationInfo } from '../models/clientInstallationInfo';
import { Store } from '@ngrx/store';
import { State } from 'src/app/app.reducer';
import * as reducer from '../instalations.reducer';
import * as actions from '../installations.actions';

@Component({
  selector: 'app-installation-list',
  templateUrl: './installation-list.component.html',
  styleUrls: ['./installation-list.component.css']
})
export class InstallationListComponent implements OnInit {

  public installations: ClientInstallationInfo[];
  public latestClientVersion: string;

  displayedColumns: string[] = ['clientName', 'version', 'tools']; // 'installDir'

  constructor(private store: Store<State>) { }

  ngOnInit() {
    this.store.select(reducer.allInstallationsSelector)
      .subscribe(data => this.installations = data);
    this.store.select(reducer.latestClientVersionSelector)
      .subscribe(version => this.latestClientVersion = version);

    this.store.dispatch(actions.loadInstallations());
    this.store.dispatch(actions.loadLatestClientVersion());
  }

  setCurrentInstallation(row: ClientInstallationInfo) {
    console.log('row click');
  }

  runApplication(row: ClientInstallationInfo) {
    this.store.dispatch(new actions.RunClientAction(row.clientName));
  }
}
