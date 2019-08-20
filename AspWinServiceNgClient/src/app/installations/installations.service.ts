import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { ClientInstallationInfo } from './models/clientInstallationInfo';
import { SignalRService } from './signalR.service';

@Injectable({
  providedIn: 'root'
})
export class InstallationsService {

  clientInstallationsUrl = 'http://localhost:5000/api/client';

  constructor(
    private httpClient: HttpClient,
    private signalRService: SignalRService) { }

  public getInstallations(): Observable<ClientInstallationInfo[]> {
    return this.httpClient.get<ClientInstallationInfo[]>(this.clientInstallationsUrl);
  }

  public getClientNeedUpgrade(clientId: string): Observable<boolean> {
    return this.httpClient.get<boolean>(this.clientInstallationsUrl + `/needUpgrade/${clientId}`, );
}

  public runClientApplication(clientName: string): Observable<object> {
    return this.httpClient.post(this.clientInstallationsUrl + '/runClient', { clientName });
  }

  public installNewClient(
    clientId: string,
    clientName: string,
    installDir: string,
    applicationServer: string,
    installationProcessId: string): Observable<ClientInstallationInfo> {
    this.signalRService.startConnection();
    return this.httpClient.post<ClientInstallationInfo>(this.clientInstallationsUrl,
      { clientId, clientName, installDir, applicationServer, installationProcessId });
  }

  public updateClient(installDir: string): Observable<ClientInstallationInfo> {
    return this.httpClient.put<ClientInstallationInfo>(this.clientInstallationsUrl, { installDir });
  }

  public deleteClient(clientName: string): Observable<ClientInstallationInfo> {
    return this.httpClient.request<ClientInstallationInfo>('delete', this.clientInstallationsUrl, { body: { clientName } });
  }
}
