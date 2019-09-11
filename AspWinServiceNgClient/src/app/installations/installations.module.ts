import { NgModule } from '@angular/core';
import { InstallationListComponent } from './installation-list/installation-list.component';
import { SharedModule } from '../shared/shared.module';
import { StoreModule, Store } from '@ngrx/store';
import { reducer } from './instalations.reducer';
import { InstallationsRoutes } from './installations.routing';
import { CommonModule } from '@angular/common';
import { EffectsModule } from '@ngrx/effects';
import { InstallationsEffects } from './installations.effects';
import { InstallationToolsComponent } from './installation-tools/installation-tools.component';
import { CurrentProcessComponent } from './current-process/current-process.component';
import { NewClientWizardComponent } from './new-client-wizard/new-client-wizard.component';
import { StepOneComponent } from './new-client-wizard/step-one/step-one.component';
import { StepTwoComponent } from './new-client-wizard/step-two/step-two.component';
import { StepThreeComponent } from './new-client-wizard/step-three/step-three.component';
import { State } from '../app.reducer';
import { loadInstallations } from './installations.actions';
import { FormsModule } from '@angular/forms';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    SharedModule,
    InstallationsRoutes,
    StoreModule.forFeature('installations', reducer),
    EffectsModule.forFeature([InstallationsEffects]),
  ],
  declarations: [
    InstallationListComponent,
    InstallationToolsComponent,
    CurrentProcessComponent,
    NewClientWizardComponent,
    StepOneComponent,
    StepTwoComponent,
    StepThreeComponent
  ],
  exports: [
    CurrentProcessComponent
  ],
  entryComponents: [
    CurrentProcessComponent
  ]
})
export class InstallationsModule {

  constructor(private store: Store<State>) {
    this.store.dispatch(loadInstallations());
  }

}
