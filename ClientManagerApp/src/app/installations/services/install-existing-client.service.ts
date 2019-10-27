import { Injectable } from '@angular/core';
import { ClientInfo } from '../models/client-info';
import { Store } from '@ngrx/store';
import { State, currentUserNameSelector } from '../../reactive-state/app.reducer';
import { Observable, of } from 'rxjs';
import { allInstallationsSelector } from '../reactive-state/installations/instalations.reducer';
import { MatDialog } from '@angular/material/dialog';
import { MessageBoxComponent } from '../../shared/message-box/message-box.component';
import { map } from 'rxjs/operators';
import { HegiService } from './hegi.service';
import { ClientExistsAction, InstallationScope } from '../models/hegi-descriptor';
import { ElectronService } from 'ngx-electron';
import { UUID } from 'angular2-uuid';

@Injectable({
  providedIn: 'root'
})
export class ClientIdGenerationService {

  constructor(
    private store: Store<State>,
    private dialog: MatDialog,
    private hegiService: HegiService,
    private electronService: ElectronService) {

    store.select(allInstallationsSelector).subscribe(i => this.allInstallations = i);
    store.select(currentUserNameSelector).subscribe(u => this.currentUserName = u);
  }

  private allInstallations: ClientInfo[] = [];
  private currentUserName: string;

  /**
   * Checks whether there is an client with same parameters installed.
   * If client already installed and user accepts to reinstal, returns installed client id.
   * If client already installed but user cancels installation, returns empty id.
   * If same client not installed, generates new client id.
   * @param clientName New client name.
   * @param installScope New client installation scope.
   */
  getClientId(clientName: string, installScope: InstallationScope): Observable<string> {
    const existingClient = this.findExistingClient(installScope, clientName);
    if (existingClient) {
      if (this.hegiService.hegiDescriptor) {
        if (this.hegiService.hegiDescriptor.clientExists === ClientExistsAction.delete) {
          return of(existingClient.clientId);
        }
        if (this.hegiService.hegiDescriptor.clientExists === ClientExistsAction.end &&
          this.hegiService.hegiDescriptor.hideWizard &&
          this.electronService.isElectronApp) {
          this.electronService.remote.getCurrentWindow().close();
          return of('');
        }
      }
      const dialogRef = this.dialog.open(MessageBoxComponent, {
        width: '80%', maxWidth: '500px',
        data: {
          title: 'Installation',
          message: `Client with name ${clientName} is already installed.
                  Reinstall existing client ?`,
          yesNo: true
        }
      });
      return dialogRef.afterClosed().pipe(
        map(res => res ? existingClient.clientId : '')
      );
    } else {
      return of(UUID.UUID());
    }
  }

  private findExistingClient(installScope: InstallationScope, clientName: string): ClientInfo {
    return this.allInstallations.find(c => this.compareClients(installScope, clientName, c));
  }

  private compareClients(newClientInstallScope: InstallationScope, newClientName: string, existingClient: ClientInfo): boolean {
    const overridingSharedClient = newClientName === existingClient.clientName &&
      newClientInstallScope === InstallationScope.perMachine &&
      (!existingClient.userName || existingClient.userName === '');
    const overridingUserClient = newClientName === existingClient.clientName &&
      newClientInstallScope === InstallationScope.perUser &&
      existingClient.userName === this.currentUserName;
    return overridingSharedClient || overridingUserClient;
  }
}


