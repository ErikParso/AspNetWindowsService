import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { State } from 'src/app/reactive-state/app.reducer';
import { Observable } from 'rxjs';
import { ClientInfo } from '../models/client-info';
import { ActivatedRoute } from '@angular/router';
import { clientInstallationInfoSelectorByClientId } from '../reactive-state/installations/instalations.reducer';

@Component({
  selector: 'app-installation-details',
  templateUrl: './installation-details.component.html',
  styleUrls: ['./installation-details.component.scss']
})
export class InstallationDetailsComponent implements OnInit {

  public clientInfo$: Observable<ClientInfo>;
  public clientId: string;

  constructor(
    private route: ActivatedRoute,
    private store: Store<State>) { }

  ngOnInit() {
    this.route.params.subscribe(p => {
      this.clientId = p.clientId;
      this.clientInfo$ = this.store.select(clientInstallationInfoSelectorByClientId(this.clientId));
    });
  }
}
