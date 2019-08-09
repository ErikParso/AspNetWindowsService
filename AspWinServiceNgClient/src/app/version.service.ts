import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { ChildProcessService } from 'ngx-childprocess';
import { ElectronService } from 'ngx-electron';

@Injectable({
  providedIn: 'root'
})
export class VersionService {

  serviceVersionsUrl = 'http://localhost:5000/api/installer';

  constructor(
    private httpClient: HttpClient,
    private childProcessService: ChildProcessService,
    private electronService: ElectronService) { }

  public getServiceVersions(): Observable<{ local: string, latest: string, client: string }> {
    return this.httpClient.get<{ local: string, latest: string, client: string }>(this.serviceVersionsUrl);
  }

  public downloadInstaller(): Observable<string> {
    return this.httpClient.post(this.serviceVersionsUrl, null, { responseType: 'text' });
  }

  runInstallerAndClose(path: string): Observable<boolean> {
    this.childProcessService.childProcess.exec(`\"${path}\"`, [], null);
    new Promise(resolve => setTimeout(resolve, 1000)).then(() => {
      this.electronService.remote.getCurrentWindow().close();
    });
    return of(true);
  }
}
