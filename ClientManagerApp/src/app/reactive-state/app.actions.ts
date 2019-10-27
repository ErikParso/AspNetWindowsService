import { createAction, props } from '@ngrx/store';
import { CurrentUserInfo } from '../model/current-user-info';

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
