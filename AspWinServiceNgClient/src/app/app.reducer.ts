import { createReducer, ActionReducerMap, createSelector, createAction, props, on, Action } from '@ngrx/store';
import { InjectionToken } from '@angular/core';
import { UpgradeInfo } from './upgradeInfo.enum';

// State declaration
export interface State {
    versionInfo: VersionInfo;
    applicationState: ApplicationState;
    startupInfo: StartupInfo;
    currentUser: CurrentUserInfo;
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
export interface CurrentUserInfo {
    userName: string;
    appLocalPath: string;
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
export const initialCurrentUserInfo: CurrentUserInfo = {
    userName: '',
    appLocalPath: ''
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
    props<{ payload: string }>()
);
export const loadCurrentUserInfo = createAction(
    '[App] Load current user info'
);
export const loadCurrentUserInfoSuccess = createAction(
    '[App] Load current user info success',
    props<{ payload: CurrentUserInfo }>()
);
export const loadCurrentUserInfoError = createAction(
    '[App] Load current user info error',
    props<{ payload: string }>()
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
export const currentUserInfoSelector = (state: State) => state.currentUser;
export const currentUserNameSelector = createSelector(
    currentUserInfoSelector,
    currentUser => currentUser.userName
);
export const currentUserAppLocalPathSelector = createSelector(
    currentUserInfoSelector,
    currentUser => currentUser.appLocalPath
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
const appReducerCurrentUserInfo = createReducer<CurrentUserInfo>(
    initialCurrentUserInfo,
    on(loadCurrentUserInfoSuccess, (s, p) => ({ ...s, userName: p.payload.userName, appLocalPath: p.payload.appLocalPath }))
);

// Reducers export
export const reducers = {
    versionInfo: appReducerVersions,
    applicationState: appReducerAppState,
    startupInfo: appReducerStartupInfo,
    currentUser: appReducerCurrentUserInfo
};
export const reducerToken = new InjectionToken<ActionReducerMap<State>>('Registered Reducers');
export const reducerProvider = [
    { provide: reducerToken, useValue: reducers }
];
