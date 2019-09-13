import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { State } from 'src/app/app.reducer';
import { Observable } from 'rxjs';
import { loadingInstallationsSelector } from '../instalations.reducer';

@Component({
  selector: 'app-installations',
  templateUrl: './installations.component.html',
  styleUrls: ['./installations.component.css']
})
export class InstallationsComponent implements OnInit {

  public loadingInstallations$: Observable<boolean>;

  constructor(private store: Store<State>) { }

  ngOnInit() {
    this.loadingInstallations$ = this.store.select(loadingInstallationsSelector);
  }

}
