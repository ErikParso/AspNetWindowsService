import { InstalationsState, installationsReducer } from './installations/instalations.reducer';
import { ConnectionsState, connectionsReducer } from './connections/connections.reducer';
import * as fromRoot from '../../reactive-state/app.reducer';
import { createFeatureSelector, createSelector, ActionReducerMap } from '@ngrx/store';
import { InjectionToken } from '@angular/core';

export interface InstallationsModuleState {
    installations: InstalationsState;
    connections: ConnectionsState;
}

export interface State extends fromRoot.State {
    installationsModule: InstallationsModuleState;
}

export const installationsModuleStateSelector = createFeatureSelector<InstallationsModuleState>('installationsModule');
export const installationsSelector = createSelector(
    installationsModuleStateSelector,
    (installationsModuleState) => installationsModuleState.installations);
export const connectionsSelector = createSelector(
    installationsModuleStateSelector,
    (installationsModuleState) => installationsModuleState.connections);


export const reducers = {
  installations: installationsReducer,
  connections: connectionsReducer,
};
export const reducerToken = new InjectionToken<ActionReducerMap<State>>('Installations module reducers');
export const reducerProvider = [
  { provide: reducerToken, useValue: reducers }
];
