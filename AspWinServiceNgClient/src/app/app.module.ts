import { NgModule } from '@angular/core';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { SharedModule } from './shared/shared.module';

import { StoreModule } from '@ngrx/store';
import { EffectsModule } from '@ngrx/effects';
import { reducerToken, reducerProvider } from './app.reducer';
import { AppEffects } from './app.effects';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    SharedModule,
    AppRoutingModule,
    StoreModule.forRoot(reducerToken),
    EffectsModule.forRoot([AppEffects]),
  ],
  providers: [reducerProvider],
  bootstrap: [AppComponent]
})
export class AppModule { }
