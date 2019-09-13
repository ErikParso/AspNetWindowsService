import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { ClientInstallationInfo } from './models/clientInstallationInfo';
import { SignalRService } from './signalR.service';
import { ElectronService } from 'ngx-electron';

@Injectable({
  providedIn: 'root'
})
export class InstallationsService {

  clientInstallationsUrl = 'http://localhost:5000/api/client';

  constructor(
    private httpClient: HttpClient,
    private signalRService: SignalRService,
    private electronService: ElectronService) { }

  public getInstallations(): Observable<ClientInstallationInfo[]> {
    return this.httpClient.get<ClientInstallationInfo[]>(this.clientInstallationsUrl + '/true');
  }

  public getClientNeedUpgrade(clientId: string): Observable<boolean> {
    return this.httpClient.get<boolean>(this.clientInstallationsUrl + `/needUpgrade/${clientId}`);
  }

  public runClientApplication(clientName: string): Observable<object> {
    return this.httpClient.post(this.clientInstallationsUrl + '/runClient', { clientName });
  }

  public installNewClient(
    clientId: string,
    clientName: string,
    language: string,
    installDir: string,
    applicationServer: string,
    installationProcessId: string): Observable<ClientInstallationInfo> {
    this.signalRService.startConnection();
    return this.httpClient.post<ClientInstallationInfo>(this.clientInstallationsUrl,
      { clientId, clientName, language, installDir, applicationServer, installationProcessId });
  }

  public updateClient(clientId: string, updateProcessId: string): Observable<ClientInstallationInfo> {
    this.signalRService.startConnection();
    return this.httpClient.put<ClientInstallationInfo>(this.clientInstallationsUrl, { clientId, updateProcessId });
  }

  public deleteClient(clientId: string, deleteProcessId: string): Observable<ClientInstallationInfo[]> {
    return this.httpClient.request<ClientInstallationInfo[]>(
      'delete', this.clientInstallationsUrl, { body: { clientId, deleteProcessId } });
  }
}
