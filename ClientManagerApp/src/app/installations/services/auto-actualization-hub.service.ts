import { Injectable } from '@angular/core';
import * as signalR from '@aspnet/signalr';
import { Observable, from } from 'rxjs';
import { flatMap } from 'rxjs/operators';
import { Store } from '@ngrx/store';
import { State } from 'src/app/reactive-state/app.reducer';
import * as actions from '../reactive-state/installations/installations.actions';
import { UpgradeInfo } from '../models/upgrade-info.enum';
import { ClientInfo } from '../models/client-info';

@Injectable({
  providedIn: 'root'
})
export class AutoActualizationRpcService {

  private hubConnection: signalR.HubConnection;

  constructor(private store: Store<State>) {

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5000/autoactualizationhub')
      .build();

    this.hubConnection.on('clientupgradecheck', (clientId: string) => {
      console.log('checking for upgrades ' + clientId);
      store.dispatch(actions.setClientCheckingForUpgrades({ payload: { clientId }}));
    });

    this.hubConnection.on('clientupgradecheckresult', (clientId: string, upgradeInfo: UpgradeInfo, message: string) => {
      console.log('checking for upgrades result: ' + message + ' ' + clientId);
      store.dispatch(actions.setClientCheckingForUpgradesResult({ payload: { clientId, upgradeInfo, message }}));
    });

    this.hubConnection.on('clientautoupgrade', (clientId: string, processId: string) => {
      console.log('started client upgrade ' + clientId + ' ' + processId);
      store.dispatch(actions.setClientUpgrading({ payload: { clientId, upgradeProcessId: processId }}));
    });

    this.hubConnection.on('clientautoupgraderesult', (result: boolean, clientInfo: ClientInfo) => {
      console.log('Client upgrade result: ' + clientInfo.upgradeInfo);
      store.dispatch(actions.setClientUpgradingResult({ payload: { result, clientInfo }}));
    });
  }

  public startConnection(): Observable<string> {

    const promise = this.hubConnection
      .start()
      .then(() => {
        const promiseGetConnectionId = this.hubConnection
          .invoke('getconnectionid')
          .then(c => c as string);
        const connectionId$ = from(promiseGetConnectionId);
        return connectionId$;
      });

    const observable$ = from(promise).pipe(
      flatMap(r => r)
    );
    return observable$;
  }

}
