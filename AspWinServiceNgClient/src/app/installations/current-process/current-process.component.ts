import { Component, OnInit, Input, Inject } from '@angular/core';
import { CurrentProcess } from '../models/current-process';
import { Observable } from 'rxjs';
import { State } from 'src/app/app.reducer';
import { Store } from '@ngrx/store';
import * as reducer from '../instalations.reducer';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

export class CurrenProcessDialogData {
  currentProcessId: string;
}

@Component({
  selector: 'app-current-process',
  templateUrl: './current-process.component.html',
  styleUrls: ['./current-process.component.css']
})
export class CurrentProcessComponent implements OnInit {

  public currentProcess$: Observable<CurrentProcess>;

  constructor(
    private store: Store<State>,
    @Inject(MAT_DIALOG_DATA) public data: CurrenProcessDialogData) { }

  ngOnInit() {
    this.currentProcess$ = this.store.select(reducer.currentProcessSelector(this.data.currentProcessId));
  }

}
