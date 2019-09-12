import { Injectable } from '@angular/core';
import { ClientInstallationInfo } from './models/clientInstallationInfo';
import { Store } from '@ngrx/store';
import { State } from '../app.reducer';
import { Observable, of } from 'rxjs';
import { allInstallationsSelector } from './instalations.reducer';
import { MatDialog } from '@angular/material/dialog';
import { MessageBoxComponent } from '../shared/message-box/message-box.component';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class InstallExistingClientService {

  private allInstallations: ClientInstallationInfo[] = [];

  constructor(
    private store: Store<State>,
    private dialog: MatDialog) {

    store.select(allInstallationsSelector).subscribe(i => this.allInstallations = i);
  }

  installExistingClient(clientName: string): Observable<boolean> {
    if (this.allInstallations.find(c => c.clientName === clientName)) {
      const dialogRef = this.dialog.open(MessageBoxComponent, {
        width: '80%', maxWidth: '500px',
        data: {
          title: 'Installation',
          message: `Client with name ${clientName} is already installed.
                  Reinstall existing client ?`,
          yesNo: true
        }
      });
      return dialogRef.afterClosed().pipe(
        map(res => res)
      );
    } else {
      return of(true);
    }
  }

}


