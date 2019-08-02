import { ClientInstallationInfo } from './models/clientInstallationInfo';
import { Action, createReducer, on, createSelector } from '@ngrx/store';
import * as fromRoot from '../app.reducer';
import * as actions from './installations.actions';

export interface InstalationsState {
    allInstallations: ClientInstallationInfo[];
    currentInstallation: ClientInstallationInfo;
    latestClientVersion: string;
    errorMessage: string;
}

export const initialState: InstalationsState = {
    allInstallations: [],
    currentInstallation: null,
    latestClientVersion: '0.0.0',
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

export const latestClientVersionSelector = createSelector(
    installationsSelector,
    (state: InstalationsState) => state.latestClientVersion
);

const installationsReducer = createReducer<InstalationsState>(
    initialState,
    on(actions.loadInstallationsSuccess, (s, p) => ({ ...s, allInstallations: p.payload })),
    on(actions.loadInstallationsError, (s, p) => ({ ...s, errorMessage: p.payload })),
    on(actions.loadLatestClientVersionSuccess, (s, p) => ({ ...s, latestClientVersion: p.payload })),
    on(actions.setCurrentInstallation, (s, p) => ({ ...s, currentInstallation: p.payload })),
    on(actions.installNewClient, (s, p) => ({
        ...s,
        allInstallations: s.allInstallations.concat({
            clientName: p.payload.clientName,
            installDir: p.payload.installDir,
            version: 'installing'
        } as ClientInstallationInfo)
    })),
    on(actions.installNewClientSuccess, (s, p) => ({
        ...s,
        allInstallations: s.allInstallations.map(i => {
            if (i.clientName === p.payload.clientName) {
                return { ...i, installDir: p.payload.installDir, version: p.payload.version };
            } else {
                return { ...i };
            }
        })
    }))
);

export function reducer(state: InstalationsState, action: Action) {
    return installationsReducer(state, action);
}
