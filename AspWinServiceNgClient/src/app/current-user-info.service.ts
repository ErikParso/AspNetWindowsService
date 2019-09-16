import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CurrentUserInfo } from './model/current-user-info';

@Injectable({
  providedIn: 'root'
})
export class CurrentUserInfoService {

  serviceVersionsUrl = 'http://localhost:5000/api/currentuser';

  constructor(private httpClient: HttpClient) { }

  public getCurrentUserInfo(): Observable<CurrentUserInfo> {
    return this.httpClient.get<CurrentUserInfo>(this.serviceVersionsUrl);
  }
}
