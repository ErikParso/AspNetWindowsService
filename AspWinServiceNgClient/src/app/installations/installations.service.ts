import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { ClientInstallationInfo } from './models/clientInstallationInfo';

@Injectable({
  providedIn: 'root'
})
export class InstallationsService {

  clientInstallationsUrl = 'http://localhost:5000/api/client';

  constructor(private httpClient: HttpClient) { }

  public getInstallations(): Observable<ClientInstallationInfo[]> {
    console.log("here");
    return this.httpClient.get<ClientInstallationInfo[]>(this.clientInstallationsUrl);
  }

}
