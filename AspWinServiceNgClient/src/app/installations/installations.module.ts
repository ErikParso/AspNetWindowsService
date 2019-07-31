import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InstallationsComponent } from './installations.component';
import { InstallationListComponent } from './installation-list/installation-list.component';

@NgModule({
  imports: [
    CommonModule
  ],
  declarations: [
    InstallationsComponent,
    InstallationListComponent
  ]
})
export class InstallationsModule { }
