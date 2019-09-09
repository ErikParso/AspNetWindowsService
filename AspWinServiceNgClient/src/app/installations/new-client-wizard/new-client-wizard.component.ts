import { Component, OnInit } from '@angular/core';
import { ElectronService } from 'ngx-electron';
import { FormGroup } from '@angular/forms';
import { StepOneComponent } from './step-one/step-one.component';

@Component({
  selector: 'app-new-client-wizard',
  templateUrl: './new-client-wizard.component.html',
  styleUrls: ['./new-client-wizard.component.scss']
})
export class NewClientWizardComponent implements OnInit {

  constructor(
    public electronService: ElectronService) { }

  ngOnInit() {

  }

  stepChanged($event, stepper){
    console.log('ss');
  }
}
