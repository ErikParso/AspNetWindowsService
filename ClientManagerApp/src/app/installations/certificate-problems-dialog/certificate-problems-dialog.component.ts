import { Component, OnInit, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AcceptCertificateRpcRequest } from '../models/accept-certificate-rpc-request';
import { AcceptCertificateRpcResponse } from '../models/accept-certificate-rpc-response';

@Component({
  selector: 'app-certificate-problems-dialog',
  templateUrl: './certificate-problems-dialog.component.html',
  styleUrls: ['./certificate-problems-dialog.component.css']
})
export class CertificateProblemsDialogComponent implements OnInit {

  constructor(
    public dialogRef: MatDialogRef<CertificateProblemsDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: AcceptCertificateRpcRequest) { }

  ngOnInit() {
  }

  yes() {
    this.dialogRef.close({
      acceptCertificate: true,
      dontAskAgain: false,
    } as AcceptCertificateRpcResponse);
  }

  yesDontAskagain() {
    this.dialogRef.close({
      acceptCertificate: true,
      dontAskAgain: true,
    } as AcceptCertificateRpcResponse);
  }

  no() {
    this.dialogRef.close({
      acceptCertificate: false,
      dontAskAgain: false,
    } as AcceptCertificateRpcResponse);
  }
}
