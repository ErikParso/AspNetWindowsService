import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { ClientInstallationInfo } from '../models/clientInstallationInfo';
import { Store, select } from '@ngrx/store';
import { State } from 'src/app/app.reducer';
import * as reducer from '../instalations.reducer';
import * as actions from '../installations.actions';

@Component({
  selector: 'app-installation-list',
  templateUrl: './installation-list.component.html',
  styleUrls: ['./installation-list.component.css']
})
export class InstallationListComponent implements OnInit {

  public installations$: Observable<ClientInstallationInfo[]>;

  constructor(private store: Store<State>) { }

  ngOnInit() {
    this.installations$ = this.store.pipe(select(reducer.allInstallationsSelector));
    this.store.dispatch(actions.loadInstallations());
  }

}
