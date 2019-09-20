import { Injectable } from '@angular/core';
import { MethodParams } from './model/method-params';
import * as signalR from '@aspnet/signalr';

@Injectable({
  providedIn: 'root'
})
export class RpcService {

  private hubConnection: signalR.HubConnection;
  private connectionStarted: boolean;
  private connectionId: string;

  constructor() { }

  public startConnection = () => {
    if (!this.connectionStarted) {
      this.hubConnection = new signalR.HubConnectionBuilder()
        .withUrl('http://localhost:5000/rpc')
        .build();

      this.hubConnection.on(
        'methodcall', (methodParams: MethodParams) => {
          console.log('request from server: ' + methodParams.methodCallId);
          this.hubConnection.invoke('methodresponsehandler', { methodCallId: methodParams.methodCallId });
        });

      this.hubConnection
        .start()
        .then(() => {
          console.log('Rpc Connection started');
          this.connectionStarted = true;
          this.hubConnection.invoke('getconnectionid').then((c) => {
            this.connectionId = c;
            console.log(this.connectionId);
          });
        })
        .catch(err => console.log('Error while starting rpc connection: ' + err));
    }
  }

}
