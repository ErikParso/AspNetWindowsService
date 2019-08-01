import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { ClientInstallationInfo } from './models/clientInstallationInfo';

@Injectable({
  providedIn: 'root'
})
export class InstallationsService {

  clientInstallationsUrl = 'http://localhost:5000/api/client';

  constructor(private httpClient: HttpClient) { }

  public getInstallations(): Observable<ClientInstallationInfo[]> {
    return this.httpClient.get<ClientInstallationInfo[]>(this.clientInstallationsUrl);
  }

  public getLatestClientVersion(): Observable<string> {
    return this.httpClient.get(this.clientInstallationsUrl + '/latestVersion', {responseType: 'text'});
  }

  public runClientApplication(clientName: string): Observable<object> {
    return this.httpClient.post(this.clientInstallationsUrl + '/' + clientName, null);
  }
}
