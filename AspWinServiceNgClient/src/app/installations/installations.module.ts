import { NgModule } from '@angular/core';
import { InstallationsComponent } from './installations.component';
import { InstallationListComponent } from './installation-list/installation-list.component';
import { SharedModule } from '../shared/shared.module';
import { StoreModule } from '@ngrx/store';
import { reducer } from './instalations.reducer';
import { InstallationsRoutes } from './installations.routing';
import { CommonModule } from '@angular/common';
import { EffectsModule } from '@ngrx/effects';
import { InstallationsEffects } from './installations.effects';

@NgModule({
  imports: [
    CommonModule,
    SharedModule,
    InstallationsRoutes,
    StoreModule.forFeature('installations', reducer),
    EffectsModule.forFeature([InstallationsEffects]),
  ],
  declarations: [
    InstallationsComponent,
    InstallationListComponent
  ]
})
export class InstallationsModule { }
