import { Injectable } from '@angular/core';
import { map, mergeMap, catchError } from 'rxjs/operators';
import { createEffect, ofType, Actions } from '@ngrx/effects';
import { VersionService } from './version.service';
import * as reducer from './app.reducer';
import { createAction } from '@ngrx/store';
import { CurrentUserInfoService } from './current-user-info.service';
import { of } from 'rxjs';

@Injectable()
export class AppEffects {

  loadVersions$ = createEffect(() => this.actions$.pipe(
    ofType(reducer.loadVersions),
    mergeMap(() => this.versionService.getServiceVersions()
      .pipe(
        map(versionInfo => reducer.loadVersionsSuccess(versionInfo))
      ))
  ));

  loadCurrentUserInfo$ = createEffect(() => this.actions$.pipe(
    ofType(reducer.loadCurrentUserInfo),
    mergeMap(() => this.currentUserInfoService.getCurrentUserInfo()
      .pipe(
        map(userInfo => reducer.loadCurrentUserInfoSuccess({ payload: userInfo })),
        catchError((e) => of(reducer.loadCurrentUserInfoError({ payload: 'load user info faied: ' + e.message })))
      ))
  ));

  downloadUpgradePackage$ = createEffect(() => this.actions$.pipe(
    ofType(reducer.downloadUpgradePackage),
    mergeMap(() => this.versionService.downloadInstaller()
      .pipe(
        map(installerPath => reducer.runUpgradePackage({ payload: installerPath }))
      ))
  ));

  runUpgradePackage$ = createEffect(() => this.actions$.pipe(
    ofType(reducer.runUpgradePackage),
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
