import { Component, OnInit } from '@angular/core';
import { FormControl, Validators, FormGroupDirective, NgForm, FormGroup } from '@angular/forms';
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
  selector: 'app-new-installation',
  templateUrl: './new-installation.component.html',
  styleUrls: ['./new-installation.component.css']
})
export class NewInstallationComponent implements OnInit {

  installationForm = new FormGroup({
    clientName: new FormControl('', Validators.required),
    installDir: new FormControl('', Validators.required),
    applicationServer: new FormControl('http://CAMEL/Source99-E5A1', Validators.required)
  });

  matcher = new MyErrorStateMatcher();

  constructor(
    public electronService: ElectronService,
    public dialogRef: MatDialogRef<NewInstallationComponent>) { }

  ngOnInit() {
  }

  selectDir() {
    this.electronService.remote.dialog.showOpenDialog(this.electronService.remote.getCurrentWindow(), { properties: ['openDirectory'] })
      .then(result => {
        if (result.filePaths && result.filePaths.length) {
          this.installationForm.get('installDir').setValue(result.filePaths[0]);
        }
      });
  }

  cancel() {
    this.dialogRef.close();
  }

  ok() {
    this.dialogRef.close({
      clientName: this.installationForm.get('clientName').value,
      installDir: this.installationForm.get('installDir').value,
      applicationServer: this.installationForm.get('applicationServer').value
    });
  }
}
