import { Injectable } from '@angular/core';
import * as signalR from '@aspnet/signalr';
import { AcceptCertificateRpcRequest } from '../models/accept-certificate-rpc-request';
import { AcceptCertificateRpcResponse } from '../models/accept-certificate-rpc-response';
import { MatDialog } from '@angular/material/dialog';
import { CertificateProblemsDialogComponent } from '../certificate-problems-dialog/certificate-problems-dialog.component';
import { Observable, from } from 'rxjs';
import { flatMap, tap } from 'rxjs/operators';
import { Mutex } from 'await-semaphore';
import { ElectronService } from 'ngx-electron';

@Injectable({
  providedIn: 'root'
})
export class AcceptCertRpcService {

  private hubConnection: signalR.HubConnection;
  private mutex: Mutex;

  constructor(
    private dialog: MatDialog,
    private electronService: ElectronService) {

    this.mutex = new Mutex();

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5000/acceptcert')
      .build();

    this.hubConnection.on(
      'request', (methodId: string, methodParams: AcceptCertificateRpcRequest) => {

        if (this.electronService.isElectronApp) {
          this.mutex.use(() => this.showCertificate$(methodId, methodParams).toPromise());
        } else {
          // synchronization context not working in browser :/
          this.showCertificate$(methodId, methodParams).subscribe();
        }

      });
  }

  private showCertificate$(methodId: string, methodParams: AcceptCertificateRpcRequest): Observable<any> {
    const dialogRef = this.dialog.open(CertificateProblemsDialogComponent, {
      width: '90%', maxWidth: '500px',
      data: methodParams
    });
    return dialogRef.afterClosed().pipe(
      tap(res => this.hubConnection.invoke('methodresponse', methodId, res as AcceptCertificateRpcResponse)));
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
