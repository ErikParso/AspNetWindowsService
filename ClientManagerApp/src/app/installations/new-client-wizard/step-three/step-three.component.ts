import { Component } from '@angular/core';
import { FormGroup, FormBuilder, FormControl } from '@angular/forms';
import { HegiService } from '../../services/hegi.service';

@Component({
  selector: 'app-step-three',
  templateUrl: './step-three.component.html',
  styleUrls: ['./step-three.component.css']
})
export class StepThreeComponent {

  public frmStepThree: FormGroup;

  public isInstallForAll: boolean;

  constructor(
    fb: FormBuilder,
    hegiService: HegiService) {
    this.frmStepThree = fb.group({
      createIcon: [hegiService.hegiDescriptor ? hegiService.hegiDescriptor.desktopIcon : true],
      lnkForAllUsers: [hegiService.hegiDescriptor ? hegiService.hegiDescriptor.lnkForAllUser : false],
      runAfterInstall: [false]
    });
  }

  get createIcon(): FormControl {
    return this.frmStepThree.get('createIcon') as FormControl;
  }

  get lnkForAllUsers(): FormControl {
    return this.frmStepThree.get('lnkForAllUsers') as FormControl;
  }

  get runAfterInstall(): FormControl {
    return this.frmStepThree.get('runAfterInstall') as FormControl;
  }
}
