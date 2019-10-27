import { createAction, props } from '@ngrx/store';

export const startConnections = createAction(
    '[App] Start signalR connections',
);

export const startConnectionsSuccess = createAction(
    '[App] Start signalR connections success',
    props<{
        payload: {
            acceptCertRpcHubConnectionId: string,
            autoActualizationHubConnectionId: string,
            progressHubConnectionId: string,
        }
    }>()
);

export const startConnectionsError = createAction(
    '[App] Start signalR connections error'
);
