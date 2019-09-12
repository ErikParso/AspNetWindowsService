import { createAction, props, Action } from '@ngrx/store';
import { ClientInstallationInfo } from './models/clientInstallationInfo';

export enum InstallationsActions {
    loadInstallations = '[Installations] Load installations',
    loadInstallationsSuccess = '[Installations] Load installations success',
    loadInstallationsError = '[Installations] Load installations error',
    runClient = '[Installations] Run client application',
    runClientSuccess = '[Installations] Run client application success',
    getClientNeedUpgrade = '[Installations] Get client need upgrade flag',
    getClientNeedUpgradeSuccess = '[Installations] Get client need upgrade flag success',
    getClientNeedUpgradeError = '[Installations] Get client need upgrade flag success',
    setCurrentInstallation = '[Installations] Set current installation',
    installNewClient = '[Installations] Install new client',
    installNewClientSuccess = '[Installations] Install new client success',
    installNewClientError = '[Installations] Install new client error',
    removeClient = '[Installations] Remove client',
    updateClient = '[Installation] Update client',
    updateClientSuccess = '[Installation] Update client success',
    updateClientError = '[Installation] Update client error',
    deleteClient = '[Installation] Delete client',
    deleteClientSuccess = '[Installation] Delete client success',
    deleteClientError = '[Installation] Delete client error',
    reportProgress = '[Installation] Progress report',
    clearCurrentProcess = '[Installation] Clear current process from store',
    dummyAction = '[Dummy] just relax'
}

export const loadInstallations = createAction(
    InstallationsActions.loadInstallations
);

export const loadInstallationsSuccess = createAction(
    InstallationsActions.loadInstallationsSuccess,
    props<{ payload: ClientInstallationInfo[] }>()
);

export const loadInstallationsError = createAction(
    InstallationsActions.loadInstallationsError,
    props<{ payload: string }>()
);

export class RunClientAction implements Action {
    type = InstallationsActions.runClient;
    constructor(public payload: string) { }
}

export const runClientSuccess = createAction(
    InstallationsActions.runClientSuccess
);

export const getClientNeedUpgrade = createAction(
    InstallationsActions.getClientNeedUpgrade,
    props<{ payload: { clientId: string } }>()
);

export const getClientNeedUpgradeSuccess = createAction(
    InstallationsActions.getClientNeedUpgradeSuccess,
    props<{ payload: { clientId: string, needUpgrade: boolean } }>()
);

export const getClientNeedUpgradeError = createAction(
    InstallationsActions.getClientNeedUpgradeError,
    props<{ payload: { clientId: string, message: boolean } }>()
);

export const setCurrentInstallation = createAction(
    InstallationsActions.setCurrentInstallation,
    props<{ payload: string }>()
);

export const installNewClient = createAction(
    InstallationsActions.installNewClient,
    props<{
        payload: {
            clientId: string,
            clientName: string,
            language: string,
            installDir: string,
            applicationServer: string,
            installationProcessId: string
        }
    }>()
);

export const installNewClientSuccess = createAction(
    InstallationsActions.installNewClientSuccess,
    props<{ payload: ClientInstallationInfo }>()
);

export const installNewClientError = createAction(
    InstallationsActions.installNewClientError,
    props<{ payload: { message: string, clientName: string, installationProcessId: string } }>()
);

export const removeCleint = createAction(
    InstallationsActions.removeClient,
    props<{ payload: ClientInstallationInfo }>()
);

export const updateClient = createAction(
    InstallationsActions.updateClient,
    props<{ payload: { clientId: string, updateProcessId: string } }>()
);

export const updateClientSuccess = createAction(
    InstallationsActions.updateClientSuccess,
    props<{ payload: { updateProcessId: string, clientId: string } }>()
);

export const updateClientError = createAction(
    InstallationsActions.updateClientError,
    props<{ payload: { message: string, updateProcessId: string } }>()
);

export const deleteClient = createAction(
    InstallationsActions.deleteClient,
    props<{ payload: { clientId: string, deleteProcessId: string } }>()
);

export const deleteClientSuccess = createAction(
    InstallationsActions.deleteClientSuccess,
    props<{ payload: { deleteProcessId: string } }>()
);

export const deleteClientError = createAction(
    InstallationsActions.deleteClientError,
    props<{ payload: { message: string, deleteProcessId: string } }>()
);

export const reportProgress = createAction(
    InstallationsActions.reportProgress,
    props<{ payload: { processId: string, logItemId: string, progress: number, message: string } }>()
);

export const clearCurrentProcess = createAction(
    InstallationsActions.clearCurrentProcess,
    props<{ payload: string }>()
);

export const dummy = createAction(
    InstallationsActions.dummyAction
);
