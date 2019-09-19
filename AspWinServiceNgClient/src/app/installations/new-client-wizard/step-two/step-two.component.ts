import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { ClientConfigItem } from '../../models/client-config-item';
import { HegiService } from '../../hegi.service';
import { InstallConfigService } from '../install-config.service';

@Component({
  selector: 'app-step-two',
  templateUrl: './step-two.component.html',
  styleUrls: ['./step-two.component.css']
})
export class StepTwoComponent implements OnInit {

  public displayedColumns = ['section', 'key', 'value', 'tools'];
  public configItems: ClientConfigItem[];
  
  public useDefaultConfig: FormControl;
  public configName: FormControl;
  public section: FormControl;
  public key: FormControl;
  public value: FormControl;

  public frmStepTwo: FormGroup;

  constructor(
    hegiService: HegiService,
    public installConfigService: InstallConfigService) {

    this.installConfigService.defaultConfigValues$.subscribe(c => this.configItems = c);

    if (hegiService.hegiDescriptor) {
      const useDefConf = hegiService.hegiDescriptor.configName === 'noris.config';
      this.useDefaultConfig = new FormControl(useDefConf);
      this.configName = new FormControl({ value: hegiService.hegiDescriptor.configName, disabled: useDefConf }, Validators.required);
      this.installConfigService.setConfigItems(hegiService.hegiDescriptor.configItems ? hegiService.hegiDescriptor.configItems : []);
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
    this.installConfigService.addConfigItem({
      section: this.section.value,
      key: this.key.value,
      value: this.value.value
    });
    this.section.setValue('');
    this.key.setValue('');
    this.value.setValue('');
  }

  deleteConfigValue(value: ClientConfigItem) {
    this.installConfigService.removeConfigItem(value);
  }

}
