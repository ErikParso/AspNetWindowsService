import { Component } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';

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
}
