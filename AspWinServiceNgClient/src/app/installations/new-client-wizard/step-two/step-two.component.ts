import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { ConfigValue } from '../../models/config-value';

@Component({
  selector: 'app-step-two',
  templateUrl: './step-two.component.html',
  styleUrls: ['./step-two.component.css']
})
export class StepTwoComponent implements OnInit {

  public displayedColumns = ['section', 'key', 'value', 'tools'];
  public defaultConfigValues: ConfigValue[] = [{
    section: 'section1', key: 'key1', value: 'value1'
  }];

  public useDefaultConfig: FormControl;
  public configName: FormControl;
  public frmStepTwo: FormGroup;

  constructor() {
    this.useDefaultConfig = new FormControl(true);
    this.configName = new FormControl({ value: '', disabled: true }, Validators.required);
    this.frmStepTwo = new FormGroup({
      useDefaultConfig: this.useDefaultConfig,
      configName: this.configName
    });
  }

  ngOnInit() {

  }

  checkChanged($event) {
    if ($event.checked) {
      this.configName.disable();
      this.configName.setValue('');
    } else {
      this.configName.enable();
    }
  }

  addConfigValue() {
    this.defaultConfigValues = this.defaultConfigValues.concat({ section: 'section', key: 'key', value: 'value' } as ConfigValue);
  }

  deleteConfigValue(value: ConfigValue) {
    this.defaultConfigValues = this.defaultConfigValues.filter(conf => conf !== value);
  }

}
