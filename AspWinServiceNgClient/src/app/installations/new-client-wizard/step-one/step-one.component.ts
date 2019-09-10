import { Component, OnInit } from '@angular/core';
import { FormControl, Validators, FormGroup, FormBuilder, AbstractControl } from '@angular/forms';
import { ElectronService } from 'ngx-electron';
import { ValidationService } from '../../validation.service';
import { map, tap } from 'rxjs/operators';

@Component({
  selector: 'app-step-one',
  templateUrl: './step-one.component.html',
  styleUrls: ['./step-one.component.css']
})
export class StepOneComponent implements OnInit {

  public clientName: FormControl;
  public installDir: FormControl;
  public applicationServer: FormControl;
  public language: FormControl;

  public frmStepOne: FormGroup;

  constructor(
    private formBuilder: FormBuilder,
    public electronService: ElectronService,
    public validationService: ValidationService) {

    this.clientName = new FormControl('', Validators.required);
    this.installDir = new FormControl('', Validators.required);
    this.applicationServer = new FormControl(
      'http://CAMEL/Source99-E5A1',
      Validators.required,
      this.validateVersionManagerAddress.bind(this));
    this.language = new FormControl('', Validators.required);
    this.frmStepOne = new FormGroup({
      clientName: this.clientName,
      installDir: this.installDir,
      applicationServer: this.applicationServer,
      language: this.language
    });
  }

  ngOnInit() {

  }

  selectDir() {
    this.electronService.remote.dialog.showOpenDialog(this.electronService.remote.getCurrentWindow(), { properties: ['openDirectory'] })
      .then(result => {
        if (result.filePaths && result.filePaths.length) {
          this.installDir.setValue(result.filePaths[0]);
        }
      });
  }

  validateVersionManagerAddress(control: AbstractControl) {
    return this.validationService.validateVersionManagerAddress(control.value).pipe(
      tap(res => {
        if (!res.isValid) {
          console.log(res.message);
        }
      }),
      map(res => res.isValid ? null : { invalidVersionManagerAddress: true }
      ));
  }
}
