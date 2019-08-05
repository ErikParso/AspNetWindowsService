import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { InstallationsService } from './installations.service';
import { map, mergeMap, catchError } from 'rxjs/operators';
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

    loadLatestClientVersion$ = createEffect(() => this.actions$.pipe(
        ofType(actions.InstallationsActions.loadLatestClientVersion),
        mergeMap(() => this.installationsService.getLatestClientVersion()
            .pipe(
                map(version => actions.loadLatestClientVersionSuccess({ payload: version }))
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
        mergeMap(({ payload }) => this.installationsService.installNewClient(payload.clientName, payload.installDir)
            .pipe(
                map(info => actions.installNewClientSuccess({ payload: info })),
                catchError((e) => of(actions.installNewClientError({ payload: { message: e.message, clientName: payload.clientName } })))
            ))
    ));

    updateClient$ = createEffect(() => this.actions$.pipe(
        ofType(actions.updateClient),
        mergeMap(({ payload }) => this.installationsService.updateClient(payload.installDir)
            .pipe(
                map(info => actions.updateClientSuccess({ payload: info })),
                catchError((e) => of(actions.updateClientError({ payload: { message: e.message, clientName: payload.clientName } })))
            ))
    ));

    deleteClient$ = createEffect(() => this.actions$.pipe(
        ofType(actions.deleteClient),
        mergeMap(({ payload }) => this.installationsService.deleteClient(payload.clientName)
            .pipe(
                map(info => actions.deleteClientSuccess({payload: info})),
                catchError((e) => of(actions.deleteClientError({ payload: { message: e.message, clientName: payload.clientName } })))
            ))
    ));

    constructor(
        private actions$: Actions,
        private installationsService: InstallationsService
    ) { }
}
