import { Injectable } from '@angular/core';
import { map, mergeMap, catchError, tap } from 'rxjs/operators';
import { createEffect, ofType, Actions } from '@ngrx/effects';
import { VersionService } from '../version.service';
import * as actions from './app.actions';
import { createAction } from '@ngrx/store';
import { CurrentUserInfoService } from '../current-user-info.service';
import { of } from 'rxjs';

@Injectable()
export class AppEffects {

  loadVersions$ = createEffect(() => this.actions$.pipe(
    ofType(actions.loadVersions),
    mergeMap(() => this.versionService.getServiceVersions()
      .pipe(
        map(versionInfo => actions.loadVersionsSuccess(versionInfo))
      ))
  ));

  loadCurrentUserInfo$ = createEffect(() => this.actions$.pipe(
    ofType(actions.loadCurrentUserInfo),
    mergeMap(() => this.currentUserInfoService.getCurrentUserInfo()
      .pipe(
        map(userInfo => actions.loadCurrentUserInfoSuccess({ payload: userInfo })),
        catchError((e) => of(actions.loadCurrentUserInfoError({ payload: 'load user info faied: ' + e.message })))
      ))
  ));

  downloadUpgradePackage$ = createEffect(() => this.actions$.pipe(
    ofType(actions.downloadUpgradePackage),
    mergeMap(() => this.versionService.downloadInstaller()
      .pipe(
        map(installerPath => actions.runUpgradePackage({ payload: installerPath }))
      ))
  ));

  runUpgradePackage$ = createEffect(() => this.actions$.pipe(
    ofType(actions.runUpgradePackage),
    mergeMap(({ payload }) => this.versionService.runInstallerAndClose(payload)
      .pipe(
        map(() => createAction('DUMMY_ACTION')())
      ))
  ));

  constructor(
    private actions$: Actions,
    private versionService: VersionService,
    private currentUserInfoService: CurrentUserInfoService
  ) { }
}
