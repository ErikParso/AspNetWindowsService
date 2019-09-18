import { Injectable } from '@angular/core';
import { ClientInfo } from './models/client-info';
import { CurrentProcess, CurrentProcessType, CurrentProcessResult } from './models/current-process';

export enum ClientState {
  ready = 'ready',
  upgradeAvailable = 'upgrade available',
  installing = 'installing',
  installationSuccessfull = 'installation successfull',
  installationError = 'installation error',
  upgrading = 'upgrading',
  upgradeSuccessfull = 'upgrade successfull',
  upgradeError = 'upgrade error',
  deleting = 'deleting',
  deleteSuccessfull = 'delete successfull',
  deleteError = 'delete error'
}

@Injectable({
  providedIn: 'root'
})
export class ClientStateService {

  constructor() { }

  public getClientState(row: ClientInfo, currentProcesses: CurrentProcess[]): ClientState {
    const process = currentProcesses.find(p => p.processId === row.currentProcessId);
    if (!process) {
      return row.needUpgrade ? ClientState.upgradeAvailable : ClientState.ready;
    } else {
      if (process.processType === CurrentProcessType.installation) {
        if (process.result === CurrentProcessResult.running) {
          return ClientState.installing;
        } else if (process.result === CurrentProcessResult.success) {
          return ClientState.installationSuccessfull;
        } else {
          return ClientState.installationError;
        }
      } else if (process.processType === CurrentProcessType.upgrade) {
        if (process.result === CurrentProcessResult.running) {
          return ClientState.upgrading;
        } else if (process.result === CurrentProcessResult.success) {
          return ClientState.upgradeSuccessfull;
        } else {
          return ClientState.upgrading;
        }
      } else {
        if (process.result === CurrentProcessResult.running) {
          return ClientState.deleting;
        } else if (process.result === CurrentProcessResult.success) {
          return ClientState.deleteSuccessfull;
        } else {
          return ClientState.deleteError;
        }
      }
    }
  }

}
