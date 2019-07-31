import { createReducer, ActionReducerMap, createSelector, createAction, props, on, Action } from '@ngrx/store';
import { InjectionToken } from '@angular/core';

export interface State {
    versionInfo: VersionInfo;
}

export interface VersionInfo {
    localVersion: string;
    latesVersion: string;
}

export const initialState: VersionInfo = {
    localVersion: '0.0.0',
    latesVersion: '0.0.0'
};

export const loadVersions = createAction('[App] Load versions');
export const loadVersionsSuccess = createAction('[App] Load versions success', props<{ local: string, latest: string }>());

export const versionInfoSelector = (state: State) => state.versionInfo;
export const localVersionSelector = createSelector(
    versionInfoSelector,
    versionInfo => versionInfo.localVersion
);
export const latestVersionSelector = createSelector(
    versionInfoSelector,
    versionInfo => versionInfo.latesVersion
);

const appReducer = createReducer<VersionInfo>(
    initialState,
    on(loadVersionsSuccess, (s, p) => ({ ...s, localVersion: p.local, latesVersion: p.latest }))
);

export const reducers = {
    versionInfo: appReducer
};

export const reducerToken = new InjectionToken<ActionReducerMap<State>>('Registered Reducers');

export const reducerProvider = [
    { provide: reducerToken, useValue: reducers }
];
