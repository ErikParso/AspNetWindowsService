import { NgModule } from '@angular/core';
import { InstallationListComponent } from './installation-list/installation-list.component';
import { SharedModule } from '../shared/shared.module';
import { StoreModule, Store } from '@ngrx/store';
import { InstallationsRoutes } from './installations.routing';
import { CommonModule } from '@angular/common';
import { EffectsModule } from '@ngrx/effects';
import { InstallationsEffects } from './reactive-state/installations/installations.effects';
import { InstallationToolsComponent } from './installation-tools/installation-tools.component';
import { CurrentProcessComponent } from './current-process/current-process.component';
import { NewClientWizardComponent } from './new-client-wizard/new-client-wizard.component';
import { StepOneComponent } from './new-client-wizard/step-one/step-one.component';
import { StepTwoComponent } from './new-client-wizard/step-two/step-two.component';
import { StepThreeComponent } from './new-client-wizard/step-three/step-three.component';
import { FormsModule } from '@angular/forms';
import { InstallationsComponent } from './installations/installations.component';
import {
  ProxyAndCredentialsDialogComponent
} from './new-client-wizard/proxy-and-credentials-dialog/proxy-and-credentials-dialog.component';
import { State } from '../reactive-state/app.reducer';
import { InstallationDetailsComponent } from './installation-details/installation-details.component';
import { reducerToken, reducers, reducerProvider } from './reactive-state/installations.module.reducers';
import { ConnectionsEffects } from './reactive-state/connections/connections.effects';
import { startConnections } from './reactive-state/connections/connections.actions';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    SharedModule,
    InstallationsRoutes,
    StoreModule.forFeature('installationsModule', reducerToken),
    EffectsModule.forFeature([InstallationsEffects, ConnectionsEffects]),
  ],
  declarations: [
    InstallationsComponent,
    InstallationListComponent,
    InstallationToolsComponent,
    CurrentProcessComponent,
    NewClientWizardComponent,
    StepOneComponent,
    StepTwoComponent,
    StepThreeComponent,
    ProxyAndCredentialsDialogComponent,
    InstallationDetailsComponent
  ],
  exports: [
    CurrentProcessComponent,
    ProxyAndCredentialsDialogComponent
  ],
  entryComponents: [
    CurrentProcessComponent,
    ProxyAndCredentialsDialogComponent
  ],
  providers: [
    reducerProvider
  ]
})
export class InstallationsModule {

  constructor(store: Store<State>) {
    store.dispatch(startConnections());
  }

}
