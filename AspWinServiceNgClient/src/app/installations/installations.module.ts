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
import { CurrentProcessComponent } from './current-process/current-process.component';
import { NewClientWizardComponent } from './new-client-wizard/new-client-wizard.component';
import { StepOneComponent } from './new-client-wizard/step-one/step-one.component';
import { StepTwoComponent } from './new-client-wizard/step-two/step-two.component';
import { StepThreeComponent } from './new-client-wizard/step-three/step-three.component';

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
    InstallationToolsComponent,
    CurrentProcessComponent,
    NewClientWizardComponent,
    StepOneComponent,
    StepTwoComponent,
    StepThreeComponent
  ],
  exports: [
    NewInstallationComponent,
    CurrentProcessComponent
  ],
  entryComponents: [
    NewInstallationComponent,
    CurrentProcessComponent
  ]
})
export class InstallationsModule { }
