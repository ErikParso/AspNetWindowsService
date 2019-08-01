import { Component, OnInit } from '@angular/core';
import { ClientInstallationInfo } from '../models/clientInstallationInfo';
import { Store } from '@ngrx/store';
import { State } from 'src/app/app.reducer';
import * as reducer from '../instalations.reducer';
import * as actions from '../installations.actions';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-installation-list',
  templateUrl: './installation-list.component.html',
  styleUrls: ['./installation-list.component.css']
})
export class InstallationListComponent implements OnInit {

  public installations$: Observable<ClientInstallationInfo[]>;
  public latestClientVersion$: Observable<string>;
  public currentInstallations$: Observable<ClientInstallationInfo>;

  displayedColumns: string[] = ['clientName', 'version', 'tools']; // 'installDir'

  constructor(private store: Store<State>) { }

  ngOnInit() {
    this.installations$ = this.store.select(reducer.allInstallationsSelector);
    this.latestClientVersion$ = this.store.select(reducer.latestClientVersionSelector);
    this.currentInstallations$ = this.store.select(reducer.currentInstallationSelector);

    this.store.dispatch(actions.loadInstallations());
    this.store.dispatch(actions.loadLatestClientVersion());
  }

  setCurrentInstallation(row: ClientInstallationInfo) {
    this.store.dispatch(actions.setCurrentInstallation({ payload: row }));
  }

  runApplication(row: ClientInstallationInfo) {
    this.store.dispatch(new actions.RunClientAction(row.clientName));
  }
}
