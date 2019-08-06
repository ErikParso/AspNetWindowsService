import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AssociationsService {

  associationsUrl = 'http://localhost:5000/api/association/runClient';

  constructor(private httpclient: HttpClient) { }

  public openFileInClient(filePath: string): Observable<object> {
    return this.httpclient.post<object>(this.associationsUrl, { filePath });
  }
}
