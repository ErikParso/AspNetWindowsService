import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ClientInstallationInfo } from './models/clientInstallationInfo';
import { SignalRService } from './signalR.service';
import { saveAs } from 'file-saver';
import { ElectronService } from 'ngx-electron';
import * as FS from 'fs';

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
    return this.httpClient.get<ClientInstallationInfo[]>(this.clientInstallationsUrl);
  }

  public getClientNeedUpgrade(clientId: string): Observable<boolean> {
    return this.httpClient.get<boolean>(this.clientInstallationsUrl + `/needUpgrade/${clientId}`);
  }

  public runClientApplication(clientName: string): Observable<object> {
    return this.httpClient.post(this.clientInstallationsUrl + '/runClient', { clientName });
  }

  public downloadHegi(): void {
    this.httpClient.post(this.clientInstallationsUrl + '/downloadhegi', {}, {
      responseType: 'blob',
      headers: new HttpHeaders().append('Content-Type', 'application/json')
    }).subscribe(data => {
      if (this.electronService.isElectronApp) {
        this.electronService.remote.dialog.showSaveDialog(
          { filters: [{ extensions: ['hegi'], name: 'hegi' } as Electron.FileFilter] } as Electron.SaveDialogOptions).then(
            res => {
              const fileReader = new FileReader();
              fileReader.addEventListener('loadend', e =>
                ((window as any).fs).writeFile(res.filePath, fileReader.result, () => { }));
              fileReader.readAsText(data);
            }
          );
      } else {
        saveAs(data, 'install.hegi');
      }
    });
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

  public deleteClient(clientId: string, deleteProcessId: string): Observable<ClientInstallationInfo> {
    return this.httpClient.request<ClientInstallationInfo>('delete', this.clientInstallationsUrl, { body: { clientId, deleteProcessId } });
  }
}
