import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { ClientInfo } from './models/client-info';
import { SignalRService } from './signalR.service';
import { ElectronService } from 'ngx-electron';
import { ClientInstallationRequest } from './models/ClientInstallationRequest';

@Injectable({
  providedIn: 'root'
})
export class InstallationsService {

  clientInstallationsUrl = 'http://localhost:5000/api/client';

  constructor(
    private httpClient: HttpClient,
    private signalRService: SignalRService,
    private electronService: ElectronService) { }

  public getInstallations(): Observable<ClientInfo[]> {
    return this.httpClient.get<ClientInfo[]>(this.clientInstallationsUrl + '/true');
  }

  public getClientNeedUpgrade(clientId: string): Observable<boolean> {
    return this.httpClient.get<boolean>(this.clientInstallationsUrl + `/needUpgrade/${clientId}`);
  }

  public runClientApplication(clientName: string): Observable<object> {
    return this.httpClient.post(this.clientInstallationsUrl + '/runClient', { clientName });
  }

  public installNewClient(installation: ClientInstallationRequest): Observable<ClientInfo> {
    this.signalRService.startConnection();
    return this.httpClient.post<ClientInfo>(this.clientInstallationsUrl, installation);
  }

  public updateClient(clientId: string, updateProcessId: string): Observable<ClientInfo> {
    this.signalRService.startConnection();
    return this.httpClient.put<ClientInfo>(this.clientInstallationsUrl, { clientId, updateProcessId });
  }

  public deleteClient(clientId: string, deleteProcessId: string): Observable<ClientInfo[]> {
    return this.httpClient.request<ClientInfo[]>(
      'delete', this.clientInstallationsUrl, { body: { clientId, deleteProcessId } });
  }
}
