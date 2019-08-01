import { createAction, props, Action } from '@ngrx/store';
import { ClientInstallationInfo } from './models/clientInstallationInfo';

export enum InstallationsActions {
    loadInstallations = '[Installations] Load installations',
    loadInstallationsSuccess = '[Installations] Load installations success',
    loadInstallationsError = '[Installations] Load installations error',
    runClient = '[Installations] Run client application',
    runClientSuccess = '[Installations] Run client application success'
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

