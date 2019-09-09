import { Component, OnInit } from '@angular/core';
import { FormControl, Validators, FormGroup, NgForm, FormGroupDirective, FormBuilder } from '@angular/forms';
import { ErrorStateMatcher } from '@angular/material/core';
import { ElectronService } from 'ngx-electron';

export class MyErrorStateMatcher implements ErrorStateMatcher {
  isErrorState(control: FormControl | null, form: FormGroupDirective | NgForm | null): boolean {
    const formInterracted = form && (form.touched || form.dirty);
    return (control && control.invalid && (control.dirty || control.touched || formInterracted));
  }
}

@Component({
  selector: 'app-step-one',
  templateUrl: './step-one.component.html',
  styleUrls: ['./step-one.component.css']
})
export class StepOneComponent implements OnInit {

  public clientName = new FormControl('', Validators.required);
  public installDir = new FormControl('', Validators.required);
  public applicationServer = new FormControl('http://CAMEL/Source99-E5A1', Validators.required);
  public language = new FormControl('', Validators.required);

  public frmStepOne = new FormGroup({
    clientName: this.clientName,
    installDir: this.installDir,
    applicationServer: this.applicationServer,
    language: this.language
  });

  constructor(
    private formBuilder: FormBuilder,
    public electronService: ElectronService) { }

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

  write(obj) {
    console.log(obj);
  }
}
