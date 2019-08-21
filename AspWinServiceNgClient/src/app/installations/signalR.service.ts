import { Injectable } from '@angular/core';
import * as signalR from '@aspnet/signalr';
import { Store } from '@ngrx/store';
import { State } from '../app.reducer';
import { reportProgress } from './installations.actions';

@Injectable({
  providedIn: 'root'
})
export class SignalRService {

  private hubConnection: signalR.HubConnection;
  private connectionStarted: boolean;

  constructor(private store: Store<State>) { }

  public startConnection = () => {
    if (!this.connectionStarted) {
      this.hubConnection = new signalR.HubConnectionBuilder()
        .withUrl('http://localhost:5000/progresshub')
        .build();

      this.hubConnection.on(
        'reportprogress', (currentProcessId: string, logItemId: string, message: string, progress: number, imageIndex: number) => {
          this.store.dispatch(reportProgress({ payload: { processId: currentProcessId, progress, message, logItemId } }));
        });

      this.hubConnection
        .start()
        .then(() => {
          console.log('Connection started');
          this.connectionStarted = true;
        })
        .catch(err => console.log('Error while starting connection: ' + err));
    }
  }
}


