import { createSelector, createReducer, on } from '@ngrx/store';
import { State } from '../installations.module.reducers';
import * as actions from './connections.actions';

export interface ConnectionsState {
    acceptCertRpcHubConnectionId: string;
    autoActualizationHubConnectionId: string;
    progressHubConnectionId: string;
}

export const initialState: ConnectionsState = {
    acceptCertRpcHubConnectionId: '',
    autoActualizationHubConnectionId: '',
    progressHubConnectionId: '',
};

export const connectionsSelector = (state: State) => state.installationsModule.connections;
export const acceptCertRpcHubConnectionIdSelector = createSelector(
    connectionsSelector,
    connections => connections.acceptCertRpcHubConnectionId
);
export const autoActualizationHubConnectionIdSelector = createSelector(
    connectionsSelector,
    connections => connections.autoActualizationHubConnectionId
);
export const progressHubConnectionIdSelector = createSelector(
    connectionsSelector,
    connections => connections.progressHubConnectionId
);

export const connectionsReducer = createReducer<ConnectionsState>(
    initialState,
    on(actions.startConnectionsSuccess, (s, p) => ({
        ...s,
        acceptCertRpcHubConnectionId: p.payload.acceptCertRpcHubConnectionId,
        autoActualizationHubConnectionId: p.payload.autoActualizationHubConnectionId,
        progressHubConnectionId: p.payload.progressHubConnectionId
    }))
);
