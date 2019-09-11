import { Component, OnInit } from '@angular/core';
import { ElectronService } from 'ngx-electron';
import { StepOneComponent } from './step-one/step-one.component';
import { Store } from '@ngrx/store';
import { State } from 'src/app/app.reducer';
import { UUID } from 'angular2-uuid';
import * as actions from '../installations.actions';
import { StepThreeComponent } from './step-three/step-three.component';
import { StepTwoComponent } from './step-two/step-two.component';
import { InstallationsService } from '../installations.service';

@Component({
  selector: 'app-new-client-wizard',
  templateUrl: './new-client-wizard.component.html',
  styleUrls: ['./new-client-wizard.component.scss']
})
export class NewClientWizardComponent implements OnInit {

  constructor(
    public electronService: ElectronService,
    public store: Store<State>,
    public installationService: InstallationsService) { }

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
    this.installationService.downloadHegi();
  }
}
