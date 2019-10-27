import { ClientInfo } from '../../models/client-info';
import { Action, createReducer, on, createSelector } from '@ngrx/store';
import * as fromRoot from '../../../reactive-state/app.reducer';
import * as actions from './installations.actions';
import { CurrentProcess, CurrentProcessType, CurrentProcessLogItem, CurrentProcessResult } from '../../models/current-process';
import { stringify } from 'querystring';
import { UpgradeInfo } from '../../models/upgrade-info.enum';
import { State } from '../installations.module.reducers';

export interface InstalationsState {
    allInstallations: ClientInfo[];
    currentInstallation: string;
    errorMessage: string;
    currentProcesses: CurrentProcess[];
    loadingInstallations: boolean;
}

export const initialState: InstalationsState = {
    allInstallations: [],
    currentInstallation: null,
    errorMessage: null,
    currentProcesses: [],
    loadingInstallations: false,
};

export const installationsSelector = (state: State) => state.installationsModule.installations;

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

export const clientInstallationInfoSelectorByClientId = (clientId: string) => createSelector(
    allInstallationsSelector,
    (allInstallations) => allInstallations.find(i => i.clientId === clientId)
);

export const loadingInstallationsSelector = createSelector(
    installationsSelector,
    installations => installations.loadingInstallations
);

