import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormControl, FormGroupDirective, NgForm } from '@angular/forms';
import { ErrorStateMatcher } from '@angular/material/core';
import { ElectronService } from 'ngx-electron';
import { MatDialogRef } from '@angular/material/dialog';

export class MyErrorStateMatcher implements ErrorStateMatcher {
  isErrorState(control: FormControl | null, form: FormGroupDirective | NgForm | null): boolean {
    const isSubmitted = form && form.submitted;
    return !!(control && control.invalid && (control.dirty || control.touched));
  }
}

@Component({
  selector: 'app-new-client-wizard',
  templateUrl: './new-client-wizard.component.html',
  styleUrls: ['./new-client-wizard.component.scss']
})
export class NewClientWizardComponent implements OnInit {

  isLinear = false;

  matcher = new MyErrorStateMatcher();

  public clientName = new FormControl('', Validators.required);
  public installDir = new FormControl('', Validators.required);
  public applicationServer = new FormControl('http://CAMEL/Source99-E5A1', Validators.required);
  public language = new FormControl('', Validators.required);

  public installationForm = new FormGroup({
    clientName: this.clientName,
    installDir: this.installDir,
    applicationServer: this.applicationServer,
    language: this.language
  });

  secondFormGroup: FormGroup;

  constructor(
    private formBuilder: FormBuilder,
    public electronService: ElectronService) {}

  ngOnInit() {
    this.secondFormGroup = this.formBuilder.group({
      secondCtrl: ['', Validators.required]
    });
  }

  selectDir() {
    this.electronService.remote.dialog.showOpenDialog(this.electronService.remote.getCurrentWindow(), { properties: ['openDirectory'] })
      .then(result => {
        if (result.filePaths && result.filePaths.length) {
          this.installationForm.get('installDir').setValue(result.filePaths[0]);
        }
      });
  }
}
