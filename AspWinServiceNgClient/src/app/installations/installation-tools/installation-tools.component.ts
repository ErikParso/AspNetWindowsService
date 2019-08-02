import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { NewInstallationComponent } from '../new-installation/new-installation.component';

@Component({
  selector: 'app-installation-tools',
  templateUrl: './installation-tools.component.html',
  styleUrls: ['./installation-tools.component.css']
})
export class InstallationToolsComponent implements OnInit {

  constructor(public dialog: MatDialog) { }

  ngOnInit() {
  }

  addNewClient() {
    const dialogRef = this.dialog.open(NewInstallationComponent, {
      width: '80%', maxWidth: '500px'
    });

    dialogRef.afterClosed().subscribe(result => {

    });
  }
}
