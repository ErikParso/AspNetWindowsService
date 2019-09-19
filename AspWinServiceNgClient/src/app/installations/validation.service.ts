import { Injectable } from '@angular/core';
import { ValidationResult } from './models/validation-result';
import { Observable } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ClientConfigItem } from './models/client-config-item';

@Injectable({
  providedIn: 'root'
})
export class ValidationService {

  versionManagerAddressValidationUrl = 'http://localhost:5000/api/validation/versionmanageraddress';

  constructor(private httpClient: HttpClient) { }

  public validateVersionManagerAddress(versionManagerAddress: string, configItems: ClientConfigItem[]): Observable<ValidationResult> {
    return this.httpClient.post<ValidationResult>(this.versionManagerAddressValidationUrl, { versionManagerAddress, configItems });
  }
}
