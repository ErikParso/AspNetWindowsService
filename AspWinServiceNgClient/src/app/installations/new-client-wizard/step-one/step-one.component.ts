import { Component, OnInit } from '@angular/core';
import { FormControl, Validators, FormGroup, FormBuilder, AbstractControl } from '@angular/forms';
import { ElectronService } from 'ngx-electron';
import { ValidationService } from '../../validation.service';
import { map, tap } from 'rxjs/operators';
import { ActivatedRoute } from '@angular/router';
import { HegiService } from '../../hegi.service';
import { InstallationScope } from '../../models/hegi-descriptor';
import { Store } from '@ngrx/store';
import { State, currentUserAppLocalPathSelector } from 'src/app/app.reducer';

@Component({
  selector: 'app-step-one',
  templateUrl: './step-one.component.html',
  styleUrls: ['./step-one.component.css']
})
export class StepOneComponent implements OnInit {

  private currentUserAppPath: string;
  private perMachineInstallDir: string;

  public availableLanguages: string[] = [];

  public clientName: FormControl;
  public installForAll: FormControl;
  public installDir: FormControl;
  public applicationServer: FormControl;
  public language: FormControl;
  public frmStepOne: FormGroup;

  public fromHegi: string;

  constructor(
    public electronService: ElectronService,
    public validationService: ValidationService,
    private hegiService: HegiService,
    store: Store<State>) {

    store.select(currentUserAppLocalPathSelector).subscribe(res => this.currentUserAppPath = res);
    const hegiDesc = hegiService.hegiDescriptor;

    if (hegiDesc) {
      this.clientName = new FormControl(hegiDesc.clientName, Validators.required);
      this.installForAll = new FormControl(hegiDesc.installScope === InstallationScope.perMachine);
      this.installDir = new FormControl(
        hegiDesc.installScope === InstallationScope.perMachine
          ? hegiDesc.installDir
          : this.currentUserAppPath + '\\Asseco Solutions\\NorisWin32Clients',
        Validators.required);
      if (hegiDesc.installScope === InstallationScope.perUser) {
        this.installDir.disable();
      }
      this.applicationServer = new FormControl(
        hegiDesc.applicationServer, Validators.required, this.validateVersionManagerAddress.bind(this));
      this.language = new FormControl(hegiDesc.language, Validators.required);
    } else {
      this.clientName = new FormControl('', Validators.required);
      this.installForAll = new FormControl(true);
      this.installDir = new FormControl('', Validators.required);
      this.applicationServer = new FormControl('', Validators.required, this.validateVersionManagerAddress.bind(this));
      this.language = new FormControl('', Validators.required);
    }

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
        if (res.isValid) {
          this.availableLanguages = JSON.parse(res.message).Languages;
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
      this.installDir.setValue(this.perMachineInstallDir);
    } else {
      this.installDir.disable();
      this.perMachineInstallDir = this.installDir.value;
      this.installDir.setValue(this.currentUserAppPath + '\\Asseco Solutions\\NorisWin32Clients');
    }
  }
}