export const installationsReducer = createReducer<InstalationsState>(
    initialState,
    on(actions.loadInstallations, (s) => ({ ...s, loadingInstallations: true })),
    on(actions.loadInstallationsSuccess, (s, p) => ({
        ...s,
        allInstallations: p.payload,
        currentInstallation: p.payload.length ? p.payload[0].clientId : null,
        loadingInstallations: false
    })),
    on(actions.loadInstallationsError, (s, p) => ({
        ...s,
        errorMessage: p.payload,
        loadingInstallations: false
    })),
    on(actions.getClientNeedUpgrade, (s, p) => ({
        ...s,
        allInstallations: s.allInstallations.map(i => {
            if (i.clientId === p.payload.clientId) {
                i.upgradeInfo = UpgradeInfo.checkingForUpgrade;
            }
            return i;
        })
    })),
    on(actions.getClientNeedUpgradeSuccess, (s, p) => ({
        ...s,
        allInstallations: s.allInstallations.map(i => {
            if (i.clientId === p.payload.clientId) {
                i.upgradeInfo = p.payload.upgradeInfo;
            }
            return i;
        })
    })),
    on(actions.setCurrentInstallation, (s, p) => ({ ...s, currentInstallation: p.payload })),
    on(actions.installNewClient, (s, p) => ({
        ...s,
        allInstallations: s.allInstallations
            .filter(i => i.clientId !== p.payload.clientId)
            .concat({
                clientId: p.payload.clientId,
                clientName: p.payload.clientName,
                installDir: p.payload.installDir,
                config: {
                    language: p.payload.language,
                    applicationServer: p.payload.applicationServer,
                    items: p.payload.configItems
                },
                extensions: [],
                currentProcessId: p.payload.installationProcessId,
                upgradeInfo: UpgradeInfo.isActual,
                userName: p.payload.userName
            }),
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
            if (i.clientId === p.payload.clientId) {
                return { ...p.payload };
            } else {
                return { ...i };
            }
        }),
        currentProcesses: s.currentProcesses.map(c => {
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
            if (i.clientId === p.payload.clientId) {
                return { ...i, version: 'error', errorMessage: p.payload.message };
            } else {
                return { ...i };
            }
        }),
        currentProcesses: s.currentProcesses.map(c => {
            if (c.processId === p.payload.installationProcessId) {
                return { ...c, result: CurrentProcessResult.error };
            } else {
                return { ...c };
            }
        })
    })),
    on(actions.removeCleint, (s, p) => ({
        ...s,
        allInstallations: s.allInstallations
            .filter(i => !(i.clientId === p.payload.clientId))
    })),
    on(actions.updateClient, (s, p) => ({
        ...s,
        allInstallations: s.allInstallations.map(i => {
            if (i.clientId === p.payload.clientId) {
                i.currentProcessId = p.payload.updateProcessId;
            }
            return i;
        }),
        currentProcesses: s.currentProcesses.concat({
            processId: p.payload.updateProcessId,
            processType: CurrentProcessType.upgrade,
            log: [],
            progress: 0,
            result: CurrentProcessResult.running
        } as CurrentProcess)
    })),
    on(actions.updateClientSuccess, (s, p) => ({
        ...s,
        allInstallations: s.allInstallations.map(i => {
            if (i.clientId === p.payload.clientId) {
                return { ...p.payload };
            } else {
                return { ...i };
            }
        }),
        currentProcesses: s.currentProcesses.map(c => {
            if (c.processId === p.payload.currentProcessId) {
                c.result = CurrentProcessResult.success;
                c.progress = 100;
            }
            return c;
        })
    })),
    on(actions.updateClientError, (s, p) => ({
        ...s,
        currentProcesses: s.currentProcesses.map(c => {
            if (c.processId === p.payload.updateProcessId) {
                c.result = CurrentProcessResult.error;
            }
            return c;
        })
    })),
    on(actions.deleteClient, (s, p) => ({
        ...s,
        allInstallations: s.allInstallations.map(i => {
            if (i.clientId === p.payload.clientId) {
                i.currentProcessId = p.payload.deleteProcessId;
            }
            return i;
        }),
        currentProcesses: s.currentProcesses.concat({
            processId: p.payload.deleteProcessId,
            processType: CurrentProcessType.delete,
            progress: 0,
            result: CurrentProcessResult.running,
            log: []
        } as CurrentProcess)
    })),
    on(actions.deleteClientSuccess, (s, p) => ({
        ...s,
        currentProcesses: s.currentProcesses.map(c => {
            if (c.processId === p.payload.deleteProcessId) {
                c.progress = 100;
                c.result = CurrentProcessResult.success;
            }
            return c;
        })
    })),
    on(actions.deleteClientError, (s, p) => ({
        ...s,
        currentProcesses: s.currentProcesses.map(c => {
            if (c.processId === p.payload.deleteProcessId) {
                c.result = CurrentProcessResult.error;
            }
            return c;
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
    on(actions.clearCurrentProcess, (s, p) => {
        const process = s.currentProcesses.find(c => c.processId === p.payload);
        return ({
            ...s,
            currentProcesses: s.currentProcesses.filter(c => c.processId !== p.payload),
            allInstallations: process.processType === CurrentProcessType.delete ||
                (process.processType === CurrentProcessType.installation && process.result === CurrentProcessResult.error)
                ? s.allInstallations.filter(i => i.currentProcessId !== p.payload)
                : s.allInstallations.map(i => {
                    if (i.currentProcessId === p.payload) {
                        i.currentProcessId = '';
                    }
                    return i;
                })
        });
    }),
    on(actions.setClientCheckingForUpgrades, (s, p) => ({
        ...s,
        allInstallations: s.allInstallations.map(i => {
            if (i.clientId === p.payload.clientId) {
                i.upgradeInfo = UpgradeInfo.checkingForUpgrade;
            }
            return i;
        })
    })),
    on(actions.setClientCheckingForUpgradesResult, (s, p) => ({
        ...s,
        allInstallations: s.allInstallations.map(i => {
            if (i.clientId === p.payload.clientId) {
                i.upgradeInfo = p.payload.upgradeInfo;
            }
            return i;
        })
    })),
    on(actions.setClientUpgrading, (s, p) => ({
        ...s,
        allInstallations: s.allInstallations.map(i => {
            if (i.clientId === p.payload.clientId) {
                i.currentProcessId = p.payload.upgradeProcessId;
                i.upgradeInfo = UpgradeInfo.upgradeAvailable;
            }
            return i;
        }),
        currentProcesses: s.currentProcesses.concat({
            processId: p.payload.upgradeProcessId,
            processType: CurrentProcessType.upgrade,
            log: [],
            progress: 0,
            result: CurrentProcessResult.running
        } as CurrentProcess)
    })),
    on(actions.setClientUpgradingResult, (s, p) => {
        const clientInfo = s.allInstallations.find(i => i.clientId === p.payload.clientInfo.clientId);
        const processId = clientInfo ? clientInfo.currentProcessId : '';
        return ({
            ...s,
            allInstallations: s.allInstallations.map(i => {
                if (i.clientId === p.payload.clientInfo.clientId) {
                    return { ...p.payload.clientInfo, currentProcessId: processId };
                } else {
                    return { ...i };
                }
            }),
            currentProcesses: s.currentProcesses.map(c => {
                if (c.processId === processId) {
                    c.result = CurrentProcessResult.success;
                    c.progress = 100;
                }
                return c;
            })
        });
    })
);

export function reducer(state: InstalationsState, action: Action) {
    return installationsReducer(state, action);
}
