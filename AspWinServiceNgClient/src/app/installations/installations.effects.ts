import { Injectable } from '@angular/core';
import { Actions, createEffect, act, ofType } from '@ngrx/effects';
import { InstallationsService } from './installations.service';
import { map, mergeMap, catchError, tap, concatMap, takeWhile } from 'rxjs/operators';
import * as actions from './installations.actions';
import { of } from 'rxjs';

@Injectable()
export class InstallationsEffects {

    loadInstallations$ = createEffect(() => this.actions$.pipe(
        ofType(actions.InstallationsActions.loadInstallations),
        mergeMap(() => this.installationsService.getInstallations()
            .pipe(
                map(installations => actions.loadInstallationsSuccess({ payload: installations })),
                catchError((e) => of(actions.loadInstallationsError({payload: 'Loading installations failed. ' + e.message})))
            ))
    ));

    runClient$ = createEffect(() => this.actions$.pipe(
        ofType<actions.RunClientAction>(actions.InstallationsActions.runClient),
        mergeMap((action) => this.installationsService.runClientApplication(action.payload)
            .pipe(
                map(result => actions.runClientSuccess()),
            ))
    ));

    constructor(
        private actions$: Actions,
        private installationsService: InstallationsService
    ) { }
}
