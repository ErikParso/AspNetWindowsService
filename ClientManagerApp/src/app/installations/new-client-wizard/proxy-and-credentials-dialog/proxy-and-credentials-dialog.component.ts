import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { InstallConfigService } from '../install-config.service';

@Component({
  selector: 'app-proxy-and-credentials-dialog',
  templateUrl: './proxy-and-credentials-dialog.component.html',
  styleUrls: ['./proxy-and-credentials-dialog.component.css']
})
export class ProxyAndCredentialsDialogComponent implements OnInit {

  public frmProxyAndCredentials: FormGroup;
  public useDefaultProxy: FormControl;
  public proxy: FormControl;
  public integratedWindowsAuthentication: FormControl;

  constructor(
    private dialogRef: MatDialogRef<ProxyAndCredentialsDialogComponent>,
    private installConfigService: InstallConfigService) {

    this.useDefaultProxy = new FormControl(
      this.installConfigService.getConfigValue('LogIn', 'UseDefaultProxy') === '1');
    this.proxy = new FormControl(
      this.installConfigService.getConfigValue('LogIn', 'Proxy'));
    if (this.installConfigService.getConfigValue('LogIn', 'UseDefaultProxy') === '1') {
      this.proxy.disable();
    }
    this.integratedWindowsAuthentication = new FormControl(
      this.installConfigService.getConfigValue('LogIn', 'IntegratedWindowsAuthentication') === '1');

    this.frmProxyAndCredentials = new FormGroup({
      useDefaultProxy: this.useDefaultProxy,
      proxy: this.proxy,
      integratedWindowsAuthentication: this.integratedWindowsAuthentication
    });
  }

  ngOnInit() {

  }

  useDefaultProxyChanged($event) {
    if ($event.checked) {
      this.proxy.disable();
    } else {
      this.proxy.enable();
    }
  }

  saveSettingsAndClose() {
    if (this.frmProxyAndCredentials.dirty) {

      if (this.useDefaultProxy.value) {
        this.installConfigService.addConfigItem({ section: 'LogIn', key: 'UseDefaultProxy', value: '1' });
      } else {
        this.installConfigService.removeConfigItem('LogIn', 'UseDefaultProxy');
      }

      if (!this.useDefaultProxy.value && this.proxy.value && this.proxy.value.length) {
        this.installConfigService.addConfigItem({ section: 'LogIn', key: 'Proxy', value: this.proxy.value });
      } else {
        this.installConfigService.removeConfigItem('LogIn', 'Proxy');
      }

      if (this.integratedWindowsAuthentication.value) {
        this.installConfigService.addConfigItem({ section: 'LogIn', key: 'IntegratedWindowsAuthentication', value: '1' });
      } else {
        this.installConfigService.removeConfigItem('LogIn', 'IntegratedWindowsAuthentication');
      }

      this.dialogRef.close(true);

    } else {
      this.dialogRef.close(true);
    }
  }

  cancel() {
    this.dialogRef.close(false);
  }
}
