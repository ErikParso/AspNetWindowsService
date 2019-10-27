import { Component, ViewChild, OnInit } from '@angular/core';
import { StepOneComponent } from './step-one/step-one.component';
import { Store } from '@ngrx/store';
import { State, currentUserNameSelector } from 'src/app/reactive-state/app.reducer';
import { UUID } from 'angular2-uuid';
import * as actions from '../reactive-state/installations/installations.actions';
import { StepThreeComponent } from './step-three/step-three.component';
import { StepTwoComponent } from './step-two/step-two.component';
import { HegiService } from '../services/hegi.service';
import { HegiDescriptor, ClientExistsAction, InstallationScope, TypeExec } from '../models/hegi-descriptor';
import { Router } from '@angular/router';
import { ClientInfo } from '../models/client-info';
import { allInstallationsSelector } from '../reactive-state/installations/instalations.reducer';
import { MatDialog } from '@angular/material/dialog';
import { ClientIdGenerationService } from '../services/install-existing-client.service';
import { STEPPER_GLOBAL_OPTIONS } from '@angular/cdk/stepper';
import { InstallConfigService } from './install-config.service';
import { ClientConfigItem } from '../models/client-config-item';
import { MatStepper, MatStep } from '@angular/material/stepper';

@Component({
  selector: 'app-new-client-wizard',
  templateUrl: './new-client-wizard.component.html',
  styleUrls: ['./new-client-wizard.component.scss'],
  providers: [
    {
      provide: STEPPER_GLOBAL_OPTIONS,
      useValue: { showError: true }
    }
  ]
})
export class NewClientWizardComponent implements OnInit {

  private allInstallations: ClientInfo[];
  private configItems: ClientConfigItem[];
  private currentUserName: string;

  constructor(
    private store: Store<State>,
    private hegiService: HegiService,
    private installExistingClientService: ClientIdGenerationService,
    private router: Router,
    private installConfigService: InstallConfigService) {

    store.select(allInstallationsSelector).subscribe(i => this.allInstallations = i);
    store.select(currentUserNameSelector).subscribe(res => this.currentUserName = res);
  }

  ngOnInit(): void {
    this.installConfigService.defaultConfigValues$.subscribe(c => this.configItems = c);
  }

  install(step1comp: StepOneComponent, step2comp: StepTwoComponent, step3comp: StepThreeComponent) {
    this.installExistingClientService.getClientId(
      step1comp.clientName.value,
      step1comp.installForAll.value ? InstallationScope.perMachine : InstallationScope.perUser)
      .subscribe(clientId => {
        if (clientId.length) {
          this.store.dispatch(actions.installNewClient({
            payload: {
              clientId,
              clientName: step1comp.clientName.value,
              language: step1comp.language.value,
              installDir: step1comp.installDir.value,
              applicationServer: step1comp.applicationServer.value,
              installationProcessId: UUID.UUID(),
              configName: step2comp.configName.value,
              configItems: this.configItems,
              desktopIcon: step3comp.createIcon.value,
              installForAllUsers: step1comp.installForAll.value,
              lnkForAllUser: step3comp.lnkForAllUsers.value,
              runAfterInstall: step3comp.runAfterInstall.value,
              userName: step1comp.installForAll.value ? '' : this.currentUserName
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
      configItems: this.configItems,
      language: step1.language.value,
      installDir: step1.installForAll ? step1.installDir.value : ''
    };
  }

  stepsValid(step1: StepOneComponent, step2: StepTwoComponent, step3: StepThreeComponent): boolean {
    return step1.frmStepOne.valid && step2.frmStepTwo.valid && step3.frmStepThree.valid;
  }
}
