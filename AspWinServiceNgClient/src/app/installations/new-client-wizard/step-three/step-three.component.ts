import { Component } from '@angular/core';
import { FormGroup, FormBuilder, FormControl } from '@angular/forms';

@Component({
  selector: 'app-step-three',
  templateUrl: './step-three.component.html',
  styleUrls: ['./step-three.component.css']
})
export class StepThreeComponent {

  public frmStepThree: FormGroup;

  public isInstallForAll: boolean;

  constructor(
    private fb: FormBuilder) {
    this.frmStepThree = this.fb.group({
      createIcon: [true],
      lnkForAllUsers: [false],
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
