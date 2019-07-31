import { NgModule } from '@angular/core';
import { InstallationsComponent } from './installations.component';
import { InstallationListComponent } from './installation-list/installation-list.component';
import { SharedModule } from '../shared/shared.module';
import { StoreModule } from '@ngrx/store';
import { reducer } from './instalation.reducer';

@NgModule({
  imports: [
    SharedModule,
    StoreModule.forFeature('installations', reducer)
  ],
  declarations: [
    InstallationsComponent,
    InstallationListComponent
  ]
})
export class InstallationsModule { }
