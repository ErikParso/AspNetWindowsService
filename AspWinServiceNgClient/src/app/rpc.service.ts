import { Injectable } from '@angular/core';
import { RpcLoginRequest } from './model/rpc-login-request';
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
        .withUrl('http://localhost:5000/loginrpc')
        .build();

      this.hubConnection.on(
        'Request', (methodId: string, methodParams: RpcLoginRequest) => {
          console.log('request from server: ' + methodId);
          this.hubConnection.invoke('methodresponse', methodId, {
            userName: 'eParso',
            password: '123',
            domain: 'sigp'
          });
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
