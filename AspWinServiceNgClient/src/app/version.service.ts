import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class VersionService {

  serviceVersionsUrl = 'http://localhost:5000/api/installer';

  constructor(private httpClient: HttpClient) { }

  public getServiceVersions(): Observable<{local: string, latest: string, client: string}> {
    return this.httpClient.get<{local: string, latest: string, client: string}>(this.serviceVersionsUrl);
  }

  public downloadInstaller(): Observable<string> {
    return this.httpClient.post(this.serviceVersionsUrl, null, { responseType: 'text' });
  }
}
