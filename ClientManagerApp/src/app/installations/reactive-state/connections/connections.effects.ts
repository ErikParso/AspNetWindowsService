import { Injectable } from '@angular/core';
import { createEffect, Actions, ofType } from '@ngrx/effects';
import { AcceptCertRpcService } from '../../services/accept-cert-rpc.service';
import { mergeMap, map, catchError } from 'rxjs/operators';
import { of, forkJoin } from 'rxjs';
import * as actions from './connections.actions';
import { AutoActualizationRpcService } from '../../services/auto-actualization-hub.service';
import { ProgressHubService } from '../../services/progress-hub.service';

@Injectable()
export class ConnectionsEffects {

    startConnections$ = createEffect(() => this.actions$.pipe(
        ofType(actions.startConnections),
        mergeMap(() => forkJoin(
            this.acceptCertService.startConnection(),
            this.autoActualizationRpcService.startConnection(),
            this.progressHubService.startConnection())
            .pipe(
                map(res => actions.startConnectionsSuccess({
                    payload: {
                        acceptCertRpcHubConnectionId: res[0],
                        autoActualizationHubConnectionId: res[1],
                        progressHubConnectionId: res[2]
                    }
                })),
                catchError(err => {
                    console.log(err);
                    return of(actions.startConnectionsError());
                })
            ))
    ));

    constructor(
        private actions$: Actions,
        private acceptCertService: AcceptCertRpcService,
        private autoActualizationRpcService: AutoActualizationRpcService,
        private progressHubService: ProgressHubService
    ) { }
}
