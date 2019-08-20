import { Component, OnInit, Input, Inject } from '@angular/core';
import { CurrentProcess, CurrentProcessType, CurrentProcessResult } from '../models/current-process';
import { Observable } from 'rxjs';
import { State } from 'src/app/app.reducer';
import { Store } from '@ngrx/store';
import * as reducer from '../instalations.reducer';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { clearCurrentProcess } from '../installations.actions';
import { ClientInstallationInfo } from '../models/clientInstallationInfo';

export class CurrenProcessDialogData {
  currentProcessId: string;
}

@Component({
  selector: 'app-current-process',
  templateUrl: './current-process.component.html',
  styleUrls: ['./current-process.component.css']
})
export class CurrentProcessComponent implements OnInit {

  public currentProcesstypes = CurrentProcessType;
  public currentProcessResults = CurrentProcessResult;

  public currentProcess$: Observable<CurrentProcess>;
  public installation$: Observable<ClientInstallationInfo>;

  constructor(
    private store: Store<State>,
    public dialogRef: MatDialogRef<CurrentProcessComponent>,
    @Inject(MAT_DIALOG_DATA) public data: CurrenProcessDialogData) { }

  ngOnInit() {
    this.currentProcess$ = this.store.select(reducer.currentProcessSelector(this.data.currentProcessId));
    this.installation$ = this.store.select(reducer.clientInstallationInfoSelector(this.data.currentProcessId));
  }

  public hide() {
    this.dialogRef.close();
  }

  public close(process: CurrentProcess) {
    this.dialogRef.close();
    this.store.dispatch(clearCurrentProcess({ payload: process.processId }));
  }

  getProgressMessage(currentProcess: CurrentProcess): string {
    if (currentProcess.result === CurrentProcessResult.running) {
      return (currentProcess.log && currentProcess.log.length)
        ? currentProcess.log[currentProcess.log.length - 1].content
        : '';
    }

    switch (currentProcess.processType) {
      case CurrentProcessType.installation: {
        switch (currentProcess.result) {
          case CurrentProcessResult.success:
            return `Installation complete`;
          case CurrentProcessResult.error:
            return `Installation failed`;
        }
        break;
      }
      case CurrentProcessType.upgrade: {
        switch (currentProcess.result) {
          case CurrentProcessResult.success:
            return `Upgrade complete`;
          case CurrentProcessResult.error:
            return `Upgrade failed`;
        }
        break;
      }
      case CurrentProcessType.delete: {
        switch (currentProcess.result) {
          case CurrentProcessResult.success:
            return `Uninstallation complete`;
          case CurrentProcessResult.error:
            return `Unistallation failed`;
        }
        break;
      }
    }
  }

  getTitle(currentProcess: CurrentProcess): string {
    switch (currentProcess.processType) {
      case CurrentProcessType.installation: return 'Client Installation';
      case CurrentProcessType.upgrade: return 'Client Upgrade';
      case CurrentProcessType.delete: return 'Uninstall Client';
    }
  }

  getBody(currentProcess: CurrentProcess, clientInfo: ClientInstallationInfo): string {
    switch (currentProcess.processType) {
      case CurrentProcessType.installation: {
        switch (currentProcess.result) {
          case CurrentProcessResult.running:
            return `The client with name ${clientInfo.clientName} is being installed to the folder ${clientInfo.installDir}.`;
          case CurrentProcessResult.success:
            return `The client with name ${clientInfo.clientName} was installed to the folder ${clientInfo.installDir}.`;
          case CurrentProcessResult.error:
            return `Client ${clientInfo.clientName} installation to folder ${clientInfo.installDir} has failed.`;
        }
        break;
      }
      case CurrentProcessType.upgrade: {
        switch (currentProcess.result) {
          case CurrentProcessResult.running:
            return `The client with name ${clientInfo.clientName} is being upgraded.`;
          case CurrentProcessResult.success:
            return `The client with name ${clientInfo.clientName} was successfully upgraded.`;
          case CurrentProcessResult.error:
            return `The client with name ${clientInfo.clientName} upgrade has failed.`;
        }
        break;
      }
      case CurrentProcessType.delete: {
        switch (currentProcess.result) {
          case CurrentProcessResult.running:
            return `Client with name ${clientInfo.clientName} is beeing uninstalled.`;
          case CurrentProcessResult.success:
            return `Client with name ${clientInfo.clientName} was uninstalled.`;
          case CurrentProcessResult.error:
            return `Client with name ${clientInfo.clientName} uninstallation has failed.`;
        }
        break;
      }
    }
  }
}
