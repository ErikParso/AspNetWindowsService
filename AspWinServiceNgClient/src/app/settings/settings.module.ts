import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SettingsComponent } from './settings.component';
import { SharedModule } from '../shared/shared.module';
import { FormsModule } from '@angular/forms';
import { SettingsRoutes } from './settings.routing';
import { ProxyComponent } from './proxy/proxy.component';

@NgModule({
  imports: [
    CommonModule,
    SharedModule,
    FormsModule,
    SettingsRoutes
  ],
  declarations: [
    SettingsComponent,
    ProxyComponent
  ]
})
export class SettingsModule { }
