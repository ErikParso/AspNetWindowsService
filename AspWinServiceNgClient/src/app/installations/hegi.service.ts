import { Injectable } from '@angular/core';
import { ElectronService } from 'ngx-electron';
import { saveAs } from 'file-saver';
import { HegiDescriptor } from './models/hegi-descriptor';
import { Store } from '@ngrx/store';
import { State, startupFileSelector } from '../app.reducer';
import * as fs from 'fs';

@Injectable({
  providedIn: 'root'
})
export class HegiService {

  private _hegiDescriptor: HegiDescriptor;

  public get hegiDescriptor(): HegiDescriptor {
    return this._hegiDescriptor;
  }

  constructor(
    private electronService: ElectronService,
    private store: Store<State>) {

    store.select(startupFileSelector).subscribe(file => {
      if (this.electronService.isElectronApp && file && file.length && file.endsWith('.hegi')) {
        const data = ((window as any).fs).readFileSync(file);
        this._hegiDescriptor = JSON.parse(data) as HegiDescriptor;
      } else {
        this._hegiDescriptor = null;
      }
    });
  }

  public downloadHegi(hegiDescriptor: HegiDescriptor): void {

    const blb = new Blob([JSON.stringify(hegiDescriptor, null, 2)], { type: 'text/plain' });

    if (this.electronService.isElectronApp) {
      this.electronService.remote.dialog.showSaveDialog(
        { filters: [{ extensions: ['hegi'], name: 'hegi' } as Electron.FileFilter] } as Electron.SaveDialogOptions).then(
          res => {
            const fileReader = new FileReader();
            fileReader.addEventListener('loadend', e =>
              ((window as any).fs).writeFile(res.filePath, fileReader.result, () => { }));
            fileReader.readAsText(blb);
          }
        );
    } else {
      saveAs(blb, 'install.hegi');
    }
  }
}
