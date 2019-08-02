import { Component, OnInit } from '@angular/core';
import { FormControl, Validators, FormGroupDirective, NgForm, FormGroup } from '@angular/forms';
import { ErrorStateMatcher } from '@angular/material/core';

export class MyErrorStateMatcher implements ErrorStateMatcher {
  isErrorState(control: FormControl | null, form: FormGroupDirective | NgForm | null): boolean {
    const isSubmitted = form && form.submitted;
    return !!(control && control.invalid && (control.dirty || control.touched || isSubmitted));
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
    installDir: new FormControl('', Validators.required)
  });

  matcher = new MyErrorStateMatcher();

  constructor() { }

  ngOnInit() {
  }

}
