import { Component, OnInit } from '@angular/core';
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

@Component({
  selector: 'app-new-client-wizard',
  templateUrl: './new-client-wizard.component.html',
  styleUrls: ['./new-client-wizard.component.scss']
})
export class NewClientWizardComponent implements OnInit {

  constructor(
    public electronService: ElectronService,
    public store: Store<State>,
    public hegiService: HegiService) { }

  ngOnInit() {

  }

  install(step1comp: StepOneComponent) {
    this.store.dispatch(actions.installNewClient({
      payload: {
        clientId: UUID.UUID(),
        clientName: step1comp.clientName.value,
        language: step1comp.language.value,
        installDir: step1comp.installDir.value,
        applicationServer: step1comp.applicationServer.value,
        installationProcessId: UUID.UUID()
      }
    }));
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
      config: step2.defaultConfigValues
    };
  }
}
