import { Injectable } from '@angular/core';
import { map, mergeMap, catchError, tap, concatMap, takeWhile } from 'rxjs/operators';
import { createEffect, ofType, Actions } from '@ngrx/effects';
import { VersionService } from './version.service';
import { Store } from '@ngrx/store';
import { State, loadVersions, loadVersionsSuccess } from './app.reducer';

@Injectable()
export class AppEffects {

  loadVersions$ = createEffect(() => this.actions$.pipe(
    ofType(loadVersions),
    mergeMap(() => this.versionService.getServiceVersions()
      .pipe(
        map(versionInfo => loadVersionsSuccess(versionInfo))
      ))
  ));

  constructor(
    private actions$: Actions,
    private versionService: VersionService
  ) { }
}
