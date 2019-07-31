import { ClientInstallationInfo } from './models/clientInstallationInfo';
import { Action, createReducer, on, createSelector } from '@ngrx/store';
import * as fromRoot from '../app.reducer';

export interface InstalationsState {
    allInstallations: ClientInstallationInfo[];
    currentInstallation: ClientInstallationInfo;
    errorMessage: string;
}

export const initialState: InstalationsState = {
    allInstallations: [],
    currentInstallation: null,
    errorMessage: null
};

export interface State extends fromRoot.State {
    installations: InstalationsState;
}

export const installationsSelector = (state: State) => state.installations;

export const currentInstallationSelector = createSelector(
    installationsSelector,
    (state: InstalationsState) => state.currentInstallation
);

export const allInstallationsSelector = createSelector(
    installationsSelector,
    (state: InstalationsState) => state.allInstallations
);

export const errorMessageSelector = createSelector(
    installationsSelector,
    (state: InstalationsState) => state.errorMessage
);

const installationsReducer = createReducer<InstalationsState>(
    initialState,
);

export function reducer(state: InstalationsState, action: Action) {
    return installationsReducer(state, action);
}
