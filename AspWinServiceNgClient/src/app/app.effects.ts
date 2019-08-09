import { Injectable } from '@angular/core';
import { map, mergeMap } from 'rxjs/operators';
import { createEffect, ofType, Actions } from '@ngrx/effects';
import { VersionService } from './version.service';
import * as reducer from './app.reducer';
import { createAction } from '@ngrx/store';

@Injectable()
export class AppEffects {

  loadVersions$ = createEffect(() => this.actions$.pipe(
    ofType(reducer.loadVersions),
    mergeMap(() => this.versionService.getServiceVersions()
      .pipe(
        map(versionInfo => reducer.loadVersionsSuccess(versionInfo))
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
        map(() => createAction('DUMMY_ACTION'))
      ))
  ));

  constructor(
    private actions$: Actions,
    private versionService: VersionService
  ) { }
}
