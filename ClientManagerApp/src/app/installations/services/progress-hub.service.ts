import { Injectable } from '@angular/core';
import * as signalR from '@aspnet/signalr';
import { Store } from '@ngrx/store';
import { State } from '../../reactive-state/app.reducer';
import { reportProgress } from '../reactive-state/installations/installations.actions';
import { Observable, from } from 'rxjs';
import { flatMap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class ProgressHubService {

  private hubConnection: signalR.HubConnection;

  constructor(private store: Store<State>) {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5000/progresshub')
      .build();

    this.hubConnection.on(
      'reportprogress', (currentProcessId: string, logItemId: string, message: string, progress: number, imageIndex: number) => {
        this.store.dispatch(reportProgress({ payload: { processId: currentProcessId, progress, message, logItemId } }));
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


