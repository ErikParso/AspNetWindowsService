import { Injectable } from '@angular/core';
import { ClientInfo } from '../models/client-info';
import { CurrentProcess, CurrentProcessType, CurrentProcessResult } from '../models/current-process';
import { UpgradeInfo } from '../models/upgrade-info.enum';

export enum ClientState {
  ready = 'ready',
  checkingForUpgrades = 'checking upgrades',
  upgradeNotChecked = 'upgrades may be available',
  upgradeAvailable = 'upgrade available',
  upgradeCheckFailed = 'upgrade check failed',
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
      return this.getClientStateFromUpgradeInfo(row.upgradeInfo);
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

  private getClientStateFromUpgradeInfo(upgradeInfo: UpgradeInfo): ClientState {
    switch (upgradeInfo) {
      case UpgradeInfo.isActual: return ClientState.ready;
      case UpgradeInfo.notChecked: return ClientState.upgradeNotChecked;
      case UpgradeInfo.upgradeAvailable: return ClientState.upgradeAvailable;
      case UpgradeInfo.upgradeCheckFailed: return ClientState.upgradeCheckFailed;
      case UpgradeInfo.checkingForUpgrade: return ClientState.checkingForUpgrades;
    }
  }
}
