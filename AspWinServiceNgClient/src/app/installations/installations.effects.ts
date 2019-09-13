import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { InstallationsService } from './installations.service';
import { map, mergeMap, catchError, switchMap, flatMap } from 'rxjs/operators';
import * as actions from './installations.actions';
import { of, Observable } from 'rxjs';
import { HegiService } from './hegi.service';
import { Router } from '@angular/router';
import { UUID } from 'angular2-uuid';
import { Store, createAction } from '@ngrx/store';
import { State } from '../app.reducer';
import { MatDialog } from '@angular/material/dialog';
import { CurrentProcessComponent, CurrenProcessDialogData } from './current-process/current-process.component';
import { ElectronService } from 'ngx-electron';
import { MessageBoxComponent } from '../shared/message-box/message-box.component';
import { InstallExistingClientService } from './install-existing-client.service';
import { Action } from 'rxjs/internal/scheduler/Action';

@Injectable()
export class InstallationsEffects {

    loadInstallations$ = createEffect(() => this.actions$.pipe(
        ofType(actions.InstallationsActions.loadInstallations),
        mergeMap(() => this.installationsService.getInstallations()
            .pipe(
                map(installations => actions.loadInstallationsSuccess({ payload: installations })),
                catchError((e) => of(actions.loadInstallationsError({ payload: 'Loading installations failed. ' + e.message })))
            ))
    ));

    // // 'loadInstallations' now does upgrade check
    // // after client list load, check for updates
    // loadInstallationsSuccess1$ = createEffect(() => this.actions$.pipe(
    //     ofType(actions.loadInstallationsSuccess),
    //     switchMap(({ payload }) => of(payload)),
    //     switchMap(res => {
    //         return res.map(cli => actions.getClientNeedUpgrade({ payload: { clientId: cli.clientId } }));
    //     }),
    // ));

    // after client list load, run .hegi installation if needed
    loadInstallationsSuccess2$ = createEffect(() => this.actions$.pipe(
        ofType(actions.loadInstallationsSuccess),
        flatMap(() => {
            if (this.hegiService.hegiDescriptor && !this.hegiService.hegiDescriptor.hideWizard) {
                this.router.navigate(['installations', 'newclient']);
                return of(actions.dummy());
            }
            if (this.hegiService.hegiDescriptor && this.hegiService.hegiDescriptor.hideWizard) {
                return this.installExistingClientService.installExistingClient(this.hegiService.hegiDescriptor.clientName).pipe(
                    map(canInstall => {
                        if (canInstall) {
                            const currentProcessId = UUID.UUID();
                            return actions.installNewClient({
                                payload: {
                                    applicationServer: this.hegiService.hegiDescriptor.applicationServer,
                                    clientId: UUID.UUID(),
                                    clientName: this.hegiService.hegiDescriptor.clientName,
                                    installDir: 'C:\\Users\\eparso\\Desktop\\installDir',
                                    installationProcessId: currentProcessId,
                                    language: 'EN'
                                }
                            });
                        } else {
                            return actions.dummy();
                        }
                    })
                );
            }
            return of(actions.dummy());
        })
    ));

    clientNeedUpgrade$ = createEffect(() => this.actions$.pipe(
        ofType(actions.getClientNeedUpgrade),
        mergeMap(({ payload }) => this.installationsService.getClientNeedUpgrade(payload.clientId)
            .pipe(
                map(res => actions.getClientNeedUpgradeSuccess(
                    { payload: { clientId: payload.clientId, needUpgrade: res } })),
                catchError(e => of(actions.getClientNeedUpgradeError(
                    { payload: { clientId: payload.clientId, message: e.message } })))
            ))
    ));

    runClient$ = createEffect(() => this.actions$.pipe(
        ofType<actions.RunClientAction>(actions.InstallationsActions.runClient),
        mergeMap((action) => this.installationsService.runClientApplication(action.payload)
            .pipe(
                map(result => actions.runClientSuccess()),
            ))
    ));

    installNewClient$ = createEffect(() => this.actions$.pipe(
        ofType(actions.installNewClient),
        mergeMap(({ payload }) => this.installationsService.installNewClient(
            payload.clientId, payload.clientName, payload.language, payload.installDir, payload.applicationServer,
            payload.installationProcessId)
            .pipe(
                map(info => actions.installNewClientSuccess({ payload: { ...info, currentProcessId: payload.installationProcessId } })),
                catchError((e) => of(actions.installNewClientError({
                    payload: {
                        message: e.message,
                        clientName: payload.clientName,
                        installationProcessId: payload.installationProcessId
                    }
                })))
            ))
    ));

    updateClient$ = createEffect(() => this.actions$.pipe(
        ofType(actions.updateClient),
        mergeMap(({ payload }) => this.installationsService.updateClient(payload.clientId, payload.updateProcessId)
            .pipe(
                map(info => actions.updateClientSuccess({
                    payload: {
                        updateProcessId: payload.updateProcessId, clientId: payload.clientId
                    }
                })),
                catchError((e) => of(actions.updateClientError({
                    payload: {
                        message: e.message, updateProcessId: payload.updateProcessId
                    }
                })))
            ))
    ));

    deleteClient$ = createEffect(() => this.actions$.pipe(
        ofType(actions.deleteClient),
        mergeMap(({ payload }) => this.installationsService.deleteClient(payload.clientId, payload.deleteProcessId)
            .pipe(
                map(info => actions.deleteClientSuccess({ payload: { deleteProcessId: payload.deleteProcessId } })),
                catchError((e) => of(actions.deleteClientError({
                    payload: {
                        message: e.message, deleteProcessId: payload.deleteProcessId
                    }
                })))
            ))
    ));

    constructor(
        private actions$: Actions,
        private installationsService: InstallationsService,
        private hegiService: HegiService,
        private router: Router,
        private store: Store<State>,
        public dialog: MatDialog,
        public electronService: ElectronService,
        public installExistingClientService: InstallExistingClientService
    ) { }
}
