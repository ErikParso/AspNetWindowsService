import { Injectable } from '@angular/core';
import { ElectronService } from 'ngx-electron';
import { saveAs } from 'file-saver';
import { HegiDescriptor } from './models/hegi-descriptor';

@Injectable({
  providedIn: 'root'
})
export class HegiService {

  constructor(private electronService: ElectronService) { }

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
