import { NgModule } from '@angular/core';
import { InstallationListComponent } from './installation-list/installation-list.component';
import { SharedModule } from '../shared/shared.module';
import { StoreModule } from '@ngrx/store';
import { reducer } from './instalations.reducer';
import { InstallationsRoutes } from './installations.routing';
import { CommonModule } from '@angular/common';
import { EffectsModule } from '@ngrx/effects';
import { InstallationsEffects } from './installations.effects';
import { NewInstallationComponent } from './new-installation/new-installation.component';
import { InstallationToolsComponent } from './installation-tools/installation-tools.component';

@NgModule({
  imports: [
    CommonModule,
    SharedModule,
    InstallationsRoutes,
    StoreModule.forFeature('installations', reducer),
    EffectsModule.forFeature([InstallationsEffects]),
  ],
  declarations: [
    InstallationListComponent,
    NewInstallationComponent,
    InstallationToolsComponent
  ],
  exports: [ NewInstallationComponent ],
  entryComponents: [ NewInstallationComponent ]
})
export class InstallationsModule { }
