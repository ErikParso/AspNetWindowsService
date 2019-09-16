import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { ElectronService } from 'ngx-electron';
import { StepOneComponent } from './step-one/step-one.component';
import { Store } from '@ngrx/store';
import { State } from 'src/app/app.reducer';
import { UUID } from 'angular2-uuid';
import * as actions from '../installations.actions';
import { StepThreeComponent } from './step-three/step-three.component';
import { StepTwoComponent } from './step-two/step-two.component';
import { HegiService } from '../hegi.service';
import { HegiDescriptor, ClientExistsAction, InstallationScope, TypeExec } from '../models/hegi-descriptor';
import { ActivatedRoute, Router } from '@angular/router';
import { ClientInstallationInfo } from '../models/clientInstallationInfo';
import { allInstallationsSelector } from '../instalations.reducer';
import { MatDialog } from '@angular/material/dialog';
import { MessageBoxComponent } from 'src/app/shared/message-box/message-box.component';
import { InstallExistingClientService } from '../install-existing-client.service';

@Component({
  selector: 'app-new-client-wizard',
  templateUrl: './new-client-wizard.component.html',
  styleUrls: ['./new-client-wizard.component.scss']
})
export class NewClientWizardComponent {

  private allInstallations: ClientInstallationInfo[];

  constructor(
    private store: Store<State>,
    private hegiService: HegiService,
    private dialog: MatDialog,
    private installExistingClientService: InstallExistingClientService,
    private router: Router) {

    store.select(allInstallationsSelector).subscribe(i => this.allInstallations = i);
  }

  install(step1comp: StepOneComponent, step2comp: StepTwoComponent, step3comp: StepThreeComponent) {
    this.installExistingClientService.installExistingClient(step1comp.clientName.value)
      .subscribe(res => {
        if (res) {
          this.store.dispatch(actions.installNewClient({
            payload: {
              clientId: UUID.UUID(),
              clientName: step1comp.clientName.value,
              language: step1comp.language.value,
              installDir: step1comp.installDir.value,
              applicationServer: step1comp.applicationServer.value,
              installationProcessId: UUID.UUID(),
              configName: step2comp.configName.value,
              config: step2comp.defaultConfigValues,
              desktopIcon: step3comp.createIcon.value,
              installForAllUsers: step1comp.installForAll.value,
              lnkForAllUser: step3comp.lnkForAllUsers.value,
              runAfterInstall: step3comp.runAfterInstall.value
            }
          }));
          this.router.navigate(['installations', 'list']);
        }
      });
  }

  selectionChanged($event: any, step1: StepOneComponent, step2: StepTwoComponent, step3: StepThreeComponent) {
    if ($event.selectedIndex === 2) {
      step3.isInstallForAll = step1.installForAll.value;
      if (!step3.isInstallForAll) {
        step3.frmStepThree.get('lnkForAllUsers').setValue(false);
      }
    }
  }

  downloadHegi(step1: StepOneComponent, step2: StepTwoComponent, step3: StepThreeComponent) {
    const hegiDesc = this.createHegiDesc(step1, step2, step3);
    this.hegiService.downloadHegi(hegiDesc);
  }

  createHegiDesc(step1: StepOneComponent, step2: StepTwoComponent, step3: StepThreeComponent): HegiDescriptor {
    return {
      applicationServer: step1.applicationServer.value,
      clientExists: ClientExistsAction.dialog,
      clientName: step1.clientName.value,
      configName: step2.configName.value,
      desktopIcon: step3.createIcon.value,
      installScope: step1.installForAll.value ? InstallationScope.perMachine : InstallationScope.perUser,
      hideWizard: false,
      lnkForAllUser: step3.lnkForAllUsers.value,
      typeExec: TypeExec.addInstall,
      config: step2.defaultConfigValues,
      language: step1.language.value,
      installDir: step1.installForAll ? step1.installDir.value : ''
    };
  }
}
