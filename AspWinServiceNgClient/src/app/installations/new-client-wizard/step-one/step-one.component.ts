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

  public availableLanguages: string[] = [];

  public clientName: FormControl;
  public installForAll: FormControl;
  public installDir: FormControl;
  public applicationServer: FormControl;
  public language: FormControl;
  public frmStepOne: FormGroup;

  constructor(
    public electronService: ElectronService,
    public validationService: ValidationService) {

    this.clientName = new FormControl('', Validators.required);
    this.installForAll = new FormControl(true);
    this.installDir = new FormControl('', Validators.required);
    this.applicationServer = new FormControl('', Validators.required, this.validateVersionManagerAddress.bind(this));
    this.language = new FormControl('', Validators.required);
    this.frmStepOne = new FormGroup({
      clientName: this.clientName,
      installForAll: this.installForAll,
      installDir: this.installDir,
      applicationServer: this.applicationServer,
      language: this.language
    });
  }

  ngOnInit() {

  }

  selectDir() {
    if (this.installForAll.value) {
      this.electronService.remote.dialog.showOpenDialog(this.electronService.remote.getCurrentWindow(), { properties: ['openDirectory'] })
        .then(result => {
          if (result.filePaths && result.filePaths.length) {
            this.installDir.setValue(result.filePaths[0]);
          }
        });
    }
  }

  validateVersionManagerAddress(control: AbstractControl) {
    return this.validationService.validateVersionManagerAddress(control.value).pipe(
      tap(res => {
        console.log(res.message);
        if (res.isValid) {
          this.availableLanguages = JSON.parse(res.message).Languages;
          console.log(this.availableLanguages);
        } else {
          this.availableLanguages = [];
        }
      }),
      map(res => res.isValid ? null : { invalidVersionManagerAddress: true }
      ));
  }

  installAllClientsChanged($event) {
    if ($event.value) {
      this.installDir.enable();
    } else {
      this.installDir.disable();
    }
  }
}
