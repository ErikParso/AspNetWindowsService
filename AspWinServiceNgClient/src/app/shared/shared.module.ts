import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';

import { NgxElectronModule } from 'ngx-electron';
import { NgxChildProcessModule } from 'ngx-childprocess';

import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatInputModule } from '@angular/material/input';
import { MatDialogModule } from '@angular/material/dialog';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MessageBoxComponent } from './message-box/message-box.component';

@NgModule({
  declarations: [
    MessageBoxComponent
  ],
  imports: [
    HttpClientModule,
    MatDialogModule,
    MatButtonModule,
  ],
  exports: [
    HttpClientModule,
    ReactiveFormsModule,
    NgxElectronModule,
    NgxChildProcessModule,
    MatToolbarModule,
    MatButtonModule,
    MatIconModule,
    MatTableModule,
    MatInputModule,
    MatDialogModule,
    MatProgressSpinnerModule,
    MatTooltipModule,
    MessageBoxComponent
  ],
  entryComponents: [
    MessageBoxComponent
  ]
})
export class SharedModule { }
