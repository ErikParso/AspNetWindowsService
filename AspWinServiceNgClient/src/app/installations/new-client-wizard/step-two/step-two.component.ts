import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { HegiConfigItem } from '../../models/hegi-config-item';
import { group } from '@angular/animations';
import { HegiService } from '../../hegi.service';

@Component({
  selector: 'app-step-two',
  templateUrl: './step-two.component.html',
  styleUrls: ['./step-two.component.css']
})
export class StepTwoComponent implements OnInit {

  public displayedColumns = ['section', 'key', 'value', 'tools'];
  public defaultConfigValues: HegiConfigItem[] = [];

  public useDefaultConfig: FormControl;
  public configName: FormControl;
  public section: FormControl;
  public key: FormControl;
  public value: FormControl;

  public frmStepTwo: FormGroup;

  constructor(
    hegiService: HegiService) {

    if (hegiService.hegiDescriptor) {
      const useDefConf = hegiService.hegiDescriptor.configName === 'noris.config';
      this.useDefaultConfig = new FormControl(useDefConf);
      this.configName = new FormControl({ value: hegiService.hegiDescriptor.configName, disabled: useDefConf }, Validators.required);
      this.defaultConfigValues = hegiService.hegiDescriptor.config ? hegiService.hegiDescriptor.config : [];
    } else {
      this.useDefaultConfig = new FormControl(true);
      this.configName = new FormControl({ value: 'noris.config', disabled: true }, Validators.required);
    }

    this.section = new FormControl('');
    this.key = new FormControl('');
    this.value = new FormControl('');

    this.frmStepTwo = new FormGroup({
      useDefaultConfig: this.useDefaultConfig,
      configName: this.configName,
      section: this.section,
      key: this.key,
      value: this.value
    });
  }

  ngOnInit() {

  }

  checkChanged($event) {
    if ($event.checked) {
      this.configName.disable();
      this.configName.setValue('noris.config');
    } else {
      this.configName.enable();
    }
  }

  addConfigValue() {
    this.defaultConfigValues = this.defaultConfigValues.concat({
      section: this.section.value,
      key: this.key.value,
      value: this.value.value
    });
    this.section.setValue('');
    this.key.setValue('');
    this.value.setValue('');
  }

  deleteConfigValue(value: HegiConfigItem) {
    this.defaultConfigValues = this.defaultConfigValues.filter(conf => conf !== value);
  }

}
