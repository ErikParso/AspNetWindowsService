import { createAction, props, Action } from '@ngrx/store';
import { ClientInstallationInfo } from './models/clientInstallationInfo';

export enum InstallationsActions {
    loadInstallations = '[Installations] Load installations',
    loadInstallationsSuccess = '[Installations] Load installations success',
    loadInstallationsError = '[Installations] Load installations error',
    runClient = '[Installations] Run client application',
    runClientSuccess = '[Installations] Run client application success',
    loadLatestClientVersion = '[Installations] Load latest client version',
    loadLatestClientVersionSuccess = '[Installations] Load latest client version success',
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

export const loadLatestClientVersion = createAction(
    InstallationsActions.loadLatestClientVersion
);

export const loadLatestClientVersionSuccess = createAction(
    InstallationsActions.loadLatestClientVersionSuccess,
    props<{ payload: string }>()
);

export const setCurrentInstallation = createAction(
    InstallationsActions.setCurrentInstallation,
    props<{ payload: ClientInstallationInfo }>()
);

export const installNewClient = createAction(
    InstallationsActions.installNewClient,
    props<{ payload: { clientName: string, installDir: string, applicationServer: string, installationProcessId: string } }>()
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
    props<{ payload: ClientInstallationInfo }>()
);

export const updateClientSuccess = createAction(
    InstallationsActions.updateClientSuccess,
    props<{ payload: ClientInstallationInfo }>()
);

export const updateClientError = createAction(
    InstallationsActions.updateClientError,
    props<{ payload: { message: string, clientName: string } }>()
);

export const deleteClient = createAction(
    InstallationsActions.deleteClient,
    props<{ payload: ClientInstallationInfo }>()
);

export const deleteClientSuccess = createAction(
    InstallationsActions.deleteClientSuccess,
    props<{ payload: ClientInstallationInfo }>()
);

export const deleteClientError = createAction(
    InstallationsActions.deleteClientError,
    props<{ payload: { message: string, clientName: string } }>()
);
