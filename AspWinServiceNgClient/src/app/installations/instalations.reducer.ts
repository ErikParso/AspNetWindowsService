import { ClientInstallationInfo } from './models/clientInstallationInfo';
import { Action, createReducer, on, createSelector } from '@ngrx/store';
import * as fromRoot from '../app.reducer';
import * as actions from './installations.actions';
import { CurrentProcess, CurrentProcessType, CurrentProcessLogItem, CurrentProcessResult } from './models/current-process';
import { stringify } from 'querystring';

export interface InstalationsState {
    allInstallations: ClientInstallationInfo[];
    currentInstallation: string;
    latestClientVersion: string;
    errorMessage: string;
    currentProcesses: CurrentProcess[];
}

export const initialState: InstalationsState = {
    allInstallations: [],
    currentInstallation: null,
    latestClientVersion: '0.0.0',
    errorMessage: null,
    currentProcesses: []
};

export interface State extends fromRoot.State {
    installations: InstalationsState;
}

export const installationsSelector = (state: State) => state.installations;

export const currentInstallationIdSelector = createSelector(
    installationsSelector,
    (state: InstalationsState) => state.currentInstallation
);

export const allInstallationsSelector = createSelector(
    installationsSelector,
    (state: InstalationsState) => state.allInstallations
);

export const currentInstallationSelector = createSelector(
    currentInstallationIdSelector,
    allInstallationsSelector,
    (currentInstallation, allInstallations) => allInstallations.find(i => i.clientId === currentInstallation)
);

export const errorMessageSelector = createSelector(
    installationsSelector,
    (state: InstalationsState) => state.errorMessage
);

export const latestClientVersionSelector = createSelector(
    installationsSelector,
    (state: InstalationsState) => state.latestClientVersion
);

export const currentProcessesSelector = createSelector(
    installationsSelector,
    (state: InstalationsState) => state.currentProcesses
);

export const currentProcessSelector = (currentProcessId: string) => createSelector(
    currentProcessesSelector,
    (currentProcesses) => currentProcesses.find(p => p.processId === currentProcessId)
);

export const clientInstallationInfoSelector = (currentProcessId: string) => createSelector(
    allInstallationsSelector,
    (allInstallations) => allInstallations.find(i => i.currentProcessId === currentProcessId)
);

const installationsReducer = createReducer<InstalationsState>(
    initialState,
    on(actions.loadInstallationsSuccess, (s, p) => ({
        ...s,
        allInstallations: p.payload,
        currentInstallation: p.payload.length ? p.payload[0].clientId : null
    })),
    on(actions.loadInstallationsError, (s, p) => ({ ...s, errorMessage: p.payload })),
    on(actions.getClientNeedUpgradeSuccess, (s, p) => ({
        ...s,
        allInstallations: s.allInstallations.map(i => {
            if (i.clientId === p.payload.clientId) {
                i.needUpgrade = p.payload.needUpgrade;
            }
            return i;
        })
    })),
    on(actions.setCurrentInstallation, (s, p) => ({ ...s, currentInstallation: p.payload })),
    on(actions.installNewClient, (s, p) => ({
        ...s,
        allInstallations: s.allInstallations.concat({
            clientId: p.payload.clientId,
            clientName: p.payload.clientName,
            installDir: p.payload.installDir,
            version: 'installing',
            currentProcessId: p.payload.installationProcessId,
            needUpgrade: false,
            errorMessage: ''
        } as ClientInstallationInfo),
        currentInstallation: p.payload.clientId,
        currentProcesses: s.currentProcesses.concat({
            processId: p.payload.installationProcessId,
            processType: CurrentProcessType.installation,
            progress: 0,
            result: CurrentProcessResult.running,
            log: []
        } as CurrentProcess)
    })),
    on(actions.installNewClientSuccess, (s, p) => ({
        ...s,
        allInstallations: s.allInstallations.map(i => {
            if (i.clientName === p.payload.clientName) {
                return { ...i, installDir: p.payload.installDir, version: p.payload.version };
            } else {
                return { ...i };
            }
        }),
        currentProcesses: s.currentProcesses.map(c => {
            console.log(c.processId, p.payload.currentProcessId);
            if (c.processId === p.payload.currentProcessId) {
                return { ...c, result: CurrentProcessResult.success, progress: 100 };
            } else {
                return { ...c };
            }
        })
    })),
    on(actions.installNewClientError, (s, p) => ({
        ...s,
        allInstallations: s.allInstallations.map(i => {
            if (i.clientName === p.payload.clientName) {
                return { ...i, version: 'error', errorMessage: p.payload.message };
            } else {
                return { ...i };
            }
        }),
        currentProcesses: s.currentProcesses.map(c => {
            if (c.processId === p.payload.installationProcessId) {
                return { ...c, result: CurrentProcessResult.error, };
            } else {
                return { ...c };
            }
        })
    })),
    on(actions.removeCleint, (s, p) => ({
        ...s,
        allInstallations: s.allInstallations
            .filter(i => !(i.clientName === p.payload.clientName && i.installDir === p.payload.installDir))
    })),
    on(actions.updateClient, (s, p) => ({
        ...s,
        allInstallations: s.allInstallations.map(i => {
            if (i.clientName === p.payload.clientName) {
                i.version = 'updating';
            }
            return i;
        })
    })),
    on(actions.updateClientSuccess, (s, p) => ({
        ...s,
        allInstallations: s.allInstallations.map(i => {
            if (i.clientName === p.payload.clientName) {
                i.installDir = p.payload.installDir;
                i.version = p.payload.version;
            }
            return i;
        })
    })),
    on(actions.updateClientError, (s, p) => ({
        ...s,
        allInstallations: s.allInstallations.map(i => {
            if (i.clientName === p.payload.clientName) {
                i.version = 'error';
                i.errorMessage = p.payload.message;
            }
            return i;
        })
    })),
    on(actions.deleteClient, (s, p) => ({
        ...s,
        allInstallations: s.allInstallations.map(i => {
            if (i.clientName === p.payload.clientName) {
                i.version = 'deleting';
            }
            return i;
        })
    })),
    on(actions.deleteClientSuccess, (s, p) => ({
        ...s,
        allInstallations: s.allInstallations.filter(i => i.clientName !== p.payload.clientName)
    })),
    on(actions.deleteClientError, (s, p) => ({
        ...s,
        allInstallations: s.allInstallations.map(i => {
            if (i.clientName === p.payload.clientName) {
                i.version = 'error';
                i.errorMessage = p.payload.message;
            }
            return i;
        })
    })),
    on(actions.reportProgress, (s, p) => ({
        ...s,
        currentProcesses: s.currentProcesses.map(c => {
            if (c.processId === p.payload.processId) {
                return {
                    ...c,
                    progress: p.payload.progress,
                    log: c.log.concat({
                        content: p.payload.message,
                        logItemId: p.payload.logItemId,
                    } as CurrentProcessLogItem)
                };
            } else {
                return { ...c };
            }
        })
    })),
    on(actions.clearCurrentProcess, (s, p) => ({
        ...s,
        currentProcesses: s.currentProcesses.filter(c => c.processId !== p.payload),
        allInstallations: s.allInstallations.map(i => {
            if (i.currentProcessId === p.payload) {
                i.currentProcessId = '';
            }
            return i;
        })
    }))
);

export function reducer(state: InstalationsState, action: Action) {
    return installationsReducer(state, action);
}
