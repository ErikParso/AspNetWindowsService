import { NgModule } from '@angular/core';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { SharedModule } from './shared/shared.module';

import { StoreModule, Store } from '@ngrx/store';
import { EffectsModule } from '@ngrx/effects';
import { reducerToken, reducerProvider, State } from './reactive-state/app.reducer';
import { AppEffects } from './reactive-state/app.effects';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { StoreDevtoolsModule } from '@ngrx/store-devtools';
import { CertificateProblemsDialogComponent } from './installations/certificate-problems-dialog/certificate-problems-dialog.component';
import { ConnectionsEffects } from './installations/reactive-state/connections/connections.effects';

@NgModule({
  declarations: [
    AppComponent,
    CertificateProblemsDialogComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    SharedModule,
    AppRoutingModule,
    StoreModule.forRoot(reducerToken),
    EffectsModule.forRoot([AppEffects, ConnectionsEffects]),
    StoreDevtoolsModule.instrument()
  ],
  providers: [reducerProvider],
  bootstrap: [AppComponent],
  entryComponents: [CertificateProblemsDialogComponent]
})
export class AppModule {

  constructor() {

  }

}
