import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { ClientConfigItem } from '../../models/client-config-item';
import { HegiService } from '../../services/hegi.service';
import { InstallConfigService } from '../install-config.service';
import { ClientConfigItemRestriction } from '../../models/client-config-item-restriction';

@Component({
  selector: 'app-step-two',
  templateUrl: './step-two.component.html',
  styleUrls: ['./step-two.component.css']
})
export class StepTwoComponent implements OnInit {

  public clientConfigItemRestrictions: ClientConfigItemRestriction[] = [
    { section: 'LogIn', key: 'UseDefaultProxy', reasonMessage: 'Edit this value in step 1 Proxy and Credentials section.' },
    { section: 'LogIn', key: 'Proxy', reasonMessage: 'Edit this value in step 1 Proxy and Credentials section.' },
    { section: 'LogIn', key: 'IntegratedWindowsAuthentication', reasonMessage: 'Edit this value in step 1 Proxy and Credentials section.' }
  ];
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

  canAddConfigValue(section: string, key: string, value: string) {
    return !this.clientConfigItemRestrictions.find(c => c.section === section && c.key === key) &&
      section.length && value.length && key.length;
  }

  getRestrictionMessage(section: string, key: string, value: string) {
    const restriction = this.clientConfigItemRestrictions.find(c => c.section === section && c.key === key);
    return restriction
      ? restriction.reasonMessage
      : !section.length || !key.length || !value.length
        ? 'Please set section, key and value.'
        : '';
  }

  deleteConfigValue(value: ClientConfigItem) {
    this.installConfigService.removeConfigItem(value.section, value.key);
  }

}
