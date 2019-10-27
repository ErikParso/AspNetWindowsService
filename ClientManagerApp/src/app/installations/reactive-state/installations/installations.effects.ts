import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { InstallationsService } from '../../services/installations.service';
import { map, mergeMap, catchError, switchMap, flatMap } from 'rxjs/operators';
import * as actions from './installations.actions';
import { of } from 'rxjs';
import { HegiService } from '../../services/hegi.service';
import { Router } from '@angular/router';
import { UUID } from 'angular2-uuid';
import { Store } from '@ngrx/store';
import { State, currentUserAppLocalPathSelector, currentUserNameSelector } from '../../../reactive-state/app.reducer';
import { MatDialog } from '@angular/material/dialog';
import { ElectronService } from 'ngx-electron';
import { ClientIdGenerationService } from '../../services/install-existing-client.service';
import { InstallationScope } from '../../models/hegi-descriptor';
import { startConnectionsSuccess } from '../connections/connections.actions';

@Injectable()
export class InstallationsEffects {

    startConnectionsSuccess$ = createEffect(() => this.actions$.pipe(
        ofType(startConnectionsSuccess),
        map(() => actions.loadInstallations())
    ));

    loadInstallations$ = createEffect(() => this.actions$.pipe(
        ofType(actions.loadInstallations),
        mergeMap(() => this.installationsService.getInstallations()
            .pipe(
                map(installations => actions.loadInstallationsSuccess({ payload: installations })),
                catchError((e) => of(actions.loadInstallationsError({ payload: 'Loading installations failed. ' + e.message })))
            ))
    ));

    // after client list load, run .hegi installation if needed
    loadInstallationsSuccess2$ = createEffect(() => this.actions$.pipe(
        ofType(actions.loadInstallationsSuccess),
        flatMap(() => {
            if (this.hegiService.hegiDescriptor && !this.hegiService.hegiDescriptor.hideWizard) {
                this.router.navigate(['installations', 'newclient']);
                return of(actions.dummy());
            }
            if (this.hegiService.hegiDescriptor && this.hegiService.hegiDescriptor.hideWizard) {
                return this.installExistingClientService.getClientId(
                    this.hegiService.hegiDescriptor.clientName,
                    this.hegiService.hegiDescriptor.installScope)
                    .pipe(
                        map(clientId => {
                            if (clientId.length) {
                                const currentProcessId = UUID.UUID();
                                return actions.installNewClient({
                                    payload: {
                                        applicationServer: this.hegiService.hegiDescriptor.applicationServer,
                                        clientId,
                                        clientName: this.hegiService.hegiDescriptor.clientName,
                                        installDir: this.hegiService.hegiDescriptor.installScope === InstallationScope.perMachine
                                            ? this.hegiService.hegiDescriptor.installDir
                                            : this.appdataPath + '\\Asseco Solutions\\NorisWin32Clients',
                                        installationProcessId: currentProcessId,
                                        language: this.hegiService.hegiDescriptor.language
                                            ? this.hegiService.hegiDescriptor.language
                                            : 'CZ',
                                        configName: this.hegiService.hegiDescriptor.configName,
                                        desktopIcon: this.hegiService.hegiDescriptor.desktopIcon,
                                        installForAllUsers: this.hegiService.hegiDescriptor.installScope === InstallationScope.perMachine,
                                        lnkForAllUser: this.hegiService.hegiDescriptor.lnkForAllUser,
                                        configItems: this.hegiService.hegiDescriptor.configItems,
                                        runAfterInstall: false,
                                        userName: this.hegiService.hegiDescriptor.installScope === InstallationScope.perMachine
                                            ? '' : this.currentUSerName
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
                    { payload: { clientId: payload.clientId, upgradeInfo: res } })),
                catchError(e => of(actions.getClientNeedUpgradeError(
                    { payload: { clientId: payload.clientId, message: e.message } })))
            ))
    ));

    runClient$ = createEffect(() => this.actions$.pipe(
        ofType(actions.runClient),
        mergeMap((action) => this.installationsService.runClientApplication(action.payload.clientId)
            .pipe(
                map(result => actions.runClientSuccess()),
            ))
    ));

    installNewClient$ = createEffect(() => this.actions$.pipe(
        ofType(actions.installNewClient),
        mergeMap(({ payload }) => this.installationsService.installNewClient(payload)
            .pipe(
                switchMap(info => [
                    actions.installNewClientSuccess({ payload: { ...info, currentProcessId: payload.installationProcessId } }),
                    payload.runAfterInstall ? actions.runClient({ payload: { clientId: info.clientId } }) : actions.dummy()]),
                catchError((e) => of(actions.installNewClientError({
                    payload: {
                        message: e.message,
                        clientId: payload.clientId,
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
                    payload: { ...info, currentProcessId: payload.updateProcessId }
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

    private appdataPath: string;
    private currentUSerName: string;

    constructor(
        private actions$: Actions,
        private installationsService: InstallationsService,
        private hegiService: HegiService,
        private router: Router,
        private store: Store<State>,
        public dialog: MatDialog,
        public electronService: ElectronService,
        public installExistingClientService: ClientIdGenerationService) {

        this.store.select(currentUserAppLocalPathSelector).subscribe(res => this.appdataPath = res);
        this.store.select(currentUserNameSelector).subscribe(res => this.currentUSerName = res);
    }
}
