import { createAction, props, Action } from '@ngrx/store';
import { ClientInfo } from '../../models/client-info';
import { ClientInstallationRequest } from '../../models/ClientInstallationRequest';
import { UpgradeInfo } from '../../models/upgrade-info.enum';

enum InstallationsActions {
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
    updateClientSuccess = '[Installations] Update client success',
    updateClientError = '[Installations] Update client error',
    deleteClient = '[Installations] Delete client',
    deleteClientSuccess = '[Installations] Delete client success',
    deleteClientError = '[Installations] Delete client error',
    reportProgress = '[Installations] Progress report',
    clearCurrentProcess = '[Installations] Clear current process from store',
    setClientCheckingForUpgrades = '[Installations][AutoActualization] Set client checking for upgrades',
    setClientCheckingForUpgradesResult = '[Installations][AutoActualization] Set client checking for upgrades result',
    setClientUpgrading = '[Installations][AutoActualization] Set client upgrading',
    setClientUpgradingResult = '[Installations][AutoActualization] Set client upgrading result',
    dummyAction = '[Dummy] Application will relax now',
}

export const loadInstallations = createAction(
    InstallationsActions.loadInstallations
);

export const loadInstallationsSuccess = createAction(
    InstallationsActions.loadInstallationsSuccess,
    props<{ payload: ClientInfo[] }>()
);

export const loadInstallationsError = createAction(
    InstallationsActions.loadInstallationsError,
    props<{ payload: string }>()
);

export const runClient = createAction(
    InstallationsActions.runClient,
    props<{ payload: { clientId: string } }>()
);

export const runClientSuccess = createAction(
    InstallationsActions.runClientSuccess
);

export const getClientNeedUpgrade = createAction(
    InstallationsActions.getClientNeedUpgrade,
    props<{ payload: { clientId: string } }>()
);

export const getClientNeedUpgradeSuccess = createAction(
    InstallationsActions.getClientNeedUpgradeSuccess,
    props<{ payload: { clientId: string, upgradeInfo: UpgradeInfo } }>()
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
    props<{ payload: ClientInstallationRequest }>()
);

export const installNewClientSuccess = createAction(
    InstallationsActions.installNewClientSuccess,
    props<{ payload: ClientInfo }>()
);

export const installNewClientError = createAction(
    InstallationsActions.installNewClientError,
    props<{ payload: { message: string, clientId: string, installationProcessId: string } }>()
);

export const removeCleint = createAction(
    InstallationsActions.removeClient,
    props<{ payload: ClientInfo }>()
);

export const updateClient = createAction(
    InstallationsActions.updateClient,
    props<{ payload: { clientId: string, updateProcessId: string } }>()
);

export const updateClientSuccess = createAction(
    InstallationsActions.updateClientSuccess,
    props<{ payload: ClientInfo }>()
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

export const setClientCheckingForUpgrades = createAction(
    InstallationsActions.setClientCheckingForUpgrades,
    props<{ payload: { clientId: string } }>()
);

export const setClientCheckingForUpgradesResult = createAction(
    InstallationsActions.setClientCheckingForUpgradesResult,
    props<{ payload: { clientId: string, upgradeInfo: UpgradeInfo, message: string } }>()
);

export const setClientUpgrading = createAction(
    InstallationsActions.setClientUpgrading,
    props<{ payload: { clientId: string, upgradeProcessId: string } }>()
);

export const setClientUpgradingResult = createAction(
    InstallationsActions.setClientUpgradingResult,
    props<{ payload: { result: boolean, clientInfo: ClientInfo } }>()
);

export const dummy = createAction(
    InstallationsActions.dummyAction
);
