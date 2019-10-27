import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ClientInfo } from '../models/client-info';
import { ProgressHubService } from './progress-hub.service';
import { ClientInstallationRequest } from '../models/ClientInstallationRequest';
import { UpgradeInfo } from '../models/upgrade-info.enum';
import { Store } from '@ngrx/store';
import { State } from 'src/app/reactive-state/app.reducer';
import {
  acceptCertRpcHubConnectionIdSelector,
  autoActualizationHubConnectionIdSelector,
  progressHubConnectionIdSelector
} from '../reactive-state/connections/connections.reducer';

@Injectable({
  providedIn: 'root'
})
export class InstallationsService {

  clientInstallationsUrl = 'http://localhost:5000/api/client';

  constructor(private httpClient: HttpClient) {
  }

  public getInstallations(): Observable<ClientInfo[]> {
    return this.httpClient.get<ClientInfo[]>(this.clientInstallationsUrl + `/true/`);
  }

  public getClientNeedUpgrade(clientId: string): Observable<UpgradeInfo> {
    return this.httpClient.get<UpgradeInfo>(this.clientInstallationsUrl + `/needUpgrade/${clientId}`);
  }

  public runClientApplication(clientId: string): Observable<object> {
    return this.httpClient.post(this.clientInstallationsUrl + '/runClient', { clientId });
  }

  public installNewClient(installation: ClientInstallationRequest): Observable<ClientInfo> {
    console.log(installation);
    return this.httpClient.post<ClientInfo>(this.clientInstallationsUrl, installation);
  }

  public updateClient(clientId: string, updateProcessId: string): Observable<ClientInfo> {
    return this.httpClient.put<ClientInfo>(this.clientInstallationsUrl, { clientId, updateProcessId });
  }

  public deleteClient(clientId: string, deleteProcessId: string): Observable<ClientInfo[]> {
    return this.httpClient.request<ClientInfo[]>('delete', this.clientInstallationsUrl, {
      body: { clientId, deleteProcessId }
    });
  }
}
