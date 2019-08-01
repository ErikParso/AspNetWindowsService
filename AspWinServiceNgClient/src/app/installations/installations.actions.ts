import { createAction, props, Action } from '@ngrx/store';
import { ClientInstallationInfo } from './models/clientInstallationInfo';

export enum InstallationsActions {
    loadInstallations = '[Installations] Load installations',
    loadInstallationsSuccess = '[Installations] Load installations success',
    loadInstallationsError = '[Installations] Load installations error',
    runClient = '[Installations] Run client application',
    runClientSuccess = '[Installations] Run client application success',
    loadLatestClientVersion = '[installations] Load latest client version',
    loadLatestClientVersionSuccess = '[installations] Load latest client version success'
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
