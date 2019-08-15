import { Component, OnInit, Input, Inject } from '@angular/core';
import { CurrentProcess, CurrentProcessType, CurrentProcessResult } from '../models/current-process';
import { Observable } from 'rxjs';
import { State } from 'src/app/app.reducer';
import { Store } from '@ngrx/store';
import * as reducer from '../instalations.reducer';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { clearCurrentProcess } from '../installations.actions';

export class CurrenProcessDialogData {
  currentProcessId: string;
}

@Component({
  selector: 'app-current-process',
  templateUrl: './current-process.component.html',
  styleUrls: ['./current-process.component.css']
})
export class CurrentProcessComponent implements OnInit {

  public currentProcesstypes = CurrentProcessType;
  public currentProcessResults = CurrentProcessResult;

  public currentProcess$: Observable<CurrentProcess>;

  constructor(
    private store: Store<State>,
    public dialogRef: MatDialogRef<CurrentProcessComponent>,
    @Inject(MAT_DIALOG_DATA) public data: CurrenProcessDialogData) { }

  ngOnInit() {
    this.currentProcess$ = this.store.select(reducer.currentProcessSelector(this.data.currentProcessId));
  }

  public hide() {
    this.dialogRef.close();
  }

  public close(process: CurrentProcess) {
    this.dialogRef.close();
    this.store.dispatch(clearCurrentProcess({ payload: process.processId }));
  }
}
