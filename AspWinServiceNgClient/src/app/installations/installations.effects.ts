import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { InstallationsService } from './installations.service';
import { map, mergeMap, catchError, switchMap } from 'rxjs/operators';
import * as actions from './installations.actions';
import { of } from 'rxjs';

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

    loadInstallationsSuccess$ = createEffect(() => this.actions$.pipe(
        ofType(actions.loadInstallationsSuccess),
        switchMap(({ payload }) => of(payload)),
        switchMap(res => res.map(cli => actions.getClientNeedUpgrade({ payload: { clientId: cli.clientId } })))
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
            payload.clientId, payload.clientName, payload.installDir, payload.applicationServer, payload.installationProcessId)
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
        private installationsService: InstallationsService
    ) { }
}
