import { createReducer, ActionReducerMap, createSelector, createAction, props, on, Action } from '@ngrx/store';
import { InjectionToken } from '@angular/core';
import { UpgradeInfo } from './upgradeInfo.enum';

// State declaration
export interface State {
    versionInfo: VersionInfo;
    applicationState: ApplicationState;
    startupInfo: StartupInfo;
}
export interface VersionInfo {
    localVersion: string;
    latesVersion: string;
}
export interface ApplicationState {
    upgradeState: UpgradeInfo;
}
export interface StartupInfo {
    startupFile: string;
}

// State initialization
export const initialStateVersions: VersionInfo = {
    localVersion: '0.0.0',
    latesVersion: '0.0.0'
};
export const initialStateAppState: ApplicationState = {
    upgradeState: UpgradeInfo.none
};
export const initialStartupInfo: StartupInfo = {
    startupFile: ''
};

// Actions definition
export const loadVersions = createAction(
    '[App] Load versions'
);
export const loadVersionsSuccess = createAction(
    '[App] Load versions success',
    props<{ local: string, latest: string }>()
);
export const downloadUpgradePackage = createAction(
    '[App] Download upgrade package'
);
export const runUpgradePackage = createAction(
    '[App] Run upgrade package',
    props<{ payload: string }>()
);
export const setStartupFile = createAction(
    '[App] Set startup file name',
    props<{payload: string}>()
);

// Selectors definition
export const versionInfoSelector = (state: State) => state.versionInfo;
export const localVersionSelector = createSelector(
    versionInfoSelector,
    versionInfo => versionInfo.localVersion
);
export const latestVersionSelector = createSelector(
    versionInfoSelector,
    versionInfo => versionInfo.latesVersion
);
export const applicationStateSelector = (state: State) => state.applicationState;
export const upgradeStateSelector = createSelector(
    applicationStateSelector,
    applicationState => applicationState.upgradeState
);
export const startupInfoSelectror = (state: State) => state.startupInfo;
export const startupFileSelector = createSelector(
    startupInfoSelectror,
    startupInfo => startupInfo.startupFile
);

// Reducers definition
const appReducerVersions = createReducer<VersionInfo>(
    initialStateVersions,
    on(loadVersionsSuccess, (s, p) => ({ ...s, localVersion: p.local, latesVersion: p.latest }))
);
const appReducerAppState = createReducer<ApplicationState>(
    initialStateAppState,
    on(downloadUpgradePackage, (s) => ({ ...s, upgradeState: UpgradeInfo.downloading })),
    on(runUpgradePackage, (s) => ({ ...s, upgradeState: UpgradeInfo.installing }))
);
const appReducerStartupInfo = createReducer<StartupInfo>(
    initialStartupInfo,
    on(setStartupFile, (s, p) => ({ ...s, startupFile: p.payload }))
);

// Reducers export
export const reducers = {
    versionInfo: appReducerVersions,
    applicationState: appReducerAppState,
    startupInfo: appReducerStartupInfo
};
export const reducerToken = new InjectionToken<ActionReducerMap<State>>('Registered Reducers');
export const reducerProvider = [
    { provide: reducerToken, useValue: reducers }
];
